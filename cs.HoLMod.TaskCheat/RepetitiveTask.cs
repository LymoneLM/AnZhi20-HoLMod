using System;
using System.Collections.Generic;
using System.Linq;

namespace cs.HoLMod.TaskCheat
{
    /// <summary>
    /// 重复任务处理类
    /// </summary>
    public static class RepetitiveTaskHandler
    {
        /// <summary>
        /// 待重复添加的任务列表
        /// </summary>
        public static List<List<int>> ToBeAdded = new List<List<int>>();

        /// <summary>
        /// 添加选中的任务到待添加列表
        /// </summary>
        /// <param name="selectedTasks">选中的任务列表</param>
        public static void AddSelectedTasks(List<List<string>> selectedTasks)
        {
            try
            {
                // 参数校验
                if (selectedTasks == null)
                {
                    TaskCheat.Log?.LogWarning("传入的任务列表为null");
                    return;
                }
                
                // 清空现有任务
                ToBeAdded.Clear();
                
                // 添加选中的任务，将string类型转换为int类型
                if (selectedTasks.Count > 0)
                {
                    int successfullyAdded = 0;
                    int failedConversions = 0;
                    
                    foreach (var task in selectedTasks)
                    {
                        if (task == null || task.Count == 0)
                        {
                            failedConversions++;
                            continue;
                        }
                        
                        List<int> intTask = new List<int>();
                        bool taskValid = false;
                        
                        foreach (var item in task)
                        {
                            if (int.TryParse(item, out int result))
                            {
                                intTask.Add(result);
                                taskValid = true;
                            }
                            else if (!string.IsNullOrEmpty(item))
                            {
                                // 记录转换失败的项
                                TaskCheat.Log?.LogWarning(string.Format("无法将字符串 \"{0}\" 转换为整数", item));
                            }
                        }
                        
                        if (taskValid)
                        {
                            ToBeAdded.Add(intTask);
                            successfullyAdded++;
                        }
                        else
                        {
                            failedConversions++;
                        }
                    }
                    
                    // 记录添加结果
                    if (successfullyAdded > 0)
                    {
                        TaskCheat.Log?.LogInfo(string.Format("成功添加 {0} 个任务到待添加列表", successfullyAdded));
                    }
                    
                    if (failedConversions > 0)
                    {
                        TaskCheat.Log?.LogWarning(string.Format("有 {0} 个任务转换失败或无效", failedConversions));
                    }
                }
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("添加选中任务时出错: " + ex.Message);
                TaskCheat.Log?.LogError(ex.StackTrace);
            }
        }
        
        // 保存上一次添加任务的现实时间
        private static DateTime lastAddTime = DateTime.MinValue;
        
        // 保存检查间隔（从配置文件加载）
        private static int checkIntervalSeconds = -1;
        
        /// <summary>
        /// 获取当前各国默认时间（如果获取失败则使用北京时间）
        /// </summary>
        /// <returns>当前时间</returns>
        private static DateTime GetCurrentDateTime()
        {
            try
            {
                // 尝试获取系统默认时区的当前时间
                return DateTime.Now;
            }
            catch
            {
                try
                {
                    // 如果失败，尝试获取北京时间 (UTC+8)
                    TimeZoneInfo chinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                    return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, chinaTimeZone);
                }
                catch
                {
                    // 如果仍然失败，使用UTC时间加上8小时作为北京时间
                    return DateTime.UtcNow.AddHours(8);
                }
            }
        }

        /// <summary>
        /// 重复添加任务方法 - 根据配置文件中的间隔执行
        /// </summary>
        public static void RepetitiveTaskAdd()
        {
            // 延迟初始化检查间隔（只在第一次调用时加载配置）
            if (checkIntervalSeconds == -1)
            {
                checkIntervalSeconds = TaskCheatConfig.LoadCheckInterval();
                lastAddTime = GetCurrentDateTime();
                return;
            }

            // 获取当前时间（使用各国默认时间或北京时间）
            DateTime currentTime = GetCurrentDateTime();

            // 距离上次添加已超过配置的间隔时间时执行添加
            if ((currentTime - lastAddTime).TotalSeconds >= checkIntervalSeconds)
            {
                TaskAdd();
                lastAddTime = currentTime;
            }
        }

