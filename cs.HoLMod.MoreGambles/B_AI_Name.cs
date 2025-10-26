using System;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace cs.HoLMod.MoreGambles
{
    /// <summary>
    /// AI姓名随机生成器
    /// 用于生成中文AI角色的随机姓名
    /// </summary>
    internal class B_AI_Name
    {
        // 随机数生成器
        private static System.Random random = new System.Random();
        
        // 姓氏列表
        private static string[] surnames = {
            "赵", "钱", "孙", "李", "周", "吴", "郑", "王", "冯", "陈", "褚", "卫", "蒋", "沈", "韩", "杨", "朱", "秦", "尤", "许",
            "何", "吕", "施", "张", "孔", "曹", "严", "华", "金", "魏", "陶", "姜", "戚", "谢", "邹", "喻", "柏", "水", "窦", "章",
            "云", "苏", "潘", "葛", "奚", "范", "彭", "郎", "鲁", "韦", "昌", "马", "苗", "凤", "花", "方", "俞", "任", "袁", "柳",
            "酆", "鲍", "史", "唐", "费", "廉", "岑", "薛", "雷", "贺", "倪", "汤", "滕", "殷", "罗", "毕", "郝", "邬", "安", "常",
            "乐", "于", "时", "傅", "皮", "卞", "齐", "康", "伍", "余", "元", "卜", "顾", "孟", "平", "黄", "和", "穆", "萧", "尹",
            "姚", "邵", "湛", "汪", "祁", "毛", "禹", "狄", "米", "贝", "明", "臧", "计", "伏", "成", "戴", "谈", "宋", "茅", "庞",
            "熊", "纪", "舒", "屈", "项", "祝", "董", "梁", "杜", "阮", "蓝", "闵", "席", "季", "麻", "强", "贾", "路", "娄", "危",
            "江", "童", "颜", "郭", "梅", "盛", "林", "刁", "钟", "徐", "邱", "骆", "高", "夏", "蔡", "田", "樊", "胡", "凌", "霍",
            "虞", "万", "支", "柯", "昝", "管", "卢", "莫", "经", "房", "裘", "缪", "干", "解", "应", "宗", "丁", "宣", "贲", "邓",
            "郁", "单", "杭", "洪", "包", "诸", "左", "石", "崔", "吉", "钮", "龚", "程", "嵇", "邢", "滑", "裴", "陆", "荣", "翁",
            "荀", "羊", "於", "惠", "甄", "麴", "家", "封", "芮", "羿", "储", "靳", "汲", "邴", "糜", "松", "井", "段", "富", "巫",
            "乌", "焦", "巴", "弓", "牧", "隗", "山", "谷", "车", "侯", "宓", "蓬", "全", "郗", "班", "仰", "秋", "仲", "伊", "宫",
            "宁", "仇", "栾", "暴", "甘", "钭", "厉", "戎", "祖", "武", "符", "刘", "景", "詹", "束", "龙", "叶", "幸", "司", "韶",
            "郜", "黎", "蓟", "薄", "印", "宿", "白", "怀", "蒲", "邰", "从", "鄂", "索", "咸", "籍", "赖", "卓", "蔺", "屠", "蒙",
            "池", "乔", "阴", "郁", "胥", "能", "苍", "双", "闻", "莘", "党", "翟", "谭", "贡", "劳", "逄", "姬", "申", "扶", "堵",
            "冉", "宰", "郦", "雍", "舄", "璩", "桑", "桂", "濮", "牛", "寿", "通", "边", "扈", "燕", "冀", "郏", "浦", "尚", "农",
            "温", "别", "庄", "晏", "柴", "瞿", "阎", "充", "慕", "连", "茹", "习", "宦", "艾", "鱼", "容", "向", "古", "易", "慎",
            "戈", "廖", "庾", "终", "暨", "居", "衡", "步", "都", "耿", "满", "弘", "匡", "国", "文", "寇", "广", "禄", "阙", "东",
            "殴", "殳", "沃", "利", "蔚", "越", "夔", "隆", "师", "巩", "厍", "聂", "晁", "勾", "敖", "融", "冷", "訾", "辛", "阚",
            "那", "简", "饶", "空", "曾", "毋", "沙", "乜", "养", "鞠", "须", "丰", "巢", "关", "蒯", "相", "查", "後", "荆", "红",
            "游", "竺", "权", "逯", "盖", "益", "桓", "公"
        };
        
        // 常用男性名字字库
        private static string[] maleGivenNames = {
            "伟", "强", "军", "勇", "杰", "涛", "磊", "超", "明", "刚",
            "辉", "飞", "阳", "宇", "浩", "俊", "鹏", "磊", "博", "涛",
            "鑫", "毅", "峰", "洋", "轩", "哲", "涵", "晨", "然", "恒",
            "铭", "远", "俊", "泽", "航", "瑞", "麟", "龙", "虎", "豹",
            "震", "宇", "轩", "昂", "畅", "达", "德", "栋", "帆", "凡",
            "方", "飞", "峰", "风", "刚", "光", "国", "海", "涵", "航",
            "豪", "浩", "宏", "鸿", "华", "辉", "晖", "会", "剑", "健"
        };
        
        // 常用女性名字字库
        private static string[] femaleGivenNames = {
            "娜", "丽", "敏", "静", "燕", "玲", "秀", "霞", "芳", "娟",
            "莉", "兰", "洁", "梅", "琴", "丹", "惠", "颖", "瑶", "敏",
            "雪", "梦", "琪", "婷", "雅", "萱", "娜", "楠", "琳", "玲",
            "菲", "雨", "悦", "雯", "诗", "佳", "怡", "茜", "颖", "颖",
            "媛", "璇", "颖", "瑶", "雪", "梅", "琳", "丹", "珍", "珠",
            "玉", "倩", "秀", "艳", "美", "娜", "丽", "芬", "芳", "燕",
            "玲", "秀", "霞", "芳", "娟", "莉", "兰", "洁", "梅", "琴"
        };
        
        /// <summary>
        /// 随机生成一个AI姓名
        /// </summary>
        /// <returns>随机生成的中文姓名</returns>
        public static string GenerateRandomName()
        {
            // 随机选择一个姓氏
            string surname = surnames[random.Next(surnames.Length)];
            
            // 随机决定性别比例（7:3 男性:女性）
            bool isMale = random.Next(10) < 7;
            
            // 根据性别选择名字字库
            string[] givenNames = isMale ? maleGivenNames : femaleGivenNames;
            
            // 随机决定名字长度（1或2个字）
            int nameLength = random.Next(10) < 3 ? 1 : 2; // 30%概率是单名，70%概率是双名
            
            string givenName = "";
            
            if (nameLength == 1)
            {
                // 单名
                givenName = givenNames[random.Next(givenNames.Length)];
            }
            else
            {
                // 双名 - 可以选择两个相同的字或不同的字
                bool useSameChar = random.Next(10) < 2; // 20%概率使用叠字
                
                if (useSameChar)
                {
                    string singleChar = givenNames[random.Next(givenNames.Length)];
                    givenName = singleChar + singleChar;
                }
                else
                {
                    // 选择两个不同的字
                    int firstIndex = random.Next(givenNames.Length);
                    int secondIndex;
                    
                    // 确保第二个字与第一个字不同
                    do
                    {
                        secondIndex = random.Next(givenNames.Length);
                    } while (secondIndex == firstIndex);
                    
                    givenName = givenNames[firstIndex] + givenNames[secondIndex];
                }
            }
            
            // 返回完整姓名（姓氏+名字）
            return surname + givenName;
        }
        
        /// <summary>
        /// 生成多个不重复的随机AI姓名
        /// </summary>
        /// <param name="count">需要生成的姓名数量</param>
        /// <returns>不重复的随机姓名列表</returns>
        public static List<string> GenerateRandomNames(int count)
        {
            List<string> names = new List<string>();
            
            // 生成指定数量的不重复姓名
            while (names.Count < count)
            {
                string name = GenerateRandomName();
                if (!names.Contains(name))
                {
                    names.Add(name);
                }
            }
            
            return names;
        }
        
        /// <summary>
        /// 从姓氏列表中随机选择一个姓氏
        /// </summary>
        /// <returns>随机姓氏</returns>
        public static string GetRandomSurname()
        {
            return surnames[random.Next(surnames.Length)];
        }
        
        /// <summary>
        /// 生成一个随机的男性名字
        /// </summary>
        /// <returns>随机男性名字</returns>
        public static string GenerateRandomMaleName()
        {
            string surname = surnames[random.Next(surnames.Length)];
            string givenName;
            
            // 随机决定名字长度
            int nameLength = random.Next(10) < 3 ? 1 : 2;
            
            if (nameLength == 1)
            {
                givenName = maleGivenNames[random.Next(maleGivenNames.Length)];
            }
            else
            {
                int firstIndex = random.Next(maleGivenNames.Length);
                int secondIndex;
                do
                {
                    secondIndex = random.Next(maleGivenNames.Length);
                } while (secondIndex == firstIndex);
                
                givenName = maleGivenNames[firstIndex] + maleGivenNames[secondIndex];
            }
            
            return surname + givenName;
        }
        
        /// <summary>
        /// 生成一个随机的女性名字
        /// </summary>
        /// <returns>随机女性名字</returns>
        public static string GenerateRandomFemaleName()
        {
            string surname = surnames[random.Next(surnames.Length)];
            string givenName;
            
            // 随机决定名字长度
            int nameLength = random.Next(10) < 3 ? 1 : 2;
            
            if (nameLength == 1)
            {
                givenName = femaleGivenNames[random.Next(femaleGivenNames.Length)];
            }
            else
            {
                bool useSameChar = random.Next(10) < 4; // 女性名字更倾向于使用叠字
                
                if (useSameChar)
                {
                    string singleChar = femaleGivenNames[random.Next(femaleGivenNames.Length)];
                    givenName = singleChar + singleChar;
                }
                else
                {
                    int firstIndex = random.Next(femaleGivenNames.Length);
                    int secondIndex;
                    do
                    {
                        secondIndex = random.Next(femaleGivenNames.Length);
                    } while (secondIndex == firstIndex);
                    
                    givenName = femaleGivenNames[firstIndex] + femaleGivenNames[secondIndex];
                }
            }
            
            return surname + givenName;
        }
    }
}
