using UnityEngine;
using System.Collections.Generic;

namespace cs.HoLMod.Member_quData
{
    /// <summary>
    /// 家族族人数据结构及工具类
    /// </summary>
    public class MemberQuData
    {
        /// <summary>
        /// 家族族人状态常量定义
        /// </summary>
        public class MemberStatus
        {
            public const string NOT_PREGNANT = "-1"; // 未怀孕
            public const string MARRIED = "1";      // 已婚(住房数据中的第一个元素)
            // 可以在这里添加其他状态常量，当明确其他状态含义时
        }

        /// <summary>
        /// 家族族人数据数组索引定义
        /// </summary>
        public class MemberIndexes
        {
            public const int PERSON_ID = 0;          // 人物编号
            public const int APPEARANCE = 1;         // 人物形象 (后发|身体|脸部|前发)
            public const int PERSON_DATA = 2;        // 人物姓名|？|天赋|天赋点|性别|技能|幸运|品性|主人人物编号|爱好|null
            public const int CHILD_PERSON_ID = 3;    // 子嗣的人物编号
            public const int HOUSING_DATA = 4;       // 住房数据 (1代表正房)|房子的建筑编号|null|房子一共住了多少人
            public const int AGE = 5;                // 年龄
            public const int LITERATURE = 6;         // 文
            public const int MARTIAL_ARTS = 7;       // 武
            public const int BUSINESS = 8;           // 商
            public const int ART = 9;                // 艺
            public const int MOOD = 10;              // 心情
            public const int STATUS = 11;            // 状态
            public const int REPUTATION = 12;        // 声誉
            public const int STATUS_DURATION = 13;   // 状态持续时间
            public const int EQUIPMENT = 14;         // 装备 (武器|珠宝|符咒)
            public const int CHARM = 15;             // 魅力
            public const int HEALTH = 16;            // 健康
            public const int RECENT_EVENTS = 17;     // 近期记事
            public const int PREGNANCY_MONTH = 18;   // 怀孕月份 (-1为未怀孕，刚怀孕为10)
            public const int STRATEGY = 19;          // 计谋
            public const int STAMINA = 20;           // 体力
            public const int UNKNOWN_21 = 21;        // ？(一般不修改)
            public const int UNKNOWN_22 = 22;        // ？(一般不修改)
            public const int SKILL_POINTS = 23;      // 技能点
            public const int PREGNANCY_PROBABILITY = 24; // 怀孕概率(数值越高怀孕概率越低，大于等于100不孕不育)
            public const int UNKNOWN_25 = 25;        // ？(一般不修改)
            public const int LIFE_EVENTS = 26;       // 生平记事(没有的时候为null，一般不修改，多个生平记事时用|分隔)
            public const int SPECIAL_TAGS = 27;      // 特殊标签(没有的时候为null，一般不修改)
            public const int UNKNOWN_28 = 28;        // ？(一般不修改)
            public const int UNKNOWN_29 = 29;        // ？(一般不修改)
            public const int OFFICIAL_POSITION = 30; // 官职功勋(@作为分割符，0@0@-1@-1@-1|0)
            public const int MARITAL_RELATIONSHIP = 31; // 夫妻关系
            public const int CLAN_RESPONSIBILITIES = 32; // 族人职责
        }

