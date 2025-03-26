using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  

public class HomeEventManager : MonoBehaviour
{
    private HomeManager homeManager;

    List<Dictionary<string, object>> TalkData;
    [SerializeField]
    private GameObject[] talkerImage; // ���� �߾� ������ ������ 0 1 2
    private Talker[] talker; // ���� �߾� ������ ������ 0 1 2
    [SerializeField]
    private GameObject talkerNameText; // ȭ�� �̸�
    [SerializeField]
    private GameObject talkText; // ��ȭ ����

    [SerializeField]
    private Color talkColor = new Color(255/255f, 255/255f, 255/255f, 255/255f);
    [SerializeField]
    private Color stayColor = new Color(100/255f, 100/255f, 100/255f, 255/255f);
    [SerializeField]
    private Vector3 talkScale = new Vector3(1.0f, 1.0f, 1.0f);
    [SerializeField]
    private Vector3 stayScale = new Vector3(1.0f, 1.0f, 1.0f);

    private string HomeCSVPath;
    private string ManagerTalkFolder = "ManagerTalkEvent/"; //���� HomeCSVPath������ �Ŵ��� �̺�Ʈ��ġ�� ����Ű�°�
    private string ManagerTalkCSVPath; // Resources�������� �Ŵ��� �̺�Ʈ������ ��ġ �����̺�Ʈ��
    private int CSVNum;

    private bool isTalk;
    private bool isTyping;
    private bool keyCheck;
    private string choiceKey;

    private float typingSpeed = 0.1f; //��ȭ ���ǵ�
    [SerializeField]
    private float defaultTalkSpeed = 0.06f; // ����Ʈ ��ȭ ���ǵ�
    [SerializeField]
    private char lineBreakForTalk = '#'; //���κ��� �ؽ�Ʈ

    private bool isWarningText = false; // ��� �ؽ�Ʈ��
    private bool isWarningTextOnUI = false; // UI�϶� ��� �����°� ����
    private int keyIgnoreCount = 0; //Ű�Է� �ߺ� ������

    //Ʃ���
    [SerializeField]
    private GameObject talkCanvas;
    [SerializeField]
    private GameObject tutorialCanvas;
    [SerializeField]
    private GameObject tutorialText;
    [SerializeField]
    private GameObject tutorialArrow;
    [SerializeField]
    private GameObject homeSceneButtons;
    [SerializeField]
    private GameObject characterStatusButton_Image;
    private bool isTutorial = false;
    //--
    [SerializeField]
    private GameObject effectCanvas;

    [SerializeField]
    private GameObject fadeImage;
    [SerializeField]
    private GameObject D_dayWindow;
    [SerializeField]
    private GameObject nextDayWindow;
    [SerializeField]
    private GameObject D_dayTexts;
    [SerializeField]
    private GameObject nextDayCenterMask;
    [SerializeField]
    private GameObject nextDayArrowImage;
    [SerializeField]
    private GameObject nextDayText;
    [SerializeField]
    private GameObject[] nextDayRotationImage;
    [SerializeField]
    private GameObject circleMoveCenter;
    private float effectTime = 0.0f;
    private Color defaultD_dayTextColor = new Color (152f/255f, 152f/255f, 152f/255f);
    private Color selectD_dayTextColor = new Color (1.0f, 1.0f, 1.0f);

    void Awake()
    {
        InitializeHomeEventManager();
    }

    void Update()
    { 
        if(homeManager.isTalkEventOn == true)
        { 
            if((GameManager.instance.KeyCheckAccept() == true) || (GameManager.instance.KeyCheckSkip() == true))
            { 
                NextTalkButton();
            } 
        }
    }

    public void NextTalkButton()
    { 
        if (keyIgnoreCount > 0)
        {
            keyIgnoreCount--;
        }
        else if(isTyping == true)
        { 
            isTyping = false;
        }
        else if(isWarningText == true)
        { 
            if (CantTalkCheck() == false)
            {
                WarningTextEnd();
            }           
        }
        else
        { 
            if(CantTalkCheck() == false)
            { 
                PrepareNextTalk();
            }    
        }

    }

    private void InitializeHomeEventManager()
    { 
        homeManager = this.gameObject.GetComponent<HomeManager>();
        talker = new Talker[talkerImage.Length];
        HomeCSVPath = GameManager.instance.CSVFolder + "Home/";
        ManagerTalkCSVPath = HomeCSVPath + "ManagerTalkEvent/";
        ClearTalkCanvas();
    }

