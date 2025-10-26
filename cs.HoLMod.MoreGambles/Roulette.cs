using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;

namespace cs.HoLMod.MoreGambles
{
    public class Roulette : BaseUnityPlugin
    {
        // 窗口设置
        private Rect windowRect = new Rect(20, 20, 800, 600);
        public bool showMenu = false;
        public bool blockGameInput = false;
        private float scaleFactor = 1f;
        private const string CURRENT_VERSION = "1.0.0";
        
        // 货币系统相关
        public bool UsePaperMoney { get; set; } = true; // true=宝钞, false=金钞
        public int InitialChips { get; set; } = 1000;
        public int RemainingMoney { get; set; } = 0;
        
        // 游戏状态枚举
        private enum GameState { NotStarted, Spinning, ShowingResult, GameOver }
        private GameState currentState = GameState.NotStarted;
        
        // 玩家信息
        private int playerChips = 1000;
        private int currentBet = 0;
        
        // 轮盘相关
        private List<NumberColor> rouletteNumbers = new List<NumberColor>();
        private float spinSpeed = 0f;
        private float initialSpinSpeed = 0f;
        private float currentRotation = 0f;
        private float decelerationRate = 0.1f;
        private int winningNumber = 0;
        private string winningColor = "";
        private float resultDisplayTimer = 0f;
        private bool bankruptcyWarning = false; // 破产警告标志
        private int bankruptciesCount = 0; // 破产次数统计
        private float bankruptcyWarningTime = 0f; // 破产警告时间戳
        
        // 重复游戏逻辑相关
        private bool hasCalledRepeatLogic = false;
        
        // 下注类型
        private enum BetType { Straight, Split, Street, Corner, Line, Dozen, Column, Even, Odd, Red, Black, Low, High }
        private Dictionary<BetType, int> betAmounts = new Dictionary<BetType, int>();
        private Dictionary<BetType, int> straightBets = new Dictionary<BetType, int>(); // 存储具体数字的下注
        
        // 轮盘数字和颜色结构
        private struct NumberColor
        {
            public int number;
            public string color;
            
            public NumberColor(int num, string col)
            {
                number = num;
                color = col;
            }
        }

        private void Awake()
        {
            InitializeGame();
        }

        // 公共初始化方法，供外部调用
        public void InitializeGame()
        {
            InitializeRouletteNumbers();
            InitializeBetAmounts();
            UpdateResolutionSettings();
            
            // 设置玩家初始筹码
            playerChips = InitialChips;
            currentBet = 0;
            
            // 初始化游戏状态
            currentState = GameState.NotStarted;
            currentRotation = 0f;
            spinSpeed = 0f;
            initialSpinSpeed = 0f;
            decelerationRate = 0f;
            winningNumber = 0;
            winningColor = LanguageManager.Instance.GetText("绿色");
            resultDisplayTimer = 0f;
            
            // 破产相关初始化
            bankruptcyWarning = false;
            bankruptcyWarningTime = 0f;
            bankruptciesCount = 0;
            
            Logger.LogInfo(LanguageManager.Instance.GetText("轮盘赌游戏初始化完成"));
        }

        private void InitializeRouletteNumbers()
        {
            // 初始化美式轮盘数字（0-36），包括0和00
            // 实际游戏中可能需要调整颜色和数字分布
            rouletteNumbers.Add(new NumberColor(0, LanguageManager.GetText("绿色")));
            
            // 1-36的数字和颜色
            string[] colors = new string[] {
                LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"),
                LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"),
                LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"),
                LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色"), LanguageManager.Instance.GetText("黑色"), LanguageManager.Instance.GetText("红色")
            };
            
            for (int i = 1; i <= 36; i++)
            {
                rouletteNumbers.Add(new NumberColor(i, colors[i-1]));
            }
        }
        
        // 检查玩家是否破产
        private void CheckPlayerBankruptcy()
        {
            if (playerChips <= 0)
            {
                bankruptciesCount++;
                currentState = GameState.GameOver;
                Logger.LogInfo(LanguageManager.Instance.GetText("玩家破产！"));
                
                // 可以添加破产后的处理逻辑，如给予一些救济金
                if (bankruptciesCount <= 3) // 给予3次救济金
                {
                    int reliefAmount = 50; // 救济金金额
                    
                    // 根据破产次数调整救济金金额
                    if (bankruptciesCount >= 3)
                    {
                        reliefAmount = 30; // 第三次及以后破产，救济金减少
                    }
                    else if (bankruptciesCount >= 5)
                    {
                        reliefAmount = 20; // 第五次及以后破产，救济金进一步减少
                    }
                    
                    playerChips = reliefAmount; // 给予筹码作为救济金
                    bankruptcyWarning = true;
                    bankruptcyWarningTime = Time.time; // 设置警告时间戳
                    Logger.LogInfo(LanguageManager.Instance.GetFormattedText("获得救济金：{0}个筹码", reliefAmount));
                    currentState = GameState.NotStarted;
                }
            }
        }

        private void InitializeBetAmounts()
        {
            // 初始化所有下注类型的金额为0
            foreach (BetType betType in System.Enum.GetValues(typeof(BetType)))
            {
                betAmounts[betType] = 0;
            }
            straightBets.Clear();
        }

