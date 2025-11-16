using BepInEx;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using YuanAPI;
using YuanAPI.UnityWindows;
using HarmonyLib;

namespace cs.HoLMod.TestTool
{
    // 插件信息类
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "cs.HoLMod.TestTool.AnZhi20";
        public const string PLUGIN_NAME = "HoLMod.TestTool";
        public const string PLUGIN_VERSION = "1.0.0";
    }

    [BepInDependency(YuanAPIPlugin.MODGUID)]
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class TestTool : BaseUnityPlugin
    {        
        private ResourceData resources;
        private string ItemNameKey;
        public void Awake()
        {
            从文件加载相关资源();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F7))
            {
                游戏中打开面板();
            }
        }

        public void Start()
        {
        }

        public void 从文件加载相关资源()
        {
            try
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                if (executingAssembly != null)
                {
                    // 获取DLL所在目录
                    var modPath = Path.GetDirectoryName(executingAssembly.Location);
                    if (!string.IsNullOrEmpty(modPath))
                    {
                        // ab包名称和资源名称
                        string bundleName = "membercheat";
                        this.ItemNameKey = "membercheat";
                        
                        // 统一文件夹路径（假设ab包放在与DLL相同的目录）
                        string assetBundlePath = Path.Combine(modPath, bundleName);
                        
                        Logger.LogInfo($"正在从路径 {assetBundlePath} 加载ab包");
                        
                        if (File.Exists(assetBundlePath))
                        {
                            // 存储路径以便后续加载
                            this.resources = new ResourceData(PluginInfo.PLUGIN_NAME, this.ItemNameKey, modPath);
                            Logger.LogInfo($"成功设置资源路径: {assetBundlePath}");
                            
                            // 加载ab包
                            AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
                            if (assetBundle != null)
                            {
                                // 从ab包中加载资源
                                GameObject prefab = assetBundle.LoadAsset<GameObject>(this.ItemNameKey);
                                if (prefab != null)
                                {
                                    Logger.LogInfo($"成功从ab包中加载资源: {this.ItemNameKey}");
                                    // 资源已加载，将在游戏中打开面板时实例化
                                }
                                else
                                {
                                    Logger.LogWarning($"无法从ab包中找到名为 {this.ItemNameKey} 的资源");
                                    assetBundle.Unload(false);
                                }
                            }
                            else
                            {
                                Logger.LogError($"无法加载AssetBundle文件: {assetBundlePath}");
                            }
                        }
                        else
                        {
                            Logger.LogError($"找不到ab包文件: {assetBundlePath}");
                        }
                    }
                    else
                    {
                        Logger.LogError("无法获取DLL所在目录路径");
                    }
                }
                else
                {
                    Logger.LogError("无法获取当前执行的程序集");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"加载资源包时出错: {ex.Message}\n堆栈跟踪: {ex.StackTrace}");
            }
        }

        public void 游戏中打开面板()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.ItemNameKey))
                {
                    var executingAssembly = Assembly.GetExecutingAssembly();
                    if (executingAssembly != null)
                    {
                        var modPath = Path.GetDirectoryName(executingAssembly.Location);
                        if (!string.IsNullOrEmpty(modPath))
                        {
                            string assetBundlePath = Path.Combine(modPath, this.ItemNameKey);
                            
                            if (File.Exists(assetBundlePath))
                            {
                                // 加载ab包
                                AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
                                if (assetBundle != null)
                                {
                                    // 从ab包中加载资源
                                    GameObject prefab = assetBundle.LoadAsset<GameObject>(this.ItemNameKey);
                                    if (prefab != null)
                                    {
                                        Logger.LogInfo($"正在实例化面板: {this.ItemNameKey}");
                                        
                                        try
                                        {
                                            // 实例化面板对象
                                            GameObject instantiatedObject = UnityEngine.Object.Instantiate(prefab);
                                            if (instantiatedObject != null)
                                            {
                                                Logger.LogInfo($"成功实例化面板，实例ID: {instantiatedObject.GetInstanceID()}");
                                                // 确保面板在场景中持续存在
                                                UnityEngine.Object.DontDestroyOnLoad(instantiatedObject);
                                            }
                                            else
                                            {
                                                Logger.LogError("实例化面板失败，返回null对象");
                                            }
                                        }
                                        catch (Exception instantiateEx)
                                        {
                                            Logger.LogError($"实例化面板时出错: {instantiateEx.Message}\n堆栈跟踪: {instantiateEx.StackTrace}");
                                        }
                                        
                                        // 注意：通常不建议在每次使用后立即卸载，除非确定不再使用
                                        // 但由于每次打开面板都需要重新加载，所以这里可以卸载
                                        assetBundle.Unload(false);
                                    }
                                    else
                                    {
                                        Logger.LogWarning($"无法从ab包中找到名为 {this.ItemNameKey} 的资源");
                                        assetBundle.Unload(false);
                                    }
                                }
                                else
                                {
                                    Logger.LogError($"无法加载AssetBundle文件: {assetBundlePath}");
                                }
                            }
                            else
                            {
                                Logger.LogError($"找不到ab包文件: {assetBundlePath}");
                            }
                        }
                        else
                        {
                            Logger.LogError("无法获取DLL所在目录路径");
                        }
                    }
                    else
                    {
                        Logger.LogError("无法获取当前执行的程序集");
                    }
                }
                else
                {
                    Logger.LogError("ItemNameKey为空，无法加载资源");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"加载或实例化面板时出错: {ex.Message}\n堆栈跟踪: {ex.StackTrace}");
            }
        }
    }
}
