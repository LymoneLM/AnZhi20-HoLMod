using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using YuanAPI.UnityWindows;

namespace cs.HoLMod.TestTool
{
    /// <summary>
    /// 窗口管理器兼容类
    /// 保留部分静态方法以确保向后兼容性
    /// </summary>
    public static class Windows
    {
        // 显示欢迎界面 (保留以确保向后兼容性)
        public static void StartAddEvents()
        {
            // 此处不再使用旧的实现，而是通过TestTool中的ToolWindowManager来显示窗口
            TestTool.Logger.LogWarning("Windows.StartAddEvents() 已弃用，请使用 ToolWindowManager.ShowWelcomeWindow()");
        }
        
        /// <summary>
        /// 检查是否有任何窗口打开
        /// </summary>
        /// <returns>如果有窗口打开则返回true，否则返回false</returns>
        public static bool IsAnyWindowOpen()
        {
            return YuanAPI.UnityWindows.Windows.AllWindows.Count > 0;
        }
        
        /// <summary>
        /// 关闭所有窗口
        /// </summary>
        public static void CloseAllWindows()
        {
            foreach (var window in YuanAPI.UnityWindows.Windows.AllWindows.ToArray())
            {
                YuanAPI.UnityWindows.Windows.DestroyWindow(window.WindowId);
            }
        }
        
        /// <summary>
        /// 清理资源
        /// </summary>
        public static void OnDestroy()
        {
            CloseAllWindows();
        }
    }
}