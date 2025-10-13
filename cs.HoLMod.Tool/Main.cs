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
            gameObject.AddComponent<WindowMain>();
            
        }
        
    }
}