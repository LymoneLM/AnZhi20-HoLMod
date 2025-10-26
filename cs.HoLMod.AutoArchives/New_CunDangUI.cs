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
    public class New_CunDangUI : MonoBehaviour
    {
        public string currentCunDangIndex_A;
        private bool isHaveData;

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
            transform.Find("NoDataTip").Find("Tip").GetComponent<Text>().text = "没有找到存档";
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
            bool flag = true;
		    try
		    {
			    if (ES3.FileExists("FW/" + base.name + "/GameData.es3"))
			    {
				    ES3.Load<List<string>>("FamilyData", "FW/" + base.name + "/GameData.es3");
			    }
		    }
		    catch (FormatException)
            {
		    
			    flag = false;
		    }
		    if (flag)
		    {
			    this.isHaveData = ES3.FileExists("FW/" + base.name + "/GameData.es3");
			    return;
		    }
		    ES3.DeleteDirectory("FW/" + base.name);
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
            // 设置标题
            transform.Find("Title").GetComponent<Text>().text = "选择存档";
            transform.Find("DelCunDPanel").gameObject.SetActive(false);
            
            // 添加动画效果
            transform.localPosition = new Vector3(0f, 500f, 0f);
            transform.DOLocalMoveY(0f, 0.3f, false).SetEase(Ease.OutBack, 1f);

            //有存档的情况
            if (this.isHaveData)
            {
                List<string> list = ES3.Load<List<string>>("FamilyData", "FW/" + base.name + "/GameData.es3");
			    List<int> list2 = ES3.Load<List<int>>("Time_now", "FW/" + base.name + "/GameData.es3");
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

        //人物显示
        private void MemberShow()
        {
            List<string> list = ES3.Load<List<string>>("Member_First", "FW/" + base.name + "/GameData.es3");
            
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

        private void ClickBT()
        {
            // 记录当前点击的存档索引
            currentCunDangIndex_A = base.name;
            
            // 隐藏当前窗口，显示Bak_CunDangUI窗口
            gameObject.SetActive(false);
            transform.parent.Find("Bak_CunDangUI").gameObject.SetActive(true);
        }

        private void CloseBT()
        {
            // 返回StartGameUI
            gameObject.SetActive(false);
            transform.parent.Find("StartGameUI").gameObject.SetActive(true);
        }
    }
}
