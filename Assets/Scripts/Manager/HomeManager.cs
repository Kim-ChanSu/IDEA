using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;    

public class HomeManager : MonoBehaviour
{
    private HomeEventManager homeEventManager;

    [SerializeField]
    private GameObject playerRosterCanvas;
    [SerializeField]
    private GameObject playerRosterCharacterListContent;
    [SerializeField]
    private GameObject characterListButtonPrefab;
    [SerializeField]
    private GameObject playerRosterStartingLineupContent;
    [SerializeField]
    private GameObject startingLineupButtonPrefab;
    [SerializeField]
    private GameObject playerRosterNextStageButton;
    [SerializeField]
    private GameObject playerRosterSelectCharacterText;
    [SerializeField]
    private GameObject homeButtonCanvas;
    [SerializeField]
    private GameObject talkCanvas;
    [SerializeField]
    private GameObject stageInformationCanvas;
    [SerializeField]
    private GameObject stageInformationCanvasLeft;
    [SerializeField]
    private GameObject stageInformationCanvasRight;
    [SerializeField]
    private GameObject selectStageWindow;
    [SerializeField]
    private GameObject selectStageContent;
    [SerializeField]
    private GameObject selectSubStageButtonPrefab;
    private GameObject[] stageInformationButtons;
    private Color stageInformationButtonNormalColor = new Color(255f/255f, 255f/255f, 255f/255f, 0f/255f);
    private Color stageInformationButtonSelectColor = new Color(241f/255f, 241f/255f, 241f/255f, 150f/255f);
    
    public TrainManager trainManager;
    public bool isTrainingUIOn = false;

    //상태창용
    [SerializeField]
    private GameObject statusCanvas;
    [SerializeField]
    private GameObject spaceRight;
    [SerializeField]
    private GameObject spaceRightBack;
    [SerializeField]
    private GameObject spaceLeftR;
    [SerializeField]
    private GameObject spaceLeftL;
    [SerializeField]
    private GameObject characterStatusButtonScrollviewContent;
    [SerializeField]
    private GameObject characterStatusButtonPrefab;
    [SerializeField]
    private GameObject characterStatusSkillContent;
    [SerializeField]
    private GameObject characterStatusSkillButtonPrefab;
    [SerializeField]
    private GameObject darkEffectImage;

    private bool isCharacterStatusBack;
    //---
    //스케줄 용
    [SerializeField]
    private GameObject D_dayText;
    [SerializeField]
    private GameObject goldText;
    [SerializeField]
    private GameObject teamHPText;
    [SerializeField]
    private GameObject teamHPImage;

    //--
    // 휴식용
    [SerializeField]
    private GameObject restCanvas;
    private bool isRestCanvasOn = false;
    //--
    [SerializeField]
    private GameObject homeMenuCanvas;

    [HideInInspector]
    public bool isStageInformationOn;
    [HideInInspector]
    public bool isPlayerRosterSetting;
    [HideInInspector]
    public bool isStatusUIOn;
    [HideInInspector]
    public bool isTalkEventOn;
    [HideInInspector]
    public bool isMenu;
    [HideInInspector]
    public bool isEffect = false;
    [HideInInspector]
    public int setPlayerNum = 0;
    [HideInInspector]
    public string D_dayTextName = "D-Day";

    void Start()
    {
        InitializeHomeManager();
    }

    void Update()
    { 
        KeyCheckEscape();
    }

    private void InitializeHomeManager()
    { 
        GameManager.instance.homeManager = this;
        //GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName("Home"));
        homeEventManager = this.gameObject.GetComponent<HomeEventManager>();
        UpdateTeamImform();
        stageInformationButtons = new GameObject[stageInformationCanvasLeft.transform.GetChild(0).childCount];
        for (int i = 0; i < stageInformationButtons.Length; i++)
        {
            stageInformationButtons[i] = stageInformationCanvasLeft.transform.GetChild(0).GetChild(i).gameObject;
        }

        homeEventManager.HomeEventCheck(); //이거 꼭 마지막에 오도록하기
    }

    public void SetAllHomeButtons(bool mode)
    { 
        UpdateTeamImform();
        homeButtonCanvas.SetActive(mode);
        if(mode == true)
        { 
            darkEffectImage.SetActive(false);
        }
        else
        { 
            darkEffectImage.SetActive(true);
        }
    }

