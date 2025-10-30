using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using YuanAPI;

namespace cs.HoLMod.NewItemsAddEquipments
{
    [BepInDependency(YuanAPIPlugin.MODGUID)]
    [BepInPlugin(MODGUID, MODNAME, VERSION)]
    public class NewItemsAddTestTool : BaseUnityPlugin
    {
        // MOD主要信息
        public const string MODNAME = "NewItemsAddEquipments";
        public const string MODGUID = "cs.HoLMod.NewItemsAddEquipments.AnZhi20";
        public const string VERSION = "1.2.0";

        // 道具列表
        public List<string> WeaponItemIds = new List<string>();
        public List<string> HorseItemIds = new List<string>();
        public List<string> JewelryItemIds = new List<string>();
        public List<string> SpellItemIds = new List<string>();

        public void Awake()
        {
            // 加载相关资源，从文件
            LoadRelatedResources();

            // 加载相关道具，游戏中
            LoadRelatedProps();
        }

        public void Start()
        {
            将武器添加到铁匠铺售卖();
            将坐骑添加到骡马市售卖();
            将珠宝添加到珠宝行售卖();
            将符咒添加到道士的背包售卖();
        }

        public void 将武器添加到铁匠铺售卖()
        {
            string[] array = Mainload.AllBuilddata[60][3].Split(new char[]{'|'});
            string 铁匠铺售卖的东西 = array[2];
            foreach (var PropID in WeaponItemIds)
            {
                int index = YuanAPI.PropRegistry.GetIndex(MODNAME,PropID);
                铁匠铺售卖的东西 += $"@{index}~1";
            }
            Mainload.AllBuilddata[60][3] = array[0] + "|" + array[1] + "|" + 铁匠铺售卖的东西 + "|" + array[3];
        }

        public void 将坐骑添加到骡马市售卖()
        {
            //
        }

        public void 将珠宝添加到珠宝行售卖()
        {
            string[] array = Mainload.AllBuilddata[59][3].Split(new char[]{'|'});
            string 珠宝行售卖的东西 = array[2];
            foreach (var PropID in JewelryItemIds)
            {
                int index = YuanAPI.PropRegistry.GetIndex(MODNAME,PropID);
                珠宝行售卖的东西 += $"@{index}~1";
            }
            Mainload.AllBuilddata[59][3] = array[0] + "|" + array[1] + "|" + 珠宝行售卖的东西 + "|" + array[3];
        }

        public void 将符咒添加到道士的背包售卖()
        {
            //
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
            var props_weapon = new List<PropConfig>()
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
            foreach (var config in props_weapon)
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
                WeaponItemIds.Add(config.PropID);
            };
            /*
            // 定义所有马匹道具配置数据
            var props_horse = new List<PropConfig>()
            {
                //new PropConfig {},
            };

            // 使用循环添加所有马匹道具
            foreach (var config in props_horse)
            {
                
            };
            HorseItemIds.Add(config.PropID);
            */
            // 定义所有珠宝道具配置数据
            var props_jewelry = new List<PropConfig>()
            {
                new PropConfig { PropID = "MuBanZhi", Description = "木扳指(男)", Price = 5000, Charisma = 1 ,Luck = 0,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "ShiBanZhi", Description = "石扳指(男)", Price = 10000, Charisma = 2 ,Luck = 0,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "TongBanZhi", Description = "铜扳指(男)", Price = 20000, Charisma = 3 ,Luck = 0,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "TieBanZhi", Description = "铁扳指(男)", Price = 30000, Charisma = 4 ,Luck = 0,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YinBanZhi", Description = "银扳指(男)", Price = 60000, Charisma = 5 ,Luck = 0,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "JinBanZhi", Description = "金扳指(男)", Price = 100000, Charisma = 6 ,Luck = 0,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_InferiorHuaqingSeed", Description = "下等花青种玉扳指(男)", Price = 200000, Charisma = 8 ,Luck = 1,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_ModerateHuaqingSeed", Description = "中等花青种玉扳指(男)", Price = 300000, Charisma = 10 ,Luck = 1,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_SuperiorHuaqingSeed", Description = "上等花青种玉扳指(男)", Price = 400000, Charisma = 12 ,Luck = 1,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_TopgradeHuaqingSeed", Description = "特等花青种玉扳指(男)", Price = 500000, Charisma = 14 ,Luck = 1,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_InferiorDouSeed", Description = "下等豆种玉扳指(男)", Price = 1000000, Charisma = 16 ,Luck = 2,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_ModerateDouSeed", Description = "中等豆种玉扳指(男)", Price = 2000000, Charisma = 18 ,Luck = 2,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_SuperiorDouSeed", Description = "上等豆种玉扳指(男)", Price = 3000000, Charisma = 20 ,Luck = 2,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_TopgradeDouSeed", Description = "特等豆种玉扳指(男)", Price = 4000000, Charisma = 22 ,Luck = 2,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_InferiorNuoSeed", Description = "下等糯种玉扳指(男)", Price = 8000000, Charisma = 25 ,Luck = 3,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_ModerateNuoSeed", Description = "中等糯种玉扳指(男)", Price = 10000000, Charisma = 28 ,Luck = 3,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_SuperiorNuoSeed", Description = "上等糯种玉扳指(男)", Price = 12000000, Charisma = 31 ,Luck = 3,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_TopgradeNuoSeed", Description = "特等糯种玉扳指(男)", Price = 14000000, Charisma = 34 ,Luck = 3,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_InferiorBingSeed", Description = "下等冰种玉扳指(男)", Price = 30000000, Charisma = 37 ,Luck = 4,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_ModerateBingSeed", Description = "中等冰种玉扳指(男)", Price = 40000000, Charisma = 40 ,Luck = 5,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_SuperiorBingSeed", Description = "上等冰种玉扳指(男)", Price = 50000000, Charisma = 43 ,Luck = 6,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_TopgradeBingSeed", Description = "特等冰种玉扳指(男)", Price = 60000000, Charisma = 46 ,Luck = 7,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_InferiorBoliSeed", Description = "下等玻璃种玉扳指(男)", Price = 100000000, Charisma = 50 ,Luck = 10,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_ModerateBoliSeed", Description = "中等玻璃种玉扳指(男)", Price = 200000000, Charisma = 54 ,Luck = 15,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_SuperiorBoliSeed", Description = "上等玻璃种玉扳指(男)", Price = 400000000, Charisma = 58 ,Luck = 20,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "YuBanZhi_TopgradeBoliSeed", Description = "特等玻璃种玉扳指(男)", Price = 800000000, Charisma = 62 ,Luck = 25,Category = PropCategory.JewelryM},
                new PropConfig { PropID = "ZhizunBanZhi", Description = "至尊扳指(男)", Price = 1600000000, Charisma = 80 ,Luck = 40,Category = PropCategory.JewelryM},
                //new PropConfig { PropID = "YuPei", Description = "玉佩(女)", Price = 16000, Charisma = 8 ,Category = PropCategory.JewelryF}
            };

