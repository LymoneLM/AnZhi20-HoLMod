using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using BepInEx.Logging;

namespace cs.HoLMod.AddEvents
{
    public class Windows : MonoBehaviour
    {
        private static Windows instance;
        private static List<GameObject> activeWindows = new List<GameObject>();
        
        private GameObject welcomeWindow;
        private GameObject mainWindow;
        
        private AddEvents pluginInstance;
        
        // 初始化方法
        public void Initialize()
        {
            instance = this;
            pluginInstance = FindObjectOfType<AddEvents>();
            Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} 窗口管理器已初始化");
        }
        
        // 显示欢迎界面
        public void StartAddEvents()
        {
            CreateWelcomeWindow();
        }
        
        // 创建欢迎界面
        private void CreateWelcomeWindow()
        {
            // 创建欢迎窗口对象
            welcomeWindow = new GameObject("WelcomeWindow");
            Canvas canvas = welcomeWindow.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            welcomeWindow.AddComponent<CanvasScaler>();
            welcomeWindow.AddComponent<GraphicRaycaster>();
            
            // 设置窗口背景
            GameObject background = new GameObject("Background");
            background.transform.SetParent(welcomeWindow.transform, false);
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(400, 300);
            bgRect.anchoredPosition = Vector2.zero;
            
            // 添加标题
            GameObject title = new GameObject("Title");
            title.transform.SetParent(background.transform, false);
            Text titleText = title.AddComponent<Text>();
            titleText.text = "欢迎使用事件编辑器";
            titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleText.fontSize = 24;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(300, 50);
            titleRect.anchoredPosition = new Vector2(0, 50);
            
            // 添加版本信息
            GameObject version = new GameObject("Version");
            version.transform.SetParent(background.transform, false);
            Text versionText = version.AddComponent<Text>();
            versionText.text = $"版本: {PluginInfo.PLUGIN_VERSION}";
            versionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            versionText.fontSize = 14;
            versionText.alignment = TextAnchor.MiddleCenter;
            versionText.color = Color.gray;
            RectTransform versionRect = version.GetComponent<RectTransform>();
            versionRect.sizeDelta = new Vector2(200, 30);
            versionRect.anchoredPosition = new Vector2(0, 0);
            
            // 添加进入按钮
            GameObject enterButton = new GameObject("EnterButton");
            enterButton.transform.SetParent(background.transform, false);
            Button button = enterButton.AddComponent<Button>();
            Image buttonImage = enterButton.GetComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.6f, 1f);
            
            // 添加按钮文本
            GameObject buttonTextObj = new GameObject("ButtonText");
            buttonTextObj.transform.SetParent(enterButton.transform, false);
            Text buttonText = buttonTextObj.AddComponent<Text>();
            buttonText.text = "进入";
            buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buttonText.fontSize = 16;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.white;
            RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
            buttonTextRect.sizeDelta = new Vector2(100, 30);
            buttonTextRect.anchoredPosition = Vector2.zero;
            
            // 设置按钮大小和位置
            RectTransform buttonRect = enterButton.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(120, 40);
            buttonRect.anchoredPosition = new Vector2(0, -50);
            
            // 添加按钮点击事件
            button.onClick.AddListener(() => 
            {
                CloseWelcomeWindow();
                CreateMainWindow();
            });
            
