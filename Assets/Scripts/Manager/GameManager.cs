using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class GameVariable
{ 
    public string VarName;
    public int Var;
}

[System.Serializable]
public class GameSwitch
{ 
    public string SwitchName;
    public bool Switch;
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance; //�̱���

    [SerializeField]
    private AudioSource gameBGM; //BGM��
    [SerializeField]
    private AudioSource gameSE; //SE��

    private string battleSceneName = "BattleScene";
    private string titleSceneName = "TitleScene";

    private const string TESTBATTLE2SCENENAME = "TestBattle2Scene";
    private const string HOMESCENENAME = "HomeScene";
    private const string TALKSCENENAME = "TalkScene";
    private const string GAMEOVERSCENENAME = "GameOverScene";


    public const int TILESIZE = 2;
    public const int RESETINDEX = -1; // ��Ʋ�������� -1�� �ʱ�ȭ�ϴµ� ����ȵ�����..
    public const int MAINSTAGEVARNUM = 0;
    public const int DDAYVARNUM = 1;

    public const int DEFAULTPASSSKILLNUM = 1;
    public const int DEFAULTINTERRUPTBALLNUM = 2;

    [SerializeField]
    private Sprite blank;
    [SerializeField]
    private Sprite white;
    [SerializeField]
    private Sprite defaultIllustration;
    [SerializeField]
    private Sprite defaultFace;
    [SerializeField]
    private Sprite defaultTalkerImage;
    //private string readAllCSVText = "";

    // �߿��Ѱ�(�����ؾ� �ϴ� ��)
    [HideInInspector]
    public bool isBGMPlay = false; //BGM�� ��������� �ƴ��� üũ 
    public GameVariable[] Var; // ���� ����
    public GameSwitch[] Switch; // ���� ����ġ
    public int gold; // �÷��̾ ������ ��

    public CharacterStatus[] PlayerCharacter;
    public CharacterStatus[] EnemyCharacter;

    public int[] selectPlayerCharacterNum = new int[5] {-1,-1,-1,-1,-1}; // ������ �����ϴ� ĳ����
    [HideInInspector]
    public bool[] useableCharacter; // �÷��̾ ������ ĳ����

    public int playerTeamHealth = 100;
    [HideInInspector]
    public int playerTeamMaxHealth = 120;
    //[HideInInspector] 
    public int beforeMainStageSetD_day = 0; // �� ���ν����������� ������ ���̰�
    [HideInInspector] 
    public bool isMainStageEnd; // Ȩ�� �̺�Ʈ ������, ��ⳡ���� ���忹��
    [HideInInspector]
    public bool checkTrainingSkipBox = false; // Ʈ���̴׿��� �̴ϰ��� ��ŵ���θ� Ȯ���ϴ� üũ�ڽ��� üũ����
    public string playerTeamName = "���̺�";
    //----------- �������� ����-----
    public string CSVName;
    public string CSVPart;
    public string nextBattleSceneName;
    public string endBattleCSVName;
    public string endBattleCSVPart;

    public string enemyTeamName = "���Ҽ�";
    public int clearEXP;
    public int clearGold;
    public int nextD_day;
    public int stageUseHealth;
    public bool isMainStage; 
    // �� Ŭ���ϰ� �������� ���� �������� ���� ����  
    //----------------------------------
    [HideInInspector]
    public string CSVFolder; //CSV������ ����Ǵ� �⺻ ����

    [HideInInspector]
    public CameraController cameraController;

    [HideInInspector]
    public BattleManager battleManager;
    [HideInInspector]
    public TalkManager talkManager;
    [HideInInspector]
    public HomeManager homeManager;

    public int[] firstUseableCharacter; // ó������ ������ �����ϴ� ĳ����

    [HideInInspector]
    public int playerLeaderNum = 0; //�Ʊ� ����(������ ���� ����)
    [HideInInspector]
    public int enemyLeaderNum = 0; // ���� ����(������ ���� �¸�)

    [HideInInspector]
    public int HPCorrection = 5; //������ HP����
    [HideInInspector]
    public int MPCorrection = 1; //������ MP����
    [HideInInspector]
    public int maxStatusEXP = 1000;
    //[HideInInspector]
    public bool isNextDay = false; //�������� ������ ���� �� 

    [HideInInspector]
    public bool isMouseOnBattleLog = false; 
    [HideInInspector]
    public bool isSaveCanvasOn = false;

    [HideInInspector]
    public int trainSpendGold = 0; //Ʈ���̴׿��� ������ ���

    [HideInInspector]
    public bool isOpeningEnd = false;

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

