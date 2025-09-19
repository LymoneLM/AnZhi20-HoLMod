using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace cs.HoLMod.AddItem
{
    [BepInPlugin("cs.HoLMod.AddItem.AnZhi20", "HoLMod.AddItem", "2.4.0")]
    public class AddItem : BaseUnityPlugin
    {
        // 窗口设置
        private static Rect windowRect;
        private static bool showMenu = false;// 滚动位置
        private static Vector2 scrollPosition;
        private static bool blockGameInput = false;
        
        // 物品列表 - 添加分类信息用于快捷搜索
        private Dictionary<int, (string, string)> itemList = new Dictionary<int, (string, string)>()
        {
            // 特殊物品
            { 0, ("香烛", "特殊物品") },
            { 1, ("肥料", "特殊物品") },
            { 2, ("粮食", "特殊物品") },
            { 3, ("蔬菜", "特殊物品") },
            { 4, ("肉食", "特殊物品") },
            
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
            { 165, ("雷公藤", "毒药") },
            { 166, ("断肠草", "毒药") },
            { 167, ("鹤顶红", "毒药") },
            
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
            { 18, ("草布", "布料") },
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
            { 41, ("青雀石黛", "香粉") },
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
            { 54, ("耳铛(女)", "珠宝（女）") },
            { 55, ("步摇(女)", "珠宝（女）") },
            { 56, ("金簪(女)", "珠宝（女）") },
            { 57, ("华胜(女)", "珠宝（女）") },
            { 59, ("梳篦(女)", "珠宝（女）") },
            { 60, ("臂钏(女)", "珠宝（女）") },
            { 61, ("璎珞(女)", "珠宝（女）") },
            
            // 珠宝（男）
            { 62, ("发钗(男)", "珠宝（男）") },
            { 63, ("戒指(男)", "珠宝（男）") },
            { 64, ("玉佩(男)", "珠宝（男）") },
            { 65, ("带钩(男)", "珠宝（男）") },
            
            // 武器
            { 97, ("竹弓", "武器") },
            { 98, ("木弓", "武器") },
            { 99, ("牛角弓", "武器") },
            { 100, ("木角弓", "武器") },
            { 101, ("木弩", "武器") },
            { 102, ("铜弩", "武器") },
            { 103, ("连弩", "武器") },
            { 105, ("朴刀", "武器") },
            { 106, ("鬼头刀", "武器") },
            { 107, ("尖两刃刀", "武器") },
            { 108, ("三尺剑", "武器") },
            { 109, ("软剑", "武器") },
            { 110, ("七尺剑", "武器") },
            
            // 书法
            { 76, ("无名字轴", "书法") },
            { 77, ("无名碑帖", "书法") },
            { 78, ("名家字轴", "书法") },
            { 79, ("名家碑帖", "书法") },
            { 80, ("传世字轴", "书法") },
            { 81, ("传世碑帖", "书法") },
            
            // 丹青
            { 82, ("无名山水画", "丹青") },
            { 83, ("无名江山图", "丹青") },
            { 84, ("名家山水画", "丹青") },
            { 85, ("名家江山图", "丹青") },
            { 86, ("传世山水画", "丹青") },
            { 87, ("传世江山图", "丹青") },
            
            // 文玩
            { 88, ("玉圭", "文玩") },
            { 89, ("玉碗", "文玩") },
            { 90, ("玉瓶", "文玩") },
            { 91, ("玉珮", "文玩") },
            { 92, ("玉璧", "文玩") },
            { 93, ("玉璜", "文玩") },
            { 94, ("玉琮", "文玩") },
            { 95, ("玉山", "文玩") },
            { 96, ("玉璧", "文玩") },
            
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
            { 126, ("香盒", "香具") },
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
            { 137, ("曲红酒", "美酒") },
            { 138, ("菊花酒", "美酒") },
            { 139, ("茱萸酒", "美酒") },
            { 140, ("竹叶青", "美酒") },
            { 141, ("荔枝酒", "美酒") },
            { 142, ("桂花酒", "美酒") },
            { 143, ("琥珀酒", "美酒") },
            
            // 乐器
            { 144, ("竹箫", "乐器") },
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
            
            // 书籍 - 文
            { 169, ("《三字经》", "书籍 - 文") },
            { 170, ("《弟子规》", "书籍 - 文") },
            { 171, ("《千字文》", "书籍 - 文") },
            { 172, ("《蒙求》", "书籍 - 文") },
            { 173, ("《广韵》", "书籍 - 文") },
            { 174, ("《广雅》", "书籍 - 文") },
            { 175, ("《九章》", "书籍 - 文") },
            { 176, ("《尔雅》", "书籍 - 文") },
            { 177, ("《陵川集》", "书籍 - 文") },
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
            { 193, ("《棍棒棒式》", "书籍 - 武") },
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
            { 226, ("《羯鸡漫志》", "书籍 - 艺") },
            { 227, ("《琴谱类集》", "书籍 - 艺") },
            { 228, ("《梅花三弄》", "书籍 - 艺") },
            { 229, ("《猗兰操》", "书籍 - 艺") },
            { 230, ("《广陵散》", "书籍 - 艺") },
            { 231, ("《阳春白雪》", "书籍 - 艺") },
            { 232, ("《高山流水》", "书籍 - 艺") },
            { 233, ("《乐记》", "书籍 - 艺") },
            { 234, ("《乐经》", "书籍 - 艺") },
            { 235, ("《琴赋》", "书籍 - 艺") },
            { 236, ("《溪琴况》", "书籍 - 艺") },
            { 237, ("《太古遗音》", "书籍 - 艺") },
            
            // 书籍 - 计谋
            { 238, ("《左传》", "书籍 - 计谋") },
            { 239, ("《史记》", "书籍 - 计谋") },
            { 240, ("《间书》", "书籍 - 计谋") },
            { 241, ("《捭阖》", "书籍 - 计谋") },
            { 242, ("《飞箝》", "书籍 - 计谋") },
            { 243, ("《许和》", "书籍 - 计谋") },
            { 244, ("《揣篇》", "书籍 - 计谋") },
            { 245, ("《苏子》", "书籍 - 计谋") },
            { 246, ("《太白应经》", "书籍 - 计谋") },
            { 247, ("《战国策》", "书籍 - 计谋") },
            { 248, ("《韩非子》", "书籍 - 计谋") },
            { 249, ("《孙子兵法》", "书籍 - 计谋") },
            { 250, ("《鬼谷子》", "书籍 - 计谋") },
            
            // 书籍 - 技能 - 巫
            { 251, ("《山海经》", "书籍 - 技能 - 巫") },
            { 252, ("《符咒秘本》", "书籍 - 技能 - 巫") },
            { 253, ("《巫阳古书》", "书籍 - 技能 - 巫") },
            { 254, ("《阴山法笈》", "书籍 - 技能 - 巫") },
            { 255, ("《鲁班书》", "书籍 - 技能 - 巫") },
            
            // 书籍 - 技能 - 医
            { 256, ("《脉经》", "书籍 - 技能 - 医") },
            { 260, ("《精要奥略》", "书籍 - 技能 - 医") },
            { 261, ("《神农本草》", "书籍 - 技能 - 医") },
            { 262, ("《本草纲目》", "书籍 - 技能 - 医") },
            { 263, ("《黄帝内经》", "书籍 - 技能 - 医") },
            
            // 书籍 - 技能 - 相
            { 264, ("《得器歌》", "书籍 - 技能 - 相") },
            { 265, ("《五官杂论》", "书籍 - 技能 - 相") },
            { 266, ("《听声相形》", "书籍 - 技能 - 相") },
            { 267, ("《永乐百问》", "书籍 - 技能 - 相") },
            { 268, ("《柳庄相法》", "书籍 - 技能 - 相") },
            
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
        private int selectedCurrencyType = 0;
        private int currencyValue = 100000; // 默认10万
        
        // 判断当前是否为中文语言环境
        private bool IsChineseLanguage()
        {
            // 检查系统语言是否为中文
            string systemLanguage = Application.systemLanguage.ToString();
            return systemLanguage.Contains("Chinese");
        }
        
        // 双语话本列表 - 结构：ID, [中文名称, 英文名称, 中文描述, 英文描述]
        private Dictionary<int, string[]> bookList = new Dictionary<int, string[]>
        {
            { 1, new string[] { "《女驸马》", "The Female Royal Son-in-Law", "冯素贞为救父，女扮男装，考中状元，招为驸马。公主疑之，终揭真相。帝感其孝，赦其罪，赐婚状元。", "Feng Suzhen disguises as a man to save her father, tops the imperial exam, and is made royal son-in-law. The princess suspects, uncovers the truth. The emperor, moved by her filial piety, pardons her and arranges her marriage to the top scholar." } },
            { 2, new string[] { "《牡丹亭》", "The Peony Pavilion", "杜丽娘梦遇柳梦梅，醒后相思成疾，逝去。柳梦梅得画，感其情，丽娘复活，二人终成眷属。", "Du Liniang dreams of Liu Mengmei, falls lovesick, and dies. Liu finds her portrait, moved by her love, Liniang revives, and they unite." } },
            { 3, new string[] { "《紫钗记》", "The Purple Hairpin", "李益与霍小玉相爱，以紫钗为信。李益赴考，霍母逼小玉改嫁。小玉病重，李益归，见钗如见人，终得团圆。", "Li Yi and Huo Xiaoyu fall in love, pledge with a purple hairpin. Li leaves for exams, Huo's mother forces her to remarry. Xiaoyu falls ill, Li returns, sees the pin as seeing her, and they reunite." } },
            { 4, new string[] { "《孔雀东南飞》", "The Peacocks Fly Southeast", "刘兰芝与焦仲卿相爱，兰芝被逼改嫁，投河自尽。仲卿闻讯，亦自缢。两家悔之，合葬二人，孔雀飞至其墓", "Liu Lanzhi and Jiao Zhongqing love each other; Lanzhi is forced to remarry and drowns herself. Zhongqing hangs himself upon hearing the news. Both families regret, bury them together, and peacocks fly to their grave." } },
            { 5, new string[] { "《焚香记》", "The Incense Burning", "王魁负心，桂英焚香告天。魁受天谴，病重。桂英感其悔，复与和好，魁病愈。", "Wang Kui betrays Guiying, who burns incense to heaven. Kui is cursed, falls ill. Guiying, moved by his remorse, reconciles, and Kui recovers." } },
            { 6, new string[] { "《琵琶记》", "The Tale of the Pipa", "赵五娘寻夫蔡伯喈，弹琵琶诉苦。伯喈闻声，终得相认。夫妻团圆，孝感动天。", "Zhao Wuniang searches for her husband Cai Bojie, playing the pipa to express her sorrow. Bojie hears it, they reunite. Their filial piety moves heaven." } },
            { 7, new string[] { "《望江亭》", "The Riverside Pavilion", "谭记儿为救夫，假扮渔妇，智斗杨衙内。于望江亭设计，终救夫出狱，夫妻团圆。", "Tan Ji'er disguises as a fisherwoman to save her husband, outwits Officer Yang. At Wangjiang Pavilion, she schemes to free him, and they reunite." } },
            { 8, new string[] { "《锁麟囊》", "The Precious Pouch", "薛湘灵出嫁，途中遇雨，避于春秋亭。贫女赵守贞亦至，湘灵赠锁麟囊。后湘灵落难，守贞报恩，二人结为姐妹。", "Xue Xiangling, on her wedding journey, shelters from rain at Chunqiu Pavilion. Poor girl Zhao Shouzhen arrives, Xiangling gifts her a precious pouch. Later, Xiangling falls into hardship, Shouzhen repays the kindness, and they become sisters." } },
            { 9, new string[] { "《碧玉簪》", "The Jade Hairpin", "王玉林与李秀英相爱，秀英赠碧玉簪为信。后玉林误会秀英，秀英含冤而死。玉林悔悟，终得昭雪。", "Wang Yulin and Li Xiuying fall in love, Xiuying gifts a jade hairpin as a pledge. Yulin misunderstands, Xiuying dies unjustly. Yulin repents, and her name is cleared." } },
            { 10, new string[] { "《罗帕记》", "The Silk Handkerchief", "王科举与妻陈赛金恩爱，赛金遗失罗帕，被姜雄拾得。姜雄诬陷赛金，科举误会。后真相大白，夫妻和好。", "Wang Keju and his wife Chen Saijin are loving; Saijin loses a silk handkerchief, picked up by Jiang Xiong. Jiang frames Saijin, Keju misunderstands. Truth revealed, they reconcile." } },
            { 11, new string[] { "《梁祝》", "The Butterfly Lovers", "梁山伯与祝英台同窗，英台女扮男装。山伯不知，情愫暗生。英台归家，山伯访之，知真相，悲恸而亡。英台殉情，二人化蝶。", "Liang Shanbo and Zhu Yingtai study together, Yingtai disguised as a man. Shanbo, unaware, falls in love. Yingtai returns home, Shanbo visits, learns the truth, dies of grief. Yingtai follows in death, they transform into butterflies." } },
            { 12, new string[] { "《花木兰》", "Hua Mulan", "木兰代父从军，女扮男装，英勇善战。十二年征战后，归家复女装，众人惊其勇，帝赐厚赏。", "Mulan joins the army in her father's place, disguised as a man, fights bravely. After twelve years, returns home, reveals her identity, astonishes all, and the emperor rewards her richly." } },
            { 13, new string[] { "《赵氏孤儿》", "The Orphan of Zhao", "屠岸贾灭赵氏，程婴救赵氏孤儿，隐姓埋名。孤儿长成，程婴告知身世，孤儿报仇雪恨，赵氏复兴。", "Tu'an Gu exterminates the Zhao clan; Cheng Ying saves the orphan, hides his identity. The orphan grows up, Cheng reveals his heritage, the orphan avenges, and the Zhao clan is restored." } },
            { 14, new string[] { "《谢小娥传》", "The Tale of Xie Xiao'e", "谢小娥父为盗所害，小娥立志复仇。女扮男装，寻得仇人，智勇双全，终报父仇，众人称颂。", "Xie Xiao'e's father is killed by bandits; she vows revenge. Disguised as a man, she finds the culprits, uses wit and courage, avenges her father, and is praised by all." } },
            { 15, new string[] { "《金鞭记》", "The Golden Whip", "呼延赞之子呼延丕显被奸臣庞文陷害，全家满门三百余口被杀，呼延丕显之子呼延守勇、呼延守信与奸臣斗争之事。", "Huyan Pixian, son of Huyan Zan, is framed by the treacherous minister Pang Wen, and his family of over 300 people are killed. Huyan Pixian's sons, Huyan Shouyong and Huyan Shouxin, fight against the treacherous minister." } },
            { 16, new string[] { "《文昭关》", "Wu Zixu at Zhaoguan", "伍子胥过昭关，一夜白头。为报父仇，忍辱负重，终得吴王重用，率兵伐楚，报仇雪恨。", "Wu Zixu passes Zhaoguan, turns white overnight. To avenge his father, endures humiliation, gains King Wu's trust, leads troops against Chu, and avenges his father's death." } },
            { 17, new string[] { "《李离伏剑》", "Li Li Draws His Sword", "李离执掌晋国刑罚，因误听下属不实之辞而错杀了人，最终拔剑自刎。", "Li Li, in charge of the penal system in Jin, mistakenly executed an innocent man due to false information from his subordinates, and ultimately drew his sword and committed suicide." } },
            { 18, new string[] { "《双婿案》", "The Two Sons-in-Law", "举人杨玉春，因家境不幸，前往燕山王府投亲，途中救出遭遇强盗抢劫的公子方天觉，两人义结金兰。", "Yang Yuchun, due to his unfortunate family circumstances, goes to the Prince of Yanshan's residence to seek refuge. On his way, he rescues Fang Tianjue, a young master who was robbed by bandits, and the two become sworn brothers." } },
            { 0, new string[] { "《天仙配》", "The Heavenly Match", "董永孝感动天，七仙女慕其德行，下凡结缘。王母怒，迫其归天。二人情深，誓不分离，终得团圆。", "Dong Yong's filial piety moves heaven; the Seventh Fairy, admiring his virtue, descends to marry. The Queen Mother, enraged, forces her return. Deeply in love, they vow never to part and ultimately reunite." } },
        };

         // 界面模式控制（0：货币，1：物品，2：话本）
        private int currentMode = 1; // 默认显示物品模式
        
        // 界面控制变量
        private int selectedItemId = 0;
        private int count = 1;
        private string searchText = "";
        private string countInput = "1";
        private string statusMessage = "准备就绪";
        private List<int> filteredItemIds = new List<int>();
        // 用于组合搜索的分类过滤器
        private string selectedCategory = "";

        // 当前插件版本
        private const string CURRENT_VERSION = "2.4.0";
        
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
            int currentWidth = Screen.width;
            int currentHeight = Screen.height;
            
            // 默认窗口大小
            float defaultWidth = 1000f;
            float defaultHeight = 1200f;
            
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
            
            // 配置文件路径
            string configFilePath = Path.Combine(Paths.ConfigPath, "cs.HoLMod.AddItem.AnZhi20.cfg");
            
            // 获取配置文件中的版本信息（如果存在）
            string loadedVersion = "";
            if (File.Exists(configFilePath))
            {
                try
                {
                    foreach (string line in File.ReadAllLines(configFilePath))
                    {
                        if (line.Trim().StartsWith("已加载版本 = "))
                        {
                            loadedVersion = line.Substring("已加载版本 = ".Length).Trim('"');
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("读取配置文件版本信息时出错: " + ex.Message);
                }
            }
            
            // 检查是否需要更新配置
            bool isVersionUpdated = loadedVersion != CURRENT_VERSION;
            
            // 如果版本更新，删除配置文件
            if (isVersionUpdated)
            {
                Logger.LogInfo($"检测到插件版本更新至 {CURRENT_VERSION}，正在删除旧配置文件...");
                
                try
                {
                    if (File.Exists(configFilePath))
                    {
                        File.Delete(configFilePath);
                        Logger.LogInfo("旧配置文件已删除。");
                    }
                    
                    // 只保存版本信息，不保存窗口大小配置
                    Config.Bind("内部配置", "已加载版本", CURRENT_VERSION, "用于跟踪插件版本，请勿手动修改");
                    
                    // 保存新的配置文件
                    Config.Save();
                    
                    Logger.LogInfo("配置文件已成功重新生成。");
                }
                catch (Exception ex)
                {
                    Logger.LogError("重新生成配置文件时出错: " + ex.Message);
                }
            }
            
            // 初始化分辨率设置
            UpdateResolutionSettings();
            
            // 加载游戏物品到字典
            LoadGameItemsToDictionary.LoadItems(itemList);
            
            // 初始化筛选后的物品列表
            filteredItemIds = itemList.Keys.ToList();
        }
        
        private void Update()
        {
            // 按F2键切换窗口显示
            if (UnityEngine.Input.GetKeyDown(KeyCode.F2))
            {
                // 按下F2时更新分辨率设置
                UpdateResolutionSettings();
                showMenu = !showMenu;
                blockGameInput = showMenu;
                Logger.LogInfo(showMenu ? "物品添加器窗口已打开" : "物品添加器窗口已关闭");
            }
            
            // 阻止游戏输入当窗口显示时（游戏会继续运行，但不允许操作游戏界面）
            if (blockGameInput)
            {
                // 阻止鼠标滚轮
                if (UnityEngine.Input.mouseScrollDelta.y != 0)
                {
                    UnityEngine.Input.ResetInputAxes();
                }
                
                // 阻止鼠标点击
                if (UnityEngine.Input.GetMouseButton(0) || UnityEngine.Input.GetMouseButton(1) || UnityEngine.Input.GetMouseButton(2))
                {
                    UnityEngine.Input.ResetInputAxes();
                }
                
                // 阻止键盘输入（保留F2键用于关闭窗口）
                if (UnityEngine.Input.anyKeyDown && !UnityEngine.Input.GetKeyDown(KeyCode.F2))
                {
                    UnityEngine.Input.ResetInputAxes();
                }
            }
        }
        
        private void OnGUI()
        {
            if (!showMenu)
                return;
            
            // 保存窗口背景色并设置为半透明
            Color originalBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f, 0.95f);
            
            // 显示一个半透明的背景遮罩，防止操作游戏界面
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
            GUI.color = new Color(0, 0, 0, 0.1f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.EndGroup();
            
            // 应用缩放因子
            Matrix4x4 guiMatrix = GUI.matrix;
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
            windowRect = GUI.Window(0, windowRect, DrawWindow, "", UnityEngine.GUI.skin.window);
            
            // 恢复原始矩阵
            GUI.matrix = guiMatrix;
            
            // 恢复原始背景色
            GUI.backgroundColor = originalBackgroundColor;
        }
        
        private void DrawWindow(int windowID)
        {
            // 设置统一的字体大小
            // 根据scaleFactor调整字体大小
            int fontSize = Mathf.RoundToInt(18 * scaleFactor);
            GUI.skin.label.fontSize = fontSize;
            GUI.skin.button.fontSize = fontSize;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter; 
            GUI.skin.toggle.fontSize = fontSize; // 设置选中项字体大小与其他元素一致
            GUI.skin.toggle.alignment = TextAnchor.MiddleCenter; 
            GUI.skin.textField.fontSize = fontSize;
            GUI.skin.window.fontSize = fontSize;
            
            // 根据scaleFactor调整窗口最小宽度
            windowRect.width = Mathf.Max(windowRect.width, 800f * scaleFactor);
            
            GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) });
            
            // 窗口最上方标题文本
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            GUILayout.FlexibleSpace();
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            GUILayout.Label("物品添加器", titleStyle, new GUILayoutOption[] { GUILayout.ExpandWidth(false) });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15f * scaleFactor);
            
            // 模式选择按钮（货币、物品、话本）
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            if (GUILayout.Button("货币", new GUILayoutOption[] { GUILayout.ExpandWidth(true) }))
            {
                currentMode = 0;
            }
            if (GUILayout.Button("物品", new GUILayoutOption[] { GUILayout.ExpandWidth(true) }))
            {
                currentMode = 1;
            }
            if (GUILayout.Button("话本", new GUILayoutOption[] { GUILayout.ExpandWidth(true) }))
            {
                currentMode = 2;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10f * scaleFactor);
            
            // 搜索文本框，支持部分搜索
            GUILayout.BeginHorizontal();
            GUILayout.Label(currentMode == 1 ? "搜索物品:" : "搜索:", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) });
            string newSearchText = GUILayout.TextField(searchText, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
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
                GUILayout.Label("分类:", new GUILayoutOption[] { GUILayout.Width(100f * scaleFactor) });
                if (GUILayout.Button("清空", new GUILayoutOption[] { GUILayout.Width(120f * scaleFactor) }))
                {
                    searchText = "";
                    selectedCategory = "";
                    FilterItems();
                }
                GUILayout.EndHorizontal();
                
                // 分类按钮布局 - 分多行显示
                // 特殊物品和新增物品按钮在同一行
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("特殊物品", new GUILayoutOption[] { GUILayout.Width(245f * scaleFactor) })) { SearchByCategory("特殊物品"); }
                GUILayout.Space(10f * scaleFactor);
                if (GUILayout.Button("新增物品", new GUILayoutOption[] { GUILayout.Width(245f * scaleFactor) })) { SearchByCategory("新增物品"); }
                GUILayout.EndHorizontal();
                
                // 第一行：丹药、符咒、毒药、美食、农产
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("丹药", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("丹药"); }
                if (GUILayout.Button("符咒", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("符咒"); }
                if (GUILayout.Button("毒药", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("毒药"); }
                if (GUILayout.Button("美食", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("美食"); }
                if (GUILayout.Button("农产", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("农产"); }
                GUILayout.EndHorizontal();
                
                // 第二行：布料、矿产、香粉、珠宝、武器
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("布料", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("布料"); }
                if (GUILayout.Button("矿产", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("矿产"); }
                if (GUILayout.Button("香粉", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("香粉"); }
                if (GUILayout.Button("珠宝", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("珠宝"); }
                if (GUILayout.Button("武器", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("武器"); }
                GUILayout.EndHorizontal();
                
                // 第三行：书法、丹青、文玩、乐器、茶具
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("书法", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("书法"); }
                if (GUILayout.Button("丹青", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("丹青"); }
                if (GUILayout.Button("文玩", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("文玩"); }
                if (GUILayout.Button("乐器", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("乐器"); }
                if (GUILayout.Button("茶具", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("茶具"); }
                GUILayout.EndHorizontal();
                
                // 第四行：香具、瓷器、美酒、皮毛、书籍
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("香具", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("香具"); }
                if (GUILayout.Button("瓷器", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("瓷器"); }
                if (GUILayout.Button("美酒", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("美酒"); }
                if (GUILayout.Button("皮毛", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("皮毛"); }
                if (GUILayout.Button("书籍", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) })) { SearchByCategory("书籍"); }
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
                GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(300f * scaleFactor) });
                
                // 货币类型选择
                GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
                GUILayout.Label("选择货币类型:", new GUILayoutOption[] { GUILayout.Width(200f * scaleFactor) });
                if (GUILayout.Button("铜钱", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) }))
                {
                    selectedCurrencyType = 0; // 0表示铜钱
                }
                if (GUILayout.Button("元宝", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) }))
                {
                    selectedCurrencyType = 1; // 1表示元宝
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10f * scaleFactor);
                
                // 数值输入
                GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
                GUILayout.Label("数值:", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) });
                string currencyValueInput = GUILayout.TextField(currencyValue.ToString(), new GUILayoutOption[] { GUILayout.Width(200f * scaleFactor) });
                if (int.TryParse(currencyValueInput, out int newCurrencyValue))
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
                GUILayout.Label(selectedCurrencyType == 0 ? "(0-10亿)" : "(0-10万)", new GUILayoutOption[] { GUILayout.Width(140f * scaleFactor) });
                GUILayout.EndHorizontal();
                
                // 预设数值按钮
                GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
                if (selectedCurrencyType == 0) // 铜钱
                {
                    if (GUILayout.Button("100万", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) }))
                    {
                        currencyValue = 1000000;
                    }
                    if (GUILayout.Button("1亿", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) }))
                    {
                        currencyValue = 100000000;
                    }
                    if (GUILayout.Button("10亿", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) }))
                    {
                        currencyValue = 1000000000;
                    }
                }
                else if (selectedCurrencyType == 1) // 元宝
                {
                    if (GUILayout.Button("1百", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) }))
                    {
                        currencyValue = 100;
                    }
                    if (GUILayout.Button("1千", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) }))
                    {
                        currencyValue = 1000;
                    }
                    if (GUILayout.Button("1万", new GUILayoutOption[] { GUILayout.Width(160f * scaleFactor) }))
                    {
                        currencyValue = 10000;
                    }
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10f * scaleFactor);
                
                // 当前货币状态显示
                GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
                GUILayout.Label(string.Format("当前铜钱: {0}", FormulaData.GetCoinsNum()), new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
                GUILayout.Label("当前元宝: " + Mainload.CGNum[1], new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
                GUILayout.EndVertical();
                
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            } else if (currentMode == 1)
            {
                // 物品模式
                GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(300f * scaleFactor) });
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) });
                
                // 创建物品列表
                int selectedIndex = filteredItemIds.IndexOf(selectedItemId);
                if (selectedIndex == -1 && filteredItemIds.Count > 0)
                {
                    selectedIndex = 0;
                    selectedItemId = filteredItemIds[0];
                }
                
                if (filteredItemIds.Count > 0)
                {
                    for (int i = 0; i < filteredItemIds.Count; i++)
                    {
                        bool isSelected = (i == selectedIndex);
                        GUIStyle buttonStyle = isSelected ? GUI.skin.toggle : GUI.skin.button;
                        
                        if (GUILayout.Button(itemList[filteredItemIds[i]].Item1, buttonStyle, new GUILayoutOption[] { GUILayout.ExpandWidth(true) }))
                        {
                            selectedItemId = filteredItemIds[i];
                        }
                    }
                }
                else
                {
                    GUILayout.Label("未找到匹配的物品", new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
                }
                
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            } else if (currentMode == 2)
            {
                // 话本模式
                GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(300f * scaleFactor) });
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) });
                
                // 创建话本列表，根据语言显示对应文本
                foreach (var book in bookList)
                {
                    string displayName = IsChineseLanguage() ? book.Value[0] : book.Value[1];
                    if (GUILayout.Button(displayName, new GUILayoutOption[] { GUILayout.ExpandWidth(true) }))
                    {
                        selectedItemId = book.Key;
                    }
                }
                
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            
            GUILayout.Space(10f * scaleFactor);
            
            // 选择框、数量输入框、添加按钮
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            
            GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            
            // 仅在物品模式下显示数量输入框
            if (currentMode == 1)
            {
                // 数量输入
                GUILayout.BeginHorizontal();
                GUILayout.Label("数量:", new GUILayoutOption[] { GUILayout.Width(100f * scaleFactor) });
                string newCountInput = GUILayout.TextField(countInput, new GUILayoutOption[] { GUILayout.Width(200f * scaleFactor) });
                if (newCountInput != countInput)
                {
                    countInput = newCountInput;
                    if (int.TryParse(countInput, out int newCount))
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
            if (GUILayout.Button(currentMode == 1 ? "添加物品" : "添加", new GUILayoutOption[] { GUILayout.Width(180f * scaleFactor), GUILayout.Height(80f * scaleFactor) }))
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
            GUILayout.Label("使用说明:", UnityEngine.GUI.skin.box);
            
            // 使用说明
            GUILayout.Label("1. 请在点击添加前先保存游戏，以便回档");
            GUILayout.Label("2. 按F2键显示/隐藏窗口");
            GUILayout.Label("3. 切换模式选择：物品模式/货币模式/话本模式");
            GUILayout.Label("4. 输入部分字符可搜索物品或话本");
            GUILayout.Label("5. 选择项目并选择或输入数量后点击添加按钮");
            GUILayout.Label("");
            
            // MOD作者及版本号说明
            GUILayout.Label("Mod作者：AnZhi20");
            GUILayout.Label("Mod版本：2.4.0");
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
                string lowerSearchText = searchText.ToLower();
                bool hasSearchText = !string.IsNullOrEmpty(searchText);
                bool hasSelectedCategory = !string.IsNullOrEmpty(selectedCategory);
                
                if (!hasSearchText && !hasSelectedCategory)
                {
                    // 无过滤条件，显示所有物品
                    filteredItemIds = itemList.Keys.ToList();
                }
                else
                {
                    filteredItemIds = itemList.Where(kv => 
                    {
                        bool matchesSearchText = !hasSearchText || 
                            kv.Value.Item1.ToLower().Contains(lowerSearchText) || 
                            kv.Value.Item2.ToLower().Contains(lowerSearchText);
                        
                        bool matchesCategory = !hasSelectedCategory || 
                            kv.Value.Item2.ToLower().Contains(selectedCategory.ToLower());
                        
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
                if (count <= 0 || count > 1000000)
                {
                    statusMessage = "无效的数量，请输入1-1000000范围内的整数";
                    Logger.LogError("无效的数量: " + count);
                    return;
                }
                
                if (currentMode == 1)
                {
                    // 物品模式
                    if (!itemList.ContainsKey(selectedItemId))
                    {
                        statusMessage = "无效的物品ID";
                        Logger.LogError("无效的物品ID: " + selectedItemId);
                        return;
                    }
                    
                    // 添加物品到游戏
                    bool itemExists = false;
                    for (int k = 0; k < Mainload.Prop_have.Count; k++)
                    {
                        if (selectedItemId.ToString() == Mainload.Prop_have[k][0])
                        {
                            // 如果物品已存在，则增加数量
                            Mainload.Prop_have[k][1] = (int.Parse(Mainload.Prop_have[k][1]) + count).ToString();
                            itemExists = true;
                            break;
                        }
                    }
                    
                    if (!itemExists)
                    {
                        // 如果物品不存在，则添加新物品
                        Mainload.Prop_have.Add(new List<string>
                        {
                            selectedItemId.ToString(),
                            count.ToString()
                        });
                    }

                    // 物品添加提示
                    if (count == 0)
                    {
                        Mainload.Tip_Show.Add(new List<string>
                        {
                            "1",
                            string.Format("添加的【{0}】数量为0，添加失败", itemList[selectedItemId].Item1)
                        });
                        statusMessage = string.Format("添加失败：物品【{0}】数量为0", itemList[selectedItemId].Item1);
                        Logger.LogWarning(statusMessage);
                    }
                    else
                    {
                        // 数量有效时添加显示信息
                        Mainload.Tip_Show.Add(new List<string>
                        {
                            "1",
                            string.Format("已添加: {0} x {1}", itemList[selectedItemId].Item1, count)
                        });
                        statusMessage = string.Format("已添加物品: {0} x {1}", itemList[selectedItemId].Item1, count);
                        Logger.LogInfo(statusMessage);
                    }
                }
                else if (currentMode == 2)
                {
                    // 话本模式
                    if (!bookList.ContainsKey(selectedItemId))
                    {
                        statusMessage = "无效的话本ID";
                        Logger.LogError("无效的话本ID: " + selectedItemId);
                        return;
                    }
                    
                    try
                        {
                            // 检查话本是否已存在
                            bool bookExists = false;
                            int j = 0;
                            while (j < Mainload.XiQuHave_Now.Count)
                            {
                                if (selectedItemId.ToString() == Mainload.XiQuHave_Now[j][0])
                                {
                                    // 如果话本已存在
                                    bookExists = true;
                                    Logger.LogInfo(string.Format("话本{0}位置信息: {1}", selectedItemId, Mainload.XiQuHave_Now[j][1]));
                                    break;
                                }
                                else
                                {
                                    j++;
                                }
                            }
                            
                            if (!bookExists)
                            {
                                // 游戏中Mainload.XiQuHave_Now的数据结构为：
                                // [0]话本ID字符串 - 用于在Mainload.AllXiQu中查找对应话本
                                // [1]位置信息字符串 - 使用竖线(|)分隔多个建筑位置ID
                                // [2]默认值字符串 - 通常为"100"
                                Mainload.XiQuHave_Now.Add(new List<string>
                                {
                                    selectedItemId.ToString(),    // 话本ID，将在Mainload.AllXiQu中使用
                                    "",  //
                                    "100"                        // 话本默认值，游戏中会被正确解析
                                });

                            // 根据语言显示对应文本
                            string displayName = IsChineseLanguage() ? bookList[selectedItemId][0] : bookList[selectedItemId][1];
                            
                            // 添加显示信息
                            Mainload.Tip_Show.Add(new List<string>
                            {
                                "1",
                                string.Format("已添加: {0}", displayName)
                            });
                            statusMessage = string.Format("已添加话本: {0}", displayName);
                            Logger.LogInfo(statusMessage);
                        }
                        else
                        {
                            // 话本已存在时的提示
                            statusMessage = string.Format("话本{0}已存在，不重复添加", bookList[selectedItemId][0]);
                            Logger.LogInfo(statusMessage);
                            
                            // 根据语言显示对应文本
                                string displayName = IsChineseLanguage() ? bookList[selectedItemId][0] : bookList[selectedItemId][1];
                                
                                // 添加失败提示
                                Mainload.Tip_Show.Add(new List<string>
                                {
                                    "1",
                                    string.Format(IsChineseLanguage() ? "添加失败: 话本{0}已存在" : "Failed to add: Book {0} already exists", displayName)
                                });
                        }
                    }
                    catch (Exception ex)
                    {
                        statusMessage = "添加话本时发生错误: " + ex.Message;
                        Logger.LogError("添加话本失败: " + ex.Message);
                    }
                } 
                else if (currentMode == 0)
                {
                    // 货币模式
                    if (currencyValue <= 0)
                    {
                        statusMessage = "请输入有效的数值";
                        Logger.LogError("无效的数值: " + currencyValue);
                        return;
                    }
                    
                    if (selectedCurrencyType == 0)
                    {
                        // 添加铜钱
                        FormulaData.ChangeCoins(currencyValue);
                        statusMessage = string.Format("已添加{0}铜钱", currencyValue);
                        Logger.LogInfo(statusMessage);
                    }
                    else if (selectedCurrencyType == 1)
                    {
                        // 添加元宝
                        if (int.TryParse(Mainload.CGNum[1], out int currentYuanBao))
                        {
                            Mainload.CGNum[1] = (currentYuanBao + currencyValue).ToString();
                            statusMessage = string.Format("已添加{0}元宝", currencyValue);
                            Logger.LogInfo(statusMessage);
                        }
                        else
                        {
                            statusMessage = "获取当前元宝数量失败";
                            Logger.LogError("获取当前元宝数量失败");
                            return;
                        }
                    }
                    
                    // 添加显示信息
                    Mainload.Tip_Show.Add(new List<string>
                    {
                        "1",
                        statusMessage
                    });
                }

            }
            catch (Exception ex)
            {
                statusMessage = "添加物品失败: " + ex.Message;
                Logger.LogError("添加物品失败: " + ex.Message);
            }
        }
    }
}