    public void ClearTalkCanvas()
    { 
        keyIgnoreCount = 0;
        isWarningText = false;
        for(int i = 0; i < talker.Length; i++)
        { 
            talker[i] = null;
            talkerImage[i].GetComponent<Image>().sprite = GameManager.instance.GetBlank();     
        }     
        ResetTalkKey();
        CSVNum = 0;
        if(TalkData != null)
        { 
            TalkData.Clear();
        }
    }

    public void SetTalkData(string name, string part)
    { 
        ClearTalkCanvas();
        homeManager.SetTalkCanvas(true);
        TalkData = CSVReader.Read(HomeCSVPath + name, part);
        Debug.Log("�뺻�� ���̴� " + TalkData.Count);
        NextTalk();
    }

    public void SetWarningText(string warningText, bool isUI = false)
    {
        ClearTalkCanvas();
        talkerNameText.GetComponent<Text>().text = "�Ŵ���";
        talkText.GetComponent<Text>().text = "";
        int position = 1;
        string name = "Manager";
        for(int i = 0; i < TalkerDatabaseManager.instance.DBLength(); i++)
        { 
            if(TalkerDatabaseManager.instance.TalkerDB(i).callName == name)
            { 
                talker[position] = TalkerDatabaseManager.instance.TalkerDB(i);
                talkerImage[position].GetComponent<Image>().sprite = talker[position].characterImage[0];

                talkerImage[position].GetComponent<Image>().color = talkColor;
                talkerImage[position].transform.localScale = talkScale;
                break;
            }
        } 
        homeManager.SetTalkCanvas(true);
        isWarningText = true;
        isTalk = true;
        keyIgnoreCount = 1;
        isWarningTextOnUI = isUI;
        StartCoroutine(Typing(warningText, defaultTalkSpeed));
    }

    public void WarningTextEnd()
    {
        homeManager.SetTalkCanvas(false);
        isWarningText = false;
        if (isWarningTextOnUI == true)
        {
            isWarningTextOnUI = false;
            homeManager.SetAllHomeButtons(false);
        }
    }

    public bool GetIsWarningText()
    {
        return isWarningText;
    }

    // �⺻ ��ȭ��
    #region
    private bool CantTalkCheck()
    { 
        if(isTalk == true || isTyping== true)
        { 
            return true;
        }
        else
        { 
            return false;
        }        
    }

    private void ResetKey()
    { 
        ResetTalkKey();
        PrepareNextTalk();        
    }

    private void ResetTalkKey()
    { 
        #region
        keyCheck = false;
        choiceKey = "";
        /*
        for(int i = 0; i < selectChoiceKey.Length; i++)
        { 
            selectChoiceKey[i] = "";
        }       
        */
        Debug.Log("Ű(������)������ �ʱ�ȭ �Ǿ����ϴ�");      
        #endregion
    }

    private void IncreaseCSVNUM()
    { 
        CSVNum++;
    }

    public void PrepareNextTalk()
    { 
        IncreaseCSVNUM();
        NextTalk();        
    }

    private void NextTalk()
    {
        #region
        if(CSVNum < TalkData.Count)
        {  
            if(TalkData[CSVNum]["Event"].ToString() == "ResetKey")
            { 
                ResetKey();
            }
            else
            { 
                if(keyCheck == true)
                {
                    Debug.Log("KeyCheck������ �ɷ��ֽ��ϴ�.");
                    if(TalkData[CSVNum]["Branch"].ToString() == choiceKey)
                    {
                        Debug.Log("Branch = choiceKey������ ������ ����մϴ�.");
                        NextTalkCheck();
                    }
                    else
                    {
                        Debug.Log("Branch != choiceKey������ ������ ��ŵ�մϴ�.");
                        PrepareNextTalk();
                    }
                
                }
                else
                { 
                    NextTalkCheck();
                }
            }
        }
        else
        { 
            TutorialModeEnd();
            homeManager.SetTalkCanvas(false);
        }
        #endregion
    }

    private void NextTalkCheck()
    {   
        #region
        if(TalkData[CSVNum]["Event"].ToString() != "")
        { 
            if(EventCheck(CSVNum) == false)
            { 
                Talk(CSVNum);
            }
        }
        else
        { 
            Talk(CSVNum);
        }  
        #endregion
    }

