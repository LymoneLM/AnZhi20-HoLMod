using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using System;

namespace cs.HoLMod.TaskCheat
{
    /// <summary>
    /// 任务修改器配置管理类
    /// </summary>
    public static class TaskCheatConfig
    {
        private const int DefaultCheckIntervalSeconds = 10; // 默认检查间隔为10秒
        private const int MinCheckIntervalSeconds = 1; // 最小检查间隔为1秒
        private const int MaxCheckIntervalSeconds = 10; // 最大检查间隔为10秒
        public const string DefaultStorageIdentifier = "default";
        private const string FamilyDataKeyDelimiter = "|";
        
        // BepInEx配置键前缀和后缀常量
        private const string ConfigKeyPrefix = "TaskCheat_";
        private const string SelectedTasksKeySuffix = "_SelectedTasks";
        private const string LastTimeRecordKeySuffix = "_LastTimeRecord";
        private const string CheckIntervalKeySuffix = "_CheckInterval";
        
        // 郡数组，索引对应郡ID
        private static string[] JunList = new string[]
        {
            LanguageManager.GetText("JunNan"),     // 0
            LanguageManager.GetText("JunSanChuan"),   // 1
            LanguageManager.GetText("JunShu"),     // 2
            LanguageManager.GetText("JunDanYang"),   // 3
            LanguageManager.GetText("JunChenLiu"),   // 4
            LanguageManager.GetText("JunChangSha"),   // 5
            LanguageManager.GetText("JunHuiJi"),   // 6
            LanguageManager.GetText("JunGuangLing"),   // 7
            LanguageManager.GetText("JunTaiYuan"),   // 8
            LanguageManager.GetText("JunYiZhou"),   // 9
            LanguageManager.GetText("JunNanHai"),   // 10
            LanguageManager.GetText("JunYunNan")    // 11
        };

        // 二维县数组，第一维是郡索引，第二维是县索引
        private static string[][] XianList = new string[][]
        {
            // 南郡 (索引0)
            new string[] { LanguageManager.GetText("XianLinJu"), LanguageManager.GetText("XianXiangFan"), LanguageManager.GetText("XianYiCheng"), LanguageManager.GetText("XianMaiCheng"), LanguageManager.GetText("XianHuaRong"), LanguageManager.GetText("XianYingTing"), LanguageManager.GetText("XianJiangLing"), LanguageManager.GetText("XianYiLing") },
            
            // 三川郡 (索引1)
            new string[] { LanguageManager.GetText("XianPingYang"), LanguageManager.GetText("XianXingYang"), LanguageManager.GetText("XianYuanWu"), LanguageManager.GetText("XianYangWu"), LanguageManager.GetText("XianXinZheng"), LanguageManager.GetText("XianYiYang") },
            
            // 蜀郡 (索引2)
            new string[] { LanguageManager.GetText("XianQiongLai"), LanguageManager.GetText("XianPiXian"), LanguageManager.GetText("XianShiFang"), LanguageManager.GetText("XianMianZhu"), LanguageManager.GetText("XianXinDu"), LanguageManager.GetText("XianChengDu") },
            
            // 丹阳郡 (索引3)
            new string[] { LanguageManager.GetText("XianMoLing"), LanguageManager.GetText("XianJiangCheng"), LanguageManager.GetText("XianJiangNing"), LanguageManager.GetText("XianLiYang"), LanguageManager.GetText("XianJianYe"), LanguageManager.GetText("XianYongShi") },
            
            // 陈留郡 (索引4)
            new string[] { LanguageManager.GetText("XianChangYuan"), LanguageManager.GetText("XianJiYang"), LanguageManager.GetText("XianChengWu"), LanguageManager.GetText("XianXiangYi"), LanguageManager.GetText("XianNingLing"), LanguageManager.GetText("XianFengQiu") },
            
            // 长沙郡 (索引5)
            new string[] { LanguageManager.GetText("XianLingLing"), LanguageManager.GetText("XianYiYang"), LanguageManager.GetText("XianXiangXian"), LanguageManager.GetText("XianYuanZhou"), LanguageManager.GetText("XianLuLing"), LanguageManager.GetText("XianHengShan"), LanguageManager.GetText("XianJianNing"), LanguageManager.GetText("XianGuiYang") },
            
            // 会稽郡 (索引6)
            new string[] { LanguageManager.GetText("XianQuA"), LanguageManager.GetText("XianSongJiang"), LanguageManager.GetText("XianShanYin"), LanguageManager.GetText("XianYuJi") },
            
            // 广陵郡 (索引7)
            new string[] { LanguageManager.GetText("XianPingAn"), LanguageManager.GetText("XianSheYang"), LanguageManager.GetText("XianHaiLing"), LanguageManager.GetText("XianJiangDu") },
            
            // 太原郡 (索引8)
            new string[] { LanguageManager.GetText("XianDaLing"), LanguageManager.GetText("XianJinYang"), LanguageManager.GetText("XianJiuYuan"), LanguageManager.GetText("XianShiCheng"), LanguageManager.GetText("XianYangQu"), LanguageManager.GetText("XianWeiYu"), LanguageManager.GetText("XianMengXian"), LanguageManager.GetText("XianZhongDu") },
            
            // 益州郡 (索引9)
            new string[] { LanguageManager.GetText("XianLianRan"), LanguageManager.GetText("XianGuChang"), LanguageManager.GetText("XianTongLao"), LanguageManager.GetText("XianKunZe"), LanguageManager.GetText("XianDianChi"), LanguageManager.GetText("XianYuYuan"), LanguageManager.GetText("XianShengXiu"), LanguageManager.GetText("XianNanAn") },
            
            // 南海郡 (索引10)
            new string[] { LanguageManager.GetText("XianSiHui"), LanguageManager.GetText("XianYangShan"), LanguageManager.GetText("XianLongChuan"), LanguageManager.GetText("XianJieLing"), LanguageManager.GetText("XianLuoYang"), LanguageManager.GetText("XianShanYu") },
            
            // 云南郡 (索引11)
            new string[] { LanguageManager.GetText("XianYunPing"), LanguageManager.GetText("XianYeYu"), LanguageManager.GetText("XianYongNing"), LanguageManager.GetText("XianSuiJiu"), LanguageManager.GetText("XianGuFu"), LanguageManager.GetText("XianQingLing"), LanguageManager.GetText("XianNongDong"), LanguageManager.GetText("XianXieLong") }
        };
        
