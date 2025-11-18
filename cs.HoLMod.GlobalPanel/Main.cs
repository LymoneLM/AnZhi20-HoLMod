using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using BepInEx;

namespace cs.HoLMod.GlobalPanel
{
    /// <summary>
    /// 插件信息类，用于存储插件的基本信息
    /// </summary>
    public static class PluginInfo 
    {
        public const string PLUGIN_GUID = "cs.HoLMod.GlobalPanel.AnZhi20"; 
        public const string PLUGIN_NAME = "HoLMod.GlobalPanel"; 
        public const string PLUGIN_VERSION = "1.0.0"; 
    }
    
    /// <summary>
    /// MOD信息类，用于存储MOD的基本信息
    /// </summary>
    public class ModInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public Texture2D Icon { get; set; }
        public bool IsEnabled { get; set; }
        public Action OnOpenAction { get; set; }
    }

    /// <summary>
    /// MOD总览面板，显示所有MOD的列表和详情
    /// </summary>
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MainPanel : BaseUnityPlugin
    {
        // MOD信息字典，用于存储所有MOD的数据
        private Dictionary<string, ModInfo> modInfos = new Dictionary<string, ModInfo>();
        // MOD按钮字典，用于快速查找和管理MOD按钮
        private Dictionary<string, GameObject> modButtons = new Dictionary<string, GameObject>();
        // 按钮容器，用于放置所有MOD按钮
        private Transform buttonContainer;
        // 按钮大小设置
        private int buttonSize = 150;
        private int buttonSpacing = 20;
        private int columns = 3;
        // MOD详情面板，点击MOD按钮时显示
        private GameObject modInfoPanel;

        private static MainPanel panelInstance; // 面板实例
        
        /// <summary>
        /// 组件初始化
        /// </summary>
        private void Awake()
        {
            // 确保脚本不会重复初始化
            if (panelInstance == null && !gameObject.name.Contains("MainPanel"))
            {
                // 默认隐藏，等待F7键激活
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 面板启动时设置事件监听
        /// </summary>
        private void Start()
        {
            // 添加关闭按钮事件
            if (base.transform.Find("CloseBT") != null)
            {
                base.transform.Find("CloseBT").GetComponent<Button>().onClick.AddListener(CloseBT);
            }
            if (base.transform.Find("Back") != null)
            {
                base.transform.Find("Back").GetComponent<Button>().onClick.AddListener(CloseBT);
            }
            
            // 初始化按钮容器
            InitButtonContainer();
            
            // 初始化MOD详情面板
            InitModInfoPanel();
        }

        /// <summary>
        /// 面板激活时的动画效果
        /// </summary>
        private void OnEnable()
        {
            // 使用与SetPanel类似的动画效果
            transform.localPosition = new Vector3(0f, 500f, 0f);
            transform.DOLocalMoveY(0f, 0.3f).SetEase(Ease.OutBack, 1f);
            
            // 刷新MOD列表显示
            RefreshModButtons();
        }
        
        /// <summary>
        /// 处理键盘输入，监听F7键打开面板和ESC键关闭面板
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F7))
            {
                TogglePanel(); // 调用静态方法
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && panelInstance != null && panelInstance.gameObject.activeSelf)
            {
                panelInstance.KeyAct();
            }
        }
        
        /// <summary>
        /// 切换面板显示与隐藏
        /// </summary>
        public static void TogglePanel()
        {
            if (panelInstance == null)
            {
                // 创建Canvas作为根对象
                GameObject canvasObj = new GameObject("MainPanelCanvas");
                Canvas canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 1000; // 确保在最上层显示
                
                // 添加CanvasScaler组件
                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                
                // 添加GraphicRaycaster组件
                canvasObj.AddComponent<GraphicRaycaster>();
                
                // 创建面板对象
                GameObject panelObject = new GameObject("MainPanel");
                panelObject.transform.SetParent(canvasObj.transform);
                panelObject.transform.localPosition = Vector3.zero;
                
                // 添加面板组件
                panelInstance = panelObject.AddComponent<MainPanel>();
                
                // 初始化面板
                panelInstance.InitPanelStructure();
                panelInstance.InitButtonContainer();
                
                // 设置面板为激活状态
                panelObject.SetActive(true);
                panelInstance.OnEnable(); // 播放显示动画
            }
            else
            {
                // 切换面板显示状态
                bool newState = !panelInstance.gameObject.activeSelf;
                panelInstance.gameObject.SetActive(newState);
                
                if (newState)
                {
                    panelInstance.OnEnable(); // 显示时播放动画
                }
            }
        }

        /// <summary>
        /// 快捷键处理
        /// </summary>
        private void KeyAct()
        {
            if (modInfoPanel != null && modInfoPanel.activeSelf)
            {
                // 如果详情面板显示，则关闭详情面板
                modInfoPanel.SetActive(false);
            }
            else if (this.gameObject.activeSelf)
            {
                // 如果主面板显示，则关闭主面板
                this.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 初始化面板结构，设置大小与SetPanel一致
        /// </summary>
        private void InitPanelStructure()
        {
            // 设置面板大小与SetPanel一致
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = gameObject.AddComponent<RectTransform>();
            }
            
            // 设置为800x600，与SetPanel同等大小
            rectTransform.sizeDelta = new Vector2(800, 600);
            rectTransform.anchoredPosition = Vector2.zero;
            
            // 添加背景图片组件
            Image background = gameObject.AddComponent<Image>();
            background.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            
            // 获取默认字体或设置字体
            Font defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf") ?? new Font("Arial");
            
            // 创建标题文本
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(transform);
            titleObj.transform.localPosition = new Vector3(0, 250, 0);
            
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "MOD总览";
            titleText.font = defaultFont;
            titleText.fontSize = 32;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;
            
            // 创建关闭按钮
            GameObject closeButtonObj = new GameObject("CloseBT");
            closeButtonObj.transform.SetParent(transform);
            closeButtonObj.transform.localPosition = new Vector3(350, 250, 0);
            
            // 设置按钮大小
            RectTransform btnRect = closeButtonObj.AddComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(40, 40);
            
            Button closeButton = closeButtonObj.AddComponent<Button>();
            
            // 添加按钮背景
            Image btnBackground = closeButtonObj.AddComponent<Image>();
            btnBackground.color = new Color(0.7f, 0.2f, 0.2f, 1f);
            
            Text closeButtonText = closeButtonObj.AddComponent<Text>();
            closeButtonText.text = "X";
            closeButtonText.font = defaultFont;
            closeButtonText.fontSize = 24;
            closeButtonText.alignment = TextAnchor.MiddleCenter;
            closeButtonText.color = Color.white;
        }

        /// <summary>
        /// 初始化按钮容器，添加滚动功能
        /// </summary>
        private void InitButtonContainer()
        {
            buttonContainer = base.transform.Find("ButtonContainer");
            if (buttonContainer == null)
            {
                GameObject containerObj = new GameObject("ButtonContainer");
                buttonContainer = containerObj.transform;
                buttonContainer.SetParent(transform);
                buttonContainer.localPosition = new Vector3(0, -100, 0);
                
                // 设置容器大小
                RectTransform containerRect = containerObj.AddComponent<RectTransform>();
                containerRect.sizeDelta = new Vector2(700, 400);
                
                // 添加ScrollRect组件以支持滚动
                ScrollRect scrollRect = containerObj.AddComponent<ScrollRect>();
                
                GameObject viewport = new GameObject("Viewport");
                viewport.transform.SetParent(containerObj.transform);
                viewport.transform.localPosition = Vector3.zero;
                RectTransform viewportRect = viewport.AddComponent<RectTransform>();
                viewportRect.sizeDelta = new Vector2(700, 400);
                viewport.AddComponent<RectMask2D>();
                
                GameObject content = new GameObject("Content");
                content.transform.SetParent(viewport.transform);
                content.transform.localPosition = Vector3.zero;
                RectTransform contentRect = content.AddComponent<RectTransform>();
                contentRect.sizeDelta = new Vector2(700, 400);
                
                scrollRect.viewport = viewportRect;
                scrollRect.content = contentRect;
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
                
                buttonContainer = content.transform;
            }
        }

        /// <summary>
        /// 初始化MOD详情面板
        /// </summary>
        private void InitModInfoPanel()
        {
            // 创建MOD详情面板（默认隐藏）
            modInfoPanel = new GameObject("ModInfoPanel");
            modInfoPanel.transform.SetParent(transform);
            modInfoPanel.SetActive(false);
            
            // 设置面板大小和位置
            RectTransform infoPanelRect = modInfoPanel.AddComponent<RectTransform>();
            infoPanelRect.sizeDelta = new Vector2(600, 400);
            infoPanelRect.localPosition = Vector3.zero;
            
            // 添加背景
            Image background = modInfoPanel.AddComponent<Image>();
            background.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            
            // 添加关闭按钮
            GameObject closeInfoButton = new GameObject("CloseInfoButton");
            closeInfoButton.transform.SetParent(modInfoPanel.transform);
            closeInfoButton.transform.localPosition = new Vector2(280, 180);
            Button closeInfoBtnComp = closeInfoButton.AddComponent<Button>();
            Text closeInfoText = closeInfoButton.AddComponent<Text>();
            closeInfoText.text = "X";
            closeInfoText.fontSize = 24;
            closeInfoBtnComp.onClick.AddListener(() => modInfoPanel.SetActive(false));
            
            // 添加标题、描述等文本组件
            CreateTextComponent(modInfoPanel, "ModName", new Vector2(0, 150), 24, "MOD名称");
            CreateTextComponent(modInfoPanel, "ModVersion", new Vector2(-200, 100), 18, "版本: ");
            CreateTextComponent(modInfoPanel, "ModAuthor", new Vector2(200, 100), 18, "作者: ");
            CreateTextComponent(modInfoPanel, "ModDescription", new Vector2(0, 0), 16, "描述: ", TextAnchor.UpperCenter, new Vector2(500, 200));
            
            // 添加打开按钮
            GameObject openButton = new GameObject("OpenModButton");
            openButton.transform.SetParent(modInfoPanel.transform);
            openButton.transform.localPosition = new Vector2(0, -150);
            Button openBtnComp = openButton.AddComponent<Button>();
            Text openBtnText = openButton.AddComponent<Text>();
            openBtnText.text = "打开MOD";
            openBtnText.fontSize = 20;
            openBtnComp.onClick.AddListener(OnOpenModButtonClick);
        }

        /// <summary>
        /// 创建文本组件的辅助方法
        /// </summary>
        private void CreateTextComponent(GameObject parent, string name, Vector2 localPos, int fontSize, string text, TextAnchor alignment = TextAnchor.MiddleCenter, Vector2 sizeDelta = default(Vector2))
        {
            // 获取默认字体
            Font defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf") ?? new Font("Arial");
            
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent.transform);
            textObj.transform.localPosition = localPos;
            
            Text textComp = textObj.AddComponent<Text>();
            textComp.text = text;
            textComp.fontSize = fontSize;
            textComp.alignment = alignment;
            textComp.color = Color.white;
            
            // 使用已定义的字体
            textComp.font = defaultFont;
            
            if (sizeDelta != default(Vector2))
            {
                RectTransform rect = textObj.GetComponent<RectTransform>();
                rect.sizeDelta = sizeDelta;
            }
        }

        /// <summary>
        /// 添加MOD到面板的公共方法
        /// </summary>
        /// <param name="modName">MOD名称</param>
        /// <param name="description">MOD描述</param>
        /// <param name="version">MOD版本</param>
        /// <param name="author">MOD作者</param>
        /// <param name="icon">MOD图标</param>
        /// <param name="onOpenAction">点击打开时执行的动作</param>
        public void AddMod(string modName, string description = "", string version = "1.0", string author = "Unknown", Texture2D icon = null, Action onOpenAction = null)
        {
            // 创建MOD信息
            ModInfo modInfo = new ModInfo
            {
                Name = modName,
                Description = description,
                Version = version,
                Author = author,
                Icon = icon,
                IsEnabled = true,
                OnOpenAction = onOpenAction
            };
            
            // 存储MOD信息
            if (modInfos.ContainsKey(modName))
            {
                modInfos[modName] = modInfo;
            }
            else
            {
                modInfos.Add(modName, modInfo);
            }
            
            // 如果面板当前激活，刷新按钮显示
            if (gameObject.activeSelf)
            {
                RefreshModButtons();
            }
        }

        /// <summary>
        /// 刷新MOD按钮显示
        /// </summary>
        private void RefreshModButtons()
        {
            // 移除所有现有按钮
            foreach (var button in modButtons.Values)
            {
                Destroy(button);
            }
            modButtons.Clear();
            
            // 创建新的按钮
            int index = 0;
            foreach (var modInfo in modInfos.Values)
            {
                CreateModButton(modInfo, index);
                index++;
            }
            
            // 更新内容容器大小
            UpdateContentSize();
        }

        /// <summary>
        /// 创建单个MOD按钮
        /// </summary>
        private void CreateModButton(ModInfo modInfo, int index)
        {
            // 创建按钮对象
            GameObject buttonObj = new GameObject("ModButton_" + modInfo.Name);
            buttonObj.transform.SetParent(buttonContainer);
            
            // 设置按钮位置
            int row = index / columns;
            int col = index % columns;
            
            float posX = (col - (columns - 1) / 2f) * (buttonSize + buttonSpacing);
            float posY = -row * (buttonSize + buttonSpacing);
            
            buttonObj.transform.localPosition = new Vector3(posX, posY, 0);
            
            // 添加按钮组件
            Button button = buttonObj.AddComponent<Button>();
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(buttonSize, buttonSize);
            
            // 添加图标（如果有）
            if (modInfo.Icon != null)
            {
                GameObject iconObj = new GameObject("Icon");
                iconObj.transform.SetParent(buttonObj.transform);
                iconObj.transform.localPosition = new Vector3(0, 20, 0);
                
                Image iconImage = iconObj.AddComponent<Image>();
                iconImage.sprite = Sprite.Create(modInfo.Icon, new Rect(0, 0, modInfo.Icon.width, modInfo.Icon.height), new Vector2(0.5f, 0.5f));
                
                RectTransform iconRect = iconObj.GetComponent<RectTransform>();
                iconRect.sizeDelta = new Vector2(60, 60);
            }
            
            // 添加按钮文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);
            textObj.transform.localPosition = new Vector3(0, -20, 0);
            
            Text buttonText = textObj.AddComponent<Text>();
            buttonText.text = modInfo.Name;
            buttonText.fontSize = 14;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.rectTransform.sizeDelta = new Vector2(buttonSize - 10, 40);
            
            // 添加版本号文本
            GameObject versionObj = new GameObject("Version");
            versionObj.transform.SetParent(buttonObj.transform);
            versionObj.transform.localPosition = new Vector3(0, -50, 0);
            
            Text versionText = versionObj.AddComponent<Text>();
            versionText.text = "v" + modInfo.Version;
            versionText.fontSize = 12;
            versionText.alignment = TextAnchor.MiddleCenter;
            versionText.rectTransform.sizeDelta = new Vector2(buttonSize - 10, 20);
            
            // 添加点击事件 - 显示MOD详情
            string modName = modInfo.Name;
            button.onClick.AddListener(() => ShowModInfo(modName));
            
            // 添加到按钮字典
            modButtons.Add(modName, buttonObj);
        }

        /// <summary>
        /// 显示MOD详情
        /// </summary>
        private void ShowModInfo(string modName)
        {
            if (modInfos.TryGetValue(modName, out ModInfo modInfo) && modInfoPanel != null)
            {
                // 更新详情面板内容
                modInfoPanel.transform.Find("ModName").GetComponent<Text>().text = modInfo.Name;
                modInfoPanel.transform.Find("ModVersion").GetComponent<Text>().text = "版本: " + modInfo.Version;
                modInfoPanel.transform.Find("ModAuthor").GetComponent<Text>().text = "作者: " + modInfo.Author;
                modInfoPanel.transform.Find("ModDescription").GetComponent<Text>().text = "描述: " + modInfo.Description;
                
                // 存储当前选中的MOD名称，供打开按钮使用
                modInfoPanel.name = "ModInfoPanel_" + modName;
                
                // 显示详情面板
                modInfoPanel.SetActive(true);
            }
        }

        /// <summary>
        /// 打开MOD按钮点击事件
        /// </summary>
        private void OnOpenModButtonClick()
        {
            // 从面板名称中提取MOD名称
            string panelName = modInfoPanel.name;
            if (panelName.StartsWith("ModInfoPanel_"))
            {
                string modName = panelName.Substring("ModInfoPanel_".Length);
                
                if (modInfos.TryGetValue(modName, out ModInfo modInfo))
                {
                    // 隐藏详情面板
                    modInfoPanel.SetActive(false);
                    
                    // 执行MOD的打开动作
                    modInfo.OnOpenAction?.Invoke();
                }
            }
        }

        /// <summary>
        /// 移除MOD
        /// </summary>
        public void RemoveMod(string modName)
        {
            if (modInfos.ContainsKey(modName))
            {
                modInfos.Remove(modName);
                
                if (modButtons.ContainsKey(modName))
                {
                    Destroy(modButtons[modName]);
                    modButtons.Remove(modName);
                }
                
                // 重新排列按钮
                RefreshModButtons();
            }
        }

        /// <summary>
        /// 更新内容容器大小
        /// </summary>
        private void UpdateContentSize()
        {
            int rowCount = Mathf.CeilToInt((float)modButtons.Count / columns);
            float contentHeight = Mathf.Max(400, rowCount * (buttonSize + buttonSpacing) + buttonSpacing);
            
            RectTransform contentRect = buttonContainer.GetComponent<RectTransform>();
            if (contentRect != null)
            {
                contentRect.sizeDelta = new Vector2(700, contentHeight);
            }
        }

        /// <summary>
        /// 关闭面板，与项目中其他面板保持一致的关闭方式
        /// </summary>
        private void CloseBT()
        {
            // 使用动画关闭面板
            transform.DOLocalMoveY(500f, 0.3f).SetEase(Ease.InBack, 1f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                
                // 如果有父对象的PanelA，则激活它
                if (transform.parent != null && transform.parent.Find("PanelA") != null)
                {
                    transform.parent.Find("PanelA").gameObject.SetActive(true);
                }
            });
        }
    }
}
