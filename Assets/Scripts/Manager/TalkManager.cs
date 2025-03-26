using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class Log
{ 
    public string talkerName;
    public string talkText;
}

public class TalkManager : MonoBehaviour
{
    List<Dictionary<string, object>> TalkData;
    [SerializeField]
    private GameObject[] talkerImage; // ���� �߾� ������ ������ 0 1 2
    private Talker[] talker; // ���� �߾� ������ ������ 0 1 2
    [SerializeField]
    private GameObject talkBackground; // ���
    [SerializeField]
    private GameObject talkWindow;
    [SerializeField]
    private GameObject talkerNameText; // ȭ�� �̸�
    [SerializeField]
    private GameObject talkText; // ��ȭ ����
    [SerializeField]
    private GameObject choiceButtons; // ������ �θ� ������Ʈ
    private GameObject[] choiceButton;// ������ ��������

    [SerializeField]
    private Color talkColor = new Color(255/255f, 255/255f, 255/255f, 255/255f);
    [SerializeField]
    private Color stayColor = new Color(100/255f, 100/255f, 100/255f, 255/255f);
    [SerializeField]
    private Vector3 talkScale = new Vector3(1.0f, 1.0f, 1.0f);
    [SerializeField]
    private Vector3 stayScale = new Vector3(1.0f, 1.0f, 1.0f);

    [SerializeField]
    private GameObject systemButton;
    [SerializeField]
    private GameObject talkEndEffect;

    private int CSVNum;
    private bool keyCheck;
    private bool isTalk;
    private bool canPass;
    private bool isTyping;
    private bool isChoice;
    private bool isSkipButton;
    private bool isTalkAutoPlay; //�ڵ������
    private bool isTalkAutoPlayDelay; 
    private float autoPlayDelayTime = 1.0f;
    //---- ����Ʈ��-----
    private bool isEffect;

    [SerializeField]
    private GameObject effectCanvas; 
    [SerializeField]
    private GameObject effectImage;   

    private int effectCanvasOriginalSortOrder = 5;
    private int effectCanvasUnderTalkCanvasSortOrder = 1;

    private float effectTime = 0;
    //-------------------

    private string choiceKey;

    private float typingSpeed = 0.1f; //��ȭ ���ǵ�
    [SerializeField]
    private float defaultTalkSpeed = 0.1f; // ����Ʈ ��ȭ ���ǵ�
    [SerializeField]
    private char lineBreakForTalk = '#'; //���κ��� �ؽ�Ʈ

    private string[] selectChoiceKey;

    //---�α׿�
    private bool isLog = false;
    private Log[] log = new Log[20];
    private int logLength = 20;
    private int logNum = 0;
    [SerializeField]
    private GameObject logCanvas;
    [SerializeField]
    private GameObject logScrollBar;
    [SerializeField]
    private GameObject logCanvasScrollViewContent;
    [SerializeField]
    private GameObject logPrefab;

    private string systemLog = ""; // ���� �����
    //---
    //-- �ƾ���
    [SerializeField]
    private GameObject cutsceneImage;
    //--
    void Start()
    { 
        InitializeTalkManager();
    }

    void Update()
    { 
        AutoPlayCheck();
        TalkKeyCheck();   
    }

    private void AutoPlayCheck()
    {
        if (CantTalkCheck() == false && isTalkAutoPlay == true)
        {
            isTalkAutoPlayDelay = true;
            Invoke("AutoPlay", autoPlayDelayTime);
        }
    }

    private void TalkKeyCheck()
    {
        if((GameManager.instance.KeyCheckAccept() == true) || (GameManager.instance.KeyCheckSkip() == true))
        { 
            if(isTyping == true)
            { 
                isTyping = false;
            }
            else
            { 
                if(CantTalkCheck() == false)
                { 
                    PrepareNextTalk();
                }    
            }          
        }   
    }

    private void AutoPlay()
    {
        isTalkAutoPlayDelay = false;
        PrepareNextTalk();
    }

