/* 작성날짜: 2024-01-15
 * 버전: 0.0.1ver  
 * 내용: 캐릭터 선택용 버튼
 * 최근 수정 날짜: 2024-01-15
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterListButton : MonoBehaviour
{
    private int characterNum;

    public void InitializeCharactListButton(int index)
    {
        characterNum = index;

        this.transform.GetChild(0).GetComponent<Image>().sprite = GameManager.instance.PlayerCharacter[index].face[0];

        this.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "이름 /"; 
        this.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[index].inGameName;

        this.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.HPName; 
        this.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[index].maxHP.ToString() + "";

        this.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.MPName;
        this.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[index].maxMP.ToString() + "";

        this.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.levelName;
        this.transform.GetChild(4).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[index].level.ToString() + "";

        this.transform.GetChild(5).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.ATKName;
        this.transform.GetChild(5).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[index].ATK.ToString() + "";

        this.transform.GetChild(6).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.DEFName;
        this.transform.GetChild(6).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[index].DEF.ToString() + "";

        this.transform.GetChild(7).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.MAKName;
        this.transform.GetChild(7).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[index].MAK.ToString() + "";

        this.transform.GetChild(8).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.MDFName;
        this.transform.GetChild(8).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[index].MDF.ToString() + "";
        //this.transform.GetChild(9).GetComponent<Text>().text = "성별: " + " ";
        this.transform.GetChild(10).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.moveName;
        this.transform.GetChild(10).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[index].move.ToString() + "";

        this.transform.GetChild(11).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.rangeName; 
        this.transform.GetChild(11).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[index].range.ToString() + ""; 
    }

    public int GetCharacterNum()
    { 
        return characterNum;
    }     

    public void SelectCharacter()
    { 
        GameManager.instance.homeManager.SelectPlayerRosterButton(characterNum);
    }
}