    private bool EventCheck(int num)
    { 
        switch(TalkData[num]["Event"].ToString())
        { 
            case "AddTalker":
                AddTalker(num);
                return true; 
            case "RemoveTalker":
                RemoveTalker(num);
                return true;
            case "SelectTalker":
                SelectTalker(num);
                return true;
            case "TalkerBack":
                TalkerBack();
                return true;
            case "ChangeFace":
                ChangeFace(num);
                return true;              
            case "Bookmark":
                Bookmark();
                return true;
            case "ChangeBGM":
                ChangeBGM(num);
                return true;
            case "StartBGM":
                StartBGM();
                return true;
            case "StopBGM":
                StopBGM();
                return true;
            case "PlaySE":
                PlaySE(num);
                return true;
            case "ChangeUseableCharacter":
                ChangeUseableCharacter(num);
                return true;
            case "SetTutorialMode":
                SetTutorialMode();
                return true;
            case "SetTutorialModeEnd":
                SetTutorialModeEnd();
                return true;
            case "SetTutorialArrow":
                SetTutorialArrow(num);
                return true;
            case "TutorialArrowEnd":
                TutorialArrowEnd();
                return true;
            default:
                return false;
        }       
    }

    private void Talk(int num)
    { 
        #region
        if (isTutorial == true)
        {
            tutorialText.GetComponent<Text>().text = "";
            isTalk = true;
            StartCoroutine(Typing(TalkData[num]["Content"].ToString(), defaultTalkSpeed));
            return;
        }

        bool isTalkerCheckFail = false;
        int talkerNum = -1;

        talkText.GetComponent<Text>().text = "";
        isTalk = true;
        talkerNameText.GetComponent<Text>().text = "";

        if(int.TryParse(TalkData[num]["Talker"].ToString(), out int index))
        { 
            talkerNum = index;
            Debug.Log("���� ���� ���� ���� ���ڴ� " + index);
        }

        if((0 <= talkerNum) && (talkerNum < talker.Length))
        { 
            if(talker[talkerNum] != null)
            {
                SetTalkerUi(talkerNum);
            }
            else
            { 
                isTalkerCheckFail = true;               
            }            
        }
        else
        {
            int TalkerCheck = 0;
            for(int j = 0; j < talker.Length; j++)
            {
                if(talker[j] != null)
                { 
                    if(talker[j].callName == TalkData[num]["Talker"].ToString())
                    {
                        SetTalkerUi(j);                 
                        break;
                    }
                } 
                TalkerCheck ++;
            }

            if(TalkerCheck >= talker.Length)
            {
                isTalkerCheckFail = true;
            }
            TalkerCheck = 0;
        }      

        if(isTalkerCheckFail == true)
        { 
            talkerNameText.GetComponent<Text>().text = TalkData[num]["Talker"].ToString();
            typingSpeed = defaultTalkSpeed;
            Debug.Log("������ �ι��� �����Ƿ� �̸��� ����մϴ�.");            
        }

        StartCoroutine(Typing(TalkData[num]["Content"].ToString(), typingSpeed));
        #endregion    
    }

    private void SetTalkerUi(int num)
    { 
        #region
        Debug.Log("�����ι��� üũ �Ǿ����ϴ�.");
        ChangeTalker(num);
        talkerNameText.GetComponent<Text>().text = talker[num].inGameName;   
        if(talker[num].talkSpeed > 0)
        { 
            typingSpeed = talker[num].talkSpeed;
        }   
        #endregion
    }

