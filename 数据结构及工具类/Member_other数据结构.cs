using System.Collections.Generic;

namespace cs.HoLMod.Member_otherData
{
    /// <summary>
    /// 世家主脉成员数据结构及工具类
    /// </summary>
    public class Member_otherData
    {
        /// <summary>
        /// 世家主脉成员状态常量定义
        /// </summary>
        public class Member_otherStatus
        {
            // 可以在这里添加状态常量，当明确各种状态含义时
        }

        /// <summary>
        /// 世家主脉成员数据数组索引定义
        /// </summary>
        public class Member_otherIndexes
        {
            public const int ID = 0;                        // 人物编号
            public const int APPEARANCE = 1;                // 人物形象 (后发|身体|脸部|前发)
            public const int PERSON_DATA = 2;               // 姓名|？|天赋|天赋点|性别|寿命|技能|幸运|性格|主人的编号
            public const int AGE = 3;                       // 年龄
            public const int LITERARY = 4;                  // 文
            public const int MARTIAL = 5;                   // 武
            public const int BUSINESS = 6;                  // 商
            public const int ART = 7;                       // 艺
            public const int MOOD = 8;                      // 心情
            public const int OFFICIAL_TITLE = 9;            // 官职功勋
            public const int MERITS = 10;                   // 功名
            public const int TITLE = 11;                    // 爵位
            public const int UNKNOWN_12 = 12;               // 未知含义
            public const int RECENT_EVENTS = 13;            // 近期记事
            public const int CHILDREN_IDS = 14;             // 子嗣人物编号（多个时用|分隔）
            public const int FAVORABILITY = 15;             // 好感度
            public const int STATUS = 16;                   // 状态
            public const int REPUTATION = 17;               // 声誉
            public const int UNKNOWN_18 = 18;               // 未知含义
            public const int CHARM = 19;                    // 魅力
            public const int HEALTH = 20;                   // 健康
            public const int MARITAL_STATUS = 21;           // 婚姻状况
            public const int STRATEGY = 22;                 // 计谋
            public const int UNKNOWN_23 = 23;               // 未知含义
            public const int UNKNOWN_24 = 24;               // 未知含义
            public const int SKILL_POINTS = 25;             // 技能点
            public const int UNKNOWN_26 = 26;               // 未知含义
            public const int LIFE_RECORDS = 27;             // 生平记事
            public const int UNKNOWN_28 = 28;               // 未知含义
            public const int SPECIAL_TAG = 29;              // 特殊标签
            public const int SCHOOL = 30;                   // 学派
        }