        CSVFolder = "CSV/"; //CSV���� ��ġ �ʱ�ȭ
        isMainStageEnd = true;
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    // ��ݰ���
    #region
    public void ChangeBGM(AudioClip music)
    { 
        Debug.Log("�ٲٴ� ����̸��� " + music.ToString());
        gameBGM.time = 0;
        gameBGM.clip = music;
        if(isBGMPlay == true)
        {     
            gameBGM.Play();
        }
    }

    public void StartBGM()
    {
        isBGMPlay = true;
        gameBGM.Play();
    }    

    public void StopBGM()
    {
        isBGMPlay = false;
        gameBGM.Stop();
    }

    public void PlaySE(AudioClip music)
    { 
        gameSE.clip = music;
        gameSE.Play();
    } 

    public void ChangePlayBGMTime(float time)
    { 
        gameBGM.time = time;
        Debug.Log("��� ���� ������ " + time);
        StartBGM();
    } 
    #endregion
    //------------

    private void InitializePlayerCharacter()
    { 
        #region

        // Ŭ���� �迭�� �迭 ���ʵ� �ʱ�ȭ �ؾ���
        PlayerCharacter = new CharacterStatus[CharacterDatabaseManager.instance.DBLength()];
        InitializePlayerCharacterStatusClass();       
       //--------------------------------------------------

        for(int i = 0; i < CharacterDatabaseManager.instance.DBLength(); i++)
        { 
            PlayerCharacter[i] = CharacterDatabaseManager.instance.DeepCopyCharacterStatus(CharacterDatabaseManager.instance.GetPlayerCharacter(i));
        }   

        useableCharacter = new bool[PlayerCharacter.Length];
        for(int i = 0; i < firstUseableCharacter.Length; i++)
        { 
            if(firstUseableCharacter[i] < useableCharacter.Length)
            { 
                useableCharacter[firstUseableCharacter[i]] = true;
            }       
            else
            { 
                Debug.LogWarning("firstUseableCharacter ���� " + i + "��° �ε����� �߸��� ���� ���Խ��ϴ�! �߸� ���� �� = " + firstUseableCharacter[i]);    
            }
        }

        #endregion
    }

    private void InitializePlayerCharacterStatusClass()
    { 
        for (int i=0; i<PlayerCharacter.Length; i++)
        {        
            PlayerCharacter[i] = new CharacterStatus();
        }          
    }

    public Sprite GetBlank()
    { 
        return blank;
    }

    public Sprite GetWhite()
    { 
        return white;    
    }

    public Sprite GetDefaultIllustration()
    { 
        return defaultIllustration;
    }

    public Sprite GetDefaultFace()
    { 
        return defaultFace;
    }

    public Sprite GetDefaultTalkerImage()
    { 
        return defaultTalkerImage;
    }
    
    public void InitializeEnemyCharacterStatusClass()
    { 
        for (int i=0; i < EnemyCharacter.Length; i++)
        {        
            EnemyCharacter[i] = new CharacterStatus();
        }          
    }
 
    public void SetCameraPosition(GameObject target, bool ignoreMovingIfAlreadyMoving = false) //ī�޶� ������� �̵�
    {
        if(cameraController != null)
        { 
            cameraController.SetCameraPosition(target, ignoreMovingIfAlreadyMoving);
        }        
    }
    
    
    public void SetCameraMoveTarget(GameObject target) //ī�޶� ����� ����ٴϰ���
    {
        if(cameraController != null)
        { 
            cameraController.SetCameraMoveTarget(target);
        }        
    }
    
    public void ResetCameraMoveTarget()
    {
        if(cameraController != null)
        { 
            cameraController.ResetCameraMoveTarget();
        }        
    }

    public void StartGame()
    { 
        InitializePlayerCharacter();  
        CharacterLevelUp(0);
        CharacterLevelUp(0);
        CharacterLevelUp(1);
        CharacterLevelUp(1);
        CharacterLevelUp(2);
        CharacterLevelUp(2);
        CharacterLevelUp(4);
        GoTalkScene();
    }

    public void LoadGame(int saveNum)
    {
        if (DatabaseManager.instance.LoadData(saveNum) == true)
        {
            DatabaseManager.instance.SetSaveCanvasActiveFalse();
            GoHomeScene();
        }
    }

    public void ExitGame()
    { 
        Application.Quit();     
    }      

    public void ChangeScene(string sceneName)
    { 
        SceneManager.LoadScene(sceneName);
    }    

    public void GoBattleScene()
    { 
        ChangeScene(battleSceneName);
    }

    public void GoNextBattleScene()
    { 
        ChangeScene(nextBattleSceneName);
    }

    public void GoTitleScene()
    { 
        Destroy(DatabaseManager.instance.gameObject);
        Destroy(this.gameObject);
        ChangeScene(titleSceneName);
    }

