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
    
    private void AddItemToGame()
    {
        try
        {
            // 地图模式下不需要检查数量
            if (panelTab != 3 && (count <= 0 || count > 1000000))
            {
                statusMessage = LanguageManager.Instance.GetText("无效的数量，请输入1-1000000范围内的整数");
                AddItem.Logger.LogError("无效的数量: " + count);
                return;
            }
            
            if (panelTab == 1)
            {
                // 物品模式
                AddItemToGameInventory(selectedItemId, count);
            }
            else if (panelTab == 2)
            {
                // 话本模式
                AddStoriesBook(selectedItemId);
            } 
            else if (panelTab == 3)
            {
                // 地图模式
                if (panelMapTab == 0)
                {
                    // 府邸模式
                    if (string.IsNullOrEmpty(selectedPrefecture) || string.IsNullOrEmpty(selectedCounty))
                    {
                        statusMessage = LanguageManager.Instance.GetText("请先选择府邸所在的郡县");
                        AddItem.Logger.LogError("未选择郡县");
                        return;
                    }
                    
                    // 获取郡索引和对应的县索引
                    var junIndex = selectedJunIndex;
                    var xianIndex = -1;
                    if (junIndex >= 0 && junIndex < XianList.Length)
                    {
                        var xianArray = XianList[junIndex];
                        for (var i = 0; i < xianArray.Length; i++)
                        {
                            if (xianArray[i] == selectedCounty)
                            {
                                xianIndex = i;
                                break;
                            }
                        }
                    }
                    
                    if (xianIndex < 0)
                    {
                        statusMessage = LanguageManager.Instance.GetText("找不到选择的县");
                        AddItem.Logger.LogError("无效的县选择");
                        return;
                    }
                    
                    // 在添加府邸前检查Mainload.CityData_now中的势力名称
                    if (Mainload.CityData_now != null && junIndex < Mainload.CityData_now.Count)
                    {
                        // 获取郡城相关信息（第一个元素）
                        var junData = Mainload.CityData_now[junIndex] as IEnumerable;
                        if (junData != null)
                        {
                            // 获取第一个元素（郡城信息）
                            var cityInfoObj = junData.Cast<object>().FirstOrDefault();
                            if (cityInfoObj != null)
                            {
                                var cityInfo = cityInfoObj as List<string>;
                                if (cityInfo != null && cityInfo.Count > 0)
                                {
                                    // 获取城池控制势力信息（第一个元素）
                                    var controlInfo = cityInfo[0];
                                    if (!string.IsNullOrEmpty(controlInfo))
                                    {
                                        // 以|分割获取势力名称
                                        var parts = controlInfo.Split('|');
                                        if (parts.Length > 0 && parts[0] != "-1")
                                        {
                                            // 势力名称不为-1，不添加府邸
                                            statusMessage = string.Format(LanguageManager.Instance.GetText("{0}未解锁或郡城叛军未清剿，无法添加府邸"), selectedPrefecture);
                                            AddItem.Logger.LogWarning(statusMessage);
                                              
                                            // 显示提示信息
                                            ShowTipMessage(statusMessage);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                    // 获取用户输入的府邸名称，如果为空则使用默认名称
                    var mansionName = string.IsNullOrEmpty(mansionCustomName) ? $"{selectedPrefecture}府" : mansionCustomName;
                     
                    // 创建位置键
                    var locationKey = $"{junIndex}|{xianIndex}";
                     
                    // 检查该位置是否已存在府邸
                    var mansionExists = false;
                    if (Mainload.Fudi_now != null)
                    {
                        foreach (var mansion in Mainload.Fudi_now)
                        {
                            if (mansion != null && mansion.Count > 0 && mansion[0] == locationKey)
                            {
                                mansionExists = true;
                                break;
                            }
                        }
                    }
                     
                    if (mansionExists)
                    {
                        statusMessage = string.Format(LanguageManager.Instance.GetText("{0}-{1}已有府邸，添加失败"), selectedPrefecture, selectedCounty);
                        AddItem.Logger.LogWarning(statusMessage);
                         
                        // 显示提示信息
                        ShowTipMessage(statusMessage);
                        return;
                    }
                     
                    // 检查该位置是否已存在农庄（状态为-1的农庄）
                    var farmExists = CheckIfFarmExistsAtLocation(locationKey);
                      
                    if (farmExists)
                    {
                        statusMessage = string.Format(LanguageManager.Instance.GetText("{0}-{1}已有农庄，添加失败"), selectedPrefecture, selectedCounty);
                        AddItem.Logger.LogWarning(statusMessage);
                          
                        // 显示提示信息
                        ShowTipMessage(statusMessage);
                        return;
                    }
                     
                    // 检查该位置是否已存在世家
                    var shiJiaExists = false;
                    if (Mainload.ShiJia_Now != null)
                    {
                        foreach (var shiJia in Mainload.ShiJia_Now)
                        {
                            if (shiJia != null && shiJia.Count > 5 && shiJia[5] == locationKey)
                            {
                                shiJiaExists = true;
                                break;
                            }
                        }
                    }
                     
                    if (shiJiaExists)
                    {
                        statusMessage = string.Format(LanguageManager.Instance.GetText("{0}-{1}已有世家，添加失败"), selectedPrefecture, selectedCounty);
                        AddItem.Logger.LogWarning(statusMessage);
                         
                        // 显示提示信息
                        ShowTipMessage(statusMessage);
                        return;
                    }
                     
                    // 创建新的府邸数组
                    var newMansion = new List<string>
                    {
                        locationKey, // 0，府邸位置，郡索引|县索引
                        mansionName, // 1，府邸名称
                        "0",         // 2
                        "0",         // 3
                        "0",         // 4
                        "1",         // 5
                        "0",         // 6
                        "0",         // 7
                        "0",         // 8
                        "0",         // 9
                        "1",         // 10
                        "0",         // 11
                        "0",         // 12
                        "0",         // 13
                        "0",         // 14
                        "0",         // 15
                        "0",         // 16
                        "0",         // 17
                        "0",         // 18
                        "0",         // 19
                        "0",         // 20
                        "0",         // 21
                        "0",         // 22
                        "0",         // 23
                        "0",         // 24
                        "0",         // 25
                        "0",         // 26
                        "0",         // 27
                        "0",         // 28
                        "0",         // 29
                        "0",         // 30，环境属性值
                        "0",         // 31，安全属性值
                        "0",         // 32，便捷属性值
                        "0",         // 33
                        "0",         // 34
                        "0",         // 35
                        "0",         // 36
                        "25",        // 37
                        "0",         // 38
                        "0"          // 39
                    };
                    
                    // 添加到游戏中
                    try
                    {
                        //添加府邸数据并保存
                        Mainload.Fudi_now.Add(newMansion);
                        ES3.Save<List<List<string>>>("Fudi_now", Mainload.Fudi_now, Mainload.CunDangIndex_now + "/GameData.es3");

                        //添加府邸文件并保存
                        var value = new List<List<string>>();
                        ES3.Save<List<List<string>>>("BuildInto_m", value, Mainload.CunDangIndex_now + "/M" + (Mainload.Fudi_now.Count -1) + ".es3");
                        
                        //添加图标
                        Mainload.NewCreateShijia.Add(junIndex + "|" + xianIndex + "|-1");

                        //提示消息
                        statusMessage = string.Format(LanguageManager.Instance.GetText("已在({0}-{1})添加府邸: {2} "), LanguageManager.Instance.GetText(selectedPrefecture), LanguageManager.Instance.GetText(selectedCounty), mansionName);
                        AddItem.Logger.LogInfo(statusMessage);

                        //根据选择的添加方式执行相应操作
                        if (onlyAddMansion)
                        {
                            Add_notEnterHouse();
                        }
                        else
                        {
                            Add_EnterHouse();
                        }
                    }
                    catch (Exception ex)
                    {
                        statusMessage = string.Format(LanguageManager.Instance.GetText("添加府邸数据失败: {0}"), ex.Message);
                        AddItem.Logger.LogError(statusMessage);
                        AddItem.Logger.LogError("错误堆栈: " + ex.StackTrace);
                         
                        // 显示提示信息
                        ShowTipMessage(statusMessage);
                        return;
                    }
                    
                    // 显示提示信息
                    ShowTipMessage(statusMessage);
                }
                else if (panelMapTab == 1)
                {
                    // 农庄子模式
                    if (string.IsNullOrEmpty(selectedPrefecture) || string.IsNullOrEmpty(selectedCounty))
                    {
                        statusMessage = LanguageManager.Instance.GetText("请先选择农庄所在的郡县");
                        AddItem.Logger.LogError("未选择郡县");
                        return;
                    }
                    
                    // 获取郡索引和对应的县索引
                    var junIndex = selectedJunIndex;
                    var xianIndex = -1;
                    if (junIndex >= 0 && junIndex < XianList.Length)
                    {
                        var xianArray = XianList[junIndex];
                        for (var i = 0; i < xianArray.Length; i++)
                        {
                            if (xianArray[i] == selectedCounty)
                            {
                                xianIndex = i;
                                break;
                            }
                        }
                    }
                    
                    if (xianIndex < 0)
                    {
                        statusMessage = LanguageManager.Instance.GetText("找不到选择的县");
                        AddItem.Logger.LogError("无效的县选择");
                        return;
                    }
                    
                    // 在添加农庄前检查Mainload.CityData_now中的势力名称
                    if (Mainload.CityData_now != null && junIndex < Mainload.CityData_now.Count)
                    {
                        // 获取郡城相关信息（第一个元素）
                        var junData = Mainload.CityData_now[junIndex] as IEnumerable;
                        if (junData != null)
                        {
                            // 获取第一个元素（郡城信息）
                            var cityInfoObj = junData.Cast<object>().FirstOrDefault();
                            if (cityInfoObj != null)
                            {
                                var cityInfo = cityInfoObj as List<string>;
                                if (cityInfo != null && cityInfo.Count > 0)
                                {
                                    // 获取城池控制势力信息（第一个元素）
                                    var controlInfo = cityInfo[0];
                                    if (!string.IsNullOrEmpty(controlInfo))
                                    {
                                        // 以|分割获取势力名称
                                        var parts = controlInfo.Split('|');
                                        if (parts.Length > 0 && parts[0] != "-1")
                                        {
                                            // 势力名称不为-1，不添加农庄
                                            statusMessage = string.Format(LanguageManager.Instance.GetText("{0}未解锁或郡城叛军未清剿，无法添加农庄"), selectedPrefecture);
                                            AddItem.Logger.LogWarning(statusMessage);
                                              
                                            // 显示提示信息
                                            ShowTipMessage(statusMessage);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                    // 获取用户输入的农庄名称，如果为空则使用默认名称
                    var farmName = string.IsNullOrEmpty(farmCustomName) ? $"{selectedPrefecture}农庄" : farmCustomName;
                    
                    // 生成肥力字符串，根据面积设置对应数量的值为1
                    var fertilityBuilder = new StringBuilder();
                    var totalFields = 25; // 总地块数
                    for (var i = 0; i < totalFields; i++)
                    {
                        if (i < farmArea)
                        {
                            fertilityBuilder.Append("1");
                        }
                        else
                        {
                            fertilityBuilder.Append("0");
                        }
                        
                        if (i < totalFields - 1)
                        {
                            fertilityBuilder.Append("|");
                        }
                    }
                    var fertilityString = fertilityBuilder.ToString();
                    
                    // 创建位置键
                    var locationKey = $"{junIndex}|{xianIndex}";
                     
                    // 检查该位置是否已存在府邸
                    var mansionExists = false;
                    if (Mainload.Fudi_now != null)
                    {
                        foreach (var mansion in Mainload.Fudi_now)
                        {
                            if (mansion != null && mansion.Count > 0 && mansion[0] == locationKey)
                            {
                                mansionExists = true;
                                break;
                            }
                        }
                    }
                     
                    if (mansionExists)
                    {
                        statusMessage = string.Format(LanguageManager.Instance.GetText("{0}-{1}已有府邸，添加失败"), selectedPrefecture, selectedCounty);
                        AddItem.Logger.LogWarning(statusMessage);
                         
                        // 显示提示信息
                        ShowTipMessage(statusMessage);
                        return;
                    }
                     
                    // 检查该位置是否已存在世家
                    var shiJiaExists = false;
                    if (Mainload.ShiJia_Now != null)
                    {
                        foreach (var shiJia in Mainload.ShiJia_Now)
                        {
                            if (shiJia != null && shiJia.Count > 5 && shiJia[5] == locationKey)
                            {
                                shiJiaExists = true;
                                break;
                            }
                        }
                    }
                     
                    if (shiJiaExists)
                    {
                        statusMessage = string.Format(LanguageManager.Instance.GetText("{0}-{1}已有世家，添加失败"), selectedPrefecture, selectedCounty);
                        AddItem.Logger.LogWarning(statusMessage);
                         
                        // 添加显示信息
                        // 显示提示信息
                        ShowTipMessage(statusMessage);
                        return;
                    }
                     
                    // 添加到游戏中
                    try
                    {
                        // 使用Mainload.NongZ_now字段
                        if (Mainload.NongZ_now != null)
                        {
                            // 在Mainload.NongZ_now中查找对应位置的农庄
                            var found = false;
                             
                            // 外层循环遍历每个郡（Mainload.NongZ_now[i]表示一个郡的所有农庄）
                            for (var i = 0; i < Mainload.NongZ_now.Count; i++)
                            {
                                try 
                                {
                                    // 获取当前郡的所有农庄数组
                                    var junFarmList = Mainload.NongZ_now[i] as IEnumerable;
                                    if (junFarmList == null)
                                    {
                                        continue;
                                    }
                                     
                                    // 内层循环遍历当前郡中的每个农庄（Mainload.NongZ_now[i][j]表示一个具体的农庄）
                                    var j = 0;
                                    foreach (var farmObj in junFarmList)
                                    {
                                        try
                                        {
                                            // 将每个农庄对象转换为List<string>
                                            var farmItem = farmObj as List<string>;
                                            if (farmItem != null && farmItem.Count > 0)
                                            {
                                                // 查找位置信息，可能在不同索引位置
                                                var locationFound = false;
                                                var locationIndex = -1;
                                                 
                                                // 遍历farmItem查找位置信息
                                                for (var k = 0; k < farmItem.Count; k++)
                                                {
                                                    if (farmItem[k] == locationKey)
                                                    {
                                                        locationFound = true;
                                                        locationIndex = k;
                                                        break;
                                                    }
                                                }
                                                 
                                                if (locationFound)
                                                {
                                                    found = true;
                                                      
                                                    // 检查第一个元素的值
                                                    var status = farmItem[0];
                                                 
                                                    if (status != "0" && status != "-2")
                                                    {
                                                        // 该处已有农庄属于其它世家
                                                        statusMessage = LanguageManager.Instance.GetText("添加失败：该处已有农庄属于【其它世家】");
                                                        AddItem.Logger.LogWarning(statusMessage);
                                                    }
                                                    else if (status == "-1")
                                                    {
                                                        // 该处已有农庄属于你
                                                        statusMessage = LanguageManager.Instance.GetText("添加失败：该处已有农庄属于【你】");
                                                        AddItem.Logger.LogWarning(statusMessage);
                                                    }
                                                    else
                                                    {
                                                        // 状态为"0"，可以解锁农庄
                                                        // 修改对应的名字、面积、肥力
                                                        farmItem[6] = farmName; // 第七个元素：农庄名称
                                                        farmItem[5] = farmArea.ToString(); // 第六个元素：农庄面积
                                                        farmItem[2] = fertilityString; // 第三个元素：肥力信息
                                                          
                                                        // 修改第一个元素为"-1"表示添加成功
                                                        farmItem[0] = "-1";
                                                          
                                                        statusMessage = string.Format(LanguageManager.Instance.GetText("已解锁农庄: {0} ({1}-{2})，面积: {3}"), farmName, LanguageManager.Instance.GetText(selectedPrefecture), LanguageManager.Instance.GetText(selectedCounty), farmArea);
                                                        AddItem.Logger.LogInfo(statusMessage);
                                                        
                                                        // 设置场景更新标志
                                                        Mainload.isUpdateScene = true;
                                                    }
                                                      
                                                    // 找到后跳出所有循环
                                                    break;
                                                }
                                            }
                                        }
                                        catch (Exception innerEx)
                                        {
                                            AddItem.Logger.LogWarning($"处理第{i}个郡的第{j}个农庄时发生异常: {innerEx.Message}");
                                        }
                                        j++;
                                    }
                                     
                                    // 如果已经找到对应的农庄，则跳出外层循环
                                    if (found)
                                    {
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AddItem.Logger.LogWarning($"处理第{i}个郡时发生异常: {ex.Message}");
                                }
                            }
                            
                            // 已经在内部找到匹配后使用break跳出循环
                            
                            if (!found)
                            {
                                statusMessage = string.Format(LanguageManager.Instance.GetText("添加失败：你所选择的位置并未解锁，请解锁{0}郡再添加"), LanguageManager.Instance.GetText(selectedPrefecture));
                                AddItem.Logger.LogWarning(statusMessage);
                            }
                        }
                        else
                        {
                            throw new Exception("Mainload.NongZ_now不存在");
                        }
                    }
                    catch (Exception ex)
                    {
                        statusMessage = LanguageManager.Instance.GetText("添加农庄失败：") + ex.Message;
                        AddItem.Logger.LogError(statusMessage);
                        return;
                    }
                    
                    // 显示提示信息
                    ShowTipMessage(statusMessage);
                } 
                else if (panelMapTab == 2) 
                { 
                    // 封地子模式 - 解锁封地
                    if (string.IsNullOrEmpty(selectedPrefecture))
                    { 
                        statusMessage = LanguageManager.Instance.GetText("请先选择要解锁的郡"); 
                        AddItem.Logger.LogError("未选择郡"); 
                        return; 
                    } 
                    
                    // 获取郡索引
                    var junIndex = selectedJunIndex; 
                    if (junIndex < 0 || junIndex >= JunList.Length) 
                    { 
                        statusMessage = LanguageManager.Instance.GetText("无效的郡选择"); 
                        AddItem.Logger.LogError("无效的郡索引: " + junIndex); 
                        return; 
                    } 
                    
                    // 记录当前郡索引和Mainload.Fengdi_now的长度，用于调试
                    AddItem.Logger.LogInfo("选择的郡: " + JunList[junIndex] + "，索引: " + junIndex);
                    AddItem.Logger.LogInfo("Mainload.Fengdi_now长度: " + (Mainload.Fengdi_now?.Count ?? -1));
                    
                    try 
                    { 
                        // 在解锁封地前检查Mainload.CityData_now中的势力名称
                        if (Mainload.CityData_now != null && junIndex < Mainload.CityData_now.Count) 
                        { 
                            // 获取郡城相关信息（第一个元素）
                            var junData = Mainload.CityData_now[junIndex] as IEnumerable; 
                            if (junData != null) 
                            { 
                                // 获取第一个元素（郡城信息）
                                var cityInfoObj = junData.Cast<object>().FirstOrDefault(); 
                                if (cityInfoObj != null) 
                                { 
                                    var cityInfo = cityInfoObj as List<string>; 
                                    if (cityInfo != null && cityInfo.Count > 0) 
                                    { 
                                        // 获取城池控制势力信息（第一个元素）
                                        var controlInfo = cityInfo[0]; 
                                        if (!string.IsNullOrEmpty(controlInfo)) 
                                        { 
                                            // 以|分割获取势力名称
                                            var parts = controlInfo.Split('|'); 
                                            if (parts.Length > 0 && parts[0] != "-1") 
                                            { 
                                                // 势力名称不为-1，不解锁封地
                                                statusMessage = string.Format(LanguageManager.Instance.GetText("{0}未解锁或郡城叛军未清剿，无法解锁封地"), selectedPrefecture); 
                                                AddItem.Logger.LogWarning(statusMessage); 
                                                 
                                                // 显示提示信息
                                                ShowTipMessage(statusMessage); 
                                                return; 
                                            } 
                                        } 
                                    } 
                                } 
                            } 
                        } 
                        
                        // 检查Mainload.Fengdi_now数组是否存在
                        if (Mainload.Fengdi_now != null && Mainload.Fengdi_now.Count > 0) 
                        { 
                            // 郡索引+1与封地索引对应
                            if (junIndex + 1 < Mainload.Fengdi_now.Count) 
                            {
                                var fengDiData = Mainload.Fengdi_now[junIndex + 1];
                                if (fengDiData != null && fengDiData.Count > 0)
                                {
                                    // 检查封地是否已解锁 (第一个元素为0表示未解锁)
                                    if (fengDiData[0] == "1")
                                    {
                                        statusMessage = string.Format(LanguageManager.Instance.GetText("{0}的封地已解锁"), selectedPrefecture);
                                        AddItem.Logger.LogInfo(statusMessage);
                                        return;
                                    }

                                    // 解锁封地
                                    fengDiData[0] = "1";
                                    statusMessage = string.Format(LanguageManager.Instance.GetText("{0}的封地已解锁"), selectedPrefecture);
                                    AddItem.Logger.LogInfo(statusMessage);
                                }  
                            }
                            else
                            {
                                statusMessage = string.Format(LanguageManager.Instance.GetText("{0}的封地数据不存在或索引错误"), selectedPrefecture);
                                AddItem.Logger.LogWarning(statusMessage);
                            }
                        } 
                        else 
                        { 
                            throw new Exception("Mainload.Fengdi_now不存在或为空"); 
                        }
                    }
                    catch (Exception ex)
                    {
                        statusMessage = LanguageManager.Instance.GetText("解锁封地失败: ") + ex.Message;
                        AddItem.Logger.LogError(statusMessage);
                        AddItem.Logger.LogError("错误堆栈: " + ex.StackTrace);
                        return; 
                    } 
                    
                    // 显示提示信息
                    ShowTipMessage(statusMessage); 
                } 
                else if (panelMapTab == 3) 
                { 
                    // 世家子模式 - 暂不实现，直接提示功能正在开发中
                    statusMessage = LanguageManager.Instance.GetText("添加失败：功能正在开发中");
                    AddItem.Logger.LogWarning(statusMessage);
                    
                    // 显示提示信息
                    ShowTipMessage(statusMessage); 
                }
            } 
            else if (panelTab == 0)
            {
                // 货币模式
                if (currencyValue <= 0)
                {
                    statusMessage = LanguageManager.Instance.GetText("请输入有效的数值");
                    AddItem.Logger.LogError("无效的数值: " + currencyValue);
                    return;
                }
                
                if (selectedCurrencyType == 0)
                {
                    // 添加铜钱
                    FormulaData.ChangeCoins(currencyValue);
                    statusMessage = string.Format(LanguageManager.Instance.GetText("已添加{0}铜钱"), currencyValue);
                    AddItem.Logger.LogInfo(statusMessage);
                }
                else if (selectedCurrencyType == 1)
                {
                    // 添加元宝
                    if (int.TryParse(Mainload.CGNum[1], out var currentYuanBao))
                    {
                        Mainload.CGNum[1] = (currentYuanBao + currencyValue).ToString();
                        statusMessage = string.Format(LanguageManager.Instance.GetText("已添加{0}元宝"), currencyValue);
                        AddItem.Logger.LogInfo(statusMessage);
                    }
                    else
                    {
                        statusMessage = LanguageManager.Instance.GetText("获取当前元宝数量失败");
                        AddItem.Logger.LogError("获取当前元宝数量失败");
                        return;
                    }
                }
                
                // 显示提示信息
                ShowTipMessage(statusMessage);
            }

        }
        catch (Exception ex)
        {
            statusMessage = LanguageManager.Instance.GetText("添加物品失败: ") + ex.Message;
            AddItem.Logger.LogError("添加物品失败: " + ex.Message);
        }
    }
    
    /// <summary>
    /// 添加物品
    /// </summary>
    /// <param name="propId">ID</param>
    /// <param name="propCount">数量</param>
    public void AddProp(int propId, int propCount)
    {
        var flag = PropTool.AddProp(propId, propCount);
        MsgTool.TipMsg(_i18N.t($"Tip.AddItem.{(flag?"Succeed":"Failed")}",
                args:[_vStr.t($"Text_AllProp.{propId}"), propCount]),
            flag ? TipLv.Info : TipLv.Warning);
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
    
    // 检查指定位置是否存在状态为-1的农庄
    private bool CheckIfFarmExistsAtLocation(string locationKey)
    {
        try
        {
            if (Mainload.NongZ_now == null)
                return false;
            
            // 遍历每个郡的所有农庄
            foreach (var junFarmListObj in Mainload.NongZ_now)
            {
                var junFarmList = junFarmListObj as IEnumerable;
                if (junFarmList == null)
                    continue;
                
                // 遍历当前郡中的每个农庄
                foreach (var farmObj in junFarmList)
                {
                    try
                    {
                        var farmItem = farmObj as List<string>;
                        if (farmItem != null && farmItem.Count > 0 && farmItem.Contains(locationKey))
                        {
                            // 检查农庄状态，只有状态为-1时才视为存在
                            if (farmItem[0] == "-1")
                            {
                                AddItem.Logger.LogWarning(string.Format(LanguageManager.Instance.GetText("发现已存在的农庄，位置: {0}，状态: {1}"), locationKey, farmItem[0]));
                                return true;
                            }

                            AddItem.Logger.LogWarning(string.Format(LanguageManager.Instance.GetText("发现状态为0的农庄，位置: {0}，允许添加府邸"), locationKey));
                        }
                    }
                    catch (Exception ex)
                    {
                        AddItem.Logger.LogError(string.Format(LanguageManager.Instance.GetText("检查农庄时出错: {0}"), ex.Message));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            AddItem.Logger.LogError(string.Format(LanguageManager.Instance.GetText("CheckIfFarmExistsAtLocation 方法出错: {0}"), ex.Message));
        }
        
        return false;
    }

    // 添加后不进入府邸
    private void Add_notEnterHouse()
    {
        //打开地图
        Mainload.isMapPanelOpen = true;

        //隐藏窗口
        showMenu = false;
    }

    // 添加后进入府邸
    private void Add_EnterHouse()
    {
        //不打开地图
        Mainload.isMapPanelOpen = false;

        //进入新添加的府邸
        Mainload.SceneID = "M|" + (Mainload.Fudi_now.Count -1);
    }
    
    // 显示话本已存在的提示
    private void ShowBookExistsMessage(int bookId)
    {
        statusMessage = string.Format(LanguageManager.Instance.GetText("话本{0}已存在，不重复添加"), bookList[bookId][0]);
        AddItem.Logger.LogInfo(statusMessage);
        
        // 获取显示名称
        var displayName = LanguageManager.Instance.IsChineseLanguage() ? bookList[bookId][0] : bookList[bookId][1];
        
        // 显示添加失败提示
        var errorMessage = string.Format(LanguageManager.Instance.GetText("添加失败: 话本{0}已存在"), displayName);
        ShowTipMessage(errorMessage);
    }
    
    // 显示提示信息到游戏界面
    private void ShowTipMessage(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            Mainload.Tip_Show.Add(new List<string>
            {
                "1",
                message
            });
        }
    }
}