        /// <summary>
        /// 检查Mainload.Member_other是否存在并初始化（如果不存在）
        /// </summary>
        public static void EnsureMember_otherExists()
        {
            try
            {
                // 尝试访问Mainload.Member_other，如果不存在则创建
                if (Mainload.Member_other == null)
                {
                    Mainload.Member_other = new List<List<List<string>>>();
                    Debug.Log("已初始化Mainload.Member_other");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("初始化Mainload.Member_other时发生错误: " + ex.Message);
            }
        }

        /// <summary>
        /// 创建一个新的世家主脉成员数据实例
        /// </summary>
        /// <param name="id">人物编号</param>
        /// <param name="appearance">人物形象</param>
        /// <param name="personData">人物数据</param>
        /// <param name="age">年龄</param>
        /// <param name="literary">文</param>
        /// <param name="martial">武</param>
        /// <param name="business">商</param>
        /// <param name="art">艺</param>
        /// <param name="mood">心情</param>
        /// <param name="officialTitle">官职功勋</param>
        /// <param name="merits">功名</param>
        /// <param name="title">爵位</param>
        /// <param name="unknown12">未知含义参数</param>
        /// <param name="recentEvents">近期记事</param>
        /// <param name="childrenIds">子嗣人物编号</param>
        /// <param name="favorability">好感度</param>
        /// <param name="status">状态</param>
        /// <param name="reputation">声誉</param>
        /// <param name="unknown18">未知含义参数</param>
        /// <param name="charm">魅力</param>
        /// <param name="health">健康</param>
        /// <param name="maritalStatus">婚姻状况</param>
        /// <param name="strategy">计谋</param>
        /// <param name="unknown23">未知含义参数</param>
        /// <param name="unknown24">未知含义参数</param>
        /// <param name="skillPoints">技能点</param>
        /// <param name="unknown26">未知含义参数</param>
        /// <param name="lifeRecords">生平记事</param>
        /// <param name="unknown28">未知含义参数</param>
        /// <param name="specialTag">特殊标签</param>
        /// <param name="school">学派</param>
        /// <returns>世家主脉成员数据列表</returns>
        public static List<string> CreateMember_otherInstance(
            string id = "0",
            string appearance = "0|0|0|0",
            string personData = "|0|0|0|0|0|0|0|0|0",
            string age = "0",
            string literary = "0",
            string martial = "0",
            string business = "0",
            string art = "0",
            string mood = "0",
            string officialTitle = "0",
            string merits = "0",
            string title = "0",
            string unknown12 = "0",
            string recentEvents = "",
            string childrenIds = "",
            string favorability = "0",
            string status = "0",
            string reputation = "0",
            string unknown18 = "0",
            string charm = "0",
            string health = "0",
            string maritalStatus = "0",
            string strategy = "0",
            string unknown23 = "0",
            string unknown24 = "0",
            string skillPoints = "0",
            string unknown26 = "0",
            string lifeRecords = "",
            string unknown28 = "0",
            string specialTag = "0",
            string school = "0")
        {
            return new List<string>
            {
                id,
                appearance,
                personData,
                age,
                literary,
                martial,
                business,
                art,
                mood,
                officialTitle,
                merits,
                title,
                unknown12,
                recentEvents,
                childrenIds,
                favorability,
                status,
                reputation,
                unknown18,
                charm,
                health,
                maritalStatus,
                strategy,
                unknown23,
                unknown24,
                skillPoints,
                unknown26,
                lifeRecords,
                unknown28,
                specialTag,
                school
            };
        }

        /// <summary>
        /// 根据索引获取世家主脉成员数据
        /// </summary>
        /// <param name="shiJiaIndex">世家索引</param>
        /// <param name="memberIndex">成员索引</param>
        /// <returns>世家主脉成员数据，如果索引无效则返回null</returns>
        public static List<string> GetMember_otherByIndex(int shiJiaIndex, int memberIndex)
        {
            EnsureMember_otherExists();
            
            if (shiJiaIndex >= 0 && shiJiaIndex < Mainload.Member_other.Count)
            {
                var shiJiaMembers = Mainload.Member_other[shiJiaIndex];
                if (memberIndex >= 0 && memberIndex < shiJiaMembers.Count)
                {
                    return shiJiaMembers[memberIndex];
                }
            }
            return null;
        }

        /// <summary>
        /// 根据姓名查找世家主脉成员
        /// </summary>
        /// <param name="shiJiaIndex">世家索引</param>
        /// <param name="name">成员姓名</param>
        /// <returns>找到的世家主脉成员数据列表，如果未找到则返回空列表</returns>
        public static List<List<string>> FindMember_otherByName(int shiJiaIndex, string name)
        {
            EnsureMember_otherExists();
            
            List<List<string>> result = new List<List<string>>();
            
            if (shiJiaIndex >= 0 && shiJiaIndex < Mainload.Member_other.Count)
            {
                var shiJiaMembers = Mainload.Member_other[shiJiaIndex];
                foreach (var member in shiJiaMembers)
                {
                    if (member != null && member.Count > Member_otherIndexes.PERSON_DATA)
                    {
                        string personData = member[Member_otherIndexes.PERSON_DATA];
                        string[] parts = personData.Split('|');
                        if (parts.Length > 0 && parts[0] == name)
                        {
                            result.Add(member);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 为指定世家添加新的主脉成员
        /// </summary>
        /// <param name="shiJiaIndex">世家索引</param>
        /// <param name="memberData">成员数据</param>
        /// <returns>添加是否成功</returns>
        public static bool AddMember_other(int shiJiaIndex, List<string> memberData)
        {
            try
            {
                EnsureMember_otherExists();
                
                // 确保世家列表足够大
                while (Mainload.Member_other.Count <= shiJiaIndex)
                {
                    Mainload.Member_other.Add(new List<List<string>>());
                }
                
                if (memberData != null && memberData.Count >= 31) // 确保数据结构完整性
                {
                    Mainload.Member_other[shiJiaIndex].Add(memberData);
                    Debug.Log("已添加新世家主脉成员");
                    return true;
                }
                else
                {
                    Debug.LogError("添加世家主脉成员失败：数据结构不完整");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("添加世家主脉成员时发生错误: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取世家主脉成员的姓名
        /// </summary>
        /// <param name="memberData">成员数据</param>
        /// <returns>成员姓名，如果无法解析则返回"未知姓名"</returns>
        public static string GetMember_otherName(List<string> memberData)
        {
            if (memberData != null && memberData.Count > Member_otherIndexes.PERSON_DATA)
            {
                string personData = memberData[Member_otherIndexes.PERSON_DATA];
                string[] parts = personData.Split('|');
                if (parts.Length > 0 && !string.IsNullOrEmpty(parts[0]))
                {
                    return parts[0];
                }
            }
            return "未知姓名";
        }

        /// <summary>
        /// 获取世家主脉成员的简要信息
        /// </summary>
        /// <param name="memberData">成员数据</param>
        /// <returns>成员简要信息</returns>
        public static string GetMember_otherSummary(List<string> memberData)
        {
            if (memberData == null || memberData.Count < 20)
            {
                return "无效的世家主脉成员数据";
            }
            
            string name = GetMember_otherName(memberData);
            string age = memberData[Member_otherIndexes.AGE];
            string literary = memberData[Member_otherIndexes.LITERARY];
            string martial = memberData[Member_otherIndexes.MARTIAL];
            string business = memberData[Member_otherIndexes.BUSINESS];
            string art = memberData[Member_otherIndexes.ART];
            string mood = memberData[Member_otherIndexes.MOOD];
            string health = memberData[Member_otherIndexes.HEALTH];
            
            return $"{name} (年龄: {age}, 四维: 文{literary} 武{martial} 商{business} 艺{art}, 心情: {mood}, 健康: {health})";
        }

        /// <summary>
        /// 加载世家主脉成员数据
        /// </summary>
        /// <param name="shiJiaIndex">世家索引</param>
        /// <returns>该世家的所有主脉成员数据</returns>
        public static List<List<string>> LoadMember_otherData(int shiJiaIndex)
        {
            EnsureMember_otherExists();
            
            if (shiJiaIndex >= 0 && shiJiaIndex < Mainload.Member_other.Count)
            {
                return Mainload.Member_other[shiJiaIndex];
            }
            return new List<List<string>>();
        }
    }
}