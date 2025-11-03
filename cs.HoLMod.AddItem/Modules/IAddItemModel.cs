using System;
using System.Collections.Generic;

namespace cs.HoLMod.AddItem;

public interface IAddItemModel
{
    event Action OnFilteredPropsChanged;
    List<int> FilteredProps { get; }
    
    // 方法
    void FilterItems(int propClass = -1, string search = "");
    void AddCoins(int count);
    void AddGold(int count);
    void AddProp(int propId, int propCount);
    void AddStoriesBook(int bookId);
}