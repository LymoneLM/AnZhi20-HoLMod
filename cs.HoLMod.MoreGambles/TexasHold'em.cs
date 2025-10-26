using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;

namespace cs.HoLMod.MoreGambles
{
    public class TexasHold_em : BaseUnityPlugin
    {
        // 窗口设置
        private Rect windowRect = new Rect(20, 20, 1200, 800);
        private bool showMenu = false;
        private bool blockGameInput = false;
        private Vector2 scrollPosition;
        private float scaleFactor = 1f;
        private const string VERSION = "2.0.0";
        private float animationTime = 0f;
        
        // 货币系统相关
        public bool UsePaperMoney { get; set; } = true; // true=宝钞, false=金钞
        public int InitialChips { get; set; } = 1000;
        public int RemainingMoney { get; set; } = 0;
        
        // 游戏结束窗口
        private Rect gameOverWindowRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 200);
        private string gameOverMessage = "";
        private bool showGameOverWindow = false;

        // 德州扑克游戏状态
        public enum GameState
        {
            Waiting,         // 等待开始
            PreFlop,         // 翻牌前
            Flop,            // 翻牌
            Turn,            // 转牌
            River,           // 河牌
            Showdown,        // 摊牌
            GameOver         // 游戏结束
        }

        // 当前游戏状态
        public GameState currentGameState = GameState.Waiting;

        // 货币系统
        private int playerMoney = 0;
        private int betAmount = 10;
        private int minBet = 10;
        private int maxBet = 1000;

        // 玩家和AI
        private List<Player> players = new List<Player>();
        private Player humanPlayer;
        private int currentPlayerIndex = 0;

        // 牌桌和牌组
        private List<Card> deck = new List<Card>();
        private List<Card> communityCards = new List<Card>();
        private int pot = 0;
        private int currentBettingRound = 0;

        // 分辨率适配
        private float originalScreenWidth = 1920f;
        private float originalScreenHeight = 1080f;

        // 音效相关（占位符）
        private bool soundEnabled = true;
        
        // 重复游戏逻辑相关
        private bool hasCalledRepeatLogic = false;

        // 纹理缓存
        private Texture2D windowBackground;
        private Texture2D titleGradient;
        private Texture2D playerCellBackground;
        private Texture2D cardBackground;
        private Texture2D highlightTexture;
        private Texture2D buttonNormalTexture;
        private Texture2D buttonHoverTexture;
        // 游戏结束窗口纹理
        private Texture2D gameOverBackground;
        private Texture2D gameOverTitleGradient;
        private Texture2D gameOverMessageBackground;
        private Texture2D continueButtonNormal;
        private Texture2D continueButtonHover;
        private Texture2D returnButtonNormal;
        private Texture2D returnButtonHover;

        // 更新分辨率设置
        private void UpdateResolutionSettings()
        {
            // 计算缩放因子以适应不同分辨率
            scaleFactor = Mathf.Min(Screen.width / originalScreenWidth, Screen.height / originalScreenHeight);
            scaleFactor = Mathf.Clamp(scaleFactor, 0.5f, 2f); // 限制缩放范围
        }

        private void Awake()
        {
            InitializePlayers();
            InitializeDeck();
            UpdateResolutionSettings();
            
            // 加载玩家货币
            playerMoney = InitialChips;
            humanPlayer.chips = playerMoney;
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

        // 创建渐变纹理
        private Texture2D MakeGradientTex(int width, int height, Color topColor, Color bottomColor)
        {
            Texture2D gradient = new Texture2D(width, height);
            for (int y = 0; y < height; y++)
            {
                Color color = Color.Lerp(topColor, bottomColor, (float)y / (float)height);
                for (int x = 0; x < width; x++)
                {
                    gradient.SetPixel(x, y, color);
                }
            }
            gradient.Apply();
            return gradient;
        }

        // 创建圆形纹理
        private Texture2D MakeCircleTex(int radius, Color color)
        {
            int size = radius * 2;
            Texture2D circle = new Texture2D(size, size);
            float rSquared = radius * radius;
            
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float dx = x - radius;
                    float dy = y - radius;
                    if (dx * dx + dy * dy <= rSquared)
                    {
                        circle.SetPixel(x, y, color);
                    }
                    else
                    {
                        circle.SetPixel(x, y, new Color(0, 0, 0, 0));
                    }
                }
            }
            
