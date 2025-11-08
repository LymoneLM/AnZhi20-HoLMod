using UnityEngine;
using YuanAPI.UnityWindows;
using System.Collections.Generic;
using System;

namespace cs.HoLMod.TestTool
{
    public class ToolWindowManager
    {
        private YuanWindow welcomeWindow;
        private YuanWindow mainWindow;
        private EventData eventData;
        private int selectedEventIndex = -1;
        
        // 按钮参数点击状态字典，用于记录每个按钮参数是否被点击
        private Dictionary<string, bool> buttonParameterClickedStates = new Dictionary<string, bool>();
        
        // 按钮参数选择值字典，用于存储按钮参数选择的具体值
        private Dictionary<string, string> buttonParameterSelectedValues = new Dictionary<string, string>();
        
        // 物品选择窗口相关
        private YuanWindow itemSelectionWindow;
        private string currentButtonParameterName;
        private string currentItemType; // "DanYao" 或 "FuZhou"
        
        public ToolWindowManager()
        {
            eventData = new EventData(); // 初始化EventData实例
            TestTool.Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} 窗口管理器已初始化");
        }
        
        /// <summary>
        /// 显示欢迎窗口
        /// </summary>
        public void ShowWelcomeWindow()
        {
            TestTool.Logger.LogInfo("ShowWelcomeWindow方法开始执行");
            
            if (welcomeWindow != null)
            {
                TestTool.Logger.LogInfo("welcomeWindow已存在，显示现有窗口");
                welcomeWindow.Show();
                return;
            }
            
            try
            {
                // 使用YuanAPI创建欢迎窗口
                TestTool.Logger.LogInfo("尝试创建欢迎窗口");
                welcomeWindow = YuanAPI.UnityWindows.Windows.CreateWindow("WelcomeWindow", "事件编辑器", WindowCategory.Dialog, 400, 300);
                TestTool.Logger.LogInfo($"欢迎窗口创建成功，WindowId: {welcomeWindow.WindowId}");
                
                // 创建主区域
                TestTool.Logger.LogInfo("创建主区域");
                var mainRegion = welcomeWindow.CreateRegion("Main");
                mainRegion.SetLayout(LayoutType.Vertical);
                
                // 添加标题
                TestTool.Logger.LogInfo("添加标题文本");
                var titleText = UI.CreateText("欢迎使用事件编辑器", "WelcomeTitle");
                titleText.TextComponent.fontSize = 24;
                titleText.TextComponent.alignment = TextAnchor.MiddleCenter;
                titleText.TextComponent.color = Color.white;
                mainRegion.AddElement(titleText);
                
                // 添加版本信息
                TestTool.Logger.LogInfo("添加版本信息文本");
                var versionText = UI.CreateText($"版本: {PluginInfo.PLUGIN_VERSION}", "VersionText");
                versionText.TextComponent.fontSize = 14;
                versionText.TextComponent.alignment = TextAnchor.MiddleCenter;
                versionText.TextComponent.color = Color.gray;
                mainRegion.AddElement(versionText);
                
                // 创建按钮区域
                TestTool.Logger.LogInfo("创建按钮区域");
                var buttonRegion = welcomeWindow.CreateRegion("ButtonRegion");
                buttonRegion.SetLayout(LayoutType.Horizontal);
                
                // 添加进入按钮
                TestTool.Logger.LogInfo("添加进入按钮");
                var enterButton = UI.CreateButton("进入", () =>
                {
                    TestTool.Logger.LogInfo("进入按钮被点击");
                    welcomeWindow.Hide();
                    ShowMainWindow();
                }, "EnterButton");
                buttonRegion.AddElement(enterButton);
                
                // 显示窗口
                TestTool.Logger.LogInfo("显示欢迎窗口");
                welcomeWindow.Show();
                TestTool.Logger.LogInfo("欢迎窗口显示成功");
            }
            catch (Exception e)
            {
                TestTool.Logger.LogError($"创建或显示欢迎窗口失败: {e.Message}\n{e.StackTrace}");
            }
        }
        
