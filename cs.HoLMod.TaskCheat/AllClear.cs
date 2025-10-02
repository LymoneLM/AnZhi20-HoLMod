using System;
using System.Collections.Generic;

namespace cs.HoLMod.TaskCheat
{
    public static class TaskClearer
    {
        // 定义一个空的任务数据列表（与Mainload.TaskOrderData_Now类型一致）
        // 作为静态字段，确保跨方法调用的一致性
        private static List<List<int>> EmptyTaskList = new List<List<int>>();
        
        // 全清任务实现逻辑
        public static void AllTaskClear()
        {
            try
            {
                // 检查Mainload.TaskOrderData_Now是否存在，然后将其设置为空列表
                if (Mainload.TaskOrderData_Now != null)
                {
                    // 使用新创建的空列表，而不是重用可能有问题的静态列表
                    Mainload.TaskOrderData_Now = new List<List<int>>();
                    TaskCheat.Log?.LogInfo("所有任务已成功清除！");
                }
                else
                {
                    // 如果Mainload.TaskOrderData_Now为null，创建一个新的空列表并赋值
                    Mainload.TaskOrderData_Now = new List<List<int>>();
                    TaskCheat.Log?.LogInfo("Mainload.TaskOrderData_Now为null，已创建新的空任务列表。");
                }
            }
            catch (System.Exception ex)
            {
                TaskCheat.Log?.LogError("清除任务时发生错误: " + ex.Message);
            }
        }
    }
}