    private void KeyCheckEscape()
    { 
        if((GameManager.instance.KeyCheckEscape() == true) && (isEffect == false))
        { 
            if(isStageInformationOn == true)
            { 
                SetStageInformationCanvas(false);
            }
            else if(isPlayerRosterSetting == true)
            { 
                SetPlayerRosterCanvas(false);
            }
            else if(isStatusUIOn == true)
            { 
                SetPlayerStatusCanvas(false);
            }
            else if(isRestCanvasOn == true)
            { 
                SetRestCanvas(false);
            }
            else if(isTrainingUIOn == true)
            { 
                trainManager.EndTrainUI();
            }
            else
            {
                MenuKeyCheck();
            }

        } 
    }

    public void UpdateTeamImform()
    {
        if (GameManager.instance.GetD_day() > 0)
        {
            D_dayText.GetComponent<Text>().text = "D-" + GameManager.instance.GetD_day(); 
        }
        else
        {
            D_dayText.GetComponent<Text>().text = D_dayTextName;
        }
        
        goldText.GetComponent<Text>().text = "G "+ GameManager.instance.gold;
        teamHPText.GetComponent<Text>().text = GameManager.instance.playerTeamHealth + " / "+ GameManager.instance.playerTeamMaxHealth;
        teamHPImage.GetComponent<Image>().fillAmount = (float)GameManager.instance.playerTeamHealth / (float)GameManager.instance.playerTeamMaxHealth;

        trainManager.SettingCostInfomation();
    }


    //매뉴용
    #region
    private bool CanOpenMenuCheck()
    { 
        if((isStageInformationOn == false) && (isPlayerRosterSetting == false) && (isStatusUIOn == false) && (isTalkEventOn == false))
        { 
            return true;
        }
        else
        { 
            return false;
        }
    }

    private void MenuKeyCheck()
    { 
        if((CanOpenMenuCheck() == true) && (GameManager.instance.KeyCheckMenu() == true))
        { 
            MenuCheck();
        }
    }

    private void MenuCheck()
    { 
        if(isMenu == true)
        { 
            SetMenu(false);
        }
        else
        { 
            SetMenu(true);
        }
    }

    public void SetMenu(bool mode)
    { 
        homeMenuCanvas.SetActive(mode);
        isMenu = mode;
        if(mode == true)
        {            
            SetAllHomeButtons(false);            
        }
        else
        { 
            GameManager.instance.ExitSaveCanvas();
            SetAllHomeButtons(true);            
        }
    }

    public void SetSaveWindow()
    {
        GameManager.instance.SetSaveCanvas();
    }
    #endregion
    //---

    //-----출전 명단용
    #region
    public void SetPlayerRosterCanvas(bool mode)
    {         
        isPlayerRosterSetting = mode;
        playerRosterCanvas.SetActive(mode);
        if(mode == true)
        { 
            SetStageInformationCanvas(false);
            SetAllHomeButtons(false);
            GameManager.instance.CheckSelectPlayerCharacterNum();
            CreatePlayerRosterButtons();
            SetPlayerRoster();
        }    
        else
        { 
            SetAllHomeButtons(true);
            BreakPlayerRosterButtons();
        }      
    }