    private void SetSystemButton(bool mode)
    { 
        systemButton.SetActive(mode);
    }

    private void InitializeTalkManager()
    { 
        #region
        talker = new Talker[talkerImage.Length];
        choiceButton = new GameObject[choiceButtons.transform.childCount];
        for(int i = 0; i < choiceButtons.transform.childCount; i++)
        { 
            choiceButton[i] = choiceButtons.transform.GetChild(i).gameObject;
        }
        selectChoiceKey = new string[choiceButton.Length];

        GameManager.instance.talkManager = this;

        log = new Log[logLength];
        for(int i = 0; i < log.Length; i++)
        { 
            log[i] = new Log();
        }
        systemLog = "";
        TalkData = CSVReader.Read(GameManager.instance.CSVFolder + GameManager.instance.CSVName, GameManager.instance.CSVPart);
        CSVNum = 0;
        ResetTalkKey();
        Debug.Log("�뺻�� ���̴� " + TalkData.Count);
        NextTalk(); 
        #endregion
    }

    public void PrepareNextTalk()
    { 
        if (isSkipButton == false)
        {
            IncreaseCSVNUM();
            ClearTalkText();
            talkerNameText.GetComponent<Text>().text = "";
            SetSystemButton(true); //�ӽ� �߰� 
            NextTalk();          
        }
  
    }

    private bool CantTalkCheck()
    { 
        if(isTalk == true || isTyping== true || isChoice == true || isEffect == true || isSkipButton == true || isLog == true || isTalkAutoPlayDelay == true)
        { 
            return true;
        }
        else
        { 
            return false;
        }        
    }