    public void GoTestBattleScene()
    { 
        ChangeScene(TESTBATTLE2SCENENAME);
    }
    
    public void GoHomeScene()
    {
        ChangeScene(HOMESCENENAME);
    }

    public void GoTalkScene()
    {
        ChangeScene(TALKSCENENAME);
    }

    public void GoGameOverScene()
    { 
        ChangeScene(GAMEOVERSCENENAME);
    }

    public void ChangeUseableCharacter(int characterNum, bool mode)
    { 
        if((characterNum < useableCharacter.Length) && (0 <= characterNum))
        { 
            useableCharacter[characterNum] = mode;
        }
        else
        { 
            Debug.LogWarning("ChangePlayerCharacter �Լ��� �߸� �� ���� ��� �Խ��ϴ�. ��� �� �� = " + characterNum);
        }
    }
    //���̺� �ε� ����
    public void SetSaveCanvas()
    {
        DatabaseManager.instance.SetSaveCanvas(false);
    }

    public void SetLoadCanvas()
    {
        DatabaseManager.instance.SetSaveCanvas(true);
    }

    public void ExitSaveCanvas()
    {
        DatabaseManager.instance.SetSaveCanvasActiveFalse();
    }
    //--
    //�������� ����
 
    public bool CheckMainStage()
    { 
        if(0 > Var[MAINSTAGEVARNUM].Var)
        { 
            Debug.LogWarning("���� �������� �������� �̻��Ͽ� 0���� �ʱ�ȭ �մϴ�!");
            Var[MAINSTAGEVARNUM].Var = 0;
        }

        if(Var[MAINSTAGEVARNUM].Var < StageDatabaseManager.instance.GetStageLength())
        { 
            SetMainStage(Var[MAINSTAGEVARNUM].Var);
            return true;
        }
        else
        { 
            Debug.Log("���� ���������� ���� Ŭ���� �Ͽ����ϴ�");
            return false;
        }
        
    }

    private void SetMainStage(int num) //���νó����� ����
    { 
        if((num < StageDatabaseManager.instance.GetStageLength()) && (0 <= num))
        { 
            Stage nextStage = StageDatabaseManager.instance.GetStage(num);
            SetStage(nextStage);
            isMainStage = true;
        }
        else
        { 
            Debug.LogWarning("�ش��ϴ� ���������� �����ϴ�!");    
        }
    }

    public void SetSubStage(int num)
    {
        if((num < StageDatabaseManager.instance.GetSubStageLength()) && (0 <= num))
        { 
            Stage nextStage = StageDatabaseManager.instance.GetSubStage(num);
            SetStage(nextStage);
            isMainStage = false;
        }
        else
        { 
            Debug.LogWarning("�ش��ϴ� ���꽺�������� �����ϴ�!");    
        }
    }

    private void SetStage(Stage nextStage)
    {
        CSVName = nextStage.stageCSVName;
        CSVPart = nextStage.stageCSVPart;
        nextBattleSceneName = nextStage.stageBattleSceneName;
        endBattleCSVName = nextStage.stageEndCSVName;
        endBattleCSVPart = nextStage.stageEndCSPart;
        clearEXP = nextStage.clearEXP;
        clearGold = nextStage.clearGold;
        nextD_day = nextStage.D_day;
        stageUseHealth = nextStage.stageUseHealth;
        enemyTeamName = nextStage.enemyTeamName;

        SetSelectPlayerCharacterNum(nextStage.playerCharacterCount);

        EnemyCharacter = new CharacterStatus[nextStage.enemyCharacterNum.Length];
        InitializeEnemyCharacterStatusClass();
        for(int i = 0; i < EnemyCharacter.Length; i++)
        { 
            EnemyCharacter[i] = CharacterDatabaseManager.instance.DeepCopyCharacterStatus(EnemyCharacterDatabaseManager.instance.GetEnemyCharacter(nextStage.enemyCharacterNum[i]).enemyStatus);
        }
    }

    public void SetSelectPlayerCharacterNum(int num) // ĳ���� �����
    { 
        int[] changeInt = new int[num];
        for(int i = 0; i < changeInt.Length; i++)
        { 
            if(i < selectPlayerCharacterNum.Length)
            { 
                changeInt[i] = selectPlayerCharacterNum[i];
            }
            else
            {
               changeInt[i] = RESETINDEX;
            }
        }    

        selectPlayerCharacterNum = changeInt;
    }
    //---

    public void ResetSelectPlayerCharacterNum()
    { 
        for(int i = 0; i < selectPlayerCharacterNum.Length; i++)
        { 
            selectPlayerCharacterNum[i] = RESETINDEX;
        }
    }