        /// <summary>
        /// 显示主窗口
        /// </summary>
        public void ShowMainWindow()
        {
            TestTool.Logger.LogInfo("ShowMainWindow方法开始执行");
            
            if (mainWindow != null)
            {
                TestTool.Logger.LogInfo("mainWindow已存在，显示现有窗口");
                mainWindow.Show();
                return;
            }
            
            try
            {
                // 使用YuanAPI创建主窗口
                TestTool.Logger.LogInfo("尝试创建主窗口");
                mainWindow = YuanAPI.UnityWindows.Windows.CreateWindow("MainWindow", "事件触发器", WindowCategory.Main, 800, 600);
                TestTool.Logger.LogInfo($"主窗口创建成功，WindowId: {mainWindow.WindowId}");
                
                // 创建左侧区域用于显示事件列表
                TestTool.Logger.LogInfo("创建左侧区域");
                var leftRegion = mainWindow.CreateRegion("LeftRegion");
                leftRegion.SetLayout(LayoutType.Vertical);
                leftRegion.SetPosition(new Vector2(-200, 0));
                leftRegion.SetSize(new Vector2(350, 400).x, new Vector2(350, 400).y);
                
                // 添加事件列表标题
                TestTool.Logger.LogInfo("添加事件列表标题");
                var eventListTitle = UI.CreateText("可触发的事件列表", "EventListTitle");
                eventListTitle.TextComponent.fontSize = 16;
                eventListTitle.TextComponent.alignment = TextAnchor.MiddleCenter;
                leftRegion.AddElement(eventListTitle);
                
                // 创建事件列表滚动视图
                TestTool.Logger.LogInfo("创建事件列表滚动视图");
                var eventScrollView = UI.CreateScrollView(320, 320, "EventScrollView");
                leftRegion.AddElement(eventScrollView);
                
                // 这里需要根据实际的事件数据填充事件列表
                // 暂时添加一个占位按钮作为示例
                TestTool.Logger.LogInfo("添加测试事件按钮");
                var testEventButton = UI.CreateButton("测试事件", () =>
                {
                    TestTool.Logger.LogInfo("触发测试事件");
                    // 这里应该调用实际的事件触发逻辑
                }, "TestEventButton");
                // 将按钮添加到区域而不是滚动视图
                leftRegion.AddElement(testEventButton);
                
                // 创建右侧区域用于显示事件参数
                TestTool.Logger.LogInfo("创建右侧区域");
                var rightRegion = mainWindow.CreateRegion("RightRegion");
                rightRegion.SetLayout(LayoutType.Vertical);
                rightRegion.SetPosition(new Vector2(200, 0));
                rightRegion.SetSize(new Vector2(350, 400).x, new Vector2(350, 400).y);
                
                // 添加参数区域标题
                TestTool.Logger.LogInfo("添加参数区域标题");
                var paramTitle = UI.CreateText("事件参数设置", "ParamTitle");
                paramTitle.TextComponent.fontSize = 16;
                paramTitle.TextComponent.alignment = TextAnchor.MiddleCenter;
                rightRegion.AddElement(paramTitle);
                
                // 创建参数列表滚动视图
                TestTool.Logger.LogInfo("创建参数列表滚动视图");
                var paramScrollView = UI.CreateScrollView(320, 320, "ParamScrollView");
                rightRegion.AddElement(paramScrollView);
                
                // 创建底部按钮区域
                TestTool.Logger.LogInfo("创建底部按钮区域");
                var bottomRegion = mainWindow.CreateRegion("BottomRegion");
                bottomRegion.SetLayout(LayoutType.Horizontal);
                bottomRegion.SetPosition(new Vector2(0, -250));
                
                // 添加触发按钮
                TestTool.Logger.LogInfo("添加触发按钮");
                var triggerButton = UI.CreateButton("触发事件", () =>
                {
                    TestTool.Logger.LogInfo("触发当前选中的事件");
                    // 这里应该调用实际的事件触发逻辑
                }, "TriggerButton");
                bottomRegion.AddElement(triggerButton);
                
                // 添加关闭按钮
                TestTool.Logger.LogInfo("添加关闭按钮");
                var closeButton = UI.CreateButton("关闭", () =>
                {
                    TestTool.Logger.LogInfo("关闭主窗口");
                    mainWindow.Hide();
                }, "CloseButton");
                bottomRegion.AddElement(closeButton);
                
                // 显示窗口
                TestTool.Logger.LogInfo("显示主窗口");
                mainWindow.Show();
                TestTool.Logger.LogInfo("主窗口显示成功");
            }
            catch (Exception e)
            {
                TestTool.Logger.LogError($"创建或显示主窗口失败: {e.Message}\n{e.StackTrace}");
            }
        }
        
