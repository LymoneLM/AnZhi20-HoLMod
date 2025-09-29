using UnityEngine;
using System.Collections.Generic;

namespace cs.HoLMod.ShiJia_NowData
{
    /// <summary>
    /// 世家数据结构及工具类
    /// </summary>
    public class ShiJiaData
    {
        /// <summary>
        /// 郡数组，索引对应郡ID
        /// </summary>
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

        /// <summary>
        /// 二维县数组，第一维是郡索引，第二维是县索引
        /// </summary>
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

        /// <summary>
        /// 世家状态常量定义
        /// </summary>
        public class ShiJiaStatus
        {
            public const string NOT_DISPLAYED = "0"; // 不显示
            public const string NORMAL = "1";        // 正常显示
            // 可以在这里添加其他状态常量，当明确其他状态含义时
        }

        /// <summary>
        /// 世家数据数组索引定义
        /// </summary>
        public class ShiJiaIndexes
        {
            public const int STATUS = 0;        // 世家状态
            public const int FAMILY_NAME = 1;   // 世家姓氏
            public const int LEVEL = 2;         // 世家等级
            public const int RELATIONSHIP = 3;  // 与玩家家族的关系
            public const int UNKNOWN_4 = 4;     // 未知含义
            public const int JUN_XIAN = 5;      // 世家所在郡县 (格式: 郡索引|县索引)，世家位置
            public const int UNKNOWN_6 = 6;     // 未知含义
            public const int UNKNOWN_7 = 7;     // 未知含义
            public const int UNKNOWN_8 = 8;     // 未知含义
            public const int UNKNOWN_9 = 9;     // 未知含义
            public const int UNKNOWN_10 = 10;   // 未知含义
            public const int UNKNOWN_11 = 11;   // 未知含义
            public const int UNKNOWN_12 = 12;   // 未知含义
        }