        /// <summary>
        /// 根据郡索引和县索引获取郡县名称
        /// </summary>
        /// <param name="junIndex">郡索引</param>
        /// <param name="xianIndex">县索引</param>
        /// <returns>郡县名称，如果索引无效则返回"未知郡县"</returns>
        private static string GetJunXianNameFromIndex(int junIndex, int xianIndex)
        {
            try
            {
                if (junIndex >= 0 && junIndex < JunList.Length &&
                    xianIndex >= 0 && xianIndex < XianList[junIndex].Length)
                {
                    return $"{JunList[junIndex]}{XianList[junIndex][xianIndex]}";
                }
                return LanguageManager.GetText("未知郡县");
                }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError(LanguageManager.GetText("获取郡县名称失败: ") + ex.Message);
                return LanguageManager.GetText("未知郡县");
            }
        }
        
        /// <summary>
        /// 获取存储标识
        /// </summary>
        /// <returns>存储标识字符串</returns>
        public static string GetStorageIdentifier()
        {
            try
            {
                // 获取家族数据
                if (Mainload.FamilyData != null && Mainload.FamilyData.Count >= 2)
                {
                    // 检查FamilyData[0]和FamilyData[1]是否为null
                    if (Mainload.FamilyData[0] == null || Mainload.FamilyData[1] == null)
                    {
                        // 为空时返回"无相关配置"，保持重复任务逻辑初始状态
                        TaskCheat.Log?.LogWarning(LanguageManager.GetText("FamilyDataNullOrEmptyWarning"));
                        return LanguageManager.GetText("无相关配置");
                    }
                    
                    string familyData0 = Mainload.FamilyData[0].ToString();
                    string familyData1 = Mainload.FamilyData[1].ToString();
                    
                    // 检查CunDangIndex_now是否为null
                    string cundangIndex = Mainload.CunDangIndex_now?.ToString() ?? "未知";
                    
                    // 解析郡县信息
                    string junXianName = "未知郡县";
                    if (!string.IsNullOrEmpty(familyData0) && familyData0.Contains(FamilyDataKeyDelimiter))
                    {
                        try
                        {
                            // 使用char类型作为Split参数
                            string[] parts = familyData0.Split(FamilyDataKeyDelimiter[0]);
                            if (parts.Length >= 2 && int.TryParse(parts[0], out int junIndex) && int.TryParse(parts[1], out int xianIndex))
                            {
                                // 使用内部辅助方法获取郡县名称
                                junXianName = GetJunXianNameFromIndex(junIndex, xianIndex);
                            }
                        }
                        catch (Exception ex)
                        {
                            TaskCheat.Log?.LogWarning(LanguageManager.GetText("ParseCountyInfoError") + ex.Message);
                        }
                    }
                    
                    // 返回格式化的存储标识
                    return $"{junXianName}{familyData1}家（存档位置：{cundangIndex}）";
                }
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError(LanguageManager.GetText("GetStorageIdentifierError") + ex.Message);
                // 添加堆栈跟踪以便更好地诊断问题
                TaskCheat.Log?.LogError(ex.StackTrace);
            }
            
            // 出错时返回"无相关配置"，保持重复任务逻辑初始状态
            return LanguageManager.GetText("无相关配置");
        }
        
