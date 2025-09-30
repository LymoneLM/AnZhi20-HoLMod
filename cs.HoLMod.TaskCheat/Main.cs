using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace cs.HoLMod.TaskCheat
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class TaskCheat : BaseUnityPlugin
    {
        // GUI窗口位置和大小
        private Rect windowRect = new Rect(20, 20, 400, 300);
        // 是否显示窗口
        private bool showWindow = false;
        // 窗口ID
        private int windowID = 1000;
        // 插件日志
        internal static ManualLogSource Log;
        // UI缩放因子
        private float scaleFactor = 1f;

        private void Awake()
        {
            Log = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void Update()
        {
            // 按F4键开关窗口
            if (Input.GetKeyDown(KeyCode.F4))
            {
                showWindow = !showWindow;
            }
        }

        private void OnGUI()
        {
            if (showWindow)
            {
                // 创建无边框窗口
                windowRect = GUI.Window(windowID, windowRect, DrawWindow, "", GUI.skin.window);
            }
        }

        private void DrawWindow(int windowID)
        {
            // 任务编辑器标题
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label("任务编辑器", titleStyle);
            GUILayout.Space(15);

            // 分类标题
            GUIStyle categoryStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };

            // 一键清除分类
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("一键清除", categoryStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            if (GUILayout.Button("清除所有任务", GUILayout.Width(150), GUILayout.Height(30)))
            {
                ClearAllTasks();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(15);

            // 任务添加分类
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("任务添加", categoryStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GUILayout.Label("功能暂未实现", GUI.skin.label);
            GUILayout.EndHorizontal();
            GUILayout.Space(15);

            // 重复任务分类
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("重复任务", categoryStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GUILayout.Label("功能暂未实现", GUI.skin.label);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            // 使用说明
            GUILayout.Space(20f * scaleFactor);
            GUILayout.BeginVertical();
            
            // 使用说明标题
            GUILayout.Label("使用说明:", UnityEngine.GUI.skin.box);
            
            // 使用说明
            GUILayout.Label("1. 请在修改前先保存游戏，以便回档");
            GUILayout.Label("2. 按F4键显示/隐藏窗口");
            GUILayout.Label("3. 切换模式选择：一键清除/任务添加/重复任务");
            GUILayout.Label("");
            
            // MOD作者及版本号说明
            GUILayout.Label("Mod作者：AnZhi20");
            GUILayout.Label(string.Format("Mod版本：{0}", PluginInfo.PLUGIN_VERSION));
            GUILayout.EndVertical();
            
            // 允许拖动窗口
            GUI.DragWindow(new Rect(0, 0, windowRect.width, windowRect.height));
        }

        private void ClearAllTasks()
        {
            try
            {
                TaskClearer.AllTaskClear();
                Logger.LogInfo("所有任务已清除!");
                // 显示成功提示
                ShowNotification("任务已清除成功!");
            }
            catch (System.Exception ex)
            {
                Logger.LogError("清除任务时出错: " + ex.Message);
                ShowNotification("清除失败: " + ex.Message);
            }
        }

        // 显示提示信息到游戏界面
        private void ShowNotification(string message)
        {
            try
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
            catch (Exception ex)
            {
                Logger.LogError(string.Format("显示提示信息时出错: {0}", ex.Message));
            }
        }
    }

    // 插件信息类
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "cs.HoLMod.TaskCheat.AnZhi20";
        public const string PLUGIN_NAME = "HoLMod.TaskCheat";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}
