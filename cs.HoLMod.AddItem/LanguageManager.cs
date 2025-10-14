using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using UnityEngine;

namespace cs.HoLMod.AddItem
{
    internal class LanguageManager
    {
        // 单例模式
        private static readonly LanguageManager instance = new LanguageManager();
        private string currentLanguageCode = "zh-CN"; // 默认中文
        private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>();

        // 私有构造函数
        private LanguageManager()
        {
            // 初始化语言翻译字典
            InitializeTranslations();
        }

        // 获取单例实例
        public static LanguageManager Instance
        {
            get { return instance; }
        }

        // 初始化翻译
        private void InitializeTranslations()
        {
            // 中文翻译
            translations["zh-CN"] = new Dictionary<string, string>
            {
                // 主界面相关
                {"物品添加器", "物品添加器"},
                {"货币", "货币"},
                {"物品", "物品"},
                {"话本", "话本"},
                {"地图", "地图"},
                {"搜索物品:", "搜索物品:"},
                {"搜索:", "搜索:"},
                {"分类:", "分类:"},
                {"清空", "清空"},
                {"添加物品", "添加物品"},
                {"添加", "添加"},
                {"使用说明:", "使用说明:"},
                {"1. 请在点击添加前先保存游戏，以便回档", "1. 请在点击添加前先保存游戏，以便回档"},
                {"2. 按F2键显示/隐藏窗口", "2. 按F2键显示/隐藏窗口。"},
                {"3. 切换模式选择：物品模式/货币模式/话本模式/地图模式", "3. 切换模式选择：物品模式/货币模式/话本模式/地图模式。"},
                {"4. 输入部分字符可搜索物品或话本", "4. 输入部分字符可搜索物品或话本。"},
                {"5. 选择项目并选择或输入数量后点击添加按钮", "5. 选择项目并选择或输入数量后点击添加按钮。"},
                {"Mod作者：AnZhi20", "Mod作者：AnZhi20"},
                {"Mod版本：", "Mod版本："},
                
                // 物品分类相关
                {"特殊物品", "特殊物品"},
                {"新增物品", "新增物品"},
                {"丹药", "丹药"},
                {"符咒", "符咒"},
                {"毒药", "毒药"},
                {"美食", "美食"},
                {"农产", "农产"},
                {"布料", "布料"},
                {"矿产", "矿产"},
                {"香粉", "香粉"},
                {"珠宝", "珠宝"},
                {"武器", "武器"},
                {"书法", "书法"},
                {"丹青", "丹青"},
                {"文玩", "文玩"},
                {"乐器", "乐器"},
                {"茶具", "茶具"},
                {"香具", "香具"},
                {"瓷器", "瓷器"},
                {"美酒", "美酒"},
                {"皮毛", "皮毛"},
                {"书籍", "书籍"},
                
                // 货币模式相关
                {"选择货币类型:", "选择货币类型:"},
                {"铜钱", "铜钱"},
                {"元宝", "元宝"},
                {"数值:", "数值:"},
                {"(0-10亿)", "(0-10亿)"},
                {"(0-10万)", "(0-10万)"},
                {"10万", "10万"},
                {"100万", "100万"},
                {"1000万", "1000万"},
                {"1亿", "1亿"},
                {"10亿", "10亿"},
                {"1百", "1百"},
                {"1千", "1千"},
                {"1万", "1万"},
                {"当前铜钱: {0}", "当前铜钱: {0}"},
                {"当前元宝: ", "当前元宝: "},
                
                // 地图模式相关
                {"府邸子模式", "府邸子模式"},
                {"生成府邸所在郡：", "生成府邸所在郡："},
                {"生成府邸所在县：", "生成府邸所在县："},
                {"仅添加府邸", "仅添加府邸"},
                {"添加后进入府邸", "添加后进入府邸"},
                {"农庄子模式", "农庄子模式"},
                {"封地子模式", "封地子模式"},
                {"世家子模式", "世家子模式"},
                {"府邸", "府邸"},
                {"农庄", "农庄"},
                {"封地", "封地"},
                {"世家", "世家"},
                {"请先选择一个郡", "请先选择一个郡"},
                {"准备就绪", "准备就绪"},
                {"无效的数量，请输入1-1000000范围内的整数", "无效的数量，请输入1-1000000范围内的整数"},
                {"请先选择府邸所在的郡县", "请先选择府邸所在的郡县"},
                {"找不到选择的县", "找不到选择的县"},
                {"请先选择农庄所在的郡县", "请先选择农庄所在的郡县"},
                {"添加失败：该处已有农庄属于【其它世家】", "添加失败：该处已有农庄属于【其它世家】"},
                {"添加失败：该处已有农庄属于【你】", "添加失败：该处已有农庄属于【你】"},
                {"添加农庄失败：", "添加农庄失败："},
                {"请先选择要解锁的郡", "请先选择要解锁的郡"},
                {"无效的郡选择", "无效的郡选择"},
                {"解锁封地失败: ", "解锁封地失败: "},
                {"添加失败：功能正在开发中", "添加失败：功能正在开发中"},
                {"请输入有效的数值", "请输入有效的数值"},
                {"获取当前元宝数量失败", "获取当前元宝数量失败"},
                {"添加物品失败: ", "添加物品失败: "},
                {"无效的物品ID", "无效的物品ID"},
                {"添加物品时发生错误: ", "添加物品时发生错误: "},
                {"无效的话本ID", "无效的话本ID"},
                {"添加话本时发生错误: ", "添加话本时发生错误: "},
                {"添加府邸方式选择", "添加府邸方式选择"},
                {"农庄的名字：", "农庄的名字："},
                {"点击下方添加按钮即可解锁选择郡的所属封地", "点击下方添加按钮即可解锁选择郡的所属封地"},
                {"未找到匹配的物品", "未找到匹配的物品"},
                
                // 新增状态消息翻译
                {"{0}未解锁或郡城叛军未清剿，无法添加府邸", "{0}未解锁或郡城叛军未清剿，无法添加府邸"},
                {"{0}未解锁或郡城叛军未清剿，无法添加农庄", "{0}未解锁或郡城叛军未清剿，无法添加农庄"},
                {"{0}-{1}已有府邸，添加失败", "{0}-{1}已有府邸，添加失败"},
                {"{0}-{1}已有农庄，添加失败", "{0}-{1}已有农庄，添加失败"},
                {"{0}-{1}已有世家，添加失败", "{0}-{1}已有世家，添加失败"},
                {"已解锁农庄: {0} ({1}-{2})，面积: {3}", "已解锁农庄: {0} ({1}-{2})，面积: {3}"},
                {"添加失败：你所选择的位置并未解锁，请解锁{0}郡再添加", "添加失败：你所选择的位置并未解锁，请解锁{0}郡再添加"},
                {"{0}未解锁或郡城叛军未清剿，无法解锁封地", "{0}未解锁或郡城叛军未清剿，无法解锁封地"},
                {"{0}的封地已解锁", "{0}的封地已解锁"},
                {"{0}的封地数据不存在或索引错误", "{0}的封地数据不存在或索引错误"},
                {"已添加{0}铜钱", "已添加{0}铜钱"},
                {"已添加{0}元宝", "已添加{0}元宝"},
                {"添加的【{0}】数量为0，添加失败", "添加的【{0}】数量为0，添加失败"},
                {"已添加: {0} x {1}", "已添加: {0} x {1}"},
                {"已添加: {0}", "已添加: {0}"},
                {"话本{0}已存在，不重复添加", "话本{0}已存在，不重复添加"},
                
                // 郡名和城市名
                {"南郡", "南郡"},
                {"临沮", "临沮"},
                {"襄樊", "襄樊"},
                {"宜城", "宜城"},
                {"麦城", "麦城"},
                {"华容", "华容"},
                {"郢亭", "郢亭"},
                {"江陵", "江陵"},
                {"夷陵", "宜昌"},
                {"三川郡", "三川郡"},
                {"平阳", "平阳"},
                {"荥阳", "荥阳"},
                {"原武", "原武"},
                {"阳武", "阳武"},
                {"新郑", "新郑"},
                {"宜阳", "宜阳"},
                {"蜀郡", "蜀郡"},
                {"邛崃", "邛崃"},
                {"郫县", "郫县"},
                {"什邡", "什邡"},
                {"绵竹", "绵竹"},
                {"新都", "新都"},
                {"成都", "成都"},
                {"丹阳郡", "丹阳郡"},
                {"秣陵", "秣陵"},
                {"江乘", "江乘"},
                {"江宁", "江宁"},
                {"溧阳", "溧阳"},
                {"建邺", "建邺"},
                {"永世", "永世"},
                {"陈留郡", "陈留郡"},
                {"长垣", "长垣"},
                {"济阳", "济阳"},
                {"成武", "成武"},
                {"襄邑", "襄邑"},
                {"宁陵", "宁陵"},
                {"封丘", "封丘"},
                {"长沙郡", "长沙郡"},
                {"零陵", "零陵"},
                {"益阳", "益阳"},
                {"湘县", "湘县"},
                {"袁州", "袁州"},
                {"庐陵", "庐陵"},
                {"衡山", "衡山"},
                {"建宁", "建宁"},
                {"桂阳", "桂阳"},
                {"会稽郡", "会稽郡"},
                {"曲阿", "曲阿"},
                {"松江", "松江"},
                {"山阴", "山阴"},
                {"余暨", "余暨"},
                {"广陵郡", "广陵郡"},
                {"平安", "平安"},
                {"射阳", "射阳"},
                {"海陵", "海陵"},
                {"江都", "江都"},
                {"太原郡", "太原郡"},
                {"大陵", "大陵"},
                {"晋阳", "晋阳"},
                {"九原", "九原"},
                {"石城", "石城"},
                {"阳曲", "阳曲"},
                {"魏榆", "魏榆"},
                {"盂县", "盂县"},
                {"中都", "中都"},
                {"益州郡", "益州郡"},
                {"连然", "连然"},
                {"谷昌", "谷昌"},
                {"同劳", "同劳"},
                {"昆泽", "昆泽"},
                {"滇池", "滇池"},
                {"俞元", "俞元"},
                {"胜休", "胜休"},
                {"南安", "南安"},
                {"南海郡", "南海郡"},
                {"四会", "四会"},
                {"阳山", "阳山"},
                {"龙川", "龙川"},
                {"揭岭", "揭岭"},
                {"罗阳", "罗阳"},
                {"善禺", "善禺"},
                {"云南郡", "云南郡"},
                {"云平", "云平"},
                {"叶榆", "叶榆"},
                {"永宁", "永宁"},
                {"遂久", "遂久"},
                {"姑复", "姑复"},
                {"蜻蛉", "蜻蛉"},
                {"弄栋", "弄栋"},
                {"邪龙", "邪龙"}
            };

            // 英文翻译
            translations["en-US"] = new Dictionary<string, string>
            {
                // 主界面相关
                {"物品添加器", "Item Adder"},
                {"货币", "Currency"},
                {"物品", "Items"},
                {"话本", "Stories"},
                {"地图", "Map"},
                {"搜索物品:", "Search Item:"},
                {"搜索:", "Search:"},
                {"分类:", "Category:"},
                {"清空", "Clear"},
                {"添加物品", "Add Item"},
                {"添加", "Add"},
                {"使用说明:", "Instructions:"},
                {"1. 请在点击添加前先保存游戏，以便回档", "1. Please save the game before adding items for rollback"},
                {"2. 按F2键显示/隐藏窗口", "2. Press F2 to show/hide window"},
                {"3. 切换模式选择：物品模式/货币模式/话本模式/地图模式", "3. Switch modes: Items/Currency/Stories/Map"},
                {"4. 输入部分字符可搜索物品或话本", "4. Enter partial characters to search items or stories"},
                {"5. 选择项目并选择或输入数量后点击添加按钮", "5. Select item, choose/enter quantity, then click Add"},
                {"Mod作者：AnZhi20", "Mod Author: AnZhi20"},
                {"Mod版本：", "Mod Version: "},
                
                // 物品分类相关
                {"特殊物品", "Special Items"},
                {"新增物品", "New Items"},
                {"丹药", "Herbal Pills"},
                {"符咒", "Talismans"},
                {"毒药", "Poison"},
                {"美食", "Food"},
                {"农产", "Agricultural"},
                {"布料", "Fabric"},
                {"矿产", "Minerals"},
                {"香粉", "Cosmetics"},
                {"珠宝", "Jewelry"},
                {"武器", "Weapons"},
                {"书法", "Calligraphy"},
                {"丹青", "Painting"},
                {"文玩", "Antiques"},
                {"乐器", "Musical Instruments"},
                {"茶具", "Tea Sets"},
                {"香具", "Incense Tools"},
                {"瓷器", "Porcelain"},
                {"美酒", "Wine"},
                {"皮毛", "Furs"},
                {"书籍", "Books"},
                
                // 货币模式相关
                {"选择货币类型:", "Select Currency Type:"},
                {"铜钱", "Copper Coins"},
                {"元宝", "Gold Ingots"},
                {"数值:", "Amount:"},
                {"(0-10亿)", "(0-1,000,000,000)"},
                {"(0-10万)", "(0-100,000)"},
                {"10万", "100K"},
                {"100万", "1M"},
                {"1000万", "10M"},
                {"1亿", "100M"},
                {"10亿", "1B"},
                {"1百", "100"},
                {"1千", "1K"},
                {"1万", "10K"},
                {"当前铜钱: {0}", "Current Coins: {0}"},
                {"当前元宝: ", "Current Gold Ingots: "},
                
                // 地图模式相关
                {"府邸子模式", "Mansion Submode"},
                {"生成府邸所在郡：", "Select Prefecture:"},
                {"生成府邸所在县：", "Select County:"},
                {"仅添加府邸", "Add Mansion Only"},
                {"添加后进入府邸", "Add & Enter Mansion"},
                {"农庄子模式", "Farm Submode"},
                {"封地子模式", "Fief Submode"},
                {"世家子模式", "Family Submode"},
                {"府邸", "Mansion"},
                {"农庄", "Farm"},
                {"封地", "Fief"},
                {"世家", "Family"},
                {"请先选择一个郡", "Please select a prefecture first"},
                {"准备就绪", "Ready"},
                {"无效的数量，请输入1-1000000范围内的整数", "Invalid quantity. Please enter an integer between 1-1,000,000"},
                {"请先选择府邸所在的郡县", "Please select the county for the mansion first"},
                {"找不到选择的县", "Cannot find the selected county"},
                {"请先选择农庄所在的郡县", "Please select the county for the farm first"},
                {"添加失败：该处已有农庄属于【其它世家】", "Failed to add: There's already a farm owned by another family here"},
                {"添加失败：该处已有农庄属于【你】", "Failed to add: You already own a farm here"},
                {"添加农庄失败：", "Failed to add farm: "},
                {"请先选择要解锁的郡", "Please select a prefecture to unlock first"},
                {"无效的郡选择", "Invalid prefecture selection"},
                {"解锁封地失败: ", "Failed to unlock fief: "},
                {"添加失败：功能正在开发中", "Failed to add: This feature is under development"},
                {"请输入有效的数值", "Please enter a valid number"},
                {"获取当前元宝数量失败", "Failed to get current gold ingot count"},
                {"添加物品失败: ", "Failed to add item: "},
                {"无效的物品ID", "Invalid item ID"},
                {"添加物品时发生错误: ", "Error occurred when adding item: "},
                {"无效的话本ID", "Invalid storybook ID"},
                {"添加话本时发生错误: ", "Error occurred when adding storybook: "},
                {"添加府邸方式选择", "Mansion Addition Method"},
                {"农庄的名字：", "Farm Name:"},
                {"点击下方添加按钮即可解锁选择郡的所属封地", "Click Add button below to unlock fiefs in selected prefecture"},
                {"未找到匹配的物品", "No matching items found"},
                
                // 新增状态消息翻译
                {"{0}未解锁或郡城叛军未清剿，无法添加府邸", "{0} not unlocked or county rebels not cleared, cannot add mansion"},
                {"{0}未解锁或郡城叛军未清剿，无法添加农庄", "{0} not unlocked or county rebels not cleared, cannot add farm"},
                {"{0}-{1}已有府邸，添加失败", "{0}-{1} already has a mansion, addition failed"},
                {"{0}-{1}已有农庄，添加失败", "{0}-{1} already has a farm, addition failed"},
                {"{0}-{1}已有世家，添加失败", "{0}-{1} already has a family, addition failed"},
                {"已解锁农庄: {0} ({1}-{2})，面积: {3}", "Unlocked farm: {0} ({1}-{2}), area: {3}"},
                {"添加失败：你所选择的位置并未解锁，请解锁{0}郡再添加", "Failed to add: The selected location is not unlocked. Please unlock {0} prefecture first"},
                {"{0}未解锁或郡城叛军未清剿，无法解锁封地", "{0} not unlocked or county rebels not cleared, cannot unlock fief"},
                {"{0}的封地已解锁", "{0}'s fief has been unlocked"},
                {"{0}的封地数据不存在或索引错误", "{0}'s fief data does not exist or index error"},
                {"已添加{0}铜钱", "{0} copper coins added"},
                {"已添加{0}元宝", "{0} gold ingots added"},
                {"添加的【{0}】数量为0，添加失败", "The quantity of [{0}] is 0, addition failed"},
                {"已添加: {0} x {1}", "Added: {0} x {1}"},
                {"已添加: {0}", "Added: {0}"},
                {"话本{0}已存在，不重复添加", "Storybook {0} already exists, not adding again"},
                
                // 郡名和城市名
                {"南郡", "Nan Province"},
                {"临沮", "Linju"},
                {"襄樊", "Xiangfan"},
                {"宜城", "Yicheng"},
                {"麦城", "Maicheng"},
                {"华容", "Huarong"},
                {"郢亭", "Yingting"},
                {"江陵", "Jiangling"},
                {"夷陵", "Yiling"},
                {"三川郡", "Sanchuan Province"},
                {"平阳", "Pingyang"},
                {"荥阳", "Yingyang"},
                {"原武", "Yuanwu"},
                {"阳武", "Yangwu"},
                {"新郑", "Xinzheng"},
                {"宜阳", "Yiyang"},
                {"蜀郡", "Shu Province"},
                {"邛崃", "Qionglai"},
                {"郫县", "Pixian"},
                {"什邡", "Shifang"},
                {"绵竹", "Mianzhu"},
                {"新都", "Xindu"},
                {"成都", "Chengdu"},
                {"丹阳郡", "Danyang Province"},
                {"秣陵", "Moling"},
                {"江乘", "Jiangcheng"},
                {"江宁", "Jiangning"},
                {"溧阳", "Liyang"},
                {"建邺", "Jianye"},
                {"永世", "Yongshi"},
                {"陈留郡", "Chenliu Province"},
                {"长垣", "Changyuan"},
                {"济阳", "Jiyang"},
                {"成武", "Chengwu"},
                {"襄邑", "Xiangyi"},
                {"宁陵", "Ningling"},
                {"封丘", "Fengqiu"},
                {"长沙郡", "Changsha Province"},
                {"零陵", "Ling"},
                {"益阳", "Yiyang"},
                {"湘县", "XiangXian"},
                {"袁州", "Yuanzhou"},
                {"庐陵", "Luling"},
                {"衡山", "Hengshan"},
                {"建宁", "Jianning"},
                {"桂阳", "Guiyang"},
                {"会稽郡", "Kuaiji Province"},
                {"曲阿", "Qu'e"},
                {"松江", "Songjiang"},
                {"山阴", "Shanyin"},
                {"余暨", "Yuji"},
                {"广陵郡", "Guangling Province"},
                {"平安", "Ping'an"},
                {"射阳", "Sheyang"},
                {"海陵", "Hailing"},
                {"江都", "Jiangdu"},
                {"太原郡", "Taiyuan Province"},
                {"大陵", "Daling"},
                {"晋阳", "Jinyang"},
                {"九原", "Jiuyuan"},
                {"石城", "Shicheng"},
                {"阳曲", "Yangqu"},
                {"魏榆", "Weiyu"},
                {"盂县", "YuXian"},
                {"中都", "Zhongdu"},
                {"益州郡", "Yizhou Province"},
                {"连然", "Lianran"},
                {"谷昌", "Guchang"},
                {"同劳", "Tonglao"},
                {"昆泽", "Kunze"},
                {"滇池", "Dianchi"},
                {"俞元", "Yuyuan"},
                {"胜休", "Shengxiu"},
                {"南安", "Nanan"},
                {"南海郡", "Nanhai Province"},
                {"四会", "Sihui"},
                {"阳山", "Yangshan"},
                {"龙川", "Longchuan"},
                {"揭岭", "Jieling"},
                {"罗阳", "Luoyang"},
                {"善禺", "Shanyu"},
                {"云南郡", "Yunnan Province"},
                {"云平", "Yunping"},
                {"叶榆", "Yeyu"},
                {"永宁", "Yongning"},
                {"遂久", "Suijiu"},
                {"姑复", "Gufu"},
                {"蜻蛉", "Qingling"},
                {"弄栋", "Nongdong"},
                {"邪龙", "Xielong"}
            };
        }