    private void IncreaseCSVNUM()
    { 
        CSVNum++;
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
            case "ChangeBackground":
                ChangeBackground(num);
                return true;                
            case "GoNextBattleScene":
                GoNextBattleScene();
                return true;
            case "GoHomeScene":
                GoHomeScene();
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
            case "SetTimer":
                SetTimer(num);
                return true;
            case "FadeOut":
                FadeOut(num);
                return true;
            case "FadeIn":
                FadeIn(num);
                return true;
            case "SetCutscene":
                SetCutscene(num);
                return true;
            case "StayPressKey":
                StayPressKey();
                return true;                
            case "CutsceneEnd":
                CutsceneEnd();
                return true;
            case "SetTalkWindowActiveFalse":
                SetTalkWindowActiveFalse();
                return true;
            default:
                return false;
        }       
    }

    private void Talk(int num)
    { 
        #region
        bool isTalkerCheckFail = false;
        int talkerNum = -1;

        ClearTalkText();
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

        //�ӽÿ�
        if (talkerNameText.GetComponent<Text>().text == "")
        {
            talkerNameText.GetComponent<Text>().text = "�����̼�";
        }

        //
        talkWindow.SetActive(true);

        StartCoroutine(Typing(TalkData[num]["Content"].ToString(), typingSpeed));

        SetTalkLog(TalkData[num]["Content"].ToString()); //�α�
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

    private void ClearTalkText()
    {
        SetTalkEndEffect(false);
        talkText.GetComponent<Text>().text = "";
    }

    public void SetTalkAutoPlay(bool mode)
    {
        isTalkAutoPlay = mode;
    }

    private void SetTalkEndEffect(bool mode)
    {
        talkEndEffect.SetActive(mode);
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
                    talkText.GetComponent<Text>().text += "\n";
                }
                else
                { 
                    talkText.GetComponent<Text>().text += word[i];
                }
                yield return new WaitForSeconds(talkSpeed);

                if(isTyping == false)
                {                    
                    ClearTalkText();
                    for(int j = 0; j < word.Length; j++)
                    {
                        if(word[j] == lineBreakForTalk)
                        { 
                            talkText.GetComponent<Text>().text += "\n";
                        }
                        else
                        { 
                            talkText.GetComponent<Text>().text += word[j];
                        }
                    }
                    isTalk= false;
                    Debug.Log("���ϱ� ��ŵ");
                    break;                   
                }                
            }
            SetTalkEndEffect(true);
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
        for(int i = 0; i < selectChoiceKey.Length; i++)
        { 
            selectChoiceKey[i] = "";
        }       
        Debug.Log("Ű(������)������ �ʱ�ȭ �Ǿ����ϴ�");      
        #endregion
    }

    private void ChangeBackground(int num)
    { 
        if (BackgroundDatabaseManager.instance.GetBackgroundWithAnimationByName(TalkData[num]["Content"].ToString()).backgroundAnimator != null)
        {
            talkBackground.GetComponent<Image>().sprite = BackgroundDatabaseManager.instance.GetBackgroundWithAnimationByName(TalkData[num]["Content"].ToString()).backgroundImage;
            talkBackground.GetComponent<Animator>().runtimeAnimatorController = BackgroundDatabaseManager.instance.GetBackgroundWithAnimationByName(TalkData[num]["Content"].ToString()).backgroundAnimator;
        }
        else
        {
            talkBackground.GetComponent<Image>().sprite = BackgroundDatabaseManager.instance.GetBackgroundByName(TalkData[num]["Content"].ToString());
            talkBackground.GetComponent<Animator>().runtimeAnimatorController = null;
        }        
        PrepareNextTalk();               
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
                
                if (mode == true)
                {
                    AddSystemLog(CharacterDatabaseManager.instance.GetPlayerCharacter(i).inGameName + "��(��) ���� ���Դ�!");
                }
                else
                {
                    AddSystemLog(CharacterDatabaseManager.instance.GetPlayerCharacter(i).inGameName + "��(��) ������ ������");
                }
                break;
            }
        }
        PrepareNextTalk();
        #endregion
    }

    private void GoNextBattleScene()
    { 
        GameManager.instance.GoNextBattleScene();
    }

    private void GoHomeScene()
    { 
        GameManager.instance.GoHomeScene();
    }

    private void StayPressKey()
    {
        //���Ƿ� ��ĭ ���а�
        SetSystemButton(false); //�ӽ��߰�
    }

    private void SetTalkWindowActiveFalse()
    {
        talkWindow.SetActive(false);
        PrepareNextTalk(); 
    }

    // ��ŵ��ư��
    public void SkipButton()
    { 
        isSkipButton = true;
        CantEventCheck(CSVNum);
    }

    private void CantEventCheck(int num)
    { 
        switch(TalkData[num]["Event"].ToString())
        {         
            case "GoNextBattleScene":
                GoNextBattleScene();
                break;
            case "GoHomeScene":
                GoHomeScene();
                break;
            case "ChangeBGM":
                ChangeBGM(num);
                break;
            case "StartBGM":
                StartBGM();
                break;
            case "StopBGM":
                StopBGM();
                break;
            case "ChangeUseableCharacter":
                ChangeUseableCharacter(num);
                break;
            default:
                break;
        }   

        CSVNum++;
        if(CSVNum < TalkData.Count)
        {             
            CantEventCheck(CSVNum);
        }
        
    }

    // �α׿�
    #region
    private void SetTalkLog(string talkText)
    {
        logNum = logNum + 1;
        if(logNum >= log.Length)
        { 
           logNum = 0; 
        }

        log[logNum].talkerName = talkerNameText.GetComponent<Text>().text;
        log[logNum].talkText = talkText;
    }

    private void CreateLog() 
    { 
        #region
        if(logPrefab == null)
        { 
            Debug.LogWarning("�α� �������� �����ϴ�!");
            return;
        }

        int logCheck = logNum;

        for (int i = 0; i < log.Length; i++)
        {
            if(log[logCheck].talkText != "" && log[logCheck].talkText != null)
            {
                GameObject newLog = Instantiate(logPrefab);
                newLog.transform.SetParent(logCanvasScrollViewContent.transform);
                newLog.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                newLog.transform.GetChild(0).GetComponent<Text>().text = "";
                newLog.transform.GetChild(1).GetComponent<Text>().text = "";

                if(log[logCheck].talkerName != "")
                {                     
                    newLog.transform.GetChild(0).GetComponent<Text>().text = log[logCheck].talkerName + " : ";
                }

                char[] logText =  log[logCheck].talkText.ToCharArray();
                { 
                    for(int j = 0; j < logText.Length; j++)
                    {
                        if(logText[j] != lineBreakForTalk)
                        { 
                            newLog.transform.GetChild(1).GetComponent<Text>().text += logText[j];
                        }
                        else
                        { 
                            newLog.transform.GetChild(1).GetComponent<Text>().text += "\n";
                        }
                    }
                }
                 
                newLog.name = "Log_" + i;     
            }
                       
            logCheck--;
            if(logCheck < 0)
            { 
                logCheck = log.Length -1;
            }                     
        }    
        #endregion
    }

    private void DeleteLog()
    { 
        for(int i = 0; i < logCanvasScrollViewContent.transform.childCount; i++)
        { 
            Destroy(logCanvasScrollViewContent.transform.GetChild(i).gameObject);
        }        
    }

    public void SetLogCanvas(bool mode)
    { 
        isLog = mode;    
        logCanvas.SetActive(mode);
        if(mode == true)
        { 
            SetSystemButton(false);
            CreateLog();
            logScrollBar.GetComponent<Scrollbar>().value = 1.0f;
            logScrollBar.GetComponent<Scrollbar>().value = 0.0f;          
        }
        else
        { 
            SetSystemButton(true);
            DeleteLog();
            logScrollBar.GetComponent<Scrollbar>().value = 1.0f;
        }
    }
    #endregion
    //---

    // ����Ʈ��
    #region
    private void EffectStart()
    { 
        Debug.Log("����Ʈ ����");
        SetSystemButton(false);
        isEffect = true;  
        effectTime = 0;
        effectCanvas.GetComponent<Canvas>().sortingOrder = effectCanvasOriginalSortOrder;
        effectCanvas.SetActive(true);
    }

    private void EffectEnd()
    { 
        Debug.Log("����Ʈ ��");
        SetSystemButton(true);
        effectTime = 0;
        isEffect = false;
        effectImage.GetComponent<Image>().sprite = GameManager.instance.GetWhite(); 
        PrepareNextTalk(); 
    }   
    
    private void ResetEffect()
    { 
        effectCanvas.SetActive(false);
        ChangeImageColor(effectImage, "White");
        ChangeImageAlpha(effectImage, 0);
        effectImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,0);
        effectCanvas.GetComponent<Canvas>().sortingOrder = effectCanvasOriginalSortOrder;          
    }

    private void SetTimer(int num)
    { 
        EffectStart();
        float time = float.Parse(TalkData[num]["Content"].ToString()); 
        StartCoroutine(Timer(time));
    }

    IEnumerator Timer(float time)
    { 
        if(effectTime <= time)
        {
            effectTime += Time.deltaTime;
            yield return null;
            StartCoroutine(Timer(time));               
        }
        else
        { 
             EffectEnd();      
        }       
    }

    private void FadeOut(int num)
    { 
        Debug.Log("fadeOut");
        EffectStart();

        float time = float.Parse(TalkData[num]["Content"].ToString()); 
        string color = TalkData[CSVNum]["Talker"].ToString();
        if(color == "")
        { 
            ChangeImageColor(effectImage, "Black");
        }
        else
        { 
            ChangeImageColor(effectImage, color);
        }

        ChangeImageAlpha(effectImage, 0);
        StartCoroutine(FadeOutEffect(time));
    }

    IEnumerator FadeOutEffect(float time)
    { 
        if(effectTime <= 1)
        {
            effectTime += Time.deltaTime/time;
            float alpha = Mathf.Lerp(0, 1, effectTime); 
            ChangeImageAlpha(effectImage, alpha);

            yield return null;
            StartCoroutine(FadeOutEffect(time));               
        }
        else
        { 
            effectCanvas.GetComponent<Canvas>().sortingOrder = effectCanvasUnderTalkCanvasSortOrder;
            EffectEnd();      
        }       
    }

    private void FadeIn(int num)
    { 
        Debug.Log("fadeIn");
        EffectStart();
        float time = float.Parse(TalkData[num]["Content"].ToString());
        StartCoroutine(FadeInEffect(time)); 
    }

    IEnumerator FadeInEffect(float time)
    { 
        if(effectTime <= 1)
        {
            effectTime += Time.deltaTime/time;
            float alpha = Mathf.Lerp(1, 0, effectTime); 
            ChangeImageAlpha(effectImage, alpha);

            yield return null;
            StartCoroutine(FadeInEffect(time));               
        }
        else
        { 
            ResetEffect();
            EffectEnd();      
        }       
    }

    private void ChangeImageAlpha(GameObject changeAlphaObject, float alpha)
    { 
        #region
        if(changeAlphaObject.GetComponent<Image>() == false)
        { 
            Debug.LogWarning("ChangeImageAlpha�� GameObject�� �̹����� ���� ������Ʈ�� ���Խ��ϴ�!");
            return;   
        }

        Color color = changeAlphaObject.GetComponent<Image>().color;
        color.a = alpha; 
        changeAlphaObject.GetComponent<Image>().color = color;    
        #endregion
    }

    private void ChangeImageColor(GameObject changeColorObject, string ChangeColor)
    { 
        #region
        if(changeColorObject.GetComponent<Image>() == false)
        { 
            Debug.LogWarning("ChangeEffectImageColor�� GameObject�� �̹����� ���� ������Ʈ�� ���Խ��ϴ�!");
            return;   
        }

        if(ChangeColor == "Red")
        { 
            changeColorObject.GetComponent<Image>().color = new Color(1, 0, 0, changeColorObject.GetComponent<Image>().color.a);

        }
        else if(ChangeColor == "Black")
        { 
            changeColorObject.GetComponent<Image>().color = new Color(0, 0, 0, changeColorObject.GetComponent<Image>().color.a);

        }
        else if(ChangeColor == "White")
        { 
            changeColorObject.GetComponent<Image>().color = new Color(1, 1, 1, changeColorObject.GetComponent<Image>().color.a);
        }
        else if(ChangeColor == "Blue")
        { 
            changeColorObject.GetComponent<Image>().color = new Color(0, 0, 1, changeColorObject.GetComponent<Image>().color.a);
        }
        else if(ChangeColor == "Green")
        { 
            changeColorObject.GetComponent<Image>().color = new Color(0, 1, 0, changeColorObject.GetComponent<Image>().color.a);
        }
        else if(ChangeColor == "Yellow")
        { 
            changeColorObject.GetComponent<Image>().color = new Color(1, 0.92f, 0.016f, changeColorObject.GetComponent<Image>().color.a);
        }
        else
        {
            Color color;

            ColorUtility.TryParseHtmlString("#" + ChangeColor, out color);
            changeColorObject.GetComponent<Image>().color = color;                   
        }     
        #endregion
    }

    private void AddSystemLog(string text)
    {
        systemLog = systemLog + text + "\n";
    }
    #endregion
    //--

    // �ƾ���
    private void SetCutscene(int num)
    {
        talkWindow.SetActive(false);
        cutsceneImage.GetComponent<Image>().sprite = BackgroundDatabaseManager.instance.GetCutsceneByName(TalkData[num]["Content"].ToString());
        cutsceneImage.SetActive(true);
        PrepareNextTalk(); 
    }

    private void CutsceneEnd()
    {
        cutsceneImage.SetActive(false);
        PrepareNextTalk(); 
    }
}