        /// <summary>
        /// 保存重复任务选择状态到BepInEx配置系统
        /// </summary>
        /// <param name="selectedTaskIndices">选中的任务索引列表</param>
        public static void SaveRepetitiveTaskSelection(List<int> selectedTaskIndices)
        {
            try
            {
                if (TaskCheat.Instance == null)
                {
                    TaskCheat.Log?.LogWarning(LanguageManager.GetText("TaskCheatInstanceIsNull"));
                    return;
                }

                string storageIdentifier = GetStorageIdentifier();
                string configKey = GetUniqueConfigKey(storageIdentifier, SelectedTasksKeySuffix);
                string configValue = string.Join(",", selectedTaskIndices);

                // 使用BepInEx配置系统保存
                ConfigEntry<string> configEntry = TaskCheat.Instance.Config.Bind<string>(
                    "重复任务配置", 
                    configKey, 
                    configValue, 
                    $"存储标识 [{storageIdentifier}] 的重复任务选择配置");
                configEntry.Value = configValue;
                
                // 确保配置被保存到文件
                TaskCheat.Instance.Config.Save();
                
                TaskCheat.Log?.LogInfo(LanguageManager.GetFormattedText("SavedRepetitiveTaskSelectionConfig", storageIdentifier));
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError(LanguageManager.GetText("SaveConfigFailed") + ex.Message);
            }
        }
        
        /// <summary>
        /// 生成唯一的配置键名，确保符合BepInEx配置系统的命名规则
        /// </summary>
        private static string GetUniqueConfigKey(string storageIdentifier, string suffix)
        {
            // 移除不适合作为配置键的字符
            string safeIdentifier = Regex.Replace(storageIdentifier, @"[^\w.-]", "_");
            // 限制长度，防止超过BepInEx的限制
            if (safeIdentifier.Length > 50)
            {
                safeIdentifier = safeIdentifier.Substring(0, 50);
            }
            return $"{ConfigKeyPrefix}{safeIdentifier}{suffix}";
        }
        
        /// <summary>
        /// 从BepInEx配置系统加载重复任务选择状态
        /// </summary>
        /// <returns>选中的任务索引列表</returns>
        public static List<int> LoadRepetitiveTaskSelection()
        {
            try
            {
                if (TaskCheat.Instance == null)
                {
                    TaskCheat.Log?.LogWarning(LanguageManager.GetText("TaskCheatInstanceIsNull"));
                    return new List<int>();
                }

                string storageIdentifier = GetStorageIdentifier();
                
                // 当存储标识为"无相关配置"时，保持重复任务逻辑初始化状态
                if (storageIdentifier == "无相关配置")
                {
                    TaskCheat.Log?.LogInfo(LanguageManager.GetText("StorageIdentifierIsDefaultValue"));
                    return new List<int>();
                }
                
                string configKey = GetUniqueConfigKey(storageIdentifier, SelectedTasksKeySuffix);
                
                // 增加详细日志记录
                TaskCheat.Log?.LogInfo(LanguageManager.GetFormattedText("TryLoadRepetitiveTaskConfig", storageIdentifier, configKey));
                
                // 使用GetConfigValueFromBepInEx方法获取配置
                string configValue = GetConfigValueFromBepInEx("重复任务配置", configKey);
                
                // 增加获取到的配置值日志
                TaskCheat.Log?.LogInfo(LanguageManager.GetFormattedText("RetrievedConfigValue", configValue));
                
                if (!string.IsNullOrEmpty(configValue))
                {
                    List<int> indices = configValue.Split(',')
                        .Where(s => int.TryParse(s, out _))
                        .Select(int.Parse)
                        .ToList();
                       
                    TaskCheat.Log?.LogInfo(LanguageManager.GetFormattedText("LoadedRepetitiveTaskConfig", indices.Count, storageIdentifier));
                    return indices;
                }
                  
                TaskCheat.Log?.LogInfo(LanguageManager.GetText("NoRepetitiveTaskSelectionConfigFound"));
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError(LanguageManager.GetText("LoadConfigFailed") + ex.Message);
                TaskCheat.Log?.LogError(ex.StackTrace);
            }
              
            return new List<int>();
        }
        
