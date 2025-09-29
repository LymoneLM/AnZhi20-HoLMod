using UnityEngine;
using System.Collections.Generic;

namespace cs.HoLMod.Member_nowData
{
    /// <summary>
    /// 家族族人数据结构及工具类
    /// </summary>
    public class MemberData
    {
        /// <summary>
        /// 家族族人状态常量定义
        /// </summary>
        public class MemberStatus
        {
            // 可以在这里添加状态常量，当明确各种状态含义时
        }

        /// <summary>
        /// 家族族人数据数组索引定义
        /// </summary>
        public class MemberIndexes
        {
            public const int ID = 0;                        // 人物编号
            public const int APPEARANCE = 1;                // 人物形象 (后发|身体|脸部|前发)
            public const int CHILDREN = 2;                  // 子嗣的人物编号 (一个元素就是1个子嗣)
            public const int HOUSING = 3;                   // 住房数据 (1代表正房)|房子的建筑编号|null（一般为null）|房子一共住了多少人
            public const int PERSON_DATA = 4;               // 人物数据 (姓名|第几代|天赋|天赋点|性别|寿命|技能|幸运|？|喜好)
            public const int CHARACTER = 5;                 // 性格（品性）
            public const int AGE = 6;                       // 年龄
            public const int LITERARY = 7;                  // 文
            public const int MARTIAL = 8;                   // 武
            public const int BUSINESS = 9;                  // 商
            public const int ART = 10;                      // 艺
            public const int MOOD = 11;                     // 心情
            public const int OFFICIAL_TITLE = 12;           // 官职功勋 (@作为分割符，0@0（这两个代表身份）@-1（根据身份对应职位）@-1（郡城编号）@-1(县城编号)|政绩值)
            public const int MERITS = 13;                   // 功名
            public const int TITLE_FENGDI = 14;             // 爵位 (0（爵位，0平民，1伯爵，2侯爵，3公爵）|0（封地所在郡的编号）)
            public const int STATUS = 15;                   // 状态
            public const int REPUTATION = 16;               // 声誉
            public const int STATUS_DURATION = 17;          // 状态持续时间
            public const int UNKNOWN_18 = 18;               // ？ (一般不修改)
            public const int STUDY_CONTENT = 19;            // 研习内容 (研习的书@研习进度，没有研习时为null)
            public const int CHARM = 20;                    // 魅力
            public const int HEALTH = 21;                   // 健康
            public const int HEAD_OF_FAMILY = 22;           // 家主判断标识符 (1为家主，0为非家主)
            public const int SPECIAL_TAG = 23;              // 特殊标签
            public const int RECENT_EVENTS = 24;            // 近期记事 (没有时为null)
            public const int PREGNANCY_MONTHS = 25;         // 怀孕月份 (-1为未怀孕，刚怀孕为10（剩余生产时间）)
            public const int MARITAL_STATUS = 26;           // 婚姻状态 (0为未婚，1为已婚，2为丧偶，3为再婚，4为夺妻，5为被休，6为离异)
            public const int STRATEGY = 27;                 // 计谋
            public const int UNKNOWN_28 = 28;               // ？ (一般不修改)
            public const int EQUIPMENT = 29;                // 装备 (武器（物品编号）|珠宝（物品编号）|符咒（物品编号）)
            public const int STAMINA = 30;                  // 体力
            public const int MONTHLY_INCREMENT = 31;        // 每月增加值 (每月增加的？|？|？|文|武|商|艺)
            public const int UNKNOWN_32 = 32;               // ？ (一般不修改)
            public const int SKILL_POINTS = 33;             // 技能点
            public const int PREGNANCY_PROBABILITY = 34;    // 怀孕概率 (数值越高怀孕概率越低，大于等于100不孕不育)
            public const int UNKNOWN_35 = 35;               // ？ (一般不修改)
            public const int LIFE_EVENTS = 36;              // 生平记事 (没有的时候为null，多个生平记事时用|分隔)
            public const int UNKNOWN_37 = 37;               // ？ (一般不修改)
            public const int UNKNOWN_38 = 38;               // ？ (一般不修改)
            public const int UNKNOWN_39 = 39;               // ？ (一般不修改)
            public const int SCHOOL = 40;                   // 学派 (0为无，1为明理学派，2为九渊学派，3为今文学派)
            public const int CLAN_RESPONSIBILITIES = 41;    // 族人职责
            public const int UNKNOWN_42 = 42;               // ？ (一般不修改)
        }

