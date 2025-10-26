using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using DG.Tweening;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace cs.HoLMod.AutoArchives
{
    public class Bak_CunDangUI : MonoBehaviour
    {
        public string currentCunDangIndex_A;
        private bool isHaveData;
        private bool A;
        private bool B;
        private void Start()
        {
            // 添加关闭按钮事件
            transform.Find("CloseBT").GetComponent<Button>().onClick.AddListener(CloseBT);
            transform.Find("Back").GetComponent<Button>().onClick.AddListener(CloseBT);

            //启动窗口
            OnEnable();
        }

        private void OnEnable()
        {
            //初始化存档按钮和根据语言设置字体
            InitShow();
            InitSize();

            //查找是否有存档
            OnEnableData();

            //显示查找到的存档
            OnEnableShow();
        }

        //初始化存档按钮
        private void InitShow()
        {
            transform.Find("NoDataTip").Find("Tip").GetComponent<Text>().text = AllText.Text_UIA[31][Mainload.SetData[4]];
        }

        //初始化字体大小，根据语言设置
        private void InitSize()
        {
            if (Mainload.SetData[4] == 0)
            {
                transform.Find("DataShow").Find("Name").GetComponent<Text>().fontSize = 22;
                transform.Find("DataShow").Find("Lv").GetComponent<Text>().fontSize = 22;
                transform.Find("DataShow").Find("Time").GetComponent<Text>().fontSize = 22;
                return;
            }
            transform.Find("DataShow").Find("Name").GetComponent<Text>().fontSize = 20;
            transform.Find("DataShow").Find("Lv").GetComponent<Text>().fontSize = 20;
            transform.Find("DataShow").Find("Time").GetComponent<Text>().fontSize = 20;
        }

        //查找是否有存档
        private void OnEnableData()
        {
            // 创建复合索引
            string compositeIndex = currentCunDangIndex_A + "_" ;

            bool flag = true;
		    try
            {
                if (ES3.FileExists("FW/" + currentCunDangIndex_A + "/GameData.es3"))
                {
                    ES3.Load<List<string>>("FamilyData", "FW/" + currentCunDangIndex_A + "/GameData.es3");
                    this.A = true;
                }
                if (ES3.FileExists("FW/" + compositeIndex + base.name + "/GameData.es3"))
                {
                    ES3.Load<List<string>>("FamilyData", "FW/" + compositeIndex + base.name + "/GameData.es3");
                    this.B = true;
                }
            }
		    catch (FormatException)
            {
		    
			    flag = false;
		    }
		    if (flag)
		    {
                if(A)
                {
                    this.isHaveData = ES3.FileExists("FW/" + currentCunDangIndex_A + "/GameData.es3");
                }
                if(B)
                {
                    this.isHaveData = ES3.FileExists("FW/" + compositeIndex + base.name + "/GameData.es3");
                }
			    return;
		    }
            if(A)
            {
                ES3.DeleteDirectory("FW/" + currentCunDangIndex_A);
            }
            if(B)
            {
                ES3.DeleteDirectory("FW/" + compositeIndex + base.name);
            }
		    this.isHaveData = false;
        }

        private void Update()
        {
            // ESC键关闭窗口
            if (Input.GetKeyDown(KeyCode.Escape) && !transform.parent.Find("LoadPanel").gameObject.activeSelf)
            {
                CloseBT();
            }
        }

        //加载显示
        private void UpdateShow()
        {
            OnEnableData();
            OnEnableShow();
        }

        //显示查找到的存档
        private void OnEnableShow()
        {
            // 创建复合索引
            string compositeIndex = currentCunDangIndex_A + "_" ;
            
            // 设置标题
            transform.Find("Title").GetComponent<Text>().text = "选择存档";
            transform.Find("DelCunDPanel").gameObject.SetActive(false);
            
            // 添加动画效果
            transform.localPosition = new Vector3(0f, 500f, 0f);
            transform.DOLocalMoveY(0f, 0.3f, false).SetEase(Ease.OutBack, 1f);

            //有存档的情况
            if (this.isHaveData)
            {
                List<string> list = new List<string>();
                List<int> list2 = new List<int>();
                
                if(A)
                {
                    list = ES3.Load<List<string>>("FamilyData", "FW/" + currentCunDangIndex_A + "/GameData.es3");
                    list2 = ES3.Load<List<int>>("Time_now", "FW/" + currentCunDangIndex_A + "/GameData.es3");
                }
                if(B)
                {
                    list = ES3.Load<List<string>>("FamilyData", "FW/" + compositeIndex  + base.name + "/GameData.es3");
                    list2 = ES3.Load<List<int>>("Time_now", "FW/" + compositeIndex  + base.name + "/GameData.es3");
                }
                this.MemberShow();
                transform.Find("DataShow").Find("Name").GetComponent<Text>().text = AllText.Text_UIA[29][Mainload.SetData[4]].Replace("@", AllText.Text_City[int.Parse(list[0].Split(new char[]
                {
                    '|'
                })[0])][Mainload.SetData[4]].Split(new char[]
                {
                    '~'
                })[1].Split(new char[]
                {
                    '|'
                })[int.Parse(list[0].Split(new char[]
                {
                    '|'
                })[1])]).Replace("$", list[1]);
                transform.Find("DataShow").Find("Lv").GetComponent<Text>().text = AllText.Text_UIA[28][Mainload.SetData[4]].Replace("@", list[2]);
                transform.Find("DataShow").Find("Time").GetComponent<Text>().text = AllText.Text_UIA[975][Mainload.SetData[4]].Replace("@", list2[0].ToString()).Replace("$", AllText.Text_Months[list2[1]][Mainload.SetData[4]]).Replace("~", list2[2].ToString());
                transform.Find("DataShow").gameObject.SetActive(true);
                transform.Find("NoDataTip").gameObject.SetActive(false);
                return;
            }

            // 没有存档的情况
            transform.Find("DataShow").gameObject.SetActive(false);
            transform.Find("NoDataTip").gameObject.SetActive(true);
        }

        private void ClickBT()
        {
            // 创建复合索引
            string compositeIndex = currentCunDangIndex_A + "_" ;
            
            // 记录当前存档索引
            if(A)
            {
                Mainload.CunDangIndex_now = "FW/" + currentCunDangIndex_A;
            }
            if(B)
            {
                Mainload.CunDangIndex_now = "FW/" + compositeIndex  + base.name;
            }

            if (this.isHaveData)
		    {
			    if (Mainload.Guide_order != 10000)
			    {
				    Mainload.Guide_order = 10000;
			    }
			    SaveData.ReadGameData();
			    this.InitRunData();
			    transform.parent.parent.Find("LoadPanel").GetComponent<LoadPanel>().ShowID = 0;
			    transform.parent.parent.Find("LoadPanel").gameObject.SetActive(true);
			    transform.parent.gameObject.SetActive(false);
			    return;
		    }
		    if (Mainload.Guide_order != 10000)
		    {
			    Mainload.Guide_order = 0;
		    }
		    if (Mainload.isFirstGame)
		    {
			    Mainload.SceneID = "M|0";
			    transform.parent.parent.Find("LoadPanel").GetComponent<LoadPanel>().ShowID = 1;
			    transform.parent.parent.Find("LoadPanel").gameObject.SetActive(true);
			    Invoke("InitGame", 0.1f);
			    return;
		    }
		    transform.parent.parent.Find("InitGameUI").gameObject.SetActive(true);
		    transform.parent.gameObject.SetActive(false);
        }

        private void CloseBT()
        {
            // 返回StartGameUI
            gameObject.SetActive(false);
            transform.parent.Find("New_CunDangUI").gameObject.SetActive(true);
        }

        private void InitGame()
        {
            StartCoroutine("InitPreGameData");
        }

        // 初始化运行数据
        private void InitRunData()
        {
            for (int i = 0; i < Mainload.NongZ_now.Count; i++)
            {
                for (int j = 0; j < Mainload.NongZ_now[i].Count; j++)
                {
                    if (Mainload.NongZ_now[i][j][0] == "-1")
                    {
                        Mainload.NongzHaveData.Add(i.ToString() + "|" + j.ToString());
                    }
                }
            }
        }

        //人物显示
        private void MemberShow()
        {
            // 创建复合索引
            string compositeIndex = currentCunDangIndex_A + "_" ;
            
            List<string> list = new List<string>();
            
            if(A)
            {
                list = ES3.Load<List<string>>("Member_First", "FW/" + currentCunDangIndex_A + "/GameData.es3");
            }
            if(B)
            {
                list = ES3.Load<List<string>>("Member_First", "FW/" + compositeIndex  + base.name + "/GameData.es3");
            }
            for (int i = 0; i < transform.Find("DataShow").Find("IconShow").childCount; i++)
            {
                UnityEngine.Object.Destroy(transform.Find("DataShow").Find("IconShow").GetChild(i).gameObject);
            }
            string text = list[5];
            string text2 = list[3];
            int num = int.Parse(list[6]);
            string text3 = list[2];
            string text4;
            if (list[4].Split(new char[]
            {
                '|'
            })[0].Split(new char[]
            {
                '@'
            })[0] == "5")
            {
                if (int.Parse(text3.Split(new char[]
                {
                    '|'
                })[1]) % 2 == 0)
                {
                    text4 = "A/" + list[4].Split(new char[]
                    {
                        '|'
                    })[0].Split(new char[]
                    {
                        '@'
                    })[1];
                }
                else
                {
                    text4 = "B/" + list[4].Split(new char[]
                    {
                        '|'
                    })[0].Split(new char[]
                    {
                        '@'
                    })[1];
                }
            }
            else
            {
                text4 = "null";
            }
            string text5;
            string text6;
            string text7;
            string text8;
            string text9;
            if (num < Mainload.OldFenjie[0])
            {
                text5 = "0";
                text6 = "0";
                text7 = "0";
                text8 = "0";
                text9 = "0";
            }
            else if (num >= Mainload.OldFenjie[0] && num < Mainload.OldFenjie[1])
            {
                text5 = "1";
                text6 = "1";
                text7 = "1";
                text8 = "1";
                text9 = "1";
            }
            else if (num >= Mainload.OldFenjie[1] && num < Mainload.OldFenjie[2])
            {
                text5 = "2";
                text6 = "2";
                text7 = "2";
                text8 = "2";
                text9 = "2";
            }
            else
            {
                text5 = "3";
                text6 = "2";
                text7 = "2";
                text8 = "2";
                text9 = "3";
            }
            string path = string.Concat(new string[]
            {
                "AllLooks/Member_B/",
                text,
                "/",
                text5,
                "/houfa/",
                text3.Split(new char[]
                {
                    '|'
                })[0]
            });
            string path2;
            if (text4 != "null")
            {
                path2 = "AllLooks/Member_B/" + text + "/5/" + text4;
            }
            else
            {
                path2 = string.Concat(new string[]
                {
                    "AllLooks/Member_B/",
                    text,
                    "/",
                    text6,
                    "/shen/",
                    text3.Split(new char[]
                    {
                        '|'
                    })[1]
                });
            }
            string path3 = string.Concat(new string[]
            {
                "AllLooks/Member_B/",
                text,
                "/",
                text7,
                "/tou/",
                text3.Split(new char[]
                {
                    '|'
                })[2]
            });
            string path4 = string.Concat(new string[]
            {
                "AllLooks/Member_B/",
                text,
                "/",
                text8,
                "/PX/",
                text2
            });
            string path5 = string.Concat(new string[]
            {
                "AllLooks/Member_B/",
                text,
                "/",
                text9,
                "/qianfa/",
                text3.Split(new char[]
                {
                    '|'
                })[3]
            });
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load(path));
            gameObject.transform.SetParent(transform.Find("DataShow").Find("IconShow"));
            gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load(path2));
            gameObject2.transform.SetParent(transform.Find("DataShow").Find("IconShow"));
            gameObject2.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
            GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load(path3));
            gameObject3.transform.SetParent(transform.Find("DataShow").Find("IconShow"));
            gameObject3.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            gameObject3.transform.localPosition = new Vector3(0f, 0f, 0f);
            GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load(path4));
            gameObject4.transform.SetParent(transform.Find("DataShow").Find("IconShow"));
            gameObject4.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            gameObject4.transform.localPosition = new Vector3(0f, 0f, 0f);
            GameObject gameObject5 = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load(path5));
            gameObject5.transform.SetParent(transform.Find("DataShow").Find("IconShow"));
            gameObject5.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            gameObject5.transform.localPosition = new Vector3(0f, 0f, 0f);
        }

        // 从PerDangBT中借鉴的InitPreGameData协程方法实现
        private IEnumerator InitPreGameData()
        {
            int num4;
            for (int h = 0; h < 30; h = num4 + 1)
            {
                if (h == 0)
                {
                    Mainload.BuildInto_m = new List<List<string>>
                    {
                        new List<string>
                        {
                            "B1",
                            "5",
                            "4",
                            "0",
                            "1",
                            "-1|5",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B5",
                            "14",
                            "1",
                            "0",
                            "1",
                            "2|5",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B7",
                            "15",
                            "1",
                            "0",
                            "1",
                            "7|5",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B12",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|6",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B19",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|13",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B21",
                            "18",
                            "1",
                            "0",
                            "1",
                            "10|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B30",
                            "18",
                            "1",
                            "0",
                            "1",
                            "9|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B35",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|4",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B36",
                            "18",
                            "1",
                            "0",
                            "1",
                            "10|4",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B37",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|5",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B38",
                            "139",
                            "1",
                            "0",
                            "1",
                            "13|5",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B39",
                            "140",
                            "1",
                            "0",
                            "1",
                            "16|5",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B40",
                            "139",
                            "1",
                            "0",
                            "1",
                            "19|5",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B41",
                            "15",
                            "1",
                            "0",
                            "1",
                            "21|5",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B42",
                            "141",
                            "1",
                            "0",
                            "1",
                            "21|8",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B50",
                            "18",
                            "1",
                            "0",
                            "1",
                            "23|7",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B51",
                            "18",
                            "1",
                            "0",
                            "1",
                            "23|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B52",
                            "18",
                            "1",
                            "0",
                            "1",
                            "23|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B53",
                            "18",
                            "1",
                            "0",
                            "1",
                            "23|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B54",
                            "18",
                            "1",
                            "0",
                            "1",
                            "23|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B55",
                            "18",
                            "1",
                            "0",
                            "1",
                            "23|12",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B56",
                            "18",
                            "1",
                            "0",
                            "1",
                            "23|13",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B57",
                            "18",
                            "1",
                            "0",
                            "1",
                            "22|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B58",
                            "18",
                            "1",
                            "0",
                            "1",
                            "21|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B59",
                            "18",
                            "1",
                            "0",
                            "1",
                            "20|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B60",
                            "18",
                            "1",
                            "0",
                            "1",
                            "19|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B61",
                            "18",
                            "1",
                            "0",
                            "1",
                            "15|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B63",
                            "18",
                            "1",
                            "0",
                            "1",
                            "12|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B64",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B65",
                            "18",
                            "1",
                            "0",
                            "1",
                            "13|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B66",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|14",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B67",
                            "142",
                            "1",
                            "0",
                            "1",
                            "2|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B68",
                            "145",
                            "1",
                            "0",
                            "1",
                            "2|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B73",
                            "18",
                            "1",
                            "0",
                            "1",
                            "1|21",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B74",
                            "18",
                            "1",
                            "0",
                            "1",
                            "1|22",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B75",
                            "18",
                            "1",
                            "0",
                            "1",
                            "2|24",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B76",
                            "18",
                            "1",
                            "0",
                            "1",
                            "3|24",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B77",
                            "18",
                            "1",
                            "0",
                            "1",
                            "4|24",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B78",
                            "18",
                            "1",
                            "0",
                            "1",
                            "5|24",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B79",
                            "18",
                            "1",
                            "0",
                            "1",
                            "6|24",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B80",
                            "18",
                            "1",
                            "0",
                            "1",
                            "7|24",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B81",
                            "18",
                            "1",
                            "0",
                            "1",
                            "8|24",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B82",
                            "18",
                            "1",
                            "0",
                            "1",
                            "9|24",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B83",
                            "18",
                            "1",
                            "0",
                            "1",
                            "10|24",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B84",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|23",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B85",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|22",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B86",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B87",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B88",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B89",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|20",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B90",
                            "170",
                            "1",
                            "0",
                            "1",
                            "11|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B92",
                            "23",
                            "1",
                            "0",
                            "1",
                            "12|13",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B99",
                            "0",
                            "1",
                            "0",
                            "1",
                            "-1|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B104",
                            "18",
                            "1",
                            "0",
                            "1",
                            "-2|13",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B111",
                            "9",
                            "1",
                            "0",
                            "1",
                            "0|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B112",
                            "9",
                            "1",
                            "0",
                            "1",
                            "-2|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B113",
                            "10",
                            "1",
                            "0",
                            "1",
                            "0|18",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B114",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-2|18",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B115",
                            "10",
                            "1",
                            "0",
                            "1",
                            "0|19",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B116",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-2|19",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B117",
                            "10",
                            "1",
                            "0",
                            "1",
                            "0|23",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B118",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-2|23",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B119",
                            "18",
                            "1",
                            "0",
                            "1",
                            "1|24",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B120",
                            "18",
                            "1",
                            "0",
                            "1",
                            "0|24",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B121",
                            "18",
                            "1",
                            "0",
                            "1",
                            "-1|24",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B122",
                            "18",
                            "1",
                            "0",
                            "1",
                            "-2|24",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B126",
                            "18",
                            "1",
                            "0",
                            "1",
                            "-2|22",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B127",
                            "18",
                            "1",
                            "0",
                            "1",
                            "-2|20",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B134",
                            "170",
                            "1",
                            "0",
                            "1",
                            "17|13",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B135",
                            "8",
                            "1",
                            "0",
                            "1",
                            "-1|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B136",
                            "138",
                            "1",
                            "0",
                            "1",
                            "-1|-1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "BMM01",
                            "15",
                            "1",
                            "0",
                            "1",
                            "5|-2",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "BMM02",
                            "16",
                            "1",
                            "0",
                            "1",
                            "7|-2",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B143",
                            "171",
                            "1",
                            "0",
                            "1",
                            "10|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B144",
                            "88",
                            "1",
                            "0",
                            "1",
                            "10|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B146",
                            "88",
                            "1",
                            "0",
                            "1",
                            "10|4",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "BMM03",
                            "15",
                            "1",
                            "0",
                            "1",
                            "11|-2",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B150",
                            "7",
                            "1",
                            "0",
                            "1",
                            "14|-2",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B152",
                            "92",
                            "1",
                            "0",
                            "1",
                            "18|-2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B153",
                            "92",
                            "1",
                            "0",
                            "1",
                            "19|-2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B154",
                            "92",
                            "1",
                            "0",
                            "1",
                            "20|-2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B155",
                            "92",
                            "1",
                            "0",
                            "1",
                            "21|-2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B158",
                            "92",
                            "1",
                            "0",
                            "1",
                            "22|-1",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B159",
                            "92",
                            "1",
                            "0",
                            "1",
                            "22|0",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B160",
                            "92",
                            "1",
                            "0",
                            "1",
                            "22|1",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B161",
                            "92",
                            "1",
                            "0",
                            "1",
                            "22|2",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B162",
                            "92",
                            "1",
                            "0",
                            "1",
                            "22|3",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B164",
                            "23",
                            "1",
                            "0",
                            "1",
                            "22|-2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B165",
                            "23",
                            "1",
                            "0",
                            "1",
                            "22|4",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B166",
                            "92",
                            "1",
                            "0",
                            "1",
                            "17|-2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B171",
                            "149",
                            "1",
                            "0",
                            "1",
                            "-6|6",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B172",
                            "150",
                            "6",
                            "0",
                            "1",
                            "-6|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B174",
                            "144",
                            "1",
                            "0",
                            "1",
                            "-5|11",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B175",
                            "3",
                            "1",
                            "0",
                            "1",
                            "-4|2",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B176",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-4|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B177",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-6|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B178",
                            "171",
                            "1",
                            "0",
                            "1",
                            "-2|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B179",
                            "18",
                            "1",
                            "0",
                            "1",
                            "-2|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B180",
                            "18",
                            "1",
                            "0",
                            "1",
                            "-2|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B181",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-4|18",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B182",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-6|18",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B183",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-4|19",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B184",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-6|19",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B185",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-4|23",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B186",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-6|23",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B187",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-7|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B188",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-7|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B189",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-7|22",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B190",
                            "10",
                            "1",
                            "0",
                            "1",
                            "-7|20",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B191",
                            "171",
                            "1",
                            "0",
                            "1",
                            "-2|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B192",
                            "83",
                            "1",
                            "0",
                            "1",
                            "5|7",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B193",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B194",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B195",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B196",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B197",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|12",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B198",
                            "85",
                            "1",
                            "0",
                            "1",
                            "6|13",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B199",
                            "18",
                            "1",
                            "0",
                            "1",
                            "8|13",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B200",
                            "18",
                            "1",
                            "0",
                            "1",
                            "4|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B201",
                            "18",
                            "1",
                            "0",
                            "1",
                            "3|13",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B202",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|14",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B203",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|13",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B204",
                            "83",
                            "1",
                            "0",
                            "1",
                            "4|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B205",
                            "83",
                            "1",
                            "0",
                            "1",
                            "5|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B206",
                            "83",
                            "1",
                            "0",
                            "1",
                            "4|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B207",
                            "83",
                            "1",
                            "0",
                            "1",
                            "5|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B208",
                            "83",
                            "1",
                            "0",
                            "1",
                            "7|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B209",
                            "83",
                            "1",
                            "0",
                            "1",
                            "8|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B210",
                            "83",
                            "1",
                            "0",
                            "1",
                            "7|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B211",
                            "83",
                            "1",
                            "0",
                            "1",
                            "8|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B212",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B213",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B214",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B215",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B216",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|19",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B217",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|20",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B218",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B219",
                            "83",
                            "1",
                            "0",
                            "1",
                            "4|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B220",
                            "83",
                            "1",
                            "0",
                            "1",
                            "5|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B221",
                            "83",
                            "1",
                            "0",
                            "1",
                            "4|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B222",
                            "83",
                            "1",
                            "0",
                            "1",
                            "5|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B223",
                            "83",
                            "1",
                            "0",
                            "1",
                            "10|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B224",
                            "83",
                            "1",
                            "0",
                            "1",
                            "9|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B225",
                            "83",
                            "1",
                            "0",
                            "1",
                            "8|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B226",
                            "83",
                            "1",
                            "0",
                            "1",
                            "7|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B227",
                            "83",
                            "1",
                            "0",
                            "1",
                            "9|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B228",
                            "83",
                            "1",
                            "0",
                            "1",
                            "8|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B229",
                            "83",
                            "1",
                            "0",
                            "1",
                            "7|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B230",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B231",
                            "83",
                            "1",
                            "0",
                            "1",
                            "5|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B232",
                            "83",
                            "1",
                            "0",
                            "1",
                            "4|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B233",
                            "83",
                            "1",
                            "0",
                            "1",
                            "3|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B234",
                            "83",
                            "1",
                            "0",
                            "1",
                            "2|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B235",
                            "83",
                            "1",
                            "0",
                            "1",
                            "1|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B236",
                            "83",
                            "1",
                            "0",
                            "1",
                            "1|4",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B237",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B238",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B239",
                            "83",
                            "1",
                            "0",
                            "1",
                            "6|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B240",
                            "83",
                            "1",
                            "0",
                            "1",
                            "10|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B241",
                            "83",
                            "1",
                            "0",
                            "1",
                            "11|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B242",
                            "84",
                            "1",
                            "0",
                            "1",
                            "13|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B243",
                            "84",
                            "1",
                            "0",
                            "1",
                            "14|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B244",
                            "84",
                            "1",
                            "0",
                            "1",
                            "15|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B245",
                            "84",
                            "1",
                            "0",
                            "1",
                            "16|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B246",
                            "84",
                            "1",
                            "0",
                            "1",
                            "16|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B247",
                            "84",
                            "1",
                            "0",
                            "1",
                            "15|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B248",
                            "84",
                            "1",
                            "0",
                            "1",
                            "14|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B249",
                            "84",
                            "1",
                            "0",
                            "1",
                            "13|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B250",
                            "84",
                            "1",
                            "0",
                            "1",
                            "13|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B251",
                            "84",
                            "1",
                            "0",
                            "1",
                            "14|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B252",
                            "84",
                            "1",
                            "0",
                            "1",
                            "15|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B253",
                            "84",
                            "1",
                            "0",
                            "1",
                            "16|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B254",
                            "83",
                            "1",
                            "0",
                            "1",
                            "12|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B255",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B256",
                            "83",
                            "1",
                            "0",
                            "1",
                            "18|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B257",
                            "83",
                            "1",
                            "0",
                            "1",
                            "19|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B258",
                            "83",
                            "1",
                            "0",
                            "1",
                            "20|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B259",
                            "83",
                            "1",
                            "0",
                            "1",
                            "21|3",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B260",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|-1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B261",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B262",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B263",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B264",
                            "83",
                            "1",
                            "0",
                            "1",
                            "12|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B265",
                            "83",
                            "1",
                            "0",
                            "1",
                            "12|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B266",
                            "83",
                            "1",
                            "0",
                            "1",
                            "12|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B267",
                            "26",
                            "1",
                            "0",
                            "1",
                            "16|0",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B268",
                            "26",
                            "1",
                            "0",
                            "1",
                            "15|0",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B269",
                            "26",
                            "1",
                            "0",
                            "1",
                            "14|0",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B270",
                            "26",
                            "1",
                            "0",
                            "1",
                            "13|0",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B271",
                            "95",
                            "1",
                            "0",
                            "1",
                            "16|1",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B272",
                            "95",
                            "1",
                            "0",
                            "1",
                            "15|1",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B273",
                            "95",
                            "1",
                            "0",
                            "1",
                            "14|1",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B274",
                            "95",
                            "1",
                            "0",
                            "1",
                            "13|1",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B275",
                            "95",
                            "1",
                            "0",
                            "1",
                            "13|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B276",
                            "95",
                            "1",
                            "0",
                            "1",
                            "13|0",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B277",
                            "95",
                            "1",
                            "0",
                            "1",
                            "14|0",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B278",
                            "95",
                            "1",
                            "0",
                            "1",
                            "15|0",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B279",
                            "95",
                            "1",
                            "0",
                            "1",
                            "16|0",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B280",
                            "95",
                            "1",
                            "0",
                            "1",
                            "16|0",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B281",
                            "90",
                            "1",
                            "0",
                            "1",
                            "19|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B282",
                            "90",
                            "1",
                            "0",
                            "1",
                            "19|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B283",
                            "90",
                            "1",
                            "0",
                            "1",
                            "20|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B284",
                            "90",
                            "1",
                            "0",
                            "1",
                            "19|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B285",
                            "90",
                            "1",
                            "0",
                            "1",
                            "18|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B286",
                            "90",
                            "1",
                            "0",
                            "1",
                            "18|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B287",
                            "90",
                            "1",
                            "0",
                            "1",
                            "18|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B288",
                            "90",
                            "1",
                            "0",
                            "1",
                            "20|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B289",
                            "90",
                            "1",
                            "0",
                            "1",
                            "20|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B290",
                            "97",
                            "1",
                            "0",
                            "1",
                            "18|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B291",
                            "97",
                            "1",
                            "0",
                            "1",
                            "18|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B292",
                            "97",
                            "1",
                            "0",
                            "1",
                            "18|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B293",
                            "97",
                            "1",
                            "0",
                            "1",
                            "18|2",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B294",
                            "97",
                            "1",
                            "0",
                            "1",
                            "19|2",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B295",
                            "97",
                            "1",
                            "0",
                            "1",
                            "20|2",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B296",
                            "97",
                            "1",
                            "0",
                            "1",
                            "18|0",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B297",
                            "97",
                            "1",
                            "0",
                            "1",
                            "19|0",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B298",
                            "97",
                            "1",
                            "0",
                            "1",
                            "20|0",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B299",
                            "97",
                            "1",
                            "0",
                            "1",
                            "21|0",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B300",
                            "78",
                            "1",
                            "0",
                            "1",
                            "18|-1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B301",
                            "79",
                            "1",
                            "0",
                            "1",
                            "19|-1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B302",
                            "79",
                            "1",
                            "0",
                            "1",
                            "20|-1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B303",
                            "79",
                            "1",
                            "0",
                            "1",
                            "21|-1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B304",
                            "79",
                            "1",
                            "0",
                            "1",
                            "11|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B305",
                            "109",
                            "1",
                            "0",
                            "1",
                            "11|1",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B306",
                            "109",
                            "1",
                            "0",
                            "1",
                            "11|2",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B307",
                            "78",
                            "1",
                            "0",
                            "1",
                            "10|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B308",
                            "180",
                            "1",
                            "0",
                            "1",
                            "3|1",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B309",
                            "26",
                            "1",
                            "0",
                            "1",
                            "5|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B310",
                            "26",
                            "1",
                            "0",
                            "1",
                            "4|0",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B311",
                            "26",
                            "1",
                            "0",
                            "1",
                            "3|0",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B312",
                            "26",
                            "1",
                            "0",
                            "1",
                            "5|1",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B313",
                            "26",
                            "1",
                            "0",
                            "1",
                            "5|2",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B314",
                            "37",
                            "1",
                            "0",
                            "1",
                            "2|2",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B316",
                            "37",
                            "1",
                            "0",
                            "1",
                            "2|1",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B318",
                            "76",
                            "1",
                            "0",
                            "1",
                            "1|10",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B319",
                            "76",
                            "1",
                            "0",
                            "1",
                            "1|13",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B320",
                            "26",
                            "1",
                            "0",
                            "1",
                            "7|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B321",
                            "26",
                            "1",
                            "0",
                            "1",
                            "7|1",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B322",
                            "26",
                            "1",
                            "0",
                            "1",
                            "7|2",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B323",
                            "26",
                            "1",
                            "0",
                            "1",
                            "8|2",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B324",
                            "26",
                            "1",
                            "0",
                            "1",
                            "8|1",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B325",
                            "26",
                            "1",
                            "0",
                            "1",
                            "8|0",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B326",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-2|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B327",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-1|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B328",
                            "83",
                            "1",
                            "0",
                            "1",
                            "0|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B329",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-3|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B330",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-4|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B331",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-5|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B332",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-5|7",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B333",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-5|6",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B334",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-4|6",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B335",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-5|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B336",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-5|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B337",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-4|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B338",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-2|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B339",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-1|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B340",
                            "83",
                            "1",
                            "0",
                            "1",
                            "0|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B341",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-3|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B342",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-4|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B343",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-5|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B344",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-6|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B345",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-2|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B346",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-1|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B347",
                            "83",
                            "1",
                            "0",
                            "1",
                            "0|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B348",
                            "83",
                            "1",
                            "0",
                            "1",
                            "1|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B349",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-3|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B350",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-4|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B351",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-5|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B352",
                            "83",
                            "1",
                            "0",
                            "1",
                            "-6|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B353",
                            "90",
                            "1",
                            "0",
                            "1",
                            "5|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B354",
                            "90",
                            "1",
                            "0",
                            "1",
                            "4|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B355",
                            "90",
                            "1",
                            "0",
                            "1",
                            "4|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B356",
                            "90",
                            "1",
                            "0",
                            "1",
                            "5|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B358",
                            "25",
                            "1",
                            "0",
                            "1",
                            "6|7",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B359",
                            "90",
                            "1",
                            "0",
                            "1",
                            "7|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B360",
                            "90",
                            "1",
                            "0",
                            "1",
                            "7|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B362",
                            "90",
                            "1",
                            "0",
                            "1",
                            "8|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B363",
                            "90",
                            "1",
                            "0",
                            "1",
                            "8|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B366",
                            "110",
                            "1",
                            "0",
                            "1",
                            "7|7",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B368",
                            "97",
                            "1",
                            "0",
                            "1",
                            "5|9",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B369",
                            "97",
                            "1",
                            "0",
                            "1",
                            "5|9",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B370",
                            "97",
                            "1",
                            "0",
                            "1",
                            "4|9",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B371",
                            "97",
                            "1",
                            "0",
                            "1",
                            "4|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B372",
                            "97",
                            "1",
                            "0",
                            "1",
                            "4|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B373",
                            "97",
                            "1",
                            "0",
                            "1",
                            "4|10",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B374",
                            "97",
                            "1",
                            "0",
                            "1",
                            "5|10",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B377",
                            "97",
                            "1",
                            "0",
                            "1",
                            "5|10",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B378",
                            "97",
                            "1",
                            "0",
                            "1",
                            "7|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B380",
                            "97",
                            "1",
                            "0",
                            "1",
                            "7|9",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B381",
                            "97",
                            "1",
                            "0",
                            "1",
                            "8|9",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B382",
                            "97",
                            "1",
                            "0",
                            "1",
                            "7|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B383",
                            "97",
                            "1",
                            "0",
                            "1",
                            "7|10",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B385",
                            "90",
                            "1",
                            "0",
                            "1",
                            "5|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B386",
                            "90",
                            "1",
                            "0",
                            "1",
                            "4|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B387",
                            "90",
                            "1",
                            "0",
                            "1",
                            "5|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B388",
                            "90",
                            "1",
                            "0",
                            "1",
                            "4|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B389",
                            "90",
                            "1",
                            "0",
                            "1",
                            "5|19",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B390",
                            "90",
                            "1",
                            "0",
                            "1",
                            "4|19",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B391",
                            "90",
                            "1",
                            "0",
                            "1",
                            "4|20",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B392",
                            "90",
                            "1",
                            "0",
                            "1",
                            "5|20",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B393",
                            "109",
                            "1",
                            "0",
                            "1",
                            "5|14",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B394",
                            "109",
                            "1",
                            "0",
                            "1",
                            "4|14",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B395",
                            "109",
                            "1",
                            "0",
                            "1",
                            "7|14",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B396",
                            "79",
                            "1",
                            "0",
                            "1",
                            "8|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B397",
                            "79",
                            "1",
                            "0",
                            "1",
                            "9|14",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B398",
                            "181",
                            "1",
                            "0",
                            "1",
                            "7|16",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B400",
                            "37",
                            "1",
                            "0",
                            "1",
                            "10|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B401",
                            "37",
                            "1",
                            "0",
                            "1",
                            "10|21",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B402",
                            "37",
                            "1",
                            "0",
                            "1",
                            "9|21",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B405",
                            "110",
                            "1",
                            "0",
                            "1",
                            "8|19",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B406",
                            "110",
                            "1",
                            "0",
                            "1",
                            "9|19",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B407",
                            "110",
                            "1",
                            "0",
                            "1",
                            "-1|7",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B408",
                            "83",
                            "1",
                            "0",
                            "1",
                            "14|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B409",
                            "83",
                            "1",
                            "0",
                            "1",
                            "15|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B411",
                            "83",
                            "1",
                            "0",
                            "1",
                            "15|7",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B412",
                            "83",
                            "1",
                            "0",
                            "1",
                            "15|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B415",
                            "83",
                            "1",
                            "0",
                            "1",
                            "16|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B416",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B417",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|12",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B418",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|13",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B419",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|14",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B420",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B421",
                            "83",
                            "1",
                            "0",
                            "1",
                            "19|7",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B422",
                            "83",
                            "1",
                            "0",
                            "1",
                            "19|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B423",
                            "83",
                            "1",
                            "0",
                            "1",
                            "19|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B426",
                            "83",
                            "1",
                            "0",
                            "1",
                            "18|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B427",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B428",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B429",
                            "83",
                            "1",
                            "0",
                            "1",
                            "20|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B430",
                            "26",
                            "1",
                            "0",
                            "1",
                            "17|7",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B431",
                            "26",
                            "1",
                            "0",
                            "1",
                            "16|7",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B432",
                            "26",
                            "1",
                            "0",
                            "1",
                            "18|7",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B433",
                            "26",
                            "1",
                            "0",
                            "1",
                            "18|8",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B434",
                            "26",
                            "1",
                            "0",
                            "1",
                            "17|8",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B435",
                            "26",
                            "1",
                            "0",
                            "1",
                            "16|8",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B436",
                            "23",
                            "1",
                            "0",
                            "1",
                            "22|13",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B440",
                            "180",
                            "1",
                            "0",
                            "1",
                            "19|10",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B441",
                            "78",
                            "1",
                            "0",
                            "1",
                            "18|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B442",
                            "79",
                            "1",
                            "0",
                            "1",
                            "18|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B443",
                            "79",
                            "1",
                            "0",
                            "1",
                            "16|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B444",
                            "79",
                            "1",
                            "0",
                            "1",
                            "16|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B445",
                            "109",
                            "1",
                            "0",
                            "1",
                            "15|10",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B446",
                            "109",
                            "1",
                            "0",
                            "1",
                            "15|11",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B447",
                            "109",
                            "1",
                            "0",
                            "1",
                            "14|10",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B448",
                            "109",
                            "1",
                            "0",
                            "1",
                            "14|11",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B449",
                            "94",
                            "1",
                            "0",
                            "1",
                            "14|7",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B450",
                            "94",
                            "1",
                            "0",
                            "1",
                            "14|8",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B451",
                            "94",
                            "1",
                            "0",
                            "1",
                            "12|8",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B452",
                            "94",
                            "1",
                            "0",
                            "1",
                            "12|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B453",
                            "94",
                            "1",
                            "0",
                            "1",
                            "14|8",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B454",
                            "94",
                            "1",
                            "0",
                            "1",
                            "14|7",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B455",
                            "94",
                            "1",
                            "0",
                            "1",
                            "16|10",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B456",
                            "94",
                            "1",
                            "0",
                            "1",
                            "16|10",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B457",
                            "94",
                            "1",
                            "0",
                            "1",
                            "16|11",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B458",
                            "94",
                            "1",
                            "0",
                            "1",
                            "15|10",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B459",
                            "94",
                            "1",
                            "0",
                            "1",
                            "14|10",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B460",
                            "94",
                            "1",
                            "0",
                            "1",
                            "18|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B461",
                            "94",
                            "1",
                            "0",
                            "1",
                            "18|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B462",
                            "94",
                            "1",
                            "0",
                            "1",
                            "18|10",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B463",
                            "94",
                            "1",
                            "0",
                            "1",
                            "16|7",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B464",
                            "94",
                            "1",
                            "0",
                            "1",
                            "16|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B465",
                            "94",
                            "1",
                            "0",
                            "1",
                            "16|8",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B466",
                            "94",
                            "1",
                            "0",
                            "1",
                            "17|8",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B467",
                            "94",
                            "1",
                            "0",
                            "1",
                            "18|8",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B468",
                            "94",
                            "1",
                            "0",
                            "1",
                            "18|8",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B469",
                            "94",
                            "1",
                            "0",
                            "1",
                            "18|7",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B470",
                            "94",
                            "1",
                            "0",
                            "1",
                            "20|7",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B471",
                            "94",
                            "1",
                            "0",
                            "1",
                            "20|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B472",
                            "96",
                            "1",
                            "0",
                            "1",
                            "5|0",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B473",
                            "129",
                            "1",
                            "0",
                            "1",
                            "4|0",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B474",
                            "129",
                            "1",
                            "0",
                            "1",
                            "5|1",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B475",
                            "96",
                            "1",
                            "0",
                            "1",
                            "7|2",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B476",
                            "96",
                            "1",
                            "0",
                            "1",
                            "7|0",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B477",
                            "129",
                            "1",
                            "0",
                            "1",
                            "7|1",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B478",
                            "96",
                            "1",
                            "0",
                            "1",
                            "4|9",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B479",
                            "96",
                            "1",
                            "0",
                            "1",
                            "5|9",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B481",
                            "96",
                            "1",
                            "0",
                            "1",
                            "7|9",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B482",
                            "129",
                            "1",
                            "0",
                            "1",
                            "7|10",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B483",
                            "96",
                            "1",
                            "0",
                            "1",
                            "16|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B484",
                            "96",
                            "1",
                            "0",
                            "1",
                            "15|0",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B485",
                            "96",
                            "1",
                            "0",
                            "1",
                            "14|0",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B486",
                            "129",
                            "1",
                            "0",
                            "1",
                            "13|0",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B487",
                            "129",
                            "1",
                            "0",
                            "1",
                            "16|7",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B488",
                            "129",
                            "1",
                            "0",
                            "1",
                            "17|7",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B489",
                            "96",
                            "1",
                            "0",
                            "1",
                            "18|7",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B490",
                            "130",
                            "1",
                            "0",
                            "1",
                            "18|0",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B491",
                            "130",
                            "1",
                            "0",
                            "1",
                            "19|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B492",
                            "130",
                            "1",
                            "0",
                            "1",
                            "20|0",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B494",
                            "130",
                            "1",
                            "0",
                            "1",
                            "5|19",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B495",
                            "130",
                            "1",
                            "0",
                            "1",
                            "5|20",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B496",
                            "130",
                            "1",
                            "0",
                            "1",
                            "5|17",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B497",
                            "130",
                            "1",
                            "0",
                            "1",
                            "5|16",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B498",
                            "83",
                            "1",
                            "0",
                            "1",
                            "11|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B499",
                            "83",
                            "1",
                            "0",
                            "1",
                            "12|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B500",
                            "83",
                            "1",
                            "0",
                            "1",
                            "13|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B501",
                            "83",
                            "1",
                            "0",
                            "1",
                            "14|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B502",
                            "83",
                            "1",
                            "0",
                            "1",
                            "15|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B503",
                            "83",
                            "1",
                            "0",
                            "1",
                            "16|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B504",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B505",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B506",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B507",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|19",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B508",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|20",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B509",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B510",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|22",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B511",
                            "83",
                            "1",
                            "0",
                            "1",
                            "17|23",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B512",
                            "83",
                            "1",
                            "0",
                            "1",
                            "18|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B513",
                            "83",
                            "1",
                            "0",
                            "1",
                            "19|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B514",
                            "83",
                            "1",
                            "0",
                            "1",
                            "20|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B515",
                            "83",
                            "1",
                            "0",
                            "1",
                            "21|18",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B517",
                            "18",
                            "1",
                            "0",
                            "1",
                            "14|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B520",
                            "90",
                            "1",
                            "0",
                            "1",
                            "16|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B521",
                            "90",
                            "1",
                            "0",
                            "1",
                            "16|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B522",
                            "90",
                            "1",
                            "0",
                            "1",
                            "16|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B523",
                            "90",
                            "1",
                            "0",
                            "1",
                            "15|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B524",
                            "90",
                            "1",
                            "0",
                            "1",
                            "14|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B525",
                            "90",
                            "1",
                            "0",
                            "1",
                            "13|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B526",
                            "90",
                            "1",
                            "0",
                            "1",
                            "12|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B527",
                            "90",
                            "1",
                            "0",
                            "1",
                            "12|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B528",
                            "90",
                            "1",
                            "0",
                            "1",
                            "12|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B529",
                            "90",
                            "1",
                            "0",
                            "1",
                            "13|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B530",
                            "90",
                            "1",
                            "0",
                            "1",
                            "14|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B531",
                            "90",
                            "1",
                            "0",
                            "1",
                            "15|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B532",
                            "90",
                            "1",
                            "0",
                            "1",
                            "15|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B533",
                            "90",
                            "1",
                            "0",
                            "1",
                            "14|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B534",
                            "90",
                            "1",
                            "0",
                            "1",
                            "13|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B535",
                            "90",
                            "1",
                            "0",
                            "1",
                            "18|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B536",
                            "90",
                            "1",
                            "0",
                            "1",
                            "18|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B537",
                            "90",
                            "1",
                            "0",
                            "1",
                            "18|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B538",
                            "90",
                            "1",
                            "0",
                            "1",
                            "19|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B539",
                            "90",
                            "1",
                            "0",
                            "1",
                            "20|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B540",
                            "90",
                            "1",
                            "0",
                            "1",
                            "21|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B541",
                            "90",
                            "1",
                            "0",
                            "1",
                            "21|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B542",
                            "90",
                            "1",
                            "0",
                            "1",
                            "20|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B543",
                            "90",
                            "1",
                            "0",
                            "1",
                            "19|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B544",
                            "90",
                            "1",
                            "0",
                            "1",
                            "19|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B545",
                            "90",
                            "1",
                            "0",
                            "1",
                            "20|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B546",
                            "90",
                            "1",
                            "0",
                            "1",
                            "21|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B555",
                            "97",
                            "1",
                            "0",
                            "1",
                            "16|15",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B556",
                            "97",
                            "1",
                            "0",
                            "1",
                            "16|16",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B557",
                            "97",
                            "1",
                            "0",
                            "1",
                            "16|17",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B558",
                            "97",
                            "1",
                            "0",
                            "1",
                            "16|17",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B559",
                            "97",
                            "1",
                            "0",
                            "1",
                            "15|17",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B560",
                            "97",
                            "1",
                            "0",
                            "1",
                            "14|17",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B561",
                            "97",
                            "1",
                            "0",
                            "1",
                            "13|17",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B562",
                            "97",
                            "1",
                            "0",
                            "1",
                            "12|17",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B563",
                            "97",
                            "1",
                            "0",
                            "1",
                            "12|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B564",
                            "97",
                            "1",
                            "0",
                            "1",
                            "12|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B565",
                            "97",
                            "1",
                            "0",
                            "1",
                            "12|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B566",
                            "97",
                            "1",
                            "0",
                            "1",
                            "12|15",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B567",
                            "97",
                            "1",
                            "0",
                            "1",
                            "13|15",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B568",
                            "97",
                            "1",
                            "0",
                            "1",
                            "14|15",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B569",
                            "97",
                            "1",
                            "0",
                            "1",
                            "15|15",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B570",
                            "97",
                            "1",
                            "0",
                            "1",
                            "16|15",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B571",
                            "97",
                            "1",
                            "0",
                            "1",
                            "18|15",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B572",
                            "97",
                            "1",
                            "0",
                            "1",
                            "19|15",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B573",
                            "97",
                            "1",
                            "0",
                            "1",
                            "20|15",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B574",
                            "97",
                            "1",
                            "0",
                            "1",
                            "21|15",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B575",
                            "97",
                            "1",
                            "0",
                            "1",
                            "21|15",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B576",
                            "97",
                            "1",
                            "0",
                            "1",
                            "21|16",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B577",
                            "97",
                            "1",
                            "0",
                            "1",
                            "21|17",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B578",
                            "97",
                            "1",
                            "0",
                            "1",
                            "21|17",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B579",
                            "97",
                            "1",
                            "0",
                            "1",
                            "20|17",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B580",
                            "97",
                            "1",
                            "0",
                            "1",
                            "19|17",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B581",
                            "97",
                            "1",
                            "0",
                            "1",
                            "18|17",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B582",
                            "97",
                            "1",
                            "0",
                            "1",
                            "18|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B583",
                            "97",
                            "1",
                            "0",
                            "1",
                            "18|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B584",
                            "97",
                            "1",
                            "0",
                            "1",
                            "18|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B585",
                            "17",
                            "1",
                            "0",
                            "1",
                            "17|19",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B586",
                            "109",
                            "1",
                            "0",
                            "1",
                            "16|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B587",
                            "109",
                            "1",
                            "0",
                            "1",
                            "15|14",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B588",
                            "78",
                            "1",
                            "0",
                            "1",
                            "14|14",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B589",
                            "78",
                            "1",
                            "0",
                            "1",
                            "13|14",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B590",
                            "79",
                            "1",
                            "0",
                            "1",
                            "12|14",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B591",
                            "79",
                            "1",
                            "0",
                            "1",
                            "11|14",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B592",
                            "79",
                            "1",
                            "0",
                            "1",
                            "11|15",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B593",
                            "79",
                            "1",
                            "0",
                            "1",
                            "11|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B594",
                            "79",
                            "1",
                            "0",
                            "1",
                            "18|14",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B595",
                            "78",
                            "1",
                            "0",
                            "1",
                            "19|14",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B596",
                            "78",
                            "1",
                            "0",
                            "1",
                            "20|14",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B597",
                            "78",
                            "1",
                            "0",
                            "1",
                            "21|14",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B598",
                            "3",
                            "1",
                            "0",
                            "1",
                            "7|-5",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B599",
                            "9",
                            "1",
                            "0",
                            "1",
                            "0|-4",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B600",
                            "9",
                            "1",
                            "0",
                            "1",
                            "3|-5",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B601",
                            "29",
                            "1",
                            "0",
                            "1",
                            "9|4",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B602",
                            "35",
                            "1",
                            "0",
                            "1",
                            "8|14",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B603",
                            "26",
                            "1",
                            "0",
                            "1",
                            "14|7",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B604",
                            "26",
                            "1",
                            "0",
                            "1",
                            "14|8",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B605",
                            "78",
                            "1",
                            "0",
                            "1",
                            "11|19",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B606",
                            "78",
                            "1",
                            "0",
                            "1",
                            "12|19",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B607",
                            "79",
                            "1",
                            "0",
                            "1",
                            "13|19",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B608",
                            "79",
                            "1",
                            "0",
                            "1",
                            "11|20",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B609",
                            "109",
                            "1",
                            "0",
                            "1",
                            "11|21",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B610",
                            "109",
                            "1",
                            "0",
                            "1",
                            "11|22",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B611",
                            "109",
                            "1",
                            "0",
                            "1",
                            "12|20",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B616",
                            "18",
                            "1",
                            "0",
                            "1",
                            "2|13",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B617",
                            "18",
                            "1",
                            "0",
                            "1",
                            "1|-3",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B618",
                            "18",
                            "1",
                            "0",
                            "1",
                            "2|-3",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B619",
                            "18",
                            "1",
                            "0",
                            "1",
                            "3|-3",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B620",
                            "18",
                            "1",
                            "0",
                            "1",
                            "4|-3",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B621",
                            "18",
                            "1",
                            "0",
                            "1",
                            "10|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B622",
                            "18",
                            "1",
                            "0",
                            "1",
                            "10|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B623",
                            "18",
                            "1",
                            "0",
                            "1",
                            "10|-1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B624",
                            "18",
                            "1",
                            "0",
                            "1",
                            "10|-2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B626",
                            "37",
                            "1",
                            "0",
                            "1",
                            "10|-2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B627",
                            "37",
                            "1",
                            "0",
                            "1",
                            "10|-1",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B628",
                            "28",
                            "1",
                            "0",
                            "1",
                            "9|-2",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B629",
                            "147",
                            "1",
                            "0",
                            "1",
                            "3|-2",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B630",
                            "26",
                            "1",
                            "0",
                            "1",
                            "4|-1",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B631",
                            "26",
                            "1",
                            "0",
                            "1",
                            "3|-1",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B632",
                            "26",
                            "1",
                            "0",
                            "1",
                            "2|-1",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B633",
                            "26",
                            "1",
                            "0",
                            "1",
                            "2|0",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B634",
                            "26",
                            "1",
                            "0",
                            "1",
                            "4|2",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B635",
                            "83",
                            "1",
                            "0",
                            "1",
                            "1|-2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B636",
                            "83",
                            "1",
                            "0",
                            "1",
                            "1|-1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B637",
                            "83",
                            "1",
                            "0",
                            "1",
                            "1|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B638",
                            "83",
                            "1",
                            "0",
                            "1",
                            "1|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B639",
                            "83",
                            "1",
                            "0",
                            "1",
                            "1|2",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B644",
                            "85",
                            "1",
                            "0",
                            "1",
                            "10|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B645",
                            "26",
                            "1",
                            "0",
                            "1",
                            "13|7",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B646",
                            "26",
                            "1",
                            "0",
                            "1",
                            "13|8",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B647",
                            "94",
                            "1",
                            "0",
                            "1",
                            "12|7",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B648",
                            "94",
                            "1",
                            "0",
                            "1",
                            "13|7",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B654",
                            "83",
                            "1",
                            "0",
                            "1",
                            "13|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B655",
                            "83",
                            "1",
                            "0",
                            "1",
                            "13|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B656",
                            "83",
                            "1",
                            "0",
                            "1",
                            "13|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B657",
                            "83",
                            "1",
                            "0",
                            "1",
                            "12|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B658",
                            "83",
                            "1",
                            "0",
                            "1",
                            "11|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B659",
                            "83",
                            "1",
                            "0",
                            "1",
                            "10|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B660",
                            "83",
                            "1",
                            "0",
                            "1",
                            "9|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B661",
                            "169",
                            "1",
                            "0",
                            "1",
                            "2|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B662",
                            "18",
                            "1",
                            "0",
                            "1",
                            "1|9",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B663",
                            "18",
                            "1",
                            "0",
                            "1",
                            "1|8",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B664",
                            "18",
                            "1",
                            "0",
                            "1",
                            "1|7",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B667",
                            "144",
                            "1",
                            "0",
                            "1",
                            "9|6",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B668",
                            "83",
                            "1",
                            "0",
                            "1",
                            "3|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B669",
                            "83",
                            "1",
                            "0",
                            "1",
                            "2|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B670",
                            "83",
                            "1",
                            "0",
                            "1",
                            "1|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B671",
                            "83",
                            "1",
                            "0",
                            "1",
                            "1|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B672",
                            "83",
                            "1",
                            "0",
                            "1",
                            "1|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B673",
                            "79",
                            "1",
                            "0",
                            "1",
                            "10|1",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B674",
                            "79",
                            "1",
                            "0",
                            "1",
                            "10|0",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B675",
                            "110",
                            "1",
                            "0",
                            "1",
                            "7|12",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B676",
                            "110",
                            "1",
                            "0",
                            "1",
                            "8|12",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B677",
                            "110",
                            "1",
                            "0",
                            "1",
                            "9|12",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B678",
                            "79",
                            "1",
                            "0",
                            "1",
                            "5|12",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B679",
                            "79",
                            "1",
                            "0",
                            "1",
                            "4|12",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B680",
                            "79",
                            "1",
                            "0",
                            "1",
                            "3|12",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B681",
                            "78",
                            "1",
                            "0",
                            "1",
                            "2|12",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B682",
                            "97",
                            "1",
                            "0",
                            "1",
                            "8|10",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B683",
                            "97",
                            "1",
                            "0",
                            "1",
                            "8|10",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B684",
                            "97",
                            "1",
                            "0",
                            "1",
                            "8|9",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B685",
                            "26",
                            "1",
                            "0",
                            "1",
                            "11|10",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B686",
                            "26",
                            "1",
                            "0",
                            "1",
                            "12|10",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B687",
                            "26",
                            "1",
                            "0",
                            "1",
                            "11|12",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B688",
                            "26",
                            "1",
                            "0",
                            "1",
                            "12|12",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B689",
                            "83",
                            "1",
                            "0",
                            "1",
                            "13|12",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B691",
                            "83",
                            "1",
                            "0",
                            "1",
                            "14|12",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B692",
                            "94",
                            "1",
                            "0",
                            "1",
                            "14|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B693",
                            "94",
                            "1",
                            "0",
                            "1",
                            "14|11",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B694",
                            "94",
                            "1",
                            "0",
                            "1",
                            "14|11",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B695",
                            "94",
                            "1",
                            "0",
                            "1",
                            "15|11",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B696",
                            "94",
                            "1",
                            "0",
                            "1",
                            "16|11",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B697",
                            "94",
                            "1",
                            "0",
                            "1",
                            "16|7",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B698",
                            "94",
                            "1",
                            "0",
                            "1",
                            "17|7",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B699",
                            "94",
                            "1",
                            "0",
                            "1",
                            "18|7",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B700",
                            "94",
                            "1",
                            "0",
                            "1",
                            "11|10",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B701",
                            "94",
                            "1",
                            "0",
                            "1",
                            "12|10",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B702",
                            "94",
                            "1",
                            "0",
                            "1",
                            "12|10",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B703",
                            "94",
                            "1",
                            "0",
                            "1",
                            "12|10",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B704",
                            "94",
                            "1",
                            "0",
                            "1",
                            "12|12",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B705",
                            "94",
                            "1",
                            "0",
                            "1",
                            "11|12",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B706",
                            "94",
                            "1",
                            "0",
                            "1",
                            "12|12",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B707",
                            "97",
                            "1",
                            "0",
                            "1",
                            "5|17",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B708",
                            "97",
                            "1",
                            "0",
                            "1",
                            "5|16",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B709",
                            "97",
                            "1",
                            "0",
                            "1",
                            "5|17",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B710",
                            "97",
                            "1",
                            "0",
                            "1",
                            "4|17",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B711",
                            "97",
                            "1",
                            "0",
                            "1",
                            "4|17",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B712",
                            "97",
                            "1",
                            "0",
                            "1",
                            "4|16",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B713",
                            "97",
                            "1",
                            "0",
                            "1",
                            "4|16",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B714",
                            "97",
                            "1",
                            "0",
                            "1",
                            "5|16",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B715",
                            "97",
                            "1",
                            "0",
                            "1",
                            "5|19",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B716",
                            "97",
                            "1",
                            "0",
                            "1",
                            "5|20",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B717",
                            "97",
                            "1",
                            "0",
                            "1",
                            "5|20",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B718",
                            "97",
                            "1",
                            "0",
                            "1",
                            "4|20",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B719",
                            "97",
                            "1",
                            "0",
                            "1",
                            "4|20",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B720",
                            "97",
                            "1",
                            "0",
                            "1",
                            "4|19",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B721",
                            "97",
                            "1",
                            "0",
                            "1",
                            "4|19",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B722",
                            "97",
                            "1",
                            "0",
                            "1",
                            "5|19",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B723",
                            "110",
                            "1",
                            "0",
                            "1",
                            "0|7",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B724",
                            "147",
                            "1",
                            "0",
                            "1",
                            "22|11",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B725",
                            "147",
                            "1",
                            "0",
                            "1",
                            "20|13",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B726",
                            "147",
                            "1",
                            "0",
                            "1",
                            "14|13",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B727",
                            "83",
                            "1",
                            "0",
                            "1",
                            "20|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B728",
                            "83",
                            "1",
                            "0",
                            "1",
                            "21|10",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B729",
                            "83",
                            "1",
                            "0",
                            "1",
                            "20|12",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B730",
                            "83",
                            "1",
                            "0",
                            "1",
                            "19|12",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B731",
                            "83",
                            "1",
                            "0",
                            "1",
                            "18|12",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B733",
                            "148",
                            "1",
                            "0",
                            "1",
                            "9|23",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B734",
                            "148",
                            "1",
                            "0",
                            "1",
                            "3|23",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B735",
                            "148",
                            "1",
                            "0",
                            "1",
                            "6|23",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B736",
                            "20",
                            "1",
                            "0",
                            "1",
                            "2|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B737",
                            "79",
                            "1",
                            "0",
                            "1",
                            "3|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B738",
                            "78",
                            "1",
                            "0",
                            "1",
                            "4|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B739",
                            "78",
                            "1",
                            "0",
                            "1",
                            "5|21",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B740",
                            "78",
                            "1",
                            "0",
                            "1",
                            "7|20",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B741",
                            "109",
                            "1",
                            "0",
                            "1",
                            "7|21",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B742",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|7",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B743",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B744",
                            "18",
                            "1",
                            "0",
                            "1",
                            "11|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B745",
                            "26",
                            "1",
                            "0",
                            "1",
                            "12|8",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B746",
                            "26",
                            "1",
                            "0",
                            "1",
                            "12|7",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B747",
                            "94",
                            "1",
                            "0",
                            "1",
                            "13|8",
                            "3",
                            "0"
                        },
                        new List<string>
                        {
                            "B748",
                            "94",
                            "1",
                            "0",
                            "1",
                            "12|7",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B749",
                            "83",
                            "1",
                            "0",
                            "1",
                            "12|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B750",
                            "83",
                            "1",
                            "0",
                            "1",
                            "11|9",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B751",
                            "78",
                            "1",
                            "0",
                            "1",
                            "11|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B752",
                            "78",
                            "1",
                            "0",
                            "1",
                            "11|7",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B753",
                            "79",
                            "1",
                            "0",
                            "1",
                            "11|6",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B754",
                            "79",
                            "1",
                            "0",
                            "1",
                            "11|5",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B755",
                            "110",
                            "1",
                            "0",
                            "1",
                            "7|19",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B756",
                            "110",
                            "1",
                            "0",
                            "1",
                            "8|20",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B757",
                            "110",
                            "1",
                            "0",
                            "1",
                            "8|21",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B758",
                            "142",
                            "1",
                            "0",
                            "1",
                            "5|5",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B760",
                            "110",
                            "1",
                            "0",
                            "1",
                            "4|7",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B761",
                            "94",
                            "1",
                            "0",
                            "1",
                            "11|10",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B763",
                            "147",
                            "1",
                            "0",
                            "1",
                            "-4|5",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B764",
                            "171",
                            "1",
                            "0",
                            "1",
                            "-2|8",
                            "0",
                            "0"
                        },
                        new List<string>
                        {
                            "B765",
                            "18",
                            "1",
                            "0",
                            "1",
                            "-3|7",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B766",
                            "18",
                            "1",
                            "0",
                            "1",
                            "-3|6",
                            "2",
                            "0"
                        },
                        new List<string>
                        {
                            "B767",
                            "167",
                            "1",
                            "0",
                            "1",
                            "6|12",
                            "1",
                            "0"
                        },
                        new List<string>
                        {
                            "B768",
                            "10",
                            "1",
                            "0",
                            "1",
                            "1|-5",
                            "1",
                            "0"
                        }
                    };
                    Mainload.Prop_have = new List<List<string>>
                    {
                        new List<string>
                        {
                            "60",
                            "10"
                        },
                        new List<string>
                        {
                            "61",
                            "13"
                        },
                        new List<string>
                        {
                            "94",
                            "5"
                        },
                        new List<string>
                        {
                            "95",
                            "3"
                        },
                        new List<string>
                        {
                            "169",
                            "2"
                        }
                    };
                    Mainload.FamilyData[2] = "38";
                    Mainload.CGNum[0] = "6897000";
                }
                else if (h == 1)
                {
                    Mainload.ShiJia_king = new List<string>
                    {
                        "null",
                        "20",
                        "0",
                        "5",
                        "null",
                        "null"
                    };
                    Mainload.KingCityData_now = new List<string>
                    {
                        "50000|" + (TrueRandom.GetRanom(20) + 80).ToString(),
                        "-2|null",
                        "-2|null",
                        "-2|null",
                        "-2|null",
                        "-2|null",
                        "-2|null",
                        "-2|null",
                        "-2|null",
                        "-2|null",
                        "-2|null",
                        "-2|null",
                        "-2|null",
                        "1000000",
                        "null",
                        "1"
                    };
                    Mainload.Member_First = new List<string>
                    {
                        "MN001",
                        "null",
                        "2|3|0|7",
                        "1",
                        "5@5@1@-1@-1|0",
                        "1",
                        "65"
                    };
                    Mainload.MenKe_Now = new List<List<string>>
                    {
                        new List<string>
                        {
                            "MK001",
                            FormulaData.Create_LiHuiID(4),
                            RandName.GetXing_Name("1") + "|1|0|0|1|75|0|50|1|null",
                            "45",
                            "60",
                            "10",
                            "10",
                            "40",
                            "80",
                            "0|B1|null",
                            "0",
                            "20",
                            "0",
                            "30",
                            "80",
                            "60",
                            "0",
                            "-1",
                            "1000",
                            "10",
                            "0",
                            "null"
                        }
                    };
                    Mainload.Member_qu = new List<List<string>>
                    {
                        new List<string>
                        {
                            "MN021",
                            FormulaData.Create_LiHuiID(0),
                            RandName.GetMemberNameShijia("0", -100, -1) + "|-100|0|0|0|80|0|35|1|MN002|3|null",
                            "MN004|MN008",
                            "0|BMM02|null|1",
                            "46",
                            "31",
                            "10",
                            "12",
                            "40",
                            "65",
                            "0",
                            "38",
                            "-1",
                            "null|null|null",
                            "20",
                            "55",
                            "null",
                            "-1",
                            "56",
                            "10",
                            "0|0|0|0|0|0|0",
                            "0|0|0|0|0|0",
                            "0",
                            "0",
                            "0",
                            "null",
                            "null",
                            "null",
                            "0",
                            "0@0@0@-1@-1|0",
                            "80|70",
                            "-1|0|0"
                        },
                        new List<string>
                        {
                            "MN022",
                            FormulaData.Create_LiHuiID(0),
                            RandName.GetMemberNameShijia("0", -100, -1) + "|-100|0|0|0|87|0|40|2|MN004|0|null",
                            "MN009",
                            "0|BMM03|null|1",
                            "27",
                            "32",
                            "8",
                            "16",
                            "32",
                            "80",
                            "0",
                            "20",
                            "-1",
                            "null|null|null",
                            "40",
                            "70",
                            "null",
                            "-1",
                            "43",
                            "10",
                            "0|0|0|0|0|0|0",
                            "0|0|0|0|0|0",
                            "0",
                            "0",
                            "0",
                            "null",
                            "null",
                            "null",
                            "0",
                            "0@0@0@-1@-1|0",
                            "85|70",
                            "-1|0|0"
                        },
                        new List<string>
                        {
                            "MN023",
                            FormulaData.Create_LiHuiID(0),
                            RandName.GetMemberNameShijia("0", -100, -1) + "|-100|0|0|0|88|0|42|3|MN003|4|null",
                            "MN005|MN006|MN007",
                            "0|BMM04|null|1",
                            "40",
                            "25",
                            "6",
                            "8",
                            "20",
                            "85",
                            "0",
                            "24",
                            "-1",
                            "null|null|null",
                            "62",
                            "85",
                            "null",
                            "-1",
                            "30",
                            "10",
                            "0|0|0|0|0|0|0",
                            "0|0|0|0|0|0",
                            "0",
                            "0",
                            "0",
                            "null",
                            "null",
                            "null",
                            "0",
                            "0@0@0@-1@-1|0",
                            "95|80",
                            "-1|0|0"
                        },
                        new List<string>
                        {
                            "MN024",
                            FormulaData.Create_LiHuiID(0),
                            RandName.GetMemberNameShijia("0", -100, -1) + "|-100|0|0|0|90|0|28|4|MN005|5|null",
                            "MN010",
                            "0|BMM05|null|1",
                            "20",
                            "20",
                            "10",
                            "15",
                            "22",
                            "60",
                            "0",
                            "23",
                            "-1",
                            "null|null|null",
                            "60",
                            "80",
                            "null",
                            "-1",
                            "20",
                            "10",
                            "0|0|0|0|0|0|0",
                            "0|0|0|0|0|0",
                            "0",
                            "0",
                            "0",
                            "null",
                            "null",
                            "null",
                            "0",
                            "0@0@0@-1@-1|0",
                            "92|80",
                            "-1|0|0"
                        }
                    };
                }
                else if (h == 2)
                {
                    Mainload.Fudi_now = new List<List<string>>
                    {
                        new List<string>
                        {
                            "0|7",
                            RandName.GetFudiName(),
                            "20",
                            "0",
                            "0",
                            (Mainload.Member_qu.Count + Mainload.Member_now.Count).ToString(),
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "50",
                            "60",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "25",
                            "0",
                            "0"
                        }
                    };
                    Mainload.CangShuGeData_Now = new List<List<string>>();
                    Mainload.CangShuGeData_Now.Add(new List<string>());
                    SaveData.SaveBuild("M", "0", "0", true);
                }
                else if (h == 3)
                {
                    for (int i = 0; i < Mainload.AllFengDiData.Count; i++)
                    {
                        if (i == 0)
                        {
                            Mainload.Fengdi_now.Add(new List<string>
                            {
                                "1",
                                "null",
                                "0|0|0",
                                "null"
                            });
                            Mainload.NongZ_now.Add(new List<List<string>>());
                            Mainload.ZhuangTou_now.Add(new List<List<List<string>>>());
                            Mainload.Kuang_now.Add(new List<List<string>>());
                            Mainload.Shan_now.Add(new List<List<string>>());
                            Mainload.Hu_Now.Add(new List<List<string>>());
                            Mainload.Sen_Now.Add(new List<List<string>>());
                            Mainload.Kong_now.Add(new List<List<string>>());
                            Mainload.Cun_now.Add(new List<List<string>>());
                            Mainload.Zhen_now.Add(new List<List<string>>());
                            Mainload.JunYing_now.Add(new List<List<string>>());
                        }
                        else
                        {
                            List<int> list = new List<int>
                            {
                                TrueRandom.GetRanom(15) + 5,
                                TrueRandom.GetRanom(15) + 5,
                                TrueRandom.GetRanom(15) + 5
                            };
                            Mainload.Fengdi_now.Add(new List<string>
                            {
                                "0",
                                "null",
                                string.Concat(new string[]
                                {
                                    list[0].ToString(),
                                    "|",
                                    list[1].ToString(),
                                    "|",
                                    list[2].ToString()
                                }),
                                "null"
                            });
                            Mainload.ZhuangTou_now.Add(new List<List<List<string>>>());
                            Mainload.NongZ_now.Add(new List<List<string>>());
                            Mainload.Kuang_now.Add(new List<List<string>>());
                            Mainload.Shan_now.Add(new List<List<string>>());
                            Mainload.Hu_Now.Add(new List<List<string>>());
                            Mainload.Sen_Now.Add(new List<List<string>>());
                            Mainload.Kong_now.Add(new List<List<string>>());
                            Mainload.Cun_now.Add(new List<List<string>>());
                            Mainload.Zhen_now.Add(new List<List<string>>());
                            Mainload.JunYing_now.Add(new List<List<string>>());
                        }
                    }
                }
                else if (h == 4)
                {
                    for (int j = 4; j < Mainload.AllShenFenData[5].Count; j++)
                    {
                        Mainload.Guan_JingCheng.Add(new List<string>());
                        for (int k = 0; k < AllText.Text_AllShenFen[5][j][Mainload.SetData[4]].Split(new char[]
                        {
                            '|'
                        }).Length; k++)
                        {
                            Mainload.Guan_JingCheng[Mainload.Guan_JingCheng.Count - 1].Add("-2|null");
                        }
                    }
                }
                else if (h == 5)
                {
                    Mainload.ShopData_updateTime = new List<List<int>>();
                    Mainload.Prop_shop_temp = new List<List<List<string>>>();
                    Mainload.Horse_Shop_Temp = new List<List<List<List<string>>>>();
                    Mainload.HuaiZhang_Shop_Temp = new List<List<List<List<string>>>>();
                    Mainload.OtherTrade_shop_Temp = new List<List<List<List<string>>>>();
                    Mainload.Trade_Playershop = new List<List<List<List<string>>>>();
                    Mainload.ShangHui_now = new List<List<string>>();
                    Mainload.PropPrice_Now = new List<List<string>>();
                    Mainload.ShijiaOutXianPoint = new List<List<int>>();
                }
                else if (h >= 6 && h <= 17)
                {
                    int num = h - 6;
                    Mainload.ShangHui_now.Add(new List<string>
                    {
                        "null",
                        "null",
                        "0|0",
                        "0",
                        "0"
                    });
                    Mainload.PropPrice_Now.Add(new List<string>
                    {
                        "2|100",
                        "3|100",
                        "4|100",
                        "5|100",
                        "16|100",
                        "17|100",
                        "18|100",
                        "19|100",
                        "39|100",
                        "38|100",
                        "37|100",
                        "36|100",
                        "34|100",
                        "33|100",
                        "32|100",
                        "31|100",
                        "30|100",
                        "29|100"
                    });
                    Mainload.Doctor_now.Add(new List<List<string>>());
                    Mainload.Member_Qinglou.Add(new List<List<string>>());
                    Mainload.ShopData_updateTime.Add(new List<int>());
                    Mainload.Prop_shop_temp.Add(new List<List<string>>());
                    Mainload.Horse_Shop_Temp.Add(new List<List<List<string>>>());
                    Mainload.HuaiZhang_Shop_Temp.Add(new List<List<List<string>>>());
                    Mainload.OtherTrade_shop_Temp.Add(new List<List<List<string>>>());
                    Mainload.Trade_Playershop.Add(new List<List<List<string>>>());
                    Mainload.ShijiaOutXianPoint.Add(new List<int>());
                    string text = TrueRandom.GetRanom(Mainload.AllPanJunData.Count - 1).ToString();
                    string item = "0";
                    if (num == 0 || num == int.Parse(Mainload.FamilyData[0].Split(new char[]
                    {
                        '|'
                    })[0]))
                    {
                        text = "-1";
                        item = "2";
                        Mainload.ShijiaOutXianPoint[num] = new List<int>
                        {
                            0,
                            2,
                            4,
                            6
                        };
                    }
                    else
                    {
                        Mainload.ShijiaOutXianPoint[num] = new List<int>
                        {
                            0,
                            1
                        };
                    }
                    string item2 = string.Concat(new string[]
                    {
                        text,
                        "|",
                        ((TrueRandom.GetRanom(5) + 5) * 5000).ToString(),
                        "|",
                        (TrueRandom.GetRanom(80) + 20).ToString(),
                        "|0"
                    });
                    List<List<string>> list2 = new List<List<string>>
                    {
                        new List<string>
                        {
                            item2,
                            "-2|null",
                            "-2|null",
                            "-2|null",
                            "-2|null",
                            "0",
                            "null",
                            ((TrueRandom.GetRanom(20) + 130) * 10000).ToString(),
                            TrueRandom.GetRanom(10).ToString(),
                            ((TrueRandom.GetRanom(10) + 10) * 10000).ToString(),
                            "0",
                            "0",
                            Mainload.All_City[num][4],
                            "0",
                            "0",
                            item,
                            "0|0|0|0",
                            "0",
                            "0",
                            "0",
                            "1|1",
                            "0",
                            "0",
                            "0|0",
                            "0|0",
                            "0",
                            "0",
                            "0",
                            "0",
                            FormulaData.AddPanJunEvent(num, true)
                        }
                    };
                    int num2 = AllText.Text_City[num][Mainload.SetData[4]].Split(new char[]
                    {
                        '~'
                    })[1].Split(new char[]
                    {
                        '|'
                    }).Length;
                    for (int l = 0; l < num2; l++)
                    {
                        string item3 = string.Concat(new string[]
                        {
                            "-1|",
                            ((TrueRandom.GetRanom(5) + 5) * 1000).ToString(),
                            "|",
                            (TrueRandom.GetRanom(80) + 20).ToString(),
                            "|0"
                        });
                        if (num == 0)
                        {
                            if (l == 0)
                            {
                                Mainload.ShiJia_Now.Add(new List<string>
                                {
                                    "0",
                                    RandName.GetXingShiOnly(),
                                    "25",
                                    "0",
                                    "0",
                                    num.ToString() + "|" + l.ToString(),
                                    "4",
                                    "0",
                                    "null",
                                    "0",
                                    TrueRandom.GetRanom(InitFudiBuild.AllFudidata.Count).ToString(),
                                    FormulaData.JunNum_Shijia(25, "1"),
                                    "100"
                                });
                                Mainload.Member_other.Add(new List<List<string>>());
                                Mainload.Member_Other_qu.Add(new List<List<string>>());
                                FormulaData.CreatNewNongZOutFengdi(Mainload.ShiJia_Now.Count - 1, num, l);
                                Mainload.Member_other[Mainload.ShiJia_Now.Count - 1] = new List<List<string>>
                                {
                                    new List<string>
                                    {
                                        "MSJ001",
                                        "1|1|0|0",
                                        RandName.GetMemberNameShijia("1", 0, 1) + "|1|1|4|1|90|0|20|2|null",
                                        "75",
                                        "89",
                                        "15",
                                        "12",
                                        "25",
                                        "50",
                                        "5@4@3@-1@-1|0",
                                        "9",
                                        "3|2",
                                        "-1",
                                        "null",
                                        "MSJ003|MSJ005",
                                        "70",
                                        "0",
                                        "80",
                                        "-1",
                                        "30",
                                        "70",
                                        "2",
                                        "56",
                                        "1",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ002",
                                        "14|5|0|4",
                                        RandName.GetMemberNameShijia("1", 0, 1) + "|2|0|0|1|91|1|22|1|null",
                                        "60",
                                        "26",
                                        "20",
                                        "21",
                                        "18",
                                        "45",
                                        "0@0@0@-1@-1|0",
                                        "1",
                                        "0|0",
                                        "-1",
                                        "null",
                                        "MSJ004",
                                        "65",
                                        "0",
                                        "34",
                                        "-1",
                                        "20",
                                        "75",
                                        "1",
                                        "33",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "3",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ003",
                                        "6|4|0|10",
                                        RandName.GetMemberNameShijia("1", 0, 1) + "|3|1|5|1|93|2|19|7|MSJ001",
                                        "43",
                                        "70",
                                        "18",
                                        "11",
                                        "12",
                                        "55",
                                        "5@3@1@0@-1|0",
                                        "6",
                                        "0|0",
                                        "-1",
                                        "43@-1@0@MN002@1",
                                        "MSJ006|MSJ007|MSJ009",
                                        "60",
                                        "0",
                                        "48",
                                        "-1",
                                        "35",
                                        "90",
                                        "1",
                                        "46",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "2",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ006",
                                        "9|13|1|1",
                                        RandName.GetMemberNameShijia("1", 0, 1) + "|4|2|6|1|85|0|32|6|MSJ003",
                                        "23",
                                        "20",
                                        "68",
                                        "8",
                                        "10",
                                        "60",
                                        "5@2@2@0@-1|0",
                                        "6",
                                        "0|0",
                                        "-1",
                                        "null",
                                        "MSJ015",
                                        "55",
                                        "0",
                                        "22",
                                        "-1",
                                        "40",
                                        "95",
                                        "1",
                                        "35",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ015",
                                        "5|15|2|7",
                                        RandName.GetMemberNameShijia("1", 0, 1) + "|5|1|1|1|80|0|26|3|MSJ006",
                                        "3",
                                        "10",
                                        "2",
                                        "1",
                                        "1",
                                        "100",
                                        "0@0@0@-1@-1|0",
                                        "0",
                                        "0|0",
                                        "-1",
                                        "null",
                                        "null",
                                        "10",
                                        "0",
                                        "1",
                                        "-1",
                                        "6",
                                        "100",
                                        "0",
                                        "2",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ007",
                                        "10|3|0|9",
                                        RandName.GetMemberNameShijia("1", 0, 1) + "|6|2|3|1|82|0|35|12|MSJ003",
                                        "22",
                                        "13",
                                        "40",
                                        "10",
                                        "12",
                                        "65",
                                        "5@1@1@0@0|0",
                                        "6",
                                        "0|0",
                                        "-1",
                                        "null",
                                        "null",
                                        "65",
                                        "0",
                                        "30",
                                        "-1",
                                        "35",
                                        "100",
                                        "1",
                                        "26",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ009",
                                        "3|7|0|2",
                                        RandName.GetMemberNameShijia("0", 0, 1) + "|0|4|4|0|83|0|36|4|MSJ003",
                                        "20",
                                        "22",
                                        "5",
                                        "10",
                                        "45",
                                        "70",
                                        "0@0@0@-1@-1|0",
                                        "0",
                                        "0|0",
                                        "-1",
                                        "null",
                                        "null",
                                        "66",
                                        "0",
                                        "18",
                                        "-1",
                                        "45",
                                        "90",
                                        "0",
                                        "30",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ004",
                                        "4|16|0|6",
                                        RandName.GetMemberNameShijia("1", 0, 1) + "|7|2|2|1|90|0|18|10|MSJ001",
                                        "40",
                                        "12",
                                        "88",
                                        "8",
                                        "4",
                                        "55",
                                        "5@2@2@2@-1|0",
                                        "0",
                                        "0|0",
                                        "-1",
                                        "null",
                                        "MSJ008|MSJ010|MSJ011",
                                        "72",
                                        "0",
                                        "42",
                                        "-1",
                                        "25",
                                        "88",
                                        "1",
                                        "45",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ008",
                                        "11|9|1|8",
                                        RandName.GetMemberNameShijia("0", 0, 1) + "|8|4|5|0|94|0|29|12|MSJ004",
                                        "19",
                                        "21",
                                        "8",
                                        "10",
                                        "66",
                                        "82",
                                        "0@0@0@-1@-1|0",
                                        "1",
                                        "0|0",
                                        "-1",
                                        "null",
                                        "null",
                                        "39",
                                        "0",
                                        "22",
                                        "-1",
                                        "32",
                                        "96",
                                        "0",
                                        "20",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ010",
                                        "12|10|0|11",
                                        RandName.GetMemberNameShijia("1", 0, 1) + "|9|2|4|1|86|0|40|11|MSJ004",
                                        "18",
                                        "10",
                                        "35",
                                        "10",
                                        "6",
                                        "90",
                                        "0@0@0@-1@-1|0",
                                        "0",
                                        "0|0",
                                        "-1",
                                        "null",
                                        "null",
                                        "42",
                                        "0",
                                        "19",
                                        "-1",
                                        "24",
                                        "98",
                                        "0",
                                        "18",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ011",
                                        "7|8|0|3",
                                        RandName.GetMemberNameShijia("1", 0, 1) + "|1|0|0|1|92|0|38|2|MSJ004",
                                        "16",
                                        "11",
                                        "14",
                                        "8",
                                        "6",
                                        "95",
                                        "0@0@0@-1@-1|0",
                                        "0",
                                        "0|0",
                                        "-1",
                                        "null",
                                        "null",
                                        "30",
                                        "0",
                                        "20",
                                        "-1",
                                        "20",
                                        "83",
                                        "0",
                                        "16",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ005",
                                        "13|14|0|14",
                                        RandName.GetMemberNameShijia("1", 0, 1) + "|4|1|7|1|88|0|27|7|MSJ001",
                                        "39",
                                        "24",
                                        "6",
                                        "21",
                                        "91",
                                        "62",
                                        "0@0@0@-1@-1|0",
                                        "2",
                                        "0|0",
                                        "-1",
                                        "null",
                                        "MSJ012|MSJ013|MSJ014",
                                        "55",
                                        "0",
                                        "33",
                                        "-1",
                                        "26",
                                        "82",
                                        "1",
                                        "60",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ012",
                                        "8|11|0|12",
                                        RandName.GetMemberNameShijia("1", 0, 1) + "|5|1|4|1|80|0|55|9|MSJ005",
                                        "18",
                                        "32",
                                        "12",
                                        "10",
                                        "8",
                                        "71",
                                        "0@0@0@-1@-1|0",
                                        "1",
                                        "0|0",
                                        "-1",
                                        "null",
                                        "null",
                                        "30",
                                        "0",
                                        "12",
                                        "-1",
                                        "20",
                                        "89",
                                        "1",
                                        "28",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ013",
                                        "15|12|0|15",
                                        RandName.GetMemberNameShijia("0", 0, 1) + "|0|4|6|0|75|0|35|8|MSJ005",
                                        "16",
                                        "19",
                                        "6",
                                        "12",
                                        "43",
                                        "82",
                                        "0@0@0@-1@-1|0",
                                        "0",
                                        "0|0",
                                        "-1",
                                        "null",
                                        "null",
                                        "26",
                                        "0",
                                        "16",
                                        "-1",
                                        "28",
                                        "95",
                                        "1",
                                        "34",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    },
                                    new List<string>
                                    {
                                        "MSJ014",
                                        "2|2|0|5",
                                        RandName.GetMemberNameShijia("0", 0, 1) + "|8|0|0|0|79|0|21|0|MSJ005",
                                        "15",
                                        "8",
                                        "10",
                                        "8",
                                        "6",
                                        "86",
                                        "0@0@0@-1@-1|0",
                                        "0",
                                        "0|0",
                                        "-1",
                                        "null",
                                        "null",
                                        "22",
                                        "0",
                                        "10",
                                        "-1",
                                        "33",
                                        "100",
                                        "1",
                                        "18",
                                        "0",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "null",
                                        "null",
                                        "0",
                                        "null",
                                        "0",
                                        "60"
                                    }
                                };
                                Mainload.Member_Other_qu[Mainload.ShiJia_Now.Count - 1] = new List<List<string>>
                                {
                                    new List<string>
                                    {
                                        "MSJ015",
                                        "7|3|0|2",
                                        RandName.GetMemberNameShijia("0", -100, -1) + "|-100|0|0|0|95|0|30|1|MSJ002|0|null",
                                        "55",
                                        "22",
                                        "2",
                                        "4",
                                        "30",
                                        "70",
                                        "0",
                                        "40",
                                        "-1",
                                        "30",
                                        "-1",
                                        "null",
                                        "20",
                                        "60",
                                        "38",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "85|85",
                                        "null",
                                        "0",
                                        "MSJ004",
                                        "null"
                                    },
                                    new List<string>
                                    {
                                        "MSJ016",
                                        "10|26|1|19",
                                        RandName.GetMemberNameShijia("0", -100, -1) + "|-100|0|0|0|95|0|30|2|MSJ003|1|null",
                                        "42",
                                        "26",
                                        "6",
                                        "6",
                                        "42",
                                        "65",
                                        "0",
                                        "38",
                                        "-1",
                                        "22",
                                        "-1",
                                        "null",
                                        "30",
                                        "65",
                                        "20",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "95|90",
                                        "null",
                                        "0",
                                        "MSJ006|MSJ007|MSJ009",
                                        "null"
                                    },
                                    new List<string>
                                    {
                                        "MSJ017",
                                        "15|18|0|10",
                                        RandName.GetMemberNameShijia("0", -100, -1) + "|-100|0|0|0|95|0|30|3|MSJ006|2|null",
                                        "21",
                                        "23",
                                        "4",
                                        "5",
                                        "40",
                                        "90",
                                        "0",
                                        "22",
                                        "-1",
                                        "10",
                                        "-1",
                                        "null",
                                        "45",
                                        "95",
                                        "25",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "100|90",
                                        "null",
                                        "0",
                                        "MSJ015",
                                        "null"
                                    },
                                    new List<string>
                                    {
                                        "MSJ018",
                                        "11|28|0|18",
                                        RandName.GetMemberNameShijia("0", -100, -1) + "|-100|0|0|0|95|0|30|4|MSJ007|3|null",
                                        "20",
                                        "18",
                                        "8",
                                        "10",
                                        "28",
                                        "100",
                                        "0",
                                        "23",
                                        "-1",
                                        "12",
                                        "-1",
                                        "null",
                                        "48",
                                        "90",
                                        "28",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "86|50",
                                        "null",
                                        "0",
                                        "null",
                                        "null"
                                    },
                                    new List<string>
                                    {
                                        "MSJ019",
                                        "2|22|1|12",
                                        RandName.GetMemberNameShijia("0", -100, -1) + "|-100|0|0|0|95|0|30|5|MSJ004|4|null",
                                        "38",
                                        "25",
                                        "3",
                                        "12",
                                        "49",
                                        "95",
                                        "0",
                                        "35",
                                        "-1",
                                        "30",
                                        "-1",
                                        "null",
                                        "60",
                                        "68",
                                        "22",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "92|80",
                                        "null",
                                        "0",
                                        "MSJ008|MSJ010|MSJ011",
                                        "null"
                                    },
                                    new List<string>
                                    {
                                        "MSJ020",
                                        "18|9|0|15",
                                        RandName.GetMemberNameShijia("0", -100, -1) + "|-100|0|0|0|95|0|30|6|MSJ005|5|null",
                                        "36",
                                        "30",
                                        "5",
                                        "8",
                                        "43",
                                        "60",
                                        "0",
                                        "36",
                                        "-1",
                                        "20",
                                        "-1",
                                        "null",
                                        "55",
                                        "80",
                                        "18",
                                        "0|0|0|0|0|0|0",
                                        "0",
                                        "90|70",
                                        "null",
                                        "0",
                                        "MSJ012|MSJ013|MSJ014",
                                        "null"
                                    }
                                };
                            }
                            else
                            {
                                FormulaData.NewShijiaData(num, l, true);
                            }
                        }
                        else
                        {
                            item3 = string.Concat(new string[]
                            {
                                text,
                                "|",
                                ((TrueRandom.GetRanom(5) + 5) * 1000).ToString(),
                                "|",
                                (TrueRandom.GetRanom(80) + 20).ToString(),
                                "|0"
                            });
                        }
                        int renNum = (TrueRandom.GetRanom(20) + 10) * 10000;
                        int num3 = FormulaData.XianTaxYear(renNum);
                        list2.Add(new List<string>
                        {
                            "-2|null",
                            "-2|null",
                            "-2|null",
                            renNum.ToString(),
                            TrueRandom.GetRanom(10).ToString(),
                            item3,
                            "null",
                            num3.ToString(),
                            "0",
                            "100"
                        });
                    }
                    Mainload.CityData_now.Add(list2);
                    Mainload.CityData_now[Mainload.CityData_now.Count - 1][0][9] = FormulaData.CityTaxYear(int.Parse(Mainload.CityData_now[Mainload.CityData_now.Count - 1][0][7]), int.Parse(Mainload.CityData_now[Mainload.CityData_now.Count - 1][0][10])).ToString();
                }
                if (h >= 17)
                {
                    base.StopCoroutine("InitPreGameData");
                    base.transform.parent.parent.Find("LoadPanel").GetComponent<LoadPanel>().SwitchScene();
                }
                yield return null;
                num4 = h;
            }
            yield break;
        }


    }
}
