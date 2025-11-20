using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using YuanAPI;

namespace cs.HoLMod.NewItems
{
    [BepInDependency(YuanAPIPlugin.MODGUID)]
    [BepInPlugin(MODGUID, MODNAME, VERSION)]
    public class NewItemsAddTestTool : BaseUnityPlugin
    {
        // MOD主要信息
        public const string MODNAME = "NewItems";
        public const string MODGUID = "cs.HoLMod.NewItems.AnZhi20";
        public const string VERSION = "1.0.0";

        // 随机数生成器
        private static readonly Random random = new Random();

        public void Awake()
        {
            // 加载相关资源，从文件
            LoadRelatedResources();

            // 加载相关道具，游戏中
            LoadRelatedProps();
        }

        /// <summary>
        /// 加载相关资源，从文件
        /// </summary>
        public void LoadRelatedResources()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var modPath = Path.GetDirectoryName(executingAssembly.Location);
            string fileName = "newitemsaddequipments";
            string ItemNameKey = "newitemsaddequipments";
            var resources = new ResourceData(MODNAME, ItemNameKey, modPath);
            resources.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources);

            // 加载语言文件
            Localization.LoadFromPath(modPath);
        }

        /// <summary>
        /// 加载相关道具，游戏中
        /// </summary>
        // 道具配置类，用于存储道具的基本信息
        private class PropConfig
        {
            public string PropID { get; set; }
            public string Description { get; set; }
            public PropCategory Category { get; set; }
            public int Price { get; set; }
            public int Might { get; set; }
            public int Charisma { get; set; }
            public int Luck { get; set; }

        }

        // 加载相关道具
        public void LoadRelatedProps()
        {
            using var propReg = PropRegistry.CreateInstance();

            // 定义所有武器道具配置数据
            var props_1 = new List<PropConfig>()
            {
                //new PropConfig { PropID = "ShiFu", Description = "石斧", Price = 1000, Might = 1 }
            };

            // 使用循环添加所有道具
            foreach (var config in props_1)
            {
                propReg.Add(new PropData()
                {
                    PropNamespace = MODNAME,
                    PropID = config.PropID, // {config.Description}
                    Price = config.Price,
                    Category = (int)PropCategory.Weapon,
                    PropEffect = new Dictionary<int, int>()
                    {
                        {(int)PropEffectType.Might, config.Might}
                    },
                    TextNamespace = "AnZhi20MODEquipments",
                    TextKey = $"NewItemsAddEquipments.{config.PropID}",
                    PrefabPath = $"Assets/Resources/allprop/newitemsaddequipments/weapon/{config.PropID}"
                });
            };
        }
    }
}