        /// <summary>
        /// 检查Mainload.Member_qu是否存在并初始化（如果不存在）
        /// </summary>
        public static void EnsureMemberQuExists()
        {
            try
            {
                // 尝试访问Mainload.Member_qu，如果不存在则创建
                if (Mainload.Member_qu == null)
                {
                    Mainload.Member_qu = new List<List<string>>();
                    Debug.Log("已初始化Mainload.Member_qu");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("初始化Mainload.Member_qu时发生错误: " + ex.Message);
            }
        }

        /// <summary>
        /// 创建一个新的家族族人数据实例
        /// </summary>
        /// <param name="personId">人物编号</param>
        /// <param name="appearance">人物形象 (后发|身体|脸部|前发)</param>
        /// <param name="personData">人物姓名|？|天赋|天赋点|性别|技能|幸运|品性|主人人物编号|爱好|null</param>
        /// <param name="childPersonId">子嗣的人物编号</param>
        /// <param name="housingData">住房数据 (1代表正房)|房子的建筑编号|null|房子一共住了多少人</param>
        /// <param name="age">年龄</param>
        /// <param name="literature">文</param>
        /// <param name="martialArts">武</param>
        /// <param name="business">商</param>
        /// <param name="art">艺</param>
        /// <param name="mood">心情</param>
        /// <param name="status">状态</param>
        /// <param name="reputation">声誉</param>
        /// <param name="statusDuration">状态持续时间</param>
        /// <param name="equipment">装备 (武器|珠宝|符咒)</param>
        /// <param name="charm">魅力</param>
        /// <param name="health">健康</param>
        /// <param name="recentEvents">近期记事</param>
        /// <param name="pregnancyMonth">怀孕月份 (-1为未怀孕，刚怀孕为10)</param>
        /// <param name="strategy">计谋</param>
        /// <param name="stamina">体力</param>
        /// <param name="unknown21">？(一般不修改)</param>
        /// <param name="unknown22">？(一般不修改)</param>
        /// <param name="skillPoints">技能点</param>
        /// <param name="pregnancyProbability">怀孕概率</param>
        /// <param name="unknown25">？(一般不修改)</param>
        /// <param name="lifeEvents">生平记事</param>
        /// <param name="specialTags">特殊标签</param>
        /// <param name="unknown28">？(一般不修改)</param>
        /// <param name="unknown29">？(一般不修改)</param>
        /// <param name="officialPosition">官职功勋</param>
        /// <param name="maritalRelationship">夫妻关系</param>
        /// <param name="clanResponsibilities">族人职责</param>
        /// <returns>家族族人数据列表</returns>
        public static List<string> CreateMemberInstance(
            string personId = "",
            string appearance = "0|0|0|0",
            string personData = "|-100|0|0|0|0|0|0|M0|0|null",
            string childPersonId = "",
            string housingData = "0|LTB0|null|0",
            string age = "0",
            string literature = "0",
            string martialArts = "0",
            string business = "0",
            string art = "0",
            string mood = "100",
            string status = "0",
            string reputation = "0",
            string statusDuration = "0",
            string equipment = "null|null|null",
            string charm = "0",
            string health = "100",
            string recentEvents = "null",
            string pregnancyMonth = "-1",
            string strategy = "0",
            string stamina = "0",
            string unknown21 = "0|0|0|0|0",
            string unknown22 = "0|0|0|0|0",
            string skillPoints = "0",
            string pregnancyProbability = "0",
            string unknown25 = "0",
            string lifeEvents = "null",
            string specialTags = "null",
            string unknown28 = "0|0|0|0|0|0|0|0|0|0|0|0|0",
            string unknown29 = "0",
            string officialPosition = "0@0@-1@-1@-1|0",
            string maritalRelationship = "0|0",
            string clanResponsibilities = "0|0|0")
        {
            return new List<string>
            {
                personId,
                appearance,
                personData,
                childPersonId,
                housingData,
                age,
                literature,
                martialArts,
                business,
                art,
                mood,
                status,
                reputation,
                statusDuration,
                equipment,
                charm,
                health,
                recentEvents,
                pregnancyMonth,
                strategy,
                stamina,
                unknown21,
                unknown22,
                skillPoints,
                pregnancyProbability,
                unknown25,
                lifeEvents,
                specialTags,
                unknown28,
                unknown29,
                officialPosition,
                maritalRelationship,
                clanResponsibilities
            };
        }

        /// <summary>
        /// 加载Mainload.Member_qu数据为家族族人的数据数组
        /// </summary>
        /// <returns>家族族人数据数组</returns>
        public static List<List<string>> LoadMemberData()
        {
            EnsureMemberQuExists();
            return Mainload.Member_qu;
        }

        /// <summary>
        /// 根据索引获取家族族人数据
        /// </summary>
        /// <param name="index">家族族人索引</param>
        /// <returns>家族族人数据，如果索引无效则返回null</returns>
        public static List<string> GetMemberByIndex(int index)
        {
            EnsureMemberQuExists();
            
            if (index >= 0 && index < Mainload.Member_qu.Count)
            {
                return Mainload.Member_qu[index];
            }
            return null;
        }

        /// <summary>
        /// 根据人物编号查找家族族人
        /// </summary>
        /// <param name="personId">人物编号</param>
        /// <returns>找到的家族族人数据，如果未找到则返回null</returns>
        public static List<string> FindMemberById(string personId)
        {
            EnsureMemberQuExists();
            
            foreach (var member in Mainload.Member_qu)
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
        /// 根据姓名查找家族族人
        /// </summary>
        /// <param name="name">人物姓名</param>
        /// <returns>找到的家族族人数据列表，如果未找到则返回空列表</returns>
        public static List<List<string>> FindMemberByName(string name)
        {
            EnsureMemberQuExists();
            
            List<List<string>> result = new List<List<string>>();
            foreach (var member in Mainload.Member_qu)
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
        /// 添加新的家族族人到Mainload.Member_qu
        /// </summary>
        /// <param name="memberData">家族族人数据</param>
        /// <returns>添加是否成功</returns>
        public static bool AddMember(List<string> memberData)
        {
            try
            {
                EnsureMemberQuExists();
                
                if (memberData != null && memberData.Count >= 33) // 确保数据结构完整性
                {
                    Mainload.Member_qu.Add(memberData);
                    Debug.Log("已添加新家族族人: " + GetMemberName(memberData));
                    return true;
                }
                else
                {
                    Debug.LogError("添加家族族人失败：数据结构不完整");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("添加家族族人时发生错误: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取家族族人的姓名
        /// </summary>
        /// <param name="memberData">家族族人数据</param>
        /// <returns>家族族人姓名，如果无法解析则返回"未知姓名"</returns>
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
        /// 获取家族族人的简要信息字符串
        /// </summary>
        /// <param name="memberData">家族族人数据</param>
        /// <returns>家族族人简要信息</returns>
        public static string GetMemberSummary(List<string> memberData)
        {
            if (memberData == null || memberData.Count < 6)
            {
                return "无效的家族族人数据";
            }
            
            string name = GetMemberName(memberData);
            string personId = memberData[MemberIndexes.PERSON_ID];
            string age = memberData[MemberIndexes.AGE];
            string literature = memberData[MemberIndexes.LITERATURE];
            string martialArts = memberData[MemberIndexes.MARTIAL_ARTS];
            string business = memberData[MemberIndexes.BUSINESS];
            string art = memberData[MemberIndexes.ART];
            
            return $"{name} (ID: {personId}, 年龄: {age}) - 文: {literature}, 武: {martialArts}, 商: {business}, 艺: {art}";
        }
    }
}