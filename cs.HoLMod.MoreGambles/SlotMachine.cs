using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;

namespace cs.HoLMod.MoreGambles
{
    public class SlotMachine : BaseUnityPlugin
    {
        // 窗口设置
        private Rect windowRect = new Rect(20, 20, 800, 650);
        public bool showMenu = false;
        public bool blockGameInput = false;
        private float scaleFactor = 1f;
        private const string CURRENT_VERSION = "2.0.0";

        // 货币系统相关
        public bool UsePaperMoney { get; set; } = true; // true=宝钞, false=金钞
        public int InitialChips { get; set; } = 1000;
        public int RemainingMoney { get; set; } = 0;

        // 游戏状态
        private enum GameState { NotStarted, ReadyToSpin, Spinning, ShowResult }
        private GameState currentState = GameState.NotStarted;
        
        // 老虎机相关属性
        private int[] reels = new int[3]; // 三个转轴的结果
        private int[] reelSymbols = new int[3]; // 三个转轴正在显示的符号
        private float[] reelSpeeds = new float[3]; // 每个转轴的速度
        private float[] reelDecay = new float[3]; // 每个转轴的减速因子
        private bool[] reelsStopped = new bool[3]; // 每个转轴是否已经停止
        private float spinStartTime; // 开始旋转的时间点
        
        // 符号定义
        private enum Symbol { Cherry, Lemon, Orange, Bell, Bar, Seven, Jackpot }
        
        // 玩家筹码
        private int playerChips = 1000;
        private int currentBet = 10;
        private int maxBet = 100;
        
        // 游戏结果信息
        private string resultMessage = "";
        private int winAmount = 0;
        private bool showWinAnimation = false;
        private float winAnimationTime = 0f;
        
        // 重复游戏逻辑相关
        private bool hasCalledRepeatLogic = false;

        // 符号权重系统
        private Dictionary<Symbol, int> symbolWeights = new Dictionary<Symbol, int>
        {
            { Symbol.Cherry, 15 },   // 高概率低奖励
            { Symbol.Lemon, 12 },
            { Symbol.Orange, 10 },
            { Symbol.Bell, 8 },
            { Symbol.Bar, 6 },
            { Symbol.Seven, 3 },    // 低概率高奖励
            { Symbol.Jackpot, 1 }   // 最低概率最高奖励
        };

        // 旋转曲线系统 - 模拟真实老虎机的物理旋转效果
        private class SpinCurve
        {
            private float acceleration = 0.2f;
            private float maxSpeed = 10f;
            private float deceleration = 0.15f;
            private bool isAccelerating = true;
            private bool isDecelerating = false;
            private bool isStopping = false;
            private float targetSpeed = 0f;
            private int targetSymbol = 0;
            private float stopTime = 0f;

            public float GetSpeed(float currentSpeed, float timeSinceStart, bool shouldStop = false)
            {
                // 加速阶段
                if (isAccelerating)
                {
                    currentSpeed += acceleration * timeSinceStart * 5f;
                    if (currentSpeed >= maxSpeed)
                    {
                        currentSpeed = maxSpeed;
                        isAccelerating = false;
                    }
                }
                // 减速停止阶段
                else if (shouldStop || isDecelerating)
                {
                    if (!isDecelerating)
                    {
                        isDecelerating = true;
                        // 根据目标符号计算需要旋转的额外步数
                        stopTime = Time.time;
                    }
                    
                    currentSpeed -= deceleration * (Time.time - stopTime) * 10f;
                    if (currentSpeed <= 0f)
                    {
                        currentSpeed = 0f;
                        isStopping = true;
                    }
                }
                
                return currentSpeed;
            }

            public bool IsStopping { get { return isStopping; } }
        }

        private SpinCurve[] spinCurves = new SpinCurve[3];

        public void Awake()
        {
            // 初始化转轴数组
            for (int i = 0; i < reels.Length; i++)
            {
                reels[i] = 0;
                reelSymbols[i] = 0;
                reelSpeeds[i] = 0f;
                reelDecay[i] = 0.1f;
                reelsStopped[i] = false;
                spinCurves[i] = new SpinCurve();
            }

            // 设置玩家初始筹码
            playerChips = InitialChips;
            
            // 更新分辨率设置
            UpdateResolutionSettings();
        }

        private void Update()
        {
            // 按F5键切换窗口显示
            if (Input.GetKeyDown(KeyCode.F5))
            {
                UpdateResolutionSettings();
                showMenu = !showMenu;
                blockGameInput = showMenu;
                Logger.LogInfo(showMenu ? "老虎机窗口已打开" : "老虎机窗口已关闭");
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

            // 更新老虎机转轴状态
            if (currentState == GameState.Spinning)
            {
                UpdateReels();
            }
        }

        private void OnGUI()
        {
            if (!showMenu) return;

            // 保存窗口背景色并设置为半透明
            Color originalBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.15f, 0.15f, 0.3f, 0.98f); // 深蓝色老虎机风格背景色

            // 显示一个半透明的更深背景遮罩
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
            GUI.color = new Color(0, 0, 0, 0.6f);
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

            // 创建主游戏窗口
            windowRect = GUI.Window(0, windowRect, DrawWindow, "老虎机 - 高级版", GUI.skin.window);

            // 恢复原始矩阵和背景色
            GUI.matrix = guiMatrix;
            GUI.backgroundColor = originalBackgroundColor;
        }

