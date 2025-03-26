using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveButtonController : MonoBehaviour
{
    [SerializeField]
    private GameObject saveNumText;
    [SerializeField]
    private GameObject saveText;
    
    private int saveNum;
    private string saveNumHaveNotFileText = "--- NoData ---";
    private int saveNumHaveNotFileFontSize = 64;
    private int saveFileFontSize = 42;

    private bool isLoad = false;
    private bool isHaveData = false;
    
    public void InitSaveButton(int num)
    {
        saveNum = num;
        saveNumText.GetComponent<Text>().text = "Save" + saveNum;
        this.gameObject.SetActive(false);
    }

    public void SetSaveButton(bool newIsHaveData, string saveTextContnent = "")
    {
        isHaveData = newIsHaveData;
        if (isHaveData == false)
        {
            saveText.GetComponent<Text>().fontSize = saveNumHaveNotFileFontSize;
            saveText.GetComponent<Text>().text = saveNumHaveNotFileText;
        }
        else
        {
            saveText.GetComponent<Text>().fontSize = saveFileFontSize;
            saveText.GetComponent<Text>().text = saveTextContnent;
        }
    }

    public void SetButtonMode(bool newIsLoad)
    {
        isLoad = newIsLoad;
        if ((newIsLoad == true) && (isHaveData == false))
        {
            this.gameObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            this.gameObject.GetComponent<Button>().interactable = true;
        }
        this.gameObject.SetActive(true);
    }

    public void ButtonEvent()
    {
        if (isLoad == true)
        {
            GameManager.instance.LoadGame(saveNum);
        }
        else
        {
            DatabaseManager.instance.SaveData(saveNum);
            DatabaseManager.instance.UpdateSaveButton(saveNum);
        }
    }
}
