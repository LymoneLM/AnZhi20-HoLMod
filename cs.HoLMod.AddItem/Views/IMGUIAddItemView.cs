using System;
using UnityEngine;
using YuanAPI;

namespace cs.HoLMod.AddItem;

public class IMGUIAddItemView : MonoBehaviour, IAddItemView
{
    // 窗口设置
    private Rect _windowRect;
    private Vector2 _scrollPosition; // 滚动位置
    private float _scaleFactor = 1.0f; // 分辨率缩放因子
    
    #region 实现 IAddItemView
    
    // 事件
    public event Action OnFilterChanged;
    public event Action OnCountInputChanged;
    public event Action OnAddButton;
    
    public bool ShowMenu { get; set; }
    
    // 页面状态
    public MenuTab PanelTab { get; set; } = MenuTab.Items;
    
    // 货币界面
    public CurrencyClass SelectedCurrency { get; set; } = CurrencyClass.Coins;
    
    // 物品页面相关
    public string SearchText { get; set; } = "";
    public PropClass? SelectedPropClass { get; set; }
    public int? SelectedPropId { get; set; }
    
    // 话本相关
    public int? SelectedBookId { get; set; }
    
    // 控制栏
    public string CountInput
    {
        get;
        set
        {
            field = value;
            OnCountInputChanged?.Invoke();
        }
    } = "1";

    #endregion

    private Localization.LocalizationInstance _i18N;
    private Localization.LocalizationInstance _vStr;
    
    private IAddItemModel _model;

