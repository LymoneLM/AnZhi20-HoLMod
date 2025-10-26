using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;
using System.Text.RegularExpressions;

namespace cs.HoLMod.MoreGambles
{
    public class Dice : BaseUnityPlugin
    {
        // 窗口设置
        private Rect windowRect = new Rect(20, 20, 800, 600);
        private bool showMenu = false;
        private bool blockGameInput = false;
        private float scaleFactor = 1f;
        private const string CURRENT_VERSION = "1.0.0";
        
        // 货币系统相关
        public bool UsePaperMoney { get; set; } = true; // true=宝钞, false=金钞
        public int InitialChips { get; set; } = 1000;
        public int RemainingMoney { get; set; } = 0;
        
        // 游戏结束窗口
        private Rect gameOverWindowRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 200);
        private string gameOverMessage = "";

        // 骰子游戏状态
        private enum GameState { NotStarted, ChoosingMode, GuessingPoint, GuessingOddEven, Rolling, GameOver }
        private GameState currentState = GameState.NotStarted;
        private int pot = 0;
        private int playerChips = 1000;
        private int currentBet = 0;

        // 骰子相关
        private int[] diceValues = new int[5]; // 存储5个骰子的点数
        private int playerGuess = 0; // 玩家猜的点数
        private string playerGuessInput = "";
        private int totalDiceSum = 0; // 骰子总和
        private bool playerGuessIsEven = true; // 动画相关
        private bool isAnimating = false; // 是否正在进行骰子动画
        private float animationTimer = 0f; // 动画计时器
        private float animationDuration = 0.8f; // 动画持续时间
        private int[] animatedDiceValues = new int[5]; // 动画中的骰子值
        private string currencyType; // 缓存的货币类型
        
        // 重复游戏逻辑相关
        private bool hasCalledRepeatLogic = false;

        // 游戏模式
        private enum GameMode { CompareSize, GuessPoint, GuessOddEven }
        private GameMode selectedMode = GameMode.CompareSize;

        // AI玩家类
        private class AIPlayer
        {
            public string Name { get; set; }
            public int Chips { get; set; }
            public int Bet { get; set; }
            public int DiceSum { get; set; } // 骰子总和
            public int[] AIDiceValues { get; set; } // AI的骰子值
            public int AIGuess { get; set; } // AI猜的点数
            public bool AIGuessIsEven { get; set; } // AI猜的奇偶
            public bool IsDealer { get; set; } // 是否是庄家

            public AIPlayer(string name)
            {
                Name = name;
                Chips = 1000;
                Bet = 0;
                DiceSum = 0;
                AIDiceValues = new int[5];
                AIGuess = 0;
                IsDealer = name.Contains("庄家");
            }
        }

        private List<AIPlayer> aiPlayers = new List<AIPlayer>(); // 5个AI玩家

        private void Awake()
        {
            InitializeAIPlayers();
            
            // 设置玩家初始筹码
            playerChips = InitialChips;
            
            // 初始化骰子值数组
            diceValues = new int[5];
            animatedDiceValues = new int[5];
        }

        // 初始化AI玩家
        private void InitializeAIPlayers()
        {
            aiPlayers.Clear();
            // 使用B_AI_Name类生成5个不重复的随机AI姓名
            List<string> aiNames = B_AI_Name.GenerateRandomNames(5);
            
            // 第一个AI作为庄家
            aiPlayers.Add(new AIPlayer(aiNames[0] + "(庄家)"));
            
            // 添加剩余4个AI玩家
            for (int i = 1; i < 5; i++)
            {
                aiPlayers.Add(new AIPlayer(aiNames[i]));
            }
        }

        private void Update()
        {
            // 按F5键切换窗口显示（如果需要的话）
            if (Input.GetKeyDown(KeyCode.F5))
            {
                UpdateResolutionSettings();
                showMenu = !showMenu;
                blockGameInput = showMenu;
                Logger.LogInfo(showMenu ? "骰子游戏窗口已打开" : "骰子游戏窗口已关闭");
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
            
            // 骰子动画效果
            if (isAnimating)
            {
                animationTimer += Time.deltaTime;
                
                if (animationTimer < animationDuration)
                {
                    // 在动画期间随机生成骰子值
                    System.Random rng = new System.Random();
                    for (int i = 0; i < animatedDiceValues.Length; i++)
                    {
                        animatedDiceValues[i] = rng.Next(1, 7);
                    }
                }
                else
                {
                    // 动画结束，显示最终结果
                    isAnimating = false;
                    for (int i = 0; i < diceValues.Length; i++)
                    {
                        animatedDiceValues[i] = diceValues[i];
                    }
                    
                    // 设置游戏状态为Rolling，显示结果
                    currentState = GameState.Rolling;
                }
            }
        }

        private void OnGUI()
        { 
            if (!showMenu) return;

            // 保存窗口背景色并设置为半透明
            Color originalBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.2f, 0.4f, 0.1f, 0.95f); // 绿色调背景

            // 显示一个半透明的背景遮罩
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
            GUI.color = new Color(0, 0, 0, 0.3f);
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
                Mathf.RoundToInt(10 * scaleFactor),
                Mathf.RoundToInt(10 * scaleFactor)
            );