            circle.Apply();
            return circle;
        }

        // 创建阴影纹理
        private Texture2D MakeShadowTex(int width, int height, float intensity = 0.3f)
        {
            Texture2D shadow = new Texture2D(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 简单的径向渐变阴影
                    float distanceFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(width / 2, height / 2)) / (Mathf.Min(width, height) / 2);
                    float alpha = Mathf.Lerp(0, intensity, distanceFromCenter);
                    shadow.SetPixel(x, y, new Color(0, 0, 0, alpha));
                }
            }
            shadow.Apply();
            return shadow;
        }

        // 创建彩色灯光效果纹理
        private Texture2D MakeColorfulLightTex(int width, int height)
        {
            Texture2D light = new Texture2D(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 创建彩虹色渐变
                    float hue = (float)(x + y) / (float)(width + height);
                    Color color = Color.HSVToRGB(hue, 0.7f, 1f);
                    color.a = 0.3f; // 半透明效果
                    light.SetPixel(x, y, color);
                }
            }
            light.Apply();
            return light;
        }
        
        // 重载：创建指定颜色的彩色灯光效果纹理
        private Texture2D MakeColorfulLightTex(int width, int height, Color color)
        {
            Texture2D light = new Texture2D(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 使用指定颜色
                    light.SetPixel(x, y, color);
                }
            }
            light.Apply();
            return light;
        }

        // 初始化玩家
        private void InitializePlayers()
        {
            players.Clear();
            
            // 创建人类玩家
            humanPlayer = new Player("你", InitialChips, true);
            players.Add(humanPlayer);
            
            // 创建5个AI玩家
            List<string> aiNames = B_AI_Name.GenerateRandomNames(5);
            for (int i = 0; i < 5; i++)
            {
                players.Add(new Player(aiNames[i], InitialChips));
            }
            
            // 设置庄家
            players[1].isDealer = true;
            players[1].name += "(庄家)";
        }

        private void Update()
        {
            // 按F5键切换窗口显示
            if (Input.GetKeyDown(KeyCode.F5))
            {
                UpdateResolutionSettings();
                showMenu = !showMenu;
            blockGameInput = showMenu;
            Logger.LogInfo(showMenu ? "德州扑克窗口已打开" : "德州扑克窗口已关闭");
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

            // 更新动画时间
            animationTime += Time.deltaTime;

            // 保存窗口背景色
            Color originalBackgroundColor = GUI.backgroundColor;

            // 绘制精美的背景效果
            DrawBackgroundEffect();

            // 应用缩放因子
            Matrix4x4 guiMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleFactor, scaleFactor, 1f));

            // 根据缩放因子调整字体大小和样式
            GUI.skin.window.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            GUI.skin.window.padding = new RectOffset(
                Mathf.RoundToInt(20 * scaleFactor),
                Mathf.RoundToInt(20 * scaleFactor),
                Mathf.RoundToInt(10 * scaleFactor),
                Mathf.RoundToInt(10 * scaleFactor)
            );

            GUI.skin.label.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            GUI.skin.button.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            GUI.skin.textField.fontSize = Mathf.RoundToInt(12 * scaleFactor);

            // 设置窗口的背景纹理
            if (windowBackground == null)
            {
                windowBackground = MakeGradientTex(100, 100, new Color(0.15f, 0.35f, 0.05f, 1f), new Color(0.05f, 0.15f, 0f, 1f));
            }
            GUI.skin.window.normal.background = windowBackground;

            // 显示游戏结束窗口
            if (showGameOverWindow)
            {
                gameOverWindowRect = GUI.Window(1, gameOverWindowRect, DrawGameOverWindow, "游戏结束");
            } else {
                windowRect = GUI.Window(0, windowRect, DrawWindow, "");
            }

            // 绘制背景效果
            void DrawBackgroundEffect()
            {
                GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
                
                // 半透明黑色背景遮罩
                GUI.color = new Color(0, 0, 0, 0.7f);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
                
                // 如果游戏正在进行，添加动态灯光效果
                if (currentGameState != GameState.Waiting)
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.1f);
                    Texture2D lightTexture = MakeColorfulLightTex(500, 500);
                    float lightRotation = animationTime * 10f;
                    GUIUtility.RotateAroundPivot(lightRotation, new Vector2(Screen.width / 2, Screen.height / 2));
                    GUI.DrawTexture(new Rect(Screen.width / 2 - 250, Screen.height / 2 - 250, 500, 500), lightTexture);
                    GUI.matrix = Matrix4x4.identity; // 重置旋转
                }
                
                GUI.color = Color.white;
                GUI.EndGroup();
            }

            // 恢复原始矩阵和背景色
            GUI.matrix = guiMatrix;
            GUI.backgroundColor = originalBackgroundColor;
        }

        // 牌型判定核心逻辑
        private HandRank EvaluateHand(List<Card> hand)
        {
            if (hand.Count < 5) return HandRank.HighCard;

            // 按点数排序
            var sortedByRank = hand.OrderByDescending(c => c.rank).ToList();

            // 检查皇家同花顺
            if (IsRoyalFlush(sortedByRank))
                return HandRank.RoyalFlush;

            // 检查同花顺
            if (IsStraightFlush(sortedByRank))
                return HandRank.StraightFlush;

            // 检查四条
            if (IsFourOfAKind(sortedByRank))
                return HandRank.FourOfAKind;

            // 检查葫芦
            if (IsFullHouse(sortedByRank))
                return HandRank.FullHouse;

            // 检查同花
            if (IsFlush(sortedByRank))
                return HandRank.Flush;

            // 检查顺子
            if (IsStraight(sortedByRank))
                return HandRank.Straight;

            // 检查三条
            if (IsThreeOfAKind(sortedByRank))
                return HandRank.ThreeOfAKind;

            // 检查两对
            if (IsTwoPair(sortedByRank))
                return HandRank.TwoPair;

            // 检查一对
            if (IsOnePair(sortedByRank))
                return HandRank.OnePair;

            // 高牌
            return HandRank.HighCard;
        }

        // 皇家同花顺判定
        private bool IsRoyalFlush(List<Card> hand)
        {
            // 必须是同花顺，且包含10、J、Q、K、A
            if (!IsStraightFlush(hand)) return false;
            
            var ranks = hand.Select(c => c.rank).Distinct().ToList();
            return ranks.Contains(Rank.Ten) && 
                   ranks.Contains(Rank.Jack) && 
                   ranks.Contains(Rank.Queen) && 
                   ranks.Contains(Rank.King) && 
                   ranks.Contains(Rank.Ace);
        }

        // 同花顺判定
        private bool IsStraightFlush(List<Card> hand)
        {
            return IsFlush(hand) && IsStraight(hand);
        }

        // 四条判定
        private bool IsFourOfAKind(List<Card> hand)
        {
            var rankCounts = hand.GroupBy(c => c.rank).Select(g => new { Rank = g.Key, Count = g.Count() });
            return rankCounts.Any(rc => rc.Count == 4);
        }

        // 葫芦判定
        private bool IsFullHouse(List<Card> hand)
        {
            var rankCounts = hand.GroupBy(c => c.rank).Select(g => new { Rank = g.Key, Count = g.Count() }).ToList();
            return rankCounts.Any(rc => rc.Count == 3) && rankCounts.Any(rc => rc.Count == 2);
        }

        // 同花判定
        private bool IsFlush(List<Card> hand)
        {
            var suitCounts = hand.GroupBy(c => c.suit).Select(g => new { Suit = g.Key, Count = g.Count() });
            return suitCounts.Any(sc => sc.Count >= 5);
        }

        // 顺子判定
        private bool IsStraight(List<Card> hand)
        {
            var distinctRanks = hand.Select(c => (int)c.rank).Distinct().OrderBy(r => r).ToList();
            
            // 检查是否有连续的5个点数
            for (int i = 0; i <= distinctRanks.Count - 5; i++)
            {
                bool isStraight = true;
                for (int j = 0; j < 4; j++)
                {
                    if (distinctRanks[i + j + 1] - distinctRanks[i + j] != 1)
                    {
                        isStraight = false;
                        break;
                    }
                }
                if (isStraight)
                    return true;
            }

            // 检查特殊情况：10-J-Q-K-A（皇家同花顺会单独判断）
            if (distinctRanks.Contains(10) && distinctRanks.Contains(11) && 
                distinctRanks.Contains(12) && distinctRanks.Contains(13) && distinctRanks.Contains(14))
                return true;

            return false;
        }

        // 三条判定
        private bool IsThreeOfAKind(List<Card> hand)
        {
            var rankCounts = hand.GroupBy(c => c.rank).Select(g => new { Rank = g.Key, Count = g.Count() });
            return rankCounts.Any(rc => rc.Count == 3);
        }

        // 两对判定
        private bool IsTwoPair(List<Card> hand)
        {
            var rankCounts = hand.GroupBy(c => c.rank).Select(g => new { Rank = g.Key, Count = g.Count() });
            return rankCounts.Count(rc => rc.Count == 2) >= 2;
        }

        // 一对判定
        private bool IsOnePair(List<Card> hand)
        {
            var rankCounts = hand.GroupBy(c => c.rank).Select(g => new { Rank = g.Key, Count = g.Count() });
            return rankCounts.Any(rc => rc.Count == 2);
        }

        // 获取手牌点数（用于比较相同牌型时的大小）
        private List<Rank> GetHandValues(List<Card> hand)
        {
            // 根据不同牌型返回相应的排序值
            var rankCounts = hand.GroupBy(c => c.rank)
                                 .Select(g => new { Rank = g.Key, Count = g.Count() })
                                 .OrderByDescending(g => g.Count)
                                 .ThenByDescending(g => g.Rank)
                                 .ToList();

            List<Rank> result = new List<Rank>();
            foreach (var group in rankCounts)
            {
                result.AddRange(Enumerable.Repeat(group.Rank, group.Count));
            }

            return result;
        }

        // 初始化卡牌
        private void InitializeDeck()
        {
            deck.Clear();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    deck.Add(new Card(suit, rank, true));
                }
            }
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
        
        private void DrawWindow(int windowID)
        {
            // 确保纹理已初始化
            InitializeTextures();

            // 窗口拖动区域
            GUI.DragWindow(new Rect(0, 0, windowRect.width, 50));

            // 设置字体大小
            int defaultFontSize = Mathf.RoundToInt(14 * scaleFactor);
            GUI.skin.label.fontSize = defaultFontSize;
            GUI.skin.button.fontSize = defaultFontSize;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;

            // 窗口最小宽度和高度
            windowRect.width = Mathf.Max(windowRect.width, 1200f * scaleFactor);
            windowRect.height = Mathf.Max(windowRect.height, 800f * scaleFactor);

            // 绘制精美的标题栏
            DrawTitleBar();
            
            // 绘制动态彩色灯光效果
            DrawDynamicLights();

            GUILayout.BeginVertical();
            GUILayout.Space(50f * scaleFactor); // 标题栏高度

            // 游戏信息区域
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawGameInfoPanel();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20f * scaleFactor);

            // 玩家网格布局 - 3行2列
            float cellWidth = (windowRect.width - 80f * scaleFactor) / 3f;
            float cellHeight = 140f * scaleFactor;

            // 第一行
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // AI玩家1 (庄家)
            if (players.Count > 1)
            {
                DrawPlayerCell(players[1], cellWidth, cellHeight);
                GUILayout.Space(10f * scaleFactor);
            }
            
            // AI玩家2
            if (players.Count > 2)
            {
                DrawPlayerCell(players[2], cellWidth, cellHeight);
                GUILayout.Space(10f * scaleFactor);
            }
            
            // AI玩家3
            if (players.Count > 3)
            {
                DrawPlayerCell(players[3], cellWidth, cellHeight);
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10f * scaleFactor);

            // 第二行 - 中间是牌桌
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // AI玩家4
            if (players.Count > 4)
            {
                DrawPlayerCell(players[4], cellWidth, cellHeight);
                GUILayout.Space(10f * scaleFactor);
            }
            
            // 牌桌区域 - 显示公共牌
            DrawTableArea(cellWidth, cellHeight);
            
            // AI玩家5
            if (players.Count > 5)
            {
                DrawPlayerCell(players[5], cellWidth, cellHeight);
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10f * scaleFactor);

            // 第三行 - 玩家自己
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawHumanPlayerArea(cellWidth, cellHeight);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20f * scaleFactor);

            // 当前轮到谁行动 - 添加发光效果
            if (currentGameState != GameState.Waiting && currentGameState != GameState.Showdown)
            {
                DrawCurrentPlayerIndicator();
                GUILayout.Space(20f * scaleFactor);
            }

            // 操作按钮区域 - 添加精美的按钮样式
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawActionButtons();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20f * scaleFactor);

            // 返回主窗口按钮
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawBackToMainMenuButton();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            // 辅助方法：初始化纹理
            void InitializeTextures()
            {
                if (titleGradient == null)
                    titleGradient = MakeGradientTex(100, 100, new Color(0.25f, 0.5f, 0.15f, 1f), new Color(0.1f, 0.25f, 0.05f, 1f));
                
                if (playerCellBackground == null)
                    playerCellBackground = MakeGradientTex(100, 100, new Color(0.15f, 0.3f, 0.05f, 0.9f), new Color(0.05f, 0.15f, 0.02f, 0.9f));
                
                if (cardBackground == null)
                    cardBackground = MakeGradientTex(100, 100, new Color(1f, 0.95f, 0.9f, 1f), new Color(0.95f, 0.85f, 0.7f, 1f));
                
                if (highlightTexture == null)
                    highlightTexture = MakeTex(100, 100, new Color(1f, 0.8f, 0.2f, 0.3f));
                
                if (buttonNormalTexture == null)
                    buttonNormalTexture = MakeGradientTex(100, 100, new Color(0.25f, 0.5f, 0.75f, 0.9f), new Color(0.15f, 0.35f, 0.55f, 0.9f));
                
                if (buttonHoverTexture == null)
                    buttonHoverTexture = MakeGradientTex(100, 100, new Color(0.35f, 0.6f, 0.85f, 0.9f), new Color(0.25f, 0.45f, 0.65f, 0.9f));
            }

            // 辅助方法：绘制标题栏
            void DrawTitleBar()
            {
                // 绘制标题栏渐变背景
                GUI.DrawTexture(new Rect(0, 0, windowRect.width, 50), titleGradient);
                
                // 添加发光效果
                float glowFactor = 0.8f + 0.2f * Mathf.Sin(animationTime * 2f);
                GUI.color = new Color(1f, 0.8f, 0.2f, glowFactor);
                
                // 绘制标题文字
                GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
                titleStyle.fontSize = Mathf.RoundToInt(24 * scaleFactor);
                titleStyle.alignment = TextAnchor.MiddleCenter;
                titleStyle.fontStyle = FontStyle.Bold;
                GUI.Label(new Rect(0, 5, windowRect.width, 40), "德州扑克", titleStyle);
                
                // 绘制版本号
                GUIStyle versionStyle = new GUIStyle(GUI.skin.label);
                versionStyle.fontSize = Mathf.RoundToInt(10 * scaleFactor);
                versionStyle.alignment = TextAnchor.MiddleRight;
                GUI.color = new Color(1f, 0.9f, 0.7f, 0.8f);
                GUI.Label(new Rect(windowRect.width - 80, 5, 70, 20), "v" + VERSION, versionStyle);
                
                GUI.color = Color.white;
            }

            // 辅助方法：绘制游戏信息面板
            void DrawGameInfoPanel()
            {
                GUILayout.BeginHorizontal(GUILayout.Width(600f * scaleFactor));
                
                // 游戏状态显示
                string stateText = currentGameState == GameState.Waiting ? "等待开始" :
                                currentGameState == GameState.PreFlop ? "底牌阶段" :
                                currentGameState == GameState.Flop ? "翻牌阶段" :
                                currentGameState == GameState.Turn ? "转牌阶段" :
                                currentGameState == GameState.River ? "河牌阶段" : "摊牌阶段";
                
                // 根据游戏状态设置颜色
                Color stateColor = currentGameState == GameState.Waiting ? new Color(0.8f, 0.8f, 1f) :
                                   currentGameState == GameState.PreFlop ? new Color(1f, 0.8f, 0.5f) :
                                   new Color(0.5f, 1f, 0.5f);
                
                GUIStyle stateStyle = new GUIStyle(GUI.skin.label);
                stateStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
                stateStyle.fontStyle = FontStyle.Bold;
                stateStyle.normal.textColor = stateColor;
                
                GUILayout.Label("当前阶段：", new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(16 * scaleFactor) });
                GUILayout.Label(stateText, stateStyle);
                GUILayout.FlexibleSpace();
                
                // 底池显示
                GUIStyle potStyle = new GUIStyle(GUI.skin.label);
                potStyle.fontSize = Mathf.RoundToInt(18 * scaleFactor);
                potStyle.fontStyle = FontStyle.Bold;
                potStyle.normal.textColor = new Color(1f, 0.9f, 0.2f);
                
                GUILayout.Label("底池：" + pot + " 筹码", potStyle);
                
                GUILayout.EndHorizontal();
            }

            // 辅助方法：绘制牌桌区域
            void DrawTableArea(float width, float height)
            {
                GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(height));
                
                // 绘制牌桌背景
                GUI.DrawTexture(new Rect(windowRect.width / 2 - width / 2, 100, width, height), playerCellBackground);
                
                // 添加轻微的发光效果
                if (communityCards.Any(c => c.isVisible))
                {
                    GUI.color = new Color(1f, 1f, 0.5f, 0.2f);
                    GUI.DrawTexture(new Rect(windowRect.width / 2 - width / 2, 100, width, height), highlightTexture);
                    GUI.color = Color.white;
                }
                
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("公共牌：", new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(16 * scaleFactor), fontStyle = FontStyle.Bold });
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(15f * scaleFactor);
                
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                foreach (var card in communityCards)
                {
                    if (card.isVisible)
                    {
                        DrawCard(card);
                        GUILayout.Space(10f * scaleFactor);
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                
                GUILayout.Space(10f * scaleFactor);
            }

            // 辅助方法：绘制人类玩家区域
            void DrawHumanPlayerArea(float width, float height)
            {
                GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(height));
                
                // 绘制玩家单元格背景
                GUI.DrawTexture(new Rect(windowRect.width / 2 - width / 2, windowRect.height - height - 120, width, height), playerCellBackground);
                
                // 如果是当前行动玩家，添加高亮效果
                if (humanPlayer.isTurn && !humanPlayer.isFolded)
                {
                    float pulse = 0.5f + 0.5f * Mathf.Sin(animationTime * 4f);
                    GUI.color = new Color(1f, 0.8f, 0.2f, 0.3f * pulse);
                    GUI.DrawTexture(new Rect(windowRect.width / 2 - width / 2, windowRect.height - height - 120, width, height), highlightTexture);
                    GUI.color = Color.white;
                }
                
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("你", new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(16 * scaleFactor), fontStyle = FontStyle.Bold });
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10f * scaleFactor);
                
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                foreach (var card in humanPlayer.handCards)
                {
                    DrawCard(card);
                    GUILayout.Space(10f * scaleFactor);
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10f * scaleFactor);
                
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                string currencyType = UsePaperMoney ? "宝钞" : "金钞";
                GUILayout.Label("你的" + currencyType + "：" + humanPlayer.chips, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = Mathf.RoundToInt(14 * scaleFactor) });
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            // 辅助方法：绘制当前行动玩家指示器
            void DrawCurrentPlayerIndicator()
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                
                string actionText = "";
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].isTurn && !players[i].isFolded)
                    {
                        actionText = "现在轮到" + players[i].name + "行动";
                        break;
                    }
                }
                
                // 创建发光效果
                float pulse = 0.8f + 0.2f * Mathf.Sin(animationTime * 2f);
                
                GUIStyle actionStyle = new GUIStyle(GUI.skin.label);
                actionStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
                actionStyle.fontStyle = FontStyle.Bold;
                actionStyle.normal.textColor = new Color(1f, pulse, 0.5f);
                
                GUILayout.Label(actionText, actionStyle);
                
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            // 辅助方法：绘制操作按钮
            void DrawActionButtons()
            {
                if (currentGameState == GameState.Waiting)
                {
                    // 创建开始游戏按钮样式
                    GUIStyle startButtonStyle = CreateButtonStyle(new Color(0.3f, 0.7f, 0.3f), new Color(0.4f, 0.8f, 0.4f));
                    startButtonStyle.fontSize = Mathf.RoundToInt(18 * scaleFactor);
                    
                    if (GUILayout.Button("开始游戏", startButtonStyle, GUILayout.Width(200f * scaleFactor), GUILayout.Height(55f * scaleFactor)))
                    {
                        StartNewGame();
                        PlaySound("Start");
                    }
                }
                else if (humanPlayer.isTurn && !humanPlayer.isFolded)
                {
                    // 创建不同操作按钮的样式
                    GUIStyle checkButtonStyle = CreateButtonStyle(new Color(0.2f, 0.4f, 0.6f), new Color(0.3f, 0.5f, 0.7f));
                    GUIStyle betButtonStyle = CreateButtonStyle(new Color(0.5f, 0.3f, 0.1f), new Color(0.6f, 0.4f, 0.2f));
                    GUIStyle callButtonStyle = CreateButtonStyle(new Color(0.3f, 0.5f, 0.3f), new Color(0.4f, 0.6f, 0.4f));
                    GUIStyle foldButtonStyle = CreateButtonStyle(new Color(0.7f, 0.2f, 0.2f), new Color(0.8f, 0.3f, 0.3f));
                    
                    if (GUILayout.Button("看牌", checkButtonStyle, GUILayout.Width(130f * scaleFactor), GUILayout.Height(50f * scaleFactor)))
                    {
                        Check();
                        PlaySound("Check");
                    }
                    GUILayout.Space(15f * scaleFactor);
                    
                    if (GUILayout.Button("下注", betButtonStyle, GUILayout.Width(130f * scaleFactor), GUILayout.Height(50f * scaleFactor)))
                    {
                        Bet(betAmount);
                        PlaySound("Bet");
                    }
                    GUILayout.Space(15f * scaleFactor);
                    
                    if (GUILayout.Button("跟注", callButtonStyle, GUILayout.Width(130f * scaleFactor), GUILayout.Height(50f * scaleFactor)))
                    {
                        Call();
                        PlaySound("Call");
                    }
                    GUILayout.Space(15f * scaleFactor);
                    
                    if (GUILayout.Button("弃牌", foldButtonStyle, GUILayout.Width(130f * scaleFactor), GUILayout.Height(50f * scaleFactor)))
                    {
                        Fold();
                        PlaySound("Fold");
                    }
                }
                else
                {
                    GUIStyle waitStyle = new GUIStyle(GUI.skin.label);
                    waitStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
                    waitStyle.alignment = TextAnchor.MiddleCenter;
                    waitStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
                    
                    GUILayout.Label("等待其他玩家行动...", waitStyle, GUILayout.Width(350f * scaleFactor));
                }
            }

            // 辅助方法：绘制返回主窗口按钮
            void DrawBackToMainMenuButton()
            {
                GUIStyle backButtonStyle = CreateButtonStyle(new Color(0.5f, 0.2f, 0.2f), new Color(0.6f, 0.3f, 0.3f));
                
                if (GUILayout.Button("返回主窗口", backButtonStyle, GUILayout.Width(150f * scaleFactor), GUILayout.Height(45f * scaleFactor)))
                {
                    ReturnToMainMenu();
                }
            }

            // 辅助方法：创建按钮样式
            GUIStyle CreateButtonStyle(Color normalColor, Color hoverColor)
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.fontSize = Mathf.RoundToInt(16 * scaleFactor);
                style.normal.textColor = Color.white;
                style.hover.textColor = Color.white;
                style.active.textColor = Color.white;
                style.fontStyle = FontStyle.Bold;
                style.normal.background = MakeGradientTex(100, 100, normalColor, normalColor * 0.8f);
                style.hover.background = MakeGradientTex(100, 100, hoverColor, hoverColor * 0.8f);
                style.active.background = MakeGradientTex(100, 100, normalColor * 0.7f, normalColor * 0.5f);
                return style;
            }
            
            // 辅助方法：绘制动态彩色灯光效果
            void DrawDynamicLights()
            {
                // 只在游戏进行中显示动态灯光效果
                if (currentGameState == GameState.Waiting || currentGameState == GameState.GameOver)
                    return;
                
                // 创建多个动态灯光效果
                int lightCount = 5; // 灯光数量
                for (int i = 0; i < lightCount; i++)
                {
                    // 计算每个灯光的位置、大小和颜色随时间变化
                    float timeOffset = i * Mathf.PI * 2f / lightCount;
                    float size = 300f + 50f * Mathf.Sin(animationTime + timeOffset);
                    float alpha = 0.05f + 0.03f * Mathf.Abs(Mathf.Sin(animationTime * 0.5f + timeOffset));
                    
                    // 计算灯光位置（围绕牌桌中心）
                    float angle = animationTime * 0.2f + timeOffset;
                    float x = windowRect.width / 2 + Mathf.Cos(angle) * 200f;
                    float y = windowRect.height / 2 + Mathf.Sin(angle) * 200f;
                    
                    // 计算灯光颜色（使用彩虹色调）
                    float hue = (animationTime * 0.1f + timeOffset) % (Mathf.PI * 2f);
                    Color color = Color.HSVToRGB(hue / (Mathf.PI * 2f), 0.8f, 1f);
                    color.a = alpha;
                    
                    // 创建并绘制彩色灯光效果
                    Texture2D lightTexture = MakeColorfulLightTex((int)size, (int)size, color);
                    GUI.DrawTexture(new Rect(x - size / 2, y - size / 2, size, size), lightTexture);
                    
                    // 立即销毁临时纹理以避免内存泄漏
                    DestroyImmediate(lightTexture);
                }
            }
        }

        // 绘制玩家单元格
        private void DrawPlayerCell(Player player, float width, float height)
        {
            GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(height));
            
            // 玩家名称
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (player.isFolded)
            {
                GUILayout.Label(player.name + " (已弃牌)", new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(14 * scaleFactor) });
            }
            else
            {
                GUILayout.Label(player.name, new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(14 * scaleFactor) });
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5f * scaleFactor);
            
            // 玩家手牌
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (!player.isFolded)
            {
                foreach (var card in player.handCards)
                {
                    DrawCard(card);
                    GUILayout.Space(5f * scaleFactor);
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5f * scaleFactor);
            
            // 玩家筹码
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(player.name + " 筹码：" + player.chips);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }

        // 绘制卡牌
        private void DrawCard(Card card)
        {
            string cardText = card.GetCardString();
            Color textColor = card.GetSuitColor();
            Color originalColor = GUI.color;
            GUI.color = textColor;
            GUIStyle cardStyle = new GUIStyle(GUI.skin.box);
            cardStyle.alignment = TextAnchor.MiddleCenter;
            cardStyle.fontSize = Mathf.RoundToInt(18 * scaleFactor);
            GUILayout.Box(cardText, cardStyle, GUILayout.Width(60f * scaleFactor), GUILayout.Height(80f * scaleFactor));
            GUI.color = originalColor;
        }

        // 开始新游戏
        private void StartNewGame()
        {
            InitializeDeck();
            ShuffleDeck();
            
            // 重置所有玩家状态
            foreach (var player in players)
            {
                player.ResetState();
            }
            
            // 重新设置庄家（轮流转）
            // 找到当前庄家
            int dealerIndex = players.FindIndex(p => p.isDealer);
            if (dealerIndex >= 0)
            {
                players[dealerIndex].isDealer = false;
                string originalName = players[dealerIndex].name.Replace("(庄家)", "");
                players[dealerIndex].name = originalName.Trim();
            }
            
            // 设置新庄家（下一位非人类玩家）
            int newDealerIndex = 1;
            for (int i = 1; i < players.Count; i++)
            {
                if (!players[i].isHuman)
                {
                    newDealerIndex = i;
                    break;
                }
            }
            players[newDealerIndex].isDealer = true;
            if (!players[newDealerIndex].name.Contains("(庄家)"))
            {
                players[newDealerIndex].name += "(庄家)";
            }
            
            // 清空之前的牌
            communityCards.Clear();
            pot = 0;
            
            // 发牌给所有玩家
            for (int i = 0; i < 2; i++)
            {
                // 给每个玩家发牌
                foreach (var player in players)
                {
                    bool isVisible = player.isHuman;
                    player.handCards.Add(new Card(deck[0].suit, deck[0].rank, isVisible));
                    deck.RemoveAt(0);
                }
            }
            
            // 开始预牌阶段
            currentGameState = GameState.PreFlop;
            currentPlayerIndex = 0;
            
            // 设置小盲注和大盲注
            SetBlinds();
            
            // 设置当前行动玩家
            SetCurrentPlayerTurn();
            
            string gameCurrencyType = UsePaperMoney ? "宝钞" : "金钞";
            Logger.LogInfo("6人德州扑克游戏已开始，使用" + gameCurrencyType);
        }

        // 设置小盲注和大盲注
        private void SetBlinds()
        {
            // 找到庄家位置
            int dealerIndex = players.FindIndex(p => p.isDealer);
            if (dealerIndex < 0) return;
            
            int smallBlindIndex = (dealerIndex + 1) % players.Count;
            while (players[smallBlindIndex].isHuman && smallBlindIndex != dealerIndex)
            {
                smallBlindIndex = (smallBlindIndex + 1) % players.Count;
            }
            
            int bigBlindIndex = (smallBlindIndex + 1) % players.Count;
            while (players[bigBlindIndex].isHuman && bigBlindIndex != dealerIndex && bigBlindIndex != smallBlindIndex)
            {
                bigBlindIndex = (bigBlindIndex + 1) % players.Count;
            }
            
            int smallBlindAmount = minBet;
            int bigBlindAmount = minBet * 2;
            
            // 小盲注
            if (!players[smallBlindIndex].isHuman && players[smallBlindIndex].chips >= smallBlindAmount)
            {
                players[smallBlindIndex].isSmallBlind = true;
                players[smallBlindIndex].Bet(smallBlindAmount);
                pot += smallBlindAmount;
                string currencyType = UsePaperMoney ? "宝钞" : "金钞";
                Logger.LogInfo(players[smallBlindIndex].name + " 下小盲注: " + smallBlindAmount + " " + currencyType);
            }
            
            // 大盲注
            if (!players[bigBlindIndex].isHuman && players[bigBlindIndex].chips >= bigBlindAmount)
            {
                players[bigBlindIndex].isBigBlind = true;
                players[bigBlindIndex].Bet(bigBlindAmount);
                pot += bigBlindAmount;
                string currencyType = UsePaperMoney ? "宝钞" : "金钞";
                Logger.LogInfo(players[bigBlindIndex].name + " 下大盲注: " + bigBlindAmount + " " + currencyType);
            }
        }

        // 设置当前行动玩家
        private void SetCurrentPlayerTurn()
        {
            // 清除所有玩家的行动标记
            foreach (var player in players)
            {
                player.isTurn = false;
            }
            
            // 找到下一个需要行动的玩家
            // 对于翻牌前，从大盲注的下一位开始
            if (currentGameState == GameState.PreFlop)
            {
                int bigBlindIndex = players.FindIndex(p => p.isBigBlind);
                if (bigBlindIndex >= 0)
                {
                    currentPlayerIndex = (bigBlindIndex + 1) % players.Count;
                }
                else
                {
                    currentPlayerIndex = 0; // 默认从人类玩家开始
                }
            }
            
            // 确保选择的是未弃牌的玩家
            int startIndex = currentPlayerIndex;
            while (players[currentPlayerIndex].isFolded)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                if (currentPlayerIndex == startIndex) break; // 所有玩家都已弃牌
            }
            
            // 设置当前行动玩家
            players[currentPlayerIndex].isTurn = true;
        }

        // 看牌
        private void Check()
        {    
            Logger.LogInfo("你看牌了");
            PlaySound("Check");
            
            // 进入下一个玩家
            NextPlayerTurn();
        }

        // 下注
        private void Bet(int amount)
        {
            if (humanPlayer.chips >= amount)
            {
                humanPlayer.Bet(amount);
                pot += amount;
                Logger.LogInfo("你下注了 " + amount + " 筹码");
                PlaySound("Bet");
                
                // 重置AI行动顺序，让所有未弃牌的AI有机会回应
                ResetBettingRound();
            }
            else
            {
                Logger.LogInfo("你的筹码不足");
            }
        }

        // 跟注
        private void Call()
        {
            // 计算需要跟注的金额
            int highestBet = GetHighestBet();
            int amountToCall = highestBet - humanPlayer.currentBet;
            
            if (humanPlayer.chips >= amountToCall)
            {
                humanPlayer.Bet(amountToCall);
                pot += amountToCall;
                Logger.LogInfo("你跟注了 " + amountToCall + " 筹码");
                PlaySound("Call");
                
                // 进入下一个玩家
                NextPlayerTurn();
            }
            else
            {
                Logger.LogInfo("你的筹码不足");
            }
        }

        // 弃牌
        private void Fold()
        {
            humanPlayer.isFolded = true;
            Logger.LogInfo("你弃牌了");
            PlaySound("Fold");
            
            // 检查是否只剩下一个玩家
            int activePlayers = GetActivePlayerCount();
            
            if (activePlayers <= 1)
            {
                // 只剩一个玩家，直接进入摊牌阶段
                DetermineWinner();
            }
            else
            {
                // 继续游戏，让AI继续行动
                NextPlayerTurn();
            }
        }

        // 获取当前最高下注额
        private int GetHighestBet()
        {
            int highestBet = 0;
            foreach (var player in players)
            {
                if (!player.isFolded && player.currentBet > highestBet)
                {
                    highestBet = player.currentBet;
                }
            }
            return highestBet;
        }

        // 获取未弃牌的玩家数量
        private int GetActivePlayerCount()
        {
            int count = 0;
            foreach (var player in players)
            {
                if (!player.isFolded)
                {
                    count++;
                }
            }
            return count;
        }

        // 进入下一个玩家的回合
        private void NextPlayerTurn()
        {
            // 清除当前玩家的行动标记
            players[currentPlayerIndex].isTurn = false;
            
            // 移动到下一个玩家
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            
            // 检查是否所有玩家都已行动且下注额一致
            if (AllPlayersActed() && AllBetsEqual())
            {
                ProceedToNextStage();
                return;
            }
            
            // 跳过已弃牌的玩家
            int startIndex = currentPlayerIndex;
            while (players[currentPlayerIndex].isFolded)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                if (currentPlayerIndex == startIndex) break; // 所有玩家都已弃牌
            }
            
            // 设置当前行动玩家
            players[currentPlayerIndex].isTurn = true;
            
            // 如果是AI玩家，执行AI行动
            if (!players[currentPlayerIndex].isHuman)
            {
                // 延迟执行AI行动，让玩家有时间看到
                StartCoroutine(AIActionWithDelay(currentPlayerIndex));
            }
        }

        // 协程：延迟执行AI行动
        private IEnumerator AIActionWithDelay(int aiIndex)
        {
            // 短暂延迟，模拟思考时间
            yield return new WaitForSeconds(1f);
            
            AIAction(aiIndex);
        }

        // AI行动
        private void AIAction(int aiIndex)
        {
            if (aiIndex >= players.Count) return;
            
            Player ai = players[aiIndex];
            if (ai.isFolded) return;
            
            // 计算AI的手牌强度
            float handStrength = EvaluateAIHandStrength(ai);
            
            // 获取当前最高下注
            int highestBet = GetHighestBet();
            int amountToCall = highestBet - ai.currentBet;
            
            // AI决策逻辑
            System.Random rng = new System.Random();
            float randomFactor = (float)rng.NextDouble();
            
            // 简单的AI策略：根据手牌强度和随机因素决定行动
            if (handStrength > 0.8f) // 强牌
            {
                if (randomFactor < 0.7f)
                {
                    // 70%的概率加注
                    int raiseAmount = amountToCall + rng.Next(10, 50);
                    if (ai.chips >= raiseAmount)
                    {
                        ai.Bet(raiseAmount);
                        pot += raiseAmount;
                        Logger.LogInfo(ai.name + " 加注到 " + raiseAmount + " 筹码");
                        PlaySound("Bet");
                        ResetBettingRound();
                        return;
                    }
                }
                
                // 否则跟注
                if (ai.chips >= amountToCall)
                {
                    ai.Bet(amountToCall);
                    pot += amountToCall;
                    Logger.LogInfo(ai.name + " 跟注了 " + amountToCall + " 筹码");
                    PlaySound("Call");
                }
            }
            else if (handStrength > 0.4f) // 中等牌力
            {
                if (amountToCall == 0 || randomFactor < 0.5f)
                {
                    // 如果不需要跟注或50%的概率，看牌或跟注
                    if (amountToCall == 0)
                    {
                        Logger.LogInfo(ai.name + " 看牌");
                        PlaySound("Check");
                    }
                    else if (ai.chips >= amountToCall)
                    {
                        ai.Bet(amountToCall);
                        pot += amountToCall;
                        Logger.LogInfo(ai.name + " 跟注了 " + amountToCall + " 筹码");
                        PlaySound("Call");
                    }
                }
                else
                {
                    // 否则弃牌
                    ai.isFolded = true;
                    Logger.LogInfo(ai.name + " 弃牌了");
                    PlaySound("Fold");
                }
            }
            else // 弱牌
            {
                if (amountToCall == 0 && randomFactor < 0.3f)
                {
                    // 如果不需要跟注且30%的概率，看牌
                    Logger.LogInfo(ai.name + " 看牌");
                    PlaySound("Check");
                }
                else if (amountToCall > 0 && randomFactor < 0.1f && ai.chips >= amountToCall)
                {
                    // 10%的概率诈唬跟注
                    ai.Bet(amountToCall);
                    pot += amountToCall;
                    Logger.LogInfo(ai.name + " 跟注了 " + amountToCall + " 筹码");
                    PlaySound("Call");
                }
                else
                {
                    // 否则弃牌
                    ai.isFolded = true;
                    Logger.LogInfo(ai.name + " 弃牌了");
                    PlaySound("Fold");
                }
            }
            
            // 进入下一个玩家
            NextPlayerTurn();
        }

        // 评估AI手牌强度
        private float EvaluateAIHandStrength(Player ai)
        {
            // 创建临时手牌（玩家手牌+公共牌）
            List<Card> allCards = new List<Card>();
            allCards.AddRange(ai.handCards);
            allCards.AddRange(communityCards.Where(c => c.isVisible));
            
            // 如果公共牌不足3张，返回一个随机值
            if (communityCards.Count < 3)
            {
                // 只根据手牌评估
                float handValue = EvaluatePocketPair(ai.handCards);
                return handValue;
            }
            
            // 评估完整手牌强度
            HandRank rank = EvaluateHand(allCards);
            return (float)rank / (float)HandRank.RoyalFlush;
        }

        // 评估手牌对子强度（翻牌前使用）
        private float EvaluatePocketPair(List<Card> hand)
        {
            if (hand.Count < 2) return 0f;
            
            // 对子
            if (hand[0].rank == hand[1].rank)
            {
                // 计算对子强度
                int rankValue = (int)hand[0].rank;
                return Mathf.Clamp01((rankValue - 2) / 12f); // 2到Ace的范围映射到0-1
            }
            
            // 同花
            if (hand[0].suit == hand[1].suit)
            {
                // 计算高牌和牌型强度
                int highCardValue = Mathf.Max((int)hand[0].rank, (int)hand[1].rank);
                int lowCardValue = Mathf.Min((int)hand[0].rank, (int)hand[1].rank);
                
                // 连牌加成
                if (highCardValue - lowCardValue == 1) // 连续
                {
                    return Mathf.Clamp01((highCardValue - 2 + 0.5f) / 12f);
                }
                return Mathf.Clamp01((highCardValue - 2) / 12f * 0.8f + 0.1f);
            }
            
            // 高牌
            int highValue = Mathf.Max((int)hand[0].rank, (int)hand[1].rank);
            return Mathf.Clamp01((highValue - 2) / 12f * 0.7f);
        }

        // 检查所有玩家是否都已行动
        private bool AllPlayersActed()
        {
            // 对于当前下注轮次，检查每个未弃牌的玩家是否下注额等于最高下注额或全押
            int highestBet = GetHighestBet();
            
            foreach (var player in players)
            {
                if (!player.isFolded && player.currentBet < highestBet)
                {
                    return false;
                }
            }
            
            return true;
        }

        // 检查所有下注是否相等
        private bool AllBetsEqual()
        {
            int? firstBet = null;
            
            foreach (var player in players)
            {
                if (!player.isFolded)
                {
                    if (firstBet == null)
                    {
                        firstBet = player.currentBet;
                    }
                    else if (player.currentBet != firstBet)
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }

        // 重置下注轮次
        private void ResetBettingRound()
        {
            // 重新开始下注轮次，从当前玩家之后开始
            int currentPlayer = currentPlayerIndex;
            
            // 清除所有玩家的行动标记
            foreach (var player in players)
            {
                player.isTurn = false;
            }
            
            // 设置下一个玩家为当前行动玩家
            currentPlayerIndex = (currentPlayer + 1) % players.Count;
            
            // 跳过已弃牌的玩家
            int startIndex = currentPlayerIndex;
            while (players[currentPlayerIndex].isFolded)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                if (currentPlayerIndex == startIndex) break; // 所有玩家都已弃牌
            }
            
            // 设置当前行动玩家
            players[currentPlayerIndex].isTurn = true;
            
            // 如果是AI玩家，执行AI行动
            if (!players[currentPlayerIndex].isHuman)
            {
                StartCoroutine(AIActionWithDelay(currentPlayerIndex));
            }
        }

        // 进入下一阶段
        private void ProceedToNextStage()
        {
            switch (currentGameState)
            {
                case GameState.PreFlop:
                    // 发三张公共牌（翻牌）
                    for (int i = 0; i < 3; i++)
                    {
                        communityCards.Add(deck[0]);
                        deck.RemoveAt(0);
                    }
                    currentGameState = GameState.Flop;
                    currentBettingRound++;
                    Logger.LogInfo("进入翻牌阶段");
                    PlaySound("Flop");
                    break;
                case GameState.Flop:
                    // 发第四张公共牌（转牌）
                    communityCards.Add(deck[0]);
                    deck.RemoveAt(0);
                    currentGameState = GameState.Turn;
                    currentBettingRound++;
                    Logger.LogInfo("进入转牌阶段");
                    PlaySound("Turn");
                    break;
                case GameState.Turn:
                    // 发第五张公共牌（河牌）
                    communityCards.Add(deck[0]);
                    deck.RemoveAt(0);
                    currentGameState = GameState.River;
                    currentBettingRound++;
                    Logger.LogInfo("进入河牌阶段");
                    PlaySound("River");
                    break;
                case GameState.River:
                    // 进入摊牌阶段，显示所有AI的牌并判定胜负
                    currentGameState = GameState.Showdown;
                    Logger.LogInfo("进入摊牌阶段");
                    PlaySound("Showdown");
                    
                    // 显示所有AI的牌
                    foreach (var player in players)
                    {
                        if (!player.isHuman && !player.isFolded)
                        {
                            foreach (var card in player.handCards)
                            {
                                card.isVisible = true;
                            }
                        }
                    }
                    
                    // 延迟判定胜负，让玩家有时间看到所有牌
                    StartCoroutine(DetermineWinnerWithDelay());
                    return;
            }
            
            // 重置下注状态
            ResetBettingState();
        }

        // 协程：延迟判定胜负
        private IEnumerator DetermineWinnerWithDelay()
        {
            // 延迟几秒钟，让玩家有时间查看所有牌
            yield return new WaitForSeconds(3f);
            
            DetermineWinner();
        }

        // 重置下注状态
        private void ResetBettingState()
        {
            // 清除所有玩家的行动标记
            foreach (var player in players)
            {
                player.isTurn = false;
            }
            
            // 设置当前玩家为庄家的下一位
            int dealerIndex = players.FindIndex(p => p.isDealer);
            if (dealerIndex >= 0)
            {
                currentPlayerIndex = (dealerIndex + 1) % players.Count;
            }
            else
            {
                currentPlayerIndex = 0;
            }
            
            // 跳过已弃牌的玩家
            int startIndex = currentPlayerIndex;
            while (players[currentPlayerIndex].isFolded)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                if (currentPlayerIndex == startIndex) break; // 所有玩家都已弃牌
            }
            
            // 设置当前行动玩家
            players[currentPlayerIndex].isTurn = true;
            
            // 如果是AI玩家，执行AI行动
            if (!players[currentPlayerIndex].isHuman)
            {
                StartCoroutine(AIActionWithDelay(currentPlayerIndex));
            }
        }

        // 判定胜负
        private void DetermineWinner()
        {
            // 收集所有未弃牌的玩家
            List<Player> activePlayers = players.Where(p => !p.isFolded).ToList();
            
            if (activePlayers.Count == 0)
            {
                // 没有活跃玩家，游戏结束
                gameOverMessage = "游戏结束，无获胜者";
                showGameOverWindow = true;
                return;
            }
            
            // 评估每个玩家的手牌
            List<(Player player, HandRank rank, List<Rank> values)> evaluatedHands = new List<(Player, HandRank, List<Rank>)>();
            
            foreach (var player in activePlayers)
            {
                // 创建临时手牌（玩家手牌+公共牌）
                List<Card> allCards = new List<Card>();
                allCards.AddRange(player.handCards);
                allCards.AddRange(communityCards);
                
                // 评估手牌
                HandRank rank = EvaluateHand(allCards);
                List<Rank> values = GetHandValues(allCards);
                
                evaluatedHands.Add((player, rank, values));
            }
            
            // 排序找出获胜者
            evaluatedHands.Sort((a, b) =>
            {
                // 先比较牌型
                int rankCompare = b.rank.CompareTo(a.rank);
                if (rankCompare != 0)
                    return rankCompare;
                
                // 牌型相同，比较具体牌值
                for (int i = 0; i < Math.Min(a.values.Count, b.values.Count); i++)
                {
                    int valueCompare = b.values[i].CompareTo(a.values[i]);
                    if (valueCompare != 0)
                        return valueCompare;
                }
                
                return 0;
            });
            
            // 获取获胜者
            List<Player> winners = new List<Player>();
            HandRank bestRank = evaluatedHands[0].rank;
            List<Rank> bestValues = evaluatedHands[0].values;
            
            foreach (var hand in evaluatedHands)
            {
                if (hand.rank == bestRank && CompareValues(hand.values, bestValues) == 0)
                {
                    winners.Add(hand.player);
                }
            }
            
            // 分配奖金
            int prizePerWinner = pot / winners.Count;
            
            string currencyType = UsePaperMoney ? "宝钞" : "金钞";
            string winnerNames = string.Join("、", winners.Select(w => w.name));
            
            if (winners.Contains(humanPlayer))
            {
                humanPlayer.chips += prizePerWinner;
                gameOverMessage = "恭喜！你赢了！\n赢得了 " + prizePerWinner + " " + currencyType + "\n当前剩余：" + humanPlayer.chips + " " + currencyType;
            }
            else
            {
                foreach (var winner in winners)
                {
                    winner.chips += prizePerWinner;
                }
                gameOverMessage = winnerNames + " 赢了\n赢得了 " + prizePerWinner + " " + currencyType + "\n你的剩余：" + humanPlayer.chips + " " + currencyType;
            }
            
            Logger.LogInfo(gameOverMessage);
            
            // 显示获胜手牌类型
            string handRankText = GetHandRankText(bestRank);
            gameOverMessage += "\n最佳牌型：" + handRankText;
            
            // 播放获胜音效
            if (winners.Contains(humanPlayer))
                PlaySound("Win");
            else
                PlaySound("Lose");
            
            // 显示游戏结束窗口
            showGameOverWindow = true;
        }

        // 比较牌值列表
        private int CompareValues(List<Rank> values1, List<Rank> values2)
        {
            for (int i = 0; i < Math.Min(values1.Count, values2.Count); i++)
            {
                int compare = values1[i].CompareTo(values2[i]);
                if (compare != 0)
                    return compare;
            }
            return 0;
        }

        // 获取牌型文本描述
        private string GetHandRankText(HandRank rank)
        {
            switch (rank)
            {
                case HandRank.RoyalFlush:
                    return "皇家同花顺";
                case HandRank.StraightFlush:
                    return "同花顺";
                case HandRank.FourOfAKind:
                    return "四条";
                case HandRank.FullHouse:
                    return "葫芦";
                case HandRank.Flush:
                    return "同花";
                case HandRank.Straight:
                    return "顺子";
                case HandRank.ThreeOfAKind:
                    return "三条";
                case HandRank.TwoPair:
                    return "两对";
                case HandRank.OnePair:
                    return "一对";
                default:
                    return "高牌";
            }
        }

        // 绘制游戏结束窗口
        private void DrawGameOverWindow(int windowID)
        {
            // 确保纹理已初始化
            InitializeGameOverTextures();

            // 窗口拖动区域
            GUI.DragWindow(new Rect(0, 0, gameOverWindowRect.width, 60));

            // 设置字体大小
            int defaultFontSize = Mathf.RoundToInt(14 * scaleFactor);
            GUI.skin.label.fontSize = defaultFontSize;
            GUI.skin.button.fontSize = defaultFontSize;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;

            // 绘制游戏结束窗口背景
            GUI.DrawTexture(new Rect(0, 0, gameOverWindowRect.width, gameOverWindowRect.height), gameOverBackground);

            GUILayout.BeginVertical();
            GUILayout.Space(70f * scaleFactor); // 标题栏高度

            // 游戏结束标题 - 添加发光动画效果
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawGameOverTitle();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(30f * scaleFactor);

            // 游戏结果消息 - 美化样式
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawGameOverMessage();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(30f * scaleFactor);
            
            // 重复游戏逻辑
            if (!hasCalledRepeatLogic)
            {
                hasCalledRepeatLogic = true;
                
                // 直接调用静态方法，传入必要参数
                B_RepeatTheGame.HandleGameEnd(
                    humanPlayer.chips, 
                    this, // 游戏实例
                    () => { 
                        // 继续游戏回调
                        showGameOverWindow = false;
                        StartNewGame();
                        hasCalledRepeatLogic = false;
                        PlaySound("Continue");
                    }, 
                    () => { 
                        // 返回主窗口回调
                        ReturnToMainMenu();
                        PlaySound("Return");
                    });
            }

            // 玩家筹码检查 - 添加视觉提示
            if (humanPlayer.chips <= 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                DrawBankruptcyWarning();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(20f * scaleFactor);
            }

            // 按钮区域
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // 继续游戏按钮 - 美化样式
            if (humanPlayer.chips > 0)
            {
                DrawContinueButton();
                GUILayout.Space(20f * scaleFactor);
            }

            // 返回主窗口按钮 - 美化样式
            DrawReturnToMainMenuButton();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20f * scaleFactor);

            GUILayout.EndVertical();

            // 辅助方法：初始化游戏结束窗口的纹理
            void InitializeGameOverTextures()
            {
                if (gameOverBackground == null)
                    gameOverBackground = MakeGradientTex(100, 100, new Color(0.1f, 0.15f, 0.05f, 0.95f), new Color(0.05f, 0.08f, 0.02f, 0.95f));
                
                if (gameOverTitleGradient == null)
                    gameOverTitleGradient = MakeGradientTex(100, 100, new Color(0.5f, 0.3f, 0.05f, 1f), new Color(0.3f, 0.15f, 0.02f, 1f));
                
                if (gameOverMessageBackground == null)
                    gameOverMessageBackground = MakeGradientTex(100, 100, new Color(0.2f, 0.25f, 0.1f, 0.7f), new Color(0.1f, 0.15f, 0.05f, 0.7f));
                
                if (continueButtonNormal == null)
                    continueButtonNormal = MakeGradientTex(100, 100, new Color(0.3f, 0.7f, 0.3f, 0.9f), new Color(0.15f, 0.5f, 0.15f, 0.9f));
                
                if (continueButtonHover == null)
                    continueButtonHover = MakeGradientTex(100, 100, new Color(0.4f, 0.8f, 0.4f, 0.9f), new Color(0.25f, 0.6f, 0.25f, 0.9f));
                
                if (returnButtonNormal == null)
                    returnButtonNormal = MakeGradientTex(100, 100, new Color(0.5f, 0.2f, 0.2f, 0.9f), new Color(0.35f, 0.1f, 0.1f, 0.9f));
                
                if (returnButtonHover == null)
                    returnButtonHover = MakeGradientTex(100, 100, new Color(0.6f, 0.3f, 0.3f, 0.9f), new Color(0.45f, 0.2f, 0.2f, 0.9f));
            }

            // 辅助方法：绘制游戏结束标题
            void DrawGameOverTitle()
            {
                // 创建发光动画效果
                float glowPulse = 0.8f + 0.2f * Mathf.Sin(animationTime * 2f);
                
                // 绘制标题渐变背景
                GUI.color = new Color(1f, 0.9f, 0.5f, glowPulse);
                GUI.DrawTexture(new Rect(gameOverWindowRect.width / 2 - 150 * scaleFactor, 60, 300 * scaleFactor, 60 * scaleFactor), gameOverTitleGradient);
                
                // 绘制标题文字
                GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
                titleStyle.fontSize = Mathf.RoundToInt(28 * scaleFactor);
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.alignment = TextAnchor.MiddleCenter;
                titleStyle.normal.textColor = new Color(1f, 0.95f, 0.5f);
                
                GUI.Label(new Rect(gameOverWindowRect.width / 2 - 150 * scaleFactor, 60, 300 * scaleFactor, 60 * scaleFactor), "游戏结束", titleStyle);
                
                GUI.color = Color.white;
            }

            // 辅助方法：绘制游戏结果消息
            void DrawGameOverMessage()
            {
                // 绘制消息背景
                GUI.DrawTexture(new Rect(gameOverWindowRect.width / 2 - 300 * scaleFactor, 150, 600 * scaleFactor, 120 * scaleFactor), gameOverMessageBackground);
                
                // 绘制消息文本
                GUIStyle messageStyle = new GUIStyle(GUI.skin.label);
                messageStyle.fontSize = Mathf.RoundToInt(18 * scaleFactor);
                messageStyle.wordWrap = true;
                messageStyle.alignment = TextAnchor.MiddleCenter;
                messageStyle.normal.textColor = new Color(1f, 0.95f, 0.85f);
                messageStyle.richText = true;
                
                // 根据消息内容添加颜色强调
                string coloredMessage = gameOverMessage;
                if (coloredMessage.Contains("获胜"))
                {
                    coloredMessage = coloredMessage.Replace("获胜", "<color=#FFFF00>获胜</color>");
                }
                else if (coloredMessage.Contains("赢了"))
                {
                    coloredMessage = coloredMessage.Replace("赢了", "<color=#FFFF00>赢了</color>");
                }
                else if (coloredMessage.Contains("失败"))
                {
                    coloredMessage = coloredMessage.Replace("失败", "<color=#FF6666>失败</color>");
                }
                
                GUI.Label(new Rect(gameOverWindowRect.width / 2 - 280 * scaleFactor, 160, 560 * scaleFactor, 100 * scaleFactor), coloredMessage, messageStyle);
            }

            // 辅助方法：绘制破产警告
            void DrawBankruptcyWarning()
            {
                // 创建警告背景
                Texture2D warningBackground = MakeGradientTex(100, 100, new Color(0.5f, 0.1f, 0.1f, 0.8f), new Color(0.3f, 0.05f, 0.05f, 0.8f));
                GUI.DrawTexture(new Rect(gameOverWindowRect.width / 2 - 200 * scaleFactor, 280, 400 * scaleFactor, 60 * scaleFactor), warningBackground);
                
                // 创建闪烁效果
                float blinkRate = 2f;
                float blinkAlpha = 0.7f + 0.3f * Mathf.Abs(Mathf.Sin(animationTime * blinkRate));
                
                // 绘制警告文字
                GUIStyle warningStyle = new GUIStyle(GUI.skin.label);
                warningStyle.fontSize = Mathf.RoundToInt(20 * scaleFactor);
                warningStyle.fontStyle = FontStyle.Bold;
                warningStyle.alignment = TextAnchor.MiddleCenter;
                warningStyle.normal.textColor = new Color(1f, blinkAlpha * 0.8f, blinkAlpha * 0.8f);
                
                GUI.Label(new Rect(gameOverWindowRect.width / 2 - 200 * scaleFactor, 280, 400 * scaleFactor, 60 * scaleFactor), "你已经没有筹码了！", warningStyle);
            }

            // 辅助方法：绘制继续游戏按钮
            void DrawContinueButton()
            {
                // 创建按钮样式
                GUIStyle continueButtonStyle = new GUIStyle(GUI.skin.button);
                continueButtonStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
                continueButtonStyle.normal.textColor = Color.white;
                continueButtonStyle.hover.textColor = Color.white;
                continueButtonStyle.active.textColor = Color.white;
                continueButtonStyle.fontStyle = FontStyle.Bold;
                continueButtonStyle.normal.background = continueButtonNormal;
                continueButtonStyle.hover.background = continueButtonHover;
                
                // 绘制按钮
                if (GUILayout.Button("继续游戏", continueButtonStyle, GUILayout.Width(150f * scaleFactor), GUILayout.Height(45f * scaleFactor)))
                {
                    showGameOverWindow = false;
                    // 保留当前筹码，重新开始游戏
                    StartNewGame();
                    PlaySound("Continue");
                }
            }

            // 辅助方法：绘制返回主窗口按钮
            void DrawReturnToMainMenuButton()
            {
                // 创建按钮样式
                GUIStyle returnButtonStyle = new GUIStyle(GUI.skin.button);
                returnButtonStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
                returnButtonStyle.normal.textColor = Color.white;
                returnButtonStyle.hover.textColor = Color.white;
                returnButtonStyle.active.textColor = Color.white;
                returnButtonStyle.fontStyle = FontStyle.Bold;
                returnButtonStyle.normal.background = returnButtonNormal;
                returnButtonStyle.hover.background = returnButtonHover;
                
                // 绘制按钮
                if (GUILayout.Button("返回主窗口", returnButtonStyle, GUILayout.Width(150f * scaleFactor), GUILayout.Height(45f * scaleFactor)))
                {
                    ReturnToMainMenu();
                    PlaySound("Return");
                }
            }
        }

        // 返回主窗口并保存结余货币
        private void ReturnToMainMenu()
        {
            // 保存结余货币
            RemainingMoney = humanPlayer.chips;
            
            // 关闭德州扑克窗口
            showMenu = false;
            showGameOverWindow = false;
            blockGameInput = false;
            
            Logger.LogInfo("已返回主窗口，结余" + (UsePaperMoney ? "宝钞" : "金钞") + "：" + RemainingMoney);
            
            // 尝试打开MoreGambles主窗口
            A_MoreGambles moreGambles = FindObjectOfType<A_MoreGambles>();
            if (moreGambles != null)
            {
                moreGambles.ReturnFromGame(UsePaperMoney, RemainingMoney);
            }
        }

        // 播放音效（占位符方法）
        private void PlaySound(string soundType)
        {
            if (soundEnabled)
            {
                // 这里应该是实际播放音效的代码
                // 目前仅作为占位符
                switch (soundType)
                {
                    case "Check":
                        // 播放看牌音效
                        break;
                    case "Bet":
                        // 播放下注音效
                        break;
                    case "Call":
                        // 播放跟注音效
                        break;
                    case "Fold":
                        // 播放弃牌音效
                        break;
                    case "Flop":
                    case "Turn":
                    case "River":
                        // 播放发牌音效
                        break;
                    case "Showdown":
                        // 播放摊牌音效
                        break;
                    case "Win":
                        // 播放获胜音效
                        break;
                    case "Lose":
                        // 播放失败音效
                        break;
                }
            }
        }

        // 检查是否可以开始游戏
        public bool CheckCanPlay()
        {
            return InitialChips > 0;
        }

        // 开始游戏
        public void StartGame()
        {
            UpdateResolutionSettings();
            showMenu = true;
            blockGameInput = true;
            Logger.LogInfo("德州扑克游戏已启动，版本：" + VERSION);
        }

        // 重置游戏状态
        public void ResetGame()
        {
            currentGameState = GameState.Waiting;
            showMenu = false;
            showGameOverWindow = false;
            blockGameInput = false;
            InitializePlayers();
            humanPlayer.chips = InitialChips;
        }
    }
}