    IEnumerator Typing(string text, float talkSpeed)
    {
        #region
        if(isTalk == true)
        { 
            isTyping = true;
            
            char[] word =  text.ToCharArray();
            for(int i = 0; i < word.Length; i++)
            {
                if(word[i] == lineBreakForTalk)
                { 
                    if (isTutorial == false)
                    {
                        talkText.GetComponent<Text>().text += "\n";
                    }
                    else
                    {
                        tutorialText.GetComponent<Text>().text += "\n";
                    }
                }
                else
                { 
                    if (isTutorial == false)
                    {
                        talkText.GetComponent<Text>().text += word[i];
                    }
                    else
                    {
                        tutorialText.GetComponent<Text>().text += word[i];
                    }                    
                }
                yield return new WaitForSeconds(talkSpeed);

                if(isTyping == false)
                {                    
                    talkText.GetComponent<Text>().text = "";
                    tutorialText.GetComponent<Text>().text = "";
                    for(int j = 0; j < word.Length; j++)
                    {
                        if(word[j] == lineBreakForTalk)
                        { 
                            if (isTutorial == false)
                            {
                                talkText.GetComponent<Text>().text += "\n";
                            }
                            else
                            {
                                tutorialText.GetComponent<Text>().text += "\n";
                            }                            
                        }
                        else
                        { 
                            if (isTutorial == false)
                            {
                                talkText.GetComponent<Text>().text += word[j];
                            }
                            else
                            {
                                tutorialText.GetComponent<Text>().text += word[j];
                            } 
                        }
                    }
                    isTalk= false;
                    Debug.Log("���ϱ� ��ŵ");
                    break;                   
                }                
            }
            isTyping = false;
            isTalk= false;
        }
        #endregion
    }

    private void ChangeTalker(int num) // -1 �Ͻ� ���� �� ���ϴ� ���·�
    {
        #region
        Debug.Log("ȭ�� ���� : " + num);
        if(num == -1)
        { 
            for(int i = 0; i < talker.Length; i++)
            { 
                talkerImage[i].GetComponent<Image>().color = stayColor;
                talkerImage[i].transform.localScale = stayScale;
            }      
        }
        else if((0 <= num) && (num < talker.Length))
        { 
            for(int i = 0; i < talker.Length; i++)
            { 
                if(i == num)
                {
                    talkerImage[i].GetComponent<Image>().color = talkColor;
                    talkerImage[i].transform.localScale = talkScale;                
                }
                else
                { 
                    talkerImage[i].GetComponent<Image>().color = stayColor;
                    talkerImage[i].transform.localScale = stayScale;
                }
            }
        }
        else
        { 
            Debug.LogWarning("TalkManager�� ChangeTalker(int num)�� �߸��� ���� ���Խ��ϴ�.");   
        }
        #endregion
    }

    private void AddTalker(int num)
    { 
        #region
        int position = int.Parse(TalkData[num]["Content"].ToString());
        string name = TalkData[num]["Talker"].ToString();

        for(int i = 0; i < TalkerDatabaseManager.instance.DBLength(); i++)
        { 
            if(TalkerDatabaseManager.instance.TalkerDB(i).callName == name)
            { 
                talker[position] = TalkerDatabaseManager.instance.TalkerDB(i);
                talkerImage[position].GetComponent<Image>().sprite = talker[position].characterImage[0];

                talkerImage[position].GetComponent<Image>().color = stayColor;
                talkerImage[position].transform.localScale = stayScale;
                break;
            }
        }        
        PrepareNextTalk();
        #endregion
    }

    private void RemoveTalker(int num)
    { 
        #region
        int position = int.Parse(TalkData[num]["Content"].ToString());

        talker[position] = null;
        talkerImage[position].GetComponent<Image>().sprite = GameManager.instance.GetBlank(); 
        
        PrepareNextTalk();
        #endregion
    }

    private void SelectTalker(int num)
    { 
        ChangeTalker(int.Parse(TalkData[num]["Content"].ToString()));
        PrepareNextTalk();
    }

    private void TalkerBack()
    {
        ChangeTalker(-1);           
        PrepareNextTalk();            
    }

    private void ChangeFace(int num)
    { 
        #region
        for(int i = 0; i < talker.Length; i++)
        { 
            if(talker[i] != null)
            { 
                if(TalkData[num]["Talker"].ToString() == talker[i].callName)
                { 
                    int faceNum = int.Parse(TalkData[num]["Content"].ToString());
                    if(faceNum < talker[i].characterImage.Length)
                    { 
                        talkerImage[i].GetComponent<Image>().sprite = talker[i].characterImage[faceNum];
                    }
                    else
                    { 
                        Debug.LogWarning("TalkManager�� ChangeFace�� �߸��� ��(Talker)�� ���Խ��ϴ�." + "CSVNum = " + num);
                    }
                    PrepareNextTalk();
                    return;
                }    
            }
        }


        int changeTargetTalker = int.Parse(TalkData[num]["Talker"].ToString());

        if(changeTargetTalker >= talker.Length)
        { 
            Debug.LogWarning("TalkManager�� ChangeFace�� �߸��� ��(Talker)�� ���Խ��ϴ�." + "CSVNum = " + num);
            PrepareNextTalk();
            return;
        }

        int changeFaceNum = int.Parse(TalkData[num]["Content"].ToString());

        if(talker[changeTargetTalker] != null)
        { 
            if(changeFaceNum >= talker[changeTargetTalker].characterImage.Length)
            { 
                Debug.LogWarning("TalkManager�� Content�� �߸��� ��(Talker)�� ���Խ��ϴ�." + "CSVNum = " + num);
                PrepareNextTalk();
                return;                
            }
           
            talkerImage[changeTargetTalker].GetComponent<Image>().sprite = talker[changeTargetTalker].characterImage[changeFaceNum];

        }
        else
        { 
            Debug.LogWarning("TalkManager�� ChangeFace�� �߸��� ��(Talker)�� ���Խ��ϴ�." + "CSVNum = " + num);
        }
        PrepareNextTalk();
        #endregion
    }

