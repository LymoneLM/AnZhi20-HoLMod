using BepInEx;
using UnityEngine;
using System;
using BepInEx.Logging;
using YuanAPI;
using YuanAPI.UnityWindows;

namespace cs.HoLMod.TestTool
{
    // 插件信息类
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "cs.HoLMod.TestTool.AnZhi20";
        public const string PLUGIN_NAME = "HoLMod.TestTool";
        public const string PLUGIN_VERSION = "1.0.0";
    }

    [BepInDependency(YuanAPIPlugin.MODGUID)]
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class TestTool : BaseUnityPlugin
    {
        private ToolWindowManager windowManager;
        internal static ManualLogSource Logger;
        
        private void Awake()
        {
            // 初始化日志
            Logger = base.Logger;
            Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} 已加载");
            
            // 初始化YuanAPI的窗口系统
            Windows.Initialize();
            
            // 创建窗口管理器
            windowManager = new ToolWindowManager();
            
            Logger.LogInfo("按下F7键打开欢迎界面");
        }
        
        private void Update()
        {
            // 监听F7键
            if (Input.GetKeyDown(KeyCode.F7))
            {
                // 检查是否有窗口打开
                if (Windows.AllWindows.Count > 0)
                {
                    Logger.LogInfo("关闭所有窗口");
                    // 关闭所有窗口
                    foreach (var window in Windows.AllWindows.ToArray())
                    {
                        Windows.DestroyWindow(window.WindowId);
                    }
                }
                else
                {
                    // 调用StartAddEvents方法显示欢迎界面
                    Logger.LogInfo("打开欢迎界面");
                    windowManager.ShowWelcomeWindow();
                }
            }
            
            // 监听ESC键关闭所有窗口
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Windows.AllWindows.Count > 0)
                {
                    Logger.LogInfo("ESC键关闭所有窗口");
                    // 关闭所有窗口
                    foreach (var window in Windows.AllWindows.ToArray())
                    {
                        Windows.DestroyWindow(window.WindowId);
                    }
                }
            }
        }
        
        private void OnDestroy()
        {
            // 清理资源
            foreach (var window in Windows.AllWindows.ToArray())
            {
                Windows.DestroyWindow(window.WindowId);
            }
        }
    }
}
