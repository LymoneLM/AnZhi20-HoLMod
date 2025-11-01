using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace cs.HoLMod.AddEvents
{
    internal class Windows : MonoBehaviour
    {
        // 窗口实例
        private static GameObject welcomeWindow;
        private static GameObject mainWindow;
        
        // 按钮列表数据
        private static List<string> buttonList = new List<string> {
            "事件类型1",
            "事件类型2",
            "事件类型3",
            "事件类型4",
            "事件类型5"
        };
        
        private static int selectedButtonIndex = -1;
        
        // 创建欢迎窗口
        public static void CreateWelcomeWindow()
        {
            if (welcomeWindow != null)
                return;
            
            // 创建窗口容器
            welcomeWindow = new GameObject("WelcomeWindow");
            Canvas canvas = welcomeWindow.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = welcomeWindow.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            welcomeWindow.AddComponent<GraphicRaycaster>();
            
            // 创建背景面板
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(welcomeWindow.transform);
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            
            // 设置面板RectTransform
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(600, 400);
            panelRect.anchoredPosition = Vector2.zero;
            
            // 创建标题文本
            GameObject titleText = new GameObject("TitleText");
            titleText.transform.SetParent(panel.transform);
            TextMeshProUGUI titleTMP = titleText.AddComponent<TextMeshProUGUI>();
            titleTMP.text = "欢迎使用事件触发器";
            titleTMP.fontSize = 32;
            titleTMP.color = Color.white;
            titleTMP.alignment = TextAlignmentOptions.Center;
            
            RectTransform titleRect = titleText.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(500, 50);
            titleRect.anchoredPosition = new Vector2(0, 100);
            
            // 创建版本信息
            GameObject versionText = new GameObject("VersionText");
            versionText.transform.SetParent(panel.transform);
            TextMeshProUGUI versionTMP = versionText.AddComponent<TextMeshProUGUI>();
            versionTMP.text = $"版本: {PluginInfo.PLUGIN_VERSION}";
            versionTMP.fontSize = 18;
            versionTMP.color = Color.gray;
            versionTMP.alignment = TextAlignmentOptions.Center;
            
            RectTransform versionRect = versionText.GetComponent<RectTransform>();
            versionRect.sizeDelta = new Vector2(300, 30);
            versionRect.anchoredPosition = new Vector2(0, 50);
            
            // 创建开始按钮
            GameObject startButton = new GameObject("StartButton");
            startButton.transform.SetParent(panel.transform);
            Button buttonComp = startButton.AddComponent<Button>();
            
            // 设置按钮样式
            Image buttonImage = startButton.AddComponent<Image>();
            buttonImage.color = new Color(0.4f, 0.4f, 0.8f, 1f);
            
            // 添加按钮点击事件
            buttonComp.onClick.AddListener(() => {
                DestroyWelcomeWindow();
                CreateMainWindow();
            });
            
            // 设置按钮文本
            GameObject buttonText = new GameObject("ButtonText");
            buttonText.transform.SetParent(startButton.transform);
            TextMeshProUGUI buttonTMP = buttonText.AddComponent<TextMeshProUGUI>();
            buttonTMP.text = "开始添加";
            buttonTMP.fontSize = 24;
            buttonTMP.color = Color.white;
            buttonTMP.alignment = TextAlignmentOptions.Center;
            
            // 设置按钮RectTransform
            RectTransform buttonRect = startButton.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(200, 60);
            buttonRect.anchoredPosition = Vector2.zero;
            
            RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
            buttonTextRect.sizeDelta = new Vector2(180, 40);
            buttonTextRect.anchoredPosition = Vector2.zero;
        }
        
        // 销毁欢迎窗口
        public static void DestroyWelcomeWindow()
        {
            if (welcomeWindow != null)
            {
                Destroy(welcomeWindow);
                welcomeWindow = null;
            }
        }
        
        // 创建主要窗口
        public static void CreateMainWindow()
        {
            if (mainWindow != null)
                return;
            
            // 创建窗口容器
            mainWindow = new GameObject("MainWindow");
            Canvas canvas = mainWindow.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = mainWindow.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            mainWindow.AddComponent<GraphicRaycaster>();
            
            // 创建背景面板
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(mainWindow.transform);
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);
            
            // 设置面板RectTransform
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(1000, 800);
            panelRect.anchoredPosition = Vector2.zero;
            
            // 创建标题文本
            GameObject titleText = new GameObject("TitleText");
            titleText.transform.SetParent(panel.transform);
            TextMeshProUGUI titleTMP = titleText.AddComponent<TextMeshProUGUI>();
            titleTMP.text = "事件触发器";
            titleTMP.fontSize = 28;
            titleTMP.color = Color.white;
            titleTMP.alignment = TextAlignmentOptions.Center;
            
            RectTransform titleRect = titleText.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(900, 40);
            titleRect.anchoredPosition = new Vector2(0, 350);
            
            // 创建左侧按钮列表面板
            GameObject buttonListPanel = new GameObject("ButtonListPanel");
            buttonListPanel.transform.SetParent(panel.transform);
            Image buttonListImage = buttonListPanel.AddComponent<Image>();
            buttonListImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            RectTransform buttonListRect = buttonListPanel.GetComponent<RectTransform>();
            buttonListRect.sizeDelta = new Vector2(400, 600);
            buttonListRect.anchoredPosition = new Vector2(-250, 0);
            
            // 添加按钮列表标题
            GameObject buttonListTitle = new GameObject("ButtonListTitle");
            buttonListTitle.transform.SetParent(buttonListPanel.transform);
            TextMeshProUGUI buttonListTitleTMP = buttonListTitle.AddComponent<TextMeshProUGUI>();
            buttonListTitleTMP.text = "N个按钮列表";
            buttonListTitleTMP.fontSize = 20;
            buttonListTitleTMP.color = Color.white;
            buttonListTitleTMP.alignment = TextAlignmentOptions.Center;
            
            RectTransform buttonListTitleRect = buttonListTitle.GetComponent<RectTransform>();
            buttonListTitleRect.sizeDelta = new Vector2(350, 30);
            buttonListTitleRect.anchoredPosition = new Vector2(0, 270);
            
            // 创建按钮列表
            CreateButtonList(buttonListPanel);
            
            // 创建右侧参数面板
            GameObject paramsPanel = new GameObject("ParamsPanel");
            paramsPanel.transform.SetParent(panel.transform);
            Image paramsImage = paramsPanel.AddComponent<Image>();
            paramsImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            RectTransform paramsRect = paramsPanel.GetComponent<RectTransform>();
            paramsRect.sizeDelta = new Vector2(400, 600);
            paramsRect.anchoredPosition = new Vector2(250, 0);
            
            // 创建参数输入区域
            CreateParamsInputs(paramsPanel);
            
            // 创建底部添加按钮
            GameObject addButton = new GameObject("AddButton");
            addButton.transform.SetParent(panel.transform);
            Button addButtonComp = addButton.AddComponent<Button>();
            
            Image addButtonImage = addButton.AddComponent<Image>();
            addButtonImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);
            
            addButtonComp.onClick.AddListener(() => {
                // 处理添加事件的逻辑
                Debug.Log("添加事件按钮点击");
            });
            
            GameObject addButtonText = new GameObject("AddButtonText");
            addButtonText.transform.SetParent(addButton.transform);
            TextMeshProUGUI addButtonTMP = addButtonText.AddComponent<TextMeshProUGUI>();
            addButtonTMP.text = "添加";
            addButtonTMP.fontSize = 24;
            addButtonTMP.color = Color.white;
            addButtonTMP.alignment = TextAlignmentOptions.Center;
            
            RectTransform addButtonRect = addButton.GetComponent<RectTransform>();
            addButtonRect.sizeDelta = new Vector2(200, 60);
            addButtonRect.anchoredPosition = new Vector2(0, -350);
            
            RectTransform addButtonTextRect = addButtonText.GetComponent<RectTransform>();
            addButtonTextRect.sizeDelta = new Vector2(180, 40);
            addButtonTextRect.anchoredPosition = Vector2.zero;
            
            // 添加关闭按钮
            GameObject closeButton = new GameObject("CloseButton");
            closeButton.transform.SetParent(panel.transform);
            Button closeButtonComp = closeButton.AddComponent<Button>();
            
            Image closeButtonImage = closeButton.AddComponent<Image>();
            closeButtonImage.color = new Color(0.8f, 0.2f, 0.2f, 1f);
            
            closeButtonComp.onClick.AddListener(() => {
                DestroyMainWindow();
            });
            
            GameObject closeButtonText = new GameObject("CloseButtonText");
            closeButtonText.transform.SetParent(closeButton.transform);
            TextMeshProUGUI closeButtonTMP = closeButtonText.AddComponent<TextMeshProUGUI>();
            closeButtonTMP.text = "关闭";
            closeButtonTMP.fontSize = 18;
            closeButtonTMP.color = Color.white;
            closeButtonTMP.alignment = TextAlignmentOptions.Center;
            
            RectTransform closeButtonRect = closeButton.GetComponent<RectTransform>();
            closeButtonRect.sizeDelta = new Vector2(80, 40);
            closeButtonRect.anchoredPosition = new Vector2(450, 360);
            
            RectTransform closeButtonTextRect = closeButtonText.GetComponent<RectTransform>();
            closeButtonTextRect.sizeDelta = new Vector2(70, 30);
            closeButtonTextRect.anchoredPosition = Vector2.zero;
        }
        
        // 创建按钮列表
        private static void CreateButtonList(GameObject parent)
        {
            float startY = 200;
            float buttonSpacing = 60;
            
            for (int i = 0; i < buttonList.Count; i++)
            {
                int index = i;
                GameObject button = new GameObject($"Button_{i}");
                button.transform.SetParent(parent.transform);
                Button buttonComp = button.AddComponent<Button>();
                
                Image buttonImage = button.AddComponent<Image>();
                buttonImage.color = new Color(0.3f, 0.3f, 0.6f, 1f);
                
                buttonComp.onClick.AddListener(() => {
                    selectedButtonIndex = index;
                    Debug.Log($"选中按钮: {buttonList[index]}");
                    // 更新参数显示
                    UpdateParamsDisplay();
                });
                
                GameObject buttonText = new GameObject("ButtonText");
                buttonText.transform.SetParent(button.transform);
                TextMeshProUGUI buttonTMP = buttonText.AddComponent<TextMeshProUGUI>();
                buttonTMP.text = buttonList[i];
                buttonTMP.fontSize = 18;
                buttonTMP.color = Color.white;
                buttonTMP.alignment = TextAlignmentOptions.Center;
                
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(300, 50);
                buttonRect.anchoredPosition = new Vector2(0, startY - i * buttonSpacing);
                
                RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
                buttonTextRect.sizeDelta = new Vector2(280, 40);
                buttonTextRect.anchoredPosition = Vector2.zero;
            }
        }
        
        // 创建参数输入区域
        private static void CreateParamsInputs(GameObject parent)
        {
            float startY = 200;
            float inputSpacing = 80;
            
            // 创建5个参数输入框
            for (int i = 0; i < 5; i++)
            {
                CreateParamInput(parent, i, startY - i * inputSpacing);
            }
            
            // 创建多行文本说明框
            CreateMultilineInput(parent);
        }
        
        // 创建单个参数输入
        private static void CreateParamInput(GameObject parent, int index, float yPosition)
        {
            GameObject paramContainer = new GameObject($"Param_{index}");
            paramContainer.transform.SetParent(parent.transform);
            
            RectTransform paramContainerRect = paramContainer.AddComponent<RectTransform>();
            paramContainerRect.sizeDelta = new Vector2(350, 60);
            paramContainerRect.anchoredPosition = new Vector2(0, yPosition);
            
            // 参数标签
            GameObject paramLabel = new GameObject($"ParamLabel_{index}");
            paramLabel.transform.SetParent(paramContainer.transform);
            TextMeshProUGUI labelTMP = paramLabel.AddComponent<TextMeshProUGUI>();
            labelTMP.text = $"参数{Convert.ToChar('A' + index)}: ";
            labelTMP.fontSize = 16;
            labelTMP.color = Color.white;
            labelTMP.alignment = TextAlignmentOptions.Right;
            
            RectTransform labelRect = paramLabel.GetComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(80, 30);
            labelRect.anchoredPosition = new Vector2(-100, 0);
            
            // 参数输入框背景
            GameObject inputBackground = new GameObject($"InputBackground_{index}");
            inputBackground.transform.SetParent(paramContainer.transform);
            Image inputBgImage = inputBackground.AddComponent<Image>();
            inputBgImage.color = Color.white;
            
            RectTransform inputBgRect = inputBackground.GetComponent<RectTransform>();
            inputBgRect.sizeDelta = new Vector2(200, 30);
            inputBgRect.anchoredPosition = new Vector2(50, 0);
            
            // 参数输入文本（使用TextMeshPro作为输入框）
            GameObject inputText = new GameObject($"InputText_{index}");
            inputText.transform.SetParent(inputBackground.transform);
            TextMeshProUGUI inputTMP = inputText.AddComponent<TextMeshProUGUI>();
            inputTMP.text = "";
            inputTMP.fontSize = 16;
            inputTMP.color = Color.black;
            inputTMP.alignment = TextAlignmentOptions.Left;
            
            RectTransform inputTextRect = inputText.GetComponent<RectTransform>();
            inputTextRect.sizeDelta = new Vector2(190, 25);
            inputTextRect.anchoredPosition = new Vector2(0, 0);
        }
        
        // 创建多行文本输入框
        private static void CreateMultilineInput(GameObject parent)
        {
            GameObject multilineContainer = new GameObject("MultilineContainer");
            multilineContainer.transform.SetParent(parent.transform);
            
            RectTransform multilineContainerRect = multilineContainer.AddComponent<RectTransform>();
            multilineContainerRect.sizeDelta = new Vector2(350, 120);
            multilineContainerRect.anchoredPosition = new Vector2(0, -250);
            
            // 多行文本标签
            GameObject multilineLabel = new GameObject("MultilineLabel");
            multilineLabel.transform.SetParent(multilineContainer.transform);
            TextMeshProUGUI labelTMP = multilineLabel.AddComponent<TextMeshProUGUI>();
            labelTMP.text = "说明: ";
            labelTMP.fontSize = 16;
            labelTMP.color = Color.white;
            labelTMP.alignment = TextAlignmentOptions.Right;
            
            RectTransform labelRect = multilineLabel.GetComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(60, 30);
            labelRect.anchoredPosition = new Vector2(-110, 40);
            
            // 多行文本输入框背景
            GameObject multilineBackground = new GameObject("MultilineBackground");
            multilineBackground.transform.SetParent(multilineContainer.transform);
            Image multilineBgImage = multilineBackground.AddComponent<Image>();
            multilineBgImage.color = Color.white;
            
            RectTransform multilineBgRect = multilineBackground.GetComponent<RectTransform>();
            multilineBgRect.sizeDelta = new Vector2(220, 80);
            multilineBgRect.anchoredPosition = new Vector2(60, 0);
            
            // 多行文本输入（使用TextMeshPro作为多行输入框）
            GameObject multilineText = new GameObject("MultilineText");
            multilineText.transform.SetParent(multilineBackground.transform);
            TextMeshProUGUI multilineTMP = multilineText.AddComponent<TextMeshProUGUI>();
            multilineTMP.text = "(多行文本)";
            multilineTMP.fontSize = 14;
            multilineTMP.color = Color.black;
            multilineTMP.alignment = TextAlignmentOptions.TopLeft;
            multilineTMP.enableWordWrapping = true;
            
            RectTransform multilineTextRect = multilineText.GetComponent<RectTransform>();
            multilineTextRect.sizeDelta = new Vector2(210, 70);
            multilineTextRect.anchoredPosition = new Vector2(0, 0);
            
            // 添加调整大小的边框标记（视觉效果）
            CreateResizeHandles(multilineBackground);
        }
        
        // 创建调整大小的边框标记
        private static void CreateResizeHandles(GameObject parent)
        {
            Vector2[] handlePositions = new Vector2[] {
                new Vector2(-110, 40),  // 左上
                new Vector2(110, 40),   // 右上
                new Vector2(-110, -40), // 左下
                new Vector2(110, -40)   // 右下
            };
            
            foreach (Vector2 pos in handlePositions)
            {
                GameObject handle = new GameObject($"ResizeHandle");
                handle.transform.SetParent(parent.transform);
                Image handleImage = handle.AddComponent<Image>();
                handleImage.color = Color.black;
                
                RectTransform handleRect = handle.GetComponent<RectTransform>();
                handleRect.sizeDelta = new Vector2(10, 10);
                handleRect.anchoredPosition = pos;
            }
        }
        
        // 更新参数显示
        private static void UpdateParamsDisplay()
        {
            // 根据选中的按钮索引更新参数显示
            Debug.Log($"更新参数显示，选中索引: {selectedButtonIndex}");
            // 这里可以根据selectedButtonIndex来动态显示不同数量的参数
        }
        
        // 销毁主要窗口
        public static void DestroyMainWindow()
        {
            if (mainWindow != null)
            {
                Destroy(mainWindow);
                mainWindow = null;
            }
        }
        
        // 检查是否有窗口打开
        public static bool IsAnyWindowOpen()
        {
            return welcomeWindow != null || mainWindow != null;
        }
        
        // 关闭所有窗口
        public static void CloseAllWindows()
        {
            DestroyWelcomeWindow();
            DestroyMainWindow();
        }
    }
}