        /// <summary>
        /// 从BepInEx配置系统查找指定键的值
        /// </summary>
        private static string GetConfigValueFromBepInEx(string section, string key)
        {
            if (TaskCheat.Instance == null)
                return string.Empty;

            try
            {
                // 使用BepInEx的Config.Bind方法获取配置项
                var configEntry = TaskCheat.Instance.Config.Bind<string>(section, key, "");
                return configEntry.Value;
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError(LanguageManager.GetText("GetConfigValueError") + ex.Message);
                return string.Empty;
            }
        }
        
        /// <summary>
        /// 保存时间检查间隔到BepInEx配置系统（按存储标识保存）
        /// </summary>
        /// <param name="intervalSeconds">检查间隔（秒）</param>
        public static void SaveCheckInterval(int intervalSeconds)
        {
            try
            {
                // 验证并限制检查间隔在有效范围内
                int validatedInterval = ValidateCheckInterval(intervalSeconds);
                
                if (TaskCheat.Instance == null)
                {
                    TaskCheat.Log?.LogWarning(LanguageManager.GetText("TaskCheatInstanceIsNull"));
                    return;
                }
                
                string storageIdentifier = GetStorageIdentifier();
                string configKey = GetUniqueConfigKey(storageIdentifier, CheckIntervalKeySuffix);
                
                // 使用BepInEx配置系统保存，每个存储标识一个配置键
                ConfigEntry<int> configEntry = TaskCheat.Instance.Config.Bind<int>(
                    LanguageManager.GetText("重复任务设置"), 
                    configKey, 
                    validatedInterval, 
                    LanguageManager.GetFormattedText("存储标识 [{0}] 的任务检查间隔（秒）", storageIdentifier));
                configEntry.Value = validatedInterval;
                
                // 确保配置被保存到文件
                TaskCheat.Instance.Config.Save();
                
                TaskCheat.Log?.LogInfo(LanguageManager.GetFormattedText("SavedCheckIntervalConfig", validatedInterval, storageIdentifier));
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError(LanguageManager.GetText("SaveCheckIntervalConfigFailed") + ex.Message);
                TaskCheat.Log?.LogError(ex.StackTrace);
            }
        }
        
        /// <summary>
        /// 从BepInEx配置系统加载时间检查间隔（按存储标识加载）
        /// </summary>
        /// <returns>检查间隔（秒）</returns>
        public static int LoadCheckInterval()
        {
            try
            {
                if (TaskCheat.Instance == null)
                {
                    TaskCheat.Log?.LogWarning(LanguageManager.GetFormattedText("TaskCheatInstanceIsNullUsingDefaultInterval", DefaultCheckIntervalSeconds));
                    return DefaultCheckIntervalSeconds;
                }
                
                string storageIdentifier = GetStorageIdentifier();
                string configKey = GetUniqueConfigKey(storageIdentifier, CheckIntervalKeySuffix);
                
                // 从BepInEx配置系统获取时间间隔
                ConfigEntry<int> configEntry = TaskCheat.Instance.Config.Bind<int>(
                    LanguageManager.GetText("重复任务设置"), 
                    configKey, 
                    DefaultCheckIntervalSeconds, 
                    LanguageManager.GetFormattedText("存储标识 [{0}] 的任务检查间隔（秒）", storageIdentifier));
                
                int interval = configEntry.Value;
                
                // 验证并返回有效范围内的检查间隔
                int validatedInterval = ValidateCheckInterval(interval);
                
                // 如果验证后的间隔与配置值不同，更新配置
                if (validatedInterval != interval)
                {
                    configEntry.Value = validatedInterval;
                    TaskCheat.Instance.Config.Save();
                    TaskCheat.Log?.LogInfo(LanguageManager.GetFormattedText("CheckIntervalAdjustedToValidRange", validatedInterval, storageIdentifier));
                }
                else
                {
                    TaskCheat.Log?.LogInfo(LanguageManager.GetFormattedText("LoadedCheckIntervalFromBepInEx", validatedInterval, storageIdentifier));
                }
                
                return validatedInterval;
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError(LanguageManager.GetText("LoadCheckIntervalFromBepInExFailed") + ex.Message);
                TaskCheat.Log?.LogError(ex.StackTrace);
            }
            
            // 出错或配置不可用时返回默认值
            return DefaultCheckIntervalSeconds;
        }
        
