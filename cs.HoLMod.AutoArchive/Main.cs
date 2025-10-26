using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace cs.HoLMod.AutoArchive
{
    [BepInPlugin("cs.HoLMod.AutoArchive.AnZhi20", "HoLMod.AutoArchive", "1.0.0")]
    public class Main : BaseUnityPlugin
    {
        private void Awake()
        {
            // 应用Harmony补丁
            Harmony.CreateAndPatchAll(typeof(Main));
        }

        [HarmonyPatch(typeof(StartGameUI), "StartBT")]
        [HarmonyPrefix]
        public static bool FixStartGameUIStartBT(StartGameUI __instance)
        {
            // 隐藏StartGameUI
            __instance.gameObject.SetActive(false);
            
            // 尝试激活CunDangUI_New
            Transform cundangUINewTransform = __instance.transform.parent.Find("CunDangUI_New");
            if (cundangUINewTransform != null)
            {
                Debug.Log("Found existing CunDangUI_New, activating it.");
                cundangUINewTransform.gameObject.SetActive(true);
            }
            else
            {
                // 如果找不到CunDangUI_New，尝试创建一个实例
                Debug.Log("CunDangUI_New not found, trying to create an instance...");
                
                // 查找原始的CunDangUI作为参考
                Transform originalCunDangUITransform = __instance.transform.parent.Find("CunDangUI");
                if (originalCunDangUITransform != null)
                {
                    Debug.Log("Found original CunDangUI, creating CunDangUI_New based on it.");
                    
                    // 尝试克隆原始CunDangUI并添加我们的脚本
                    GameObject originalUI = originalCunDangUITransform.gameObject;
                    GameObject newUI = UnityEngine.Object.Instantiate(originalUI, originalUI.transform.parent);
                    newUI.name = "CunDangUI_New";
                    
                    // 移除原始组件并添加我们的新组件
                    Component[] originalComponents = newUI.GetComponents<Component>();
                    foreach (Component component in originalComponents)
                    {
                        if (!(component is Transform) && !(component is RectTransform))
                        {
                            UnityEngine.Object.Destroy(component);
                        }
                    }
                    
                    // 添加我们的新组件
                    newUI.AddComponent<CunDangUI_New>();
                    
                    // 激活新创建的UI
                    newUI.SetActive(true);
                    Debug.Log("Successfully created and activated CunDangUI_New.");
                }
                else
                {
                    // 如果找不到原始的CunDangUI，尝试创建一个简单的版本
                    Debug.LogWarning("Original CunDangUI not found, creating a basic version of CunDangUI_New.");
                    
                    // 创建一个新的GameObject
                    GameObject newUI = new GameObject("CunDangUI_New");
                    newUI.transform.SetParent(__instance.transform.parent);
                    
                    // 添加必要的组件
                    RectTransform rectTransform = newUI.AddComponent<RectTransform>();
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    
                    Canvas canvas = newUI.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    
                    CanvasScaler scaler = newUI.AddComponent<CanvasScaler>();
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    
                    GraphicRaycaster raycaster = newUI.AddComponent<GraphicRaycaster>();
                    
                    // 添加我们的自定义组件
                    newUI.AddComponent<CunDangUI_New>();
                    
                    // 激活新创建的UI
                    newUI.SetActive(true);
                    Debug.Log("Successfully created a basic version of CunDangUI_New.");
                }
            }
            
            // 返回false表示不执行原方法
            return false;
        }
    }
}
