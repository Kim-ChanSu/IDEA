using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ShootingGame
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private ShootingGame.Character character;

        [SerializeField]
        private Text scoreText;
        [SerializeField]
        private GameObject gameoverPanel;

        [SerializeField]
        private Image hpGauge;
        [SerializeField]
        private Image powerGauge;

        [SerializeField]
        private Image characterFaceImage;
        [SerializeField]
        private Sprite[] characterFaceImages;

        [SerializeField]
        private Animator startStageAnimator;
        [SerializeField]
        private Animator endStageAnimator;  
        [SerializeField]
        private Animator fadeAnimator;

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
            this.scoreText.text = this.character.GetPlayerScore().ToString();
        }

        public void SettingUI()
        {
            SetPlayerScore();
            SetPlayerHPGauge();
            SetPlayerPowerGauge();
            ShowGameover();
            ChangeFace();
        }

        private void SetPlayerScore()
        {
            this.scoreText.text = this.character.GetPlayerScore().ToString(); 
        }

        private void SetPlayerHPGauge()
        {
            this.hpGauge.fillAmount = this.character.GetPlayerHP() / 100f;
        }

        public void SetPlayerPowerGauge()
        {
            this.powerGauge.fillAmount = this.character.GetPlayerPowerGauge() / 100f;
        }

        private void ShowGameover()
        {
            if (this.character.GetPlayerHP() <= 0) 
            {
                this.gameoverPanel.SetActive(true);
            }
         
        }

        public void GameOver()
        {
            this.gameoverPanel.SetActive(true);
        }

        private void ResetShowGameover()
        {
            this.gameoverPanel.SetActive(false);
        }

        private void ChangeFace()
        {
            if(this.character.GetIsHit() == true)
            {
                this.characterFaceImage.sprite = this.characterFaceImages[2];
            }
            else
            {
                this.characterFaceImage.sprite = this.characterFaceImages[0];
            }
        }

        public void ReStart()
        {
            SceneManager.LoadScene("ShootingGame");
        }

        public void SettingStartStageUI(int stageNum)
        {
            this.startStageAnimator.SetTrigger("OnEffect");
            this.startStageAnimator.GetComponent<Text>().text = "Stage" + stageNum.ToString() + "\n" + "Start";
            this.endStageAnimator.GetComponent<Text>().text = "Stage" + stageNum.ToString() + "\n" + "Clear";
        }

        public void SettingEndStageUI()
        {
            this.endStageAnimator.SetTrigger("OnEffect");
        }

        public void SettingFadeInStage()
        {
            this.fadeAnimator.SetTrigger("FadeIn");
        }

        public void SettingFadeOutStage()
        {
            this.fadeAnimator.SetTrigger("FadeOut");
        }

    }

}