        /// <summary>
        /// 验证检查间隔是否在有效范围内
        /// 如果配置错误则使用默认值
        /// </summary>
        /// <param name="intervalSeconds">检查间隔（秒）</param>
        /// <returns>验证后的检查间隔（秒）</returns>
        private static int ValidateCheckInterval(int intervalSeconds)
        {
            if (intervalSeconds < MinCheckIntervalSeconds || intervalSeconds > MaxCheckIntervalSeconds)
            {
                TaskCheat.Log?.LogWarning(LanguageManager.GetFormattedText("CheckIntervalOutOfValidRange", MinCheckIntervalSeconds, MaxCheckIntervalSeconds, DefaultCheckIntervalSeconds));
                return DefaultCheckIntervalSeconds;
            }
            return intervalSeconds;
        }
        
        /// <summary>
        /// 保存最后记录的时间到BepInEx配置系统
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        public static void SaveLastTimeRecord(int year, int month, int day)
        {
            try
            {
                if (TaskCheat.Instance == null)
                {
                    TaskCheat.Log?.LogWarning(LanguageManager.GetText("TaskCheatInstanceIsNull"));
                    return;
                }

                string storageIdentifier = GetStorageIdentifier();
                string configKey = GetUniqueConfigKey(storageIdentifier, LastTimeRecordKeySuffix);
                string configValue = $"{year},{month},{day}";

                // 使用BepInEx配置系统保存
                ConfigEntry<string> configEntry = TaskCheat.Instance.Config.Bind<string>(
                    "时间记录配置", 
                    configKey, 
                    configValue, 
                    $"存储标识 [{storageIdentifier}] 的最后记录时间");
                configEntry.Value = configValue;
                
                // 确保配置被保存到文件
                TaskCheat.Instance.Config.Save();
                
                TaskCheat.Log?.LogInfo(LanguageManager.GetFormattedText("SavedTimeRecordConfig", year, month, day, storageIdentifier));
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError(LanguageManager.GetText("SaveTimeRecordConfigFailed") + ex.Message);
            }
        }
        
