using UnityEngine;

public class membercheat : MonoBehaviour
{
    private int MemberNowIndex_Enter_last;

    private int MemberQuIndex_Enter_Last;

    private int MenkeIndex_Enter_Last;

    private string MemberShijiaIndex_Enter_Last;

    private string MemberShijiaQuIndex_Enter_Last;

    private int MemberKingIndex_Enter_Last;

    private int MemberKingQuIndex_Enter_Last;

    private string ZhuangTouData_Enter_Last;

    private string DoctorIndex_Enter_Last;

    private void OnEnable()
    {
        OnEnableShow();
    }

    private void Update()
    {
        KeyAct();
        updateShow();
    }

    private void OnEnableShow()
    {
        Mainload.MemberNowIndex_Enter = -1;
        Mainload.MemberQuIndex_Enter = -1;
        Mainload.MenkeIndex_Enter = -1;
        Mainload.MemberShijiaIndex_Enter = "null";
        Mainload.MemberShijiaQuIndex_Enter = "null";
        Mainload.MemberKingIndex_Enter = -1;
        Mainload.MemberKingQuIndex_Enter = -1;
        Mainload.ZhuangTouData_Enter = "null";
        Mainload.DoctorIndex_Enter = "null";
        MemberNowIndex_Enter_last = Mainload.MemberNowIndex_Enter;
        MemberQuIndex_Enter_Last = Mainload.MemberQuIndex_Enter;
        MenkeIndex_Enter_Last = Mainload.MenkeIndex_Enter;
        MemberShijiaIndex_Enter_Last = Mainload.MemberShijiaIndex_Enter;
        MemberShijiaQuIndex_Enter_Last = Mainload.MemberShijiaQuIndex_Enter;
        MemberKingIndex_Enter_Last = Mainload.MemberKingIndex_Enter;
        MemberKingQuIndex_Enter_Last = Mainload.MemberKingQuIndex_Enter;
        ZhuangTouData_Enter_Last = Mainload.ZhuangTouData_Enter;
        DoctorIndex_Enter_Last = Mainload.DoctorIndex_Enter;
        base.transform.Find("PanelA").gameObject.SetActive(value: true);
        base.transform.Find("MemberNowInfoPanel").gameObject.SetActive(value: false);
        base.transform.Find("MemberQuInfoPanel").gameObject.SetActive(value: false);
        base.transform.Find("MenKeNowInfoPanel").gameObject.SetActive(value: false);
        base.transform.Find("MemberShijiaInfoPanel").gameObject.SetActive(value: false);
        base.transform.Find("MemberShijiaQuInfoPanel").gameObject.SetActive(value: false);
        base.transform.Find("MemberKingQuInfoPanel").gameObject.SetActive(value: false);
        base.transform.Find("MemberKingInfoPanel").gameObject.SetActive(value: false);
        base.transform.Find("ZhuangTouInfoPanel").gameObject.SetActive(value: false);
        base.transform.Find("DoctorInfoPanel").gameObject.SetActive(value: false);
        base.transform.Find("PropInfoPanel").gameObject.SetActive(value: false);
        base.transform.Find("HorseInfoPanel").gameObject.SetActive(value: false);
    }

