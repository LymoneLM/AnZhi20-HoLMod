using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace cs.HoLMod.Tool
{
    public class Window : MonoBehaviour
    {
        // 窗口位置和大小设置
        private Rect windowRect = new Rect(20, 20, 1400, 600);
        private bool windowVisible = false;

        // 每列的宽度
        private int columnWidth = 260;
        private int columnSpacing = 20;

        // 输入缓存，用于存储用户在修改框中的输入
        private Dictionary<string, string> inputCache = new Dictionary<string, string>();
        
        // 存储每列的滚动位置
        private Vector2[] scrollPositions = new Vector2[5];

        // 存储每一级的选中路径
        private string selectedPath1 = null;
        private string selectedPath2 = null;
        private string selectedPath3 = null;
        private string selectedPath4 = null;
        private string selectedPath5 = null;

        // 存储Mainload中的所有数据数组
        private List<string> dataArrayNames = new List<string>();
        
        // 标记是否已添加列标题的语言条目
        private bool columnTitlesAdded = false;

        private void Start()
        {
            // 添加热键来切换窗口显示
            Debug.Log("cs.HoLMod.Tool: 按Tab键打开/关闭数据编辑器");
            
            // 初始化数据数组名称
            InitializeDataArrayNames();
        }

        private void Update()
        {
            // 按Tab键切换窗口显示状态
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                windowVisible = !windowVisible;
            }
        }

        private void OnGUI()
        {
            if (windowVisible)
            {
                windowRect = GUI.Window(0, windowRect, DrawWindow, LanguageManager.Instance.GetText("windowTitle"));
            }
        }

        private void InitializeDataArrayNames()
        {
            // 获取Main类中的_dataArrays字典内容
            Type mainType = typeof(Main);
            FieldInfo dataArraysField = mainType.GetField("_dataArrays", BindingFlags.NonPublic | BindingFlags.Static);
            
            if (dataArraysField != null)
            {
                Dictionary<string, object> dataArrays = dataArraysField.GetValue(null) as Dictionary<string, object>;
                if (dataArrays != null)
                {
                    dataArrayNames = new List<string>(dataArrays.Keys);
                }
            }
        }

        private void DrawWindow(int windowID)
        {
            // 语言切换按钮
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(LanguageManager.Instance.GetText("chinese"), GUILayout.Width(80))){
                LanguageManager.Instance.CurrentLanguage = LanguageManager.Language.Chinese;
            }
            if (GUILayout.Button(LanguageManager.Instance.GetText("english"), GUILayout.Width(80))){
                LanguageManager.Instance.CurrentLanguage = LanguageManager.Language.English;
            }
            GUILayout.EndHorizontal();
            
            // 创建水平布局来放置五列窗口
            GUILayout.BeginHorizontal();

            // 第一列：加载Mainload中的所有数据数组
            DrawColumn(0, selectedPath1, (indexStr) => 
            {
                if (int.TryParse(indexStr, out int index) && index >= 0 && index < dataArrayNames.Count)
                {
                    selectedPath1 = dataArrayNames[index]; // 使用实际的键名而非索引
                    selectedPath2 = null;
                    selectedPath3 = null;
                    selectedPath4 = null;
                    selectedPath5 = null;
                }
            });

            // 第二列：根据第一列的选择加载数据
            if (!string.IsNullOrEmpty(selectedPath1))
            {
                DrawColumn(1, selectedPath2, (path) => 
                {
                    selectedPath2 = path;
                    selectedPath3 = null;
                    selectedPath4 = null;
                    selectedPath5 = null;
                });
            }

            // 第三列：根据第二列的选择加载数据
            if (!string.IsNullOrEmpty(selectedPath1) && !string.IsNullOrEmpty(selectedPath2))
            {
                DrawColumn(2, selectedPath3, (path) => 
                {
                    selectedPath3 = path;
                    selectedPath4 = null;
                    selectedPath5 = null;
                });
            }

            // 第四列：根据第三列的选择加载数据
            if (!string.IsNullOrEmpty(selectedPath1) && !string.IsNullOrEmpty(selectedPath2) && !string.IsNullOrEmpty(selectedPath3))
            {
                DrawColumn(3, selectedPath4, (path) => 
                {
                    selectedPath4 = path;
                    selectedPath5 = null;
                });
            }

            // 第五列：根据第四列的选择加载数据
            if (!string.IsNullOrEmpty(selectedPath1) && !string.IsNullOrEmpty(selectedPath2) && !string.IsNullOrEmpty(selectedPath3) && !string.IsNullOrEmpty(selectedPath4))
            {
                DrawColumn(4, selectedPath5, (path) => 
                {
                    selectedPath5 = path;
                });
            }

            GUILayout.EndHorizontal();
            
            // 允许窗口拖动
            GUI.DragWindow();
        }

        private void DrawColumn(int columnIndex, string selectedPath, Action<string> onSelectPath)
        {
            GUILayout.BeginVertical(GUILayout.Width(columnWidth));
            
            // 显示列标题
            string columnTitle = LanguageManager.Instance.GetText("column") + " " + (columnIndex + 1);
            GUILayout.Label(columnTitle, GUILayout.Height(20));
            
            // 添加语言条目（如果还不存在）
            if (!columnTitlesAdded) {
                LanguageManager.Instance.AddLanguageEntry("column", "列", "Column");
                columnTitlesAdded = true;
            }
            
            // 添加滚动视图
            scrollPositions[columnIndex] = GUILayout.BeginScrollView(scrollPositions[columnIndex]);
            
            try
            {
                // 如果是第一列且数据数组名称为空，尝试重新初始化
                if (columnIndex == 0 && dataArrayNames.Count == 0)
                {
                    Debug.Log("重新初始化数据数组名称");
                    InitializeDataArrayNames();
                }
                
                // 根据列索引和选择的路径获取数据
                object data = GetDataByPath(columnIndex, selectedPath);
                
                // 显示数据
                if (data == null)
                {
                    // 显示加载为空
                    GUILayout.Label(LanguageManager.Instance.GetText("emptyArray"));
                }
                else if (IsArrayOrList(data))
                {
                    // 显示为N个按钮
                    int count = GetArrayCount(data);
                    for (int i = 0; i < count; i++)
                    {
                        string buttonText = GetObjectPreview(GetArrayElement(data, i));
                        if (GUILayout.Button(buttonText))
                        {
                            onSelectPath(i.ToString());
                        }
                    }
                }
                else
                {
                    // 显示为三列：原始值、修改文本框、修改按钮
                    GUILayout.BeginHorizontal();
                    
                    // 原始值
                    GUILayout.Label(LanguageManager.Instance.GetText("originalValue") + ": ", GUILayout.Width(80));
                    GUILayout.Label(data.ToString(), GUILayout.Width(80));
                    
                    // 修改文本框
                    string cacheKey = columnIndex + ":" + selectedPath;
                    if (!inputCache.ContainsKey(cacheKey))
                    {
                        inputCache[cacheKey] = data.ToString();
                    }
                    inputCache[cacheKey] = GUILayout.TextField(inputCache[cacheKey], GUILayout.Width(80));
                    
                    // 修改按钮
                    if (GUILayout.Button(LanguageManager.Instance.GetText("modify"), GUILayout.Width(60)))
                    {
                        // 尝试转换输入值并修改数据
                        object newValue = ConvertInputValue(inputCache[cacheKey], data);
                        if (newValue != null)
                        {
                            bool success = UpdateDataByPath(columnIndex, selectedPath, newValue);
                            if (success)
                            {
                                Debug.Log("数据修改成功");
                            }
                        }
                    }
                    
                    GUILayout.EndHorizontal();
                }
            }
            catch (Exception e)
            {
                GUILayout.Label("错误: " + e.Message);
            }
            
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private object GetDataByPath(int columnIndex, string selectedPath)
        {
            // 第一列：显示所有数据数组名称
            if (columnIndex == 0)
            {
                return dataArrayNames;
            }
            
            // 第二列及以后：根据选择的路径获取数据
            if (string.IsNullOrEmpty(selectedPath1))
                return null;
            
            // 获取Main类中的_dataArrays字典
            Type mainType = typeof(Main);
            FieldInfo dataArraysField = mainType.GetField("_dataArrays", BindingFlags.NonPublic | BindingFlags.Static);
            if (dataArraysField == null)
                return null;
            
            Dictionary<string, object> dataArrays = dataArraysField.GetValue(null) as Dictionary<string, object>;
            if (dataArrays == null || !dataArrays.ContainsKey(selectedPath1))
                return null;
            
            // 直接从Mainload获取最新数据而非使用缓存
            Type mainloadType = typeof(Mainload);
            PropertyInfo propInfo = mainloadType.GetProperty(selectedPath1, BindingFlags.Public | BindingFlags.Static);
            if (propInfo == null) return null;
            object currentData = propInfo.GetValue(null);
            
            // 第二列 - 总是返回完整数组，不处理selectedPath参数
            if (columnIndex == 1)
            {
                return currentData;
            }
            // 第三列
            else if (columnIndex == 2 && !string.IsNullOrEmpty(selectedPath2))
            {
                int index2;
                if (!int.TryParse(selectedPath2, out index2))
                    return null;
                
                object level2Data = GetArrayElement(currentData, index2);
                if (string.IsNullOrEmpty(selectedPath))
                    return level2Data;
                
                int index3;
                if (int.TryParse(selectedPath, out index3))
                {
                    return GetArrayElement(level2Data, index3);
                }
            }
            // 第四列
            else if (columnIndex == 3 && !string.IsNullOrEmpty(selectedPath2) && !string.IsNullOrEmpty(selectedPath3))
            {
                int index2;
                int index3;
                if (!int.TryParse(selectedPath2, out index2) || !int.TryParse(selectedPath3, out index3))
                    return null;
                
                object level2Data = GetArrayElement(currentData, index2);
                object level3Data = GetArrayElement(level2Data, index3);
                
                if (string.IsNullOrEmpty(selectedPath))
                    return level3Data;
                
                int index4;
                if (int.TryParse(selectedPath, out index4))
                {
                    return GetArrayElement(level3Data, index4);
                }
            }
            // 第五列
            else if (columnIndex == 4 && !string.IsNullOrEmpty(selectedPath2) && !string.IsNullOrEmpty(selectedPath3) && !string.IsNullOrEmpty(selectedPath4))
            {
                int index2;
                int index3;
                int index4;
                if (!int.TryParse(selectedPath2, out index2) || !int.TryParse(selectedPath3, out index3) || !int.TryParse(selectedPath4, out index4))
                    return null;
                
                object level2Data = GetArrayElement(currentData, index2);
                object level3Data = GetArrayElement(level2Data, index3);
                object level4Data = GetArrayElement(level3Data, index4);
                
                if (string.IsNullOrEmpty(selectedPath))
                    return level4Data;
                
                int index5;
                if (int.TryParse(selectedPath, out index5))
                {
                    return GetArrayElement(level4Data, index5);
                }
            }
            
            return null;
        }

        private bool UpdateDataByPath(int columnIndex, string selectedPath, object newValue)
        {
            // 获取Main类中的_dataArrays字典
            Type mainType = typeof(Main);
            FieldInfo dataArraysField = mainType.GetField("_dataArrays", BindingFlags.NonPublic | BindingFlags.Static);
            if (dataArraysField == null)
                return false;
            
            Dictionary<string, object> dataArrays = dataArraysField.GetValue(null) as Dictionary<string, object>;
            if (dataArrays == null || !dataArrays.ContainsKey(selectedPath1))
                return false;
            
            object currentData = dataArrays[selectedPath1];
            
            // 第二列
            if (columnIndex == 1 && !string.IsNullOrEmpty(selectedPath) && int.TryParse(selectedPath, out int index))
            {
                return SetArrayElement(currentData, index, newValue);
            }
            // 第三列
            else if (columnIndex == 2 && !string.IsNullOrEmpty(selectedPath2) && !string.IsNullOrEmpty(selectedPath) &&
                     int.TryParse(selectedPath2, out int index2) && int.TryParse(selectedPath, out int index3))
            {
                object level2Data = GetArrayElement(currentData, index2);
                return SetArrayElement(level2Data, index3, newValue);
            }
            // 第四列
            else if (columnIndex == 3 && !string.IsNullOrEmpty(selectedPath2) && !string.IsNullOrEmpty(selectedPath3) && !string.IsNullOrEmpty(selectedPath) &&
                     int.TryParse(selectedPath2, out index2) && int.TryParse(selectedPath3, out index3) && int.TryParse(selectedPath, out int index4))
            {
                object level2Data = GetArrayElement(currentData, index2);
                object level3Data = GetArrayElement(level2Data, index3);
                return SetArrayElement(level3Data, index4, newValue);
            }
            // 第五列
            else if (columnIndex == 4 && !string.IsNullOrEmpty(selectedPath2) && !string.IsNullOrEmpty(selectedPath3) && !string.IsNullOrEmpty(selectedPath4) && !string.IsNullOrEmpty(selectedPath) &&
                     int.TryParse(selectedPath2, out index2) && int.TryParse(selectedPath3, out index3) && int.TryParse(selectedPath4, out index4) && int.TryParse(selectedPath, out int index5))
            {
                object level2Data = GetArrayElement(currentData, index2);
                object level3Data = GetArrayElement(level2Data, index3);
                object level4Data = GetArrayElement(level3Data, index4);
                return SetArrayElement(level4Data, index5, newValue);
            }
            
            return false;
        }

        // 检查对象是否为数组或列表（根据用户要求简化，只检查是否为数组）
        private bool IsArrayOrList(object obj)
        {
            if (obj == null)
                return false;
            
            // 根据用户要求：Mainload.XXX必定为数组，只需要检查是否为数组
            return obj is Array || obj is System.Collections.IList;
        }

        // 获取数组或列表的元素数量（根据用户要求简化）
        private int GetArrayCount(object array)
        {
            if (array == null)
                return 0;
            
            // 根据用户要求：Mainload.XXX必定为数组
            if (array is Array arr)
                return arr.Length;
            else if (array is System.Collections.IList list)
                return list.Count;
            
            return 0;
        }

        // 获取数组或列表的指定索引元素（根据用户要求简化）
        private object GetArrayElement(object array, int index)
        {
            if (array == null || index < 0)
                return null;
            
            try
            {
                // 根据用户要求：Mainload.XXX必定为数组
                if (array is Array arr && index < arr.Length)
                    return arr.GetValue(index);
                else if (array is System.Collections.IList list && index < list.Count)
                    return list[index];
            }
            catch {}
            
            return null;
        }

        // 设置数组或列表的指定索引元素（根据用户要求简化）
        private bool SetArrayElement(object array, int index, object value)
        {
            if (array == null || index < 0)
                return false;
            
            try
            {
                // 根据用户要求：Mainload.XXX必定为数组
                if (array is Array arr && index < arr.Length)
                {
                    arr.SetValue(value, index);
                    return true;
                }
                else if (array is System.Collections.IList list && index < list.Count)
                {
                    list[index] = value;
                    return true;
                }
            }
            catch {}
            
            return false;
        }

        // 根据原始值的类型转换输入值
        private object ConvertInputValue(string input, object originalValue)
        {
            try
            {
                if (originalValue is int)
                {
                    return int.Parse(input);
                }
                else if (originalValue is float)
                {
                    return float.Parse(input);
                }
                else if (originalValue is double)
                {
                    return double.Parse(input);
                }
                else if (originalValue is bool)
                {
                    return bool.Parse(input);
                }
                else if (originalValue is string)
                {
                    return input;
                }
                // 可以添加更多类型的转换
                else
                {
                    // 默认作为字符串处理
                    return input;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("输入值转换失败: " + e.Message);
                return null;
            }
        }

        // 获取对象的预览文本
        private string GetObjectPreview(object obj)
        {
            if (obj == null)
                return "null";
            
            // 对于列表类型给出更有意义的预览
            if (obj is IList<object> array)
            {
                return $"数组 ({array.Count} 项)";
            }
            
            // 处理其他列表类型的情况
            if (obj.GetType().IsGenericType && 
                obj.GetType().GetInterface("System.Collections.IList") != null)
            {
                int count = ((System.Collections.IList)obj).Count;
                return $"列表 ({count} 项)";
            }
            
            return obj.ToString();
        }
    }
}