        private void DrawWindow(int windowID)
        {
            // 允许拖动窗口
            GUI.DragWindow(new Rect(0, 0, windowRect.width, 30));

            // 设置字体大小
            int fontSize = Mathf.RoundToInt(14 * scaleFactor);
            GUI.skin.label.fontSize = fontSize;
            GUI.skin.button.fontSize = fontSize;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            GUI.skin.window.fontSize = fontSize;

            // 窗口最小宽度和高度
            windowRect.width = Mathf.Max(windowRect.width, 800f * scaleFactor);
            windowRect.height = Mathf.Max(windowRect.height, 650f * scaleFactor);

            GUILayout.BeginVertical();

            // 标题和版本信息区域 - 带有金色渐变背景
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            GUIStyle headerStyle = new GUIStyle();
            headerStyle.normal.background = MakeGradientTex(100, 30, new Color(0.9f, 0.75f, 0.2f), new Color(0.7f, 0.5f, 0.1f));
            headerStyle.padding = new RectOffset(Mathf.RoundToInt(30 * scaleFactor), Mathf.RoundToInt(30 * scaleFactor), Mathf.RoundToInt(15 * scaleFactor), Mathf.RoundToInt(15 * scaleFactor));
            
            GUILayout.BeginVertical(headerStyle, GUILayout.Width(400f * scaleFactor));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("豪华老虎机", new GUIStyle(GUI.skin.label) 
            {
                fontSize = Mathf.RoundToInt(24 * scaleFactor), 
                normal = { textColor = new Color(0.8f, 0.8f, 0f) },
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Version " + CURRENT_VERSION, new GUIStyle(GUI.skin.label) 
            { 
                fontSize = Mathf.RoundToInt(10 * scaleFactor), 
                normal = { textColor = new Color(0.9f, 0.9f, 0.6f) },
                alignment = TextAnchor.MiddleCenter
            });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20f * scaleFactor);

            // 当前余额和下注信息 - 更现代的面板设计
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 玩家信息面板 - 带阴影和圆角
            GUIStyle infoPanelStyle = new GUIStyle(GUI.skin.box);
            infoPanelStyle.normal.background = MakeTex(1, 1, new Color(0.25f, 0.25f, 0.45f, 0.95f));
            infoPanelStyle.padding = new RectOffset(
                Mathf.RoundToInt(20 * scaleFactor),
                Mathf.RoundToInt(20 * scaleFactor),
                Mathf.RoundToInt(15 * scaleFactor),
                Mathf.RoundToInt(15 * scaleFactor)
            );
            
            // 添加阴影效果
            GUI.skin.box.normal.background = MakeShadowTex(300 * scaleFactor, 150 * scaleFactor, 10f);
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(320f * scaleFactor), GUILayout.Height(170f * scaleFactor));
            GUILayout.Space(10f * scaleFactor);
            
            GUILayout.BeginVertical(infoPanelStyle);
            
            string currencyType = UsePaperMoney ? "宝钞" : "金钞";
            
