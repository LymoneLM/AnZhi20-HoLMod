using System;
using System.Collections.Generic;

namespace cs.HoLMod.TaskCheat
{
    public static class TaskClearer
    {
        // 定义一个空的任务数据列表（与Mainload.TaskOrderData_Now类型一致）
        public static List<List<int>> New_TaskOrderData_Now = new List<List<int>>();

        // 全清任务实现逻辑
        public static void AllTaskClear()
        {
            try
            {
                // 检查Mainload.TaskOrderData_Now是否存在，然后将其设置为空列表
                if (Mainload.TaskOrderData_Now != null)
                {
                    Mainload.TaskOrderData_Now = New_TaskOrderData_Now;
                    TaskCheat.Log?.LogInfo("所有任务已成功清除！");
                }
                else
                {
                    TaskCheat.Log?.LogWarning("Mainload.TaskOrderData_Now为null，无法清除任务。");
                }
            }
            catch (System.Exception ex)
            {
                TaskCheat.Log?.LogError("清除任务时发生错误: " + ex.Message);
            }
        }
    }
}