    internal IMGUIAddItemView(IAddItemModel model)
    {
        _i18N = Localization.CreateInstance(@namespace: AddItem.LocaleNamespace);
        _vStr = Localization.CreateInstance(@namespace: Localization.VanillaNamespace);
        
        _model = model;
        
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
            _windowRect = new Rect(150, 100, defaultWidth + 100f, defaultHeight - 100f);
        }
        else if (currentWidth == 1920 && currentHeight == 1080)
        {
            // 中等分辨率，调整窗口位置和大小，控件缩放为0.8倍
            _scaleFactor = 0.8f;
            _windowRect = new Rect(100, 200, defaultWidth *1.2f, defaultHeight *1.2f - 500f);
        }
        else if (currentWidth == 1280 && currentHeight == 720)
        {
            // 低分辨率，调整窗口位置和大小，控件缩放为0.5倍
            _scaleFactor = 0.5f;
            _windowRect = new Rect(100, 200, defaultWidth *1.2f, defaultHeight *1.2f - 800f);
        }
        else
        {
            // 其他分辨率，使用中分辨率设置
            _scaleFactor = 0.8f;
            _windowRect = new Rect(100, 200, defaultWidth * 1.2f, defaultHeight * 1.2f - 500f);
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
            _windowRect.width = Mathf.Max(_windowRect.width, 800f * _scaleFactor);

            // 创建窗口
            _windowRect = GUI.Window(0, _windowRect, DrawWindow, "", GUI.skin.window);
            
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
                PanelTab = (MenuTab)index;
        });
        GUILayout.EndHorizontal();
        GUILayout.Space(10f * _scaleFactor);
        
        // 根据当前模式显示不同内容
        switch (PanelTab)
        {
            case MenuTab.Currency:
                DrawAddCurrency();
                break;
            case MenuTab.Items:
                DrawAddItem();
                break;
            case MenuTab.Stories:
                DrawAddStories();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        GUILayout.Space(10f * _scaleFactor);
        
        // 提示信息、数量输入框，添加按钮
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            //TODO: 当前添加物品显示
            
            // 仅在物品模式下显示数量输入框
            if (PanelTab == MenuTab.Items)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_i18N.t("Info.Count"), GUILayout.Width(100f * _scaleFactor));
                var countInputNow = GUILayout.TextField(CountInput, GUILayout.Width(200f * _scaleFactor));
                if(countInputNow != CountInput)
                    CountInput = countInputNow;
                GUILayout.EndHorizontal();
                GUILayout.Space(10f * _scaleFactor);
                
                //TODO: 快捷添加按键区
                
            }
            
            GUILayout.EndVertical();

            // 添加按钮
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(_i18N.t("Button.Add"),
                        GUILayout.Width(180f * _scaleFactor), GUILayout.Height(80f * _scaleFactor)))
                {
                    OnAddButton?.Invoke();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }
            
            GUILayout.EndHorizontal();
        }
        
        // 使用说明
        {
            // GUILayout.Space(20f * _scaleFactor);
            GUILayout.BeginVertical();
            
            GUILayout.Label(_i18N.t("Info.Description"), GUI.skin.box);
            for (var i = 1; i <= 6; i++)
                GUILayout.Label(_i18N.t($"Description.{i}"));
            
            GUILayout.EndVertical();
        }
        
        GUILayout.EndVertical();
        
        // 允许拖动窗口
        GUI.DragWindow(new Rect(0, 0, _windowRect.width, _windowRect.height));
    }

    private void DrawAddCurrency()
    {
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.Height(300f * _scaleFactor));
        
        // 货币类型选择
        GUILayout.Label(_i18N.t("Info.Category"), GUILayout.Width(200f * _scaleFactor));
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(_i18N.t("CurrencyClass.Coins"), GUILayout.Width(160f * _scaleFactor)))
        {
            SelectedCurrency = CurrencyClass.Coins;
        }
        if (GUILayout.Button(_i18N.t("CurrencyClass.Gold"), GUILayout.Width(160f * _scaleFactor)))
        {
            SelectedCurrency = CurrencyClass.Gold;
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10f * _scaleFactor);
        
        // 当前货币数量显示
        GUILayout.BeginVertical();
        GUILayout.Label(_i18N.t("Info.CurrencyNow.Coins",  FormulaData.GetCoinsNum()),
            GUILayout.ExpandWidth(true));
        GUILayout.Label(_i18N.t("Info.CurrencyNow.Gold", args: Mainload.CGNum[1]), 
            GUILayout.ExpandWidth(true));
        GUILayout.EndVertical();
        
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }

    private void DrawAddItem()
    {
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_i18N.t("Info.Search"), GUILayout.Width(160f * _scaleFactor));
            var newSearchText = GUILayout.TextField(SearchText, GUILayout.ExpandWidth(true));
            if (newSearchText != SearchText)
            {
                SearchText = newSearchText;
                OnFilterChanged?.Invoke();
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10f * _scaleFactor);
        
        // 物品分类按钮
        {
            GUILayout.Space(10f * _scaleFactor);
            GUILayout.BeginHorizontal();
            GUILayout.Label(_i18N.t("Info.Category")+_i18N.t($"PropClass.{SelectedPropClass.ToString()}"),
                GUILayout.Width(160f * _scaleFactor));
            if (GUILayout.Button(_i18N.t("Button.clear"), GUILayout.Width(120f * _scaleFactor)))
            {
                SearchText = null;
                SelectedPropClass = null;
                OnFilterChanged?.Invoke();
            }
            GUILayout.EndHorizontal();

            // 分类按钮布局 - 分多行显示
            Enum.GetNames(typeof(PropClass)).ForEach((cate, index) =>
            {
                if (index == 0 || index % 5 == 2) 
                    GUILayout.BeginHorizontal();

                if (GUILayout.Button(_i18N.t($"PropClass.{cate}"), 
                     GUILayout.Width((index < 2 ? 300 : 180) * _scaleFactor)))
                {
                    SelectedPropClass = (PropClass)Enum.Parse(typeof(PropClass), cate);
                    OnFilterChanged?.Invoke();
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
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // 创建物品列表
            if (_model.FilteredProps.Count > 0)
            {
                _model.FilteredProps.ForEach(propId =>
                {
                    if (GUILayout.Button(_vStr.t($"Text_AllProp.{propId}"), 
                            GUI.skin.button, GUILayout.ExpandWidth(true)))
                    {
                        SelectedPropId = propId;
                    }
                });
            }
            else
            {
                GUILayout.Label(_i18N.t("Error.NoProp"), GUILayout.ExpandWidth(true));
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
    }

    private void DrawAddStories()
    {
        // 话本模式
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.Height(300f * _scaleFactor));
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, 
            GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
        // 创建话本列表，根据语言显示对应文本
        ItemData.StoriesList.ForEach((book, index) =>
        {
            if (GUILayout.Button(book, GUILayout.ExpandWidth(true)))
            {
                SelectedBookId = index;
            }
        });
            
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
}