        private void Update()
        {
            // 按F5键切换窗口显示
            if (Input.GetKeyDown(KeyCode.F5))
            {
                UpdateResolutionSettings();
                showMenu = !showMenu;
                blockGameInput = showMenu;
                Logger.LogInfo(showMenu ? LanguageManager.Instance.GetText("轮盘赌窗口已打开") : LanguageManager.Instance.GetText("轮盘赌窗口已关闭"));
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

            // 处理轮盘旋转
            if (currentState == GameState.Spinning)
            {
                // 更加真实的旋转物理效果
                // 添加非线性减速效果，模拟真实轮盘
                if (spinSpeed > 0)
                {
                    float decelerationFactor = 1.0f - (spinSpeed / initialSpinSpeed);
                    float adjustedDeceleration = decelerationRate * (1.0f + 2.0f * decelerationFactor);
                    spinSpeed -= adjustedDeceleration * Time.deltaTime;
                    spinSpeed = Mathf.Max(spinSpeed, 0f);
                }
                
                // 应用旋转
                currentRotation += spinSpeed * Time.deltaTime;
                
                if (spinSpeed <= 0)
                {
                    spinSpeed = 0;
                    currentState = GameState.ShowingResult;
                    DetermineWinningNumber();
                    CalculateWinnings();
                    CheckPlayerBankruptcy();
                    // 显示结果一段时间后自动回到待开始状态
                    resultDisplayTimer = Time.time + 3f;
                }
            }
            else if (currentState == GameState.ShowingResult && Time.time > resultDisplayTimer)
            {
                if (!hasCalledRepeatLogic)
                {
                    // 标记已调用重复游戏逻辑
                    hasCalledRepeatLogic = true;
                    
                    // 直接调用静态方法，传入必要参数
                    B_RepeatTheGame.HandleGameEnd(
                        playerChips, 
                        this, // 游戏实例
                        () => 
                        {
                            // 继续游戏回调
                            PrepareForNextGame();
                        }, 
                        ReturnToMainMenu
                    );
                }
            }
            
            // 游戏状态处理
            if (currentState == GameState.GameOver)
            {
                // 可以添加游戏结束的特殊效果或逻辑
            }
            
            // 破产警告计时
            if (bankruptcyWarning)
            {
                if (Time.time > bankruptcyWarningTime + 5f) // 显示5秒后自动关闭
                {
                    bankruptcyWarning = false;
                }
            }
        }

        private void OnGUI()
        { 
            if (!showMenu) return;

            // 保存窗口背景色并设置为半透明
            Color originalBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.1f, 0.15f, 0.3f, 0.98f); // 深蓝色调背景，更现代感

            // 显示一个半透明的背景遮罩
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
            GUI.color = new Color(0, 0, 0, 0.5f); // 更深的遮罩，提升窗口清晰度
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.EndGroup();

            // 应用缩放因子
            Matrix4x4 guiMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleFactor, scaleFactor, 1f));

            // 根据缩放因子调整字体大小和样式
            GUI.skin.window.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            GUI.skin.window.padding = new RectOffset(
                Mathf.RoundToInt(20 * scaleFactor),
                Mathf.RoundToInt(20 * scaleFactor),
                Mathf.RoundToInt(15 * scaleFactor),
                Mathf.RoundToInt(15 * scaleFactor)
            );

            GUI.skin.label.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            GUI.skin.button.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            GUI.skin.textField.fontSize = Mathf.RoundToInt(12 * scaleFactor);

            // 创建主游戏窗口
            windowRect = GUI.Window(0, windowRect, DrawWindow, LanguageManager.Instance.GetText("轮盘赌 - 高级版"), GUI.skin.window);

