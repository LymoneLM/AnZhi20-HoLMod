using UnityEngine;
using System.Collections.Generic;

namespace cs.HoLMod.ZhuangTou_nowData
{
    /// <summary>
    /// 庄头数据结构及工具类
    /// </summary>
    public class ZhuangTouData
    {
        /// <summary>
        /// 庄头数据数组索引定义
        /// </summary>
        public class ZhuangTouIndexes
        {
            public const int ID = 0;                   // 庄头的人物编号
            public const int APPEARANCE = 1;           // 庄头的人物形象(后发|身体|脸部|前发)
            public const int PERSON_INFO = 2;          // 人物信息（庄头姓名|？|？|品性）
            public const int AGE = 3;                  // 年龄
            public const int MANAGEMENT = 4;           // 管理
            public const int LOYALTY = 5;              // 忠诚
        }

        /// <summary>
        /// 检查Mainload.ZhuangTou_now是否存在并初始化（如果不存在）
        /// </summary>
        public static void EnsureZhuangTouNowExists()
        {
            try
            {
                // 尝试访问Mainload.ZhuangTou_now，如果不存在则创建
                if (Mainload.ZhuangTou_now == null)
                {
                    // 索引0对应大地图的所有庄头数组，索引1-12对应各郡（郡索引的0-11）的所有庄头数组
                    Mainload.ZhuangTou_now = new List<List<List<List<string>>>>();
                    // 初始化13个层级（索引0-12）
                    for (int i = 0; i < 13; i++)
                    {
                        Mainload.ZhuangTou_now.Add(new List<List<List<string>>>());
                    }
                    Debug.Log("已初始化Mainload.ZhuangTou_now");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("初始化Mainload.ZhuangTou_now时发生错误: " + ex.Message);
            }
        }

        /// <summary>
        /// 创建一个新的庄头数据实例
        /// </summary>
        /// <param name="id">庄头的人物编号</param>
        /// <param name="appearance">庄头的人物形象(后发|身体|脸部|前发)</param>
        /// <param name="personInfo">人物信息（庄头姓名|？|？|品性）</param>
        /// <param name="age">年龄</param>
        /// <param name="management">管理</param>
        /// <param name="loyalty">忠诚</param>
        /// <returns>庄头数据列表</returns>
        public static List<string> CreateZhuangTouInstance(
            string id = "0",
            string appearance = "0|0|0|0",
            string personInfo = "未知|0|0|0",
            string age = "0",
            string management = "0",
            string loyalty = "0")
        {
            List<string> zhuangTouData = new List<string>();
            // 填充0-5的索引，确保数组长度足够
            for (int i = 0; i < 6; i++)
            {
                zhuangTouData.Add("0"); // 默认值
            }

            zhuangTouData[ZhuangTouIndexes.ID] = id;                    // 0: 庄头的人物编号
            zhuangTouData[ZhuangTouIndexes.APPEARANCE] = appearance;    // 1: 庄头的人物形象(后发|身体|脸部|前发)
            zhuangTouData[ZhuangTouIndexes.PERSON_INFO] = personInfo;   // 2: 人物信息（庄头姓名|？|？|品性）
            zhuangTouData[ZhuangTouIndexes.AGE] = age;                  // 3: 年龄
            zhuangTouData[ZhuangTouIndexes.MANAGEMENT] = management;    // 4: 管理
            zhuangTouData[ZhuangTouIndexes.LOYALTY] = loyalty;          // 5: 忠诚

            return zhuangTouData;
        }

        /// <summary>
        /// 获取指定层级、农庄和索引的庄头数据
        /// </summary>
        /// <param name="levelIndex">层级索引（0:大地图, 1-12:各郡）</param>
        /// <param name="farmIndex">农庄索引</param>
        /// <param name="zhuangTouIndex">庄头索引</param>
        /// <returns>庄头数据，如果索引无效则返回null</returns>
        public static List<string> GetZhuangTouByIndex(int levelIndex, int farmIndex, int zhuangTouIndex)
        {
            EnsureZhuangTouNowExists();
            
            if (levelIndex >= 0 && levelIndex < Mainload.ZhuangTou_now.Count)
            {
                if (farmIndex >= 0 && farmIndex < Mainload.ZhuangTou_now[levelIndex].Count)
                {
                    if (zhuangTouIndex >= 0 && zhuangTouIndex < Mainload.ZhuangTou_now[levelIndex][farmIndex].Count)
                    {
                        return Mainload.ZhuangTou_now[levelIndex][farmIndex][zhuangTouIndex];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 添加新的庄头到指定层级和农庄
        /// </summary>
        /// <param name="levelIndex">层级索引（0:大地图, 1-12:各郡）</param>
        /// <param name="farmIndex">农庄索引</param>
        /// <param name="zhuangTouData">庄头数据</param>
        /// <returns>添加是否成功</returns>
        public static bool AddZhuangTou(int levelIndex, int farmIndex, List<string> zhuangTouData)
        {
            try
            {
                EnsureZhuangTouNowExists();
                
                if (levelIndex >= 0 && levelIndex < Mainload.ZhuangTou_now.Count)
                {
                    // 确保农庄层级存在
                    while (farmIndex >= Mainload.ZhuangTou_now[levelIndex].Count)
                    {
                        Mainload.ZhuangTou_now[levelIndex].Add(new List<List<string>>());
                    }
                    
                    if (zhuangTouData != null && zhuangTouData.Count >= 6) // 确保数据结构完整性
                    {
                        Mainload.ZhuangTou_now[levelIndex][farmIndex].Add(zhuangTouData);
                        // 尝试获取庄头名称进行日志记录
                        string name = GetZhuangTouName(zhuangTouData);
                        Debug.Log($"已添加新庄头: {name} (层级索引: {levelIndex}, 农庄索引: {farmIndex})");
                        return true;
                    }
                    else
                    {
                        Debug.LogError("添加庄头失败：数据结构不完整");
                        return false;
                    }
                }
                else
                {
                    Debug.LogError("添加庄头失败：层级索引无效");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("添加庄头时发生错误: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取指定庄头的简要信息字符串
        /// </summary>
        /// <param name="zhuangTouData">庄头数据</param>
        /// <returns>庄头简要信息</returns>
        public static string GetZhuangTouSummary(List<string> zhuangTouData)
        {
            if (zhuangTouData == null || zhuangTouData.Count < 6)
            {
                return "无效的庄头数据";
            }
            
            string name = GetZhuangTouName(zhuangTouData);
            string id = zhuangTouData[ZhuangTouIndexes.ID];
            string age = zhuangTouData[ZhuangTouIndexes.AGE];
            string management = zhuangTouData[ZhuangTouIndexes.MANAGEMENT];
            string loyalty = zhuangTouData[ZhuangTouIndexes.LOYALTY];
            
            return $"{name} (ID: {id}, 年龄: {age}, 管理: {management}, 忠诚: {loyalty})";
        }

        /// <summary>
        /// 加载指定层级和农庄的庄头数据
        /// </summary>
        /// <param name="levelIndex">层级索引（0:大地图, 1-12:各郡）</param>
        /// <param name="farmIndex">农庄索引</param>
        /// <returns>庄头数据数组</returns>
        public static List<List<string>> LoadZhuangTouDataByFarm(int levelIndex, int farmIndex)
        {
            EnsureZhuangTouNowExists();
            
            if (levelIndex >= 0 && levelIndex < Mainload.ZhuangTou_now.Count)
            {
                if (farmIndex >= 0 && farmIndex < Mainload.ZhuangTou_now[levelIndex].Count)
                {
                    return Mainload.ZhuangTou_now[levelIndex][farmIndex];
                }
            }
            return new List<List<string>>();
        }

        /// <summary>
        /// 获取指定庄头的名称
        /// </summary>
        /// <param name="zhuangTouData">庄头数据</param>
        /// <returns>庄头名称</returns>
        public static string GetZhuangTouName(List<string> zhuangTouData)
        {
            if (zhuangTouData != null && zhuangTouData.Count > ZhuangTouIndexes.PERSON_INFO)
            {
                string personInfo = zhuangTouData[ZhuangTouIndexes.PERSON_INFO];
                if (!string.IsNullOrEmpty(personInfo))
                {
                    string[] parts = personInfo.Split('|');
                    if (parts.Length > 0)
                    {
                        return parts[0];
                    }
                }
            }
            return "未知庄头";
        }

        /// <summary>
        /// 获取指定庄头的品性
        /// </summary>
        /// <param name="zhuangTouData">庄头数据</param>
        /// <returns>庄头品性</returns>
        public static string GetZhuangTouCharacter(List<string> zhuangTouData)
        {
            if (zhuangTouData != null && zhuangTouData.Count > ZhuangTouIndexes.PERSON_INFO)
            {
                string personInfo = zhuangTouData[ZhuangTouIndexes.PERSON_INFO];
                if (!string.IsNullOrEmpty(personInfo))
                {
                    string[] parts = personInfo.Split('|');
                    if (parts.Length > 3)
                    {
                        return parts[3];
                    }
                }
            }
            return "0";
        }

        /// <summary>
        /// 设置庄头的管理属性
        /// </summary>
        /// <param name="zhuangTouData">庄头数据</param>
        /// <param name="management">管理值</param>
        public static void SetZhuangTouManagement(List<string> zhuangTouData, string management)
        {
            if (zhuangTouData != null && zhuangTouData.Count > ZhuangTouIndexes.MANAGEMENT)
            {
                zhuangTouData[ZhuangTouIndexes.MANAGEMENT] = management;
            }
        }

        /// <summary>
        /// 设置庄头的忠诚属性
        /// </summary>
        /// <param name="zhuangTouData">庄头数据</param>
        /// <param name="loyalty">忠诚值</param>
        public static void SetZhuangTouLoyalty(List<string> zhuangTouData, string loyalty)
        {
            if (zhuangTouData != null && zhuangTouData.Count > ZhuangTouIndexes.LOYALTY)
            {
                zhuangTouData[ZhuangTouIndexes.LOYALTY] = loyalty;
            }
        }

        /// <summary>
        /// 查找指定ID的庄头
        /// </summary>
        /// <param name="id">庄头ID</param>
        /// <returns>找到的庄头数据列表，如果未找到则返回空列表</returns>
        public static List<List<string>> FindZhuangTouById(string id)
        {
            EnsureZhuangTouNowExists();
            
            List<List<string>> result = new List<List<string>>();
            
            foreach (var level in Mainload.ZhuangTou_now)
            {
                foreach (var farm in level)
                {
                    foreach (var zhuangTou in farm)
                    {
                        if (zhuangTou != null && zhuangTou.Count > ZhuangTouIndexes.ID && zhuangTou[ZhuangTouIndexes.ID] == id)
                        {
                            result.Add(zhuangTou);
                        }
                    }
                }
            }
            
            return result;
        }
    }
}