        /// <summary>
        /// 检查Mainload.ShiJia_Now是否存在并初始化（如果不存在）
        /// </summary>
        public static void EnsureShiJiaNowExists()
        {
            try
            {
                // 尝试访问Mainload.ShiJia_Now，如果不存在则创建
                if (Mainload.ShiJia_Now == null)
                {
                    Mainload.ShiJia_Now = new List<List<string>>();
                    Debug.Log("已初始化Mainload.ShiJia_Now");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("初始化Mainload.ShiJia_Now时发生错误: " + ex.Message);
            }
        }

        /// <summary>
        /// 创建一个新的世家数据实例
        /// </summary>
        /// <param name="status">世家状态</param>
        /// <param name="familyName">世家姓氏</param>
        /// <param name="level">世家等级</param>
        /// <param name="relationship">与玩家家族的关系</param>
        /// <param name="unknown4">未知含义参数</param>
        /// <param name="junXian">世家所在郡县 (格式: 郡索引|县索引)</param>
        /// <param name="unknown6">未知含义参数</param>
        /// <param name="unknown7">未知含义参数</param>
        /// <param name="unknown8">未知含义参数</param>
        /// <param name="unknown9">未知含义参数</param>
        /// <param name="unknown10">未知含义参数</param>
        /// <param name="unknown11">未知含义参数</param>
        /// <param name="unknown12">未知含义参数</param>
        /// <returns>世家数据列表</returns>
        public static List<string> CreateShiJiaInstance(
            string status = "1",
            string familyName = "",
            string level = "0",
            string relationship = "0",
            string unknown4 = "0",
            string junXian = "0|0",
            string unknown6 = "0",
            string unknown7 = "0",
            string unknown8 = "0",
            string unknown9 = "0",
            string unknown10 = "0",
            string unknown11 = "0",
            string unknown12 = "0")
        {
            return new List<string>
            {
                status,
                familyName,
                level,
                relationship,
                unknown4,
                junXian,
                unknown6,
                unknown7,
                unknown8,
                unknown9,
                unknown10,
                unknown11,
                unknown12
            };
        }

        /// <summary>
        /// 根据索引获取世家数据
        /// </summary>
        /// <param name="index">世家索引</param>
        /// <returns>世家数据，如果索引无效则返回null</returns>
        public static List<string> GetShiJiaByIndex(int index)
        {
            EnsureShiJiaNowExists();
            
            if (index >= 0 && index < Mainload.ShiJia_Now.Count)
            {
                return Mainload.ShiJia_Now[index];
            }
            return null;
        }

        /// <summary>
        /// 根据姓氏查找世家
        /// </summary>
        /// <param name="familyName">世家姓氏</param>
        /// <returns>找到的世家数据列表，如果未找到则返回空列表</returns>
        public static List<List<string>> FindShiJiaByFamilyName(string familyName)
        {
            EnsureShiJiaNowExists();
            
            List<List<string>> result = new List<List<string>>();
            foreach (var shiJia in Mainload.ShiJia_Now)
            {
                if (shiJia != null && shiJia.Count > ShiJiaIndexes.FAMILY_NAME && 
                    shiJia[ShiJiaIndexes.FAMILY_NAME] == familyName)
                {
                    result.Add(shiJia);
                }
            }
            return result;
        }

        /// <summary>
        /// 添加新的世家到Mainload.ShiJia_Now
        /// </summary>
        /// <param name="shiJiaData">世家数据</param>
        /// <returns>添加是否成功</returns>
        public static bool AddShiJia(List<string> shiJiaData)
        {
            try
            {
                EnsureShiJiaNowExists();
                
                if (shiJiaData != null && shiJiaData.Count >= 13) // 确保数据结构完整性
                {
                    Mainload.ShiJia_Now.Add(shiJiaData);
                    Debug.Log("已添加新世家: " + (shiJiaData.Count > ShiJiaIndexes.FAMILY_NAME ? shiJiaData[ShiJiaIndexes.FAMILY_NAME] : "未知姓氏"));
                    return true;
                }
                else
                {
                    Debug.LogError("添加世家失败：数据结构不完整");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("添加世家时发生错误: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 根据郡县索引查找世家
        /// </summary>
        /// <param name="junIndex">郡索引</param>
        /// <param name="xianIndex">县索引</param>
        /// <returns>找到的世家数据列表，如果未找到则返回空列表</returns>
        public static List<List<string>> FindShiJiaByJunXian(int junIndex, int xianIndex)
        {
            EnsureShiJiaNowExists();
            
            List<List<string>> result = new List<List<string>>();
            string junXianKey = $"{junIndex}|{xianIndex}";
            
            foreach (var shiJia in Mainload.ShiJia_Now)
            {
                if (shiJia != null && shiJia.Count > ShiJiaIndexes.JUN_XIAN && 
                    shiJia[ShiJiaIndexes.JUN_XIAN] == junXianKey)
                {
                    result.Add(shiJia);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取郡县名称
        /// </summary>
        /// <param name="junIndex">郡索引</param>
        /// <param name="xianIndex">县索引</param>
        /// <returns>郡县名称，如果索引无效则返回"未知郡县"</returns>
        public static string GetJunXianName(int junIndex, int xianIndex)
        {
            if (junIndex >= 0 && junIndex < JunList.Length && 
                xianIndex >= 0 && xianIndex < XianList[junIndex].Length)
            {
                return $"{JunList[junIndex]}{XianList[junIndex][xianIndex]}";
            }
            return "未知郡县";
        }

        /// <summary>
        /// 获取世家所在郡县的显示名称
        /// </summary>
        /// <param name="shiJiaData">世家数据</param>
        /// <returns>郡县显示名称，如果无法解析则返回"未知郡县"</returns>
        public static string GetJunXianDisplayName(List<string> shiJiaData)
        {
            if (shiJiaData != null && shiJiaData.Count > ShiJiaIndexes.JUN_XIAN)
            {
                string junXianStr = shiJiaData[ShiJiaIndexes.JUN_XIAN];
                string[] parts = junXianStr.Split('|');
                
                if (parts.Length >= 2 && 
                    int.TryParse(parts[0], out int junIndex) && 
                    int.TryParse(parts[1], out int xianIndex))
                {
                    return GetJunXianName(junIndex, xianIndex);
                }
            }
            return "未知郡县";
        }

        /// <summary>
        /// 设置世家所在郡县位置
        /// </summary>
        /// <param name="shiJiaData">世家数据</param>
        /// <param name="junIndex">郡索引</param>
        /// <param name="xianIndex">县索引</param>
        public static void SetJunXianPosition(List<string> shiJiaData, int junIndex, int xianIndex)
        {
            if (shiJiaData != null && shiJiaData.Count > ShiJiaIndexes.JUN_XIAN)
            {
                shiJiaData[ShiJiaIndexes.JUN_XIAN] = $"{junIndex}|{xianIndex}";
            }
        }

        /// <summary>
        /// 获取所有世家的简要信息
        /// </summary>
        /// <returns>所有世家的简要信息列表</returns>
        public static List<string> GetAllShiJiaSummaries()
        {
            EnsureShiJiaNowExists();
            
            List<string> summaries = new List<string>();
            foreach (var shiJia in Mainload.ShiJia_Now)
            {
                summaries.Add(GetShiJiaSummary(shiJia));
            }
            return summaries;
        }

        /// <summary>
        /// 获取世家的简要信息字符串
        /// </summary>
        /// <param name="shiJiaData">世家数据</param>
        /// <returns>世家简要信息</returns>
        public static string GetShiJiaSummary(List<string> shiJiaData)
        {
            if (shiJiaData == null || shiJiaData.Count < 4)
            {
                return "无效的世家数据";
            }
            
            string familyName = shiJiaData[ShiJiaIndexes.FAMILY_NAME];
            string level = shiJiaData[ShiJiaIndexes.LEVEL];
            string relationship = shiJiaData[ShiJiaIndexes.RELATIONSHIP];
            string junXian = GetJunXianDisplayName(shiJiaData);
            
            return $"{familyName}家 (等级: {level}, 关系: {relationship}) - {junXian}";
        }
    }
}
/*
3. 如果您希望在现有代码中立即使用世家数据结构，也可以将`ShiJiaData`类的代码添加到`AddItem.cs`文件的开头（在`AddItem`类之前）

此实现完全按照您提供的需求：
- 定义了`Mainload.ShiJia_Now`作为一个`List<List<string>>`类型的数据结构
- 每个世家元素是一个包含13个字符串的列表
- 第一个元素"0"表示世家状态（0为不显示）
- 第二个元素为世家姓氏
- 第三个元素为世家等级
- 第四个元素为玩家家族和该世家的关系
- 第六个元素为世家所在郡县（格式为"郡索引|县索引"）

同时，还提供了一系列辅助方法，包括初始化世家数据、创建新世家实例、查找世家、获取世家信息等功能，可以方便地在代码中操作世家数据。
*/