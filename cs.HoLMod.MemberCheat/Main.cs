using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HoLMod;

namespace cs.HoLMod.MemberCheat
{
    // 插件信息类
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "cs.HoLMod.MemberCheat.AnZhi20";
        public const string PLUGIN_NAME = "HoLMod.MemberCheat";
        public const string PLUGIN_VERSION = "3.0.0";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MenberCheat : BaseUnityPlugin
    {
        // 加载ab包
    }
}
