using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace cs.HoLMod.NameManager
{
    public static class ExistingNamesList
    {
        // 已存在的姓名列表
        public static List<string> ExistingNames = new List<string>();

        // 检查存档中已存在的姓名并添加到列表中
        public static void CheckExistingNames()
        {
            // 确保现有姓名列表已初始化
            if (ExistingNames == null)
            {
                ExistingNames = new List<string>();
            }
            else
            {
                // 清空列表以避免重复添加
                ExistingNames.Clear();
            }

            try
            {
                // 遍历Mainload.Member_Qinglou (i索引0-11，j可以用count来确定长度)
                if (Mainload.Member_Qinglou != null)
                {
                    for (int i = 0; i < 12 && i < Mainload.Member_Qinglou.Count; i++)
                    {
                        if (Mainload.Member_Qinglou[i] != null)
                        {
                            for (int j = 0; j < Mainload.Member_Qinglou[i].Count; j++)
                            {
                                if (Mainload.Member_Qinglou[i][j] != null && Mainload.Member_Qinglou[i][j].Count > 3)
                                {
                                    string nameInfo = Mainload.Member_Qinglou[i][j][3].Split(new char[]{'|'})[0];
                                    ExistingNames.Add(nameInfo);
                                }
                            }
                        }
                    }
                }

                // 遍历Mainload.Member_Hanmen
                if (Mainload.Member_Hanmen != null)
                {
                    for (int i = 0; i < Mainload.Member_Hanmen.Count; i++)
                    {
                        if (Mainload.Member_Hanmen[i] != null && Mainload.Member_Hanmen[i].Count > 2)
                        {
                            string nameInfo = Mainload.Member_Hanmen[i][2].Split(new char[]{'|'})[0];
                            ExistingNames.Add(nameInfo);
                        }
                    }
                }

                // 遍历Mainload.Member_Other_qu
                if (Mainload.Member_Other_qu != null)
                {
                    for (int i = 0; i < Mainload.Member_Other_qu.Count; i++)
                    {
                        if (Mainload.Member_Other_qu[i] != null)
                        {
                            for (int j = 0; j < Mainload.Member_Other_qu[i].Count; j++)
                            {
                                if (Mainload.Member_Other_qu[i][j] != null && Mainload.Member_Other_qu[i][j].Count > 2)
                                {
                                    string nameInfo = Mainload.Member_Other_qu[i][j][2].Split(new char[]{'|'})[0];
                                    ExistingNames.Add(nameInfo);
                                }
                            }
                        }
                    }
                }

                // 遍历Mainload.Member_other
                if (Mainload.Member_other != null)
                {
                    for (int i = 0; i < Mainload.Member_other.Count; i++)
                    {
                        if (Mainload.Member_other[i] != null)
                        {
                            for (int j = 0; j < Mainload.Member_other[i].Count; j++)
                            {
                                if (Mainload.Member_other[i][j] != null && Mainload.Member_other[i][j].Count > 2)
                                {
                                    string nameInfo = Mainload.Member_other[i][j][2].Split(new char[]{'|'})[0];
                                    ExistingNames.Add(nameInfo);
                                }
                            }
                        }
                    }
                }

                // 遍历Mainload.Member_now
                if (Mainload.Member_now != null)
                {
                    for (int i = 0; i < Mainload.Member_now.Count; i++)
                    {
                        if (Mainload.Member_now[i] != null && Mainload.Member_now[i].Count > 4)
                        {
                            string nameInfo = Mainload.Member_now[i][4].Split(new char[]{'|'})[0];
                            ExistingNames.Add(nameInfo);
                        }
                    }
                }

                // 遍历Mainload.Member_qu
                if (Mainload.Member_qu != null)
                {
                    for (int i = 0; i < Mainload.Member_qu.Count; i++)
                    {
                        if (Mainload.Member_qu[i] != null && Mainload.Member_qu[i].Count > 2)
                        {
                            string nameInfo = Mainload.Member_qu[i][2].Split(new char[]{'|'})[0];
                            ExistingNames.Add(nameInfo);
                        }
                    }
                }

                // 遍历Mainload.Member_King_qu
                if (Mainload.Member_King_qu != null)
                {
                    for (int i = 0; i < Mainload.Member_King_qu.Count; i++)
                    {
                        if (Mainload.Member_King_qu[i] != null && Mainload.Member_King_qu[i].Count > 2)
                        {
                            string nameInfo = Mainload.Member_King_qu[i][2].Split(new char[]{'|'})[0];
                            ExistingNames.Add(nameInfo);
                        }
                    }
                }

                // 遍历Mainload.Member_King
                if (Mainload.Member_King != null)
                {
                    for (int i = 0; i < Mainload.Member_King.Count; i++)
                    {
                        if (Mainload.Member_King[i] != null && Mainload.Member_King[i].Count > 2)
                        {
                            string nameInfo = Mainload.Member_King[i][2].Split(new char[]{'|'})[0];
                            ExistingNames.Add(nameInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("遍历获取名字时出错: " + ex.Message);
            }
        }

        // 获取需要检查的名字并检查
        public static string CheckName(string name, string sexID, int ShijiaIndex, int DaiNum)
        {
            string result = name;
            
            try
            {
                // 在检查前调用CheckExistingNames()
                CheckExistingNames();
                
                // 检查姓名是否已存在
                int maxAttempts = 100; // 设置最大尝试次数，防止无限循环
                int attempts = 0;
                
                while (ExistingNames.Contains(result) && attempts < maxAttempts)
                {
                    // 如果姓名已经存在，则重新生成新姓名
                    result = typeof(RandName).GetMethod("GetMemberNameShijia", 
                        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                        ?.Invoke(null, new object[] { sexID, ShijiaIndex, DaiNum }) as string ?? name;
                    attempts++;
                }
                
                // 如果达到最大尝试次数仍未获得唯一姓名，记录警告
                if (attempts >= maxAttempts)
                {
                    Debug.LogWarning("生成唯一姓名时达到最大尝试次数: " + maxAttempts);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("检查姓名时出错: " + ex.Message);
            }
            
            // 在成功return result的时候清空ExistingNames数组
            ExistingNames.Clear();
            
            return result;
        }

    }
}
