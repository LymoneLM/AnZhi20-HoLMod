using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;

namespace cs.HoLMod.MoreGambles
{
    public class FightTheLandlord : BaseUnityPlugin
    {
        // 窗口设置
        private Rect windowRect = new Rect(20, 20, 1000, 750); // 增加高度以容纳更多UI元素
        private bool showMenu = false;
        private bool blockGameInput = false;
        private Vector2 scrollPosition;
        private float scaleFactor = 1f;
        private const string CURRENT_VERSION = "1.0.0";
        
        // 货币系统相关
        public bool UsePaperMoney { get; set; } = true; // true=宝钞, false=金钞
        public int InitialChips { get; set; } = 1000;
        public int RemainingMoney { get; set; } = 0;
        
        // 游戏结束窗口
        private Rect gameOverWindowRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 200);
        private string gameOverMessage = "";

        // 斗地主游戏状态
        private enum GameState { NotStarted, Bidding, Playing, GameOver }
        private GameState currentState = GameState.NotStarted;
        private int playerChips = 1000; // 玩家筹码
        private int pot = 0; // 底池
        private bool hasSomeoneBid = false; // 是否已经有人叫地主
        
        // 重复游戏逻辑相关
        private bool hasCalledRepeatLogic = false;

        // 玩家类
        private class Player
        {
            public string Name { get; set; }
            public List<Card> Hand { get; set; }
            public List<Card> SelectedCards { get; set; } // 玩家已选择的卡牌
            public int Chips { get; set; }
            public bool IsLandlord { get; set; }
            public bool HasPassed { get; set; }
            public bool IsPlaying { get; set; } // 是否还在游戏中
            public bool HasPlayedInCurrentRound { get; set; }
            public bool HasBid { get; set; } // 是否叫过地主
            public int HasPassedCount { get; set; } // 累积Pass次数
            public int TimesPlayed { get; set; } // 累积出牌次数

            public Player(string name)
            {
                Name = name;
                Hand = new List<Card>();
                SelectedCards = new List<Card>();
                Chips = 1000;
                IsLandlord = false;
                HasPassed = false;
                IsPlaying = true;
                HasPlayedInCurrentRound = false;
                HasBid = false;
                HasPassedCount = 0;
                TimesPlayed = 0;
            }
        }
        
        // 当前回合的地主牌
        private List<Card> currentPublicCards = new List<Card>();
        // 最近一次出牌的玩家索引
        private int lastPlayerIndex = -1;
        // 最近一次出牌的牌型
        private List<Card> lastPlayedCards = new List<Card>();
        // 最近一次出牌的牌型类型
        private CardType lastPlayedCardType = CardType.Invalid;

        // 卡牌数据
        private enum Suit { Hearts, Diamonds, Clubs, Spades }
        private enum Rank { Three = 3, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace, Two, Joker, BigJoker }
        
        private struct Card
        {
            public Suit suit;
            public Rank rank;
            public bool isVisible;
            public int id; // 卡牌唯一标识，用于选择

            public Card(Suit suit, Rank rank, bool isVisible = true)
            {
                this.suit = suit;
                this.rank = rank;
                this.isVisible = isVisible;
                this.id = (int)suit * 15 + (int)rank; // 生成唯一ID
            }

            public override string ToString()
            {
                if (!isVisible) return "??";
                string rankStr;
                if (rank == Rank.Joker) rankStr = "Joker";
                else if (rank == Rank.BigJoker) rankStr = "JOKER";
                else rankStr = ((int)rank).ToString(); // 所有普通牌都使用阿拉伯数字显示
                
                string suitStr = suit == Suit.Hearts ? "♥" : 
                               suit == Suit.Diamonds ? "♦" : 
                               suit == Suit.Clubs ? "♣" : "♠";
                
                return rank < Rank.Joker ? $"{rankStr}{suitStr}" : rankStr;
            }
        }
        
        // 牌型枚举
        private enum CardType { Invalid, Single, Pair, Triple, Straight, StraightPair, Bomb, Rocket, TripleWithSingle, TripleWithPair, QuadWithTwoSingle, QuadWithTwoPair, TripleSequence }

        private List<Card> deck = new List<Card>();
        private List<Card> landlordCards = new List<Card>(); // 地主牌
        private List<Player> players = new List<Player>(); // 玩家和2个AI
        private Player currentPlayer = null; // 当前行动玩家
        private List<Card> currentPlayedCards = new List<Card>(); // 当前打出的牌
        private Player lastPlayerWhoPlayed = null; // 最后出牌的玩家

        private void Awake()
        {
            InitializeDeck();
            UpdateResolutionSettings();
            InitializePlayers();
            
            // 设置玩家初始筹码
            playerChips = InitialChips;
        }

        // 初始化玩家
        private void InitializePlayers()
        {
            players.Clear();
            // 创建玩家和2个AI
            players.Add(new Player("你")); // 玩家
            
            // 使用B_AI_Name类生成2个不重复的随机AI姓名
            List<string> aiNames = B_AI_Name.GenerateRandomNames(2);
            
            players.Add(new Player(aiNames[0])); // AI玩家1
            players.Add(new Player(aiNames[1])); // AI玩家2
        }

