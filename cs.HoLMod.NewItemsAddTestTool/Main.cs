using System.Collections.Generic;
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
            using var propReg = PropRegistry.CreateInstance(MODNAME);
            propReg.PropList.Add(new PropData()
            {
                ID = "TestThing",
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,100}
                },
                Text = ["测试物品", "TestThing"],
                PrefabPath = "AllProp/5"
            });
        }
    }
}