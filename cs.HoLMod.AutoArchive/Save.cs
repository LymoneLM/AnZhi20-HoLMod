using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace cs.HoLMod.AutoArchive
{
    public static class AutoArchiveSaver
    {
        public static string BakIndex { get; set; } = "_0";

        public static void SaveAutoArchiveGameData(string SceneIDSave)
        {
            string[] array = SceneIDSave.Split(new char[]
            {
                '|'
            });
            if (array[0] == "Z")
            {
                SaveData.SaveBuild(array[0], array[1], array[2], false);
            }
            else
            {
                SaveData.SaveBuild(array[0], array[1], "0", false);
            }

            ES3.Save<string>("VersionID", Mainload.VersionID, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<int>>("ZhiZeData_ZhangMu", Mainload.ZhiZeData_ZhangMu, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("XiQuHave_Now", Mainload.XiQuHave_Now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("CangShuGeData_Now", Mainload.CangShuGeData_Now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<int>>("CitySawSetData", Mainload.CitySawSetData, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<int>>("SelectNumPerAct", Mainload.SelectNumPerAct, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<int>>>("SelectNumPer_Shop", Mainload.SelectNumPer_Shop, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<int>>("GetMoney_Month", Mainload.GetMoney_Month, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<string>("BuildTaoZhuangID_SelectNow", Mainload.BuildTaoZhuangID_SelectNow, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<string>>("ZuZhangJueCeID_now", Mainload.ZuZhangJueCeID_now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<int>>("CityID_CanAttack", Mainload.CityID_CanAttack, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<string>("SceneID", Mainload.SceneID, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<int>("VisionIndex", Mainload.VisionIndex, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<int>("NuLiNum", Mainload.NuLiNum, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<int>("MemberNumWillDead", Mainload.MemberNumWillDead, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<int>>("PlayTimeRun", Mainload.PlayTimeRun, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("Guan_JingCheng", Mainload.Guan_JingCheng, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("CityData_now", Mainload.CityData_now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("ShangHui_now", Mainload.ShangHui_now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<int>>("IdIndex", Mainload.IdIndex, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("Horse_Have", Mainload.Horse_Have, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("Prop_have", Mainload.Prop_have, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<int>>("Time_now", Mainload.Time_now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<string>>("FamilyData", Mainload.FamilyData, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<string>>("CGNum", Mainload.CGNum, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("Member_Ci", Mainload.Member_Ci, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("Member_King", Mainload.Member_King, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("Member_King_qu", Mainload.Member_King_qu, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("Member_qu", Mainload.Member_qu, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("Member_now", Mainload.Member_now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("Member_other", Mainload.Member_other, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("Member_Other_qu", Mainload.Member_Other_qu, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("Member_Hanmen", Mainload.Member_Hanmen, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("Member_Qinglou", Mainload.Member_Qinglou, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("MenKe_Now", Mainload.MenKe_Now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("Doctor_now", Mainload.Doctor_now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<string>>("Member_First", Mainload.Member_First, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<List<string>>>>>("ZhuangTou_now", Mainload.ZhuangTou_now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("Fudi_now", Mainload.Fudi_now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("Fengdi_now", Mainload.Fengdi_now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("NongZ_now", Mainload.NongZ_now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("Kuang_now", Mainload.Kuang_now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("Shan_now", Mainload.Shan_now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("Hu_Now", Mainload.Hu_Now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("Sen_Now", Mainload.Sen_Now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("Kong_now", Mainload.Kong_now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("Cun_now", Mainload.Cun_now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("Zhen_now", Mainload.Zhen_now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<string>>>>("JunYing_now", Mainload.JunYing_now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("ShiJia_Now", Mainload.ShiJia_Now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<string>>("ShiJia_king", Mainload.ShiJia_king, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<string>>("KingCityData_now", Mainload.KingCityData_now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("Buluo_Now", Mainload.Buluo_Now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("JieDai_now", Mainload.JieDai_now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<List<List<string>>>>>("Trade_Playershop", Mainload.Trade_Playershop, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("WarEvent_Now", Mainload.WarEvent_Now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<int>>>("ShijiaOutXianPoint", Mainload.ShijiaOutXianPoint, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("PropPrice_Now", Mainload.PropPrice_Now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<int>>>("TaskOrderData_Now", Mainload.TaskOrderData_Now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("TaskYanShi_Now", Mainload.TaskYanShi_Now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("TaskHD_Now", Mainload.TaskHD_Now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<int>>("JiHuiMeet_now", Mainload.JiHuiMeet_now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<string>>>("WangGData_now", Mainload.WangGData_now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<string>>("ZhengLing_Now", Mainload.ZhengLing_Now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<float>>("PerData", Mainload.PerData, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<int>>("Cost_King", Mainload.Cost_King, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<string>>("ZiBei_Now", Mainload.ZiBei_Now, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<string>>("XunXing_King", Mainload.XunXing_King, Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<int>>>("ZhengLing_City_now", Mainload.ZhengLing_City_now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            ES3.Save<List<List<float>>>("CityData_XueYuan_now", Mainload.CityData_XueYuan_now, 
                Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            if (Mainload.isSwichPanelOpen)
            {
                Mainload.isSaveFinish = true;
            }
        }

        public static void SaveBuild_NongZOutFendi_Addlater(List<List<string>> BuildNongZ, string ScenIndexA, string ScenIndexB)
        {
            ES3.Save<List<List<string>>>("BuildInto_z", BuildNongZ, string.Concat(new string[]
            {
                Mainload.CunDangIndex_now + BakIndex,
                "/Z",
                ScenIndexA,
                "/",
                ScenIndexB,
                ".es3"
            }));
        }

        public static void SaveBuild(string SceneClass, string ScenIndexA, string ScenIndexB, bool isInit)
        {
            if (SceneClass == "M")
            {
                if (isInit)
                {
                    List<List<string>> value = new List<List<string>>();
                    ES3.Save<List<List<string>>>("BuildInto_m", value, Mainload.CunDangIndex_now + BakIndex + "/M" + ScenIndexA + ".es3");
                    return;
                }
                ES3.Save<List<List<string>>>("BuildInto_m", Mainload.BuildInto_m, Mainload.CunDangIndex_now + BakIndex + "/M" + ScenIndexA + ".es3");
                return;
            }
            else if (SceneClass == "Z")
            {
                if (isInit)
                {
                    List<List<string>> value2 = new List<List<string>>();
                    ES3.Save<List<List<string>>>("BuildInto_z", value2, string.Concat(new string[]
                    {
                        Mainload.CunDangIndex_now + BakIndex,
                        "/Z",
                        ScenIndexA,
                        "/",
                        ScenIndexB,
                        ".es3"
                    }));
                    return;
                }
                ES3.Save<List<List<string>>>("BuildInto_z", Mainload.BuildInto_z, string.Concat(new string[]
                {
                    Mainload.CunDangIndex_now + BakIndex,
                    "/Z",
                    ScenIndexA,
                    "/",
                    ScenIndexB,
                    ".es3"
                }));
                return;
            }
            else
            {
                if (!(SceneClass == "S"))
                {
                    if (SceneClass == "H")
                    {
                        if (isInit)
                        {
                            List<List<string>> value3 = new List<List<string>>();
                            ES3.Save<List<List<string>>>("BuildInto_h", value3, Mainload.CunDangIndex_now + BakIndex + "/H" + ScenIndexA + ".es3");
                            return;
                        }
                        ES3.Save<List<List<string>>>("BuildInto_h", Mainload.BuildInto_h, Mainload.CunDangIndex_now + BakIndex + "/H" + ScenIndexA + ".es3");
                    }
                    return;
                }
                if (isInit)
                {
                    List<List<string>> value4 = new List<List<string>>();
                    ES3.Save<List<List<string>>>("BuildInto_s", value4, Mainload.CunDangIndex_now + BakIndex + "/S" + ScenIndexA + ".es3");
                    ES3.Save<List<List<string>>>("BuildInto_c", value4, Mainload.CunDangIndex_now + BakIndex + "/S" + ScenIndexA + ".es3");
                    return;
                }
                ES3.Save<List<List<string>>>("BuildInto_s", Mainload.BuildInto_s, Mainload.CunDangIndex_now + BakIndex + "/S" + ScenIndexA + ".es3");
                ES3.Save<List<List<string>>>("BuildInto_c", Mainload.BuildInto_c, Mainload.CunDangIndex_now + BakIndex + "/S" + ScenIndexA + ".es3");
                return;
            }
        }

        public static void SaveSetData()
        {
            ES3.Save<int>("Guide_order", Mainload.Guide_order, "FW/SetData.es3");
            ES3.Save<List<int>>("Guide_Nei", Mainload.Guide_Nei, "FW/SetData.es3");
            ES3.Save<bool>("IsPreGuideFinish", Mainload.IsPreGuideFinish, "FW/SetData.es3");
            ES3.Save<List<int>>("SetData", Mainload.SetData, "FW/SetData.es3");
            ES3.Save<List<KeyCode>>("FastKey", Mainload.FastKey, "FW/SetData.es3");
            ES3.Save<string>("GongGaoID_Read", Mainload.GongGaoID_Read, "FW/SetData.es3");
        }
    }
}
