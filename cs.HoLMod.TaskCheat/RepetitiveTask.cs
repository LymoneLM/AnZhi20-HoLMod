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
                return; // 首次加载配置后不立即执行添加
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
        /// 添加选中的任务到待添加列表
        /// </summary>
        /// <param name="selectedTasks">选中的任务列表</param>
        public static void AddSelectedTasks(List<List<string>> selectedTasks)
        {
            try
            {
                // 清空现有任务
                ToBeAdded.Clear();
                
                // 添加选中的任务，将string类型转换为int类型
                if (selectedTasks != null && selectedTasks.Count > 0)
                {
                    foreach (var task in selectedTasks)
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
                }
            }
            catch (Exception ex)
            {
                TaskCheat.Log?.LogError("添加选中任务时出错: " + ex.Message);
                TaskCheat.Log?.LogError(ex.StackTrace);
            }
        }

        /// <summary>
        /// 检查两个任务是否相同
        /// </summary>
        /// <param name="task1">第一个任务</param>
        /// <param name="task2">第二个任务</param>
        /// <returns>如果任务相同则返回true，否则返回false</returns>
        private static bool AreTasksEqual(List<int> task1, List<int> task2)
        {
            if (task1 == null || task2 == null || task1.Count != task2.Count)
            {
                return false;
            }
            
            for (int i = 0; i < task1.Count; i++)
            {
                if (task1[i] != task2[i])
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// 添加重复任务到当前任务列表
        /// </summary>
        public static void TaskAdd()
        {
            try
            {
                // 读取当前任务列表
                List<List<int>> Add_TaskOrderData_Now = Mainload.TaskOrderData_Now;
                
                if (Add_TaskOrderData_Now == null)
                {
                    Add_TaskOrderData_Now = new List<List<int>>();
                }
                
                // 添加待添加任务，如果任务不存在则添加
                if (ToBeAdded.Count > 0)
                {
                    int addedCount = 0; // 记录成功添加的任务数量
                    
                    foreach (var taskToAdd in ToBeAdded)
                    {
                        bool taskExists = false;
                        
                        // 检查任务是否已经存在（只比较第一个元素）
                        foreach (var existingTask in Add_TaskOrderData_Now)
                        {
                            if (existingTask != null && existingTask.Count > 0 && taskToAdd != null && taskToAdd.Count > 0 && existingTask[0] == taskToAdd[0])
                            {
                                taskExists = true;
                                break;
                            }
                        }
                        
                        // 如果任务不存在，则添加
                        if (!taskExists)
                        {
                            Add_TaskOrderData_Now.Add(taskToAdd);
                            addedCount++;
                        }
                    }
                    
                    // 更新Mainload中的任务列表
                    Mainload.TaskOrderData_Now = Add_TaskOrderData_Now;
                    
                    // 显示成功提示（只有当有新任务被添加时才显示）
                    if (TaskCheat.Instance != null && addedCount > 0)
                    {
                        TaskCheat.Instance.ShowNotification($"成功添加了 {addedCount} 个任务！");
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录异常信息
                TaskCheat.Log?.LogError("TaskAdd方法执行异常: " + ex.Message);
                TaskCheat.Log?.LogError(ex.StackTrace);
                
                // 显示错误提示
                if (TaskCheat.Instance != null)
                {
                    TaskCheat.Instance.ShowNotification("任务添加失败！");
                }
            }
        }
    }
}