    private void CreatePlayerRosterButtons()
    { 
        for(int i = 0; i < GameManager.instance.useableCharacter.Length; i++)
        { 
            if(GameManager.instance.useableCharacter[i] == true)
            { 
                if(characterListButtonPrefab != null)
                { 
                    GameObject characterListButton = Instantiate(characterListButtonPrefab);
                    characterListButton.name = "characterListButton_" + i;
                    characterListButton.GetComponent<CharacterListButton>().InitializeCharactListButton(i);

                    characterListButton.transform.SetParent(playerRosterCharacterListContent.transform); 
                    characterListButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }

                if(startingLineupButtonPrefab != null)
                { 
                    GameObject startingLineupButton = Instantiate(startingLineupButtonPrefab);
                    startingLineupButton.name = "StartingLineupButton" + i;
                    startingLineupButton.GetComponent<StartingLineupButton>().InitializeStartingLineupButton(i);

                    startingLineupButton.transform.SetParent(playerRosterStartingLineupContent.transform); 
                    startingLineupButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
            }    
        }
    }

    private void BreakPlayerRosterButtons()
    { 
        for(int i = 0; i < playerRosterCharacterListContent.transform.childCount; i++)
        { 
             Destroy(playerRosterCharacterListContent.transform.GetChild(i).gameObject);
        }

        for(int i = 0; i < playerRosterStartingLineupContent.transform.childCount; i++)
        { 
             Destroy(playerRosterStartingLineupContent.transform.GetChild(i).gameObject);
        }
    }

    private void SetPlayerRoster()
    {
        #region
        setPlayerNum = 0;
        for(int i = 0; i < GameManager.instance.selectPlayerCharacterNum.Length; i++)
        { 
            if(GameManager.instance.selectPlayerCharacterNum[i] != GameManager.RESETINDEX)
            { 
                for(int j = 0; j < playerRosterCharacterListContent.transform.childCount; j++)
                { 
                    if((playerRosterCharacterListContent.transform.GetChild(j).GetComponent<CharacterListButton>().GetCharacterNum() == GameManager.instance.selectPlayerCharacterNum[i]) && (playerRosterCharacterListContent.transform.GetChild(j).GetComponent<CharacterListButton>() == true))
                    { 
                        for(int k = 0; k < playerRosterStartingLineupContent.transform.childCount; k++)
                        { 
                            if((playerRosterStartingLineupContent.transform.GetChild(k).GetComponent<StartingLineupButton>().GetCharacterNum() == GameManager.instance.selectPlayerCharacterNum[i]) && (playerRosterStartingLineupContent.transform.GetChild(k).GetComponent<StartingLineupButton>() == true))
                            {                                     
                                playerRosterCharacterListContent.transform.GetChild(j).gameObject.SetActive(false);                                
                                playerRosterStartingLineupContent.transform.GetChild(k).gameObject.SetActive(true);
                                playerRosterStartingLineupContent.transform.GetChild(k).gameObject.transform.SetSiblingIndex(i);
                                setPlayerNum ++;    
                                break;
                            }                          
                        }
                        break;
                    }
                }
            }
        }
        CheckPlayerRoster();
        #endregion
    }

    public void SelectPlayerRosterButton(int characterNum)
    { 
        #region
        if(characterNum >= GameManager.instance.useableCharacter.Length)
        { 
            Debug.LogWarning("SelectPlayerRosterButton 함수에 잘못된 값이 들어왔습니다! 들어 온 값 " + characterNum);
            return;
        }

        if(setPlayerNum <= GameManager.instance.selectPlayerCharacterNum.Length)
        { 
            for(int i = 0; i < playerRosterCharacterListContent.transform.childCount; i++)
            { 
                if(playerRosterCharacterListContent.transform.GetChild(i).GetComponent<CharacterListButton>().GetCharacterNum() == characterNum)
                { 
                    for(int j = 0; j < playerRosterStartingLineupContent.transform.childCount; j++)
                    { 
                        if(playerRosterStartingLineupContent.transform.GetChild(j).GetComponent<StartingLineupButton>().GetCharacterNum() == characterNum)
                        { 
                            for(int a = 0; a < GameManager.instance.selectPlayerCharacterNum.Length; a++)
                            { 
                                if(GameManager.instance.selectPlayerCharacterNum[a] == GameManager.RESETINDEX)
                                { 
                                    GameManager.instance.selectPlayerCharacterNum[a] = characterNum;
                                    playerRosterCharacterListContent.transform.GetChild(i).gameObject.SetActive(false);                                    
                                    playerRosterStartingLineupContent.transform.GetChild(j).gameObject.SetActive(true);
                                    playerRosterStartingLineupContent.transform.GetChild(j).gameObject.transform.SetSiblingIndex(a);
                                    setPlayerNum ++;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }
        else
        { 
            Debug.LogWarning("플레이어 캐릭터가 가득 찼습니다!");    
        }
        CheckPlayerRoster();
        #endregion
    }

    public void DeselectPlayerRosterButton(int characterNum)
    { 
        #region
        if(characterNum >= GameManager.instance.useableCharacter.Length)
        { 
            Debug.LogWarning("DeSelectPlayerRosterButton 함수에 잘못된 값이 들어왔습니다! 들어 온 값 " + characterNum);
            return;
        }

        for(int i = 0; i < GameManager.instance.selectPlayerCharacterNum.Length; i++)
        { 
            if(GameManager.instance.selectPlayerCharacterNum[i] == characterNum)
            { 
                for(int j = 0; j < playerRosterCharacterListContent.transform.childCount; j++)
                { 
                    if((playerRosterCharacterListContent.transform.GetChild(j).GetComponent<CharacterListButton>().GetCharacterNum() == characterNum) && (playerRosterCharacterListContent.transform.GetChild(j).GetComponent<CharacterListButton>() == true))
                    { 
                        for(int k = 0; k < playerRosterStartingLineupContent.transform.childCount; k++)
                        { 
                            if((playerRosterStartingLineupContent.transform.GetChild(k).GetComponent<StartingLineupButton>().GetCharacterNum() == characterNum) && (playerRosterStartingLineupContent.transform.GetChild(k).GetComponent<StartingLineupButton>() == true))
                            {                                     
                                playerRosterCharacterListContent.transform.GetChild(j).gameObject.SetActive(true);                                
                                playerRosterStartingLineupContent.transform.GetChild(k).gameObject.SetActive(false);
                                GameManager.instance.selectPlayerCharacterNum[i] = GameManager.RESETINDEX;
                                setPlayerNum --; 
                                GameManager.instance.SortCheckSelectPlayerCharacterNum(i);
                                SortStartingLineupContentChild(i);
                                break;
                            }                          
                        }
                        break;
                    }
                }
                break;
            }
        } 
        CheckPlayerRoster();
        #endregion
    }

    public void SortStartingLineupContentChild(int childNum)
    { 
        if(childNum < playerRosterStartingLineupContent.transform.childCount)
        { 
            if(playerRosterStartingLineupContent.transform.GetChild(childNum).transform.gameObject.activeSelf == false)
            { 
                int num = childNum;
                for(int i = num + 1; i < playerRosterStartingLineupContent.transform.childCount; i++)
                { 
                    if(playerRosterStartingLineupContent.transform.GetChild(i).transform.gameObject.activeSelf != false)
                    { 
                        playerRosterStartingLineupContent.transform.GetChild(i).gameObject.transform.SetSiblingIndex(num);
                        num++;
                    }     
                }
            }
        }
        else
        { 
            Debug.LogWarning("SortStartingLineupContentChild 함수에 잘못된 값이 들어 왔습니다! 잘못 들어있는 값 = " + childNum);
        }        
    }

    public void CheckPlayerRoster()
    { 
        playerRosterSelectCharacterText.GetComponent<Text>().text = "( " + setPlayerNum + " / " + GameManager.instance.selectPlayerCharacterNum.Length + " )"; 
        if(setPlayerNum >= GameManager.instance.selectPlayerCharacterNum.Length)
        { 
            //playerRosterNextStageButton.GetComponent<Button>().interactable = true;
            playerRosterNextStageButton.SetActive(true);
        }
        else
        { 
            //playerRosterNextStageButton.GetComponent<Button>().interactable = false;
            playerRosterNextStageButton.SetActive(false);
        }
    }

    //스테이지 정보
    public void SetStageInformationCanvas(bool mode)
    { 
        stageInformationCanvas.SetActive(mode);
        isStageInformationOn = mode;
        if(mode == true)
        { 
            SetAllHomeButtons(false);
            if (GameManager.instance.isTimeToMainScenario() == true)
            {
                SetMainStage();
            }
            else
            {
                SetSelectSubStage();
            }
            
        }    
        else
        { 
            SetSelectStageWindow(false);
            SetAllHomeButtons(true);

            SetStageInformationColor();
        }              
    }

    public void SetMainStage()
    { 
        SetSelectStageWindow(false);
        stageInformationCanvasRight.SetActive(true);
        if((GameManager.instance.CheckMainStage() == true) && (GameManager.instance.isTimeToMainScenario() == true))
        { 
            Stage nextStage = StageDatabaseManager.instance.GetStage(GameManager.instance.Var[GameManager.MAINSTAGEVARNUM].Var);
            SetStageInform(nextStage);                       
        }
        else if(GameManager.instance.CheckMainStage() == false)
        { 
            SetStageTheEnd(true); 
        }        
        else
        {
            SetStageD_day(true);
        }

        SetStageInformationColor(stageInformationButtons[0]); //임시용
    }

    public void SetSubStage(int num)
    {
        SetSelectStageWindow(false);
        stageInformationCanvasRight.SetActive(true);
        if((num < StageDatabaseManager.instance.GetSubStageLength()) && (0 <= num))
        { 
            GameManager.instance.SetSubStage(num);
            Stage nextStage = StageDatabaseManager.instance.GetSubStage(num);
            SetStageInform(nextStage);
        }
        else
        { 
            Debug.LogWarning("해당하는 서브스테이지가 없습니다!");    
        }
    }

    public void SetSelectSubStage()
    {
        BreakSelectStageContentChild();
        CreateSelectSubStageButtonPrefab();
        SetSelectStageWindow(true);

        SetStageInformationColor(stageInformationButtons[1]); //임시용
    }

    private void SetSelectStageWindow(bool mode)
    {
        selectStageWindow.SetActive(mode);
        SetPlayMainStage(false);
        if (mode == true)
        {
            if (GameManager.instance.isTimeToMainScenario() == true)
            {
                SetPlayMainStage(true);
            }
            stageInformationCanvasRight.SetActive(false);
        }
        else
        {
            BreakSelectStageContentChild();
        }
    }

    public void SetStageInform(Stage nextStage)
    {
        #region
        stageInformationCanvasRight.SetActive(true);
        SetStageTheEnd(false);
        stageInformationCanvasRight.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = nextStage.stageName;
        stageInformationCanvasRight.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = nextStage.stageExplain;
        stageInformationCanvasRight.transform.GetChild(1).GetChild(2).GetComponent<Image>().sprite = EnemyCharacterDatabaseManager.instance.GetEnemyCharacter(nextStage.enemyCharacterNum[0]).enemyStatus.face[0];
        stageInformationCanvasRight.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = "<" + EnemyCharacterDatabaseManager.instance.GetEnemyCharacter(nextStage.enemyCharacterNum[0]).enemyStatus.inGameName + ">";
        stageInformationCanvasRight.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = nextStage.stageWinCondition;

        stageInformationCanvasRight.transform.GetChild(3).GetChild(0).GetChild(1).GetComponent<Text>().text = nextStage.playerCharacterCount + "명";
        stageInformationCanvasRight.transform.GetChild(3).GetChild(1).GetChild(1).GetComponent<Text>().text = NameDatabaseManager.EXPName + " : " + nextStage.clearEXP;
        stageInformationCanvasRight.transform.GetChild(3).GetChild(1).GetChild(2).GetComponent<Text>().text = NameDatabaseManager.goldName + " : " + nextStage.clearGold;
        #endregion
    }

    private void SetStageTheEnd(bool mode)
    { 
        #region
        if(mode == true)
        {
            stageInformationCanvasRight.transform.GetChild(0).gameObject.SetActive(false);
            stageInformationCanvasRight.transform.GetChild(1).gameObject.SetActive(false);
            stageInformationCanvasRight.transform.GetChild(2).gameObject.SetActive(false);
            stageInformationCanvasRight.transform.GetChild(3).gameObject.SetActive(false);
            stageInformationCanvasRight.transform.GetChild(4).gameObject.SetActive(false);
            stageInformationCanvasRight.transform.GetChild(5).gameObject.SetActive(true);
            stageInformationCanvasRight.transform.GetChild(5).GetChild(0).gameObject.GetComponent<Text>().text = "-THE END-";
        }
        else
        { 
            stageInformationCanvasRight.transform.GetChild(0).gameObject.SetActive(true);
            stageInformationCanvasRight.transform.GetChild(1).gameObject.SetActive(true);
            stageInformationCanvasRight.transform.GetChild(2).gameObject.SetActive(true);
            stageInformationCanvasRight.transform.GetChild(3).gameObject.SetActive(true);
            stageInformationCanvasRight.transform.GetChild(4).gameObject.SetActive(true);
            stageInformationCanvasRight.transform.GetChild(5).gameObject.SetActive(false);            
        }
        #endregion
    }

    private void SetPlayMainStage(bool mode)
    { 
        #region
        if(mode == true)
        {
            selectStageWindow.transform.GetChild(0).gameObject.SetActive(false);
            selectStageWindow.transform.GetChild(1).gameObject.SetActive(false);
            selectStageWindow.transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        { 
            selectStageWindow.transform.GetChild(0).gameObject.SetActive(true);
            selectStageWindow.transform.GetChild(1).gameObject.SetActive(true);
            selectStageWindow.transform.GetChild(2).gameObject.SetActive(false);       
        }
        #endregion
    }

    private void SetStageD_day(bool mode)
    { 
        #region
        SetStageTheEnd(mode);
        if(mode == true)
        {
            stageInformationCanvasRight.transform.GetChild(5).GetChild(0).gameObject.GetComponent<Text>().text = "다음 경기 " + GameManager.instance.GetD_day() + "일 후";
        }
        #endregion
    }

    private void CreateSelectSubStageButtonPrefab()
    { 
        #region
        for(int i = 0; i < StageDatabaseManager.instance.GetSubStageLength(); i++)
        { 
            if(StageDatabaseManager.instance.GetSubStage(i).stageNeedMainStageNum <= GameManager.instance.Var[GameManager.MAINSTAGEVARNUM].Var)
            { 
                if(selectSubStageButtonPrefab != null)
                { 
                    GameObject selectSubStageButton = Instantiate(selectSubStageButtonPrefab);
                    selectSubStageButton.name = "selectSubStageButton_" + i;
                    selectSubStageButton.GetComponent<SelectSubStageButton>().SetSubStageButton(i);

                    selectSubStageButton.transform.SetParent(selectStageContent.transform); 
                    selectSubStageButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                } 
            }
        }
        #endregion
    }

    private void BreakSelectStageContentChild()
    { 
        #region
        for(int i = 0; i < selectStageContent.transform.childCount; i++)
        { 
             Destroy(selectStageContent.transform.GetChild(i).gameObject);
        }
        #endregion
    }

    private void SetStageInformationColor(GameObject selectObject = null)
    {
        for (int i = 0; i < stageInformationButtons.Length; i++)
        {
            if ((selectObject != null) && (stageInformationButtons[i] == selectObject))
            {
                stageInformationButtons[i].GetComponent<Image>().GetComponent<Image>().color = stageInformationButtonSelectColor;
            }
            else
            {
                stageInformationButtons[i].GetComponent<Image>().GetComponent<Image>().color = stageInformationButtonNormalColor;
            }
        }
    }

    #endregion
    //----

    //---- 스테이터스 용
    #region
    public void SetPlayerStatusCanvas(bool mode)
    { 
        isStatusUIOn = mode;
        statusCanvas.SetActive(mode);
        if(mode == true)
        { 
            if(homeMenuCanvas.activeSelf == true)
            { 
                homeMenuCanvas.SetActive(false);    
            }

            SetAllHomeButtons(false);
            CreatePlayerStatusButtons();
            SetPlayerStatus(0);
        }    
        else
        { 
            SetCharacterStatusFlip(false);
            SetCharacterSkillButton(0, false);
            SetAllHomeButtons(true);
            BreakPlayerStatusButtons();
        }           
    }

    public void SetPlayerStatus(int num)
    {
        #region
        if((num < GameManager.instance.PlayerCharacter.Length) && (num >= 0))
        { 
            SetCharacterSkillButton(num, false);

            if(GameManager.instance.PlayerCharacter[num].characterInformation.characterIllustration != null)
            { 
                spaceLeftR.transform.GetChild(0).GetComponent<Image>().sprite = GameManager.instance.PlayerCharacter[num].characterInformation.characterIllustration;
            }
            else
            { 
                spaceLeftR.transform.GetChild(0).GetComponent<Image>().sprite = GameManager.instance.GetDefaultIllustration();
            }
            spaceRight.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.levelName + " : " + GameManager.instance.PlayerCharacter[num].level;           
            spaceRight.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = (float)GameManager.instance.PlayerCharacter[num].EXP/GameManager.instance.PlayerCharacter[num].maxEXP;
            spaceRight.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].EXP + " / " + GameManager.instance.PlayerCharacter[num].maxEXP;
            spaceRight.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].inGameName;
            spaceRight.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].characterInformation.characterHeight + "cm";
            spaceRight.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].characterInformation.characterWeight + "kg";
            spaceRight.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].characterInformation.characterBirthMonth + "월 " + GameManager.instance.PlayerCharacter[num].characterInformation.characterBirthDay + "일 " + "(" + GameManager.instance.PlayerCharacter[num].characterInformation.characterAge+ ")";
            spaceRight.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].characterInformation.characterExplain;

            spaceRight.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.HPName;
            spaceRight.transform.GetChild(4).GetChild(0).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].maxHP + "";

            spaceRight.transform.GetChild(4).GetChild(1).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.MPName;
            spaceRight.transform.GetChild(4).GetChild(1).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].maxMP + "";

            spaceRight.transform.GetChild(4).GetChild(2).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.ATKName;
            spaceRight.transform.GetChild(4).GetChild(2).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].ATK + "";

            spaceRight.transform.GetChild(4).GetChild(3).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.MAKName;
            spaceRight.transform.GetChild(4).GetChild(3).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].MAK + "";

