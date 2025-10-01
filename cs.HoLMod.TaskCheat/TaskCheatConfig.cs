using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace cs.HoLMod.TaskCheat
{
    /// <summary>
    /// 任务修改器配置管理类
    /// </summary>
    public static class TaskCheatConfig
    {
        private static string ConfigFilePath => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "TaskCheatConfig.txt");
        private const int DefaultCheckIntervalSeconds = 10; // 默认检查间隔为10秒
        private const int MinCheckIntervalSeconds = 1; // 最小检查间隔为1秒
        private const int MaxCheckIntervalSeconds = 10; // 最大检查间隔为10秒
        private const string DefaultStorageIdentifier = "default";
        private const string SectionDelimiter = "##";
        private const string ConfigKeyDelimiter = "=";
        private const string FamilyDataKeyDelimiter = "|";
        
        // 郡数组，索引对应郡ID
        private static string[] JunList = new string[]
        {
            "南郡",     // 0
            "三川郡",   // 1
            "蜀郡",     // 2
            "丹阳郡",   // 3
            "陈留郡",   // 4
            "长沙郡",   // 5
            "会稽郡",   // 6
            "广陵郡",   // 7
            "太原郡",   // 8
            "益州郡",   // 9
            "南海郡",   // 10
            "云南郡"    // 11
        };

        // 二维县数组，第一维是郡索引，第二维是县索引
        private static string[][] XianList = new string[][]
        {
            // 南郡 (索引0)
            new string[] { "临沮", "襄樊", "宜城", "麦城", "华容", "郢亭", "江陵", "夷陵" },
            
            // 三川郡 (索引1)
            new string[] { "平阳", "荥阳", "原武", "阳武", "新郑", "宜阳" },
            
            // 蜀郡 (索引2)
            new string[] { "邛崃", "郫县", "什邡", "绵竹", "新都", "成都" },
            
            // 丹阳郡 (索引3)
            new string[] { "秣陵", "江乘", "江宁", "溧阳", "建邺", "永世" },
            
            // 陈留郡 (索引4)
            new string[] { "长垣", "济阳", "成武", "襄邑", "宁陵", "封丘" },
            
            // 长沙郡 (索引5)
            new string[] { "零陵", "益阳", "湘县", "袁州", "庐陵", "衡山", "建宁", "桂阳" },
            
            // 会稽郡 (索引6)
            new string[] { "曲阿", "松江", "山阴", "余暨" },
            
            // 广陵郡 (索引7)
            new string[] { "平安", "射阳", "海陵", "江都" },
            
            // 太原郡 (索引8)
            new string[] { "大陵", "晋阳", "九原", "石城", "阳曲", "魏榆", "孟县", "中都" },
            
            // 益州郡 (索引9)
            new string[] { "连然", "谷昌", "同劳", "昆泽", "滇池", "俞元", "胜休", "南安" },
            
            // 南海郡 (索引10)
            new string[] { "四会", "阳山", "龙川", "揭岭", "罗阳", "善禺" },
            
            // 云南郡 (索引11)
            new string[] { "云平", "叶榆", "永宁", "遂久", "姑复", "蜻陵", "弄栋", "邪龙" }
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
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("获取郡县名称时出错: " + ex.Message);
            }
            
            return "未知郡县";
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
                    string familyData0 = Mainload.FamilyData[0].ToString();
                    string familyData1 = Mainload.FamilyData[1].ToString();
                    string cundangIndex = Mainload.CunDangIndex_now.ToString();
                    
                    // 解析郡县信息
                    string junXianName = "未知郡县";
                    if (familyData0.Contains(FamilyDataKeyDelimiter))
                    {
                        // 使用char类型作为Split参数
                        string[] parts = familyData0.Split(FamilyDataKeyDelimiter[0]);
                        if (parts.Length >= 2 && int.TryParse(parts[0], out int junIndex) && int.TryParse(parts[1], out int xianIndex))
                        {
                            // 使用内部辅助方法获取郡县名称
                            junXianName = GetJunXianNameFromIndex(junIndex, xianIndex);
                        }
                    }
                    
                    // 返回格式化的存储标识
                    return $"{junXianName}{familyData1}家（存档位置：{cundangIndex}）";
                }
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("获取存储标识时出错: " + ex.Message);
            }
            
            // 出错时返回默认标识
            return DefaultStorageIdentifier;
        }
        
        /// <summary>
        /// 保存重复任务选择状态到配置文件
        /// </summary>
        /// <param name="selectedTaskIndices">选中的任务索引列表</param>
        public static void SaveRepetitiveTaskSelection(List<int> selectedTaskIndices)
        {
            try
            {
                string storageIdentifier = GetStorageIdentifier();
                string configKey = "SelectedTasks";
                string configValue = string.Join(",", selectedTaskIndices);
                
                SaveConfigValue(storageIdentifier, configKey, configValue);
                
                TaskCheat.Log?.LogInfo($"已保存重复任务选择配置到: {ConfigFilePath}（存储标识: {storageIdentifier}）");
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("保存配置文件失败: " + ex.Message);
            }
        }
        
        /// <summary>
        /// 保存配置值到指定存储标识下
        /// </summary>
        /// <param name="storageIdentifier">存储标识</param>
        /// <param name="configKey">配置键</param>
        /// <param name="configValue">配置值</param>
        private static void SaveConfigValue(string storageIdentifier, string configKey, string configValue)
        {
            try
            {
                // 读取现有配置
                string existingContent = File.Exists(ConfigFilePath) ? File.ReadAllText(ConfigFilePath, Encoding.UTF8) : string.Empty;
                List<string> lines = existingContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                
                // 构建完整的配置键
                string fullConfigKey = $"{storageIdentifier}.{configKey}";
                bool foundSection = false;
                bool updated = false;
                
                // 查找并更新现有配置
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].StartsWith(SectionDelimiter))
                    {
                        string section = lines[i].Substring(SectionDelimiter.Length).Trim();
                        if (section.Equals(storageIdentifier))
                        {
                            foundSection = true;
                            // 在当前标识下查找配置键
                            for (int j = i + 1; j < lines.Count && !lines[j].StartsWith(SectionDelimiter); j++)
                            {
                                if (lines[j].StartsWith($"{configKey}{ConfigKeyDelimiter}"))
                                {
                                    lines[j] = $"{configKey}{ConfigKeyDelimiter}{configValue}";
                                    updated = true;
                                    break;
                                }
                            }
                            
                            // 如果当前标识下没有找到配置键，添加新的配置键值对
                            if (!updated && foundSection)
                            {
                                // 找到当前标识下的最后一行
                                int lastLineOfSection = i + 1;
                                while (lastLineOfSection < lines.Count && !lines[lastLineOfSection].StartsWith(SectionDelimiter))
                                {
                                    lastLineOfSection++;
                                }
                                lines.Insert(lastLineOfSection, $"{configKey}{ConfigKeyDelimiter}{configValue}");
                            }
                            break;
                        }
                    }
                }
                
                // 如果没有找到标识，添加新的标识和配置
                if (!foundSection)
                {
                    if (lines.Count > 0 && !string.IsNullOrWhiteSpace(lines[lines.Count - 1]))
                    {
                        lines.Add(string.Empty); // 添加空行分隔
                    }
                    lines.Add($"{SectionDelimiter} {storageIdentifier}");
                    lines.Add($"{configKey}{ConfigKeyDelimiter}{configValue}");
                }
                
                // 写入配置文件
                File.WriteAllText(ConfigFilePath, string.Join(Environment.NewLine, lines), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("保存配置值失败: " + ex.Message);
                throw;
            }
        }
        
        /// <summary>
        /// 从配置文件加载重复任务选择状态
        /// </summary>
        /// <returns>选中的任务索引列表</returns>
        public static List<int> LoadRepetitiveTaskSelection()
        {
            try
            {
                string storageIdentifier = GetStorageIdentifier();
                string configKey = "SelectedTasks";
                string configValue = LoadConfigValue(storageIdentifier, configKey);
                
                if (!string.IsNullOrEmpty(configValue))
                {
                    List<int> indices = configValue.Split(',')
                        .Where(s => int.TryParse(s, out _))
                        .Select(int.Parse)
                        .ToList();
                    
                    TaskCheat.Log?.LogInfo($"已加载重复任务选择配置，选中任务数量: {indices.Count}（存储标识: {storageIdentifier}）");
                    return indices;
                }
                
                TaskCheat.Log?.LogInfo("未找到重复任务选择配置，将使用默认配置");
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("加载配置文件失败: " + ex.Message);
            }
            
            return new List<int>();
        }
        
        /// <summary>
        /// 从指定存储标识下加载配置值
        /// </summary>
        /// <param name="storageIdentifier">存储标识</param>
        /// <param name="configKey">配置键</param>
        /// <returns>配置值，如果未找到则返回空字符串</returns>
        private static string LoadConfigValue(string storageIdentifier, string configKey)
        {
            try
            {
                if (!File.Exists(ConfigFilePath))
                {
                    return string.Empty;
                }
                
                string content = File.ReadAllText(ConfigFilePath, Encoding.UTF8);
                List<string> lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                
                bool inTargetSection = false;
                
                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();
                    
                    // 检查是否进入目标标识区域
                    if (trimmedLine.StartsWith(SectionDelimiter))
                    {
                        string section = trimmedLine.Substring(SectionDelimiter.Length).Trim();
                        inTargetSection = section.Equals(storageIdentifier);
                    }
                    // 在目标标识区域内查找配置键
                    else if (inTargetSection && trimmedLine.StartsWith($"{configKey}{ConfigKeyDelimiter}"))
                    {
                        int delimiterIndex = trimmedLine.IndexOf(ConfigKeyDelimiter);
                        if (delimiterIndex > 0)
                        {
                            return trimmedLine.Substring(delimiterIndex + 1).Trim();
                        }
                    }
                }
                
                return string.Empty;
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("加载配置值失败: " + ex.Message);
                return string.Empty;
            }
        }
        
        /// <summary>
        /// 保存时间检查间隔到配置文件
        /// </summary>
        /// <param name="intervalSeconds">检查间隔（秒）</param>
        public static void SaveCheckInterval(int intervalSeconds)
        {
            try
            {
                // 验证并限制检查间隔在有效范围内
                int validatedInterval = ValidateCheckInterval(intervalSeconds);
                string storageIdentifier = GetStorageIdentifier();
                string configKey = "CheckInterval";
                string configValue = validatedInterval.ToString();
                
                SaveConfigValue(storageIdentifier, configKey, configValue);
                
                TaskCheat.Log?.LogInfo($"已保存时间检查间隔配置: {validatedInterval}秒（存储标识: {storageIdentifier}）");
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("保存检查间隔配置失败: " + ex.Message);
            }
        }
        
        /// <summary>
        /// 从配置文件加载时间检查间隔
        /// </summary>
        /// <returns>检查间隔（秒）</returns>
        public static int LoadCheckInterval()
        {
            try
            {
                string storageIdentifier = GetStorageIdentifier();
                string configKey = "CheckInterval";
                string configValue = LoadConfigValue(storageIdentifier, configKey);
                
                if (!string.IsNullOrEmpty(configValue) && int.TryParse(configValue, out int interval))
                {
                    // 验证并返回有效范围内的检查间隔
                    int validatedInterval = ValidateCheckInterval(interval);
                    TaskCheat.Log?.LogInfo($"已加载时间检查间隔: {validatedInterval}秒（存储标识: {storageIdentifier}）");
                    return validatedInterval;
                }
                
                // 如果没有找到当前标识的配置，尝试加载默认标识的配置
                if (storageIdentifier != DefaultStorageIdentifier)
                {
                    string defaultConfigValue = LoadConfigValue(DefaultStorageIdentifier, configKey);
                    if (!string.IsNullOrEmpty(defaultConfigValue) && int.TryParse(defaultConfigValue, out int defaultInterval))
                    {
                        int validatedDefaultInterval = ValidateCheckInterval(defaultInterval);
                        TaskCheat.Log?.LogInfo($"未找到当前标识的检查间隔配置，使用默认标识的配置: {validatedDefaultInterval}秒");
                        return validatedDefaultInterval;
                    }
                }
                
                TaskCheat.Log?.LogInfo($"未找到检查间隔配置，使用默认值: {DefaultCheckIntervalSeconds}秒");
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("加载检查间隔配置失败: " + ex.Message);
            }
            
            // 出错或配置无效时返回默认值
            return DefaultCheckIntervalSeconds;
        }
        
        /// <summary>
        /// 验证检查间隔是否在有效范围内
        /// 如果配置错误则使用默认值并将配置文件更新为默认值
        /// </summary>
        /// <param name="intervalSeconds">检查间隔（秒）</param>
        /// <returns>验证后的检查间隔（秒）</returns>
        private static int ValidateCheckInterval(int intervalSeconds)
        {
            if (intervalSeconds < MinCheckIntervalSeconds || intervalSeconds > MaxCheckIntervalSeconds)
            {
                TaskCheat.Log?.LogWarning("检查间隔不在有效范围内（" + MinCheckIntervalSeconds + "-" + MaxCheckIntervalSeconds + "秒），使用默认值: " + DefaultCheckIntervalSeconds + "秒");
                
                // 配置错误时，将配置文件更新为默认值
                string storageIdentifier = GetStorageIdentifier();
                string configKey = "CheckInterval";
                SaveConfigValue(storageIdentifier, configKey, DefaultCheckIntervalSeconds.ToString());
                
                return DefaultCheckIntervalSeconds;
            }
            return intervalSeconds;
        }
        
        /// <summary>
        /// 保存最后记录的时间到配置文件
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        public static void SaveLastTimeRecord(int year, int month, int day)
        {
            try
            {
                string storageIdentifier = GetStorageIdentifier();
                string configKey = "LastTimeRecord";
                string configValue = $"{year},{month},{day}";
                
                SaveConfigValue(storageIdentifier, configKey, configValue);
                
                TaskCheat.Log?.LogInfo($"已保存时间记录配置: {year}年{month}月{day}日（存储标识: {storageIdentifier}）");
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("保存时间记录配置失败: " + ex.Message);
            }
        }
        
        /// <summary>
        /// 从配置文件加载最后记录的时间
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
                string storageIdentifier = GetStorageIdentifier();
                string configKey = "LastTimeRecord";
                string configValue = LoadConfigValue(storageIdentifier, configKey);

                if (!string.IsNullOrEmpty(configValue))
                {
                    string[] parts = configValue.Split(',');
                    if (parts.Length == 3 && 
                        int.TryParse(parts[0], out year) && 
                        int.TryParse(parts[1], out month) && 
                        int.TryParse(parts[2], out day))
                    {
                        TaskCheat.Log?.LogInfo($"已加载时间记录配置: {year}年{month}月{day}日（存储标识: {storageIdentifier}）");
                        return true;
                    }
                }

                TaskCheat.Log?.LogInfo("未找到时间记录配置");
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("加载时间记录配置失败: " + ex.Message);
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
                if (!File.Exists(ConfigFilePath))
                {
                    TaskCheat.Log?.LogInfo("配置文件不存在");
                    return true;
                }

                string content = File.ReadAllText(ConfigFilePath, Encoding.UTF8);
                List<string> lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

                List<string> newLines = new List<string>();
                bool inTargetSection = false;

                for (int i = 0; i < lines.Count; i++)
                {
                    string trimmedLine = lines[i].Trim();

                    // 检查是否进入目标标识区域
                    if (trimmedLine.StartsWith(SectionDelimiter))
                    {
                        string section = trimmedLine.Substring(SectionDelimiter.Length).Trim();
                        if (section.Equals(storageIdentifier))
                        {
                            inTargetSection = true;
                            // 跳过目标标识的所有内容
                            while (i + 1 < lines.Count && !lines[i + 1].StartsWith(SectionDelimiter))
                            {
                                i++;
                            }
                        }
                        else
                        {
                            inTargetSection = false;
                            newLines.Add(lines[i]);
                        }
                    }
                    else if (!inTargetSection)
                    {
                        newLines.Add(lines[i]);
                    }
                }

                // 写入更新后的配置
                File.WriteAllText(ConfigFilePath, string.Join(Environment.NewLine, newLines), Encoding.UTF8);
                TaskCheat.Log?.LogInfo($"已清除存储标识为 '{storageIdentifier}' 的配置");
                return true;
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("清除配置文件时出错: " + ex.Message);
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
                if (File.Exists(ConfigFilePath))
                {
                    File.Delete(ConfigFilePath);
                    TaskCheat.Log?.LogInfo("已成功清除所有配置");
                }
                return true;
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("清除所有配置时出错: " + ex.Message);
                return false;
            }
        }
    }
}