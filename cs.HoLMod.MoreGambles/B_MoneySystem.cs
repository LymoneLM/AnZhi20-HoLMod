using System;
using System;
using System.Collections.Generic;
using BepInEx.Logging;
using UnityEngine;

namespace cs.HoLMod.MoreGambles
{
    /// <summary>
    /// 货币类型枚举
    /// </summary>
    public enum CurrencyType
    {
        Copper = 0,     // 铜钱
        Silver = 1,     // 元宝
        PaperMoney = 2, // 宝钞
        GoldNotes = 3   // 金钞
    }

    /// <summary>
        /// 货币系统类 - 负责处理游戏中的货币管理
        /// </summary>
        public class B_MoneySystem
        {
            // 兑换比例常量
            private const int COPPER_TO_PAPER_EXCHANGE_RATE = 10000; // 1万铜钱兑换1宝钞
            private const int SILVER_TO_GOLD_NOTES_EXCHANGE_RATE = 100; // 100元宝兑换1金钞
            
            // 货币系统变量
            private int copperCoins = 0; // 铜钱
            private int silverIngots = 0; // 元宝
            public string A_coins_T = "0";    //铜钱兑换宝钞中间值
            public string A_coins_Y = "0";   //元宝兑换金钞中间值
            public string B_coins_T = "0";   //宝钞兑换铜钱中间值
            public string B_coins_Y = "0";   //金钞兑换元宝中间值
            private int paperMoney = 0; // 宝钞
            private int goldNotes = 0; // 金钞
            
            // 兑换输入框 - 保留但不使用，用于向后兼容
            public string paperMoneyInput = "0";
            public string goldNotesInput = "0";
            
            // 日志记录器
            private ManualLogSource logger;
            
            // 引用到主插件实例
            private A_MoreGambles mainPlugin;
            
            // 构造函数
            public B_MoneySystem(ManualLogSource logger)
            {
                this.logger = logger;
                LoadGameCurrency();
            }
            
            // 设置主插件引用的方法
            public void SetMainPlugin(A_MoreGambles plugin)
            {
                this.mainPlugin = plugin;
            }
        
