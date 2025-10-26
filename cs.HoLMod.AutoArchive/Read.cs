using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace cs.HoLMod.AutoArchive
{
    public static class AutoArchiveReader
    {
        public static string BakIndex { get; set; } = "_0";

        public static void ReadAutoArchiveGameData()
        {
            Mainload.VersionID = ES3.Load<string>("VersionID", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            if (Mainload.VersionID != Mainload.Vision_now)
            {
                Mainload.VersionID = Mainload.Vision_now;
            }
            Mainload.ZhiZeData_ZhangMu = ES3.Load<List<int>>("ZhiZeData_ZhangMu", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.XiQuHave_Now = ES3.Load<List<List<string>>>("XiQuHave_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.CangShuGeData_Now = ES3.Load<List<List<string>>>("CangShuGeData_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.CitySawSetData = ES3.Load<List<int>>("CitySawSetData", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.SelectNumPerAct = ES3.Load<List<int>>("SelectNumPerAct", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.SelectNumPer_Shop = ES3.Load<List<List<int>>>("SelectNumPer_Shop", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.GetMoney_Month = ES3.Load<List<int>>("GetMoney_Month", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.BuildTaoZhuangID_SelectNow = ES3.Load<string>("BuildTaoZhuangID_SelectNow", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.ZuZhangJueCeID_now = ES3.Load<List<string>>("ZuZhangJueCeID_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.CityID_CanAttack = ES3.Load<List<int>>("CityID_CanAttack", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.SceneID = ES3.Load<string>("SceneID", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.VisionIndex = ES3.Load<int>("VisionIndex", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.NuLiNum = ES3.Load<int>("NuLiNum", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.MemberNumWillDead = ES3.Load<int>("MemberNumWillDead", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.PlayTimeRun = ES3.Load<List<int>>("PlayTimeRun", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Guan_JingCheng = ES3.Load<List<List<string>>>("Guan_JingCheng", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.CityData_now = ES3.Load<List<List<List<string>>>>("CityData_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.ShangHui_now = ES3.Load<List<List<string>>>("ShangHui_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.IdIndex = ES3.Load<List<int>>("IdIndex", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Horse_Have = ES3.Load<List<List<string>>>("Horse_Have", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Prop_have = ES3.Load<List<List<string>>>("Prop_have", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Time_now = ES3.Load<List<int>>("Time_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.FamilyData = ES3.Load<List<string>>("FamilyData", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.CGNum = ES3.Load<List<string>>("CGNum", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Member_Ci = ES3.Load<List<List<string>>>("Member_Ci", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Member_King = ES3.Load<List<List<string>>>("Member_King", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Member_King_qu = ES3.Load<List<List<string>>>("Member_King_qu", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Member_qu = ES3.Load<List<List<string>>>("Member_qu", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Member_now = ES3.Load<List<List<string>>>("Member_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Member_other = ES3.Load<List<List<List<string>>>>("Member_other", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Member_Other_qu = ES3.Load<List<List<List<string>>>>("Member_Other_qu", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Member_Hanmen = ES3.Load<List<List<string>>>("Member_Hanmen", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Member_Qinglou = ES3.Load<List<List<List<string>>>>("Member_Qinglou", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.MenKe_Now = ES3.Load<List<List<string>>>("MenKe_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Doctor_now = ES3.Load<List<List<List<string>>>>("Doctor_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Member_First = ES3.Load<List<string>>("Member_First", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.ZhuangTou_now = ES3.Load<List<List<List<List<string>>>>>("ZhuangTou_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Fudi_now = ES3.Load<List<List<string>>>("Fudi_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Fengdi_now = ES3.Load<List<List<string>>>("Fengdi_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.NongZ_now = ES3.Load<List<List<List<string>>>>("NongZ_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Kuang_now = ES3.Load<List<List<List<string>>>>("Kuang_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Shan_now = ES3.Load<List<List<List<string>>>>("Shan_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Hu_Now = ES3.Load<List<List<List<string>>>>("Hu_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Sen_Now = ES3.Load<List<List<List<string>>>>("Sen_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Kong_now = ES3.Load<List<List<List<string>>>>("Kong_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Cun_now = ES3.Load<List<List<List<string>>>>("Cun_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Zhen_now = ES3.Load<List<List<List<string>>>>("Zhen_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.JunYing_now = ES3.Load<List<List<List<string>>>>("JunYing_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3", Mainload.JunYing_now);
            Mainload.ShiJia_Now = ES3.Load<List<List<string>>>("ShiJia_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.ShiJia_king = ES3.Load<List<string>>("ShiJia_king", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.KingCityData_now = ES3.Load<List<string>>("KingCityData_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Buluo_Now = ES3.Load<List<List<string>>>("Buluo_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.JieDai_now = ES3.Load<List<List<string>>>("JieDai_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.Trade_Playershop = ES3.Load<List<List<List<List<string>>>>>("Trade_Playershop", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.WarEvent_Now = ES3.Load<List<List<string>>>("WarEvent_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.ShijiaOutXianPoint = ES3.Load<List<List<int>>>("ShijiaOutXianPoint", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.PropPrice_Now = ES3.Load<List<List<string>>>("PropPrice_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.TaskOrderData_Now = ES3.Load<List<List<int>>>("TaskOrderData_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.TaskYanShi_Now = ES3.Load<List<List<string>>>("TaskYanShi_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.TaskHD_Now = ES3.Load<List<List<string>>>("TaskHD_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.JiHuiMeet_now = ES3.Load<List<int>>("JiHuiMeet_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3");
            Mainload.WangGData_now = ES3.Load<List<List<string>>>("WangGData_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3", Mainload.WangGData_now);
            Mainload.ZhengLing_Now = ES3.Load<List<string>>("ZhengLing_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3", Mainload.ZhengLing_Now);
            Mainload.PerData = ES3.Load<List<float>>("PerData", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3", Mainload.PerData);
            Mainload.Cost_King = ES3.Load<List<int>>("Cost_King", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3", Mainload.Cost_King);
            Mainload.ZiBei_Now = ES3.Load<List<string>>("ZiBei_Now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3", Mainload.ZiBei_Now);
            Mainload.XunXing_King = ES3.Load<List<string>>("XunXing_King", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3", Mainload.XunXing_King);
            Mainload.ZhengLing_City_now = ES3.Load<List<List<int>>>("ZhengLing_City_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3", Mainload.ZhengLing_City_now);
            Mainload.CityData_XueYuan_now = ES3.Load<List<List<float>>>("CityData_XueYuan_now", Mainload.CunDangIndex_now + BakIndex + "/GameData.es3", Mainload.CityData_XueYuan_now);
            for (int i = 0; i < Mainload.Member_King.Count; i++)
            {
                if (Mainload.Member_King[i].Count < 29)
                {
                    Mainload.Member_King[i].Add("6");
                    Mainload.Member_King[i].Add("-2|null");
                    Mainload.Member_King[i].Add("0|0|0|0");
                }
                else
                {
                    if (Mainload.Member_King[i].Count >= 30)
                    {
                        break;
                    }
                    Mainload.Member_King[i].Add("-2|null");
                    Mainload.Member_King[i].Add("0|0|0|0");
                }
            }
            for (int j = 0; j < Mainload.Member_King_qu.Count; j++)
            {
                if (Mainload.Member_King_qu[j].Count < 26)
                {
                    Mainload.Member_King_qu[j].Add("6");
                    Mainload.Member_King_qu[j].Add("null");
                }
                else
                {
                    if (Mainload.Member_King_qu[j].Count >= 27)
                    {
                        break;
                    }
                    Mainload.Member_King_qu[j].Add("null");
                }
            }
            for (int k = 0; k < Mainload.ShiJia_Now.Count; k++)
            {
                if (Mainload.ShiJia_Now[k].Count < 12)
                {
                    Mainload.ShiJia_Now[k].Add(FormulaData.JunNum_Shijia(int.Parse(Mainload.ShiJia_Now[k][2]), Mainload.ShiJia_Now[k][6]));
                    Mainload.ShiJia_Now[k].Add("100");
                }
                else
                {
                    if (Mainload.ShiJia_Now[k].Count >= 13)
                    {
                        break;
                    }
                    Mainload.ShiJia_Now[k].Add("100");
                }
            }
            if (Mainload.JunYing_now.Count <= 0)
            {
                for (int l = 0; l < Mainload.AllFengDiData.Count; l++)
                {
                    Mainload.JunYing_now.Add(new List<List<string>>());
                }
            }
            for (int m = 0; m < Mainload.Fudi_now.Count - Mainload.CangShuGeData_Now.Count; m++)
            {
                Mainload.CangShuGeData_Now.Add(new List<string>());
            }
            for (int n = 0; n < Mainload.NongZ_now.Count; n++)
            {
                for (int num = 0; num < Mainload.NongZ_now[n].Count; num++)
                {
                    if (Mainload.NongZ_now[n][num].Count >= 27)
                    {
                        n = 1000000;
                        break;
                    }
                    Mainload.NongZ_now[n][num].Add("5|3|2");
                }
            }
            for (int num2 = 0; num2 < Mainload.Shan_now.Count; num2++)
            {
                for (int num3 = 0; num3 < Mainload.Shan_now[num2].Count; num3++)
                {
                    if (Mainload.Shan_now[num2][num3].Count >= 6)
                    {
                        num2 = 1000000;
                        break;
                    }
                    Mainload.Shan_now[num2][num3].Add(FormulaData.LiuMinNum_Shan());
                }
            }
            if (Mainload.NuLiNum < 0)
            {
                Mainload.NuLiNum = 0;
            }
        }

        public static void ReadAutoArchiveBuildData(string SceneClass, string ScenIndexA, string ScenIndexB)
        {
            List<List<string>> defaultValue = new List<List<string>>();
            if (SceneClass == "M")
            {
                Mainload.BuildInto_m = ES3.Load<List<List<string>>>("BuildInto_m", Mainload.CunDangIndex_now + BakIndex + "/M" + ScenIndexA + ".es3", defaultValue);
                return;
            }
            if (SceneClass == "Z")
            {
                Mainload.BuildInto_z = ES3.Load<List<List<string>>>("BuildInto_z", string.Concat(new string[]
                {
                    Mainload.CunDangIndex_now + BakIndex,
                    "/Z",
                    ScenIndexA,
                    "/",
                    ScenIndexB,
                    ".es3"
                }), defaultValue);
                return;
            }
            if (SceneClass == "S")
            {
                Mainload.BuildInto_s = ES3.Load<List<List<string>>>("BuildInto_s", Mainload.CunDangIndex_now + BakIndex + "/S" + ScenIndexA + ".es3", defaultValue);
                Mainload.BuildInto_c = ES3.Load<List<List<string>>>("BuildInto_c", Mainload.CunDangIndex_now + BakIndex + "/S" + ScenIndexA + ".es3", defaultValue);
                return;
            }
            if (SceneClass == "H")
            {
                Mainload.BuildInto_h = ES3.Load<List<List<string>>>("BuildInto_h", Mainload.CunDangIndex_now + BakIndex + "/H" + ScenIndexA + ".es3", defaultValue);
            }
        }

        public static void ReadAutoArchiveSetData()
        {
            bool flag = true;
            try
            {
                ES3.Load<int>("Guide_order", "FW/SetData.es3", Mainload.Guide_order);
            }
            catch (FormatException)
            {
                flag = false;
            }
            if (flag)
            {
                Mainload.Guide_order = ES3.Load<int>("Guide_order", "FW/SetData.es3", Mainload.Guide_order);
                Mainload.Guide_Nei = ES3.Load<List<int>>("Guide_Nei", "FW/SetData.es3", Mainload.Guide_Nei);
                Mainload.IsPreGuideFinish = ES3.Load<bool>("IsPreGuideFinish", "FW/SetData.es3", Mainload.IsPreGuideFinish);
                Mainload.SetData = ES3.Load<List<int>>("SetData", "FW/SetData.es3", Mainload.SetData);
                Mainload.FastKey = ES3.Load<List<KeyCode>>("FastKey", "FW/SetData.es3", Mainload.FastKey);
                Mainload.GongGaoID_Read = ES3.Load<string>("GongGaoID_Read", "FW/SetData.es3", Mainload.GongGaoID);
                if (Mainload.SetData[4] > 1)
                {
                    Mainload.SetData[4] = 0;
                }
                if (Mainload.SetData[5] > 3)
                {
                    Mainload.SetData[5] = 1;
                }
                if (Mainload.SetData[2] > 3)
                {
                    Mainload.SetData[2] = 3;
                    return;
                }
            }
            else
            {
                ES3.DeleteFile("FW/SetData.es3");
            }
        }
    }
}