using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace cs.HoLMod.Tool
{
    public class WindowB
    {
        // GUI绘制方法
        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            
            // 第一列 - 可以放置按钮或其他UI元素
            GUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Label("窗口B - 第一列");
            
            if (GUILayout.Button("刷新数据"))
            {
                // 刷新数据的逻辑
            }
            
            // 这里可以添加更多UI元素
            GUILayout.Space(20);
            GUILayout.Label("窗口B的第一列内容");
            
            GUILayout.EndVertical();
            
            // 垂直分割线
            GUILayout.Box("", GUILayout.Width(2), GUILayout.ExpandHeight(true));
            
            // 第二列 - 可以放置详细信息或其他内容
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("窗口B - 第二列");
            
            GUILayout.FlexibleSpace();
            GUILayout.Label("窗口B的第二列内容", EditorStyles.centeredGreyMiniLabel);
            GUILayout.FlexibleSpace();
            
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        // 更新方法
        public void Update()
        {
            // 窗口B的更新逻辑
        }

        // 根据第一列的点击，选择对应的加载方法加载第二列数据
        private void LoadTheSecondColumnData(string selectedButton)
        {
            switch (selectedButton)
            {
                case "CityData_XueYuan_now":
                    TheSecondColumnData = Mainload.CityData_XueYuan_now;
                    break;
                case "ZhengLing_City_now":
                    TheSecondColumnData = Mainload.ZhengLing_City_now;
                    break;
                case "XunXing_King":
                    TheSecondColumnData = Mainload.XunXing_King;
                    break;
                case "ZiBei_Now":
                    TheSecondColumnData = Mainload.ZiBei_Now;
                    break;
                case "Cost_King":
                    TheSecondColumnData = Mainload.Cost_King;
                    break;
                case "PerData":
                    TheSecondColumnData = Mainload.PerData;
                    break;
                case "ZhengLing_Now":
                    TheSecondColumnData = Mainload.ZhengLing_Now;
                    break;
                case "WangGData_now":
                    TheSecondColumnData = Mainload.WangGData_now;
                    break;
                case "JiHuiMeet_now":
                    TheSecondColumnData = Mainload.JiHuiMeet_now;
                    break;
                case "TaskHD_Now":
                    TheSecondColumnData = Mainload.TaskHD_Now;
                    break;
                case "TaskYanShi_Now":
                    TheSecondColumnData = Mainload.TaskYanShi_Now;
                    break;
                case "TaskOrderData_Now":
                    TheSecondColumnData = Mainload.TaskOrderData_Now;
                    break;
                case "PropPrice_Now":
                    TheSecondColumnData = Mainload.PropPrice_Now;
                    break;
                case "ShijiaOutXianPoint":
                    TheSecondColumnData = Mainload.ShijiaOutXianPoint;
                    break;
                case "WarEvent_Now":
                    TheSecondColumnData = Mainload.WarEvent_Now;
                    break;
                case "Trade_Playershop":
                    TheSecondColumnData = Mainload.Trade_Playershop;
                    break;
                case "JieDai_now":
                    TheSecondColumnData = Mainload.JieDai_now;
                    break;
                case "Buluo_Now":
                    TheSecondColumnData = Mainload.Buluo_Now;
                    break;
                case "KingCityData_now":
                    TheSecondColumnData = Mainload.KingCityData_now;
                    break;
                case "ShiJia_king":
                    TheSecondColumnData = Mainload.ShiJia_king;
                    break;
                case "ShiJia_Now":
                    TheSecondColumnData = Mainload.ShiJia_Now;
                    break;
                case "JunYing_now":
                    TheSecondColumnData = Mainload.JunYing_now;
                    break;
                case "Zhen_now":
                    TheSecondColumnData = Mainload.Zhen_now;
                    break;
                case "Cun_now":
                    TheSecondColumnData = Mainload.Cun_now;
                    break;
                case "Kong_now":
                    TheSecondColumnData = Mainload.Kong_now;
                    break;
                case "Sen_Now":
                    TheSecondColumnData = Mainload.Sen_Now;
                    break;
                case "Hu_Now":
                    TheSecondColumnData = Mainload.Hu_Now;
                    break;
                case "Shan_now":
                    TheSecondColumnData = Mainload.Shan_now;
                    break;
                case "Kuang_now":
                    TheSecondColumnData = Mainload.Kuang_now;
                    break;
                case "NongZ_now":
                    TheSecondColumnData = Mainload.NongZ_now;
                    break;
                case "Fengdi_now":
                    TheSecondColumnData = Mainload.Fengdi_now;
                    break;
                case "Fudi_now":
                    TheSecondColumnData = Mainload.Fudi_now;
                    break;
                case "ZhuangTou_now":
                    TheSecondColumnData = Mainload.ZhuangTou_now;
                    break;
                case "Member_First":
                    TheSecondColumnData = Mainload.Member_First;
                    break;
                case "Doctor_now":
                    TheSecondColumnData = Mainload.Doctor_now;
                    break;
                case "MenKe_Now":
                    TheSecondColumnData = Mainload.MenKe_Now;
                    break;
                case "Member_Qinglou":
                    TheSecondColumnData = Mainload.Member_Qinglou;
                    break;
                case "Member_Hanmen":
                    TheSecondColumnData = Mainload.Member_Hanmen;
                    break;
                case "Member_Other_qu":
                    TheSecondColumnData = Mainload.Member_Other_qu;
                    break;
                case "Member_other":
                    TheSecondColumnData = Mainload.Member_other;
                    break;
                case "Member_now":
                    TheSecondColumnData = Mainload.Member_now;
                    break;
                case "Member_qu":
                    TheSecondColumnData = Mainload.Member_qu;
                    break;
                case "Member_King_qu":
                    TheSecondColumnData = Mainload.Member_King_qu;
                    break;
                case "Member_King":
                    TheSecondColumnData = Mainload.Member_King;
                    break;
                case "Member_Ci":
                    TheSecondColumnData = Mainload.Member_Ci;
                    break;
                case "CGNum":
                    TheSecondColumnData = Mainload.CGNum;
                    break;
                case "FamilyData":
                    TheSecondColumnData = Mainload.FamilyData;
                    break;
                case "Time_now":
                    TheSecondColumnData = Mainload.Time_now;
                    break;
                case "Prop_have":
                    TheSecondColumnData = Mainload.Prop_have;
                    break;
                case "Horse_Have":
                    TheSecondColumnData = Mainload.Horse_Have;
                    break;
                case "IdIndex":
                    TheSecondColumnData = Mainload.IdIndex;
                    break;
                case "ShangHui_now":
                    TheSecondColumnData = Mainload.ShangHui_now;
                    break;
                case "CityData_now":
                    TheSecondColumnData = Mainload.CityData_now;
                    break;
                case "Guan_JingCheng":
                    TheSecondColumnData = Mainload.Guan_JingCheng;
                    break;
                case "PlayTimeRun":
                    TheSecondColumnData = Mainload.PlayTimeRun;
                    break;
                case "MemberNumWillDead":
                    TheSecondColumnData = Mainload.MemberNumWillDead;
                    break;
                case "NuLiNum":
                    TheSecondColumnData = Mainload.NuLiNum;
                    break;
                case "VisionIndex":
                    TheSecondColumnData = Mainload.VisionIndex;
                    break;
                case "SceneID":
                    TheSecondColumnData = Mainload.SceneID;
                    break;
                case "CityID_CanAttack":
                    TheSecondColumnData = Mainload.CityID_CanAttack;
                    break;
                case "ZuZhangJueCeID_now":
                    TheSecondColumnData = Mainload.ZuZhangJueCeID_now;
                    break;
                case "BuildTaoZhuangID_SelectNow":
                    TheSecondColumnData = Mainload.BuildTaoZhuangID_SelectNow;
                    break;
                case "GetMoney_Month":
                    TheSecondColumnData = Mainload.GetMoney_Month;
                    break;
                case "SelectNumPer_Shop":
                    TheSecondColumnData = Mainload.SelectNumPer_Shop;
                    break;
                case "SelectNumPerAct":
                    TheSecondColumnData = Mainload.SelectNumPerAct;
                    break;
                case "CitySawSetData":
                    TheSecondColumnData = Mainload.CitySawSetData;
                    break;
                case "CangShuGeData_Now":
                    TheSecondColumnData = Mainload.CangShuGeData_Now;
                    break;
                case "XiQuHave_Now":
                    TheSecondColumnData = Mainload.XiQuHave_Now;
                    break;
                case "ZhiZeData_ZhangMu":
                    TheSecondColumnData = Mainload.ZhiZeData_ZhangMu;
                    break;
                case "VersionID":
                    TheSecondColumnData = Mainload.VersionID;
                    break;
                default:
                    // 处理其他情况
                    break;
            }
        }

    }
}
