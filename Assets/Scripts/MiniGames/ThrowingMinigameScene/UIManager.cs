using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ThrowingMinigame
{
    public class UIManager : MonoBehaviour
    {

        [SerializeField]
        private ThrowingMinigame.TargetConroller targetConroller;

        [SerializeField]
        private Text scoreText;
        [SerializeField]
        private Text timeText;

        [Tooltip("���� �ð�")]
        [SerializeField]
        private float finishTime;
        private int currentTime;

        private readonly string GOLD = "#F6D72E";
        private readonly string SILVER = "#7D8785";
        private readonly string COPPER = "#6F461D";
        private readonly string NONE = "#FFFFFF";

        [Tooltip("�ּҰ����� �������ּ���")]
        [SerializeField]
        private int minGoldScore;
        [Tooltip("�ִ����� �������ּ���")]
        [SerializeField]
        private int maxSilverScore;
        [Tooltip("�ּڰ����� �������ּ���")]
        [SerializeField]
        private int minSilverScore;
        [Tooltip("�ִ����� �������ּ���")]
        [SerializeField]
        private int maxCopperScore;
        [Tooltip("�ּڰ����� �������ּ���")]
        [SerializeField]
        private int minCopperScore;
        [Tooltip("�ִ����� �������ּ���")]
        [SerializeField]
        private int maxNoneScore;
        [Tooltip("�ּڰ����� �������ּ���")]
        [SerializeField]
        private int minNoneScore;

        [SerializeField]
        private Image successUI;
        private Color tempColor;

        [SerializeField]
        private Text nextGradeText;

        [SerializeField]
        private RunningMinigame.UIAnimation uiAnimation;

        void Start()
        {
            UIManagerInit();
        }

        void Update()
        {
            SettingUI();
        }

        public void UIManagerInit()
        {
            this.scoreText.text = this.targetConroller.GetHitNumber().ToString();
            SetColor(this.NONE);
            this.timeText.text = this.finishTime.ToString();
            PlayerPrefs.SetString("ResultReward", "NONE");
        }

        private void SettingUI()
        {
           
            if(this.uiAnimation.GetIsSceneLock() == true)
            {
                return;
            }

            UIManagerInit();
            this.currentTime = (int)this.finishTime;
            this.timeText.text = this.currentTime.ToString();
            SettingSuccessColor();

            this.finishTime = this.finishTime - Time.deltaTime;

            if (this.finishTime < 0)
            {
                this.finishTime = 0;
                SceneManager.LoadScene("TrainingResultScene");
            }

        }

        private void SettingSuccessColor()
        {
            int score = this.targetConroller.GetHitNumber();

            if (score <= this.maxNoneScore)
            {
                SetColor(NONE);
                SetNextGrade(this.minCopperScore, score);
                PlayerPrefs.SetString("ResultReward", "NONE");
            }
            else if (score <= this.maxCopperScore)
            {
                SetColor(COPPER);
                SetNextGrade(this.minSilverScore, score);
                PlayerPrefs.SetString("ResultReward", "COPPER");
            }
            else if (score <= this.maxSilverScore)
            {
                SetColor(SILVER);
                SetNextGrade(this.minGoldScore, score);
                PlayerPrefs.SetString("ResultReward", "SILVER");
            }
            else if (score <= this.minGoldScore)
            {
                SetColor(GOLD);
                SetNextGrade(this.minGoldScore, score);
                PlayerPrefs.SetString("ResultReward", "GOLD");
            }
            else
            {
                SetColor(GOLD);
                SetNextGrade(this.minGoldScore, score);
                PlayerPrefs.SetString("ResultReward", "GOLD");
            }

        }

        private void SetColor(string type)
        {
            ColorUtility.TryParseHtmlString(type, out tempColor);
            this.successUI.color = this.tempColor;
        }

        private void SetNextGrade(int targetValue, int currentScore)
        {
            if (currentScore >= minGoldScore)
            {
                this.nextGradeText.text = "�ְ���!";
            }
            else
            {
                int tempScore = targetValue - currentScore;
                this.nextGradeText.text = "���� ��ޱ��� " + tempScore.ToString() + "ȸ!";
            }
        }

    }
}