// 牌型枚举
public enum HandRank
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
    RoyalFlush
}

// 花色枚举
public enum Suit
{
    Club,      // 梅花
    Diamond,   // 方块
    Heart,     // 红桃
    Spade      // 黑桃
}

// 点数枚举
public enum Rank
{
    Two = 2,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
}

// 卡牌类
public class Card
{
    public Suit suit;
    public Rank rank;
    public bool isVisible;

    public Card(Suit s, Rank r, bool visible)
    {
        suit = s;
        rank = r;
        isVisible = visible;
    }

    // 获取卡牌字符串表示
    public string GetCardString()
    {
        if (!isVisible) return "??";
        string rankStr = rank.ToString();
        if (rankStr.Length > 2) rankStr = rankStr.Substring(0, 1);
        string suitStr = suit.ToString().Substring(0, 1);
        return rankStr + suitStr;
    }

    // 获取卡牌颜色
    public Color GetSuitColor()
    {
        if (suit == Suit.Heart || suit == Suit.Diamond)
            return Color.red;
        else
            return Color.black;
    }
}

// 玩家类
public class Player
{
    public string name;
    public List<Card> handCards = new List<Card>();
    public int chips;
    public int currentBet;
    public bool isFolded = false;
    public bool isDealer = false;
    public bool isSmallBlind = false;
    public bool isBigBlind = false;
    public bool isHuman = false;
    public bool isTurn = false;

    public Player(string playerName, int startingChips, bool human = false)
    {
        name = playerName;
        chips = startingChips;
        isHuman = human;
    }

    // 重置玩家状态
    public void ResetState()
    {
        handCards.Clear();
        currentBet = 0;
        isFolded = false;
        isSmallBlind = false;
        isBigBlind = false;
        isTurn = false;
    }

    // 下注
    public bool Bet(int amount)
    {
        if (amount <= 0 || amount > chips)
            return false;

        chips -= amount;
        currentBet += amount;
        return true;
    }
}
