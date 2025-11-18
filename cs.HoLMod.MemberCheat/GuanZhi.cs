using System.Collections.Generic;

namespace HoLMod
{
    public class GuanZhiData
    {
        // 五维数组GuanZhi，类型为List<List<List<List<List<int>>>>>
        public static List<List<List<List<List<int>>>>> GuanZhi = new List<List<List<List<List<int>>>>>()
        {
            //0,空数组，里面的任何一维的值均为-1
            new List<List<List<List<int>>>>(),

            //1,皇家官职
            new List<List<List<List<int>>>>()
            {
                //0，"1@0都马",皇家官职剩下的三维的值为0、-1、-1
                new List<List<List<int>>>(),

                //1，"1@1世子妃",皇家官职剩下的三维的值为0、-1、-1
                new List<List<List<int>>>(),

                //2,"1@2王妃",皇家官职剩下的三维的值为0、-1、-1
                new List<List<List<int>>>(),
                
                //3,"1@3驸马",皇家官职剩下的三维的值为0、-1、-1
                new List<List<List<int>>>(),
                
                //4,"1@4皇子妃",皇家官职剩下的三维的值为0、-1、-1
                new List<List<List<int>>>(),
                
                //5,"1@5皇后",皇家官职剩下的三维的值为0、-1、-1
                new List<List<List<int>>>(),
                
                //6,"1@6太后",皇家官职剩下的三维的值为0、-1、-1
                new List<List<List<int>>>(),
                
                //7"1@7太妃",皇家官职剩下的三维的值为0、-1、-1
                new List<List<List<int>>>()
            },

            //2,商会官职，游戏正在开发中，目前仅有0
            new List<List<List<List<int>>>>()
            {
                //0,"2@0商会代表",剩下的三维的值为0、-1、-1
                new List<List<List<int>>>()
            },

            //3,内宫官职，游戏正在开发中，目前仅有0
            new List<List<List<List<int>>>>()
            {
                //0,"3@0内宫宦臣",剩下的三维的值为0、-1、-1
                new List<List<List<int>>>()
            },

            //4,空数组，里面的任何一维的值均为-1
            new List<List<List<List<int>>>>(),

            //5，朝廷官职
            new List<List<List<List<int>>>>()
            {
                //0,"5@0七品"
                new List<List<List<int>>>()
                {
                    //0,"5@0@0散官",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //1,"5@0@1县丞",剩下的二维为郡索引、县索引
                    new List<List<int>>(),

                    //2,"5@0@2县尉",剩下的二维为郡索引、县索引
                    new List<List<int>>(),

                    //3,"5@0@3翊麾校尉",剩下的二维的值为-1、-1
                    new List<List<int>>()
                },

                //1,"5@1六品"
                new List<List<List<int>>>()
                {
                    //0,"5@1@0散官",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //1,"5@1@1县令",剩下的二维为郡索引、县索引
                    new List<List<int>>(),

                    //2,"5@1@2归德司阶",剩下的二维的值为-1、-1
                    new List<List<int>>()
                },

                //2,"5@2五品"
                new List<List<List<int>>>()
                {
                    //0,"5@2@0散官",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //1,"5@2@1郡丞",剩下的二维为郡索引、值为-1
                    new List<List<int>>(),

                    //2,"5@2@2郡尉",剩下的二维为郡索引、值为-1
                    new List<List<int>>(),

                    //3,值为-1,剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //4,"5@2@4游骑将军",剩下的二维的值为-1、-1
                    new List<List<int>>()
                },

                //3,"5@3四品"
                new List<List<List<int>>>()
                {
                    //0,"5@3@0散官",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //1,"5@3@1郡守",剩下的二维为郡索引、值为-1
                    new List<List<int>>(),

                    //2,"5@3@2宣威将军",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //3,"5@3@3左翊卫将军",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //4,"5@3@4右翊卫将军",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //5,"5@3@5左骁卫将军",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //6,"5@3@6右骁卫将军",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //7,"5@3@7左武卫将军",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //8,"5@3@8右武卫将军",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //9,"5@3@9左屯卫将军",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //10,"5@3@10右屯卫将军",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //11,"5@3@11右屯卫将军",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //12,"5@3@12右候卫将军",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //13,"5@3@13左御卫将军",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //14,"5@3@14右御卫将军",剩下的二维的值为-1、-1
                    new List<List<int>>()
                },

                //4,"5@4三品"
                new List<List<List<int>>>()
                {
                    //0,"5@4@0散官",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //1,"5@4@1刑部尚书",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //2,"5@4@2吏部尚书",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //3,"5@4@3户部尚书",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //4,"5@4@4礼部尚书",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //5,"5@4@5工部尚书",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //6,"5@4@6兵部尚书",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //7,"5@4@7御史",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //8,"5@4@8御史",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //9,"5@4@9御史",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //10,"5@4@10御史",剩下的二维的值为-1、-1
                    new List<List<int>>()
                },

                //5,"5@5二品"
                new List<List<List<int>>>()
                {
                    //0,"5@5@0散官",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //1,"5@5@1尚书令",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //2,"5@5@2御史大夫",剩下的二维的值为-1、-1
                    new List<List<int>>()
                },

                //6,"5@6一品"
                new List<List<List<int>>>()
                {
                    //0,"5@6@0散官",剩下的二维的值为-1、-1
                    new List<List<int>>(),

                    //1,"5@6@1丞相",剩下的二维的值为-1、-1
                    new List<List<int>>()
                }
            },

            //皇家官职
            new List<List<List<List<int>>>>()
            {
                //0,"6@0郡主",剩下的三维的值为0、-1、-1
                new List<List<List<int>>>(),

                //1,"6@1世子",剩下的三维的值为0、-1、-1
                new List<List<List<int>>>(),

                //2,"6@2亲王",剩下的三维的值为0、-1、-1
                new List<List<List<int>>>(),

                //3,"6@3公主",剩下的三维的值为0、-1、-1
                new List<List<List<int>>>(),

                //4,"6@4皇子",剩下的三维的值为0、-1、-1
                new List<List<List<int>>>(),

                //5,"6@5皇帝",剩下的三维的值为0、-1、-1
                new List<List<List<int>>>(),

                //6,"6@6太上皇",剩下的三维的值为0、-1、-1
                new List<List<List<int>>>()
            }
        };

