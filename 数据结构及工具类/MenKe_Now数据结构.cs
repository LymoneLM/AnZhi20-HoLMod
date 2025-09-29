using UnityEngine;
using System.Collections.Generic;

namespace cs.HoLMod.MenKe_NowData
{
    /// <summary>
    /// 门客数据结构及工具类
    /// </summary>
    public class MenKeData
    {
        /// <summary>
        /// 门客状态常量定义
        /// </summary>
        public class MenKeStatus
        {
            public const string NORMAL = "0"; // 正常状态
            // 可以在这里添加其他状态常量，当明确其他状态含义时
        }

        /// <summary>
        /// 门客数据数组索引定义
        /// </summary>
        public class MenKeIndexes
        {
            public const int ID = 0;               // 人物编号
            public const int APPEARANCE = 1;       // 人物形象 (后发|身体|脸部|前发)
            public const int PERSON_DATA = 2;      // 人物数据 (姓名|？|天赋|天赋点|性别|寿命|技能|幸运|性格|null)
            public const int AGE = 3;              // 年龄
            public const int LITERARY = 4;         // 文
            public const int MARTIAL = 5;          // 武
            public const int BUSINESS = 6;         // 商
            public const int ART = 7;              // 艺
            public const int MOOD = 8;             // 心情
            public const int TEACHING_INFO = 9;    // 建筑教学信息 (0|所在建筑编号（没有时为null）|null)
            public const int STATUS = 10;          // 状态 (通常为0)
            public const int REPUTATION = 11;      // 声誉
            public const int UNKNOWN_12 = 12;      // ？ (默认为0就好)
            public const int CHARM = 13;           // 魅力
            public const int HEALTH = 14;          // 健康
            public const int STRATEGY = 15;        // 计谋
            public const int SKILL_POINTS = 16;    // 技能点
            public const int PREGNANCY = 17;       // 怀孕月份 (-1为未怀孕，刚怀孕为10)
            public const int SALARY = 18;          // 门客工资
            public const int STAMINA = 19;         // 体力
            public const int UNKNOWN_20 = 20;      // ？ (默认为0就好)
            public const int SPECIAL_TAG = 21;     // 特殊标签
        }

