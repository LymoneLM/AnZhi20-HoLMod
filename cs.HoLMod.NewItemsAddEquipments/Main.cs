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

            // 加载道具-石斧的资源包
            string ItemNameKey1 = "ShiFu";
            var resources1 = new ResourceData(MODNAME, ItemNameKey1, modPath);
            resources1.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources1);

            // 加载道具-石镰的资源包
            string ItemNameKey2 = "ShiLian";
            var resources2 = new ResourceData(MODNAME, ItemNameKey2, modPath);
            resources2.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources2);

            // 加载道具-青铜戈的资源包
            string ItemNameKey3 = "QingTongGe";
            var resources3 = new ResourceData(MODNAME, ItemNameKey3, modPath);
            resources3.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources3);

            // 加载道具-青铜剑的资源包
            string ItemNameKey4 = "QingTongJian";
            var resources4 = new ResourceData(MODNAME, ItemNameKey4, modPath);
            resources4.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources4);

            // 加载道具-青铜矛的资源包
            string ItemNameKey5 = "QingTongMao";
            var resources5 = new ResourceData(MODNAME, ItemNameKey5, modPath);
            resources5.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources5);

            // 加载道具-青铜钺的资源包
            string ItemNameKey6 = "QingTongYue";
            var resources6 = new ResourceData(MODNAME, ItemNameKey6, modPath);
            resources6.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources6);

            // 加载道具-铁刀的资源包
            string ItemNameKey7 = "TieDao";
            var resources7 = new ResourceData(MODNAME, ItemNameKey7, modPath);
            resources7.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources7);

            // 加载道具-铁枪的资源包
            string ItemNameKey8 = "TieQiang";
            var resources8 = new ResourceData(MODNAME, ItemNameKey8, modPath);
            resources8.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources8);

            // 加载道具-铁剑的资源包
            string ItemNameKey9 = "TieJian";
            var resources9 = new ResourceData(MODNAME, ItemNameKey9, modPath);
            resources9.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources9);

            // 加载道具-铁戟的资源包
            string ItemNameKey10 = "TieJi";
            var resources10 = new ResourceData(MODNAME, ItemNameKey10, modPath);
            resources10.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources10);

            // 加载道具-铁斧的资源包
            string ItemNameKey11 = "TieFu";
            var resources11 = new ResourceData(MODNAME, ItemNameKey11, modPath);
            resources11.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources11);

            // 加载道具-铁钺的资源包
            string ItemNameKey12 = "TieYue";
            var resources12 = new ResourceData(MODNAME, ItemNameKey12, modPath);
            resources12.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources12);

            // 加载道具-铁铉的资源包
            string ItemNameKey13 = "TieXuan";
            var resources13 = new ResourceData(MODNAME, ItemNameKey13, modPath);
            resources13.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources13);

            // 加载道具-铁叉的资源包
            string ItemNameKey14 = "TieCha";
            var resources14 = new ResourceData(MODNAME, ItemNameKey14, modPath);
            resources14.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources14);

            // 加载道具-铁鞭的资源包
            string ItemNameKey15 = "TieBian";
            var resources15 = new ResourceData(MODNAME, ItemNameKey15, modPath);
            resources15.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources15);

            // 加载道具-铁锏的资源包
            string ItemNameKey16 = "TieJian1";
            var resources16 = new ResourceData(MODNAME, ItemNameKey16, modPath);
            resources16.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources16);

            // 加载道具-铁锤的资源包
            string ItemNameKey17 = "TieChui";
            var resources17 = new ResourceData(MODNAME, ItemNameKey17, modPath);
            resources17.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources17);

            // 加载道具-铁抓的资源包
            string ItemNameKey18 = "TieZhua";
            var resources18 = new ResourceData(MODNAME, ItemNameKey18, modPath);
            resources18.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources18);

            // 加载道具-铁镋的资源包
            string ItemNameKey19 = "TieTang";
            var resources19 = new ResourceData(MODNAME, ItemNameKey19, modPath);
            resources19.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources19);

            // 加载道具-铁棍的资源包
            string ItemNameKey20 = "TieGun";
            var resources20 = new ResourceData(MODNAME, ItemNameKey20, modPath);
            resources20.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources20);

            // 加载道具-铁棒的资源包
            string ItemNameKey21 = "TieBang";
            var resources21 = new ResourceData(MODNAME, ItemNameKey21, modPath);
            resources21.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources21);

            // 加载道具-铁拐的资源包
            string ItemNameKey22 = "TieGuai";
            var resources22 = new ResourceData(MODNAME, ItemNameKey22, modPath);
            resources22.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources22);

            // 加载道具-铁流星锤的资源包
            string ItemNameKey23 = "TieLiuXingChui";
            var resources23 = new ResourceData(MODNAME, ItemNameKey23, modPath);
            resources23.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources23);

            // 加载道具-精铁刀的资源包
            string ItemNameKey24 = "JingTieDao";
            var resources24 = new ResourceData(MODNAME, ItemNameKey24, modPath);
            resources24.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources24);

            // 加载道具-精铁枪的资源包
            string ItemNameKey25 = "JingTieQiang";
            var resources25 = new ResourceData(MODNAME, ItemNameKey25, modPath);
            resources25.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources25);

            // 加载道具-精铁剑的资源包
            string ItemNameKey26 = "JingTieJian";
            var resources26 = new ResourceData(MODNAME, ItemNameKey26, modPath);
            resources26.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources26);

            // 加载道具-精铁戟的资源包
            string ItemNameKey27 = "JingTieJi";
            var resources27 = new ResourceData(MODNAME, ItemNameKey27, modPath);
            resources27.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources27);

            // 加载道具-精铁斧的资源包
            string ItemNameKey28 = "JingTieFu";
            var resources28 = new ResourceData(MODNAME, ItemNameKey28, modPath);
            resources28.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources28);

            // 加载道具-精铁钺的资源包
            string ItemNameKey29 = "JingTieYue";
            var resources29 = new ResourceData(MODNAME, ItemNameKey29, modPath);
            resources29.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources29);

            // 加载道具-精铁铉的资源包
            string ItemNameKey30 = "JingTieXuan";
            var resources30 = new ResourceData(MODNAME, ItemNameKey30, modPath);
            resources30.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources30);

            // 加载道具-精铁叉的资源包
            string ItemNameKey31 = "JingTieCha";
            var resources31 = new ResourceData(MODNAME, ItemNameKey31, modPath);
            resources31.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources31);

            // 加载道具-精铁鞭的资源包
            string ItemNameKey32 = "JingTieBian";
            var resources32 = new ResourceData(MODNAME, ItemNameKey32, modPath);
            resources32.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources32);

            // 加载道具-精铁锏的资源包
            string ItemNameKey33 = "JingTieJian1";
            var resources33 = new ResourceData(MODNAME, ItemNameKey33, modPath);
            resources33.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources33);

            // 加载道具-精铁锤的资源包
            string ItemNameKey34 = "JingTieChui";
            var resources34 = new ResourceData(MODNAME, ItemNameKey34, modPath);
            resources34.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources34);

            // 加载道具-精铁抓的资源包
            string ItemNameKey35 = "JingTieZhua";
            var resources35 = new ResourceData(MODNAME, ItemNameKey35, modPath);
            resources35.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources35);

            // 加载道具-精铁棍的资源包
            string ItemNameKey36 = "JingTieGun";
            var resources36 = new ResourceData(MODNAME, ItemNameKey36, modPath);
            resources36.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources36);

            // 加载道具-精铁棒的资源包
            string ItemNameKey37 = "JingTieBang";
            var resources37 = new ResourceData(MODNAME, ItemNameKey37, modPath);
            resources37.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources37);

            // 加载道具-精铁拐的资源包
            string ItemNameKey38 = "JingTieGuai";
            var resources38 = new ResourceData(MODNAME, ItemNameKey38, modPath);
            resources38.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources38);

            // 加载道具-精铁镋的资源包
            string ItemNameKey39 = "JingTieTang";
            var resources39 = new ResourceData(MODNAME, ItemNameKey39, modPath);
            resources39.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources39);

            // 加载道具-精铁流星锤的资源包
            string ItemNameKey40 = "JingTieLiuXingChui";
            var resources40 = new ResourceData(MODNAME, ItemNameKey40, modPath);
            resources40.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources40);

            // 加载道具-精钢刀的资源包
            string ItemNameKey41 = "JingGangDao";
            var resources41 = new ResourceData(MODNAME, ItemNameKey41, modPath);
            resources41.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources41);

            // 加载道具-精钢枪的资源包
            string ItemNameKey42 = "JingGangQiang";
            var resources42 = new ResourceData(MODNAME, ItemNameKey42, modPath);
            resources42.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources42);

            // 加载道具-精钢剑的资源包
            string ItemNameKey43 = "JingGangJian";
            var resources43 = new ResourceData(MODNAME, ItemNameKey43, modPath);
            resources43.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources43);

            // 加载道具-精钢戟的资源包
            string ItemNameKey44 = "JingGangJi";
            var resources44 = new ResourceData(MODNAME, ItemNameKey44, modPath);
            resources44.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources44);

            // 加载道具-精钢斧的资源包
            string ItemNameKey45 = "JingGangFu";
            var resources45 = new ResourceData(MODNAME, ItemNameKey45, modPath);
            resources45.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources45);

            // 加载道具-精钢钺的资源包
            string ItemNameKey46 = "JingGangYue";
            var resources46 = new ResourceData(MODNAME, ItemNameKey46, modPath);
            resources46.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources46);

            // 加载道具-精钢铉的资源包
            string ItemNameKey47 = "JingGangXuan";
            var resources47 = new ResourceData(MODNAME, ItemNameKey47, modPath);
            resources47.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources47);

            // 加载道具-精钢叉的资源包
            string ItemNameKey48 = "JingGangCha";
            var resources48 = new ResourceData(MODNAME, ItemNameKey48, modPath);
            resources48.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources48);

            // 加载道具-精钢鞭的资源包
            string ItemNameKey49 = "JingGangBian";
            var resources49 = new ResourceData(MODNAME, ItemNameKey49, modPath);
            resources49.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources49);

            // 加载道具-精钢锏的资源包
            string ItemNameKey50 = "JingGangJian1";
            var resources50 = new ResourceData(MODNAME, ItemNameKey50, modPath);
            resources50.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources50);

            // 加载道具-精钢锤的资源包
            string ItemNameKey51 = "JingGangChui";
            var resources51 = new ResourceData(MODNAME, ItemNameKey51, modPath);
            resources51.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources51);

            // 加载道具-精钢抓的资源包
            string ItemNameKey52 = "JingGangZhua";
            var resources52 = new ResourceData(MODNAME, ItemNameKey52, modPath);
            resources52.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources52);

            // 加载道具-精钢棍的资源包
            string ItemNameKey53 = "JingGangGun";
            var resources53 = new ResourceData(MODNAME, ItemNameKey53, modPath);
            resources53.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources53);

            // 加载道具-精钢棒的资源包
            string ItemNameKey54 = "JingGangBang";
            var resources54 = new ResourceData(MODNAME, ItemNameKey54, modPath);
            resources54.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources54);

            // 加载道具-精钢拐的资源包
            string ItemNameKey55 = "JingGangGuai";
            var resources55 = new ResourceData(MODNAME, ItemNameKey55, modPath);
            resources55.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources55);

            // 加载道具-精钢镋的资源包
            string ItemNameKey56 = "JingGangTang";
            var resources56 = new ResourceData(MODNAME, ItemNameKey56, modPath);
            resources56.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources56);

            // 加载道具-精钢流星锤的资源包
            string ItemNameKey57 = "JingGangLiuXingChui";
            var resources57 = new ResourceData(MODNAME, ItemNameKey57, modPath);
            resources57.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources57);

            // 加载道具-54式手枪的资源包
            string ItemNameKey58 = "54ShouQiang";
            var resources58 = new ResourceData(MODNAME, ItemNameKey58, modPath);
            resources58.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources58);

            // 加载道具-64式手枪的资源包
            string ItemNameKey59 = "64ShouQiang";
            var resources59 = new ResourceData(MODNAME, ItemNameKey59, modPath);
            resources59.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources59);

            // 加载道具-92式手枪的资源包
            string ItemNameKey60 = "92ShouQiang";
            var resources60 = new ResourceData(MODNAME, ItemNameKey60, modPath);
            resources60.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources60);

            // 加载道具-AK-47步枪的资源包
            string ItemNameKey61 = "AK47BuQiang";
            var resources61 = new ResourceData(MODNAME, ItemNameKey61, modPath);
            resources61.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources61);

            // 加载道具-M4步枪的资源包
            string ItemNameKey62 = "M4BuQiang";
            var resources62 = new ResourceData(MODNAME, ItemNameKey62, modPath);
            resources62.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources62);

            // 加载道具-M16步枪的资源包
            string ItemNameKey63 = "M16BuQiang";
            var resources63 = new ResourceData(MODNAME, ItemNameKey63, modPath);
            resources63.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources63);

            // 加载道具-56式步枪的资源包
            string ItemNameKey64 = "56BuQiang";
            var resources64 = new ResourceData(MODNAME, ItemNameKey64, modPath);
            resources64.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources64);

            // 加载道具-95式步枪的资源包
            string ItemNameKey65 = "95BuQiang";
            var resources65 = new ResourceData(MODNAME, ItemNameKey65, modPath);
            resources65.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources65);

            // 加载道具-81式步枪的资源包
            string ItemNameKey66 = "81BuQiang";
            var resources66 = new ResourceData(MODNAME, ItemNameKey66, modPath);
            resources66.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources66);

            // 加载道具-97式步枪的资源包
            string ItemNameKey67 = "97BuQiang";
            var resources67 = new ResourceData(MODNAME, ItemNameKey67, modPath);
            resources67.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources67);

            // 加载道具-M24狙击步枪的资源包
            string ItemNameKey68 = "M24JuJiBuQiang";
            var resources68 = new ResourceData(MODNAME, ItemNameKey68, modPath);
            resources68.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources68);

            // 加载道具-M95狙击步枪的资源包
            string ItemNameKey69 = "M95JuJiBuQiang";
            var resources69 = new ResourceData(MODNAME, ItemNameKey69, modPath);
            resources69.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources69);

            // 加载道具-M99狙击步枪的资源包
            string ItemNameKey70 = "M99JuJiBuQiang";
            var resources70 = new ResourceData(MODNAME, ItemNameKey70, modPath);
            resources70.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources70);

            // 加载道具-QBU88式狙击步枪的资源包
            string ItemNameKey71 = "QBU88JuJiBuQiang";
            var resources71 = new ResourceData(MODNAME, ItemNameKey71, modPath);
            resources71.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources71);

            // 加载道具-QBU10式狙击步枪的资源包
            string ItemNameKey72 = "QBU10JuJiBuQiang";
            var resources72 = new ResourceData(MODNAME, ItemNameKey72, modPath);
            resources72.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources72);

            // 加载道具-CS/LR3式狙击步枪的资源包
            string ItemNameKey73 = "CS/LR3JuJiBuQiang";
            var resources73 = new ResourceData(MODNAME, ItemNameKey73, modPath);
            resources73.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources73);

            // 加载道具-CS/LR4式狙击步枪的资源包
            string ItemNameKey74 = "CS/LR4JuJiBuQiang";
            var resources74 = new ResourceData(MODNAME, ItemNameKey74, modPath);
            resources74.LoadAssetBundle(fileName);
            ResourceRegistry.AddResource(resources74);

            // 加载语言文件
            Localization.LoadFromPath(modPath);
        }

        /// <summary>
        /// 加载相关道具，游戏中
        /// </summary>
        public void LoadRelatedProps()
        {
            using var propReg = PropRegistry.CreateInstance();

            // 石器（很久很久以前）
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "ShiFu",// 石斧
                Price = 1000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,1}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.ShiFu",
                PrefabPath = "Assets/Resources/allprop/ShiFu"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "ShiLian",// 石镰
                Price = 1000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,1}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.ShiLian",
                PrefabPath = "Assets/Resources/allprop/ShiLian"
            });

            // 青铜器（商周—春秋战国）
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "QingTongGe",// 青铜戈
                Price = 2000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,2}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.QingTongGe",
                PrefabPath = "Assets/Resources/allprop/QingTongGe"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "QingTongJian",// 青铜剑
                Price = 2000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,2}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.QingTongJian",
                PrefabPath = "Assets/Resources/allprop/QingTongJian"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "QingTongMao",// 青铜矛
                Price = 2000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,2}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.QingTongMao",
                PrefabPath = "Assets/Resources/allprop/QingTongMao"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "QingTongYue",// 青铜钺
                Price = 2000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,2}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.QingTongYue",
                PrefabPath = "Assets/Resources/allprop/QingTongYue"
            });

            // 铁器（秦—明）
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieDao",// 铁刀
                Price = 2000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,2}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieDao",
                PrefabPath = "Assets/Resources/allprop/TieDao"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieQiang",// 铁枪
                Price = 2000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,2}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieQiang",
                PrefabPath = "Assets/Resources/allprop/TieQiang"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieJian",// 铁剑
                Price = 2000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,2}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieJian",
                PrefabPath = "Assets/Resources/allprop/TieJian"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieJi",// 铁戟
                Price = 2000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,2}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieJi",
                PrefabPath = "Assets/Resources/allprop/TieJi"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieFu",// 铁斧
                Price = 2000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,2}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieFu",
                PrefabPath = "Assets/Resources/allprop/TieFu"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieYue",// 铁钺
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,3}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieYue",
                PrefabPath = "Assets/Resources/allprop/TieYue"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieXuan",// 铁铉
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,3}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieXuan",
                PrefabPath = "Assets/Resources/allprop/TieXuan"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieCha",// 铁叉
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,3}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieCha",
                PrefabPath = "Assets/Resources/allprop/TieCha"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieBian",// 铁鞭
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,3}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieBian",
                PrefabPath = "Assets/Resources/allprop/TieBian"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieJian1",// 铁锏
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,3}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieJian1",
                PrefabPath = "Assets/Resources/allprop/TieJian1"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieChui",// 铁锤
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,3}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieChui",
                PrefabPath = "Assets/Resources/allprop/TieChui"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieZhua",// 铁抓
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,3}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieZhua",
                PrefabPath = "Assets/Resources/allprop/TieZhua"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieTang",// 铁镋
                Price = 7000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,4}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieTang",
                PrefabPath = "Assets/Resources/allprop/TieTang"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieGun",// 铁棍
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,3}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieGun",
                PrefabPath = "Assets/Resources/allprop/TieGun"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieBang",// 铁棒
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,3}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieBang",
                PrefabPath = "Assets/Resources/allprop/TieBang"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieGuai",// 铁拐
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,3}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieGuai",
                PrefabPath = "Assets/Resources/allprop/TieGuai"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "TieLiuXingChui",// 铁流星锤
                Price = 7000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,4}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.TieLiuXingChui",
                PrefabPath = "Assets/Resources/allprop/TieLiuXingChui"
            });
            
            // 精铁器和精钢器（DIY）
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieDao",// 精铁刀
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,4}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieDao",
                PrefabPath = "Assets/Resources/allprop/JingTieDao"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieQiang",// 精铁枪
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,4}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieQiang",
                PrefabPath = "Assets/Resources/allprop/JingTieQiang"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieJian",// 精铁剑
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,4}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieJian",
                PrefabPath = "Assets/Resources/allprop/JingTieJian"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieJi",// 精铁戟
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,4}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieJi",
                PrefabPath = "Assets/Resources/allprop/JingTieJi"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieFu",// 精铁斧
                Price = 4000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,4}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieFu",
                PrefabPath = "Assets/Resources/allprop/JingTieFu"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieYue",// 精铁钺
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,5}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieYue",
                PrefabPath = "Assets/Resources/allprop/JingTieYue"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieXuan",// 精铁铉
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,5}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieXuan",
                PrefabPath = "Assets/Resources/allprop/JingTieXuan"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieCha",// 精铁叉
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,5}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieCha",
                PrefabPath = "Assets/Resources/allprop/JingTieCha"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieBian",// 精铁鞭
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,5}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieBian",
                PrefabPath = "Assets/Resources/allprop/JingTieBian"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieJian1",// 精铁锏
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,5}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieJian1",
                PrefabPath = "Assets/Resources/allprop/JingTieJian1"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieChui",// 精铁锤
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,5}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieChui",
                PrefabPath = "Assets/Resources/allprop/JingTieChui"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieZhua",// 精铁抓
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,5}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieZhua",
                PrefabPath = "Assets/Resources/allprop/JingTieZhua"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieGun",// 精铁棍
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,5}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieGun",
                PrefabPath = "Assets/Resources/allprop/JingTieGun"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieBang",// 精铁棒
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,5}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieBang",
                PrefabPath = "Assets/Resources/allprop/JingTieBang"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieGuai",// 精铁拐
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,5}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieGuai",
                PrefabPath = "Assets/Resources/allprop/JingTieGuai"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieTang",// 精铁镋
                Price = 16000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,6}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieTang",
                PrefabPath = "Assets/Resources/allprop/JingTieTang"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingTieLiuXingChui",// 精铁流星锤
                Price = 16000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,6}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingTieLiuXingChui",
                PrefabPath = "Assets/Resources/allprop/JingTieLiuXingChui"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangDao",// 精钢刀
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,7}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangDao",
                PrefabPath = "Assets/Resources/allprop/JingGangDao"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangQiang",// 精钢枪
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,7}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangQiang",
                PrefabPath = "Assets/Resources/allprop/JingGangQiang"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangJian",// 精钢剑
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,7}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangJian",
                PrefabPath = "Assets/Resources/allprop/JingGangJian"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangJi",// 精钢戟
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,7}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangJi",
                PrefabPath = "Assets/Resources/allprop/JingGangJi"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangFu",// 精钢斧
                Price = 11000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,7}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangFu",
                PrefabPath = "Assets/Resources/allprop/JingGangFu"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangYue",// 精钢钺
                Price = 16000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,8}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangYue",
                PrefabPath = "Assets/Resources/allprop/JingGangYue"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangXuan",// 精钢铉
                Price = 16000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,8}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangXuan",
                PrefabPath = "Assets/Resources/allprop/JingGangXuan"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangCha",// 精钢叉
                Price = 16000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,8}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangCha",
                PrefabPath = "Assets/Resources/allprop/JingGangCha"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangBian",// 精钢鞭
                Price = 16000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,8}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangBian",
                PrefabPath = "Assets/Resources/allprop/JingGangBian"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangJian1",// 精钢锏
                Price = 16000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,8}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangJian1",
                PrefabPath = "Assets/Resources/allprop/JingGangJian1"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangChui",// 精钢锤
                Price = 16000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,8}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangChui",
                PrefabPath = "Assets/Resources/allprop/JingGangChui"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangZhua",// 精钢抓
                Price = 16000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,8}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangZhua",
                PrefabPath = "Assets/Resources/allprop/JingGangZhua"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangGun",// 精钢棍
                Price = 16000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,8}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangGun",
                PrefabPath = "Assets/Resources/allprop/JingGangGun"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangBang",// 精钢棒
                Price = 16000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,8}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangBang",
                PrefabPath = "Assets/Resources/allprop/JingGangBang"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangGuai",// 精钢拐
                Price = 16000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,8}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangGuai",
                PrefabPath = "Assets/Resources/allprop/JingGangGuai"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangTang",// 精钢镋
                Price = 22000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,9}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangTang",
                PrefabPath = "Assets/Resources/allprop/JingGangTang"
            });
            
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "JingGangLiuXingChui",// 精钢流星锤
                Price = 22000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,9}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.JingGangLiuXingChui",
                PrefabPath = "Assets/Resources/allprop/JingGangLiuXingChui"
            });

            // 火器（现代）
            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "54ShouQiang",// 54式手枪
                Price = 190000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,20}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.54ShouQiang",
                PrefabPath = "Assets/Resources/allprop/54ShouQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "64ShouQiang",// 64式手枪
                Price = 276000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,24}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.64ShouQiang",
                PrefabPath = "Assets/Resources/allprop/64ShouQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "92ShouQiang",// 92式手枪
                Price = 435000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,30}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.92ShouQiang",
                PrefabPath = "Assets/Resources/allprop/92ShouQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "AK47BuQiang",// AK-47步枪
                Price = 300000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,25}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.AK47BuQiang",
                PrefabPath = "Assets/Resources/allprop/AK47BuQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "M4BuQiang",// M4步枪
                Price = 300000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,25}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.M4BuQiang",
                PrefabPath = "Assets/Resources/allprop/M4BuQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "M16BuQiang",// M16步枪
                Price = 435000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,30}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.M16BuQiang",
                PrefabPath = "Assets/Resources/allprop/M16BuQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "56BuQiang",// 56式步枪
                Price = 276000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,24}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.56BuQiang",
                PrefabPath = "Assets/Resources/allprop/56BuQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "95BuQiang",// 95式步枪
                Price = 378000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,28}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.95BuQiang",
                PrefabPath = "Assets/Resources/allprop/95BuQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "81BuQiang",// 81式步枪
                Price = 435000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,30}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.81BuQiang",
                PrefabPath = "Assets/Resources/allprop/81BuQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "97BuQiang",// 97式步枪
                Price = 496000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,32}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.97BuQiang",
                PrefabPath = "Assets/Resources/allprop/97BuQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "M24JuJiBuQiang",// M24狙击步枪
                Price = 435000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,30}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.M24JuJiBuQiang",
                PrefabPath = "Assets/Resources/allprop/M24JuJiBuQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "M95JuJiBuQiang",// M95狙击步枪
                Price = 595000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,35}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.M95JuJiBuQiang",
                PrefabPath = "Assets/Resources/allprop/M95JuJiBuQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "M99JuJiBuQiang",// M99狙击步枪
                Price = 780000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,40}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.M99JuJiBuQiang",
                PrefabPath = "Assets/Resources/allprop/M99JuJiBuQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "QBU88JuJiBuQiang",// QBU88式狙击步枪
                Price = 595000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,35}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.QBU88JuJiBuQiang",
                PrefabPath = "Assets/Resources/allprop/QBU88JuJiBuQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "QBU10JuJiBuQiang",// QBU10式狙击步枪
                Price = 780000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,40}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.QBU10JuJiBuQiang",
                PrefabPath = "Assets/Resources/allprop/QBU10JuJiBuQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "CS/LR3JuJiBuQiang",// CS/LR3式狙击步枪
                Price = 595000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,35}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.CSLR3JuJiBuQiang",
                PrefabPath = "Assets/Resources/allprop/CS/LR3JuJiBuQiang"
            });

            propReg.Add(new PropData()
            {
                PropNamespace = MODNAME,
                PropID = "CS/LR4JuJiBuQiang",// CS/LR4式狙击步枪
                Price = 780000,
                Category = (int)PropCategory.Weapon,
                PropEffect = new Dictionary<int, int>()
                {
                    {(int)PropEffectType.Might,40}
                },
                TextNamespace = "Common",
                TextKey = "NewItemsAddEquipments.CSLR4JuJiBuQiang",
                PrefabPath = "Assets/Resources/allprop/CS/LR4JuJiBuQiang"
            });
        }
    }
}
