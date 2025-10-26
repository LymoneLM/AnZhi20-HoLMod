using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace cs.HoLMod.MoreGambles
{
    // 游戏类型枚举
    public enum GameType
    {
        TexasHoldem = 0,    // 德州扑克
        Roulette = 1,       // 轮盘赌
        FightTheLandlord = 2, // 斗地主
        Dice = 3,           // 骰子游戏
        SlotMachine = 4,    // 老虎机
        FriedGoldenFlower = 5 // 炸金花
    }
    
    [BepInPlugin("cs.HoLMod.MoreGambles.AnZhi20", "HoLMod.MoreGambles", "1.0.0")]
    public class A_MoreGambles : BaseUnityPlugin
    {
        // 选择窗口设置
        private Rect windowRect = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 300);
        public bool showSelectMenu = false;
        private bool blockGameInput = false;
        private float scaleFactor = 1f;
        private Font chineseFont;
        
        // 货币系统
        private B_MoneySystem moneySystem;
        
        // 游戏打开管理器
        private B_Open gameOpener;
        
        // 赌注选择窗口设置
        private Rect betWindowRect = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 250);
        public bool showBetWindow = false;
        private int selectedGameType = 0;
        public bool usePaperMoney = true; // true=宝钞, false=金钞
        public string betAmountInput = "0";
        
        // 提供Logger访问
        public BepInEx.Logging.ManualLogSource PluginLogger { get { return Logger; } }

        private void Awake()
        {
            InitializeWindowPosition();
            // 尝试加载中文字体
            TryLoadChineseFont();
            // 初始化货币系统
            moneySystem = new B_MoneySystem(Logger);
            // 设置主插件引用，使货币系统能够访问主插件实例
            moneySystem.SetMainPlugin(this);
            // 初始化游戏打开管理器
            gameOpener = new B_Open(this);
        }

        private void Update()
        {
            // 按F4键切换选择窗口显示
            if (Input.GetKeyDown(KeyCode.F4))
            {
                if (showSelectMenu)
            {
                // 关闭窗口前，自动兑换回游戏货币
                moneySystem.AutoExchangeToGameCurrency();
            }
            else
            {
                // 打开窗口前，读取游戏中的货币数量
                moneySystem.LoadGameCurrency();
                // 重置输入，防止上次输入影响
                betAmountInput = "0";
                usePaperMoney = true;
            }
                
                showSelectMenu = !showSelectMenu;
                blockGameInput = showSelectMenu;
                Logger.LogInfo(showSelectMenu ? "赌博选择窗口已打开" : "赌博选择窗口已关闭");
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

                if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.F4))
                {
                    Input.ResetInputAxes();
                }
            }
            
            // 输入验证 - 确保betAmountInput只包含数字
            if (!string.IsNullOrEmpty(betAmountInput) && !IsDigitsOnly(betAmountInput))
            {
                betAmountInput = new string(betAmountInput.Where(char.IsDigit).ToArray());
                if (string.IsNullOrEmpty(betAmountInput))
                    betAmountInput = "0";
            }
        }
        
        // 检查字符串是否只包含数字
        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void OnGUI()
        {
            if (!showSelectMenu && !showBetWindow)
                return;

            // 设置GUI缩放和字体
            GUI.skin = GUI.skin.customStyles.Length > 0 ? GUI.skin : CreateCustomSkin();
            if (chineseFont != null)
            {
                GUI.skin.font = chineseFont;
            }

            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleFactor, scaleFactor, 1f));
            
            if (showSelectMenu)
            {
                // 只绘制合并后的游戏选择窗口
                windowRect = GUI.Window(0, windowRect, DrawWindow, "");
            }
            else if (showBetWindow)
            {
                // 显示赌注选择窗口
                betWindowRect = GUI.Window(2, betWindowRect, DrawBetWindow, "选择赌注");
            }
        }

        private void DrawWindow(int windowID)
        {
            // 窗口标题栏无文本，所以不需要绘制标题
            GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));

            // 第一行：居中显示"赌场"
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontSize = 24;
            titleStyle.fontStyle = FontStyle.Bold;
            GUI.Label(new Rect(0, 30, windowRect.width / scaleFactor, 40), "赌场", titleStyle);

            // 保存原始背景色并设置为半透明
            Color originalBackgroundColor = GUI.backgroundColor;
            Color semiTransparent = originalBackgroundColor;
            semiTransparent.a = 0.8f;
            GUI.backgroundColor = semiTransparent;
            
            // 显示货币持有量
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 14;
            
            float labelX = 20;
            float valueX = 120;
            float goldLabelX = 220;
            float goldValueX = 320;
            float lineHeight = 30;
            float startY = 80;
            
            // 绘制货币信息
            moneySystem.DrawCurrencyWindow(labelStyle, new Rect(labelX, startY - 30, windowRect.width / scaleFactor, 100), 
                labelX, valueX, goldLabelX, goldValueX, lineHeight);
            
            // 第三行：宝钞兑换
            startY += lineHeight * 2;
            GUI.Label(new Rect(labelX, startY, 100, lineHeight), "宝钞兑换：", labelStyle);
            moneySystem.A_coins_T = GUI.TextField(new Rect(valueX, startY, 100, lineHeight - 10), moneySystem.A_coins_T);
            
            // 第四行：金钞兑换
            GUI.Label(new Rect(goldLabelX, startY, 100, lineHeight), "金钞兑换：", labelStyle);
            moneySystem.A_coins_Y = GUI.TextField(new Rect(goldValueX, startY, 100, lineHeight - 10), moneySystem.A_coins_Y);
            
            // 兑换按钮
            float buttonWidth = 80;
            float buttonHeight = 25;
            startY += lineHeight + 5;
            
            if (GUI.Button(new Rect(valueX, startY, buttonWidth, buttonHeight), "兑换宝钞"))
            {
                moneySystem.ExchangePaperMoney();
            }
            // 显示宝钞兑换比例
            GUIStyle rateStyle = new GUIStyle(labelStyle);
            rateStyle.alignment = TextAnchor.MiddleCenter;
            rateStyle.fontSize = 12;
            GUI.Label(new Rect(valueX, startY + buttonHeight + 2, buttonWidth, 20), "1万铜钱=1宝钞", rateStyle);
            
            if (GUI.Button(new Rect(goldValueX, startY, buttonWidth, buttonHeight), "兑换金钞"))
            {
                moneySystem.ExchangeGoldNotes();
            }
            // 显示金钞兑换比例
            GUI.Label(new Rect(goldValueX, startY + buttonHeight + 2, buttonWidth, 20), "100元宝=1金钞", rateStyle);
            
            // 恢复原始背景色
            GUI.backgroundColor = originalBackgroundColor;
            
            // 下方显示"请选择赌博方式："
            startY += buttonHeight + 25;
            GUIStyle instructionStyle = new GUIStyle(GUI.skin.label);
            instructionStyle.alignment = TextAnchor.MiddleLeft;
            instructionStyle.fontSize = 16;
            GUI.Label(new Rect(20, startY, windowRect.width / scaleFactor - 40, 30), "请选择赌博方式：", instructionStyle);

            // 第一行：3个按钮
            float gameButtonWidth = (windowRect.width / scaleFactor - 50) / 3;
            float gameButtonHeight = 40;
            float buttonY = startY + 40;
            float buttonX = 20;

            if (GUI.Button(new Rect(buttonX, buttonY, gameButtonWidth, gameButtonHeight), "斗地主"))
            {
                // 显示赌注选择窗口，设置游戏类型为斗地主
                ShowBetWindow((int)GameType.FightTheLandlord);
            }

            buttonX += gameButtonWidth + 10;
            if (GUI.Button(new Rect(buttonX, buttonY, gameButtonWidth, gameButtonHeight), "炸金花"))
            {
                // 显示赌注选择窗口，设置游戏类型为炸金花
                ShowBetWindow((int)GameType.FriedGoldenFlower);
            }

            buttonX += gameButtonWidth + 10;
            if (GUI.Button(new Rect(buttonX, buttonY, gameButtonWidth, gameButtonHeight), "德州扑克"))
            {
                // 显示赌注选择窗口，设置游戏类型为德州扑克
                ShowBetWindow((int)GameType.TexasHoldem);
            }
            
            // 第二行：3个按钮（骰子、老虎机、轮盘赌） - 调整为与第一行相同的布局方式确保对齐
            buttonY += gameButtonHeight + 15;
            buttonX = 20;

            if (GUI.Button(new Rect(buttonX, buttonY, gameButtonWidth, gameButtonHeight), "骰子"))
            {
                ShowBetWindow((int)GameType.Dice);
            }

            buttonX += gameButtonWidth + 10;
            if (GUI.Button(new Rect(buttonX, buttonY, gameButtonWidth, gameButtonHeight), "老虎机"))
            {
                ShowBetWindow((int)GameType.SlotMachine);
            }

            buttonX += gameButtonWidth + 10;
            if (GUI.Button(new Rect(buttonX, buttonY, gameButtonWidth, gameButtonHeight), "轮盘赌"))
            {
                ShowBetWindow((int)GameType.Roulette);
            }

            // 关闭按钮 - 置于窗口右上角，使用"x"符号
            if (GUI.Button(new Rect(windowRect.width / scaleFactor - 40, 5, 35, 25), "x"))
            {
                // 关闭窗口前，自动兑换回游戏货币
                moneySystem.AutoExchangeToGameCurrency();
                showSelectMenu = false;
                blockGameInput = false;
            }
        }

        // 显示赌注选择窗口
        private void ShowBetWindow(int gameType)
        {
            selectedGameType = gameType;
            showSelectMenu = false; // 隐藏主菜单
            showBetWindow = true; // 显示赌注选择窗口
            betAmountInput = "0"; // 重置输入金额
            blockGameInput = true; // 显示窗口时阻止游戏输入
            UpdateResolutionSettings(); // 更新窗口位置
        }
        
        // 绘制赌注选择窗口
        private void DrawBetWindow(int windowID)
        {
            // 允许拖动窗口
            GUI.DragWindow(new Rect(0, 0, betWindowRect.width, 20));
            
            // 保存原始背景色并设置为半透明
            Color originalBackgroundColor = GUI.backgroundColor;
            Color semiTransparent = originalBackgroundColor;
            semiTransparent.a = 0.8f;
            GUI.backgroundColor = semiTransparent;
            
            // 显示游戏名称
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontSize = 20;
            titleStyle.fontStyle = FontStyle.Bold;
            
            // 显示游戏名称
            string gameName = GetGameName(selectedGameType);
            GUI.Label(new Rect(0, 30, betWindowRect.width / scaleFactor, 40), "选择" + gameName + "赌注", titleStyle);
            
            // 选择货币类型
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 14;
            
            float labelX = 40;
            float optionX = 150;
            float lineHeight = 30;
            float currentY = 80;
            
            GUI.Label(new Rect(labelX, currentY, 100, lineHeight), "选择货币：", labelStyle);
            usePaperMoney = GUI.Toggle(new Rect(optionX, currentY, 100, lineHeight), usePaperMoney, "宝钞");
            usePaperMoney = !GUI.Toggle(new Rect(optionX + 100, currentY, 100, lineHeight), !usePaperMoney, "金钞");
            
            // 显示当前货币持有量
            currentY += 35;
            string currencyType = usePaperMoney ? "宝钞" : "金钞";
            int currentAmount = usePaperMoney ? moneySystem.PaperMoney : moneySystem.GoldNotes;
            GUI.Label(new Rect(labelX, currentY, 200, lineHeight), "当前" + currencyType + "持有：" + currentAmount, labelStyle);
            
            // 输入赌注数量
            currentY += 35;
            GUI.Label(new Rect(labelX, currentY, 100, lineHeight), "赌注数量：", labelStyle);
            betAmountInput = GUI.TextField(new Rect(optionX, currentY, 120, lineHeight - 10), betAmountInput);
            
            // 限制只能输入数字
            betAmountInput = Regex.Replace(betAmountInput, "[^0-9]", "");
            
            // 确认和取消按钮
            currentY += 45;
            float buttonWidth = 80;
            float buttonHeight = 30;
            float buttonGap = 40;
            float buttonX = (betWindowRect.width / scaleFactor - buttonWidth * 2 - buttonGap) / 2;
            
            if (GUI.Button(new Rect(buttonX, currentY, buttonWidth, buttonHeight), "确认"))
            {
                ConfirmBet();
            }
            
            if (GUI.Button(new Rect(buttonX + buttonWidth + buttonGap, currentY, buttonWidth, buttonHeight), "取消"))
            {
                showBetWindow = false;
                showSelectMenu = true;
                blockGameInput = true; // 主菜单仍然显示，保持阻止游戏输入
                // 重新读取铜钱和元宝
                moneySystem.LoadGameCurrency();
            }
            
            // 恢复原始背景色
            GUI.backgroundColor = originalBackgroundColor;
        }
        
        // 根据游戏类型获取游戏名称
        private string GetGameName(int gameType)
        {
            switch (gameType)
            {
                case (int)GameType.TexasHoldem:
                    return "德州扑克";
                case (int)GameType.Roulette:
                    return "轮盘赌";
                case (int)GameType.FightTheLandlord:
                    return "斗地主";
                case (int)GameType.Dice:
                    return "骰子游戏";
                case (int)GameType.SlotMachine:
                    return "老虎机";
                case (int)GameType.FriedGoldenFlower:
                    return "炸金花";
                default:
                    return "未知游戏";
            }
        }
        
        // 检查是否可以确认下注
        private bool CanConfirmBet()
        {
            try
            {
                // 解析下注金额
                int betAmount = int.Parse(betAmountInput);

                // 检查下注金额是否有效
                if (betAmount <= 0)
                    return false;

                // 检查是否有足够的货币
                int currentMoney = usePaperMoney ? moneySystem.PaperMoney : moneySystem.GoldNotes;
                return betAmount <= currentMoney;
            }
            catch
            {
                return false;
            }
        }
        
        // 确认赌注并进入游戏
        private void ConfirmBet()
        {
            try
            {
                // 检查是否可以确认下注
                if (!CanConfirmBet())
                {
                    Logger.LogWarning("赌注金额无效或货币不足");
                    moneySystem.LoadGameCurrency(); // 重新读取铜钱和元宝
                    return;
                }
                
                int betAmount = int.Parse(betAmountInput);
                
                // 扣除赌注
                if (!moneySystem.DeductBet(betAmount, usePaperMoney))
                {
                    moneySystem.LoadGameCurrency(); // 重新读取铜钱和元宝
                    return;
                }
                
                Logger.LogInfo("已扣除" + (usePaperMoney ? "宝钞" : "金钞") + "：" + betAmount + "，进入" + 
                              GetGameName(selectedGameType) + "游戏");
                
                // 关闭赌注窗口
                showBetWindow = false;
                
                // 进入选择的游戏
                gameOpener.EnterSelectedGame(selectedGameType, betAmount, usePaperMoney);
            }
            catch (Exception ex)
            {
                Logger.LogError("确认赌注时出错: " + ex.Message);
                // 发生错误时也重新读取铜钱和元宝
                moneySystem.LoadGameCurrency();
            }
        }
        
        // 打开德州扑克游戏
        private void OpenTexasHoldem()
        {
            try
            {
                // 解析赌注数量
                int betAmount = int.Parse(betAmountInput);
                
                // 查找德州扑克游戏实例
                TexasHold_em texasHoldem = FindObjectOfType<TexasHold_em>();
                if (texasHoldem == null)
                {
                    // 如果实例不存在，创建一个新的实例
                    Logger.LogInfo("创建新的德州扑克游戏实例");
                    texasHoldem = gameObject.AddComponent<TexasHold_em>();
                    // 手动调用Awake方法初始化游戏
                    typeof(TexasHold_em).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(texasHoldem, null);
                }

                // 设置德州扑克窗口为显示状态
                typeof(TexasHold_em).GetField("showMenu", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(texasHoldem, true);
                typeof(TexasHold_em).GetField("blockGameInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(texasHoldem, true);
                
                // 设置货币类型和初始筹码
                texasHoldem.UsePaperMoney = usePaperMoney;
                texasHoldem.InitialChips = betAmount;
                
                // 设置玩家筹码（覆盖Awake中设置的默认值）
                typeof(TexasHold_em).GetField("playerChips", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(texasHoldem, betAmount);
                
                // 确保分辨率设置正确
                typeof(TexasHold_em).GetMethod("UpdateResolutionSettings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(texasHoldem, null);
                
                string currencyType = usePaperMoney ? "宝钞" : "金钞";
                Logger.LogInfo("已成功打开德州扑克游戏，你将以玩家身份参与，初始" + currencyType + "：" + betAmount);
            }
            catch (Exception ex)
            {
                Logger.LogError("打开德州扑克游戏时出错: " + ex.Message);
            }
        }
        
        // 从游戏返回主窗口并接收结余货币
        public void ReturnFromGame(bool isPaperMoney, int remainingMoney)
        {
            try
            {
                // 更新主窗口的货币数量
                moneySystem.ReturnFromGame(isPaperMoney, remainingMoney);
                
                // 显示主窗口
                showSelectMenu = true;
                blockGameInput = true;
                
                // 更新窗口位置
                UpdateResolutionSettings();
            }
            catch (Exception ex)
            {
                Logger.LogError("处理游戏返回时出错: " + ex.Message);
            }
        }
        
        // 打开斗地主游戏
        private void OpenFightTheLandlord()
        {
            try
            {
                // 解析赌注数量
                int betAmount = int.Parse(betAmountInput);
                
                // 查找斗地主游戏实例
                FightTheLandlord fightTheLandlord = FindObjectOfType<FightTheLandlord>();
                if (fightTheLandlord == null)
                {
                    // 如果实例不存在，创建一个新的实例
                    Logger.LogInfo("创建新的斗地主游戏实例");
                    fightTheLandlord = gameObject.AddComponent<FightTheLandlord>();
                    // 手动调用Awake方法初始化游戏
                    typeof(FightTheLandlord).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(fightTheLandlord, null);
                }

                // 设置斗地主窗口为显示状态
                typeof(FightTheLandlord).GetField("showMenu", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(fightTheLandlord, true);
                typeof(FightTheLandlord).GetField("blockGameInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(fightTheLandlord, true);
                
                // 设置货币类型和初始筹码
                fightTheLandlord.UsePaperMoney = usePaperMoney;
                fightTheLandlord.InitialChips = betAmount;
                
                // 设置玩家筹码（覆盖Awake中设置的默认值）
                typeof(FightTheLandlord).GetField("playerChips", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(fightTheLandlord, betAmount);
                
                // 确保分辨率设置正确
                typeof(FightTheLandlord).GetMethod("UpdateResolutionSettings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(fightTheLandlord, null);
                
                string currencyType = usePaperMoney ? "宝钞" : "金钞";
                Logger.LogInfo("已成功打开斗地主游戏，你将以玩家身份参与，初始" + currencyType + "：" + betAmount);
            }
            catch (Exception ex)
            {
                Logger.LogError("打开斗地主游戏时出错: " + ex.Message);
            }
        }




        private void UpdateResolutionSettings()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // 缩放因子基于屏幕高度
            scaleFactor = screenHeight / 1080f;
            if (scaleFactor < 0.5f) scaleFactor = 0.5f;
            if (scaleFactor > 2f) scaleFactor = 2f;
        }
        
        // 窗口初始化时设置位置
        private void InitializeWindowPosition()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            // 根据屏幕分辨率调整窗口位置和尺寸
            windowRect = new Rect(screenWidth / 2 - 275, screenHeight / 2 - 200, 550, 400);
            
            // 赌注窗口居中显示
            betWindowRect = new Rect(screenWidth / 2 - 200, screenHeight / 2 - 150, 400, 250);
        }

        private void TryLoadChineseFont()
        {
            try
            {
                // 尝试加载系统中的中文字体
                string[] fontNames = { "SimHei", "Microsoft YaHei", "Arial Unicode MS", "SimSun" };
                foreach (string fontName in fontNames)
                {
                    Font font = Resources.GetBuiltinResource<Font>(fontName + ".ttf");
                    if (font == null)
                    {
                        font = Resources.Load<Font>(fontName);
                    }
                    if (font != null)
                    {
                        chineseFont = font;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning("加载中文字体时出错: " + ex.Message);
            }
        }

        private GUISkin CreateCustomSkin()
        {
            GUISkin customSkin = ScriptableObject.CreateInstance<GUISkin>();
            customSkin.label = new GUIStyle();
            customSkin.button = new GUIStyle();
            customSkin.window = new GUIStyle(GUI.skin.window);
            return customSkin;
        }
        
        // 打开炸金花游戏
        private void OpenFriedGoldenFlower()
        {
            try
            {
                // 解析赌注数量
                int betAmount = int.Parse(betAmountInput);
                
                // 查找炸金花游戏实例
                FriedGoldenFlower friedGoldenFlower = FindObjectOfType<FriedGoldenFlower>();
                if (friedGoldenFlower == null)
                {
                    // 如果实例不存在，创建一个新的实例
                    Logger.LogInfo("创建新的炸金花游戏实例");
                    friedGoldenFlower = gameObject.AddComponent<FriedGoldenFlower>();
                    // 手动调用Awake方法初始化游戏
                    typeof(FriedGoldenFlower).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(friedGoldenFlower, null);
                }

                // 设置炸金花游戏窗口为显示状态
                typeof(FriedGoldenFlower).GetField("showMenu", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(friedGoldenFlower, true);
                typeof(FriedGoldenFlower).GetField("blockGameInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(friedGoldenFlower, true);
                
                // 设置货币类型和初始筹码
                friedGoldenFlower.UsePaperMoney = usePaperMoney;
                friedGoldenFlower.InitialChips = betAmount;
                
                // 设置玩家筹码（覆盖Awake中设置的默认值）
                typeof(FriedGoldenFlower).GetField("playerChips", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(friedGoldenFlower, betAmount);
                
                // 确保分辨率设置正确
                typeof(FriedGoldenFlower).GetMethod("UpdateResolutionSettings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(friedGoldenFlower, null);
                
                string currencyType = usePaperMoney ? "宝钞" : "金钞";
                Logger.LogInfo("已成功打开炸金花游戏，你将以玩家身份参与，初始" + currencyType + "：" + betAmount);
            }
            catch (Exception ex)
            {
                Logger.LogError("打开炸金花游戏时出错: " + ex.Message);
            }
        }
        
        // 从游戏返回，处理结余筹码
        public void ReturnFromGame(int chips, bool usePaperMoney)
        {
            try
            {
                if (chips > 0)
                {
                    // 调用B_MoneySystem的ReturnFromGame方法来正确更新货币
                    moneySystem.ReturnFromGame(usePaperMoney, chips);
                }
                
                // 显示主菜单
                showSelectMenu = true;
                blockGameInput = true;
                
                // 重新读取铜钱和元宝
                moneySystem.LoadGameCurrency();
            }
            catch (Exception ex)
            {
                Logger.LogError("从游戏返回时处理筹码出错: " + ex.Message);
            }
        }
        
        // 打开骰子游戏
        private void OpenDiceGame()
        {
            try
            {
                // 解析赌注数量
                int betAmount = int.Parse(betAmountInput);
                
                // 查找骰子游戏实例
                Dice diceGame = FindObjectOfType<Dice>();
                if (diceGame == null)
                {
                    // 如果实例不存在，创建一个新的实例
                    Logger.LogInfo("创建新的骰子游戏实例");
                    diceGame = gameObject.AddComponent<Dice>();
                    // 手动调用Awake方法初始化游戏
                    typeof(Dice).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(diceGame, null);
                }

                // 设置骰子游戏窗口为显示状态
                typeof(Dice).GetField("showMenu", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(diceGame, true);
                typeof(Dice).GetField("blockGameInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(diceGame, true);
                
                // 设置货币类型和初始筹码
                diceGame.UsePaperMoney = usePaperMoney;
                diceGame.InitialChips = betAmount;
                
                // 设置玩家筹码（覆盖Awake中设置的默认值）
                typeof(Dice).GetField("playerChips", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(diceGame, betAmount);
                
                // 确保分辨率设置正确
                typeof(Dice).GetMethod("UpdateResolutionSettings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(diceGame, null);
                
                string currencyType = usePaperMoney ? "宝钞" : "金钞";
                Logger.LogInfo("已成功打开骰子游戏，你将以玩家身份参与，初始" + currencyType + "：" + betAmount);
            }
            catch (Exception ex)
            {
                Logger.LogError("打开骰子游戏时出错: " + ex.Message);
            }
        }
        
        private void OpenSlotMachine(int amount, bool usePaperMoney)
        {
            try
            {
                Logger.LogInfo("打开老虎机游戏，初始筹码: " + amount + (usePaperMoney ? " 宝钞" : " 金钞"));

                // 查找或创建老虎机游戏实例
                SlotMachine slotMachine = FindObjectOfType<SlotMachine>();
                if (slotMachine == null)
                {
                    GameObject slotMachineObject = new GameObject("SlotMachine");
                    slotMachine = slotMachineObject.AddComponent<SlotMachine>();
                    Logger.LogInfo("创建了新的老虎机游戏实例");
                }
                else
                {
                    Logger.LogInfo("找到了已存在的老虎机游戏实例");
                }

                // 调用Awake初始化
                slotMachine.Awake();

                // 设置窗口状态
                slotMachine.gameObject.SetActive(true);

                // 设置货币类型和初始筹码
                slotMachine.UsePaperMoney = usePaperMoney;
                slotMachine.InitialChips = amount;

                // 显示老虎机窗口
                slotMachine.showMenu = true;
                slotMachine.blockGameInput = true;

                // 更新分辨率设置
                slotMachine.UpdateResolutionSettings();

                // 隐藏MoreGambles主窗口
                showSelectMenu = false;
                showBetWindow = false;

                Logger.LogInfo("老虎机游戏已打开");
            }
            catch (System.Exception ex)
            {
                Logger.LogError("打开老虎机游戏时出错: " + ex.Message);
                Logger.LogError(ex.StackTrace);
            }
        }

        // 打开轮盘赌游戏
        private void OpenRouletteGame()
        {
            try
            {
                int amount = int.Parse(betAmountInput);
                
                // 查找或创建轮盘赌实例
                Roulette roulette = FindObjectOfType<Roulette>();
                if (roulette == null)
                {
                    roulette = gameObject.AddComponent<Roulette>();
                    Logger.LogInfo("创建了新的轮盘赌游戏实例");
                }
                else
                {
                    Logger.LogInfo("找到了已存在的轮盘赌游戏实例");
                }

                // 调用公共初始化方法
                roulette.InitializeGame();

                // 设置窗口状态
                roulette.gameObject.SetActive(true);

                // 设置货币类型和初始筹码
                roulette.UsePaperMoney = usePaperMoney;
                roulette.InitialChips = amount;

                // 显示轮盘赌窗口
                roulette.showMenu = true;
                roulette.blockGameInput = true;

                // 更新分辨率设置
                roulette.UpdateResolutionSettings();

                // 隐藏MoreGambles主窗口
                showSelectMenu = false;
                showBetWindow = false;

                Logger.LogInfo("轮盘赌游戏已打开");
            }
            catch (System.Exception ex)
            {
                Logger.LogError("打开轮盘赌游戏时出错: " + ex.Message);
                Logger.LogError(ex.StackTrace);
            }
        }
    }
}
