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

        [Tooltip("종료 시간")]
        [SerializeField]
        private float finishTime;
        private int currentTime;

        private readonly string GOLD = "#F6D72E";
        private readonly string SILVER = "#7D8785";
        private readonly string COPPER = "#6F461D";
        private readonly string NONE = "#FFFFFF";

        [Tooltip("최소값으로 설정해주세요")]
        [SerializeField]
        private int minGoldScore;
        [Tooltip("최댓값으로 설정해주세요")]
        [SerializeField]
        private int maxSilverScore;
        [Tooltip("최솟값으로 설정해주세요")]
        [SerializeField]
        private int minSilverScore;
        [Tooltip("최댓값으로 설정해주세요")]
        [SerializeField]
        private int maxCopperScore;
        [Tooltip("최솟값으로 설정해주세요")]
        [SerializeField]
        private int minCopperScore;
        [Tooltip("최댓값으로 설정해주세요")]
        [SerializeField]
        private int maxNoneScore;
        [Tooltip("최솟값으로 설정해주세요")]
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
                this.nextGradeText.text = "최고등급!";
            }
            else
            {
                int tempScore = targetValue - currentScore;
                this.nextGradeText.text = "다음 등급까지 " + tempScore.ToString() + "회!";
            }
        }

    }
}