    private void Bookmark()
    { 
        PrepareNextTalk();
    }

    private void ChangeBGM(int num)
    { 
        GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName(TalkData[num]["Content"].ToString()));
        PrepareNextTalk();             
    }

    private void StartBGM()
    { 
        GameManager.instance.StartBGM();
        PrepareNextTalk();             
    }

    private void StopBGM()
    { 
        GameManager.instance.StopBGM();
        PrepareNextTalk();              
    }

    private void PlaySE(int num)
    { 
        GameManager.instance.PlaySE(SEDatabaseManager.instance.GetSEByName(TalkData[num]["Content"].ToString()));
        PrepareNextTalk();        
    }

    private void ChangeUseableCharacter(int num)
    { 
        #region
        bool mode;

        if(TalkData[num]["Content"].ToString() == "true")
        { 
            mode = true;
        }
        else if(TalkData[num]["Content"].ToString() == "false")
        { 
            mode = false;
        }
        else
        { 
            Debug.LogWarning("ChangeUseableCharacter �̺�Ʈ�� Content�� �߸� �� ���� ���Խ��ϴ�! CSVNum = " + num);
            PrepareNextTalk();
            return;
        }

        for(int i = 0; i < CharacterDatabaseManager.instance.DBLength(); i++)
        { 
            if(CharacterDatabaseManager.instance.GetPlayerCharacter(i).name == TalkData[num]["Talker"].ToString())
            { 
                GameManager.instance.ChangeUseableCharacter(i, mode);
                Debug.Log(i + "�� ĳ���� " + CharacterDatabaseManager.instance.GetPlayerCharacter(i).inGameName + "�� ���� �۾� ����, �۾� ������ = " + mode);
                break;
            }
        }
        PrepareNextTalk();
        #endregion
    }

    private void SetTutorialMode()
    {
        talkCanvas.SetActive(false);
        tutorialCanvas.SetActive(true);
        tutorialText.GetComponent<Text>().text = "";
        tutorialArrow.SetActive(false);
        SetHomeButtonInteractable(false);
        homeManager.SetAllHomeButtons(true);
        isTutorial = true;
        PrepareNextTalk(); 
    }

    private void SetTutorialModeEnd()
    {
        TutorialModeEnd();
        PrepareNextTalk(); 
    }

    private void TutorialModeEnd()
    {
        talkCanvas.SetActive(true);
        tutorialCanvas.SetActive(false);
        tutorialText.GetComponent<Text>().text = "";
        tutorialArrow.SetActive(false);
        SetHomeButtonInteractable(true);
        homeManager.SetAllHomeButtons(false);
        isTutorial = false;
    }

    private void SetHomeButtonInteractable(bool mode)
    {
        if (mode == false)
        {
            homeManager.isEffect = true;
        }
        else
        {
            homeManager.isEffect = false;
        }

        for (int i = 0; i < homeSceneButtons.transform.childCount; i++)
        {
            homeSceneButtons.transform.GetChild(i).GetComponent<Button>().interactable = mode;
        }
        characterStatusButton_Image.GetComponent<Button>().interactable = mode;
    }

    private void SetTutorialArrow(int num)
    {
        Vector3 setStageButtonArrowPos = new Vector3 (-550, -220, 0);
        Vector3 trainingButtonArrowPos = new Vector3 (-275, -220, 0);
        Vector3 restButtonArrowPos = new Vector3 (0, -220, 0);
        Vector3 characterStatusButtonArrowPos = new Vector3 (275, -220, 0);
        Vector3 menuButtonArrowPos = new Vector3 (550, -220, 0);
        Vector3 D_DayUIPos = new Vector3 (-845, 180, 0);
        Vector3 teamInformUIPos = new Vector3 (760, 255, 0);

        switch(TalkData[num]["Content"].ToString())
        { 
            case "SetStageButton":
                tutorialArrow.GetComponent<TutorialArrow>().SetTutorialArrowPosition(setStageButtonArrowPos);
                break;
            case "TrainingButton":
                tutorialArrow.GetComponent<TutorialArrow>().SetTutorialArrowPosition(trainingButtonArrowPos);
                break;
            case "RestButton":
                tutorialArrow.GetComponent<TutorialArrow>().SetTutorialArrowPosition(restButtonArrowPos);
                break;
            case "CharacterStatusButton":
                tutorialArrow.GetComponent<TutorialArrow>().SetTutorialArrowPosition(characterStatusButtonArrowPos);
                break;
            case "MenuButton":
                tutorialArrow.GetComponent<TutorialArrow>().SetTutorialArrowPosition(menuButtonArrowPos);
                break;
            case "D_DayUI":
                tutorialArrow.GetComponent<TutorialArrow>().SetTutorialArrowPosition(D_DayUIPos, false);
                break;
            case "TeamInformUI":
                tutorialArrow.GetComponent<TutorialArrow>().SetTutorialArrowPosition(teamInformUIPos, false);
                break;
            default:
                Debug.LogWarning("�ش��ϴ� ���� �����ϴ�!");
                break;
        }
        PrepareNextTalk(); 
    }

    private void TutorialArrowEnd()
    {
        tutorialArrow.SetActive(false);
        PrepareNextTalk(); 
    }
    #endregion

    //-- ����Ʈ��
    private void SetEffectCanvas(bool mode)
    {
        if (mode == true)
        {
            effectCanvas.SetActive(true);
        }
        else
        {
            effectCanvas.SetActive(false);

            for (int i = 0; i < effectCanvas.transform.childCount; i++)
            {
                effectCanvas.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void SetFadeEffect(bool mode)
    {
        fadeImage.SetActive(mode);
    }

    private void SetFadeImageAlpha(GameObject changeAlphaObject, float alpha)
    {
        if(changeAlphaObject.GetComponent<Image>() == false)
        { 
            Debug.LogWarning("ChangeImageAlpha�� GameObject�� �̹����� ���� ������Ʈ�� ���Խ��ϴ�!");
            return;   
        }

        Color color = changeAlphaObject.GetComponent<Image>().color;
        color.a = alpha; 
        changeAlphaObject.GetComponent<Image>().color = color;  
    }

    private void SetDark()
    {
        SetFadeEffect(true);
        SetHomeButtonInteractable(false);
        SetFadeImageAlpha(fadeImage, 1.0f);
    }

    private void SetLight()
    {
        GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName("Home"));
        SetEffectCanvas(false);
        SetFadeImageAlpha(fadeImage, 0.0f);
        SetHomeButtonInteractable(true);
    }

    private void SetD_dayWindow(bool mode, bool isNextDay)
    {
        if (mode == false)
        {
            D_dayWindow.SetActive(false);
            for (int i = 0; i < D_dayWindow.transform.childCount; i++)
            {
                D_dayWindow.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            D_dayWindow.SetActive(true);
            if (isNextDay == true)
            {
                SetD_dayTexts();
                nextDayWindow.SetActive(true);
            }
            else
            {
                Debug.LogWarning("�̱����Դϴ�!");
            }
        }
    }

    private void SetD_dayTexts() //�ϵ��ڵ�..
    {
        #region
        int thisDay = GameManager.instance.GetD_day() + 1; //���� ������ ���� �ڵ屸�� �� -1�� �����Ƿ� 1�� ������
        int maxDay = GameManager.instance.beforeMainStageSetD_day; 

        if (maxDay - thisDay - 1 > 0)
        {
            D_dayTexts.transform.GetChild(0).GetComponent<Text>().text = "D - " + (thisDay + 2);
        }
        else
        {
            D_dayTexts.transform.GetChild(0).GetComponent<Text>().text = "";
        }

        if (maxDay - thisDay > 0)
        {
            D_dayTexts.transform.GetChild(1).GetComponent<Text>().text = "D - " + (thisDay + 1);
        }
        else
        {
            D_dayTexts.transform.GetChild(1).GetComponent<Text>().text = "";
        }

        D_dayTexts.transform.GetChild(2).GetComponent<Text>().text = "D - " + thisDay;
        D_dayTexts.transform.GetChild(2).GetComponent<Text>().color = selectD_dayTextColor;

        if (thisDay - 1 == 0)
        {
            D_dayTexts.transform.GetChild(3).GetComponent<Text>().text = homeManager.D_dayTextName;
        }
        else if (thisDay - 1 < 0)
        {
            D_dayTexts.transform.GetChild(3).GetComponent<Text>().text = "";
        }
        else
        {
            D_dayTexts.transform.GetChild(3).GetComponent<Text>().text = "D - " + (thisDay - 1);
        }

        if (thisDay - 2 == 0)
        {
            D_dayTexts.transform.GetChild(4).GetComponent<Text>().text = homeManager.D_dayTextName;
        }
        else if (thisDay - 2 < 0)
        {
            D_dayTexts.transform.GetChild(4).GetComponent<Text>().text = "";
        }
        else
        {
            D_dayTexts.transform.GetChild(4).GetComponent<Text>().text = "D - " + (thisDay - 2);
        }
        #endregion
    }

    private void StartNextDayEffect()
    {
        StartCoroutine(NextDayEffect(2.0f));
    }

    IEnumerator NextDayEffect(float nextDayEffectTime)
    { 
        if(effectTime <= nextDayEffectTime * 0.25f)
        {
            effectTime += Time.deltaTime;
            yield return null;
            StartCoroutine(NextDayEffect(nextDayEffectTime));               
        }
        else
        { 
            EffectEnd();      
            for (int i = 0; i < D_dayTexts.transform.childCount; i++)
            {
                D_dayTexts.transform.GetChild(i).GetComponent<Text>().color = defaultD_dayTextColor;
            }
            StartCoroutine(NextDayMoveEffect(nextDayEffectTime, D_dayTexts.GetComponent<RectTransform>().anchoredPosition.y));
        }       
    }

    IEnumerator NextDayMoveEffect(float nextDayEffectTime, float saveYPos = 0.0f)
    { 
        if(effectTime <= 1)
        {
            effectTime += Time.deltaTime/(nextDayEffectTime * 0.5f);
            float yPos = Mathf.Lerp(saveYPos, saveYPos + 360.0f, effectTime);
            D_dayTexts.GetComponent<RectTransform>().anchoredPosition = new Vector2 (D_dayTexts.GetComponent<RectTransform>().anchoredPosition.x, yPos);   
            
            float circleR = 180.0f;
            float circleMoveTime = effectTime * 180.0f;
            for (int i = 0; i < nextDayRotationImage.Length; i++)
            {
                float circleRad = Mathf.Deg2Rad * (circleMoveTime + (i * (360.0f / nextDayRotationImage.Length)));
                float circleX = circleR * Mathf.Sin(circleRad);
                float circleY = circleR * Mathf.Cos(circleRad);
                nextDayRotationImage[i].GetComponent<RectTransform>().anchoredPosition = circleMoveCenter.GetComponent<RectTransform>().anchoredPosition + new Vector2(circleX, circleY);
                //YS[i].transform.rotation = Quaternion.Euler(0, 0, (deg + (i * (360 / objSize))) * -1);
            }

            yield return null;
            StartCoroutine(NextDayMoveEffect(nextDayEffectTime, saveYPos));               
        }
        else
        { 
            EffectEnd();     
            D_dayTexts.transform.GetChild(3).GetComponent<Text>().color = selectD_dayTextColor;
            StartCoroutine(NextDayMiddleDelay(nextDayEffectTime));
        }       
    }

    IEnumerator NextDayMiddleDelay(float nextDayEffectTime)
    {
        if(effectTime <= 1)
        {
            effectTime += Time.deltaTime/(nextDayEffectTime * 0.1f);
            yield return null;
            StartCoroutine(NextDayMiddleDelay(nextDayEffectTime));               
        }
        else
        { 
            EffectEnd();     
            StartCoroutine(NextDayCenterImageMoveEffect(nextDayEffectTime, nextDayCenterMask.GetComponent<RectTransform>().anchoredPosition.x));
        } 
    }

    IEnumerator NextDayCenterImageMoveEffect(float nextDayEffectTime, float saveXPos)
    { 
        if(effectTime <= 1)
        {
            effectTime += Time.deltaTime/(nextDayEffectTime * 0.25f);
            float xPos = Mathf.Lerp(saveXPos, 0.0f, effectTime);
            nextDayCenterMask.GetComponent<RectTransform>().anchoredPosition = new Vector2 (xPos, nextDayCenterMask.GetComponent<RectTransform>().anchoredPosition.y);  
            
            float circleR = 180.0f;
            float circleMoveTime = effectTime * 180.0f + 180.0f;
            for (int i = 0; i < nextDayRotationImage.Length; i++)
            {
                float circleRad = Mathf.Deg2Rad * (circleMoveTime + (i * (360.0f / nextDayRotationImage.Length)));
                float circleX = circleR * Mathf.Sin(circleRad);
                float circleY = circleR * Mathf.Cos(circleRad);
                nextDayRotationImage[i].GetComponent<RectTransform>().anchoredPosition = circleMoveCenter.GetComponent<RectTransform>().anchoredPosition + new Vector2(circleX, circleY);
                //YS[i].transform.rotation = Quaternion.Euler(0, 0, (deg + (i * (360 / objSize))) * -1);
            }

            yield return null;
            StartCoroutine(NextDayCenterImageMoveEffect(nextDayEffectTime, saveXPos));               
        }
        else
        { 
            EffectEnd();     
            StartCoroutine(NextDayEffectEndDelay(nextDayEffectTime));
        }       
    }

    IEnumerator NextDayEffectEndDelay(float nextDayEffectTime)
    { 
        if(effectTime <= nextDayEffectTime * 0.25f)
        {
            effectTime += Time.deltaTime;
            yield return null;
            StartCoroutine(NextDayEffectEndDelay(nextDayEffectTime));               
        }
        else
        { 
            EffectEnd();      
            SetLight();
        }       
    }

    private void EffectEnd()
    {
        effectTime = 0;
    }
    //--
    public void ManagerTalkEvent()
    { 
        switch(GameManager.instance.Var[GameManager.MAINSTAGEVARNUM].Var)
        { 
            
            case 0:
                GetRandomManagerEvent("Stage0");
                break;
             case 2:
                GetRandomManagerEvent("Stage2");
                break;           
            default:
                GetRandomManagerEvent("Default");
                break; 
        }    
    }

    private void GetRandomManagerEvent(string eventPath)
    { 
        TextAsset[] eventCSV;
        eventCSV = Resources.LoadAll<TextAsset>(ManagerTalkCSVPath + eventPath);

        Debug.Log("�ľǵ� �̺�Ʈ�� ���� " + eventCSV.Length);
        if(eventCSV.Length < 1)
        { 
            Debug.LogWarning("�̺�Ʈ�� �� ���� �������� �ʽ��ϴ�!");
            homeManager.SetTalkCanvas(false);
            return;
        }

        int eventNum = Random.Range(0, eventCSV.Length);
        Debug.Log("���õ� �̺�Ʈ�� " + eventCSV[eventNum].name);
        SetTalkData(ManagerTalkFolder + eventPath + "/" + eventCSV[eventNum].name, ""); 
        keyIgnoreCount = 1;
    }

    public void HomeEventCheck() //Ʃ�丮���� ���ؼ� HomeManager�� �ʱ�ȭ ���� �Ŀ� ������Ѽ� ���������� �̺�Ʈ üũ�� ���� ����ġ�� �̹� ������ �̺�Ʈ���� Ȯ�ε� ����
    { 
        SetDark();
        if (GameManager.instance.isNextDay == true)
        {
            GameManager.instance.isNextDay = false;
            GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName("NextDay"));
            GameManager.instance.ChangePlayBGMTime(120.0f);
            SetD_dayWindow(true, true);
            StartNextDayEffect();
        }        
        else if (GameManager.instance.isMainStageEnd == true)
        {
            GameManager.instance.isMainStageEnd = false;
            SetLight();

            switch(GameManager.instance.Var[GameManager.MAINSTAGEVARNUM].Var)
            { 
                case 0:
                    SetTalkData("HomeEvent/HomeTutorial", "");
                    break;
                case 1:
                    SetTalkData("HomeEvent/HomeTutorial2", "");
                    break;
                default:
                    break; 
            }    
        }
        else
        {
            SetLight();
        }
    }
}
