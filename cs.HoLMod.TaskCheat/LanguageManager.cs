using System.Collections.Generic;
using UnityEngine;

namespace cs.HoLMod.TaskCheat
{
    /// <summary>
    /// 语言管理器，用于处理多语言文本显示
    /// </summary>
    internal static class LanguageManager
    {
        private static bool isChinese = true;
        private static Dictionary<string, string> textDictionary = new Dictionary<string, string>();
        
        /// <summary>
        /// 初始化语言设置和文本字典
        /// </summary>
        static LanguageManager()
        {
            // 检测系统语言
            SystemLanguage systemLanguage = Application.systemLanguage;
            isChinese = systemLanguage == SystemLanguage.Chinese || 
                        systemLanguage == SystemLanguage.ChineseSimplified || 
                        systemLanguage == SystemLanguage.ChineseTraditional;
            
            // 初始化文本字典
            InitializeTextDictionary();
        }
        
        /// <summary>
        /// 初始化文本字典，添加所有需要翻译的文本
        /// </summary>
        private static void InitializeTextDictionary()
        {
            // 主窗口标题和分类
            AddText("任务编辑器", "Task Editor");
            AddText("一键清除", "All Tasks Clear");
            AddText("任务添加", "Add Tasks");
            AddText("重复任务", "Repetitive Tasks");
            
            // 按钮文本
            AddText("清除所有任务", "Clear All Tasks");
            AddText("选择需要添加的任务", "Select Tasks to Add");
            AddText("选择需要重复的任务", "Select Tasks to Repeat");
            AddText("清除配置文件", "Clear Config File");
            AddText("确认选择", "Confirm Selection");
            AddText("确认添加", "Confirm Addition");
            AddText("关闭", "Close");
            AddText("是", "Yes");
            AddText("否", "No");
            
            // 使用说明
            AddText("使用说明:", "Instructions:");
            AddText("1. 请在修改前先保存游戏，以便回档", "1. Save the game before making changes to allow rollback");
            AddText("2. 按F4键显示/隐藏窗口", "2. Press F4 to show/hide the window");
            AddText("3. 切换模式选择：一键清除/任务添加/重复任务", "3. Switch between modes: One-Click Clear/Add Tasks/Repetitive Tasks");
            AddText("Mod作者：AnZhi20", "Mod Author: AnZhi20");
            AddText("Mod版本：", "Mod Version: ");
            
            // 窗口标题
            AddText("选择要重复添加的任务", "Select Tasks to Repeat");
            AddText("选择要添加的任务", "Select Tasks to Add");
            AddText("确认操作", "Confirm Operation");
            
            // 确认对话框文本
            AddText("未找到本存档的配置，可能未生成或已删除。", "No configuration found for this save, it may not have been generated or has been deleted.");
            AddText("是否清除所有配置？", "Do you want to clear all configurations?");
            
            // 提示信息
            AddText("任务已清除成功!", "Tasks cleared successfully!");
            AddText("清除失败: ", "Clear failed: ");
            AddText("打开任务选择窗口失败!", "Failed to open task selection window!");
            AddText("打开任务添加窗口失败!", "Failed to open task addition window!");
            AddText("确认任务选择失败!", "Failed to confirm task selection!");
            AddText("确认任务添加失败!", "Failed to confirm task addition!");
            AddText("配置文件已清除成功！", "Configuration file cleared successfully!");
            AddText("配置文件清除失败！", "Failed to clear configuration file!");
            AddText("所有配置已清除成功！", "All configurations cleared successfully!");
            
            // 游戏相关文本
            AddText("清除所有配置失败！", "Failed to clear all configurations!");
            AddText("未选择任何任务！", "No tasks selected!");
            AddText("成功选择了 {0} 个任务！", "Successfully selected {0} tasks!");
            AddText("配置格式错误，已清除，请重新配置任务", "Configuration format error, cleared, please reconfigure tasks");
            AddText("未找到匹配的存储标识配置，ToBeAdded保持为空", "No matching storage identifier configuration found, ToBeAdded remains empty");
            AddText("未找到匹配的任务配置，请在任务编辑器中设置", "No matching task configuration found, please set it in the task editor");
            AddText("加载任务配置时出错，请查看日志了解详情", "Error loading task configuration, please check the log for details");
            AddText("已加载 {0} 个重复任务配置", "Loaded {0} repetitive task configurations");
            AddText("无法确定当前场景，任务添加失败！", "Unable to determine current scene, task addition failed!");
            AddText("成功添加了 {0} 个任务！", "Successfully added {0} tasks!");
            AddText("没有添加新任务，可能已全部存在！", "No new tasks added, they may all already exist!");
            AddText("任务添加失败！", "Task addition failed!");
            AddText("无相关配置", "No related configuration");
            
            // 任务系统相关文本
            AddText("TaskCheatInstance为空", "TaskCheat instance is null");
            AddText("MainloadSceneID为空", "Mainload.SceneID is null");
            AddText("CannotDetermineScene", "Unable to determine current scene, task addition failed!");
            AddText("任务添加错误", "Task addition error: ");
            AddText("所有任务已清除", "All tasks cleared!");
            AddText("MainloadTaskOrderData为空", "Mainload.TaskOrderData_Now is null");
            AddText("任务清除错误", "Task clear error: ");
            AddText("TaskList为空", "Task list is null");
            AddText("无法将字符串转换为整数", "Cannot convert string to integer: {0}");
            AddText("成功添加到待添加列表的任务数量", "Successfully added {0} tasks to the to-be-added list");
            AddText("任务转换失败", "{0} tasks conversion failed");
            AddText("添加选中任务错误", "Error adding selected tasks: ");
            AddText("成功添加到重复任务列表的任务数量", "Successfully added {0} tasks to the repetitive task list");
            AddText("ClearedToBeAddedListBeforeRepetitiveAdd", "Cleared to-be-added list before repetitive addition");
            AddText("开始重新从配置文件读取任务", "Starting to read tasks from configuration file again");
            AddText("ReloadedTasksToBeAddedList", "Reloaded {0} tasks to the to-be-added list");
            AddText("无任务配置", "No tasks found in configuration");
            AddText("ReloadTasksFromConfigError", "Error reloading tasks from configuration: ");
            AddText("SuccessfullyExecutedTaskAdd", "Successfully executed task addition");
            AddText("CannotAddTaskInstanceOrSceneInvalid", "Cannot add task: instance does not exist or scene is invalid");
            AddText("ToBeAddedList为空", "To-be-added task list is empty");
            AddText("TaskAddMethodException", "Task addition method exception: ");
            AddText("清除ToBeAddedList", "Cleared to-be-added list after repetitive addition");
            AddText("存档位置为空，无法加载特定存档的配置文件", "Save location is empty, cannot load configuration file for specific save");
            AddText("读取存档数据后，根据存档位置 {0} 开始查找匹配的配置文件", "After reading save data, start searching for matching configuration file based on save location {0}");
            AddText("当前存储标识: '{0}'", "Current storage identifier: '{0}'");
            AddText("找到与当前存档位置匹配的存储标识，尝试加载配置", "Found storage identifier matching current save location, attempting to load configuration");
            AddText("存储标识存在，但没有找到选中的任务配置", "Storage identifier exists, but no selected task configuration found");
            AddText("配置格式错误，删除存储标识 '{0}' 的配置: {1}", "Configuration format error, deleting configuration for storage identifier '{0}': {1}");
            AddText("配置已成功清除，下次启动将使用默认设置", "Configuration has been successfully cleared, default settings will be used next startup");
            AddText("成功加载匹配的配置文件，添加了 {0} 个任务", "Successfully loaded matching configuration file, added {0} tasks");
            AddText("显示提示信息时出错: {0}", "Error displaying notification: {0}");
            AddText("未知郡县", "Unknown county");
            // 任务奖励显示文本
            AddText("，任务奖励：声望+{0}、铜钱+{1}、元宝+{2}", ", Reward: Reputation+{0}, Coins+{1}, Gold+{2}");
        }
        
