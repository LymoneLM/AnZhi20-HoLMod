using UnityEngine;
using System.Collections.Generic;

namespace cs.HoLMod.Member_King_quData
{
    /// <summary>
    /// 皇室妻妾人物数据结构及工具类
    /// </summary>
    public class MemberKingQuData
    {
        /// <summary>
        /// 皇室妻妾人物状态常量定义
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
        /// 皇室妻妾人物数据数组索引定义
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
            public const int UNKNOWN_9 = 9;          // ？
            public const int UNKNOWN_10 = 10;        // 声誉
            public const int UNKNOWN_11 = 11;        // ？
            public const int UNKNOWN_12 = 12;        // 好感度
            public const int UNKNOWN_13 = 13;        // 怀孕月份，-1表示未怀孕，1-10代表怀胎10-1月
            public const int UNKNOWN_14 = 14;        // ？
            public const int UNKNOWN_15 = 15;        // 魅力
            public const int UNKNOWN_16 = 16;        // 健康
            public const int UNKNOWN_17 = 17;        // 计谋
            public const int UNKNOWN_18 = 18;        // 官职功勋
            public const int UNKNOWN_19 = 19;        // 每月成长
            public const int UNKNOWN_20 = 20;        // ？
            public const int UNKNOWN_21 = 21;        // 夫妻关系
            public const int UNKNOWN_22 = 22;        // 生平记事，结构为A@B@C@D(A代表事件的年龄，B代表事件编号，C和D根据B有相应的值没有值的时候为null)，多个事件之间用|分隔
            public const int UNKNOWN_23 = 23;        // 子嗣人物编号，多个子嗣用|分隔
            public const int UNKNOWN_24 = 24;        // ？
            public const int UNKNOWN_25 = 25;        // ？
            public const int UNKNOWN_26 = 26;        // ？
        }

