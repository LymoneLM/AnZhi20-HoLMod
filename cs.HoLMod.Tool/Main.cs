using BepInEx;
using UnityEngine;
using System;
using BepInEx.Logging;
using YuanAPI;
using YuanAPI.UnityWindows;
using HarmonyLib;

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
        public static ManualLogSource Logger;
        private ToolWindowManager toolWindowManager;
        private bool isInitialized = false;
        
        private void Awake()
        {
            try
            {
                // 初始化日志
                Logger = base.Logger;
                Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} 开始初始化");
                
                // 检查YuanAPI是否可用
                try
                {
                    // 尝试访问YuanAPI窗口系统来验证可用性
                    var windowsCount = YuanAPI.UnityWindows.Windows.AllWindows.Count;
                    Logger.LogInfo($"YuanAPI窗口系统可用，当前窗口数: {windowsCount}");
                }
                catch
                {
                    Logger.LogError("YuanAPI窗口系统不可用！");
                    return;
                }
                
                // 初始化YuanAPI的窗口系统
                Logger.LogInfo("初始化YuanAPI窗口系统");
                YuanAPI.UnityWindows.Windows.Initialize();
                
                // 创建新的ToolWindowManager
                Logger.LogInfo("创建ToolWindowManager实例");
                toolWindowManager = new ToolWindowManager();
                
                // 使用Harmony打补丁
                Logger.LogInfo("应用Harmony补丁");
                Harmony.CreateAndPatchAll(typeof(TestTool));
                
                isInitialized = true;
                Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} 初始化完成");
                Logger.LogInfo("按下F7键打开欢迎界面");
            }
            catch (Exception e)
            {
                Logger.LogError($"插件初始化失败: {e.Message}\n{e.StackTrace}");
                isInitialized = false;
            }
        }
        
        private void Update()
        {
            // 监听F7键
            if (Input.GetKeyDown(KeyCode.F7))
            {
                Logger.LogInfo("检测到F7键按下");
                
                // 检查插件是否已初始化
                if (!isInitialized)
                {
                    Logger.LogError("插件未正确初始化！无法打开窗口");
                    return;
                }
                
                // 检查toolWindowManager是否已初始化
                if (toolWindowManager == null)
                {
                    Logger.LogError("toolWindowManager未初始化！尝试重新初始化");
                    try
                    {
                        toolWindowManager = new ToolWindowManager();
                        Logger.LogInfo("toolWindowManager重新初始化成功");
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"重新初始化toolWindowManager失败: {e.Message}");
                        return;
                    }
                }
                
                // 检查YuanAPI窗口系统是否可用
                try
                {
                    // 尝试访问YuanAPI窗口系统来验证可用性
                    var windowsCount = YuanAPI.UnityWindows.Windows.AllWindows.Count;
                    Logger.LogInfo($"YuanAPI窗口系统可用，当前窗口数: {windowsCount}");
                }
                catch
                {
                    Logger.LogError("YuanAPI窗口系统不可用！尝试重新初始化");
                    try
                    {
                        YuanAPI.UnityWindows.Windows.Initialize();
                        Logger.LogInfo("YuanAPI窗口系统重新初始化成功");
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"重新初始化YuanAPI窗口系统失败: {e.Message}");
                        return;
                    }
                }
                
                // 检查是否有窗口打开
                bool anyWindowOpen = false;
                try
                {
                    anyWindowOpen = cs.HoLMod.TestTool.Windows.IsAnyWindowOpen();
                    Logger.LogInfo($"窗口状态检查: {(anyWindowOpen ? "有窗口打开" : "无窗口打开")}");
                }
                catch (Exception e)
                {
                    Logger.LogError($"检查窗口状态失败: {e.Message}");
                    anyWindowOpen = false;
                }
                
                if (anyWindowOpen)
                {
                    Logger.LogInfo("关闭所有窗口");
                    try
                    {
                        cs.HoLMod.TestTool.Windows.CloseAllWindows();
                        Logger.LogInfo("所有窗口已关闭");
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"关闭窗口失败: {e.Message}");
                    }
                }
                else
                {
                    // 调用ShowWelcomeWindow方法显示欢迎界面
                    Logger.LogInfo("尝试打开欢迎界面");
                    try
                    {
                        toolWindowManager.ShowWelcomeWindow();
                        Logger.LogInfo("ShowWelcomeWindow方法调用成功");
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"ShowWelcomeWindow方法调用失败: {e.Message}\n{e.StackTrace}");
                    }
                }
            }
            
            // 监听ESC键关闭所有窗口
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Logger.LogInfo("检测到ESC键按下");
                try
                {
                    if (cs.HoLMod.TestTool.Windows.IsAnyWindowOpen())
                    {
                        Logger.LogInfo("ESC键关闭所有窗口");
                        cs.HoLMod.TestTool.Windows.CloseAllWindows();
                        Logger.LogInfo("所有窗口已关闭");
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError($"ESC键关闭窗口失败: {e.Message}");
                }
            }
        }
        
        private void OnDestroy()
        {
            // 清理窗口
            cs.HoLMod.TestTool.Windows.OnDestroy();
        }
        
        // Harmony补丁方法 - 示例
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mainload), "Awake")]
        public static void Mainload_Awake_Postfix()
        {
            Logger.LogInfo("Mainload已初始化");
        }
    }
}
