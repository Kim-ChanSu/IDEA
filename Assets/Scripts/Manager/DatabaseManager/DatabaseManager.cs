using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class SaveData
{
    public bool isBGMPlay; //BGM�� ��������� �ƴ��� üũ 
    public GameVariable[] Var; // ���� ����
    public GameSwitch[] Switch; // ���� ����ġ
    public int gold; // �÷��̾ ������ ��

    public CharacterStatus[] PlayerCharacter;
    public CharacterStatus[] EnemyCharacter;

    public int[] selectPlayerCharacterNum; // ������ �����ϴ� ĳ����
    public bool[] useableCharacter; // �÷��̾ ������ ĳ����

    public int playerTeamHealth;
    public int playerTeamMaxHealth;
    public int beforeMainStageSetD_day; // �� ���ν����������� ������ ���̰�
    public bool isMainStageEnd;
    public bool checkTrainingSkipBox; // Ʈ���̴׿��� �̴ϰ��� ��ŵ���θ� Ȯ���ϴ� üũ�ڽ��� üũ����
    public string playerTeamName;
}

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance; //�̱���
    private string saveFile;
    private string saveFolder;
    private int saveDataCount = 20;

    public SaveData gameSaveData = new SaveData();

    [SerializeField]
    private GameObject saveCanvas;
    [SerializeField]
    private GameObject saveWindowTopText;
    [SerializeField]
    private GameObject saveContent;
    [SerializeField]
    private GameObject saveButtonPrefab;

    void Awake() 
    {
        // �̱���
        #region
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Debug.LogWarning("���� ���ӸŴ����� 2���̻� �����մϴ�.");
            Destroy(gameObject);
        }
        #endregion
        InitDatabaseManager();
    }

    private void InitDatabaseManager()
    {
        #region
        gameSaveData = new SaveData();
        saveFolder = Application.dataPath + "/save/";
        saveFile = saveFolder + "savefile"; 

        if(Directory.Exists(saveFolder) == false)
        {
            Debug.Log("���̺� ������ �����Ƿ� ������ �����մϴ�.");
            Directory.CreateDirectory(saveFolder);
        }

        if (saveButtonPrefab != null)
        {
            for (int i = 0; i < saveDataCount; i++)
            {
                GameObject newSaveButton= Instantiate(saveButtonPrefab);
                newSaveButton.transform.SetParent(saveContent.transform);
                newSaveButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                newSaveButton.name = "SaveButton" + i;
                newSaveButton.GetComponent<SaveButtonController>().InitSaveButton(i);
            }
        }
        #endregion
    }

    public void SetSaveCanvas(bool isLoad)
    {
        for (int i = 0; i < saveContent.transform.childCount; i++)
        {
            UpdateSaveButton(i);
            SetSaveButtonActiveTrueByMode(i, isLoad);
        }

        if (isLoad == true)
        {
            saveWindowTopText.GetComponent<Text>().text = "�ҷ�����";
        }
        else
        {
            saveWindowTopText.GetComponent<Text>().text = "����";
        }

        GameManager.instance.isSaveCanvasOn = true;
        saveCanvas.SetActive(true);
    }

    public void SetSaveCanvasActiveFalse()
    {
        SetAllSaveButtonActiveFalse();
        saveCanvas.SetActive(false);
        GameManager.instance.isSaveCanvasOn = false;
    }

    public void UpdateSaveButton(int num)
    {
        #region
        if ((num < 0) || (num >= saveContent.transform.childCount))
        {
            Debug.LogWarning("UpdateSaveButton�� �߸��� ���� ���Խ��ϴ�.");
            return;
        }

        string saveFileNum = saveFile + num;

        if (File.Exists(saveFileNum) == true)
        {
            string saveButtonText = "";
            SaveData checkSaveData = new SaveData();
            string data = File.ReadAllText(saveFileNum.ToString());
            checkSaveData = JsonUtility.FromJson<SaveData>(data);
            saveButtonText = "���ν������� ���൵ : " + checkSaveData.Var[GameManager.MAINSTAGEVARNUM].Var + " / ���� �ϼ� : " + checkSaveData.Var[GameManager.DDAYVARNUM].Var + " / �� ü�� : " + checkSaveData.playerTeamHealth + " / " + NameDatabaseManager.goldName + " " + checkSaveData.gold;
            
            saveContent.transform.GetChild(num).GetComponent<SaveButtonController>().SetSaveButton(true, saveButtonText);
        }
        else
        {
            saveContent.transform.GetChild(num).GetComponent<SaveButtonController>().SetSaveButton(false);
        }
        #endregion
    }

    public void SetSaveButtonActiveTrueByMode(int num, bool isLoad)
    {
        #region
        if ((num < 0) || (num >= saveContent.transform.childCount))
        {
            Debug.LogWarning("UpdateSaveButton�� �߸��� ���� ���Խ��ϴ�.");
            return;
        }

        saveContent.transform.GetChild(num).GetComponent<SaveButtonController>().SetButtonMode(isLoad);
        #endregion
    }

    public void SetAllSaveButtonActiveFalse()
    {
        for (int i = 0; i < saveContent.transform.childCount; i++)
        {
            saveContent.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SaveData(int fileNum)
    {
        Debug.Log("���̺� üũ");
        if(Directory.Exists(saveFolder) == false)
        {
            Debug.Log("���̺� ������ �����Ƿ� ������ �����մϴ�.");
            Directory.CreateDirectory(saveFolder);
        }
        if(File.Exists(saveFile + fileNum) == true)
        { 
            Debug.Log("������ ���̺� ������ �����Ƿ� ���� �մϴ�."); 
            File.Delete(saveFile + fileNum);
        }
        Save(fileNum);
    }

    private void Save(int fileNum)
    {
        string saveFileNum = saveFile + fileNum;
        GameManagerToSaveData();
        string data = JsonUtility.ToJson(gameSaveData);
        File.WriteAllText(saveFileNum.ToString(), data);
        Debug.Log("���̺� �Ϸ�");
    }

    public bool LoadData(int fileNum)
    {
        string saveFileNum = saveFile + fileNum;

        if (File.Exists(saveFileNum) == true)
        {
            gameSaveData = new SaveData();
            string data = File.ReadAllText(saveFileNum.ToString());
            gameSaveData = JsonUtility.FromJson<SaveData>(data);
            SaveDataToGameManager();
            return true;
        }
        else
        {
            Debug.LogError("�ش� ���̺������� �������� �ʽ��ϴ�! " + fileNum);
            return false;
        }
    }

    private void GameManagerToSaveData()
    {
        #region
        gameSaveData = new SaveData();

        gameSaveData.isBGMPlay = GameManager.instance.isBGMPlay;
        gameSaveData.Var = GameManager.instance.Var;
        gameSaveData.Switch = GameManager.instance.Switch;
        gameSaveData.gold = GameManager.instance.gold;

        gameSaveData.PlayerCharacter = GameManager.instance.PlayerCharacter;
        gameSaveData.EnemyCharacter = GameManager.instance.EnemyCharacter;

        gameSaveData.selectPlayerCharacterNum = GameManager.instance.selectPlayerCharacterNum;
        gameSaveData.useableCharacter = GameManager.instance.useableCharacter;

        gameSaveData.playerTeamHealth = GameManager.instance.playerTeamHealth;
        gameSaveData.playerTeamMaxHealth = GameManager.instance.playerTeamMaxHealth;
        gameSaveData.beforeMainStageSetD_day = GameManager.instance.beforeMainStageSetD_day;
        gameSaveData.isMainStageEnd = GameManager.instance.isMainStageEnd;
        gameSaveData.checkTrainingSkipBox = GameManager.instance.checkTrainingSkipBox;
        gameSaveData.playerTeamName = GameManager.instance.playerTeamName;
        #endregion
    }

    private void SaveDataToGameManager()
    {
        #region
        GameManager.instance.isBGMPlay = gameSaveData.isBGMPlay;
        GameManager.instance.Var = gameSaveData.Var;
        GameManager.instance.Switch = gameSaveData.Switch;
        GameManager.instance.gold = gameSaveData.gold;

        GameManager.instance.PlayerCharacter = gameSaveData.PlayerCharacter;
        GameManager.instance.EnemyCharacter = gameSaveData.EnemyCharacter;

        GameManager.instance.selectPlayerCharacterNum = gameSaveData.selectPlayerCharacterNum;
        GameManager.instance.useableCharacter = gameSaveData.useableCharacter;

        GameManager.instance.playerTeamHealth = gameSaveData.playerTeamHealth;
        GameManager.instance.playerTeamMaxHealth = gameSaveData.playerTeamMaxHealth;
        GameManager.instance.beforeMainStageSetD_day = gameSaveData.beforeMainStageSetD_day;
        GameManager.instance.isMainStageEnd = gameSaveData.isMainStageEnd;
        GameManager.instance.checkTrainingSkipBox = gameSaveData.checkTrainingSkipBox;
        GameManager.instance.playerTeamName = gameSaveData.playerTeamName;
        #endregion
    }
}