    public void HealAllPlayerCharacter()
    { 
        for(int i = 0; i < PlayerCharacter.Length; i++)
        { 
            PlayerCharacter[i].HP = PlayerCharacter[i].maxHP;
            PlayerCharacter[i].MP = PlayerCharacter[i].maxMP;
            PlayerCharacter[i].Sympathy = 0;
        }     
    }

    public string GetSympathyTypeName(SympathyType sympathyType)
    { 
        #region
        switch(sympathyType)
        { 
            case SympathyType.None:
                return NameDatabaseManager.SympathyTypeNoneName; 
            case SympathyType.Rational:
                return NameDatabaseManager.SympathyTypeRationalName;
            case SympathyType.Anger:
                return NameDatabaseManager.SympathyTypeAngerName;
            case SympathyType.Enjoy:
                return NameDatabaseManager.SympathyTypeEnjoyName;
            default:
                return NameDatabaseManager.SympathyTypeNoneName;               
        }
        #endregion
    }

    public void CheckSelectPlayerCharacterNum()
    { 
        #region
        for(int i = 0; i < selectPlayerCharacterNum.Length; i++)
        { 
            if((selectPlayerCharacterNum[i] < useableCharacter.Length) && (0 < selectPlayerCharacterNum[i]))
            { 
                if(useableCharacter[selectPlayerCharacterNum[i]] == false)
                { 
                    selectPlayerCharacterNum[i] = RESETINDEX;
                }
            }
            else if(selectPlayerCharacterNum[i] >= useableCharacter.Length)
            { 
                Debug.LogWarning("selectPlayerCharacterNum ���� " + i + "��° �ε����� �߸��� ���� ����ֽ��ϴ�! �߸� ����ִ� �� = " + selectPlayerCharacterNum[i]);      
            }
        }        

        for(int i = 0; i < selectPlayerCharacterNum.Length; i++)
        { 
            if(selectPlayerCharacterNum[i] == RESETINDEX)
            { 
                SortCheckSelectPlayerCharacterNum(i);
                break;
            }
        }
        #endregion
    }

    public void SortCheckSelectPlayerCharacterNum(int selectPlayerCharacterNumIndex)
    { 
        #region
        if(selectPlayerCharacterNumIndex < selectPlayerCharacterNum.Length)
        { 
            if(selectPlayerCharacterNum[selectPlayerCharacterNumIndex] == RESETINDEX)
            { 
                int num = selectPlayerCharacterNumIndex;
                for(int i = num + 1; i < selectPlayerCharacterNum.Length; i++)
                { 
                    if(selectPlayerCharacterNum[i] != RESETINDEX)
                    { 
                        selectPlayerCharacterNum[num] = selectPlayerCharacterNum[i];
                        selectPlayerCharacterNum[i] = RESETINDEX;
                        Debug.Log(num + "���� " + i + "�� ��ü �Ϸ�!");
                        num++;                         
                    }     
                }
            }
        }
        else
        { 
            Debug.LogWarning("SortCheckSelectPlayerCharacterNum �Լ��� �߸��� ���� ��� �Խ��ϴ�! �߸� ����ִ� �� = " + selectPlayerCharacterNumIndex);
        }
        #endregion
    }


    //������ ����
    #region
    public void CharacterEXPUp(int characterNum,int exp)
    { 
        if((characterNum < PlayerCharacter.Length) && (characterNum >= 0))
        { 
            PlayerCharacter[characterNum].EXP += exp;
            Debug.Log(PlayerCharacter[characterNum].name + "�� ����ġ�� " + exp + "��ŭ �����߽��ϴ�");
            CheckCharacterLevelUp(characterNum);
        }
        else
        { 
            Debug.LogWarning("CharacterEXPUp�Լ��� �߸� �� ���� ��� �Խ��ϴ�");
        }
    }

    public void CheckCharacterLevelUp(int characterNum)
    { 
        if((characterNum < PlayerCharacter.Length) && (characterNum >= 0))
        { 
            if(PlayerCharacter[characterNum].EXP >=  PlayerCharacter[characterNum].maxEXP)
            { 
                PlayerCharacter[characterNum].EXP -= PlayerCharacter[characterNum].maxEXP;
                CharacterLevelUp(characterNum);
            }
        }
        else
        { 
            Debug.LogWarning("CheckCharacterLevelUp�Լ��� �߸� �� ���� ��� �Խ��ϴ�");
        }
    }