            // 恢复原始矩阵和背景色
            GUI.matrix = guiMatrix;
            GUI.backgroundColor = originalBackgroundColor;
        }

        private void DrawWindow(int windowID)
        {
            // 允许拖动窗口
            GUI.DragWindow(new Rect(0, 0, windowRect.width, 25));
            
            // 设置字体大小和样式
            int fontSize = Mathf.RoundToInt(14 * scaleFactor);
            GUI.skin.label.fontSize = fontSize;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin.button.fontSize = fontSize;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            GUI.skin.window.fontSize = fontSize;

            // 窗口最小宽度和高度，增大以适应新设计
            windowRect.width = Mathf.Max(windowRect.width, 900f * scaleFactor);
            windowRect.height = Mathf.Max(windowRect.height, 700f * scaleFactor);

            GUILayout.BeginVertical();

            // 标题和版本信息 - 更现代的设计
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("轮盘赌高级版", new GUIStyle(GUI.skin.label) { 
                fontSize = Mathf.RoundToInt(22 * scaleFactor),
                fontStyle = FontStyle.Bold,
                normal = {
                    textColor = new Color(0.9f, 0.7f, 0.2f) // 金色文字
                }
            });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5f * scaleFactor);
            
            // 版本信息
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("版本: 1.0.0", new GUIStyle(GUI.skin.label) { 
                fontSize = Mathf.RoundToInt(12 * scaleFactor),
                normal = {
                    textColor = new Color(0.6f, 0.6f, 0.6f) // 灰色文字
                }
            });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15f * scaleFactor);

            // 游戏信息区域 - 玩家筹码和游戏状态
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 玩家筹码显示 - 更醒目的设计
            string currencyType = UsePaperMoney ? "宝钞" : "金钞";
            GUIStyle chipsStyle = new GUIStyle(GUI.skin.label) {
                fontSize = Mathf.RoundToInt(18 * scaleFactor),
                fontStyle = FontStyle.Bold,
                normal = {
                    textColor = new Color(1f, 0.9f, 0.4f) // 亮金色文字
                }
            };
            GUILayout.Label("你的" + currencyType + "：", chipsStyle);
            GUILayout.Label(playerChips.ToString(), chipsStyle);
            
            GUILayout.FlexibleSpace();
            
            // 游戏状态显示 - 添加颜色和边框
            string stateText = currentState == GameState.NotStarted ? "等待开始" :
                             currentState == GameState.Spinning ? "旋转中" :
                             currentState == GameState.ShowingResult ? "显示结果" : "游戏结束";
            
            Color stateColor = currentState == GameState.NotStarted ? new Color(0.3f, 0.8f, 0.3f) :
                             currentState == GameState.Spinning ? new Color(0.3f, 0.3f, 0.8f) :
                             currentState == GameState.ShowingResult ? new Color(0.9f, 0.7f, 0.2f) : new Color(0.8f, 0.2f, 0.2f);
            
            GUIStyle stateStyle = new GUIStyle(GUI.skin.label) {
                fontSize = Mathf.RoundToInt(16 * scaleFactor),
                fontStyle = FontStyle.Bold,
                normal = {
                    textColor = stateColor
                },
                padding = new RectOffset(
                    Mathf.RoundToInt(8 * scaleFactor),
                    Mathf.RoundToInt(8 * scaleFactor),
                    Mathf.RoundToInt(4 * scaleFactor),
                    Mathf.RoundToInt(4 * scaleFactor)
                ),
                border = new RectOffset(1, 1, 1, 1)
            };
            stateStyle.normal.background = MakeTex(1, 1, new Color(0.2f, 0.2f, 0.2f, 0.5f));
            
            GUILayout.Label("状态: " + stateText, stateStyle);
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(25f * scaleFactor);

            // 主内容区域 - 轮盘和下注区域并排显示
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 轮盘显示区域 - 增大轮盘尺寸
            GUILayout.BeginVertical();
            GUILayout.Space(10f * scaleFactor);
            DrawRouletteWheel(220f * scaleFactor);
            GUILayout.EndVertical();
            
            GUILayout.Space(40f * scaleFactor);
            
            // 下注区域 - 全新设计
            GUILayout.BeginVertical(GUILayout.Width(550f * scaleFactor));
            // 下注区域标题
            GUILayout.Label("下注区域", new GUIStyle(GUI.skin.label) {
                fontSize = Mathf.RoundToInt(16 * scaleFactor),
                fontStyle = FontStyle.Bold,
                normal = {
                    textColor = new Color(0.9f, 0.7f, 0.2f) // 金色文字
                }
            });
            GUILayout.Space(15f * scaleFactor);
            
            // 主要下注按钮（外围投注） - 改进样式和布局
            GUILayout.Label("外围投注 (基础赔率)", new GUIStyle(GUI.skin.label) {
                fontSize = Mathf.RoundToInt(14 * scaleFactor),
                normal = {
                    textColor = new Color(0.7f, 0.7f, 0.7f)
                }
            });
            GUILayout.Space(10f * scaleFactor);
            
            GUILayout.BeginHorizontal();
            // 红色/黑色按钮
            GUI.backgroundColor = new Color(0.9f, 0.2f, 0.2f); // 鲜红色
            GUIStyle colorButtonStyle = new GUIStyle(GUI.skin.button);
            colorButtonStyle.normal.textColor = Color.white;
            colorButtonStyle.fontStyle = FontStyle.Bold;
            colorButtonStyle.alignment = TextAnchor.MiddleCenter;
            colorButtonStyle.border = new RectOffset(2, 2, 2, 2);
            
            if (GUILayout.Button("红色\n(1:1)", colorButtonStyle, GUILayout.Width(100f * scaleFactor), GUILayout.Height(45f * scaleFactor))) PlaceBet(BetType.Red, 10);
            
            GUILayout.Space(10f * scaleFactor);
            
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f); // 深黑色
            if (GUILayout.Button("黑色\n(1:1)", colorButtonStyle, GUILayout.Width(100f * scaleFactor), GUILayout.Height(45f * scaleFactor))) PlaceBet(BetType.Black, 10);
            
            GUILayout.Space(15f * scaleFactor);
            
            // 偶数/奇数按钮
            GUI.backgroundColor = new Color(0.3f, 0.3f, 0.8f); // 深蓝色
            if (GUILayout.Button("偶数\n(1:1)", colorButtonStyle, GUILayout.Width(100f * scaleFactor), GUILayout.Height(45f * scaleFactor))) PlaceBet(BetType.Even, 10);
            
            GUILayout.Space(10f * scaleFactor);
            
            GUI.backgroundColor = new Color(0.6f, 0.3f, 0.6f); // 紫色
            if (GUILayout.Button("奇数\n(1:1)", colorButtonStyle, GUILayout.Width(100f * scaleFactor), GUILayout.Height(45f * scaleFactor))) PlaceBet(BetType.Odd, 10);
            
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space(15f * scaleFactor);
            
            // 大小和十二宫投注 - 更大的按钮和更好的视觉效果
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color(0.3f, 0.7f, 0.3f); // 亮绿色
            if (GUILayout.Button("小 (1-18)\n(1:1)", colorButtonStyle, GUILayout.Width(125f * scaleFactor), GUILayout.Height(45f * scaleFactor))) PlaceBet(BetType.Low, 10);
            
            GUILayout.Space(10f * scaleFactor);
            
            GUI.backgroundColor = new Color(0.8f, 0.4f, 0.1f); // 橙色
            if (GUILayout.Button("大 (19-36)\n(1:1)", colorButtonStyle, GUILayout.Width(125f * scaleFactor), GUILayout.Height(45f * scaleFactor))) PlaceBet(BetType.High, 10);
            
            GUILayout.Space(10f * scaleFactor);
            
            GUI.backgroundColor = new Color(0.2f, 0.6f, 0.6f); // 青色
            if (GUILayout.Button("第一十二宫\n(1-12) (2:1)", colorButtonStyle, GUILayout.Width(140f * scaleFactor), GUILayout.Height(45f * scaleFactor))) PlaceBet(BetType.Dozen, 10);
            
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space(10f * scaleFactor);
            
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color(0.7f, 0.5f, 0.1f); // 金色
            if (GUILayout.Button("第二十二宫\n(13-24) (2:1)", colorButtonStyle, GUILayout.Width(140f * scaleFactor), GUILayout.Height(45f * scaleFactor))) PlaceBet(BetType.Column, 10);
            
            GUILayout.Space(10f * scaleFactor);
            
            GUI.backgroundColor = new Color(0.7f, 0.2f, 0.5f); // 粉色
            if (GUILayout.Button("第三十二宫\n(25-36) (2:1)", colorButtonStyle, GUILayout.Width(140f * scaleFactor), GUILayout.Height(45f * scaleFactor))) PlaceBet(BetType.Line, 10);
            
            GUILayout.Space(10f * scaleFactor);
            
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space(25f * scaleFactor);
            
            // 数字下注（直选，显示部分数字） - 重新设计的数字网格
            GUILayout.Label("数字下注（直选，赔率35:1）", new GUIStyle(GUI.skin.label) {
                fontSize = Mathf.RoundToInt(14 * scaleFactor),
                fontStyle = FontStyle.Bold,
                normal = {
                    textColor = new Color(0.9f, 0.7f, 0.2f) // 金色文字
                }
            });
            GUILayout.Space(10f * scaleFactor);
            
            // 创建数字按钮容器，增加边框和背景
            GUIStyle numberGridStyle = new GUIStyle();
            numberGridStyle.padding = new RectOffset(
                Mathf.RoundToInt(10 * scaleFactor),
                Mathf.RoundToInt(10 * scaleFactor),
                Mathf.RoundToInt(10 * scaleFactor),
                Mathf.RoundToInt(10 * scaleFactor)
            );
            numberGridStyle.normal.background = MakeTex(1, 1, new Color(0.15f, 0.15f, 0.3f, 0.8f)); // 半透明蓝黑色背景
            
            GUILayout.BeginVertical(numberGridStyle);
            
            // 改进的数字按钮布局 - 更紧凑和美观
            for (int row = 0; row < 3; row++)
            {
                GUILayout.BeginHorizontal();
                for (int col = 0; col < 12; col++)
                {
                    int number = row * 12 + col + 1;
                    if (number > 36) break;
                    
                    // 设置按钮颜色
                    Color btnColor = GetNumberColor(number);
                    Color textColor = (btnColor.r + btnColor.g + btnColor.b) / 3 > 0.5 ? Color.black : Color.white;
                    
                    // 增强按钮样式
                    GUI.backgroundColor = btnColor;
                    GUIStyle numberButtonStyle = new GUIStyle(GUI.skin.button);
                    numberButtonStyle.normal.textColor = textColor;
                    numberButtonStyle.fontStyle = FontStyle.Bold;
                    numberButtonStyle.alignment = TextAnchor.MiddleCenter;
                    numberButtonStyle.border = new RectOffset(2, 2, 2, 2);
                    
                    // 按下效果
                    if (Event.current.type == EventType.Repaint && GUIUtility.hotControl == GUIUtility.GetControlID(FocusType.Passive))
                    {
                        numberButtonStyle.normal.background = MakeTex(1, 1, btnColor);
                        numberButtonStyle.active.background = MakeTex(1, 1, btnColor * 0.7f); // 按下时颜色变深
                    }
                    
                    if (GUILayout.Button(number.ToString(), numberButtonStyle, GUILayout.Width(35f * scaleFactor), GUILayout.Height(35f * scaleFactor))) PlaceStraightBet(number, 10);
                    GUILayout.Space(2f * scaleFactor);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(2f * scaleFactor);
            }
            GUILayout.EndVertical();
            
            // 0号下注按钮 - 增强视觉效果
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f); // 亮绿色
            GUIStyle zeroButtonStyle = new GUIStyle(GUI.skin.button);
            zeroButtonStyle.normal.textColor = Color.white;
            zeroButtonStyle.fontStyle = FontStyle.Bold;
            zeroButtonStyle.alignment = TextAnchor.MiddleCenter;
            zeroButtonStyle.border = new RectOffset(3, 3, 3, 3);
            
            if (GUILayout.Button("0\n(35:1)", zeroButtonStyle, GUILayout.Width(90f * scaleFactor), GUILayout.Height(45f * scaleFactor))) PlaceStraightBet(0, 10);
            GUI.backgroundColor = Color.white;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(25f * scaleFactor);

            // 功能按钮区域 - 重新设计的操作按钮
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 游戏规则按钮 - 更精致的设计
            GUI.backgroundColor = new Color(0.3f, 0.5f, 0.7f); // 蓝色
            GUIStyle infoButtonStyle = new GUIStyle(GUI.skin.button);
            infoButtonStyle.normal.textColor = Color.white;
            infoButtonStyle.fontStyle = FontStyle.Bold;
            infoButtonStyle.alignment = TextAnchor.MiddleCenter;
            
            if (GUILayout.Button("游戏规则", infoButtonStyle, GUILayout.Width(100f * scaleFactor), GUILayout.Height(35f * scaleFactor)))
            {
                ShowGameRules();
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(20f * scaleFactor);
            
            // 操作按钮 - 根据游戏状态显示
            if (currentState == GameState.NotStarted || currentState == GameState.ShowingResult)
            {
                // 开始旋转按钮 - 主操作按钮
                GUI.backgroundColor = currentBet > 0 ? new Color(0.2f, 0.7f, 0.2f) : new Color(0.5f, 0.5f, 0.5f); // 绿色或灰色
                GUIStyle actionButtonStyle = new GUIStyle(GUI.skin.button);
                actionButtonStyle.normal.textColor = Color.white;
                actionButtonStyle.fontStyle = FontStyle.Bold;
                actionButtonStyle.alignment = TextAnchor.MiddleCenter;
                actionButtonStyle.border = new RectOffset(3, 3, 3, 3);
                
                if (GUILayout.Button("开始旋转", actionButtonStyle, GUILayout.Width(130f * scaleFactor), GUILayout.Height(45f * scaleFactor)) && currentBet > 0)
                {
                    StartSpinning();
                }
                GUI.backgroundColor = Color.white;
                
                GUILayout.Space(15f * scaleFactor);
                
                // 清除下注按钮
                GUI.backgroundColor = new Color(0.7f, 0.3f, 0.3f); // 红色
                if (GUILayout.Button("清除下注", actionButtonStyle, GUILayout.Width(130f * scaleFactor), GUILayout.Height(45f * scaleFactor)))
                {
                    ClearBets();
                }
                GUI.backgroundColor = Color.white;
            }
            else if (currentState == GameState.Spinning)
            {
                // 旋转中显示
                GUIStyle spinningStyle = new GUIStyle(GUI.skin.label);
                spinningStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
                spinningStyle.normal.textColor = new Color(0.3f, 0.7f, 0.3f);
                spinningStyle.fontStyle = FontStyle.Bold;
                
                GUILayout.Label("轮盘正在旋转...", spinningStyle);
            }
            
            GUILayout.Space(20f * scaleFactor);
            
            // 返回主窗口按钮 - 醒目且易于识别
            GUI.backgroundColor = new Color(0.5f, 0.2f, 0.2f); // 暗红色
            if (GUILayout.Button("返回主窗口", infoButtonStyle, GUILayout.Width(120f * scaleFactor), GUILayout.Height(35f * scaleFactor)))
            {
                ReturnToMainMenu();
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15f * scaleFactor);
            
            GUILayout.EndVertical();
        }
        
        // 辅助方法：创建简单的纹理
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

        private void DrawRouletteWheel(float radius)
        {
            // 绘制轮盘阴影效果
            Color originalColor = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.3f);
            GUI.DrawTexture(new Rect(-radius - 5, -radius - 5, radius * 2 + 10, radius * 2 + 10), MakeTex(1, 1, GUI.color));
            
            // 绘制轮盘外圈（金属质感）
            GUI.color = new Color(0.7f, 0.7f, 0.7f);
            GUI.DrawTexture(new Rect(-radius, -radius, radius * 2, radius * 2), MakeTex(1, 1, GUI.color));
            
            // 绘制轮盘内部背景
            GUI.color = new Color(0.1f, 0.1f, 0.1f);
            GUI.DrawTexture(new Rect(-radius * 0.95f, -radius * 0.95f, radius * 1.9f, radius * 1.9f), MakeTex(1, 1, GUI.color));
            
            // 绘制分隔线，增加视觉层次感
            int totalNumbers = rouletteNumbers.Count;
            float angleStep = 360f / totalNumbers;
            
            for (int i = 0; i < totalNumbers; i++)
            {
                float angle = i * angleStep + currentRotation;
                float radians = angle * Mathf.Deg2Rad;
                
                // 保存当前GUI矩阵
                Matrix4x4 originalMatrix = GUI.matrix;
                
                // 旋转以绘制分隔线
                GUIUtility.RotateAroundPivot(-angle, Vector2.zero);
                
                // 绘制分隔线
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(-1, -radius * 0.5f, 2, radius * 0.4f), MakeTex(1, 1, GUI.color));
                
                // 恢复原始矩阵
                GUI.matrix = originalMatrix;
            }
            
            // 绘制轮盘数字和扇区
            for (int i = 0; i < totalNumbers; i++)
            {
                float angle = i * angleStep + currentRotation;
                float radians = angle * Mathf.Deg2Rad;
                float nextRadians = (angle + angleStep) * Mathf.Deg2Rad;
                float x = Mathf.Sin(radians) * (radius * 0.8f);
                float y = -Mathf.Cos(radians) * (radius * 0.8f);
                
                // 设置数字颜色
                NumberColor numberColor = rouletteNumbers[i];
                GUI.color = numberColor.color == "红色" ? new Color(0.9f, 0.2f, 0.2f) : numberColor.color == "黑色" ? new Color(0.1f, 0.1f, 0.1f) : new Color(0.3f, 0.8f, 0.3f);
                
                // 绘制扇区背景
                Texture2D sectorTex = MakeTex(1, 1, GUI.color);
                GUI.DrawTexture(new Rect(-radius, -radius, radius * 2, radius * 2), sectorTex);
                
                // 保存当前GUI矩阵
                Matrix4x4 originalMatrix = GUI.matrix;
                
                // 旋转文字以匹配轮盘
                GUIUtility.RotateAroundPivot(-angle, Vector2.zero);
                
                // 绘制数字文本，确保在不同背景色上都清晰可见
                Color textColor = (GUI.color.r + GUI.color.g + GUI.color.b) / 3 > 0.5 ? Color.black : Color.white;
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontSize = Mathf.RoundToInt(12 * scaleFactor);
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = textColor;
                style.fontStyle = FontStyle.Bold;
                
                GUI.Label(new Rect(x - 10, y - 10, 20, 20), numberColor.number.ToString(), style);
                
                // 恢复原始矩阵
                GUI.matrix = originalMatrix;
            }
            
            // 绘制中心区域
            GUI.color = new Color(0.2f, 0.2f, 0.2f);
            GUI.DrawTexture(new Rect(-radius * 0.3f, -radius * 0.3f, radius * 0.6f, radius * 0.6f), MakeTex(1, 1, GUI.color));
            
            // 绘制中心装饰
            GUI.color = new Color(0.9f, 0.7f, 0.2f); // 金色
            GUI.DrawTexture(new Rect(-radius * 0.15f, -radius * 0.15f, radius * 0.3f, radius * 0.3f), MakeTex(1, 1, GUI.color));
            
            // 绘制指示器
            GUI.color = Color.red;
            GUI.DrawTexture(new Rect(-5, -radius - 10, 10, 20), MakeTex(1, 1, GUI.color));
            
            // 添加指示器装饰
            GUI.color = new Color(0.9f, 0.1f, 0.1f);
            GUI.DrawTexture(new Rect(-3, -radius - 8, 6, 16), MakeTex(1, 1, GUI.color));
            
            // 如果正在显示结果，显示中奖信息
            if (currentState == GameState.ShowingResult)
            {
                GUI.color = winningColor == "红色" ? Color.red : winningColor == "黑色" ? Color.black : Color.green;
                GUIStyle resultStyle = new GUIStyle(GUI.skin.label);
                resultStyle.fontSize = Mathf.RoundToInt(20 * scaleFactor);
                resultStyle.fontStyle = FontStyle.Bold;
                resultStyle.alignment = TextAnchor.MiddleCenter;
                resultStyle.normal.textColor = GUI.color;
                
                GUI.Label(new Rect(-70, radius + 20, 140, 30), $"中奖号码：{winningNumber}", resultStyle);
                
                // 显示中奖颜色文本
                string colorText = winningColor;
                GUIStyle colorStyle = new GUIStyle(GUI.skin.label);
                colorStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
                colorStyle.alignment = TextAnchor.MiddleCenter;
                colorStyle.normal.textColor = GUI.color;
                
                GUI.Label(new Rect(-50, radius + 50, 100, 25), colorText, colorStyle);
            }
            
            // 显示破产警告
            if (bankruptcyWarning)
            {
                GUIStyle warningStyle = new GUIStyle(GUI.skin.label);
                warningStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
                warningStyle.fontStyle = FontStyle.Bold;
                warningStyle.alignment = TextAnchor.MiddleCenter;
                warningStyle.normal.textColor = new Color(1f, 0.5f, 0f); // 橙色警告
                
                GUI.Label(new Rect(-120, radius + 80, 240, 30), "救济金已发放！请谨慎下注。", warningStyle);
            }
            
            GUI.color = originalColor;
        }

        private void PlaceBet(BetType betType, int amount)
        {
            if (currentState != GameState.NotStarted && currentState != GameState.ShowingResult) return;
            
            if (playerChips >= amount)
            {
                playerChips -= amount;
                betAmounts[betType] += amount;
                currentBet += amount;
                
                // 记录下注操作
                Logger.LogInfo($"玩家下注：{GetBetTypeName(betType)}，金额：{amount}");
                
                // 检查玩家是否破产
                CheckPlayerBankruptcy();
            }
        }

        private Color GetNumberColor(int number)
        {
            foreach (var numColor in rouletteNumbers)
            {
                if (numColor.number == number)
                {
                    return numColor.color == "红色" ? Color.red : numColor.color == "黑色" ? Color.black : Color.green;
                }
            }
            return Color.white;
        }
        
        // 获取投注类型的中文名称
        private string GetBetTypeName(BetType betType)
        {
            switch (betType)
            {
                case BetType.Red:
                    return "红色";
                case BetType.Black:
                    return "黑色";
                case BetType.Even:
                    return "偶数";
                case BetType.Odd:
                    return "奇数";
                case BetType.Low:
                    return "小 (1-18)";
                case BetType.High:
                    return "大 (19-36)";
                case BetType.Dozen:
                    return "第一十二宫 (1-12)";
                case BetType.Column:
                    return "第二十二宫 (13-24)";
                case BetType.Line:
                    return "第三十二宫 (25-36)";
                default:
                    return "直选数字";
            }
        }

        private void PlaceStraightBet(int number, int amount)
        {
            if (currentState != GameState.NotStarted && currentState != GameState.ShowingResult) return;
            
            if (playerChips >= amount)
            {
                playerChips -= amount;
                BetType betType = (BetType)((int)BetType.Straight + number); // 使用唯一的BetType值表示不同数字的下注
                straightBets[betType] = number;
                if (!betAmounts.ContainsKey(betType))
                {
                    betAmounts[betType] = 0;
                }
                betAmounts[betType] += amount;
                currentBet += amount;
                
                // 记录直选下注操作
                Logger.LogInfo($"玩家直选下注：数字 {number}，金额：{amount}");
                
                // 检查玩家是否破产
                CheckPlayerBankruptcy();
            }
        }

        private void ClearBets()
        {
            if (currentState != GameState.NotStarted && currentState != GameState.ShowingResult) return;
            
            // 退还所有下注金额给玩家
            playerChips += currentBet;
            currentBet = 0;
            InitializeBetAmounts();
            
            Logger.LogInfo("玩家清除了所有下注");
        }

        private void StartSpinning()
        {
            currentState = GameState.Spinning;
            
            // 随机初始旋转速度，增加游戏变化性
            float baseSpeed = 1000f;
            float randomVariation = UnityEngine.Random.Range(-100f, 100f);
            spinSpeed = baseSpeed + randomVariation;
            initialSpinSpeed = spinSpeed;
            
            // 随机减速速率，模拟不同的轮盘阻力
            float baseDeceleration = 5f;
            float randomDeceleration = UnityEngine.Random.Range(-1f, 1f);
            decelerationRate = baseDeceleration + randomDeceleration;
            
            // 确保减速速率为正
            decelerationRate = Mathf.Max(decelerationRate, 3f);
            
            Logger.LogInfo("轮盘开始旋转，初始速度：" + spinSpeed);
        }

        private void DetermineWinningNumber()
        {
            // 根据当前旋转角度确定中奖号码
            float normalizedRotation = currentRotation % 360f;
            if (normalizedRotation < 0) normalizedRotation += 360f;
            
            int totalNumbers = rouletteNumbers.Count;
            float angleStep = 360f / totalNumbers;
            int winningIndex = Mathf.FloorToInt((360f - normalizedRotation) / angleStep) % totalNumbers;
            
            NumberColor winningNC = rouletteNumbers[winningIndex];
            winningNumber = winningNC.number;
            winningColor = winningNC.color;
        }

        private void CalculateWinnings()
        {
            int winnings = 0;
            
            // 检查所有下注类型
            foreach (var bet in betAmounts)
            {
                BetType betType = bet.Key;
                int amount = bet.Value;
                
                if (amount == 0) continue;
                
                // 检查是否中奖
                if (IsWinningBet(betType, winningNumber, winningColor))
                {
                    int payout = CalculatePayout(betType, amount);
                    winnings += payout;
                }
            }
            
            // 添加奖金到玩家筹码
            playerChips += winnings;
            
            // 重置下注
            currentBet = 0;
            InitializeBetAmounts();
            
            // 记录中奖信息
            Logger.LogInfo($"轮盘赌中奖号码：{winningNumber}，玩家赢得：{winnings}个{(UsePaperMoney ? "宝钞" : "金钞")}");
        }

        private bool IsWinningBet(BetType betType, int winningNumber, string winningColor)
        {
            // 检查下注是否中奖
            switch (betType)
            {
                case BetType.Straight:
                    // 直选下注（单个数字）
                    if (straightBets.ContainsKey(betType))
                    {
                        return straightBets[betType] == winningNumber;
                    }
                    return false;
                case BetType.Split:
                    // 分割下注（两个相邻数字）
                    if (straightBets.ContainsKey(betType))
                    {
                        int betNumber = straightBets[betType];
                        // 检查是否是相邻数字（横向或纵向）
                        return (winningNumber == betNumber + 1 || winningNumber == betNumber - 1 ||
                                winningNumber == betNumber + 3 || winningNumber == betNumber - 3);
                    }
                    return false;
                case BetType.Street:
                    // 街道下注（三个数字一行）
                    return winningNumber >= 1 && winningNumber <= 36 && ((winningNumber - 1) % 3 == 0);
                case BetType.Corner:
                    // 角落下注（四个数字形成一个方块）
                    return winningNumber >= 2 && winningNumber <= 35 && winningNumber % 3 != 0;
                case BetType.Line:
                    // 线下注（六个数字两行）
                    return winningNumber >= 1 && winningNumber <= 36 && ((winningNumber - 1) % 3 < 2);
                case BetType.Dozen:
                    // 第一十二宫（1-12）
                    return winningNumber >= 1 && winningNumber <= 12;
                case BetType.Column:
                    // 第二十二宫（13-24）
                    return winningNumber >= 13 && winningNumber <= 24;
                case BetType.Even:
                    return winningNumber > 0 && winningNumber % 2 == 0;
                case BetType.Odd:
                    return winningNumber > 0 && winningNumber % 2 == 1;
                case BetType.Red:
                    return winningColor == "红色";
                case BetType.Black:
                    return winningColor == "黑色";
                case BetType.Low:
                    return winningNumber >= 1 && winningNumber <= 18;
                case BetType.High:
                    return winningNumber >= 19 && winningNumber <= 36;
                default:
                    // 检查自定义数字下注
                    if (straightBets.ContainsKey(betType))
                    {
                        return straightBets[betType] == winningNumber;
                    }
                    return false;
            }
        }

        private int CalculatePayout(BetType betType, int amount)
        {
            // 计算奖金倍率
            int multiplier = 0;
            
            switch (betType)
            {
                case BetType.Straight:
                    multiplier = 35; // 直选赔率35:1
                    break;
                case BetType.Split:
                    multiplier = 17; // 分割赔率17:1
                    break;
                case BetType.Street:
                    multiplier = 11; // 街道赔率11:1
                    break;
                case BetType.Corner:
                    multiplier = 8; // 角落赔率8:1
                    break;
                case BetType.Line:
                    multiplier = 5; // 线赔率5:1
                    break;
                case BetType.Dozen:
                    multiplier = 2; // 第一十二宫赔率2:1
                    break;
                case BetType.Column:
                    // 检查是否是Column下注还是第二十二宫下注
                    if (straightBets.ContainsKey(betType))
                    {
                        // 如果是直选数字，返回35:1赔率
                        multiplier = 35;
                    }
                    else
                    {
                        multiplier = 2; // 第二十二宫赔率2:1
                    }
                    break;
                case BetType.Even:
                case BetType.Odd:
                case BetType.Red:
                case BetType.Black:
                case BetType.Low:
                case BetType.High:
                    multiplier = 1; // 偶数/奇数/红/黑/低/高赔率1:1
                    break;
                default:
                    // 检查自定义数字下注（直选）
                    if (straightBets.ContainsKey(betType))
                    {
                        multiplier = 35;
                    }
                    else
                    {
                        multiplier = 0;
                    }
                    break;
            }
            
            // 计算奖金（包括原始下注）
            int payout = amount * (multiplier + 1);
            
            // 记录奖金信息
            Logger.LogInfo("玩家下注类型：" + GetBetTypeName(betType) + "，金额：" + amount + "，赔率：" + multiplier + ":1，获得奖金：" + payout);
            
            return payout;
        }

        // 返回主窗口并保存结余货币
        private void ReturnToMainMenu()
        {
            // 保存结余货币
            RemainingMoney = playerChips;
            
            // 关闭轮盘赌窗口
            showMenu = false;
        }
        
        // 准备下一局游戏
        private void PrepareForNextGame()
        {
            // 重置标志
            hasCalledRepeatLogic = false;
            
            // 设置为待开始状态
            currentState = GameState.NotStarted;
            
            // 清除破产警告
            bankruptcyWarning = false;
            blockGameInput = false;
            
            Logger.LogInfo("玩家退出轮盘赌游戏");
            Logger.LogInfo("已返回主窗口，结余" + (UsePaperMoney ? "宝钞" : "金钞") + "：" + RemainingMoney);
            
            // 尝试打开MoreGambles主窗口
            A_MoreGambles moreGambles = FindObjectOfType<A_MoreGambles>();
            if (moreGambles != null)
            {
                moreGambles.ReturnFromGame(UsePaperMoney, RemainingMoney);
            }
        }
        
        // 显示游戏规则
        private void ShowGameRules()
        {
            string rules = "轮盘赌游戏规则：\n\n" +
                          "1. 这是美式轮盘，包含数字0-36。\n" +
                          "2. 下注方式：\n" +
                          "   - 红色/黑色/偶数/奇数/小(1-18)/大(19-36)：赔率1:1\n" +
                          "   - 十二宫(1-12)/(13-24)/(25-36)：赔率2:1\n" +
                          "   - 直选数字：赔率35:1\n" +
                          "3. 每次旋转后，系统会自动计算中奖结果并发放奖金。\n" +
                          "4. 当玩家破产时，系统会发放救济金（最多6次）。\n" +
                          "5. 按下F5可以快速关闭窗口。";
            
            Logger.LogInfo(rules);
        }

        // 更新分辨率设置
        public void UpdateResolutionSettings()
        {
            // 根据屏幕分辨率调整UI比例
            float baseWidth = 1920f;
            float baseHeight = 1080f;
            
            // 计算适合当前屏幕的缩放因子，确保UI不会过大
            float widthScale = Screen.width / baseWidth;
            float heightScale = Screen.height / baseHeight;
            scaleFactor = Mathf.Min(widthScale, heightScale);
            
            // 确保缩放因子在合理范围内
            scaleFactor = Mathf.Max(0.7f, Mathf.Min(1.8f, scaleFactor));
            
            // 调整窗口大小以适应缩放
            windowRect.width = 800f * scaleFactor;
            windowRect.height = 600f * scaleFactor;
        }
    }
}