        /// <summary>
        /// 加载游戏中的真实货币数量
        /// </summary>
        public void LoadGameCurrency()
        {
            try
            {
                // 通过Mainload.CGNum[0]直接读取游戏中的铜钱数量
                if (IsMainloadValid())
                {
                    if (Mainload.CGNum.Count > 0 && int.TryParse(Mainload.CGNum[0], out int gameCopperCoins))
                    {
                        copperCoins = Math.Max(0, gameCopperCoins);
                        logger.LogInfo("加载游戏铜钱数量: " + copperCoins);
                    }
                    else
                    {
                        logger.LogWarning("获取游戏铜钱数量失败");
                    }
                    
                    // 通过Mainload.CGNum[1]直接读取游戏中的元宝数量
                    if (Mainload.CGNum.Count > 1 && int.TryParse(Mainload.CGNum[1], out int gameSilverIngots))
                    {
                        silverIngots = Math.Max(0, gameSilverIngots);
                        logger.LogInfo("加载游戏元宝数量: " + silverIngots);
                    }
                    else
                    {
                        logger.LogWarning("获取游戏元宝数量失败");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("加载游戏货币时出错: " + ex.Message);
            }
        }
        
        /// <summary>
        /// 更新游戏中的货币数量
        /// </summary>
        /// <param name="currencyType">货币类型</param>
        /// <param name="amount">数量</param>
        private void UpdateGameCurrency(CurrencyType currencyType, int amount)
        {
            try
            {
                if (!IsMainloadValid())
                    return;

                // 确保金额非负
                amount = Math.Max(0, amount);

                switch (currencyType)
                {
                    case CurrencyType.Copper:
                        if (Mainload.CGNum.Count > 0)
                        {
                            Mainload.CGNum[0] = amount.ToString();
                            copperCoins = amount;
                            logger.LogInfo("更新游戏铜钱数量: " + amount);
                        }
                        break;
                    case CurrencyType.Silver:
                        if (Mainload.CGNum.Count > 1)
                        {
                            Mainload.CGNum[1] = amount.ToString();
                            silverIngots = amount;
                            logger.LogInfo("更新游戏元宝数量: " + amount);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"更新游戏{currencyType}时出错: " + ex.Message);
            }
        }
        
        /// <summary>
        /// 兑换宝钞 - 1万铜钱兑换1宝钞
        /// </summary>
        /// <returns>兑换是否成功</returns>
        public bool ExchangePaperMoney()
        {
            try
            {
                // 使用TryParse替代直接Parse，增强错误处理
                if (!int.TryParse(A_coins_T, out int amount) || amount <= 0)
                {
                    logger.LogWarning("请输入有效的兑换数量");
                    return false;
                }

                int requiredCopperCoins = amount * COPPER_TO_PAPER_EXCHANGE_RATE;
                if (copperCoins >= requiredCopperCoins && IsMainloadValid())
                {
                    // 更新本地货币数量
                    paperMoney += amount;
                    copperCoins -= requiredCopperCoins;
                    
                    // 同步回游戏的铜钱数量
                    UpdateGameCurrency(CurrencyType.Copper, copperCoins);
                    
                    logger.LogInfo("兑换成功！获得 " + amount + " 宝钞 (消耗 " + requiredCopperCoins + " 铜钱)");
                    
                    // 添加成功提示
                    Mainload.Tip_Show.Add(new List<string>
                    {
                        "1",
                        string.Format("成功兑换{0}宝钞，消耗{1}铜币", amount, requiredCopperCoins)
                    });
                    
                    // 成功兑换后重置输入框
                    A_coins_T = "";
                    return true;
                }
                else
                {
                    int shortage = requiredCopperCoins - copperCoins;
                    logger.LogWarning("铜钱不足，需要 " + requiredCopperCoins + " 铜钱 (当前持有: " + copperCoins + ")");
                    
                    // 添加失败提示
                    Mainload.Tip_Show.Add(new List<string>
                    {
                        "1",
                        string.Format("宝钞兑换失败，还需{0}铜币", shortage)
                    });
                    
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("兑换宝钞失败：" + ex.Message);
                return false;
            }
            finally
            {
                // 重新读取铜钱和元宝以确保同步
                LoadGameCurrency();
            }
        }
        
        /// <summary>
        /// 兑换金钞 - 100元宝兑换1金钞
        /// </summary>
        /// <returns>兑换是否成功</returns>
        public bool ExchangeGoldNotes()
        {
            try
            {
                // 使用TryParse替代直接Parse，增强错误处理
                if (!int.TryParse(A_coins_Y, out int amount) || amount <= 0)
                {
                    logger.LogWarning("请输入有效的兑换数量");
                    return false;
                }

                int requiredSilverIngots = amount * SILVER_TO_GOLD_NOTES_EXCHANGE_RATE;
                if (silverIngots >= requiredSilverIngots && IsMainloadValid())
                {
                    // 更新本地货币数量
                    goldNotes += amount;
                    silverIngots -= requiredSilverIngots;
                    
                    // 同步回游戏的元宝数量
                    UpdateGameCurrency(CurrencyType.Silver, silverIngots);
                    
                    logger.LogInfo("兑换成功！获得 " + amount + " 金钞 (消耗 " + requiredSilverIngots + " 元宝)");
                    
                    // 添加成功提示
                    Mainload.Tip_Show.Add(new List<string>
                    {
                        "1",
                        string.Format("成功兑换{0}金钞，消耗{1}元宝", amount, requiredSilverIngots)
                    });
                    
                    // 成功兑换后重置输入框
                    A_coins_Y = "";
                    return true;
                }
                else
                {
                    int shortage = requiredSilverIngots - silverIngots;
                    logger.LogWarning("元宝不足，需要 " + requiredSilverIngots + " 元宝 (当前持有: " + silverIngots + ")");
                    
                    // 添加失败提示
                    Mainload.Tip_Show.Add(new List<string>
                    {
                        "1",
                        string.Format("金钞兑换失败，还需{0}元宝", shortage)
                    });
                    
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("兑换金钞失败：" + ex.Message);
                return false;
            }
            finally
            {
                // 重新读取铜钱和元宝以确保同步
                LoadGameCurrency();
            }
        }
        
        /// <summary>
        /// 自动将宝钞和金钞兑换回游戏货币
        /// </summary>
        public void AutoExchangeToGameCurrency()
        {
            try
            {
                if (!IsMainloadValid())
                    return;

                if (paperMoney > 0)
                {
                    // 宝钞兑换回铜钱 - 1宝钞兑换1万铜钱
                    B_coins_T = paperMoney.ToString();
                    int exchangedCopperCoins = paperMoney * COPPER_TO_PAPER_EXCHANGE_RATE;
                    int totalCopperCoins = copperCoins + exchangedCopperCoins;
                    
                    // 更新游戏中的铜钱数量
                    if (Mainload.CGNum.Count > 0)
                    {
                        Mainload.CGNum[0] = totalCopperCoins.ToString();
                        logger.LogInfo("宝钞兑换回铜钱: " + paperMoney + " 宝钞 = " + exchangedCopperCoins + " 铜钱");
                    }
                    
                    paperMoney = 0;
                    B_coins_T = "0";
                }
                
                if (goldNotes > 0)
                {
                    // 金钞兑换回元宝 - 1金钞兑换100元宝
                    B_coins_Y = goldNotes.ToString();
                    int exchangedSilverIngots = goldNotes * SILVER_TO_GOLD_NOTES_EXCHANGE_RATE;
                    int totalSilverIngots = silverIngots + exchangedSilverIngots;
                    
                    // 更新游戏中的元宝数量
                    if (Mainload.CGNum.Count > 1)
                    {
                        Mainload.CGNum[1] = totalSilverIngots.ToString();
                        logger.LogInfo("金钞兑换回元宝: " + goldNotes + " 金钞 = " + exchangedSilverIngots + " 元宝");
                    }
                    
                    goldNotes = 0;
                    B_coins_Y = "0";
                }
                
                // 再次检查并确保宝钞和金钞值为0
                if (paperMoney != 0)
                {
                    paperMoney = 0;
                    B_coins_T = "0";
                    logger.LogInfo("确保宝钞数量为0");
                }
                
                if (goldNotes != 0)
                {
                    goldNotes = 0;
                    B_coins_Y = "0";
                    logger.LogInfo("确保金钞数量为0");
                }
                
                // 同步回本地变量
                LoadGameCurrency();
                
                // 添加兑换完成提示
                if (Mainload.Tip_Show != null)
                {
                    Mainload.Tip_Show.Add(new List<string>
                    {
                        "1",
                        "已将筹码自动按比例兑换成铜钱和元宝，欢迎下次再来！"
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError("自动兑换货币时出错: " + ex.Message);
            }
        }
        
        /// <summary>
        /// 从游戏返回并接收结余货币
        /// </summary>
        /// <param name="isPaperMoney">是否为宝钞</param>
        /// <param name="remainingMoney">剩余货币数量</param>
        public void ReturnFromGame(bool isPaperMoney, int remainingMoney)
        {
            try
            {
                // 验证输入参数
                if (remainingMoney < 0)
                {
                    logger.LogWarning("剩余货币数量不能为负数");
                    return;
                }

                // 更新本地货币数量
                if (isPaperMoney)
                {
                    paperMoney += remainingMoney;
                }
                else
                {
                    goldNotes += remainingMoney;
                }
                
                string currencyType = isPaperMoney ? "宝钞" : "金钞";
                logger.LogInfo("从游戏返回，获得结余" + currencyType + "：" + remainingMoney);
            }
            catch (Exception ex)
            {
                logger.LogError("处理游戏返回时出错: " + ex.Message);
            }
        }
        
        /// <summary>
        /// 扣除赌注
        /// </summary>
        /// <param name="amount">赌注金额</param>
        /// <param name="usePaperMoney">是否使用宝钞（否则使用金钞）</param>
        /// <returns>扣除是否成功</returns>
        public bool DeductBet(int amount, bool usePaperMoney)
        {
            try
            {
                // 参数验证
                if (amount <= 0)
                {
                    logger.LogWarning("赌注金额必须大于0");
                    return false;
                }
                
                // 检查货币是否足够并扣除
                if (usePaperMoney)
                {
                    if (paperMoney < amount)
                    {
                        logger.LogWarning("宝钞不足，当前持有：" + paperMoney + "，需要：" + amount);
                        // 显示宝钞不足提示
                        if (Mainload.Tip_Show != null)
                        {
                            Mainload.Tip_Show.Add(new List<string>
                            {
                                "1",
                                "宝钞数量不足，请返回兑换或重新输入"
                            });
                        }
                        // 清空带入数量输入框
                        if (mainPlugin != null)
                        {
                            mainPlugin.betAmountInput = "";
                        }
                        return false;
                    }
                    
                    // 扣除宝钞
                    paperMoney -= amount;
                    logger.LogInfo("已扣除宝钞：" + amount + "，剩余：" + paperMoney);
                }
                else
                {
                    if (goldNotes < amount)
                    {
                        logger.LogWarning("金钞不足，当前持有：" + goldNotes + "，需要：" + amount);
                        // 显示金钞不足提示
                        if (Mainload.Tip_Show != null)
                        {
                            Mainload.Tip_Show.Add(new List<string>
                            {
                                "1",
                                "金钞数量不足，请返回兑换或重新输入"
                            });
                        }
                        // 清空带入数量输入框
                        if (mainPlugin != null)
                        {
                            mainPlugin.betAmountInput = "";
                        }
                        return false;
                    }
                    
                    // 扣除金钞
                    goldNotes -= amount;
                    logger.LogInfo("已扣除金钞：" + amount + "，剩余：" + goldNotes);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError("扣除赌注时出错: " + ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// 获取指定货币类型的当前数量
        /// </summary>
        /// <param name="currencyType">货币类型</param>
        /// <returns>货币数量</returns>
        public int GetCurrencyAmount(CurrencyType currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.Copper:
                    return copperCoins;
                case CurrencyType.Silver:
                    return silverIngots;
                case CurrencyType.PaperMoney:
                    return paperMoney;
                case CurrencyType.GoldNotes:
                    return goldNotes;
                default:
                    logger.LogWarning("未知的货币类型: " + currencyType);
                    return 0;
            }
        }
        
        // 获取货币信息的属性
        public int CopperCoins { get { return copperCoins; } }
        public int SilverIngots { get { return silverIngots; } }
        public int PaperMoney { get { return paperMoney; } }
        public int GoldNotes { get { return goldNotes; } }
        
        /// <summary>
        /// 绘制货币窗口
        /// </summary>
        public void DrawCurrencyWindow(GUIStyle labelStyle, Rect rect, float labelX, float valueX, float goldLabelX, float goldValueX, float lineHeight)
        {
            // 第一行：铜钱和宝物
            GUI.Label(new Rect(labelX, rect.y + 30, 100, lineHeight), "铜钱持有：", labelStyle);
            GUI.Label(new Rect(valueX, rect.y + 30, 100, lineHeight), copperCoins.ToString(), labelStyle);
            
            GUI.Label(new Rect(goldLabelX, rect.y + 30, 100, lineHeight), "元宝持有：", labelStyle);
            GUI.Label(new Rect(goldValueX, rect.y + 30, 100, lineHeight), silverIngots.ToString(), labelStyle);
            
            // 第二行：宝钞和金钞
            GUI.Label(new Rect(labelX, rect.y + 60, 100, lineHeight), "宝钞持有：", labelStyle);
            GUI.Label(new Rect(valueX, rect.y + 60, 100, lineHeight), paperMoney.ToString(), labelStyle);
            
            GUI.Label(new Rect(goldLabelX, rect.y + 60, 100, lineHeight), "金钞持有：", labelStyle);
            GUI.Label(new Rect(goldValueX, rect.y + 60, 100, lineHeight), goldNotes.ToString(), labelStyle);
        }
        
        /// <summary>
        /// 检查Mainload对象是否有效
        /// </summary>
        /// <returns>Mainload是否有效</returns>
        private bool IsMainloadValid()
        {
            bool isValid = Mainload.CGNum != null;
            if (!isValid)
            {
                logger.LogWarning("Mainload.CGNum为null");
            }
            return isValid;
        }
    }
}
