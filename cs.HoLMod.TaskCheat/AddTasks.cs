using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs.HoLMod.TaskCheat
{
    /// <summary>
    /// 任务添加处理类
    /// </summary>
    public static class AddTaskHandler
    {
        /// <summary>
        /// 直接添加选中的任务到当前任务列表
        /// </summary>
        /// <param name="selectedTasks">选中的任务列表</param>
        public static void AddTasksToCurrent(List<List<string>> selectedTasks)
        {
            try
            {
                // 读取当前任务列表
                List<List<int>> currentTasks = Mainload.TaskOrderData_Now;
                
                if (currentTasks == null)
                {
                    currentTasks = new List<List<int>>();
                }
                
                int addedCount = 0;
                
                // 处理选中的任务
                if (selectedTasks != null && selectedTasks.Count > 0)
                {
                    foreach (var task in selectedTasks)
                    {
                        // 转换为int类型
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
                            // 检查任务是否已存在
                            bool taskExists = false;
                            foreach (var existingTask in currentTasks)
                            {
                                if (existingTask != null && existingTask.Count > 0 && existingTask[0] == intTask[0])
                                {
                                    taskExists = true;
                                    break;
                                }
                            }
                            
                            // 如果任务不存在，则添加
                            if (!taskExists)
                            {
                                currentTasks.Add(intTask);
                                addedCount++;
                            }
                        }
                    }
                    
                    // 更新Mainload中的任务列表
                    Mainload.TaskOrderData_Now = currentTasks;
                    
                    // 显示成功提示
                    if (TaskCheat.Instance != null)
                    {
                        if (addedCount > 0)
                        {
                            TaskCheat.Instance.ShowNotification(string.Format("成功添加了 {0} 个任务！", addedCount));
                        }
                        else
                        {
                            TaskCheat.Instance.ShowNotification("没有添加新任务，可能已全部存在！");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录异常信息
                TaskCheat.Log?.LogError("添加任务时出错: " + ex.Message);
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
