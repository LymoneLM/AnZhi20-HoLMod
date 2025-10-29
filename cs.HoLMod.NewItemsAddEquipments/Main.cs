using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using YuanAPI;

namespace cs.HoLMod.NewItemsAddEquipments
{
    [BepInDependency(YuanAPIPlugin.MODGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(MODGUID, MODNAME, VERSION)]
    public class NewItemsAddTestTool : BaseUnityPlugin
    {
        public const string MODNAME = "NewItemsAddEquipments";
        public const string MODGUID = "cs.HoLMod.NewItemsAddEquipments.AnZhi20";
        public const string VERSION = "1.0.0";

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
            public int Price { get; set; }
            public int Might { get; set; }
        }

        public void LoadRelatedProps()
        {
            using var propReg = PropRegistry.CreateInstance();

            // 定义所有武器道具配置数据
            var props = new List<PropConfig>()
            {
                // 石器（很久很久以前）
                new PropConfig { PropID = "ShiFu", Description = "石斧", Price = 1000, Might = 1 },
                new PropConfig { PropID = "ShiLian", Description = "石镰", Price = 1000, Might = 1 },
                
                // 青铜器（商周—春秋战国）
                new PropConfig { PropID = "QingTongGe", Description = "青铜戈", Price = 2000, Might = 2 },
                new PropConfig { PropID = "QingTongJian", Description = "青铜剑", Price = 2000, Might = 2 },
                new PropConfig { PropID = "QingTongMao", Description = "青铜矛", Price = 2000, Might = 2 },
                new PropConfig { PropID = "QingTongYue", Description = "青铜钺", Price = 2000, Might = 2 },
                
                // 铁器（秦—明）
                new PropConfig { PropID = "TieDao", Description = "铁刀", Price = 2000, Might = 2 },
                new PropConfig { PropID = "TieQiang", Description = "铁枪", Price = 2000, Might = 2 },
                new PropConfig { PropID = "TieJian", Description = "铁剑", Price = 2000, Might = 2 },
                new PropConfig { PropID = "TieJi", Description = "铁戟", Price = 2000, Might = 2 },
                new PropConfig { PropID = "TieFu", Description = "铁斧", Price = 2000, Might = 2 },
                new PropConfig { PropID = "TieYue", Description = "铁钺", Price = 4000, Might = 3 },
                new PropConfig { PropID = "TieGou", Description = "铁钩", Price = 4000, Might = 3 },
                new PropConfig { PropID = "TieCha", Description = "铁叉", Price = 4000, Might = 3 },
                new PropConfig { PropID = "TieBian", Description = "铁鞭", Price = 4000, Might = 3 },
                new PropConfig { PropID = "TieJian1", Description = "铁锏", Price = 4000, Might = 3 },
                new PropConfig { PropID = "TieChui", Description = "铁锤", Price = 4000, Might = 3 },
                new PropConfig { PropID = "TieZhua", Description = "铁抓", Price = 4000, Might = 3 },
                new PropConfig { PropID = "TieGun", Description = "铁棍", Price = 4000, Might = 3 },
                new PropConfig { PropID = "TieBang", Description = "铁棒", Price = 4000, Might = 3 },
                new PropConfig { PropID = "TieGuai", Description = "铁拐", Price = 4000, Might = 3 },
                new PropConfig { PropID = "TieTang", Description = "铁镋", Price = 7000, Might = 4 },
                new PropConfig { PropID = "TieLiuXingChui", Description = "铁流星锤", Price = 7000, Might = 4 },
                
                // 精铁器（DIY）
                new PropConfig { PropID = "JingTieDao", Description = "精铁刀", Price = 4000, Might = 4 },
                new PropConfig { PropID = "JingTieQiang", Description = "精铁枪", Price = 4000, Might = 4 },
                new PropConfig { PropID = "JingTieJian", Description = "精铁剑", Price = 4000, Might = 4 },
                new PropConfig { PropID = "JingTieJi", Description = "精铁戟", Price = 4000, Might = 4 },
                new PropConfig { PropID = "JingTieFu", Description = "精铁斧", Price = 4000, Might = 4 },
                new PropConfig { PropID = "JingTieYue", Description = "精铁钺", Price = 11000, Might = 5 },
                new PropConfig { PropID = "JingTieGou", Description = "精铁钩", Price = 11000, Might = 5 },
                new PropConfig { PropID = "JingTieCha", Description = "精铁叉", Price = 11000, Might = 5 },
                new PropConfig { PropID = "JingTieBian", Description = "精铁鞭", Price = 11000, Might = 5 },
                new PropConfig { PropID = "JingTieJian1", Description = "精铁锏", Price = 11000, Might = 5 },
                new PropConfig { PropID = "JingTieChui", Description = "精铁锤", Price = 11000, Might = 5 },
                new PropConfig { PropID = "JingTieZhua", Description = "精铁抓", Price = 11000, Might = 5 },
                new PropConfig { PropID = "JingTieGun", Description = "精铁棍", Price = 11000, Might = 5 },
                new PropConfig { PropID = "JingTieBang", Description = "精铁棒", Price = 11000, Might = 5 },
                new PropConfig { PropID = "JingTieGuai", Description = "精铁拐", Price = 11000, Might = 5 },
                new PropConfig { PropID = "JingTieTang", Description = "精铁镋", Price = 16000, Might = 6 },
                new PropConfig { PropID = "JingTieLiuXingChui", Description = "精铁流星锤", Price = 16000, Might = 6 },
                
                // 精钢器（DIY）
                new PropConfig { PropID = "JingGangDao", Description = "精钢刀", Price = 11000, Might = 7 },
                new PropConfig { PropID = "JingGangQiang", Description = "精钢枪", Price = 11000, Might = 7 },
                new PropConfig { PropID = "JingGangJian", Description = "精钢剑", Price = 11000, Might = 7 },
                new PropConfig { PropID = "JingGangJi", Description = "精钢戟", Price = 11000, Might = 7 },
                new PropConfig { PropID = "JingGangFu", Description = "精钢斧", Price = 11000, Might = 7 },
                new PropConfig { PropID = "JingGangYue", Description = "精钢钺", Price = 16000, Might = 8 },
                new PropConfig { PropID = "JingGangGou", Description = "精钢钩", Price = 16000, Might = 8 },
                new PropConfig { PropID = "JingGangCha", Description = "精钢叉", Price = 16000, Might = 8 },
                new PropConfig { PropID = "JingGangBian", Description = "精钢鞭", Price = 16000, Might = 8 },
                new PropConfig { PropID = "JingGangJian1", Description = "精钢锏", Price = 16000, Might = 8 },
                new PropConfig { PropID = "JingGangChui", Description = "精钢锤", Price = 16000, Might = 8 },
                new PropConfig { PropID = "JingGangZhua", Description = "精钢抓", Price = 16000, Might = 8 },
                new PropConfig { PropID = "JingGangGun", Description = "精钢棍", Price = 16000, Might = 8 },
                new PropConfig { PropID = "JingGangBang", Description = "精钢棒", Price = 16000, Might = 8 },
                new PropConfig { PropID = "JingGangGuai", Description = "精钢拐", Price = 16000, Might = 8 },
                new PropConfig { PropID = "JingGangTang", Description = "精钢镋", Price = 22000, Might = 9 },
                new PropConfig { PropID = "JingGangLiuXingChui", Description = "精钢流星锤", Price = 22000, Might = 9 },
                
                // 火器（现代）
                new PropConfig { PropID = "54ShouQiang", Description = "54式手枪", Price = 190000, Might = 20 },
                new PropConfig { PropID = "64ShouQiang", Description = "64式手枪", Price = 276000, Might = 24 },
                new PropConfig { PropID = "92ShouQiang", Description = "92式手枪", Price = 435000, Might = 30 },
                new PropConfig { PropID = "AK47BuQiang", Description = "AK-47步枪", Price = 300000, Might = 25 },
                new PropConfig { PropID = "M4BuQiang", Description = "M4步枪", Price = 300000, Might = 25 },
                new PropConfig { PropID = "M16BuQiang", Description = "M16步枪", Price = 435000, Might = 30 },
                new PropConfig { PropID = "56BuQiang", Description = "56式步枪", Price = 276000, Might = 24 },
                new PropConfig { PropID = "95BuQiang", Description = "95式步枪", Price = 378000, Might = 28 },
                new PropConfig { PropID = "81BuQiang", Description = "81式步枪", Price = 435000, Might = 30 },
                new PropConfig { PropID = "97BuQiang", Description = "97式步枪", Price = 496000, Might = 32 },
                new PropConfig { PropID = "M24JuJiBuQiang", Description = "M24狙击步枪", Price = 435000, Might = 30 },
                new PropConfig { PropID = "M95JuJiBuQiang", Description = "M95狙击步枪", Price = 595000, Might = 35 },
                new PropConfig { PropID = "M99JuJiBuQiang", Description = "M99狙击步枪", Price = 780000, Might = 40 },
                new PropConfig { PropID = "QBU88JuJiBuQiang", Description = "QBU88式狙击步枪", Price = 595000, Might = 35 },
                new PropConfig { PropID = "QBU10JuJiBuQiang", Description = "QBU10式狙击步枪", Price = 780000, Might = 40 },
                new PropConfig { PropID = "CSLR3JuJiBuQiang", Description = "CS/LR3式狙击步枪", Price = 595000, Might = 35 },
                new PropConfig { PropID = "CSLR4JuJiBuQiang", Description = "CS/LR4式狙击步枪", Price = 780000, Might = 40 }
            };

            // 使用循环添加所有武器道具
            foreach (var config in props)
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
                    TextNamespace = "AnZhi20MODWeapon",
                    TextKey = $"NewItemsAddEquipments.{config.PropID}",
                    PrefabPath = $"Assets/Resources/allprop/newitemsaddequipments/{config.PropID}"
                });
            }
        }
    }
}