        /// <summary>
        /// 检查Mainload.MenKe_Now是否存在并初始化（如果不存在）
        /// </summary>
        public static void EnsureMenKeNowExists()
        {
            try
            {
                // 尝试访问Mainload.MenKe_Now，如果不存在则创建
                if (Mainload.MenKe_Now == null)
                {
                    Mainload.MenKe_Now = new List<List<string>>();
                    Debug.Log("已初始化Mainload.MenKe_Now");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("初始化Mainload.MenKe_Now时发生错误: " + ex.Message);
            }
        }

        /// <summary>
        /// 创建一个新的门客数据实例
        /// </summary>
        /// <param name="id">人物编号</param>
        /// <param name="appearance">人物形象 (后发|身体|脸部|前发)</param>
        /// <param name="personData">人物数据 (姓名|？|天赋|天赋点|性别|寿命|技能|幸运|性格|null)</param>
        /// <param name="age">年龄</param>
        /// <param name="literary">文</param>
        /// <param name="martial">武</param>
        /// <param name="business">商</param>
        /// <param name="art">艺</param>
        /// <param name="mood">心情</param>
        /// <param name="teachingInfo">建筑教学信息 (0|所在建筑编号（没有时为null）|null)</param>
        /// <param name="status">状态 (通常为0)</param>
        /// <param name="reputation">声誉</param>
        /// <param name="unknown12">？ (默认为0就好)</param>
        /// <param name="charm">魅力</param>
        /// <param name="health">健康</param>
        /// <param name="strategy">计谋</param>
        /// <param name="skillPoints">技能点</param>
        /// <param name="pregnancy">怀孕月份 (-1为未怀孕，刚怀孕为10)</param>
        /// <param name="salary">门客工资</param>
        /// <param name="stamina">体力</param>
        /// <param name="unknown20">？ (默认为0就好)</param>
        /// <param name="specialTag">特殊标签</param>
        /// <returns>门客数据列表</returns>
        public static List<string> CreateMenKeInstance(
            string id = "",
            string appearance = "0|0|0|0",
            string personData = "未知|0|0|0|0|0|0|0|0|null",
            string age = "0",
            string literary = "0",
            string martial = "0",
            string business = "0",
            string art = "0",
            string mood = "100",
            string teachingInfo = "0|null|null",
            string status = "0",
            string reputation = "0",
            string unknown12 = "0",
            string charm = "0",
            string health = "100",
            string strategy = "0",
            string skillPoints = "0",
            string pregnancy = "-1",
            string salary = "0",
            string stamina = "100",
            string unknown20 = "0",
            string specialTag = "null")
        {
            return new List<string>
            {
                id,               // 0: 人物编号
                appearance,       // 1: 人物形象 (后发|身体|脸部|前发)
                personData,       // 2: 人物数据 (姓名|？|天赋|天赋点|性别|寿命|技能|幸运|性格|null)
                age,              // 3: 年龄
                literary,         // 4: 文
                martial,          // 5: 武
                business,         // 6: 商
                art,              // 7: 艺
                mood,             // 8: 心情
                teachingInfo,     // 9: 建筑教学信息 (0|所在建筑编号（没有时为null）|null)
                status,           // 10: 状态 (通常为0)
                reputation,       // 11: 声誉
                unknown12,        // 12: ？ (默认为0就好)
                charm,            // 13: 魅力
                health,           // 14: 健康
                strategy,         // 15: 计谋
                skillPoints,      // 16: 技能点
                pregnancy,        // 17: 怀孕月份 (-1为未怀孕，刚怀孕为10)
                salary,           // 18: 门客工资
                stamina,          // 19: 体力
                unknown20,        // 20: ？ (默认为0就好)
                specialTag        // 21: 特殊标签
            };
        }

        /// <summary>
        /// 根据索引获取门客数据
        /// </summary>
        /// <param name="index">门客索引</param>
        /// <returns>门客数据，如果索引无效则返回null</returns>
        public static List<string> GetMenKeByIndex(int index)
        {
            EnsureMenKeNowExists();
            
            if (index >= 0 && index < Mainload.MenKe_Now.Count)
            {
                return Mainload.MenKe_Now[index];
            }
            return null;
        }

        /// <summary>
        /// 根据姓名查找门客
        /// </summary>
        /// <param name="name">门客姓名</param>
        /// <returns>找到的门客数据列表，如果未找到则返回空列表</returns>
        public static List<List<string>> FindMenKeByName(string name)
        {
            EnsureMenKeNowExists();
            
            List<List<string>> result = new List<List<string>>();
            foreach (var menKe in Mainload.MenKe_Now)
            {
                if (menKe != null && menKe.Count > MenKeIndexes.PERSON_DATA)
                {
                    string personData = menKe[MenKeIndexes.PERSON_DATA];
                    if (!string.IsNullOrEmpty(personData))
                    {
                        string[] parts = personData.Split('|');
                        if (parts.Length > 0 && parts[0] == name)
                        {
                            result.Add(menKe);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 添加新的门客到Mainload.MenKe_Now
        /// </summary>
        /// <param name="menKeData">门客数据</param>
        /// <returns>添加是否成功</returns>
        public static bool AddMenKe(List<string> menKeData)
        {
            try
            {
                EnsureMenKeNowExists();
                
                if (menKeData != null && menKeData.Count >= 22) // 确保数据结构完整性
                {
                    Mainload.MenKe_Now.Add(menKeData);
                    // 尝试获取门客姓名进行日志记录
                    string name = "未知姓名";
                    string personData = menKeData[MenKeIndexes.PERSON_DATA];
                    if (!string.IsNullOrEmpty(personData))
                    {
                        string[] parts = personData.Split('|');
                        if (parts.Length > 0)
                        {
                            name = parts[0];
                        }
                    }
                    Debug.Log("已添加新门客: " + name);
                    return true;
                }
                else
                {
                    Debug.LogError("添加门客失败：数据结构不完整");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("添加门客时发生错误: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取门客的简要信息字符串
        /// </summary>
        /// <param name="menKeData">门客数据</param>
        /// <returns>门客简要信息</returns>
        public static string GetMenKeSummary(List<string> menKeData)
        {
            if (menKeData == null || menKeData.Count < 22)
            {
                return "无效的门客数据";
            }
            
            // 获取姓名
            string name = "未知姓名";
            string personData = menKeData[MenKeIndexes.PERSON_DATA];
            if (!string.IsNullOrEmpty(personData))
            {
                string[] parts = personData.Split('|');
                if (parts.Length > 0)
                {
                    name = parts[0];
                }
            }
            
            // 获取主要属性
            string age = menKeData[MenKeIndexes.AGE];
            string literary = menKeData[MenKeIndexes.LITERARY];
            string martial = menKeData[MenKeIndexes.MARTIAL];
            string reputation = menKeData[MenKeIndexes.REPUTATION];
            string salary = menKeData[MenKeIndexes.SALARY];
            
            return $"{name} (年龄: {age}, 文: {literary}, 武: {martial}, 声誉: {reputation}, 工资: {salary})";
        }

        /// <summary>
        /// 加载门客数据并解析为结构化的门客数组
        /// </summary>
        /// <returns>门客数据数组</returns>
        public static List<List<string>> LoadMenKeData()
        {
            EnsureMenKeNowExists();
            return Mainload.MenKe_Now;
        }

        /// <summary>
        /// 获取指定门客的姓名
        /// </summary>
        /// <param name="menKeData">门客数据</param>
        /// <returns>门客姓名</returns>
        public static string GetMenKeName(List<string> menKeData)
        {
            if (menKeData != null && menKeData.Count > MenKeIndexes.PERSON_DATA)
            {
                string personData = menKeData[MenKeIndexes.PERSON_DATA];
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
    }
}