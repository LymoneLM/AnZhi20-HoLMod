using System;
using System.Collections.Generic;
using UnityEngine;
using YuanAPI;

namespace cs.HoLMod.AddItem;

public class IMGUIView : BaseView
{
    // 窗口设置
    private Rect windowRect;
    private Vector2 scrollPosition; // 滚动位置
    private float _scaleFactor = 1.0f; // 分辨率缩放因子

    public bool ShowMenu { get; internal set; }
    
    // 界面模式控制（0：货币，1：物品，2：话本，3：地图）
    private MenuTab panelTab = MenuTab.Items; // 默认显示物品模式
    
    // 地图子模式控制（0：府邸，1：农庄，2：封地，3：世家）
    private int panelMapTab; // 默认显示府邸模式
    
    // 货币类型（0：铜钱，1：元宝）
    private int selectedCurrencyType;
    private int currencyValue = 100000; // 默认10万
    
    // 地图模式相关变量
    private string selectedPrefecture = "";
    private string selectedCounty = "";
    private int selectedJunIndex = -1; // 当前选择的郡索引
    private string mansionCustomName = "";
    private string farmCustomName = "";
    private int farmArea = 16; // 农庄面积，默认16
    private bool onlyAddMansion = true; // 添加府邸方式：true=仅添加府邸，false=添加后进入府邸
    
    // 界面控制变量
    private int selectedItemId;
    private int count = 1;
    private string searchText = "";
    private string countInput = "1";
    private string statusMessage;
    
    
    private List<int> filteredItemIds = [];
    // 用于组合搜索的分类过滤器
    private string selectedCategory = "";

    private Localization.LocalizationInstance _i18N;
    private Localization.LocalizationInstance _vStr;

    public IMGUIView()
    {
        AddItem.OnUpdate += Update;
        AddItem.OnOnGUI += OnGUI;

        _i18N = AddItem.I18N;
        _vStr = AddItem.VStr;
        
        UpdateResolutionSettings();
    }

    private void Update()
    {
        // 按F2键切换窗口显示
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ShowMenu = !ShowMenu;

            if (ShowMenu)
            {
                UpdateResolutionSettings();
                // 关闭地图面板，避免误操作
                Mainload.isMapPanelOpen = false;
            }
            
            AddItem.Logger.LogInfo($"物品添加器窗口已{(ShowMenu?"打开":"关闭")}" );
        }
        
        // 阻止游戏输入当窗口显示时（游戏会继续运行，但不允许操作游戏界面）
        if (!ShowMenu) 
            return;
        
