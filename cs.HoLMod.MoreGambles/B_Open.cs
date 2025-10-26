using System;
using System;
using UnityEngine;

namespace cs.HoLMod.MoreGambles
{
    /// <summary>
    /// 游戏打开管理类 - 负责处理各种游戏的启动逻辑
    /// </summary>
    internal class B_Open
    {
        private A_MoreGambles parentScript;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="parent">A_MoreGambles引用</param>
        public B_Open(A_MoreGambles parent)
        {
            parentScript = parent;
        }

        /// <summary>
        /// 进入选择的游戏
        /// </summary>
        /// <param name="selectedGameType">选择的游戏类型</param>
        /// <param name="betAmount">赌注金额</param>
        /// <param name="usePaperMoney">是否使用宝钞</param>
        public void EnterSelectedGame(int selectedGameType, int betAmount, bool usePaperMoney)
        {
            // 保存赌注信息到父脚本，以便OpenGame方法可以访问
            parentScript.betAmountInput = betAmount.ToString();
            parentScript.usePaperMoney = usePaperMoney;
            
            // 进入选择的游戏
            switch (selectedGameType)
            {
                case (int)GameType.TexasHoldem:
                    OpenTexasHoldem();
                    break;
                case (int)GameType.FriedGoldenFlower:
                    OpenFriedGoldenFlower();
                    break;
                case (int)GameType.FightTheLandlord:
                    OpenFightTheLandlord();
                    break;
                case (int)GameType.Dice:
                    OpenDiceGame();
                    break;
                case (int)GameType.SlotMachine:
                    OpenSlotMachine(betAmount, usePaperMoney);
                    break;
                case (int)GameType.Roulette:
                    OpenRouletteGame();
                    break;
            }
        }

        // 通用游戏打开方法 - 使用泛型减少代码重复
        private T OpenGame<T>(string gameName) where T : Component
        {
            try
            {
                // 解析赌注数量
                int betAmount;
                if (!int.TryParse(parentScript.betAmountInput, out betAmount))
                {
                    parentScript.PluginLogger.LogError("无法解析赌注数量: " + parentScript.betAmountInput);
                    return null;
                }
                
                bool usePaperMoney = parentScript.usePaperMoney;
                GameObject gameObject = parentScript.gameObject;

                // 查找游戏实例
                T gameInstance = UnityEngine.Object.FindObjectOfType<T>();
                if (gameInstance == null)
                {
                    // 如果实例不存在，创建一个新的实例
                    parentScript.PluginLogger.LogInfo("创建新的" + gameName + "游戏实例");
                    gameInstance = gameObject.AddComponent<T>();
                    // 手动调用Awake方法初始化游戏
                    typeof(T).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(gameInstance, null);
                }

                // 设置游戏窗口为显示状态
                typeof(T).GetField("showMenu", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(gameInstance, true);
                typeof(T).GetField("blockGameInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(gameInstance, true);
                
                // 设置货币类型和初始筹码
                if (typeof(T).GetProperty("UsePaperMoney") != null)
                    typeof(T).GetProperty("UsePaperMoney").SetValue(gameInstance, usePaperMoney);
                if (typeof(T).GetProperty("InitialChips") != null)
                    typeof(T).GetProperty("InitialChips").SetValue(gameInstance, betAmount);
                
                // 设置玩家筹码（覆盖Awake中设置的默认值）
                typeof(T).GetField("playerChips", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(gameInstance, betAmount);
                
                // 确保分辨率设置正确
                typeof(T).GetMethod("UpdateResolutionSettings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(gameInstance, null);
                
                string currencyType = usePaperMoney ? "宝钞" : "金钞";
                parentScript.PluginLogger.LogInfo("已成功打开" + gameName + "游戏，你将以玩家身份参与，初始" + currencyType + "：" + betAmount);
                
                return gameInstance;
            }
            catch (Exception ex)
            {
                parentScript.PluginLogger.LogError("打开" + gameName + "游戏时出错: " + ex.Message);
                return null;
            }
        }

        // 打开德州扑克游戏
        public void OpenTexasHoldem()
        {
            OpenGame<TexasHold_em>("德州扑克");
        }

        // 打开斗地主游戏
        public void OpenFightTheLandlord()
        {
            OpenGame<FightTheLandlord>("斗地主");
        }

        // 打开炸金花游戏
        public void OpenFriedGoldenFlower()
        {
            OpenGame<FriedGoldenFlower>("炸金花");
        }

        // 打开骰子游戏
        public void OpenDiceGame()
        {
            OpenGame<Dice>("骰子");
        }

        // 打开老虎机游戏
        public void OpenSlotMachine(int betAmount, bool usePaperMoney)
        {
            try
            {
                GameObject gameObject = parentScript.gameObject;

                // 查找老虎机游戏实例
                SlotMachine slotMachine = UnityEngine.Object.FindObjectOfType<SlotMachine>();
                if (slotMachine == null)
                {
                    // 如果实例不存在，创建一个新的实例
                    parentScript.PluginLogger.LogInfo("创建新的老虎机游戏实例");
                    slotMachine = gameObject.AddComponent<SlotMachine>();
                    // 手动调用Awake方法初始化游戏
                    typeof(SlotMachine).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(slotMachine, null);
                }

                // 设置老虎机游戏窗口为显示状态
                typeof(SlotMachine).GetField("showMenu", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(slotMachine, true);
                typeof(SlotMachine).GetField("blockGameInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(slotMachine, true);
                
                // 设置货币类型和初始筹码
                if (typeof(SlotMachine).GetProperty("UsePaperMoney") != null)
                    typeof(SlotMachine).GetProperty("UsePaperMoney").SetValue(slotMachine, usePaperMoney);
                if (typeof(SlotMachine).GetProperty("InitialChips") != null)
                    typeof(SlotMachine).GetProperty("InitialChips").SetValue(slotMachine, betAmount);
                
                // 设置玩家筹码（覆盖Awake中设置的默认值）
                typeof(SlotMachine).GetField("playerChips", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(slotMachine, betAmount);
                
                // 确保分辨率设置正确
                typeof(SlotMachine).GetMethod("UpdateResolutionSettings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(slotMachine, null);
                
                string currencyType = usePaperMoney ? "宝钞" : "金钞";
                parentScript.PluginLogger.LogInfo("已成功打开老虎机游戏，你将以玩家身份参与，初始" + currencyType + "：" + betAmount);
            }
            catch (Exception ex)
            {
                parentScript.PluginLogger.LogError("打开老虎机游戏时出错: " + ex.Message);
            }
        }

        // 打开轮盘赌游戏
        public void OpenRouletteGame()
        {
            OpenGame<Roulette>("轮盘赌");
        }
    }
}
