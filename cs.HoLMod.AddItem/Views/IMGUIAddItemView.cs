using System;
using System.Diagnostics;
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
            if (field == value) 
                return;
            field = value;
            OnCountInputChanged?.Invoke();
        }
    } = "1";

    #endregion

    private Localization.LocalizationInstance _i18N;
    private Localization.LocalizationInstance _vStr;
    
    private IAddItemModel _model;

    public void Initialize(IAddItemModel model)
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
    
    private static float _defaultWidth = 800f;
    private static float _defaultHeight = 1000f;
    // 分辨率更新处理方法
    private void UpdateResolutionSettings()
    {
        // 获取当前屏幕分辨率
        var currentWidth = (float)Screen.width;
        var currentHeight = (float)Screen.height;

        const float refW = 1920f;
        const float refH = 1080f;
        
        _scaleFactor = Mathf.Min(currentWidth / refW, currentHeight / refH);
        _windowRect = new Rect(150, 50, _defaultWidth, _defaultHeight);

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

            // 创建窗口（逻辑尺寸）
            _windowRect = GUI.Window(0, _windowRect, DrawWindow, 
                $"{_i18N.t("Mod.Name")} v{AddItem.VERSION} by:{_i18N.t("Mod.Author")}", GUI.skin.window);
            
            // 恢复原始矩阵
            GUI.matrix = guiMatrix;
        }
        
        // 恢复原始背景色
        GUI.backgroundColor = originalBackgroundColor;
    }
    
    private void DrawWindow(int windowID)
    {
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        
        // 菜单页面标签
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        Enum.GetNames(typeof(MenuTab)).ForEach((key, index) =>
        {
            if (GUILayout.Button($"{_i18N.t("MenuTab." + key)}", GUILayout.ExpandWidth(true)))
                PanelTab = (MenuTab)index;
        });
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        
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
        
        GUILayout.BeginVertical();
        
        GUILayout.FlexibleSpace();
        GUILayout.Space(10f);
        DrawAddButton();
        GUILayout.Space(10f);
        
        // 使用说明
        {
            GUILayout.BeginVertical();
            
            GUILayout.Label(_i18N.t("Info.Description"), GUI.skin.box);
            for (var i = 1; i <= 6; i++)
                GUILayout.Label(_i18N.t($"Description.{i}"));
            
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
        
        GUILayout.EndVertical();
        
        // 允许拖动窗口
        GUI.DragWindow(new Rect(0, 0, _windowRect.width, _windowRect.height));
    }

    private void DrawAddCurrency()
    {
        GUILayout.BeginVertical();
        
        // 货币类型选择
        GUILayout.Label(_i18N.t("Info.Category"), GUILayout.Width(160f));
        
        GUILayout.BeginVertical();
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition,
            GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(_i18N.t("CurrencyClass.Coins"), GUILayout.Width(160f)))
        {
            SelectedCurrency = CurrencyClass.Coins;
        }
        if (GUILayout.Button(_i18N.t("CurrencyClass.Gold"), GUILayout.Width(160f)))
        {
            SelectedCurrency = CurrencyClass.Gold;
        }
        GUILayout.EndHorizontal();
        
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        
        GUILayout.Space(10f);
        
        // 当前货币数量显示
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUILayout.Label(_i18N.t("Info.CurrencyNow.Coins",  FormulaData.GetCoinsNum()),
            GUILayout.ExpandWidth(true));
        GUILayout.Label(_i18N.t("Info.CurrencyNow.Gold", args: Mainload.CGNum[1]), 
            GUILayout.ExpandWidth(true));
        GUILayout.EndVertical();
        
        GUILayout.EndVertical();
    }

    private void DrawAddItem()
    {
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_i18N.t("Info.Search"), GUILayout.Width(160f));
            var newSearchText = GUILayout.TextField(SearchText, GUILayout.ExpandWidth(true));
            if (newSearchText != SearchText)
            {
                SearchText = newSearchText;
                OnFilterChanged?.Invoke();
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10f);
        
        // 物品分类按钮
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_i18N.t("Info.Category")+
                            (SelectedPropClass != null ? _i18N.t($"PropClass.{SelectedPropClass.ToString()}") : ""),
                GUILayout.Width(160f));
            if (GUILayout.Button(_i18N.t("Button.Clear"), GUILayout.Width(120f)))
            {
                SearchText = "";
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
                     GUILayout.Width(index < 2 ? 285f : 140f)))
                {
                    SelectedPropClass = (PropClass)Enum.Parse(typeof(PropClass), cate);
                    OnFilterChanged?.Invoke();
                }
                
                if (index % 5 == 1) 
                    GUILayout.EndHorizontal();
                
                if (index == 1)
                    GUILayout.Space(10f);
            });
        }
        
        GUILayout.Space(10f);
         
        // 物品列表滚动栏
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
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
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
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

    private void DrawAddButton()
    {
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            var text = PanelTab switch
            {
                MenuTab.Currency => _i18N.t($"CurrencyClass.{SelectedCurrency.ToString()}"),
                MenuTab.Items => SelectedPropId != null ? _vStr.t($"Text_AllProp.{SelectedPropId}") : "",
                MenuTab.Stories => SelectedBookId != null ? ItemData.StoriesList[(int)SelectedBookId] : "",
                _ => ""
            };
            GUILayout.Label($"{_i18N.t("Info.Selected")}{text}", GUILayout.ExpandWidth(true));

            GUILayout.Space(6f);

            // 数量输入框
            GUILayout.BeginHorizontal();
            GUILayout.Label(_i18N.t("Info.Count"), GUILayout.Width(100f));
            CountInput = GUILayout.TextField(CountInput, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);

            DrawQuickAddRow();
            
            GUILayout.EndVertical();
        }

        // 添加按钮
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        {
            GUILayout.Space(15f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_i18N.t("Button.Add"),
                    GUILayout.Width(180f), GUILayout.Height(80f)))
            {
                OnAddButton?.Invoke();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
            
        GUILayout.EndHorizontal();
    }
    
    private static (string label, int value)[] _kQuickAdds =
    [
        ("1", 1),
        ("100", 100),
        ("1w", 10_000),
        ("100w", 1_000_000),
        ("Max", int.MaxValue)
    ];

    private void DrawQuickAddRow()
    {
        GUILayout.BeginHorizontal();
        foreach (var (label, value) in _kQuickAdds)
        {
            if (GUILayout.Button(label, GUILayout.Width(80f), GUILayout.Height(36f)))
            {
                CountInput = value.ToString();
            }
            GUILayout.Space(6f);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

}