        // 郡数组，索引对应郡ID
        public static string[] JunList = new string[]
        {
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
        public static string[][] XianList = new string[][]
        {
            // 南郡 (索引0)
            new string[] { "临沮", "襄樊", "宜城", "麦城", "华容", "郢亭", "江陵", "夷陵" },
            
            // 三川郡 (索引1)
            new string[] { "平阳", "荥阳", "原武", "阳武", "新郑", "宜阳" },
            
            // 蜀郡 (索引2)
            new string[] { "邛崃", "郫县", "什邡", "绵竹", "新都", "成都" },
            
            // 丹阳郡 (索引3)
            new string[] { "秣陵", "江乘", "江宁", "溧阳", "建邺", "永世" },
            
            // 陈留郡 (索引4)
            new string[] { "长垣", "济阳", "成武", "襄邑", "宁陵", "封丘" },
            
            // 长沙郡 (索引5)
            new string[] { "零陵", "益阳", "湘县", "袁州", "庐陵", "衡山", "建宁", "桂阳" },
            
            // 会稽郡 (索引6)
            new string[] { "曲阿", "松江", "山阴", "余暨" },
            
            // 广陵郡 (索引7)
            new string[] { "平安", "射阳", "海陵", "江都" },
            
            // 太原郡 (索引8)
            new string[] { "大陵", "晋阳", "九原", "石城", "阳曲", "魏榆", "孟县", "中都" },
            
            // 益州郡 (索引9)
            new string[] { "连然", "谷昌", "同劳", "昆泽", "滇池", "俞元", "胜休", "南安" },
            
            // 南海郡 (索引10)
            new string[] { "四会", "阳山", "龙川", "揭岭", "罗阳", "善禺" },
            
            // 云南郡 (索引11)
            new string[] { "云平", "叶榆", "永宁", "遂久", "姑复", "蜻陵", "弄栋", "邪龙" }
        };


        // 根据i@j@k@l@m格式读取官职名称
        public static string GetOfficialPosition(int i, int j, int k, int l, int m)
        {
            // 当i为0时，返回"未知官职"
            if (i == 0)
            {
                return "未知官职";
            }

            // 检查索引是否在有效范围内
            try
            {
                bool isValidIndex = false;
                
                // 对于朝廷官职（i=5），特殊处理索引检查
                if (i == 5)
                {
                    // 检查j和k的基本范围
                    if (j >= 0 && j < GuanZhi[i].Count && k >= 0 && k < GuanZhi[i][j].Count)
                    {
                        // 对于朝廷官职，我们不依赖GuanZhi数组的结构来返回官职名称，而是直接使用代码中的条件判断
                        // 所以只要j和k在有效范围内，就认为索引有效
                        // 不需要进一步检查l和m的值
                        isValidIndex = true;
                    }
                }
                else if (i < GuanZhi.Count && 
                    j < GuanZhi[i].Count && 
                    k < GuanZhi[i][j].Count)
                {
                    // 对于非朝廷官职，检查所有索引，但允许l和m为-1
                    isValidIndex = l == -1 || (GuanZhi[i][j].Count > 0 && l < GuanZhi[i][j][k].Count);
                    if (isValidIndex && l != -1 && m != -1)
                    {
                        isValidIndex = GuanZhi[i][j][k].Count > 0 && m < GuanZhi[i][j][k][l].Count;
                    }
                }

                if (isValidIndex)
                {
                    // 根据索引返回对应的官职描述
                    switch (i)
                    {
                        case 1: // 皇家官职
                            switch (j)
                            {
                                case 0: return "都马";
                                case 1: return "世子妃";
                                case 2: return "王妃";
                                case 3: return "驸马";
                                case 4: return "皇子妃";
                                case 5: return "皇后";
                                case 6: return "太后";
                                case 7: return "太妃";
                                case 8: return "皇妃";
                                default: return "未知皇家官职";
                            }
                        case 2: // 商会官职
                            return "商会代表";
                        case 3: // 内宫官职
                            return "内宫宦臣";
                        case 5: // 朝廷官职
                            if (j == 0 && k == 0) return "七品散官";
                            if (j == 0 && k == 1 && l >= 0 && l < JunList.Length && m >= 0 && m < XianList[l].Length) return $"{JunList[l]}{XianList[l][m]}县丞";
                            if (j == 0 && k == 2 && l >= 0 && l < JunList.Length && m >= 0 && m < XianList[l].Length) return $"{JunList[l]}{XianList[l][m]}县尉";
                            if (j == 0 && k == 3) return "翊麾校尉";
                            if (j == 1 && k == 0) return "六品散官";
                            if (j == 1 && k == 1 && l >= 0 && l < JunList.Length && m >= 0 && m < XianList[l].Length) return $"{JunList[l]}{XianList[l][m]}县令";
                            if (j == 1 && k == 2) return "归德司阶";
                            if (j == 2 && k == 0) return "五品散官";
                            if (j == 2 && k == 1 && l >= 0 && l < JunList.Length) return $"{JunList[l]}郡丞";
                            if (j == 2 && k == 2 && l >= 0 && l < JunList.Length) return $"{JunList[l]}郡尉";
                            if (j == 2 && k == 4) return "游骑将军";
                            if (j == 3 && k == 0) return "四品散官";
                            if (j == 3 && k == 1&& l >= 0 && l < JunList.Length) return $"{JunList[l]}郡守";
                            if (j == 3 && k == 2) return "宣威将军";
                            if (j == 3 && k == 3) return "左翊卫将军";
                            if (j == 3 && k == 4) return "右翊卫将军";
                            if (j == 3 && k == 5) return "左骁卫将军";
                            if (j == 3 && k == 6) return "右骁卫将军";
                            if (j == 3 && k == 7) return "左武卫将军";
                            if (j == 3 && k == 8) return "右武卫将军";
                            if (j == 3 && k == 9) return "左屯卫将军";
                            if (j == 3 && k == 10) return "右屯卫将军";
                            if (j == 3 && k == 11) return "右屯卫将军";
                            if (j == 3 && k == 12) return "右候卫将军";
                            if (j == 3 && k == 13) return "左御卫将军";
                            if (j == 3 && k == 14) return "右御卫将军";
                            if (j == 4 && k == 0) return "三品散官";
                            if (j == 4 && k == 1) return "刑部尚书";
                            if (j == 4 && k == 2) return "吏部尚书";
                            if (j == 4 && k == 3) return "户部尚书";
                            if (j == 4 && k == 4) return "礼部尚书";
                            if (j == 4 && k == 5) return "工部尚书";
                            if (j == 4 && k == 6) return "兵部尚书";
                            if (j == 4 && k >= 7 && k <= 10) return "御史";
                            if (j == 5 && k == 0) return "二品散官";
                            if (j == 5 && k == 1) return "尚书令";
                            if (j == 5 && k == 2) return "御史大夫";
                            if (j == 6 && k == 0) return "一品散官";
                            if (j == 6 && k == 1) return "丞相";
                            return "未知朝廷官职";
                        case 6: // 皇家官职
                            switch (j)
                            {
                                case 0: return "郡主";
                                case 1: return "世子";
                                case 2: return "亲王";
                                case 3: return "公主";
                                case 4: return "皇子";
                                case 5: return "皇帝";
                                case 6: return "太上皇";
                                default: return "未知皇家身份";
                            }
                        default:
                            return "未知官职";
                    }
                }
                else
                {
                    return "未知官职";
                }
            }
            catch
            {
                return "未知官职";
            }
        }

        // 根据i@j@k@l@m格式的字符串获取官职名称
        public static string GetOfficialPosition(string positionString)
        {
            if (string.IsNullOrEmpty(positionString))
                return "未知官职";

            try
            {
                string[] parts = positionString.Split('@');
                if (parts.Length >= 5)
                {
                    int i = int.Parse(parts[0]);
                    int j = int.Parse(parts[1]);
                    int k = int.Parse(parts[2]);
                    int l = int.Parse(parts[3]);
                    int m = int.Parse(parts[4]);
                    return GetOfficialPosition(i, j, k, l, m);
                }
                else
                {
                    return "未知官职";
                }
            }
            catch
            {
                return "未知官职";
            }
        }
    }
}