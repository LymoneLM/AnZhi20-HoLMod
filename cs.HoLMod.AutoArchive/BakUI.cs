using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace cs.HoLMod.AutoArchive
{
    public class BakUI : MonoBehaviour
    {
        private void Start()
        {
            transform.Find("CloseBT").GetComponent<Button>().onClick.AddListener(new UnityAction(this.CloseBT));
            transform.Find("Back").GetComponent<Button>().onClick.AddListener(new UnityAction(this.CloseBT));
        }

        private void OnEnable()
        {
            this.OnEnableShow();
        }

        private void Update()
        {
            if (Input.GetKeyDown(Mainload.FastKey[0]) && !transform.parent.Find("LoadPanel").gameObject.activeSelf)
            {
                this.CloseBT();
            }
        }

        private void OnEnableShow()
        {
            transform.Find("Title").GetComponent<Text>().text = AllText.Text_UIA[27][Mainload.SetData[4]];
            transform.Find("DelCunDPanel").gameObject.SetActive(false);
            transform.localPosition = new Vector3(0f, 500f, 0f);
            transform.DOLocalMoveY(0f, 0.3f, false).SetEase(Ease.OutBack, 1f);
        }

        private void CloseBT()
        {
            transform.parent.Find("CunDangUI_New").gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}