        /// <summary>
        /// 添加待添加列表中的任务
        /// </summary>
        public static void TaskAdd()
        {
            try
            {
                // 检查是否有待添加的任务
                if (ToBeAdded != null && ToBeAdded.Count > 0)
                {
                    // 将List<List<int>>转换为List<List<string>>
                    List<List<string>> stringTasks = new List<List<string>>();
                    foreach (var intTask in ToBeAdded)
                    {
                        List<string> stringTask = new List<string>();
                        foreach (var item in intTask)
                        {
                            stringTask.Add(item.ToString());
                        }
                        stringTasks.Add(stringTask);
                    }

                    // 使用AddTasks.cs中的添加逻辑
                    if (TaskCheat.Instance != null)
                    {
                        AddTaskHandler.AddTasksToCurrent(stringTasks);
                    }
                    else
                    {
                        TaskCheat.Log?.LogWarning("TaskCheat.Instance为null，无法添加任务");
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录异常信息
                TaskCheat.Log?.LogError("TaskAdd方法执行异常: " + ex.Message);
                TaskCheat.Log?.LogError(ex.StackTrace);
            }
            finally
            {
                // 确保任务添加后清空ToBeAdded列表
                if (ToBeAdded != null)
                {
                    ToBeAdded.Clear();
                    TaskCheat.Log?.LogInfo("任务添加后已清空ToBeAdded列表");

                    /// <summary>
                    /// 检查是否处于游戏中
                    /// </summary>
                    
                    try
                    {
                        // 检查TaskCheat实例是否存在以及Mainload.SceneID是否为空
                        if (TaskCheat.Instance != null)
                        {
                            // 安全地访问Mainload.SceneID，Mainload是静态类
                            if (!string.IsNullOrEmpty(Mainload.SceneID))
                            {
                                // 加载当前场景并用|分割
                                string[] array = Mainload.SceneID.Split(new char[]
                                {
                                    '|'
                                });

                                // 读取场景类型，M府邸、Z封地、S郡、H皇宫
                                if (array.Length > 0)
                                {
                                    string SceneClass = array[0];
                                    
                                    // 在清空后重新根据配置文件读取并添加tobeadded
                                    if (SceneClass == "M" || SceneClass == "Z" || SceneClass == "S" || SceneClass == "H")
                                    {
                                        try
                                        {
                                            TaskCheat.Log?.LogInfo("开始重新从配置文件读取任务");
                                            List<int> selectedTaskIndices = TaskCheatConfig.LoadRepetitiveTaskSelection();

                                            if (selectedTaskIndices.Count > 0)
                                            {
                                                // 转换为RepetitiveTaskHandler需要的格式
                                                List<List<string>> tasksToAdd = new List<List<string>>();
                                                foreach (int index in selectedTaskIndices)
                                                {
                                                    tasksToAdd.Add(new List<string> { index.ToString(), "0" });
                                                }

                                                // 直接添加到ToBeAdded列表，不调用AddSelectedTasks避免循环调用
                                                foreach (var task in tasksToAdd)
                                                {
                                                    List<int> intTask = new List<int>();
                                                    foreach (var item in task)
                                                    {
                                                        if (int.TryParse(item, out int result))
                                                        {
                                                            intTask.Add(result);
                                                        }
                                                    }
                                                    if (intTask.Count > 0)
                                                    {
                                                        ToBeAdded.Add(intTask);
                                                    }
                                                }

                                                // 清空场景类型
                                                SceneClass = "";
                                                
                                                TaskCheat.Log?.LogInfo(string.Format("重新从配置文件加载了 {0} 个任务到ToBeAdded列表", ToBeAdded.Count));
                                            }
                                            else
                                            {
                                                TaskCheat.Log?.LogInfo("配置文件中没有找到任务，ToBeAdded保持为空");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            TaskCheat.Log?.LogError("重新从配置文件读取任务时出错: " + ex.Message);
                                            TaskCheat.Log?.LogError(ex.StackTrace);
                                        }
                                    }
                                }
                                else
                                {
                                    TaskCheat.Log?.LogWarning("Mainload.SceneID分割后数组为空");
                                }
                            }
                            else
                            {
                                TaskCheat.Log?.LogWarning("Mainload.SceneID为空，跳过场景检查和任务重新加载");
                            }
                        }
                        else
                        {
                            TaskCheat.Log?.LogWarning("TaskCheat.Instance为空，跳过场景检查和任务重新加载");
                        }
                    }
                    catch (Exception ex)
                    {
                        TaskCheat.Log?.LogError("处理场景信息时出错: " + ex.Message);
                        TaskCheat.Log?.LogError(ex.StackTrace);
                    }
                }
            }
        }
    }
}
