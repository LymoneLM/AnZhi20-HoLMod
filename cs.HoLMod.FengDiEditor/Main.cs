using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace cs.HoLMod.FengDiEditor
{
    // 插件信息类
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "cs.HoLMod.FengDiEditor.AnZhi20";
        public const string PLUGIN_NAME = "HoLMod.FengDiEditor";
        public const string PLUGIN_VERSION = "1.0.0";
        public const string PLUGIN_CONFIG = "cs.HoLMod.FengDiEditor.AnZhi20.cfg";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class FengDiEditor : BaseUnityPlugin
    {
        private EditorWindow editorWindow;
        private Harmony harmony;

        private void Awake()
        {
            // 初始化语言管理器
            LanguageManager.Instance.SetLanguage("zh-CN"); // 默认使用中文

            // 初始化编辑器窗口
            editorWindow = new EditorWindow();
            editorWindow.OnSubCategoryButtonClicked += HandleSubCategoryButtonClick;

            // 初始化Harmony补丁
            harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Logger.LogInfo("HoLMod.FengDiEditor 已加载，按F5显示/隐藏窗口");
        }

        private void OnGUI()
        {    
            // 处理F5键按下事件
            if (Input.GetKeyDown(KeyCode.F5))
            {    
                editorWindow.ToggleWindowVisibility();
                Logger.LogInfo($"窗口状态: {(editorWindow.IsWindowVisible() ? "显示" : "隐藏")}");
            }

            // 绘制窗口
            editorWindow.OnGUI();
        }

        private void HandleSubCategoryButtonClick(string category, string subCategory)
        {    
            // 这里实现具体的逻辑处理
            // 暂时只记录日志
            string categoryName = LanguageManager.Instance.GetTranslation(category);
            string subCategoryName = LanguageManager.Instance.GetTranslation(subCategory);
            
            Logger.LogInfo($"执行操作: {categoryName} - {subCategoryName}");
            
            // 根据不同的分类和子分类执行不同的逻辑
            switch (category)
            {    
                case "CategoryAdd":
                    HandleAddOperation(subCategory);
                    break;
                case "CategoryDelete":
                    HandleDeleteOperation(subCategory);
                    break;
                case "CategoryModify":
                    HandleModifyOperation(subCategory);
                    break;
            }
        }

        private void HandleAddOperation(string subCategory)
        {    
            // 增加操作的具体逻辑
            Logger.LogInfo($"执行增加操作: {subCategory}");
            // TODO: 实现增加操作的具体代码
        }

        private void HandleDeleteOperation(string subCategory)
        {    
            // 删除操作的具体逻辑
            Logger.LogInfo($"执行删除操作: {subCategory}");
            // TODO: 实现删除操作的具体代码
        }

        private void HandleModifyOperation(string subCategory)
        {    
            // 修改操作的具体逻辑
            Logger.LogInfo($"执行修改操作: {subCategory}");
            // TODO: 实现修改操作的具体代码
        }

        private void OnDestroy()
        {    
            // 清理Harmony补丁
            if (harmony != null)
            {    
                harmony.UnpatchSelf();
            }
        }
    }
}