        /// <summary>
        /// 添加文本到字典
        /// </summary>
        /// <param name="chineseText">中文文本</param>
        /// <param name="englishText">英文文本</param>
        private static void AddText(string chineseText, string englishText)
        {
            textDictionary[chineseText] = englishText;
        }
        
        /// <summary>
        /// 获取当前语言的文本
        /// </summary>
        /// <param name="chineseText">中文文本</param>
        /// <returns>根据当前语言返回对应文本</returns>
        public static string GetText(string chineseText)
        {
            if (isChinese)
            {
                return chineseText;
            }
            else if (textDictionary.ContainsKey(chineseText))
            {
                return textDictionary[chineseText];
            }
            // 如果找不到对应翻译，返回原文
            return chineseText;
        }
        
        /// <summary>
        /// 获取格式化的文本
        /// </summary>
        /// <param name="chineseFormat">中文格式化字符串</param>
        /// <param name="args">格式化参数</param>
        /// <returns>根据当前语言返回格式化后的文本</returns>
        public static string GetFormattedText(string chineseFormat, params object[] args)
        {
            string format = GetText(chineseFormat);
            return string.Format(format, args);
        }
        
        /// <summary>
        /// 获取任务显示文本
        /// </summary>
        /// <param name="taskTexts">任务文本列表，第一个元素是中文，第二个是英文</param>
        /// <returns>根据当前语言返回任务文本</returns>
        public static string GetTaskText(List<string> taskTexts)
        {
            if (taskTexts == null || taskTexts.Count == 0)
            {
                return string.Empty;
            }
            
            int index = isChinese ? 0 : 1;
            if (index < taskTexts.Count)
            {
                return taskTexts[index];
            }
            
            return taskTexts[0];
        }
    }
}