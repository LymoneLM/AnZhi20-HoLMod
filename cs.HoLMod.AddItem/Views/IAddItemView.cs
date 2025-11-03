using System;

namespace cs.HoLMod.AddItem;

public interface IAddItemView
{
    event Action OnFilterChanged;
    event Action OnCountInputChanged;
    event Action OnAddButton;
    
    bool ShowMenu { get; set; }
    
    // 页面状态
    MenuTab PanelTab { get; set; }
    
    // 货币界面
    CurrencyClass SelectedCurrency { get; set; }
    
    // 物品页面相关
    string SearchText { get; set; }
    PropClass? SelectedPropClass { get; set; }
    int? SelectedPropId { get; set; }
    
    // 话本相关
    public int? SelectedBookId { get; set; }
    
    // 控制栏
    string CountInput { get; set; }
}