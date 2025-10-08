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
            Logger.LogInfo("姓名管理器已加载！当前版本：" + CURRENT_VERSION);
            Logger.LogInfo("Name Manager has been loaded! Current version:" + CURRENT_VERSION);
            
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
            
            Harmony.CreateAndPatchAll(typeof(PluginMain), null);
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
                
                // 添加FamilyNameList中的姓氏
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
                
                // 添加MaleNameList中的男性名字首字
                foreach (string maleNameA in MaleNameList.Nan_MingA)
                {
                    if (!RandName.Nan_MingA.Contains(maleNameA))
                    {
                        RandName.Nan_MingA.Add(maleNameA);
                    }
                }
                
                // 添加MaleNameList中的男性名字末字
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
                
                // 添加FemaleNameList中的女性名字首字
                foreach (string femaleNameA in FemaleNameList.Nv_MingA)
                {
                    if (!RandName.Nv_MingA.Contains(femaleNameA))
                    {
                        RandName.Nv_MingA.Add(femaleNameA);
                    }
                }
                
                // 添加FemaleNameList中的女性名字末字
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
