using System.Collections.Generic;
using System.IO;
using BepInEx;
using UnityEngine;
using YuanAPI;

namespace YuanTest
{
    [BepInDependency(YuanAPIPlugin.MODGUID)]
    [BepInPlugin(MODGUID, MODNAME, VERSION)]
    public class YuanTest : BaseUnityPlugin
    {
        public const string MODNAME = "AddTest";
        public const string MODGUID = YuanAPIPlugin.MODGUID + "." + MODNAME;
        public const string VERSION = "1.0.0";

        public void Awake()
        {
            // 加载ab包
            var AssetBundlePath = Path.Combine(Paths.PluginPath, "modallprop_0");
            var MyAssetBundle = AssetBundle.LoadFromFile(AssetBundlePath);
            if (MyAssetBundle != null)
            {
                Logger.LogInfo($"成功加载ab包：{AssetBundlePath}");
                return;
            }
            else
            {
                Logger.LogError($"加载ab包失败：{AssetBundlePath}");
                return;
            }

            // 加载道具注册器
            using var propReg = PropRegistry.CreateInstance();
            PropData TestItem1 = new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TestThing",
                Price = 100,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,100}
                },
                TextNamespace = "common",
                TextKey = "Prop.TestThing",
                PrefabPath = "AllProp/5"
                //PrefabPath = AssetBundlePath
            });

            PropRegistry.RegisterProps(propReg);
        }
    }
}