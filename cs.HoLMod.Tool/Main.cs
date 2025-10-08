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
        // 单例模式
        private static Main _instance;
        public static Main Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Main>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("cs.HoLMod.Tool");
                        _instance = obj.AddComponent<Main>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }

        // 数据容器，用于存储所有需要编辑的数据数组
        private Dictionary<string, object> _dataArrays = new Dictionary<string, object>();
        public Dictionary<string, object> DataArrays { get { return _dataArrays; } }

        // 当前选中的数组路径，用于在窗口之间传递数据选择状态
        public string CurrentSelectedPath { get; set; }
        public string CurrentSelectedPath2 { get; set; }
        public string CurrentSelectedPath3 { get; set; }

        private void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                InitializeData();
                InitializeWindow();
            }
        }

        // 初始化数据，直接使用Mainload.XXX进行加载
        private void InitializeData()
        {
            try
            {
                // 直接从Mainload类加载所有需要的数据数组
                _dataArrays["CityData_XueYuan_now"] = Mainload.CityData_XueYuan_now;
                _dataArrays["ZhengLing_City_now"] = Mainload.ZhengLing_City_now;
                _dataArrays["XunXing_King"] = Mainload.XunXing_King;
                _dataArrays["ZiBei_Now"] = Mainload.ZiBei_Now;
                _dataArrays["Cost_King"] = Mainload.Cost_King;
                _dataArrays["PerData"] = Mainload.PerData;
                _dataArrays["ZhengLing_Now"] = Mainload.ZhengLing_Now;
                _dataArrays["WangGData_now"] = Mainload.WangGData_now;
                _dataArrays["JiHuiMeet_now"] = Mainload.JiHuiMeet_now;
                _dataArrays["TaskHD_Now"] = Mainload.TaskHD_Now;
                _dataArrays["TaskOrderData_Now"] = Mainload.TaskOrderData_Now;
                _dataArrays["PropPrice_Now"] = Mainload.PropPrice_Now;
                _dataArrays["ShijiaOutXianPoint"] = Mainload.ShijiaOutXianPoint;
                _dataArrays["WarEvent_Now"] = Mainload.WarEvent_Now;
                _dataArrays["Trade_Playershop"] = Mainload.Trade_Playershop;
                _dataArrays["JieDai_now"] = Mainload.JieDai_now;
                _dataArrays["Buluo_Now"] = Mainload.Buluo_Now;
                _dataArrays["KingCityData_now"] = Mainload.KingCityData_now;
                _dataArrays["ShiJia_king"] = Mainload.ShiJia_king;
                _dataArrays["ShiJia_Now"] = Mainload.ShiJia_Now;
                _dataArrays["JunYing_now"] = Mainload.JunYing_now;
                _dataArrays["Zhen_now"] = Mainload.Zhen_now;
                _dataArrays["Cun_now"] = Mainload.Cun_now;
                _dataArrays["Kong_now"] = Mainload.Kong_now;
                _dataArrays["Sen_Now"] = Mainload.Sen_Now;
                _dataArrays["Hu_Now"] = Mainload.Hu_Now;
                _dataArrays["Shan_now"] = Mainload.Shan_now;
                _dataArrays["Kuang_now"] = Mainload.Kuang_now;
                _dataArrays["NongZ_now"] = Mainload.NongZ_now;
                _dataArrays["Fengdi_now"] = Mainload.Fengdi_now;
                _dataArrays["Fudi_now"] = Mainload.Fudi_now;
                _dataArrays["ZhuangTou_now"] = Mainload.ZhuangTou_now;
                _dataArrays["Member_First"] = Mainload.Member_First;
                _dataArrays["Doctor_now"] = Mainload.Doctor_now;
                _dataArrays["MenKe_Now"] = Mainload.MenKe_Now;
                _dataArrays["Member_Qinglou"] = Mainload.Member_Qinglou;
                _dataArrays["Member_Other_qu"] = Mainload.Member_Other_qu;
                _dataArrays["Member_other"] = Mainload.Member_other;
                _dataArrays["Member_now"] = Mainload.Member_now;
                _dataArrays["Member_qu"] = Mainload.Member_qu;
                _dataArrays["Member_King_qu"] = Mainload.Member_King_qu;
                _dataArrays["Member_Ci"] = Mainload.Member_Ci;
                _dataArrays["Member_King"] = Mainload.Member_King;
                _dataArrays["CGNum"] = Mainload.CGNum;
                _dataArrays["FamilyData"] = Mainload.FamilyData;
                _dataArrays["Time_now"] = Mainload.Time_now;
                _dataArrays["Prop_have"] = Mainload.Prop_have;
                _dataArrays["Horse_Have"] = Mainload.Horse_Have;
                _dataArrays["IdIndex"] = Mainload.IdIndex;
                _dataArrays["ShangHui_now"] = Mainload.ShangHui_now;
                _dataArrays["CityData_now"] = Mainload.CityData_now;
                _dataArrays["Guan_JingCheng"] = Mainload.Guan_JingCheng;
                _dataArrays["PlayTimeRun"] = Mainload.PlayTimeRun;
                _dataArrays["MemberNumWillDead"] = Mainload.MemberNumWillDead;
                _dataArrays["NuLiNum"] = Mainload.NuLiNum;
                _dataArrays["VisionIndex"] = Mainload.VisionIndex;
                _dataArrays["SceneID"] = Mainload.SceneID;
                _dataArrays["CityID_CanAttack"] = Mainload.CityID_CanAttack;
                _dataArrays["ZuZhangJueCeID_now"] = Mainload.ZuZhangJueCeID_now;
                _dataArrays["BuildTaoZhuangID_SelectNow"] = Mainload.BuildTaoZhuangID_SelectNow;
                _dataArrays["GetMoney_Month"] = Mainload.GetMoney_Month;
                _dataArrays["SelectNumPer_Shop"] = Mainload.SelectNumPer_Shop;
                _dataArrays["SelectNumPerAct"] = Mainload.SelectNumPerAct;
                _dataArrays["CitySawSetData"] = Mainload.CitySawSetData;
                _dataArrays["CangShuGeData_Now"] = Mainload.CangShuGeData_Now;
                _dataArrays["XiQuHave_Now"] = Mainload.XiQuHave_Now;
                _dataArrays["ZhiZeData_ZhangMu"] = Mainload.ZhiZeData_ZhangMu;
                _dataArrays["VersionID"] = Mainload.VersionID;
            }
            catch (Exception e)
            {
                Debug.LogWarning("无法直接加载Mainload数据，保持数据为空: " + e.Message);
                // 如果无法直接加载Mainload数据，保持数据为空
                LoadMockData();
            }
        }

        // 直接显示空数据，不使用模拟数据作为后备
        private void LoadMockData()
        {
            // 不创建任何模拟数据，保持数组为空
        }
        

        // 初始化窗口
        private void InitializeWindow()
        {
            // 在BepInEx环境中创建窗口GameObject和组件
            GameObject windowObj = new GameObject("cs.HoLMod.Tool.Window");
            windowObj.AddComponent<Window>();
            // 确保窗口对象不会被销毁
            DontDestroyOnLoad(windowObj);
        }

        // 根据路径获取数据
        public object GetDataByPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (_dataArrays.ContainsKey(path))
            {
                return _dataArrays[path];
            }

            // 处理嵌套路径
            string[] pathParts = path.Split('.');
            if (pathParts.Length > 1)
            {
                string mainKey = pathParts[0];
                if (_dataArrays.ContainsKey(mainKey))
                {
                    object data = _dataArrays[mainKey];
                    for (int i = 1; i < pathParts.Length; i++)
                    {
                        if (int.TryParse(pathParts[i], out int index))
                        {
                            if (data is IList<object> list && index >= 0 && index < list.Count)
                            {
                                data = list[index];
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    return data;
                }
            }

            return null;
        }

        // 修改数据
        public bool ModifyData(string path, object newValue)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            // 处理嵌套路径
            string[] pathParts = path.Split('.');
            if (pathParts.Length > 1)
            {
                string mainKey = pathParts[0];
                if (_dataArrays.ContainsKey(mainKey))
                {
                    object data = _dataArrays[mainKey];
                    object parentData = null;
                    int parentIndex = -1;

                    for (int i = 1; i < pathParts.Length - 1; i++)
                    {
                        if (int.TryParse(pathParts[i], out int index))
                        {
                            if (data is IList<object> list && index >= 0 && index < list.Count)
                            {
                                parentData = data;
                                parentIndex = index;
                                data = list[index];
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }

                    if (int.TryParse(pathParts[pathParts.Length - 1], out int finalIndex))
                    {
                        if (parentData is IList<object> parentList && finalIndex >= 0 && finalIndex < parentList.Count)
                        {
                            parentList[finalIndex] = newValue;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