        // TODO: 尝试是否需要这样阻止输入
        if (Input.mouseScrollDelta.y != 0 || // 阻止鼠标滚轮
            Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) && // 阻止鼠标点击
            Input.anyKeyDown && !Input.GetKeyDown(KeyCode.F2)) // 阻止键盘输入（保留F2键用于关闭窗口）
        {
            Input.ResetInputAxes();
        }
    }
    
    // 分辨率更新处理方法
    private void UpdateResolutionSettings()
    {
        // 获取当前屏幕分辨率
        var currentWidth = Screen.width;
        var currentHeight = Screen.height;
        
        // 默认窗口大小
        var defaultWidth = 1000f;
        var defaultHeight = 1200f;
        
        // TODO: 无级缩放
        
        // 根据分辨率设置窗口大小和缩放因子
        if (currentWidth == 2560 && currentHeight == 1440 || currentWidth == 3840 && currentHeight == 2160)
        {
            // 高分辨率，调整窗口位置和大小，控件缩放为1.0倍
            _scaleFactor = 1.0f;
            windowRect = new Rect(150, 100, defaultWidth + 100f, defaultHeight - 100f);
        }
        else if (currentWidth == 1920 && currentHeight == 1080)
        {
            // 中等分辨率，调整窗口位置和大小，控件缩放为0.8倍
            _scaleFactor = 0.8f;
            windowRect = new Rect(100, 200, defaultWidth *1.2f, defaultHeight *1.2f - 500f);
        }
        else if (currentWidth == 1280 && currentHeight == 720)
        {
            // 低分辨率，调整窗口位置和大小，控件缩放为0.5倍
            _scaleFactor = 0.5f;
            windowRect = new Rect(100, 200, defaultWidth *1.2f, defaultHeight *1.2f - 800f);
        }
        else
        {
            // 其他分辨率，使用中分辨率设置
            _scaleFactor = 0.8f;
            windowRect = new Rect(100, 200, defaultWidth * 1.2f, defaultHeight * 1.2f - 500f);
        }
        
        AddItem.Logger.LogInfo($"当前分辨率: {currentWidth}x{currentHeight}，缩放因子: {_scaleFactor}");
    }
    
    private void OnGUI()
    {
        if (!ShowMenu)
            return;
        
        // 保存窗口背景色并设置为半透明
        var originalBackgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f, 0.95f);

        {
            // 显示一个半透明的背景遮罩，防止操作游戏界面
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
            GUI.color = new Color(0, 0, 0, 0.1f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.EndGroup();

            // 应用缩放因子
            var guiMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
                new Vector3(_scaleFactor, _scaleFactor, 1f));
            
            // 根据缩放因子调整窗体内边距
            GUI.skin.window.padding = new RectOffset(
                Mathf.RoundToInt(20 * _scaleFactor),
                Mathf.RoundToInt(20 * _scaleFactor),
                Mathf.RoundToInt(10 * _scaleFactor),
                Mathf.RoundToInt(10 * _scaleFactor)
            );
            
            // 根据缩放因子调整字体大小
            var fontSize = Mathf.RoundToInt(18 * _scaleFactor);
            GUI.skin.textField.fontSize = fontSize;
            GUI.skin.window.fontSize = fontSize;
            GUI.skin.label.fontSize = fontSize;
            GUI.skin.button.fontSize = fontSize;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter; 
            GUI.skin.toggle.fontSize = fontSize;
            GUI.skin.toggle.alignment = TextAnchor.MiddleCenter; 
            
            // 调整窗口最小宽度
            windowRect.width = Mathf.Max(windowRect.width, 800f * _scaleFactor);

            // 创建窗口
            windowRect = GUI.Window(0, windowRect, DrawWindow, "", GUI.skin.window);
            
            // 恢复原始矩阵
            GUI.matrix = guiMatrix;
        }
        
        // 恢复原始背景色
        GUI.backgroundColor = originalBackgroundColor;
    }
    
    private void DrawWindow(int windowID)
    {
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        
        // 窗口最上方标题文本
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        GUILayout.FlexibleSpace();
        var titleStyle = new GUIStyle(GUI.skin.label);
        GUILayout.Label($"{_i18N.t("Mod.Name")} v{AddItem.VERSION} by:{_i18N.t("Mod.Author")}", 
            titleStyle, GUILayout.ExpandWidth(false));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(15f * _scaleFactor);
        
        // 菜单页面标签
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        Enum.GetNames(typeof(MenuTab)).ForEach((key, index) =>
        {
            if (GUILayout.Button($"{_i18N.t("MenuTab." + key)}", GUILayout.ExpandWidth(true)))
                panelTab = (MenuTab)index;
        });
        GUILayout.EndHorizontal();
        GUILayout.Space(10f * _scaleFactor);
        
        // 根据当前模式显示不同内容
        if (panelTab == MenuTab.Currency)
        {
            // 货币模式
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.Height(300f * _scaleFactor));
            
            // 货币类型选择
            GUILayout.BeginHorizontal();
            GUILayout.Label(LanguageManager.Instance.GetText("选择货币类型:"), GUILayout.Width(200f * _scaleFactor));
            if (GUILayout.Button(LanguageManager.Instance.GetText("铜钱"), GUILayout.Width(160f * _scaleFactor)))
            {
                selectedCurrencyType = 0; // 0表示铜钱
            }
            if (GUILayout.Button(LanguageManager.Instance.GetText("元宝"), GUILayout.Width(160f * _scaleFactor)))
            {
                selectedCurrencyType = 1; // 1表示元宝
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10f * _scaleFactor);
            
            // 数值输入
            GUILayout.BeginHorizontal();
            GUILayout.Label(LanguageManager.Instance.GetText("数值:"), GUILayout.Width(160f * _scaleFactor));
            var currencyValueInput = GUILayout.TextField(currencyValue.ToString(), GUILayout.Width(200f * _scaleFactor));
            if (int.TryParse(currencyValueInput, out var newCurrencyValue))
            {
                if (selectedCurrencyType == 0) // 铜钱
                {
                    currencyValue = Mathf.Clamp(newCurrencyValue, 0, 1000000000); // 限制在0到10亿之间
                }
                else // 元宝
                {
                    currencyValue = Mathf.Clamp(newCurrencyValue, 0, 100000); // 限制在0到10万之间
                }
            }
            // 显示输入限制
            GUILayout.Label(selectedCurrencyType == 0 ? LanguageManager.Instance.GetText("(0-10亿)") : LanguageManager.Instance.GetText("(0-10万)"), GUILayout.Width(140f * _scaleFactor));
            GUILayout.EndHorizontal();
            
            // 预设数值按钮
            GUILayout.BeginHorizontal();
            if (selectedCurrencyType == 0) // 铜钱
            {
                if (GUILayout.Button(LanguageManager.Instance.GetText("100万"), GUILayout.Width(160f * _scaleFactor)))
                {
                    currencyValue = 1000000;
                }
                if (GUILayout.Button(LanguageManager.Instance.GetText("1亿"), GUILayout.Width(160f * _scaleFactor)))
                {
                    currencyValue = 100000000;
                }
                if (GUILayout.Button(LanguageManager.Instance.GetText("10亿"), GUILayout.Width(160f * _scaleFactor)))
                {
                    currencyValue = 1000000000;
                }
            }
            else if (selectedCurrencyType == 1) // 元宝
            {
                if (GUILayout.Button(LanguageManager.Instance.GetText("1百"), GUILayout.Width(160f * _scaleFactor)))
                {
                    currencyValue = 100;
                }
                if (GUILayout.Button(LanguageManager.Instance.GetText("1千"), GUILayout.Width(160f * _scaleFactor)))
                {
                    currencyValue = 1000;
                }
                if (GUILayout.Button(LanguageManager.Instance.GetText("1万"), GUILayout.Width(160f * _scaleFactor)))
                {
                    currencyValue = 10000;
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10f * _scaleFactor);
            
            // 当前货币状态显示
            GUILayout.BeginVertical();
            GUILayout.Label(string.Format(LanguageManager.Instance.GetText("当前铜钱: {0}"), FormulaData.GetCoinsNum()), GUILayout.ExpandWidth(true));
            GUILayout.Label(LanguageManager.Instance.GetText("当前元宝: ") + Mainload.CGNum[1], GUILayout.ExpandWidth(true));
            GUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        } 
        else if (panelTab == MenuTab.Items)
        {
            // 搜索文本框，支持部分搜索
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_i18N.t("Info.Search"), GUILayout.Width(160f * _scaleFactor));
                var newSearchText = GUILayout.TextField(searchText, GUILayout.ExpandWidth(true));
                if (newSearchText != searchText)
                {
                    searchText = newSearchText;
                    FilterItems();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10f * _scaleFactor);
            
            // 物品分类按钮
            {
                GUILayout.Space(10f * _scaleFactor);
                GUILayout.BeginHorizontal();
                GUILayout.Label(LanguageManager.Instance.GetText("分类:"), GUILayout.Width(100f * _scaleFactor));
                if (GUILayout.Button(LanguageManager.Instance.GetText("清空"), GUILayout.Width(120f * _scaleFactor)))
                {
                    searchText = "";
                    selectedCategory = "";
                    FilterItems();
                }

                GUILayout.EndHorizontal();

                // 分类按钮布局 - 分多行显示
                Enum.GetNames(typeof(PropClass)).ForEach((cate, index) =>
                {
                if (index == 0 || index % 5 == 2) 
                    GUILayout.BeginHorizontal();

                if (GUILayout.Button(_i18N.t($"PropCategory.{cate}"), 
                     GUILayout.Width((index < 2 ? 300 : 180) * _scaleFactor)))
                { 
                    throw new NullReferenceException();
                }
                
                if (index == 0)
                    GUILayout.Space(10f * _scaleFactor);

                if (index % 5 == 1) 
                    GUILayout.EndHorizontal();
                });
            }
             
            // 物品列表滚动栏
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.Height(300f * _scaleFactor));
                scrollPosition = GUILayout.BeginScrollView(scrollPosition,
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                // 创建物品列表
                var selectedIndex = filteredItemIds.IndexOf(selectedItemId);
                if (selectedIndex == -1 && filteredItemIds.Count > 0)
                {
                    selectedIndex = 0;
                    selectedItemId = filteredItemIds[0];
                }

                if (filteredItemIds.Count > 0)
                {
                    for (var i = 0; i < filteredItemIds.Count; i++)
                    {
                        var isSelected = (i == selectedIndex);
                        var buttonStyle = isSelected ? GUI.skin.toggle : GUI.skin.button;

                        var itemName = itemList[filteredItemIds[i]].Item1;
                        var translatedItemName = LanguageManager.Instance.GetText(itemName);
                        if (GUILayout.Button(translatedItemName, buttonStyle, GUILayout.ExpandWidth(true)))
                        {
                            selectedItemId = filteredItemIds[i];
                        }
                    }
                }
                else
                {
                    GUILayout.Label(LanguageManager.Instance.GetText("未找到匹配的物品"), GUILayout.ExpandWidth(true));
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
        } 
        else if (panelTab == MenuTab.Stories)
        {
            // 话本模式
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.Height(300f * _scaleFactor));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
            // 创建话本列表，根据语言显示对应文本
            foreach (var book in bookList)
            {
                var displayName = LanguageManager.Instance.IsChineseLanguage() ? book.Value[0] : book.Value[1];
                if (GUILayout.Button(displayName, GUILayout.ExpandWidth(true)))
                {
                    selectedItemId = book.Key;
                }
            }
            
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        } 
        else if (panelTab == MenuTab.Map)
        {
            // 地图模式
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.Height(500f * _scaleFactor));
            
            // 地图子模式选择按钮 - 仅在地图模式下显示
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            if (GUILayout.Button(LanguageManager.Instance.GetText("府邸"), GUILayout.ExpandWidth(true)))
            {
                panelMapTab = 0;
            }
            if (GUILayout.Button(LanguageManager.Instance.GetText("农庄"), GUILayout.ExpandWidth(true)))
            {
                panelMapTab = 1;
            }
            if (GUILayout.Button(LanguageManager.Instance.GetText("封地"), GUILayout.ExpandWidth(true)))
            {
                panelMapTab = 2;
            }
            if (GUILayout.Button(LanguageManager.Instance.GetText("世家"), GUILayout.ExpandWidth(true)))
            {
                panelMapTab = 3;
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10f * _scaleFactor);
            
            // 根据选择的地图子模式显示内容
            GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
            if (panelMapTab == 0)
            {
                GUILayout.Label(LanguageManager.Instance.GetText("府邸子模式"), GUILayout.ExpandWidth(true));
                
                // 生成府邸所在郡
                GUILayout.Space(10f * _scaleFactor);
                GUILayout.Label(LanguageManager.Instance.GetText("生成府邸所在郡："), GUILayout.ExpandWidth(true));
                
                // 第一行6个郡按钮，使用固定宽度确保对齐
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                var buttonWidth = 180f * _scaleFactor; 
                for (var i = 0; i < 6 && i < JunList.Length; i++)
                {
                    var junName = JunList[i];
                    var translatedJunName = LanguageManager.Instance.GetText(junName);
                    var junIndex = i;
                    if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                    {
                        selectedPrefecture = junName;
                        selectedJunIndex = junIndex;
                        selectedCounty = "";
                    }
                }
                GUILayout.EndHorizontal();
                
                // 第二行6个郡按钮，使用固定宽度确保对齐
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                for (var i = 6; i < 12 && i < JunList.Length; i++)
                {
                    var junName = JunList[i];
                    var translatedJunName = LanguageManager.Instance.GetText(junName);
                    var junIndex = i;
                    if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                    {
                        selectedPrefecture = junName;
                        selectedJunIndex = junIndex;
                        selectedCounty = "";
                    }
                }
                GUILayout.EndHorizontal();
                
                // 生成府邸所在县
                GUILayout.Space(10f * _scaleFactor);
                GUILayout.Label(LanguageManager.Instance.GetText("生成府邸所在县："), GUILayout.ExpandWidth(true));
                
                // 只有选择了郡才显示县按钮
                if (selectedJunIndex >= 0 && selectedJunIndex < XianList.Length)
                {
                    var xianArray = XianList[selectedJunIndex];
                    var xianCount = xianArray.Length;
                    
                    // 所有县按钮都均布在一行
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    
                    // 设置每个县按钮的固定宽度，确保均匀分布
                    var xianButtonWidth = 120f * _scaleFactor; 
                    
                    for (var i = 0; i < xianCount; i++)
                    {
                        var xianName = xianArray[i];
                        var translatedXianName = LanguageManager.Instance.GetText(xianName);
                        if (GUILayout.Button(translatedXianName, GUILayout.Width(xianButtonWidth))) 
                        {
                            selectedCounty = xianName;
                        }
                    }
                    
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Label(LanguageManager.Instance.GetText("请先选择一个郡"), GUILayout.ExpandWidth(true));
                }
                
                // 显示选择的郡县
                GUILayout.Space(10f * _scaleFactor);
                var displayPrefectureCounty = string.IsNullOrEmpty(selectedPrefecture) || string.IsNullOrEmpty(selectedCounty) ? "--" : $"{LanguageManager.Instance.GetText(selectedPrefecture)}-{LanguageManager.Instance.GetText(selectedCounty)}";
                GUILayout.Label($"{LanguageManager.Instance.GetText("生成府邸所在的郡县：")}{displayPrefectureCounty}", GUILayout.ExpandWidth(true));
                
                // 府邸自定义名字
                GUILayout.Space(5f * _scaleFactor);
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                GUILayout.Label(LanguageManager.Instance.GetText("府邸的名字："), GUILayout.Width(120f * _scaleFactor));
                mansionCustomName = GUILayout.TextField(mansionCustomName, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
                
                // 添加府邸方式选择
                GUILayout.Space(5f * _scaleFactor);
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                var buttonStyle1 = new GUIStyle(GUI.skin.button);
                var buttonStyle2 = new GUIStyle(GUI.skin.button);
                
                if (onlyAddMansion)
                {
                    buttonStyle1.normal.textColor = Color.green;
                    buttonStyle1.fontStyle = FontStyle.Bold;
                }
                else
                {
                    buttonStyle2.normal.textColor = Color.green;
                    buttonStyle2.fontStyle = FontStyle.Bold;
                }
                
                if (GUILayout.Button(LanguageManager.Instance.GetText("仅添加府邸"), buttonStyle1, GUILayout.ExpandWidth(true)))
                {
                    onlyAddMansion = true;
                }
                if (GUILayout.Button(LanguageManager.Instance.GetText("添加后进入府邸"), buttonStyle2, GUILayout.ExpandWidth(true)))
                {
                    onlyAddMansion = false;
                }
                GUILayout.EndHorizontal();
            } 
            else if (panelMapTab == 1)
            {
                GUILayout.Label(LanguageManager.Instance.GetText("农庄子模式"), GUILayout.ExpandWidth(true));
                
                // 生成农庄所在郡
                GUILayout.Space(10f * _scaleFactor);
                GUILayout.Label(LanguageManager.Instance.GetText("生成农庄所在郡："), GUILayout.ExpandWidth(true));
                
                // 第一行6个郡按钮，使用固定宽度确保对齐
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                var buttonWidth = 180f * _scaleFactor; 
                for (var i = 0; i < 6 && i < JunList.Length; i++)
                {
                    var junName = JunList[i];
                    var translatedJunName = LanguageManager.Instance.GetText(junName);
                    var junIndex = i;
                    if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                    {
                        selectedPrefecture = junName;
                        selectedJunIndex = junIndex;
                        selectedCounty = "";
                    }
                }
                GUILayout.EndHorizontal();
                
                // 第二行6个郡按钮，使用固定宽度确保对齐
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                for (var i = 6; i < 12 && i < JunList.Length; i++)
                {
                    var junName = JunList[i];
                    var translatedJunName = LanguageManager.Instance.GetText(junName);
                    var junIndex = i;
                    if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                    {
                        selectedPrefecture = junName;
                        selectedJunIndex = junIndex;
                        selectedCounty = "";
                    }
                }
                GUILayout.EndHorizontal();
                
                // 生成农庄所在县
                GUILayout.Space(10f * _scaleFactor);
                GUILayout.Label(LanguageManager.Instance.GetText("生成农庄所在县："), GUILayout.ExpandWidth(true));
                
                // 只有选择了郡才显示县按钮
                if (selectedJunIndex >= 0 && selectedJunIndex < XianList.Length)
                {
                    var xianArray = XianList[selectedJunIndex];
                    var xianCount = xianArray.Length;
                    
                    // 所有县按钮都均布在一行
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    
                    // 设置每个县按钮的固定宽度，确保均匀分布
                    var xianButtonWidth = 120f * _scaleFactor; 
                    
                    for (var i = 0; i < xianCount; i++)
                    {
                        var xianName = xianArray[i];
                        var translatedXianName = LanguageManager.Instance.GetText(xianName);
                        if (GUILayout.Button(translatedXianName, GUILayout.Width(xianButtonWidth))) 
                        {
                            selectedCounty = xianName;
                        }
                    }
                    
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Label(LanguageManager.Instance.GetText("请先选择一个郡"), GUILayout.ExpandWidth(true));
                }
                
                // 显示选择的郡县
                GUILayout.Space(10f * _scaleFactor);
                var displayPrefectureCounty = string.IsNullOrEmpty(selectedPrefecture) || string.IsNullOrEmpty(selectedCounty) ? "--" : $"{LanguageManager.Instance.GetText(selectedPrefecture)}-{LanguageManager.Instance.GetText(selectedCounty)}";
                GUILayout.Label(LanguageManager.Instance.GetText("生成农庄所在的郡县：") + displayPrefectureCounty, GUILayout.ExpandWidth(true));
                
                // 农庄自定义名字
                GUILayout.Space(5f * _scaleFactor);
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                GUILayout.Label(LanguageManager.Instance.GetText("农庄的名字："), GUILayout.Width(120f * _scaleFactor));
                    farmCustomName = GUILayout.TextField(farmCustomName, GUILayout.ExpandWidth(true), GUILayout.MinWidth(250f * _scaleFactor));
                GUILayout.EndHorizontal();
                
                // 农庄面积选择
                GUILayout.Space(5f * _scaleFactor);
                GUILayout.Label(LanguageManager.Instance.GetText("农庄面积选择："), GUILayout.ExpandWidth(true));
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                if (GUILayout.Button(LanguageManager.Instance.GetText("4"), GUILayout.ExpandWidth(true)))
                {
                    farmArea = 4;
                }
                if (GUILayout.Button(LanguageManager.Instance.GetText("9"), GUILayout.ExpandWidth(true)))
                {
                    farmArea = 9;
                }
                if (GUILayout.Button(LanguageManager.Instance.GetText("16"), GUILayout.ExpandWidth(true)))
                {
                    farmArea = 16;
                }
                if (GUILayout.Button(LanguageManager.Instance.GetText("25"), GUILayout.ExpandWidth(true)))
                {
                    farmArea = 25;
                }
                GUILayout.EndHorizontal();
            } 
            else if (panelMapTab == 2)
            {
                GUILayout.Label(LanguageManager.Instance.GetText("封地子模式"), GUILayout.ExpandWidth(true));
                
                // 生成封地所在郡
                GUILayout.Space(10f * _scaleFactor);
                GUILayout.Label(LanguageManager.Instance.GetText("选择要解锁的郡："), GUILayout.ExpandWidth(true));
                
                // 第一行6个郡按钮，使用固定宽度确保对齐
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                var buttonWidth = 180f * _scaleFactor; 
                for (var i = 0; i < 6 && i < JunList.Length; i++)
                {
                    var junName = JunList[i];
                    var translatedJunName = LanguageManager.Instance.GetText(junName);
                    var junIndex = i;
                    if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                    {
                        selectedPrefecture = junName;
                        selectedJunIndex = junIndex;
                        selectedCounty = "";
                    }
                }
                GUILayout.EndHorizontal();
                
                // 第二行6个郡按钮，使用固定宽度确保对齐
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                for (var i = 6; i < 12 && i < JunList.Length; i++)
                {
                    var junName = JunList[i];
                    var translatedJunName = LanguageManager.Instance.GetText(junName);
                    var junIndex = i;
                    if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                    {
                        selectedPrefecture = junName;
                        selectedJunIndex = junIndex;
                        selectedCounty = "";
                    }
                }
                GUILayout.EndHorizontal();
                
                // 显示选择的郡
                GUILayout.Space(10f * _scaleFactor);
                var displayPrefecture = string.IsNullOrEmpty(selectedPrefecture) ? "--" : LanguageManager.Instance.GetText(selectedPrefecture);
                GUILayout.Label(LanguageManager.Instance.GetText("当前选择的郡：") + displayPrefecture, GUILayout.ExpandWidth(true));
                
                GUILayout.Space(10f * _scaleFactor);
                GUILayout.Label(LanguageManager.Instance.GetText("点击下方添加按钮即可解锁选择郡的所属封地"), GUILayout.ExpandWidth(true));
            } 
            else if (panelMapTab == 3)
            {
                GUILayout.Label(LanguageManager.Instance.GetText("世家子模式"), GUILayout.ExpandWidth(true));
                
                // 生成世家所在郡
                GUILayout.Space(10f * _scaleFactor);
                GUILayout.Label(LanguageManager.Instance.GetText("生成世家所在郡："), GUILayout.ExpandWidth(true));
                
                // 第一行6个郡按钮，使用固定宽度确保对齐
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                var buttonWidth = 180f * _scaleFactor; 
                for (var i = 0; i < 6 && i < JunList.Length; i++)
                {
                    var junName = JunList[i];
                    var translatedJunName = LanguageManager.Instance.GetText(junName);
                    var junIndex = i;
                    if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                    {
                        selectedPrefecture = junName;
                        selectedJunIndex = junIndex;
                        selectedCounty = "";
                    }
                }
                GUILayout.EndHorizontal();
                
                // 第二行6个郡按钮，使用固定宽度确保对齐
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                for (var i = 6; i < 12 && i < JunList.Length; i++)
                {
                    var junName = JunList[i];
                    var translatedJunName = LanguageManager.Instance.GetText(junName);
                    var junIndex = i;
                    if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                    {
                        selectedPrefecture = junName;
                        selectedJunIndex = junIndex;
                        selectedCounty = "";
                    }
                }
                GUILayout.EndHorizontal();
                
                // 生成世家所在县
                GUILayout.Space(10f * _scaleFactor);
                GUILayout.Label(LanguageManager.Instance.GetText("生成世家所在县："), GUILayout.ExpandWidth(true));
                
                // 只有选择了郡才显示县按钮
                if (selectedJunIndex >= 0 && selectedJunIndex < XianList.Length)
                {
                    var xianArray = XianList[selectedJunIndex];
                    var xianCount = xianArray.Length;
                    
                    // 所有县按钮都均布在一行
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    
                    // 设置每个县按钮的固定宽度，确保均匀分布
                    var xianButtonWidth = 120f * _scaleFactor; 
                    
                    for (var i = 0; i < xianCount; i++)
                    {
                        var xianName = xianArray[i];
                        var translatedXianName = LanguageManager.Instance.GetText(xianName);
                        if (GUILayout.Button(translatedXianName, GUILayout.Width(xianButtonWidth))) 
                        {
                            selectedCounty = xianName;
                        }
                    }
                    
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Label(LanguageManager.Instance.GetText("请先选择一个郡"), GUILayout.ExpandWidth(true));
                }
                
                // 显示选择的郡县
                GUILayout.Space(10f * _scaleFactor);
                var displayPrefectureCounty = string.IsNullOrEmpty(selectedPrefecture) || string.IsNullOrEmpty(selectedCounty) ? "--" : $"{LanguageManager.Instance.GetText(selectedPrefecture)}-{LanguageManager.Instance.GetText(selectedCounty)}";
                GUILayout.Label(LanguageManager.Instance.GetText("生成世家所在的郡县：") + displayPrefectureCounty, GUILayout.ExpandWidth(true));
            }
            
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        
        GUILayout.Space(10f * _scaleFactor);
        
        // 选择框、数量输入框、添加按钮
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            // 仅在物品模式下显示数量输入框
            if (panelTab == MenuTab.Items)
            {
                // 数量输入
                GUILayout.BeginHorizontal();
                GUILayout.Label(_i18N.t("Info.Count"), GUILayout.Width(100f * _scaleFactor));
                var newCountInput = GUILayout.TextField(countInput, GUILayout.Width(200f * _scaleFactor));
                if (newCountInput != countInput)
                {
                    countInput = newCountInput;
                    if (int.TryParse(countInput, out var newCount))
                    {
                        // 限制数量在0到10000之间
                        count = Mathf.Clamp(newCount, 0, 1000000);
                        // 如果输入的数量超出范围，自动修正显示
                        if (newCount != count)
                        {
                            countInput = count.ToString();
                        }
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(10f * _scaleFactor);
            }

            // 添加按钮
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(_i18N.t("Button.Add"),
                        GUILayout.Width(180f * _scaleFactor), GUILayout.Height(80f * _scaleFactor)))
                {
                    AddItemToGame();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        
        // 使用说明
        {
            // GUILayout.Space(20f * _scaleFactor);
            GUILayout.BeginVertical();

            // 标题
            GUILayout.Label(_i18N.t("Info.Description"), GUI.skin.box);
            
            for (var i = 1; i <= 5; i++)
                GUILayout.Label(_i18N.t($"Description.{i}"));
            
            GUILayout.EndVertical();
        }
        
        GUILayout.EndVertical();
        
        // 允许拖动窗口
        GUI.DragWindow(new Rect(0, 0, windowRect.width, windowRect.height));
    }
}
