using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using YuanAPI;

namespace cs.HoLMod.AddItem;

[BepInPlugin(MODGUID, MODNAME, VERSION)]
[BepInDependency(YuanAPIPlugin.MODGUID)]
public class AddItem : BaseUnityPlugin
{
    public const string MODGUID = "cs.HoLMod.AddItem.AnZhi20";
    public const string MODNAME = "HoLMod.AddItem";
    public const string VERSION = "3.0.0";
    
    public new static ManualLogSource Logger;
    public static Localization.LocalizationInstance I18N;
    public static Localization.LocalizationInstance VStr;
    
    public static BaseView UI;
    
    public static event Action OnUpdate;
    public static event Action OnOnGUI;
    
    
    private void Awake()
    {
        // 初始化
        Logger = base.Logger;
        I18N = Localization.CreateInstance(@namespace:"AddItem");
        VStr = Localization.CreateInstance(@namespace: Localization.VanillaNamespace);
        UI = new IMGUIView();

        Localization.OnLanguageChanged += ItemData.RefreshText;
        
        // 版本检查
        var versionConfig = Config.Bind(new ConfigDefinition("内部配置", "已加载版本"), VERSION,
            new ConfigDescription("用于跟踪插件版本，请勿手动修改"));
        
        if (versionConfig.Value != VERSION)
        {
            Logger.LogInfo($"检测到插件版本更新至 {VERSION}，配置更新");
            versionConfig.Value = VERSION;
        }
        
        Logger.LogInfo("物品添加器初始化完毕");
    }

    private void Start()
    {
        ItemData.RefreshProp();
        ItemData.RefreshText(I18N.Locale);
        
        
        
        Logger.LogInfo("物品添加器已加载完毕");
    }

    private void Update()
    {
        OnUpdate?.Invoke();
    }

    private void OnGUI()
    {
        OnOnGUI?.Invoke();
    }
}