    public void CharacterLevelUp(int characterNum)
    { 
        #region
        if((characterNum < PlayerCharacter.Length) && (characterNum >= 0))
        { 
            Debug.Log(characterNum + "�� ĳ���� " + PlayerCharacter[characterNum].inGameName + "��(��) ������ �߽��ϴ�!");
            PlayerCharacter[characterNum].level++;
            //PlayerCharacter[characterNum].EXP = 0;
            MaxEXPUp(characterNum);
            Debug.Log(characterNum + "�� ĳ���� " + PlayerCharacter[characterNum].inGameName + "�� ������ " + PlayerCharacter[characterNum].level);

            int SMAXValue = 13;
            int SMINValue = 8;

            int AMAXValue = 11;
            int AMINValue = 6;

            int BMAXValue = 9;
            int BMINValue = 5;

            int CMAXValue = 8;
            int CMINValue = 3;

            int DMAXValue = 6;
            int DMINValue = 2;

            int EMAXValue = 4; 
            int EMINValue = 1;

            int FMAXValue = 3;
            int FMINValue = 0;

            //HP
            int maxHPUp = 0;
            switch(PlayerCharacter[characterNum].HPValue)
            { 
                case StatusValue.S:
                maxHPUp = Random.Range(SMAXValue, SMINValue) * HPCorrection;
                break;

                case StatusValue.A:
                maxHPUp = Random.Range(AMAXValue, AMINValue) * HPCorrection;
                break;

                case StatusValue.B:
                maxHPUp = Random.Range(BMAXValue, BMINValue) * HPCorrection;
                break;

                case StatusValue.C:
                maxHPUp = Random.Range(CMAXValue, CMINValue) * HPCorrection;
                break;

                case StatusValue.D:
                maxHPUp = Random.Range(DMAXValue, DMINValue) * HPCorrection;
                break;

                case StatusValue.E:
                maxHPUp = Random.Range(EMAXValue, EMINValue) * HPCorrection;
                break;

                case StatusValue.F:
                maxHPUp = Random.Range(FMAXValue, FMINValue) * HPCorrection;
                break;

                default:
                Debug.LogWarning("HPValue�� �̻��մϴ�. ���� HPValue�� �� = " + PlayerCharacter[characterNum].HPValue);
                break; 
            }

            PlayerCharacter[characterNum].maxHP += maxHPUp;
            PlayerCharacter[characterNum].HP += maxHPUp;
            Debug.Log(characterNum + "�� ĳ���� " + PlayerCharacter[characterNum].inGameName + "�� maxHP�� " + maxHPUp + "��ŭ �����Ͽ����ϴ�!");

            //MP
            int maxMPUp = 0;
            switch(PlayerCharacter[characterNum].MPValue)
            { 
                case StatusValue.S:
                maxMPUp = Random.Range(SMAXValue, SMINValue) * MPCorrection;
                break;

                case StatusValue.A:
                maxMPUp = Random.Range(AMAXValue, AMINValue) * MPCorrection;
                break;

                case StatusValue.B:
                maxMPUp = Random.Range(BMAXValue, BMINValue) * MPCorrection;
                break;

                case StatusValue.C:
                maxMPUp = Random.Range(CMAXValue, CMINValue) * MPCorrection;
                break;

                case StatusValue.D:
                maxMPUp = Random.Range(DMAXValue, DMINValue) * MPCorrection;
                break;

                case StatusValue.E:
                maxMPUp = Random.Range(EMAXValue, EMINValue) * MPCorrection;
                break;

                case StatusValue.F:
                maxMPUp = Random.Range(FMAXValue, FMINValue) * MPCorrection;
                break;

                default:
                Debug.LogWarning("MPValue�� �̻��մϴ�. ���� MPValue�� �� = " + PlayerCharacter[characterNum].MPValue);
                break; 
            }

            PlayerCharacter[characterNum].maxMP += maxMPUp;
            PlayerCharacter[characterNum].MP += maxMPUp;
            Debug.Log(characterNum + "�� ĳ���� " + PlayerCharacter[characterNum].inGameName + "�� maxMP�� " + maxMPUp + "��ŭ �����Ͽ����ϴ�!");

            //ATK
            int ATKUp = 0;
            switch(PlayerCharacter[characterNum].ATKValue)
            { 
                case StatusValue.S:
                ATKUp = Random.Range(SMAXValue, SMINValue);
                break;

                case StatusValue.A:
                ATKUp = Random.Range(AMAXValue, AMINValue);
                break;

                case StatusValue.B:
                ATKUp = Random.Range(BMAXValue, BMINValue);
                break;

                case StatusValue.C:
                ATKUp = Random.Range(CMAXValue, CMINValue);
                break;

                case StatusValue.D:
                ATKUp = Random.Range(DMAXValue, DMINValue);
                break;

                case StatusValue.E:
                ATKUp = Random.Range(EMAXValue, EMINValue);
                break;

                case StatusValue.F:
                ATKUp = Random.Range(FMAXValue, FMINValue);
                break;

                default:
                Debug.LogWarning("ATKValue�� �̻��մϴ�. ���� ATKValue�� �� = " + PlayerCharacter[characterNum].ATKValue);
                break; 
            }

            PlayerCharacter[characterNum].ATK += ATKUp;
            Debug.Log(characterNum + "�� ĳ���� " + PlayerCharacter[characterNum].inGameName + "�� ATK�� " + ATKUp + "��ŭ �����Ͽ����ϴ�!");

            //MAK
            int MAKUp = 0;
            switch(PlayerCharacter[characterNum].MAKValue)
            { 
                case StatusValue.S:
                MAKUp = Random.Range(SMAXValue, SMINValue);
                break;

                case StatusValue.A:
                MAKUp = Random.Range(AMAXValue, AMINValue);
                break;

                case StatusValue.B:
                MAKUp = Random.Range(BMAXValue, BMINValue);
                break;

                case StatusValue.C:
                MAKUp = Random.Range(CMAXValue, CMINValue);
                break;

                case StatusValue.D:
                MAKUp = Random.Range(DMAXValue, DMINValue);
                break;

                case StatusValue.E:
                MAKUp = Random.Range(EMAXValue, EMINValue);
                break;

                case StatusValue.F:
                MAKUp = Random.Range(FMAXValue, FMINValue);
                break;

                default:
                Debug.LogWarning("MAKValue�� �̻��մϴ�. ���� MAKValue�� �� = " + PlayerCharacter[characterNum].MAKValue);
                break; 
            }

            PlayerCharacter[characterNum].MAK += MAKUp;
            Debug.Log(characterNum + "�� ĳ���� " + PlayerCharacter[characterNum].inGameName + "�� MAK�� " + MAKUp + "��ŭ �����Ͽ����ϴ�!");

            //DEF
            int DEFUp = 0;
            switch(PlayerCharacter[characterNum].DEFValue)
            { 
                case StatusValue.S:
                DEFUp = Random.Range(SMAXValue, SMINValue);
                break;

                case StatusValue.A:
                DEFUp = Random.Range(AMAXValue, AMINValue);
                break;

                case StatusValue.B:
                DEFUp = Random.Range(BMAXValue, BMINValue);
                break;

                case StatusValue.C:
                DEFUp = Random.Range(CMAXValue, CMINValue);
                break;

                case StatusValue.D:
                DEFUp = Random.Range(DMAXValue, DMINValue);
                break;

                case StatusValue.E:
                DEFUp = Random.Range(EMAXValue, EMINValue);
                break;

                case StatusValue.F:
                DEFUp = Random.Range(FMAXValue, FMINValue);
                break;

                default:
                Debug.LogWarning("DEFValue�� �̻��մϴ�. ���� DEFValue�� �� = " + PlayerCharacter[characterNum].DEFValue);
                break; 
            }

            PlayerCharacter[characterNum].DEF += DEFUp;
            Debug.Log(characterNum + "�� ĳ���� " + PlayerCharacter[characterNum].inGameName + "�� DEF�� " + DEFUp + "��ŭ �����Ͽ����ϴ�!");

            //MDF
            int MDFUp = 0;
            switch(PlayerCharacter[characterNum].MDFValue)
            { 
                case StatusValue.S:
                MDFUp = Random.Range(SMAXValue, SMINValue);
                break;

                case StatusValue.A:
                MDFUp = Random.Range(AMAXValue, AMINValue);
                break;

                case StatusValue.B:
                MDFUp = Random.Range(BMAXValue, BMINValue);
                break;

                case StatusValue.C:
                MDFUp = Random.Range(CMAXValue, CMINValue);
                break;

                case StatusValue.D:
                MDFUp = Random.Range(DMAXValue, DMINValue);
                break;

                case StatusValue.E:
                MDFUp = Random.Range(EMAXValue, EMINValue);
                break;

                case StatusValue.F:
                MDFUp = Random.Range(FMAXValue, FMINValue);
                break;

                default:
                Debug.LogWarning("MDFValue�� �̻��մϴ�. ���� MDFValue�� �� = " + PlayerCharacter[characterNum].MDFValue);
                break; 
            }

            PlayerCharacter[characterNum].MDF += MDFUp;
            Debug.Log(characterNum + "�� ĳ���� " + PlayerCharacter[characterNum].inGameName + "�� MDF�� " + MDFUp + "��ŭ �����Ͽ����ϴ�!");

            LevelUpLearnSKillCheck(characterNum);
            CheckCharacterLevelUp(characterNum);
        }
        else
        { 
            Debug.LogWarning("CharacterLevelUp�Լ��� �߸� �� ���� ��� �Խ��ϴ�");
        }
        #endregion
    }