            // 使用循环添加所有珠宝道具
            foreach (var config in props_jewelry)
            {
                propReg.Add(new PropData()
                {
                    PropNamespace = MODNAME,
                    PropID = config.PropID, // {config.Description}
                    Price = config.Price,
                    Category = (int)config.Category,
                    PropEffect = new Dictionary<int, int>()
                    {
                        {(int)PropEffectType.Charisma, config.Charisma},
                        {(int)PropEffectType.Luck, config.Luck}
                    },
                    TextNamespace = "AnZhi20MODEquipments",
                    TextKey = $"NewItemsAddEquipments.{config.PropID}",
                    PrefabPath = $"Assets/Resources/allprop/newitemsaddequipments/jewelry/{config.PropID}"
                });
                JewelryItemIds.Add(config.PropID);
            };

            // 定义所有符咒道具配置数据
            var props_spell = new List<PropConfig>()
            {
                new PropConfig { PropID = "Level 1 talisman", Description = "1级符咒", Price = 100000, Charisma = 1 ,Luck = 0, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 2 talisman", Description = "2级符咒", Price = 200000, Charisma = 2 ,Luck = 0, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 3 talisman", Description = "3级符咒", Price = 400000, Charisma = 3 ,Luck = 0, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 4 talisman", Description = "4级符咒", Price = 700000, Charisma = 4 ,Luck = 0, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 5 talisman", Description = "5级符咒", Price = 1100000, Charisma = 5 ,Luck = 0, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 6 talisman", Description = "6级符咒", Price = 1600000, Charisma = 6 ,Luck = 0, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 7 talisman", Description = "7级符咒", Price = 2200000, Charisma = 7 ,Luck = 0, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 8 talisman", Description = "8级符咒", Price = 2900000, Charisma = 8 ,Luck = 0, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 9 talisman", Description = "9级符咒", Price = 3700000, Charisma = 9 ,Luck = 0, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 10 talisman", Description = "10级符咒", Price = 4600000, Charisma = 10 ,Luck = 0, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 11 talisman", Description = "11级符咒", Price = 6000000, Charisma = 12 ,Luck = 1, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 12 talisman", Description = "12级符咒", Price = 7000000, Charisma = 14 ,Luck = 2, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 13 talisman", Description = "13级符咒", Price = 8000000, Charisma = 16 ,Luck = 3, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 14 talisman", Description = "14级符咒", Price = 9000000, Charisma = 18 ,Luck = 4, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 15 talisman", Description = "15级符咒", Price = 10000000, Charisma = 20 ,Luck = 5, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 16 talisman", Description = "16级符咒", Price = 11000000, Charisma = 22 ,Luck = 6, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 17 talisman", Description = "17级符咒", Price = 12000000, Charisma = 24 ,Luck = 7, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 18 talisman", Description = "18级符咒", Price = 13000000, Charisma = 26 ,Luck = 8, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 19 talisman", Description = "19级符咒", Price = 14000000, Charisma = 28 ,Luck = 9, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 20 talisman", Description = "20级符咒", Price = 15000000, Charisma = 30 ,Luck = 10, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 21 talisman", Description = "21级符咒", Price = 30000000, Charisma = 35 ,Luck = 15, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 22 talisman", Description = "22级符咒", Price = 60000000, Charisma = 40 ,Luck = 20, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 23 talisman", Description = "23级符咒", Price = 120000000, Charisma = 45 ,Luck = 25, Category = PropCategory.Spell},
                new PropConfig { PropID = "Level 24 talisman", Description = "24级符咒", Price = 240000000, Charisma = 50 ,Luck = 30, Category = PropCategory.Spell},
            };

            // 使用循环添加所有符咒道具
            foreach (var config in props_spell)
            {
                propReg.Add(new PropData()
                {
                    PropNamespace = MODNAME,
                    PropID = config.PropID, // {config.Description}
                    Price = config.Price,
                    Category = (int)config.Category,
                    PropEffect = new Dictionary<int, int>()
                    {
                        {(int)PropEffectType.Charisma, config.Charisma},
                        {(int)PropEffectType.Luck, config.Luck}
                    },
                    TextNamespace = "AnZhi20MODEquipments",
                    TextKey = $"NewItemsAddEquipments.{config.PropID}",
                    PrefabPath = $"Assets/Resources/allprop/newitemsaddequipments/spell/{config.PropID}"
                });
                SpellItemIds.Add(config.PropID);
            }
        }
    }
}
