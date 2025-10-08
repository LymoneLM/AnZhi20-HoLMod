using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace cs.HoLMod.Tool
{
    public class Window : MonoBehaviour
    {
        // 窗口位置和大小设置
        private Rect windowRect = new Rect(20, 20, 1200, 600);
        private bool windowVisible = false;

        // 每列的宽度
        private int columnWidth = 280;
        private int columnSpacing = 20;

        // 输入缓存，用于存储用户在修改框中的输入
        private Dictionary<string, string> inputCache = new Dictionary<string, string>();
        
        // 存储每列的滚动位置
        private Vector2[] scrollPositions = new Vector2[4];

        // 存储每一级的选中路径
        private string selectedPath1 = null;
        private string selectedPath2 = null;
        private string selectedPath3 = null;
        private string selectedPath4 = null;

        private void Start()
        {
            // 添加热键来切换窗口显示
            Debug.Log("cs.HoLMod.Tool: 按Tab键打开/关闭数据编辑器");
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
                windowRect = GUI.Window(0, windowRect, DrawWindow, "数据编辑器");
            }
        }

        private void DrawWindow(int windowID)
        {
            // 创建水平布局来放置四列窗口
            GUILayout.BeginHorizontal();

            // 第一列：显示Mainload.XXX[i]
            DrawColumn(0, selectedPath1, (path) => 
            {
                selectedPath1 = path;
                selectedPath2 = null;
                selectedPath3 = null;
                selectedPath4 = null;
            });

            // 第二列：根据第一列的选择显示Mainload.XXX[i][j]
            if (!string.IsNullOrEmpty(selectedPath1))
            {
                object firstLevelData = GetDataByPath(selectedPath1);
                // 只有当第一列加载的元素是数组时才加载第二列
                if (firstLevelData is IList<object> && ((IList<object>)firstLevelData).Count > 0)
                {
                    DrawColumn(1, selectedPath2, (path) => 
                    {
                        selectedPath2 = path;
                        selectedPath3 = null;
                        selectedPath4 = null;
                    }, selectedPath1);
                }
                else
                {
                    DrawEmptyColumn("无可用数据");
                }
            }
            else
            {
                DrawEmptyColumn("请在左侧选择一个项目");
            }

            // 第三列：根据第二列的选择显示Mainload.XXX[i][j][k]
            if (!string.IsNullOrEmpty(selectedPath2))
            {
                DrawColumn(2, selectedPath3, (path) => 
                {
                    selectedPath3 = path;
                    selectedPath4 = null;
                }, selectedPath2);
            }
            else
            {
                DrawEmptyColumn("请在左侧选择一个项目");
            }

            // 第四列：根据第三列的选择显示Mainload.XXX[i][j][k][l]
            if (!string.IsNullOrEmpty(selectedPath3))
            {
                DrawColumn(3, selectedPath4, (path) => 
                {
                    selectedPath4 = path;
                }, selectedPath3);
            }
            else
            {
                DrawEmptyColumn("请在左侧选择一个项目");
            }

            GUILayout.EndHorizontal();

            // 允许窗口拖动
            GUI.DragWindow();
        }

        // 绘制空列
        private void DrawEmptyColumn(string message)
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(columnWidth), GUILayout.ExpandHeight(true));
            GUILayout.Label(message);
            GUILayout.EndVertical();
            GUILayout.Space(columnSpacing);
        }

        // 绘制单个列
        private void DrawColumn(int columnIndex, string currentPath, Action<string> onPathSelected, string dataPath = null)
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(columnWidth), GUILayout.ExpandHeight(true));

            if (columnIndex == 0)
            {
                // 第一列显示Mainload.XXX数组
                GUILayout.Label("选择顶层数据", GUI.skin.label);
                GUILayout.Space(5);
                DrawMainLoadArrays();
            }
            else if (!string.IsNullOrEmpty(dataPath))
            {
                // 其他列显示选定的数据内容
                object data = GetDataByPath(dataPath);
                if (data != null)
                {
                    if (data is IList<object> arrayData)
                    {
                        if (arrayData.Count == 0)
                        {
                            GUILayout.Label("空数组");
                        }
                        else
                        {
                            DrawArrayItems(arrayData, dataPath, currentPath, onPathSelected, columnIndex);
                        }
                    }
                    else
                    {
                        // 如果是基本类型，显示修改界面
                        DrawValueEditor(dataPath, data);
                    }
                }
                else
                {
                    GUILayout.Label("数据为空");
                }
            }

            GUILayout.EndVertical();
            GUILayout.Space(columnSpacing);
        }
        
        // 获取Mainload中的数据
        private object GetMainLoadData()
        {
            try
            {
                // 尝试获取Mainload对象
                Type mainType = typeof(Main);
                FieldInfo mainLoadField = mainType.GetField("Mainload", BindingFlags.Public | BindingFlags.Static);
                if (mainLoadField != null)
                {
                    return mainLoadField.GetValue(null);
                }
                
                // 如果不存在Mainload静态字段，尝试获取Main实例中的Mainload属性
                Main mainInstance = Main.Instance;
                if (mainInstance != null)
                {
                    PropertyInfo mainLoadProperty = mainType.GetProperty("Mainload", BindingFlags.Public | BindingFlags.Instance);
                    if (mainLoadProperty != null)
                    {
                        return mainLoadProperty.GetValue(mainInstance, null);
                    }
                    
                    // 尝试获取字段
                    FieldInfo instanceField = mainType.GetField("Mainload", BindingFlags.Public | BindingFlags.Instance);
                    if (instanceField != null)
                    {
                        return instanceField.GetValue(mainInstance);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("获取Mainload数据失败: " + e.Message);
            }
            
            return null;
        }

        // 根据路径获取数据
        private object GetDataByPath(string path)
        {
            try
            {
                // 从路径解析各级索引
                string[] parts = path.Split('.');
                object currentData = GetMainLoadData();
                
                // 跳过第一个部分("Mainload")
                for (int i = 1; i < parts.Length; i++)
                {
                    if (currentData == null)
                        return null;
                    
                    if (int.TryParse(parts[i], out int index))
                    {
                        // 尝试作为数组访问
                        if (currentData is IList<object> list)
                        {
                            if (index >= 0 && index < list.Count)
                            {
                                currentData = list[index];
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        // 尝试作为属性或字段访问
                        Type type = currentData.GetType();
                        PropertyInfo property = type.GetProperty(parts[i]);
                        if (property != null)
                        {
                            currentData = property.GetValue(currentData, null);
                        }
                        else
                        {
                            FieldInfo field = type.GetField(parts[i]);
                            if (field != null)
                            {
                                currentData = field.GetValue(currentData);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                
                return currentData;
            }
            catch (Exception e)
            {
                Debug.LogError("根据路径获取数据失败: " + e.Message);
                return null;
            }
        }

        // 绘制Mainload中的顶层数组
        private void DrawMainLoadArrays()
        {
            scrollPositions[0] = GUILayout.BeginScrollView(scrollPositions[0], false, true, GUILayout.ExpandHeight(true));

            object mainLoadData = GetMainLoadData();
            if (mainLoadData != null)
            {
                Type mainLoadType = mainLoadData.GetType();
                
                // 获取所有公共属性和字段
                PropertyInfo[] properties = mainLoadType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                FieldInfo[] fields = mainLoadType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                
                // 先处理属性
                foreach (PropertyInfo property in properties)
                {
                    if (GUILayout.Button(property.Name, GUILayout.Height(25)))
                    {
                        selectedPath1 = "Mainload." + property.Name;
                        selectedPath2 = null;
                        selectedPath3 = null;
                        selectedPath4 = null;
                    }
                }
                
                // 再处理字段
                foreach (FieldInfo field in fields)
                {
                    if (GUILayout.Button(field.Name, GUILayout.Height(25)))
                    {
                        selectedPath1 = "Mainload." + field.Name;
                        selectedPath2 = null;
                        selectedPath3 = null;
                        selectedPath4 = null;
                    }
                }
            }
            else
            {
                GUILayout.Label("无法获取Mainload数据");
            }

            GUILayout.EndScrollView();
        }

        // 绘制数组中的项目
        private void DrawArrayItems(IList<object> array, string basePath, string currentPath, Action<string> onPathSelected, int columnIndex)
        {
            scrollPositions[columnIndex] = GUILayout.BeginScrollView(scrollPositions[columnIndex], false, true, GUILayout.ExpandHeight(true));

            for (int i = 0; i < array.Count; i++)
            {
                string itemPath = $"{basePath}.{i}";
                object item = array[i];

                string buttonText = item is IList<object> ? 
                    $"[{i}] 嵌套数组 ({((IList<object>)item).Count} 项)" : 
                    $"[{i}] {GetObjectPreview(item)}";

                if (GUILayout.Button(buttonText, GUILayout.Height(25)))
                {
                    if (onPathSelected != null)
                    {
                        onPathSelected(itemPath);
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        // 绘制值编辑器，用于修改基本类型的值
        private void DrawValueEditor(string dataPath, object value)
        {
            GUILayout.Label("原始值", GUI.skin.label);
            GUILayout.TextField(GetObjectPreview(value), GUILayout.Height(25));
            GUILayout.Space(10);

            GUILayout.Label("新值", GUI.skin.label);
            
            // 获取或创建输入缓存
            string inputKey = $"{dataPath}_input";
            if (!inputCache.ContainsKey(inputKey))
            {
                inputCache[inputKey] = value?.ToString() ?? "";
            }
            
            inputCache[inputKey] = GUILayout.TextField(inputCache[inputKey], GUILayout.Height(25));
            GUILayout.Space(10);

            if (GUILayout.Button("修改", GUILayout.Height(30)))
            {
                // 尝试根据原始值的类型转换输入值
                object newValue = ConvertInputValue(inputCache[inputKey], value);
                if (newValue != null)
                {
                    if (ModifyDataByPath(dataPath, newValue))
                    {
                        Debug.Log($"数据已修改: {dataPath} = {newValue}");
                    }
                }
            }
        }

        // 根据路径修改数据
        private bool ModifyDataByPath(string path, object newValue)
        {
            try
            {
                // 从路径解析各级索引
                string[] parts = path.Split('.');
                object currentData = GetMainLoadData();
                object parentData = null;
                string lastPart = null;
                int? lastIndex = null;
                
                // 跳过第一个部分("Mainload")，找到最后一级的父对象
                for (int i = 1; i < parts.Length - 1; i++)
                {
                    if (currentData == null)
                        return false;
                    
                    parentData = currentData;
                    lastPart = parts[i];
                    
                    if (int.TryParse(parts[i], out int index))
                    {
                        // 作为数组访问
                        if (currentData is IList<object> list)
                        {
                            if (index >= 0 && index < list.Count)
                            {
                                currentData = list[index];
                                lastIndex = index;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // 作为属性或字段访问
                        Type type = currentData.GetType();
                        PropertyInfo property = type.GetProperty(parts[i]);
                        if (property != null)
                        {
                            currentData = property.GetValue(currentData, null);
                            lastIndex = null;
                        }
                        else
                        {
                            FieldInfo field = type.GetField(parts[i]);
                            if (field != null)
                            {
                                currentData = field.GetValue(currentData);
                                lastIndex = null;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                
                // 处理最后一级
                string lastPartOfPath = parts[parts.Length - 1];
                
                if (parentData == null)
                {
                    // 如果是顶层属性或字段
                    Type mainLoadType = GetMainLoadData()?.GetType();
                    if (mainLoadType != null)
                    {
                        PropertyInfo property = mainLoadType.GetProperty(lastPartOfPath);
                        if (property != null && property.CanWrite)
                        {
                            property.SetValue(GetMainLoadData(), newValue, null);
                            return true;
                        }
                        else
                        {
                            FieldInfo field = mainLoadType.GetField(lastPartOfPath);
                            if (field != null)
                            {
                                field.SetValue(GetMainLoadData(), newValue);
                                return true;
                            }
                        }
                    }
                }
                else if (int.TryParse(lastPartOfPath, out int index))
                {
                    // 如果最后一级是数组索引
                    if (currentData is IList<object> list)
                    {
                        if (index >= 0 && index < list.Count)
                        {
                            list[index] = newValue;
                            return true;
                        }
                    }
                }
                else
                {
                    // 如果最后一级是属性或字段
                    Type type = currentData.GetType();
                    PropertyInfo property = type.GetProperty(lastPartOfPath);
                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(currentData, newValue, null);
                        return true;
                    }
                    else
                    {
                        FieldInfo field = type.GetField(lastPartOfPath);
                        if (field != null)
                        {
                            field.SetValue(currentData, newValue);
                            return true;
                        }
                    }
                }
                
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError("修改数据失败: " + e.Message);
                return false;
            }
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