    private void MaxEXPUp(int characterNum)
    { 
        int value = 3;
        PlayerCharacter[characterNum].maxEXP += (PlayerCharacter[characterNum].level + value) * (PlayerCharacter[characterNum].level + value) + (value + 4) * (PlayerCharacter[characterNum].level - 1);
    }

    private void LevelUpLearnSKillCheck(int characterNum)
    {
        if ((0 <= characterNum) && (characterNum < PlayerCharacter.Length))
        {
            LearnSkill[] learnSkill = SympathyDatabaseManager.instance.GetSympathyDataByCharacterSympathyType(PlayerCharacter[characterNum].sympathyType).learnSkill;
            for(int i = 0; i < learnSkill.Length; i++)
            { 
                if(PlayerCharacter[characterNum].level == learnSkill[i].level)
                { 
                    SetPlayerCharacterSkill(characterNum, learnSkill[i].skillNum, true);
                }
            }        
        }
        else
        { 
            Debug.LogWarning("LevelUpLearnSKillCheck �߸� �� ���� ���Խ��ϴ�. characterNum = " + characterNum);
        }
    }

    public void SetPlayerCharacterSkill(int characterNum, int skillNum, bool isLearn)
    { 
        if((0 <= characterNum) && (characterNum < PlayerCharacter.Length) && (0 <= skillNum) && (skillNum < SkillDatabaseManager.instance.SkillLength()))
        { 
            PlayerCharacter[characterNum].characterSkill[skillNum] = isLearn;
            if(isLearn == true)
            { 
                Debug.Log(PlayerCharacter[characterNum].inGameName + "��(��) " + SkillDatabaseManager.instance.GetSkill(skillNum).ingameSkillName + "�� ������ϴ�!");    
            }
            else
            { 
                Debug.Log(PlayerCharacter[characterNum].inGameName + "��(��) " + SkillDatabaseManager.instance.GetSkill(skillNum).ingameSkillName + "�� �ؾ����ϴ�!");
            }
        }
        else
        { 
            Debug.LogWarning("SetPlayerCharacterSkill�� �߸� �� ���� ���Խ��ϴ�. characterNum = " + characterNum + ", skillNum = " + skillNum);
        }
    }
    #endregion
    //----------
    //������ ����
    public void NextDay()
    {
        Var[DDAYVARNUM].Var = Var[DDAYVARNUM].Var - 1;
        isNextDay = true;
        GoHomeScene();
    }

