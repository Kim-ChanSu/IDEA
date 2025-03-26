using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTopCharacterInformWindow : MonoBehaviour
{
    [SerializeField]
    private GameObject leaderHPImage;
    [SerializeField]
    private GameObject leaderNameText;
    [SerializeField]
    private GameObject teamNameText;
    [SerializeField]
    private GameObject strategicPointText;
    [SerializeField]
    private GameObject leaderFace;
    [SerializeField]
    private GameObject characterWindow;
    [SerializeField]
    private GameObject battleTopCharacterInformPrefab;
    private BattleManager battleManager;
    private bool isEnemy;
    
    public void InitBattleTopCharacterInformWindow(BattleManager newBattleManager, bool newIsEnemy)
    {
        isEnemy = newIsEnemy;
        battleManager = newBattleManager;
        CreateBattleTopCharacterInformPrefab();
        UpdateBattleTopCharacterInformWindow();
    }

    public void UpdateBattleTopCharacterInformWindow()
    {
        #region
        if (isEnemy == false)
        {
            leaderNameText.GetComponent<Text>().text = battleManager.playerCharacters[GameManager.instance.playerLeaderNum].GetComponent<Character>().status.inGameName;
            teamNameText.GetComponent<Text>().text = GameManager.instance.playerTeamName;
            strategicPointText.GetComponent<Text>().text = battleManager.GetPlayerStrategicPointCount() + "";

            Character character = battleManager.playerCharacters[GameManager.instance.playerLeaderNum].GetComponent<Character>();
            int faceNum = (int)character.sympathyType;
            if(character.status.face.Length > faceNum) //공감 상태에 따라 얼굴바꾸기
            { 
                leaderFace.gameObject.GetComponent<Image>().sprite = character.status.face[faceNum];
            }
            else
            { 
                if(character.status.face.Length > 0)
                { 
                    leaderFace.GetComponent<Image>().sprite = character.status.face[0];
                }
                else
                { 
                    leaderFace.GetComponent<Image>().sprite = GameManager.instance.GetDefaultFace();
                }
            }

            leaderHPImage.GetComponent<Image>().fillAmount = (float)battleManager.playerCharacters[GameManager.instance.playerLeaderNum].GetComponent<Character>().status.HP / (float)battleManager.playerCharacters[GameManager.instance.playerLeaderNum].GetComponent<Character>().status.maxHP;
            
        }
        else
        {
            leaderNameText.GetComponent<Text>().text = battleManager.enemyCharacters[GameManager.instance.enemyLeaderNum].GetComponent<Character>().status.inGameName;
            teamNameText.GetComponent<Text>().text = GameManager.instance.enemyTeamName;
            strategicPointText.GetComponent<Text>().text = battleManager.GetEnemyStrategicPointCount() + "";

            Character character = battleManager.enemyCharacters[GameManager.instance.enemyLeaderNum].GetComponent<Character>();
            int faceNum = (int)character.sympathyType;
            if(character.status.face.Length > faceNum) //공감 상태에 따라 얼굴바꾸기
            { 
                leaderFace.gameObject.GetComponent<Image>().sprite = character.status.face[faceNum];
            }
            else
            { 
                if(character.status.face.Length > 0)
                { 
                    leaderFace.GetComponent<Image>().sprite = character.status.face[0];
                }
                else
                { 
                    leaderFace.GetComponent<Image>().sprite = GameManager.instance.GetDefaultFace();
                }
            }

            leaderHPImage.GetComponent<Image>().fillAmount = (float)battleManager.enemyCharacters[GameManager.instance.enemyLeaderNum].GetComponent<Character>().status.HP / (float)battleManager.enemyCharacters[GameManager.instance.enemyLeaderNum].GetComponent<Character>().status.maxHP;

        }    
        
        UpdateBattleTopCharacterInform();
        #endregion
    }

    private void CreateBattleTopCharacterInformPrefab()
    {
        if (isEnemy == false)
        {
            for (int i = 0; i < battleManager.playerCharacters.Length; i++)
            {
                if (i != GameManager.instance.playerLeaderNum)
                {
                    GameObject informPrefab = Instantiate(battleTopCharacterInformPrefab);
                    informPrefab.name = "PlayerCharacterInform_" + i;
                    informPrefab.GetComponent<BattleTopCharacterInform>().SetBattleTopCharacterInform(battleManager.playerCharacters[i].GetComponent<Character>());

                    informPrefab.transform.SetParent(characterWindow.transform);
                    informPrefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
            }
        }
        else
        {
            for (int i = 0; i < battleManager.enemyCharacters.Length; i++)
            {
                if (i != GameManager.instance.enemyLeaderNum)
                {
                    GameObject informPrefab = Instantiate(battleTopCharacterInformPrefab);
                    informPrefab.name = "EnemyCharacterInform_" + i;
                    informPrefab.GetComponent<BattleTopCharacterInform>().SetBattleTopCharacterInform(battleManager.enemyCharacters[i].GetComponent<Character>());

                    informPrefab.transform.SetParent(characterWindow.transform);
                    informPrefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
            }
        }
    }

    private void UpdateBattleTopCharacterInform()
    {
        for (int i = 0; i < characterWindow.transform.childCount; i++)
        {
            characterWindow.transform.GetChild(i).GetComponent<BattleTopCharacterInform>().UpdateCharacterInform();
        }
    }
}
