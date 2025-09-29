using UnityEngine;
using System.Collections.Generic;

namespace cs.HoLMod.Member_KingData
{
    /// <summary>
    /// 皇室主脉人物数据结构及工具类
    /// </summary>
    public class MemberKingQuData
    {
        /// <summary>
        /// 皇室主脉人物状态常量定义
        /// </summary>
        public class MemberStatus
        {
            public const string NOT_PREGNANT = "-1"; // 未怀孕
            public const string MARRIED = "1";      // 已婚
            
            // 婚姻状况常量
            public const string MARITAL_STATUS_UNMARRIED = "0";   // 未婚
            public const string MARITAL_STATUS_MARRIED = "1";     // 已婚
            public const string MARITAL_STATUS_WIDOWED = "2";     // 丧偶
            public const string MARITAL_STATUS_REMARRIED = "3";   // 再婚
            public const string MARITAL_STATUS_SEIZED = "4";      // 夺妻
            public const string MARITAL_STATUS_DIVORCED_WIFE = "5"; // 被休
            public const string MARITAL_STATUS_DIVORCED = "6";    // 离婚
        }

        /// <summary>
        /// 皇室主脉人物数据数组索引定义
        /// </summary>
        public class MemberIndexes
        {
            public const int PERSON_ID = 0;          // 人物编号
            public const int APPEARANCE = 1;         // 人物形象 (后发|身体|脸部|前发)
            public const int PERSON_DATA = 2;        // 人物姓名|？|天赋|天赋点|性别|寿命|技能|幸运|品性|主人人物编号|爱好|null
            public const int UNKNOWN_3 = 3;          // 年龄
            public const int UNKNOWN_4 = 4;          // 文
            public const int UNKNOWN_5 = 5;          // 武
            public const int UNKNOWN_6 = 6;          // 商
            public const int UNKNOWN_7 = 7;          // 艺
            public const int UNKNOWN_8 = 8;          // 心情
            public const int UNKNOWN_9 = 9;          // 官职功勋
            public const int UNKNOWN_10 = 10;        // 爵位
            public const int UNKNOWN_11 = 11;        // 怀孕月份，-1表示未怀孕，1-10代表怀胎10-1月
            public const int UNKNOWN_12 = 12;        // 未知字段12
            public const int UNKNOWN_13 = 13;        // 子嗣人物编号，多个子嗣用|分隔
            public const int UNKNOWN_14 = 14;        // 好感度
            public const int UNKNOWN_15 = 15;        // 状态
            public const int UNKNOWN_16 = 16;        // 声誉
            public const int UNKNOWN_17 = 17;        // 未知字段17
            public const int UNKNOWN_18 = 18;        // 魅力
            public const int UNKNOWN_19 = 19;        // 健康
            public const int UNKNOWN_20 = 20;        // 婚姻状况，0表示未婚，1表示已婚，2表示丧偶，3表示再婚，4表示夺妻，5表示被休，6表示离婚
            public const int UNKNOWN_21 = 21;        // 计谋
            public const int UNKNOWN_22 = 22;        // 每月成长
            public const int UNKNOWN_23 = 23;        // 技能点
            public const int UNKNOWN_24 = 24;        // 未知字段24
            public const int UNKNOWN_25 = 25;        // 生平记事，结构为A@B@C@D(A代表事件的年龄，B代表事件编号，C和D根据B有相应的值没有值的时候为null)，多个事件之间用|分隔
            public const int UNKNOWN_26 = 26;        // 未知字段26
            public const int UNKNOWN_27 = 27;        // 未知字段27
            public const int UNKNOWN_28 = 28;        // 未知字段28
            public const int UNKNOWN_29 = 29;        // 夫妻关系
            public const int UNKNOWN_30 = 30;        // 未知字段30
        }