        // 设置当前语言
        public void SetLanguage(string languageCode)
        {
            if (translations.ContainsKey(languageCode))
            {
                currentLanguageCode = languageCode;
            }
            else
            {
                // 如果请求的语言不存在，使用默认语言
                currentLanguageCode = "zh-CN";
            }
        }

        // 获取翻译
        public string GetTranslation(string key)
        {
            if (translations[currentLanguageCode].ContainsKey(key))
            {
                return translations[currentLanguageCode][key];
            }
            else
            {
                // 如果键不存在，返回键本身
                return key;
            }
        }

        // 获取支持的语言列表
        public List<string> GetSupportedLanguages()
        {
            return translations.Keys.ToList();
        }

        // 获取语言显示名称
        public string GetLanguageDisplayName(string languageCode)
        {
            switch (languageCode)
            {
                case "zh-CN": return "中文简体";
                case "en-US": return "English";
                // 可以在这里添加更多语言的显示名称
                default:
                    return languageCode;
            }
        }
        
        // 检查系统语言是否为中文
        public bool IsChineseLanguage()
        {
            // 检查系统语言是否为中文
            string systemLanguage = Application.systemLanguage.ToString();
            return systemLanguage.Contains("Chinese");
        }
        
        // 根据当前语言获取文本
        public string GetText(string key)
        {
            string languageCode = IsChineseLanguage() ? "zh-CN" : "en-US";
            if (translations.ContainsKey(languageCode) && translations[languageCode].ContainsKey(key))
            {
                return translations[languageCode][key];
            }
            return key; // 如果没有找到对应的翻译，返回原文本
        }
    }
}