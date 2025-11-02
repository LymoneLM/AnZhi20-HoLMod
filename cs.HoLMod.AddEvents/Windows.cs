using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace cs.HoLMod.AddEvents
{
    public class Windows : MonoBehaviour
    {
        private static Windows instance;
        private static List<GameObject> activeWindows = new List<GameObject>();
        
        private GameObject welcomeWindow;
        private GameObject mainWindow;
        
        private AddEvents pluginInstance;
        private int selectedEventIndex = -1;
        private GameObject paramListContent; // 存储参数列表容器引用
        
        // 按钮参数点击状态字典，用于记录每个按钮参数是否被点击
        private Dictionary<string, bool> buttonParameterClickedStates = new Dictionary<string, bool>();
        
        // 按钮参数选择值字典，用于存储按钮参数选择的具体值
        private Dictionary<string, string> buttonParameterSelectedValues = new Dictionary<string, string>();
        
        // 物品选择窗口相关
        private GameObject itemSelectionWindow;
        private GameObject itemListContent;
        private string currentButtonParameterName;
        private string currentItemType; // "DanYao" 或 "FuZhou"
        
        // 初始化方法
        public void Initialize()
        {
            instance = this;
            pluginInstance = FindObjectOfType<AddEvents>();
            AddEvents.Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} 窗口管理器已初始化");
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
            Image buttonImage = enterButton.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.6f, 1f);
            Button button = enterButton.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            
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
            if (EventData.EventList != null)
            {
                for (int i = 0; i < EventData.EventList.Count; i++)
                {
                    int index = i; // 捕获当前索引
                    GameObject eventItem = new GameObject($"EventItem_{index}");
                    eventItem.transform.SetParent(eventListContent.transform, false);
                    Image eventButtonImage = eventItem.AddComponent<Image>();
                    eventButtonImage.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);
                    Button eventButton = eventItem.AddComponent<Button>();
                    eventButton.targetGraphic = eventButtonImage;
                    
                    // 添加事件文本
                    GameObject eventTextObj = new GameObject("EventText");
                    eventTextObj.transform.SetParent(eventItem.transform, false);
                    Text eventText = eventTextObj.AddComponent<Text>();
                    eventText.text = EventData.EventList[index];
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
                        
                        // 更新选中的事件索引
                        selectedEventIndex = index;
                        AddEvents.Logger.LogInfo($"选中事件: {EventData.EventList[index]} (索引: {index})");
                        
                        // 更新参数列表
                        UpdateParameterList();
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
            paramListContent = new GameObject("ParamListContent");
            paramListContent.transform.SetParent(paramListPanel.transform, false);
            RectTransform paramListContentRect = paramListContent.AddComponent<RectTransform>();
            paramListContentRect.sizeDelta = new Vector2(320, 320);
            paramListContentRect.anchoredPosition = Vector2.zero;
            
            // 添加初始提示文本
            GameObject tipTextObj = new GameObject("TipText");
            tipTextObj.transform.SetParent(paramListContent.transform, false);
            Text tipText = tipTextObj.AddComponent<Text>();
            tipText.text = "请先选择左侧的事件"; 
            tipText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            tipText.fontSize = 16;
            tipText.alignment = TextAnchor.MiddleCenter;
            tipText.color = Color.gray;
            RectTransform tipTextRect = tipTextObj.GetComponent<RectTransform>();
            tipTextRect.sizeDelta = new Vector2(200, 50);
            tipTextRect.anchoredPosition = Vector2.zero;
            
            // 添加底部触发事件按钮
            GameObject triggerButton = new GameObject("TriggerButton");
            triggerButton.transform.SetParent(background.transform, false);
            Image buttonImage = triggerButton.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.8f, 0.3f);
            Button button = triggerButton.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            
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
                if (selectedEventIndex >= 0 && selectedEventIndex < EventData.EventParams.Count)
                {
                    // 创建参数映射字典，用于存储所有参数名和对应的值
                    Dictionary<string, string> paramValues = new Dictionary<string, string>();
                    
                    // 遍历参数列表内容，查找所有参数
                    foreach (Transform child in paramListContent.transform)
                    {
                        if (child.name.StartsWith("ParamItem_"))
                        {
                            // 查找参数名称文本
                            Transform nameObj = child.Find("ParamName");
                            if (nameObj != null)
                            {
                                Text nameText = nameObj.GetComponent<Text>();
                                if (nameText != null)
                                {
                                    string paramName = nameText.text;
                                    string paramValue = "";
                                    
                                    // 查找对应的输入控件
                                    Transform inputObj = child.Find("ParamInput");
                                    if (inputObj != null)
                                    {
                                        // 检查是否为InputField类型（文本输入）
                                        InputField inputField = inputObj.GetComponent<InputField>();
                                        if (inputField != null)
                                        {
                                            paramValue = inputField.text;
                                        }
                                        // 检查是否为Button类型（按钮参数）
                                        Button paramButton = inputObj.GetComponent<Button>();
                                        if (paramButton != null)
                                        {
                                            // 从按钮选择值字典中获取值
                                            if (buttonParameterSelectedValues.TryGetValue(paramName, out string selectedValue))
                                            {
                                                paramValue = selectedValue;
                                            }
                                            else
                                            {
                                                paramValue = "" ; // 默认未选择
                                            }
                                            
                                            // 检查参数名是否为ButtonParameter类型
                                            if (paramName.StartsWith("按钮参数"))
                                            {
                                                AddEvents.Logger.LogDebug($"找到按钮参数: {paramName}, 选中值: {paramValue}");
                                            }
                                        }
                                    }
                                    
                                    // 将参数名和值添加到字典中
                                    paramValues[paramName] = paramValue;
                                    AddEvents.Logger.LogDebug($"找到参数: {paramName}, 值: {paramValue}");
                                }
                            }
                        }
                    }
                    
                    // 创建一个新的参数列表副本
                    List<string> updatedParams = new List<string>(EventData.EventParams[selectedEventIndex]);
                    
                    // 更新参数中的所有占位符
                    if (updatedParams.Count > 1)
                    {
                        string paramString = updatedParams[1];
                        
                        // 替换所有找到的参数占位符
                        foreach (var param in paramValues)
                        {
                            // 替换原始参数名（如"文本参数A"）
                            paramString = paramString.Replace(param.Key, param.Value);
                            
                            // 同时替换可能的内部占位符名称（如"TextParameterA"）
                            // 这里使用反射查找所有以"TextParameter"开头的静态字段
                            foreach (var field in typeof(EventData).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy))
                            {
                                if (field.Name.StartsWith("TextParameter") && field.FieldType == typeof(string))
                                {
                                    string fieldValue = (string)field.GetValue(null);
                                    if (fieldValue == param.Key)
                                    {
                                        paramString = paramString.Replace(field.Name, param.Value);
                                    }
                                }
                            }
                        }
                        
                        updatedParams[1] = paramString;
                    }
                    
                    // 添加更新后的参数到事件列表
                    Mainload.Event_now.Add(updatedParams);

                    // 关闭所有窗口
                    CloseAllWindows();
                }
                else
                {
                    AddEvents.Logger.LogWarning("请先选择要触发的事件");
                }
            });
            
            // 添加窗口到活动窗口列表
            activeWindows.Add(mainWindow);
        }
        
        // 添加参数项
        private void AddParamItem(GameObject parent, string paramName, string paramType, int index, Vector2 position)
        {            
            // 初始化按钮参数点击状态和选择值
            if (paramName.StartsWith("按钮参数"))
            {
                buttonParameterClickedStates[paramName] = false;
                buttonParameterSelectedValues[paramName] = "";
            }
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
            
            // 根据参数类型创建不同的输入控件
            GameObject inputObj = new GameObject("ParamInput");
            inputObj.transform.SetParent(paramItem.transform, false);
            
            if (paramName.StartsWith("按钮参数"))
            {                
                // 创建按钮控件
                Image buttonImage = inputObj.AddComponent<Image>();
                buttonImage.color = new Color(0.2f, 0.6f, 1f, 0.9f); // 蓝色背景
                Button button = inputObj.AddComponent<Button>();
                button.targetGraphic = buttonImage;
                
                // 添加按钮文本
                GameObject buttonTextObj = new GameObject("ButtonText");
                buttonTextObj.transform.SetParent(inputObj.transform, false);
                Text buttonText = buttonTextObj.AddComponent<Text>();
                buttonText.text = "点击选择";
                buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                buttonText.fontSize = 14;
                buttonText.alignment = TextAnchor.MiddleCenter;
                buttonText.color = Color.white;
                RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
                buttonTextRect.sizeDelta = new Vector2(140, 20);
                buttonTextRect.anchoredPosition = new Vector2(5, 0);
                
                // 添加按钮点击事件
                button.onClick.AddListener(() => {
                    // 根据选中的事件类型确定显示丹药还是符咒列表
                    string itemType = "";
                    if (selectedEventIndex >= 0 && selectedEventIndex < EventData.EventList.Count)
                    {
                        string eventName = EventData.EventList[selectedEventIndex];
                        if (eventName.Contains("丹药"))
                        {
                            itemType = "DanYao";
                        }
                        else if (eventName.Contains("符咒"))
                        {
                            itemType = "FuZhou";
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(itemType))
                    {
                        // 打开物品选择窗口
                        OpenItemSelectionWindow(paramName, itemType);
                    }
                    else
                    {
                        AddEvents.Logger.LogWarning("无法确定物品类型，请先选择有效的事件");
                    }
                });
            }
            else
            {
                // 创建文本输入框（默认）
                Image inputImage = inputObj.AddComponent<Image>();
                inputImage.color = new Color(1f, 1f, 1f, 1f); // 白色背景
                InputField inputField = inputObj.AddComponent<InputField>();
                inputField.targetGraphic = inputImage;
                
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
            
            // 设置输入控件大小和位置
            RectTransform inputRect = inputObj.GetComponent<RectTransform>();
            inputRect.sizeDelta = new Vector2(150, 30);
            inputRect.anchoredPosition = new Vector2(40, 0);
        }
        
        // 更新参数列表
        private void UpdateParameterList()
        {
            // 清除现有参数项
            foreach (Transform child in paramListContent.transform)
            {
                Destroy(child.gameObject);
            }
            
            // 检查是否有选中的事件
            if (selectedEventIndex >= 0 && selectedEventIndex < EventData.EventParams.Count)
            {
                // 显示两个参数：文本参数A和文本参数B
                AddParamItem(paramListContent, EventData.TextParameterA, "文本", 1, new Vector2(0, 50));
                AddParamItem(paramListContent, EventData.TextParameterB, "文本", 2, new Vector2(0, -20));
                
                AddEvents.Logger.LogInfo($"已更新参数列表，显示 {EventData.EventList[selectedEventIndex]} 的参数");
            }
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
        
        // 打开物品选择窗口
        private void OpenItemSelectionWindow(string paramName, string itemType)
        {
            // 如果窗口已存在，先关闭
            CloseItemSelectionWindow();
            
            // 保存当前参数名和物品类型
            currentButtonParameterName = paramName;
            currentItemType = itemType;
            
            // 创建物品选择窗口
            itemSelectionWindow = new GameObject("ItemSelectionWindow");
            Canvas canvas = itemSelectionWindow.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = itemSelectionWindow.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            itemSelectionWindow.AddComponent<GraphicRaycaster>();
            
            // 设置窗口背景
            GameObject background = new GameObject("Background");
            background.transform.SetParent(itemSelectionWindow.transform, false);
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(400, 500);
            bgRect.anchoredPosition = Vector2.zero;
            
            // 添加窗口标题
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(background.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = itemType == "DanYao" ? "选择丹药" : "选择符咒";
            titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleText.fontSize = 20;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(380, 40);
            titleRect.anchoredPosition = new Vector2(0, 220);
            
            // 创建滚动视图
            GameObject scrollViewObj = new GameObject("ScrollView");
            scrollViewObj.transform.SetParent(background.transform, false);
            ScrollRect scrollRect = scrollViewObj.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            RectTransform scrollViewRect = scrollViewObj.GetComponent<RectTransform>();
            scrollViewRect.sizeDelta = new Vector2(380, 400);
            scrollViewRect.anchoredPosition = Vector2.zero;
            
            // 添加滚动视图背景
            GameObject scrollViewBgObj = new GameObject("Viewport");
            scrollViewBgObj.transform.SetParent(scrollViewObj.transform, false);
            Image scrollViewBgImage = scrollViewBgObj.AddComponent<Image>();
            scrollViewBgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            RectTransform scrollViewBgRect = scrollViewBgObj.GetComponent<RectTransform>();
            scrollViewBgRect.sizeDelta = new Vector2(380, 400);
            scrollViewBgRect.anchoredPosition = Vector2.zero;
            
            // 创建内容区域
            itemListContent = new GameObject("Content");
            itemListContent.transform.SetParent(scrollViewBgObj.transform, false);
            RectTransform contentRect = itemListContent.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = new Vector2(0, 0);
            contentRect.sizeDelta = new Vector2(0, 0);
            
            // 配置ScrollRect
            scrollRect.viewport = scrollViewBgRect;
            scrollRect.content = contentRect;
            
            // 添加滚动条
            GameObject scrollbarObj = new GameObject("Scrollbar");
            scrollbarObj.transform.SetParent(scrollViewObj.transform, false);
            Scrollbar scrollbar = scrollbarObj.AddComponent<Scrollbar>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            RectTransform scrollbarRect = scrollbarObj.GetComponent<RectTransform>();
            scrollbarRect.sizeDelta = new Vector2(20, 380);
            scrollbarRect.anchoredPosition = new Vector2(180, 0);
            
            // 设置滚动条背景
            GameObject scrollbarBgObj = new GameObject("Background");
            scrollbarBgObj.transform.SetParent(scrollbarObj.transform, false);
            Image scrollbarBgImage = scrollbarBgObj.AddComponent<Image>();
            scrollbarBgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            RectTransform scrollbarBgRect = scrollbarBgObj.GetComponent<RectTransform>();
            scrollbarBgRect.sizeDelta = new Vector2(20, 380);
            scrollbarBgRect.anchoredPosition = Vector2.zero;
            
            // 设置滚动条滑块
            GameObject scrollbarHandleObj = new GameObject("Handle");
            scrollbarHandleObj.transform.SetParent(scrollbarObj.transform, false);
            Image scrollbarHandleImage = scrollbarHandleObj.AddComponent<Image>();
            scrollbarHandleImage.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            RectTransform scrollbarHandleRect = scrollbarHandleObj.GetComponent<RectTransform>();
            scrollbarHandleRect.sizeDelta = new Vector2(20, 60);
            scrollbarHandleRect.anchoredPosition = Vector2.zero;
            
            // 配置Scrollbar
            scrollbar.handleRect = scrollbarHandleRect;
            scrollRect.verticalScrollbar = scrollbar;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            
            // 填充物品列表
            FillItemList();
            
            // 添加关闭按钮
            GameObject closeButtonObj = new GameObject("CloseButton");
            closeButtonObj.transform.SetParent(background.transform, false);
            Image closeButtonImage = closeButtonObj.AddComponent<Image>();
            closeButtonImage.color = new Color(0.8f, 0.3f, 0.3f, 1f);
            Button closeButton = closeButtonObj.AddComponent<Button>();
            closeButton.targetGraphic = closeButtonImage;
            RectTransform closeButtonRect = closeButtonObj.GetComponent<RectTransform>();
            closeButtonRect.sizeDelta = new Vector2(80, 30);
            closeButtonRect.anchoredPosition = new Vector2(0, -230);
            
            // 添加关闭按钮文本
            GameObject closeButtonTextObj = new GameObject("Text");
            closeButtonTextObj.transform.SetParent(closeButtonObj.transform, false);
            Text closeButtonText = closeButtonTextObj.AddComponent<Text>();
            closeButtonText.text = "关闭";
            closeButtonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            closeButtonText.fontSize = 16;
            closeButtonText.alignment = TextAnchor.MiddleCenter;
            closeButtonText.color = Color.white;
            RectTransform closeButtonTextRect = closeButtonTextObj.GetComponent<RectTransform>();
            closeButtonTextRect.sizeDelta = new Vector2(70, 20);
            closeButtonTextRect.anchoredPosition = Vector2.zero;
            
            // 添加关闭事件
            closeButton.onClick.AddListener(() => {
                CloseItemSelectionWindow();
            });
            
            // 添加到活动窗口列表
            activeWindows.Add(itemSelectionWindow);
            
            AddEvents.Logger.LogInfo($"物品选择窗口已打开，参数名: {paramName}, 物品类型: {itemType}");
        }
        
        // 填充物品列表
        private void FillItemList()
        {
            if (itemListContent == null || string.IsNullOrEmpty(currentItemType))
                return;
            
            // 获取物品列表
            List<string> itemList = new List<string>();
            if (currentItemType == "DanYao")
            {
                // 如果丹药名称列表为空，使用模拟数据
                if (EventData.DanYaoNameList.Count == 0)
                {
                    itemList = new List<string> { "补气丹", "培元丹", "聚气丹", "筑基丹", "金丹" };
                    AddEvents.Logger.LogWarning("使用模拟丹药数据");
                }
                else
                {
                    itemList = EventData.DanYaoNameList;
                }
            }
            else if (currentItemType == "FuZhou")
            {
                // 如果符咒名称列表为空，使用模拟数据
                if (EventData.FuZhouNameList.Count == 0)
                {
                    itemList = new List<string> { "金光符", "烈火符", "玄水符", "风刃符", "神行符" };
                    AddEvents.Logger.LogWarning("使用模拟符咒数据");
                }
                else
                {
                    itemList = EventData.FuZhouNameList;
                }
            }
            
            // 创建物品项
            float startY = 0;
            float itemHeight = 40;
            
            foreach (string itemName in itemList)
            {
                // 创建物品项
                GameObject itemObj = new GameObject($"Item_{itemName}");
                itemObj.transform.SetParent(itemListContent.transform, false);
                
                // 添加按钮组件
                Image itemImage = itemObj.AddComponent<Image>();
                itemImage.color = new Color(0.3f, 0.6f, 0.9f, 1f);
                Button itemButton = itemObj.AddComponent<Button>();
                itemButton.targetGraphic = itemImage;
                
                // 设置位置和大小
                RectTransform itemRect = itemObj.GetComponent<RectTransform>();
                itemRect.anchorMin = new Vector2(0, 1);
                itemRect.anchorMax = new Vector2(1, 1);
                itemRect.pivot = new Vector2(0.5f, 1);
                itemRect.sizeDelta = new Vector2(360, itemHeight);
                itemRect.anchoredPosition = new Vector2(0, startY);
                startY -= itemHeight + 5; // 5是间隔
                
                // 添加物品名称
                GameObject itemTextObj = new GameObject("Text");
                itemTextObj.transform.SetParent(itemObj.transform, false);
                Text itemText = itemTextObj.AddComponent<Text>();
                itemText.text = itemName;
                itemText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                itemText.fontSize = 16;
                itemText.alignment = TextAnchor.MiddleCenter;
                itemText.color = Color.white;
                RectTransform itemTextRect = itemTextObj.GetComponent<RectTransform>();
                itemTextRect.sizeDelta = new Vector2(340, 30);
                itemTextRect.anchoredPosition = Vector2.zero;
                
                // 添加点击事件
                string selectedItemName = itemName; // 保存当前物品名称到局部变量
                itemButton.onClick.AddListener(() => {
                    // 保存选择的物品
                    if (!string.IsNullOrEmpty(currentButtonParameterName))
                    {
                        buttonParameterSelectedValues[currentButtonParameterName] = selectedItemName;
                        
                        // 更新按钮文本显示选中的物品
                        UpdateButtonParameterText(currentButtonParameterName, selectedItemName);
                        
                        AddEvents.Logger.LogInfo($"已选择物品: {selectedItemName} 对应参数: {currentButtonParameterName}");
                        
                        // 关闭选择窗口
                        CloseItemSelectionWindow();
                    }
                });
            }
            
            // 更新内容区域高度
            RectTransform contentRect = itemListContent.GetComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2(0, Mathf.Abs(startY) + 10);
        }
        
        // 更新按钮参数的显示文本
        private void UpdateButtonParameterText(string paramName, string itemName)
        {
            if (paramListContent == null)
                return;
            
            // 遍历参数列表中的所有项
            foreach (Transform paramItem in paramListContent.transform)
            {
                // 查找参数名文本组件
                Transform nameObj = paramItem.Find("ParamName");
                if (nameObj != null)
                {
                    Text nameText = nameObj.GetComponent<Text>();
                    if (nameText != null && nameText.text == paramName)
                    {
                        // 找到参数名对应的项，更新按钮文本
                        Transform inputObj = paramItem.Find("ParamInput");
                        if (inputObj != null)
                        {
                            Transform buttonTextObj = inputObj.Find("ButtonText");
                            if (buttonTextObj != null)
                            {
                                Text buttonText = buttonTextObj.GetComponent<Text>();
                                if (buttonText != null)
                                {
                                    buttonText.text = itemName; // 显示选中的物品名称
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
        
        // 关闭物品选择窗口
        private void CloseItemSelectionWindow()
        {
            if (itemSelectionWindow != null)
            {
                activeWindows.Remove(itemSelectionWindow);
                Destroy(itemSelectionWindow);
                itemSelectionWindow = null;
                itemListContent = null;
                currentButtonParameterName = string.Empty;
                currentItemType = string.Empty;
                AddEvents.Logger.LogInfo("物品选择窗口已关闭");
            }
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
                instance.itemSelectionWindow = null;
                instance.selectedEventIndex = -1;
                instance.paramListContent = null;
                instance.itemListContent = null;
                instance.currentButtonParameterName = string.Empty;
                instance.currentItemType = string.Empty;
                // 清空字典
                if (instance.buttonParameterClickedStates != null)
                    instance.buttonParameterClickedStates.Clear();
                if (instance.buttonParameterSelectedValues != null)
                    instance.buttonParameterSelectedValues.Clear();
            }
        }
    }
}