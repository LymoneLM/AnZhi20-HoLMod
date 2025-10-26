using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace cs.HoLMod.AutoArchives
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Main : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        private Harmony harmony;

        private void Awake()
        {
            Log = Logger;
            Logger.LogInfo("Auto Archives Mod loaded!");

            // 初始化Harmony补丁
            harmony = new Harmony(PluginInfo.GUID);
            harmony.PatchAll();

            Logger.LogInfo("Patched StartGameUI to open New_CunDangUI instead of CunDangUI");
        }
    }

    public static class PluginInfo
    {
        public const string GUID = "com.anzbi20.hol.autoarchives";
        public const string Name = "Auto Archives";
        public const string Version = "1.0.0";
    }

    // 使用Harmony补丁来修改StartGameUI中的StartBT方法
    [HarmonyPatch(typeof(StartGameUI))]
    [HarmonyPatch("StartBT")]
    public static class StartGameUIPatch
    {
        private static bool Prefix(StartGameUI __instance)
        {
            // 检查实例是否为null
            if (__instance == null)
            {
                Main.Log.LogWarning("StartGameUI instance is null");
                return true; // 执行原始方法
            }
             
            // 检查transform是否为null
            if (__instance.transform == null)
            {
                Main.Log.LogWarning("StartGameUI transform is null");
                return true; // 执行原始方法
            }
             
            // 检查父级transform是否为null
            Transform parentTransform = __instance.transform.parent;
            if (parentTransform == null)
            {
                Main.Log.LogWarning("StartGameUI parent transform is null");
                return true; // 执行原始方法
            }
             
            // 检查New_CunDangUI是否存在，如果不存在则创建
            Transform newCunDangUITransform = parentTransform.Find("New_CunDangUI");
            GameObject newCunDangUI = null;
             
            if (newCunDangUITransform == null)
            {
                // 查找CunDangUI作为模板
                Transform cunDangUITemplate = parentTransform.Find("CunDangUI");
                if (cunDangUITemplate != null)
                {
                    // 克隆CunDangUI来创建New_CunDangUI
                    newCunDangUI = UnityEngine.Object.Instantiate(cunDangUITemplate.gameObject, parentTransform);
                    newCunDangUI.name = "New_CunDangUI";
                     
                    // 移除原有的CunDangUI组件，添加我们的New_CunDangUI组件
                    UnityEngine.Object.Destroy(newCunDangUI.GetComponent<CunDangUI>());
                    newCunDangUI.AddComponent<New_CunDangUI>();
                     
                    Main.Log.LogInfo("New_CunDangUI created successfully");
                }
                else
                {
                    // 如果找不到CunDangUI模板，记录警告并执行原始方法
                    Main.Log.LogWarning("CunDangUI template not found, cannot create New_CunDangUI");
                    return true; // 执行原始方法
                }
            }
            else
            {
                newCunDangUI = newCunDangUITransform.gameObject;
            }

            // 检查newCunDangUI是否为null
            if (newCunDangUI == null)
            {
                Main.Log.LogWarning("New_CunDangUI is null");
                return true; // 执行原始方法
            }

            // 显示New_CunDangUI并隐藏当前UI
            newCunDangUI.SetActive(true);
            __instance.gameObject.SetActive(false);

            return false; // 不执行原始方法
        }
    }

    // 如果需要，还可以添加其他补丁来确保存档系统的兼容性
    [HarmonyPatch(typeof(SaveData))]
    [HarmonyPatch("ReadGameData")]
    public static class SaveDataPatch
    {
        private static void Prefix()
        {
            // 这里可以添加读取数据前的准备工作
        }

        private static void Postfix()
        {
            // 这里可以添加读取数据后的处理工作
        }
    }
}
