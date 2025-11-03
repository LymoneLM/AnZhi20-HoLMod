using System;
using UnityEngine;
using YuanAPI;

namespace cs.HoLMod.AddItem;

public class AddItemController : IAddItemController
{
    private IAddItemModel _model;
    private IAddItemView _view;

    private Localization.LocalizationInstance _i18N;
    
    internal AddItemController(IAddItemModel model, IAddItemView view)
    {
        _model = model;
        _view = view;

        _i18N = Localization.CreateInstance(@namespace: AddItem.LocaleNamespace);
        
        BindView();
    }
    
    private void BindView()
    {
        _view.OnFilterChanged += FilteredProps;
        _view.OnCountInputChanged += CheckCountInput;
        _view.OnAddButton += CallAddButton;
    }

    private void FilteredProps()
    {
        var selectClass = _view.SelectedPropClass == null ? -1 : (int)_view.SelectedPropClass;
        _model.FilterItems(selectClass, _view.SearchText);
    }

    private void CheckCountInput()
    {
        switch (_view.PanelTab)
        {
            case MenuTab.Currency:
                var count = int.Parse(_view.CountInput);
                count = _view.SelectedCurrency switch
                {
                    CurrencyClass.Coins => Mathf.Clamp(count, 1, 1000000000),   // 10亿
                    CurrencyClass.Gold => Mathf.Clamp(count, 1, 100000),        // 10万
                    _ => throw new ArgumentOutOfRangeException(),
                };
                _view.CountInput = count.ToString();
                break;
            case MenuTab.Items:
                var storage = int.Parse(Mainload.FamilyData[5]);
                storage = storage < 1 ? 1 : storage;
                _view.CountInput = Mathf.Clamp(int.Parse(_view.CountInput), 1, storage).ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CallAddButton()
    {
        switch (_view.PanelTab)
        {
            case MenuTab.Currency:
                WhenAddCurrency();
                break;
            case MenuTab.Items:
                WhenAddItem();
                break;
            case MenuTab.Stories:
                WhenAddStories();
                break;
            case MenuTab.Map:
                WhenAddMap();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    #region 添加按键处理
    
    private void WhenAddCurrency()
    {
        // TODO
    }
    private void WhenAddItem()
    {
        if (_view.SelectedPropId == null)
        {
            MsgTool.TipMsg(_i18N.t("Tip.SelectedNone"));
            return;
        }
        // TODO
    }
    private void WhenAddStories()
    {
        // TODO
    }
    private void WhenAddMap()
    {
        // TODO
    }
    
    #endregion
}