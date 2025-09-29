using System.Collections.Generic;

namespace cs.HoLMod.Member_Other_quData
{
    /// <summary>
    /// 世家妻妾成员数据结构及工具类
    /// </summary>
    public class Member_Other_quData
    {
        /// <summary>
        /// 世家妻妾成员状态常量定义
        /// </summary>
        public class Member_Other_quStatus
        {
            // 可以在这里添加状态常量，当明确各种状态含义时
        }

        /// <summary>
        /// 世家妻妾成员数据数组索引定义
        /// </summary>
        public class Member_Other_quIndexes
        {
            public const int ID = 0;                        // 人物编号
            public const int APPEARANCE = 1;                // 人物形象 (后发|身体|脸部|前发)
            public const int PERSON_DATA = 2;               // 人物姓名|？|天赋|天赋点|性别|寿命|技能|幸运|品性|主人人物编号|爱好|父母人物编号
            public const int AGE = 3;                       // 年龄
            public const int LITERARY = 4;                  // 文
            public const int MARTIAL = 5;                   // 武
            public const int BUSINESS = 6;                  // 商
            public const int ART = 7;                       // 艺
            public const int MOOD = 8;                      // 心情
            public const int UNKNOWN_9 = 9;                 // 未知含义
            public const int REPUTATION = 10;               // 声誉
            public const int UNKNOWN_11 = 11;               // 未知含义
            public const int FAVORABILITY = 12;             // 好感度
            public const int PREGNANCY_MONTHS = 13;         // 怀孕月份
            public const int RECENT_EVENTS = 14;            // 近期记事
            public const int CHARM = 15;                    // 魅力
            public const int HEALTH = 16;                   // 健康
            public const int STRATEGY = 17;                 // 计谋
            public const int UNKNOWN_18 = 18;               // 未知含义
            public const int UNKNOWN_19 = 19;               // 未知含义
            public const int UNKNOWN_20 = 20;               // 未知含义
            public const int UNKNOWN_21 = 21;               // 未知含义
            public const int UNKNOWN_22 = 22;               // 未知含义
            public const int UNKNOWN_23 = 23;               // 未知含义
            public const int UNKNOWN_24 = 24;               // 未知含义
        }

        /// <summary>
        /// 检查Mainload.Member_Other_qu是否存在并初始化（如果不存在）
        /// </summary>
        public static void EnsureMember_Other_quExists()
        {
            try
            {
                // 尝试访问Mainload.Member_Other_qu，如果不存在则创建
                if (Mainload.Member_Other_qu == null)
                {
                    Mainload.Member_Other_qu = new List<List<List<string>>>();
                    Debug.Log("已初始化Mainload.Member_Other_qu");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("初始化Mainload.Member_Other_qu时发生错误: " + ex.Message);
            }
        }

        /// <summary>
        /// 创建一个新的世家妻妾成员数据实例
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
        /// <param name="unknown9">未知含义参数</param>
        /// <param name="reputation">声誉</param>
        /// <param name="unknown11">未知含义参数</param>
        /// <param name="favorability">好感度</param>
        /// <param name="pregnancyMonths">怀孕月份</param>
        /// <param name="recentEvents">近期记事</param>
        /// <param name="charm">魅力</param>
        /// <param name="health">健康</param>
        /// <param name="strategy">计谋</param>
        /// <param name="unknown18">未知含义参数</param>
        /// <param name="unknown19">未知含义参数</param>
        /// <param name="unknown20">未知含义参数</param>
        /// <param name="unknown21">未知含义参数</param>
        /// <param name="unknown22">未知含义参数</param>
        /// <param name="unknown23">未知含义参数</param>
        /// <param name="unknown24">未知含义参数</param>
        /// <returns>世家妻妾成员数据列表</returns>
        public static List<string> CreateMember_Other_quInstance(
            string id = "0",
            string appearance = "0|0|0|0",
            string personData = "|0|0|0|0|0|0|0|0|0|0|0|0",
            string age = "0",
            string literary = "0",
            string martial = "0",
            string business = "0",
            string art = "0",
            string mood = "0",
            string unknown9 = "0",
            string reputation = "0",
            string unknown11 = "0",
            string favorability = "0",
            string pregnancyMonths = "-1",
            string recentEvents = "",
            string charm = "0",
            string health = "0",
            string strategy = "0",
            string unknown18 = "0",
            string unknown19 = "0",
            string unknown20 = "0",
            string unknown21 = "0",
            string unknown22 = "0",
            string unknown23 = "0",
            string unknown24 = "0")
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
                unknown9,
                reputation,
                unknown11,
                favorability,
                pregnancyMonths,
                recentEvents,
                charm,
                health,
                strategy,
                unknown18,
                unknown19,
                unknown20,
                unknown21,
                unknown22,
                unknown23,
                unknown24
            };
        }

