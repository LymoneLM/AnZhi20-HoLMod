using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace cs.HoLMod.Tool
{
    public class WindowA
    {
        // 第一列显示的按钮名称列表
        private List<string> TheFirstColumnData = new List<string>()
        {
            // 按钮文案
            "CityData_XueYuan_now",
            "ZhengLing_City_now",
            "XunXing_King",
            "ZiBei_Now",
            "Cost_King",
            "PerData",
            "ZhengLing_Now",
            "WangGData_now",
            "JiHuiMeet_now",
            "TaskHD_Now",
            "TaskYanShi_Now",
            "TaskOrderData_Now",
            "PropPrice_Now",
            "ShijiaOutXianPoint",
            "WarEvent_Now",
            "Trade_Playershop",
            "JieDai_now",
            "Buluo_Now",
            "KingCityData_now",
            "ShiJia_king",
            "ShiJia_Now",
            "JunYing_now",
            "Zhen_now",
            "Cun_now",
            "Kong_now",
            "Sen_Now",
            "Hu_Now",
            "Shan_now",
            "Kuang_now",
            "NongZ_now",
            "Fengdi_now",
            "Fudi_now",
            "ZhuangTou_now",
            "Member_First",
            "Doctor_now",
            "MenKe_Now",
            "Member_Qinglou",
            "Member_Hanmen",
            "Member_Other_qu",
            "Member_other",
            "Member_now",
            "Member_qu",
            "Member_King_qu",
            "Member_King",
            "Member_Ci",
            "CGNum",
            "FamilyData",
            "Time_now",
            "Prop_have",
            "Horse_Have",
            "IdIndex",
            "ShangHui_now",
            "CityData_now",
            "Guan_JingCheng",
            "PlayTimeRun",
            "MemberNumWillDead",
            "NuLiNum",
            "VisionIndex",
            "SceneID",
            "CityID_CanAttack",
            "ZuZhangJueCeID_now",
            "BuildTaoZhuangID_SelectNow",
            "GetMoney_Month",
            "SelectNumPer_Shop",
            "SelectNumPerAct",
            "CitySawSetData",
            "CangShuGeData_Now",
            "XiQuHave_Now",
            "ZhiZeData_ZhangMu",
            "VersionID"
        };

        // 第二列数据（这里需要定义数据类型，假设是字符串列表）
        private List<string> TheSecondColumnData = new List<string>();
        private string selectedFirstColumnItem = null;

        // GUI绘制方法
        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            
            // 第一列 - 显示按钮列表
            GUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Label("第一列数据");
            
            if (GUILayout.Button("刷新数据"))
            {
                // 刷新数据的逻辑
            }
            
            GUILayout.BeginScrollView(Vector2.zero, GUILayout.ExpandHeight(true));
            foreach (string item in TheFirstColumnData)
            {
                if (GUILayout.Button(item))
                {
                    selectedFirstColumnItem = item;
                    // 由于Mainload未定义，这里暂时不调用LoadTheSecondColumnData
                    // LoadTheSecondColumnData(item);
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            
            // 垂直分割线
            GUILayout.Box("", GUILayout.Width(2), GUILayout.ExpandHeight(true));
            
            // 第二列 - 显示选中项的详细信息
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("第二列数据");
            
            if (selectedFirstColumnItem != null)
            {
                GUILayout.Label("选中项: " + selectedFirstColumnItem);
                GUILayout.Space(10);
                GUILayout.BeginScrollView(Vector2.zero, GUILayout.ExpandHeight(true));
                if (TheSecondColumnData.Count > 0)
                {
                    foreach (string item in TheSecondColumnData)
                    {
                        GUILayout.Label(item);
                    }
                }
                else
                {
                    GUILayout.Label("数据加载中...");
                }
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("请从左侧选择一个项目", EditorStyles.centeredGreyMiniLabel);
                GUILayout.FlexibleSpace();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        // 更新方法
        public void Update()
        {
            // 窗口A的更新逻辑
        }

        
}
