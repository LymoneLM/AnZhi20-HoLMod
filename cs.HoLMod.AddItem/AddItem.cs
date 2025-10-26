using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace cs.HoLMod.AddItem
{
    // 插件信息类
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "cs.HoLMod.AddItem.AnZhi20";
        public const string PLUGIN_NAME = "HoLMod.AddItem";
        public const string PLUGIN_VERSION = "2.8.0";
    }
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class AddItem : BaseUnityPlugin
    {
        // 窗口设置
        private static Rect windowRect;
        private static bool showMenu;// 滚动位置
        private static Vector2 scrollPosition;
        private static bool blockGameInput;
        

        
        // 物品列表 - 添加分类信息用于快捷搜索
        private Dictionary<int, (string, string)> itemList = new Dictionary<int, (string, string)>
        {
            // 特殊物品
            { 0, ("香烛", "特殊物品") },
            { 1, ("肥料", "特殊物品") },
            { 2, ("粮食", "特殊物品") },
            { 3, ("蔬菜", "特殊物品") },
            { 4, ("肉食", "特殊物品") },
            
            // 美食
            { 6, ("奶酪", "美食") },
            { 7, ("果脯", "美食") },
            { 8, ("青梅", "美食") },
            { 9, ("樱桃", "美食") },
            { 10, ("柿子", "美食") },
            { 11, ("枇杷", "美食") },
            { 12, ("桑葚", "美食") },
            { 13, ("桔子", "美食") },
            { 14, ("荔枝", "美食") },
            { 15, ("熊掌", "美食") },
            { 66, ("鱼", "美食") },
            { 73, ("团饼茶", "美食") },
            { 284, ("鸡蛋", "美食") },
            
            // 农产
            { 5, ("生茶", "农产") },
            { 67, ("棉花", "农产") },
            { 74, ("盐", "农产") },
            
            // 布料
            { 16, ("麻布", "布料") },
            { 17, ("粗布", "布料") },
            { 18, ("皮革", "布料") },
            { 19, ("素绫", "布料") },
            { 20, ("交织绫", "布料") },
            { 21, ("单罗纱", "布料") },
            { 22, ("四罗纱", "布料") },
            { 23, ("雪缎", "布料") },
            { 24, ("花缎", "布料") },
            { 25, ("云锦", "布料") },
            { 26, ("蜀锦", "布料") },
            { 27, ("筛绢", "布料") },
            { 28, ("天香绢", "布料") },
            
            // 矿产
            { 29, ("黄金", "矿产") },
            { 30, ("白银", "矿产") },
            { 31, ("青铜", "矿产") },
            { 32, ("黑铁", "矿产") },
            { 33, ("白锡", "矿产") },
            { 34, ("赤汞", "矿产") },
            { 35, ("玉石", "矿产") },
            { 36, ("硫磺", "矿产") },
            { 37, ("硝石", "矿产") },
            { 38, ("煤炭", "矿产") },
            { 39, ("石灰", "矿产") },
            
            // 香粉
            { 40, ("石黛", "香粉") },
            { 41, ("青雀头黛", "香粉") },
            { 42, ("铜黛", "香粉") },
            { 43, ("螺子黛", "香粉") },
            { 44, ("铅粉", "香粉") },
            { 45, ("紫粉", "香粉") },
            { 46, ("桃花粉", "香粉") },
            { 47, ("胭脂", "香粉") },
            { 48, ("乌膏", "香粉") },
            { 49, ("泡花露", "香粉") },
            { 50, ("蔷薇露", "香粉") },
            
            // 珠宝（女）
            { 51, ("银笄(女)", "珠宝（女）") },
            { 52, ("钿花(女)", "珠宝（女）") },
            { 53, ("花钗(女)", "珠宝（女）") },
            { 54, ("铒铛(女)", "珠宝（女）") },
            { 55, ("步摇(女)", "珠宝（女）") },
            { 56, ("金簪(女)", "珠宝（女）") },
            { 57, ("华胜(女)", "珠宝（女）") },
            { 58, ("宫绦(女)", "珠宝（女）") },
            { 59, ("梳篦(女)", "珠宝（女）") },
            { 60, ("臂钏(女)", "珠宝（女）") },
            { 61, ("璎珞(女)", "珠宝（女）") },
            
            // 珠宝（男）
            { 62, ("发钗(男)", "珠宝（男）") },
            { 63, ("扳指(男)", "珠宝（男）") },
            { 64, ("玉佩(男)", "珠宝（男）") },
            { 65, ("带钩(男)", "珠宝（男）") },
            
            // 丹药
            { 68, ("长寿丹", "丹药") },
            { 69, ("极乐丹", "丹药") },
            { 70, ("美颜丹", "丹药") },
            { 71, ("极品美颜丹", "丹药") },
            { 72, ("极品长寿丹", "丹药") },
            { 75, ("合欢丹", "丹药") },
            { 168, ("回魂丹", "丹药") },
            { 282, ("堕胎丹", "丹药") },
            { 283, ("避孕丹", "丹药") },
            { 285, ("灵慧丹", "丹药") },
            
            // 书法
            { 76, ("无名字轴", "书法") },
            { 77, ("无名碑帖", "书法") },
            { 78, ("名家字轴", "书法") },
            { 79, ("名家碑帖", "书法") },
            { 80, ("传世字轴", "书法") },
            { 81, ("传世碑帖", "书法") },
            
            // 丹青
            { 82, ("无名山水图", "丹青") },
            { 83, ("无名江山图", "丹青") },
            { 84, ("名家山水图", "丹青") },
            { 85, ("名家江山图", "丹青") },
            { 86, ("传世山水图", "丹青") },
            { 87, ("传世江山图", "丹青") },
            
            // 文玩
            { 88, ("玉圭", "文玩") },
            { 89, ("玉碗", "文玩") },
            { 90, ("玉瓶", "文玩") },
            { 91, ("玉琥", "文玩") },
            { 92, ("玉璧", "文玩") },
            { 93, ("玉璜", "文玩") },
            { 94, ("玉琮", "文玩") },
            { 95, ("玉山", "文玩") },
            { 96, ("玉屏", "文玩") },
            
            // 武器
            { 97, ("竹弓", "武器") },
            { 98, ("木弓", "武器") },
            { 99, ("牛角弓", "武器") },
            { 100, ("木角弓", "武器") },
            { 101, ("木弩", "武器") },
            { 102, ("铜弩", "武器") },
            { 103, ("连弩", "武器") },
            { 104, ("单刀", "武器") },
            { 105, ("朴刀", "武器") },
            { 106, ("鬼头刀", "武器") },
            { 107, ("三尖两刃刀", "武器") },
            { 108, ("三尺剑", "武器") },
            { 109, ("软剑", "武器") },
            { 110, ("七尺剑", "武器") },
            { 111, ("子母剑", "武器") },
            
            // 茶具
            { 112, ("提梁壶", "茶具") },
            { 113, ("油滴盏", "茶具") },
            { 114, ("天目盏", "茶具") },
            { 115, ("青瓷盏", "茶具") },
            { 116, ("红釉盏", "茶具") },
            { 117, ("青瓷壶", "茶具") },
            { 118, ("楠木茶盘", "茶具") },
            { 119, ("檀木茶盘", "茶具") },
            { 120, ("玉壶", "茶具") },
            
            // 香具
            { 121, ("铜薰球", "香具") },
            { 122, ("铜香炉", "香具") },
            { 123, ("楠木香筒", "香具") },
            { 124, ("金薰球", "香具") },
            { 125, ("檀木香盒", "香具") },
            { 126, ("金香盒", "香具") },
            { 127, ("檀木香筒", "香具") },
            { 128, ("金香炉", "香具") },
            
            // 瓷器
            { 129, ("高足碗", "瓷器") },
            { 130, ("葫芦瓶", "瓷器") },
            { 131, ("扁瓶", "瓷器") },
            { 132, ("瓷枕", "瓷器") },
            { 133, ("琮式瓶", "瓷器") },
            { 134, ("梅瓶", "瓷器") },
            { 135, ("凤尾瓶", "瓷器") },
            
            // 美酒
            { 136, ("黄酒", "美酒") },
            { 137, ("红曲酒", "美酒") },
            { 138, ("菊花酒", "美酒") },
            { 139, ("茱萸酒", "美酒") },
            { 140, ("竹叶青", "美酒") },
            { 141, ("荔枝酒", "美酒") },
            { 142, ("桂花酒", "美酒") },
            { 143, ("琥珀酒", "美酒") },
            
            // 乐器
            { 144, ("竹萧", "乐器") },
            { 145, ("木笛", "乐器") },
            { 146, ("芦笙", "乐器") },
            { 147, ("二胡", "乐器") },
            { 148, ("琵琶", "乐器") },
            { 149, ("月琴", "乐器") },
            { 150, ("中瑟", "乐器") },
            { 151, ("伏羲琴", "乐器") },
            
            // 皮毛
            { 152, ("兔皮", "皮毛") },
            { 153, ("狼皮", "皮毛") },
            { 154, ("鹿皮", "皮毛") },
            { 155, ("狐皮", "皮毛") },
            { 156, ("虎皮", "皮毛") },
            
            // 符咒
            { 157, ("一阶符咒", "符咒") },
            { 158, ("二阶符咒", "符咒") },
            { 159, ("三阶符咒", "符咒") },
            { 160, ("四阶符咒", "符咒") },
            { 161, ("五阶符咒", "符咒") },
            { 162, ("六阶符咒", "符咒") },
            
            // 毒药
            { 163, ("番木鳖", "毒药") },
            { 164, ("乌头", "毒药") },
            { 165, ("雷公腾", "毒药") },
            { 166, ("断肠草", "毒药") },
            { 167, ("鹤顶红", "毒药") },
            
            // 书籍 - 文
            { 169, ("《三字经》", "书籍 - 文") },
            { 170, ("《弟子规》", "书籍 - 文") },
            { 171, ("《千字文》", "书籍 - 文") },
            { 172, ("《蒙求》", "书籍 - 文") },
            { 173, ("《广韵》", "书籍 - 文") },
            { 174, ("《广雅》", "书籍 - 文") },
            { 175, ("《九章》", "书籍 - 文") },
            { 176, ("《尔雅》", "书籍 - 文") },
            { 177, ("《临川集》", "书籍 - 文") },
            { 178, ("《花间集》", "书籍 - 文") },
            { 179, ("《诗品》", "书籍 - 文") },
            { 180, ("《文选》", "书籍 - 文") },
            { 181, ("《诗经》", "书籍 - 文") },
            { 182, ("《尚书》", "书籍 - 文") },
            { 183, ("《礼记》", "书籍 - 文") },
            { 184, ("《周易》", "书籍 - 文") },
            { 185, ("《春秋》", "书籍 - 文") },
            { 186, ("《大学》", "书籍 - 文") },
            { 187, ("《中庸》", "书籍 - 文") },
            { 188, ("《论语》", "书籍 - 文") },
            { 189, ("《孟子》", "书籍 - 文") },
            { 190, ("《法言》", "书籍 - 文") },
            { 191, ("《潜夫论》", "书籍 - 文") },
            
            // 书籍 - 武
            { 192, ("《手搏六篇》", "书籍 - 武") },
            { 193, ("《棍棒体势》", "书籍 - 武") },
            { 194, ("《麻杈棍谱》", "书籍 - 武") },
            { 195, ("《手臂录》", "书籍 - 武") },
            { 196, ("《拳术教范》", "书籍 - 武") },
            { 197, ("《苌氏武技书》", "书籍 - 武") },
            { 198, ("《阴符枪谱》", "书籍 - 武") },
            { 199, ("《太极拳经》", "书籍 - 武") },
            { 200, ("《马槊谱》", "书籍 - 武") },
            { 201, ("《三才图会》", "书籍 - 武") },
            { 202, ("《拳经》", "书籍 - 武") },
            { 203, ("《射经》", "书籍 - 武") },
            { 204, ("《剑经》", "书籍 - 武") },
            { 205, ("《角力记》", "书籍 - 武") },
            { 206, ("《耕余剩技》", "书籍 - 武") },
            
            // 书籍 - 商
            { 207, ("《士商类要》", "书籍 - 商") },
            { 208, ("《货殖列传》", "书籍 - 商") },
            { 209, ("《平准书》", "书籍 - 商") },
            { 210, ("《计然书》", "书籍 - 商") },
            { 211, ("《贸易赋》", "书籍 - 商") },
            { 212, ("《食货志》", "书籍 - 商") },
            { 213, ("《轻重篇》", "书籍 - 商") },
            { 214, ("《考工记》", "书籍 - 商") },
            { 215, ("《商君书》", "书籍 - 商") },
            { 216, ("《陶朱公生意经》", "书籍 - 商") },
            { 217, ("《商经》", "书籍 - 商") },
            
            // 书籍 - 艺
            { 218, ("《琴律发微》", "书籍 - 艺") },
            { 219, ("《琴声十六法》", "书籍 - 艺") },
            { 220, ("《唱论》", "书籍 - 艺") },
            { 221, ("《琴操》", "书籍 - 艺") },
            { 222, ("《乐府杂录》", "书籍 - 艺") },
            { 223, ("《乐章集》", "书籍 - 艺") },
            { 224, ("《乐府诗集》", "书籍 - 艺") },
            { 225, ("《乐书要录》", "书籍 - 艺") },
            { 226, ("《碧鸡漫志》", "书籍 - 艺") },
            { 227, ("《琴清英》", "书籍 - 艺") },
            { 228, ("《梅花三弄》", "书籍 - 艺") },
            { 229, ("《猗兰操》", "书籍 - 艺") },
            { 230, ("《广陵散》", "书籍 - 艺") },
            { 231, ("《阳春白雪》", "书籍 - 艺") },
            { 232, ("《高山流水》", "书籍 - 艺") },
            { 233, ("《乐记》", "书籍 - 艺") },
            { 234, ("《乐经》", "书籍 - 艺") },
            { 235, ("《琴赋》", "书籍 - 艺") },
            { 236, ("《溪山琴况》", "书籍 - 艺") },
            { 237, ("《太古遗音》", "书籍 - 艺") },
            
            // 书籍 - 计谋
            { 238, ("《左传》", "书籍 - 计谋") },
            { 239, ("《史记》", "书籍 - 计谋") },
            { 240, ("《间书》", "书籍 - 计谋") },
            { 241, ("《捭阖》", "书籍 - 计谋") },
            { 242, ("《飞箝》", "书籍 - 计谋") },
            { 243, ("《许合》", "书籍 - 计谋") },
            { 244, ("《揣篇》", "书籍 - 计谋") },
            { 245, ("《苏子》", "书籍 - 计谋") },
            { 246, ("《太白阴经》", "书籍 - 计谋") },
            { 247, ("《战国策》", "书籍 - 计谋") },
            { 248, ("《韩非子》", "书籍 - 计谋") },
            { 249, ("《孙子兵法》", "书籍 - 计谋") },
            { 250, ("《鬼谷子》", "书籍 - 计谋") },
            
            // 书籍 - 技能 - 巫
            { 251, ("《本经阴符》", "书籍 - 技能 - 巫") },
            { 252, ("《山海经》", "书籍 - 技能 - 巫") },
            { 253, ("《符咒秘本》", "书籍 - 技能 - 巫") },
            { 254, ("《巫阳古书》", "书籍 - 技能 - 巫") },
            { 255, ("《阴山法笈》", "书籍 - 技能 - 巫") },
            
            // 书籍 - 技能 - 医
            { 256, ("《鲁班书》", "书籍 - 技能 - 医") },
            { 257, ("《千金方》", "书籍 - 技能 - 医") },
            { 258, ("《温热经纬》", "书籍 - 技能 - 医") },
            { 259, ("《伤寒杂病论》", "书籍 - 技能 - 医") },
            { 260, ("《脉经》", "书籍 - 技能 - 医") },
            { 261, ("《金匮要略》", "书籍 - 技能 - 医") },
            { 262, ("《神农本草》", "书籍 - 技能 - 医") },
            { 263, ("《本草纲目》", "书籍 - 技能 - 医") },
            { 264, ("《黄帝内经》", "书籍 - 技能 - 医") },
            
            // 书籍 - 技能 - 相
            { 265, ("《德器歌》", "书籍 - 技能 - 相") },
            { 266, ("《五官杂论》", "书籍 - 技能 - 相") },
            { 267, ("《听声相行》", "书籍 - 技能 - 相") },
            { 268, ("《永乐百问》", "书籍 - 技能 - 相") },
            { 269, ("《柳庄相法》", "书籍 - 技能 - 相") },
            
            // 书籍 - 技能 - 卜
            { 270, ("《紫微斗数》", "书籍 - 技能 - 卜") },
            { 271, ("《梅花易数》", "书籍 - 技能 - 卜") },
            { 272, ("《神峰通考》", "书籍 - 技能 - 卜") },
            { 273, ("《渊海子平》", "书籍 - 技能 - 卜") },
            { 274, ("《三命通会》", "书籍 - 技能 - 卜") },
            
            // 书籍 - 技能 - 媚
            { 275, ("《素女经》", "书籍 - 技能 - 媚") },
            { 276, ("《洞玄子》", "书籍 - 技能 - 媚") },
            
            // 书籍 - 技能 - 工
            { 277, ("《长物志》", "书籍 - 技能 - 工") },
            { 278, ("《梓人遗制》", "书籍 - 技能 - 工") },
            { 279, ("《齐民要术》", "书籍 - 技能 - 工") },
            { 280, ("《天工开物》", "书籍 - 技能 - 工") },
            { 281, ("《墨经》", "书籍 - 技能 - 工") }
        };
        
        // 货币类型（0：铜钱，1：元宝）
        private int selectedCurrencyType;
        private int currencyValue = 100000; // 默认10万
        
        // 判断当前是否为中文语言环境

        
        // 双语话本列表 - 结构：ID, [中文名称, 英文名称, 中文描述, 英文描述]
        private Dictionary<int, string[]> bookList = new Dictionary<int, string[]>
        {
            { 0, new[] { "《天仙配》", "The Heavenly Match", "董永孝感动天，七仙女慕其德行，下凡结缘。王母怒，迫其归天。二人情深，誓不分离，终得团圆。", "Dong Yong's filial piety moves heaven; the Seventh Fairy, admiring his virtue, descends to marry. The Queen Mother, enraged, forces her return. Deeply in love, they vow never to part and ultimately reunite." } },
            { 1, new[] { "《女驸马》", "The Female Royal Son-in-Law", "冯素贞为救父，女扮男装，考中状元，招为驸马。公主疑之，终揭真相。帝感其孝，赦其罪，赐婚状元。", "Feng Suzhen disguises as a man to save her father, tops the imperial exam, and is made royal son-in-law. The princess suspects, uncovers the truth. The emperor, moved by her filial piety, pardons her and arranges her marriage to the top scholar." } },
            { 2, new[] { "《牡丹亭》", "The Peony Pavilion", "杜丽娘梦遇柳梦梅，醒后相思成疾，逝去。柳梦梅得画，感其情，丽娘复活，二人终成眷属。", "Du Liniang dreams of Liu Mengmei, falls lovesick, and dies. Liu finds her portrait, moved by her love, Liniang revives, and they unite." } },
            { 3, new[] { "《紫钗记》", "The Purple Hairpin", "李益与霍小玉相爱，以紫钗为信。李益赴考，霍母逼小玉改嫁。小玉病重，李益归，见钗如见人，终得团圆。", "Li Yi and Huo Xiaoyu fall in love, pledge with a purple hairpin. Li leaves for exams, Huo's mother forces her to remarry. Xiaoyu falls ill, Li returns, sees the pin as seeing her, and they reunite." } },
            { 4, new[] { "《孔雀东南飞》", "The Peacocks Fly Southeast", "刘兰芝与焦仲卿相爱，兰芝被逼改嫁，投河自尽。仲卿闻讯，亦自缢。两家悔之，合葬二人，孔雀飞至其墓", "Liu Lanzhi and Jiao Zhongqing love each other; Lanzhi is forced to remarry and drowns herself. Zhongqing hangs himself upon hearing the news. Both families regret, bury them together, and peacocks fly to their grave." } },
            { 5, new[] { "《焚香记》", "The Incense Burning", "王魁负心，桂英焚香告天。魁受天谴，病重。桂英感其悔，复与和好，魁病愈。", "Wang Kui betrays Guiying, who burns incense to heaven. Kui is cursed, falls ill. Guiying, moved by his remorse, reconciles, and Kui recovers." } },
            { 6, new[] { "《琵琶记》", "The Tale of the Pipa", "赵五娘寻夫蔡伯喈，弹琵琶诉苦。伯喈闻声，终得相认。夫妻团圆，孝感动天。", "Zhao Wuniang searches for her husband Cai Bojie, playing the pipa to express her sorrow. Bojie hears it, they reunite. Their filial piety moves heaven." } },
            { 7, new[] { "《望江亭》", "The Riverside Pavilion", "谭记儿为救夫，假扮渔妇，智斗杨衙内。于望江亭设计，终救夫出狱，夫妻团圆。", "Tan Ji'er disguises as a fisherwoman to save her husband, outwits Officer Yang. At Wangjiang Pavilion, she schemes to free him, and they reunite." } },
            { 8, new[] { "《锁麟囊》", "The Precious Pouch", "薛湘灵出嫁，途中遇雨，避于春秋亭。贫女赵守贞亦至，湘灵赠锁麟囊。后湘灵落难，守贞报恩，二人结为姐妹。", "Xue Xiangling, on her wedding journey, shelters from rain at Chunqiu Pavilion. Poor girl Zhao Shouzhen arrives, Xiangling gifts her a precious pouch. Later, Xiangling falls into hardship, Shouzhen repays the kindness, and they become sisters." } },
            { 9, new[] { "《碧玉簪》", "The Jade Hairpin", "王玉林与李秀英相爱，秀英赠碧玉簪为信。后玉林误会秀英，秀英含冤而死。玉林悔悟，终得昭雪。", "Wang Yulin and Li Xiuying fall in love, Xiuying gifts a jade hairpin as a pledge. Yulin misunderstands, Xiuying dies unjustly. Yulin repents, and her name is cleared." } },
            { 10, new[] { "《罗帕记》", "The Silk Handkerchief", "王科举与妻陈赛金恩爱，赛金遗失罗帕，被姜雄拾得。姜雄诬陷赛金，科举误会。后真相大白，夫妻和好。", "Wang Keju and his wife Chen Saijin are loving; Saijin loses a silk handkerchief, picked up by Jiang Xiong. Jiang frames Saijin, Keju misunderstands. Truth revealed, they reconcile." } },
            { 11, new[] { "《梁祝》", "The Butterfly Lovers", "梁山伯与祝英台同窗，英台女扮男装。山伯不知，情愫暗生。英台归家，山伯访之，知真相，悲恸而亡。英台殉情，二人化蝶。", "Liang Shanbo and Zhu Yingtai study together, Yingtai disguised as a man. Shanbo, unaware, falls in love. Yingtai returns home, Shanbo visits, learns the truth, dies of grief. Yingtai follows in death, they transform into butterflies." } },
            { 12, new[] { "《花木兰》", "Hua Mulan", "木兰代父从军，女扮男装，英勇善战。十二年征战后，归家复女装，众人惊其勇，帝赐厚赏。", "Mulan joins the army in her father's place, disguised as a man, fights bravely. After twelve years, returns home, reveals her identity, astonishes all, and the emperor rewards her richly." } },
            { 13, new[] { "《赵氏孤儿》", "The Orphan of Zhao", "屠岸贾灭赵氏，程婴救赵氏孤儿，隐姓埋名。孤儿长成，程婴告知身世，孤儿报仇雪恨，赵氏复兴。", "Tu'an Gu exterminates the Zhao clan; Cheng Ying saves the orphan, hides his identity. The orphan grows up, Cheng reveals his heritage, the orphan avenges, and the Zhao clan is restored." } },
            { 14, new[] { "《谢小娥传》", "The Tale of Xie Xiao'e", "谢小娥父为盗所害，小娥立志复仇。女扮男装，寻得仇人，智勇双全，终报父仇，众人称颂。", "Xie Xiao'e's father is killed by bandits; she vows revenge. Disguised as a man, she finds the culprits, uses wit and courage, avenges her father, and is praised by all." } },
            { 15, new[] { "《金鞭记》", "The Golden Whip", "呼延赞之子呼延丕显被奸臣庞文陷害，全家满门三百余口被杀，呼延丕显之子呼延守勇、呼延守信与奸臣斗争之事。", "Huyan Pixian, son of Huyan Zan, is framed by the treacherous minister Pang Wen, and his family of over 300 people are killed. Huyan Pixian's sons, Huyan Shouyong and Huyan Shouxin, fight against the treacherous minister." } },
            { 16, new[] { "《文昭关》", "Wu Zixu at Zhaoguan", "伍子胥过昭关，一夜白头。为报父仇，忍辱负重，终得吴王重用，率兵伐楚，报仇雪恨。", "Wu Zixu passes Zhaoguan, turns white overnight. To avenge his father, endures humiliation, gains King Wu's trust, leads troops against Chu, and avenges his father's death." } },
            { 17, new[] { "《李离伏剑》", "Li Li Draws His Sword", "李离执掌晋国刑罚，因误听下属不实之辞而错杀了人，最终拔剑自刎。", "Li Li, in charge of the penal system in Jin, mistakenly executed an innocent man due to false information from his subordinates, and ultimately drew his sword and committed suicide." } },
            { 18, new[] { "《双婿案》", "The Two Sons-in-Law", "举人杨玉春，因家境不幸，前往燕山王府投亲，途中救出遭遇强盗抢劫的公子方天觉，两人义结金兰。", "Yang Yuchun, due to his unfortunate family circumstances, goes to the Prince of Yanshan's residence to seek refuge. On his way, he rescues Fang Tianjue, a young master who was robbed by bandits, and the two become sworn brothers." } } };

         // 界面模式控制（0：货币，1：物品，2：话本，3：地图）
        private int currentMode = 1; // 默认显示物品模式
        
        // 地图子模式控制（0：府邸，1：农庄，2：封地，3：世家）
        private int mapSubMode; // 默认显示府邸模式
        
        // 地图模式相关变量
        private string selectedPrefecture = "";
        private string selectedCounty = "";
        private int selectedJunIndex = -1; // 当前选择的郡索引
        private string mansionCustomName = "";
        private string farmCustomName = "";
        private int farmArea = 16; // 农庄面积，默认16
        private bool onlyAddMansion = true; // 添加府邸方式：true=仅添加府邸，false=添加后进入府邸
        
        // 郡数组，索引对应郡ID
        private string[] JunList = {
            "南郡",     // 0
            "三川郡",   // 1
            "蜀郡",     // 2
            "丹阳郡",   // 3
            "陈留郡",   // 4
            "长沙郡",   // 5
            "会稽郡",   // 6
            "广陵郡",   // 7
            "太原郡",   // 8
            "益州郡",   // 9
            "南海郡",   // 10
            "云南郡"    // 11
        };

        // 二维县数组，第一维是郡索引，第二维是县索引
        private string[][] XianList = {
            // 南郡 (索引0)
            new[] { "临沮", "襄樊", "宜城", "麦城", "华容", "郢亭", "江陵", "夷陵" },
            
            // 三川郡 (索引1)
            new[] { "平阳", "荥阳", "原武", "阳武", "新郑", "宜阳" },
            
            // 蜀郡 (索引2)
            new[] { "邛崃", "郫县", "什邡", "绵竹", "新都", "成都" },
            
            // 丹阳郡 (索引3)
            new[] { "秣陵", "江乘", "江宁", "溧阳", "建邺", "永世" },
            
            // 陈留郡 (索引4)
            new[] { "长垣", "济阳", "成武", "襄邑", "宁陵", "封丘" },
            
            // 长沙郡 (索引5)
            new[] { "零陵", "益阳", "湘县", "袁州", "庐陵", "衡山", "建宁", "桂阳" },
            
            // 会稽郡 (索引6)
            new[] { "曲阿", "松江", "山阴", "余暨" },
            
            // 广陵郡 (索引7)
            new[] { "平安", "射阳", "海陵", "江都" },
            
            // 太原郡 (索引8)
            new[] { "大陵", "晋阳", "九原", "石城", "阳曲", "魏榆", "孟县", "中都" },
            
            // 益州郡 (索引9)
            new[] { "连然", "谷昌", "同劳", "昆泽", "滇池", "俞元", "胜休", "南安" },
            
            // 南海郡 (索引10)
            new[] { "四会", "阳山", "龙川", "揭岭", "罗阳", "善禺" },
            
            // 云南郡 (索引11)
            new[] { "云平", "叶榆", "永宁", "遂久", "姑复", "蜻陵", "弄栋", "邪龙" }
        };
        
        // 界面控制变量
        private int selectedItemId;
        private int count = 1;
        private string searchText = "";
        private string countInput = "1";
        private string statusMessage;

        public AddItem()
        {
            statusMessage = LanguageManager.Instance.GetText("准备就绪");
        }
        private List<int> filteredItemIds = new List<int>();
        // 用于组合搜索的分类过滤器
        private string selectedCategory = "";

        // 当前插件版本
        private const string CURRENT_VERSION = PluginInfo.PLUGIN_VERSION;
        
        // 分辨率缩放因子
        private float scaleFactor = 1.0f;
        
        // 初始化分辨率列表
        static AddItem()
        {
            // 初始化分辨率数据
            if (Mainload.AllFenBData == null)
            {
                Mainload.AllFenBData = new List<List<int>>
                {
                    new List<int> { 1280, 720 },
                    new List<int> { 1920, 1080 },
                    new List<int> { 2560, 1440 },
                    new List<int> { 3840, 2160 }
                };
            }
        }
        
        // 分辨率更新处理方法
        private void UpdateResolutionSettings()
        {
            // 获取当前屏幕分辨率
            var currentWidth = Screen.width;
            var currentHeight = Screen.height;
            
            // 默认窗口大小
            var defaultWidth = 1000f;
            var defaultHeight = 1200f;
            
            // 根据分辨率设置窗口大小和缩放因子
            if (currentWidth == 2560 && currentHeight == 1440 || currentWidth == 3840 && currentHeight == 2160)
            {
                // 高分辨率，调整窗口位置和大小，控件缩放为1.0倍
                scaleFactor = 1.0f;
                windowRect = new Rect(150, 100, defaultWidth + 100f, defaultHeight - 100f);
            }
            else if (currentWidth == 1920 && currentHeight == 1080)
            {
                // 中等分辨率，调整窗口位置和大小，控件缩放为0.8倍
                scaleFactor = 0.8f;
                windowRect = new Rect(100, 200, defaultWidth *1.2f, defaultHeight *1.2f - 500f);
            }
            else if (currentWidth == 1280 && currentHeight == 720)
            {
                // 低分辨率，调整窗口位置和大小，控件缩放为0.5倍
                scaleFactor = 0.5f;
                windowRect = new Rect(100, 200, defaultWidth *1.2f, defaultHeight *1.2f - 800f);
            }
            else
            {
                // 其他分辨率，使用中分辨率设置
                scaleFactor = 0.8f;
                windowRect = new Rect(100, 200, defaultWidth * 1.2f, defaultHeight * 1.2f - 500f);
            }
            
            Logger.LogInfo($"当前分辨率: {currentWidth}x{currentHeight}，缩放因子: {scaleFactor}");
        }

        private void Awake()
        {
            Logger.LogInfo("物品添加器已加载！");
            
            // 版本检查逻辑
            try
            {
                // 获取之前保存的版本
                var savedVersion = Config.Bind("内部配置", "已加载版本", "", "用于跟踪插件版本，请勿手动修改").Value;
                
                // 检查版本是否更新
                if (savedVersion != CURRENT_VERSION)
                {
                    Logger.LogInfo($"检测到插件版本更新至 {CURRENT_VERSION}，正在清除配置...");
                    
                    // 清除配置
                    Config.Clear();
                    
                    // 保存新版本信息
                    Config.Bind("内部配置", "已加载版本", CURRENT_VERSION, "用于跟踪插件版本，请勿手动修改");
                    Config.Save();
                    
                    Logger.LogInfo("配置已成功清除并更新版本信息。");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("版本检查时出错: " + ex.Message);
            }
            
            // 初始化分辨率设置
            UpdateResolutionSettings();
            
            // 加载游戏物品到字典，并根据当前语言环境选择显示中文或英文名称
            LoadGameItemsToDictionary.LoadItems(itemList, LanguageManager.Instance.IsChineseLanguage());
            
            // 初始化筛选后的物品列表
            filteredItemIds = itemList.Keys.ToList();
        }
        
        private void Update()
        {
            // 按F2键切换窗口显示
            if (Input.GetKeyDown(KeyCode.F2))
            {
                // 按下F2时更新分辨率设置
                UpdateResolutionSettings();
                showMenu = !showMenu;
                blockGameInput = showMenu;
                
                // 在显示窗口前，如果地图面板是打开的，则先关闭地图面板
                if (showMenu && Mainload.isMapPanelOpen)
                {
                    Mainload.isMapPanelOpen = false;
                }
                
                Logger.LogInfo(showMenu ? "物品添加器窗口已打开" : "物品添加器窗口已关闭");
            }
            
            // 阻止游戏输入当窗口显示时（游戏会继续运行，但不允许操作游戏界面）
            if (blockGameInput)
            {
                // 阻止鼠标滚轮
                if (Input.mouseScrollDelta.y != 0)
                {
                    Input.ResetInputAxes();
                }
                
                // 阻止鼠标点击
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
                {
                    Input.ResetInputAxes();
                }
                
                // 阻止键盘输入（保留F2键用于关闭窗口）
                if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.F2))
                {
                    Input.ResetInputAxes();
                }
            }
        }
        
        private void OnGUI()
        {
            if (!showMenu)
                return;
            
            // 保存窗口背景色并设置为半透明
            var originalBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f, 0.95f);
            
            // 显示一个半透明的背景遮罩，防止操作游戏界面
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
            GUI.color = new Color(0, 0, 0, 0.1f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.EndGroup();
            
            // 应用缩放因子
            var guiMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleFactor, scaleFactor, 1f));
            
            // 根据缩放因子调整字体大小
            GUI.skin.window.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            GUI.skin.window.padding = new RectOffset(
                Mathf.RoundToInt(20 * scaleFactor), 
                Mathf.RoundToInt(20 * scaleFactor), 
                Mathf.RoundToInt(10 * scaleFactor), 
                Mathf.RoundToInt(10 * scaleFactor)
            );
            
            GUI.skin.label.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            GUI.skin.button.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            GUI.skin.textField.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            
            // 创建窗口
            windowRect = GUI.Window(0, windowRect, DrawWindow, "", GUI.skin.window);
            
            // 恢复原始矩阵
            GUI.matrix = guiMatrix;
            
            // 恢复原始背景色
            GUI.backgroundColor = originalBackgroundColor;
        }
        
        private void DrawWindow(int windowID)
        {
            // 设置统一的字体大小
            // 根据scaleFactor调整字体大小
            var fontSize = Mathf.RoundToInt(18 * scaleFactor);
            GUI.skin.label.fontSize = fontSize;
            GUI.skin.button.fontSize = fontSize;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter; 
            GUI.skin.toggle.fontSize = fontSize; // 设置选中项字体大小与其他元素一致
            GUI.skin.toggle.alignment = TextAnchor.MiddleCenter; 
            GUI.skin.textField.fontSize = fontSize;
            GUI.skin.window.fontSize = fontSize;
            
            // 根据scaleFactor调整窗口最小宽度
            windowRect.width = Mathf.Max(windowRect.width, 800f * scaleFactor);
            
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
            // 窗口最上方标题文本
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            var titleStyle = new GUIStyle(GUI.skin.label);
            GUILayout.Label(LanguageManager.Instance.GetText("物品添加器"), titleStyle, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15f * scaleFactor);
            
            // 模式选择按钮（货币、物品、话本、地图）
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            if (GUILayout.Button(LanguageManager.Instance.GetText("货币"), GUILayout.ExpandWidth(true)))
            {
                currentMode = 0;
            }
            if (GUILayout.Button(LanguageManager.Instance.GetText("物品"), GUILayout.ExpandWidth(true)))
            {
                currentMode = 1;
            }
            if (GUILayout.Button(LanguageManager.Instance.GetText("话本"), GUILayout.ExpandWidth(true)))
            {
                currentMode = 2;
            }
            if (GUILayout.Button(LanguageManager.Instance.GetText("地图"), GUILayout.ExpandWidth(true)))
            {
                currentMode = 3;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10f * scaleFactor);
            
            // 搜索文本框，支持部分搜索
            GUILayout.BeginHorizontal();
            GUILayout.Label(currentMode == 1 ? LanguageManager.Instance.GetText("搜索物品:") : LanguageManager.Instance.GetText("搜索:"), GUILayout.Width(160f * scaleFactor));
            var newSearchText = GUILayout.TextField(searchText, GUILayout.ExpandWidth(true));
            if (newSearchText != searchText)
            {
                searchText = newSearchText;
                FilterItems();
            }
            GUILayout.EndHorizontal();
            
            // 物品分类快捷搜索按钮 - 仅在物品模式下显示
            if (currentMode == 1)
            {
                GUILayout.Space(10f * scaleFactor);
                GUILayout.BeginHorizontal();
                GUILayout.Label(LanguageManager.Instance.GetText("分类:"), GUILayout.Width(100f * scaleFactor));
                if (GUILayout.Button(LanguageManager.Instance.GetText("清空"), GUILayout.Width(120f * scaleFactor)))
                {
                    searchText = "";
                    selectedCategory = "";
                    FilterItems();
                }
                GUILayout.EndHorizontal();
                
                // 分类按钮布局 - 分多行显示
                // 特殊物品和新增物品按钮在同一行
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(LanguageManager.Instance.GetText("特殊物品"), GUILayout.Width(300f * scaleFactor))) { SearchByCategory("特殊物品"); }
                GUILayout.Space(10f * scaleFactor);
                if (GUILayout.Button(LanguageManager.Instance.GetText("新增物品"), GUILayout.Width(300f * scaleFactor))) { SearchByCategory("新增物品"); }
                GUILayout.EndHorizontal();
                
                // 第一行：丹药、符咒、毒药、美食、农产
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(LanguageManager.Instance.GetText("丹药"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("丹药"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("符咒"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("符咒"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("毒药"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("毒药"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("美食"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("美食"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("农产"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("农产"); }
                GUILayout.EndHorizontal();
                
                // 第二行：布料、矿产、香粉、珠宝、武器
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(LanguageManager.Instance.GetText("布料"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("布料"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("矿产"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("矿产"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("香粉"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("香粉"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("珠宝"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("珠宝"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("武器"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("武器"); }
                GUILayout.EndHorizontal();
                
                // 第三行：书法、丹青、文玩、乐器、茶具
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(LanguageManager.Instance.GetText("书法"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("书法"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("丹青"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("丹青"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("文玩"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("文玩"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("乐器"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("乐器"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("茶具"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("茶具"); }
                GUILayout.EndHorizontal();
                
                // 第四行：香具、瓷器、美酒、皮毛、书籍
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(LanguageManager.Instance.GetText("香具"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("香具"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("瓷器"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("瓷器"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("美酒"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("美酒"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("皮毛"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("皮毛"); }
                if (GUILayout.Button(LanguageManager.Instance.GetText("书籍"), GUILayout.Width(180f * scaleFactor))) { SearchByCategory("书籍"); }
                GUILayout.EndHorizontal();
                GUILayout.Space(10f * scaleFactor);
            }
            else
            {
                GUILayout.Space(10f * scaleFactor);
            }
            
            // 根据当前模式显示不同内容
            if (currentMode == 0)
            {
                // 货币模式
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.Height(300f * scaleFactor));
                
                // 货币类型选择
                GUILayout.BeginHorizontal();
                GUILayout.Label(LanguageManager.Instance.GetText("选择货币类型:"), GUILayout.Width(200f * scaleFactor));
                if (GUILayout.Button(LanguageManager.Instance.GetText("铜钱"), GUILayout.Width(160f * scaleFactor)))
                {
                    selectedCurrencyType = 0; // 0表示铜钱
                }
                if (GUILayout.Button(LanguageManager.Instance.GetText("元宝"), GUILayout.Width(160f * scaleFactor)))
                {
                    selectedCurrencyType = 1; // 1表示元宝
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10f * scaleFactor);
                
                // 数值输入
                GUILayout.BeginHorizontal();
                GUILayout.Label(LanguageManager.Instance.GetText("数值:"), GUILayout.Width(160f * scaleFactor));
                var currencyValueInput = GUILayout.TextField(currencyValue.ToString(), GUILayout.Width(200f * scaleFactor));
                if (int.TryParse(currencyValueInput, out var newCurrencyValue))
                {
                    if (selectedCurrencyType == 0) // 铜钱
                    {
                        currencyValue = Mathf.Clamp(newCurrencyValue, 0, 1000000000); // 限制在0到10亿之间
                    }
                    else // 元宝
                    {
                        currencyValue = Mathf.Clamp(newCurrencyValue, 0, 100000); // 限制在0到10万之间
                    }
                }
                // 显示输入限制
                GUILayout.Label(selectedCurrencyType == 0 ? LanguageManager.Instance.GetText("(0-10亿)") : LanguageManager.Instance.GetText("(0-10万)"), GUILayout.Width(140f * scaleFactor));
                GUILayout.EndHorizontal();
                
                // 预设数值按钮
                GUILayout.BeginHorizontal();
                if (selectedCurrencyType == 0) // 铜钱
                {
                    if (GUILayout.Button(LanguageManager.Instance.GetText("100万"), GUILayout.Width(160f * scaleFactor)))
                    {
                        currencyValue = 1000000;
                    }
                    if (GUILayout.Button(LanguageManager.Instance.GetText("1亿"), GUILayout.Width(160f * scaleFactor)))
                    {
                        currencyValue = 100000000;
                    }
                    if (GUILayout.Button(LanguageManager.Instance.GetText("10亿"), GUILayout.Width(160f * scaleFactor)))
                    {
                        currencyValue = 1000000000;
                    }
                }
                else if (selectedCurrencyType == 1) // 元宝
                {
                    if (GUILayout.Button(LanguageManager.Instance.GetText("1百"), GUILayout.Width(160f * scaleFactor)))
                    {
                        currencyValue = 100;
                    }
                    if (GUILayout.Button(LanguageManager.Instance.GetText("1千"), GUILayout.Width(160f * scaleFactor)))
                    {
                        currencyValue = 1000;
                    }
                    if (GUILayout.Button(LanguageManager.Instance.GetText("1万"), GUILayout.Width(160f * scaleFactor)))
                    {
                        currencyValue = 10000;
                    }
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10f * scaleFactor);
                
                // 当前货币状态显示
                GUILayout.BeginVertical();
                GUILayout.Label(string.Format(LanguageManager.Instance.GetText("当前铜钱: {0}"), FormulaData.GetCoinsNum()), GUILayout.ExpandWidth(true));
                GUILayout.Label(LanguageManager.Instance.GetText("当前元宝: ") + Mainload.CGNum[1], GUILayout.ExpandWidth(true));
                GUILayout.EndVertical();
                
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            } else if (currentMode == 1)
            {
                // 物品模式
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.Height(300f * scaleFactor));
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                
                // 创建物品列表
                var selectedIndex = filteredItemIds.IndexOf(selectedItemId);
                if (selectedIndex == -1 && filteredItemIds.Count > 0)
                {
                    selectedIndex = 0;
                    selectedItemId = filteredItemIds[0];
                }
                
                if (filteredItemIds.Count > 0)
                {
                    for (var i = 0; i < filteredItemIds.Count; i++)
                    {
                        var isSelected = (i == selectedIndex);
                        var buttonStyle = isSelected ? GUI.skin.toggle : GUI.skin.button;
                        
                        var itemName = itemList[filteredItemIds[i]].Item1;
                        var translatedItemName = LanguageManager.Instance.GetText(itemName);
                        if (GUILayout.Button(translatedItemName, buttonStyle, GUILayout.ExpandWidth(true)))
                        {
                            selectedItemId = filteredItemIds[i];
                        }
                    }
                }
                else
                {
                    GUILayout.Label(LanguageManager.Instance.GetText("未找到匹配的物品"), GUILayout.ExpandWidth(true));
                }
                
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            } else if (currentMode == 2)
            {
                // 话本模式
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.Height(300f * scaleFactor));
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                
                // 创建话本列表，根据语言显示对应文本
                foreach (var book in bookList)
                {
                    var displayName = LanguageManager.Instance.IsChineseLanguage() ? book.Value[0] : book.Value[1];
                    if (GUILayout.Button(displayName, GUILayout.ExpandWidth(true)))
                    {
                        selectedItemId = book.Key;
                    }
                }
                
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            } else if (currentMode == 3)
            {
                // 地图模式
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.Height(500f * scaleFactor));
                
                // 地图子模式选择按钮 - 仅在地图模式下显示
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                if (GUILayout.Button(LanguageManager.Instance.GetText("府邸"), GUILayout.ExpandWidth(true)))
                {
                    mapSubMode = 0;
                }
                if (GUILayout.Button(LanguageManager.Instance.GetText("农庄"), GUILayout.ExpandWidth(true)))
                {
                    mapSubMode = 1;
                }
                if (GUILayout.Button(LanguageManager.Instance.GetText("封地"), GUILayout.ExpandWidth(true)))
                {
                    mapSubMode = 2;
                }
                if (GUILayout.Button(LanguageManager.Instance.GetText("世家"), GUILayout.ExpandWidth(true)))
                {
                    mapSubMode = 3;
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10f * scaleFactor);
                
                // 根据选择的地图子模式显示内容
                GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                
                if (mapSubMode == 0)
                {
                    GUILayout.Label(LanguageManager.Instance.GetText("府邸子模式"), GUILayout.ExpandWidth(true));
                    
                    // 生成府邸所在郡
                    GUILayout.Space(10f * scaleFactor);
                    GUILayout.Label(LanguageManager.Instance.GetText("生成府邸所在郡："), GUILayout.ExpandWidth(true));
                    
                    // 第一行6个郡按钮，使用固定宽度确保对齐
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    var buttonWidth = 180f * scaleFactor; 
                    for (var i = 0; i < 6 && i < JunList.Length; i++)
                    {
                        var junName = JunList[i];
                        var translatedJunName = LanguageManager.Instance.GetText(junName);
                        var junIndex = i;
                        if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                        {
                            selectedPrefecture = junName;
                            selectedJunIndex = junIndex;
                            selectedCounty = "";
                        }
                    }
                    GUILayout.EndHorizontal();
                    
                    // 第二行6个郡按钮，使用固定宽度确保对齐
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    for (var i = 6; i < 12 && i < JunList.Length; i++)
                    {
                        var junName = JunList[i];
                        var translatedJunName = LanguageManager.Instance.GetText(junName);
                        var junIndex = i;
                        if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                        {
                            selectedPrefecture = junName;
                            selectedJunIndex = junIndex;
                            selectedCounty = "";
                        }
                    }
                    GUILayout.EndHorizontal();
                    
                    // 生成府邸所在县
                    GUILayout.Space(10f * scaleFactor);
                    GUILayout.Label(LanguageManager.Instance.GetText("生成府邸所在县："), GUILayout.ExpandWidth(true));
                    
                    // 只有选择了郡才显示县按钮
                    if (selectedJunIndex >= 0 && selectedJunIndex < XianList.Length)
                    {
                        var xianArray = XianList[selectedJunIndex];
                        var xianCount = xianArray.Length;
                        
                        // 所有县按钮都均布在一行
                        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                        
                        // 设置每个县按钮的固定宽度，确保均匀分布
                        var xianButtonWidth = 120f * scaleFactor; 
                        
                        for (var i = 0; i < xianCount; i++)
                        {
                            var xianName = xianArray[i];
                            var translatedXianName = LanguageManager.Instance.GetText(xianName);
                            if (GUILayout.Button(translatedXianName, GUILayout.Width(xianButtonWidth))) 
                            {
                                selectedCounty = xianName;
                            }
                        }
                        
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.Label(LanguageManager.Instance.GetText("请先选择一个郡"), GUILayout.ExpandWidth(true));
                    }
                    
                    // 显示选择的郡县
                    GUILayout.Space(10f * scaleFactor);
                    var displayPrefectureCounty = string.IsNullOrEmpty(selectedPrefecture) || string.IsNullOrEmpty(selectedCounty) ? "--" : $"{LanguageManager.Instance.GetText(selectedPrefecture)}-{LanguageManager.Instance.GetText(selectedCounty)}";
                    GUILayout.Label($"{LanguageManager.Instance.GetText("生成府邸所在的郡县：")}{displayPrefectureCounty}", GUILayout.ExpandWidth(true));
                    
                    // 府邸自定义名字
                    GUILayout.Space(5f * scaleFactor);
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    GUILayout.Label(LanguageManager.Instance.GetText("府邸的名字："), GUILayout.Width(120f * scaleFactor));
                    mansionCustomName = GUILayout.TextField(mansionCustomName, GUILayout.ExpandWidth(true));
                    GUILayout.EndHorizontal();
                    
                    // 添加府邸方式选择
                    GUILayout.Space(5f * scaleFactor);
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    var buttonStyle1 = new GUIStyle(GUI.skin.button);
                    var buttonStyle2 = new GUIStyle(GUI.skin.button);
                    
                    if (onlyAddMansion)
                    {
                        buttonStyle1.normal.textColor = Color.green;
                        buttonStyle1.fontStyle = FontStyle.Bold;
                    }
                    else
                    {
                        buttonStyle2.normal.textColor = Color.green;
                        buttonStyle2.fontStyle = FontStyle.Bold;
                    }
                    
                    if (GUILayout.Button(LanguageManager.Instance.GetText("仅添加府邸"), buttonStyle1, GUILayout.ExpandWidth(true)))
                    {
                        onlyAddMansion = true;
                    }
                    if (GUILayout.Button(LanguageManager.Instance.GetText("添加后进入府邸"), buttonStyle2, GUILayout.ExpandWidth(true)))
                    {
                        onlyAddMansion = false;
                    }
                    GUILayout.EndHorizontal();
                } 
                else if (mapSubMode == 1)
                {
                    GUILayout.Label(LanguageManager.Instance.GetText("农庄子模式"), GUILayout.ExpandWidth(true));
                    
                    // 生成农庄所在郡
                    GUILayout.Space(10f * scaleFactor);
                    GUILayout.Label(LanguageManager.Instance.GetText("生成农庄所在郡："), GUILayout.ExpandWidth(true));
                    
                    // 第一行6个郡按钮，使用固定宽度确保对齐
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    var buttonWidth = 180f * scaleFactor; 
                    for (var i = 0; i < 6 && i < JunList.Length; i++)
                    {
                        var junName = JunList[i];
                        var translatedJunName = LanguageManager.Instance.GetText(junName);
                        var junIndex = i;
                        if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                        {
                            selectedPrefecture = junName;
                            selectedJunIndex = junIndex;
                            selectedCounty = "";
                        }
                    }
                    GUILayout.EndHorizontal();
                    
                    // 第二行6个郡按钮，使用固定宽度确保对齐
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    for (var i = 6; i < 12 && i < JunList.Length; i++)
                    {
                        var junName = JunList[i];
                        var translatedJunName = LanguageManager.Instance.GetText(junName);
                        var junIndex = i;
                        if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                        {
                            selectedPrefecture = junName;
                            selectedJunIndex = junIndex;
                            selectedCounty = "";
                        }
                    }
                    GUILayout.EndHorizontal();
                    
                    // 生成农庄所在县
                    GUILayout.Space(10f * scaleFactor);
                    GUILayout.Label(LanguageManager.Instance.GetText("生成农庄所在县："), GUILayout.ExpandWidth(true));
                    
                    // 只有选择了郡才显示县按钮
                    if (selectedJunIndex >= 0 && selectedJunIndex < XianList.Length)
                    {
                        var xianArray = XianList[selectedJunIndex];
                        var xianCount = xianArray.Length;
                        
                        // 所有县按钮都均布在一行
                        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                        
                        // 设置每个县按钮的固定宽度，确保均匀分布
                        var xianButtonWidth = 120f * scaleFactor; 
                        
                        for (var i = 0; i < xianCount; i++)
                        {
                            var xianName = xianArray[i];
                            var translatedXianName = LanguageManager.Instance.GetText(xianName);
                            if (GUILayout.Button(translatedXianName, GUILayout.Width(xianButtonWidth))) 
                            {
                                selectedCounty = xianName;
                            }
                        }
                        
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.Label(LanguageManager.Instance.GetText("请先选择一个郡"), GUILayout.ExpandWidth(true));
                    }
                    
                    // 显示选择的郡县
                    GUILayout.Space(10f * scaleFactor);
                    var displayPrefectureCounty = string.IsNullOrEmpty(selectedPrefecture) || string.IsNullOrEmpty(selectedCounty) ? "--" : $"{LanguageManager.Instance.GetText(selectedPrefecture)}-{LanguageManager.Instance.GetText(selectedCounty)}";
                    GUILayout.Label(LanguageManager.Instance.GetText("生成农庄所在的郡县：") + displayPrefectureCounty, GUILayout.ExpandWidth(true));
                    
                    // 农庄自定义名字
                    GUILayout.Space(5f * scaleFactor);
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    GUILayout.Label(LanguageManager.Instance.GetText("农庄的名字："), GUILayout.Width(120f * scaleFactor));
                        farmCustomName = GUILayout.TextField(farmCustomName, GUILayout.ExpandWidth(true), GUILayout.MinWidth(250f * scaleFactor));
                    GUILayout.EndHorizontal();
                    
                    // 农庄面积选择
                    GUILayout.Space(5f * scaleFactor);
                    GUILayout.Label(LanguageManager.Instance.GetText("农庄面积选择："), GUILayout.ExpandWidth(true));
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(LanguageManager.Instance.GetText("4"), GUILayout.ExpandWidth(true)))
                    {
                        farmArea = 4;
                    }
                    if (GUILayout.Button(LanguageManager.Instance.GetText("9"), GUILayout.ExpandWidth(true)))
                    {
                        farmArea = 9;
                    }
                    if (GUILayout.Button(LanguageManager.Instance.GetText("16"), GUILayout.ExpandWidth(true)))
                    {
                        farmArea = 16;
                    }
                    if (GUILayout.Button(LanguageManager.Instance.GetText("25"), GUILayout.ExpandWidth(true)))
                    {
                        farmArea = 25;
                    }
                    GUILayout.EndHorizontal();
                } 
                else if (mapSubMode == 2)
                {
                    GUILayout.Label(LanguageManager.Instance.GetText("封地子模式"), GUILayout.ExpandWidth(true));
                    
                    // 生成封地所在郡
                    GUILayout.Space(10f * scaleFactor);
                    GUILayout.Label(LanguageManager.Instance.GetText("选择要解锁的郡："), GUILayout.ExpandWidth(true));
                    
                    // 第一行6个郡按钮，使用固定宽度确保对齐
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    var buttonWidth = 180f * scaleFactor; 
                    for (var i = 0; i < 6 && i < JunList.Length; i++)
                    {
                        var junName = JunList[i];
                        var translatedJunName = LanguageManager.Instance.GetText(junName);
                        var junIndex = i;
                        if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                        {
                            selectedPrefecture = junName;
                            selectedJunIndex = junIndex;
                            selectedCounty = "";
                        }
                    }
                    GUILayout.EndHorizontal();
                    
                    // 第二行6个郡按钮，使用固定宽度确保对齐
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    for (var i = 6; i < 12 && i < JunList.Length; i++)
                    {
                        var junName = JunList[i];
                        var translatedJunName = LanguageManager.Instance.GetText(junName);
                        var junIndex = i;
                        if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                        {
                            selectedPrefecture = junName;
                            selectedJunIndex = junIndex;
                            selectedCounty = "";
                        }
                    }
                    GUILayout.EndHorizontal();
                    
                    // 显示选择的郡
                    GUILayout.Space(10f * scaleFactor);
                    var displayPrefecture = string.IsNullOrEmpty(selectedPrefecture) ? "--" : LanguageManager.Instance.GetText(selectedPrefecture);
                    GUILayout.Label(LanguageManager.Instance.GetText("当前选择的郡：") + displayPrefecture, GUILayout.ExpandWidth(true));
                    
                    GUILayout.Space(10f * scaleFactor);
                    GUILayout.Label(LanguageManager.Instance.GetText("点击下方添加按钮即可解锁选择郡的所属封地"), GUILayout.ExpandWidth(true));
                } 
                else if (mapSubMode == 3)
                {
                    GUILayout.Label(LanguageManager.Instance.GetText("世家子模式"), GUILayout.ExpandWidth(true));
                    
                    // 生成世家所在郡
                    GUILayout.Space(10f * scaleFactor);
                    GUILayout.Label(LanguageManager.Instance.GetText("生成世家所在郡："), GUILayout.ExpandWidth(true));
                    
                    // 第一行6个郡按钮，使用固定宽度确保对齐
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    var buttonWidth = 180f * scaleFactor; 
                    for (var i = 0; i < 6 && i < JunList.Length; i++)
                    {
                        var junName = JunList[i];
                        var translatedJunName = LanguageManager.Instance.GetText(junName);
                        var junIndex = i;
                        if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                        {
                            selectedPrefecture = junName;
                            selectedJunIndex = junIndex;
                            selectedCounty = "";
                        }
                    }
                    GUILayout.EndHorizontal();
                    
                    // 第二行6个郡按钮，使用固定宽度确保对齐
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    for (var i = 6; i < 12 && i < JunList.Length; i++)
                    {
                        var junName = JunList[i];
                        var translatedJunName = LanguageManager.Instance.GetText(junName);
                        var junIndex = i;
                        if (GUILayout.Button(translatedJunName, GUILayout.Width(buttonWidth))) 
                        {
                            selectedPrefecture = junName;
                            selectedJunIndex = junIndex;
                            selectedCounty = "";
                        }
                    }
                    GUILayout.EndHorizontal();
                    
                    // 生成世家所在县
                    GUILayout.Space(10f * scaleFactor);
                    GUILayout.Label(LanguageManager.Instance.GetText("生成世家所在县："), GUILayout.ExpandWidth(true));
                    
                    // 只有选择了郡才显示县按钮
                    if (selectedJunIndex >= 0 && selectedJunIndex < XianList.Length)
                    {
                        var xianArray = XianList[selectedJunIndex];
                        var xianCount = xianArray.Length;
                        
                        // 所有县按钮都均布在一行
                        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                        
                        // 设置每个县按钮的固定宽度，确保均匀分布
                        var xianButtonWidth = 120f * scaleFactor; 
                        
                        for (var i = 0; i < xianCount; i++)
                        {
                            var xianName = xianArray[i];
                            var translatedXianName = LanguageManager.Instance.GetText(xianName);
                            if (GUILayout.Button(translatedXianName, GUILayout.Width(xianButtonWidth))) 
                            {
                                selectedCounty = xianName;
                            }
                        }
                        
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.Label(LanguageManager.Instance.GetText("请先选择一个郡"), GUILayout.ExpandWidth(true));
                    }
                    
                    // 显示选择的郡县
                    GUILayout.Space(10f * scaleFactor);
                    var displayPrefectureCounty = string.IsNullOrEmpty(selectedPrefecture) || string.IsNullOrEmpty(selectedCounty) ? "--" : $"{LanguageManager.Instance.GetText(selectedPrefecture)}-{LanguageManager.Instance.GetText(selectedCounty)}";
                    GUILayout.Label(LanguageManager.Instance.GetText("生成世家所在的郡县：") + displayPrefectureCounty, GUILayout.ExpandWidth(true));
                }
                
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            
            GUILayout.Space(10f * scaleFactor);
            
            // 选择框、数量输入框、添加按钮
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            
            // 仅在物品模式下显示数量输入框
            if (currentMode == 1)
            {
                // 数量输入
                GUILayout.BeginHorizontal();
                GUILayout.Label(LanguageManager.Instance.GetText("数量:"), GUILayout.Width(100f * scaleFactor));
                var newCountInput = GUILayout.TextField(countInput, GUILayout.Width(200f * scaleFactor));
                if (newCountInput != countInput)
                {
                    countInput = newCountInput;
                    if (int.TryParse(countInput, out var newCount))
                    {
                        // 限制数量在0到10000之间
                        count = Mathf.Clamp(newCount, 0, 1000000);
                        // 如果输入的数量超出范围，自动修正显示
                        if (newCount != count)
                        {
                            countInput = count.ToString();
                        }
                    }
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10f * scaleFactor);
            }
            
            // 添加按钮
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(currentMode == 1 ? LanguageManager.Instance.GetText("添加物品") : LanguageManager.Instance.GetText("添加"), GUILayout.Width(180f * scaleFactor), GUILayout.Height(80f * scaleFactor)))
            {
                AddItemToGame();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            // 使用说明
            GUILayout.Space(20f * scaleFactor);
            GUILayout.BeginVertical();
            
            // 使用说明标题
            GUILayout.Label(LanguageManager.Instance.GetText("使用说明:"), GUI.skin.box);
            
            // 使用说明
            GUILayout.Label(LanguageManager.Instance.GetText("1. 请在点击添加前先保存游戏，以便回档"));
            GUILayout.Label(LanguageManager.Instance.GetText("2. 按F2键显示/隐藏窗口"));
            GUILayout.Label(LanguageManager.Instance.GetText("3. 切换模式选择：物品模式/货币模式/话本模式/地图模式"));
            GUILayout.Label(LanguageManager.Instance.GetText("4. 输入部分字符可搜索物品或话本"));
            GUILayout.Label(LanguageManager.Instance.GetText("5. 选择项目并选择或输入数量后点击添加按钮"));
            GUILayout.Label("");
            
            // MOD作者及版本号说明
            GUILayout.Label(LanguageManager.Instance.GetText("Mod作者：AnZhi20"));
            GUILayout.Label(LanguageManager.Instance.GetText("Mod版本：") + PluginInfo.PLUGIN_VERSION);
            GUILayout.EndVertical();
            
            GUILayout.EndVertical();
            
            // 允许拖动窗口
            GUI.DragWindow(new Rect(0, 0, windowRect.width, windowRect.height));
        }
        
        private void FilterItems()
        {
            // 只在物品模式下进行搜索过滤
            if (currentMode == 1)
            {
                // 组合搜索：同时考虑文本搜索和分类过滤
                var lowerSearchText = searchText.ToLower();
                var hasSearchText = !string.IsNullOrEmpty(searchText);
                var hasSelectedCategory = !string.IsNullOrEmpty(selectedCategory);
                
                if (!hasSearchText && !hasSelectedCategory)
                {
                    // 无过滤条件，显示所有物品
                    filteredItemIds = itemList.Keys.ToList();
                }
                else
                {
                    filteredItemIds = itemList.Where(kv => 
                {
                    // 获取物品名称和分类名称
                    var itemName = kv.Value.Item1;
                    var categoryName = kv.Value.Item2;
                    
                    // 获取物品名称的翻译
                    var translatedItemName = LanguageManager.Instance.GetText(itemName);
                    var translatedCategoryName = LanguageManager.Instance.GetText(categoryName);
                    
                    var matchesSearchText = !hasSearchText || 
                                            itemName.ToLower().Contains(lowerSearchText) || 
                                            translatedItemName.ToLower().Contains(lowerSearchText) ||
                                            categoryName.ToLower().Contains(lowerSearchText) || 
                                            translatedCategoryName.ToLower().Contains(lowerSearchText);
                    
                    var matchesCategory = !hasSelectedCategory || 
                                          categoryName.ToLower().Contains(selectedCategory.ToLower()) ||
                                          translatedCategoryName.ToLower().Contains(selectedCategory.ToLower());
                    
                    // 同时满足文本搜索和分类过滤条件
                    return matchesSearchText && matchesCategory;
                })
                    .Select(kv => kv.Key)
                    .ToList();
                }
            }
            // 话本模式下无需过滤（列表较短）
        }
        
        // 分类搜索方法
        private void SearchByCategory(string category)
        {
            // 清空搜索文本以显示完整分类结果
            //searchText = "";
            selectedCategory = category;
            FilterItems();
        }
        
        private void AddItemToGame()
        {
            try
            {
                // 地图模式下不需要检查数量
                if (currentMode != 3 && (count <= 0 || count > 1000000))
                {
                    statusMessage = LanguageManager.Instance.GetText("无效的数量，请输入1-1000000范围内的整数");
                    Logger.LogError("无效的数量: " + count);
                    return;
                }
                
                if (currentMode == 1)
                {
                    // 物品模式
                    AddItemToGameInventory(selectedItemId, count);
                }
                else if (currentMode == 2)
                {
                    // 话本模式
                    AddScriptBookToGame(selectedItemId);
                } 
                else if (currentMode == 3)
                {
                    // 地图模式
                    if (mapSubMode == 0)
                    {
                        // 府邸模式
                        if (string.IsNullOrEmpty(selectedPrefecture) || string.IsNullOrEmpty(selectedCounty))
                        {
                            statusMessage = LanguageManager.Instance.GetText("请先选择府邸所在的郡县");
                            Logger.LogError("未选择郡县");
                            return;
                        }
                        
                        // 获取郡索引和对应的县索引
                        var junIndex = selectedJunIndex;
                        var xianIndex = -1;
                        if (junIndex >= 0 && junIndex < XianList.Length)
                        {
                            var xianArray = XianList[junIndex];
                            for (var i = 0; i < xianArray.Length; i++)
                            {
                                if (xianArray[i] == selectedCounty)
                                {
                                    xianIndex = i;
                                    break;
                                }
                            }
                        }
                        
                        if (xianIndex < 0)
                        {
                            statusMessage = LanguageManager.Instance.GetText("找不到选择的县");
                            Logger.LogError("无效的县选择");
                            return;
                        }
                        
                        // 在添加府邸前检查Mainload.CityData_now中的势力名称
                        if (Mainload.CityData_now != null && junIndex < Mainload.CityData_now.Count)
                        {
                            // 获取郡城相关信息（第一个元素）
                            var junData = Mainload.CityData_now[junIndex] as IEnumerable;
                            if (junData != null)
                            {
                                // 获取第一个元素（郡城信息）
                                var cityInfoObj = junData.Cast<object>().FirstOrDefault();
                                if (cityInfoObj != null)
                                {
                                    var cityInfo = cityInfoObj as List<string>;
                                    if (cityInfo != null && cityInfo.Count > 0)
                                    {
                                        // 获取城池控制势力信息（第一个元素）
                                        var controlInfo = cityInfo[0];
                                        if (!string.IsNullOrEmpty(controlInfo))
                                        {
                                            // 以|分割获取势力名称
                                            var parts = controlInfo.Split('|');
                                            if (parts.Length > 0 && parts[0] != "-1")
                                            {
                                                // 势力名称不为-1，不添加府邸
                                                statusMessage = string.Format(LanguageManager.Instance.GetText("{0}未解锁或郡城叛军未清剿，无法添加府邸"), selectedPrefecture);
                                                Logger.LogWarning(statusMessage);
                                                  
                                                // 显示提示信息
                                                ShowTipMessage(statusMessage);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        
                        // 获取用户输入的府邸名称，如果为空则使用默认名称
                        var mansionName = string.IsNullOrEmpty(mansionCustomName) ? $"{selectedPrefecture}府" : mansionCustomName;
                         
                        // 创建位置键
                        var locationKey = $"{junIndex}|{xianIndex}";
                         
                        // 检查该位置是否已存在府邸
                        var mansionExists = false;
                        if (Mainload.Fudi_now != null)
                        {
                            foreach (var mansion in Mainload.Fudi_now)
                            {
                                if (mansion != null && mansion.Count > 0 && mansion[0] == locationKey)
                                {
                                    mansionExists = true;
                                    break;
                                }
                            }
                        }
                         
                        if (mansionExists)
                        {
                            statusMessage = string.Format(LanguageManager.Instance.GetText("{0}-{1}已有府邸，添加失败"), selectedPrefecture, selectedCounty);
                            Logger.LogWarning(statusMessage);
                             
                            // 显示提示信息
                            ShowTipMessage(statusMessage);
                            return;
                        }
                         
                        // 检查该位置是否已存在农庄（状态为-1的农庄）
                        var farmExists = CheckIfFarmExistsAtLocation(locationKey);
                          
                        if (farmExists)
                        {
                            statusMessage = string.Format(LanguageManager.Instance.GetText("{0}-{1}已有农庄，添加失败"), selectedPrefecture, selectedCounty);
                            Logger.LogWarning(statusMessage);
                              
                            // 显示提示信息
                            ShowTipMessage(statusMessage);
                            return;
                        }
                         
                        // 检查该位置是否已存在世家
                        var shiJiaExists = false;
                        if (Mainload.ShiJia_Now != null)
                        {
                            foreach (var shiJia in Mainload.ShiJia_Now)
                            {
                                if (shiJia != null && shiJia.Count > 5 && shiJia[5] == locationKey)
                                {
                                    shiJiaExists = true;
                                    break;
                                }
                            }
                        }
                         
                        if (shiJiaExists)
                        {
                            statusMessage = string.Format(LanguageManager.Instance.GetText("{0}-{1}已有世家，添加失败"), selectedPrefecture, selectedCounty);
                            Logger.LogWarning(statusMessage);
                             
                            // 显示提示信息
                            ShowTipMessage(statusMessage);
                            return;
                        }
                         
                        // 创建新的府邸数组
                        var newMansion = new List<string>
                        {
                            locationKey, // 0，府邸位置，郡索引|县索引
                            mansionName, // 1，府邸名称
                            "0",         // 2
                            "0",         // 3
                            "0",         // 4
                            "1",         // 5
                            "0",         // 6
                            "0",         // 7
                            "0",         // 8
                            "0",         // 9
                            "1",         // 10
                            "0",         // 11
                            "0",         // 12
                            "0",         // 13
                            "0",         // 14
                            "0",         // 15
                            "0",         // 16
                            "0",         // 17
                            "0",         // 18
                            "0",         // 19
                            "0",         // 20
                            "0",         // 21
                            "0",         // 22
                            "0",         // 23
                            "0",         // 24
                            "0",         // 25
                            "0",         // 26
                            "0",         // 27
                            "0",         // 28
                            "0",         // 29
                            "0",         // 30，环境属性值
                            "0",         // 31，安全属性值
                            "0",         // 32，便捷属性值
                            "0",         // 33
                            "0",         // 34
                            "0",         // 35
                            "0",         // 36
                            "25",        // 37
                            "0",         // 38
                            "0"          // 39
                        };
                        
                        // 添加到游戏中
                        try
                        {
                            //添加府邸数据并保存
                            Mainload.Fudi_now.Add(newMansion);
                            ES3.Save<List<List<string>>>("Fudi_now", Mainload.Fudi_now, Mainload.CunDangIndex_now + "/GameData.es3");

                            //添加府邸文件并保存
                            var value = new List<List<string>>();
                            ES3.Save<List<List<string>>>("BuildInto_m", value, Mainload.CunDangIndex_now + "/M" + (Mainload.Fudi_now.Count -1) + ".es3");
                            
                            //添加图标
                            Mainload.NewCreateShijia.Add(junIndex + "|" + xianIndex + "|-1");

                            //提示消息
                            statusMessage = string.Format(LanguageManager.Instance.GetText("已在({0}-{1})添加府邸: {2} "), LanguageManager.Instance.GetText(selectedPrefecture), LanguageManager.Instance.GetText(selectedCounty), mansionName);
                            Logger.LogInfo(statusMessage);

                            //根据选择的添加方式执行相应操作
                            if (onlyAddMansion)
                            {
                                Add_notEnterHouse();
                            }
                            else
                            {
                                Add_EnterHouse();
                            }
                        }
                        catch (Exception ex)
                        {
                            statusMessage = string.Format(LanguageManager.Instance.GetText("添加府邸数据失败: {0}"), ex.Message);
                            Logger.LogError(statusMessage);
                            Logger.LogError("错误堆栈: " + ex.StackTrace);
                             
                            // 显示提示信息
                            ShowTipMessage(statusMessage);
                            return;
                        }
                        
                        // 显示提示信息
                        ShowTipMessage(statusMessage);
                    }
                    else if (mapSubMode == 1)
                    {
                        // 农庄子模式
                        if (string.IsNullOrEmpty(selectedPrefecture) || string.IsNullOrEmpty(selectedCounty))
                        {
                            statusMessage = LanguageManager.Instance.GetText("请先选择农庄所在的郡县");
                            Logger.LogError("未选择郡县");
                            return;
                        }
                        
                        // 获取郡索引和对应的县索引
                        var junIndex = selectedJunIndex;
                        var xianIndex = -1;
                        if (junIndex >= 0 && junIndex < XianList.Length)
                        {
                            var xianArray = XianList[junIndex];
                            for (var i = 0; i < xianArray.Length; i++)
                            {
                                if (xianArray[i] == selectedCounty)
                                {
                                    xianIndex = i;
                                    break;
                                }
                            }
                        }
                        
                        if (xianIndex < 0)
                        {
                            statusMessage = LanguageManager.Instance.GetText("找不到选择的县");
                            Logger.LogError("无效的县选择");
                            return;
                        }
                        
                        // 在添加农庄前检查Mainload.CityData_now中的势力名称
                        if (Mainload.CityData_now != null && junIndex < Mainload.CityData_now.Count)
                        {
                            // 获取郡城相关信息（第一个元素）
                            var junData = Mainload.CityData_now[junIndex] as IEnumerable;
                            if (junData != null)
                            {
                                // 获取第一个元素（郡城信息）
                                var cityInfoObj = junData.Cast<object>().FirstOrDefault();
                                if (cityInfoObj != null)
                                {
                                    var cityInfo = cityInfoObj as List<string>;
                                    if (cityInfo != null && cityInfo.Count > 0)
                                    {
                                        // 获取城池控制势力信息（第一个元素）
                                        var controlInfo = cityInfo[0];
                                        if (!string.IsNullOrEmpty(controlInfo))
                                        {
                                            // 以|分割获取势力名称
                                            var parts = controlInfo.Split('|');
                                            if (parts.Length > 0 && parts[0] != "-1")
                                            {
                                                // 势力名称不为-1，不添加农庄
                                                statusMessage = string.Format(LanguageManager.Instance.GetText("{0}未解锁或郡城叛军未清剿，无法添加农庄"), selectedPrefecture);
                                                Logger.LogWarning(statusMessage);
                                                  
                                                // 显示提示信息
                                                ShowTipMessage(statusMessage);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        
                        // 获取用户输入的农庄名称，如果为空则使用默认名称
                        var farmName = string.IsNullOrEmpty(farmCustomName) ? $"{selectedPrefecture}农庄" : farmCustomName;
                        
                        // 生成肥力字符串，根据面积设置对应数量的值为1
                        var fertilityBuilder = new StringBuilder();
                        var totalFields = 25; // 总地块数
                        for (var i = 0; i < totalFields; i++)
                        {
                            if (i < farmArea)
                            {
                                fertilityBuilder.Append("1");
                            }
                            else
                            {
                                fertilityBuilder.Append("0");
                            }
                            
                            if (i < totalFields - 1)
                            {
                                fertilityBuilder.Append("|");
                            }
                        }
                        var fertilityString = fertilityBuilder.ToString();
                        
                        // 创建位置键
                        var locationKey = $"{junIndex}|{xianIndex}";
                         
                        // 检查该位置是否已存在府邸
                        var mansionExists = false;
                        if (Mainload.Fudi_now != null)
                        {
                            foreach (var mansion in Mainload.Fudi_now)
                            {
                                if (mansion != null && mansion.Count > 0 && mansion[0] == locationKey)
                                {
                                    mansionExists = true;
                                    break;
                                }
                            }
                        }
                         
                        if (mansionExists)
                        {
                            statusMessage = string.Format(LanguageManager.Instance.GetText("{0}-{1}已有府邸，添加失败"), selectedPrefecture, selectedCounty);
                            Logger.LogWarning(statusMessage);
                             
                            // 显示提示信息
                            ShowTipMessage(statusMessage);
                            return;
                        }
                         
                        // 检查该位置是否已存在世家
                        var shiJiaExists = false;
                        if (Mainload.ShiJia_Now != null)
                        {
                            foreach (var shiJia in Mainload.ShiJia_Now)
                            {
                                if (shiJia != null && shiJia.Count > 5 && shiJia[5] == locationKey)
                                {
                                    shiJiaExists = true;
                                    break;
                                }
                            }
                        }
                         
                        if (shiJiaExists)
                        {
                            statusMessage = string.Format(LanguageManager.Instance.GetText("{0}-{1}已有世家，添加失败"), selectedPrefecture, selectedCounty);
                            Logger.LogWarning(statusMessage);
                             
                            // 添加显示信息
                            // 显示提示信息
                            ShowTipMessage(statusMessage);
                            return;
                        }
                         
                        // 添加到游戏中
                        try
                        {
                            // 使用Mainload.NongZ_now字段
                            if (Mainload.NongZ_now != null)
                            {
                                // 在Mainload.NongZ_now中查找对应位置的农庄
                                var found = false;
                                 
                                // 外层循环遍历每个郡（Mainload.NongZ_now[i]表示一个郡的所有农庄）
                                for (var i = 0; i < Mainload.NongZ_now.Count; i++)
                                {
                                    try 
                                    {
                                        // 获取当前郡的所有农庄数组
                                        var junFarmList = Mainload.NongZ_now[i] as IEnumerable;
                                        if (junFarmList == null)
                                        {
                                            continue;
                                        }
                                         
                                        // 内层循环遍历当前郡中的每个农庄（Mainload.NongZ_now[i][j]表示一个具体的农庄）
                                        var j = 0;
                                        foreach (var farmObj in junFarmList)
                                        {
                                            try
                                            {
                                                // 将每个农庄对象转换为List<string>
                                                var farmItem = farmObj as List<string>;
                                                if (farmItem != null && farmItem.Count > 0)
                                                {
                                                    // 查找位置信息，可能在不同索引位置
                                                    var locationFound = false;
                                                    var locationIndex = -1;
                                                     
                                                    // 遍历farmItem查找位置信息
                                                    for (var k = 0; k < farmItem.Count; k++)
                                                    {
                                                        if (farmItem[k] == locationKey)
                                                        {
                                                            locationFound = true;
                                                            locationIndex = k;
                                                            break;
                                                        }
                                                    }
                                                     
                                                    if (locationFound)
                                                    {
                                                        found = true;
                                                          
                                                        // 检查第一个元素的值
                                                        var status = farmItem[0];
                                                     
                                                        if (status != "0" && status != "-2")
                                                        {
                                                            // 该处已有农庄属于其它世家
                                                            statusMessage = LanguageManager.Instance.GetText("添加失败：该处已有农庄属于【其它世家】");
                                                            Logger.LogWarning(statusMessage);
                                                        }
                                                        else if (status == "-1")
                                                        {
                                                            // 该处已有农庄属于你
                                                            statusMessage = LanguageManager.Instance.GetText("添加失败：该处已有农庄属于【你】");
                                                            Logger.LogWarning(statusMessage);
                                                        }
                                                        else
                                                        {
                                                            // 状态为"0"，可以解锁农庄
                                                            // 修改对应的名字、面积、肥力
                                                            farmItem[6] = farmName; // 第七个元素：农庄名称
                                                            farmItem[5] = farmArea.ToString(); // 第六个元素：农庄面积
                                                            farmItem[2] = fertilityString; // 第三个元素：肥力信息
                                                              
                                                            // 修改第一个元素为"-1"表示添加成功
                                                            farmItem[0] = "-1";
                                                              
                                                            statusMessage = string.Format(LanguageManager.Instance.GetText("已解锁农庄: {0} ({1}-{2})，面积: {3}"), farmName, LanguageManager.Instance.GetText(selectedPrefecture), LanguageManager.Instance.GetText(selectedCounty), farmArea);
                                                            Logger.LogInfo(statusMessage);
                                                            
                                                            // 设置场景更新标志
                                                            Mainload.isUpdateScene = true;
                                                        }
                                                          
                                                        // 找到后跳出所有循环
                                                        break;
                                                    }
                                                }
                                            }
                                            catch (Exception innerEx)
                                            {
                                                Logger.LogWarning($"处理第{i}个郡的第{j}个农庄时发生异常: {innerEx.Message}");
                                            }
                                            j++;
                                        }
                                         
                                        // 如果已经找到对应的农庄，则跳出外层循环
                                        if (found)
                                        {
                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.LogWarning($"处理第{i}个郡时发生异常: {ex.Message}");
                                    }
                                }
                                
                                // 已经在内部找到匹配后使用break跳出循环
                                
                                if (!found)
                                {
                                    statusMessage = string.Format(LanguageManager.Instance.GetText("添加失败：你所选择的位置并未解锁，请解锁{0}郡再添加"), LanguageManager.Instance.GetText(selectedPrefecture));
                                    Logger.LogWarning(statusMessage);
                                }
                            }
                            else
                            {
                                throw new Exception("Mainload.NongZ_now不存在");
                            }
                        }
                        catch (Exception ex)
                        {
                            statusMessage = LanguageManager.Instance.GetText("添加农庄失败：") + ex.Message;
                            Logger.LogError(statusMessage);
                            return;
                        }
                        
                        // 显示提示信息
                        ShowTipMessage(statusMessage);
                    } 
                    else if (mapSubMode == 2) 
                    { 
                        // 封地子模式 - 解锁封地
                        if (string.IsNullOrEmpty(selectedPrefecture))
                        { 
                            statusMessage = LanguageManager.Instance.GetText("请先选择要解锁的郡"); 
                            Logger.LogError("未选择郡"); 
                            return; 
                        } 
                        
                        // 获取郡索引
                        var junIndex = selectedJunIndex; 
                        if (junIndex < 0 || junIndex >= JunList.Length) 
                        { 
                            statusMessage = LanguageManager.Instance.GetText("无效的郡选择"); 
                            Logger.LogError("无效的郡索引: " + junIndex); 
                            return; 
                        } 
                        
                        // 记录当前郡索引和Mainload.Fengdi_now的长度，用于调试
                        Logger.LogInfo("选择的郡: " + JunList[junIndex] + "，索引: " + junIndex);
                        Logger.LogInfo("Mainload.Fengdi_now长度: " + (Mainload.Fengdi_now?.Count ?? -1));
                        
                        try 
                        { 
                            // 在解锁封地前检查Mainload.CityData_now中的势力名称
                            if (Mainload.CityData_now != null && junIndex < Mainload.CityData_now.Count) 
                            { 
                                // 获取郡城相关信息（第一个元素）
                                var junData = Mainload.CityData_now[junIndex] as IEnumerable; 
                                if (junData != null) 
                                { 
                                    // 获取第一个元素（郡城信息）
                                    var cityInfoObj = junData.Cast<object>().FirstOrDefault(); 
                                    if (cityInfoObj != null) 
                                    { 
                                        var cityInfo = cityInfoObj as List<string>; 
                                        if (cityInfo != null && cityInfo.Count > 0) 
                                        { 
                                            // 获取城池控制势力信息（第一个元素）
                                            var controlInfo = cityInfo[0]; 
                                            if (!string.IsNullOrEmpty(controlInfo)) 
                                            { 
                                                // 以|分割获取势力名称
                                                var parts = controlInfo.Split('|'); 
                                                if (parts.Length > 0 && parts[0] != "-1") 
                                                { 
                                                    // 势力名称不为-1，不解锁封地
                                                    statusMessage = string.Format(LanguageManager.Instance.GetText("{0}未解锁或郡城叛军未清剿，无法解锁封地"), selectedPrefecture); 
                                                    Logger.LogWarning(statusMessage); 
                                                     
                                                    // 显示提示信息
                                                    ShowTipMessage(statusMessage); 
                                                    return; 
                                                } 
                                            } 
                                        } 
                                    } 
                                } 
                            } 
                            
                            // 检查Mainload.Fengdi_now数组是否存在
                            if (Mainload.Fengdi_now != null && Mainload.Fengdi_now.Count > 0) 
                            { 
                                // 郡索引+1与封地索引对应
                                if (junIndex + 1 < Mainload.Fengdi_now.Count) 
                                {
                                    var fengDiData = Mainload.Fengdi_now[junIndex + 1];
                                    if (fengDiData != null && fengDiData.Count > 0)
                                    {
                                        // 检查封地是否已解锁 (第一个元素为0表示未解锁)
                                        if (fengDiData[0] == "1")
                                        {
                                            statusMessage = string.Format(LanguageManager.Instance.GetText("{0}的封地已解锁"), selectedPrefecture);
                                            Logger.LogInfo(statusMessage);
                                            return;
                                        }

                                        // 解锁封地
                                        fengDiData[0] = "1";
                                        statusMessage = string.Format(LanguageManager.Instance.GetText("{0}的封地已解锁"), selectedPrefecture);
                                        Logger.LogInfo(statusMessage);
                                    }  
                                }
                                else
                                {
                                    statusMessage = string.Format(LanguageManager.Instance.GetText("{0}的封地数据不存在或索引错误"), selectedPrefecture);
                                    Logger.LogWarning(statusMessage);
                                }
                            } 
                            else 
                            { 
                                throw new Exception("Mainload.Fengdi_now不存在或为空"); 
                            }
                        }
                        catch (Exception ex)
                        {
                            statusMessage = LanguageManager.Instance.GetText("解锁封地失败: ") + ex.Message;
                            Logger.LogError(statusMessage);
                            Logger.LogError("错误堆栈: " + ex.StackTrace);
                            return; 
                        } 
                        
                        // 显示提示信息
                        ShowTipMessage(statusMessage); 
                    } 
                    else if (mapSubMode == 3) 
                    { 
                        // 世家子模式 - 暂不实现，直接提示功能正在开发中
                        statusMessage = LanguageManager.Instance.GetText("添加失败：功能正在开发中");
                        Logger.LogWarning(statusMessage);
                        
                        // 显示提示信息
                        ShowTipMessage(statusMessage); 
                    }
                } 
                else if (currentMode == 0)
                {
                    // 货币模式
                    if (currencyValue <= 0)
                    {
                        statusMessage = LanguageManager.Instance.GetText("请输入有效的数值");
                        Logger.LogError("无效的数值: " + currencyValue);
                        return;
                    }
                    
                    if (selectedCurrencyType == 0)
                    {
                        // 添加铜钱
                        FormulaData.ChangeCoins(currencyValue);
                        statusMessage = string.Format(LanguageManager.Instance.GetText("已添加{0}铜钱"), currencyValue);
                        Logger.LogInfo(statusMessage);
                    }
                    else if (selectedCurrencyType == 1)
                    {
                        // 添加元宝
                        if (int.TryParse(Mainload.CGNum[1], out var currentYuanBao))
                        {
                            Mainload.CGNum[1] = (currentYuanBao + currencyValue).ToString();
                            statusMessage = string.Format(LanguageManager.Instance.GetText("已添加{0}元宝"), currencyValue);
                            Logger.LogInfo(statusMessage);
                        }
                        else
                        {
                            statusMessage = LanguageManager.Instance.GetText("获取当前元宝数量失败");
                            Logger.LogError("获取当前元宝数量失败");
                            return;
                        }
                    }
                    
                    // 显示提示信息
                    ShowTipMessage(statusMessage);
                }

            }
            catch (Exception ex)
            {
                statusMessage = LanguageManager.Instance.GetText("添加物品失败: ") + ex.Message;
                Logger.LogError("添加物品失败: " + ex.Message);
            }
        }
        
        // 添加物品到游戏物品栏
        private void AddItemToGameInventory(int itemId, int itemCount)
        {
            try
            {
                // 验证物品ID是否有效
                if (!itemList.ContainsKey(itemId))
                {
                    statusMessage = LanguageManager.Instance.GetText("无效的物品ID");
                    Logger.LogError(string.Format(LanguageManager.Instance.GetText("无效的物品ID: {0}"), itemId));
                    return;
                }
                
                // 检查数量是否有效
                if (itemCount <= 0)
                {
                    ShowItemCountZeroMessage(itemId);
                    return;
                }
                
                // 添加或更新物品
                UpdateOrAddItem(itemId, itemCount);
                
                // 显示成功添加的信息
                ShowItemAddedMessage(itemId, itemCount);
            }
            catch (Exception ex)
            {
                statusMessage = LanguageManager.Instance.GetText("添加物品时发生错误: ") + ex.Message;
                Logger.LogError(string.Format(LanguageManager.Instance.GetText("添加物品失败: {0}"), ex.Message));
            }
        }
        
        // 检查并更新或添加物品
        private void UpdateOrAddItem(int itemId, int itemCount)
        {
            var itemIdStr = itemId.ToString();
            
            // 查找物品是否已存在
            var existingItemIndex = FindExistingItemIndex(itemIdStr);
            
            if (existingItemIndex >= 0)
            {
                // 物品已存在，更新数量
                UpdateExistingItemQuantity(existingItemIndex, itemCount);
            }
            else
            {
                // 物品不存在，添加新物品
                AddNewItem(itemIdStr, itemCount);
            }
        }
        
        // 查找现有物品的索引
        private int FindExistingItemIndex(string itemIdStr)
        {
            for (var k = 0; k < Mainload.Prop_have.Count; k++)
            {
                if (Mainload.Prop_have[k][0] == itemIdStr)
                {
                    return k;
                }
            }
            return -1; // 未找到
        }
        
        // 更新现有物品的数量
        private void UpdateExistingItemQuantity(int itemIndex, int quantityToAdd)
        {
            var currentQuantity = int.Parse(Mainload.Prop_have[itemIndex][1]);
            Mainload.Prop_have[itemIndex][1] = (currentQuantity + quantityToAdd).ToString();
        }
        
        // 添加新物品
        private void AddNewItem(string itemIdStr, int quantity)
        {
            Mainload.Prop_have.Add(new List<string>
            {
                itemIdStr,
                quantity.ToString()
            });
        }
        
        // 显示物品数量为0的提示
        private void ShowItemCountZeroMessage(int itemId)
        {
            var itemName = itemList[itemId].Item1;
            var message = string.Format(LanguageManager.Instance.GetText("添加的【{0}】数量为0，添加失败"), itemName);
            ShowTipMessage(message);
            statusMessage = string.Format(LanguageManager.Instance.GetText("添加失败：物品【{0}】数量为0"), itemName);
            Logger.LogWarning(statusMessage);
        }
        
        // 显示物品添加成功的提示
        private void ShowItemAddedMessage(int itemId, int quantity)
        {
            var itemName = itemList[itemId].Item1;
            var message = string.Format(LanguageManager.Instance.GetText("已添加: {0} x {1}"), itemName, quantity);
            ShowTipMessage(message);
            statusMessage = string.Format(LanguageManager.Instance.GetText("已添加物品: {0} x {1}"), itemName, quantity);
            Logger.LogInfo(statusMessage);
        }
        
        // 添加话本到游戏
        private void AddScriptBookToGame(int bookId)
        {
            try
            {
                // 验证话本ID是否有效
                if (!bookList.ContainsKey(bookId))
                {
                    statusMessage = LanguageManager.Instance.GetText("无效的话本ID");
                    Logger.LogError(string.Format(LanguageManager.Instance.GetText("无效的话本ID: {0}"), bookId));
                    return;
                }
                
                // 检查话本是否已存在
                var bookExists = CheckIfBookExists(bookId);
                
                if (!bookExists)
                {
                    // 添加新话本
                    AddNewBookToGame(bookId);
                }
                else
                {
                    // 话本已存在，显示提示
                    ShowBookExistsMessage(bookId);
                }
            }
            catch (Exception ex)
            {
                statusMessage = LanguageManager.Instance.GetText("添加话本时发生错误: ") + ex.Message;
                Logger.LogError(string.Format(LanguageManager.Instance.GetText("添加话本失败: {0}"), ex.Message));
            }
        }
        
        // 检查话本是否已存在
        private bool CheckIfBookExists(int bookId)
        {
            if (Mainload.XiQuHave_Now == null || Mainload.XiQuHave_Now.Count <= 0)
                return false;
                
            return Mainload.XiQuHave_Now.Any(book => book[0] == bookId.ToString());
        }
        
        // 添加新话本到游戏
        private void AddNewBookToGame(int bookId)
        {
            // 游戏中Mainload.XiQuHave_Now的数据结构为：
            // [0]话本ID字符串 - 用于在Mainload.AllXiQu中查找对应话本
            // [1]位置信息字符串 - 使用竖线(|)分隔多个建筑位置ID
            // [2]默认值字符串 - 通常为"100"
            Mainload.XiQuHave_Now.Add(new List<string>
            {
                bookId.ToString(),  // 话本ID
                "",                 // 位置信息（留空）
                "100"               // 话本默认值
            });
            
            // 获取显示名称
            var displayName = LanguageManager.Instance.IsChineseLanguage() ? bookList[bookId][0] : bookList[bookId][1];
            
            // 显示添加成功提示
            var successMessage = string.Format(LanguageManager.Instance.GetText("已添加: {0}"), displayName);
            ShowTipMessage(successMessage);
            
            statusMessage = string.Format(LanguageManager.Instance.GetText("已添加话本: {0}"), displayName);
            Logger.LogInfo(statusMessage);
        }
        
        // 显示话本已存在的提示
        private void ShowBookExistsMessage(int bookId)
        {
            statusMessage = string.Format(LanguageManager.Instance.GetText("话本{0}已存在，不重复添加"), bookList[bookId][0]);
            Logger.LogInfo(statusMessage);
            
            // 获取显示名称
            var displayName = LanguageManager.Instance.IsChineseLanguage() ? bookList[bookId][0] : bookList[bookId][1];
            
            // 显示添加失败提示
            var errorMessage = string.Format(LanguageManager.Instance.GetText("添加失败: 话本{0}已存在"), displayName);
            ShowTipMessage(errorMessage);
        }
        
        // 检查指定位置是否存在状态为-1的农庄
        private bool CheckIfFarmExistsAtLocation(string locationKey)
        {
            try
            {
                if (Mainload.NongZ_now == null)
                    return false;
                
                // 遍历每个郡的所有农庄
                foreach (var junFarmListObj in Mainload.NongZ_now)
                {
                    var junFarmList = junFarmListObj as IEnumerable;
                    if (junFarmList == null)
                        continue;
                    
                    // 遍历当前郡中的每个农庄
                    foreach (var farmObj in junFarmList)
                    {
                        try
                        {
                            var farmItem = farmObj as List<string>;
                            if (farmItem != null && farmItem.Count > 0 && farmItem.Contains(locationKey))
                            {
                                // 检查农庄状态，只有状态为-1时才视为存在
                                if (farmItem[0] == "-1")
                                {
                                    Logger.LogWarning(string.Format(LanguageManager.Instance.GetText("发现已存在的农庄，位置: {0}，状态: {1}"), locationKey, farmItem[0]));
                                    return true;
                                }

                                Logger.LogWarning(string.Format(LanguageManager.Instance.GetText("发现状态为0的农庄，位置: {0}，允许添加府邸"), locationKey));
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(string.Format(LanguageManager.Instance.GetText("检查农庄时出错: {0}"), ex.Message));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format(LanguageManager.Instance.GetText("CheckIfFarmExistsAtLocation 方法出错: {0}"), ex.Message));
            }
            
            return false;
        }

        // 添加后不进入府邸
        private void Add_notEnterHouse()
        {
            //打开地图
            Mainload.isMapPanelOpen = true;

            //隐藏窗口
            showMenu = false;
            blockGameInput = false;
        }

        // 添加后进入府邸
        private void Add_EnterHouse()
        {
            //不打开地图
            Mainload.isMapPanelOpen = false;

            //进入新添加的府邸
            Mainload.SceneID = "M|" + (Mainload.Fudi_now.Count -1);
        }
        
        // 显示提示信息到游戏界面
        private void ShowTipMessage(string message)
        {
            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    Mainload.Tip_Show.Add(new List<string>
                    {
                        "1",
                        message
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format(LanguageManager.Instance.GetText("显示提示信息时出错: {0}"), ex.Message));
            }
        }
    }
}