        /// <summary>
        /// 根据索引获取世家妻妾成员数据
        /// </summary>
        /// <param name="shiJiaIndex">世家索引</param>
        /// <param name="memberIndex">成员索引</param>
        /// <returns>世家妻妾成员数据，如果索引无效则返回null</returns>
        public static List<string> GetMember_Other_quByIndex(int shiJiaIndex, int memberIndex)
        {
            EnsureMember_Other_quExists();
            
            if (shiJiaIndex >= 0 && shiJiaIndex < Mainload.Member_Other_qu.Count)
            {
                var shiJiaMembers = Mainload.Member_Other_qu[shiJiaIndex];
                if (memberIndex >= 0 && memberIndex < shiJiaMembers.Count)
                {
                    return shiJiaMembers[memberIndex];
                }
            }
            return null;
        }

        /// <summary>
        /// 根据姓名查找世家妻妾成员
        /// </summary>
        /// <param name="shiJiaIndex">世家索引</param>
        /// <param name="name">成员姓名</param>
        /// <returns>找到的世家妻妾成员数据列表，如果未找到则返回空列表</returns>
        public static List<List<string>> FindMember_Other_quByName(int shiJiaIndex, string name)
        {
            EnsureMember_Other_quExists();
            
            List<List<string>> result = new List<List<string>>();
            
            if (shiJiaIndex >= 0 && shiJiaIndex < Mainload.Member_Other_qu.Count)
            {
                var shiJiaMembers = Mainload.Member_Other_qu[shiJiaIndex];
                foreach (var member in shiJiaMembers)
                {
                    if (member != null && member.Count > Member_Other_quIndexes.PERSON_DATA)
                    {
                        string personData = member[Member_Other_quIndexes.PERSON_DATA];
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
        /// 为指定世家添加新的妻妾成员
        /// </summary>
        /// <param name="shiJiaIndex">世家索引</param>
        /// <param name="memberData">成员数据</param>
        /// <returns>添加是否成功</returns>
        public static bool AddMember_Other_qu(int shiJiaIndex, List<string> memberData)
        {
            try
            {
                EnsureMember_Other_quExists();
                
                // 确保世家列表足够大
                while (Mainload.Member_Other_qu.Count <= shiJiaIndex)
                {
                    Mainload.Member_Other_qu.Add(new List<List<string>>());
                }
                
                if (memberData != null && memberData.Count >= 25) // 确保数据结构完整性
                {
                    Mainload.Member_Other_qu[shiJiaIndex].Add(memberData);
                    Debug.Log("已添加新世家妻妾成员");
                    return true;
                }
                else
                {
                    Debug.LogError("添加世家妻妾成员失败：数据结构不完整");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("添加世家妻妾成员时发生错误: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取世家妻妾成员的姓名
        /// </summary>
        /// <param name="memberData">成员数据</param>
        /// <returns>成员姓名，如果无法解析则返回"未知姓名"</returns>
        public static string GetMember_Other_quName(List<string> memberData)
        {
            if (memberData != null && memberData.Count > Member_Other_quIndexes.PERSON_DATA)
            {
                string personData = memberData[Member_Other_quIndexes.PERSON_DATA];
                string[] parts = personData.Split('|');
                if (parts.Length > 0 && !string.IsNullOrEmpty(parts[0]))
                {
                    return parts[0];
                }
            }
            return "未知姓名";
        }

        /// <summary>
        /// 获取世家妻妾成员的简要信息
        /// </summary>
        /// <param name="memberData">成员数据</param>
        /// <returns>成员简要信息</returns>
        public static string GetMember_Other_quSummary(List<string> memberData)
        {
            if (memberData == null || memberData.Count < 17)
            {
                return "无效的世家妻妾成员数据";
            }
            
            string name = GetMember_Other_quName(memberData);
            string age = memberData[Member_Other_quIndexes.AGE];
            string literary = memberData[Member_Other_quIndexes.LITERARY];
            string martial = memberData[Member_Other_quIndexes.MARTIAL];
            string business = memberData[Member_Other_quIndexes.BUSINESS];
            string art = memberData[Member_Other_quIndexes.ART];
            string mood = memberData[Member_Other_quIndexes.MOOD];
            string favorability = memberData[Member_Other_quIndexes.FAVORABILITY];
            string health = memberData[Member_Other_quIndexes.HEALTH];
            string charm = memberData[Member_Other_quIndexes.CHARM];
            
            string pregnancyStatus = "未怀孕";
            if (memberData.Count > Member_Other_quIndexes.PREGNANCY_MONTHS)
            {
                string pregnancyMonths = memberData[Member_Other_quIndexes.PREGNANCY_MONTHS];
                if (pregnancyMonths != "-1")
                {
                    pregnancyStatus = $"已怀孕（{pregnancyMonths}月）";
                }
            }
            
            return $"{name} (年龄: {age}, 四维: 文{literary} 武{martial} 商{business} 艺{art}, 心情: {mood}, 好感度: {favorability}, 魅力: {charm}, 健康: {health}, {pregnancyStatus})";
        }

        /// <summary>
        /// 加载世家妻妾成员数据
        /// </summary>
        /// <param name="shiJiaIndex">世家索引</param>
        /// <returns>该世家的所有妻妾成员数据</returns>
        public static List<List<string>> LoadMember_Other_quData(int shiJiaIndex)
        {
            EnsureMember_Other_quExists();
            
            if (shiJiaIndex >= 0 && shiJiaIndex < Mainload.Member_Other_qu.Count)
            {
                return Mainload.Member_Other_qu[shiJiaIndex];
            }
            return new List<List<string>>();
        }

        /// <summary>
        /// 获取世家妻妾成员的主人编号
        /// </summary>
        /// <param name="memberData">成员数据</param>
        /// <returns>主人编号，如果无法解析则返回"0"</returns>
        public static string GetMember_Other_quMasterId(List<string> memberData)
        {
            if (memberData != null && memberData.Count > Member_Other_quIndexes.PERSON_DATA)
            {
                string personData = memberData[Member_Other_quIndexes.PERSON_DATA];
                string[] parts = personData.Split('|');
                if (parts.Length > 9)
                {
                    return parts[9];
                }
            }
            return "0";
        }

        /// <summary>
        /// 设置世家妻妾成员的好感度
        /// </summary>
        /// <param name="memberData">成员数据</param>
        /// <param name="favorability">新的好感度值</param>
        public static void SetMember_Other_quFavorability(List<string> memberData, string favorability)
        {
            if (memberData != null && memberData.Count > Member_Other_quIndexes.FAVORABILITY)
            {
                memberData[Member_Other_quIndexes.FAVORABILITY] = favorability;
            }
        }

        /// <summary>
        /// 设置世家妻妾成员的怀孕状态
        /// </summary>
        /// <param name="memberData">成员数据</param>
        /// <param name="pregnancyMonths">怀孕月份（-1表示未怀孕）</param>
        public static void SetMember_Other_quPregnancy(List<string> memberData, string pregnancyMonths)
        {
            if (memberData != null && memberData.Count > Member_Other_quIndexes.PREGNANCY_MONTHS)
            {
                memberData[Member_Other_quIndexes.PREGNANCY_MONTHS] = pregnancyMonths;
            }
        }
    }
}