        /// <summary>
        /// 检查Mainload.Member_King是否存在并初始化（如果不存在）
        /// </summary>
        public static void EnsureMemberKingQuExists()
        {
            try
            {
                // 尝试访问Mainload.Member_King，如果不存在则创建
                if (Mainload.Member_King == null)
                {
                    Mainload.Member_King = new List<List<string>>();
                    Debug.Log("已初始化Mainload.Member_King");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("初始化Mainload.Member_King时发生错误: " + ex.Message);
            }
        }

        /// <summary>
        /// 创建一个新的皇室主脉人物数据实例
        /// </summary>
        /// <param name="personId">人物编号</param>
        /// <param name="appearance">人物形象 (后发|身体|脸部|前发)</param>
        /// <param name="personData">人物姓名|？|天赋|天赋点|性别|技能|幸运|品性|主人人物编号|爱好|null</param>
        /// <param name="age">年龄</param>
        /// <param name="literature">文</param>
        /// <param name="martialArts">武</param>
        /// <param name="business">商</param>
        /// <param name="art">艺</param>
        /// <param name="mood">心情</param>
        /// <param name="officialPosition">官职功勋</param>
        /// <param name="title">爵位</param>
        /// <param name="pregnancyMonth">怀孕月份，-1表示未怀孕，1-10代表怀胎10-1月</param>
        /// <param name="unknown12">未知字段12</param>
        /// <param name="childrenIds">子嗣人物编号，多个子嗣用|分隔</param>
        /// <param name="affection">好感度</param>
        /// <param name="status">状态</param>
        /// <param name="reputation">声誉</param>
        /// <param name="unknown17">未知字段17</param>
        /// <param name="charm">魅力</param>
        /// <param name="health">健康</param>
        /// <param name="maritalStatus">婚姻状况，0表示未婚，1表示已婚，2表示丧偶，3表示再婚，4表示夺妻，5表示被休，6表示离婚</param>
        /// <param name="strategy">计谋</param>
        /// <param name="monthlyGrowth">每月成长</param>
        /// <param name="skillPoints">技能点</param>
        /// <param name="unknown24">未知字段24</param>
        /// <param name="lifeEvents">生平记事，结构为A@B@C@D(A代表事件的年龄，B代表事件编号，C和D根据B有相应的值没有值的时候为null)，多个事件之间用|分隔</param>
        /// <param name="unknown26">未知字段26</param>
        /// <param name="unknown27">未知字段27</param>
        /// <param name="unknown28">未知字段28</param>
        /// <param name="maritalRelationship">夫妻关系</param>
        /// <param name="unknown30">未知字段30</param>
        /// <returns>皇室主脉人物数据列表</returns>
        public static List<string> CreateMemberInstance(
            string personId = "",
            string appearance = "0|0|0|0",
            string personData = "|-100|0|0|0|0|0|0|M0|0|null",
            string age = "0",
            string literature = "0",
            string martialArts = "0",
            string business = "0",
            string art = "0",
            string mood = "100",
            string officialPosition = "0",
            string title = "0",
            string pregnancyMonth = "-1",
            string unknown12 = "0",
            string childrenIds = "null",
            string affection = "100",
            string status = "0",
            string reputation = "0",
            string unknown17 = "0",
            string charm = "0",
            string health = "100",
            string maritalStatus = "0",
            string strategy = "0",
            string monthlyGrowth = "0|0|0|0|0|0|0",
            string skillPoints = "0",
            string unknown24 = "0",
            string lifeEvents = "null",
            string unknown26 = "0",
            string unknown27 = "0",
            string unknown28 = "0",
            string maritalRelationship = "null",
            string unknown30 = "null")
        {
            return new List<string>
            {
                personId,
                appearance,
                personData,
                age,
                literature,
                martialArts,
                business,
                art,
                mood,
                officialPosition,
                title,
                pregnancyMonth,
                unknown12,
                childrenIds,
                affection,
                status,
                reputation,
                unknown17,
                charm,
                health,
                maritalStatus,
                strategy,
                monthlyGrowth,
                skillPoints,
                unknown24,
                lifeEvents,
                unknown26,
                unknown27,
                unknown28,
                maritalRelationship,
                unknown30
            };
        }

        /// <summary>
        /// 加载Mainload.Member_King数据为皇室主脉人物的数据数组
        /// </summary>
        /// <returns>皇室主脉人物数据数组</returns>
        public static List<List<string>> LoadMemberData()
        {
            EnsureMemberKingQuExists();
            return Mainload.Member_King;
        }

        /// <summary>
        /// 根据索引获取皇室主脉人物数据
        /// </summary>
        /// <param name="index">皇室主脉人物索引</param>
        /// <returns>皇室主脉人物数据，如果索引无效则返回null</returns>
        public static List<string> GetMemberByIndex(int index)
        {
            EnsureMemberKingQuExists();
            
            if (index >= 0 && index < Mainload.Member_King.Count)
            {
                return Mainload.Member_King[index];
            }
            return null;
        }

        /// <summary>
        /// 根据人物编号查找皇室主脉人物
        /// </summary>
        /// <param name="personId">人物编号</param>
        /// <returns>找到的皇室主脉人物数据，如果未找到则返回null</returns>
        public static List<string> FindMemberById(string personId)
        {
            EnsureMemberKingQuExists();
            
            foreach (var member in Mainload.Member_King)
            {
                if (member != null && member.Count > MemberIndexes.PERSON_ID && 
                    member[MemberIndexes.PERSON_ID] == personId)
                {
                    return member;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据姓名查找皇室主脉人物
        /// </summary>
        /// <param name="name">人物姓名</param>
        /// <returns>找到的皇室主脉人物数据列表，如果未找到则返回空列表</returns>
        public static List<List<string>> FindMemberByName(string name)
        {
            EnsureMemberKingQuExists();
            
            List<List<string>> result = new List<List<string>>();
            foreach (var member in Mainload.Member_King)
            {
                if (member != null && member.Count > MemberIndexes.PERSON_DATA)
                {
                    string personData = member[MemberIndexes.PERSON_DATA];
                    string[] parts = personData.Split('|');
                    if (parts.Length > 0 && parts[0] == name)
                    {
                        result.Add(member);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 添加新的皇室主脉人物到Mainload.Member_King
        /// </summary>
        /// <param name="memberData">皇室主脉人物数据</param>
        /// <returns>添加是否成功</returns>
        public static bool AddMember(List<string> memberData)
        {
            try
            {
                EnsureMemberKingQuExists();
                
                if (memberData != null && memberData.Count >= 31) // 确保数据结构完整性
                {
                    Mainload.Member_King.Add(memberData);
                    Debug.Log("已添加新皇室主脉人物: " + GetMemberName(memberData));
                    return true;
                }
                else
                {
                    Debug.LogError("添加皇室主脉人物失败：数据结构不完整，需要至少31个字段");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("添加皇室主脉人物时发生错误: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取皇室主脉人物的姓名
        /// </summary>
        /// <param name="memberData">皇室主脉人物数据</param>
        /// <returns>皇室主脉人物姓名，如果无法解析则返回"未知姓名"</returns>
        public static string GetMemberName(List<string> memberData)
        {
            if (memberData != null && memberData.Count > MemberIndexes.PERSON_DATA)
            {
                string personData = memberData[MemberIndexes.PERSON_DATA];
                string[] parts = personData.Split('|');
                if (parts.Length > 0 && !string.IsNullOrEmpty(parts[0]) && parts[0] != "null")
                {
                    return parts[0];
                }
            }
            return "未知姓名";
        }

        /// <summary>
        /// 获取皇室主脉人物的简要信息字符串
        /// </summary>
        /// <param name="memberData">皇室主脉人物数据</param>
        /// <returns>皇室主脉人物简要信息</returns>
        public static string GetMemberSummary(List<string> memberData)
        {
            if (memberData == null || memberData.Count < 20)
            {
                return "无效的皇室主脉人物数据";
            }
            
            string name = GetMemberName(memberData);
            string personId = memberData[MemberIndexes.PERSON_ID];
            string age = memberData[MemberIndexes.UNKNOWN_3];
            string literature = memberData[MemberIndexes.UNKNOWN_4];
            string martialArts = memberData[MemberIndexes.UNKNOWN_5];
            string business = memberData[MemberIndexes.UNKNOWN_6];
            string art = memberData[MemberIndexes.UNKNOWN_7];
            string mood = memberData[MemberIndexes.UNKNOWN_8];
            string affection = memberData[MemberIndexes.UNKNOWN_14];
            string health = memberData[MemberIndexes.UNKNOWN_19];
            string pregnancyStatus = memberData[MemberIndexes.UNKNOWN_11] == "-1" ? "未怀孕" : "怀孕中";
            string maritalStatusText = GetMaritalStatusText(memberData[MemberIndexes.UNKNOWN_20]);
            
            return $"{name} (ID: {personId}, 年龄: {age}) - 文: {literature}, 武: {martialArts}, 商: {business}, 艺: {art}\n" +
                   $"心情: {mood}, 好感度: {affection}, 健康: {health}, 婚姻状况: {maritalStatusText}\n" +
                   $"{pregnancyStatus}";
        }

        /// <summary>
        /// 获取皇室主脉的技能属性
        /// </summary>
        /// <param name="memberData">皇室主脉人物数据</param>
        /// <returns>包含文、武、商、艺的字典</returns>
        public static Dictionary<string, string> GetSkills(List<string> memberData)
        {
            Dictionary<string, string> skills = new Dictionary<string, string>();
            
            if (memberData != null && memberData.Count > 7)
            {
                skills.Add("文", memberData[MemberIndexes.UNKNOWN_4]);
                skills.Add("武", memberData[MemberIndexes.UNKNOWN_5]);
                skills.Add("商", memberData[MemberIndexes.UNKNOWN_6]);
                skills.Add("艺", memberData[MemberIndexes.UNKNOWN_7]);
            }
            
            return skills;
        }

        /// <summary>
        /// 获取皇室主脉的状态信息
        /// </summary>
        /// <param name="memberData">皇室主脉人物数据</param>
        /// <returns>包含心情、好感度、健康的字典</returns>
        public static Dictionary<string, string> GetStatusInfo(List<string> memberData)
        {
            Dictionary<string, string> statusInfo = new Dictionary<string, string>();
            
            if (memberData != null && memberData.Count > 20)
            {
                statusInfo.Add("心情", memberData[MemberIndexes.UNKNOWN_8]);
                statusInfo.Add("好感度", memberData[MemberIndexes.UNKNOWN_14]);
                statusInfo.Add("健康", memberData[MemberIndexes.UNKNOWN_19]);
                statusInfo.Add("魅力", memberData[MemberIndexes.UNKNOWN_18]);
                statusInfo.Add("声誉", memberData[MemberIndexes.UNKNOWN_16]);
                statusInfo.Add("婚姻状况", GetMaritalStatusText(memberData[MemberIndexes.UNKNOWN_20]));
            }
            
            return statusInfo;
        }

        /// <summary>
        /// 获取婚姻状况的中文描述
        /// </summary>
        /// <param name="maritalStatus">婚姻状况代码</param>
        /// <returns>婚姻状况的中文描述</returns>
        public static string GetMaritalStatusText(string maritalStatus)
        {
            switch (maritalStatus)
            {
                case MemberStatus.MARITAL_STATUS_UNMARRIED:
                    return "未婚";
                case MemberStatus.MARITAL_STATUS_MARRIED:
                    return "已婚";
                case MemberStatus.MARITAL_STATUS_WIDOWED:
                    return "丧偶";
                case MemberStatus.MARITAL_STATUS_REMARRIED:
                    return "再婚";
                case MemberStatus.MARITAL_STATUS_SEIZED:
                    return "夺妻";
                case MemberStatus.MARITAL_STATUS_DIVORCED_WIFE:
                    return "被休";
                case MemberStatus.MARITAL_STATUS_DIVORCED:
                    return "离婚";
                default:
                    return "未知";
            }
        }

        /// <summary>
        /// 检查皇室主脉是否已婚
        /// </summary>
        /// <param name="memberData">皇室主脉人物数据</param>
        /// <returns>是否已婚</returns>
        public static bool IsMarried(List<string> memberData)
        {
            if (memberData != null && memberData.Count > 20)
            {
                return memberData[MemberIndexes.UNKNOWN_20] == MemberStatus.MARITAL_STATUS_MARRIED;
            }
            return false;
        }

        /// <summary>
        /// 获取皇室主脉的婚姻状况代码
        /// </summary>
        /// <param name="memberData">皇室主脉人物数据</param>
        /// <returns>婚姻状况代码</returns>
        public static string GetMaritalStatus(List<string> memberData)
        {
            if (memberData != null && memberData.Count > 20)
            {
                return memberData[MemberIndexes.UNKNOWN_20];
            }
            return "0";
        }

        /// <summary>
        /// 检查皇室主脉是否怀孕
        /// </summary>
        /// <param name="memberData">皇室主脉人物数据</param>
        /// <returns>是否怀孕</returns>
        public static bool IsPregnant(List<string> memberData)
        {
            if (memberData != null && memberData.Count > 11)
            {
                return memberData[MemberIndexes.UNKNOWN_11] != "-1";
            }
            return false;
        }

        /// <summary>
        /// 获取皇室主脉的怀孕月份
        /// </summary>
        /// <param name="memberData">皇室主脉人物数据</param>
        /// <returns>怀孕月份，如果未怀孕则返回-1</returns>
        public static int GetPregnancyMonth(List<string> memberData)
        {
            if (memberData != null && memberData.Count > 11)
            {
                int month;
                if (int.TryParse(memberData[MemberIndexes.UNKNOWN_11], out month))
                {
                    return month;
                }
            }
            return -1;
        }

        /// <summary>
        /// 获取皇室主脉的子嗣列表
        /// </summary>
        /// <param name="memberData">皇室主脉人物数据</param>
        /// <returns>子嗣ID列表</returns>
        public static List<string> GetChildrenIds(List<string> memberData)
        {
            List<string> childrenIds = new List<string>();
            
            if (memberData != null && memberData.Count > 13 && memberData[MemberIndexes.UNKNOWN_13] != "null")
            {
                string[] ids = memberData[MemberIndexes.UNKNOWN_13].Split('|');
                foreach (string id in ids)
                {
                    if (!string.IsNullOrEmpty(id) && id != "null")
                    {
                        childrenIds.Add(id);
                    }
                }
            }
            
            return childrenIds;
        }
    }
}