    public int GetD_day()
    {
        return Var[DDAYVARNUM].Var;
    }


    public bool isTimeToMainScenario()
    {
        if (Var[DDAYVARNUM].Var > 0)
        {
            return false; 
        }
        else
        {
            return true;
        }
    }

    public void IncreasePlayerTeamHP(int value)
    {
        playerTeamHealth = playerTeamHealth + value;
        if (playerTeamHealth < 0)
        {
            playerTeamHealth = 0;
        }
        else if (playerTeamHealth > playerTeamMaxHealth)
        {
            playerTeamHealth = playerTeamMaxHealth;
        }
    }

    public void SetBattleHPbyTeamHealth()
    {
        for(int i = 0; i < PlayerCharacter.Length; i++)
        {
            PlayerCharacter[i].HP = (int)((float)PlayerCharacter[i].maxHP * ((float)playerTeamHealth / 100.0f));

            if (PlayerCharacter[i].HP <= 0)
            {
                PlayerCharacter[i].HP = 1;
            }
        }
    }

    public bool CheckPlayerTeamHP(int value)
    {
        if (playerTeamHealth >= value)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckGold(int value)
    {
        if ((gold >= value) && (value >= 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //--
    //�ɷ�ġ ����
    public void IncreaseStatusEXP(int characterNum, IncreasableStatus increaseStatus, int exp)
    {
        if ((0 <= characterNum) && (characterNum < PlayerCharacter.Length))
        {
            switch (increaseStatus)
            {
                case IncreasableStatus.maxHP:
                    PlayerCharacter[characterNum].maxHPEXP += exp;
                    break;
                case IncreasableStatus.maxMP:
                    PlayerCharacter[characterNum].maxMPEXP += exp;
                    break;
                case IncreasableStatus.ATK:
                    PlayerCharacter[characterNum].ATKEXP += exp;
                    break;
                case IncreasableStatus.MAK:
                    PlayerCharacter[characterNum].MAKEXP += exp;
                    break;
                case IncreasableStatus.DEF:
                    PlayerCharacter[characterNum].DEFEXP += exp;
                    break;
                case IncreasableStatus.MDF:
                    PlayerCharacter[characterNum].MDFEXP += exp;
                    break;
                default:
                    break;
            }

            CheckStatusUP(characterNum);
        }
        else
        {
            Debug.LogWarning("IncreaseStatusEXP�� �߸��� ���� ���Խ��ϴ�! ���� ���� " + characterNum);
        }
    }


    private void CheckStatusUP(int characterNum)
    {
        while (PlayerCharacter[characterNum].maxHPEXP >= maxStatusEXP)
        {
            PlayerCharacter[characterNum].maxHPEXP -= maxStatusEXP;
            PlayerCharacter[characterNum].maxHP += 1 * HPCorrection;
            PlayerCharacter[characterNum].increaseMaxHPByTrain += 1 * HPCorrection;
        }

        while (PlayerCharacter[characterNum].maxMPEXP >= maxStatusEXP)
        {
            PlayerCharacter[characterNum].maxMPEXP -= maxStatusEXP;
            PlayerCharacter[characterNum].maxMP += 1 * MPCorrection;
            PlayerCharacter[characterNum].increaseMaxMPByTrain += 1 * MPCorrection;
        }

        while (PlayerCharacter[characterNum].ATKEXP >= maxStatusEXP)
        {
            PlayerCharacter[characterNum].ATKEXP -= maxStatusEXP;
            PlayerCharacter[characterNum].ATK += 1;
            PlayerCharacter[characterNum].increaseATKByTrain++;
        }

        while (PlayerCharacter[characterNum].MAKEXP >= maxStatusEXP)
        {
            PlayerCharacter[characterNum].MAKEXP -= maxStatusEXP;
            PlayerCharacter[characterNum].MAK += 1;
            PlayerCharacter[characterNum].increaseMAKByTrain++;
        }

        while (PlayerCharacter[characterNum].DEFEXP >= maxStatusEXP)
        {
            PlayerCharacter[characterNum].DEFEXP -= maxStatusEXP;
            PlayerCharacter[characterNum].DEF += 1;
            PlayerCharacter[characterNum].increaseDEFByTrain++;
        }

        while (PlayerCharacter[characterNum].MDFEXP >= maxStatusEXP)
        {
            PlayerCharacter[characterNum].MDFEXP -= maxStatusEXP;
            PlayerCharacter[characterNum].MDF += 1;
            PlayerCharacter[characterNum].increaseMDFByTrain++;
        }
    }

    //--
    public bool CantMoveTagCheck(string tag)
    { 
        if(tag == "Character" || tag == "Object")
        { 
            return true;
        }
        else
        { 
            return false;   
        }
    }

    public bool InterruptBallMoveCheck(string tag)
    {
        if(tag == "Character"|| tag == "Object")
        { 
            return true;
        }
        else
        { 
            return false;   
        }
    }

    public bool CantCameraZoomCheck()
    {
        if (isMouseOnBattleLog == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool KeyCheckAccept()
    { 
        if(Input.GetKeyUp(KeyCode.Return) == true || Input.GetKeyUp(KeyCode.Mouse0) == true || Input.GetKeyUp(KeyCode.Space) == true )
        { 
            return true;    
        }    
        else
        { 
            return false;    
        }
    }

    public bool KeyCheckSkip()
    { 
        if(Input.GetKey(KeyCode.LeftControl) == true)
        { 
            return true;    
        }    
        else
        { 
            return false;    
        }
    }

    public bool KeyCheckTurnSkip()
    { 
        if(Input.GetKey(KeyCode.BackQuote) == true)
        { 
            return true;    
        }    
        else
        { 
            return false;    
        }
    }

    public bool KeyCheckEscape()
    { 
        if(Input.GetKeyUp(KeyCode.Escape) == true || Input.GetKeyUp(KeyCode.Mouse1) == true)
        { 
            return true;    
        }    
        else
        { 
            return false;    
        }
    }

    public bool KeyCheckMenu()
    { 
        if(Input.GetKeyUp(KeyCode.Escape) == true)
        { 
            return true;    
        }    
        else
        { 
            return false;    
        }                
    }

    public bool KeyCheckUp()
    { 
        if(Input.GetKey(KeyCode.W) == true)
        { 
            return true;    
        }    
        else
        { 
            return false;    
        }
    }

    public bool KeyCheckDown()
    { 
        if(Input.GetKey(KeyCode.S) == true)
        { 
            return true;    
        }    
        else
        { 
            return false;    
        }
    }

    public bool KeyCheckLeft()
    { 
        if(Input.GetKey(KeyCode.A) == true)
        { 
            return true;    
        }    
        else
        { 
            return false;    
        }
    }

    public bool KeyCheckRight()
    { 
        if(Input.GetKey(KeyCode.D) == true)
        { 
            return true;    
        }    
        else
        { 
            return false;    
        }
    }
}