            GUI.skin.label.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            GUI.skin.button.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            GUI.skin.textField.fontSize = Mathf.RoundToInt(12 * scaleFactor);

            // 显示游戏结束窗口
            if (currentState == GameState.GameOver)
            {
                gameOverWindowRect = GUI.Window(1, gameOverWindowRect, DrawGameOverWindow, "游戏结束");
            }
            else
            {
                // 创建主游戏窗口
                windowRect = GUI.Window(0, windowRect, DrawWindow, "骰子游戏", GUI.skin.window);
            }

            // 恢复原始矩阵和背景色
            GUI.matrix = guiMatrix;
            GUI.backgroundColor = originalBackgroundColor;
        }

        private void DrawWindow(int windowID)
        {
            // 允许拖动窗口
            GUI.DragWindow(new Rect(0, 0, windowRect.width, 25));
            
            // 重置UI样式
            ResetUIStyles();

            // 窗口最小宽度和高度
            windowRect.width = Mathf.Max(windowRect.width, 800f * scaleFactor);
            windowRect.height = Mathf.Max(windowRect.height, 600f * scaleFactor);

            GUILayout.BeginVertical();

            // 标题区域 - 改进的标题栏
            DrawTitleBar();

            // 状态信息区域
            DrawStatusInfo();

            // 根据游戏状态显示不同内容
            switch (currentState)
            {
                case GameState.NotStarted:
                    DrawStartGameScreen();
                    break;
                    
                case GameState.ChoosingMode:
                    DrawGameModeSelection();
                    break;
                    
                case GameState.GuessingPoint:
                    DrawGuessPointScreen();
                    break;
                    
                case GameState.GuessingOddEven:
                    DrawGuessOddEvenScreen();
                    break;
                    
                case GameState.Rolling:
                    DrawRollingScreen();
                    break;
            }

            // 底部操作按钮区域
            DrawBottomButtons();

            GUILayout.EndVertical();
        }

        // 重置UI样式
        private void ResetUIStyles()
        {
            int fontSize = Mathf.RoundToInt(14 * scaleFactor);
            GUI.skin.label.fontSize = fontSize;
            GUI.skin.button.fontSize = fontSize;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            GUI.skin.window.fontSize = fontSize;
            GUI.skin.textField.fontSize = fontSize;
        }

        // 绘制标题栏
        private void DrawTitleBar()
        {
            // 创建精美的标题栏背景
            GUI.backgroundColor = new Color(0.1f, 0.3f, 0.1f, 0.95f);
            GUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            
            GUILayout.Label("骰子游戏", new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(22 * scaleFactor),
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1f, 0.9f, 0.5f, 1f) }
            });
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5f * scaleFactor);
            
            // 版本信息
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("版本: " + CURRENT_VERSION, new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(10 * scaleFactor),
                normal = { textColor = Color.gray }
            });
            GUILayout.EndHorizontal();
            GUILayout.Space(15f * scaleFactor);
        }

        // 绘制状态信息
        private void DrawStatusInfo()
        {
            // 创建状态信息面板
            GUI.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
            GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.Height(60f * scaleFactor));
            
            GUILayout.BeginHorizontal();
            // 显示余额
            string currencyType = UsePaperMoney ? "宝钞" : "金钞";
            GUILayout.Label("你的" + currencyType + "：", new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(14 * scaleFactor),
                normal = { textColor = Color.white }
            });
            GUILayout.Label(playerChips.ToString(), new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(14 * scaleFactor),
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1f, 0.85f, 0.3f, 1f) }
            });
            
            GUILayout.FlexibleSpace();
            
            // 显示游戏状态
            string stateText = currentState == GameState.NotStarted ? "等待开始" :
                             currentState == GameState.ChoosingMode ? "选择游戏模式" :
                             currentState == GameState.GuessingPoint ? "猜点数" :
                             currentState == GameState.GuessingOddEven ? "猜奇偶" :
                             currentState == GameState.Rolling ? "摇骰子" : "游戏结束";
            
            GUILayout.Label("当前阶段：", new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(14 * scaleFactor),
                normal = { textColor = Color.white }
            });
            GUILayout.Label(stateText, new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(14 * scaleFactor),
                fontStyle = FontStyle.Bold,
                normal = { textColor = GetStateColor(currentState) }
            });
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            GUILayout.Space(20f * scaleFactor);
        }

        // 根据游戏状态获取对应颜色
        private Color GetStateColor(GameState state)
        {
            switch (state)
            {
                case GameState.NotStarted:
                    return Color.gray;
                case GameState.ChoosingMode:
                case GameState.GuessingPoint:
                case GameState.GuessingOddEven:
                    return new Color(0.5f, 0.8f, 1f, 1f);
                case GameState.Rolling:
                    return new Color(0.7f, 0.9f, 0.7f, 1f);
                case GameState.GameOver:
                    return new Color(1f, 0.5f, 0.5f, 1f);
                default:
                    return Color.white;
            }
        }

        // 绘制开始游戏界面
        private void DrawStartGameScreen()
        {
            GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.Height(200f * scaleFactor));
            GUILayout.FlexibleSpace();
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 美化开始游戏按钮
            GUIStyle startButtonStyle = CreateButtonStyle(new Color(0.25f, 0.65f, 0.25f, 0.95f), new Color(0.35f, 0.75f, 0.35f, 0.95f), new Color(0.2f, 0.55f, 0.2f, 0.95f));
            startButtonStyle.fontSize = Mathf.RoundToInt(18 * scaleFactor);
            startButtonStyle.fontStyle = FontStyle.Bold;
            
            if (GUILayout.Button("开始游戏", startButtonStyle, GUILayout.Width(200f * scaleFactor), GUILayout.Height(60f * scaleFactor)))
            {
                StartNewGame();
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        // 绘制游戏模式选择界面
        private void DrawGameModeSelection()
        {
            GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.MinHeight(250f * scaleFactor));
            
            GUILayout.Space(20f * scaleFactor);
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("请选择游戏模式：", new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(16 * scaleFactor),
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1f, 0.9f, 0.5f, 1f) }
            });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(30f * scaleFactor);
            
            // 游戏模式按钮
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 比大小模式
            GUIStyle modeButtonStyle = CreateButtonStyle(
                selectedMode == GameMode.CompareSize ? new Color(0.3f, 0.7f, 0.3f, 0.95f) : new Color(0.2f, 0.4f, 0.6f, 0.95f),
                selectedMode == GameMode.CompareSize ? new Color(0.4f, 0.8f, 0.4f, 0.95f) : new Color(0.3f, 0.5f, 0.7f, 0.95f),
                selectedMode == GameMode.CompareSize ? new Color(0.25f, 0.6f, 0.25f, 0.95f) : new Color(0.15f, 0.35f, 0.55f, 0.95f)
            );
            modeButtonStyle.fontSize = Mathf.RoundToInt(15 * scaleFactor);
            
            if (GUILayout.Button("比大小", modeButtonStyle, GUILayout.Width(160f * scaleFactor), GUILayout.Height(50f * scaleFactor)))
            {
                selectedMode = GameMode.CompareSize;
            }
            
            GUILayout.Space(25f * scaleFactor);
            
            // 猜点数模式
            modeButtonStyle.normal.background = MakeTex(1, 1, selectedMode == GameMode.GuessPoint ? new Color(0.3f, 0.7f, 0.3f, 0.95f) : new Color(0.2f, 0.4f, 0.6f, 0.95f));
            modeButtonStyle.hover.background = MakeTex(1, 1, selectedMode == GameMode.GuessPoint ? new Color(0.4f, 0.8f, 0.4f, 0.95f) : new Color(0.3f, 0.5f, 0.7f, 0.95f));
            modeButtonStyle.active.background = MakeTex(1, 1, selectedMode == GameMode.GuessPoint ? new Color(0.25f, 0.6f, 0.25f, 0.95f) : new Color(0.15f, 0.35f, 0.55f, 0.95f));
            
            if (GUILayout.Button("猜点数", modeButtonStyle, GUILayout.Width(160f * scaleFactor), GUILayout.Height(50f * scaleFactor)))
            {
                selectedMode = GameMode.GuessPoint;
            }
            
            GUILayout.Space(25f * scaleFactor);
            
            // 猜奇偶模式
            modeButtonStyle.normal.background = MakeTex(1, 1, selectedMode == GameMode.GuessOddEven ? new Color(0.3f, 0.7f, 0.3f, 0.95f) : new Color(0.2f, 0.4f, 0.6f, 0.95f));
            modeButtonStyle.hover.background = MakeTex(1, 1, selectedMode == GameMode.GuessOddEven ? new Color(0.4f, 0.8f, 0.4f, 0.95f) : new Color(0.3f, 0.5f, 0.7f, 0.95f));
            modeButtonStyle.active.background = MakeTex(1, 1, selectedMode == GameMode.GuessOddEven ? new Color(0.25f, 0.6f, 0.25f, 0.95f) : new Color(0.15f, 0.35f, 0.55f, 0.95f));
            
            if (GUILayout.Button("猜奇偶", modeButtonStyle, GUILayout.Width(160f * scaleFactor), GUILayout.Height(50f * scaleFactor)))
            {
                selectedMode = GameMode.GuessOddEven;
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(40f * scaleFactor);
            
            // 确认按钮
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            GUIStyle confirmButtonStyle = CreateButtonStyle(new Color(0.25f, 0.5f, 0.75f, 0.95f), new Color(0.35f, 0.6f, 0.85f, 0.95f), new Color(0.2f, 0.4f, 0.65f, 0.95f));
            confirmButtonStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
            confirmButtonStyle.fontStyle = FontStyle.Bold;
            
            if (GUILayout.Button("确认", confirmButtonStyle, GUILayout.Width(120f * scaleFactor), GUILayout.Height(45f * scaleFactor)))
            {
                if (selectedMode == GameMode.GuessPoint)
                {
                    currentState = GameState.GuessingPoint;
                    playerGuessInput = "";
                }
                else if (selectedMode == GameMode.GuessOddEven)
                {
                    currentState = GameState.GuessingOddEven;
                    playerGuessIsEven = true; // 默认猜偶数
                }
                else
                {
                    RollDice();
                }
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(20f * scaleFactor);
            GUILayout.EndVertical();
        }

        // 绘制猜点数界面
        private void DrawGuessPointScreen()
        {
            GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.MinHeight(250f * scaleFactor));
            
            GUILayout.Space(20f * scaleFactor);
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("请猜骰子总和（5-30之间）：", new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(16 * scaleFactor),
                normal = { textColor = new Color(1f, 0.9f, 0.5f, 1f) }
            });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(20f * scaleFactor);
            
            // 输入框区域
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 美化输入框
            GUIStyle inputStyle = new GUIStyle(GUI.skin.textField);
            inputStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
            inputStyle.alignment = TextAnchor.MiddleCenter;
            inputStyle.normal.background = MakeTex(1, 1, new Color(0.25f, 0.25f, 0.25f, 0.95f));
            inputStyle.normal.textColor = Color.white;
            
            playerGuessInput = GUILayout.TextField(playerGuessInput, inputStyle, GUILayout.Width(120f * scaleFactor), GUILayout.Height(40f * scaleFactor));
            // 限制只能输入数字
            playerGuessInput = System.Text.RegularExpressions.Regex.Replace(playerGuessInput, "[^0-9]", "");
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10f * scaleFactor);
            
            // 提示信息
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("输入范围：5-30", new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(12 * scaleFactor),
                normal = { textColor = Color.gray }
            });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(30f * scaleFactor);
            
            // 确认按钮
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            GUIStyle confirmButtonStyle = CreateButtonStyle(new Color(0.25f, 0.5f, 0.75f, 0.95f), new Color(0.35f, 0.6f, 0.85f, 0.95f), new Color(0.2f, 0.4f, 0.65f, 0.95f));
            confirmButtonStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
            confirmButtonStyle.fontStyle = FontStyle.Bold;
            
            if (GUILayout.Button("确认猜测", confirmButtonStyle, GUILayout.Width(140f * scaleFactor), GUILayout.Height(45f * scaleFactor)))
            {
                if (int.TryParse(playerGuessInput, out int guess) && guess >= 5 && guess <= 30)
                {
                    playerGuess = guess;
                    RollDice();
                }
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(20f * scaleFactor);
            GUILayout.EndVertical();
        }

        // 绘制猜奇偶界面
        private void DrawGuessOddEvenScreen()
        {
            GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.MinHeight(250f * scaleFactor));
            
            GUILayout.Space(20f * scaleFactor);
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("请猜测骰子总和是奇数还是偶数：", new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(16 * scaleFactor),
                normal = { textColor = new Color(1f, 0.9f, 0.5f, 1f) }
            });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(30f * scaleFactor);
            
            // 奇偶选择按钮
            GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    
                    // 偶数按钮
                    GUIStyle guessButtonStyle = CreateButtonStyle(
                        playerGuessIsEven ? new Color(0.3f, 0.7f, 0.3f, 0.95f) : new Color(0.2f, 0.4f, 0.6f, 0.95f),
                        playerGuessIsEven ? new Color(0.4f, 0.8f, 0.4f, 0.95f) : new Color(0.3f, 0.5f, 0.7f, 0.95f),
                        playerGuessIsEven ? new Color(0.25f, 0.6f, 0.25f, 0.95f) : new Color(0.15f, 0.35f, 0.55f, 0.95f)
                    );
                    guessButtonStyle.fontSize = Mathf.RoundToInt(15 * scaleFactor);
                    
                    if (GUILayout.Button("偶数", guessButtonStyle, GUILayout.Width(120f * scaleFactor), GUILayout.Height(50f * scaleFactor)))
                    {
                        playerGuessIsEven = true;
                    }
                    
                    GUILayout.Space(40f * scaleFactor);
                    
                    // 奇数按钮
                    guessButtonStyle.normal.background = MakeTex(1, 1, !playerGuessIsEven ? new Color(0.3f, 0.7f, 0.3f, 0.95f) : new Color(0.2f, 0.4f, 0.6f, 0.95f));
                    guessButtonStyle.hover.background = MakeTex(1, 1, !playerGuessIsEven ? new Color(0.4f, 0.8f, 0.4f, 0.95f) : new Color(0.3f, 0.5f, 0.7f, 0.95f));
                    guessButtonStyle.active.background = MakeTex(1, 1, !playerGuessIsEven ? new Color(0.25f, 0.6f, 0.25f, 0.95f) : new Color(0.15f, 0.35f, 0.55f, 0.95f));
                    
                    if (GUILayout.Button("奇数", guessButtonStyle, GUILayout.Width(120f * scaleFactor), GUILayout.Height(50f * scaleFactor)))
                    {
                        playerGuessIsEven = false;
                    }
                    
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    
                    GUILayout.Space(40f * scaleFactor);
                    
                    // 确认按钮
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    
                    GUIStyle confirmButtonStyle = CreateButtonStyle(new Color(0.25f, 0.5f, 0.75f, 0.95f), new Color(0.35f, 0.6f, 0.85f, 0.95f), new Color(0.2f, 0.4f, 0.65f, 0.95f));
                    confirmButtonStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
                    confirmButtonStyle.fontStyle = FontStyle.Bold;
                    
                    if (GUILayout.Button("确认猜测", confirmButtonStyle, GUILayout.Width(140f * scaleFactor), GUILayout.Height(45f * scaleFactor)))
                    {
                        RollDice();
                    }
                    
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    
                    GUILayout.Space(20f * scaleFactor);
                    GUILayout.EndVertical();
                }

        // 绘制摇骰子结果界面
        private void DrawRollingScreen()
        {
            GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.MinHeight(300f * scaleFactor));
            
            GUILayout.Space(20f * scaleFactor);
            
            // 状态提示
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (isAnimating)
            {
                GUILayout.Label("骰子摇动中...", new GUIStyle(GUI.skin.label)
                {
                    fontSize = Mathf.RoundToInt(16 * scaleFactor),
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = new Color(1f, 0.7f, 0.3f, 1f) }
                });
            }
            else
            {
                GUILayout.Label("骰子结果：", new GUIStyle(GUI.skin.label)
                {
                    fontSize = Mathf.RoundToInt(16 * scaleFactor),
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = new Color(1f, 0.9f, 0.5f, 1f) }
                });
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(20f * scaleFactor);
            
            // 骰子显示区域 - 使用更精美的骰子绘制
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 骰子容器背景
            GUI.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
            GUILayout.BeginVertical("box", GUILayout.Width(400f * scaleFactor), GUILayout.Height(120f * scaleFactor));
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (isAnimating)
            {
                // 动画期间显示动画中的骰子值
                for (int i = 0; i < animatedDiceValues.Length; i++)
                {
                    DrawStyledDice(animatedDiceValues[i]);
                    GUILayout.Space(15f * scaleFactor);
                }
            }
            else
            {
                // 正常显示骰子值
                for (int i = 0; i < diceValues.Length; i++)
                {
                    DrawStyledDice(diceValues[i]);
                    GUILayout.Space(15f * scaleFactor);
                }
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(20f * scaleFactor);
            
            // 总和显示
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            GUI.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
            GUILayout.BeginVertical("box", GUILayout.Width(200f * scaleFactor), GUILayout.Height(60f * scaleFactor));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("总和：", new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(14 * scaleFactor),
                normal = { textColor = Color.white }
            });
            GUILayout.Label(totalDiceSum.ToString(), new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(16 * scaleFactor),
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1f, 0.85f, 0.3f, 1f) }
            });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(30f * scaleFactor);
            
            // 游戏结果显示
            if (!isAnimating)
            {
                GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.95f);
                GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.Height(80f * scaleFactor));
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(gameOverMessage, new GUIStyle(GUI.skin.label)
                {
                    fontSize = Mathf.RoundToInt(15 * scaleFactor),
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = GetResultColor(gameOverMessage) }
                });
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                
                GUILayout.Space(25f * scaleFactor);
                
                // AI结果显示区域
                GUI.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
                GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.MinHeight(100f * scaleFactor));
                GUILayout.Space(10f * scaleFactor);
                
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("AI玩家结果：", new GUIStyle(GUI.skin.label)
                {
                    fontSize = Mathf.RoundToInt(14 * scaleFactor),
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = new Color(1f, 0.9f, 0.5f, 1f) }
                });
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10f * scaleFactor);
                
                // 显示AI结果
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                
                GUILayout.BeginVertical();
                foreach (var ai in aiPlayers)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(ai.Name + "：", new GUIStyle(GUI.skin.label)
                    {
                        fontSize = Mathf.RoundToInt(12 * scaleFactor),
                        normal = { textColor = ai.IsDealer ? new Color(1f, 0.7f, 0.3f, 1f) : Color.white }
                    });
                    GUILayout.Label(ai.DiceSum.ToString(), new GUIStyle(GUI.skin.label)
                    {
                        fontSize = Mathf.RoundToInt(12 * scaleFactor),
                        normal = { textColor = Color.white }
                    });
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10f * scaleFactor);
                GUILayout.EndVertical();
            }
            
            GUILayout.Space(30f * scaleFactor);
            
            // 操作按钮
            if (!isAnimating && currentState == GameState.GameOver && !hasCalledRepeatLogic)
            {
                hasCalledRepeatLogic = true;
                
                // 直接调用静态方法，传入必要参数
                B_RepeatTheGame.HandleGameEnd(
                    playerChips, 
                    this, // 游戏实例
                    () => { 
                        // 继续游戏回调
                        StartNewGame();
                        hasCalledRepeatLogic = false;
                    }, 
                    () => { 
                        // 返回主窗口回调
                        ReturnToMainMenu();
                    });
                // 默认行为：如果还有筹码，继续游戏；否则返回主窗口
                if (playerChips <= 0)
                {
                    ReturnToMainMenu();
                }
                else
                {
                    StartNewGame();
                }
            }
            
            GUILayout.Space(20f * scaleFactor);
            GUILayout.EndVertical();
        }

        // 绘制底部操作按钮
        private void DrawBottomButtons()
        {
            // 如果是Rolling状态且动画结束，不显示底部按钮（避免重复）
            if (currentState == GameState.Rolling && !isAnimating)
            {
                return;
            }
            
            GUILayout.Space(20f * scaleFactor);
            
            GUI.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
            GUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            
            // 返回主窗口按钮
            GUIStyle exitButtonStyle = CreateButtonStyle(new Color(0.6f, 0.3f, 0.3f, 0.95f), new Color(0.7f, 0.4f, 0.4f, 0.95f), new Color(0.5f, 0.25f, 0.25f, 0.95f));
            exitButtonStyle.fontSize = Mathf.RoundToInt(13 * scaleFactor);
            
            if (GUILayout.Button("返回主窗口", exitButtonStyle, GUILayout.Width(120f * scaleFactor), GUILayout.Height(35f * scaleFactor)))
            {
                ReturnToMainMenu();
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        // 创建通用按钮样式
        private GUIStyle CreateButtonStyle(Color normalColor, Color hoverColor, Color activeColor)
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.active.textColor = Color.white;
            style.normal.background = MakeTex(1, 1, normalColor);
            style.hover.background = MakeTex(1, 1, hoverColor);
            style.active.background = MakeTex(1, 1, activeColor);
            return style;
        }

        // 获取结果文本颜色
        private Color GetResultColor(string message)
        {
            if (message.Contains("恭喜") || message.Contains("获得"))
            {
                return new Color(0.5f, 0.9f, 0.5f, 1f);
            }
            else if (message.Contains("遗憾") || message.Contains("错误"))
            {
                return new Color(0.9f, 0.5f, 0.5f, 1f);
            }
            else
            {
                return new Color(1f, 0.9f, 0.5f, 1f);
            }
        }
        
        // 绘制骰子
        private void DrawDice(int value)
        {
            GUIStyle diceStyle = new GUIStyle(GUI.skin.box);
            diceStyle.alignment = TextAnchor.MiddleCenter;
            diceStyle.fontSize = Mathf.RoundToInt(20 * scaleFactor);
            diceStyle.normal.background = MakeTex(1, 1, new Color(1f, 1f, 1f, 0.9f));
            
            // 如果是动画期间，骰子值会快速变化，所以使用动画中的骰子值
            GUILayout.Box(value.ToString(), diceStyle, GUILayout.Width(60f * scaleFactor), GUILayout.Height(60f * scaleFactor));
        }

        // 绘制更精美的骰子
        private void DrawStyledDice(int value)
        {
            // 骰子样式
            GUIStyle diceStyle = new GUIStyle(GUI.skin.box);
            diceStyle.alignment = TextAnchor.MiddleCenter;
            diceStyle.fontSize = Mathf.RoundToInt(22 * scaleFactor);
            diceStyle.fontStyle = FontStyle.Bold;
            
            // 根据骰子值设置不同颜色
            Color diceColor = GetDiceColor(value);
            diceStyle.normal.background = MakeTex(1, 1, diceColor);
            diceStyle.normal.textColor = GetDiceTextColor(diceColor);
            
            // 设置骰子的圆角效果（通过边框实现）
            diceStyle.border = new RectOffset(5, 5, 5, 5);
            diceStyle.margin = new RectOffset(5, 5, 5, 5);
            
            // 绘制骰子
            GUILayout.Box(value.ToString(), diceStyle, GUILayout.Width(70f * scaleFactor), GUILayout.Height(70f * scaleFactor));
        }

        // 根据骰子值获取骰子颜色
        private Color GetDiceColor(int value)
        {
            switch (value)
            {
                case 1:
                    return new Color(0.9f, 0.4f, 0.4f, 0.95f); // 红色
                case 2:
                    return new Color(0.4f, 0.7f, 0.9f, 0.95f); // 蓝色
                case 3:
                    return new Color(0.5f, 0.9f, 0.5f, 0.95f); // 绿色
                case 4:
                    return new Color(0.9f, 0.9f, 0.4f, 0.95f); // 黄色
                case 5:
                    return new Color(0.7f, 0.5f, 0.9f, 0.95f); // 紫色
                case 6:
                    return new Color(0.9f, 0.6f, 0.3f, 0.95f); // 橙色
                default:
                    return new Color(1f, 1f, 1f, 0.9f); // 白色
            }
        }

        // 根据背景色获取合适的文本颜色
        private Color GetDiceTextColor(Color bgColor)
        {
            // 计算颜色亮度
            float brightness = (bgColor.r * 0.299f + bgColor.g * 0.587f + bgColor.b * 0.114f);
            return brightness > 0.5f ? Color.black : Color.white;
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

        // 开始新游戏
        private void StartNewGame()
        {
            currentState = GameState.ChoosingMode;
            pot = 0;
            currentBet = 0;
            playerGuessInput = "";
            selectedMode = GameMode.CompareSize;
            
            // 重置AI玩家并随机设置筹码
            InitializeAIPlayers();
            RandomizeAIChips();
            
            string gameCurrencyType = UsePaperMoney ? "宝钞" : "金钞";
            Logger.LogInfo("骰子游戏已开始，使用" + gameCurrencyType);
        }
        
        // 随机设置AI筹码为玩家持有筹码的0.5到3倍范围
        private void RandomizeAIChips()
        {
            System.Random random = new System.Random();
            foreach (AIPlayer ai in aiPlayers)
            {
                // 生成0.5到3倍玩家筹码的随机数
                float randomFactor = (float)(random.NextDouble() * 2.5 + 0.5); // 0.5-3.0之间的随机数
                ai.Chips = Mathf.RoundToInt(playerChips * randomFactor);
            }
        }

        // 摇骰子
        private void RollDice()
        {
            System.Random rng = new System.Random();
            totalDiceSum = 0;
            
            // 玩家摇骰子
            for (int i = 0; i < diceValues.Length; i++)
            {
                diceValues[i] = rng.Next(1, 7); // 1-6的随机数
                totalDiceSum += diceValues[i];
            }
            
            // AI摇骰子
            foreach (var ai in aiPlayers)
            {
                ai.DiceSum = 0;
                for (int i = 0; i < ai.AIDiceValues.Length; i++)
                {
                    ai.AIDiceValues[i] = rng.Next(1, 7);
                    ai.DiceSum += ai.AIDiceValues[i];
                }
                
                // AI猜点数或奇偶
                if (selectedMode == GameMode.GuessPoint)
                {
                    ai.AIGuess = rng.Next(5, 31); // 5-30的随机数
                }
                else if (selectedMode == GameMode.GuessOddEven)
                {
                    ai.AIGuessIsEven = rng.Next(2) == 0; // 50%概率猜偶数，50%概率猜奇数
                }
            }
            
            // 判定胜负
            DetermineWinner();
            
            // 开始动画
            isAnimating = true;
            animationTimer = 0f;
            animationDuration = 1.5f; // 动画持续1.5秒
            // 复制最终骰子值到动画数组
            for (int i = 0; i < diceValues.Length; i++)
            {
                animatedDiceValues[i] = diceValues[i];
            }
            
            // 这里不直接设置为Rolling，而是在动画结束时设置
        }

        // 判定胜负
        private void DetermineWinner()
        {
            string currencyType = UsePaperMoney ? "宝钞" : "金钞";
            
            if (selectedMode == GameMode.CompareSize)
            {
                // 比大小模式：玩家与AI比总和，赢的AI数量决定奖励
                int winCount = 0;
                foreach (var ai in aiPlayers)
                {
                    if (totalDiceSum > ai.DiceSum)
                    {
                        winCount++;
                    }
                }
                
                int reward = winCount * 10; // 每赢一个AI奖励10筹码
                playerChips += reward;
                
                gameOverMessage = "你赢了" + winCount + "个AI玩家，获得" + reward + " " + currencyType;
                Logger.LogInfo(gameOverMessage);
            }
            else if (selectedMode == GameMode.GuessPoint)
            {
                // 猜点数模式：根据猜测的接近程度给予奖励
                int difference = Mathf.Abs(totalDiceSum - playerGuess);
                int reward = 0;
                
                if (difference == 0)
                {
                    reward = 100; // 完全猜对，奖励100筹码
                    gameOverMessage = "恭喜！你完全猜对了，获得" + reward + " " + currencyType;
                }
                else if (difference <= 2)
                {
                    reward = 50; // 非常接近，奖励50筹码
                    gameOverMessage = "非常接近！获得" + reward + " " + currencyType;
                }
                else if (difference <= 5)
                {
                    reward = 20; // 比较接近，奖励20筹码
                    gameOverMessage = "比较接近！获得" + reward + " " + currencyType;
                }
                else
                {
                    reward = 0; // 猜错了，没有奖励
                    gameOverMessage = "很遗憾，猜测错误。正确答案是" + totalDiceSum;
                }
                
                playerChips += reward;
                Logger.LogInfo(gameOverMessage);
            }
            else if (selectedMode == GameMode.GuessOddEven)
            {
                // 猜奇偶模式：判断玩家猜测是否正确
                bool isEven = totalDiceSum % 2 == 0;
                int reward = 0;
                
                if (playerGuessIsEven == isEven)
                {
                    // 猜对了，计算奖励
                    reward = 50;
                    
                    // 检查是否有庄家，如果有且庄家猜错，奖励翻倍
                    bool hasDealerWrong = false;
                    foreach (var ai in aiPlayers)
                    {
                        if (ai.IsDealer && ai.AIGuessIsEven != isEven)
                        {
                            hasDealerWrong = true;
                            break;
                        }
                    }
                    
                    if (hasDealerWrong)
                    {
                        reward *= 2; // 庄家猜错，奖励翻倍
                        gameOverMessage = "恭喜！你猜对了奇偶，而且庄家猜错了，获得" + reward + " " + currencyType;
                    }
                    else
                    {
                        gameOverMessage = "恭喜！你猜对了奇偶，获得" + reward + " " + currencyType;
                    }
                }
                else
                {
                    reward = 0; // 猜错了，没有奖励
                    gameOverMessage = "很遗憾，猜测错误。总和" + totalDiceSum + "是" + (isEven ? "偶数" : "奇数");
                }
                
                playerChips += reward;
                Logger.LogInfo(gameOverMessage);
            }
        }
        
        // 绘制游戏结束窗口
        private void DrawGameOverWindow(int windowID)
        {
            // 允许拖动窗口
            GUI.DragWindow(new Rect(0, 0, gameOverWindowRect.width, 20));
            
            // 设置字体大小
            int fontSize = Mathf.RoundToInt(14 * scaleFactor);
            GUI.skin.label.fontSize = fontSize;
            GUI.skin.button.fontSize = fontSize;
            
            GUILayout.BeginVertical();
            GUILayout.Space(20f * scaleFactor);
            
            // 显示游戏结束信息
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(gameOverMessage, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(30f * scaleFactor);
            
            // 按钮区域
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 继续游戏按钮
            if (GUILayout.Button("继续游戏", GUILayout.Width(100f * scaleFactor), GUILayout.Height(40f * scaleFactor)))
            {
                // 检查玩家是否还有筹码
                if (playerChips <= 0)
                {
                    Logger.LogInfo("你的筹码已用完，无法继续游戏");
                    ReturnToMainMenu();
                }
                else
                {
                    currentState = GameState.ChoosingMode;
                }
            }
            
            GUILayout.Space(20f * scaleFactor);
            
            // 返回主窗口按钮
            if (GUILayout.Button("返回主窗口", GUILayout.Width(100f * scaleFactor), GUILayout.Height(40f * scaleFactor)))
            {
                ReturnToMainMenu();
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20f * scaleFactor);
            
            GUILayout.EndVertical();
        }

        // 返回主窗口并保存结余货币
        private void ReturnToMainMenu()
        {
            // 保存结余货币
            RemainingMoney = playerChips;
            
            // 关闭骰子游戏窗口
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
