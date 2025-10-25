using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MultifunctionalCheat
{
    // 插件信息类
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "cs.HoLMod.MultifunctionalCheat.AnZhi20";
        public const string PLUGIN_NAME = "HoLMod.MultifunctionalCheat";
        public const string PLUGIN_VERSION = "1.6.0";
        public const string PLUGIN_CONFIG = "cs.HoLMod.MultifunctionalCheat.AnZhi20.cfg";
    }
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class PluginMain : BaseUnityPlugin
    {
        // 当前插件版本（设为public static以便其他方法访问）
        public static readonly string CURRENT_VERSION = PluginInfo.PLUGIN_VERSION; // 与BepInPlugin属性中定义的版本保持一致
        
        private void Awake()
        {
            Logger.LogInfo("多功能修改器已加载！当前版本：" + CURRENT_VERSION);
            Logger.LogInfo("The multifunctional modifier has been loaded! Current version:" + CURRENT_VERSION);
            
            // 配置文件路径
            string configFilePath = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_CONFIG);
            
            try
            {
                // 检查是否存在配置文件
                if (File.Exists(configFilePath))
                {
                    // 读取配置文件中的版本信息
                    string loadedVersion = "";
                    using (StreamReader reader = new StreamReader(configFilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            // 修改：使用更宽松的匹配方式，不依赖等号前后空格
                            if (line.Contains("已加载版本（Loaded Version）"))
                            {
                                // 使用IndexOf('=')找到等号位置，然后提取等号后面的内容
                                int equalsIndex = line.IndexOf('=');
                                if (equalsIndex > 0)
                                {
                                    loadedVersion = line.Substring(equalsIndex + 1).Trim();
                                }
                                break;
                            }
                        }
                    }
                    
                    // 检查是否需要更新配置
                    bool isVersionUpdated = loadedVersion != CURRENT_VERSION;
                    
                    // 如果版本更新，保存原有配置数据后删除配置文件
                    if (isVersionUpdated)
                    {
                        Logger.LogInfo($"检测到插件版本更新至 {CURRENT_VERSION}，正在保存原有配置数据...");
                        Logger.LogInfo($"Detected plugin version update to {CURRENT_VERSION}, saving existing configuration data...");
                        
                        // 保存原有配置数据
                        Dictionary<string, string> oldConfigData = new Dictionary<string, string>();
                        if (!File.Exists(configFilePath))
                        {
                            Logger.LogWarning("配置文件不存在，无法保存原有配置数据。");
                            Logger.LogWarning("Configuration file does not exist, cannot save original configuration data.");
                        }
                        else
                        {
                            using (StreamReader reader = new StreamReader(configFilePath))
                            {
                                string line;
                                string currentSection = "";
                                
                                while ((line = reader.ReadLine()) != null)
                                {
                                    // 跳过注释和空行
                                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
                                    {
                                        continue;
                                    }
                                    
                                    // 检测段落标记
                                    if (line.StartsWith("["))
                                    {
                                        currentSection = line.Trim();
                                        continue;
                                    }
                                    
                                    // 解析配置项
                                    int equalsIndex = line.IndexOf('=');
                                    if (equalsIndex > 0 && !string.IsNullOrEmpty(currentSection))
                                    {
                                        string key = currentSection + ":" + line.Substring(0, equalsIndex).Trim();
                                        string value = line.Substring(equalsIndex + 1).Trim();
                                        oldConfigData[key] = value;
                                    }
                                }
                            }
                        }
                        
                        // 删除旧配置文件前确保它存在
                        if (File.Exists(configFilePath))
                        {
                            try
                            {
                                // 确保没有文件句柄被锁定
                                File.Delete(configFilePath);
                                
                                // 验证文件是否真的被删除
                                if (!File.Exists(configFilePath))
                                {
                                    Logger.LogInfo("旧配置文件已成功删除。");
                                    Logger.LogInfo("Old configuration file has been successfully deleted.");
                                    Logger.LogInfo("确认旧配置文件已被完全删除。");
                                    Logger.LogInfo("Confirmed old configuration file has been completely deleted.");
                                }
                                else
                                {
                                    Logger.LogWarning("警告：旧配置文件可能未被完全删除。");
                                    Logger.LogWarning("Warning: Old configuration file may not have been completely deleted.");
                                    
                                    // 尝试使用另一种方式删除
                                    try
                                    {
                                        // 设置为正常属性（以防只读）
                                        File.SetAttributes(configFilePath, FileAttributes.Normal);
                                        File.Delete(configFilePath);
                                        
                                        if (!File.Exists(configFilePath))
                                        {
                                            Logger.LogInfo("通过备用方法成功删除旧配置文件。");
                                            Logger.LogInfo("Successfully deleted old configuration file using alternative method.");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.LogError($"备用删除方法失败: {ex.Message}");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError($"删除旧配置文件时发生错误: {ex.Message}");
                            }
                        }
                        else
                        {
                            Logger.LogWarning("未找到配置文件：" + configFilePath);
                            Logger.LogWarning("Configuration file not found: " + configFilePath);
                        }
                        
                        try
                        {
                            // 保存旧配置数据，以便在新配置文件生成后恢复
                            PluginMain._tempOldConfigData = oldConfigData;
                            Logger.LogInfo("已保存旧配置数据，将在新配置文件生成后恢复。");
                            Logger.LogInfo("Old configuration data saved, will be restored after new config file generation.");
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("保存临时配置数据时出错: " + ex.Message);
                            // 释放资源
                            oldConfigData.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("读取或删除配置文件时出错: " + ex.Message);
            }
            
            // 配置游戏倍率（无论是否删除配置文件都会执行，确保配置项存在）
            PluginMain.城中可招门客上限倍数 = base.Config.Bind<int>("倍率调整（Magnification）", "城中可招门客上限倍数（CityMaxRecruitableCustomersMultiplier）", 1, "城中可招募门客的数量=家族等级*2*城中可招门客上限倍数，填1为不修改（The number of potential recruits in the city = family level * 2 * CityMaxRecruitableCustomersMultiplier , default: 1）");
            PluginMain.城中可建民居上限倍数 = base.Config.Bind<int>("倍率调整（Magnification）", "城中可建民居上限倍数（CityMaxBuildableHousesMultiplier）", 1, "所有城中可购买民居数量总和=家族等级*城中可建民居上限倍数，填1为不修改（The total number of potential buildings in the city = family level * CityMaxBuildableHousesMultiplier , default: 1）");
            PluginMain.城中可建商铺上限倍数 = base.Config.Bind<int>("倍率调整（Magnification）", "城中可建商铺上限倍数（CityMaxBuildableShopsMultiplier）", 1, "所有城中可建造商铺数量总和=家族等级*城中可建商铺上限倍数，填1为不修改（The total number of potential shops in the city = family level * CityMaxBuildableShopsMultiplier , default: 1）");
            PluginMain.全角色体力上限倍数 = base.Config.Bind<int>("倍率调整（Magnification）", "全角色体力上限倍数（AllCharactersMaxHealthMultiplier）", 1, "全角色体力上限=默认上限*全角色体力上限倍数，填1为不修改（The maximum health of all characters = default health * AllCharactersMaxHealthMultiplier , default: 1）");
            PluginMain.城中商铺兑换元宝上限 = base.Config.Bind<int>("倍率调整（Magnification）", "城中商铺兑换元宝上限倍数（CityShopExchangeGoldMultiplier）", 1, "所有城中每个商铺可兑换元宝上限=钱庄等级*10*城中商铺兑换元宝上限倍数，填1为不修改（The maximum gold exchange per shop in the city = bank level * 10 * CityShopExchangeGoldMultiplier , default: 1）");
            PluginMain.科举人数上限 = base.Config.Bind<int>("倍率调整（Magnification）", "科举人数上限倍数（ExaminationNumMaxMultiplier）", 1, "科举可选人数上限=家族等级*科举人数上限倍数，填1为不修改（The maximum number of candidates for the exam = family level * ExaminationNumMaxMultiplier , default: 1）");
            PluginMain.最大子嗣上限 = base.Config.Bind<int>("倍率调整（Magnification）", "最大子嗣上限倍数（MaxOffspringNumMultiplier）", 1, "每个女性可以生的子嗣上限=1（或2）*最大子嗣上限倍数，填1为不修改（The maximum number of children that each woman can have = 1（or 2） * MaxOffspringNumMultiplier , default: 1）");
            
            // 折扣相关配置
            PluginMain.民居售价折扣 = base.Config.Bind<float>("折扣（Discount）", "民居售价折扣（HousePriceMultiplier）", 10, "每个民居的售价=默认售价*民居售价折扣/10，填10为不修改（The price of each house = default price * HousePriceMultiplier /10, default: 10）");
            PluginMain.府邸销售折扣 = base.Config.Bind<float>("折扣（Discount）", "府邸销售折扣（HouseSaleMultiplier）", 10, "每个府邸的销售价格=默认售价*府邸销售折扣/10，填10为不修改（The price of each mansion = default price * HouseSaleMultiplier /10, default: 10）");

            // 封地税收倍率配置
            PluginMain.封地农业税收倍数 = base.Config.Bind<float>("封地参数（FengDiParameters）", "封地农业税收（AgriculturalTaxMultiplier）", 1, "每个封地的农业税收=原本计算逻辑*封地农业税收倍数，填1为不修改（Agricultural tax revenue for each fiefdom = Original calculation logic * AgriculturalTaxMultiplier , default: 1）");
            PluginMain.封地人民税收倍数 = base.Config.Bind<float>("封地参数（FengDiParameters）", "封地人民税收（ResidentTaxMultiplier）", 1, "每个封地的人民税收=原本计算逻辑*封地人民税收倍数，填1为不修改（Resident tax revenue for each fiefdom = Original calculation logic * ResidentTaxMultiplier , default: 1）");
            PluginMain.封地商业税收倍数 = base.Config.Bind<float>("封地参数（FengDiParameters）", "封地商业税收（CommercialTaxMultiplier）", 1, "每个封地的商业税收=原本计算逻辑*封地商业税收倍数，填1为不修改（Commercial tax revenue for each fiefdom = Original calculation logic * CommercialTaxMultiplier , default: 1）");
            
            // 生育年龄相关配置
            PluginMain.最小生育年龄 = base.Config.Bind<int>("怀孕参数（PregnancyParameters）", "最小生育年龄（MinPregnancyAge）", 18, "女性可怀孕的最小年龄，最好不要低于18岁，人不能至少不应该（The minimum age at which women can become pregnant,it is best not to be under 18 years old, as people should not be at least）");
            PluginMain.最大生育年龄 = base.Config.Bind<int>("怀孕参数（PregnancyParameters）", "最大生育年龄（MaxPregnancyAge）", 40, "女性可怀孕的最大年龄（The maximum age at which women can become pregnant）");
            
            // 保存当前版本号
            base.Config.Bind("内部配置（Internal Settings）", "已加载版本（Loaded Version）", CURRENT_VERSION, "用于跟踪插件版本，请勿手动修改");
            
            // 保存配置文件
            base.Config.Save();
            
            // 恢复旧配置数据（如果有）
            if (PluginMain._tempOldConfigData != null && PluginMain._tempOldConfigData.Count > 0)
            {
                try
                {
                    Logger.LogInfo("正在恢复原有配置数据...");
                    Logger.LogInfo("Restoring existing configuration data...");
                    
                    string newConfigFilePath = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_CONFIG);
                    
                    // 确保新配置文件存在
                    if (!File.Exists(newConfigFilePath))
                    {
                        Logger.LogError("错误：新配置文件不存在，无法恢复配置数据。");
                        Logger.LogError("Error: New configuration file does not exist, cannot restore configuration data.");
                        return;
                    }
                    
                    // 读取新生成的配置文件内容
                    List<string> fileLines = new List<string>();
                    string currentSection = "";
                    int originalConfigCount = 0;
                    
                    using (StreamReader reader = new StreamReader(newConfigFilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            fileLines.Add(line);
                            
                            // 跳过注释和空行，统计有效配置项数量
                            if (!string.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("#") && !line.StartsWith("["))
                            {
                                int equalsIndex = line.IndexOf('=');
                                if (equalsIndex > 0)
                                {
                                    originalConfigCount++;
                                }
                            }
                        }
                    }
                    
                    Logger.LogInfo($"新配置文件中的有效配置项数量: {originalConfigCount}");
                    
                    // 恢复配置数据
                    bool hasChanges = false;
                    int restoredConfigCount = 0;
                    
                    for (int i = 0; i < fileLines.Count; i++)
                    {
                        string line = fileLines[i];
                        
                        // 跳过注释和空行
                        if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
                        {
                            continue;
                        }
                        
                        // 检测段落标记
                        if (line.StartsWith("["))
                        {
                            currentSection = line.Trim();
                            continue;
                        }
                        
                        // 解析配置项
                        int equalsIndex = line.IndexOf('=');
                        if (equalsIndex > 0 && !string.IsNullOrEmpty(currentSection))
                        {
                            string key = currentSection + ":" + line.Substring(0, equalsIndex).Trim();
                            
                            // 跳过内部配置中的版本参数，确保它始终使用当前版本
                            if (currentSection == "[内部配置（Internal Settings）]" && 
                                line.Substring(0, equalsIndex).Trim() == "已加载版本（Loaded Version）")
                            {
                                // 确保版本号始终为当前版本，无需从旧配置恢复
                                Logger.LogInfo("保持内部版本参数为当前版本: " + PluginMain.CURRENT_VERSION);
                                continue;
                            }
                            
                            // 如果旧配置中有这个配置项
                            if (PluginMain._tempOldConfigData.ContainsKey(key))
                            {
                                // 更新为旧配置的值
                                string oldValue = PluginMain._tempOldConfigData[key];
                                fileLines[i] = line.Substring(0, equalsIndex + 1).Trim() + " " + oldValue;
                                hasChanges = true;
                                restoredConfigCount++;
                                Logger.LogInfo($"恢复配置项: {key} = {oldValue}");
                            }
                        }
                    }
                    
                    // 清理重复段落和配置项
                    Logger.LogInfo("开始清理配置文件中的重复内容...");
                    List<string> cleanedLines = new List<string>();
                    HashSet<string> seenSections = new HashSet<string>();
                    HashSet<string> seenConfigs = new HashSet<string>();
                    string currentSectionClean = "";
                    int duplicateCount = 0;
                    
                    for (int i = 0; i < fileLines.Count; i++)
                    {
                        string line = fileLines[i];
                        
                        // 处理段落标记
                        if (line.Trim().StartsWith("["))
                        {
                            string section = line.Trim();
                            // 检查是否重复段落
                            if (seenSections.Contains(section))
                            {
                                // 标记进入重复段落，跳过其内容
                                currentSectionClean = "DUPLICATE:" + section;
                                duplicateCount++;
                                Logger.LogInfo($"检测到重复段落: {section}");
                                continue;
                            }
                            else
                            {
                                seenSections.Add(section);
                                currentSectionClean = section;
                                cleanedLines.Add(line);
                            }
                        }
                        // 处理配置项
                        else if (!string.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("#") && 
                                 currentSectionClean != "" && !currentSectionClean.StartsWith("DUPLICATE:"))
                        {
                            int equalsIndex = line.IndexOf('=');
                            if (equalsIndex > 0)
                            {
                                string configKey = currentSectionClean + ":" + line.Substring(0, equalsIndex).Trim();
                                // 检查是否重复配置项
                                if (seenConfigs.Contains(configKey))
                                {
                                    duplicateCount++;
                                    Logger.LogInfo($"检测到重复配置项: {configKey}");
                                    continue;
                                }
                                else
                                {
                                    seenConfigs.Add(configKey);
                                    cleanedLines.Add(line);
                                }
                            }
                            else
                            {
                                cleanedLines.Add(line);
                            }
                        }
                        // 保留注释、空行和非重复段落的内容
                        else if (currentSectionClean == "" || !currentSectionClean.StartsWith("DUPLICATE:"))
                        {
                            cleanedLines.Add(line);
                        }
                    }
                    
                    // 如果清理后有变化，使用清理后的行
                    if (duplicateCount > 0)
                    {
                        fileLines = cleanedLines;
                        hasChanges = true;
                        Logger.LogInfo($"成功清理了 {duplicateCount} 个重复项");
                    }
                    
                    // 更新内部版本参数为当前版本（无论是否有其他更改）
                    bool versionUpdated = false;
                    for (int i = 0; i < fileLines.Count; i++)
                    {
                        string line = fileLines[i];
                        if (line.Contains("[内部配置（Internal Settings）]"))
                        {
                            // 找到内部配置部分，继续查找版本参数行
                            for (int j = i + 1; j < fileLines.Count; j++)
                            {
                                string versionLine = fileLines[j];
                                // 使用更宽松的匹配方式，不依赖等号前后空格
                                if (versionLine.Trim().StartsWith("已加载版本（Loaded Version）"))
                                {
                                    // 更新版本号为当前版本，使用统一的格式
                                    fileLines[j] = "已加载版本（Loaded Version）= " + PluginMain.CURRENT_VERSION;
                                    versionUpdated = true;
                                    hasChanges = true;
                                    Logger.LogInfo("自动更新内部版本参数为: " + PluginMain.CURRENT_VERSION);
                                    break;
                                }
                                // 如果遇到新的段落标记，结束查找
                                else if (versionLine.StartsWith("["))
                                {
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    
                    // 如果有更改，写回配置文件
                    if (hasChanges)
                    {
                        // 确保文件可以写入，使用FileMode.Create确保完全覆盖文件
                        try
                        {
                            using (FileStream fs = new FileStream(newConfigFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                            using (StreamWriter writer = new StreamWriter(fs))
                            {
                                foreach (string line in fileLines)
                                {
                                    writer.WriteLine(line);
                                }
                            }
                            
                            // 验证文件是否成功写入
                            if (File.Exists(newConfigFilePath))
                            {
                                // 重新读取验证写入项数量
                                int finalConfigCount = 0;
                                using (StreamReader reader = new StreamReader(newConfigFilePath))
                                {
                                    string line;
                                    while ((line = reader.ReadLine()) != null)
                                    {
                                        if (!string.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("#") && !line.StartsWith("["))
                                        {
                                            int equalsIndex = line.IndexOf('=');
                                            if (equalsIndex > 0)
                                            {
                                                finalConfigCount++;
                                            }
                                        }
                                    }
                                }
                                
                                Logger.LogInfo($"恢复完成，共恢复 {restoredConfigCount} 个配置项。");
                                Logger.LogInfo($"最终配置文件中的有效配置项数量: {finalConfigCount}");
                                Logger.LogInfo("配置数据恢复完成。");
                                Logger.LogInfo("Configuration data restoration completed.");
                                
                                // 重新加载配置
                                base.Config.Reload();
                            }
                            else
                            {
                                Logger.LogError("错误：配置文件写入失败，文件不存在。");
                            }
                        }
                        catch (IOException ex)
                        {
                            Logger.LogError($"写入配置文件时发生IO错误: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("恢复配置数据时出错: " + ex.Message);
                }
                finally
                {
                    // 释放临时数据
                    PluginMain._tempOldConfigData.Clear();
                    PluginMain._tempOldConfigData = null;
                    Logger.LogInfo("临时配置数据已释放。");
                }
            }
            
            Harmony.CreateAndPatchAll(typeof(PluginMain), null);
        }

       
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FormulaData), "MenkeNumMax")]
        public static bool MenkeNumMax(FormulaData __instance, ref int __result)
        {
            __result = int.Parse(Mainload.FamilyData[2]) * 2 * PluginMain.城中可招门客上限倍数.Value;
            return false;
        }

        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FormulaData), "MinjuNumMaxPerCity")]
        public static bool MinjuNumMaxPerCity(FormulaData __instance, ref int __result)
        {
            __result = int.Parse(Mainload.FamilyData[2]) * PluginMain.城中可建民居上限倍数.Value;
            return false;
        }

        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FormulaData), "ShopNumMaxPerCity")]
        public static bool ShopNumMaxPerCity(FormulaData __instance, ref int __result)
        {
            __result = int.Parse(Mainload.FamilyData[2]) * PluginMain.城中可建商铺上限倍数.Value;
            return false;
        }

        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FormulaData), "GetTiliMax")]
        public static bool GetTiliMax(FormulaData __instance, ref int __result, ref int OldNum)
        {
            bool flag = OldNum <= 30;
            int num;
            if (flag)
            {
                num = OldNum;
            }
            else
            {
                bool flag2 = OldNum > 30 && OldNum <= 50;
                if (flag2)
                {
                    num = 60 - OldNum;
                }
                else
                {
                    num = 10 + Mathf.FloorToInt((float)(50 - OldNum) / 10f) * 2;
                }
            }
            bool flag3 = num <= 0;
            if (flag3)
            {
                num = 2;
            }
            __result = num * PluginMain.全角色体力上限倍数.Value;
            return false;
        }

        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FormulaData), "YunBaoToCoinsNum")]
        public static bool YunBaoToCoinsNum(FormulaData __instance, ref int __result, ref int ShopLv)
        {
            __result = ShopLv * 10 * PluginMain.城中商铺兑换元宝上限.Value;
            return false;
        }

        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FormulaData), "GetCanKejuNumMax")]
        public static bool GetCanKejuNumMax(FormulaData __instance, ref int __result)
        {
            __result = int.Parse(Mainload.FamilyData[2]) * PluginMain.科举人数上限.Value;
            return false;
        }

        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FormulaData), "GetChildMax")]
        public static bool GetChildMax(FormulaData __instance, ref int __result, ref string MemberID)
        {
            int num = 9;
            int num2;
            bool flag = int.TryParse(MemberID.Substring(MemberID.Length - 1, 1), out num2);
            if (flag)
            {
                num = num2 % 10;
            }
            bool flag2 = num == 0 || num == 1 || num == 2 || num == 3 || num == 4;
            int num3;
            if (flag2)
            {
                num3 = 2;
            }
            else
            {
                num3 = 1;
            }
            __result = num3 * PluginMain.最大子嗣上限.Value;
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mainload), "LoadData")]
        public static void UpdatePregnancyAge()
        {
            Mainload.OldShengYu = new List<int> { PluginMain.最小生育年龄.Value, PluginMain.最大生育年龄.Value };
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(H_MinJuPanelA), "OnEnableData")]
        public static void H_MinJuPanelA_OnEnableData(H_MinJuPanelA __instance)
        {
            // 获取 H_MinJuPanelA 实例中的 BuyMunjuCost 字段
            // 通过反射获取私有字段
            var fieldInfo = typeof(H_MinJuPanelA).GetField("BuyMunjuCost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldInfo != null)
            {
                // 获取原始值
                int originalCost = (int)fieldInfo.GetValue(__instance);
                
                // 应用折扣计算
                float discountMultiplier = PluginMain.民居售价折扣.Value / 10f;
                int discountedCost = Mathf.FloorToInt((float)originalCost * discountMultiplier);
                
                // 确保价格不低于1
                discountedCost = Math.Max(1, discountedCost);
                
                // 设置新值
                fieldInfo.SetValue(__instance, discountedCost);
            }
        }
        
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FormulaData), "FengdiGetCoins")]
        public static bool FengdiGetCoins_Prefix(int FengdiIndex, int RenNum, float NongNum, float ShangNum, ref int __result)
        {
            // 重现原始方法的计算逻辑，但应用倍数调整
            string[] splitResult = Mainload.Fengdi_now[FengdiIndex][2].Split(new char[] { '|' });
            int num = int.Parse(splitResult[0]);
            int num2 = int.Parse(splitResult[1]);
            int num3 = int.Parse(splitResult[2]);
            
            // 计算调整后的税收，分别应用相应的倍数
            int num4 = Mathf.CeilToInt(NongNum / 100f * 100000f * ((float)num / 100f) * PluginMain.封地农业税收倍数.Value);
            int num5 = Mathf.CeilToInt((float)RenNum * 50f * ((float)num2 / 100f) * PluginMain.封地人民税收倍数.Value);
            int num6 = Mathf.CeilToInt(ShangNum / 100f * 200000f * ((float)num3 / 100f) * PluginMain.封地商业税收倍数.Value);
            
            // 设置结果并跳过原始方法
            __result = num4 + num5 + num6;
            return false;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FormulaData), "FudiPrice")]
        public static void FudiPrice_Postfix(ref List<int> __result)
        {
            if (__result != null && __result.Count >= 2)
            {
                // 应用府邸销售折扣到item和item2
                float discountMultiplier = PluginMain.府邸销售折扣.Value / 10f;
                __result[0] = Mathf.FloorToInt((float)__result[0] * discountMultiplier);
                __result[1] = Mathf.FloorToInt((float)__result[1] * discountMultiplier);
                
                // 确保价格不低于1
                __result[0] = Math.Max(1, __result[0]);
                __result[1] = Math.Max(1, __result[1]);
            }
        }
        
        public static ConfigEntry<int> 城中可招门客上限倍数;
        public static ConfigEntry<int> 城中可建民居上限倍数;
        public static ConfigEntry<int> 城中可建商铺上限倍数;
        public static ConfigEntry<int> 全角色体力上限倍数;
        public static ConfigEntry<int> 城中商铺兑换元宝上限;
        public static ConfigEntry<int> 科举人数上限;
        public static ConfigEntry<int> 最大子嗣上限;
        public static ConfigEntry<int> 最小生育年龄;
        public static ConfigEntry<int> 最大生育年龄;
        public static ConfigEntry<float> 民居售价折扣;
        public static ConfigEntry<float> 封地农业税收倍数;
        public static ConfigEntry<float> 封地人民税收倍数;
        public static ConfigEntry<float> 封地商业税收倍数;
        public static ConfigEntry<float> 府邸销售折扣;
        
        // 临时存储旧配置数据的字段
        private static Dictionary<string, string> _tempOldConfigData = null;
        
    }
}
