using BepInEx;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;

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
        private Windows windowsManager;
        internal static ManualLogSource Logger;
        
        private void Awake()
        {
            // 初始化日志
            Logger = base.Logger;
            Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} 已加载");
            
            // 创建Windows管理器对象
            GameObject managerObj = new GameObject("WindowsManager");
            DontDestroyOnLoad(managerObj);
            windowsManager = managerObj.AddComponent<Windows>();
            windowsManager.Initialize();
            
            Logger.LogInfo("按下F7键打开欢迎界面");
        }
        
        private void Update()
        {
            // 监听F7键
            if (Input.GetKeyDown(KeyCode.F7))
            {
                // 如果有窗口打开，则关闭所有窗口
                if (Windows.IsAnyWindowOpen())
                {
                    Logger.LogInfo("关闭所有窗口");
                    Windows.CloseAllWindows();
                }
                else
                {
                    // 调用StartAddEvents方法显示欢迎界面
                    Logger.LogInfo("打开欢迎界面");
                    windowsManager.StartAddEvents();
                }
            }
            
            // 监听ESC键关闭所有窗口
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Windows.IsAnyWindowOpen())
                {
                    Logger.LogInfo("ESC键关闭所有窗口");
                    Windows.CloseAllWindows();
                }
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

        public List<List<string>> EventList = new List<List<string>>()
        {
            // 0，云游道士卖丹药
            new List<string>
            {
                "20",
                string.Concat(new string[]
                {
                    "28|1|null|5@",
                    "100",
                    "@1@",
                    "100",
                    "|0"
                })
            },

            // 1，云游道士卖符咒
            new List<string>
            {
                "20",
                string.Concat(new string[]
                {
                    "29|1|null|5@",
                    "542",
                    "@1@",
                    "100",
                    "|0"
                })
            }
        };
    }
}
