using System;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace cs.HoLMod.MoreGambles
{
    internal class FriedGoldenFlower : MonoBehaviour
    {
        // 窗口设置 - 增大尺寸以提供更好的视觉体验
        private Rect windowRect = new Rect(20, 20, 1000, 750);
        private Rect smallWindowRect = new Rect(20, 20, 800, 500);
        private bool showMenu = false;
        private bool blockGameInput = false;
        private float scaleFactor = 1f;

        // 动画和视觉效果相关
        private float aiActionTimer = 0f;
        private string aiActionText = "";
        private bool isAIActionAnimating = false;
        private float cardHoverScale = 1.05f;
        private float cardHoverOffset = 5f;

        // 货币系统相关
        public bool UsePaperMoney { get; set; }
        public int InitialChips { get; set; }

        // 游戏相关
        private int playerChips = 0;
        private int[] aiChips = new int[5];
        private List<AIPlayer> AIPlayers = new List<AIPlayer>();

        // 卡牌系统
        private List<Card> deck = new List<Card>();
        private List<Card> playerHand = new List<Card>();
        private List<List<Card>> aiHands = new List<List<Card>>();
        private List<bool> aiHasFolded = new List<bool>();
        private List<bool> aiIsPlaying = new List<bool>();

        // 游戏状态
        private enum GameState { Waiting, Dealing, Betting, Comparing, Ended }
        private GameState currentState = GameState.Waiting;
        private int currentPlayer = -1; // 0-5, 0为玩家
        private int currentBet = 0;
        private int pot = 0;
        private int minBet = 10;
        private int round = 0;
        
        // 重复游戏逻辑相关
        private bool hasCalledRepeatLogic = false;

        // 随机数生成器
        private System.Random random = new System.Random();

        // 卡牌定义
        public enum Suit { Spades, Hearts, Clubs, Diamonds }
        public enum Rank { Three = 3, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace, Two }
        public struct Card
        {
            public Suit Suit;
            public Rank Rank;
            public bool IsVisible;

            public Card(Suit suit, Rank rank, bool isVisible = true)
            {
                Suit = suit;
                Rank = rank;
                IsVisible = isVisible;
            }

            public override string ToString()
            {
                string suitStr = Suit == Suit.Spades ? "♠" : Suit == Suit.Hearts ? "♥" : Suit == Suit.Clubs ? "♣" : "♦";
                string rankStr;
                // 确保所有卡牌等级都显示为阿拉伯数字
                switch (Rank)
                {
                    case Rank.Ace: rankStr = "1";
                        break;
                    case Rank.Jack: rankStr = "11";
                        break;
                    case Rank.Queen: rankStr = "12";
                        break;
                    case Rank.King: rankStr = "13";
                        break;
                    default: rankStr = ((int)Rank).ToString();
                        break;
                }
                return $"{suitStr}{rankStr}";
            }
        }

        // AI玩家类
        private class AIPlayer
        {
            public string Name { get; set; }
            public int Position { get; set; }
            public int Chips { get; set; }
            public List<Card> Hand { get; set; }
            public bool IsFolded { get; set; }
            public bool IsPlaying { get; set; }
            public int LastActionBet { get; set; }

            public AIPlayer(string name, int position, int chips)
            {
                Name = name;
                Position = position;
                Chips = chips;
                Hand = new List<Card>();
                IsFolded = false;
                IsPlaying = true;
                LastActionBet = 0;
            }
        }

        // 初始化
        private void Awake()
        {
            // 初始化AI玩家
            InitializeAIPlayers();
            UpdateResolutionSettings();
        }

        // 初始化AI玩家
        private void InitializeAIPlayers()
        {
            AIPlayers.Clear();
            aiChips = new int[5];
            aiHands = new List<List<Card>>();
            aiHasFolded = new List<bool>();
            aiIsPlaying = new List<bool>();

            // 使用B_AI_Name类生成5个不重复的随机AI姓名
            List<string> selectedNames = B_AI_Name.GenerateRandomNames(5);
            
            // 初始化AI玩家，筹码为玩家带入筹码的0.5到3倍随机
            for (int i = 0; i < 5; i++)
            {
                // 计算随机筹码值 (0.5到3倍的玩家初始筹码)
                double randomMultiplier = 0.5 + random.NextDouble() * 2.5; // 0.5到3之间
                aiChips[i] = (int)(InitialChips * randomMultiplier);
                
                AIPlayers.Add(new AIPlayer(selectedNames[i], i + 1, aiChips[i]));
                aiHands.Add(new List<Card>());
                aiHasFolded.Add(false);
                aiIsPlaying.Add(true);
            }
        }

        // 更新分辨率设置
        private void UpdateResolutionSettings()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // 缩放因子基于屏幕高度
            scaleFactor = screenHeight / 1080f;
            if (scaleFactor < 0.5f) scaleFactor = 0.5f;
            if (scaleFactor > 2f) scaleFactor = 2f;

            // 根据缩放因子调整窗口尺寸
            windowRect.width = 800 * scaleFactor;
            windowRect.height = 600 * scaleFactor;
            smallWindowRect.width = 600 * scaleFactor;
            smallWindowRect.height = 400 * scaleFactor;

            // 窗口居中
            windowRect.x = (screenWidth - windowRect.width) / 2;
            windowRect.y = (screenHeight - windowRect.height) / 2;
            smallWindowRect.x = (screenWidth - smallWindowRect.width) / 2;
            smallWindowRect.y = (screenHeight - smallWindowRect.height) / 2;
        }

        // 更新逻辑 - 添加AI动画处理
        private void Update()
        {
            if (!showMenu) return;

            // 阻止游戏输入
            if (blockGameInput && Input.anyKeyDown)
            {
                // 阻止事件传递给游戏
                EventSystem.current.SetSelectedGameObject(null);
            }

            // 处理AI动画计时器
            if (isAIActionAnimating)
            {
                aiActionTimer -= Time.deltaTime;
                if (aiActionTimer <= 0f)
                {
                    // AI动画结束，执行实际AI动作
                    if (currentState == GameState.Betting && currentPlayer > 0 && currentPlayer <= 5 && !AIPlayers[currentPlayer - 1].IsFolded)
                    {
                        int aiIndex = currentPlayer - 1;
                        AIPlayer aiPlayer = AIPlayers[aiIndex];
                        
                        // 评估手牌强度
                        int handStrength = EvaluateHandStrength(aiPlayer.Hand);
                        int potOdds = pot > 0 ? (currentBet * 100) / pot : 0; // 底池赔率
                        int actionProbability = 0;

                        // 根据手牌强度、当前回合和底池赔率决定行动概率
                        if (handStrength >= 500) // 强牌（豹子、顺金）
                        {
                            actionProbability = 90; // 90%概率加注
                        }
                        else if (handStrength >= 300) // 中等偏强牌（金花、顺子）
                        {
                            actionProbability = 70 + (potOdds > 30 ? 20 : 0); // 70-90%概率加注
                        }
                        else if (handStrength >= 200) // 中等牌（对子）
                        {
                            actionProbability = 40 + (potOdds > 20 ? 30 : 0); // 40-70%概率加注
                        }
                        else if (handStrength >= 100) // 弱牌（高牌）
                        {
                            actionProbability = 10 + (potOdds > 10 ? 20 : 0); // 10-30%概率加注
                        }
                        else // 很弱的牌
                        {
                            actionProbability = 5; // 5%概率加注
                        }

                        // 根据概率决定行动
                        int randomAction = random.Next(1, 101);
                        if (randomAction <= actionProbability)
                        {
                            // 加注
                            aiActionText = AIPlayers[aiIndex].Name + " 选择了加注!";
                            Invoke("PerformAIRaise", 0.5f);
                        }
                        else if (randomAction <= actionProbability + 50)
                        {
                            // 跟注
                            aiActionText = AIPlayers[aiIndex].Name + " 选择了跟注!";
                            Invoke("PerformAICall", 0.5f);
                        }
                        else
                        {
                            // 弃牌
                            aiActionText = AIPlayers[aiIndex].Name + " 选择了弃牌!";
                            Invoke("PerformAIFold", 0.5f);
                        }
                    }
                }
            }
        }
        
        // 执行AI加注
        private void PerformAIRaise()
        {
            int raiseAmount = Math.Max(minBet, (int)(minBet * (0.8f + random.NextDouble() * 0.4f))); // 最小加注的80-120%
            Raise(raiseAmount);
            isAIActionAnimating = false;
        }
        
        // 执行AI跟注
        private void PerformAICall()
        {
            Call();
            isAIActionAnimating = false;
        }
        
        // 执行AI弃牌
        private void PerformAIFold()
        {
            Fold();
            isAIActionAnimating = false;
        }

        // 绘制GUI
        private void OnGUI()
        {
            if (!showMenu) return;

            // 使用本地缩放因子
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleFactor, scaleFactor, 1f));

            // 绘制游戏窗口
            windowRect = GUI.Window(1002, windowRect, DrawWindow, "炸金花游戏");

            // 恢复GUI矩阵
            GUI.matrix = Matrix4x4.identity;
        }

        // 绘制游戏窗口
        private void DrawWindow(int windowID)
        {
            // 允许拖动窗口
            GUI.DragWindow(new Rect(0, 0, windowRect.width / scaleFactor, 30));

            // 绘制深色调背景 - 提升视觉体验
            GUI.DrawTexture(new Rect(0, 0, windowRect.width / scaleFactor, windowRect.height / scaleFactor), MakeTexture((int)(windowRect.width / scaleFactor), (int)(windowRect.height / scaleFactor), new Color(0.08f, 0.15f, 0.08f)));

            // 绘制醒目的顶部标题栏
            GUI.DrawTexture(new Rect(0, 0, windowRect.width / scaleFactor, 30), MakeTexture((int)(windowRect.width / scaleFactor), 30, new Color(0.2f, 0.4f, 0.2f)));

            GUILayout.BeginVertical();
            GUILayout.Space(40);

            // 游戏标题 - 醒目的金色标题
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 32;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = new Color(1f, 0.9f, 0.4f);
            GUILayout.Label("炸金花", titleStyle);
            GUILayout.Space(20);

            // 游戏状态和信息面板 - 改进视觉效果
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(GUILayout.Width(600));

            // 状态和底池信息
            GUIStyle panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = MakeTexture(1, 1, new Color(0.25f, 0.25f, 0.25f, 0.9f));
            panelStyle.border = new RectOffset(4, 4, 4, 4);
            panelStyle.padding = new RectOffset(10, 10, 10, 10);
            GUILayout.BeginVertical(panelStyle, GUILayout.Height(90));

            // 游戏状态和回合信息
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle stateStyle = new GUIStyle(GUI.skin.label);
            stateStyle.fontSize = 18;
            stateStyle.fontStyle = FontStyle.Bold;
            stateStyle.alignment = TextAnchor.MiddleCenter;
            stateStyle.normal.textColor = GetStateColor(currentState);
            GUILayout.Label(GetStateText() + (round > 0 ? " (回合 " + round + ")" : ""), stateStyle, GUILayout.Width(220));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // 底池显示 - 醒目的金色显示
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle potStyle = new GUIStyle(GUI.skin.label);
            potStyle.fontSize = 24;
            potStyle.fontStyle = FontStyle.Bold;
            potStyle.alignment = TextAnchor.MiddleCenter;
            GUI.contentColor = new Color(1f, 0.85f, 0.3f);
            GUILayout.Label("底池: " + pot + (UsePaperMoney ? " 宝钞" : " 金钞"), potStyle);
            GUI.contentColor = Color.white;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // AI行动动态提示
            if (isAIActionAnimating && currentPlayer > 0 && currentPlayer <= 5 && !AIPlayers[currentPlayer - 1].IsFolded)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUIStyle aiActionStyle = new GUIStyle(GUI.skin.label);
                aiActionStyle.fontSize = 16;
                aiActionStyle.alignment = TextAnchor.MiddleCenter;
                aiActionStyle.normal.textColor = Color.cyan;
                GUILayout.Label(aiActionText, aiActionStyle, GUILayout.Width(300));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            // 玩家布局 - 改进的环绕式设计，增加间距
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 左侧AI玩家
            GUILayout.BeginVertical();
            DrawPlayerArea(3, true); // AI玩家3
            GUILayout.EndVertical();
            
            GUILayout.Space(30);
            
            // 中间垂直布局
            GUILayout.BeginVertical();
            // 上方AI玩家 - 增大间距
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawPlayerArea(1, true); // AI玩家1
            GUILayout.Space(60);
            DrawPlayerArea(2, true); // AI玩家2
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(30);
            
            // 玩家区域 - 突出显示
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawPlayerArea(0, false); // 玩家
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(30);
            
            // 操作区域 - 改进的操作区布局
            GUILayout.BeginVertical();
            // 当前回合和最小下注信息
            if (currentState == GameState.Betting)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                string playerName = currentPlayer == 0 ? "你的回合" : AIPlayers[currentPlayer - 1].Name + "的回合";
                GUIStyle turnStyle = new GUIStyle(GUI.skin.label);
                turnStyle.fontSize = 18;
                turnStyle.fontStyle = FontStyle.Bold;
                turnStyle.alignment = TextAnchor.MiddleCenter;
                turnStyle.normal.textColor = Color.yellow;
                GUILayout.Label(playerName, turnStyle, GUILayout.Width(250));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                GUILayout.Space(15);
                
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUIStyle minBetStyle = new GUIStyle(GUI.skin.label);
                minBetStyle.fontSize = 16;
                minBetStyle.alignment = TextAnchor.MiddleCenter;
                minBetStyle.normal.textColor = new Color(0.8f, 0.8f, 1f);
                GUILayout.Label("最小下注: " + minBet + (UsePaperMoney ? " 宝钞" : " 金钞"), minBetStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            
            // 操作按钮
            GUILayout.Space(25);
            DrawActionButtons();
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            
            GUILayout.Space(30);
            
            // 右侧AI玩家
            GUILayout.BeginVertical();
            DrawPlayerArea(4, true); // AI玩家4
            GUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            // 底部AI玩家
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawPlayerArea(5, true); // AI玩家5
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        // 获取游戏状态对应的颜色 - 更丰富的颜色编码
        private Color GetStateColor(GameState state)
        {
            switch (state)
            {
                case GameState.Waiting:
                    return Color.yellow; // 黄色表示准备开始
                case GameState.Dealing:
                    return new Color(0.7f, 0.7f, 1f); // 浅蓝色表示发牌中
                case GameState.Betting:
                    return Color.green; // 绿色表示下注阶段
                case GameState.Comparing:
                    return new Color(1f, 0.7f, 0f); // 橙色表示比牌阶段
                case GameState.Ended:
                    return Color.red; // 红色表示游戏结束
                default:
                    return Color.white;
            }
        }

        // 绘制操作按钮 - 改进按钮样式和交互体验
        private void DrawActionButtons()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (currentState == GameState.Betting && currentPlayer == 0 && !aiHasFolded[0])
            {
                // 玩家操作按钮 - 增大按钮尺寸和间距
                DrawButton("弃牌", 140, 50, Color.red, () => Fold());
                GUILayout.Space(30);
                DrawButton("跟注", 140, 50, Color.green, () => Call());
                GUILayout.Space(30);
                DrawButton("加注", 140, 50, new Color(0.6f, 0.3f, 0.9f), () => Raise(minBet * 2));
            }
            else if (currentState == GameState.Waiting)
            {
                // 开始游戏按钮 - 醒目设计
                DrawButton("开始游戏", 180, 55, new Color(0.2f, 0.8f, 0.2f), () => StartGame());
            }
            else if (currentState == GameState.Ended)
            {
                // 重复游戏逻辑
                if (!hasCalledRepeatLogic)
                {
                    hasCalledRepeatLogic = true;
                    
                    // 直接调用静态方法，传入必要参数
                    B_RepeatTheGame.HandleGameEnd(
                        playerChips, 
                        this, // 游戏实例
                        () => { 
                            // 继续游戏回调
                            ResetGame();
                            StartGame();
                            hasCalledRepeatLogic = false;
                        }, 
                        () => { 
                            // 返回主窗口回调
                            ReturnToMainMenu();
                        });
                }
                
                // 游戏结束按钮 - 金色标题
                GUIStyle endTitleStyle = new GUIStyle(GUI.skin.label);
                endTitleStyle.fontSize = 24;
                endTitleStyle.fontStyle = FontStyle.Bold;
                endTitleStyle.alignment = TextAnchor.MiddleCenter;
                endTitleStyle.normal.textColor = new Color(1f, 0.9f, 0.4f);
                GUILayout.Label("游戏结束", endTitleStyle);
                GUILayout.Space(20);
                
                // 结果显示
                int activePlayers = 1 + AIPlayers.Count(ai => !ai.IsFolded);
                string resultText = activePlayers <= 1 ? "恭喜你赢了这一局！" : "游戏结束！";
                GUIStyle resultStyle = new GUIStyle(GUI.skin.label);
                resultStyle.fontSize = 18;
                resultStyle.alignment = TextAnchor.MiddleCenter;
                resultStyle.normal.textColor = Color.white;
                GUILayout.Label(resultText, resultStyle);
                GUILayout.Space(20);
                
                // 结束按钮
                DrawButton("继续游戏", 160, 45, new Color(0.2f, 0.8f, 0.2f), () => { ResetGame(); StartGame(); });
                GUILayout.Space(30);
                DrawButton("返回主窗口", 160, 45, Color.red, () => ReturnToMainMenu());
            }
            else if (currentState == GameState.Dealing || currentState == GameState.Comparing)
            {
                // 显示状态信息 - 动态效果
                GUIStyle infoStyle = new GUIStyle(GUI.skin.label);
                infoStyle.fontSize = 18;
                infoStyle.alignment = TextAnchor.MiddleCenter;
                infoStyle.normal.textColor = Color.yellow;
                GUILayout.Label(GetStateText() + "中...", infoStyle);
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            // 关闭游戏按钮始终显示在底部 - 统一风格
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawButton("关闭游戏", 140, 40, new Color(0.8f, 0.2f, 0.2f), () => ReturnToMainMenu());
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        // 绘制美化的按钮 - 增强视觉效果和交互反馈
        private void DrawButton(string text, int width, int height, Color color, System.Action onClick)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.background = MakeTexture(1, 1, color);
            buttonStyle.hover.background = MakeTexture(1, 1, Color.Lerp(color, Color.white, 0.3f)); // 更强的悬停效果
            buttonStyle.active.background = MakeTexture(1, 1, Color.Lerp(color, Color.black, 0.4f)); // 更强的按下效果
            buttonStyle.fontSize = 16;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.hover.textColor = Color.white;
            buttonStyle.active.textColor = Color.white;
            buttonStyle.border = new RectOffset(4, 4, 4, 4);
            buttonStyle.padding = new RectOffset(10, 10, 10, 10);
            
            if (GUILayout.Button(text, buttonStyle, GUILayout.Width(width), GUILayout.Height(height)))
            {
                // 添加按钮点击音效（如果游戏支持）
                onClick();
            }
        }

        // 绘制玩家区域 - 增强视觉层次和当前玩家高亮效果
        private void DrawPlayerArea(int playerIndex, bool isAI)
        {
            int width = 170;
            int height = 200;
            string playerName = isAI ? AIPlayers[playerIndex - 1].Name : "你";
            int chips = isAI ? AIPlayers[playerIndex - 1].Chips : playerChips;
            bool isFolded = isAI ? AIPlayers[playerIndex - 1].IsFolded : false;
            bool isCurrentPlayer = currentState == GameState.Betting && currentPlayer == playerIndex;
            List<Card> hand = isAI ? AIPlayers[playerIndex - 1].Hand : playerHand;
            bool showCards = !isAI || (currentState == GameState.Comparing || currentState == GameState.Ended);

            // 玩家区域样式 - 增强视觉效果和当前玩家高亮
            GUIStyle playerBoxStyle = new GUIStyle(GUI.skin.box);
            playerBoxStyle.normal.background = MakeTexture(1, 1, isCurrentPlayer ? 
                new Color(0.3f, 0.6f, 0.3f, 0.95f) : 
                (isFolded ? new Color(0.2f, 0.2f, 0.2f, 0.7f) : new Color(0.25f, 0.25f, 0.25f, 0.9f)));
            playerBoxStyle.border = new RectOffset(5, 5, 5, 5);
            playerBoxStyle.padding = new RectOffset(10, 10, 10, 10);
            
            GUILayout.BeginVertical(playerBoxStyle, GUILayout.Width(width), GUILayout.Height(height));
            GUILayout.Space(10);

            // 玩家名称 - 增强当前玩家的显示效果
            GUIStyle nameStyle = new GUIStyle(GUI.skin.label);
            nameStyle.fontSize = 16;
            nameStyle.fontStyle = FontStyle.Bold;
            nameStyle.alignment = TextAnchor.MiddleCenter;
            nameStyle.normal.textColor = isFolded ? Color.gray : (isCurrentPlayer ? new Color(1f, 0.9f, 0.4f) : Color.white);
            
            if (isFolded)
            {
                GUILayout.Label(playerName + " (已弃牌)", nameStyle);
            }
            else
            {
                GUILayout.Label(playerName, nameStyle);
            }

            // 筹码显示 - 金色高亮显示
            GUILayout.Space(10);
            GUIStyle chipsStyle = new GUIStyle(GUI.skin.label);
            chipsStyle.fontSize = 14;
            chipsStyle.alignment = TextAnchor.MiddleCenter;
            chipsStyle.normal.textColor = new Color(1f, 0.85f, 0.3f);
            GUILayout.Label("筹码: " + chips + (UsePaperMoney ? " 宝钞" : " 金钞"), chipsStyle);

            // 手牌
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // 绘制手牌
            for (int i = 0; i < hand.Count; i++)
            {
                DrawCard(hand[i], showCards, i);
                if (i < hand.Count - 1) GUILayout.Space(8);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        // 绘制美化的卡牌 - 增强视觉效果和花色差异化
        private void DrawCard(Card card, bool showCard, int cardIndex)
        {
            int width = 55;
            int height = 75;
            string cardText = showCard ? card.ToString() : "";
            
            GUIStyle cardStyle = new GUIStyle(GUI.skin.box);
            
            if (!showCard)
            {
                // 背面卡牌样式 - 精致的红色背面
                cardStyle.normal.background = MakeTexture(width, height, new Color(0.7f, 0.15f, 0.15f));
                cardStyle.border = new RectOffset(4, 4, 4, 4);
                cardStyle.fontSize = 16;
                cardStyle.fontStyle = FontStyle.Bold;
                cardStyle.alignment = TextAnchor.MiddleCenter;
                cardStyle.normal.textColor = Color.white;
                
                // 添加卡牌背面图案
                GUILayout.Box("??", cardStyle, GUILayout.Width(width), GUILayout.Height(height));
            }
            else
            {
                // 正面卡牌样式 - 根据花色设置差异化的背景色
                Color backgroundColor;
                switch (card.Suit)
                {
                    case Suit.Hearts:
                        backgroundColor = new Color(0.98f, 0.95f, 0.95f);
                        break;
                    case Suit.Diamonds:
                        backgroundColor = new Color(0.97f, 0.97f, 0.95f);
                        break;
                    case Suit.Clubs:
                        backgroundColor = new Color(0.95f, 0.97f, 0.95f);
                        break;
                    case Suit.Spades:
                        backgroundColor = new Color(0.95f, 0.95f, 0.97f);
                        break;
                    default:
                        backgroundColor = new Color(0.95f, 0.95f, 0.95f);
                        break;
                }
                
                cardStyle.normal.background = MakeTexture(width, height, backgroundColor);
                cardStyle.border = new RectOffset(4, 4, 4, 4);
                
                // 根据花色设置文字颜色
                cardStyle.normal.textColor = card.Suit == Suit.Hearts || card.Suit == Suit.Diamonds ? 
                    Color.red : Color.black;
                cardStyle.fontSize = 22;
                cardStyle.fontStyle = FontStyle.Bold;
                cardStyle.alignment = TextAnchor.MiddleCenter;
                
                GUILayout.Box(cardText, cardStyle, GUILayout.Width(width), GUILayout.Height(height));
            }
        }

        // 创建简单纹理
        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        // 获取游戏状态文本
        private string GetStateText()
        {
            switch (currentState)
            {
                case GameState.Waiting:
                    return "等待开始";
                case GameState.Dealing:
                    return "发牌中";
                case GameState.Betting:
                    return "下注阶段";
                case GameState.Comparing:
                    return "比牌阶段";
                case GameState.Ended:
                    return "游戏结束";
                default:
                    return "未知状态";
            }
        }

        // 开始游戏
        private void StartGame()
        {
            // 初始化牌组
            InitializeDeck();

            // 洗牌
            ShuffleDeck();

            // 发牌
            DealCards();

            // 重置游戏状态
            currentState = GameState.Betting;
            currentPlayer = 0; // 玩家先开始
            currentBet = 0;
            pot = 0;
            round = 1;

            // 强制最小下注
            minBet = Math.Max(10, InitialChips / 10);
        }

        // 初始化牌组
        private void InitializeDeck()
        {
            deck.Clear();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    deck.Add(new Card(suit, rank));
                }
            }
        }

        // 洗牌
        private void ShuffleDeck()
        {
            for (int i = 0; i < deck.Count; i++)
            {
                int j = random.Next(i, deck.Count);
                Card temp = deck[i];
                deck[i] = deck[j];
                deck[j] = temp;
            }
        }

        // 发牌
        private void DealCards()
        {
            // 清空所有玩家的手牌
            playerHand.Clear();
            foreach (AIPlayer ai in AIPlayers)
            {
                ai.Hand.Clear();
                ai.IsFolded = false;
                ai.IsPlaying = true;
            }

            // 每个玩家发3张牌
            for (int i = 0; i < 3; i++)
            {
                // 给玩家发牌
                playerHand.Add(deck[0]);
                deck.RemoveAt(0);

                // 给AI玩家发牌
                foreach (AIPlayer ai in AIPlayers)
                {
                    ai.Hand.Add(deck[0]);
                    deck.RemoveAt(0);
                }
            }
        }

        // 弃牌
        private void Fold()
        {
            if (currentPlayer == 0)
            {
                // 玩家弃牌
                Debug.Log("你选择了弃牌");
                // 直接进入比牌阶段
                currentState = GameState.Comparing;
                CompareHands();
            }
            else if (currentPlayer > 0 && currentPlayer <= 5)
            {
                // AI弃牌
                int aiIndex = currentPlayer - 1;
                AIPlayers[aiIndex].IsFolded = true;
                Debug.Log(AIPlayers[aiIndex].Name + " 选择了弃牌");
                // 进入下一个玩家回合
                NextPlayer();
            }
        }

        // 跟注
        private void Call()
        {
            int requiredChips = currentBet;
            if (currentPlayer == 0)
            {
                // 玩家跟注
                if (playerChips >= requiredChips)
                {
                    playerChips -= requiredChips;
                    pot += requiredChips;
                    Debug.Log("你选择了跟注: " + requiredChips);
                    NextPlayer();
                }
                else
                {
                    // 筹码不足，只能全下
                    pot += playerChips;
                    playerChips = 0;
                    Debug.Log("你筹码不足，只能全下: " + playerChips);
                    NextPlayer();
                }
            }
            else if (currentPlayer > 0 && currentPlayer <= 5)
            {
                // AI跟注
                int aiIndex = currentPlayer - 1;
                if (AIPlayers[aiIndex].Chips >= requiredChips)
                {
                    AIPlayers[aiIndex].Chips -= requiredChips;
                    pot += requiredChips;
                    Debug.Log(AIPlayers[aiIndex].Name + " 选择了跟注: " + requiredChips);
                    NextPlayer();
                }
                else
                {
                    // 筹码不足，只能全下
                    pot += AIPlayers[aiIndex].Chips;
                    AIPlayers[aiIndex].Chips = 0;
                    Debug.Log(AIPlayers[aiIndex].Name + " 筹码不足，只能全下: " + AIPlayers[aiIndex].Chips);
                    NextPlayer();
                }
            }
        }

        // 加注
        private void Raise(int amount)
        {
            int raiseAmount = Math.Max(amount, minBet);
            if (currentPlayer == 0)
            {
                // 玩家加注
                if (playerChips >= raiseAmount)
                {
                    playerChips -= raiseAmount;
                    pot += raiseAmount;
                    currentBet = raiseAmount;
                    minBet = raiseAmount * 2;
                    Debug.Log("你选择了加注: " + raiseAmount);
                    NextPlayer();
                }
                else
                {
                    // 筹码不足，只能全下
                    pot += playerChips;
                    playerChips = 0;
                    currentBet = Math.Max(currentBet, playerChips);
                    Debug.Log("你筹码不足，只能全下: " + playerChips);
                    NextPlayer();
                }
            }
            else if (currentPlayer > 0 && currentPlayer <= 5)
            {
                // AI加注
                int aiIndex = currentPlayer - 1;
                if (AIPlayers[aiIndex].Chips >= raiseAmount)
                {
                    AIPlayers[aiIndex].Chips -= raiseAmount;
                    pot += raiseAmount;
                    currentBet = raiseAmount;
                    minBet = raiseAmount * 2;
                    Debug.Log(AIPlayers[aiIndex].Name + " 选择了加注: " + raiseAmount);
                    NextPlayer();
                }
                else
                {
                    // 筹码不足，只能全下
                    pot += AIPlayers[aiIndex].Chips;
                    AIPlayers[aiIndex].Chips = 0;
                    currentBet = Math.Max(currentBet, AIPlayers[aiIndex].Chips);
                    Debug.Log(AIPlayers[aiIndex].Name + " 筹码不足，只能全下: " + AIPlayers[aiIndex].Chips);
                    NextPlayer();
                }
            }
        }

        // AI行动 - 添加动态等待提示
        private void AIAction()
        {
            if (currentPlayer <= 0 || currentPlayer > 5 || currentState != GameState.Betting)
                return;

            int aiIndex = currentPlayer - 1;
            AIPlayer aiPlayer = AIPlayers[aiIndex];
            
            if (aiPlayer.IsFolded)
            {
                NextPlayer();
                return;
            }
            
            // 开始AI行动动画
            isAIActionAnimating = true;
            aiActionText = AIPlayers[aiIndex].Name + " 正在思考...";
            aiActionTimer = 1f;

            // 评估手牌强度
            int handStrength = EvaluateHandStrength(aiPlayer.Hand);
            int potOdds = pot > 0 ? (currentBet * 100) / pot : 0; // 底池赔率
            int actionProbability = 0;

            // 根据手牌强度、当前回合和底池赔率决定行动概率
            switch (handStrength)
            {
                case 6: // 豹子 - 最高牌型
                    actionProbability = 95; // 95%概率继续游戏
                    break;
                case 5: // 顺金（同花顺）
                    actionProbability = 90;
                    break;
                case 4: // 金花
                    // 根据回合调整概率：第一轮更激进，后面更谨慎
                    actionProbability = round == 0 ? 80 : (round == 1 ? 70 : 60);
                    break;
                case 3: // 顺子
                    actionProbability = round == 0 ? 70 : (round == 1 ? 60 : 50);
                    break;
                case 2: // 对子
                    actionProbability = round == 0 ? 60 : (round == 1 ? 50 : 40);
                    break;
                case 1: // 单牌
                    // 单牌时考虑底池赔率和手牌大小
                    int highCard = GetHighCard(aiPlayer.Hand);
                    actionProbability = highCard >= (int)Rank.Jack ? (round == 0 ? 40 : 20) : 10;
                    break;
            }

            // 根据底池赔率调整行动概率
            if (potOdds > 30) // 底池赔率较高时，增加继续游戏的概率
                actionProbability = Math.Min(100, actionProbability + 15);
            else if (potOdds < 10) // 底池赔率较低时，减少继续游戏的概率
                actionProbability = Math.Max(0, actionProbability - 15);

            // 随机决定行动
            int randomAction = random.Next(100);
            if (randomAction < actionProbability)
            {
                // 根据手牌强度和回合决定是跟注还是加注
                int raiseProbability = 0;
                switch (handStrength)
                {
                    case 6: // 豹子
                        raiseProbability = 70; // 70%概率加注
                        break;
                    case 5: // 顺金
                        raiseProbability = 60;
                        break;
                    case 4: // 金花
                        raiseProbability = round == 0 ? 50 : 30;
                        break;
                    case 3: // 顺子
                        raiseProbability = round == 0 ? 40 : 20;
                        break;
                    case 2: // 对子
                        raiseProbability = round == 0 ? 30 : 10;
                        break;
                    default: // 单牌
                        raiseProbability = 10;
                        break;
                }

                if (random.Next(100) < raiseProbability)
                {
                    // 决定加注金额（手牌越强，加注越多）
                    int raiseAmount = minBet;
                    switch (handStrength)
                    {
                        case 6: // 豹子
                            raiseAmount = Math.Min(aiPlayer.Chips, minBet * 3);
                            break;
                        case 5: // 顺金
                            raiseAmount = Math.Min(aiPlayer.Chips, minBet * 2);
                            break;
                        default:
                            raiseAmount = Math.Min(aiPlayer.Chips, minBet);
                            break;
                    }
                    Raise(raiseAmount);
                }
                else
                {
                    Call();
                }
            }
            else
            {
                Fold();
            }
        }
        
        // 获取手牌中的最大牌
        private int GetHighCard(List<Card> hand)
        {
            return Math.Max(Math.Max((int)hand[0].Rank, (int)hand[1].Rank), (int)hand[2].Rank);
        }

        // 评估手牌强度
        private int EvaluateHandStrength(List<Card> hand)
        {
            // 确保有3张牌
            if (hand.Count != 3)
                return 0;

            // 按牌面大小排序
            List<Card> sortedHand = hand.OrderBy(c => c.Rank).ToList();

            // 检查是否为豹子（最大牌型）
            if (IsThreeOfAKind(sortedHand))
                return 6;

            // 检查是否为顺金（同花顺）
            if (IsStraightFlush(sortedHand))
                return 5;

            // 检查是否为金花
            if (IsFlush(sortedHand))
                return 4;

            // 检查是否为顺子
            if (IsStraight(sortedHand))
                return 3;

            // 检查是否为对子
            if (IsPair(sortedHand))
                return 2;

            // 单牌
            return 1;
        }

        // 比较相同牌型的手牌大小
        private int CompareSameTypeHands(List<Card> hand1, List<Card> hand2, int handType)
        {
            // 按牌面大小排序
            List<Card> sortedHand1 = hand1.OrderBy(c => c.Rank).ToList();
            List<Card> sortedHand2 = hand2.OrderBy(c => c.Rank).ToList();

            switch (handType)
            {
                case 6: // 豹子 - 比较单张大小
                    return CompareRanks(sortedHand1[0].Rank, sortedHand2[0].Rank);

                case 5: // 顺金 - 比较最大牌大小
                case 3: // 顺子 - 比较最大牌大小
                    int maxRank1 = Math.Max(Math.Max((int)sortedHand1[0].Rank, (int)sortedHand1[1].Rank), (int)sortedHand1[2].Rank);
                    int maxRank2 = Math.Max(Math.Max((int)sortedHand2[0].Rank, (int)sortedHand2[1].Rank), (int)sortedHand2[2].Rank);
                    // 特殊处理2,3,A和Q,K,A的顺子
                    if (sortedHand1[0].Rank == Rank.Two && sortedHand1[1].Rank == Rank.Three && sortedHand1[2].Rank == Rank.Ace)
                        maxRank1 = 4; // 视为最小顺子
                    if (sortedHand2[0].Rank == Rank.Two && sortedHand2[1].Rank == Rank.Three && sortedHand2[2].Rank == Rank.Ace)
                        maxRank2 = 4; // 视为最小顺子
                    if (sortedHand1[0].Rank == Rank.Queen && sortedHand1[1].Rank == Rank.King && sortedHand1[2].Rank == Rank.Ace)
                        maxRank1 = 15; // 视为最大顺子
                    if (sortedHand2[0].Rank == Rank.Queen && sortedHand2[1].Rank == Rank.King && sortedHand2[2].Rank == Rank.Ace)
                        maxRank2 = 15; // 视为最大顺子
                    return maxRank1.CompareTo(maxRank2);

                case 4: // 金花 - 依次比较单张大小，最后比较花色
                case 1: // 单牌 - 依次比较单张大小，最后比较花色
                    // 按从大到小排序
                    sortedHand1 = hand1.OrderByDescending(c => c.Rank).ToList();
                    sortedHand2 = hand2.OrderByDescending(c => c.Rank).ToList();
                    
                    // 比较单张大小
                    for (int i = 0; i < 3; i++)
                    {
                        int rankCompare = CompareRanks(sortedHand1[i].Rank, sortedHand2[i].Rank);
                        if (rankCompare != 0)
                            return rankCompare;
                    }
                    // 所有单张都相同，比较花色
                    return CompareSuits(sortedHand1[0].Suit, sortedHand2[0].Suit);

                case 2: // 对子 - 比较对子大小，再比较单张大小
                    int pairRank1 = GetPairRank(sortedHand1);
                    int pairRank2 = GetPairRank(sortedHand2);
                    
                    if (pairRank1 != pairRank2)
                        return pairRank1.CompareTo(pairRank2);
                    
                    // 对子大小相同，比较单张
                    int singleRank1 = GetSingleRank(sortedHand1, pairRank1);
                    int singleRank2 = GetSingleRank(sortedHand2, pairRank2);
                    return singleRank1.CompareTo(singleRank2);

                default:
                    return 0;
            }
        }

        // 比较牌面大小
        private int CompareRanks(Rank rank1, Rank rank2)
        {
            // 特殊处理A，在某些情况下A可以作为最小牌
            return ((int)rank1).CompareTo((int)rank2);
        }

        // 比较花色大小（黑桃>红桃>梅花>方块）
        private int CompareSuits(Suit suit1, Suit suit2)
        {
            // 黑桃 > 红桃 > 梅花 > 方块
            int suitValue1 = suit1 == Suit.Spades ? 4 : suit1 == Suit.Hearts ? 3 : suit1 == Suit.Clubs ? 2 : 1;
            int suitValue2 = suit2 == Suit.Spades ? 4 : suit2 == Suit.Hearts ? 3 : suit2 == Suit.Clubs ? 2 : 1;
            return suitValue1.CompareTo(suitValue2);
        }

        // 获取对子的牌面大小
        private int GetPairRank(List<Card> hand)
        {
            if (hand[0].Rank == hand[1].Rank)
                return (int)hand[0].Rank;
            if (hand[1].Rank == hand[2].Rank)
                return (int)hand[1].Rank;
            return 0; // 不应该到达这里
        }

        // 获取单张的牌面大小
        private int GetSingleRank(List<Card> hand, int pairRank)
        {
            foreach (Card card in hand)
            {
                if ((int)card.Rank != pairRank)
                    return (int)card.Rank;
            }
            return 0; // 不应该到达这里
        }

        // 检查是否为豹子（三张相同牌面）
        private bool IsThreeOfAKind(List<Card> hand)
        {
            return hand[0].Rank == hand[1].Rank && hand[1].Rank == hand[2].Rank;
        }

        // 检查是否为顺金（同花顺）
        private bool IsStraightFlush(List<Card> hand)
        {
            return IsFlush(hand) && IsStraight(hand);
        }

        // 检查是否为金花（三张相同花色）
        private bool IsFlush(List<Card> hand)
        {
            return hand[0].Suit == hand[1].Suit && hand[1].Suit == hand[2].Suit;
        }

        // 检查是否为顺子
        private bool IsStraight(List<Card> hand)
        {
            // 特殊情况：2,3,A也视为顺子
            if (hand[0].Rank == Rank.Two && hand[1].Rank == Rank.Three && hand[2].Rank == Rank.Ace)
                return true;
            // 特殊情况：J,Q,K也视为顺子
            if (hand[0].Rank == Rank.Jack && hand[1].Rank == Rank.Queen && hand[2].Rank == Rank.King)
                return true;
            // 特殊情况：Q,K,A也视为顺子
            if (hand[0].Rank == Rank.Queen && hand[1].Rank == Rank.King && hand[2].Rank == Rank.Ace)
                return true;

            // 正常顺子检查
            return (int)hand[1].Rank - (int)hand[0].Rank == 1 && (int)hand[2].Rank - (int)hand[1].Rank == 1;
        }

        // 计算顺子的等级（用于比较顺子大小）
        private int CalculateStraightLevel(List<Card> hand)
        {
            // 特殊情况：Q,K,A是最大的顺子
            if (hand[0].Rank == Rank.Queen && hand[1].Rank == Rank.King && hand[2].Rank == Rank.Ace)
                return 15; // 最高级顺子
            // 特殊情况：J,Q,K
            if (hand[0].Rank == Rank.Jack && hand[1].Rank == Rank.Queen && hand[2].Rank == Rank.King)
                return 14;
            // 特殊情况：2,3,A是最小的顺子
            if (hand[0].Rank == Rank.Two && hand[1].Rank == Rank.Three && hand[2].Rank == Rank.Ace)
                return 4; // 最低级顺子
            
            // 正常顺子返回最大牌的等级
            return Math.Max(Math.Max((int)hand[0].Rank, (int)hand[1].Rank), (int)hand[2].Rank);
        }

        // 检查是否为对子
        private bool IsPair(List<Card> hand)
        {
            return hand[0].Rank == hand[1].Rank || hand[1].Rank == hand[2].Rank;
        }

        // 进入下一个玩家回合
        private void NextPlayer()
        {
            // 找出下一个活跃的玩家
            int nextPlayer = currentPlayer + 1;
            while (nextPlayer < 6)
            {
                if (nextPlayer == 0 || !AIPlayers[nextPlayer - 1].IsFolded)
                {
                    currentPlayer = nextPlayer;
                    return;
                }
                nextPlayer++;
            }

            // 如果已经轮完一圈，检查是否所有玩家都已跟注
            if (AllPlayersCalled())
            {
                // 检查是否需要进入下一轮或比牌阶段
                if (round < 3) // 炸金花通常有3轮下注
                {
                    round++;
                    currentBet = 0;
                    minBet = Math.Max(10, InitialChips / 5); // 第二轮下注翻倍
                    currentPlayer = 0;
                }
                else
                {
                    // 进入比牌阶段
                    currentState = GameState.Comparing;
                    CompareHands();
                }
            }
            else
            {
                // 否则回到第一个未弃牌的玩家
                currentPlayer = 0;
                while (currentPlayer < 6)
                {
                    if (currentPlayer == 0 || !AIPlayers[currentPlayer - 1].IsFolded)
                    {
                        return;
                    }
                    currentPlayer++;
                }
            }
        }

        // 检查是否所有玩家都已跟注
        private bool AllPlayersCalled()
        {
            // 检查有多少玩家仍在游戏中
            int activePlayers = 0;
            if (currentBet > 0) // 只有当前有下注时才检查
            {
                // 玩家是否已全下或弃牌
                bool playerAllInOrFolded = playerChips == 0;
                if (!playerAllInOrFolded) activePlayers++;
                
                // 检查所有AI玩家
                foreach (AIPlayer ai in AIPlayers)
                {
                    if (!ai.IsFolded && ai.Chips > 0)
                    {
                        activePlayers++;
                    }
                }
            }
            
            // 如果只有一个玩家活跃，则自动获胜
            if (activePlayers <= 1)
                return true;
            
            // 检查玩家是否已跟注或全下
            bool playerCalled = playerChips == 0;
            if (!playerCalled) return false;
            
            // 检查所有AI玩家是否已跟注、全下或弃牌
            foreach (AIPlayer ai in AIPlayers)
            {
                if (!ai.IsFolded && ai.Chips > 0)
                {
                    return false;
                }
            }
            return true;
        }

        // 比牌并确定胜者
        private void CompareHands()
        {
            currentState = GameState.Ended;

            // 收集所有未弃牌的玩家及其手牌强度
            List<(int playerIndex, List<Card> hand, int handStrength)> activePlayers = new List<(int, List<Card>, int)>();

            // 添加玩家（如果未弃牌）
            activePlayers.Add((0, playerHand, EvaluateHandStrength(playerHand)));

            // 添加未弃牌的AI玩家
            for (int i = 0; i < 5; i++)
            {
                if (!AIPlayers[i].IsFolded)
                {
                    activePlayers.Add((i + 1, AIPlayers[i].Hand, EvaluateHandStrength(AIPlayers[i].Hand)));
                }
            }

            // 找到手牌最强的玩家
            int maxStrength = activePlayers.Max(p => p.handStrength);
            var strongestPlayers = activePlayers.Where(p => p.handStrength == maxStrength).ToList();

            // 如果有多个玩家手牌强度相同，进一步比较
            List<(int playerIndex, List<Card> hand)> winners = new List<(int, List<Card>)>();
            if (strongestPlayers.Count > 1)
            {
                // 保存第一个玩家作为临时胜者
                winners.Add((strongestPlayers[0].playerIndex, strongestPlayers[0].hand));
                
                // 与其他玩家比较
                for (int i = 1; i < strongestPlayers.Count; i++)
                {
                    int compareResult = CompareSameTypeHands(winners[0].hand, strongestPlayers[i].hand, maxStrength);
                    if (compareResult < 0)
                    {
                        // 当前玩家手牌更大
                        winners.Clear();
                        winners.Add((strongestPlayers[i].playerIndex, strongestPlayers[i].hand));
                    }
                    else if (compareResult == 0)
                    {
                        // 手牌相同，加入胜者列表
                        winners.Add((strongestPlayers[i].playerIndex, strongestPlayers[i].hand));
                    }
                }
            }
            else
            {
                // 只有一个胜者
                winners.Add((strongestPlayers[0].playerIndex, strongestPlayers[0].hand));
            }

            // 如果有多个胜者，平分底池
            int splitPot = pot / winners.Count;

            // 分配筹码给胜者
            foreach (var winner in winners)
            {
                if (winner.playerIndex == 0)
                {
                    // 玩家获胜
                    playerChips += splitPot;
                    Debug.Log("恭喜你赢了这一局！获得筹码: " + splitPot);
                }
                else
                {
                    // AI获胜
                    AIPlayers[winner.playerIndex - 1].Chips += splitPot;
                    Debug.Log(AIPlayers[winner.playerIndex - 1].Name + " 赢了这一局，获得筹码: " + splitPot);
                }
            }

            // 重置底池
            pot = 0;
        }

        // 重置游戏 - 确保重置所有视觉状态
        private void ResetGame()
        {
            // 清空牌组和手牌
            deck.Clear();
            playerHand.Clear();
            foreach (AIPlayer ai in AIPlayers)
            {
                ai.Hand.Clear();
                ai.IsFolded = false;
                ai.IsPlaying = true;
            }

            // 重置游戏状态
            currentState = GameState.Waiting;
            currentPlayer = -1;
            currentBet = 0;
            pot = 0;
            round = 0;
            
            // 重置视觉状态
            isAIActionAnimating = false;
            aiActionText = "";
            aiActionTimer = 0f;
        }

        // 返回主菜单
        private void ReturnToMainMenu()
        {
            // 保存玩家筹码
            int finalChips = playerChips;

            // 关闭游戏窗口
            showMenu = false;
            blockGameInput = false;

            // 查找主窗口并传递筹码
            A_MoreGambles moreGambles = FindObjectOfType<A_MoreGambles>();
            if (moreGambles != null)
            {
                // 返回主窗口并带回结余筹码
                moreGambles.ReturnFromGame(finalChips, UsePaperMoney);
            }
        }

        // 主窗口调用此方法来打开游戏
        public void OpenGame(int initialChips, bool usePaperMoney)
        {
            UsePaperMoney = usePaperMoney;
            InitialChips = initialChips;
            playerChips = initialChips;
            InitializeAIPlayers();
            
            // 随机设置AI筹码为玩家持有筹码的0.5到3倍范围
            System.Random random = new System.Random();
            for (int i = 0; i < AIPlayers.Count; i++)
            {
                // 生成0.5到3倍玩家筹码的随机数
                float randomFactor = (float)(random.NextDouble() * 2.5 + 0.5); // 0.5-3.0之间的随机数
                AIPlayers[i].Chips = Mathf.RoundToInt(playerChips * randomFactor);
            }
            
            showMenu = true;
            blockGameInput = true;
            UpdateResolutionSettings();
        }
    }
}
