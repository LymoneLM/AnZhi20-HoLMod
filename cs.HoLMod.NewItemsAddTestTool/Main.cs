using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using YuanAPI;

namespace YuanTest
{
    [BepInDependency(YuanAPIPlugin.MODGUID)]
    [BepInPlugin(MODGUID, MODNAME, VERSION)]
    public class YuanTest : BaseUnityPlugin
    {
        public const string MODNAME = "YuanTest";
        public const string MODGUID = YuanAPIPlugin.MODGUID + "." + MODNAME;
        public const string VERSION = "1.0.0";

        public void Awake()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var modPath = Path.GetDirectoryName(executingAssembly.Location);

            // 加载道具-XXX的资源包
            string ItemNameKey1 = "MOD_0";
            string fileName1 = "modallprop_0";
            var resources1 = new ResourceData(MODNAME, ItemNameKey1, modPath);
            resources1.LoadAssetBundle(fileName1);
            ResourceRegistry.AddResource(resources1);

            // 加载道具-XXX的资源包
            //
            //
            //
            //
            //

            Localization.LoadFromPath(modPath);
            using var propReg = PropRegistry.CreateInstance();
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TestThing1",
                Price = 100,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,100}
                },
                TextNamespace = "Common",
                TextKey = "TestItem.Thing1",
                PrefabPath = "AllProp/5"
            });
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TestThing2",
                Price = 20000,
                Category = (int)PropCategory.JewelryM,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Charisma,20}
                },
                TextNamespace = "Common",
                TextKey = "TestItem.Thing2",
                PrefabPath = "Assets/Resources/allprop/MOD_0"
            });
        }
    }
}