    private void updateShow()
    {
        if (Mainload.MemberNowIndex_Enter != -1)
        {
            if (base.transform.Find("MemberNowInfoPanel").position != Mainload.MemberNowInfoPanelPosi || MemberNowIndex_Enter_last != Mainload.MemberNowIndex_Enter)
            {
                MemberNowIndex_Enter_last = Mainload.MemberNowIndex_Enter;
                base.transform.Find("MemberNowInfoPanel").position = Mainload.MemberNowInfoPanelPosi;
                base.transform.Find("MemberNowInfoPanel").gameObject.SetActive(value: false);
                base.transform.Find("MemberNowInfoPanel").gameObject.SetActive(value: true);
            }
            if (!base.transform.Find("MemberNowInfoPanel").gameObject.activeSelf)
            {
                base.transform.Find("MemberNowInfoPanel").gameObject.SetActive(value: true);
            }
        }
        else if (base.transform.Find("MemberNowInfoPanel").gameObject.activeSelf)
        {
            base.transform.Find("MemberNowInfoPanel").gameObject.SetActive(value: false);
        }

        if (Mainload.MemberQuIndex_Enter != -1)
        {
            if (base.transform.Find("MemberQuInfoPanel").position != Mainload.MemberQuInfoPanelPosi || MemberQuIndex_Enter_Last != Mainload.MemberQuIndex_Enter)
            {
                MemberQuIndex_Enter_Last = Mainload.MemberQuIndex_Enter;
                base.transform.Find("MemberQuInfoPanel").position = Mainload.MemberQuInfoPanelPosi;
                base.transform.Find("MemberQuInfoPanel").gameObject.SetActive(value: false);
                base.transform.Find("MemberQuInfoPanel").gameObject.SetActive(value: true);
            }
            if (!base.transform.Find("MemberQuInfoPanel").gameObject.activeSelf)
            {
                base.transform.Find("MemberQuInfoPanel").gameObject.SetActive(value: true);
            }
        }
        else if (base.transform.Find("MemberQuInfoPanel").gameObject.activeSelf)
        {
            base.transform.Find("MemberQuInfoPanel").gameObject.SetActive(value: false);
        }

        if (Mainload.MenkeIndex_Enter != -1)
        {
            if (base.transform.Find("MenKeNowInfoPanel").position != Mainload.MenkeInfoPanelPosi || MenkeIndex_Enter_Last != Mainload.MenkeIndex_Enter)
            {
                MenkeIndex_Enter_Last = Mainload.MenkeIndex_Enter;
                base.transform.Find("MenKeNowInfoPanel").position = Mainload.MenkeInfoPanelPosi;
                base.transform.Find("MenKeNowInfoPanel").gameObject.SetActive(value: false);
                base.transform.Find("MenKeNowInfoPanel").gameObject.SetActive(value: true);
            }
            if (!base.transform.Find("MenKeNowInfoPanel").gameObject.activeSelf)
            {
                base.transform.Find("MenKeNowInfoPanel").gameObject.SetActive(value: true);
            }
        }
        else if (base.transform.Find("MenKeNowInfoPanel").gameObject.activeSelf)
        {
            base.transform.Find("MenKeNowInfoPanel").gameObject.SetActive(value: false);
        }

        if (Mainload.MemberShijiaIndex_Enter != "null")
        {
            if (base.transform.Find("MemberShijiaInfoPanel").position != Mainload.MemberShijiaInfoPanelPosi || MemberShijiaIndex_Enter_Last != Mainload.MemberShijiaIndex_Enter)
            {
                MemberShijiaIndex_Enter_Last = Mainload.MemberShijiaIndex_Enter;
                base.transform.Find("MemberShijiaInfoPanel").position = Mainload.MemberShijiaInfoPanelPosi;
                base.transform.Find("MemberShijiaInfoPanel").gameObject.SetActive(value: false);
                base.transform.Find("MemberShijiaInfoPanel").gameObject.SetActive(value: true);
            }
            if (!base.transform.Find("MemberShijiaInfoPanel").gameObject.activeSelf)
            {
                base.transform.Find("MemberShijiaInfoPanel").gameObject.SetActive(value: true);
            }
        }
        else if (base.transform.Find("MemberShijiaInfoPanel").gameObject.activeSelf)
        {
            base.transform.Find("MemberShijiaInfoPanel").gameObject.SetActive(value: false);
        }

        if (Mainload.MemberShijiaQuIndex_Enter != "null")
        {
            if (base.transform.Find("MemberShijiaQuInfoPanel").position != Mainload.MemberShijiaQuInfoPanelPosi || MemberShijiaQuIndex_Enter_Last != Mainload.MemberShijiaQuIndex_Enter)
            {
                MemberShijiaQuIndex_Enter_Last = Mainload.MemberShijiaQuIndex_Enter;
                base.transform.Find("MemberShijiaQuInfoPanel").position = Mainload.MemberShijiaQuInfoPanelPosi;
                base.transform.Find("MemberShijiaQuInfoPanel").gameObject.SetActive(value: false);
                base.transform.Find("MemberShijiaQuInfoPanel").gameObject.SetActive(value: true);
            }
            if (!base.transform.Find("MemberShijiaQuInfoPanel").gameObject.activeSelf)
            {
                base.transform.Find("MemberShijiaQuInfoPanel").gameObject.SetActive(value: true);
            }
        }
        else if (base.transform.Find("MemberShijiaQuInfoPanel").gameObject.activeSelf)
        {
            base.transform.Find("MemberShijiaQuInfoPanel").gameObject.SetActive(value: false);
        }

        if (Mainload.MemberKingIndex_Enter != -1)
        {
            if (base.transform.Find("MemberKingInfoPanel").position != Mainload.MemberKingInfoPanelPosi || MemberKingIndex_Enter_Last != Mainload.MemberKingIndex_Enter)
            {
                MemberKingIndex_Enter_Last = Mainload.MemberKingIndex_Enter;
                base.transform.Find("MemberKingInfoPanel").position = Mainload.MemberKingInfoPanelPosi;
                base.transform.Find("MemberKingInfoPanel").gameObject.SetActive(value: false);
                base.transform.Find("MemberKingInfoPanel").gameObject.SetActive(value: true);
            }
            if (!base.transform.Find("MemberKingInfoPanel").gameObject.activeSelf)
            {
                base.transform.Find("MemberKingInfoPanel").gameObject.SetActive(value: true);
            }
        }
        else if (base.transform.Find("MemberKingInfoPanel").gameObject.activeSelf)
        {
            base.transform.Find("MemberKingInfoPanel").gameObject.SetActive(value: false);
        }

        if (Mainload.MemberKingQuIndex_Enter != -1)
        {
            if (base.transform.Find("MemberKingQuInfoPanel").position != Mainload.MemberKingQuInfoPanelPosi || MemberKingQuIndex_Enter_Last != Mainload.MemberKingQuIndex_Enter)
            {
                MemberKingQuIndex_Enter_Last = Mainload.MemberKingQuIndex_Enter;
                base.transform.Find("MemberKingQuInfoPanel").position = Mainload.MemberKingQuInfoPanelPosi;
                base.transform.Find("MemberKingQuInfoPanel").gameObject.SetActive(value: false);
                base.transform.Find("MemberKingQuInfoPanel").gameObject.SetActive(value: true);
            }
            if (!base.transform.Find("MemberKingQuInfoPanel").gameObject.activeSelf)
            {
                base.transform.Find("MemberKingQuInfoPanel").gameObject.SetActive(value: true);
            }
        }
        else if (base.transform.Find("MemberKingQuInfoPanel").gameObject.activeSelf)
        {
            base.transform.Find("MemberKingQuInfoPanel").gameObject.SetActive(value: false);
        }

        if (Mainload.ZhuangTouData_Enter != "null")
        {
            if (base.transform.Find("ZhuangTouInfoPanel").position != Mainload.ZhuangTouInfoPanelPosi || ZhuangTouData_Enter_Last != Mainload.ZhuangTouData_Enter)
            {
                ZhuangTouData_Enter_Last = Mainload.ZhuangTouData_Enter;
                base.transform.Find("ZhuangTouInfoPanel").position = Mainload.ZhuangTouInfoPanelPosi;
                base.transform.Find("ZhuangTouInfoPanel").gameObject.SetActive(value: false);
                base.transform.Find("ZhuangTouInfoPanel").gameObject.SetActive(value: true);
            }
            if (!base.transform.Find("ZhuangTouInfoPanel").gameObject.activeSelf)
            {
                base.transform.Find("ZhuangTouInfoPanel").gameObject.SetActive(value: true);
            }
        }
        else if (base.transform.Find("ZhuangTouInfoPanel").gameObject.activeSelf)
        {
            base.transform.Find("ZhuangTouInfoPanel").gameObject.SetActive(value: false);
        }

        if (Mainload.DoctorIndex_Enter != "null")
        {
            if (base.transform.Find("DoctorInfoPanel").position != Mainload.DoctorInfoPanelPosi || DoctorIndex_Enter_Last != Mainload.DoctorIndex_Enter)
            {
                DoctorIndex_Enter_Last = Mainload.DoctorIndex_Enter;
                base.transform.Find("DoctorInfoPanel").position = Mainload.DoctorInfoPanelPosi;
                base.transform.Find("DoctorInfoPanel").gameObject.SetActive(value: false);
                base.transform.Find("DoctorInfoPanel").gameObject.SetActive(value: true);
            }
            if (!base.transform.Find("DoctorInfoPanel").gameObject.activeSelf)
            {
                base.transform.Find("DoctorInfoPanel").gameObject.SetActive(value: true);
            }
        }
        else if (base.transform.Find("DoctorInfoPanel").gameObject.activeSelf)
        {
            base.transform.Find("DoctorInfoPanel").gameObject.SetActive(value: false);
        }
    }

    private void KeyAct()
    {
        if (Input.GetKeyDown(Mainload.FastKey[0]) || Mainload.isClick_NullA)
        {
            OnEnableShow();
            base.transform.Find("PanelA").gameObject.SetActive(value: false);
        }
    }
}
