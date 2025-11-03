using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs.HoLMod.TestTool
{
    // 事件列表及参数
    public class EventData
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
                "28|1|null|5@TextParameterA@1@TextParameterB|0"
            },
            new List<string>
            {
                "20",
                "29|1|null|5@TextParameterA@1@TextParameterB|0"
            }
        };

        // 读取并添加丹药名称列表
        public void AddDanYaoNameList()
        {
            foreach (var DanYaoId in DanYaoIdList)
            {
                DanYaoNameList.Add(AllText.Text_AllProp[DanYaoId][0]);  // 添加丹药的中文名称
            }
        }

        // 读取并添加符咒名称列表
        public void AddFuZhouNameList()
        {
            foreach (var FuZhouId in FuZhouIdList)
            {
                FuZhouNameList.Add(AllText.Text_AllProp[FuZhouId][0]);  // 添加符咒的中文名称
            }
        }

        // 预定义按钮列表
        public static List<int> DanYaoIdList = Mainload.PropIDForDanYao;  // 预定义的丹药ID列表
        public static List<string> DanYaoNameList = new List<string>();  // 预定义的丹药名称列表
        public static List<int> FuZhouIdList = Mainload.PropIDForFuZhou;  // 预定义的符咒ID列表
        public static List<string> FuZhouNameList = new List<string>();  // 预定义的符咒名称列表

        
        // 预定义的参数名称
        public static string TextParameterA = "文本参数A";
        public static string TextParameterB = "文本参数B";
        public static string TextParameterC = "文本参数C";
        public static string TextParameterD = "文本参数D";
        public static string TextParameterE = "文本参数E";
        public static string ButtonParameterA = "按钮参数A";
        public static string ButtonParameterB = "按钮参数B";
        public static string ButtonParameterC = "按钮参数C";
        public static string ButtonParameterD = "按钮参数D";
        public static string ButtonParameterE = "按钮参数E";


    }
}
