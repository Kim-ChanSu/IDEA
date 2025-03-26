using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePhaseManager : MonoBehaviour
{
    private BattleManager battleManager;

    private bool isEnemyTurn = false;
    private float effectTime = 0;
    private float fadeOutTime = 1.5f;
    private float fadeInTime = 1.0f;
    private float fadeDelayTime = 0.5f;

    public GameObject turnLogo;
    public Sprite playerTurnLogo;
    public Sprite enemyTurnLogo;
    public Sprite GameClearLogo;
    public Sprite GameOverLogo;

    private bool isStageClear = false;

    [SerializeField] private GameObject battleResultCanvas;
    [SerializeField] private GameObject battleResultPlayerWindow;
    [SerializeField] private GameObject battleResultEnemyWindow;
    [SerializeField] private GameObject battleResultWindow;
    [SerializeField] private GameObject battleResultNextButton;
    [SerializeField] private GameObject battleResultCharacterFacePrefab;
    [SerializeField] private GameObject battleLevelUpWindow;
    [SerializeField] private GameObject battleLearnSkillWindow;
    [SerializeField] private GameObject battleResultLearnSkillPrefab;
    private List<Character> levelUpCharacter = new List<Character>();

    //전투씬 상단용 ChangePhase가 여깄어서 여기에 추가
    [SerializeField]
    private GameObject battleTopDiamond;
    [SerializeField]
    private GameObject battleTopActionUI;
    [SerializeField]
    private GameObject battleTopSelectActionText;

    void Start()
    {
        InitializeBattlePhaseManager();
    }

    void Update()
    {
        ReadyToBattle();        
    }

    
    void OnMouseOver()
    {
        #region
        if((battleManager.IsMenu() == false) && (battleManager.nowPhase != BattlePhase.IsEvent) && (battleManager.nowPhase != BattlePhase.GameResult))
        {  
            if(battleManager.IsCanCheckInformationCheck() == true)
            { 
                battleManager.battleGetInformationManager.CallShootRay();
            }
            else
            { 
                battleManager.battleGetInformationManager.ResetInformation();
            }

           switch(battleManager.nowPhase)
            {
                case BattlePhase.MyTurn_ReadyToStart:
                    break;
                case BattlePhase.MyTurn_Start:
                    break;
                case BattlePhase.MyTurn_CharcterSelect:
                    if((battleManager.IsMenu() == false) && (GameManager.instance.KeyCheckMenu() == true))
                    { 
                        Invoke("SetMenuActiveTrue", 0.01f);
                    }

                    if((battleManager.IsMenu() == false) && (GameManager.instance.KeyCheckTurnSkip() == true))
                    {
                        battleManager.SkipPlayerTurn();
                    }

                    battleManager.battleGetInformationManager.SetTargetCharacter(); 
                    battleManager.battleCharacterManager.CharacterTap();
                    break;
                case BattlePhase.MyTurn_Moving:
                    if((battleManager.IsPlayerSelectToCharacterMove() == true) && (GameManager.instance.KeyCheckEscape() == true))
                    { 
                        battleManager.battleGetInformationManager.ClearTargetCharacter();
                        ChangePhase(BattlePhase.MyTurn_CharcterSelect);
                    }
                    battleManager.battleCharacterManager.CharacterMove(ref battleManager.targetCharacter);
                    break;
                case BattlePhase.MyTurn_Command:
                    if(GameManager.instance.KeyCheckEscape() == true)
                    { 
                        if(battleManager.battleSkillWindow.activeSelf == true)
                        { 
                            battleManager.battleCommandManager.SetBattleSkillWindow(false);
                        }
                        else
                        { 
                            battleManager.battleCharacterManager.ReturnForBeforeMove();
                        }
                    }
                    break;
                case BattlePhase.MyTurn_SkillTargeting:
                    if(GameManager.instance.KeyCheckEscape() == true)
                    { 
                        battleManager.battleCharacterManager.ReturnForBeforeCommand();
                    }
                    battleManager.battleCommandManager.SelectTarget();
                    break;
                case BattlePhase.MyTurn_ItemTargeting:
                    if(GameManager.instance.KeyCheckEscape() == true)
                    { 
                        battleManager.battleCharacterManager.ReturnForBeforeCommand();
                    }
                    break;
                case BattlePhase.EnemyTurn_ReadyToStart:
                    break;
                case BattlePhase.EnemyTurn_Start:
                    break;
                case BattlePhase.EnemyTurn_Result:
                    break;
                case BattlePhase.IsAnimation:
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
    
    private void InitializeBattlePhaseManager()
    {
        battleManager = this.gameObject.GetComponent<BattleManager>();
        battleManager.nowPhase = BattlePhase.MyTurn_ReadyToStart;
    }

    private void ReadyToBattle()
    {
        #region
        if((battleManager.IsMenu() == true) && (GameManager.instance.KeyCheckMenu() == true))
        {
            battleManager.battleGetInformationManager.SetBattleMenuWindow(false);
        }
        else if(battleManager.IsMenu() == false)
        { 
            switch(battleManager.nowPhase)
            {
                case BattlePhase.MyTurn_ReadyToStart:
                    ReadyToStartPhase();
                    break;
                case BattlePhase.MyTurn_Start:
                    TurnStart();
                    break;
                case BattlePhase.MyTurn_CharcterSelect:
                    break;
                case BattlePhase.MyTurn_Command:
                    break;
                case BattlePhase.MyTurn_SkillTargeting:
                    break;
                case BattlePhase.MyTurn_ItemTargeting:
                    break;
                case BattlePhase.MyTurn_Result:
                    CheckPlayerTurnEnd();
                    break;
                case BattlePhase.EnemyTurn_ReadyToStart:
                    EnemyTurnReadyToStart();
                    break;
                case BattlePhase.EnemyTurn_Start:
                    EnemyTurnStart();
                    break;
                case BattlePhase.EnemyTurn_Moving:
                    EnemyToMoving();
                    break;
                case BattlePhase.EnemyTurn_Command:
                    EnemyToCommand();
                    break;
                case BattlePhase.EnemyTurn_Result:
                    AllEnemyTurnEnd();
                    break;
                case BattlePhase.IsAnimation:
                    break;
                case BattlePhase.GameClear: 
                    GameEnd(true);
                    break;
                case BattlePhase.GameOver:
                    GameEnd(false);
                    break;
                case BattlePhase.GameResult:
                    break;
                default:
                    break;

            }

            /*
            if(battleManager.IsAnimation() == true)
            { 
                ChangePhase(BattlePhase.IsAnimation);           
            }
            */
        }
        #endregion
    }
    
    private void SetMenuActiveTrue()
    { 
        battleManager.battleGetInformationManager.SetBattleMenuWindow(true);
    }

    private void ReadyToStartPhase()
    { 
        #region
        if (battleManager.GetCharacterSettingComplete() == true)
        {
            Debug.Log("배틀 준비");
            battleManager.SetBattleSceneHinderUI(true);
            battleManager.SetBattleLog(null, "경기 시작!");
            ChangePhase(BattlePhase.MyTurn_Start);
        }    
        #endregion
    }

    private void TurnStart()
    {
        #region
        isEnemyTurn = false;
        for (int i = 0; i < battleManager.playerCharacters.Length; i++)
        { 
            battleManager.playerCharacters[i].GetComponent<Character>().SetIsTurnEnd(false);
            CheckCharacterSympathyRational();//감정 상태이상 해제 검사
            CheckStatusEffect(battleManager.playerCharacters[i]);
        }

        for(int i = 0; i < battleManager.enemyCharacters.Length; i++)
        { 
            battleManager.enemyCharacters[i].GetComponent<Character>().SetIsTurnEnd(false);
        }

        battleManager.battleTurnCount++;
        Debug.Log("턴이 증가 했습니다 현재 턴은 " + battleManager.battleTurnCount);
        if(battleManager.battleEventManager.BattleEventCheck() == false) // 이벤트 체크
        {
            StartTurnEffect(false);
        }

        //CheckPlayerTurnEnd();
        //ChangePhase(BattlePhase.MyTurn_CharcterSelect);
        #endregion
    }

    public void StartTurnEffect(bool isEnemy)
    { 
        battleManager.SetIsAnimation(true);
        //isEnemyTurn = isEnemy; 애니메이션 순서때문에 꺼둠
        ChangeGameObjectColorAndAlpha(turnLogo,0);
        turnLogo.SetActive(true);
        ChangePhase(BattlePhase.IsTurnEffect);

        if(isEnemyTurn == true)
        { 
            turnLogo.GetComponent<Image>().sprite = enemyTurnLogo;           
        }
        else
        { 
            turnLogo.GetComponent<Image>().sprite = playerTurnLogo;
        }

        effectTime = 0;
        StartCoroutine(TurnEffectFadeIn(fadeInTime));
    }

    private void GameEnd(bool isClear)
    {
        isStageClear = isClear;
        ChangeGameObjectColorAndAlpha(turnLogo,0);
        turnLogo.SetActive(true);
        ChangePhase(BattlePhase.GameResult);
        
        if(isClear == true)
        {
            turnLogo.GetComponent<Image>().sprite = GameClearLogo;
        }
        else
        {
            turnLogo.GetComponent<Image>().sprite = GameOverLogo;
        }

        effectTime = 0;
        StartCoroutine(TurnEffectFadeIn(fadeInTime));
    }

    IEnumerator TurnEffectFadeIn(float moveSpeed)
    { 
        #region
        if(effectTime < moveSpeed)
        { 
            effectTime += Time.deltaTime/moveSpeed;
            float titleImageColor = Mathf.Lerp(0.0f, 1.0f, effectTime);
            turnLogo.GetComponent<Image>().color = new Color(titleImageColor,titleImageColor,titleImageColor);

            ChangeGameObjectAlpha(turnLogo , titleImageColor);
                
            yield return null;
            StartCoroutine(TurnEffectFadeIn(moveSpeed)); 
        }
        else
        { 
            ChangeGameObjectColorAndAlpha(turnLogo,1);
            effectTime = 0;  
            Invoke("CallTurnEffectFadeOut", fadeDelayTime);
        } 
        #endregion
    }

    private void CallTurnEffectFadeOut()
    { 
        effectTime = 0;
        StartCoroutine(TurnEffectFadeOut(fadeOutTime));
    }

    IEnumerator TurnEffectFadeOut(float moveSpeed)
    { 
        #region
        if(effectTime < moveSpeed)
        { 
            effectTime += Time.deltaTime/moveSpeed;
            float ImageColor = Mathf.Lerp(1.0f, 0.0f, effectTime);
            turnLogo.GetComponent<Image>().color = new Color(ImageColor,ImageColor,ImageColor);

            ChangeGameObjectAlpha(turnLogo, ImageColor);
                
            yield return null;
            StartCoroutine(TurnEffectFadeOut(moveSpeed)); 
        }
        else
        { 
            ChangeGameObjectColorAndAlpha(turnLogo,0);
            effectTime = 0;  
            TurnEffectEnd();
        } 
        #endregion
    }

    private void TurnEffectEnd()
    { 
        #region
        battleManager.SetIsAnimation(false);
        ChangeGameObjectColorAndAlpha(turnLogo,0);
        turnLogo.SetActive(false);

        if(battleManager.nowPhase == BattlePhase.GameResult)
        { 
            if(isStageClear == true)
            { 
                //GameClear();
                ClearReward();
            }
            else
            { 
                GameOver();
            }
            return;    
        }

        if(isEnemyTurn == true)
        { 
            CheckAITurn();
        }
        else
        { 
            CheckPlayerTurnEnd(); //대화 이벤트 턴제로 넣을꺼면 여기에 eventcheck넣고 eventcheck에서 저 함수(CheckPlayerTurnEnd) 호출하면 됨 <- 플레이어턴 뜨기전에 호출하기 위해서 TurnStart에 넣었음
        }
        #endregion
    }

    private void ChangeGameObjectColorAndAlpha(GameObject target,float alpha)
    { 
        turnLogo.GetComponent<Image>().color = new Color(alpha,alpha,alpha);
        ChangeGameObjectAlpha(turnLogo, alpha);        
    }

    private void ChangeGameObjectAlpha(GameObject target,float alpha)
    { 
        #region
        Color color = target.GetComponent<Image>().color;
        color.a = alpha;
        target.GetComponent<Image>().color = color;      
        #endregion
    }


    public void CheckStatusEffect(GameObject target) //적응은 여기서하고 턴은 나중에 캐릭터 턴 종료시 적용함 + 배틀매니저 CheckSkillStatusEffect()여기서 이거 한번 실행시킴
    { 
        #region
        bool targetCantUseSkill = false;
        
        for(int j = 0; j < target.GetComponent<Character>().characterStatusEffect.Length; j++)
        { 
            if(target.GetComponent<Character>().characterStatusEffect[j].isOn == true)
            { 
                StatusEffect checkEffect = StatusEffectDatabaseManager.instance.GetStatusEffect(j);

                if((StatusEffectDatabaseManager.instance.GetMinCheckStatusChange() < checkEffect.turnDamage) || (StatusEffectDatabaseManager.instance.GetMinCheckStatusChange() * -1 > checkEffect.turnDamage))
                { 
                    battleManager.TurnHPDamage(target, checkEffect.turnDamage);   
                }
                battleManager.UpdateCharacterStatus();

                if((checkEffect.isCantMove == true) || (checkEffect.isDead == true))
                { 
                    if ((target.GetComponent<Character>().GetIsTurnEnd() == false) && (checkEffect.isCantMove == true) && (checkEffect.isDead == false))
                    {
                        battleManager.SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 상태이상으로 인해 행동할 수 없다!");
                    }

                    target.GetComponent<Character>().SetIsTurnEnd(true);                 
                }  
                else if(checkEffect.isCantUseSkill == true)
                { 
                    targetCantUseSkill = true;
                }
            }
        } 
        CheckTileTurnEffect(target);

        target.GetComponent<Character>().SetIsCantUseSkill(targetCantUseSkill);
        #endregion
    }

    public void CheckTurnEndStatusEffect(GameObject target) //아군이 아군에게 버프 줄 시 검사용 //위에 함수 이용하면 독뎀때문에 분리함
    {
        #region
        bool targetCantUseSkill = false;

        for(int j = 0; j < target.GetComponent<Character>().characterStatusEffect.Length; j++)
        { 
            if(target.GetComponent<Character>().characterStatusEffect[j].isOn == true)
            { 
                StatusEffect checkEffect = StatusEffectDatabaseManager.instance.GetStatusEffect(j);

                if((checkEffect.isCantMove == true) || (checkEffect.isDead == true))
                { 
                    if ((target.GetComponent<Character>().GetIsTurnEnd() == false) && (checkEffect.isCantMove == true) && (checkEffect.isDead == false))
                    {
                        battleManager.SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 상태이상으로 인해 행동할 수 없다!");
                    }

                    target.GetComponent<Character>().SetIsTurnEnd(true);                 
                }  
                else if(checkEffect.isCantUseSkill == true)
                { 
                    targetCantUseSkill = true;
                }
            }
        }   
        target.GetComponent<Character>().SetIsCantUseSkill(targetCantUseSkill);
        #endregion
    }

    private void CheckTileTurnEffect(GameObject target)
    {
        #region
        RaycastHit2D[] tileChecker;
        tileChecker = Physics2D.RaycastAll(new Vector2 (target.transform.position.x, target.transform.position.y), Vector2.zero);
        for (int i = 0; i < tileChecker.Length; i++)
        {
            if ((tileChecker[i].transform.tag == "Tile") && (tileChecker[i].transform.GetComponent<MapBlock>() == true))
            {
                tileChecker[i].transform.GetComponent<MapBlock>().TileTurnEffect(target);
            }
        }
        #endregion
    }

    private void CheckCharacterSympathyRational()
    { 
        #region
        int rationalCount = 0;
        //플레이어
        for(int i = 0; i < battleManager.playerCharacters.Length; i++)
        {
            if((battleManager.playerCharacters[i].GetComponent<Character>().sympathyType == SympathyType.Rational) && (battleManager.playerCharacters[i].GetComponent<Character>().GetIsDead() == false))
            { 
                rationalCount++;
            }
        }

        if(rationalCount >= 5)
        { 
            Debug.Log("플레이어의 냉정 스택을 확인 상태이상을 제거 합니다!");
            for(int i = 0; i < battleManager.playerCharacters.Length; i++)
            {                 
                for(int j = 0; j < StatusEffectDatabaseManager.instance.GetStatusEffectLength(); j++)
                { 
                    if(StatusEffectDatabaseManager.instance.IsCanDispelBySympathy(j) == true)
                    { 
                        battleManager.playerCharacters[i].GetComponent<Character>().SetStatusEffect(j, false);
                    }
                }
            }
        }
        rationalCount = 0;
        //적
        for(int i = 0; i < battleManager.enemyCharacters.Length; i++)
        {
            if((battleManager.enemyCharacters[i].GetComponent<Character>().sympathyType == SympathyType.Rational) && (battleManager.enemyCharacters[i].GetComponent<Character>().GetIsDead() == false))
            { 
                rationalCount++;
            }
        }

        if(rationalCount >= 5)
        { 
            Debug.Log("적의 냉정 스택을 확인 상태이상을 제거 합니다!");
            for(int i = 0; i < battleManager.enemyCharacters.Length; i++)
            {                 
                for(int j = 0; j < StatusEffectDatabaseManager.instance.GetStatusEffectLength(); j++)
                { 
                    if(StatusEffectDatabaseManager.instance.IsCanDispelBySympathy(j) == true)
                    { 
                        battleManager.enemyCharacters[i].GetComponent<Character>().SetStatusEffect(j, false);
                    }
                }
            }
        }
        #endregion
    }


    private void CheckPlayerTurnEnd()
    { 
        int turnEndCount = 0; 

        for(int i = 0; i < battleManager.playerCharacters.Length; i++)
        { 
            if(battleManager.playerCharacters[i].GetComponent<Character>().GetIsTurnEnd() == true)
            { 
                turnEndCount += 1;
            }
        }        

        if(turnEndCount >= battleManager.playerCharacters.Length)
        { 
            ChangePhase(BattlePhase.EnemyTurn_ReadyToStart);
        }
        else
        { 
            ChangePhase(BattlePhase.MyTurn_CharcterSelect);
        }
    }

    public void CheckAITurn()
    {
        //상태이상 검사하는 과정에서 미리 turnend 걸어버려서 AIIdx값 변동이 없기에 무한루프 버그 걸려서 따로 분리함
        int turnEndCount = 0;

        for (int i = 0; i < battleManager.enemyCharacters.Length; i++)
        {
            if (battleManager.enemyCharacters[i].GetComponent<Character>().GetIsTurnEnd() == true)
            {
                turnEndCount += 1;
            }
        }

        if (turnEndCount >= battleManager.enemyCharacters.Length)
        {
            ChangePhase(BattlePhase.EnemyTurn_Result);
        }
        else
        {
            ChangePhase(BattlePhase.EnemyTurn_Start);
        }
    }

    private void EnemyTurnReadyToStart()
    {
        AISetting();
    }

    private void AISetting()
    {
        isEnemyTurn = true;
        //상태이상 체크
        for(int i = 0; i < battleManager.enemyCharacters.Length; i++)
        { 
            CheckCharacterSympathyRational();//감정 상태이상 해제검사
            CheckStatusEffect(battleManager.enemyCharacters[i]);            
        }
        //-----------------------------------------------

        battleManager.aiManager.ReadyForAITurnStart();

        //한번만 발생함
        for(int i = 0; i < battleManager.playerCharacters.Length; i++) //캐릭터 애니메이션떄문에..
        {
            battleManager.playerCharacters[i].GetComponent<Character>().SetCharacterColorTurnEnd(false);
        }

        StartTurnEffect(true);
        //CheckAITurn(); 전부다 이동불가 상태일시 멈추는거 방지하기 위해서 이펙트 끝난 후 이걸로 판정함
        //ChangePhase(BattlePhase.EnemyTurn_Start);
    }

    private void EnemyTurnStart()
    {       
        battleManager.aiManager.AITurnStart();
    }

    private void EnemyToMoving()
    {
        //battleManager.aiManager.AICharacterSelectOrderReset();
        //battleManager.aiManager.SelectAICharacter();
        Debug.Log("EnemyMovingStart");
        battleManager.aiManager.AIMove();
    }

    private void EnemyToCommand()
    {
        Debug.Log("EnemyCommandStart");
        battleManager.aiManager.AICommand();
    }

    private void AllEnemyTurnEnd()
    {
        Debug.Log("AllEnemyTurnEnd");
        battleManager.aiManager.AllAITurnEnd();
    }


    public void ChangePhase(BattlePhase Phase)
    {
        battleManager.nowPhase = Phase;
        switch (Phase)
        {
            case BattlePhase.MyTurn_Moving:
                SetBattleTopActionUI(true, "이동");
                break;
            case BattlePhase.MyTurn_Command:
                SetBattleTopActionUI(true, "행동 선택");
                break;
            case BattlePhase.MyTurn_SkillTargeting:
                SetBattleTopActionUI(true, battleManager.useSkill.ingameSkillName);
                break;
            case BattlePhase.IsAnimation:
                break;
            case BattlePhase.Stay:
                break;
            default:
                SetBattleTopActionUI(false);
                break;
        }
        battleManager.UpdateStrategicPointCount();
        Debug.Log("페이즈 변경됨 " + Phase);      
    }

    private void SetBattleTopActionUI(bool mode, string actionText = "")
    {
        if (mode == false)
        {
            battleTopActionUI.SetActive(false);
            battleTopDiamond.SetActive(true);
        }
        else
        {
            battleTopSelectActionText.GetComponent<Text>().text = actionText;             
            battleTopDiamond.SetActive(false);
            battleTopActionUI.SetActive(true);
        }
    }


    public bool GetIsEnemyTurn()
    { 
        return isEnemyTurn;    
    }

    //전투 결과용---------
    #region
    private void GameOver()
    { 
        Debug.LogWarning("쳐발림");
        GameManager.instance.IncreasePlayerTeamHP(GameManager.instance.stageUseHealth * -1);
        GameManager.instance.HealAllPlayerCharacter();
        if (GameManager.instance.isMainStage == false)
        {
            GameManager.instance.NextDay();
            return;
        }
        GameManager.instance.GoGameOverScene();
    }

    private void ClearReward() //결과페이즈에서는 스테이터스 업데이트 안되니 그걸로 전 후 비교하면 될듯
    { 
        Debug.LogWarning("인생의 승리자");
        battleManager.battleGetInformationManager.ResetInformation();
        battleManager.SetBattleSceneHinderUI(false);

        for(int i = 0; i < battleManager.playerCharacters.Length; i++)
        { 
            battleManager.CallInitializePlayerCharacterStatus(i); 
        }

        GameManager.instance.gold = GameManager.instance.gold + GameManager.instance.clearGold;
        Debug.Log("승리 보상으로 " + GameManager.instance.clearGold + "만큼의 재화를 얻었습니다.");        

        for(int i = 0; i < battleManager.playerCharacters.Length; i++)
        { 
            GameManager.instance.CharacterEXPUp(battleManager.playerCharacters[i].GetComponent<Character>().characterDatabaseNum, GameManager.instance.clearEXP);
            GameManager.instance.CheckCharacterLevelUp(battleManager.playerCharacters[i].GetComponent<Character>().characterDatabaseNum);
        }

        GameManager.instance.IncreasePlayerTeamHP(GameManager.instance.stageUseHealth * -1);

        if (GameManager.instance.isMainStage == true)
        {
            GameManager.instance.Var[GameManager.DDAYVARNUM].Var = GameManager.instance.nextD_day;
            GameManager.instance.beforeMainStageSetD_day = GameManager.instance.nextD_day;
            GameManager.instance.isMainStageEnd = true;
        }

        SetBattleResultWindow(true);
    }

    private void SetBattleResultWindow(bool mode)
    { 
        battleResultCanvas.SetActive(mode);
        levelUpCharacter.Clear();
        if(mode == true)
        {
            CreateBattleResultCharacterFace();
            SetbattleResultWindow();
            //SetCheckCharacterLevelUp(); 기능추가로 이동
        }
        else
        { 
            BreakBattleResultCharacterFace();
        }           
    }

    private void CreateBattleResultCharacterFace()
    { 
        #region
        for(int i = 0; i < battleManager.playerCharacters.Length; i++)
        { 
            if(battleResultCharacterFacePrefab != null)
            { 
                GameObject battleResultCharacterFace = Instantiate(battleResultCharacterFacePrefab);
                battleResultCharacterFace.name = "battleResultPlayerFace" + i;
                battleResultCharacterFace.GetComponent<BattleResultCharacterFace>().SetBattleResultCharacterFace(battleManager.playerCharacters[i].GetComponent<Character>());

                battleResultCharacterFace.transform.SetParent(battleResultPlayerWindow.transform); 
                battleResultCharacterFace.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }            
        }

        for(int i = 0; i < battleManager.enemyCharacters.Length; i++)
        { 
            if(battleResultCharacterFacePrefab != null)
            { 
                GameObject battleResultCharacterFace = Instantiate(battleResultCharacterFacePrefab);
                battleResultCharacterFace.name = "battleResultEnemyFace" + i;
                battleResultCharacterFace.GetComponent<BattleResultCharacterFace>().SetBattleResultCharacterFace(battleManager.enemyCharacters[i].GetComponent<Character>());

                battleResultCharacterFace.transform.SetParent(battleResultEnemyWindow.transform); 
                battleResultCharacterFace.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }            
        }
        #endregion
    }

    private void SetbattleResultWindow() //여기에
    { 
        #region
        battleResultWindow.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "0"; //+ battleManager.battleTurnCount;
        battleResultWindow.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.EXPName + "";
        battleResultWindow.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "0"; //+ GameManager.instance.clearEXP;
        battleResultWindow.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.goldName + "";
        battleResultWindow.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = "0"; //+ GameManager.instance.clearGold;
        StartCoroutine(ClearCountEffect());
        #endregion
    }

    private IEnumerator ClearCountEffect()
    {
        effectTime = 0.0f;
        float moveTime = 1.0f;

        while (effectTime < 1.0f)
        {
            battleResultWindow.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "" + (int)(battleManager.battleTurnCount * effectTime);
            battleResultWindow.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "" + (int)(GameManager.instance.clearEXP * effectTime);
            battleResultWindow.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = "" + (int)(GameManager.instance.clearGold * effectTime);
            effectTime += Time.deltaTime/moveTime;
            yield return null;
        }
        effectTime = 0.0f;
        battleResultWindow.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "" + battleManager.battleTurnCount;
        battleResultWindow.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "" + GameManager.instance.clearEXP;
        battleResultWindow.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = "" + GameManager.instance.clearGold;
        Invoke("SetCheckCharacterLevelUp", 0.5f);
    }

    private void BreakBattleResultCharacterFace()
    { 
        #region
        for(int i = 0; i < battleResultPlayerWindow.transform.childCount; i++)
        { 
             Destroy(battleResultPlayerWindow.transform.GetChild(i).gameObject);
        }

        for(int i = 0; i < battleResultEnemyWindow.transform.childCount; i++)
        { 
             Destroy(battleResultEnemyWindow.transform.GetChild(i).gameObject);
        }    
        #endregion
    }


    private void SetCheckCharacterLevelUp()
    { 
        levelUpCharacter.Clear();
        for(int i = 0; i < battleManager.playerCharacters.Length; i++)
        { 
            if(battleManager.playerCharacters[i].GetComponent<Character>().status.level < GameManager.instance.PlayerCharacter[battleManager.playerCharacters[i].GetComponent<Character>().characterDatabaseNum].level)
            { 
                Debug.Log(battleManager.playerCharacters[i].GetComponent<Character>().status.inGameName + "의 레벨업 확인");
                levelUpCharacter.Add(battleManager.playerCharacters[i].GetComponent<Character>());
            }
        }
        CheckCharacterLevelUp();
        /* 테스트용 <- 테스트 성공
        while(levelUpCharacter.Count > 0)
        { 
            Debug.LogWarning(levelUpCharacter[0].status.inGameName);
            levelUpCharacter.RemoveAt(0);
        }     
        */
    }

    private void SetBattleLevelUpWindow(bool mode)
    { 
        battleLevelUpWindow.SetActive(mode);
        if(mode == true)
        { 
            SetStatusIncrease();
        }
        else
        { 
            levelUpCharacter.Clear();
            BreakBattleResultLearnSkill();
            battleResultNextButton.SetActive(true);
        }
    }

    private void SetStatusIncrease()
    { 
        #region
        int faceNum = 0;

        if((levelUpCharacter != null) && (levelUpCharacter.Count > 0))
        {
            if(faceNum < levelUpCharacter[0].status.face.Length)
            { 
                battleLevelUpWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite = levelUpCharacter[0].status.face[faceNum];
            }
            else
            { 
                battleLevelUpWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite = GameManager.instance.GetDefaultFace();
            }
            battleLevelUpWindow.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = levelUpCharacter[0].status.inGameName;
            battleLevelUpWindow.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.levelName + " : " + levelUpCharacter[0].status.level + " → " + GameManager.instance.PlayerCharacter[levelUpCharacter[0].characterDatabaseNum].level;

            battleLevelUpWindow.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.HPName;
            battleLevelUpWindow.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<Text>().text = levelUpCharacter[0].status.maxHP + " → " + GameManager.instance.PlayerCharacter[levelUpCharacter[0].characterDatabaseNum].maxHP;

            battleLevelUpWindow.transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.ATKName;
            battleLevelUpWindow.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<Text>().text = levelUpCharacter[0].status.ATK + " → " + GameManager.instance.PlayerCharacter[levelUpCharacter[0].characterDatabaseNum].ATK;

            battleLevelUpWindow.transform.GetChild(2).GetChild(3).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.MAKName;
            battleLevelUpWindow.transform.GetChild(2).GetChild(3).GetChild(1).GetComponent<Text>().text = levelUpCharacter[0].status.MAK + " → " + GameManager.instance.PlayerCharacter[levelUpCharacter[0].characterDatabaseNum].MAK;

            battleLevelUpWindow.transform.GetChild(2).GetChild(4).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.MPName;
            battleLevelUpWindow.transform.GetChild(2).GetChild(4).GetChild(1).GetComponent<Text>().text = levelUpCharacter[0].status.maxMP + " → " + GameManager.instance.PlayerCharacter[levelUpCharacter[0].characterDatabaseNum].maxMP;

            battleLevelUpWindow.transform.GetChild(2).GetChild(5).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.DEFName;
            battleLevelUpWindow.transform.GetChild(2).GetChild(5).GetChild(1).GetComponent<Text>().text = levelUpCharacter[0].status.DEF + " → " + GameManager.instance.PlayerCharacter[levelUpCharacter[0].characterDatabaseNum].DEF;

            battleLevelUpWindow.transform.GetChild(2).GetChild(6).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.MDFName;
            battleLevelUpWindow.transform.GetChild(2).GetChild(6).GetChild(1).GetComponent<Text>().text = levelUpCharacter[0].status.MDF + " → " + GameManager.instance.PlayerCharacter[levelUpCharacter[0].characterDatabaseNum].MDF;

            CreateBattleResultLearnSkill();
        }
        else
        { 
            Debug.LogWarning("levelUpCharacter가 이상합니다!");
            SetBattleLevelUpWindow(false);   
        }    
        #endregion
    }

    private void CreateBattleResultLearnSkill()
    {
        BreakBattleResultLearnSkill();
        for(int i = 0; i < levelUpCharacter[0].status.characterSkill.Length; i++)
        { 
            if((GameManager.instance.PlayerCharacter[levelUpCharacter[0].characterDatabaseNum].characterSkill[i] == true) && (levelUpCharacter[0].status.characterSkill[i] == false))
            { 
                if(battleResultLearnSkillPrefab != null)
                { 
                    GameObject battleResultLearnSkill= Instantiate(battleResultLearnSkillPrefab);
                    battleResultLearnSkill.name = "BattleResultLearnSkill" + i;
                    battleResultLearnSkill.GetComponent<BattleResultLearnSkillPrefab>().SetBattleResultLearnSkill(i);

                    battleResultLearnSkill.transform.SetParent(battleLearnSkillWindow.transform); 
                    battleResultLearnSkill.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }                  
            }
        }
    }

    private void BreakBattleResultLearnSkill()
    { 
        for(int i = 0; i < battleLearnSkillWindow.transform.childCount; i++)
        { 
             Destroy(battleLearnSkillWindow.transform.GetChild(i).gameObject);
        }        
    }

    public void CheckCharacterLevelUp()
    { 
        if((levelUpCharacter != null) && (levelUpCharacter.Count > 0))
        { 
            SetBattleLevelUpWindow(true);
            levelUpCharacter.RemoveAt(0);
        }
        else
        { 
            SetBattleLevelUpWindow(false);
        }
    }

    public void GameClear()
    { 
        GameManager.instance.HealAllPlayerCharacter();

        if (GameManager.instance.isMainStage == false)
        {
            GameManager.instance.NextDay();
            return;
        }

        if(GameManager.instance.endBattleCSVName != "")
        { 
            GameManager.instance.CSVName = GameManager.instance.endBattleCSVName;
            GameManager.instance.CSVPart = GameManager.instance.endBattleCSVPart;
            GameManager.instance.GoTalkScene();
        }
        else
        { 
            GameManager.instance.GoHomeScene();
        }       
    }
    #endregion
    //----------
}
