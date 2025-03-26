using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSubStageButton : MonoBehaviour
{
    private int subStageNum;

    public void SetSubStageButton(int num)
    {
        subStageNum = num;
        Stage stage = StageDatabaseManager.instance.GetSubStage(num);
        this.gameObject.transform.GetChild(0).transform.GetComponent<Text>().text = stage.stageName;
        this.gameObject.transform.GetChild(1).transform.GetComponent<Text>().text = "���� �ο�: " + stage.playerCharacterCount + " / " + "�Ҹ� ü��: " + stage.stageUseHealth + " / " + NameDatabaseManager.EXPName + ": " + stage.clearEXP + " / " + NameDatabaseManager.goldName + ": " + stage.clearGold;
        
        if (GameManager.instance.CheckPlayerTeamHP(stage.stageUseHealth) == false)
        {
            this.GetComponent<Button>().interactable = false;
        }
    
    }

    public void ClickButton()
    {
        if (GameManager.instance.homeManager != null)
        {
            GameManager.instance.homeManager.SetSubStage(subStageNum);
        }
    }


}