        /// <summary>
        /// 从BepInEx配置系统加载最后记录的时间
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <returns>是否成功加载时间记录</returns>
        public static bool LoadLastTimeRecord(out int year, out int month, out int day)
        {
            year = -1;
            month = -1;
            day = -1;

            try
            {
                if (TaskCheat.Instance == null)
                {
                    TaskCheat.Log?.LogWarning(LanguageManager.GetText("TaskCheatInstanceIsNull"));
                    return false;
                }

                string storageIdentifier = GetStorageIdentifier();
                string configKey = GetUniqueConfigKey(storageIdentifier, LastTimeRecordKeySuffix);
                
                // 查找配置
                string configValue = GetConfigValueFromBepInEx("时间记录配置", configKey);

                if (!string.IsNullOrEmpty(configValue))
                {
                    string[] parts = configValue.Split(',');
                    if (parts.Length == 3 && 
                        int.TryParse(parts[0], out year) && 
                        int.TryParse(parts[1], out month) && 
                        int.TryParse(parts[2], out day))
                    {
                        TaskCheat.Log?.LogInfo(LanguageManager.GetFormattedText("LoadedTimeRecordConfig", year, month, day, storageIdentifier));
                        return true;
                    }
                }

                TaskCheat.Log?.LogInfo(LanguageManager.GetText("NoTimeRecordConfigFound"));
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError(LanguageManager.GetText("LoadTimeRecordConfigFailed") + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// 清除指定存储标识的配置
        /// </summary>
        /// <param name="storageIdentifier">存储标识</param>
        /// <returns>是否清除成功</returns>
        public static bool ClearConfigByStorageIdentifier(string storageIdentifier)
        {
            try
            {
                if (TaskCheat.Instance == null)
                {
                    TaskCheat.Log?.LogWarning(LanguageManager.GetText("TaskCheatInstanceIsNull"));
                    return false;
                }

                string safeIdentifier = Regex.Replace(storageIdentifier, @"[^\w.-]", "_");
                string[] sections = { "重复任务配置", "时间记录配置", "重复任务设置" };
                
                // 尝试清空所有可能的配置键
                string[] possibleSuffixes = { SelectedTasksKeySuffix, LastTimeRecordKeySuffix, CheckIntervalKeySuffix };
                foreach (string suffix in possibleSuffixes)
                {
                    string configKey = GetUniqueConfigKey(safeIdentifier, suffix);
                    foreach (string section in sections)
                    {
                        try
                        {
                            var configEntry = TaskCheat.Instance.Config.Bind<string>(section, configKey, "");
                            configEntry.Value = "";
                        }
                        catch { }
                    }
                }

                // 保存更新后的配置
                TaskCheat.Instance.Config.Save();
                
                TaskCheat.Log?.LogInfo(LanguageManager.GetFormattedText("ClearedConfigByStorageIdentifier", storageIdentifier));
                return true;
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError(LanguageManager.GetText("ClearConfigError") + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 清除所有配置
        /// </summary>
        /// <returns>是否清除成功</returns>
        public static bool ClearAllConfig()
        {
            try
            {
                if (TaskCheat.Instance == null)
                {
                    TaskCheat.Log?.LogWarning(LanguageManager.GetText("TaskCheatInstanceIsNull"));
                    return false;
                }

                // 清空任务检查间隔配置
                if (TaskCheat.TaskCheckInterval != null)
                {
                    TaskCheat.TaskCheckInterval.Value = DefaultCheckIntervalSeconds;
                }

                // 定义我们需要查找的所有可能的section
                string[] sections = { "重复任务配置", "时间记录配置", "重复任务设置" };
                
                // 使用反射获取BepInEx配置系统中的所有配置节
                System.Reflection.PropertyInfo configDataProperty = TaskCheat.Instance.Config.GetType().GetProperty("ConfigData", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (configDataProperty != null)
                {
                    try
                    {
                        var configData = configDataProperty.GetValue(TaskCheat.Instance.Config) as System.Collections.Generic.Dictionary<string, object>;
                        if (configData != null)
                        {
                            // 保存所有键的副本，以便安全地迭代和修改
                            List<string> keysToRemove = new List<string>();
                            foreach (var key in configData.Keys)
                            {
                                // 检查键是否以我们关心的任何section开头
                                foreach (string section in sections)
                                {
                                    if (key.Equals(section))
                                    {
                                        keysToRemove.Add(key);
                                        break;
                                    }
                                }
                            }

                            // 移除所有匹配的section
                            foreach (string key in keysToRemove)
                            {
                                configData.Remove(key);
                                TaskCheat.Log?.LogInfo(LanguageManager.GetFormattedText("ClearedConfigSection", key));
                            }

                        }
                    }
                    catch (Exception reflectionEx)
                    {
                        TaskCheat.Log?.LogWarning(LanguageManager.GetText("ReflectionClearConfigFailed") + reflectionEx.Message);
                    }
                }

                // 强制删除配置文件作为最后的手段，确保完全清除
                try
                {
                    string configFilePath = TaskCheat.Instance.Config.ConfigFilePath;
                    if (System.IO.File.Exists(configFilePath))
                    {
                        // 先保存当前配置（确保没有未保存的更改）
                        TaskCheat.Instance.Config.Save();
                        
                        // 等待配置文件写入完成
                        System.Threading.Thread.Sleep(100);
                        
                        // 强制删除配置文件
                        System.IO.File.Delete(configFilePath);
                        TaskCheat.Log?.LogInfo(LanguageManager.GetFormattedText("ForceDeletedConfigFile", configFilePath));
                          
                        // 重新创建一个空的配置文件
                        TaskCheat.Instance.Config.Save();
                        TaskCheat.Log?.LogInfo(LanguageManager.GetText("RecreatedEmptyConfigFile"));
                    }
                }
                catch (Exception fileEx)
                {
                    TaskCheat.Log?.LogWarning(LanguageManager.GetText("ForceDeleteConfigFileFailed") + fileEx.Message);
                }
                
                // 保存更新后的配置
                TaskCheat.Instance.Config.Save();
                
                TaskCheat.Log?.LogInfo(LanguageManager.GetText("SuccessfullyClearedAllConfig"));
                return true;
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError(LanguageManager.GetText("ClearAllConfigError") + ex.Message);
                TaskCheat.Log?.LogError(ex.StackTrace);
                return false;
            }
        }
    }
}