        /// <summary>
        /// 检查Mainload.Member_now是否存在并初始化（如果不存在）
        /// </summary>
        public static void EnsureMemberNowExists()
        {
            try
            {
                // 尝试访问Mainload.Member_now，如果不存在则创建
                if (Mainload.Member_now == null)
                {
                    Mainload.Member_now = new List<List<string>>();
                    Debug.Log("已初始化Mainload.Member_now");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("初始化Mainload.Member_now时发生错误: " + ex.Message);
            }
        }

        /// <summary>
        /// 创建一个新的家族族人数据实例
        /// </summary>
        /// <param name="id">人物编号</param>
        /// <param name="appearance">人物形象 (后发|身体|脸部|前发)</param>
        /// <param name="children">子嗣的人物编号 (一个元素就是1个子嗣)</param>
        /// <param name="housing">住房数据</param>
        /// <param name="personData">人物数据</param>
        /// <param name="character">性格（品性）</param>
        /// <param name="age">年龄</param>
        /// <param name="literary">文</param>
        /// <param name="martial">武</param>
        /// <param name="business">商</param>
        /// <param name="art">艺</param>
        /// <param name="mood">心情</param>
        /// <param name="officialTitle">官职功勋</param>
        /// <param name="merits">功名</param>
        /// <param name="titleFengdi">爵位</param>
        /// <param name="status">状态</param>
        /// <param name="reputation">声誉</param>
        /// <param name="statusDuration">状态持续时间</param>
        /// <param name="unknown18">？</param>
        /// <param name="studyContent">研习内容</param>
        /// <param name="charm">魅力</param>
        /// <param name="health">健康</param>
        /// <param name="headOfFamily">家主判断标识符</param>
        /// <param name="specialTag">特殊标签</param>
        /// <param name="recentEvents">近期记事</param>
        /// <param name="pregnancyMonths">怀孕月份</param>
        /// <param name="maritalStatus">婚姻状态</param>
        /// <param name="strategy">计谋</param>
        /// <param name="unknown28">？</param>
        /// <param name="equipment">装备</param>
        /// <param name="stamina">体力</param>
        /// <param name="monthlyIncrement">每月增加值</param>
        /// <param name="unknown32">？</param>
        /// <param name="skillPoints">技能点</param>
        /// <param name="pregnancyProbability">怀孕概率</param>
        /// <param name="unknown35">？</param>
        /// <param name="lifeEvents">生平记事</param>
        /// <param name="unknown37">？</param>
        /// <param name="unknown38">？</param>
        /// <param name="unknown39">？</param>
        /// <param name="school">学派</param>
        /// <param name="clanResponsibilities">族人职责</param>
        /// <param name="unknown42">？</param>
        /// <returns>家族族人数据列表</returns>
        public static List<string> CreateMemberInstance(
            string id = "",
            string appearance = "0|0|0|0",
            string children = "",
            string housing = "1|LTB0|null|1",
            string personData = "未知|0|0|0|0|0|0|0|0|0",
            string character = "0",
            string age = "0",
            string literary = "0",
            string martial = "0",
            string business = "0",
            string art = "0",
            string mood = "100",
            string officialTitle = "0@0@-1@-1@-1|0",
            string merits = "0",
            string titleFengdi = "0|0",
            string status = "0",
            string reputation = "0",
            string statusDuration = "0",
            string unknown18 = "0",
            string studyContent = "null",
            string charm = "0",
            string health = "100",
            string headOfFamily = "0",
            string specialTag = "null",
            string recentEvents = "null",
            string pregnancyMonths = "-1",
            string maritalStatus = "0",
            string strategy = "0",
            string unknown28 = "0",
            string equipment = "0|0|null",
            string stamina = "100",
            string monthlyIncrement = "0|0|0|0|0|0|0",
            string unknown32 = "0|0|0|0|0",
            string skillPoints = "0",
            string pregnancyProbability = "0",
            string unknown35 = "0",
            string lifeEvents = "null",
            string unknown37 = "null",
            string unknown38 = "0",
            string unknown39 = "0",
            string school = "0",
            string clanResponsibilities = "0|0|0",
            string unknown42 = "0|0")
        {
            return new List<string>
            {
                id,                // 0: 人物编号
                appearance,        // 1: 人物形象 (后发|身体|脸部|前发)
                children,          // 2: 子嗣的人物编号 (一个元素就是1个子嗣)
                housing,           // 3: 住房数据
                personData,        // 4: 人物数据 (姓名|第几代|天赋|天赋点|性别|寿命|技能|幸运|？|喜好)
                character,         // 5: 性格（品性）
                age,               // 6: 年龄
                literary,          // 7: 文
                martial,           // 8: 武
                business,          // 9: 商
                art,               // 10: 艺
                mood,              // 11: 心情
                officialTitle,     // 12: 官职功勋
                merits,            // 13: 功名
                titleFengdi,       // 14: 爵位
                status,            // 15: 状态
                reputation,        // 16: 声誉
                statusDuration,    // 17: 状态持续时间
                unknown18,         // 18: ？
                studyContent,      // 19: 研习内容
                charm,             // 20: 魅力
                health,            // 21: 健康
                headOfFamily,      // 22: 家主判断标识符
                specialTag,        // 23: 特殊标签
                recentEvents,      // 24: 近期记事
                pregnancyMonths,   // 25: 怀孕月份
                maritalStatus,     // 26: 婚姻状态
                strategy,          // 27: 计谋
                unknown28,         // 28: ？
                equipment,         // 29: 装备
                stamina,           // 30: 体力
                monthlyIncrement,  // 31: 每月增加值
                unknown32,         // 32: ？
                skillPoints,       // 33: 技能点
                pregnancyProbability, // 34: 怀孕概率
                unknown35,         // 35: ？
                lifeEvents,        // 36: 生平记事
                unknown37,         // 37: ？
                unknown38,         // 38: ？
                unknown39,         // 39: ？
                school,            // 40: 学派
                clanResponsibilities, // 41: 族人职责
                unknown42          // 42: ？
            };
        }

        /// <summary>
        /// 根据索引获取家族族人数据
        /// </summary>
        /// <param name="index">家族族人索引</param>
        /// <returns>家族族人数据，如果索引无效则返回null</returns>
        public static List<string> GetMemberByIndex(int index)
        {
            EnsureMemberNowExists();
            
            if (index >= 0 && index < Mainload.Member_now.Count)
            {
                return Mainload.Member_now[index];
            }
            return null;
        }

        /// <summary>
        /// 根据姓名查找家族族人
        /// </summary>
        /// <param name="name">家族族人姓名</param>
        /// <returns>找到的家族族人数据列表，如果未找到则返回空列表</returns>
        public static List<List<string>> FindMemberByName(string name)
        {
            EnsureMemberNowExists();
            
            List<List<string>> result = new List<List<string>>();
            foreach (var member in Mainload.Member_now)
            {
                if (member != null && member.Count > MemberIndexes.PERSON_DATA)
                {
                    string personData = member[MemberIndexes.PERSON_DATA];
                    if (!string.IsNullOrEmpty(personData))
                    {
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
        /// 添加新的家族族人到Mainload.Member_now
        /// </summary>
        /// <param name="memberData">家族族人数据</param>
        /// <returns>添加是否成功</returns>
        public static bool AddMember(List<string> memberData)
        {
            try
            {
                EnsureMemberNowExists();
                
                if (memberData != null && memberData.Count >= 43) // 确保数据结构完整性
                {
                    Mainload.Member_now.Add(memberData);
                    // 尝试获取族人姓名进行日志记录
                    string name = "未知姓名";
                    string personData = memberData[MemberIndexes.PERSON_DATA];
                    if (!string.IsNullOrEmpty(personData))
                    {
                        string[] parts = personData.Split('|');
                        if (parts.Length > 0)
                        {
                            name = parts[0];
                        }
                    }
                    Debug.Log("已添加新族人: " + name);
                    return true;
                }
                else
                {
                    Debug.LogError("添加族人失败：数据结构不完整");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("添加族人时发生错误: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取家族族人的简要信息字符串
        /// </summary>
        /// <param name="memberData">家族族人数据</param>
        /// <returns>家族族人简要信息</returns>
        public static string GetMemberSummary(List<string> memberData)
        {
            if (memberData == null || memberData.Count < 43)
            {
                return "无效的族人数据";
            }
            
            // 获取姓名
            string name = "未知姓名";
            string personData = memberData[MemberIndexes.PERSON_DATA];
            if (!string.IsNullOrEmpty(personData))
            {
                string[] parts = personData.Split('|');
                if (parts.Length > 0)
                {
                    name = parts[0];
                }
            }
            
            // 获取主要属性
            string age = memberData[MemberIndexes.AGE];
            string literary = memberData[MemberIndexes.LITERARY];
            string martial = memberData[MemberIndexes.MARTIAL];
            string reputation = memberData[MemberIndexes.REPUTATION];
            string health = memberData[MemberIndexes.HEALTH];
            string position = "平民";
            string[] titleFengdi = memberData[MemberIndexes.TITLE_FENGDI].Split('|');
            if (titleFengdi.Length > 0)
            {
                switch (titleFengdi[0])
                {
                    case "1": position = "伯爵";
                        break;
                    case "2": position = "侯爵";
                        break;
                    case "3": position = "公爵";
                        break;
                }
            }
            
            return $"{name} (年龄: {age}, 文: {literary}, 武: {martial}, 声誉: {reputation}, 健康: {health}, 职位: {position})";
        }

        /// <summary>
        /// 加载家族族人数据并解析为结构化的数组
        /// </summary>
        /// <returns>家族族人数据数组</returns>
        public static List<List<string>> LoadMemberData()
        {
            EnsureMemberNowExists();
            return Mainload.Member_now;
        }

        /// <summary>
        /// 获取指定家族族人的姓名
        /// </summary>
        /// <param name="memberData">家族族人数据</param>
        /// <returns>家族族人姓名</returns>
        public static string GetMemberName(List<string> memberData)
        {
            if (memberData != null && memberData.Count > MemberIndexes.PERSON_DATA)
            {
                string personData = memberData[MemberIndexes.PERSON_DATA];
                if (!string.IsNullOrEmpty(personData))
                {
                    string[] parts = personData.Split('|');
                    if (parts.Length > 0)
                    {
                        return parts[0];
                    }
                }
            }
            return "未知";
        }

        /// <summary>
        /// 获取指定家族族人的封地信息
        /// </summary>
        /// <param name="memberData">家族族人数据</param>
        /// <returns>封地所在郡的编号，如果没有封地则返回-1</returns>
        public static int GetFengDiJunIndex(List<string> memberData)
        {
            if (memberData != null && memberData.Count > MemberIndexes.TITLE_FENGDI)
            {
                string titleFengdiStr = memberData[MemberIndexes.TITLE_FENGDI];
                string[] parts = titleFengdiStr.Split('|');
                
                if (parts.Length >= 2 && int.TryParse(parts[1], out int junIndex))
                {
                    return junIndex;
                }
            }
            return -1;
        }
    }
}