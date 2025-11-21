using System;
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
using HoLMod;

namespace cs.HoLMod.MemberCheat
{
    // 插件信息类
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "cs.HoLMod.MemberCheat.AnZhi20";
        public const string PLUGIN_NAME = "HoLMod.MemberCheat";
        public const string PLUGIN_VERSION = "3.0.0";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MemberCheat : BaseUnityPlugin
    {
        public static AssetBundle memberCheatBundle;
        public static GameObject memberCheatObject;
        
        private void Awake()
        {
            LoadAssetBundle();
            打开修改器();
        }
        
        private void LoadAssetBundle()
        {
            try
            {
                // 加载ab包
                string bundlePath = Path.Combine(Paths.PluginPath, "membercheat");
                if (File.Exists(bundlePath))
                {
                    memberCheatBundle = AssetBundle.LoadFromFile(bundlePath);
                    if (memberCheatBundle != null)
                    {
                        Logger.LogInfo("成功加载membercheat包");
                        // 从ab包中加载membercheat对象
                        memberCheatObject = memberCheatBundle.LoadAsset<GameObject>("membercheat");
                        if (memberCheatObject != null)
                        {
                            Logger.LogInfo("成功从ab包中加载membercheat对象");
                            // 实例化对象
                            memberCheatObject = GameObject.Instantiate(memberCheatObject);
                            GameObject.DontDestroyOnLoad(memberCheatObject);
                            
                            // 挂载Panel_Main.cs到membercheat对象
                            if (!memberCheatObject.GetComponent<membercheat>())
                            {
                                memberCheatObject.AddComponent<membercheat>();
                                Logger.LogInfo("成功挂载Panel_Main.cs到membercheat对象");
                            }
                            
                            // 挂载PanelA.cs到membercheat子对象PanelA
                            Transform panelATransform = memberCheatObject.transform.Find("PanelA");
                            if (panelATransform != null)
                            {
                                if (!panelATransform.gameObject.GetComponent<MemberCheatPanelA>())
                                {
                                    panelATransform.gameObject.AddComponent<MemberCheatPanelA>();
                                    Logger.LogInfo("成功挂载PanelA.cs到membercheat子对象PanelA");
                                }
                            }
                            else
                            {
                                Logger.LogError("未找到membercheat对象的子对象PanelA");
                            }
                        }
                        else
                        {
                            Logger.LogError("无法从ab包中加载membercheat对象");
                        }
                    }
                    else
                    {
                        Logger.LogError("无法加载membercheat包");
                    }
                }
                else
                {
                    Logger.LogError("membercheat包文件不存在: " + bundlePath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("加载ab包时发生错误: " + ex.Message);
            }
        }

        private void 打开修改器()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                base.transform.Find("PanelA").gameObject.SetActive(value: true);
            }
        }

        private void OnDestroy()
        {
            // 卸载ab包
            if (memberCheatBundle != null)
            {
                memberCheatBundle.Unload(false);
            }
        }
    }
}