            // 余额显示 - 更大更突出
            GUILayout.BeginHorizontal();
            GUILayout.Label("余额：", new GUIStyle(GUI.skin.label) 
            { 
                fontSize = Mathf.RoundToInt(16 * scaleFactor),
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f) },
                fontStyle = FontStyle.Bold
            });
            GUILayout.FlexibleSpace();
            GUILayout.Label(playerChips.ToString(), new GUIStyle(GUI.skin.label) 
            { 
                fontSize = Mathf.RoundToInt(18 * scaleFactor),
                normal = { textColor = new Color(1f, 0.9f, 0.2f) }, 
                alignment = TextAnchor.MiddleRight,
                fontStyle = FontStyle.Bold
            });
            GUILayout.EndHorizontal();
            GUILayout.Space(10f * scaleFactor);
            
            // 当前下注显示
            GUILayout.BeginHorizontal();
            GUILayout.Label("下注：", new GUIStyle(GUI.skin.label) 
            { 
                fontSize = Mathf.RoundToInt(16 * scaleFactor),
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f) },
                fontStyle = FontStyle.Bold
            });
            GUILayout.FlexibleSpace();
            GUILayout.Label(currentBet + " " + currencyType, new GUIStyle(GUI.skin.label) 
            { 
                fontSize = Mathf.RoundToInt(16 * scaleFactor),
                normal = { textColor = new Color(0.5f, 0.9f, 0.5f) }, 
                alignment = TextAnchor.MiddleRight
            });
            GUILayout.EndHorizontal();
            GUILayout.Space(10f * scaleFactor);
            
            // 游戏状态显示 - 带动态颜色
            GUILayout.BeginHorizontal();
            GUILayout.Label("状态：", new GUIStyle(GUI.skin.label) 
            { 
                fontSize = Mathf.RoundToInt(16 * scaleFactor),
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f) },
                fontStyle = FontStyle.Bold
            });
            GUILayout.FlexibleSpace();
            string stateText = "准备就绪";
            Color stateColor = new Color(0.4f, 0.7f, 1f);
            if (currentState == GameState.Spinning) {
                stateText = "旋转中...";
                stateColor = new Color(1f, 0.4f, 0.4f);
            } else if (currentState == GameState.ShowResult) {
                stateText = "查看结果";
                stateColor = new Color(0.4f, 1f, 0.4f);
            }
            
            // 状态指示器带背景色
            GUIStyle stateIndicatorStyle = new GUIStyle(GUI.skin.label);
            stateIndicatorStyle.normal.background = MakeTex(1, 1, new Color(stateColor.r * 0.2f, stateColor.g * 0.2f, stateColor.b * 0.2f, 0.8f));
            stateIndicatorStyle.padding = new RectOffset(10, 10, 5, 5);
            stateIndicatorStyle.alignment = TextAnchor.MiddleCenter;
            
            GUILayout.Label(stateText, new GUIStyle(stateIndicatorStyle) 
            { 
                fontSize = Mathf.RoundToInt(14 * scaleFactor),
                normal = { textColor = stateColor }
            }, GUILayout.Width(100f * scaleFactor));
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(25f * scaleFactor);

            // 老虎机主体显示区域 - 豪华设计
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 老虎机外壳 - 金色渐变边框
            GUIStyle machineOuterStyle = new GUIStyle();
            machineOuterStyle.normal.background = MakeGradientTex(1, 1, new Color(0.95f, 0.8f, 0.3f), new Color(0.7f, 0.5f, 0.1f));
            machineOuterStyle.padding = new RectOffset(
                Mathf.RoundToInt(25 * scaleFactor),
                Mathf.RoundToInt(25 * scaleFactor),
                Mathf.RoundToInt(25 * scaleFactor),
                Mathf.RoundToInt(25 * scaleFactor)
            );
            
            // 添加外部阴影
            GUI.skin.box.normal.background = MakeShadowTex(450 * scaleFactor, 300 * scaleFactor, 15f);
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(480f * scaleFactor), GUILayout.Height(330f * scaleFactor));
            GUILayout.Space(15f * scaleFactor);
            
            GUILayout.BeginVertical(machineOuterStyle);
            
            // 内部黑色背景
            GUIStyle innerBackgroundStyle = new GUIStyle();
            innerBackgroundStyle.normal.background = MakeGradientTex(1, 1, new Color(0.05f, 0.05f, 0.05f), new Color(0.15f, 0.15f, 0.15f));
            innerBackgroundStyle.padding = new RectOffset(
                Mathf.RoundToInt(20 * scaleFactor),
                Mathf.RoundToInt(20 * scaleFactor),
                Mathf.RoundToInt(20 * scaleFactor),
                Mathf.RoundToInt(20 * scaleFactor)
            );
            
            GUILayout.BeginVertical(innerBackgroundStyle);
            
            // 装饰性顶部灯条
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 创建彩色灯条效果
            GUIStyle lightBarStyle = new GUIStyle();
            lightBarStyle.normal.background = MakeColorfulLightTex(300 * scaleFactor, 10 * scaleFactor);
            GUILayout.Box("", lightBarStyle, GUILayout.Width(300f * scaleFactor), GUILayout.Height(10f * scaleFactor));
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20f * scaleFactor);
            
            // 三个转轴显示
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 转轴1
            DrawReel(0);
            GUILayout.Space(20f * scaleFactor);
            
            // 转轴2
            DrawReel(1);
            GUILayout.Space(20f * scaleFactor);
            
            // 转轴3
            DrawReel(2);
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(20f * scaleFactor);
            
            // 装饰性底部灯条
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box("", lightBarStyle, GUILayout.Width(300f * scaleFactor), GUILayout.Height(10f * scaleFactor));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(25f * scaleFactor);
            
            // 游戏结果信息 - 更炫的动画效果
            if (!string.IsNullOrEmpty(resultMessage))
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                
                // 中奖动画效果增强
                float animationScale = 1f;
                Color textColor = winAmount > 0 ? new Color(1f, 0.9f, 0.2f) : new Color(0.9f, 0.9f, 0.9f);
                if (winAmount > 0 && showWinAnimation)
                {
                    // 更复杂的动画效果：缩放 + 颜色变化
                    animationScale = 1f + Mathf.Abs(Mathf.Sin(Time.time * 10f)) * 0.15f;
                    textColor = Color.Lerp(new Color(1f, 0.9f, 0.2f), new Color(1f, 0.5f, 0.2f), Mathf.Abs(Mathf.Sin(Time.time * 5f)));
                }
                
                // 结果信息面板带背景
                GUIStyle resultPanelStyle = new GUIStyle();
                resultPanelStyle.normal.background = MakeTex(1, 1, new Color(0.15f, 0.15f, 0.3f, 0.8f));
                resultPanelStyle.padding = new RectOffset(30, 30, 15, 15);
                
                // 保存当前GUI矩阵以应用缩放
                Matrix4x4 originalMatrix = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(new Vector3(animationScale, animationScale, 1f));
                
                GUILayout.Label(resultMessage, new GUIStyle(resultPanelStyle) 
                {
                    fontSize = Mathf.RoundToInt(18 * scaleFactor), 
                    alignment = TextAnchor.MiddleCenter, 
                    normal = { textColor = textColor },
                    fontStyle = FontStyle.Bold
                });
                
                // 恢复原始矩阵
                GUI.matrix = originalMatrix;
                
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(20f * scaleFactor);
            }

            // 下注调整按钮 - 更现代的滑块设计
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 下注调整面板 - 带背景
            GUIStyle betPanelStyle = new GUIStyle();
            betPanelStyle.normal.background = MakeGradientTex(1, 1, new Color(0.25f, 0.25f, 0.45f, 0.95f), new Color(0.15f, 0.15f, 0.35f, 0.95f));
            betPanelStyle.padding = new RectOffset(
                Mathf.RoundToInt(20 * scaleFactor),
                Mathf.RoundToInt(20 * scaleFactor),
                Mathf.RoundToInt(15 * scaleFactor),
                Mathf.RoundToInt(15 * scaleFactor)
            );
            
            // 添加阴影
            GUI.skin.box.normal.background = MakeShadowTex(300 * scaleFactor, 80 * scaleFactor, 10f);
            GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(320f * scaleFactor), GUILayout.Height(100f * scaleFactor));
            GUILayout.Space(10f * scaleFactor);
            
            GUILayout.BeginHorizontal(betPanelStyle);
            
            // 减注按钮 - 圆形按钮设计
            GUIStyle betButtonStyle = new GUIStyle(GUI.skin.button);
            betButtonStyle.normal.background = MakeCircleTex(50 * scaleFactor, new Color(0.7f, 0.2f, 0.2f, 0.9f));
            betButtonStyle.hover.background = MakeCircleTex(50 * scaleFactor, new Color(0.8f, 0.3f, 0.3f, 0.9f));
            betButtonStyle.active.background = MakeCircleTex(50 * scaleFactor, new Color(0.9f, 0.4f, 0.4f, 0.9f));
            betButtonStyle.normal.textColor = Color.white;
            betButtonStyle.fontSize = Mathf.RoundToInt(18 * scaleFactor);
            
            bool canDecreaseBet = currentState == GameState.ReadyToSpin && currentBet > 10;
            if (canDecreaseBet) {
                betButtonStyle.normal.background = MakeCircleTex(50 * scaleFactor, new Color(0.7f, 0.2f, 0.2f, 0.9f));
            } else {
                betButtonStyle.normal.background = MakeCircleTex(50 * scaleFactor, new Color(0.4f, 0.4f, 0.4f, 0.7f));
            }
            
            if (GUILayout.Button("-", betButtonStyle, GUILayout.Width(50f * scaleFactor), GUILayout.Height(50f * scaleFactor)) && canDecreaseBet)
            {
                currentBet -= 10;
                PlayButtonSound();
            }
            
            GUILayout.Space(20f * scaleFactor);
            
            // 下注金额显示 - 更大更突出
            GUIStyle betAmountStyle = new GUIStyle(GUI.skin.label);
            betAmountStyle.normal.background = MakeTex(1, 1, new Color(0.15f, 0.15f, 0.35f, 0.8f));
            betAmountStyle.padding = new RectOffset(20, 20, 10, 10);
            betAmountStyle.fontSize = Mathf.RoundToInt(22 * scaleFactor);
            betAmountStyle.alignment = TextAnchor.MiddleCenter;
            betAmountStyle.normal.textColor = new Color(1f, 0.9f, 0.2f);
            betAmountStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label(currentBet.ToString(), betAmountStyle, GUILayout.Width(100f * scaleFactor), GUILayout.Height(50f * scaleFactor));
            
            GUILayout.Space(20f * scaleFactor);
            
            // 加注按钮 - 圆形按钮设计
            bool canIncreaseBet = currentState == GameState.ReadyToSpin && currentBet < playerChips && currentBet < maxBet;
            if (canIncreaseBet) {
                betButtonStyle.normal.background = MakeCircleTex(50 * scaleFactor, new Color(0.2f, 0.7f, 0.2f, 0.9f));
            } else {
                betButtonStyle.normal.background = MakeCircleTex(50 * scaleFactor, new Color(0.4f, 0.4f, 0.4f, 0.7f));
            }
            
            if (GUILayout.Button("+", betButtonStyle, GUILayout.Width(50f * scaleFactor), GUILayout.Height(50f * scaleFactor)) && canIncreaseBet)
            {
                currentBet += 10;
                PlayButtonSound();
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(25f * scaleFactor);

            // 操作按钮 - 大型发光按钮
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // 添加按钮阴影
            GUI.skin.box.normal.background = MakeShadowTex(250 * scaleFactor, 80 * scaleFactor, 15f);
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(270f * scaleFactor), GUILayout.Height(100f * scaleFactor));
            GUILayout.Space(10f * scaleFactor);
            
            GUIStyle spinButtonStyle = new GUIStyle(GUI.skin.button);
            spinButtonStyle.fontSize = Mathf.RoundToInt(20 * scaleFactor);
            spinButtonStyle.normal.textColor = Color.white;
            spinButtonStyle.fontStyle = FontStyle.Bold;
            spinButtonStyle.padding = new RectOffset(
                Mathf.RoundToInt(15 * scaleFactor),
                Mathf.RoundToInt(15 * scaleFactor),
                Mathf.RoundToInt(15 * scaleFactor),
                Mathf.RoundToInt(15 * scaleFactor)
            );
            
            if (currentState == GameState.ReadyToSpin)
            {
                // 绿色渐变按钮
                spinButtonStyle.normal.background = MakeGradientTex(1, 1, new Color(0.3f, 0.8f, 0.3f), new Color(0.1f, 0.6f, 0.1f));
                spinButtonStyle.hover.background = MakeGradientTex(1, 1, new Color(0.4f, 0.9f, 0.4f), new Color(0.2f, 0.7f, 0.2f));
                spinButtonStyle.active.background = MakeGradientTex(1, 1, new Color(0.2f, 0.7f, 0.2f), new Color(0.05f, 0.5f, 0.05f));
                
                if (GUILayout.Button("开始游戏", spinButtonStyle, GUILayout.Width(250f * scaleFactor), GUILayout.Height(80f * scaleFactor)))
                {
                    if (playerChips >= currentBet)
                    {
                        SpinReels();
                        PlaySpinSound();
                    }
                    else
                    {
                        resultMessage = "筹码不足！";
                        winAmount = 0;
                        PlayErrorSound();
                    }
                }
            }
            else if (currentState == GameState.Spinning)
            {
                // 红色渐变按钮
                spinButtonStyle.normal.background = MakeGradientTex(1, 1, new Color(0.8f, 0.3f, 0.3f), new Color(0.6f, 0.1f, 0.1f));
                GUILayout.Button("旋转中...", spinButtonStyle, GUILayout.Width(250f * scaleFactor), GUILayout.Height(80f * scaleFactor));
            }
            else if (currentState == GameState.ShowResult)
            {
                // 游戏结束后，调用重复游戏逻辑
                if (!hasCalledRepeatLogic)
                {                    
                    hasCalledRepeatLogic = true;
                    
                    // 直接调用静态方法，传入必要参数
                    B_RepeatTheGame.HandleGameEnd(
                        playerChips, 
                        this, // 游戏实例
                        () => { 
                            // 继续游戏回调
                            PrepareForNextSpin();
                            hasCalledRepeatLogic = false;
                            PlayButtonSound();
                        }, 
                        () => { 
                            // 返回主窗口回调
                            ReturnToMainMenu();
                            PlayButtonSound();
                        });
                }
                
                // 显示游戏结束状态
                spinButtonStyle.normal.background = MakeGradientTex(1, 1, new Color(0.5f, 0.5f, 0.5f, 0.5f), new Color(0.3f, 0.3f, 0.3f, 0.5f));
                GUILayout.Button("游戏结束", spinButtonStyle, GUILayout.Width(250f * scaleFactor), GUILayout.Height(80f * scaleFactor));
            }
            
            GUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
        
        // 创建渐变纹理
        private Texture2D MakeGradientTex(int width, int height, Color topColor, Color bottomColor)
        {
            Texture2D texture = new Texture2D(width, height);
            for (int y = 0; y < height; y++)
            {
                float t = (float)y / (float)(height - 1);
                Color color = Color.Lerp(topColor, bottomColor, t);
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();
            return texture;
        }
        
        // 创建圆形纹理
        private Texture2D MakeCircleTex(float size, Color color)
        {
            int diameter = Mathf.RoundToInt(size);
            Texture2D texture = new Texture2D(diameter, diameter);
            float radius = diameter / 2f;
            Vector2 center = new Vector2(radius, radius);
            
            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    if (distance <= radius)
                    {
                        texture.SetPixel(x, y, color);
                    }
                    else
                    {
                        texture.SetPixel(x, y, new Color(0, 0, 0, 0));
                    }
                }
            }
            
            texture.Apply();
            return texture;
        }
        
        // 创建阴影纹理
        private Texture2D MakeShadowTex(float width, float height, float blurRadius)
        {
            int w = Mathf.RoundToInt(width);
            int h = Mathf.RoundToInt(height);
            Texture2D texture = new Texture2D(w, h);
            
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // 简单的阴影效果
                    float alpha = 0.15f;
                    texture.SetPixel(x, y, new Color(0, 0, 0, alpha));
                }
            }
            
            texture.Apply();
            return texture;
        }
        
        // 创建彩色灯条纹理
        private Texture2D MakeColorfulLightTex(float width, float height)
        {
            int w = Mathf.RoundToInt(width);
            int h = Mathf.RoundToInt(height);
            Texture2D texture = new Texture2D(w, h);
            
            // 创建彩虹色灯条效果
            for (int x = 0; x < w; x++)
            {
                float hue = (float)x / (float)w;
                Color color = Color.HSVToRGB(hue, 0.8f, 0.9f);
                for (int y = 0; y < h; y++)
                {
                    texture.SetPixel(x, y, color);
                }
            }
            
            texture.Apply();
            return texture;
        }

        // 绘制单个转轴 - 增强视觉效果
        private void DrawReel(int reelIndex)
        {
            // 根据符号类型设置不同的背景色和文本色
            Symbol symbol = (Symbol)reelSymbols[reelIndex];
            Color reelBgColor = new Color(0.2f, 0.2f, 0.2f, 0.95f);
            Color textColor = Color.white;
            
            switch (symbol)
            {
                case Symbol.Cherry:
                    reelBgColor = new Color(0.4f, 0.1f, 0.1f, 0.95f);
                    break;
                case Symbol.Lemon:
                    reelBgColor = new Color(0.3f, 0.4f, 0.1f, 0.95f);
                    break;
                case Symbol.Orange:
                    reelBgColor = new Color(0.5f, 0.3f, 0.1f, 0.95f);
                    break;
                case Symbol.Bell:
                    reelBgColor = new Color(0.5f, 0.5f, 0.1f, 0.95f);
                    break;
                case Symbol.Bar:
                    reelBgColor = new Color(0.3f, 0.3f, 0.3f, 0.95f);
                    break;
                case Symbol.Seven:
                    reelBgColor = new Color(0.1f, 0.1f, 0.4f, 0.95f);
                    textColor = new Color(1f, 0.9f, 0.2f);
                    break;
                case Symbol.Jackpot:
                    reelBgColor = new Color(0.6f, 0.5f, 0.1f, 0.95f);
                    textColor = new Color(1f, 0.9f, 0.2f);
                    break;
            }
            
            // 装饰性外框 - 金色渐变
            GUIStyle outerBorderStyle = new GUIStyle();
            outerBorderStyle.normal.background = MakeGradientTex(1, 1, new Color(0.95f, 0.8f, 0.3f), new Color(0.7f, 0.5f, 0.1f));
            outerBorderStyle.padding = new RectOffset(
                Mathf.RoundToInt(5 * scaleFactor),
                Mathf.RoundToInt(5 * scaleFactor),
                Mathf.RoundToInt(5 * scaleFactor),
                Mathf.RoundToInt(5 * scaleFactor)
            );
            
            GUILayout.BeginVertical(outerBorderStyle);
            
            // 内部黑色边框
            GUIStyle innerBorderStyle = new GUIStyle();
            innerBorderStyle.normal.background = MakeTex(1, 1, new Color(0.05f, 0.05f, 0.05f, 0.95f));
            innerBorderStyle.padding = new RectOffset(
                Mathf.RoundToInt(5 * scaleFactor),
                Mathf.RoundToInt(5 * scaleFactor),
                Mathf.RoundToInt(5 * scaleFactor),
                Mathf.RoundToInt(5 * scaleFactor)
            );
            
            GUILayout.BeginVertical(innerBorderStyle);
            
            // 创建转轴样式
            GUIStyle reelStyle = new GUIStyle(GUI.skin.box);
            reelStyle.alignment = TextAnchor.MiddleCenter;
            reelStyle.fontSize = Mathf.RoundToInt(32 * scaleFactor); // 更大的字体
            reelStyle.fontStyle = FontStyle.Bold;
            
            // 旋转中的动画效果增强
            if (currentState == GameState.Spinning && !reelsStopped[reelIndex])
            {
                // 添加更明显的旋转动画效果
                float spinOffset = Mathf.Abs(Mathf.Sin(Time.time * 10f)) * 5f;
                
                // 动态背景色变化增强视觉效果
                float colorOffset = Mathf.Abs(Mathf.Sin(Time.time * 5f)) * 0.2f;
                Color dynamicBgColor = new Color(
                    Mathf.Clamp(reelBgColor.r + colorOffset, 0f, 1f),
                    Mathf.Clamp(reelBgColor.g + colorOffset, 0f, 1f),
                    Mathf.Clamp(reelBgColor.b + colorOffset, 0f, 1f),
                    reelBgColor.a
                );
                
                reelStyle.normal.background = MakeGradientTex(1, 1, dynamicBgColor, new Color(dynamicBgColor.r * 0.8f, dynamicBgColor.g * 0.8f, dynamicBgColor.b * 0.8f, dynamicBgColor.a));
                reelStyle.normal.textColor = new Color(textColor.r, textColor.g, textColor.b, 0.7f);
                
                // 保存当前GUI矩阵以应用旋转动画
                Matrix4x4 originalMatrix = GUI.matrix;
                GUI.matrix = Matrix4x4.TRS(
                    new Vector3(spinOffset, 0, 0), 
                    Quaternion.identity, 
                    new Vector3(1.05f, 1.05f, 1f)
                );
                
                GUILayout.Box(GetSymbolText(reelSymbols[reelIndex]), reelStyle, GUILayout.Width(100f * scaleFactor), GUILayout.Height(120f * scaleFactor));
                
                // 恢复原始矩阵
                GUI.matrix = originalMatrix;
            }
            else if (winAmount > 0 && reelSymbols[reelIndex] == reelSymbols[(reelIndex + 1) % 3] && reelSymbols[reelIndex] == reelSymbols[(reelIndex + 2) % 3])
            {
                // 中奖符号的特殊效果
                float animationScale = 1f + Mathf.Abs(Mathf.Sin(Time.time * 10f)) * 0.1f;
                float glowIntensity = 0.5f + Mathf.Abs(Mathf.Sin(Time.time * 5f)) * 0.5f;
                
                // 创建发光效果背景
                Color glowColor = new Color(
                    textColor.r * glowIntensity, 
                    textColor.g * glowIntensity, 
                    textColor.b * glowIntensity, 
                    0.3f
                );
                
                reelStyle.normal.background = MakeGradientTex(1, 1, reelBgColor, new Color(reelBgColor.r * 0.7f, reelBgColor.g * 0.7f, reelBgColor.b * 0.7f, reelBgColor.a));
                reelStyle.normal.textColor = textColor;
                
                // 保存当前GUI矩阵以应用缩放
                Matrix4x4 originalMatrix = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(new Vector3(animationScale, animationScale, 1f));
                
                // 绘制发光效果
                GUI.skin.box.normal.background = MakeTex(1, 1, glowColor);
                GUILayout.Box("", GUI.skin.box, GUILayout.Width(110f * scaleFactor), GUILayout.Height(130f * scaleFactor));
                
                GUILayout.Box(GetSymbolText(reelSymbols[reelIndex]), reelStyle, GUILayout.Width(100f * scaleFactor), GUILayout.Height(120f * scaleFactor));
                
                // 恢复原始矩阵
                GUI.matrix = originalMatrix;
            }
            else
            {
                // 静态显示 - 添加渐变效果
                reelStyle.normal.background = MakeGradientTex(1, 1, reelBgColor, new Color(reelBgColor.r * 0.7f, reelBgColor.g * 0.7f, reelBgColor.b * 0.7f, reelBgColor.a));
                reelStyle.normal.textColor = textColor;
                
                GUILayout.Box(GetSymbolText(reelSymbols[reelIndex]), reelStyle, GUILayout.Width(100f * scaleFactor), GUILayout.Height(120f * scaleFactor));
            }
            
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        // 获取符号的文本表示
        private string GetSymbolText(int symbolIndex)
        {
            switch ((Symbol)symbolIndex)
            {
                case Symbol.Cherry:
                    return "🍒";
                case Symbol.Lemon:
                    return "🍋";
                case Symbol.Orange:
                    return "🍊";
                case Symbol.Bell:
                    return "🔔";
                case Symbol.Bar:
                    return "BAR";
                case Symbol.Seven:
                    return "7";
                case Symbol.Jackpot:
                    return "💰";
                default:
                    return "?";
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

        // 更新转轴状态
        private void UpdateReels()
        {
            bool allReelsStopped = true;
            float timeSinceStart = Time.time - spinStartTime;
            
            // 依次停止转轴的逻辑
            bool shouldStopFirstReel = timeSinceStart > 1.0f;
            bool shouldStopSecondReel = timeSinceStart > 1.8f;
            bool shouldStopThirdReel = timeSinceStart > 2.6f;
            
            for (int i = 0; i < reels.Length; i++)
            {
                if (!reelsStopped[i])
                {
                    bool shouldStopThisReel = false;
                    if (i == 0 && shouldStopFirstReel) shouldStopThisReel = true;
                    if (i == 1 && shouldStopSecondReel) shouldStopThisReel = true;
                    if (i == 2 && shouldStopThirdReel) shouldStopThisReel = true;
                    
                    // 使用旋转曲线计算当前速度
                    reelSpeeds[i] = spinCurves[i].GetSpeed(reelSpeeds[i], timeSinceStart, shouldStopThisReel);
                    
                    if (spinCurves[i].IsStopping)
                    {
                        reelSpeeds[i] = 0f;
                        reelsStopped[i] = true;
                        reelSymbols[i] = reels[i];
                        
                        // 播放转轴停止音效
                        PlayReelStopSound();
                    }
                    else
                    {
                        allReelsStopped = false;
                        // 旋转中，更新显示的符号
                        float rotationProgress = Time.time * reelSpeeds[i];
                        reelSymbols[i] = Mathf.FloorToInt(rotationProgress) % 7;
                    }
                }
            }
            
            // 中奖动画效果更新
            if (showWinAnimation)
            {
                winAnimationTime += Time.deltaTime;
                if (winAnimationTime > 3f) // 3秒后结束中奖动画
                {
                    showWinAnimation = false;
                    winAnimationTime = 0f;
                }
            }
            
            // 如果所有转轴都停止了，显示结果
            if (allReelsStopped && currentState == GameState.Spinning)
            {
                ShowSpinResult();
            }
        }

        // 使用权重系统选择符号
        private Symbol SelectSymbolByWeight()
        {
            // 计算总权重
            int totalWeight = 0;
            foreach (var weight in symbolWeights.Values)
            {
                totalWeight += weight;
            }
            
            // 生成随机数
            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            
            // 根据权重选择符号
            int accumulatedWeight = 0;
            foreach (var pair in symbolWeights)
            {
                accumulatedWeight += pair.Value;
                if (randomValue < accumulatedWeight)
                {
                    return pair.Key;
                }
            }
            
            // 默认返回第一个符号
            return symbolWeights.Keys.First();
        }
        
        // 开始旋转转轴
        private void SpinReels()
        {
            currentState = GameState.Spinning;
            spinStartTime = Time.time;
            
            // 扣除下注金额
            playerChips -= currentBet;
            
            // 重置转轴状态
            for (int i = 0; i < reels.Length; i++)
            {
                // 使用权重系统选择最终符号
                reels[i] = (int)SelectSymbolByWeight();
                reelSpeeds[i] = 2f + i * 0.5f; // 初始速度
                reelsStopped[i] = false;
                spinCurves[i] = new SpinCurve();
            }
            
            resultMessage = "";
            winAmount = 0;
            showWinAnimation = false;
        }

        // 准备下一次旋转
        private void PrepareForNextSpin()
        {
            currentState = GameState.ReadyToSpin;
            resultMessage = "";
            winAmount = 0;
        }

        // 显示旋转结果
        private void ShowSpinResult()
        {
            currentState = GameState.ShowResult;
            
            // 检查是否中奖
            CheckWinningCombination();
            
            string currencyType = UsePaperMoney ? "宝钞" : "金钞";
            
            if (winAmount > 0)
            {
                resultMessage = "恭喜中奖！获得" + winAmount + " " + currencyType;
                playerChips += winAmount;
                showWinAnimation = true;
                winAnimationTime = 0f;
                
                // 播放中奖音效
                PlayWinSound();
            }
            else
            {
                resultMessage = "再接再厉！";
                PlayLoseSound();
            }
            
            Logger.LogInfo("老虎机旋转结果: " + string.Join(", ", reelSymbols.Select(s => GetSymbolText(s))) + ", " + resultMessage);
        }

        // 检查中奖组合
        private void CheckWinningCombination()
        {
            winAmount = 0;
            
            // 定义中奖倍率表
            Dictionary<Symbol, int> threeOfAKindMultipliers = new Dictionary<Symbol, int>
            {
                { Symbol.Cherry, 5 },
                { Symbol.Lemon, 10 },
                { Symbol.Orange, 15 },
                { Symbol.Bell, 25 },
                { Symbol.Bar, 50 },
                { Symbol.Seven, 100 },
                { Symbol.Jackpot, 500 }
            };
            
            Dictionary<Symbol, int> twoOfAKindMultipliers = new Dictionary<Symbol, int>
            {
                { Symbol.Cherry, 2 },
                { Symbol.Lemon, 3 },
                { Symbol.Orange, 4 },
                { Symbol.Bell, 6 },
                { Symbol.Bar, 10 },
                { Symbol.Seven, 20 },
                { Symbol.Jackpot, 50 }
            };
            
            // 三个相同符号 - 最高奖励
            if (reelSymbols[0] == reelSymbols[1] && reelSymbols[1] == reelSymbols[2])
            {
                Symbol symbol = (Symbol)reelSymbols[0];
                if (threeOfAKindMultipliers.ContainsKey(symbol))
                {
                    winAmount = currentBet * threeOfAKindMultipliers[symbol];
                }
            }
            // 两个相同符号 - 中等奖励
            else if (reelSymbols[0] == reelSymbols[1] || reelSymbols[1] == reelSymbols[2])
            {
                Symbol symbol = (Symbol)(reelSymbols[0] == reelSymbols[1] ? reelSymbols[0] : reelSymbols[1]);
                if (twoOfAKindMultipliers.ContainsKey(symbol))
                {
                    winAmount = currentBet * twoOfAKindMultipliers[symbol];
                }
            }
            // 特殊组合：任意两个Seven - 特殊奖励
            else if ((reelSymbols[0] == (int)Symbol.Seven && reelSymbols[1] == (int)Symbol.Seven) ||
                     (reelSymbols[1] == (int)Symbol.Seven && reelSymbols[2] == (int)Symbol.Seven) ||
                     (reelSymbols[0] == (int)Symbol.Seven && reelSymbols[2] == (int)Symbol.Seven))
            {
                winAmount = currentBet * 15;
            }
            // 特殊组合：Cherry组合（至少两个Cherry）
            else if (
                (reelSymbols[0] == (int)Symbol.Cherry && reelSymbols[1] == (int)Symbol.Cherry) ||
                (reelSymbols[1] == (int)Symbol.Cherry && reelSymbols[2] == (int)Symbol.Cherry) ||
                (reelSymbols[0] == (int)Symbol.Cherry && reelSymbols[2] == (int)Symbol.Cherry)
            )
            {
                winAmount = currentBet * 4;
            }
        }
        
        // 音效播放辅助方法（这里仅做示意，实际游戏中应使用AudioSource播放音效）
        private void PlayButtonSound()
        {
            // 实际实现中应播放按钮点击音效
            Logger.LogDebug("播放按钮点击音效");
        }
        
        private void PlaySpinSound()
        {
            // 实际实现中应播放旋转开始音效
            Logger.LogDebug("播放旋转开始音效");
        }
        
        private void PlayReelStopSound()
        {
            // 实际实现中应播放转轴停止音效
            Logger.LogDebug("播放转轴停止音效");
        }
        
        private void PlayWinSound()
        {
            // 实际实现中应播放中奖音效
            Logger.LogDebug("播放中奖音效");
        }
        
        private void PlayLoseSound()
        {
            // 实际实现中应播放未中奖音效
            Logger.LogDebug("播放未中奖音效");
        }
        
        private void PlayErrorSound()
        {
            // 实际实现中应播放错误提示音效
            Logger.LogDebug("播放错误提示音效");
        }

        // 返回主窗口并保存结余货币
        private void ReturnToMainMenu()
        {
            // 保存结余货币
            RemainingMoney = playerChips;
            
            // 关闭老虎机游戏窗口
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
        public void UpdateResolutionSettings()
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
