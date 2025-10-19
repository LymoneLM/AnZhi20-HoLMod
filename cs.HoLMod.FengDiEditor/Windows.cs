using System;
using System.Collections.Generic;
using UnityEngine;

namespace cs.HoLMod.FengDiEditor
{
    internal class EditorWindow
    {
        private Rect windowRect = new Rect(20, 20, 500, 400);
        private bool isWindowVisible = false;
        private string selectedCategory = "CategoryAdd"; // 默认选中"增加"分类
        private LanguageManager languageManager;
        
        // 定义委托来处理子分类按钮点击事件
        public delegate void SubCategoryButtonClickHandler(string category, string subCategory);
        public event SubCategoryButtonClickHandler OnSubCategoryButtonClicked;

        public EditorWindow()
        {
            languageManager = LanguageManager.Instance;
        }

        public void ToggleWindowVisibility()
        {
            isWindowVisible = !isWindowVisible;
        }

        public bool IsWindowVisible()
        {    
            return isWindowVisible;
        }

        public void OnGUI()
        {    
            if (isWindowVisible)
            {    
                windowRect = GUI.Window(0, windowRect, DrawWindow, languageManager.GetTranslation("WindowTitle"), GUI.skin.window);
            }
        }

        private void DrawWindow(int windowID)
        {    
            // 分类按钮行
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(languageManager.GetTranslation("CategoryAdd"), GUILayout.Width(100)))    
            {    
                selectedCategory = "CategoryAdd";
            }
            if (GUILayout.Button(languageManager.GetTranslation("CategoryDelete"), GUILayout.Width(100)))    
            {    
                selectedCategory = "CategoryDelete";
            }
            if (GUILayout.Button(languageManager.GetTranslation("CategoryModify"), GUILayout.Width(100)))    
            {    
                selectedCategory = "CategoryModify";
            }
            GUILayout.EndHorizontal();

            // 小分类按钮网格
            GUILayout.Space(20);
            GUILayout.Label($"{languageManager.GetTranslation(selectedCategory)}" + " - 选择子分类:", GUILayout.Width(200));
            GUILayout.Space(10);

            GUILayout.BeginVertical();
            // 创建4x2的按钮网格
            for (int i = 0; i < 2; i++)    
            {    
                GUILayout.BeginHorizontal();
                for (int j = 0; j < 4; j++)    
                {    
                    int index = i * 4 + j;
                    string subCategoryKey = GetSubCategoryKey(index);
                    if (GUILayout.Button(languageManager.GetTranslation(subCategoryKey), GUILayout.Width(100), GUILayout.Height(40)))    
                    {    
                        OnSubCategoryButtonClick(selectedCategory, subCategoryKey);
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            GUILayout.EndVertical();

            // 允许窗口拖动
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private string GetSubCategoryKey(int index)
        {    
            switch (index)
            {    
                case 0: return "SubCategoryField";
                case 1: return "SubCategoryVillage";
                case 2: return "SubCategoryTown";
                case 3: return "SubCategoryCamp";
                case 4: return "SubCategoryLake";
                case 5: return "SubCategoryFarm";
                case 6: return "SubCategoryForest";
                case 7: return "SubCategoryMountain";
                default: return "SubCategoryField";
            }
        }

        private void OnSubCategoryButtonClick(string category, string subCategory)    
        {    
            // 触发事件，由Main.cs处理具体逻辑
            OnSubCategoryButtonClicked?.Invoke(category, subCategory);
        }
    }
}
