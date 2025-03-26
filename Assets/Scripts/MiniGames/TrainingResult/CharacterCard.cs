using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TrainingResult
{

    public class CharacterCard : MonoBehaviour
    {

        [SerializeField]
        private GameObject statusUpInfo;

        [SerializeField]
        private GameObject happyFace;
        [SerializeField]
        private Image defalutFaceImage;
        [SerializeField]
        private Image happyImage;
        [SerializeField]
        private Image expAmount;

        [SerializeField]
        private Text selectStatus;
        [SerializeField]
        private Text beforeStatusText;
        [SerializeField]
        private Text afterStatusText;
        [SerializeField]
        private Text addStatusText;
        [SerializeField]
        private Text NameText;

        [SerializeField]
        private float beforeExpNum;
        [SerializeField]
        private float afterExpNum;// 총 경험치 량
        [SerializeField]
        private int upgradeExpNum;

        [SerializeField]
        private GameObject statusInfo;

        [SerializeField]
        private float minusExp = 0f;

        private float speed = 2f;
        private float expDirectProportionTime = 1.0f;
        private float targetTime;
        private float resultExp;

        private Color tempColor;

        private void Start()
        {
            HideFace();
            minusExp = 0f;
        }

        private void Update()
        {
            ShowExp();
        }

        public void SetFace(Sprite sprite)
        {
            this.defalutFaceImage.sprite = sprite;
        }

        public void SetHappyFace(Sprite sprite)
        {
            this.happyImage.sprite = sprite;
        }

        public void SetSelectStatus(int value)
        {
            switch (value)
            {
                case 0:
                    this.selectStatus.text = "체력";
                    ColorUtility.TryParseHtmlString("#980000", out tempColor);
                    this.selectStatus.color = tempColor;
                    break;
                case 1:
                    this.selectStatus.text = "마나";
                    ColorUtility.TryParseHtmlString("#87003A", out tempColor);
                    this.selectStatus.color = tempColor;
                    break;
                case 2:
                    this.selectStatus.text = "공격력";
                    ColorUtility.TryParseHtmlString("#980000", out tempColor);
                    this.selectStatus.color = tempColor;
                    break;
                case 3:
                    this.selectStatus.text = "마법력";
                    ColorUtility.TryParseHtmlString("#664B00", out tempColor);
                    this.selectStatus.color = tempColor;
                    break;
                case 4:
                    this.selectStatus.text = "방어력";
                    ColorUtility.TryParseHtmlString("#005E75", out tempColor);
                    this.selectStatus.color = tempColor;
                    break;
                case 5:
                    this.selectStatus.text = "저항력";
                    ColorUtility.TryParseHtmlString("#3F0099", out tempColor);
                    this.selectStatus.color = tempColor;
                    break;
                default:
                    Debug.LogError("능력치 값 오류입니다.");
                    break;
            }
        }

        public void SetBeforeStatusText(int value)
        {
            this.beforeStatusText.text = value.ToString();
        }

        public void SetAfterStatusText(int value)
        {
            this.afterStatusText.text = value.ToString();
        }

        public void SetAddStatusText(int value)
        {
            this.addStatusText.text = "+" + value.ToString();
        }

        public void SetBeforeExp(float value)
        {
            this.beforeExpNum = value;
        }

        public void SetAfterExp(float value)
        {
            this.afterExpNum = value;
        }

        public void SetBeforeExpAmount()
        {
            this.expAmount.fillAmount = this.beforeExpNum / (float)GameManager.instance.maxStatusEXP;
        }

        public void SetAfterExpAmount()
        {
            this.expAmount.fillAmount = this.afterExpNum / (float)GameManager.instance.maxStatusEXP;
        }

        public void SetRepeatExpCount()
        {
            this.upgradeExpNum = (int)this.afterExpNum / GameManager.instance.maxStatusEXP;
        }

        public void ShowExp()
        {
            if(this.minusExp >= this.targetTime)
            {
                this.expAmount.fillAmount = this.resultExp; // 최종값 보정
                SetShowStatusNumber();
                return;
            }

            if(this.speed < 1.0f) // 경험치량 비례 속도증가
            {
                this.speed = 1.0f;
            }
            else
            {
                this.speed = this.upgradeExpNum;
            }

            this.minusExp = this.minusExp + Time.deltaTime * this.speed * this.expDirectProportionTime;
            this.expAmount.fillAmount = this.expAmount.fillAmount + 0.1f * Time.deltaTime * this.speed * this.expDirectProportionTime;

            if (this.expAmount.fillAmount >= 1.0f)
            {
                ShowFace();
                this.expAmount.fillAmount = 0.0f;
            }

        }
        
        public void HideFace()
        {
            this.happyFace.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        public void ShowFace()
        {
            this.happyFace.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }

        public void SetExpPlayTime(float DirectProportionTime)
        {
            this.expDirectProportionTime = DirectProportionTime;
        }

        public void SetExpBarInit()
        {
            this.expAmount.fillAmount = this.beforeExpNum / 1000f;
            this.targetTime = (this.afterExpNum - this.beforeExpNum) / 100;
            this.resultExp = (this.afterExpNum % 1000) / 1000;
        }

        public void SetStatusUpInfo()
        {
            if(this.upgradeExpNum < 1)
            {
                this.statusUpInfo.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                this.statusUpInfo.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        public void SetHideStatusNumber()
        {
            for(int i=0;i< this.statusInfo.gameObject.transform.childCount;i++)
            {
                this.statusInfo.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        public void SetShowStatusNumber()
        {
            for (int i = 0; i < this.statusInfo.gameObject.transform.childCount; i++)
            {
                this.statusInfo.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        public void SetCharacterName(string name)
        {
            this.NameText.text = name;
        }

    }
}