            // 添加窗口到活动窗口列表
            activeWindows.Add(welcomeWindow);
        }
        
        // 关闭欢迎界面
        private void CloseWelcomeWindow()
        {
            if (welcomeWindow != null)
            {
                activeWindows.Remove(welcomeWindow);
                Destroy(welcomeWindow);
                welcomeWindow = null;
            }
        }
        
        // 创建主窗口
        private void CreateMainWindow()
        {
            // 创建主窗口对象
            mainWindow = new GameObject("MainWindow");
            Canvas canvas = mainWindow.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainWindow.AddComponent<CanvasScaler>();
            mainWindow.AddComponent<GraphicRaycaster>();
            
            // 设置窗口背景
            GameObject background = new GameObject("Background");
            background.transform.SetParent(mainWindow.transform, false);
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(800, 600);
            bgRect.anchoredPosition = Vector2.zero;
            
            // 添加标题
            GameObject title = new GameObject("Title");
            title.transform.SetParent(background.transform, false);
            Text titleText = title.AddComponent<Text>();
            titleText.text = "事件触发器";
            titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleText.fontSize = 20;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(200, 40);
            titleRect.anchoredPosition = new Vector2(0, 250);
            
            // 创建左侧可触发事件列表区域
            GameObject eventListPanel = new GameObject("EventListPanel");
            eventListPanel.transform.SetParent(background.transform, false);
            Image eventListImage = eventListPanel.AddComponent<Image>();
            eventListImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            RectTransform eventListRect = eventListPanel.GetComponent<RectTransform>();
            eventListRect.sizeDelta = new Vector2(350, 400);
            eventListRect.anchoredPosition = new Vector2(-200, 0);
            
            // 添加事件列表标题
            GameObject eventListTitle = new GameObject("EventListTitle");
            eventListTitle.transform.SetParent(eventListPanel.transform, false);
            Text eventListTitleText = eventListTitle.AddComponent<Text>();
            eventListTitleText.text = "可触发的事件列表";
            eventListTitleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            eventListTitleText.fontSize = 16;
            eventListTitleText.alignment = TextAnchor.MiddleCenter;
            eventListTitleText.color = Color.white;
            RectTransform eventListTitleRect = eventListTitle.GetComponent<RectTransform>();
            eventListTitleRect.sizeDelta = new Vector2(300, 30);
            eventListTitleRect.anchoredPosition = new Vector2(0, 170);
            
            // 创建事件列表内容区域
            GameObject eventListContent = new GameObject("EventListContent");
            eventListContent.transform.SetParent(eventListPanel.transform, false);
            RectTransform eventListContentRect = eventListContent.AddComponent<RectTransform>();
            eventListContentRect.sizeDelta = new Vector2(320, 320);
            eventListContentRect.anchoredPosition = Vector2.zero;
            
            // 添加事件列表项
            if (pluginInstance != null && pluginInstance.EventList != null)
            {
                for (int i = 0; i < pluginInstance.EventList.Count; i++)
                {
                    int index = i; // 捕获当前索引
                    GameObject eventItem = new GameObject($"EventItem_{index}");
                    eventItem.transform.SetParent(eventListContent.transform, false);
                    Button eventButton = eventItem.AddComponent<Button>();
                    Image eventButtonImage = eventItem.GetComponent<Image>();
                    eventButtonImage.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);
                    
                    // 添加事件文本
                    GameObject eventTextObj = new GameObject("EventText");
                    eventTextObj.transform.SetParent(eventItem.transform, false);
                    Text eventText = eventTextObj.AddComponent<Text>();
                    eventText.text = $"事件 {index + 1}";
                    eventText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    eventText.fontSize = 14;
                    eventText.alignment = TextAnchor.MiddleLeft;
                    eventText.color = Color.white;
                    RectTransform eventTextRect = eventTextObj.GetComponent<RectTransform>();
                    eventTextRect.sizeDelta = new Vector2(280, 30);
                    eventTextRect.anchoredPosition = new Vector2(10, 0);
                    
                    // 设置按钮大小和位置
                    RectTransform eventButtonRect = eventItem.GetComponent<RectTransform>();
                    eventButtonRect.sizeDelta = new Vector2(300, 40);
                    eventButtonRect.anchoredPosition = new Vector2(0, 120 - index * 50);
                    
                    // 添加选中效果
                    eventButton.onClick.AddListener(() => 
                    {
                        // 重置所有按钮颜色
                        foreach (Transform child in eventListContent.transform)
                        {
                            if (child.TryGetComponent<Image>(out Image img))
                            {
                                img.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);
                            }
                        }
                        // 高亮选中的按钮
                        eventButtonImage.color = new Color(0.2f, 0.6f, 1f, 0.9f);
                        // 这里可以添加选中事件后的逻辑
                        Logger.LogInfo($"选中事件: {index}");
                    });
                }
            }
            
            // 创建右侧可修改参数列表区域
            GameObject paramListPanel = new GameObject("ParamListPanel");
            paramListPanel.transform.SetParent(background.transform, false);
            Image paramListImage = paramListPanel.AddComponent<Image>();
            paramListImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            RectTransform paramListRect = paramListPanel.GetComponent<RectTransform>();
            paramListRect.sizeDelta = new Vector2(350, 400);
            paramListRect.anchoredPosition = new Vector2(200, 0);
            
            // 添加参数列表标题
            GameObject paramListTitle = new GameObject("ParamListTitle");
            paramListTitle.transform.SetParent(paramListPanel.transform, false);
            Text paramListTitleText = paramListTitle.AddComponent<Text>();
            paramListTitleText.text = "可修改参数列表";
            paramListTitleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            paramListTitleText.fontSize = 16;
            paramListTitleText.alignment = TextAnchor.MiddleCenter;
            paramListTitleText.color = Color.white;
            RectTransform paramListTitleRect = paramListTitle.GetComponent<RectTransform>();
            paramListTitleRect.sizeDelta = new Vector2(300, 30);
            paramListTitleRect.anchoredPosition = new Vector2(0, 170);
            
            // 创建参数列表内容区域
            GameObject paramListContent = new GameObject("ParamListContent");
            paramListContent.transform.SetParent(paramListPanel.transform, false);
            RectTransform paramListContentRect = paramListContent.AddComponent<RectTransform>();
            paramListContentRect.sizeDelta = new Vector2(320, 320);
            paramListContentRect.anchoredPosition = Vector2.zero;
            
            // 添加示例参数项
            AddParamItem(paramListContent, "参数1", "数值", 1, new Vector2(0, 100));
            AddParamItem(paramListContent, "参数2", "文本", 2, new Vector2(0, 40));
            AddParamItem(paramListContent, "参数3", "布尔值", 3, new Vector2(0, -20));
            
            // 添加底部触发事件按钮
            GameObject triggerButton = new GameObject("TriggerButton");
            triggerButton.transform.SetParent(background.transform, false);
            Button button = triggerButton.AddComponent<Button>();
            Image buttonImage = triggerButton.GetComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.8f, 0.3f);
            
            // 添加按钮文本
            GameObject buttonTextObj = new GameObject("ButtonText");
            buttonTextObj.transform.SetParent(triggerButton.transform, false);
            Text buttonText = buttonTextObj.AddComponent<Text>();
            buttonText.text = "触发事件";
            buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buttonText.fontSize = 18;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.white;
            RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
            buttonTextRect.sizeDelta = new Vector2(150, 40);
            buttonTextRect.anchoredPosition = Vector2.zero;
            
            // 设置按钮大小和位置
            RectTransform buttonRect = triggerButton.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(180, 50);
            buttonRect.anchoredPosition = new Vector2(0, -250);
            
            // 添加按钮点击事件
            button.onClick.AddListener(() => 
            {
                Logger.LogInfo("触发事件按钮被点击");
                // 这里可以添加触发事件的逻辑
            });
            
            // 添加窗口到活动窗口列表
            activeWindows.Add(mainWindow);
        }
        
        // 添加参数项
        private void AddParamItem(GameObject parent, string paramName, string paramType, int index, Vector2 position)
        {
            GameObject paramItem = new GameObject($"ParamItem_{index}");
            paramItem.transform.SetParent(parent.transform, false);
            
            // 添加背景
            Image paramBg = paramItem.AddComponent<Image>();
            paramBg.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);
            
            // 设置大小和位置
            RectTransform paramRect = paramItem.GetComponent<RectTransform>();
            paramRect.sizeDelta = new Vector2(300, 50);
            paramRect.anchoredPosition = position;
            
            // 添加参数名称
            GameObject nameObj = new GameObject("ParamName");
            nameObj.transform.SetParent(paramItem.transform, false);
            Text nameText = nameObj.AddComponent<Text>();
            nameText.text = paramName;
            nameText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            nameText.fontSize = 14;
            nameText.alignment = TextAnchor.MiddleLeft;
            nameText.color = Color.white;
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.sizeDelta = new Vector2(100, 30);
            nameRect.anchoredPosition = new Vector2(-80, 0);
            
            // 添加输入框
            GameObject inputObj = new GameObject("ParamInput");
            inputObj.transform.SetParent(paramItem.transform, false);
            InputField inputField = inputObj.AddComponent<InputField>();
            
            // 设置输入框大小和位置
            RectTransform inputRect = inputObj.GetComponent<RectTransform>();
            inputRect.sizeDelta = new Vector2(150, 30);
            inputRect.anchoredPosition = new Vector2(40, 0);
            
            // 设置输入框文本组件
            GameObject inputTextObj = new GameObject("Text");
            inputTextObj.transform.SetParent(inputObj.transform, false);
            Text inputText = inputTextObj.AddComponent<Text>();
            inputText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            inputText.fontSize = 14;
            inputText.alignment = TextAnchor.MiddleLeft;
            inputText.color = Color.black;
            RectTransform inputTextRect = inputTextObj.GetComponent<RectTransform>();
            inputTextRect.sizeDelta = new Vector2(140, 20);
            inputTextRect.anchoredPosition = new Vector2(5, 0);
            
            // 设置输入框占位符
            GameObject placeholderObj = new GameObject("Placeholder");
            placeholderObj.transform.SetParent(inputObj.transform, false);
            Text placeholderText = placeholderObj.AddComponent<Text>();
            placeholderText.text = paramType;
            placeholderText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            placeholderText.fontSize = 14;
            placeholderText.alignment = TextAnchor.MiddleLeft;
            placeholderText.color = Color.gray;
            RectTransform placeholderRect = placeholderObj.GetComponent<RectTransform>();
            placeholderRect.sizeDelta = new Vector2(140, 20);
            placeholderRect.anchoredPosition = new Vector2(5, 0);
            
            // 配置InputField组件
            inputField.textComponent = inputText;
            inputField.placeholder = placeholderText;
            inputField.text = "";
            inputField.characterLimit = 50;
            inputField.lineType = InputField.LineType.SingleLine;
        }
        
        // 关闭主窗口
        private void CloseMainWindow()
        {
            if (mainWindow != null)
            {
                activeWindows.Remove(mainWindow);
                Destroy(mainWindow);
                mainWindow = null;
            }
        }
        
        // 检查是否有窗口打开
        public static bool IsAnyWindowOpen()
        {
            return activeWindows.Count > 0;
        }
        
        // 关闭所有窗口
        public static void CloseAllWindows()
        {
            foreach (GameObject window in activeWindows.ToArray())
            {
                if (window != null)
                {
                    Destroy(window);
                }
            }
            activeWindows.Clear();
            
            if (instance != null)
            {
                instance.welcomeWindow = null;
                instance.mainWindow = null;
            }
        }
    }
}