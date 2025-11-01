using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs.HoLMod.AddEvents
{
    // 事件列表及参数
    public static class EventData
    {
        // 事件列表
        public static List<string> EventList = new List<string>
        {
            "云游道人卖丹药",
            "云游道人卖符咒",
        };

        // 事件参数
        public static List<List<string>> EventParams = new List<List<string>>
        {
            new List<string>
            {
                "20",
                $"28|1|null|5@{A}@1@{B}|0"
            }
            new List<string>
            {
                "20",
                $"29|1|null|5@{A}@1@{B}|0"
            }
        };
    }
}
