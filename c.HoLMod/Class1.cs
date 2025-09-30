using System;
using System.Collections.Generic;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace 吾今有世家农场修改
{
    [BepInPlugin("HoLMod.AnZhi20", "HoLMod.FarmCheat", "1.0.0")]
    public class  HoLMod_FarmCheat : BaseUnityPlugin
    {
        private void Awake()
        {
            HoLMod_FarmCheat.庄头年龄修改启用 = base.Config.Bind<int>("庄头年龄调整", "修改启用", 0, "是否启用本大类下列修改，1为启用，0为停用");
            HoLMod_FarmCheat.庄头年龄 = base.Config.Bind<int>("庄头年龄调整", "庄头年龄", 0, "庄头年龄固定每年1月1日修改为设定值，填0为不修改");

            HoLMod_FarmCheat.庄头管理修改启用 = base.Config.Bind<int>("庄头管理调整", "修改启用", 0, "是否启用本大类下列修改，1为启用，0为停用");
            HoLMod_FarmCheat.庄头管理 = base.Config.Bind<int>("庄头管理调整", "庄头管理", 0, "庄头管理固定每年1月1日修改为设定值，填0为不修改");

            HoLMod_FarmCheat.庄头忠诚度修改启用 = base.Config.Bind<int>("庄头忠诚度调整", "修改启用", 0, "是否启用本大类下列修改，1为启用，0为停用");
            HoLMod_FarmCheat.庄头忠诚度 = base.Config.Bind<int>("庄头忠诚度调整", "庄头忠诚度", 0, "庄头忠诚度固定每年1月1日修改为设定值，填0为不修改");
            
            Harmony.CreateAndPatchAll(typeof(HoLMod_FarmCheat), null);
            base.Logger.LogError("欢迎使用吾今有世家农场修改MOD，作者：AnZhi20");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ZhuangTouInfoPanel), "ShouMingNum")]
        public static bool ZhuangTouInfoPanel_ShouMingNum(ZhuangTouInfoPanel __instance)
        {
            bool flag = HoLMod_FarmCheat.庄头年龄修改启用.Value == 1;
            bool result;
            if (flag)
            {
                Traverse.Create(__instance).Field("SexID").SetValue("1");
                Traverse.Create(__instance).Field("CiTaio_Point_Have").SetValue(100);
                Traverse.Create(__instance).Field("TianFuData").SetValue(new List<int>
                {
                    0,
                    0
                });
                Traverse.Create(__instance).Field("AllShuxingCiTiao_Index").SetValue(new List<int>());
                Traverse.Create(__instance).Field("CreateShuXing_CiTiao").SetValue(new List<int>());
                for (int i = 0; i < Mainload.ShuXingCiTiao_Member.Count; i++)
                {
                    Traverse.Create(__instance).Field("AllShuxingCiTiao_Index").GetValue<List<int>>().Add(1);
                    Traverse.Create(__instance).Field("CreateShuXing_CiTiao").GetValue<List<int>>().Add(PluginMain.族人属性阈值1.Value / 4 + 1);
                }
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }

        public static ConfigEntry<int> 庄头年龄修改启用;
        public static ConfigEntry<int> 庄头年龄;
        public static ConfigEntry<int> 庄头管理修改启用;
        public static ConfigEntry<int> 庄头管理;
        public static ConfigEntry<int> 庄头忠诚度修改启用;
        public static ConfigEntry<int> 庄头忠诚度;
    }
}
