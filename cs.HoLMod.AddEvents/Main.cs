using BepInEx;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs.HoLMod.AddEvents
{
    // 插件信息类
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "cs.HoLMod.AddEvents.AnZhi20";
        public const string PLUGIN_NAME = "HoLMod.AddEvents";
        public const string PLUGIN_VERSION = "1.0.0";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class AddEvents : BaseUnityPlugin
    {
        private GameObject windowsManager;
        
        private void Awake()
        {
            // 初始化日志
            Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} 已加载");
            
            // 创建Windows管理器对象
            windowsManager = new GameObject("WindowsManager");
            DontDestroyOnLoad(windowsManager);
            windowsManager.AddComponent<Windows>();
            
            Logger.LogInfo("按下F7键打开欢迎界面");
        }
        
        private void Update()
        {
            // 监听F7键
            if (Input.GetKeyDown(KeyCode.F7))
            {
                if (Windows.IsAnyWindowOpen())
                {
                    // 如果已有窗口打开，先关闭所有窗口
                    Windows.CloseAllWindows();
                }
                else
                {
                    // 否则打开欢迎界面
                    Windows.CreateWelcomeWindow();
                }
            }
            
            // 监听ESC键关闭所有窗口
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Windows.CloseAllWindows();
            }
        }
        
        private void OnDestroy()
        {
            // 清理资源
            Windows.CloseAllWindows();
            if (windowsManager != null)
            {
                Destroy(windowsManager);
            }
        }
    }
}
