using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TrainingResult
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Text resultText;

        [SerializeField]
        private GameObject InstanceParent;

        [SerializeField]
        private GameObject characterPrefab;

        private int playableCharacterNumber;
        private float expRatio;
        private float getExp;

        private int beforeAbilityNum;
        private int afterAbilityNum;
        private float beforeExp;

        [Tooltip("경험치바 재생시간 속도")]
        [SerializeField]
        [Range(1f,10f)]
        private float expAnimationTime;

        void Start()
        {
            SettingUI();
            CreateUI();
            GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName("MiniGame_Result"));
        }

        public void SetNextButton()
        {
            GameManager.instance.NextDay();
        }

        public int GetPlayableCharacterNumber()
        {
            int number = 0;

            for (int i = 0; i < GameManager.instance.useableCharacter.Length; i++)
            {
                if (GameManager.instance.useableCharacter[i] == true)
                {
                    number++;
                }
            }

            return number;
        }

        public void CreateUI()
        {
            for (int i = 0; i < playableCharacterNumber; i++)
            {
                GameObject tempCharcterCard = Instantiate(characterPrefab, InstanceParent.transform);     
                SettingCharacterFace(ref tempCharcterCard, i);
                SettingCharacterAbility(ref tempCharcterCard, i);
                SettingCharacterExp(ref tempCharcterCard, i);
            }
        }

        public void SettingCharacterFace(ref GameObject prefab, int index)
        {
            prefab.GetComponent<TrainingResult.CharacterCard>().SetHappyFace(GameManager.instance.PlayerCharacter[index].face[3]);
            prefab.GetComponent<TrainingResult.CharacterCard>().SetFace(GameManager.instance.PlayerCharacter[index].face[0]);
        }

        public void SettingCharacterAbility(ref GameObject prefab, int index)
        {
            prefab.GetComponent<TrainingResult.CharacterCard>().SetSelectStatus((int)GameManager.instance.PlayerCharacter[index].trainStatus);
            SetBeforeSelectStatus(ref prefab, index);
            this.getExp = GameManager.instance.trainSpendGold * this.expRatio;
            SetEXPUpgrade(index, (int)getExp);
            SetAfterSelectStatus(ref prefab, index);
            prefab.GetComponent<TrainingResult.CharacterCard>().SetAddStatusText(afterAbilityNum - beforeAbilityNum);
            prefab.GetComponent<TrainingResult.CharacterCard>().SetHideStatusNumber();
            prefab.GetComponent<TrainingResult.CharacterCard>().SetCharacterName(GameManager.instance.PlayerCharacter[index].inGameName);
        }

        public void SettingCharacterExp(ref GameObject prefab, int index)
        {
            prefab.GetComponent<TrainingResult.CharacterCard>().SetExpPlayTime(this.expAnimationTime);
            prefab.GetComponent<TrainingResult.CharacterCard>().SetBeforeExpAmount();
            prefab.GetComponent<TrainingResult.CharacterCard>().SetRepeatExpCount();
            prefab.GetComponent<TrainingResult.CharacterCard>().SetExpBarInit();
            prefab.GetComponent<TrainingResult.CharacterCard>().SetStatusUpInfo();
        }

        public void SettingUI()
        {
            this.playableCharacterNumber = GetPlayableCharacterNumber();
            SettingResultUI();
        }

        public void SettingResultUI()
        {
            string tempResult = PlayerPrefs.GetString("ResultReward");

            switch (tempResult)
            {
                case "NONE":
                    this.resultText.text = "Bad!";
                    this.expRatio = 0.75f;
                    break;
                case "COPPER":
                    this.resultText.text = "Soso!";
                    this.expRatio = 1.0f;
                    break;
                case "SILVER":
                    this.resultText.text = "Good!";
                    this.expRatio = 1.25f;
                    break;
                case "GOLD":
                    this.resultText.text = "Perfect!";
                    this.expRatio = 1.5f;
                    break;
            }
        }

        public void SetBeforeSelectStatus(ref GameObject prefab, int index)
        {
            int value = (int)GameManager.instance.PlayerCharacter[index].trainStatus;

            switch (value)
            {
                case 0:
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetBeforeStatusText(GameManager.instance.PlayerCharacter[index].maxHP);
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetBeforeExp((float)GameManager.instance.PlayerCharacter[index].maxHPEXP);
                    this.beforeAbilityNum = GameManager.instance.PlayerCharacter[index].maxHP;
                    this.beforeExp = (float)GameManager.instance.PlayerCharacter[index].maxHPEXP;
                    break;
                case 1:
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetBeforeStatusText(GameManager.instance.PlayerCharacter[index].maxMP);
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetBeforeExp((float)GameManager.instance.PlayerCharacter[index].maxMPEXP);
                    this.beforeAbilityNum = GameManager.instance.PlayerCharacter[index].maxMP;
                    this.beforeExp = (float)GameManager.instance.PlayerCharacter[index].maxMPEXP;
                    break;
                case 2:
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetBeforeStatusText(GameManager.instance.PlayerCharacter[index].ATK);
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetBeforeExp((float)GameManager.instance.PlayerCharacter[index].ATKEXP);
                    this.beforeAbilityNum = GameManager.instance.PlayerCharacter[index].ATK;
                    this.beforeExp = (float)GameManager.instance.PlayerCharacter[index].ATKEXP;
                    break;
                case 3:
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetBeforeStatusText(GameManager.instance.PlayerCharacter[index].MAK);
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetBeforeExp((float)GameManager.instance.PlayerCharacter[index].MAKEXP);
                    this.beforeAbilityNum = GameManager.instance.PlayerCharacter[index].MAK;
                    this.beforeExp = (float)GameManager.instance.PlayerCharacter[index].MAKEXP;
                    break;
                case 4:
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetBeforeStatusText(GameManager.instance.PlayerCharacter[index].DEF);
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetBeforeExp((float)GameManager.instance.PlayerCharacter[index].DEFEXP);
                    this.beforeAbilityNum = GameManager.instance.PlayerCharacter[index].DEF;
                    this.beforeExp = (float)GameManager.instance.PlayerCharacter[index].DEFEXP;
                    break;
                case 5:
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetBeforeStatusText(GameManager.instance.PlayerCharacter[index].MDF);
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetBeforeExp((float)GameManager.instance.PlayerCharacter[index].MDFEXP);
                    this.beforeAbilityNum = GameManager.instance.PlayerCharacter[index].MDF;
                    this.beforeExp = (float)GameManager.instance.PlayerCharacter[index].MDFEXP;
                    break;
                default:
                    Debug.LogError("능력치 값 오류입니다.");
                    break;
            }
        }

        public void SetAfterSelectStatus(ref GameObject prefab, int index)
        {
            int value = (int)GameManager.instance.PlayerCharacter[index].trainStatus;

            switch (value)
            {
                case 0:
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetAfterStatusText(GameManager.instance.PlayerCharacter[index].maxHP);
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetAfterExp(this.beforeExp + this.getExp);
                    this.afterAbilityNum = GameManager.instance.PlayerCharacter[index].maxHP;
                    break;
                case 1:
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetAfterStatusText(GameManager.instance.PlayerCharacter[index].maxMP);
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetAfterExp(this.beforeExp + this.getExp);
                    this.afterAbilityNum = GameManager.instance.PlayerCharacter[index].maxMP;
                    break;
                case 2:
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetAfterStatusText(GameManager.instance.PlayerCharacter[index].ATK);
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetAfterExp(this.beforeExp + this.getExp);
                    this.afterAbilityNum = GameManager.instance.PlayerCharacter[index].ATK;
                    break;
                case 3:
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetAfterStatusText(GameManager.instance.PlayerCharacter[index].MAK);
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetAfterExp(this.beforeExp + this.getExp);
                    this.afterAbilityNum = GameManager.instance.PlayerCharacter[index].MAK;
                    break;
                case 4:
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetAfterStatusText(GameManager.instance.PlayerCharacter[index].DEF);
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetAfterExp(this.beforeExp + this.getExp);
                    this.afterAbilityNum = GameManager.instance.PlayerCharacter[index].DEF;
                    break;
                case 5:
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetAfterStatusText(GameManager.instance.PlayerCharacter[index].MDF);
                    prefab.GetComponent<TrainingResult.CharacterCard>().SetAfterExp(this.beforeExp + this.getExp);
                    this.afterAbilityNum = GameManager.instance.PlayerCharacter[index].MDF;
                    break;
                default:
                    Debug.LogError("능력치 값 오류입니다.");
                    break;
            }
        }

        public void SetEXPUpgrade(int index, int value)
        {
            GameManager.instance.IncreaseStatusEXP(index, GameManager.instance.PlayerCharacter[index].trainStatus, value);
        }

    }
}