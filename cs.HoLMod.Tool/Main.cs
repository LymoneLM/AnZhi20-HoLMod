using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace cs.HoLMod.Tool
{
    // 插件信息类
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "cs.HoLMod.Tool.AnZhi20";
        public const string PLUGIN_NAME = "HoLMod.Tool";
        public const string PLUGIN_VERSION = "1.0.0";
    }
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Main : BaseUnityPlugin
    {
        // 存储所有加载的数据数组
        private static Dictionary<string, object> _dataArrays = new Dictionary<string, object>();
        private void Awake()
        {
            Logger.LogInfo("HoLMod.Tool 已加载");
            
            // 初始化窗口组件
            gameObject.AddComponent<Window>();
            
            // 加载数据
            LoadData();
        }
        
        // 初始化数据，直接使用Mainload.XXX进行加载
        private void LoadData()
        {
            Logger.LogInfo("开始加载数据到_dataArrays字典...");
            
            // 添加数据加载日志
            AddDataWithLog("CityData_XueYuan_now", Mainload.CityData_XueYuan_now);
            AddDataWithLog("ZhengLing_City_now", Mainload.ZhengLing_City_now);
            AddDataWithLog("XunXing_King", Mainload.XunXing_King);
            AddDataWithLog("ZiBei_Now", Mainload.ZiBei_Now);
            AddDataWithLog("Cost_King", Mainload.Cost_King);
            AddDataWithLog("PerData", Mainload.PerData);
            AddDataWithLog("ZhengLing_Now", Mainload.ZhengLing_Now);
            AddDataWithLog("WangGData_now", Mainload.WangGData_now);
            AddDataWithLog("JiHuiMeet_now", Mainload.JiHuiMeet_now);
            AddDataWithLog("TaskHD_Now", Mainload.TaskHD_Now);
            AddDataWithLog("TaskOrderData_Now", Mainload.TaskOrderData_Now);
            AddDataWithLog("PropPrice_Now", Mainload.PropPrice_Now);
            AddDataWithLog("ShijiaOutXianPoint", Mainload.ShijiaOutXianPoint);
            AddDataWithLog("WarEvent_Now", Mainload.WarEvent_Now);
            AddDataWithLog("Trade_Playershop", Mainload.Trade_Playershop);
            AddDataWithLog("JieDai_now", Mainload.JieDai_now);
            AddDataWithLog("Buluo_Now", Mainload.Buluo_Now);
            AddDataWithLog("KingCityData_now", Mainload.KingCityData_now);
            AddDataWithLog("ShiJia_king", Mainload.ShiJia_king);
            AddDataWithLog("ShiJia_Now", Mainload.ShiJia_Now);
            AddDataWithLog("JunYing_now", Mainload.JunYing_now);
            AddDataWithLog("Zhen_now", Mainload.Zhen_now);
            AddDataWithLog("Cun_now", Mainload.Cun_now);
            AddDataWithLog("Kong_now", Mainload.Kong_now);
            AddDataWithLog("Sen_Now", Mainload.Sen_Now);
            AddDataWithLog("Hu_Now", Mainload.Hu_Now);
            AddDataWithLog("Shan_now", Mainload.Shan_now);
            AddDataWithLog("Kuang_now", Mainload.Kuang_now);
            AddDataWithLog("NongZ_now", Mainload.NongZ_now);
            AddDataWithLog("Fengdi_now", Mainload.Fengdi_now);
            AddDataWithLog("Fudi_now", Mainload.Fudi_now);
            AddDataWithLog("ZhuangTou_now", Mainload.ZhuangTou_now);
            AddDataWithLog("Member_First", Mainload.Member_First);
            AddDataWithLog("Doctor_now", Mainload.Doctor_now);
            AddDataWithLog("MenKe_Now", Mainload.MenKe_Now);
            AddDataWithLog("Member_Qinglou", Mainload.Member_Qinglou);
            AddDataWithLog("Member_Other_qu", Mainload.Member_Other_qu);
            AddDataWithLog("Member_other", Mainload.Member_other);
            AddDataWithLog("Member_now", Mainload.Member_now);
            AddDataWithLog("Member_qu", Mainload.Member_qu);
            AddDataWithLog("Member_King_qu", Mainload.Member_King_qu);
            AddDataWithLog("Member_Ci", Mainload.Member_Ci);
            AddDataWithLog("Member_King", Mainload.Member_King);
            AddDataWithLog("CGNum", Mainload.CGNum);
            AddDataWithLog("FamilyData", Mainload.FamilyData);
            AddDataWithLog("Time_now", Mainload.Time_now);
            AddDataWithLog("Prop_have", Mainload.Prop_have);
            AddDataWithLog("Horse_Have", Mainload.Horse_Have);
            AddDataWithLog("IdIndex", Mainload.IdIndex);
            AddDataWithLog("ShangHui_now", Mainload.ShangHui_now);
            AddDataWithLog("CityData_now", Mainload.CityData_now);
            AddDataWithLog("Guan_JingCheng", Mainload.Guan_JingCheng);
            AddDataWithLog("PlayTimeRun", Mainload.PlayTimeRun);
            AddDataWithLog("MemberNumWillDead", Mainload.MemberNumWillDead);
            AddDataWithLog("NuLiNum", Mainload.NuLiNum);
            AddDataWithLog("VisionIndex", Mainload.VisionIndex);
            AddDataWithLog("SceneID", Mainload.SceneID);
            AddDataWithLog("CityID_CanAttack", Mainload.CityID_CanAttack);
            AddDataWithLog("ZuZhangJueCeID_now", Mainload.ZuZhangJueCeID_now);
            AddDataWithLog("BuildTaoZhuangID_SelectNow", Mainload.BuildTaoZhuangID_SelectNow);
            AddDataWithLog("GetMoney_Month", Mainload.GetMoney_Month);
            AddDataWithLog("SelectNumPer_Shop", Mainload.SelectNumPer_Shop);
            AddDataWithLog("SelectNumPerAct", Mainload.SelectNumPerAct);
            AddDataWithLog("CitySawSetData", Mainload.CitySawSetData);
            AddDataWithLog("CangShuGeData_Now", Mainload.CangShuGeData_Now);
            AddDataWithLog("XiQuHave_Now", Mainload.XiQuHave_Now);
            AddDataWithLog("ZhiZeData_ZhangMu", Mainload.ZhiZeData_ZhangMu);
            AddDataWithLog("VersionID", Mainload.VersionID);
            
            Logger.LogInfo("数据加载完成，共加载了" + _dataArrays.Count + "个数组");
        }
        
        // 辅助方法：添加数据到字典并记录日志
        private void AddDataWithLog(string key, object data)
        {
            if (data == null)
            {
                Logger.LogWarning("数据加载警告: 数组" + key + "为空");
                _dataArrays[key] = data;
                return;
            }
            
            Type dataType = data.GetType();
            string typeName = dataType.Name;
            string countInfo = "未知长度";
            
            if (data is Array array)
            {
                countInfo = array.Length + "个元素";
            }
            else if (data is System.Collections.IList list)
            {
                countInfo = list.Count + "个元素";
            }
            else if (data is System.Collections.IEnumerable enumerable)
            {
                int count = 0;
                foreach (var item in enumerable)
                    count++;
                countInfo = count + "个元素";
            }
            
            Logger.LogInfo("加载数据: " + key + " (类型: " + typeName + ", " + countInfo + ")");
            _dataArrays[key] = data;
        }
    }
}