        /// <summary>
        /// 检查Mainload.Member_King_qu是否存在并初始化（如果不存在）
        /// </summary>
        public static void EnsureMemberKingQuExists()
        {
            try
            {
                // 尝试访问Mainload.Member_King_qu，如果不存在则创建
                if (Mainload.Member_King_qu == null)
                {
                    Mainload.Member_King_qu = new List<List<string>>();
                    Debug.Log("已初始化Mainload.Member_King_qu");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("初始化Mainload.Member_King_qu时发生错误: " + ex.Message);
            }
        }

        /// <summary>
        /// 创建一个新的皇室妻妾数据实例
        /// </summary>
        /// <param name="personId">人物编号</param>
        /// <param name="appearance">人物形象 (后发|身体|脸部|前发)</param>
        /// <param name="personData">人物姓名|？|天赋|天赋点|性别|寿命|技能|幸运|品性|主人人物编号|爱好|null</param>
        /// <param name="unknown3">年龄</param>
        /// <param name="unknown4">文</param>
        /// <param name="unknown5">武</param>
        /// <param name="unknown6">商</param>
        /// <param name="unknown7">艺</param>
        /// <param name="unknown8">心情</param>
        /// <param name="unknown9">？</param>
        /// <param name="unknown10">声誉</param>
        /// <param name="unknown11">？</param>
        /// <param name="unknown12">好感度</param>
        /// <param name="unknown13">怀孕月份，-1表示未怀孕，1-10代表怀胎10-1月</param>
        /// <param name="unknown14">？</param>
        /// <param name="unknown15">魅力</param>
        /// <param name="unknown16">健康</param>
        /// <param name="unknown17">计谋</param>
        /// <param name="unknown18">官职功勋</param>
        /// <param name="unknown19">每月成长</param>
        /// <param name="unknown20">？</param>
        /// <param name="unknown21">夫妻关系</param>
        /// <param name="unknown22">生平记事，结构为A@B@C@D(A代表事件的年龄，B代表事件编号，C和D根据B有相应的值没有值的时候为null)，多个事件之间用|分隔</param>
        /// <param name="unknown23">子嗣人物编号，多个子嗣用|分隔</param>
        /// <param name="unknown24">？</param>
        /// <param name="unknown25">？</param>
        /// <param name="unknown26">？</param>
        /// <returns>皇室妻妾数据列表</returns>
        public static List<string> CreateMemberInstance(
            string personId = "",
            string appearance = "0|0|0|0",
            string personData = "|-100|0|0|0|0|0|0|M0|0|null",
            string unknown3 = "0",
            string unknown4 = "0",
            string unknown5 = "0",
            string unknown6 = "0",
            string unknown7 = "0",
            string unknown8 = "100",
            string unknown9 = "0",
            string unknown10 = "0",
            string unknown11 = "-1",
            string unknown12 = "null",
            string unknown13 = "null",
            string unknown14 = "0",
            string unknown15 = "100",
            string unknown16 = "100",
            string unknown17 = "1@5@1@-1@-1|0",
            string unknown18 = "0|0|0|0|0|0|0",
            string unknown19 = "0",
            string unknown20 = "100|100",
            string unknown21 = "22@81@您家族@赵安民",
            string unknown22 = "M1964|M2059",
            string unknown23 = "null",
            string unknown24 = "6",
            string unknown25 = "null",
            string unknown26 = "null")
        {
            return new List<string>
            {
                personId,
                appearance,
                personData,
                unknown3,
                unknown4,
                unknown5,
                unknown6,
                unknown7,
                unknown8,
                unknown9,
                unknown10,
                unknown11,
                unknown12,
                unknown13,
                unknown14,
                unknown15,
                unknown16,
                unknown17,
                unknown18,
                unknown19,
                unknown20,
                unknown21,
                unknown22,
                unknown23,
                unknown24,
                unknown25,
                unknown26
            };
        }

        /// <summary>
        /// 加载Mainload.Member_King_qu数据为皇室妻妾的数据数组
        /// </summary>
        /// <returns>皇室妻妾数据数组</returns>
        public static List<List<string>> LoadMemberData()
        {
            EnsureMemberKingQuExists();
            return Mainload.Member_King_qu;
        }

        /// <summary>
        /// 根据索引获取皇室妻妾数据
        /// </summary>
        /// <param name="index">皇室妻妾索引</param>
        /// <returns>皇室妻妾数据，如果索引无效则返回null</returns>
        public static List<string> GetMemberByIndex(int index)
        {
            EnsureMemberKingQuExists();
            
            if (index >= 0 && index < Mainload.Member_King_qu.Count)
            {
                return Mainload.Member_King_qu[index];
            }
            return null;
        }

        /// <summary>
        /// 根据人物编号查找皇室妻妾
        /// </summary>
        /// <param name="personId">人物编号</param>
        /// <returns>找到的皇室妻妾数据，如果未找到则返回null</returns>
        public static List<string> FindMemberById(string personId)
        {
            EnsureMemberKingQuExists();
            
            foreach (var member in Mainload.Member_King_qu)
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
        /// 根据姓名查找皇室妻妾
        /// </summary>
        /// <param name="name">人物姓名</param>
        /// <returns>找到的皇室妻妾数据列表，如果未找到则返回空列表</returns>
        public static List<List<string>> FindMemberByName(string name)
        {
            EnsureMemberKingQuExists();
            
            List<List<string>> result = new List<List<string>>();
            foreach (var member in Mainload.Member_King_qu)
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
        /// 添加新的皇室妻妾到Mainload.Member_King_qu
        /// </summary>
        /// <param name="memberData">皇室妻妾数据</param>
        /// <returns>添加是否成功</returns>
        public static bool AddMember(List<string> memberData)
        {
            try
            {
                EnsureMemberKingQuExists();
                
                if (memberData != null && memberData.Count >= 27) // 确保数据结构完整性
                {
                    Mainload.Member_King_qu.Add(memberData);
                    Debug.Log("已添加新皇室妻妾: " + GetMemberName(memberData));
                    return true;
                }
                else
                {
                    Debug.LogError("添加皇室妻妾失败：数据结构不完整");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("添加皇室妻妾时发生错误: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取皇室妻妾的姓名
        /// </summary>
        /// <param name="memberData">皇室妻妾数据</param>
        /// <returns>皇室妻妾姓名，如果无法解析则返回"未知姓名"</returns>
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
        /// 获取皇室妻妾的简要信息字符串
        /// </summary>
        /// <param name="memberData">皇室妻妾数据</param>
        /// <returns>皇室妻妾简要信息</returns>
        public static string GetMemberSummary(List<string> memberData)
        {
            if (memberData == null || memberData.Count < 8)
            {
                return "无效的皇室妻妾数据";
            }
            
            string name = GetMemberName(memberData);
            string personId = memberData[MemberIndexes.PERSON_ID];
            string age = memberData[MemberIndexes.UNKNOWN_3];
            string literature = memberData[MemberIndexes.UNKNOWN_4];
            string martialArts = memberData[MemberIndexes.UNKNOWN_5];
            string business = memberData[MemberIndexes.UNKNOWN_6];
            string art = memberData[MemberIndexes.UNKNOWN_7];
            string mood = memberData[MemberIndexes.UNKNOWN_8];
            
            return $"{name} (ID: {personId}, 年龄: {age}) - 文: {literature}, 武: {martialArts}, 商: {business}, 艺: {art}, 心情: {mood}";
        }

        /// <summary>
        /// 获取皇室妻妾的年龄
        /// </summary>
        /// <param name="memberData">皇室妻妾数据</param>
        /// <returns>年龄，如果无法获取则返回"0"</returns>
        public static string GetMemberAge(List<string> memberData)
        {
            if (memberData != null && memberData.Count > MemberIndexes.UNKNOWN_3)
            {
                return memberData[MemberIndexes.UNKNOWN_3];
            }
            return "0";
        }

        /// <summary>
        /// 检查皇室妻妾是否怀孕
        /// </summary>
        /// <param name="memberData">皇室妻妾数据</param>
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
        /// 获取皇室妻妾的怀孕月份
        /// </summary>
        /// <param name="memberData">皇室妻妾数据</param>
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
        /// 获取皇室妻妾的子嗣列表
        /// </summary>
        /// <param name="memberData">皇室妻妾数据</param>
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