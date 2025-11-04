using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuanAPI;

namespace cs.HoLMod.AddItem;

public partial class AddItemModule : IAddItemModel
{
    # region IAddItemModel 实现
    
    public event Action OnFilteredPropsChanged;
    public List<int> FilteredProps
    {
        get;
        private set
        {
            field = value;
            OnFilteredPropsChanged?.Invoke();
        }
    }
    
    # endregion

    #region 私有字段

    private Localization.LocalizationInstance _i18N;
    private Localization.LocalizationInstance _vStr;
    
    #endregion

    internal AddItemModule()
    {
        _i18N = Localization.CreateInstance(@namespace: AddItem.LocaleNamespace);
        _vStr = Localization.CreateInstance(@namespace: Localization.VanillaNamespace);

        FilteredProps = ItemData.AllProps;
    }
    
    public void FilterItems(int propClass = -1 , string search = "")
    {
        var list = propClass == -1 ? ItemData.AllProps : ItemData.ClassifiedProps[propClass];
        if (string.IsNullOrEmpty(search))
        {
            FilteredProps = list;
            return;
        }
        search = search.ToLower();
        FilteredProps = list.Where(index => _vStr.t($"Text_AllProp.{index}")
            .ToLower().Contains(search)).ToList();
    }

    public void AddCoins(int count)
    {
        FormulaData.ChangeCoins(count);
        MsgTool.TipMsg(_i18N.t("Tip.AddCoins.Succeed", count));
    }

    public void AddGold(int count)
    {
        var gold = int.Parse(Mainload.CGNum[1]);
        Mainload.CGNum[1] = (gold + count).ToString();
        MsgTool.TipMsg(_i18N.t("Tip.AddGold.Succeed", count));
    }
    
    /// <summary>
    /// 添加物品
    /// </summary>
    /// <param name="propId">ID</param>
    /// <param name="propCount">数量</param>
    public void AddProp(int propId, int propCount)
    {
        var flag = PropTool.AddProp(propId, propCount);
        var msg = _i18N.t($"Tip.AddItem.{(flag ? "Succeed" : "Failed")}", 
            args: [_vStr.t($"Text_AllProp.{propId}"), propCount]);
        MsgTool.TipMsg(msg, flag ? TipLv.Info : TipLv.Warning);
    }
    
    /// <summary>
    /// 添加话本
    /// </summary>
    /// <param name="bookId">话本ID</param>
    public void AddStoriesBook(int bookId)
    {
        // 验证话本ID是否有效
        if (bookId<0 || bookId >= ItemData.StoriesList.Count)
        {
            MsgTool.TipMsg(_i18N.t($"Tip.AddItem.None", bookId), TipLv.Warning);
            return;
        }
        
        if (!CheckIfBookExists(bookId))
        {
            Mainload.XiQuHave_Now.Add([
                bookId.ToString(),  // 话本ID
                "",                 // 位置信息（留空）
                "100"
            ]);
            MsgTool.TipMsg(_i18N.t($"Tip.AddItem.Succeed", args:ItemData.StoriesList[bookId]));
        }
        else
        {
            MsgTool.TipMsg(_i18N.t($"Tip.AddItem.Failed", args:ItemData.StoriesList[bookId]), TipLv.Warning);
        }
    }
    
    /// <summary>
    /// 检查话本是否已存在
    /// </summary>
    /// <param name="bookId">话本ID</param>
    /// <returns>是否存在</returns>
    private static bool CheckIfBookExists(int bookId)
    {
        if (Mainload.XiQuHave_Now == null || Mainload.XiQuHave_Now.Count == 0)
            return false;

        var bookIdStr = bookId.ToString();
        return Mainload.XiQuHave_Now.Any(book => book[0] == bookIdStr);
    }
}