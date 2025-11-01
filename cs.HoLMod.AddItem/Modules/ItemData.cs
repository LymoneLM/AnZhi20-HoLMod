﻿using System;
using System.Collections.Generic;
using System.Linq;
using YuanAPI;

namespace cs.HoLMod.AddItem;

public class ItemData
{
    // 数据
    internal static int AllPropCount;
    internal static List<int> AllPtops;
    internal static List<List<int>> ClassifiedProps;
    
    // 缓存需要处理的文本，避免多次处理
    internal static List<string> StoriesList;
    internal static List<string> JunList;
    internal static List<List<string>> XianList;

    internal static void RefreshText(string local)
    {
        var i18N = 
            Localization.CreateInstance(local, Localization.VanillaNamespace, false);

        StoriesList = [];
        var storiesCount = AllText.Text_AllXiQu.Count;
        for (var i = 0; i < storiesCount; i++)
        {
            var str = i18N.t($"Text_AllXiQu.Count.{i}").Split('|')[0];
            StoriesList.Add(str);
        }

        JunList = [];
        XianList = [];
        var cityCount = AllText.Text_City.Count;
        for (var i = 0; i < cityCount; i++)
        {
            var array = i18N.t($"Text_City.Count.{i}").Split('~');
            JunList.Add(array[0]);
            array = array[1].Split('|');
            XianList.Add(array.ToList());
        }
    }

    internal static void RefreshProp()
    {
        AllPropCount = Mainload.AllPropdata.Count;
        AllPtops = [];
        ClassifiedProps = [];
        
        var max = Enum.GetValues(typeof(PropClass)).Cast<int>().Max();
        for (var i = 0; i < max; i++)
            ClassifiedProps.Add([]);
        
        Mainload.AllPropdata.ForEach((propData, index) =>
        {
            var cate = int.Parse(propData[1]);
                cate = cate <= 2 ? 0 : cate;
                cate = cate == 8 ? 7 : cate;
                cate = cate > max ? 1 : cate;

            ClassifiedProps[cate].Add(index);
            AllPtops.Add(index);
        });
    }
}