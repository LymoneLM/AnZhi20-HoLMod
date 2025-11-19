using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MemberCheatPanelA : MonoBehaviour
{
    public bool IsMemberCheatPanelOpen;

    public int MemberClass;

    public int MemberIndex;

    private bool IsMenkeClass;

    private bool IsShiJiaClass;

    private bool IsHuangShiClass;

    private bool IsNongZhuangClass;

    private int MemberClass_Shai;

    private int State_Shai;

    private int Sex_Shai;

    private int OldMin_Shai;

    private int OldMax_Shai;

    private int ShuXing_Shai;

    private int ShenFen_Shai;

    private int HunYin_Shai;

    private int ZhiZe_Shai;

    private int ZhiShi_Menke_Shai;

    private int Sex_Menke_Shai;

    private int OldMin_Menke_Shai;

    private int OldMax_Menke_Shai;

    private int ShuXing_Menke_Shai;

    private int WeiZhi_Shai;

    private int SuoShuJun_Shai;

    private int NongZhuangMingZi_Shai;

    private string MemberName_SouSuo;

    private int ActID_now;

    private string MemberID_ShowNow;

    private int ShowID_Shai_Sou;

    private Vector3 MemberKtipPosi_last;

    private GameObject PerMemberA;

    private GameObject MSelectTipA;

    public List<List<string>>  Mod_Text_AllShaiSelect = new List<List<string>>
    {
        new List<string>
        {
            "全部|族中子弟|族人配偶",
            "All|Descendants|Consorts"
        },
        new List<string>
        {
            "全部|赋闲在家|外出为官|健康过低|出征|游学|游历|流放|徒刑|其他",
            "All|Idle|Official|Low Health|Military Mission|Study Tour|Travelling|Exile|Imprisonment|Others"
        },
        new List<string>
        {
            "全部|男|女",
            "All|Male|Female"
        },
        new List<string>
        {
            "文(优先)|武(优先)|商(优先)|艺(优先)|计谋(优先)|魅力(优先)|巫(优先)|医(优先)|相(优先)|卜(优先)|魅(优先)|工(优先)",
            "Writing|Might|Business|Arts|Cunning|Charisma|Sorcery Skill|Medicine Skill|Daoism Skill|Augur Skill|Charisma Skill|Crafting Skill"
        },
        new List<string>
        {
            "全部|文官|武官|无官职",
            "All|Civil Officials|Military Officials|Unranked"
        },
        new List<string>
        {
            "全部|未婚|已婚|其他",
            "All|Single|Married|Others"
        },
        new List<string>
        {
            "全部",
            "All"
        },
        new List<string>
        {
            "全部|无职事|写信做工|街头交易|打探消息|散播流言|私人教习|戏台演出|私塾教学|武场教学",
            "All|No Duty|Doing Gigs|Street Trading|Collecting Intel|Spreading Rumor|Private Teacher|Stage Actor|Clan Teacher|Training Coach"
        },
        new List<string>
        {
            "全部|大地图|封地",
            "All|Map|FengDi"
        },
        new List<string>
        {
            "全部|南郡|三川郡|蜀郡|丹阳郡|陈留郡|长沙郡|会稽郡|广陵郡|太原郡|益州郡|南海郡|云南郡",
            "All|Nan Province|Sanchuan Province|Shu Province|Danyang Province|Chenliu Province|Changsha Province|Kuaiji Province|Guangling Province|Taiyuan Province|Yizhou Province|Nanhai Province|Yunnan Province"
        },
        new List<string> { }
    };

    public List<List<string>> Mod_Text_UIA = new List<List<string>>
    {
        AllText.Text_UIA[129],      //0
        AllText.Text_UIA[267],      //1
        new List<string>            //2
        {
            "世家",
            "Clan"
        },
        new List<string>            //3
        {
            "皇室",
            "Royal"
        },
        new List<string>            //4
        {
            "农庄",
            "Farm"
        },
        new List<string>            //5
        {
            "位置",
            "Position"
        },
        new List<string>            //6
        {
            "所属郡",
            "Province"
        },
        new List<string>            //7
        {
            "农庄名字",
            "Farm Name"
        },
        new List<string>            //8
        {
            "成员编辑",
            "Member Cheat"
        },
        new List<string>            //9
        {
            "By AnZhi20 Version " + cs.HoLMod.MemberCheat.PluginInfo.PLUGIN_VERSION
        }
    };

    private void Awake()
    {
        AwakeData();
        PerMemberA = (GameObject)Resources.Load("PerMemberBT");
        MSelectTipA = (GameObject)Resources.Load("MSelectTip");
        for (int num1 = 0; num1 < Mod_Text_AllShaiSelect[0][Mainload.SetData[4]].Split('|').Length; num1++)
        {
            Dropdown.OptionData optionData = new Dropdown.OptionData();
            optionData.text = Mod_Text_AllShaiSelect[0][Mainload.SetData[4]].Split('|')[num1];
            base.transform.Find("AllShaiA").Find("STipA").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData);
        }
        for (int num2 = 0; num2 < Mod_Text_AllShaiSelect[1][Mainload.SetData[4]].Split('|').Length; num2++)
        {
            Dropdown.OptionData optionData2 = new Dropdown.OptionData();
            optionData2.text = Mod_Text_AllShaiSelect[1][Mainload.SetData[4]].Split('|')[num2];
            base.transform.Find("AllShaiA").Find("STipB").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData2);
        }
        for (int num3 = 0; num3 < Mod_Text_AllShaiSelect[2][Mainload.SetData[4]].Split('|').Length; num3++)
        {
            Dropdown.OptionData optionData3 = new Dropdown.OptionData();
            optionData3.text = Mod_Text_AllShaiSelect[2][Mainload.SetData[4]].Split('|')[num3];
            base.transform.Find("AllShaiA").Find("STipC").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData3);
        }
        for (int num4 = 0; num4 < Mod_Text_AllShaiSelect[3][Mainload.SetData[4]].Split('|').Length; num4++)
        {
            Dropdown.OptionData optionData4 = new Dropdown.OptionData();
            optionData4.text = Mod_Text_AllShaiSelect[3][Mainload.SetData[4]].Split('|')[num4];
            base.transform.Find("AllShaiA").Find("STipE").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData4);
        }
        for (int num5 = 0; num5 < Mod_Text_AllShaiSelect[4][Mainload.SetData[4]].Split('|').Length; num5++)
        {
            Dropdown.OptionData optionData5 = new Dropdown.OptionData();
            optionData5.text = Mod_Text_AllShaiSelect[4][Mainload.SetData[4]].Split('|')[num5];
            base.transform.Find("AllShaiA").Find("STipF").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData5);
        }
        for (int num6 = 0; num6 < Mod_Text_AllShaiSelect[5][Mainload.SetData[4]].Split('|').Length; num6++)
        {
            Dropdown.OptionData optionData6 = new Dropdown.OptionData();
            optionData6.text = Mod_Text_AllShaiSelect[5][Mainload.SetData[4]].Split('|')[num6];
            base.transform.Find("AllShaiA").Find("STipG").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData6);
        }
        for (int num7 = 0; num7 < Mod_Text_AllShaiSelect[6][Mainload.SetData[4]].Split('|').Length; num7++)
        {
            Dropdown.OptionData optionData7 = new Dropdown.OptionData();
            optionData7.text = Mod_Text_AllShaiSelect[6][Mainload.SetData[4]].Split('|')[num7];
            base.transform.Find("AllShaiA").Find("STipH").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData7);
        }
        for (int num8 = 0; num8 < AllText.AllMemberZhize.Count; num8++)
        {
            Dropdown.OptionData optionData8 = new Dropdown.OptionData();
            optionData8.text = AllText.AllMemberZhize[num8][Mainload.SetData[4]].Split('|')[1].Replace("<color=#387E3Aff>", "").Replace("</color>", "");
            base.transform.Find("AllShaiA").Find("STipH").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData8);
        }
        for (int num9 = 0; num9 < Mod_Text_AllShaiSelect[7][Mainload.SetData[4]].Split('|').Length; num9++)
        {
            Dropdown.OptionData optionData9 = new Dropdown.OptionData();
            optionData9.text = Mod_Text_AllShaiSelect[7][Mainload.SetData[4]].Split('|')[num9];
            base.transform.Find("AllShaiB").Find("STipA").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData9);
        }
        for (int num10 = 0; num10 < Mod_Text_AllShaiSelect[2][Mainload.SetData[4]].Split('|').Length; num10++)
        {
            Dropdown.OptionData optionData10 = new Dropdown.OptionData();
            optionData10.text = Mod_Text_AllShaiSelect[2][Mainload.SetData[4]].Split('|')[num10];
            base.transform.Find("AllShaiB").Find("STipB").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData10);
        }
        for (int num11 = 0; num11 < Mod_Text_AllShaiSelect[3][Mainload.SetData[4]].Split('|').Length; num11++)
        {
            Dropdown.OptionData optionData11 = new Dropdown.OptionData();
            optionData11.text = Mod_Text_AllShaiSelect[3][Mainload.SetData[4]].Split('|')[num11];
            base.transform.Find("AllShaiB").Find("STipD").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData11);
        }
        for (int num12 = 0; num12 < Mod_Text_AllShaiSelect[0][Mainload.SetData[4]].Split('|').Length; num12++)
        {
            Dropdown.OptionData optionData12 = new Dropdown.OptionData();
            optionData12.text = Mod_Text_AllShaiSelect[0][Mainload.SetData[4]].Split('|')[num12];
            base.transform.Find("AllShaiC").Find("STipA").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData12);
        }
        for (int num13 = 0; num13 < Mod_Text_AllShaiSelect[1][Mainload.SetData[4]].Split('|').Length; num13++)
        {
            Dropdown.OptionData optionData13 = new Dropdown.OptionData();
            optionData13.text = Mod_Text_AllShaiSelect[1][Mainload.SetData[4]].Split('|')[num13];
            base.transform.Find("AllShaiC").Find("STipB").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData13);
        }
        for (int num14 = 0; num14 < Mod_Text_AllShaiSelect[2][Mainload.SetData[4]].Split('|').Length; num14++)
        {
            Dropdown.OptionData optionData14 = new Dropdown.OptionData();
            optionData14.text = Mod_Text_AllShaiSelect[2][Mainload.SetData[4]].Split('|')[num14];
            base.transform.Find("AllShaiC").Find("STipC").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData14);
        }
        for (int num15 = 0; num15 < Mod_Text_AllShaiSelect[3][Mainload.SetData[4]].Split('|').Length; num15++)
        {
            Dropdown.OptionData optionData15 = new Dropdown.OptionData();
            optionData15.text = Mod_Text_AllShaiSelect[3][Mainload.SetData[4]].Split('|')[num15];
            base.transform.Find("AllShaiC").Find("STipE").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData15);
        }
        for (int num16 = 0; num16 < Mod_Text_AllShaiSelect[4][Mainload.SetData[4]].Split('|').Length; num16++)
        {
            Dropdown.OptionData optionData16 = new Dropdown.OptionData();
            optionData16.text = Mod_Text_AllShaiSelect[4][Mainload.SetData[4]].Split('|')[num16];
            base.transform.Find("AllShaiC").Find("STipF").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData16);
        }
        for (int num17 = 0; num17 < Mod_Text_AllShaiSelect[5][Mainload.SetData[4]].Split('|').Length; num17++)
        {
            Dropdown.OptionData optionData17 = new Dropdown.OptionData();
            optionData17.text = Mod_Text_AllShaiSelect[5][Mainload.SetData[4]].Split('|')[num17];
            base.transform.Find("AllShaiC").Find("STipG").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData17);
        }
        for (int num18 = 0; num18 < Mod_Text_AllShaiSelect[6][Mainload.SetData[4]].Split('|').Length; num18++)
        {
            Dropdown.OptionData optionData18 = new Dropdown.OptionData();
            optionData18.text = Mod_Text_AllShaiSelect[6][Mainload.SetData[4]].Split('|')[num18];
            base.transform.Find("AllShaiC").Find("STipH").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData18);
        }
        for (int num19 = 0; num19 < AllText.AllMemberZhize.Count; num19++)
        {
            Dropdown.OptionData optionData19 = new Dropdown.OptionData();
            optionData19.text = AllText.AllMemberZhize[num19][Mainload.SetData[4]].Split('|')[1].Replace("<color=#387E3Aff>", "").Replace("</color>", "");
            base.transform.Find("AllShaiC").Find("STipH").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData19);
        }
        for (int num20 = 0; num20 < Mod_Text_AllShaiSelect[0][Mainload.SetData[4]].Split('|').Length; num20++)
        {
            Dropdown.OptionData optionData20 = new Dropdown.OptionData();
            optionData20.text = Mod_Text_AllShaiSelect[0][Mainload.SetData[4]].Split('|')[num20];
            base.transform.Find("AllShaiD").Find("STipA").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData20);
        }
        for (int num21 = 0; num21 < Mod_Text_AllShaiSelect[1][Mainload.SetData[4]].Split('|').Length; num21++)
        {
            Dropdown.OptionData optionData21 = new Dropdown.OptionData();
            optionData21.text = Mod_Text_AllShaiSelect[1][Mainload.SetData[4]].Split('|')[num21];
            base.transform.Find("AllShaiD").Find("STipB").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData21);
        }
        for (int num22 = 0; num22 < Mod_Text_AllShaiSelect[2][Mainload.SetData[4]].Split('|').Length; num22++)
        {
            Dropdown.OptionData optionData22 = new Dropdown.OptionData();
            optionData22.text = Mod_Text_AllShaiSelect[2][Mainload.SetData[4]].Split('|')[num22];
            base.transform.Find("AllShaiD").Find("STipC").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData22);
        }
        for (int num23 = 0; num23 < Mod_Text_AllShaiSelect[3][Mainload.SetData[4]].Split('|').Length; num23++)
        {
            Dropdown.OptionData optionData23 = new Dropdown.OptionData();
            optionData23.text = Mod_Text_AllShaiSelect[3][Mainload.SetData[4]].Split('|')[num23];
            base.transform.Find("AllShaiD").Find("STipE").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData23);
        }
        for (int num24 = 0; num24 < Mod_Text_AllShaiSelect[4][Mainload.SetData[4]].Split('|').Length; num24++)
        {
            Dropdown.OptionData optionData24 = new Dropdown.OptionData();
            optionData24.text = Mod_Text_AllShaiSelect[4][Mainload.SetData[4]].Split('|')[num24];
            base.transform.Find("AllShaiD").Find("STipF").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData24);
        }
        for (int num25 = 0; num25 < Mod_Text_AllShaiSelect[5][Mainload.SetData[4]].Split('|').Length; num25++)
        {
            Dropdown.OptionData optionData25 = new Dropdown.OptionData();
            optionData25.text = Mod_Text_AllShaiSelect[5][Mainload.SetData[4]].Split('|')[num25];
            base.transform.Find("AllShaiD").Find("STipG").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData25);
        }
        for (int num26 = 0; num26 < Mod_Text_AllShaiSelect[6][Mainload.SetData[4]].Split('|').Length; num26++)
        {
            Dropdown.OptionData optionData26 = new Dropdown.OptionData();
            optionData26.text = Mod_Text_AllShaiSelect[6][Mainload.SetData[4]].Split('|')[num26];
            base.transform.Find("AllShaiD").Find("STipH").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData26);
        }
        for (int num27 = 0; num27 < AllText.AllMemberZhize.Count; num27++)
        {
            Dropdown.OptionData optionData27 = new Dropdown.OptionData();
            optionData27.text = AllText.AllMemberZhize[num27][Mainload.SetData[4]].Split('|')[1].Replace("<color=#387E3Aff>", "").Replace("</color>", "");
            base.transform.Find("AllShaiD").Find("STipH").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData27);
        }
        for (int num28 = 0; num28 < Mod_Text_AllShaiSelect[8][Mainload.SetData[4]].Split('|').Length; num28++)
        {
            Dropdown.OptionData optionData28 = new Dropdown.OptionData();
            optionData28.text = Mod_Text_AllShaiSelect[8][Mainload.SetData[4]].Split('|')[num28];
            base.transform.Find("AllShaiE").Find("STipA").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData28);
        }
        for (int num29 = 0; num29 < Mod_Text_AllShaiSelect[9][Mainload.SetData[4]].Split('|').Length; num29++)
        {
            Dropdown.OptionData optionData29 = new Dropdown.OptionData();
            optionData29.text = Mod_Text_AllShaiSelect[9][Mainload.SetData[4]].Split('|')[num29];
            base.transform.Find("AllShaiE").Find("STipB").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData29);
        }
        for (int num30 = 0; num30 < Mod_Text_AllShaiSelect[10][Mainload.SetData[4]].Split('|').Length; num30++)
        {
            Dropdown.OptionData optionData30 = new Dropdown.OptionData();
            optionData30.text = Mod_Text_AllShaiSelect[10][Mainload.SetData[4]].Split('|')[num30];
            base.transform.Find("AllShaiE").Find("STipC").Find("AllClass")
                .GetComponent<Dropdown>()
                .options.Add(optionData30);
        }
    }

    private void Start()
    {
        base.transform.Find("CloseBT").GetComponent<Button>().onClick.AddListener(CloseBT);
        base.transform.Find("SelectA").GetComponent<Button>().onClick.AddListener(SelectABT);
        base.transform.Find("SelectB").GetComponent<Button>().onClick.AddListener(SelectBBT);
        base.transform.Find("SelectC").GetComponent<Button>().onClick.AddListener(SelectCBT);
        base.transform.Find("SelectD").GetComponent<Button>().onClick.AddListener(SelectDBT);
        base.transform.Find("SelectE").GetComponent<Button>().onClick.AddListener(SelectEBT);
        initShow();
        initSize();
    }

    private void OnEnable()
    {
        base.transform.localPosition = new Vector3(0f, 500f, 0f);
        OnEnableData();
        OnEnableShow();
        base.transform.DOLocalMoveY(0f, 0.3f).SetEase(Ease.OutBack, 1f);
    }

    private void AwakeData()
    {
        State_Shai = 0;
        Sex_Shai = 0;
        OldMin_Shai = 1;
        OldMax_Shai = 150;
        ShuXing_Shai = 0;
        ShenFen_Shai = 0;
        HunYin_Shai = 0;
        ZhiZe_Shai = 0;
        ZhiShi_Menke_Shai = 0;
        Sex_Menke_Shai = 0;
        OldMin_Menke_Shai = 1;
        OldMax_Menke_Shai = 150;
        ShuXing_Menke_Shai = 0;
        WeiZhi_Shai = 0;
        SuoShuJun_Shai = 0;
        NongZhuangMingZi_Shai = 0;
        IsMenkeClass = false;
        IsShiJiaClass = false;
        IsHuangShiClass = false;
        IsNongZhuangClass = false;
    }

    private void initShow()
    {
        base.transform.Find("panel").Find("Title").GetComponent<Text>()
            .text = Mod_Text_UIA[8][Mainload.SetData[4]];
        base.transform.Find("panel").Find("VersionAndAuthor").GetComponent<Text>()
            .text = Mod_Text_UIA[9][0];
        base.transform.Find("SelectA").Find("Text").GetComponent<Text>()
            .text = Mod_Text_UIA[0][Mainload.SetData[4]];
        base.transform.Find("SelectB").Find("Text").GetComponent<Text>()
            .text = Mod_Text_UIA[1][Mainload.SetData[4]];
        base.transform.Find("SelectC").Find("Text").GetComponent<Text>()
            .text = Mod_Text_UIA[2][Mainload.SetData[4]];
        base.transform.Find("SelectD").Find("Text").GetComponent<Text>()
            .text = Mod_Text_UIA[3][Mainload.SetData[4]];
        base.transform.Find("SelectE").Find("Text").GetComponent<Text>()
            .text = Mod_Text_UIA[4][Mainload.SetData[4]];
        base.transform.Find("SouSuoInput").Find("Placeholder").GetComponent<Text>()
            .text = AllText.Text_UIA[1578][Mainload.SetData[4]];
        base.transform.Find("AllShaiA").Find("STipA").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1861][Mainload.SetData[4]];
        base.transform.Find("AllShaiA").Find("STipB").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1862][Mainload.SetData[4]];
        base.transform.Find("AllShaiA").Find("STipC").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1863][Mainload.SetData[4]];
        base.transform.Find("AllShaiA").Find("STipD").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1864][Mainload.SetData[4]];
        base.transform.Find("AllShaiA").Find("STipE").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1865][Mainload.SetData[4]];
        base.transform.Find("AllShaiA").Find("STipF").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1866][Mainload.SetData[4]];
        base.transform.Find("AllShaiA").Find("STipG").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1867][Mainload.SetData[4]];
        base.transform.Find("AllShaiA").Find("STipH").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1868][Mainload.SetData[4]];
        base.transform.Find("AllShaiA").Find("STipD").Find("MinOld")
            .Find("Placeholder")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1869][Mainload.SetData[4]];
        base.transform.Find("AllShaiA").Find("STipD").Find("MaxOld")
            .Find("Placeholder")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1870][Mainload.SetData[4]];
        base.transform.Find("AllShaiB").Find("STipA").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1871][Mainload.SetData[4]];
        base.transform.Find("AllShaiB").Find("STipB").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1863][Mainload.SetData[4]];
        base.transform.Find("AllShaiB").Find("STipC").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1864][Mainload.SetData[4]];
        base.transform.Find("AllShaiB").Find("STipD").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1865][Mainload.SetData[4]];
        base.transform.Find("AllShaiB").Find("STipC").Find("MinOld")
            .Find("Placeholder")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1869][Mainload.SetData[4]];
        base.transform.Find("AllShaiB").Find("STipC").Find("MaxOld")
            .Find("Placeholder")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1870][Mainload.SetData[4]];
        base.transform.Find("AllShaiC").Find("STipA").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1861][Mainload.SetData[4]];
        base.transform.Find("AllShaiC").Find("STipB").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1862][Mainload.SetData[4]];
        base.transform.Find("AllShaiC").Find("STipC").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1863][Mainload.SetData[4]];
        base.transform.Find("AllShaiC").Find("STipD").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1864][Mainload.SetData[4]];
        base.transform.Find("AllShaiC").Find("STipE").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1865][Mainload.SetData[4]];
        base.transform.Find("AllShaiC").Find("STipF").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1866][Mainload.SetData[4]];
        base.transform.Find("AllShaiC").Find("STipG").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1867][Mainload.SetData[4]];
        base.transform.Find("AllShaiC").Find("STipH").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1868][Mainload.SetData[4]];
        base.transform.Find("AllShaiC").Find("STipD").Find("MinOld")
            .Find("Placeholder")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1869][Mainload.SetData[4]];
        base.transform.Find("AllShaiC").Find("STipD").Find("MaxOld")
            .Find("Placeholder")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1870][Mainload.SetData[4]];
        base.transform.Find("AllShaiD").Find("STipA").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1861][Mainload.SetData[4]];
        base.transform.Find("AllShaiD").Find("STipB").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1862][Mainload.SetData[4]];
        base.transform.Find("AllShaiD").Find("STipC").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1863][Mainload.SetData[4]];
        base.transform.Find("AllShaiD").Find("STipD").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1864][Mainload.SetData[4]];
        base.transform.Find("AllShaiD").Find("STipE").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1865][Mainload.SetData[4]];
        base.transform.Find("AllShaiD").Find("STipF").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1866][Mainload.SetData[4]];
        base.transform.Find("AllShaiD").Find("STipG").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1867][Mainload.SetData[4]];
        base.transform.Find("AllShaiD").Find("STipH").Find("Tip")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1868][Mainload.SetData[4]];
        base.transform.Find("AllShaiD").Find("STipD").Find("MinOld")
            .Find("Placeholder")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1869][Mainload.SetData[4]];
        base.transform.Find("AllShaiD").Find("STipD").Find("MaxOld")
            .Find("Placeholder")
            .GetComponent<Text>()
            .text = AllText.Text_UIA[1870][Mainload.SetData[4]];
        base.transform.Find("AllShaiE").Find("STipF").Find("Tip")
            .GetComponent<Text>()
            .text = Mod_Text_UIA[5][Mainload.SetData[4]];
        base.transform.Find("AllShaiE").Find("STipG").Find("Tip")
            .GetComponent<Text>()
            .text = Mod_Text_UIA[6][Mainload.SetData[4]];
        base.transform.Find("AllShaiE").Find("STipH").Find("Tip")
            .GetComponent<Text>()
            .text = Mod_Text_UIA[7][Mainload.SetData[4]];
    }

    private void initSize()
    {
        if (Mainload.SetData[4] == 0)
        {
            base.transform.Find("AllShaiA").Find("STipA").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiA").Find("STipB").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiA").Find("STipC").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiA").Find("STipD").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiA").Find("STipE").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiA").Find("STipF").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiA").Find("STipG").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiA").Find("STipH").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiA").Find("STipD").Find("MinOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiA").Find("STipD").Find("MaxOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiB").Find("STipA").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiB").Find("STipB").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiB").Find("STipC").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiB").Find("STipD").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiB").Find("STipC").Find("MinOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiB").Find("STipC").Find("MaxOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiC").Find("STipA").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiC").Find("STipB").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiC").Find("STipC").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiC").Find("STipD").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiC").Find("STipE").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiC").Find("STipF").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiC").Find("STipG").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiC").Find("STipH").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiC").Find("STipD").Find("MinOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiC").Find("STipD").Find("MaxOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiD").Find("STipA").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiD").Find("STipB").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiD").Find("STipC").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiD").Find("STipD").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiD").Find("STipE").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiD").Find("STipF").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiD").Find("STipG").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiD").Find("STipH").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiD").Find("STipD").Find("MinOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiD").Find("STipD").Find("MaxOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiE").Find("STipA").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiE").Find("STipB").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
            base.transform.Find("AllShaiE").Find("STipC").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 18;
        }
        else
        {
            base.transform.Find("AllShaiA").Find("STipA").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiA").Find("STipB").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiA").Find("STipC").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiA").Find("STipD").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiA").Find("STipE").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiA").Find("STipF").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiA").Find("STipG").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiA").Find("STipH").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiA").Find("STipD").Find("MinOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiA").Find("STipD").Find("MaxOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiB").Find("STipA").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiB").Find("STipB").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiB").Find("STipC").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiB").Find("STipD").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiB").Find("STipC").Find("MinOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiB").Find("STipC").Find("MaxOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiC").Find("STipA").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiC").Find("STipB").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiC").Find("STipC").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiC").Find("STipD").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiC").Find("STipE").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiC").Find("STipF").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiC").Find("STipG").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiC").Find("STipH").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiC").Find("STipD").Find("MinOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiC").Find("STipD").Find("MaxOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiD").Find("STipA").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiD").Find("STipB").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiD").Find("STipC").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiD").Find("STipD").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiD").Find("STipE").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiD").Find("STipF").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiD").Find("STipG").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiD").Find("STipH").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiD").Find("STipD").Find("MinOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiD").Find("STipD").Find("MaxOld")
                .Find("Placeholder")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiE").Find("STipA").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiE").Find("STipB").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
            base.transform.Find("AllShaiE").Find("STipC").Find("Tip")
                .GetComponent<Text>()
                .fontSize = 16;
        }
    }

    private void OnEnableData()
    {
        MemberName_SouSuo = null;
        base.transform.Find("SouSuoInput").GetComponent<InputField>().SetTextWithoutNotify(MemberName_SouSuo);
        ShowID_Shai_Sou = 0;
    }

    private void OnEnableShow()
    {
        base.transform.Find("AllShaiA").Find("STipA").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(MemberClass_Shai);
        base.transform.Find("AllShaiA").Find("STipB").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(State_Shai);
        base.transform.Find("AllShaiA").Find("STipC").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(Sex_Shai);
        base.transform.Find("AllShaiA").Find("STipD").Find("MinOld")
            .GetComponent<InputField>()
            .SetTextWithoutNotify(OldMin_Shai.ToString());
        base.transform.Find("AllShaiA").Find("STipD").Find("MaxOld")
            .GetComponent<InputField>()
            .SetTextWithoutNotify(OldMax_Shai.ToString());
        base.transform.Find("AllShaiA").Find("STipE").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(ShuXing_Shai);
        base.transform.Find("AllShaiA").Find("STipF").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(ShenFen_Shai);
        base.transform.Find("AllShaiA").Find("STipG").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(HunYin_Shai);
        base.transform.Find("AllShaiA").Find("STipH").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(ZhiZe_Shai);
        base.transform.Find("AllShaiB").Find("STipA").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(ZhiShi_Menke_Shai);
        base.transform.Find("AllShaiB").Find("STipB").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(Sex_Menke_Shai);
        base.transform.Find("AllShaiB").Find("STipC").Find("MinOld")
            .GetComponent<InputField>()
            .SetTextWithoutNotify(OldMin_Menke_Shai.ToString());
        base.transform.Find("AllShaiB").Find("STipC").Find("MaxOld")
            .GetComponent<InputField>()
            .SetTextWithoutNotify(OldMax_Menke_Shai.ToString());
        base.transform.Find("AllShaiB").Find("STipD").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(ShuXing_Menke_Shai);
        base.transform.Find("AllShaiC").Find("STipA").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(MemberClass_Shai);
        base.transform.Find("AllShaiC").Find("STipB").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(State_Shai);
        base.transform.Find("AllShaiC").Find("STipC").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(Sex_Shai);
        base.transform.Find("AllShaiC").Find("STipD").Find("MinOld")
            .GetComponent<InputField>()
            .SetTextWithoutNotify(OldMin_Shai.ToString());
        base.transform.Find("AllShaiC").Find("STipD").Find("MaxOld")
            .GetComponent<InputField>()
            .SetTextWithoutNotify(OldMax_Shai.ToString());
        base.transform.Find("AllShaiC").Find("STipE").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(ShuXing_Shai);
        base.transform.Find("AllShaiC").Find("STipF").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(ShenFen_Shai);
        base.transform.Find("AllShaiC").Find("STipG").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(HunYin_Shai);
        base.transform.Find("AllShaiC").Find("STipH").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(ZhiZe_Shai);
        base.transform.Find("AllShaiD").Find("STipA").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(MemberClass_Shai);
        base.transform.Find("AllShaiD").Find("STipB").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(State_Shai);
        base.transform.Find("AllShaiD").Find("STipC").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(Sex_Shai);
        base.transform.Find("AllShaiD").Find("STipD").Find("MinOld")
            .GetComponent<InputField>()
            .SetTextWithoutNotify(OldMin_Shai.ToString());
        base.transform.Find("AllShaiD").Find("STipD").Find("MaxOld")
            .GetComponent<InputField>()
            .SetTextWithoutNotify(OldMax_Shai.ToString());
        base.transform.Find("AllShaiD").Find("STipE").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(ShuXing_Shai);
        base.transform.Find("AllShaiD").Find("STipF").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(ShenFen_Shai);
        base.transform.Find("AllShaiD").Find("STipG").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(HunYin_Shai);
        base.transform.Find("AllShaiD").Find("STipH").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(ZhiZe_Shai);
        base.transform.Find("AllShaiE").Find("STipA").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(WeiZhi_Shai);
        base.transform.Find("AllShaiE").Find("STipB").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(SuoShuJun_Shai);
        base.transform.Find("AllShaiE").Find("STipC").Find("AllClass")
            .GetComponent<Dropdown>()
            .SetValueWithoutNotify(NongZhuangMingZi_Shai);
        if (!IsMenkeClass && !IsShiJiaClass && !IsHuangShiClass && !IsNongZhuangClass)
        {
            for (int i = 0; i < base.transform.Find("AllMember").Find("Viewport").Find("Content")
                .childCount; i++)
            {
                Object.Destroy(base.transform.Find("AllMember").Find("Viewport").Find("Content")
                    .GetChild(i)
                    .gameObject);
            }
            Invoke("SelectABT", 0.4f);
        }
        else if(IsMenkeClass)
        {
            for (int j = 0; j < base.transform.Find("AllMember").Find("Viewport").Find("Content")
                .childCount; j++)
            {
                Object.Destroy(base.transform.Find("AllMember").Find("Viewport").Find("Content")
                    .GetChild(j)
                    .gameObject);
            }
            Invoke("SelectBBT", 0.3f);
        }
        else if (IsShiJiaClass)
        {
            for (int j = 0; j < base.transform.Find("AllMember").Find("Viewport").Find("Content")
                     .childCount; j++)
            {
                Object.Destroy(base.transform.Find("AllMember").Find("Viewport").Find("Content")
                    .GetChild(j)
                    .gameObject);
            }
            Invoke("SelectCBT", 0.3f);
        }
        else if (IsHuangShiClass)
        {
            for (int j = 0; j < base.transform.Find("AllMember").Find("Viewport").Find("Content")
                     .childCount; j++)
            {
                Object.Destroy(base.transform.Find("AllMember").Find("Viewport").Find("Content")
                    .GetChild(j)
                    .gameObject);
            }
            Invoke("SelectDBT", 0.3f);
        }
        else if (IsNongZhuangClass)
        {
            for (int j = 0; j < base.transform.Find("AllMember").Find("Viewport").Find("Content")
                     .childCount; j++)
            {
                Object.Destroy(base.transform.Find("AllMember").Find("Viewport").Find("Content")
                    .GetChild(j)
                    .gameObject);
            }
            Invoke("SelectEBT", 0.3f);
        }
    }

    private void SelectABT()
    {
        base.transform.Find("SelectA").GetComponent<Button>().interactable = false;
        base.transform.Find("SelectB").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectC").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectD").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectE").GetComponent<Button>().interactable = true;
        base.transform.Find("AllShaiA").gameObject.SetActive(value: true);
        base.transform.Find("AllShaiB").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiC").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiD").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiE").gameObject.SetActive(value: false);
        MemberName_SouSuo = null;
        base.transform.Find("SouSuoInput").GetComponent<InputField>().SetTextWithoutNotify(MemberName_SouSuo);
        IsMenkeClass = false;
        IsShiJiaClass = false;
        IsHuangShiClass = false;
        IsNongZhuangClass = false;
        Shai_MemberShow();
    }

    private void SelectBBT()
    {
        base.transform.Find("SelectA").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectB").GetComponent<Button>().interactable = false;
        base.transform.Find("SelectC").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectD").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectE").GetComponent<Button>().interactable = true;
        base.transform.Find("AllShaiA").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiB").gameObject.SetActive(value: true);
        base.transform.Find("AllShaiC").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiD").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiE").gameObject.SetActive(value: false);
        MemberName_SouSuo = null;
        base.transform.Find("SouSuoInput").GetComponent<InputField>().SetTextWithoutNotify(MemberName_SouSuo);
        IsMenkeClass = true;
        IsShiJiaClass = false;
        IsHuangShiClass = false;
        IsNongZhuangClass = false;
        Shai_MemberShow();
    }

    private void SelectCBT()
    {
        base.transform.Find("SelectA").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectB").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectC").GetComponent<Button>().interactable = false;
        base.transform.Find("SelectD").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectE").GetComponent<Button>().interactable = true;
        base.transform.Find("AllShaiA").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiB").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiC").gameObject.SetActive(value: true);
        base.transform.Find("AllShaiD").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiE").gameObject.SetActive(value: false);
        MemberName_SouSuo = null;
        base.transform.Find("SouSuoInput").GetComponent<InputField>().SetTextWithoutNotify(MemberName_SouSuo);
        IsMenkeClass = false;
        IsShiJiaClass = true;
        IsHuangShiClass = false;
        IsNongZhuangClass = false;
        Shai_MemberShow();
    }

    private void SelectDBT()
    {
        base.transform.Find("SelectA").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectB").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectC").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectD").GetComponent<Button>().interactable = false;
        base.transform.Find("SelectE").GetComponent<Button>().interactable = true;
        base.transform.Find("AllShaiA").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiB").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiC").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiD").gameObject.SetActive(value: true);
        base.transform.Find("AllShaiE").gameObject.SetActive(value: false);
        MemberName_SouSuo = null;
        base.transform.Find("SouSuoInput").GetComponent<InputField>().SetTextWithoutNotify(MemberName_SouSuo);
        IsMenkeClass = false;
        IsShiJiaClass = false;
        IsHuangShiClass = true;
        IsNongZhuangClass = false;
        Shai_MemberShow();
    }

    private void SelectEBT()
    {
        base.transform.Find("SelectA").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectB").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectC").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectD").GetComponent<Button>().interactable = true;
        base.transform.Find("SelectE").GetComponent<Button>().interactable = false;
        base.transform.Find("AllShaiA").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiB").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiC").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiD").gameObject.SetActive(value: false);
        base.transform.Find("AllShaiE").gameObject.SetActive(value: true);
        MemberName_SouSuo = null;
        base.transform.Find("SouSuoInput").GetComponent<InputField>().SetTextWithoutNotify(MemberName_SouSuo);
        IsMenkeClass = false;
        IsShiJiaClass = false;
        IsHuangShiClass = false;
        IsNongZhuangClass = true;
        Shai_NongZhuangShow();
    }

    public void ShaiChangeA(string ID)
    {
        MemberName_SouSuo = null;
        base.transform.Find("SouSuoInput").GetComponent<InputField>().SetTextWithoutNotify(MemberName_SouSuo);
        switch (ID)
        {
            case "AA":
                MemberClass_Shai = base.transform.Find("AllShaiA").Find("STipA").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "AB":
                State_Shai = base.transform.Find("AllShaiA").Find("STipB").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "AC":
                Sex_Shai = base.transform.Find("AllShaiA").Find("STipC").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "AE":
                ShuXing_Shai = base.transform.Find("AllShaiA").Find("STipE").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "AF":
                ShenFen_Shai = base.transform.Find("AllShaiA").Find("STipF").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "AG":
                HunYin_Shai = base.transform.Find("AllShaiA").Find("STipG").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "AH":
                ZhiZe_Shai = base.transform.Find("AllShaiA").Find("STipH").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "BA":
                ZhiShi_Menke_Shai = base.transform.Find("AllShaiB").Find("STipA").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "BB":
                Sex_Menke_Shai = base.transform.Find("AllShaiB").Find("STipB").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "BD":
                ShuXing_Menke_Shai = base.transform.Find("AllShaiB").Find("STipD").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "CA":
                MemberClass_Shai = base.transform.Find("AllShaiC").Find("STipA").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "CB":
                State_Shai = base.transform.Find("AllShaiC").Find("STipB").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "CC":
                Sex_Shai = base.transform.Find("AllShaiC").Find("STipC").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "CE":
                ShuXing_Shai = base.transform.Find("AllShaiC").Find("STipE").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "CF":
                ShenFen_Shai = base.transform.Find("AllShaiC").Find("STipF").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "CG":
                HunYin_Shai = base.transform.Find("AllShaiC").Find("STipG").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "CH":
                ZhiZe_Shai = base.transform.Find("AllShaiC").Find("STipH").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "DA":
                MemberClass_Shai = base.transform.Find("AllShaiD").Find("STipA").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "DB":
                State_Shai = base.transform.Find("AllShaiD").Find("STipB").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "DC":
                Sex_Shai = base.transform.Find("AllShaiD").Find("STipC").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "DE":
                ShuXing_Shai = base.transform.Find("AllShaiD").Find("STipE").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "DF":
                ShenFen_Shai = base.transform.Find("AllShaiD").Find("STipF").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "DG":
                HunYin_Shai = base.transform.Find("AllShaiD").Find("STipG").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "DH":
                ZhiZe_Shai = base.transform.Find("AllShaiD").Find("STipH").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "EA":
                WeiZhi_Shai = base.transform.Find("AllShaiE").Find("STipA").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "EB":
                SuoShuJun_Shai = base.transform.Find("AllShaiE").Find("STipB").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
            case "EC":
                NongZhuangMingZi_Shai = base.transform.Find("AllShaiE").Find("STipC").Find("AllClass")
                    .GetComponent<Dropdown>()
                    .value;
                break;
        }
        Shai_MemberShow();
    }

    public void ShaiChangeB(string ID)
    {
        MemberName_SouSuo = null;
        base.transform.Find("SouSuoInput").GetComponent<InputField>().SetTextWithoutNotify(MemberName_SouSuo);
        switch (ID)
        {
            case "ADA":
                if (base.transform.Find("AllShaiA").Find("STipD").Find("MinOld")
                    .GetComponent<InputField>()
                    .text != "-" && base.transform.Find("AllShaiA").Find("STipD").Find("MinOld")
                    .GetComponent<InputField>()
                    .text != "" && base.transform.Find("AllShaiA").Find("STipD").Find("MinOld")
                    .GetComponent<InputField>()
                    .text != null)
                {
                    OldMin_Shai = int.Parse(base.transform.Find("AllShaiA").Find("STipD").Find("MinOld")
                        .GetComponent<InputField>()
                        .text);
                    if (OldMin_Shai <= 0)
                    {
                        OldMin_Shai = 1;
                        base.transform.Find("AllShaiA").Find("STipD").Find("MinOld")
                            .GetComponent<InputField>()
                            .SetTextWithoutNotify(OldMin_Shai.ToString());
                    }
                }
                else
                {
                    OldMin_Shai = 1;
                    base.transform.Find("AllShaiA").Find("STipD").Find("MinOld")
                        .GetComponent<InputField>()
                        .SetTextWithoutNotify(OldMin_Shai.ToString());
                }
                break;
            case "ADB":
                if (base.transform.Find("AllShaiA").Find("STipD").Find("MaxOld")
                    .GetComponent<InputField>()
                    .text != "-" && base.transform.Find("AllShaiA").Find("STipD").Find("MaxOld")
                    .GetComponent<InputField>()
                    .text != "" && base.transform.Find("AllShaiA").Find("STipD").Find("MaxOld")
                    .GetComponent<InputField>()
                    .text != null)
                {
                    OldMax_Shai = int.Parse(base.transform.Find("AllShaiA").Find("STipD").Find("MaxOld")
                        .GetComponent<InputField>()
                        .text);
                    if (OldMax_Shai <= 0)
                    {
                        OldMax_Shai = 1;
                        base.transform.Find("AllShaiA").Find("STipD").Find("MaxOld")
                            .GetComponent<InputField>()
                            .SetTextWithoutNotify(OldMax_Shai.ToString());
                    }
                }
                else
                {
                    OldMax_Shai = 150;
                    base.transform.Find("AllShaiA").Find("STipD").Find("MaxOld")
                        .GetComponent<InputField>()
                        .SetTextWithoutNotify(OldMax_Shai.ToString());
                }
                break;
            case "BCA":
                if (base.transform.Find("AllShaiB").Find("STipC").Find("MinOld")
                    .GetComponent<InputField>()
                    .text != "-" && base.transform.Find("AllShaiB").Find("STipC").Find("MinOld")
                    .GetComponent<InputField>()
                    .text != "" && base.transform.Find("AllShaiB").Find("STipC").Find("MinOld")
                    .GetComponent<InputField>()
                    .text != null)
                {
                    OldMin_Menke_Shai = int.Parse(base.transform.Find("AllShaiB").Find("STipC").Find("MinOld")
                        .GetComponent<InputField>()
                        .text);
                    if (OldMin_Menke_Shai <= 0)
                    {
                        OldMin_Menke_Shai = 1;
                        base.transform.Find("AllShaiB").Find("STipC").Find("MinOld")
                            .GetComponent<InputField>()
                            .SetTextWithoutNotify(OldMin_Menke_Shai.ToString());
                    }
                }
                else
                {
                    OldMin_Menke_Shai = 1;
                    base.transform.Find("AllShaiB").Find("STipC").Find("MinOld")
                        .GetComponent<InputField>()
                        .SetTextWithoutNotify(OldMin_Menke_Shai.ToString());
                }
                break;
            case "BCB":
                if (base.transform.Find("AllShaiB").Find("STipC").Find("MaxOld")
                    .GetComponent<InputField>()
                    .text != "-" && base.transform.Find("AllShaiB").Find("STipC").Find("MaxOld")
                    .GetComponent<InputField>()
                    .text != "" && base.transform.Find("AllShaiB").Find("STipC").Find("MaxOld")
                    .GetComponent<InputField>()
                    .text != null)
                {
                    OldMax_Menke_Shai = int.Parse(base.transform.Find("AllShaiB").Find("STipC").Find("MaxOld")
                        .GetComponent<InputField>()
                        .text);
                    if (OldMax_Menke_Shai <= 0)
                    {
                        OldMax_Menke_Shai = 1;
                        base.transform.Find("AllShaiB").Find("STipC").Find("MaxOld")
                            .GetComponent<InputField>()
                            .SetTextWithoutNotify(OldMax_Menke_Shai.ToString());
                    }
                }
                else
                {
                    OldMax_Menke_Shai = 150;
                    base.transform.Find("AllShaiB").Find("STipC").Find("MaxOld")
                        .GetComponent<InputField>()
                        .SetTextWithoutNotify(OldMax_Menke_Shai.ToString());
                }
                break;
            case "CDA":
                if (base.transform.Find("AllShaiC").Find("STipD").Find("MinOld")
                    .GetComponent<InputField>()
                    .text != "-" && base.transform.Find("AllShaiC").Find("STipD").Find("MinOld")
                    .GetComponent<InputField>()
                    .text != "" && base.transform.Find("AllShaiC").Find("STipD").Find("MinOld")
                    .GetComponent<InputField>()
                    .text != null)
                {
                    OldMin_Shai = int.Parse(base.transform.Find("AllShaiC").Find("STipD").Find("MinOld")
                        .GetComponent<InputField>()
                        .text);
                    if (OldMin_Shai <= 0)
                    {
                        OldMin_Shai = 1;
                        base.transform.Find("AllShaiC").Find("STipD").Find("MinOld")
                            .GetComponent<InputField>()
                            .SetTextWithoutNotify(OldMin_Shai.ToString());
                    }
                }
                else
                {
                    OldMin_Shai = 1;
                    base.transform.Find("AllShaiC").Find("STipD").Find("MinOld")
                        .GetComponent<InputField>()
                        .SetTextWithoutNotify(OldMin_Shai.ToString());
                }
                break;
            case "CDB":
                if (base.transform.Find("AllShaiC").Find("STipD").Find("MaxOld")
                    .GetComponent<InputField>()
                    .text != "-" && base.transform.Find("AllShaiC").Find("STipD").Find("MaxOld")
                    .GetComponent<InputField>()
                    .text != "" && base.transform.Find("AllShaiC").Find("STipD").Find("MaxOld")
                    .GetComponent<InputField>()
                    .text != null)
                {
                    OldMax_Shai = int.Parse(base.transform.Find("AllShaiC").Find("STipD").Find("MaxOld")
                        .GetComponent<InputField>()
                        .text);
                    if (OldMax_Shai <= 0)
                    {
                        OldMax_Shai = 1;
                        base.transform.Find("AllShaiC").Find("STipD").Find("MaxOld")
                            .GetComponent<InputField>()
                            .SetTextWithoutNotify(OldMax_Shai.ToString());
                    }
                }
                else
                {
                    OldMax_Shai = 150;
                    base.transform.Find("AllShaiC").Find("STipD").Find("MaxOld")
                        .GetComponent<InputField>()
                        .SetTextWithoutNotify(OldMax_Shai.ToString());
                }
                break;
            case "DDA":
                if (base.transform.Find("AllShaiD").Find("STipD").Find("MinOld")
                    .GetComponent<InputField>()
                    .text != "-" && base.transform.Find("AllShaiD").Find("STipD").Find("MinOld")
                    .GetComponent<InputField>()
                    .text != "" && base.transform.Find("AllShaiD").Find("STipD").Find("MinOld")
                    .GetComponent<InputField>()
                    .text != null)
                {
                    OldMin_Shai = int.Parse(base.transform.Find("AllShaiD").Find("STipD").Find("MinOld")
                        .GetComponent<InputField>()
                        .text);
                    if (OldMin_Shai <= 0)
                    {
                        OldMin_Shai = 1;
                        base.transform.Find("AllShaiD").Find("STipD").Find("MinOld")
                            .GetComponent<InputField>()
                            .SetTextWithoutNotify(OldMin_Shai.ToString());
                    }
                }
                else
                {
                    OldMin_Shai = 1;
                    base.transform.Find("AllShaiD").Find("STipD").Find("MinOld")
                        .GetComponent<InputField>()
                        .SetTextWithoutNotify(OldMin_Shai.ToString());
                }
                break;
            case "DDB":
                if (base.transform.Find("AllShaiD").Find("STipD").Find("MaxOld")
                    .GetComponent<InputField>()
                    .text != "-" && base.transform.Find("AllShaiD").Find("STipD").Find("MaxOld")
                    .GetComponent<InputField>()
                    .text != "" && base.transform.Find("AllShaiD").Find("STipD").Find("MaxOld")
                    .GetComponent<InputField>()
                    .text != null)
                {
                    OldMax_Shai = int.Parse(base.transform.Find("AllShaiD").Find("STipD").Find("MaxOld")
                        .GetComponent<InputField>()
                        .text);
                    if (OldMax_Shai <= 0)
                    {
                        OldMax_Shai = 1;
                        base.transform.Find("AllShaiD").Find("STipD").Find("MaxOld")
                            .GetComponent<InputField>()
                            .SetTextWithoutNotify(OldMax_Shai.ToString());
                    }
                }
                else
                {
                    OldMax_Shai = 150;
                    base.transform.Find("AllShaiD").Find("STipD").Find("MaxOld")
                        .GetComponent<InputField>()
                        .SetTextWithoutNotify(OldMax_Shai.ToString());
                }
                break;
        }
        Shai_MemberShow();
    }

    public void SouSuoInput()
    {
        if (base.transform.Find("SouSuoInput").GetComponent<InputField>().text != "" && base.transform.Find("SouSuoInput").GetComponent<InputField>().text != null)
        {
            MemberName_SouSuo = base.transform.Find("SouSuoInput").GetComponent<InputField>().text;
            List<List<int>> list = new List<List<int>>();
            if (!IsMenkeClass && !IsShiJiaClass && !IsHuangShiClass && !IsNongZhuangClass)
            {
                for (int i = 0; i < Mainload.Member_now.Count; i++)
                {
                    if (Mainload.Member_now[i][4].Split('|')[0].Contains(MemberName_SouSuo))
                    {
                        list.Add(new List<int> { 0, i });
                    }
                }
                for (int j = 0; j < Mainload.Member_qu.Count; j++)
                {
                    if (Mainload.Member_qu[j][2].Split('|')[0].Contains(MemberName_SouSuo))
                    {
                        list.Add(new List<int> { 1, j });
                    }
                }
            }
            else if(IsMenkeClass)
            {
                for (int i = 0; i < Mainload.MenKe_Now.Count; i++)
                {
                    if (Mainload.MenKe_Now[i][2].Split('|')[0].Contains(MemberName_SouSuo))
                    {
                        list.Add(new List<int> { 2, i });
                    }
                }
            }
            else if (IsShiJiaClass)
            {
                for (int i = 0; i < Mainload.Member_other.Count; i++)
                {
                    for (int j = 0; j < Mainload.Member_other[i].Count; j++)
                    {
                        if (Mainload.Member_other[i][j][2].Split('|')[0].Contains(MemberName_SouSuo))
                        {
                            list.Add(new List<int> { 3, i, j });
                        }
                    }
                }

                for (int i = 0; i < Mainload.Member_Other_qu.Count; i++)
                {
                    for (int j = 0; j < Mainload.Member_Other_qu[i].Count; j++)
                    {
                        if (Mainload.Member_Other_qu[i][j][2].Split('|')[0].Contains(MemberName_SouSuo))
                        {
                            list.Add(new List<int> { 4, i, j });
                        }
                    }
                }
            }
            else if (IsHuangShiClass)
            {
                for (int i = 0; i < Mainload.Member_King.Count; i++)
                {
                    if (Mainload.Member_King[i][2].Split('|')[0].Contains(MemberName_SouSuo))
                    {
                        list.Add(new List<int> { 5, i });
                    }
                }
                for (int j = 0; j < Mainload.Member_King_qu.Count; j++)
                {
                    if (Mainload.Member_King_qu[j][2].Split('|')[0].Contains(MemberName_SouSuo))
                    {
                        list.Add(new List<int> { 6, j });
                    }
                }
            }
            else if (IsNongZhuangClass)
            {
                for (int i = 0; i < Mainload.ZhuangTou_now.Count; i++)
                {
                    for (int j = 0; i < Mainload.ZhuangTou_now[i].Count; j++)
                    {
                        for (int k = 0; i < Mainload.ZhuangTou_now[i][j].Count; k++)
                        {
                            if (Mainload.ZhuangTou_now[i][j][k][2].Split('|')[0].Contains(MemberName_SouSuo))
                            {
                                list.Add(new List<int> { 7, i, j, k });
                            }
                        }
                    }
                }
            }
            SouSuoMemberShow(list);
        }
        else
        {
            Shai_MemberShow();
        }
    }

    private void SouSuoMemberShow(List<List<int>> MemberSouSuo)
    {
        MemberID_ShowNow = "null";
        ShowID_Shai_Sou = 1;
        SelectBT(0, 0, "null", new Vector3(0f, 0f, 0f));
        for (int i = 0; i < base.transform.Find("AllMember").Find("Viewport").Find("Content")
            .childCount; i++)
        {
            Object.Destroy(base.transform.Find("AllMember").Find("Viewport").Find("Content")
                .GetChild(i)
                .gameObject);
        }
        GameObject obj = Object.Instantiate(MSelectTipA);
        obj.name = "MSelectTip";
        obj.transform.SetParent(base.transform.Find("AllMember").Find("Viewport").Find("Content"));
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.localPosition = new Vector3(0f, 0f, 0f);
        if (MemberSouSuo.Count <= 0)
        {
            return;
        }
        int num = 0;
        for (int j = 0; j < MemberSouSuo.Count; j++)
        {
            if (MemberSouSuo[j][0] == 0)
            {
                GameObject obj2 = Object.Instantiate(PerMemberA);
                obj2.name = MemberSouSuo[j][1].ToString();
                obj2.transform.GetComponent<PerMemberBT>().ShowId = 210;
                obj2.transform.SetParent(base.transform.Find("AllMember").Find("Viewport").Find("Content"));
                obj2.transform.localScale = new Vector3(1f, 1f, 1f);
                obj2.transform.localPosition = new Vector3(50 + 90 * (num % 10), -63 - 115 * Mathf.FloorToInt((float)num / 10f), 0f);
                num++;
            }
            else if (MemberSouSuo[j][0] == 1)
            {
                GameObject obj3 = Object.Instantiate(PerMemberA);
                obj3.name = MemberSouSuo[j][1].ToString();
                obj3.transform.GetComponent<PerMemberBT>().ShowId = 211;
                obj3.transform.SetParent(base.transform.Find("AllMember").Find("Viewport").Find("Content"));
                obj3.transform.localScale = new Vector3(1f, 1f, 1f);
                obj3.transform.localPosition = new Vector3(50 + 90 * (num % 10), -63 - 115 * Mathf.FloorToInt((float)num / 10f), 0f);
                num++;
            }
            else if (MemberSouSuo[j][0] == 2)
            {
                GameObject obj4 = Object.Instantiate(PerMemberA);
                obj4.name = MemberSouSuo[j][1].ToString();
                obj4.transform.GetComponent<PerMemberBT>().ShowId = 212;
                obj4.transform.SetParent(base.transform.Find("AllMember").Find("Viewport").Find("Content"));
                obj4.transform.localScale = new Vector3(1f, 1f, 1f);
                obj4.transform.localPosition = new Vector3(50 + 90 * (num % 10), -63 - 115 * Mathf.FloorToInt((float)num / 10f), 0f);
                num++;
            }
            else if (MemberSouSuo[j][0] == 3)
            {
                GameObject obj4 = Object.Instantiate(PerMemberA);
                obj4.name = MemberSouSuo[j][1].ToString();
                obj4.transform.GetComponent<PerMemberBT>().ShowId = 213;
                obj4.transform.SetParent(base.transform.Find("AllMember").Find("Viewport").Find("Content"));
                obj4.transform.localScale = new Vector3(1f, 1f, 1f);
                obj4.transform.localPosition = new Vector3(50 + 90 * (num % 10), -63 - 115 * Mathf.FloorToInt((float)num / 10f), 0f);
                num++;
            }
            else if (MemberSouSuo[j][0] == 4)
            {
                GameObject obj4 = Object.Instantiate(PerMemberA);
                obj4.name = MemberSouSuo[j][1].ToString();
                obj4.transform.GetComponent<PerMemberBT>().ShowId = 214;
                obj4.transform.SetParent(base.transform.Find("AllMember").Find("Viewport").Find("Content"));
                obj4.transform.localScale = new Vector3(1f, 1f, 1f);
                obj4.transform.localPosition = new Vector3(50 + 90 * (num % 10), -63 - 115 * Mathf.FloorToInt((float)num / 10f), 0f);
                num++;
            }
            else if (MemberSouSuo[j][0] == 5)
            {
                GameObject obj4 = Object.Instantiate(PerMemberA);
                obj4.name = MemberSouSuo[j][1].ToString();
                obj4.transform.GetComponent<PerMemberBT>().ShowId = 215;
                obj4.transform.SetParent(base.transform.Find("AllMember").Find("Viewport").Find("Content"));
                obj4.transform.localScale = new Vector3(1f, 1f, 1f);
                obj4.transform.localPosition = new Vector3(50 + 90 * (num % 10), -63 - 115 * Mathf.FloorToInt((float)num / 10f), 0f);
                num++;
            }
            else if (MemberSouSuo[j][0] == 6)
            {
                GameObject obj4 = Object.Instantiate(PerMemberA);
                obj4.name = MemberSouSuo[j][1].ToString();
                obj4.transform.GetComponent<PerMemberBT>().ShowId = 216;
                obj4.transform.SetParent(base.transform.Find("AllMember").Find("Viewport").Find("Content"));
                obj4.transform.localScale = new Vector3(1f, 1f, 1f);
                obj4.transform.localPosition = new Vector3(50 + 90 * (num % 10), -63 - 115 * Mathf.FloorToInt((float)num / 10f), 0f);
                num++;
            }
            else if (MemberSouSuo[j][0] == 7)
            {
                GameObject obj4 = Object.Instantiate(PerMemberA);
                obj4.name = MemberSouSuo[j][1].ToString();
                obj4.transform.GetComponent<PerMemberBT>().ShowId = 217;
                obj4.transform.SetParent(base.transform.Find("AllMember").Find("Viewport").Find("Content"));
                obj4.transform.localScale = new Vector3(1f, 1f, 1f);
                obj4.transform.localPosition = new Vector3(50 + 90 * (num % 10), -63 - 115 * Mathf.FloorToInt((float)num / 10f), 0f);
                num++;
            }
        }
        base.transform.Find("AllMember").Find("Viewport").Find("Content")
            .GetComponent<RectTransform>()
            .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80 + 115 * Mathf.CeilToInt((float)num / 10f));
    }

    private void Shai_MemberShow()
    {
        MemberID_ShowNow = "null";
        ShowID_Shai_Sou = 0;
        SelectBT(0, 0, "null", new Vector3(0f, 0f, 0f));
        List<List<int>> list = new List<List<int>>();
        List<List<int>> list2 = new List<List<int>>();
        if ((!IsMenkeClass && !IsShiJiaClass && !IsHuangShiClass && !IsNongZhuangClass) || IsShiJiaClass || IsHuangShiClass)
        {
            List<string> list3 = new List<string>();
            float num = 10000000f;
            if (State_Shai == 1)
            {
                list3 = new List<string> { "0" };
            }
            else if (State_Shai == 2)
            {
                list3 = new List<string> { "16" };
            }
            else if (State_Shai == 3)
            {
                num = Mainload.HealthWarnNum;
            }
            else if (State_Shai == 4)
            {
                list3 = new List<string> { "10" };
            }
            else if (State_Shai == 5)
            {
                list3 = new List<string> { "2" };
            }
            else if (State_Shai == 6)
            {
                list3 = new List<string> { "11" };
            }
            else if (State_Shai == 7)
            {
                list3 = new List<string> { "5", "6", "7", "8" };
            }
            else if (State_Shai == 8)
            {
                list3 = new List<string> { "4" };
            }
            else if (State_Shai == 9)
            {
                list3 = new List<string>
                                            {
                                                "1", "3", "9", "12", "13", "14", "15", "17", "18", "19",
                                                "20", "21", "22", "23", "24", "25", "26"
                                            };
            }

            if (!IsMenkeClass && !IsShiJiaClass && !IsHuangShiClass && !IsNongZhuangClass)
            {
                if (MemberClass_Shai == 0 || MemberClass_Shai == 1)
                {
                    for (int i = 0; i < Mainload.Member_now.Count; i++)
                    {
                        if (float.Parse(Mainload.Member_now[i][21]) <= num &&
                            (list3.Contains(Mainload.Member_now[i][15]) || list3.Count <= 0) &&
                            (Sex_Shai == 0 || (Sex_Shai == 1 && Mainload.Member_now[i][4].Split('|')[4] == "1") || (Sex_Shai == 2 && Mainload.Member_now[i][4].Split('|')[4] == "0")) &&
                            int.Parse(Mainload.Member_now[i][6]) >= OldMin_Shai &&
                            int.Parse(Mainload.Member_now[i][6]) <= OldMax_Shai &&
                            (ShenFen_Shai == 0 || (ShenFen_Shai == 1 && Mainload.Member_now[i][12].Split('|')[0].Split('@')[0] == "5" && !FormulaData.is_WuGuan(Mainload.Member_now[i][12].Split('|')[0])) || (ShenFen_Shai == 2 && Mainload.Member_now[i][12].Split('|')[0].Split('@')[0] == "5" && FormulaData.is_WuGuan(Mainload.Member_now[i][12].Split('|')[0])) || (ShenFen_Shai == 3 && Mainload.Member_now[i][12].Split('|')[0].Split('@')[0] != "5")) &&
                            (HunYin_Shai == 0 || (HunYin_Shai == 1 && Mainload.Member_now[i][26] == "0") || (HunYin_Shai == 2 && Mainload.Member_now[i][26] == "1") || (HunYin_Shai == 3 && Mainload.Member_now[i][26] != "0" && Mainload.Member_now[i][26] != "1")) &&
                            (ZhiZe_Shai == 0 || ZhiZe_Shai == int.Parse(Mainload.Member_now[i][41].Split('|')[0]) + 1))
                        {
                            list.Add(new List<int> { 0, i });
                        }
                    }
                }
                if (MemberClass_Shai == 0 || MemberClass_Shai == 2)
                {
                    for (int j = 0; j < Mainload.Member_qu.Count; j++)
                    {
                        if (float.Parse(Mainload.Member_qu[j][16]) <= num &&
                            (list3.Contains(Mainload.Member_qu[j][11]) || list3.Count <= 0) &&
                            (Sex_Shai == 0 || (Sex_Shai == 1 && Mainload.Member_qu[j][2].Split('|')[4] == "1") || (Sex_Shai == 2 && Mainload.Member_qu[j][2].Split('|')[4] == "0")) &&
                            int.Parse(Mainload.Member_qu[j][5]) >= OldMin_Shai &&
                            int.Parse(Mainload.Member_qu[j][5]) <= OldMax_Shai &&
                            (ShenFen_Shai == 0 || ShenFen_Shai == 3) &&
                            (HunYin_Shai == 0 || (HunYin_Shai == 2 && Mainload.Member_qu[j][29] != "2") || (HunYin_Shai == 3 && Mainload.Member_qu[j][29] == "2")) &&
                            (ZhiZe_Shai == 0 || ZhiZe_Shai == int.Parse(Mainload.Member_qu[j][32].Split('|')[0]) + 1))
                        {
                            list.Add(new List<int> { 1, j });
                        }
                    }
                }
            }
            else if (IsShiJiaClass)
            {
                if (MemberClass_Shai == 0 || MemberClass_Shai == 1)
                {

                }

                if (MemberClass_Shai == 0 || MemberClass_Shai == 2)
                {

                }
            }
            else if (IsHuangShiClass)
            {
                if (MemberClass_Shai == 0 || MemberClass_Shai == 1)
                {

                }

                if (MemberClass_Shai == 0 || MemberClass_Shai == 2)
                {

                }
            }
            int index = 0;
            int num2 = 0;
            int index2 = 0;
            int num3 = 0;
            if (ShuXing_Shai == 0)
            {
                num2 = 0;
                index = 7;
            }
            else if (ShuXing_Shai == 1)
            {
                num2 = 0;
                index = 8;
            }
            else if (ShuXing_Shai == 2)
            {
                num2 = 0;
                index = 9;
            }
            else if (ShuXing_Shai == 3)
            {
                num2 = 0;
                index = 10;
            }
            else if (ShuXing_Shai == 4)
            {
                num2 = 0;
                index = 27;
            }
            else if (ShuXing_Shai == 5)
            {
                num2 = 0;
                index = 20;
            }
            else if (ShuXing_Shai >= 6)
            {
                num2 = ShuXing_Shai - 5;
                index = 33;
            }
            if (ShuXing_Shai == 0)
            {
                num3 = 0;
                index2 = 6;
            }
            else if (ShuXing_Shai == 1)
            {
                num3 = 0;
                index2 = 7;
            }
            else if (ShuXing_Shai == 2)
            {
                num3 = 0;
                index2 = 8;
            }
            else if (ShuXing_Shai == 3)
            {
                num3 = 0;
                index2 = 9;
            }
            else if (ShuXing_Shai == 4)
            {
                num3 = 0;
                index2 = 19;
            }
            else if (ShuXing_Shai == 5)
            {
                num3 = 0;
                index2 = 15;
            }
            else if (ShuXing_Shai >= 6)
            {
                num3 = ShuXing_Shai - 5;
                index2 = 23;
            }
            for (int k = 0; k < list.Count; k++)
            {
                if (list[k][0] == 0)
                {
                    if (num2 == 0)
                    {
                        bool flag = false;
                        for (int l = 0; l < list2.Count; l++)
                        {
                            if (list2[l][0] == 0)
                            {
                                if (float.Parse(Mainload.Member_now[list[k][1]][index]) > float.Parse(Mainload.Member_now[list2[l][1]][index]))
                                {
                                    list2.Insert(l, new List<int>
                                                                {
                                                                    list[k][0],
                                                                    list[k][1]
                                                                });
                                    flag = true;
                                    break;
                                }
                            }
                            else if (float.Parse(Mainload.Member_now[list[k][1]][index]) > float.Parse(Mainload.Member_qu[list2[l][1]][index2]))
                            {
                                list2.Insert(l, new List<int>
                                                            {
                                                                list[k][0],
                                                                list[k][1]
                                                            });
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            list2.Add(new List<int>
                                                        {
                                                            list[k][0],
                                                            list[k][1]
                                                        });
                        }
                        continue;
                    }
                    bool flag2 = false;
                    for (int m = 0; m < list2.Count; m++)
                    {
                        if (list2[m][0] == 0)
                        {
                            if (Mainload.Member_now[list[k][1]][4].Split('|')[6] == num2.ToString() && Mainload.Member_now[list2[m][1]][4].Split('|')[6] == num2.ToString() && float.Parse(Mainload.Member_now[list[k][1]][index]) > float.Parse(Mainload.Member_now[list2[m][1]][index]))
                            {
                                list2.Insert(m, new List<int>
                                                            {
                                                                list[k][0],
                                                                list[k][1]
                                                            });
                                flag2 = true;
                                break;
                            }
                        }
                        else if (Mainload.Member_now[list[k][1]][4].Split('|')[6] == num2.ToString() && Mainload.Member_qu[list2[m][1]][2].Split('|')[6] == num3.ToString() && float.Parse(Mainload.Member_now[list[k][1]][index]) > float.Parse(Mainload.Member_qu[list2[m][1]][index2]))
                        {
                            list2.Insert(m, new List<int>
                                                        {
                                                            list[k][0],
                                                            list[k][1]
                                                        });
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2 && Mainload.Member_now[list[k][1]][4].Split('|')[6] == num2.ToString())
                    {
                        list2.Add(new List<int>
                                                    {
                                                        list[k][0],
                                                        list[k][1]
                                                    });
                    }
                }
                else
                {
                    if (list[k][0] != 1)
                    {
                        continue;
                    }
                    if (num3 == 0)
                    {
                        bool flag3 = false;
                        for (int n = 0; n < list2.Count; n++)
                        {
                            if (list2[n][0] == 0)
                            {
                                if (float.Parse(Mainload.Member_qu[list[k][1]][index2]) > float.Parse(Mainload.Member_now[list2[n][1]][index]))
                                {
                                    list2.Insert(n, new List<int>
                                                                {
                                                                    list[k][0],
                                                                    list[k][1]
                                                                });
                                    flag3 = true;
                                    break;
                                }
                            }
                            else if (float.Parse(Mainload.Member_qu[list[k][1]][index2]) > float.Parse(Mainload.Member_qu[list2[n][1]][index2]))
                            {
                                list2.Insert(n, new List<int>
                                                            {
                                                                list[k][0],
                                                                list[k][1]
                                                            });
                                flag3 = true;
                                break;
                            }
                        }
                        if (!flag3)
                        {
                            list2.Add(new List<int>
                                                        {
                                                            list[k][0],
                                                            list[k][1]
                                                        });
                        }
                        continue;
                    }
                    bool flag4 = false;
                    for (int num4 = 0; num4 < list2.Count; num4++)
                    {
                        if (list2[num4][0] == 0)
                        {
                            if (Mainload.Member_qu[list[k][1]][2].Split('|')[6] == num3.ToString() && Mainload.Member_now[list2[num4][1]][4].Split('|')[6] == num2.ToString() && float.Parse(Mainload.Member_qu[list[k][1]][index2]) > float.Parse(Mainload.Member_now[list2[num4][1]][index]))
                            {
                                list2.Insert(num4, new List<int>
                                                            {
                                                                list[k][0],
                                                                list[k][1]
                                                            });
                                flag4 = true;
                                break;
                            }
                        }
                        else if (Mainload.Member_qu[list[k][1]][2].Split('|')[6] == num3.ToString() && Mainload.Member_qu[list2[num4][1]][2].Split('|')[6] == num3.ToString() && float.Parse(Mainload.Member_qu[list[k][1]][index2]) > float.Parse(Mainload.Member_qu[list2[num4][1]][index2]))
                        {
                            list2.Insert(num4, new List<int>
                                                        {
                                                            list[k][0],
                                                            list[k][1]
                                                        });
                            flag4 = true;
                            break;
                        }
                    }
                    if (!flag4 && Mainload.Member_qu[list[k][1]][2].Split('|')[6] == num3.ToString())
                    {
                        list2.Add(new List<int>
                                                    {
                                                        list[k][0],
                                                        list[k][1]
                                                    });
                    }
                }
            }
        }
        else if (IsMenkeClass)
        {
            for (int num5 = 0; num5 < Mainload.MenKe_Now.Count; num5++)
            {
                if ((ZhiShi_Menke_Shai == 0 || (ZhiShi_Menke_Shai == 1 && Mainload.MenKe_Now[num5][10] == "0") || (ZhiShi_Menke_Shai >= 2 && int.Parse(Mainload.MenKe_Now[num5][10]) - 15 == ZhiShi_Menke_Shai)) && (Sex_Menke_Shai == 0 || (Sex_Menke_Shai == 1 && Mainload.MenKe_Now[num5][2].Split('|')[4] == "1") || (Sex_Menke_Shai == 2 && Mainload.MenKe_Now[num5][2].Split('|')[4] == "0")) && int.Parse(Mainload.MenKe_Now[num5][3]) >= OldMin_Menke_Shai && int.Parse(Mainload.MenKe_Now[num5][3]) <= OldMax_Menke_Shai)
                {
                    list.Add(new List<int> { 2, num5 });
                }
            }
            int index3 = 0;
            int num6 = 0;
            for (int num7 = 0; num7 < list.Count; num7++)
            {
                if (ShuXing_Menke_Shai == 0)
                {
                    num6 = 0;
                    index3 = 4;
                }
                else if (ShuXing_Menke_Shai == 1)
                {
                    num6 = 0;
                    index3 = 5;
                }
                else if (ShuXing_Menke_Shai == 2)
                {
                    num6 = 0;
                    index3 = 6;
                }
                else if (ShuXing_Menke_Shai == 3)
                {
                    num6 = 0;
                    index3 = 7;
                }
                else if (ShuXing_Menke_Shai == 4)
                {
                    num6 = 0;
                    index3 = 15;
                }
                else if (ShuXing_Menke_Shai == 5)
                {
                    num6 = 0;
                    index3 = 13;
                }
                else if (ShuXing_Menke_Shai >= 6)
                {
                    num6 = ShuXing_Menke_Shai - 5;
                    index3 = 16;
                }
                if (num6 == 0)
                {
                    bool flag5 = false;
                    for (int num8 = 0; num8 < list2.Count; num8++)
                    {
                        if (float.Parse(Mainload.MenKe_Now[list[num7][1]][index3]) > float.Parse(Mainload.MenKe_Now[list2[num8][1]][index3]))
                        {
                            list2.Insert(num8, new List<int>
                                                        {
                                                            list[num7][0],
                                                            list[num7][1]
                                                        });
                            flag5 = true;
                            break;
                        }
                    }
                    if (!flag5)
                    {
                        list2.Add(new List<int>
                                                    {
                                                        list[num7][0],
                                                        list[num7][1]
                                                    });
                    }
                    continue;
                }
                bool flag6 = false;
                for (int num9 = 0; num9 < list2.Count; num9++)
                {
                    if (Mainload.MenKe_Now[list[num7][1]][2].Split('|')[6] == num6.ToString() && Mainload.MenKe_Now[list2[num9][1]][2].Split('|')[6] == num6.ToString() && float.Parse(Mainload.MenKe_Now[list[num7][1]][index3]) > float.Parse(Mainload.MenKe_Now[list2[num9][1]][index3]))
                    {
                        list2.Insert(num9, new List<int>
                                                    {
                                                        list[num7][0],
                                                        list[num7][1]
                                                    });
                        flag6 = true;
                        break;
                    }
                }
                if (!flag6 && Mainload.MenKe_Now[list[num7][1]][2].Split('|')[6] == num6.ToString())
                {
                    list2.Add(new List<int>
                                                {
                                                    list[num7][0],
                                                    list[num7][1]
                                                });
                }
            }
        }
        else if (IsNongZhuangClass)
        {

        }
        for (int num10 = 0; num10 < base.transform.Find("AllMember").Find("Viewport").Find("Content")
                 .childCount; num10++)
        {
            Object.Destroy(base.transform.Find("AllMember").Find("Viewport").Find("Content")
                    .GetChild(num10)
                    .gameObject);
        }
        GameObject obj = Object.Instantiate(MSelectTipA);
        obj.name = "MSelectTip";
        obj.transform.SetParent(base.transform.Find("AllMember").Find("Viewport").Find("Content"));
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.localPosition = new Vector3(0f, 0f, 0f);
        if (list.Count <= 0)
        {
            return;
        }
        int num11 = 0;
        for (int num12 = 0; num12 < list2.Count; num12++)
        {
            if (list2[num12][0] == 0)
            {
                GameObject obj2 = Object.Instantiate(PerMemberA);
                obj2.name = list2[num12][1].ToString();
                obj2.transform.GetComponent<PerMemberBT>().ShowId = 210;
                obj2.transform.SetParent(base.transform.Find("AllMember").Find("Viewport").Find("Content"));
                obj2.transform.localScale = new Vector3(1f, 1f, 1f);
                obj2.transform.localPosition = new Vector3(50 + 90 * (num11 % 10), -63 - 115 * Mathf.FloorToInt((float)num11 / 10f), 0f);
                num11++;
            }
            else if (list2[num12][0] == 1)
            {
                GameObject obj3 = Object.Instantiate(PerMemberA);
                obj3.name = list2[num12][1].ToString();
                obj3.transform.GetComponent<PerMemberBT>().ShowId = 211;
                obj3.transform.SetParent(base.transform.Find("AllMember").Find("Viewport").Find("Content"));
                obj3.transform.localScale = new Vector3(1f, 1f, 1f);
                obj3.transform.localPosition = new Vector3(50 + 90 * (num11 % 10), -63 - 115 * Mathf.FloorToInt((float)num11 / 10f), 0f);
                num11++;
            }
            else if (list2[num12][0] == 2)
            {
                GameObject obj4 = Object.Instantiate(PerMemberA);
                obj4.name = list2[num12][1].ToString();
                obj4.transform.GetComponent<PerMemberBT>().ShowId = 212;
                obj4.transform.SetParent(base.transform.Find("AllMember").Find("Viewport").Find("Content"));
                obj4.transform.localScale = new Vector3(1f, 1f, 1f);
                obj4.transform.localPosition = new Vector3(50 + 90 * (num11 % 10), -63 - 115 * Mathf.FloorToInt((float)num11 / 10f), 0f);
                num11++;
            }
        }
        base.transform.Find("AllMember").Find("Viewport").Find("Content")
            .GetComponent<RectTransform>()
            .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80 + 115 * Mathf.CeilToInt((float)num11 / 10f));
    }

    public void SelectBT(int MemberClass_S, int MemberIndex_S, string MemberID, Vector3 SMPosi)
    {
        if (MemberID == "null")
        {
            base.transform.Find("CheatPanelA1").gameObject.SetActive(value: false);
            base.transform.Find("CheatPanelA2").gameObject.SetActive(value: false);
            base.transform.Find("CheatPanelB").gameObject.SetActive(value: false);
            base.transform.Find("CheatPanelC1").gameObject.SetActive(value: false);
            base.transform.Find("CheatPanelC2").gameObject.SetActive(value: false);
            base.transform.Find("CheatPanelD1").gameObject.SetActive(value: false);
            base.transform.Find("CheatPanelD2").gameObject.SetActive(value: false);
            base.transform.Find("CheatPanelE").gameObject.SetActive(value: false);
        }
        else if (MemberID_ShowNow != MemberID)
        {
            base.transform.Find("AllMember").Find("Viewport").Find("Content")
                .Find("MSelectTip")
                .gameObject.SetActive(value: true);
            base.transform.Find("AllMember").Find("Viewport").Find("Content")
                .Find("MSelectTip")
                .position = SMPosi;
            MemberID_ShowNow = MemberID;
            MemberClass = MemberClass_S;
            MemberIndex = MemberIndex_S;
            AllActShow();
        }
    }


    public void AllActShow()    //修改窗口显示逻辑
    {
        if (!IsMenkeClass && !IsShiJiaClass && !IsHuangShiClass && !IsNongZhuangClass)
        {
            if (MemberClass == 0)
            {
                base.transform.Find("CheatPanelA1").gameObject.SetActive(value: true);
                base.transform.Find("CheatPanelA2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelB").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelE").gameObject.SetActive(value: false);
            }
            else if(MemberClass == 1)
            {
                base.transform.Find("CheatPanelA1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelA2").gameObject.SetActive(value: true);
                base.transform.Find("CheatPanelB").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelE").gameObject.SetActive(value: false);
            }
        }
        else if (IsMenkeClass)
        {
            if (MemberClass == 2)
            {
                base.transform.Find("CheatPanelA1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelA2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelB").gameObject.SetActive(value: true);
                base.transform.Find("CheatPanelC1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelE").gameObject.SetActive(value: false);
            }
        }
        else if (IsShiJiaClass)
        {
            if (MemberClass == 3)
            {
                base.transform.Find("CheatPanelA1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelA2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelB").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC1").gameObject.SetActive(value: true);
                base.transform.Find("CheatPanelC2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelE").gameObject.SetActive(value: false);
            }
            else if (MemberClass == 4)
            {
                base.transform.Find("CheatPanelA1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelA2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelB").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC2").gameObject.SetActive(value: true);
                base.transform.Find("CheatPanelD1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelE").gameObject.SetActive(value: false);
            }
        }
        else if (IsHuangShiClass)
        {
            if (MemberClass == 5)
            {
                base.transform.Find("CheatPanelA1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelA2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelB").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD1").gameObject.SetActive(value: true);
                base.transform.Find("CheatPanelD2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelE").gameObject.SetActive(value: false);
            }
            else if (MemberClass == 6)
            {
                base.transform.Find("CheatPanelA1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelA2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelB").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD2").gameObject.SetActive(value: true);
                base.transform.Find("CheatPanelE").gameObject.SetActive(value: false);
            }
        }
        else if (IsNongZhuangClass)
        {
            if (MemberClass == 7)
            {
                base.transform.Find("CheatPanelA1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelA2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelB").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelC2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD1").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelD2").gameObject.SetActive(value: false);
                base.transform.Find("CheatPanelE").gameObject.SetActive(value: true);
            }
        }
    }

    private void CloseBT()
    {
        IsMemberCheatPanelOpen = false;
    }

    public void UpdateMemberShow()
    {
        if (ShowID_Shai_Sou == 0)
        {
            Shai_MemberShow();
        }
        else
        {
            SouSuoInput();
        }
    }
}