        private void Update()
        {
            // 按F5键切换窗口显示
            if (Input.GetKeyDown(KeyCode.F5))
            {
                UpdateResolutionSettings();
                showMenu = !showMenu;
                blockGameInput = showMenu;
                Logger.LogInfo(showMenu ? "斗地主窗口已打开" : "斗地主窗口已关闭");
            }

            // 阻止游戏输入当窗口显示时
            if (blockGameInput)
            {
                if (Input.mouseScrollDelta.y != 0)
                {
                    Input.ResetInputAxes();
                }

                if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
                {
                    Input.ResetInputAxes();
                }

                if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.F5))
                {
                    Input.ResetInputAxes();
                }
            }
        }

        private void OnGUI()
        {
            if (!showMenu) return;

            // 保存窗口背景色并设置为半透明
            Color originalBackgroundColor = GUI.backgroundColor;
            Color originalContentColor = GUI.contentColor;

            // 显示一个半透明的背景遮罩
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
            GUI.color = new Color(0, 0, 0, 0.5f); // 加深透明度以突出主窗口
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.EndGroup();

            // 应用缩放因子
            Matrix4x4 guiMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleFactor, scaleFactor, 1f));

            // 重置UI样式为默认值
            ResetUIStyles();

            // 显示游戏结束窗口
            if (currentState == GameState.GameOver)
            {
                gameOverWindowRect = GUI.Window(1, gameOverWindowRect, DrawGameOverWindow, "游戏结束");
            }
            else
            {
                // 创建主游戏窗口
                windowRect = GUI.Window(0, windowRect, DrawWindow, "", GUI.skin.window);
            }

            // 恢复原始矩阵和背景色
            GUI.matrix = guiMatrix;
            GUI.backgroundColor = originalBackgroundColor;
            GUI.contentColor = originalContentColor;
        }

        // 重置UI样式
        private void ResetUIStyles()
        {
            int baseFontSize = Mathf.RoundToInt(12 * scaleFactor);
            int titleFontSize = Mathf.RoundToInt(18 * scaleFactor);
            int buttonFontSize = Mathf.RoundToInt(14 * scaleFactor);

            // 窗口样式
            GUI.skin.window.fontSize = baseFontSize;
            GUI.skin.window.padding = new RectOffset(
                Mathf.RoundToInt(20 * scaleFactor),
                Mathf.RoundToInt(20 * scaleFactor),
                Mathf.RoundToInt(10 * scaleFactor),
                Mathf.RoundToInt(10 * scaleFactor)
            );
            GUI.skin.window.normal.background = MakeTex(1, 1, new Color(0.15f, 0.15f, 0.2f, 0.95f)); // 深色调窗口背景
            GUI.skin.window.border = new RectOffset(5, 5, 5, 5);
            GUI.skin.window.alignment = TextAnchor.UpperLeft;

            // 标签样式
            GUI.skin.label.fontSize = baseFontSize;
            GUI.skin.label.normal.textColor = Color.white;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;

            // 按钮样式
            GUI.skin.button.fontSize = buttonFontSize;
            GUI.skin.button.normal.background = MakeTex(1, 1, new Color(0.4f, 0.4f, 0.4f, 0.9f));
            GUI.skin.button.hover.background = MakeTex(1, 1, new Color(0.5f, 0.5f, 0.5f, 0.9f));
            GUI.skin.button.active.background = MakeTex(1, 1, new Color(0.3f, 0.3f, 0.3f, 0.9f));
            GUI.skin.button.normal.textColor = Color.white;
            GUI.skin.button.hover.textColor = Color.white;
            GUI.skin.button.active.textColor = Color.white;
            GUI.skin.button.padding = new RectOffset(10, 10, 5, 5);
        }

        // 创建自定义按钮样式
        private GUIStyle CreateButtonStyle(Color normalColor, Color hoverColor)
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.normal.background = MakeTex(1, 1, normalColor);
            style.hover.background = MakeTex(1, 1, hoverColor);
            style.active.background = MakeTex(1, 1, new Color(normalColor.r * 0.8f, normalColor.g * 0.8f, normalColor.b * 0.8f, 0.9f));
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.active.textColor = Color.white;
            style.fontSize = Mathf.RoundToInt(14 * scaleFactor);
            style.fontStyle = FontStyle.Bold;
            return style;
        }

        private void DrawWindow(int windowID)
        {
            // 添加额外的null检查以防止空引用异常
            if (players == null || players.Count == 0)
            {
                Logger.LogError("DrawWindow: 玩家列表为空，无法显示游戏窗口");
                GUILayout.BeginVertical();
                GUILayout.Space(50);
                GUILayout.Label("游戏数据异常，请返回主菜单并重新开始游戏", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("返回主窗口", CreateButtonStyle(new Color(0.6f, 0.4f, 0.2f, 0.9f), new Color(0.7f, 0.5f, 0.3f, 0.9f))))
                {
                    ReturnToMainMenu();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return;
            }
            
            // 允许拖动窗口
            GUI.DragWindow(new Rect(0, 0, windowRect.width, 25));
            
            // 窗口最小宽度和高度
            windowRect.width = Mathf.Max(windowRect.width, 900f * scaleFactor);
            windowRect.height = Mathf.Max(windowRect.height, 750f * scaleFactor);
            
            GUILayout.BeginVertical();
            GUILayout.Space(10f * scaleFactor);
            
            // 绘制标题栏
            DrawTitleBar();
            
            // 绘制状态信息面板
            DrawStatusInfo();
            
            // 绘制游戏区域，根据不同状态显示不同内容
            if (currentState == GameState.NotStarted)
            {
                DrawStartGameScreen();
            }
            else if (currentState == GameState.Bidding)
            {
                DrawGameModeSelection();
            }
            else if (currentState == GameState.Playing)
            {
                DrawGamePlayingScreen();
            }
            
            // 绘制底部按钮
            DrawBottomButtons();
            
            GUILayout.EndVertical();
        }

        // 绘制标题栏
        private void DrawTitleBar()
        {
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontSize = Mathf.RoundToInt(22 * scaleFactor);
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = new Color(1.0f, 0.84f, 0.0f, 1.0f); // 金色文字
            titleStyle.padding = new RectOffset(0, 0, 10, 10);
            
            // 创建标题背景
            GUI.backgroundColor = new Color(0.2f, 0.5f, 0.2f, 0.8f); // 绿色背景
            GUILayout.Box("", GUILayout.Height(50f * scaleFactor));
            GUI.backgroundColor = Color.white;
            
            // 将标题放在背景上
            GUI.BeginGroup(new Rect(0, 0, windowRect.width / scaleFactor, 50f * scaleFactor));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("三人斗地主游戏 v" + CURRENT_VERSION, titleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.EndGroup();
            
            GUILayout.Space(20f * scaleFactor);
        }

        // 绘制状态信息面板
        private void DrawStatusInfo()
        {
            GUIStyle statusStyle = new GUIStyle(GUI.skin.box);
            statusStyle.padding = new RectOffset(20, 20, 15, 15);
            statusStyle.normal.background = MakeTex(1, 1, new Color(0.25f, 0.25f, 0.35f, 0.8f));
            
            GUILayout.BeginVertical(statusStyle);
            
            GUILayout.BeginHorizontal();
            
            // 游戏状态
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
            labelStyle.normal.textColor = GetStateColor(currentState);
            labelStyle.alignment = TextAnchor.MiddleLeft;
            
            GUILayout.Label("当前阶段：", GUILayout.Width(100f * scaleFactor));
            GUILayout.Label(GetGameStateText(), labelStyle, GUILayout.Width(200f * scaleFactor));
            
            GUILayout.FlexibleSpace();
            
            // 底池信息
            string currencyType = UsePaperMoney ? "宝钞" : "金钞";
            GUILayout.Label("底池：", GUILayout.Width(60f * scaleFactor));
            GUILayout.Label(pot + " " + currencyType, labelStyle, GUILayout.Width(150f * scaleFactor));
            
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            
            GUILayout.Space(20f * scaleFactor);
        }

        // 根据游戏状态获取颜色
        private Color GetStateColor(GameState state)
        {
            switch (state)
            {
                case GameState.NotStarted:
                    return new Color(0.8f, 0.8f, 0.8f, 1f); // 灰色
                case GameState.Bidding:
                    return new Color(0.0f, 0.8f, 0.8f, 1f); // 青色
                case GameState.Playing:
                    return new Color(0.0f, 1.0f, 0.0f, 1f); // 绿色
                case GameState.GameOver:
                    return new Color(1.0f, 0.5f, 0.0f, 1f); // 橙色
                default:
                    return Color.white;
            }
        }

        // 绘制游戏开始界面
        private void DrawStartGameScreen()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            
            // 居中显示开始游戏按钮
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            GUIStyle startButtonStyle = CreateButtonStyle(
                new Color(0.2f, 0.6f, 0.2f, 0.9f), // 绿色按钮
                new Color(0.3f, 0.7f, 0.3f, 0.9f)  // 悬停颜色
            );
            startButtonStyle.fontSize = Mathf.RoundToInt(20 * scaleFactor);
            
            if (GUILayout.Button("开始游戏", startButtonStyle, 
                GUILayout.Width(250f * scaleFactor), 
                GUILayout.Height(80f * scaleFactor)))
            {
                StartNewGame();
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            // 游戏说明
            GUIStyle descriptionStyle = new GUIStyle(GUI.skin.label);
            descriptionStyle.alignment = TextAnchor.MiddleCenter;
            descriptionStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            descriptionStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
            descriptionStyle.wordWrap = true;
            
            GUILayout.Space(40f * scaleFactor);
            GUILayout.Label("三人斗地主游戏，与两位AI玩家一起游戏\n按F5键可以切换窗口显示", 
                descriptionStyle, GUILayout.Width(600f * scaleFactor));
            
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        // 绘制游戏模式选择界面（叫地主阶段）
        private void DrawGameModeSelection()
        {
            // 显示玩家信息
            DrawPlayerInfo();
            
            GUILayout.Space(20f * scaleFactor);
            
            // 显示地主牌
            if (landlordCards != null && landlordCards.Count > 0)
            {
                GUIStyle sectionTitleStyle = new GUIStyle(GUI.skin.label);
                sectionTitleStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
                sectionTitleStyle.normal.textColor = new Color(1.0f, 0.84f, 0.0f, 1.0f); // 金色文字
                sectionTitleStyle.alignment = TextAnchor.MiddleCenter;
                
                GUILayout.Label("地主牌（3张）", sectionTitleStyle);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                
                for (int i = 0; i < 3; i++)
                {
                    // 显示背面
                    DrawCard(new Card(Suit.Hearts, Rank.Three, false));
                    GUILayout.Space(10f * scaleFactor);
                }
                
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(30f * scaleFactor);
            }
            
            // 玩家决策区域
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (currentPlayer != null && currentPlayer.Name == "你")
            {
                // 玩家回合：根据是否有人叫过地主显示不同的按钮
                string bidButtonText = hasSomeoneBid ? "抢地主" : "叫地主";
                string passButtonText = hasSomeoneBid ? "不抢" : "不叫";
                
                GUIStyle bidButtonStyle = CreateButtonStyle(
                    new Color(0.7f, 0.3f, 0.3f, 0.9f), // 红色按钮
                    new Color(0.8f, 0.4f, 0.4f, 0.9f)
                );
                
                GUIStyle passButtonStyle = CreateButtonStyle(
                    new Color(0.3f, 0.3f, 0.6f, 0.9f), // 蓝色按钮
                    new Color(0.4f, 0.4f, 0.7f, 0.9f)
                );
                
                if (GUILayout.Button(bidButtonText, bidButtonStyle, 
                    GUILayout.Width(180f * scaleFactor), 
                    GUILayout.Height(60f * scaleFactor)))
                {
                    BidLandlord(true);
                }
                
                GUILayout.Space(20f * scaleFactor);
                
                if (GUILayout.Button(passButtonText, passButtonStyle, 
                    GUILayout.Width(180f * scaleFactor), 
                    GUILayout.Height(60f * scaleFactor)))
                {
                    BidLandlord(false);
                }
            }
            else
            {
                // AI回合：显示等待文本和动画
                GUIStyle waitStyle = new GUIStyle(GUI.skin.label);
                waitStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
                waitStyle.alignment = TextAnchor.MiddleCenter;
                waitStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 1f);
                
                // 添加简单的动画效果
                string waitingText = (currentPlayer != null ? currentPlayer.Name : "") + " 正在决定是否叫地主";
                int dotsCount = (int)(Time.time * 2) % 4;
                for (int i = 0; i < dotsCount; i++)
                {
                    waitingText += ".";
                }
                
                GUILayout.Label(waitingText, waitStyle, GUILayout.Width(400f * scaleFactor), GUILayout.Height(60f * scaleFactor));
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        // 绘制游戏进行中界面
        private void DrawGamePlayingScreen()
        {
            // 显示玩家信息
            DrawPlayerInfo();
            
            GUILayout.Space(20f * scaleFactor);
            
            // 显示最后出牌信息
            if (lastPlayerWhoPlayed != null && currentPlayedCards != null && currentPlayedCards.Count > 0)
            {
                GUIStyle lastPlayStyle = new GUIStyle(GUI.skin.box);
                lastPlayStyle.padding = new RectOffset(15, 15, 10, 10);
                lastPlayStyle.normal.background = MakeTex(1, 1, new Color(0.3f, 0.3f, 0.4f, 0.7f));
                
                GUILayout.BeginHorizontal(lastPlayStyle);
                GUILayout.Label("最后出牌：" + lastPlayerWhoPlayed.Name + " 出了 ", GUILayout.Width(200f * scaleFactor));
                
                foreach (var card in currentPlayedCards)
                {
                    DrawSmallCard(card);
                    GUILayout.Space(3f * scaleFactor);
                }
                
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                GUILayout.Space(20f * scaleFactor);
            }
            
            // 显示地主牌（如果已翻开）
            if (landlordCards != null && landlordCards.Count > 0 && currentState == GameState.Playing)
            {
                GUIStyle sectionTitleStyle = new GUIStyle(GUI.skin.label);
                sectionTitleStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
                sectionTitleStyle.normal.textColor = new Color(1.0f, 0.84f, 0.0f, 1.0f); // 金色文字
                sectionTitleStyle.alignment = TextAnchor.MiddleCenter;
                
                GUILayout.Label("地主牌", sectionTitleStyle);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                
                foreach (var card in landlordCards)
                {
                    DrawCard(card);
                    GUILayout.Space(10f * scaleFactor);
                }
                
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(30f * scaleFactor);
            }
            
            // 玩家手牌区域
            GUILayout.BeginVertical();
            
            GUIStyle handTitleStyle = new GUIStyle(GUI.skin.label);
            handTitleStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
            handTitleStyle.normal.textColor = Color.white;
            handTitleStyle.alignment = TextAnchor.MiddleLeft;
            
            GUILayout.Label("你的手牌：", handTitleStyle);
            
            // 可滚动的手牌区域
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, 
                GUILayout.Width(windowRect.width / scaleFactor - 40), 
                GUILayout.Height(150f * scaleFactor));
            
            GUILayout.BeginHorizontal();
            
            if (players[0] != null && players[0].Hand != null)
            {
                // 按牌面值排序
                var sortedHand = players[0].Hand.OrderBy(c => c.rank).ThenBy(c => c.suit).ToList();
                
                foreach (var card in sortedHand)
                {
                    DrawPlayerCard(card);
                    GUILayout.Space(5f * scaleFactor);
                }
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            
            // 显示已选择的卡牌数量
            if (players[0] != null && players[0].SelectedCards != null)
            {
                GUIStyle selectedCountStyle = new GUIStyle(GUI.skin.label);
                selectedCountStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
                selectedCountStyle.normal.textColor = players[0].SelectedCards.Count > 0 ? 
                    new Color(1.0f, 0.84f, 0.0f, 1.0f) : Color.white; // 金色文字
                
                GUILayout.Label("已选择：" + players[0].SelectedCards.Count + " 张牌", selectedCountStyle);
            }
            
            GUILayout.EndVertical();
            
            GUILayout.Space(30f * scaleFactor);
            
            // 操作按钮区域
            DrawActionButtons();
        }

        // 绘制玩家信息
        private void DrawPlayerInfo()
        {
            GUIStyle playerInfoStyle = new GUIStyle(GUI.skin.box);
            playerInfoStyle.padding = new RectOffset(20, 20, 15, 15);
            playerInfoStyle.normal.background = MakeTex(1, 1, new Color(0.2f, 0.2f, 0.3f, 0.8f));
            
            GUILayout.BeginHorizontal(playerInfoStyle);
            
            foreach (var player in players)
            {
                if (player == null) continue;
                
                GUIStyle playerLabelStyle = new GUIStyle(GUI.skin.label);
                playerLabelStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
                
                // 玩家自己显示为绿色，地主显示为红色，其他显示为白色
                if (player.Name == "你")
                    playerLabelStyle.normal.textColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                else if (player.IsLandlord)
                    playerLabelStyle.normal.textColor = new Color(1.0f, 0.4f, 0.4f, 1.0f);
                else
                    playerLabelStyle.normal.textColor = Color.white;
                
                GUILayout.BeginVertical();
                GUILayout.Label(player.Name + (player.IsLandlord ? " (地主)" : ""), playerLabelStyle);
                GUILayout.Label("筹码：" + player.Chips, playerLabelStyle);
                
                if (player.Name != "你")
                {
                    // 显示AI手牌数量
                    GUILayout.Label("手牌：" + (player.Hand != null ? player.Hand.Count.ToString() : "0") + " 张", playerLabelStyle);
                }
                
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }
            
            GUILayout.EndHorizontal();
        }

        // 绘制玩家可点击的卡牌
        private void DrawPlayerCard(Card card)
        {
            // 检查卡牌是否被选中
            bool isSelected = players[0] != null && players[0].SelectedCards != null && 
                              players[0].SelectedCards.Any(c => c.id == card.id);
            
            // 创建卡牌样式
            GUIStyle cardStyle = new GUIStyle(GUI.skin.box);
            cardStyle.alignment = TextAnchor.MiddleCenter;
            cardStyle.fontSize = Mathf.RoundToInt(20 * scaleFactor);
            cardStyle.fontStyle = FontStyle.Bold;
            
            // 设置卡牌背景色
            if (isSelected && currentState == GameState.Playing)
            {
                cardStyle.normal.background = MakeTex(1, 1, new Color(1.0f, 0.84f, 0.0f, 0.9f)); // 金色背景
                cardStyle.normal.textColor = Color.black;
            }
            else
            {
                // 根据牌的花色设置不同的背景色
                cardStyle.normal.background = MakeTex(1, 1, GetCardColor(card));
                // 根据牌的花色设置文字颜色
                cardStyle.normal.textColor = (card.suit == Suit.Hearts || card.suit == Suit.Diamonds) ? 
                    Color.red : Color.black;
                if (card.rank == Rank.Joker || card.rank == Rank.BigJoker)
                    cardStyle.normal.textColor = Color.red;
            }
            
            // 绘制卡牌并检测点击
            if (GUILayout.Button(card.ToString(), cardStyle, 
                GUILayout.Width(80f * scaleFactor), 
                GUILayout.Height(110f * scaleFactor)) && 
                currentState == GameState.Playing && 
                players[0] != null && 
                players[0].Name == "你")
            {
                // 点击卡牌时选择或取消选择
                SelectCard(players[0], card);
            }
        }

        // 绘制小型卡牌（用于显示最后出牌）
        private void DrawSmallCard(Card card)
        {
            GUIStyle cardStyle = new GUIStyle(GUI.skin.box);
            cardStyle.alignment = TextAnchor.MiddleCenter;
            cardStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
            
            // 设置卡牌背景和文字颜色
            cardStyle.normal.background = MakeTex(1, 1, GetCardColor(card));
            cardStyle.normal.textColor = (card.suit == Suit.Hearts || card.suit == Suit.Diamonds) ? 
                Color.red : Color.black;
            if (card.rank == Rank.Joker || card.rank == Rank.BigJoker)
                cardStyle.normal.textColor = Color.red;
            
            GUILayout.Box(card.ToString(), cardStyle, 
                GUILayout.Width(60f * scaleFactor), 
                GUILayout.Height(40f * scaleFactor));
        }

        // 获取卡牌的背景颜色
        private Color GetCardColor(Card card)
        {
            // 根据牌的类型设置不同的背景色
            if (!card.isVisible) // 背面
                return new Color(0.7f, 0.2f, 0.2f, 0.9f); // 红色背面
            
            // 普通牌的浅色背景
            switch (card.suit)
            {
                case Suit.Hearts:
                    return new Color(1.0f, 0.95f, 0.95f, 0.9f); // 浅红色
                case Suit.Diamonds:
                    return new Color(1.0f, 0.98f, 0.9f, 0.9f);  // 浅黄色
                case Suit.Clubs:
                    return new Color(0.95f, 1.0f, 0.95f, 0.9f); // 浅绿色
                case Suit.Spades:
                    return new Color(0.95f, 0.95f, 1.0f, 0.9f); // 浅蓝色
                default:
                    return new Color(1.0f, 1.0f, 1.0f, 0.9f);   // 白色
            }
        }

        // 绘制操作按钮
        private void DrawActionButtons()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (currentPlayer != null && currentPlayer.Name == "你")
            {
                // 玩家回合：出牌/不出
                bool hasSelectedCards = players[0].SelectedCards != null && players[0].SelectedCards.Count > 0;
                string buttonText = hasSelectedCards ? "出牌" : "随机出牌";
                
                GUIStyle playButtonStyle = CreateButtonStyle(
                    new Color(0.2f, 0.6f, 0.2f, 0.9f), // 绿色按钮
                    new Color(0.3f, 0.7f, 0.3f, 0.9f)
                );
                
                GUIStyle passButtonStyle = CreateButtonStyle(
                    new Color(0.5f, 0.5f, 0.5f, 0.9f), // 灰色按钮
                    new Color(0.6f, 0.6f, 0.6f, 0.9f)
                );
                
                if (GUILayout.Button(buttonText, playButtonStyle, 
                    GUILayout.Width(150f * scaleFactor), 
                    GUILayout.Height(50f * scaleFactor)))
                {
                    if (hasSelectedCards)
                    {
                        // 使用玩家选择的卡牌
                        PlaySelectedCards();
                    }
                    else
                    {
                        // 随机出牌
                        PlayRandomCard();
                    }
                }
                
                GUILayout.Space(15f * scaleFactor);
                
                if (GUILayout.Button("不出", passButtonStyle, 
                    GUILayout.Width(150f * scaleFactor), 
                    GUILayout.Height(50f * scaleFactor)))
                {
                    Pass();
                }
                
                // 添加清空选择按钮
                if (hasSelectedCards)
                {
                    GUILayout.Space(15f * scaleFactor);
                    GUIStyle clearButtonStyle = CreateButtonStyle(
                        new Color(0.7f, 0.3f, 0.3f, 0.9f), // 红色按钮
                        new Color(0.8f, 0.4f, 0.4f, 0.9f)
                    );
                    
                    if (GUILayout.Button("清空选择", clearButtonStyle, 
                        GUILayout.Width(150f * scaleFactor), 
                        GUILayout.Height(50f * scaleFactor)))
                    {
                        ClearSelectedCards(players[0]);
                    }
                }
            }
            else
            {
                // AI回合：显示等待文本和动画
                GUIStyle waitStyle = new GUIStyle(GUI.skin.label);
                waitStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
                waitStyle.alignment = TextAnchor.MiddleCenter;
                waitStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 1f);
                
                // 添加简单的动画效果
                string waitingText = (currentPlayer != null ? currentPlayer.Name : "") + " 正在出牌";
                int dotsCount = (int)(Time.time * 2) % 4;
                for (int i = 0; i < dotsCount; i++)
                {
                    waitingText += ".";
                }
                
                GUILayout.Label(waitingText, waitStyle, GUILayout.Width(300f * scaleFactor), GUILayout.Height(50f * scaleFactor));
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        // 绘制底部按钮
        private void DrawBottomButtons()
        {
            GUILayout.Space(30f * scaleFactor);
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            GUIStyle returnButtonStyle = CreateButtonStyle(
                new Color(0.6f, 0.4f, 0.2f, 0.9f), // 棕色按钮
                new Color(0.7f, 0.5f, 0.3f, 0.9f)
            );
            
            if (GUILayout.Button("返回主窗口", returnButtonStyle, 
                GUILayout.Width(180f * scaleFactor), 
                GUILayout.Height(50f * scaleFactor)))
            {
                ReturnToMainMenu();
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10f * scaleFactor);
        }

        // 绘制卡牌
        private void DrawCard(Card card)
        {
            // 使用DrawSmallCard方法来保持一致性
            DrawSmallCard(card);
        }

        // 初始化卡牌（包含大小王）
        private void InitializeDeck()
        {
            deck.Clear();
            // 添加普通牌
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                for (int r = (int)Rank.Three; r <= (int)Rank.Two; r++)
                {
                    deck.Add(new Card(suit, (Rank)r));
                }
            }
            // 添加大小王
            deck.Add(new Card(Suit.Hearts, Rank.Joker)); // 小王
            deck.Add(new Card(Suit.Hearts, Rank.BigJoker)); // 大王
        }

        // 洗牌
        private void ShuffleDeck()
        {
            System.Random rng = new System.Random();
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
        }
        
        // 按牌面大小排序卡牌
        private List<Card> SortCards(List<Card> cards)
        {
            return cards.OrderBy(card => card.rank).ToList();
        }
        
        // 获取卡牌的点数（用于牌型判断）
        private int GetCardValue(Card card)
        {
            switch (card.rank)
            {
                case Rank.Joker:
                    return 16;
                case Rank.BigJoker:
                    return 17;
                default:
                    return (int)card.rank;
            }
        }
        
        // 判断牌型
        private (CardType, int) DetermineCardType(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
                return (CardType.Invalid, 0);
                
            // 对卡牌按点数排序
            List<Card> sortedCards = SortCards(cards);
            int cardCount = sortedCards.Count;
            
            // 单牌
            if (cardCount == 1)
                return (CardType.Single, GetCardValue(sortedCards[0]));
            
            // 王炸
            if (cardCount == 2 && sortedCards[0].rank == Rank.Joker && sortedCards[1].rank == Rank.BigJoker)
                return (CardType.Rocket, 100);
            
            // 对子
            if (cardCount == 2 && sortedCards[0].rank == sortedCards[1].rank)
                return (CardType.Pair, GetCardValue(sortedCards[0]));
            
            // 三带
            if (cardCount == 3 && sortedCards[0].rank == sortedCards[1].rank && sortedCards[1].rank == sortedCards[2].rank)
                return (CardType.Triple, GetCardValue(sortedCards[0]));
            
            // 炸弹
            if (cardCount == 4 && sortedCards[0].rank == sortedCards[1].rank && sortedCards[1].rank == sortedCards[2].rank && sortedCards[2].rank == sortedCards[3].rank)
                return (CardType.Bomb, GetCardValue(sortedCards[0]));
            
            // 三带一
            if (cardCount == 4)
            {
                var rankGroups = sortedCards.GroupBy(c => c.rank).ToDictionary(g => g.Key, g => g.Count());
                if (rankGroups.Count == 2 && (rankGroups.Values.Max() == 3))
                {
                    var tripleRank = rankGroups.First(g => g.Value == 3).Key;
                    return (CardType.TripleWithSingle, GetCardValue(new Card(Suit.Hearts, tripleRank)));
                }
            }
            
            // 三带对
            if (cardCount == 5)
            {
                var rankGroups = sortedCards.GroupBy(c => c.rank).ToDictionary(g => g.Key, g => g.Count());
                if (rankGroups.Count == 2 && rankGroups.Values.Contains(3) && rankGroups.Values.Contains(2))
                {
                    var tripleRank = rankGroups.First(g => g.Value == 3).Key;
                    return (CardType.TripleWithPair, GetCardValue(new Card(Suit.Hearts, tripleRank)));
                }
            }
            
            // 四带二
            if (cardCount == 6)
            {
                var rankGroups = sortedCards.GroupBy(c => c.rank).ToDictionary(g => g.Key, g => g.Count());
                if (rankGroups.Count == 2 && rankGroups.Values.Contains(4) && rankGroups.Values.Contains(2))
                {
                    var quadRank = rankGroups.First(g => g.Value == 4).Key;
                    return (CardType.QuadWithTwoSingle, GetCardValue(new Card(Suit.Hearts, quadRank)));
                }
            }
            
            // 四带两对
            if (cardCount == 8)
            {
                var rankGroups = sortedCards.GroupBy(c => c.rank).ToDictionary(g => g.Key, g => g.Count());
                if (rankGroups.Count == 3 && rankGroups.Values.Contains(4) && rankGroups.Values.Count(v => v == 2) == 2)
                {
                    var quadRank = rankGroups.First(g => g.Value == 4).Key;
                    return (CardType.QuadWithTwoPair, GetCardValue(new Card(Suit.Hearts, quadRank)));
                }
            }
            
            // 检查是否是顺子
            bool isStraight = true;
            if (cardCount >= 5 && cardCount <= 12)
            {
                // 顺子不能包含2和大小王
                for (int i = 0; i < cardCount; i++)
                {
                    if (sortedCards[i].rank >= Rank.Two || sortedCards[i].rank == Rank.Joker || sortedCards[i].rank == Rank.BigJoker)
                    {
                        isStraight = false;
                        break;
                    }
                    if (i > 0 && GetCardValue(sortedCards[i]) != GetCardValue(sortedCards[i-1]) + 1)
                    {
                        isStraight = false;
                        break;
                    }
                }
                if (isStraight)
                    return (CardType.Straight, GetCardValue(sortedCards[cardCount - 1])); // 返回最大的牌点
            }
            
            // 检查是否是对子顺子（连对）
            bool isStraightPair = true;
            if (cardCount >= 6 && cardCount % 2 == 0)
            {
                for (int i = 0; i < cardCount; i += 2)
                {
                    // 检查是否是对子
                    if (i + 1 >= cardCount || sortedCards[i].rank != sortedCards[i + 1].rank)
                    {
                        isStraightPair = false;
                        break;
                    }
                    
                    // 检查是否包含2或大小王
                    if (sortedCards[i].rank >= Rank.Two || sortedCards[i].rank == Rank.Joker || sortedCards[i].rank == Rank.BigJoker)
                    {
                        isStraightPair = false;
                        break;
                    }
                    
                    // 检查是否连续
                    if (i > 0 && GetCardValue(sortedCards[i]) != GetCardValue(sortedCards[i - 2]) + 1)
                    {
                        isStraightPair = false;
                        break;
                    }
                }
                if (isStraightPair)
                    return (CardType.StraightPair, GetCardValue(sortedCards[cardCount - 1])); // 返回最大的牌点
            }
            
            // 检查是否是三带顺子（飞机）
            bool isTripleSequence = true;
            if (cardCount >= 6 && cardCount % 3 == 0)
            {
                for (int i = 0; i < cardCount; i += 3)
                {
                    // 检查是否是三张
                    if (i + 2 >= cardCount || sortedCards[i].rank != sortedCards[i + 1].rank || sortedCards[i + 1].rank != sortedCards[i + 2].rank)
                    {
                        isTripleSequence = false;
                        break;
                    }
                    
                    // 检查是否包含2或大小王
                    if (sortedCards[i].rank >= Rank.Two || sortedCards[i].rank == Rank.Joker || sortedCards[i].rank == Rank.BigJoker)
                    {
                        isTripleSequence = false;
                        break;
                    }
                    
                    // 检查是否连续
                    if (i > 0 && GetCardValue(sortedCards[i]) != GetCardValue(sortedCards[i - 3]) + 1)
                    {
                        isTripleSequence = false;
                        break;
                    }
                }
                if (isTripleSequence)
                    return (CardType.TripleSequence, GetCardValue(sortedCards[cardCount - 1])); // 返回最大的牌点
            }
            
            return (CardType.Invalid, 0);
        }
        
        // 检查牌型是否合法
        private bool IsValidCardType(List<Card> cards, List<Card> lastCards, CardType lastType)
        {
            if (cards == null || cards.Count == 0)
                return false;
                
            var (currentType, currentValue) = DetermineCardType(cards);
            
            // 牌型不合法
            if (currentType == CardType.Invalid)
                return false;
                
            // 第一次出牌，只要牌型合法即可
            if (lastCards == null || lastCards.Count == 0)
                return true;
                
            // 王炸可以打任何牌型
            if (currentType == CardType.Rocket)
                return true;
                
            // 炸弹可以打非炸弹和小王炸以外的牌型
            if (currentType == CardType.Bomb && lastType != CardType.Rocket && lastType != CardType.Bomb)
                return true;
                
            // 炸弹之间比大小
            if (currentType == CardType.Bomb && lastType == CardType.Bomb)
                return currentValue > (DetermineCardType(lastCards)).Item2;
                
            // 检查是否是相同牌型且点数更大
            if (currentType == lastType)
            {
                // 对于不同牌型，需要确保牌的数量也匹配
                switch (currentType)
                {
                    case CardType.Single:
                    case CardType.Pair:
                    case CardType.Triple:
                    case CardType.Straight:
                    case CardType.StraightPair:
                    case CardType.TripleSequence:
                    case CardType.TripleWithSingle:
                    case CardType.TripleWithPair:
                    case CardType.QuadWithTwoSingle:
                    case CardType.QuadWithTwoPair:
                        return cards.Count == lastCards.Count && currentValue > (DetermineCardType(lastCards)).Item2;
                    default:
                        return false;
                }
            }
                
            // 对于复杂牌型的特殊处理规则
            // 例如：三带一可以打三带一，三带对可以打三带对，等等
            // 这里已经在上面的switch语句中涵盖了这些情况
                
            return false;
        }
        
        // 选择或取消选择卡牌
        private void SelectCard(Player player, Card card)
        {
            if (player.SelectedCards.Any(c => c.id == card.id))
            {
                // 取消选择
                player.SelectedCards.RemoveAll(c => c.id == card.id);
            }
            else
            {
                // 选择卡牌
                player.SelectedCards.Add(card);
            }
        }
        
        // 清空选择的卡牌
        private void ClearSelectedCards(Player player)
        {
            player.SelectedCards.Clear();
        }

        // 开始新游戏
        private void StartNewGame()
        {
            InitializeDeck();
            ShuffleDeck();
            
            // 保存玩家当前的筹码
            int userChips = players.Count > 0 ? players[0].Chips : InitialChips;
            
            // 初始化玩家
            InitializePlayers();
            
            // 设置玩家的筹码
            if (players.Count > 0)
            {
                players[0].Chips = userChips;
                
                // 随机设置AI筹码为玩家持有筹码的0.5到3倍范围
                System.Random random = new System.Random();
                for (int i = 1; i < players.Count; i++)
                {
                    // 生成0.5到3倍玩家筹码的随机数
                    float randomFactor = (float)(random.NextDouble() * 2.5 + 0.5); // 0.5-3.0之间的随机数
                    players[i].Chips = Mathf.RoundToInt(userChips * randomFactor);
                }
            }

            // 清空之前的牌
            landlordCards.Clear();
            foreach (var player in players)
            {
                player.Hand.Clear();
                player.IsLandlord = false;
                player.HasPassed = false;
                player.IsPlaying = true;
                player.HasBid = false;
            }

            // 发牌：每个玩家17张牌，剩下的3张作为地主牌
            for (int i = 0; i < 17; i++)
            {
                foreach (var player in players)
                {
                    player.Hand.Add(deck[0]);
                    deck.RemoveAt(0);
                }
            }
            
            // 剩下的3张牌作为地主牌
            while (deck.Count > 0)
            {
                landlordCards.Add(deck[0]);
                deck.RemoveAt(0);
            }

            // 开始叫地主阶段
            currentState = GameState.Bidding;
            // 随机选择一个玩家开始叫地主
            System.Random rng = new System.Random();
            int startIndex = rng.Next(players.Count);
            currentPlayer = players[startIndex];
            
            // 记录是否已经有人叫地主
            hasSomeoneBid = false;
            
            lastPlayerWhoPlayed = null;
            currentPlayedCards.Clear();
            pot = 0;

            string gameCurrencyType = UsePaperMoney ? "宝钞" : "金钞";
            Logger.LogInfo("三人斗地主游戏已开始，使用" + gameCurrencyType);
        }
        
        // 叫地主
        private void BidLandlord(bool isBid)
        {
            // 添加额外的null检查以防止空引用异常
            if (currentPlayer == null || players == null || landlordCards == null)
            {
                Logger.LogError("BidLandlord: 缺少必要的游戏数据，无法继续");
                return;
            }
            
            if (isBid)
            {
                // 标记当前玩家已叫地主
                currentPlayer.HasBid = true;
                
                // 标记已经有人叫地主
                if (!hasSomeoneBid)
                {
                    hasSomeoneBid = true;
                    
                    // 普通叫地主
                    Logger.LogInfo(currentPlayer.Name + " 叫地主");
                    
                    // 让所有玩家都有机会抢地主
                    int nextIndex = (players.IndexOf(currentPlayer) + 1) % players.Count;
                    currentPlayer = players[nextIndex];
                    
                    // 如果下一个是AI，自动进行决策
                    if (currentPlayer.Name != "你")
                    {
                        Invoke("AIBid", 1.5f);
                    }
                }
                else
                {
                    // 抢地主成功，成为地主
                    currentPlayer.IsLandlord = true;
                    currentPlayer.Hand.AddRange(landlordCards);
                    landlordCards.Clear();
                    
                    Logger.LogInfo(currentPlayer.Name + " 抢地主成功！成为了地主");
                    
                    // 进入游戏阶段
                    currentState = GameState.Playing;
                    lastPlayerWhoPlayed = currentPlayer; // 地主先出牌
                    
                    // 自动让AI玩家进行操作
                    if (currentPlayer.Name != "你")
                    {
                        Invoke("AIPlay", 1.5f);
                    }
                }
            }
            else
            {
                // 根据当前状态显示不同的日志信息
                string actionText = hasSomeoneBid ? "不抢" : "不叫";
                Logger.LogInfo(currentPlayer.Name + " " + actionText);
                currentPlayer.HasPassed = true;
                    currentPlayer.HasPassedCount++;
                
                // 检查是否所有人都不叫/不抢
                int passedCount = 0;
                Player firstBidder = null;
                
                foreach (var player in players)
                {
                    if (player == null) continue; // 跳过null玩家
                    if (player.HasPassed)
                    {
                        passedCount++;
                    }
                    else if (player.HasBid && firstBidder == null)
                    {
                        firstBidder = player; // 找到第一个叫地主的玩家
                    }
                }
                
                bool allPassed = passedCount == players.Count;
                
                if (allPassed)
                {
                    if (hasSomeoneBid && firstBidder != null)
                    {
                        // 所有人都不抢地主，第一个叫地主的玩家成为地主
                        firstBidder.IsLandlord = true;
                        firstBidder.Hand.AddRange(landlordCards);
                        landlordCards.Clear();
                        
                        Logger.LogInfo(firstBidder.Name + " 成为了地主");
                        
                        // 进入游戏阶段
                        currentState = GameState.Playing;
                    }
                    else
                    {
                        // 所有人都不叫地主，重新洗牌和发牌
                        Logger.LogInfo("所有人都不叫地主，重新洗牌和发牌");
                        
                        // 清空地主牌和所有玩家的牌
                        landlordCards.Clear();
                        foreach (var player in players)
                        {
                            if (player != null)
                            {
                                player.Hand.Clear();
                                player.HasPassed = false;
                                player.HasBid = false;
                            }
                        }
                        
                        // 重新洗牌和发牌
                        InitializeDeck();
                        ShuffleDeck();
                        
                        // 重新发牌：每个玩家17张牌，剩下的3张作为地主牌
                        for (int i = 0; i < 17; i++)
                        {
                            foreach (var player in players)
                            {
                                if (player != null)
                                {
                                    player.Hand.Add(deck[0]);
                                    deck.RemoveAt(0);
                                }
                            }
                        }
                        
                        // 剩下的3张牌作为地主牌
                        while (deck.Count > 0)
                        {
                            landlordCards.Add(deck[0]);
                            deck.RemoveAt(0);
                        }
                        
                        // 重新开始叫地主阶段，随机选择一个玩家开始
                        hasSomeoneBid = false;
                        System.Random rng = new System.Random();
                        int startIndex = rng.Next(players.Count);
                        currentPlayer = players[startIndex];
                        
                        Logger.LogInfo("重新开始叫地主阶段，由" + currentPlayer.Name + " 开始叫地主");
                        
                        // 如果是AI玩家，自动叫地主
                        if (currentPlayer.Name != "玩家")
                        {
                            AIBid();
                        }
                    }
                }
                else
                {
                    // 找到下一个没有pass的玩家
                    try
                    {
                        int nextIndex = (players.IndexOf(currentPlayer) + 1) % players.Count;
                        int attempts = 0;
                        int maxAttempts = players.Count;
                        
                        while (attempts < maxAttempts && players[nextIndex] != null && players[nextIndex].HasPassed)
                        {
                            nextIndex = (nextIndex + 1) % players.Count;
                            attempts++;
                        }
                        
                        // 确保找到一个有效的玩家
                        if (attempts < maxAttempts && players[nextIndex] != null)
                        {
                            currentPlayer = players[nextIndex];
                            
                            // 自动让AI玩家进行操作
                            if (currentPlayer.Name != "你")
                            {
                                Invoke("AIBid", 1.5f);
                            }
                        }
                        else
                        {
                            Logger.LogWarning("无法找到有效的下一个玩家，重新开始游戏");
                            StartNewGame();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("选择下一个玩家时出错: " + ex.Message);
                        StartNewGame(); // 出错时重新开始游戏
                    }
                }
            }
        }
        
        // 评估手牌强度
        private float EvaluateHandStrength(List<Card> hand)
        {
            if (hand == null || hand.Count == 0)
                return 0;
            
            float strength = 0;
            
            // 统计各种牌型
            var rankGroups = hand.GroupBy(c => c.rank).ToDictionary(g => g.Key, g => g.Count());
            
            // 炸弹加分（4张相同点数）
            int bombCount = rankGroups.Values.Count(v => v == 4);
            strength += bombCount * 100;
            
            // 三带加分
            int tripleCount = rankGroups.Values.Count(v => v == 3);
            strength += tripleCount * 40;
            
            // 对子加分
            int pairCount = rankGroups.Values.Count(v => v == 2);
            strength += pairCount * 15;
            
            // 大牌点数加分（J, Q, K, A, 2, 王）
            foreach (var card in hand)
            {
                if (card.rank >= Rank.Jack && card.rank <= Rank.Two)
                    strength += 5;
                if (card.rank == Rank.Joker || card.rank == Rank.BigJoker)
                    strength += 15;
            }
            
            // 计算顺子潜力
            // 对子顺子加分
            var straightPairCount = CountStraightPairs(hand);
            strength += straightPairCount * 8;
            
            // 三带顺子加分
            var tripleSequenceCount = CountTripleSequences(hand);
            strength += tripleSequenceCount * 15;
            
            // 王炸特殊加分
            bool hasRocket = hand.Any(c => c.rank == Rank.Joker) && hand.Any(c => c.rank == Rank.BigJoker);
            if (hasRocket)
                strength += 150;
            
            return strength;
        }
        
        // 计算手牌中连续对子的数量
        private int CountStraightPairs(List<Card> hand)
        {
            var rankGroups = hand.GroupBy(c => c.rank).Where(g => g.Count() >= 2).Select(g => GetCardValue(new Card(Suit.Hearts, g.Key))).OrderBy(v => v).ToList();
            return CountConsecutiveGroups(rankGroups);
        }
        
        // 计算手牌中连续三带的数量
        private int CountTripleSequences(List<Card> hand)
        {
            var rankGroups = hand.GroupBy(c => c.rank).Where(g => g.Count() >= 3).Select(g => GetCardValue(new Card(Suit.Hearts, g.Key))).OrderBy(v => v).ToList();
            return CountConsecutiveGroups(rankGroups);
        }
        
        // 辅助方法：计算连续序列的组数
        private int CountConsecutiveGroups(List<int> values)
        {
            if (values.Count < 2)
                return 0;
            
            int maxCount = 1;
            int currentCount = 1;
            
            for (int i = 1; i < values.Count; i++)
            {
                if (values[i] == values[i - 1] + 1)
                {
                    currentCount++;
                    maxCount = Math.Max(maxCount, currentCount);
                }
                else
                {
                    currentCount = 1;
                }
            }
            
            return maxCount >= 2 ? maxCount : 0;
        }
        
        // AI叫地主决策
        private void AIBid()
        {
            if (currentPlayer == null || currentPlayer.Name == "你") return;
            
            // 评估手牌强度
            float handStrength = EvaluateHandStrength(currentPlayer.Hand);
            
            // 基于手牌强度调整叫/抢地主的概率
            System.Random rng = new System.Random();
            bool willBid;
            int probability;
            
            // 手牌强度阈值：低(0-80)、中(81-150)、高(151+)
            if (!hasSomeoneBid)
            {
                // 还没有人叫地主
                if (handStrength > 150) // 高牌力
                    probability = 90; // 90%概率叫地主
                else if (handStrength > 80) // 中牌力
                    probability = 60; // 60%概率叫地主
                else // 低牌力
                    probability = 20; // 20%概率叫地主
            }
            else
            {
                // 已经有人叫地主，抢地主需要更高的牌力
                if (handStrength > 180) // 非常高的牌力
                    probability = 70; // 70%概率抢地主
                else if (handStrength > 120) // 较高牌力
                    probability = 40; // 40%概率抢地主
                else // 低牌力
                    probability = 10; // 10%概率抢地主
            }
            
            willBid = rng.Next(100) < probability;
            
            // 特殊情况：如果有王炸，提高叫/抢地主的意愿
            bool hasRocket = currentPlayer.Hand.Any(c => c.rank == Rank.Joker) && currentPlayer.Hand.Any(c => c.rank == Rank.BigJoker);
            if (hasRocket)
            {
                if (!hasSomeoneBid)
                    willBid = rng.Next(100) < 95; // 95%概率叫地主
                else
                    willBid = rng.Next(100) < 80; // 80%概率抢地主
            }
            
            BidLandlord(willBid);
        }
        
        // 玩家出牌（使用选择的卡牌）
        private void PlaySelectedCards()
        {
            if (currentPlayer == null || currentPlayer.SelectedCards == null || currentPlayer.SelectedCards.Count == 0)
                return;
            
            // 检查牌型是否合法
            if (!IsValidCardType(currentPlayer.SelectedCards, currentPlayedCards, lastPlayedCardType))
            {
                Logger.LogInfo("不合法的牌型，无法出牌");
                return;
            }
            
            // 保存牌型信息
            var (cardType, cardValue) = DetermineCardType(currentPlayer.SelectedCards);
            lastPlayedCardType = cardType;
            
            // 从手牌中移除已选择的卡牌
            string playedCardsText = "";
            foreach (var selectedCard in currentPlayer.SelectedCards)
            {
                currentPlayer.Hand.RemoveAll(card => card.id == selectedCard.id);
                playedCardsText += selectedCard.ToString() + " ";
            }
            
            // 更新当前出牌信息
            currentPlayedCards.Clear();
            currentPlayedCards.AddRange(currentPlayer.SelectedCards);
            lastPlayerWhoPlayed = currentPlayer;
            
            // 清空选择的卡牌
            ClearSelectedCards(currentPlayer);
            
            Logger.LogInfo(currentPlayer.Name + " 出牌：" + playedCardsText);
            
            // 检查是否获胜
            if (currentPlayer.Hand.Count == 0)
            {
                DetermineWinner(currentPlayer);
                return;
            }
            
            // 标记当前玩家已在本回合出牌
            currentPlayer.HasPlayedInCurrentRound = true;
            currentPlayer.TimesPlayed++;
            
            // 重置所有玩家的Pass状态
            foreach (var player in players)
            {
                player.HasPassed = false;
            }
            
            // 轮到下一个玩家
            NextPlayer();
        }
        
        // 更智能的随机出牌方法（主要用于AI玩家）
        private void PlayRandomCard()
        {
            if (currentPlayer == null || currentPlayer.Hand.Count == 0)
                return;
            
            // 获取排序后的手牌
            List<Card> sortedHand = SortCards(new List<Card>(currentPlayer.Hand));
            List<Card> bestRandomCards = null;
            System.Random rng = new System.Random();
            
            // 策略1：优先出单张大牌，避免被对手压制
            if (currentPlayedCards == null || currentPlayedCards.Count == 0)
            {
                // 首回合尝试出最小的牌，保留大牌
                bestRandomCards = new List<Card> { sortedHand[0] };
            }
            else
            {
                // 尝试出能够瓦解手牌结构的牌，优先出散牌或难以组合的牌
                // 1. 尝试出单牌
                if (sortedHand.Count > 0)
                {
                    // 如果有单个的大牌，可以考虑出掉
                    int highCardThreshold = 12; // J以上的牌
                    List<Card> highCards = sortedHand.Where(card => GetCardValue(card) >= highCardThreshold && 
                                                                 !IsPartOfCombo(card, sortedHand)).ToList();
                    
                    if (highCards.Count > 0)
                    {
                        bestRandomCards = new List<Card> { highCards[0] };
                    }
                    else
                    {
                        // 没有合适的大牌，尝试出最小的散牌
                            Card? minCard = FindSmallestUnmatchedCard(sortedHand);
                            if (minCard.HasValue)
                            {
                                bestRandomCards = new List<Card> { minCard.Value };
                            }
                            else
                            {
                                // 最后才随机选一张
                                bestRandomCards = new List<Card> { sortedHand[rng.Next(sortedHand.Count)] };
                            }
                    }
                }
            }
            
            // 执行出牌
            if (bestRandomCards != null && bestRandomCards.Count > 0)
            {
                // 从手牌中移除要出的牌
                foreach (var card in bestRandomCards)
                {
                    currentPlayer.Hand.RemoveAll(c => c.id == card.id);
                }
                
                // 保存牌型信息
                var (cardType, cardValue) = DetermineCardType(bestRandomCards);
                lastPlayedCardType = cardType;
                
                currentPlayedCards.Clear();
                currentPlayedCards.AddRange(bestRandomCards);
                lastPlayerWhoPlayed = currentPlayer;
                
                // 输出日志
                string playedCardsText = string.Join(" ", bestRandomCards.Select(c => c.ToString()));
                Logger.LogInfo(currentPlayer.Name + " 出牌：" + playedCardsText);
                
                // 检查是否获胜
                if (currentPlayer.Hand.Count == 0)
                {
                    DetermineWinner(currentPlayer);
                    return;
                }
                
                // 标记当前玩家已在本回合出牌
                currentPlayer.HasPlayedInCurrentRound = true;
                currentPlayer.TimesPlayed++;
                
                // 重置所有玩家的Pass状态
                foreach (var player in players)
                {
                    player.HasPassed = false;
                }
                
                // 轮到下一个玩家
                NextPlayer();
            }
            else
            {
                // 无法出牌，选择Pass
                Pass();
            }
        }
        
        // 检查一张牌是否是某个组合的一部分
        private bool IsPartOfCombo(Card card, List<Card> hand)
        {
            // 检查这张牌是否属于对子、三带、顺子等组合
            int value = GetCardValue(card);
            int sameValueCount = hand.Count(c => GetCardValue(c) == value);
            
            // 如果这张牌是对子或三带的一部分，认为它是组合的一部分
            if (sameValueCount >= 2)
                return true;
            
            // 检查是否是顺子的一部分
            // 这里简化处理，实际实现需要更复杂的顺子检测
            return false;
        }
        
        // 找到最小的无法组成组合的单牌
        private Card? FindSmallestUnmatchedCard(List<Card> hand)
        {
            // 统计每种牌的数量
            Dictionary<int, int> valueCounts = new Dictionary<int, int>();
            foreach (var card in hand)
            {
                int value = GetCardValue(card);
                if (valueCounts.ContainsKey(value))
                    valueCounts[value]++;
                else
                    valueCounts[value] = 1;
            }
            
            // 找到最小的只出现一次的牌
            int minValue = int.MaxValue;
            Card? result = null;
            
            foreach (var card in hand)
            {
                int value = GetCardValue(card);
                if (valueCounts[value] == 1 && value < minValue)
                {
                    minValue = value;
                    result = card;
                }
            }
            
            return result;
        }
        
        // 不出牌
        private void Pass()
        {
            if (currentPlayer == null)
                return;
            
            currentPlayer.HasPassed = true;
            currentPlayer.HasPassedCount++;
            Logger.LogInfo(currentPlayer.Name + " 不出牌");

            
            // 检查是否其他两个玩家都不出
            int passedCount = 0;
            foreach (var player in players)
            {
                if (player.HasPassed)
                    passedCount++;
            }
            
            if (passedCount == players.Count - 1 && lastPlayerWhoPlayed != null)
            {
                // 其他两个玩家都不出，当前玩家可以重新出牌
                currentPlayedCards.Clear();
                foreach (var player in players)
                {
                    player.HasPassed = false;
                }
                currentPlayer = lastPlayerWhoPlayed;
                
                // 自动让AI玩家进行操作
                if (currentPlayer.Name != "你")
                {
                    Invoke("AIPlay", 1.5f);
                }
            }
            else
            {
                // 轮到下一个玩家
                NextPlayer();
            }
        }
        
        // AI出牌决策
        private void AIPlay()
        {
            if (currentPlayer == null || currentPlayer.Name == "你" || currentPlayer.Hand.Count == 0)
                return;
            
            // 更智能的AI决策
            System.Random rng = new System.Random();
            
            // 尝试找到合适的牌型出牌
            List<Card> bestCardsToPlay = FindBestCardsToPlay(currentPlayer.Hand);
            
            if (bestCardsToPlay != null && bestCardsToPlay.Count > 0 && IsValidCardType(bestCardsToPlay, currentPlayedCards, lastPlayedCardType))
            {
                // 有合适的牌型可以出
                // 模拟选择卡牌
                currentPlayer.SelectedCards.Clear();
                currentPlayer.SelectedCards.AddRange(bestCardsToPlay);
                
                // 延迟执行，模拟思考时间
                Invoke("PlaySelectedCardsForAI", 1.0f);
            }
            else
            {
                // 没有合适的牌型，随机决定是否出牌或不出
                bool willPlay = rng.Next(100) < 30; // 30%的概率尝试随机出牌
                
                if (willPlay && currentPlayer.Hand.Count > 0)
                {
                    PlayRandomCard();
                }
                else
                {
                    Pass();
                }
            }
        }
        
        // 为AI玩家执行出牌操作
        private void PlaySelectedCardsForAI()
        {
            PlaySelectedCards();
        }
        
        // 为AI找到最佳出牌组合
        private List<Card> FindBestCardsToPlay(List<Card> hand)
        {
            if (hand == null || hand.Count == 0)
                return null;
            
            // 排序手牌
            List<Card> sortedHand = SortCards(new List<Card>(hand));
            
            // 如果是第一个出牌的玩家，选择最优的开局牌型
            if (currentPlayedCards == null || currentPlayedCards.Count == 0)
            {
                return SelectBestOpeningCards(sortedHand);
            }
            else
            {
                // 尝试用相同牌型但更大的牌压制
                var (lastType, lastValue) = DetermineCardType(currentPlayedCards);
                
                // 如果当前没有出牌或者可以压制，尝试找合适的牌型
                if (lastType != CardType.Invalid)
                {
                    // 根据不同的牌型实现压制逻辑
                    switch (lastType)
                    {
                        case CardType.Single:
                            return FindHigherSingle(sortedHand, lastValue);
                        case CardType.Pair:
                            return FindHigherPair(sortedHand, lastValue);
                        case CardType.Triple:
                            return FindHigherTriple(sortedHand, lastValue);
                        case CardType.Straight:
                            return FindHigherStraight(sortedHand, lastValue, currentPlayedCards.Count);
                        case CardType.StraightPair:
                            return FindHigherStraightPair(sortedHand, lastValue, currentPlayedCards.Count);
                        case CardType.TripleSequence:
                            return FindHigherTripleSequence(sortedHand, lastValue, currentPlayedCards.Count);
                        case CardType.TripleWithSingle:
                            return FindHigherTripleWithExtras(sortedHand, lastValue, true, currentPlayedCards.Count);
                        case CardType.TripleWithPair:
                            return FindHigherTripleWithExtras(sortedHand, lastValue, false, currentPlayedCards.Count);
                        case CardType.QuadWithTwoSingle:
                        case CardType.QuadWithTwoPair:
                            // 四带可以用更大的四带或炸弹压制
                            var quad = FindHigherQuad(sortedHand, lastValue);
                            if (quad != null)
                                return quad;
                            break;
                    }
                }
                
                // 尝试用炸弹压制
                List<Card> bomb = FindBomb(sortedHand);
                if (bomb != null && IsValidCardType(bomb, currentPlayedCards, lastPlayedCardType))
                {
                    // 除非万不得已，否则不要轻易使用炸弹
                    // 如果手牌数量较少或者对方牌力很强，可以考虑使用炸弹
                    bool shouldUseBomb = sortedHand.Count < 6 || IsStrongOpponentHand();
                    if (shouldUseBomb || GetCardValue(bomb[0]) > 15) // 大牌炸弹可以考虑使用
                    {
                        return bomb;
                    }
                }
            }
            
            return null;
        }
        
        // 选择最佳的开局牌型
        private List<Card> SelectBestOpeningCards(List<Card> sortedHand)
        {
            // 1. 尝试出连对、飞机等可以减少手牌数量的组合
            List<Card> bestCombo = null;
            
            // 寻找最长的三带顺子（飞机）
            bestCombo = FindLongestTripleSequence(sortedHand) ?? bestCombo;
            
            // 寻找最长的连对
            if (bestCombo == null)
            {
                bestCombo = FindLongestStraightPair(sortedHand);
            }
            
            // 寻找最长的顺子
            if (bestCombo == null)
            {
                bestCombo = FindLongestStraight(sortedHand);
            }
            
            // 尝试出三带
            if (bestCombo == null)
            {
                bestCombo = FindLowestTriple(sortedHand);
            }
            
            // 尝试出对子
            if (bestCombo == null)
            {
                bestCombo = FindLowestPair(sortedHand);
            }
            
            // 最后尝试出最小的单牌
            if (bestCombo == null && sortedHand.Count > 0)
            {
                bestCombo = new List<Card> { sortedHand[0] };
            }
            
            return bestCombo;
        }
        
        // 寻找比指定值大的最小单牌
        private List<Card> FindHigherSingle(List<Card> sortedHand, int minValue)
        {
            for (int i = 0; i < sortedHand.Count; i++)
            {
                if (GetCardValue(sortedHand[i]) > minValue)
                {
                    return new List<Card> { sortedHand[i] };
                }
            }
            return null;
        }
        
        // 寻找比指定值大的最小对子
        private List<Card> FindHigherPair(List<Card> sortedHand, int minValue)
        {
            for (int i = 0; i < sortedHand.Count - 1; i++)
            {
                if (sortedHand[i].rank == sortedHand[i + 1].rank && GetCardValue(sortedHand[i]) > minValue)
                {
                    return new List<Card> { sortedHand[i], sortedHand[i + 1] };
                }
            }
            return null;
        }
        
        // 寻找比指定值大的最小三带
        private List<Card> FindHigherTriple(List<Card> sortedHand, int minValue)
        {
            for (int i = 0; i < sortedHand.Count - 2; i++)
            {
                if (sortedHand[i].rank == sortedHand[i + 1].rank && 
                    sortedHand[i + 1].rank == sortedHand[i + 2].rank && 
                    GetCardValue(sortedHand[i]) > minValue)
                {
                    return new List<Card> { sortedHand[i], sortedHand[i + 1], sortedHand[i + 2] };
                }
            }
            return null;
        }
        
        // 寻找比指定值大的最小顺子
        private List<Card> FindHigherStraight(List<Card> sortedHand, int minValue, int length)
        {
            // 确保至少需要5张牌才能组成顺子
            if (length < 5 || sortedHand.Count < length)
                return null;
            
            // 获取去重后的值，按升序排列
            List<int> distinctValues = sortedHand.Select(card => GetCardValue(card))
                                                .Distinct()
                                                .Where(v => v > minValue && v < 16) // 不考虑2和王
                                                .OrderBy(v => v)
                                                .ToList();
            
            // 寻找连续的值序列
            for (int i = 0; i <= distinctValues.Count - length; i++)
            {
                bool isStraight = true;
                for (int j = 0; j < length - 1; j++)
                {
                    if (distinctValues[i + j + 1] - distinctValues[i + j] != 1)
                    {
                        isStraight = false;
                        break;
                    }
                }
                
                if (isStraight)
                {
                    // 找到了符合条件的顺子，收集对应的牌
                    List<Card> straight = new List<Card>();
                    for (int j = 0; j < length; j++)
                    {
                        int value = distinctValues[i + j];
                        // 取一张该值的牌
                        straight.Add(sortedHand.First(card => GetCardValue(card) == value));
                    }
                    return straight;
                }
            }
            
            return null;
        }
        
        // 寻找比指定值大的最小连对
        private List<Card> FindHigherStraightPair(List<Card> sortedHand, int minValue, int length)
        {
            // 连对长度需要是偶数，且至少为6（3对）
            if (length % 2 != 0 || length < 6)
                return null;
            
            int pairCount = length / 2;
            
            // 统计每种牌值的数量
            Dictionary<int, List<Card>> valueGroups = new Dictionary<int, List<Card>>();
            foreach (var card in sortedHand)
            {
                int value = GetCardValue(card);
                if (value > minValue && value < 16) // 不考虑2和王
                {
                    if (!valueGroups.ContainsKey(value))
                        valueGroups[value] = new List<Card>();
                    valueGroups[value].Add(card);
                }
            }
            
            // 找出有至少两张牌的牌值，并按升序排列
            List<int> validValues = valueGroups.Where(g => g.Value.Count >= 2)
                                              .Select(g => g.Key)
                                              .OrderBy(v => v)
                                              .ToList();
            
            // 寻找连续的对子序列
            for (int i = 0; i <= validValues.Count - pairCount; i++)
            {
                bool isStraightPair = true;
                for (int j = 0; j < pairCount - 1; j++)
                {
                    if (validValues[i + j + 1] - validValues[i + j] != 1)
                    {
                        isStraightPair = false;
                        break;
                    }
                }
                
                if (isStraightPair)
                {
                    // 找到了符合条件的连对，收集对应的牌
                    List<Card> straightPair = new List<Card>();
                    for (int j = 0; j < pairCount; j++)
                    {
                        int value = validValues[i + j];
                        // 取两张该值的牌
                        straightPair.AddRange(valueGroups[value].Take(2));
                    }
                    return straightPair;
                }
            }
            
            return null;
        }
        
        // 寻找比指定值大的最小飞机
        private List<Card> FindHigherTripleSequence(List<Card> sortedHand, int minValue, int length)
        {
            // 飞机长度需要是3的倍数，且至少为6（2个三带）
            if (length % 3 != 0 || length < 6)
                return null;
            
            int tripleCount = length / 3;
            
            // 统计每种牌值的数量
            Dictionary<int, List<Card>> valueGroups = new Dictionary<int, List<Card>>();
            foreach (var card in sortedHand)
            {
                int value = GetCardValue(card);
                if (value > minValue && value < 16) // 不考虑2和王
                {
                    if (!valueGroups.ContainsKey(value))
                        valueGroups[value] = new List<Card>();
                    valueGroups[value].Add(card);
                }
            }
            
            // 找出有至少三张牌的牌值，并按升序排列
            List<int> validValues = valueGroups.Where(g => g.Value.Count >= 3)
                                              .Select(g => g.Key)
                                              .OrderBy(v => v)
                                              .ToList();
            
            // 寻找连续的三带序列
            for (int i = 0; i <= validValues.Count - tripleCount; i++)
            {
                bool isTripleSequence = true;
                for (int j = 0; j < tripleCount - 1; j++)
                {
                    if (validValues[i + j + 1] - validValues[i + j] != 1)
                    {
                        isTripleSequence = false;
                        break;
                    }
                }
                
                if (isTripleSequence)
                {
                    // 找到了符合条件的飞机，收集对应的牌
                    List<Card> tripleSequence = new List<Card>();
                    for (int j = 0; j < tripleCount; j++)
                    {
                        int value = validValues[i + j];
                        // 取三张该值的牌
                        tripleSequence.AddRange(valueGroups[value].Take(3));
                    }
                    return tripleSequence;
                }
            }
            
            return null;
        }
        
        // 寻找比指定值大的最小三带一或三带对
        private List<Card> FindHigherTripleWithExtras(List<Card> sortedHand, int minValue, bool withSingle, int length)
        {
            // 三带一长度为4，三带对长度为5
            if (length != 4 && length != 5)
                return null;
            
            // 统计每种牌值的数量
            Dictionary<int, List<Card>> valueGroups = new Dictionary<int, List<Card>>();
            foreach (var card in sortedHand)
            {
                int value = GetCardValue(card);
                if (!valueGroups.ContainsKey(value))
                    valueGroups[value] = new List<Card>();
                valueGroups[value].Add(card);
            }
            
            // 找出有至少三张牌的牌值，并按升序排列
            List<int> tripleValues = valueGroups.Where(g => g.Value.Count >= 3 && GetCardValue(g.Value[0]) > minValue)
                                               .Select(g => g.Key)
                                               .OrderBy(v => v)
                                               .ToList();
            
            foreach (int tripleValue in tripleValues)
            {
                // 复制一份手牌，移除三带的牌
                List<Card> remainingCards = new List<Card>(sortedHand);
                remainingCards.RemoveAll(c => GetCardValue(c) == tripleValue);
                
                if (withSingle && remainingCards.Count >= 1)
                {
                    // 三带一，选择最小的单牌
                    Card smallestSingle = remainingCards.OrderBy(c => GetCardValue(c)).First();
                    List<Card> result = new List<Card>(valueGroups[tripleValue].Take(3));
                    result.Add(smallestSingle);
                    return result;
                }
                else if (!withSingle && remainingCards.Count >= 2)
                {
                    // 三带对，尝试找到对子
                    for (int i = 0; i < remainingCards.Count - 1; i++)
                    {
                        if (GetCardValue(remainingCards[i]) == GetCardValue(remainingCards[i + 1]))
                        {
                            List<Card> result = new List<Card>(valueGroups[tripleValue].Take(3));
                            result.AddRange(remainingCards.GetRange(i, 2));
                            return result;
                        }
                    }
                }
            }
            
            return null;
        }
        
        // 寻找比指定值大的最小四带
        private List<Card> FindHigherQuad(List<Card> sortedHand, int minValue)
        {
            for (int i = 0; i < sortedHand.Count - 3; i++)
            {
                if (sortedHand[i].rank == sortedHand[i + 1].rank && 
                    sortedHand[i + 1].rank == sortedHand[i + 2].rank && 
                    sortedHand[i + 2].rank == sortedHand[i + 3].rank && 
                    GetCardValue(sortedHand[i]) > minValue)
                {
                    return new List<Card> { sortedHand[i], sortedHand[i + 1], sortedHand[i + 2], sortedHand[i + 3] };
                }
            }
            return null;
        }
        
        // 寻找最长的顺子
        private List<Card> FindLongestStraight(List<Card> sortedHand)
        {
            // 获取去重后的值，按升序排列，排除2和王
            List<int> distinctValues = sortedHand.Select(card => GetCardValue(card))
                                                .Distinct()
                                                .Where(v => v < 16) // 不考虑2和王
                                                .OrderBy(v => v)
                                                .ToList();
            
            if (distinctValues.Count < 5) // 顺子至少需要5张牌
                return null;
            
            int maxLength = 0;
            int startIndex = -1;
            
            // 寻找最长的连续值序列
            for (int i = 0; i < distinctValues.Count - 1; i++)
            {
                int currentLength = 1;
                int currentStart = i;
                
                while (i < distinctValues.Count - 1 && distinctValues[i + 1] - distinctValues[i] == 1)
                {
                    currentLength++;
                    i++;
                }
                
                if (currentLength > maxLength && currentLength >= 5)
                {
                    maxLength = currentLength;
                    startIndex = currentStart;
                }
            }
            
            if (startIndex == -1)
                return null;
            
            // 收集对应的牌
            List<Card> longestStraight = new List<Card>();
            for (int i = startIndex; i < startIndex + maxLength; i++)
            {
                int value = distinctValues[i];
                // 取一张该值的牌
                longestStraight.Add(sortedHand.First(card => GetCardValue(card) == value));
            }
            
            return longestStraight;
        }
        
        // 寻找最长的连对
        private List<Card> FindLongestStraightPair(List<Card> sortedHand)
        {
            // 统计每种牌值的数量
            Dictionary<int, List<Card>> valueGroups = new Dictionary<int, List<Card>>();
            foreach (var card in sortedHand)
            {
                int value = GetCardValue(card);
                if (value < 16) // 不考虑2和王
                {
                    if (!valueGroups.ContainsKey(value))
                        valueGroups[value] = new List<Card>();
                    valueGroups[value].Add(card);
                }
            }
            
            // 找出有至少两张牌的牌值，并按升序排列
            List<int> pairValues = valueGroups.Where(g => g.Value.Count >= 2)
                                             .Select(g => g.Key)
                                             .OrderBy(v => v)
                                             .ToList();
            
            if (pairValues.Count < 3) // 连对至少需要3对
                return null;
            
            int maxLength = 0;
            int startIndex = -1;
            
            // 寻找最长的连续对子序列
            for (int i = 0; i < pairValues.Count - 1; i++)
            {
                int currentLength = 1;
                int currentStart = i;
                
                while (i < pairValues.Count - 1 && pairValues[i + 1] - pairValues[i] == 1)
                {
                    currentLength++;
                    i++;
                }
                
                if (currentLength > maxLength && currentLength >= 3)
                {
                    maxLength = currentLength;
                    startIndex = currentStart;
                }
            }
            
            if (startIndex == -1)
                return null;
            
            // 收集对应的牌
            List<Card> longestStraightPair = new List<Card>();
            for (int i = startIndex; i < startIndex + maxLength; i++)
            {
                int value = pairValues[i];
                // 取两张该值的牌
                longestStraightPair.AddRange(valueGroups[value].Take(2));
            }
            
            return longestStraightPair;
        }
        
        // 寻找最长的飞机
        private List<Card> FindLongestTripleSequence(List<Card> sortedHand)
        {
            // 统计每种牌值的数量
            Dictionary<int, List<Card>> valueGroups = new Dictionary<int, List<Card>>();
            foreach (var card in sortedHand)
            {
                int value = GetCardValue(card);
                if (value < 16) // 不考虑2和王
                {
                    if (!valueGroups.ContainsKey(value))
                        valueGroups[value] = new List<Card>();
                    valueGroups[value].Add(card);
                }
            }
            
            // 找出有至少三张牌的牌值，并按升序排列
            List<int> tripleValues = valueGroups.Where(g => g.Value.Count >= 3)
                                               .Select(g => g.Key)
                                               .OrderBy(v => v)
                                               .ToList();
            
            if (tripleValues.Count < 2) // 飞机至少需要2个三带
                return null;
            
            int maxLength = 0;
            int startIndex = -1;
            
            // 寻找最长的连续三带序列
            for (int i = 0; i < tripleValues.Count - 1; i++)
            {
                int currentLength = 1;
                int currentStart = i;
                
                while (i < tripleValues.Count - 1 && tripleValues[i + 1] - tripleValues[i] == 1)
                {
                    currentLength++;
                    i++;
                }
                
                if (currentLength > maxLength && currentLength >= 2)
                {
                    maxLength = currentLength;
                    startIndex = currentStart;
                }
            }
            
            if (startIndex == -1)
                return null;
            
            // 收集对应的牌
            List<Card> longestTripleSequence = new List<Card>();
            for (int i = startIndex; i < startIndex + maxLength; i++)
            {
                int value = tripleValues[i];
                // 取三张该值的牌
                longestTripleSequence.AddRange(valueGroups[value].Take(3));
            }
            
            return longestTripleSequence;
        }
        
        // 寻找最小的三带
        private List<Card> FindLowestTriple(List<Card> sortedHand)
        {
            for (int i = 0; i < sortedHand.Count - 2; i++)
            {
                if (sortedHand[i].rank == sortedHand[i + 1].rank && 
                    sortedHand[i + 1].rank == sortedHand[i + 2].rank)
                {
                    return new List<Card> { sortedHand[i], sortedHand[i + 1], sortedHand[i + 2] };
                }
            }
            return null;
        }
        
        // 寻找最小的对子
        private List<Card> FindLowestPair(List<Card> sortedHand)
        {
            for (int i = 0; i < sortedHand.Count - 1; i++)
            {
                if (sortedHand[i].rank == sortedHand[i + 1].rank)
                {
                    return new List<Card> { sortedHand[i], sortedHand[i + 1] };
                }
            }
            return null;
        }
        
        // 判断对手是否有强牌
        private bool IsStrongOpponentHand()
        {
            // 根据游戏状态、对手剩余牌数和出牌历史来判断对手是否有强牌
            
            // 1. 如果当前玩家是地主，且两个农民都只剩下很少的牌，他们可能有强牌
            int remainingPlayers = players.Count(p => p.Hand.Count > 0);
            if (currentPlayer.IsLandlord && remainingPlayers == 2)
            {
                int minHandCount = players.Where(p => p != currentPlayer && p.Hand.Count > 0)
                                         .Min(p => p.Hand.Count);
                if (minHandCount <= 3) // 农民只剩下很少的牌
                    return true;
            }
            
            // 2. 如果对手连续出了大牌或者炸弹，说明对手有强牌
            if (lastPlayedCardType == CardType.Bomb || 
                (lastPlayedCardType == CardType.Single && 
                 currentPlayedCards != null && currentPlayedCards.Count > 0 && 
                 GetCardValue(currentPlayedCards[0]) >= 14)) // 大牌 (A以上)
            {
                return true;
            }
            
            // 3. 根据游戏阶段判断
            // 游戏后期（大部分玩家已经出了很多牌），剩下的玩家可能有强牌
            int totalRemainingCards = players.Sum(p => p.Hand.Count);
            if (totalRemainingCards < 20) // 游戏后期
            {
                // 对手剩余牌少但一直没出大牌，可能在憋炸弹
                foreach (var player in players.Where(p => p != currentPlayer && p.Hand.Count > 0))
                {
                    if (player.Hand.Count <= 6 && HasHistoryOfHoldingBack(player))
                        return true;
                }
            }
            
            return false;
        }
        
        // 检查玩家是否有保留牌的历史（即经常Pass但突然出牌）
        private bool HasHistoryOfHoldingBack(Player player)
        {
            // 简单实现：检查玩家的Pass次数和出牌次数的比例
            // 实际实现中可以维护一个更详细的出牌历史记录
            return player.HasPassedCount > player.TimesPlayed;
        }
        
        // 寻找炸弹
        private List<Card> FindBomb(List<Card> sortedHand)
        {
            if (sortedHand == null || sortedHand.Count < 4)
                return null;
            
            // 检查四张相同点数的牌
            for (int i = 0; i <= sortedHand.Count - 4; i++)
            {
                if (sortedHand[i].rank == sortedHand[i + 1].rank && 
                    sortedHand[i + 1].rank == sortedHand[i + 2].rank && 
                    sortedHand[i + 2].rank == sortedHand[i + 3].rank)
                {
                    return new List<Card> { sortedHand[i], sortedHand[i + 1], sortedHand[i + 2], sortedHand[i + 3] };
                }
            }
            
            // 检查王炸
            bool hasJoker = sortedHand.Any(card => card.rank == Rank.Joker);
            bool hasBigJoker = sortedHand.Any(card => card.rank == Rank.BigJoker);
            if (hasJoker && hasBigJoker)
            {
                Card joker = sortedHand.Find(card => card.rank == Rank.Joker);
                Card bigJoker = sortedHand.Find(card => card.rank == Rank.BigJoker);
                return new List<Card> { joker, bigJoker };
            }
            
            return null;
        }
        
        // 切换到下一个玩家
        private void NextPlayer()
        {
            if (currentPlayer == null)
                return;
            
            int currentIndex = players.IndexOf(currentPlayer);
            int nextIndex = (currentIndex + 1) % players.Count;
            currentPlayer = players[nextIndex];
            
            // 自动让AI玩家进行操作
            if (currentPlayer.Name != "你")
            {
                Invoke("AIPlay", 1.5f);
            }
        }
        
        // 判定胜负
        private void DetermineWinner(Player winner)
        {
            // 增加底池（简化版）
            pot += 100; // 默认底池增加100
            
            // 分配奖金
            if (winner.IsLandlord)
            {
                // 地主获胜，获得所有底池
                winner.Chips += pot;
                string currencyType = UsePaperMoney ? "宝钞" : "金钞";
                gameOverMessage = winner.Name + " (地主) 获胜，赢得了 " + pot + " " + currencyType + "\n当前剩余：" + players[0].Chips + " " + currencyType;
            }
            else
            {
                // 农民获胜，平分底池
                int share = pot / 2; // 两个农民平分
                foreach (var player in players)
                {
                    if (!player.IsLandlord && player.IsPlaying)
                    {
                        player.Chips += share;
                    }
                }
                string currencyType = UsePaperMoney ? "宝钞" : "金钞";
                gameOverMessage = "农民获胜，赢得了 " + pot + " " + currencyType + "\n当前剩余：" + players[0].Chips + " " + currencyType;
            }
            
            Logger.LogInfo(gameOverMessage);
            
            // 设置游戏结束状态
            currentState = GameState.GameOver;
            
            // 更新玩家的剩余筹码
            players[0].Chips = Mathf.Max(0, players[0].Chips);
        }
        
        // 获取游戏状态文本
        private string GetGameStateText()
        {
            switch (currentState)
            {
                case GameState.NotStarted:
                    return "等待开始";
                case GameState.Bidding:
                    return "叫地主阶段";
                case GameState.Playing:
                    return "游戏进行中";
                case GameState.GameOver:
                    return "游戏结束";
                default:
                    return "未知状态";
            }
        }
        
        // 绘制游戏结束窗口
        private void DrawGameOverWindow(int windowID)
        {
            // 重置窗口样式
            ResetUIStyles();
            
            // 设置窗口背景
            GUI.backgroundColor = new Color(0.2f, 0.2f, 0.3f, 0.95f);
            
            // 允许拖动窗口
            GUI.DragWindow(new Rect(0, 0, gameOverWindowRect.width, 25));
            
            GUILayout.BeginVertical();
            GUILayout.Space(30f * scaleFactor);
            
            // 创建标题
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontSize = Mathf.RoundToInt(24 * scaleFactor);
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = new Color(1.0f, 0.84f, 0.0f, 1.0f); // 金色文字
            
            GUILayout.Label("游戏结束", titleStyle);
            GUILayout.Space(30f * scaleFactor);
            
            // 显示游戏结束信息
            GUIStyle messageStyle = new GUIStyle(GUI.skin.label);
            messageStyle.alignment = TextAnchor.MiddleCenter;
            messageStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
            messageStyle.normal.textColor = Color.white;
            messageStyle.wordWrap = true;
            messageStyle.padding = new RectOffset(20, 20, 10, 10);
            
            GUILayout.Label(gameOverMessage, messageStyle, GUILayout.Width(350f * scaleFactor));
            GUILayout.Space(40f * scaleFactor);
            
            // 重复游戏逻辑
            if (!hasCalledRepeatLogic)
            {
                hasCalledRepeatLogic = true;
                // 直接调用静态方法，传入必要参数
                B_RepeatTheGame.HandleGameEnd(
                    players[0].Chips, 
                    this, // 游戏实例
                    () => { 
                        // 继续游戏回调
                        currentState = GameState.NotStarted;
                        StartNewGame();
                        hasCalledRepeatLogic = false;
                    }, 
                    () => { 
                        // 返回主窗口回调
                        ReturnToMainMenu();
                    });
            }
            
            // 按钮区域
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 继续游戏按钮
            GUIStyle continueButtonStyle = CreateButtonStyle(
                new Color(0.2f, 0.6f, 0.2f, 0.9f), // 绿色按钮
                new Color(0.3f, 0.7f, 0.3f, 0.9f)
            );
            
            // 检查玩家是否还有筹码
            bool canContinue = players != null && players.Count > 0 && players[0].Chips > 0;
            
            if (canContinue)
            {
                if (GUILayout.Button("继续游戏", continueButtonStyle, 
                    GUILayout.Width(160f * scaleFactor), 
                    GUILayout.Height(50f * scaleFactor)))
                {
                    currentState = GameState.NotStarted;
                    StartNewGame();
                }
            }
            
            GUILayout.Space(20f * scaleFactor);
            
            // 返回主窗口按钮
            GUIStyle returnButtonStyle = CreateButtonStyle(
                new Color(0.6f, 0.4f, 0.2f, 0.9f), // 棕色按钮
                new Color(0.7f, 0.5f, 0.3f, 0.9f)
            );
            
            if (GUILayout.Button("返回主窗口", returnButtonStyle, 
                GUILayout.Width(160f * scaleFactor), 
                GUILayout.Height(50f * scaleFactor)))
            {
                ReturnToMainMenu();
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            // 如果筹码用完，显示提示
            if (!canContinue)
            {
                GUIStyle hintStyle = new GUIStyle(GUI.skin.label);
                hintStyle.alignment = TextAnchor.MiddleCenter;
                hintStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
                hintStyle.normal.textColor = new Color(1.0f, 0.5f, 0.5f, 1.0f); // 红色文字
                
                GUILayout.Space(20f * scaleFactor);
                GUILayout.Label("你的筹码已用完，返回主窗口获取更多筹码", hintStyle);
            }
            
            GUILayout.Space(30f * scaleFactor);
            GUILayout.EndVertical();
        }
        
        // 返回主窗口并保存结余货币
        private void ReturnToMainMenu()
        {
            // 保存结余货币
            RemainingMoney = players[0].Chips;
            
            // 关闭斗地主窗口
            showMenu = false;
            blockGameInput = false;
            
            Logger.LogInfo("已返回主窗口，结余" + (UsePaperMoney ? "宝钞" : "金钞") + "：" + RemainingMoney);
            
            // 尝试打开MoreGambles主窗口
            A_MoreGambles moreGambles = FindObjectOfType<A_MoreGambles>();
            if (moreGambles != null)
            {
                moreGambles.ReturnFromGame(UsePaperMoney, RemainingMoney);
            }
        }
        
        // 创建简单的纹理
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        // 更新分辨率设置
        private void UpdateResolutionSettings()
        {
            // 基于屏幕分辨率调整缩放因子
            float screenWidth = Screen.width;
            if (screenWidth < 1024)
            {
                scaleFactor = 0.8f;
            }
            else if (screenWidth < 1440)
            {
                scaleFactor = 1.0f;
            }
            else
            {
                scaleFactor = 1.2f;
            }
        }
    }
}