            spaceRight.transform.GetChild(4).GetChild(4).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.DEFName;
            spaceRight.transform.GetChild(4).GetChild(4).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].DEF + "";

            spaceRight.transform.GetChild(4).GetChild(5).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.MDFName;
            spaceRight.transform.GetChild(4).GetChild(5).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].MDF + "";

            spaceRight.transform.GetChild(4).GetChild(6).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.rangeName;
            spaceRight.transform.GetChild(4).GetChild(6).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].range + "";

            spaceRight.transform.GetChild(4).GetChild(7).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.moveName;
            spaceRight.transform.GetChild(4).GetChild(7).GetChild(1).GetComponent<Text>().text = GameManager.instance.PlayerCharacter[num].move + "";

            spaceRight.transform.GetChild(5).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.SympathyTypeRationalName + ": " + SympathyDatabaseManager.instance.GetSympathyDataByCharacterSympathyType(GameManager.instance.PlayerCharacter[num].sympathyType).minRational + " ~ " + SympathyDatabaseManager.instance.GetSympathyDataByCharacterSympathyType(GameManager.instance.PlayerCharacter[num].sympathyType).maxRational;
            spaceRight.transform.GetChild(5).GetChild(1).GetComponent<Text>().text = NameDatabaseManager.SympathyTypeAngerName + ": " + SympathyDatabaseManager.instance.GetSympathyDataByCharacterSympathyType(GameManager.instance.PlayerCharacter[num].sympathyType).minAnger+ " ~ " + SympathyDatabaseManager.instance.GetSympathyDataByCharacterSympathyType(GameManager.instance.PlayerCharacter[num].sympathyType).maxAnger;
            spaceRight.transform.GetChild(5).GetChild(2).GetComponent<Text>().text = NameDatabaseManager.SympathyTypeEnjoyName + ": " + SympathyDatabaseManager.instance.GetSympathyDataByCharacterSympathyType(GameManager.instance.PlayerCharacter[num].sympathyType).minEnjoy+ " ~ " + SympathyDatabaseManager.instance.GetSympathyDataByCharacterSympathyType(GameManager.instance.PlayerCharacter[num].sympathyType).maxEnjoy;
            SetCharacterSkillButton(num, true);
                
        }
        else
        { 
            Debug.LogWarning("HomeManager스크립트 SetPlayerStatus함수에 잘못 된 값이 들어왔습니다! 들어온 값은 " + num);    
        }
        #endregion
    }

    public void SetSkillExplainText(string explain)
    { 
        spaceRightBack.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = explain;
    }

    private void CreatePlayerStatusButtons()
    { 
        #region
        for(int i = 0; i < GameManager.instance.useableCharacter.Length; i++)
        { 
            if(GameManager.instance.useableCharacter[i] == true)
            { 
                if(characterStatusButtonPrefab != null)
                { 
                    GameObject characterStatusButton = Instantiate(characterStatusButtonPrefab);
                    characterStatusButton.name = "characterStatusButton_" + i;
                    characterStatusButton.GetComponent<CharacterStatusButton>().SetCharacterStatusButton(i);

                    characterStatusButton.transform.SetParent(characterStatusButtonScrollviewContent.transform); 
                    characterStatusButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
            }            
        }
        #endregion
    }

    private void BreakPlayerStatusButtons()
    { 
        #region
        for(int i = 0; i < characterStatusButtonScrollviewContent.transform.childCount; i++)
        { 
             Destroy(characterStatusButtonScrollviewContent.transform.GetChild(i).gameObject);
        }
        #endregion
    }

    private void SetCharacterSkillButton(int num, bool mode)
    { 
        #region
        SetSkillExplainText("");
        if(mode == true)
        { 
            for(int i = 0; i < GameManager.instance.PlayerCharacter[num].characterSkill.Length; i++)
            { 
                if(GameManager.instance.PlayerCharacter[num].characterSkill[i] == true)
                { 
                    GameObject skillButton = Instantiate(characterStatusSkillButtonPrefab);
                    skillButton.name = "SkillButton_" + i;
                    skillButton.GetComponent<StatusCharacterSkillButton>().SetSkillNum(i);

                    skillButton.transform.SetParent(characterStatusSkillContent.transform);     
                    skillButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); 
                }
            }            
        }   
        else
        { 
            for(int i = 0; i < characterStatusSkillContent.transform.childCount; i++)
            { 
                 Destroy(characterStatusSkillContent.transform.GetChild(i).gameObject);
            }           
        }
        #endregion
    }

    public void CharacterStatusFlip()
    { 
        if(isCharacterStatusBack == true)
        { 
            SetCharacterStatusFlip(false);
            
        }
        else
        { 
            SetCharacterStatusFlip(true);          
        }
    }

    private void SetCharacterStatusFlip(bool mode)
    { 
        isCharacterStatusBack = mode;

        if(mode == true)
        { 
            spaceRight.SetActive(false);
            spaceRightBack.SetActive(true);
        }
        else
        { 
            spaceRight.SetActive(true);
            spaceRightBack.SetActive(false);
        }
    }
    
    #endregion
    //------
    
    //휴식용
    #region
    public void SetRestCanvas(bool mode)
    {        
        if (GameManager.instance.isTimeToMainScenario() == true)
        {
            SetWarningText("오늘은 경기가 있는 날이니 경기준비 버튼을 눌러서 경기를 진행하자");
            return;
        }

        isRestCanvasOn = mode;
        restCanvas.SetActive(mode);

        if (mode == true)
        {
            SetAllHomeButtons(false);
            CheckRestButton();
        }
        else
        {
            SetAllHomeButtons(true);
        }
    }

    private void CheckRestButton()
    {
        GameObject restScollViewContent = restCanvas.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;

        for (int i = 0; i < restScollViewContent.transform.childCount; i++)
        {
            if (restScollViewContent.transform.GetChild(i).GetComponent<RestButtonController>() == true)
            {
                if (GameManager.instance.CheckGold(restScollViewContent.transform.GetChild(i).GetComponent<RestButtonController>().GetCost()) == false)
                {
                    restScollViewContent.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = false;
                }
                else
                {
                    restScollViewContent.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                }
            }
        }
    }

    public void Rest(int fill, int cost)
    {
        if (GameManager.instance.CheckGold(cost) == true)
        {
            GameManager.instance.gold -= cost;
            GameManager.instance.IncreasePlayerTeamHP(fill);
            SetRestCanvas(false);
            GameManager.instance.NextDay();
        }
    }
    #endregion

    //대화 이벤트 관련
    public void SetTalkCanvas(bool mode)
    {  
        isTalkEventOn = mode;
        talkCanvas.SetActive(mode); 
        if(mode == true)
        { 
            SetAllHomeButtons(false);
        }
        else
        { 
            SetAllHomeButtons(true);
        } 
    }

    public void SetTalkEvent(bool mode)
    { 
        SetTalkCanvas(mode);
        if(mode == true)
        { 
            homeEventManager.ManagerTalkEvent();
        }
        else
        { 
            homeEventManager.ClearTalkCanvas(); 
        }       
    }

    public void SetWarningText(string warningText, bool isUI = false)
    {
        homeEventManager.SetWarningText(warningText, isUI);
    }

    public bool GetIsWarningText()
    {
        return homeEventManager.GetIsWarningText();
    }
}
