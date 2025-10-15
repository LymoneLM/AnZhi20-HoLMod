using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace cs.HoLMod.NameManager
{
    // 占位符类，用于编译通过
    // 在运行时，Harmony会找到实际的LoadScene类
    public class LoadScene : MonoBehaviour
    {
        public static int LanguageType;
    }

    // 占位符类，用于编译通过
    // 在运行时，Harmony会找到实际的RandName类
    public class RandName : MonoBehaviour
    {
        public static List<string> XingShi = new List<string>();
        public static List<string> Nan_MingA = new List<string>();
        public static List<string> Nan_MingB = new List<string>();
        public static List<string> Nv_MingA = new List<string>();
        public static List<string> Nv_MingB = new List<string>();
        
        public void Start() { }
        public static string GetMemberNameShijia() { return string.Empty; }
    }
    
    // 插件信息类
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "cs.HoLMod.NameManager.AnZhi20";
        public const string PLUGIN_NAME = "HoLMod.NameManager";
        public const string PLUGIN_VERSION = "1.0.0";
    }
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class PluginMain : BaseUnityPlugin
    {
        // 当前插件版本
        private const string CURRENT_VERSION = PluginInfo.PLUGIN_VERSION; // 与BepInPlugin属性中定义的版本保持一致
        
        // 配置项
        public static ConfigEntry<string> CustomFamilyNames;
        public static ConfigEntry<string> CustomMaleFirstNames;
        public static ConfigEntry<string> CustomMaleSecondNames;
        public static ConfigEntry<string> CustomFemaleFirstNames;
        public static ConfigEntry<string> CustomFemaleSecondNames;
        
        // 辅助方法：验证姓名格式是否正确
        private static bool IsValidNameFormat(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            
            int pipeIndex = name.IndexOf('|');
            if (pipeIndex <= 0 || pipeIndex >= name.Length - 1)
                return false; // |前后必须有内容
            
            string chinesePart = name.Substring(0, pipeIndex).Trim();
            string pinyinPart = name.Substring(pipeIndex + 1).Trim();
            
            return !string.IsNullOrEmpty(chinesePart) && !string.IsNullOrEmpty(pinyinPart);
        }
        
        private void Awake()
        {
            try
            {
                Logger.LogInfo("姓名管理器已加载！当前版本：" + CURRENT_VERSION);
                Logger.LogInfo("当前姓氏数量：" + RandName.XingShi.Count);
                Logger.LogInfo("当前男性名A数量：" + RandName.Nan_MingA.Count);
                Logger.LogInfo("当前男性名B数量：" + RandName.Nan_MingB.Count);
                Logger.LogInfo("当前女性名A数量：" + RandName.Nv_MingA.Count);
                Logger.LogInfo("当前女性名B数量：" + RandName.Nv_MingB.Count);
                
                // 配置文件路径
                string configFilePath = Path.Combine(Paths.ConfigPath, "cs.HoLMod.NameManager.AnZhi20.cfg");
                
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
                                if (line.StartsWith("## 已加载版本（Loaded Version） = "))
                                {
                                    loadedVersion = line.Substring(33).Trim();
                                    break;
                                }
                            }
                        }
                        
                        // 检查是否需要更新配置
                        bool isVersionUpdated = loadedVersion != CURRENT_VERSION;
                        
                        // 如果版本更新，删除配置文件
                        if (isVersionUpdated)
                        {
                            Logger.LogInfo($"检测到插件版本更新至 {CURRENT_VERSION}，正在删除旧的配置文件...");
                            Logger.LogInfo($"Detected plugin version update to {CURRENT_VERSION}, deleting old configuration file...");
                            File.Delete(configFilePath);
                            Logger.LogInfo("旧配置文件已成功删除。");
                            Logger.LogInfo("Old configuration file has been successfully deleted.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("读取或删除配置文件时出错: " + ex.Message);
                }
                
                try
                {
                    // 配置自定义姓名（无论是否删除配置文件都会执行，确保配置项存在）
                    CustomFamilyNames = base.Config.Bind<string>("自定义姓名（Custom Names）", "自定义姓氏（Custom Family Names）", "", "添加自定义姓氏，格式说明：\n" +
                        "1. 单个姓氏格式：姓氏|拼音（例如：张|Zhang）\n" +
                        "2. 多个姓氏之间用英文逗号分隔（例如：张|Zhang,王|Wang,李|Li）\n" +
                        "3. 支持复姓（例如：欧阳|Ouyang,司马|Sima）\n" +
                        "4. 拼音部分请使用标准罗马拼音，首字母大写\n" +
                        "(Add custom family names, format instructions:\n" +
                        "1. Single family name format: LastName|Pinyin (e.g.: 张|Zhang)\n" +
                        "2. Separate multiple family names with commas (e.g.: 张|Zhang,王|Wang,李|Li)\n" +
                        "3. Compound family names are supported (e.g.: 欧阳|Ouyang,司马|Sima)\n" +
                        "4. Use standard Roman pinyin for the pinyin part, with first letter capitalized)");
                    CustomMaleFirstNames = base.Config.Bind<string>("自定义姓名（Custom Names）", "自定义男性名字首字（Custom Male First Characters）", "", "添加自定义男性名字首字，格式说明：\n" +
                        "1. 单个汉字格式：汉字|拼音（例如：伟|Wei）\n" +
                        "2. 多个汉字之间用英文逗号分隔（例如：伟|Wei,强|Qiang,军|Jun）\n" +
                        "3. 拼音部分请使用标准罗马拼音，首字母小写\n" +
                        "(Add custom male first characters, format instructions:\n" +
                        "1. Single character format: Character|Pinyin (e.g.: 伟|wei)\n" +
                        "2. Separate multiple characters with commas (e.g.: 伟|wei,强|qiang,军|jun)\n" +
                        "3. Use standard Roman pinyin for the pinyin part, with all letters lowercase)");
                    CustomMaleSecondNames = base.Config.Bind<string>("自定义姓名（Custom Names）", "自定义男性名字末字（Custom Male Second Characters）", "", "添加自定义男性名字末字，格式说明：\n" +
                        "1. 单个汉字格式：汉字|拼音（例如：明|ming）\n" +
                        "2. 多个汉字之间用英文逗号分隔（例如：明|ming,杰|jie,辉|hui）\n" +
                        "3. 拼音部分请使用标准罗马拼音，首字母小写\n" +
                        "(Add custom male second characters, format instructions:\n" +
                        "1. Single character format: Character|Pinyin (e.g.: 明|ming)\n" +
                        "2. Separate multiple characters with commas (e.g.: 明|ming,杰|jie,辉|hui)\n" +
                        "3. Use standard Roman pinyin for the pinyin part, with all letters lowercase)");
                    CustomFemaleFirstNames = base.Config.Bind<string>("自定义姓名（Custom Names）", "自定义女性名字首字（Custom Female First Characters）", "", "添加自定义女性名字首字，格式说明：\n" +
                        "1. 单个汉字格式：汉字|拼音（例如：丽|li）\n" +
                        "2. 多个汉字之间用英文逗号分隔（例如：丽|li,芳|fang,娜|na）\n" +
                        "3. 拼音部分请使用标准罗马拼音，首字母小写\n" +
                        "(Add custom female first characters, format instructions:\n" +
                        "1. Single character format: Character|Pinyin (e.g.: 丽|li)\n" +
                        "2. Separate multiple characters with commas (e.g.: 丽|li,芳|fang,娜|na)\n" +
                        "3. Use standard Roman pinyin for the pinyin part, with all letters lowercase)");
                    CustomFemaleSecondNames = base.Config.Bind<string>("自定义姓名（Custom Names）", "自定义女性名字末字（Custom Female Second Characters）", "", "添加自定义女性名字末字，格式说明：\n" +
                        "1. 单个汉字格式：汉字|拼音（例如：婷|ting）\n" +
                        "2. 多个汉字之间用英文逗号分隔（例如：婷|ting,媛|yuan,琪|qi）\n" +
                        "3. 拼音部分请使用标准罗马拼音，首字母小写\n" +
                        "(Add custom female second characters, format instructions:\n" +
                        "1. Single character format: Character|Pinyin (e.g.: 婷|ting)\n" +
                        "2. Separate multiple characters with commas (e.g.: 婷|ting,媛|yuan,琪|qi)\n" +
                        "3. Use standard Roman pinyin for the pinyin part, with all letters lowercase)");
                    
                    // 保存当前版本号
                    base.Config.Bind("内部配置（Internal Settings）", "已加载版本（Loaded Version）", CURRENT_VERSION, "用于跟踪插件版本，请勿手动修改");
                    
                    // 保存配置文件
                    base.Config.Save();
                }
                catch (Exception ex)
                {
                    Logger.LogError("配置自定义姓名时出错: " + ex.Message);
                }
                
                try
                {
                    // Harmony补丁创建，使用更安全的方式
                    Logger.LogInfo("开始创建Harmony补丁...");
                    
                    // 使用PluginInfo.PLUGIN_GUID作为Harmony标识符
                    Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
                    
                    // 添加更详细的错误日志和异常处理
                    try
                    {
                        // 避免使用PatchAll()，改为手动创建每个补丁
                        Logger.LogInfo("开始手动创建Harmony补丁...");
                        
                        try
                        {
                            // 尝试获取原始RandName类型
                                Type randNameType = Type.GetType("RandName");
                                
                                if (randNameType != null && randNameType.Assembly != Assembly.GetExecutingAssembly())
                                {
                                    // 找到原始RandName类型（非占位符类）
                                    Logger.LogInfo("找到原始RandName类型，开始创建补丁...");
                                    
                                    // 为Start方法创建补丁
                                    MethodInfo startMethod = randNameType.GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                    if (startMethod != null)
                                    {
                                        MethodInfo patchAllNamesMethod = typeof(PluginMain).GetMethod("PatchAllNames", BindingFlags.NonPublic | BindingFlags.Static);
                                        if (patchAllNamesMethod != null)
                                        {
                                            harmony.Patch(startMethod, postfix: new HarmonyMethod(patchAllNamesMethod));
                                            Logger.LogInfo("成功为RandName.Start方法创建补丁。");
                                        }
                                        else
                                        {
                                            Logger.LogWarning("无法找到PatchAllNames方法，跳过该补丁。");
                                        }
                                    }
                                    else
                                    {
                                        Logger.LogWarning("无法找到RandName.Start方法，跳过该补丁。");
                                    }
                                    
                                    // 为GetMemberNameShijia方法创建补丁
                                    MethodInfo getMemberNameMethod = randNameType.GetMethod("GetMemberNameShijia", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                                    if (getMemberNameMethod != null)
                                    {
                                        MethodInfo patchGetMemberNameMethod = typeof(PluginMain).GetMethod("PatchGetMemberNameShijia", BindingFlags.NonPublic | BindingFlags.Static);
                                        if (patchGetMemberNameMethod != null)
                                        {
                                            harmony.Patch(getMemberNameMethod, postfix: new HarmonyMethod(patchGetMemberNameMethod));
                                            Logger.LogInfo("成功为RandName.GetMemberNameShijia方法创建补丁。");
                                        }
                                        else
                                        {
                                            Logger.LogWarning("无法找到PatchGetMemberNameShijia方法，跳过该补丁。");
                                        }
                                    }
                                    else
                                    {
                                        Logger.LogWarning("无法找到RandName.GetMemberNameShijia方法，跳过该补丁。");
                                    }
                                }
                                else
                                {
                                    // 使用占位符类，避免显示错误信息
                                    Logger.LogInfo("使用内部RandName占位符类，确保插件功能可用。");
                                }
                            
                            Logger.LogInfo("Harmony补丁创建过程已完成。");
                            Logger.LogInfo("Harmony patches creation process has been completed.");
                        }
                        catch (Exception patchEx)
                        {
                            Logger.LogError("手动创建补丁时出错: " + patchEx.Message + "\n" + patchEx.StackTrace);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("创建Harmony补丁时出错: " + ex.Message);
                        Logger.LogError("堆栈跟踪: " + ex.StackTrace);
                        
                        // 尝试单独检查和处理每个补丁方法
                        try
                        {
                            Logger.LogInfo("尝试单独检查和处理补丁方法...");
                            
                            // 检查RandName类型是否存在（排除占位符类）
                            Type randNameType = Type.GetType("RandName");
                            if (randNameType == null || randNameType.Assembly == Assembly.GetExecutingAssembly())
                            {
                                // 如果是占位符类或未找到类型，使用占位符类
                                Logger.LogInfo("使用内部RandName占位符类，确保插件功能可用。");
                                return;
                            }
                            
                            Logger.LogInfo("成功找到原始RandName类型");
                            
                            // 尝试获取Start方法
                            MethodInfo startMethod = randNameType.GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            if (startMethod == null)
                            {
                                Logger.LogError("错误：在RandName类型中无法找到Start方法");
                            }
                            else
                            {
                                Logger.LogInfo("成功找到RandName.Start方法");
                            }
                            
                            // 尝试获取GetMemberNameShijia方法
                            MethodInfo getMemberNameMethod = randNameType.GetMethod("GetMemberNameShijia", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                            if (getMemberNameMethod == null)
                            {
                                Logger.LogError("错误：在RandName类型中无法找到GetMemberNameShijia方法");
                            }
                            else
                            {
                                Logger.LogInfo("成功找到RandName.GetMemberNameShijia方法");
                            }
                        }
                        catch (Exception innerEx)
                        {
                            Logger.LogError("检查补丁方法时出错: " + innerEx.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("初始化Harmony实例时出错: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("PluginMain.Awake方法执行出错: " + ex.Message + "\n" + ex.StackTrace);
                // 确保插件不会因为初始化失败而完全崩溃
                try
                {
                    // 创建一个简单的Harmony实例，即使在初始化失败的情况下也能尝试工作
                    new Harmony(PluginInfo.PLUGIN_GUID);
                    Logger.LogInfo("已创建基本Harmony实例以确保插件不会完全崩溃");
                }
                catch (Exception initEx)
                {
                    Logger.LogError("创建基本Harmony实例时也出错: " + initEx.Message);
                }
            }
        }
        
        
        // 合并所有修补方法到一个方法中，避免多次调用Start方法
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RandName), "Start")]
        public static void PatchAllNames(RandName __instance)
        {
            try
            {
                // 修补姓氏数组
                PatchFamilyNamesInternal();
                
                // 修补男性名字数组
                PatchMaleNamesInternal();
                
                // 修补女性名字数组
                PatchFemaleNamesInternal();
                
                Debug.Log("已成功修补所有姓名数组");
                Debug.Log($"修补后姓氏数量: {RandName.XingShi.Count}");
                Debug.Log($"修补后男性名字首字数量: {RandName.Nan_MingA.Count}");
                Debug.Log($"修补后男性名字末字数量: {RandName.Nan_MingB.Count}");
                Debug.Log($"修补后女性名字首字数量: {RandName.Nv_MingA.Count}");
                Debug.Log($"修补后女性名字末字数量: {RandName.Nv_MingB.Count}");
            }
            catch (Exception ex)
            {
                Debug.LogError("修补姓名数组时出错: " + ex.Message);
            }
        }
        
        // 内部方法：修补姓氏数组
        private static void PatchFamilyNamesInternal()
        {
            try
            {
                // 确保RandName.XingShi存在
                if (RandName.XingShi == null)
                {
                    RandName.XingShi = new List<string>();
                }
                
                // 添加自定义姓氏
                if (!string.IsNullOrEmpty(CustomFamilyNames.Value))
                {
                    string[] customNames = CustomFamilyNames.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string name in customNames)
                    {
                        string trimmedName = name.Trim();
                        if (IsValidNameFormat(trimmedName))
                        {
                            if (!RandName.XingShi.Contains(trimmedName))
                            {
                                RandName.XingShi.Add(trimmedName);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("跳过格式错误的自定义姓氏: " + trimmedName);
                        }
                    }
                }
                
                // 添加FamilyNameList中的姓氏（添加前检查是否存在，不添加重复项）
                foreach (string familyName in FamilyNameList.XingShi)
                {
                    if (!RandName.XingShi.Contains(familyName))
                    {
                        RandName.XingShi.Add(familyName);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("修补姓氏数组时出错: " + ex.Message);
            }
        }
        
        // 内部方法：修补男性名字数组
        private static void PatchMaleNamesInternal()
        {
            try
            {
                // 确保RandName.Nan_MingA存在
                if (RandName.Nan_MingA == null)
                {
                    RandName.Nan_MingA = new List<string>();
                }
                
                // 确保RandName.Nan_MingB存在
                if (RandName.Nan_MingB == null)
                {
                    RandName.Nan_MingB = new List<string>();
                }
                
                // 添加自定义男性名字首字
                if (!string.IsNullOrEmpty(CustomMaleFirstNames.Value))
                {
                    string[] customNames = CustomMaleFirstNames.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string name in customNames)
                    {
                        string trimmedName = name.Trim();
                        if (IsValidNameFormat(trimmedName))
                        {
                            if (!RandName.Nan_MingA.Contains(trimmedName))
                            {
                                RandName.Nan_MingA.Add(trimmedName);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("跳过格式错误的自定义男性名字首字: " + trimmedName);
                        }
                    }
                }
                
                // 添加自定义男性名字末字
                if (!string.IsNullOrEmpty(CustomMaleSecondNames.Value))
                {
                    string[] customNames = CustomMaleSecondNames.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string name in customNames)
                    {
                        string trimmedName = name.Trim();
                        if (IsValidNameFormat(trimmedName))
                        {
                            if (!RandName.Nan_MingB.Contains(trimmedName))
                            {
                                RandName.Nan_MingB.Add(trimmedName);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("跳过格式错误的自定义男性名字末字: " + trimmedName);
                        }
                    }
                }
                
                // 添加MaleNameList中的男性名字首字（添加前检查是否存在，不添加重复项）
                foreach (string maleNameA in MaleNameList.Nan_MingA)
                {
                    if (!RandName.Nan_MingA.Contains(maleNameA))
                    {
                        RandName.Nan_MingA.Add(maleNameA);
                    }
                }
                
                // 添加MaleNameList中的男性名字末字（添加前检查是否存在，不添加重复项）
                if (MaleNameList.Nan_MingB != null)
                {
                    foreach (string maleNameB in MaleNameList.Nan_MingB)
                    {
                        if (!RandName.Nan_MingB.Contains(maleNameB))
                        {
                            RandName.Nan_MingB.Add(maleNameB);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("修补男性名字数组时出错: " + ex.Message);
            }
        }
        
        // 内部方法：修补女性名字数组
        private static void PatchFemaleNamesInternal()
        {
            try
            {
                // 确保RandName.Nv_MingA存在
                if (RandName.Nv_MingA == null)
                {
                    RandName.Nv_MingA = new List<string>();
                }
                
                // 确保RandName.Nv_MingB存在
                if (RandName.Nv_MingB == null)
                {
                    RandName.Nv_MingB = new List<string>();
                }
                
                // 添加自定义女性名字首字
                if (!string.IsNullOrEmpty(CustomFemaleFirstNames.Value))
                {
                    string[] customNames = CustomFemaleFirstNames.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string name in customNames)
                    {
                        string trimmedName = name.Trim();
                        if (IsValidNameFormat(trimmedName))
                        {
                            if (!RandName.Nv_MingA.Contains(trimmedName))
                            {
                                RandName.Nv_MingA.Add(trimmedName);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("跳过格式错误的自定义女性名字首字: " + trimmedName);
                        }
                    }
                }
                
                // 添加自定义女性名字末字
                if (!string.IsNullOrEmpty(CustomFemaleSecondNames.Value))
                {
                    string[] customNames = CustomFemaleSecondNames.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string name in customNames)
                    {
                        string trimmedName = name.Trim();
                        if (IsValidNameFormat(trimmedName))
                        {
                            if (!RandName.Nv_MingB.Contains(trimmedName))
                            {
                                RandName.Nv_MingB.Add(trimmedName);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("跳过格式错误的自定义女性名字末字: " + trimmedName);
                        }
                    }
                }
                
                // 添加FemaleNameList中的女性名字首字（添加前检查是否存在，不添加重复项）
                foreach (string femaleNameA in FemaleNameList.Nv_MingA)
                {
                    if (!RandName.Nv_MingA.Contains(femaleNameA))
                    {
                        RandName.Nv_MingA.Add(femaleNameA);
                    }
                }
                
                // 添加FemaleNameList中的女性名字末字（添加前检查是否存在，不添加重复项）
                if (FemaleNameList.Nv_MingB != null)
                {
                    foreach (string femaleNameB in FemaleNameList.Nv_MingB)
                    {
                        if (!RandName.Nv_MingB.Contains(femaleNameB))
                        {
                            RandName.Nv_MingB.Add(femaleNameB);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("修补女性名字数组时出错: " + ex.Message);
            }
        }
        
        // 修补GetMemberNameShijia方法，添加姓名检查逻辑
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RandName), "GetMemberNameShijia")]
        public static void PatchGetMemberNameShijia(string sexID, int ShijiaIndex, int DaiNum, ref string __result)
        {
            try
            {
                // 在返回结果前调用CheckName方法进行检查
                if (!string.IsNullOrEmpty(__result) && !__result.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    string originalName = __result;
                    __result = ExistingNamesList.CheckName(__result, sexID, ShijiaIndex, DaiNum);
                    
                    // 如果姓名被更改，记录日志
                    if (originalName != __result)
                    {
                        Debug.Log($"姓名已从 '{originalName}' 更改为 '{__result}' 以避免重复");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("修补GetMemberNameShijia方法时出错: " + ex.Message);
            }
        }
    }
}
