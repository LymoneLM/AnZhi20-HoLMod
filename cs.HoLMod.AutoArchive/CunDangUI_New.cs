using System;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace cs.HoLMod.AutoArchive
{
    public class CunDangUI_New : MonoBehaviour
    {
        public string currentCunDangIndex_A;
        private bool isHaveData;

		
        private void Start()
	    {
            AllText.Text_UIA[31] = new List<string>()
            {
            "（空白存档）",
            "(Empty Slot)"
            };
		    
            // 添加空引用检查
            Transform closeBT = transform.Find("CloseBT");
            if (closeBT != null)
            {
                Button closeButton = closeBT.GetComponent<Button>();
                if (closeButton != null)
                {
                    closeButton.onClick.AddListener(new UnityAction(this.CloseBT));
                }
            }
            
            Transform back = transform.Find("Back");
            if (back != null)
            {
                Button backButton = back.GetComponent<Button>();
                if (backButton != null)
                {
                    backButton.onClick.AddListener(new UnityAction(this.CloseBT));
                }
            }
            
            Transform backBT = transform.Find("BackBT");
            if (backBT != null)
            {
                Button backBTButton = backBT.GetComponent<Button>();
                if (backBTButton != null)
                {
                    backBTButton.onClick.AddListener(delegate()
                    {
                        this.ClickBT();
                    });
                }
            }
            
            Transform dataShow = transform.Find("DataShow");
            if (dataShow != null)
            {
                Transform removeBT = dataShow.Find("RemoveBT");
                if (removeBT != null)
                {
                    Button removeButton = removeBT.GetComponent<Button>();
                    if (removeButton != null)
                    {
                        removeButton.onClick.AddListener(delegate()
                        {
                            this.RemoveBT();
                        });
                    }
                }
            }
		}

        private void OnEnable()
	    {
		    this.InitShow();
		    this.InitSize();
		    this.OnEnableData();
		    this.OnEnableShow();
		}

        private void InitShow()
	    {
            // 添加空引用检查以防止NullReferenceException
            Transform noDataTip = transform.Find("NoDataTip");
            if (noDataTip != null)
            {
                Transform tip = noDataTip.Find("Tip");
                if (tip != null)
                {
                    Text tipText = tip.GetComponent<Text>();
                    if (tipText != null)
                    {
                        tipText.text = AllText.Text_UIA[31][Mainload.SetData[4]];
                    }
                }
            }
		}

        private void InitSize()
	    {
            // 添加空引用检查
            Transform dataShow = transform.Find("DataShow");
            if (dataShow != null)
            {
                if (Mainload.SetData[4] == 0)
                {
                    SetTextFontSize(dataShow, "Name", 22);
                    SetTextFontSize(dataShow, "Lv", 22);
                    SetTextFontSize(dataShow, "Time", 22);
                    return;
                }
                SetTextFontSize(dataShow, "Name", 20);
                SetTextFontSize(dataShow, "Lv", 20);
                SetTextFontSize(dataShow, "Time", 20);
            }
		}

        // 辅助方法，用于设置文本字体大小并添加空引用检查
        private void SetTextFontSize(Transform parent, string childName, int fontSize)
        {
            Transform child = parent.Find(childName);
            if (child != null)
            {
                Text text = child.GetComponent<Text>();
                if (text != null)
                {
                    text.fontSize = fontSize;
                }
            }
        }

        private void OnEnableData()
	    {
		    bool flag = true;
		    try
		    {
			    if (ES3.FileExists("FW/" + base.name + "/GameData.es3"))
			    {
				    ES3.Load<System.Collections.Generic.List<string>>("FamilyData", "FW/" + base.name + "/GameData.es3");
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

        private void OnEnableShow()
	    {
            // 添加空引用检查
            Transform title = transform.Find("Title");
            if (title != null)
            {
                Text titleText = title.GetComponent<Text>();
                if (titleText != null)
                {
                    titleText.text = AllText.Text_UIA[27][Mainload.SetData[4]];
                }
            }
            
            Transform delCunDPanel = transform.Find("DelCunDPanel");
            if (delCunDPanel != null)
            {
                delCunDPanel.gameObject.SetActive(false);
            }
            
            transform.localPosition = new Vector3(0f, 500f, 0f);
            transform.DOLocalMoveY(0f, 0.3f, false).SetEase(Ease.OutBack, 1f);
	    if (this.isHaveData)
	    {
            try
            {
                System.Collections.Generic.List<string> list = ES3.Load<System.Collections.Generic.List<string>>("FamilyData", "FW/" + base.name + "/GameData.es3");
                System.Collections.Generic.List<int> list2 = ES3.Load<System.Collections.Generic.List<int>>("Time_now", "FW/" + base.name + "/GameData.es3");
                this.MemberShow();
                
                // 添加空引用检查
                Transform dataShow = transform.Find("DataShow");
                if (dataShow != null)
                {
                    // 设置Name文本
                    Transform nameText = dataShow.Find("Name");
                    if (nameText != null)
                    {
                        Text textName = nameText.GetComponent<Text>();
                        if (textName != null)
                        {
                            textName.text = AllText.Text_UIA[29][Mainload.SetData[4]].Replace("@", AllText.Text_City[int.Parse(list[0].Split(new char[]
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
                        }
                    }
                    
                    // 设置Lv文本
                    Transform lvText = dataShow.Find("Lv");
                    if (lvText != null)
                    {
                        Text textLv = lvText.GetComponent<Text>();
                        if (textLv != null)
                        {
                            textLv.text = AllText.Text_UIA[28][Mainload.SetData[4]].Replace("@", list[2]);
                        }
                    }
                    
                    // 设置Time文本
                    Transform timeText = dataShow.Find("Time");
                    if (timeText != null)
                    {
                        Text textTime = timeText.GetComponent<Text>();
                        if (textTime != null)
                        {
                            textTime.text = AllText.Text_UIA[975][Mainload.SetData[4]].Replace("@", list2[0].ToString()).Replace("$", AllText.Text_Months[list2[1]][Mainload.SetData[4]]).Replace("~", list2[2].ToString());
                        }
                    }
                    
                    dataShow.gameObject.SetActive(true);
                }
                
                // 隐藏NoDataTip
                Transform noDataTip = transform.Find("NoDataTip");
                if (noDataTip != null)
                {
                    noDataTip.gameObject.SetActive(false);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error in OnEnableShow(): " + e.Message);
            }
            return;
	    }
	    
            // 当没有数据时
            Transform dataShow2 = transform.Find("DataShow");
            if (dataShow2 != null)
            {
                dataShow2.gameObject.SetActive(false);
            }
            
            Transform noDataTip2 = transform.Find("NoDataTip");
            if (noDataTip2 != null)
            {
                noDataTip2.gameObject.SetActive(true);
            }
	    }

        private void Update()
        {
            if (Input.GetKeyDown(Mainload.FastKey[0]) && !transform.parent.Find("LoadPanel").gameObject.activeSelf)
            {
                this.CloseBT();
            }
        }

        public void UpdateShow()
	    {
		    this.OnEnableData();
		    this.OnEnableShow();
	    }

        private void RemoveBT()
	    {
		    Mainload.CunDangIndex_now = "FW/" + base.name;
		    transform.parent.Find("DelCunDPanel").gameObject.SetActive(true);
	    }

        private void ClickBT()
        {
            transform.parent.Find("BakUI").gameObject.SetActive(true);
            gameObject.SetActive(false);
            currentCunDangIndex_A = base.name + "_";
        }

        private void CloseBT()
        {
            transform.parent.Find("StartGameUI").gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        private void MemberShow()
	    {
		    System.Collections.Generic.List<string> list = ES3.Load<System.Collections.Generic.List<string>>("Member_First", "FW/" + base.name + "/GameData.es3");
		    for (int i = 0; i < base.transform.Find("DataShow").Find("IconShow").childCount; i++)
		    {
			     UnityEngine.Object.Destroy(base.transform.Find("DataShow").Find("IconShow").GetChild(i).gameObject);
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
		    gameObject.transform.SetParent(base.transform.Find("DataShow").Find("IconShow"));
		    gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		    gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		    GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load(path2));
		    gameObject2.transform.SetParent(base.transform.Find("DataShow").Find("IconShow"));
		    gameObject2.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		    gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
		    GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load(path3));
		    gameObject3.transform.SetParent(base.transform.Find("DataShow").Find("IconShow"));
		    gameObject3.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		    gameObject3.transform.localPosition = new Vector3(0f, 0f, 0f);
		    GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load(path4));
		    gameObject4.transform.SetParent(base.transform.Find("DataShow").Find("IconShow"));
		    gameObject4.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		    gameObject4.transform.localPosition = new Vector3(0f, 0f, 0f);
		    GameObject gameObject5 = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load(path5));
		    gameObject5.transform.SetParent(base.transform.Find("DataShow").Find("IconShow"));
		    gameObject5.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		    gameObject5.transform.localPosition = new Vector3(0f, 0f, 0f);
	    }
	}
}