using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;

namespace cs.HoLMod.MorePopulation
{
    // 插件信息类
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "cs.HoLMod.MorePopulation.AnZhi20";
        public const string PLUGIN_NAME = "HoLMod.MorePopulation";
        public const string PLUGIN_VERSION = "1.0.0";
    }
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Main : BaseUnityPlugin
    {
        // 配置游戏倍率（无论是否删除配置文件都会执行，确保配置项存在）
        private void Awake()
        {
            // 读取配置文件中的人口修正倍率
            PopulationCheatRate = Config.Bind("倍率调整", "自然增加人口修正", 1, "每月自然增加人口修正倍数，填1为不修改").Value;
        }

        // 人口相关变量
        public static int PopulationCheatRate;
        public static float PopulationCorrectionRate;
        
        // 记录上一次执行人口修正的年月，格式为：年*100+月
        private int lastCorrectionYearMonth = 0;

        private void Update()
        {
            // 检查游戏进度修改修正倍率并判断修改
            int year = Mainload.Time_now[0];
            int month = Mainload.Time_now[1];
            int day = Mainload.Time_now[2];

            switch (year)
            {
                case 1:
                    PopulationCorrectionRate = 0.1f;
                    break;
                case 2:
                    PopulationCorrectionRate = 0.2f;
                    break;
                case 3:
                    PopulationCorrectionRate = 0.3f;
                    break;
                case 4:
                    PopulationCorrectionRate = 0.4f;
                    break;
                case 5:
                    PopulationCorrectionRate = 0.5f;
                    break;
                case 6:
                    PopulationCorrectionRate = 0.6f;
                    break;
                case 7:
                    PopulationCorrectionRate = 0.7f;
                    break;
                case 8:
                    PopulationCorrectionRate = 0.8f;
                    break;
                case 9:
                    PopulationCorrectionRate = 0.9f;
                    break;
                case 10:
                    PopulationCorrectionRate = 1.0f;
                    break;
                case 11:
                    PopulationCorrectionRate = 1.2f;
                    break;
                case 12:
                    PopulationCorrectionRate = 1.4f;
                    break;
                case 13:
                    PopulationCorrectionRate = 1.6f;
                    break;
                case 14:
                    PopulationCorrectionRate = 1.8f;
                    break;
                case 15:
                    PopulationCorrectionRate = 2.0f;
                    break;
                case 16:
                    PopulationCorrectionRate = 2.2f;
                    break;
                case 17:
                    PopulationCorrectionRate = 2.4f;
                    break;
                case 18:
                    PopulationCorrectionRate = 2.6f;
                    break;
                case 19:
                    PopulationCorrectionRate = 2.8f;
                    break;
                case 20:
                    PopulationCorrectionRate = 3.0f;
                    break;
                default:
                    PopulationCorrectionRate = 4.0f;
                    break;
                    
            }
        
            // 修正人口的判断：每月1日执行，且同年同月只执行一次
            if (day == 1)
            {
                int currentYearMonth = year * 100 + month;
                
                // 检查是否是新的年月
                if (currentYearMonth != lastCorrectionYearMonth)
                {
                    // 将Main类的变量值同步到PluginCommonInfo类
                    PluginCommonInfo.PopulationCorrectionRate = (int)PopulationCorrectionRate;
                    PluginCommonInfo.PopulationCheatRate = PopulationCheatRate;
                    CorrectPopulation();

                    // 日志记录修正年月
                    Logger.LogInfo($"已修正{year}年{month}月的人口，修正倍率为{PopulationCheatRate}，修正比例为{PopulationCorrectionRate}");
                    
                    // 更新上一次执行的年月
                    lastCorrectionYearMonth = currentYearMonth;
                }
            }
        }

        // 修正人口的方法
        private void CorrectPopulation()
        {
            // 修正郡城人口
            for (int i = 0; i < 12; i++)
            {
                // 加载原始郡人口
                string JunPopulationOriginal = Mainload.CityData_now[i][0][8];
                // 计算新的郡人口
                int JunPopulation = (int)(int.Parse(JunPopulationOriginal) + PluginCommonInfo.PopulationCorrection * 3 * PopulationCorrectionRate * PopulationCheatRate);
                // 计算修正值
                int junCorrectionValue = JunPopulation - int.Parse(JunPopulationOriginal);
                // 赋值回数据结构
                Mainload.CityData_now[i][0][8] = JunPopulation.ToString();
                // 清除原始数据（设置为空字符串）
                JunPopulationOriginal = "";

                // 日志记录修正郡人口
                Logger.LogInfo($"已修正{PluginCommonInfo.JunList[i]}的人口，修正值为{junCorrectionValue}");
            }

            // 修正县城人口
            for (int i = 0; i < 12; i++)
            {
                // 获取当前郡的县数量
                int countyCount = PluginCommonInfo.XianList[i].Length;
                
                for (int j = 0; j < countyCount; j++)
                {
                    // 加载原始县人口
                    string XianPopulationOriginal = Mainload.CityData_now[i][j+1][8];
                    // 计算新的县人口
                    int XianPopulation = (int)(int.Parse(XianPopulationOriginal) + PluginCommonInfo.PopulationCorrection * PopulationCorrectionRate * PopulationCheatRate);
                    // 计算修正值
                    int xianCorrectionValue = XianPopulation - int.Parse(XianPopulationOriginal);
                    // 赋值回数据结构
                    Mainload.CityData_now[i][j+1][8] = XianPopulation.ToString();
                    // 清除原始数据
                    XianPopulationOriginal = "";
                    
                    // 日志记录修正县人口
                    Logger.LogInfo($"已修正{PluginCommonInfo.XianList[i][j]}的人口，修正值为{xianCorrectionValue}");
                }
            }
        }
    }

    // 插件公共信息类
    public static class PluginCommonInfo
    {
        // 人口修正值
        public const int PopulationCorrection = 100;
        // 倍率变量（保持兼容性）
        public static int PopulationCorrectionRate;
        public static int PopulationCheatRate;

        // 郡数组，索引对应郡索引（郡ID）
        public static string[] JunList = new string[]
        {
            "南郡",     // 0
            "三川郡",   // 1
            "蜀郡",     // 2
            "丹阳郡",   // 3
            "陈留郡",   // 4
            "长沙郡",   // 5
            "会稽郡",   // 6
            "广陵郡",   // 7
            "太原郡",   // 8
            "益州郡",   // 9
            "南海郡",   // 10
            "云南郡"    // 11
        };

        // 二维县数组，第一维是郡索引（郡ID），第二维是县索引（县ID）
        public static string[][] XianList = new string[][]
        {
            // 南郡 (索引0)
            new string[] { "临沮", "襄樊", "宜城", "麦城", "华容", "郢亭", "江陵", "夷陵" },
            
            // 三川郡 (索引1)
            new string[] { "平阳", "荥阳", "原武", "阳武", "新郑", "宜阳" },
            
            // 蜀郡 (索引2)
            new string[] { "邛崃", "郫县", "什邡", "绵竹", "新都", "成都" },
            
            // 丹阳郡 (索引3)
            new string[] { "秣陵", "江乘", "江宁", "溧阳", "建邺", "永世" },
            
            // 陈留郡 (索引4)
            new string[] { "长垣", "济阳", "成武", "襄邑", "宁陵", "封丘" },
            
            // 长沙郡 (索引5)
            new string[] { "零陵", "益阳", "湘县", "袁州", "庐陵", "衡山", "建宁", "桂阳" },
            
            // 会稽郡 (索引6)
            new string[] { "曲阿", "松江", "山阴", "余暨" },
            
            // 广陵郡 (索引7)
            new string[] { "平安", "射阳", "海陵", "江都" },
            
            // 太原郡 (索引8)
            new string[] { "大陵", "晋阳", "九原", "石城", "阳曲", "魏榆", "孟县", "中都" },
            
            // 益州郡 (索引9)
            new string[] { "连然", "谷昌", "同劳", "昆泽", "滇池", "俞元", "胜休", "南安" },
            
            // 南海郡 (索引10)
            new string[] { "四会", "阳山", "龙川", "揭岭", "罗阳", "善禺" },
            
            // 云南郡 (索引11)
            new string[] { "云平", "叶榆", "永宁", "遂久", "姑复", "蜻陵", "弄栋", "邪龙" }
        };
    }
}
