using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEventManager : MonoBehaviour
{
    [HideInInspector]
    public BattleManager battleManager;

    List<Dictionary<string, object>> TalkData;
    [SerializeField]
    private GameObject talkCanvas;
    [SerializeField]
    private GameObject talkerImage;
    private Talker talker;
    private GameObject talkerObject;
    [SerializeField]
    private GameObject talkWindow;
    [SerializeField]
    private GameObject talkerNameText; // 화자 이름
    [SerializeField]
    private GameObject talkText; // 대화 내용
    private string HomeCSVPath;
    private int CSVNum;

    private bool isTalk;
    private bool isTyping;
    private bool keyCheck;
    private string choiceKey;

    private float typingSpeed = 0.1f; //대화 스피드
    [SerializeField]
    private float defaultTalkSpeed = 0.06f; // 디폴트 대화 스피드
    [SerializeField]
    private char lineBreakForTalk = '#'; //라인변경 텍스트

    private float effectTime = 0.0f;
    private bool isEffect = false;

    private Vector3 isPlayerTalkerImagePosition = new Vector3(0,0,0);
    private Vector3 isPlayerTalkWindowPosition = new Vector3(0,0,0);
    private Vector3 isEnemyTalkerImagePosition = new Vector3(1416,0,0);
    private Vector3 isEnemyTalkWindowPosition = new Vector3(-420,0,0);

    void Start()
    {
        InitializeBattleEventManager();
    }

    void Update()
    { 
        KeyCheck();
    }

    private void KeyCheck()
    { 
        if(battleManager.nowPhase ==  BattlePhase.IsEvent)
        { 
            if((GameManager.instance.KeyCheckAccept() == true) || (GameManager.instance.KeyCheckSkip() == true))
            { 
                NextTalkButton();
            } 
        }        
    }

    public void NextTalkButton()
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

    private void InitializeBattleEventManager()
    { 
        #region
        battleManager = this.gameObject.GetComponent<BattleManager>();
        HomeCSVPath = GameManager.instance.CSVFolder + "Battle/";

        ClearTalkCanvas();
        #endregion
    }

    public void SetTalkData(string name, string part)
    { 
        ClearTalkCanvas();
        TalkData = CSVReader.Read(HomeCSVPath + name, part);
        Debug.Log("대본의 길이는 " + TalkData.Count);
        NextTalk();
    }

    public void SetTalkCanvas(bool mode)
    { 
        ClearTalkCanvas();
        talkCanvas.SetActive(mode);
        
    }

    private void ClearTalkCanvas()
    {
        #region
        talkerImage.GetComponent<Image>().sprite = GameManager.instance.GetBlank();
        talkerObject = null;
        talker = null;
        talkerImage.GetComponent<RectTransform>().anchoredPosition = isPlayerTalkerImagePosition;
        talkWindow.GetComponent<RectTransform>().anchoredPosition = isPlayerTalkWindowPosition;
        ResetTalkKey();
        CSVNum = 0;
        if(TalkData != null)
        { 
            TalkData.Clear();
        }
        #endregion
    }

    //-- 대화이벤트용
    #region
    private bool CantTalkCheck()
    { 
        if(isTalk == true || isTyping == true || isEffect == true)
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
                    Debug.Log("KeyCheck조건이 걸려있습니다.");
                    if(TalkData[CSVNum]["Branch"].ToString() == choiceKey)
                    {
                        Debug.Log("Branch = choiceKey임으로 내용을 출력합니다.");
                        NextTalkCheck();
                    }
                    else
                    {
                        Debug.Log("Branch != choiceKey임으로 내용을 스킵합니다.");
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
            EventEnd();
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
            case "CameraMoveToTarget":
                CameraMoveToTarget(num);
                return true;
            case "CameraMoveToBall":
                CameraMoveToBall();
                return true;
            case"SetTimer":
                SetTimer(num);
                return true;
            case "CameraMoveToPlayerTargetByNum":
                CameraMoveToTargetByNum(num, false);
                return true;
            case "CameraMoveToEnemyTargetByNum":
                CameraMoveToTargetByNum(num, true); //
                return true;
            case "SetEffectByName":
                SetEffectByName(num);
                return true;
            case "SetAllPlayerCharacterEffect":
                SetAllPlayerCharacterEffect(num);
                return true;
            case "SetAllEnemyCharacterEffect":
                SetAllEnemyCharacterEffect(num);
                return true;
            case "CameraMoveToStrategicPoint":
                CameraMoveToStrategicPoint(num);
                return true;              
            default:
                return false;
        }       
    }

    private void Talk(int num)
    { 
        #region
        talkText.GetComponent<Text>().text = "";
        isTalk = true;
        talkerNameText.GetComponent<Text>().text = "";
        AddTalker(num);
        talkCanvas.SetActive(true);
        StartCoroutine(Typing(TalkData[num]["Content"].ToString(), typingSpeed));
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
                    talkText.GetComponent<Text>().text += "\n";
                }
                else
                { 
                    talkText.GetComponent<Text>().text += word[i];
                }
                yield return new WaitForSeconds(talkSpeed);

                if(isTyping == false)
                {                    
                    talkText.GetComponent<Text>().text = "";
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
                    Debug.Log("말하기 스킵");
                    break;                   
                }                
            }
            isTyping = false;
            isTalk= false;
        }
        #endregion
    }

    private void AddTalker(int num)
    { 
        #region
        battleManager.battleCharacterManager.AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Target); //타일 초기화
        bool isTalkerInBattle = false;
        talkerObject = null;
        talker = null;

        string name = TalkData[num]["Talker"].ToString();

        int changeFaceNum = 0;
        if(TalkData[num]["Key"].ToString() != "")
        { 
            int.TryParse(TalkData[num]["Key"].ToString(), out changeFaceNum);
        }

        //데이터 받고 얼굴 설정
        int talkDataCheck = 0;

        for(int i = 0; i < TalkerDatabaseManager.instance.DBLength(); i++)
        { 
            if(TalkerDatabaseManager.instance.TalkerDB(i).callName == name)
            { 
                talker = TalkerDatabaseManager.instance.TalkerDB(i);
                if(changeFaceNum < talker.characterImage.Length)
                { 
                    talkerImage.GetComponent<Image>().sprite = talker.characterImage[changeFaceNum];
                }
                else
                { 
                    if(talker.characterImage.Length > 0)
                    { 
                        talkerImage.GetComponent<Image>().sprite = talker.characterImage[0];
                    }
                    else
                    { 
                        talkerImage.GetComponent<Image>().sprite  = GameManager.instance.GetDefaultTalkerImage();
                    }            
                }
                break;                
            }
            else
            {
                talkDataCheck++;
            }
        } 

        if(talkDataCheck >= TalkerDatabaseManager.instance.DBLength())
        {
            talkerImage.GetComponent<Image>().sprite  = GameManager.instance.GetDefaultTalkerImage();
        }

        //이름하고 대화스피드 설정
        if(talker != null)
        { 
            talkerNameText.GetComponent<Text>().text = talker.inGameName;   

            if(talker.talkSpeed > 0)
            { 
                typingSpeed = talker.talkSpeed;
            }  
            else
            { 
                typingSpeed = defaultTalkSpeed;
            }
        }
        else
        { 
            talkerNameText.GetComponent<Text>().text = name;
            typingSpeed = defaultTalkSpeed;
        }

        for(int i = 0; i < battleManager.playerCharacters.Length; i++)
        { 
            if(name == battleManager.playerCharacters[i].GetComponent<Character>().status.name)
            {     
                talkerObject = battleManager.playerCharacters[i];
                isTalkerInBattle = true;
                break;
            }
        }

        for(int i = 0; i < battleManager.enemyCharacters.Length; i++)
        { 
            if(name == battleManager.enemyCharacters[i].GetComponent<Character>().status.name)
            {     
                talkerObject = battleManager.enemyCharacters[i];
                isTalkerInBattle = true;
                break;
            }
        }

        if((isTalkerInBattle == true) && (talkerObject != null))
        { 
            if(talkerObject.GetComponent<Character>().isEnemy == true)
            { 
                talkerImage.GetComponent<RectTransform>().anchoredPosition = isEnemyTalkerImagePosition;
                talkWindow.GetComponent<RectTransform>().anchoredPosition = isEnemyTalkWindowPosition;                
            }
            else
            { 
                talkerImage.GetComponent<RectTransform>().anchoredPosition = isPlayerTalkerImagePosition;
                talkWindow.GetComponent<RectTransform>().anchoredPosition = isPlayerTalkWindowPosition;
            }

            RaycastHit2D[] hits; //타일 빛나게 하기
            hits = Physics2D.RaycastAll(new Vector2 ((int)talkerObject.transform.position.x, (int)talkerObject.transform.position.y), Vector2.zero);
            for(int i = 0; i < hits.Length; i++)
            { 
                if(hits[i].transform.tag == "Tile")
                {
                    hits[i].transform.GetComponent<MapBlock>().SetSelectionMode(MapBlock.Highlight.Target);
                    break;
                }
            }
            talkerNameText.GetComponent<Text>().text = talkerObject.GetComponent<Character>().status.inGameName;
            GameManager.instance.SetCameraPosition(talkerObject); //카메라 이동
        }
        else
        { 
            talkerImage.GetComponent<RectTransform>().anchoredPosition = isPlayerTalkerImagePosition;
            talkWindow.GetComponent<RectTransform>().anchoredPosition = isPlayerTalkWindowPosition;
        }
        #endregion
    }

    private void CameraMoveToTarget(int num)
    { 
        #region
        talkCanvas.SetActive(false);
        GameObject targetObject = null;
        string name = TalkData[num]["Content"].ToString();

        if(name == "PlayerLeader")
        {
            targetObject = battleManager.playerCharacters[GameManager.instance.playerLeaderNum];
        }
        else if(name == "EnemyLeader")
        { 
            targetObject = battleManager.enemyCharacters[GameManager.instance.enemyLeaderNum];
        }
        else
        { 
            for (int i = 0; i < battleManager.playerCharacters.Length; i++)
            { 
                if(name == battleManager.playerCharacters[i].GetComponent<Character>().status.name)
                {     
                    targetObject = battleManager.playerCharacters[i];
                    break;
                }
            }

            for(int i = 0; i < battleManager.enemyCharacters.Length; i++)
            { 
                if(name == battleManager.enemyCharacters[i].GetComponent<Character>().status.name)
                {     
                    targetObject = battleManager.enemyCharacters[i];
                    break;
                }
            }
        }

        if(targetObject != null)
        { 
            battleManager.battleCharacterManager.AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Target); //타일 초기화
            RaycastHit2D[] hits; //타일 빛나게 하기
            hits = Physics2D.RaycastAll(new Vector2 ((int)targetObject.transform.position.x, (int)targetObject.transform.position.y), Vector2.zero);
            for(int i = 0; i < hits.Length; i++)
            { 
                if(hits[i].transform.tag == "Tile")
                {
                    hits[i].transform.GetComponent<MapBlock>().SetSelectionMode(MapBlock.Highlight.Target);
                    break;
                }
            }

            GameManager.instance.SetCameraPosition(targetObject); 
        }

        PrepareNextTalk();
        #endregion
    }

    private void CameraMoveToTargetByNum(int num, bool isEnemy)
    { 
        talkCanvas.SetActive(false);
        GameObject targetObject = null;
        int characterNum = 0;
        int.TryParse(TalkData[num]["Content"].ToString(), out characterNum);   
        
        if((isEnemy == true) && (characterNum < battleManager.enemyCharacters.Length))
        { 
            targetObject = battleManager.enemyCharacters[characterNum];
        }
        else if((isEnemy == false) && (characterNum < battleManager.enemyCharacters.Length))
        { 
            targetObject = battleManager.playerCharacters[characterNum];
        }
        else
        { 
            Debug.LogWarning("characterNum에 잘못 된 값이 들어왔습니다!");    
        }
    }

    private void CameraMoveToBall()
    { 
        #region
        talkCanvas.SetActive(false);
        GameObject targetObject = GameObject.Find("Ball");

        if(targetObject != null)
        { 
            battleManager.battleCharacterManager.AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Target); //타일 초기화
            RaycastHit2D[] hits; //타일 빛나게 하기
            hits = Physics2D.RaycastAll(new Vector2 ((int)targetObject.transform.position.x, (int)targetObject.transform.position.y), Vector2.zero);
            for(int i = 0; i < hits.Length; i++)
            { 
                if(hits[i].transform.tag == "Tile")
                {
                    hits[i].transform.GetComponent<MapBlock>().SetSelectionMode(MapBlock.Highlight.Target);
                    break;
                }
            }

            GameManager.instance.SetCameraPosition(targetObject); 
        }

        PrepareNextTalk(); 
        #endregion
    }

    private void CameraMoveToStrategicPoint(int num)
    { 
        #region
        talkCanvas.SetActive(false);

        int strategicPointNum = 0;
        int.TryParse(TalkData[num]["Content"].ToString(), out strategicPointNum); 
        MapBlock targetMapBlock = null;

        if ((0 <= strategicPointNum) && (strategicPointNum < battleManager.strategicPointMapBlocks.Count))
        {
            targetMapBlock = battleManager.strategicPointMapBlocks[strategicPointNum];
        }

        if(targetMapBlock != null)
        { 
            battleManager.battleCharacterManager.AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Target); //타일 초기화
            targetMapBlock.SetSelectionMode(MapBlock.Highlight.Target);

            GameManager.instance.SetCameraPosition(targetMapBlock.gameObject); 
        }

        PrepareNextTalk(); 
        #endregion
    }

    private void SetTimer(int num)
    { 
        EffectStart();
        float time = float.Parse(TalkData[num]["Content"].ToString()); 
        StartCoroutine(Timer(time));
    }

    private IEnumerator Timer(float time)
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

    private void EffectStart()
    { 
        Debug.Log("이펙트 시작");
        isEffect = true;  
        effectTime = 0;
        talkCanvas.SetActive(false);
        /*
        effectCanvas.GetComponent<Canvas>().sortingOrder = effectCanvasOriginalSortOrder;
        effectCanvas.SetActive(true);
        */
    }

    private void EffectEnd()
    { 
        Debug.Log("이펙트 끝");
        effectTime = 0;
        isEffect = false;
        talkCanvas.SetActive(true);
        PrepareNextTalk();
        /*
        effectImage.GetComponent<Image>().sprite = GameManager.instance.GetWhite(); 
         
        */
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

    private void ResetKey()
    { 
        ResetTalkKey();
        //PrepareNextTalk();        
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
        Debug.Log("키(선택지)설정이 초기화 되었습니다");      
        #endregion
    }

    private void SetEffectByName(int num)
    {
        #region
        talkCanvas.SetActive(false);
        Character effectTargetCharacter = null;
        string name = TalkData[num]["Talker"].ToString();

        if(name == "PlayerLeader")
        {
            effectTargetCharacter = battleManager.playerCharacters[GameManager.instance.playerLeaderNum].GetComponent<Character>();
        }
        else if(name == "EnemyLeader")
        { 
            effectTargetCharacter = battleManager.enemyCharacters[GameManager.instance.enemyLeaderNum].GetComponent<Character>();
        }
        else
        { 
            for (int i = 0; i < battleManager.playerCharacters.Length; i++)
            { 
                if(name == battleManager.playerCharacters[i].GetComponent<Character>().status.name)
                {     
                    effectTargetCharacter = battleManager.playerCharacters[i].GetComponent<Character>();
                    break;
                }
            }

            for(int i = 0; i < battleManager.enemyCharacters.Length; i++)
            { 
                if(name == battleManager.enemyCharacters[i].GetComponent<Character>().status.name)
                {     
                    effectTargetCharacter = battleManager.enemyCharacters[i].GetComponent<Character>();
                    break;
                }
            }
        }

        if(effectTargetCharacter != null)
        {
            int effectNum = 0;
            int.TryParse(TalkData[num]["Content"].ToString(), out effectNum);

            effectTargetCharacter.StartEffect(EffectDatabaseManager.instance.GetEffect(effectNum));
        }
        PrepareNextTalk();  
        #endregion
    }

    private void SetAllPlayerCharacterEffect(int num)
    {
        talkCanvas.SetActive(false);
        int effectNum = 0;
        int.TryParse(TalkData[num]["Content"].ToString(), out effectNum); 

        for (int i = 0; i < battleManager.playerCharacters.Length; i++)
        {
            battleManager.playerCharacters[i].GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(effectNum));        
        }
        PrepareNextTalk();  
    }

    private void SetAllEnemyCharacterEffect(int num)
    {
        talkCanvas.SetActive(false);
        int effectNum = 0;
        int.TryParse(TalkData[num]["Content"].ToString(), out effectNum); 

        for (int i = 0; i < battleManager.enemyCharacters.Length; i++)
        {
            battleManager.enemyCharacters[i].GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(effectNum));        
        }
        PrepareNextTalk();  
    }

    #endregion
    //--이벤트용
    #region
    public bool BattleEventCheck() //false 반환이면 이벤트 끝난다음 턴로그 진행하게 할 예정 // 첫턴은 언제나 1이고 현재는 첫 전투가 게임매니저 var 1임
    { 
        bool isEvent = false;
        if (GameManager.instance.isMainStage == true)
        {
            switch(GameManager.instance.Var[GameManager.MAINSTAGEVARNUM].Var)
            { 
                case 1:
                    isEvent = StageFirstEventCheck();
                    break;
                case 2:
                    isEvent = StageSecondEventCheck();
                    break;
                case 3:
                    isEvent = StageThirdEventCheck();
                    break;
                default:
                    isEvent = false;
                    break;
            }
        }

        return isEvent;
    }

    private bool StageFirstEventCheck()
    { 
        bool isEvent = false;

        switch(battleManager.battleTurnCount)
        { 
            case 1:
                isEvent = true; 
                StageFirstEvent0();
                break;
            default:
                isEvent = false;
                break;
        }     
        
        return isEvent;
    }

    private bool StageSecondEventCheck()
    { 
        bool isEvent = false;

        switch(battleManager.battleTurnCount)
        { 
            case 1:
                isEvent = true; 
                StageSecondEvent0();
                break;
            case 3:
                isEvent = true; 
                StageSecondEvent1();
                break;
            case 4:
                isEvent = true; 
                StageSecondEvent2();
                break;
            default:
                isEvent = false;
                break;
        }     
        
        return isEvent;
    }

    private bool StageThirdEventCheck()
    { 
        bool isEvent = false;

        switch(battleManager.battleTurnCount)
        { 
            case 1:
                isEvent = true; 
                StageThirdEvent0();
                break;
            default:
                isEvent = false;
                break;
        }     
        
        return isEvent;
    }

    private void EventEnd()
    { 
        SetTalkCanvas(false);
        battleManager.battleCharacterManager.AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Target); //타일 초기화
        battleManager.battlePhaseManager.StartTurnEffect(false); //구조상 페이즈 돌리는건 저 함수 실행후 페이즈매니저에서 알아서 함
        battleManager.SetBattleSceneHinderUI(true);
    }

    private void EventStart()
    { 
        battleManager.SetBattleSceneHinderUI(false);
        battleManager.battlePhaseManager.ChangePhase(BattlePhase.IsEvent);
        battleManager.battleGetInformationManager.ResetInformation();
        SetTalkCanvas(true);
    }

    private void StageFirstEvent0()
    { 
        EventStart();
        Debug.Log("첫번째 이벤트확인");
        SetEventSilentAll();
        SetTalkData("BattleSceneTutorial","");
        //EventEnd();
    }

    private void StageSecondEvent0()
    { 
        SetEventSilentAll();
        EventEnd();
    }

    private void StageSecondEvent1()
    { 
        for (int i = 0; i < battleManager.playerCharacters.Length; i++)
        {
            if (battleManager.playerCharacters[i].GetComponent<Character>().GetIsDead() == true)
            {
                GameManager.instance.GoGameOverScene();
                return;
            }         
        }
        EventStart();
        TutorialDebuffEvent();
        SetTalkData("BattleSceneTutorial2","");
    }

    private void StageSecondEvent2()
    { 
        for (int i = 0; i < battleManager.playerCharacters.Length; i++)
        {
            if (battleManager.playerCharacters[i].GetComponent<Character>().GetIsDead() == true)
            {
                GameManager.instance.GoGameOverScene();
                return;
            }         
        }
        EventStart();
        TutorialDebuffEvent2();
        SetEventSilentAllFalse();
        SetTutorialSympathy();
        SetTalkData("BattleSceneTutorial3","");
    }

    private void StageThirdEvent0()
    {
        EventStart();
        SetTalkData("BattleSceneTutorial4","");
    }

    #endregion
    //-- 이벤트용 강제
    private void SetEventSilentAll()
    {
        for (int i = 0; i < battleManager.playerCharacters.Length; i++)
        {
            battleManager.playerCharacters[i].GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetSystemSilent(),true);
            battleManager.playerCharacters[i].GetComponent<Character>().SetIsCantUseSkill(true);          
        }

        for (int i = 0; i < battleManager.enemyCharacters.Length; i++)
        {
            battleManager.enemyCharacters[i].GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetSystemSilent(),true);
            battleManager.enemyCharacters[i].GetComponent<Character>().SetIsCantUseSkill(true);
        }

        battleManager.isCantUseSkill = true;
        
    }

    private void SetEventSilentAllFalse()
    {
        for (int i = 0; i < battleManager.playerCharacters.Length; i++)
        {
            battleManager.playerCharacters[i].GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetSystemSilent(),false);
            battleManager.playerCharacters[i].GetComponent<Character>().SetIsCantUseSkill(false);          
        }

        for (int i = 0; i < battleManager.enemyCharacters.Length; i++)
        {
            battleManager.enemyCharacters[i].GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetSystemSilent(),false);
            battleManager.enemyCharacters[i].GetComponent<Character>().SetIsCantUseSkill(false);
        }

        battleManager.isCantUseSkill = false;
    }

    private void TutorialDebuffEvent()
    {
        for (int i = 0; i < battleManager.playerCharacters.Length; i++)
        {
            battleManager.playerCharacters[i].GetComponent<Character>().SetStatusEffect(14, true);         
            battleManager.playerCharacters[i].GetComponent<Character>().SetStatusEffect(15, true);         
            battleManager.playerCharacters[i].GetComponent<Character>().SetStatusEffect(16, true);         
        }
    }

    private void TutorialDebuffEvent2()
    {
        for (int i = 0; i < battleManager.playerCharacters.Length; i++)
        {
            battleManager.playerCharacters[i].GetComponent<Character>().SetStatusEffect(14, false);         
            battleManager.playerCharacters[i].GetComponent<Character>().SetStatusEffect(15, false);         
            battleManager.playerCharacters[i].GetComponent<Character>().SetStatusEffect(16, false);         
        }

        for (int i = 0; i < battleManager.enemyCharacters.Length; i++)
        {
            battleManager.enemyCharacters[i].GetComponent<Character>().SetStatusEffect(14, true);
            battleManager.enemyCharacters[i].GetComponent<Character>().SetStatusEffect(15, true);
            battleManager.enemyCharacters[i].GetComponent<Character>().SetStatusEffect(16, true);
        }
    }
    
    private void SetTutorialSympathy()
    {
        for (int i = 0; i < battleManager.playerCharacters.Length; i++)
        {
            SympathyDatabaseManager.instance.SetCharacterSympathy(battleManager.playerCharacters[i].GetComponent<Character>(), SympathyType.Rational);     
        }
    }
    //--

}
