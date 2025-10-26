using System.Collections.Generic;
using System.Linq;

namespace cs.HoLMod.MenberCheat
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
            // 中文翻译（代码中编写的中文，翻译的中文）；此数组不编写也不影响使用，因为开发者编写的代码需要显示的逻辑本身就是中文
            translations["zh-CN"] = new Dictionary<string, string>
            {
                // 基本界面文本
                {"成员修改器", "成员修改器"},
                {"族人修改", "族人修改"},
                {"门客修改", "门客修改"},
                {"皇室修改", "皇室修改"},
                {"世家修改", "世家修改"},
                {"农庄修改", "农庄修改"},
                {"族人家族", "族人家族"},
                {"族人妻妾", "族人妻妾"},
                {"皇室主脉", "皇室主脉"},
                {"皇家妻妾", "皇家妻妾"},
                {"世家主脉", "世家主脉"},
                {"世家妻妾", "世家妻妾"},
                {"名字搜索:", "名字搜索:"},
                {"清空", "清空"},
                {"修改", "修改"},

                // 界面元素文本
                {"选择郡: ", "选择郡: "},
                {"大地图", "大地图"},
                {"选择家族: ", "选择家族: "},
                {"全部家族", "全部家族"},
                {"选择成员: ", "选择成员: "},
                {"全郡", "全郡"},
                {"已选择：", "已选择："},
                {"官职: ", "官职: "},
                {"一品", "一品"},
                {"二品", "二品"},
                {"三品", "三品"},
                {"四品", "四品"},
                {"五品", "五品"},
                {"六品", "六品"},
                {"七品", "七品"},
                {"声誉:", "声誉:"},
                {"体力:", "体力:"},
                {"年龄:", "年龄:"},
                {"健康:", "健康:"},
                {"心情:", "心情:"},
                {"魅力:", "魅力:"},
                {"幸运:", "幸运:"},
                {"品性:", "品性:"},
                {"天赋:", "天赋:"},
                {"天赋点:", "天赋点:"},
                {"技能:", "技能:"},
                {"技能点:", "技能点:"},
                {"喜好:", "喜好:"},
                {"文才:", "文才:"},
                {"武才:", "武才:"},
                {"商才:", "商才:"},
                {"艺才:", "艺才:"},
                {"计谋:", "计谋:"},
                {"爵位:", "爵位:"},
                {"封地:", "封地:"},
                {"门客酬劳:", "门客酬劳:"},
                {"农庄面积: ", "农庄面积: "},
                {"农户数量-种植: ", "农户数量-种植: "},
                {"农户数量-养殖: ", "农户数量-养殖: "},
                {"农户数量-手工: ", "农户数量-手工: "},
                {"庄头属性:", "庄头属性:"},
                {"管理: ", "管理: "},
                {"忠诚: ", "忠诚: "},
                
                // 错误信息
                {"天赋为空，天赋点不为零", "天赋为空，天赋点不为零"},
                {"技能为空，技能点不为零", "技能为空，技能点不为零"},
                {"可容纳农户数量不足，请升级或修建更多农房", "可容纳农户数量不足，请升级或修建更多农房"},
                {"应用修改时出错，请查看日志", "应用修改时出错，请查看日志"},
                {"当前农庄暂无庄头", "当前农庄暂无庄头"},
                {"请先选择分类", "请先选择分类"},
                {"加载庄头数据时出错: {0}", "加载庄头数据时出错: {0}"},
                {"应用{0}属性修改时出错: {1}", "应用{0}属性修改时出错: {1}"},

                // 更多错误信息
                {"处理世家主脉成员数据时出错: {0}", "处理世家主脉成员数据时出错: {0}"},
                {"处理世家妻妾成员数据时出错: {0}", "处理世家妻妾成员数据时出错: {0}"},
                {"筛选成员时出错: {0}", "筛选成员时出错: {0}"},
                {"处理成员数据时出错: {0}", "处理成员数据时出错: {0}"},
                {"处理农庄数据时出错: {0}", "处理农庄数据时出错: {0}"},
                {"搜索农庄数据时出错: {0}", "搜索农庄数据时出错: {0}"},
                {"加载{0}数据时出错: {1}", "加载{0}数据时出错: {1}"},

                // 成功信息
                {"【{0}】属性修改成功", "【{0}】属性修改成功"},
                {"成功应用世家主脉{0}的属性修改", "成功应用世家主脉{0}的属性修改"},
                {"属性修改成功", "属性修改成功"},
                {"成功修改农庄 {0} 的属性", "成功修改农庄 {0} 的属性"},
                {"【{0}】的{1}，修改失败", "【{0}】的{1}，修改失败"},
                {"应用族人家族{0}的属性修改时出错: {1}", "应用族人家族{0}的属性修改时出错: {1}"},
                {"成功应用皇室主脉{0}的属性修改", "成功应用皇室主脉{0}的属性修改"},
                {"Mainload.Member_King集合无效或索引超出范围", "Mainload.Member_King集合无效或索引超出范围"},
                {"应用皇室主脉{0}的属性修改时出错: {1}", "应用皇室主脉{0}的属性修改时出错: {1}"},
                {"成功应用皇家妻妾{0}的属性修改", "成功应用皇家妻妾{0}的属性修改"},
                {"Mainload.Member_King_qu集合无效或索引超出范围", "Mainload.Member_King_qu集合无效或索引超出范围"},
                {"应用皇家妻妾{0}的属性修改时出错: {1}", "应用皇家妻妾{0}的属性修改时出错: {1}"},

                // 使用说明和其他文本
                {"使用说明:", "使用说明:"},
                {"1. 请在点击修改前先保存游戏，以便回档", "1. 请在点击修改前先保存游戏，以便回档"},
                {"2. 按F3键显示/隐藏窗口", "2. 按F3键显示/隐藏窗口"},
                {"3. 选择修改类型：族人修改/门客修改/皇室修改/世家修改/农庄修改", "3. 选择修改类型：族人修改/门客修改/皇室修改/世家修改/农庄修改"},
                {"4. 族人模式下可选择：族人家族/族人妻妾", "4. 族人模式下可选择：族人家族/族人妻妾"},
                {"5. 皇室模式下可选择：皇室主脉/皇家妻妾","5. 皇室模式下可选择：皇室主脉/皇家妻妾"},
                {"6. 世家模式下可选择：世家主脉/世家妻妾","6. 世家模式下可选择：世家主脉/世家妻妾"},
                {"7. 输入部分字符可搜索角色", "7. 输入部分字符可搜索角色"},
                {"8. 选择角色后可以通过点击按钮或者直接在文本框中输入来修改对应的属性值", "8. 选择角色后可以通过点击按钮或者直接在文本框中输入来修改对应的属性值"},
                {"9. 点击修改按钮应用更改", "9. 点击修改按钮应用更改"},
                {"Mod作者：AnZhi20", "Mod作者：AnZhi20"},
                {"Mod版本：{0}", "Mod版本：{0}"},
                
                // 功名中文翻译
                {"无", "无"},
                {"秀才", "秀才"},
                {"举人", "举人"},
                {"解元", "解元"},
                {"贡士", "贡士"},
                {"会元", "会元"},
                {"进士", "进士"},
                {"探花", "探花"},
                {"榜眼", "榜眼"},
                {"状元", "状元"},
                {"武秀才", "武秀才"},
                {"武举人", "武举人"},
                {"武解元", "武解元"},
                {"武贡士", "武贡士"},
                {"武会元", "武会元"},
                {"武进士", "武进士"},
                {"武探花", "武探花"},
                {"武榜眼", "武榜眼"},
                {"武状元", "武状元"},
                
                // 爵位中文翻译
                {"无爵位", "无爵位"},
                {"@伯", "@伯"},
                {"@侯", "@侯"},
                {"@公", "@公"},
                {"@王", "@王"},
                {"伯爵", "伯爵"},
                {"侯爵", "侯爵"},
                {"公爵", "公爵"},
                {"王爵", "王爵"},

                
                // 官职
                {"未知官职", "未知官职"},
                {"一品散官", "一品散官"},
                {"二品散官", "二品散官"},
                {"三品散官", "三品散官"},
                {"四品散官", "四品散官"},
                {"五品散官", "五品散官"},
                {"六品散官", "六品散官"},
                {"七品散官", "七品散官"},
                
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
                {"邪龙", "邪龙"},

                // 品性
                {"不明", "不明"},
                {"傲娇", "傲娇"},
                {"刚正", "刚正"},
                {"活泼", "活泼"},
                {"善良", "善良"},
                {"真诚", "真诚"},
                {"洒脱", "洒脱"},
                {"高冷", "高冷"},
                {"自卑", "自卑"},
                {"怯懦", "怯懦"},
                {"腼腆", "腼腆"},
                {"凶狠", "凶狠"},
                {"善变", "善变"},
                {"忧郁", "忧郁"},
                {"多疑", "多疑"},

                // 封地
                {"无封地","无封地"},
                {"汉阳","汉阳"},
                {"左亭","左亭"},
                {"华阳","华阳"},
                {"宛陵","宛陵"},
                {"长罗","长罗"},
                {"安成","安成"},
                {"太末","太末"},
                {"盐渎","盐渎"},
                {"霍人","霍人"},
                {"比苏","比苏"},
                {"新会","新会"},
                {"越隽","越隽"},

                // 天赋和技能都存在的
                {"空","空"},

                // 天赋
                {"文","文"},
                {"武","武"},
                {"商","商"},
                {"艺","艺"},

                // 技能
                {"巫","巫"},
                {"医","医"},
                {"相","相"},
                {"卜","卜"},
                {"魅","魅"},
                {"工","工"},

                // 喜好
                {"香粉","香粉"},
                {"书法","书法"},
                {"丹青","丹青"},
                {"文玩","文玩"},
                {"茶具","茶具"},
                {"香具","香具"},
                {"瓷器","瓷器"},
                {"美酒","美酒"},
                {"琴瑟","琴瑟"},
                {"皮毛","皮毛"},

                // 其它翻译
                {"家","家"}
            };

            // 英文翻译（代码中编写的中文，翻译的英文）
            translations["en-US"] = new Dictionary<string, string>
            {
                // 基本界面文本
                {"成员修改器", "Member Cheat"},
                {"族人修改", "Clan Member"},
                {"门客修改", "Guest"},
                {"皇室修改", "Royal"},
                {"世家修改", "Noble"},
                {"农庄修改", "Farm"},
                {"族人家族", "Clan Family"},
                {"族人妻妾", "Clan Spouse"},
                {"皇室主脉", "Royal Main"},
                {"皇家妻妾", "Royal Spouse"},
                {"世家主脉", "Noble Main"},
                {"世家妻妾", "Noble Spouse"},
                {"名字搜索:", "Name Search:"},
                {"清空", "Clear"},
                {"修改", "Modify"},

                // 界面元素文本
                {"选择郡: ", "Select Province: "},
                {"大地图", "World Map"},
                {"选择家族: ", "Select Family: "},
                {"全部家族", "All Families"},
                {"选择成员: ", "Select Member: "},
                {"全郡", "All Provinces"},
                {"已选择：", "Selected: "},
                {"官职: ", "Official Position: "},
                {"一品", "Rank 1"},
                {"二品", "Rank 2"},
                {"三品", "Rank 3"},
                {"四品", "Rank 4"},
                {"五品", "Rank 5"},
                {"六品", "Rank 6"},
                {"七品", "Rank 7"},
                {"声誉:", "Reputation:"},
                {"体力:", "Stamina:"},
                {"年龄:", "Age:"},
                {"健康:", "Health:"},
                {"心情:", "Mood:"},
                {"魅力:", "Charm:"},
                {"幸运:", "Luck:"},
                {"品性:", "Character:"},
                {"天赋:", "Talent:"},
                {"天赋点:", "Talent Points:"},
                {"技能:", "Skill:"},
                {"技能点:", "Skill Points:"},
                {"喜好:", "Preference:"},
                {"文才:", "Literary Talent:"},
                {"武才:", "Martial Talent:"},
                {"商才:", "Business Talent:"},
                {"艺才:", "Artistic Talent:"},
                {"计谋:", "Strategy:"},
                {"爵位:", "Noble Title:"},
                {"封地:", "Fief:"},
                {"门客酬劳:", "Guest Reward:"},
                {"农庄面积: ", "Farm Area: "},
                {"农户数量-种植: ", "Farmers - Planting: "},
                {"农户数量-养殖: ", "Farmers - Livestock: "},
                {"农户数量-手工: ", "Farmers - Crafts: "},
                {"庄头属性:", "Farm Manager Attributes:"},
                {"管理: ", "Management: "},
                {"忠诚: ", "Loyalty: "},
                
                // 错误信息
                {"天赋为空，天赋点不为零", "Talent is empty, but talent points are non-zero"},
                {"技能为空，技能点不为零", "Skill is empty, but skill points are non-zero"},
                {"可容纳农户数量不足，请升级或修建更多农房", "Insufficient housing for farmers, please upgrade or build more farmhouses"},
                {"应用修改时出错，请查看日志", "Error applying changes, please check the log"},
                {"当前农庄暂无庄头", "Current farm has no manager"},
                {"请先选择分类", "Please select a category first"},
                {"加载庄头数据时出错: {0}", "Error loading farm manager data: {0}"},
                {"应用{0}属性修改时出错: {1}", "Error applying attribute modifications for {0}: {1}"},

                // 更多错误信息
                {"处理世家主脉成员数据时出错: {0}", "Error processing noble main member data: {0}"},
                {"处理世家妻妾成员数据时出错: {0}", "Error processing noble spouse data: {0}"},
                {"筛选成员时出错: {0}", "Error filtering members: {0}"},
                {"处理成员数据时出错: {0}", "Error processing member data: {0}"},
                {"处理农庄数据时出错: {0}", "Error processing farm data: {0}"},
                {"搜索农庄数据时出错: {0}", "Error searching farm data: {0}"},
                {"加载{0}数据时出错: {1}", "Error loading {0} data: {1}"},

                // 成功信息
                {"【{0}】属性修改成功", "【{0}】attribute modification successful"},
                {"成功应用世家主脉{0}的属性修改", "Successfully applied attribute modifications for noble main {0}"},
                {"属性修改成功", "attribute modification successful"},
                {"成功修改农庄 {0} 的属性", "Successfully modified farm {0} attributes"},
                {"【{0}】的{1}，修改失败", "【{0}】{1} modification failed"},
                {"应用族人家族{0}的属性修改时出错: {1}", "Error applying clan family {0} attribute modifications: {1}"},
                {"成功应用皇室主脉{0}的属性修改", "Successfully applied royal main {0} attribute modifications"},
                {"Mainload.Member_King集合无效或索引超出范围", "Mainload.Member_King collection is invalid or index out of range"},
                {"应用皇室主脉{0}的属性修改时出错: {1}", "Error applying royal main {0} attribute modifications: {1}"},
                {"成功应用皇家妻妾{0}的属性修改", "Successfully applied royal spouse {0} attribute modifications"},
                {"Mainload.Member_King_qu集合无效或索引超出范围", "Mainload.Member_King_qu collection is invalid or index out of range"},
                {"应用皇家妻妾{0}的属性修改时出错: {1}", "Error applying royal spouse {0} attribute modifications: {1}"},

                // 使用说明和其他文本
                {"使用说明:", "Usage Instructions:"},
                {"1. 请在点击修改前先保存游戏，以便回档", "1. Please save the game before clicking to ensure rollback"},
                {"2. 按F3键显示/隐藏窗口", "2. Press F3 to show/hide the window"},
                {"3. 选择修改类型：族人修改/门客修改/皇室修改/世家修改/农庄修改", "3. Select modification type: Clan family/Guest/Royal main/Noble family/Farm"},
                {"4. 族人模式下可选择：族人家族/族人妻妾","4. In the clan mode, you can choose: clan family/clan wives and concubines"},
                {"5. 皇室模式下可选择：皇室主脉/皇家妻妾", "5. Select royal main/royal spouses"},
                {"6. 世家模式下可选择：世家主脉/世家妻妾", "6. Select noble main/noble spouses"},
                {"7. 输入部分字符可搜索角色", "7. Input partial characters to search for roles"},
                {"8. 选择角色后可以通过点击按钮或者直接在文本框中输入来修改对应的属性值", "8. Select a role to modify its attributes by clicking the button or directly inputting in the text box"},
                {"9. 点击修改按钮应用更改", "9. Click the modify button to apply changes"},
                {"Mod作者：AnZhi20", "Mod Author: AnZhi20"},
                {"Mod版本：{0}", "Mod Version: {0}"},
                
                // 功名英文翻译
                {"无", "None"},
                {"秀才", "Xiucai"},
                {"举人", "Juren"},
                {"解元", "Jieyuan"},
                {"贡士", "Gongshi"},
                {"会元", "Huiyuan"},
                {"进士", "Jinshi"},
                {"探花", "Tanhua"},
                {"榜眼", "Bangyan"},
                {"状元", "Zhuangyuan"},
                {"武秀才", "M. Xiucai"},
                {"武举人", "M. Juren"},
                {"武解元", "M. Jieyuan"},
                {"武贡士", "M. Gongshi"},
                {"武会元", "M. Huiyuan"},
                {"武进士", "M. Jinshi"},
                {"武探花", "M. Tanhua"},
                {"武榜眼", "M. Bangyan"},
                {"武状元", "M. Zhuangyuan"},
                
                // 爵位英文翻译
                {"无爵位", "No Title"},
                {"@伯", "Earl of @"},
                {"@侯", "Marquis of @"},
                {"@公", "Duke of @"},
                {"@王", "King of @"},
                {"伯爵", "Earl"},
                {"侯爵", "Marquis"},
                {"公爵", "Duke"},
                {"王爵", "King"},

                // 官职英文翻译
                {"未知官职", "Unknown Title"},
                {"一品散官", "First Class Scoundrel"},
                {"二品散官", "Second Class Scoundrel"},
                {"三品散官", "Third Class Scoundrel"},
                {"四品散官", "Fourth Class Scoundrel"},
                {"五品散官", "Fifth Class Scoundrel"},
                {"六品散官", "Sixth Class Scoundrel"},
                {"七品散官", "Seventh Class Scoundrel"},


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
                {"邪龙", "Xielong"},
                
                // 封地
                {"无封地","None"},
                {"汉阳","Hanyang"},
                {"左亭","Zuoting"},
                {"华阳","Huayang"},
                {"宛陵","Wanling"},
                {"长罗","Changluo"},
                {"安成","Ancheng"},
                {"太末","Taimo"},
                {"盐渎","Yandu"},
                {"霍人","Huoren"},
                {"比苏","Bisu"},
                {"新会","Xinhui"},
                {"越隽","Yuejuan"},

                // 品性
                {"不明", "Unclear"},
                {"傲娇", "Proud"},
                {"刚正", "Righteous"},
                {"活泼", "Lively"},
                {"善良", "Kind"},
                {"真诚", "Honest"},
                {"洒脱", "Carefree"},
                {"高冷", "Cold"},
                {"自卑", "Insecure"},
                {"怯懦", "Timid"},
                {"腼腆", "Shy"},
                {"凶狠", "Mean"},
                {"善变", "Fickle"},
                {"忧郁", "Gloomy"},
                {"多疑", "Paranoid"},

                // 天赋和技能都存在的
                {"空","None"},

                // 天赋
                {"文","Writing"},
                {"武","Might"},
                {"商","Business"},
                {"艺","Arts"},

                // 技能
                {"巫","Sorcery"},
                {"医","Medicine"},
                {"相","Daoism"},
                {"卜","Augur"},
                {"魅","Charisma"},
                {"工","Crafting"},

                // 喜好
                {"香粉","Rouge"},
                {"书法","Ink"},
                {"丹青","Art"},
                {"文玩","Antique"},
                {"茶具","Tea Set"},
                {"香具","Incense"},
                {"瓷器","Vase"},
                {"美酒","Wine"},
                {"琴瑟","Music"},
                {"皮毛","Pelt"},

                // 其它翻译
                {"家","Family"},
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
    }
}