        /// <summary>
        /// 显示物品选择窗口
        /// </summary>
        /// <param name="buttonParameterName">按钮参数名称</param>
        /// <param name="itemType">物品类型："DanYao" 或 "FuZhou"</param>
        public void ShowItemSelectionWindow(string buttonParameterName, string itemType)
        {
            currentButtonParameterName = buttonParameterName;
            currentItemType = itemType;
            
            if (itemSelectionWindow != null)
            {
                itemSelectionWindow.Show();
                return;
            }
            
            // 使用YuanAPI创建物品选择窗口
            string windowTitle = itemType == "DanYao" ? "选择丹药" : "选择符咒";
            itemSelectionWindow = YuanAPI.UnityWindows.Windows.CreateWindow("ItemSelectionWindow", windowTitle, WindowCategory.Dialog, 500, 400);
            
            // 创建主区域
            var mainRegion = itemSelectionWindow.CreateRegion("Main");
            mainRegion.SetLayout(LayoutType.Vertical);
            
            // 创建物品列表滚动视图
            var itemScrollView = UI.CreateScrollView(450, 300, "ItemScrollView");
            mainRegion.AddElement(itemScrollView);
            
            // 这里需要根据实际的物品数据填充物品列表
            // 暂时添加一些占位按钮作为示例
            for (int i = 1; i <= 10; i++)
            {
                int index = i; // 避免闭包问题
                var itemButton = UI.CreateButton($"{itemType} {index}", () =>
                {
                    SelectItem($"{itemType}_{index}");
                }, $"ItemButton_{i}");
                // 将按钮添加到区域而不是滚动视图
                mainRegion.AddElement(itemButton);
            }
            
            // 创建底部按钮区域
            var buttonRegion = itemSelectionWindow.CreateRegion("ButtonRegion");
            buttonRegion.SetLayout(LayoutType.Horizontal);
            
            // 添加取消按钮
            var cancelButton = UI.CreateButton("取消", () =>
            {
                itemSelectionWindow.Hide();
            }, "CancelButton");
            buttonRegion.AddElement(cancelButton);
            
            // 显示窗口
            itemSelectionWindow.Show();
        }
        
        /// <summary>
        /// 选择物品
        /// </summary>
        /// <param name="itemValue">选中的物品值</param>
        private void SelectItem(string itemValue)
        {
            if (!string.IsNullOrEmpty(currentButtonParameterName))
            {
                buttonParameterSelectedValues[currentButtonParameterName] = itemValue;
                TestTool.Logger.LogInfo($"选中物品：{itemValue} 对应参数：{currentButtonParameterName}");
            }
            
            if (itemSelectionWindow != null)
            {
                itemSelectionWindow.Hide();
            }
        }
        
        /// <summary>
        /// 清理所有窗口
        /// </summary>
        public void Cleanup()
        {
            if (welcomeWindow != null)
            {
                YuanAPI.UnityWindows.Windows.DestroyWindow(welcomeWindow.WindowId);
                welcomeWindow = null;
            }
            
            if (mainWindow != null)
            {
                YuanAPI.UnityWindows.Windows.DestroyWindow(mainWindow.WindowId);
                mainWindow = null;
            }
            
            if (itemSelectionWindow != null)
            {
                YuanAPI.UnityWindows.Windows.DestroyWindow(itemSelectionWindow.WindowId);
                itemSelectionWindow = null;
            }
        }
    }
}