using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleGetInformationManager : MonoBehaviour
{
    private BattleManager battleManager;

    private string defaultColor = "<color=#ffffff>";
    private string warningColor = "<color=#ffd400>";
    private string rationalColor  = "<color=#3ef8ff>";
    private string angerColor  = "<color=#FFA254>";
    private string enjoyColor  = "<color=#32CD32>";
    private string upperColor  = "<color=#FF3737>";
    private string lowerColor  = "<color=#3244FF>";
    
    [SerializeField] private GameObject characterSympathyBackgroundObject;
    [SerializeField] private GameObject characterNameBackgroundObject;
    [SerializeField] private GameObject characterFaceMaskBackgroundObject;
    [SerializeField] private GameObject characterHPObject;
    [SerializeField] private GameObject characterMPObject;
    [SerializeField] private GameObject characterSPObject;
    [SerializeField] private GameObject characterStatusObject;
    [SerializeField] private GameObject warningImageObject;
    [SerializeField] private GameObject statusEffectBackgroundObject;

    [SerializeField] private GameObject statusEffectInformationPrepab;

    //로고용
    [SerializeField] private GameObject battleLogWindow;
    [SerializeField] private GameObject battleLogWindowScrollViewContent;
    [SerializeField] private GameObject battleLogPrefab;
    [SerializeField] private int battleLogLength = 30;
    private int battleLogCount = 0;
    private List<GameObject> battleLog = new List<GameObject>();


    //스테이테스용
    public GameObject battleStatusCanvas;
    public GameObject spaceRight;
    public GameObject spaceRightBack;
    public GameObject spaceLeftR;
    public GameObject spaceLeftL;
    public GameObject characterStatusButtonScrollviewContent;
    public GameObject characterStatusButtonPrefab;
    public GameObject characterStatusSkillContent;
    public GameObject characterStatusSkillButtonPrefab;
    [HideInInspector]
    public bool isCharacterStatusBack;
    [HideInInspector]
    public bool isEnemyCharacterStatus = true;
    [SerializeField]
    private Sprite[] characterFaceMaskBackground;

    void Start()
    {
        InitializeBattleGetInformationManager();
    }

   
    void OnMouseExit()
    {       
        ResetInformation();
    }

    void Update()
    {
        CheckBattleLogWindowSet();
    }

    public void ResetInformation()
    { 
        AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Select);
        AllCharacterSelectionModeClear();
        ClearTileInformation();        
    }

    private void InitializeBattleGetInformationManager()
    {
        battleManager = this.gameObject.GetComponent<BattleManager>();
        CreateStatusEffectInformationPrefab();
        CreateBattleLogPrefab();
    }

    private void CreateStatusEffectInformationPrefab()
    { 
        for(int i = 0; i < StatusEffectDatabaseManager.instance.GetStatusEffectLength(); i++)
        { 
            //if(StatusEffectDatabaseManager.instance.IsSystemStatusEffect(i) == false) 이거하면 데이터베이스하고 계수가 달라져서 일단은 생성하고 봄
            //{ 
                GameObject statusEffectInformation = Instantiate(statusEffectInformationPrepab);
                statusEffectInformation.name = "StatusEffectInformation" + i;
                statusEffectInformation.transform.GetChild(0).GetComponent<Image>().sprite = StatusEffectDatabaseManager.instance.GetStatusEffect(i).statusEffectIcon;
                statusEffectInformation.transform.SetParent(statusEffectBackgroundObject.transform); 
                statusEffectInformation.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); 
                statusEffectInformation.SetActive(false);
            //}
        }    
    }

    private void CreateBattleLogPrefab()
    {
        battleLog = new List<GameObject>();
        battleLogCount = 0;

        for(int i = 0; i < battleLogLength; i++)
        { 
            GameObject newBattleLog = Instantiate(battleLogPrefab);
            newBattleLog.name = "BattleLog" + i;
            newBattleLog.transform.SetParent(battleLogWindowScrollViewContent.transform); 
            newBattleLog.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); 
            newBattleLog.SetActive(false);
            battleLog.Add(newBattleLog);
        }  
    }

    private void ShootRay()
    {
        #region
        GameObject targetmapBlockObject = null; 
        GameObject targetCharacterObject = null;
       
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits; 
        hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);

        GetMapBlockPosition(ref targetmapBlockObject, hits);
        GetCharacterInformation(ref targetCharacterObject, hits);
        #endregion
    }
    
    private void GetMapBlockPosition(ref GameObject targetObject, RaycastHit2D[] hits)
    {
        #region
        bool isTile = false;
        
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.tag == "Tile")
            {
                targetObject = hits[i].collider.gameObject;
                if ((targetObject.GetComponent<MapBlock>() != null) && (targetObject.GetComponent<MapBlock>().GetisSelect() == false))
                {
                    battleManager.selectTileXpos = (int)targetObject.GetComponent<Transform>().position.x;
                    battleManager.selectTileYpos = (int)targetObject.GetComponent<Transform>().position.y;
                    SelectBlock(targetObject.GetComponent<MapBlock>());        
                    isTile = true;
                }
            }
        }

        if(isTile == true)
        { 
            RaycastHit2D[] hitsCheck;
            hitsCheck = Physics2D.RaycastAll(new Vector2 (targetObject.transform.position.x, targetObject.transform.position.y), Vector2.zero);
            for(int i = 0; i < hitsCheck.Length; i++)
            {
                if(hitsCheck[i].transform.tag == "Ball")
                { 
                    battleManager.tileInformation.transform.GetChild(0).gameObject.GetComponent<Text>().text += " + 공";
                }
            }
        }
        #endregion
    }
    
    private void GetCharacterInformation(ref GameObject targetObject, RaycastHit2D[] hits)
    {
        #region
        int cantMoveOnThisTileCount = 0;
        int NoneCharacterCount = 0;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.tag == "Character")
            {
                targetObject = hits[i].collider.gameObject;
                if (targetObject.GetComponent<Character>() != null)
                {
                    if (targetObject.GetComponent<Character>().GetisSelect() == false)
                    {
                        AllCharacterSelectionModeClear();
                        targetObject.GetComponent<Character>().SetCharacterSelectionMode(true);
                        SetCharacterInformation(targetObject.GetComponent<Character>());

                        if((battleManager.nowPhase == BattlePhase.MyTurn_SkillTargeting) && (battleManager.useSkill != null) && (battleManager.targetCharacter != null))
                        { 
                            CheckBallRoute(targetObject);
                        }
                    }
                }
            }
            else
            {
                NoneCharacterCount++;
            }
            
            if (GameManager.instance.CantMoveTagCheck(hits[i].transform.tag) == true)
            {
                //Debug.Log(hits[i]+ " 그리고 " + hits[i].transform.tag);
                cantMoveOnThisTileCount++;
            }
            
        }

        if (cantMoveOnThisTileCount > 0)
        {
            battleManager.cantMoveOnThisTile = true;
            //Debug.Log("타일 위에 무언가 있습니다");
        }
        else
        {
            battleManager.cantMoveOnThisTile = false;
            //Debug.Log("타일 위에 아무것도 없습니다");
        }

        if (NoneCharacterCount >= hits.Length)
        {
            AllCharacterSelectionModeClear();
        }

    #endregion
    }

    public void SetTargetCharacter()
    {
        #region
        if((GameManager.instance.KeyCheckAccept() == true)) //&& (battleManager.IsPlayerSelectToCharacterMove() == false)) 
        { 
            GameObject targetObject = null;
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);
             
            for (int i = 0; i < hits.Length; i++)
            {
                if(hits[i].transform.tag == "Character")
                { 
                    targetObject = hits[i].collider.gameObject;
                    if(targetObject.GetComponent<Character>() != null)
                    {                        
                        if ((targetObject.GetComponent<Character>().isEnemy == false) &&  (targetObject.GetComponent<Character>().GetIsTurnEnd()== false))
                        { 
                            battleManager.targetCharacter = targetObject;
                            VoiceDatabaseManager.instance.PlayVoice(targetObject.GetComponent<Character>().status.voice, VoiceType.Select);
                            Debug.Log("선택한 캐릭터는 " + targetObject);
                        }
                        else if(targetObject.GetComponent<Character>().isEnemy == true)
                        { 
                            SelectEnemy(ref targetObject);
                        }
                    }
                }
            }          
        }

        #endregion
    }

    private void SelectEnemy(ref GameObject targetObject) //플레이어가 적을 선택한 경우
    {
        if (targetObject.GetComponent<Character>().isEnemy == true)
        {
            ShowEnemyMoveArea(ref targetObject);
        }
    }


    private void ShowEnemyMoveArea(ref GameObject targetObject) 
    {
        battleManager.battleCharacterManager.ShowCharacterMoveRange(ref targetObject);
    }

    private void SelectBlock(MapBlock targetBlock)
    {
        #region
        AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Select);
        if(battleManager.battlePhaseManager.GetIsEnemyTurn() == false) //적 턴 일 때 타일 안보이게 하기
        { 
            targetBlock.SetSelectionMode(MapBlock.Highlight.Select);
        }

        SetTileInformation(targetBlock);
        #endregion
    }    

    private void SetTileInformation(MapBlock targetBlock)
    {
        #region
        battleManager.tileInformation.transform.GetChild(0).gameObject.GetComponent<Text>().text = targetBlock.mapBlockName;
        battleManager.tileInformation.transform.GetChild(1).gameObject.GetComponent<Text>().text = targetBlock.mapBlockEffectText;
        battleManager.tileInformation.SetActive(true);
        #endregion
    }

    private void ClearTileInformation()
    {
        #region
        battleManager.tileInformation.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
        battleManager.tileInformation.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
        battleManager.tileInformation.SetActive(false);
        #endregion
    }

    private void SetCharacterInformation(Character character)
    {
        #region
        string sympathyColor = GetSympathyColor(character.sympathyType);
        int faceNum = (int)character.sympathyType;

        characterNameBackgroundObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = character.status.inGameName;
        characterSympathyBackgroundObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "감정 : " + sympathyColor + GameManager.instance.GetSympathyTypeName(character.sympathyType) + "</color>";
        if(character.status.face.Length > faceNum) //공감 상태에 따라 얼굴바꾸기
        { 
            characterFaceMaskBackgroundObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = character.status.face[faceNum];
        }
        else
        { 
            if(character.status.face.Length > 0)
            { 
                characterFaceMaskBackgroundObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = character.status.face[0];
            }
            else
            { 
                characterFaceMaskBackgroundObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GameManager.instance.GetDefaultFace();
            }
        }

        characterFaceMaskBackgroundObject.gameObject.GetComponent<Image>().sprite = GetCharacterFaceMaskBackground(faceNum);//감정상태에 따른 테두리 뒷면 바꾸기

        if((int)(character.status.maxHP/4) < character.status.HP)
        { 
            characterHPObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = character.status.HP + "";           
        }
        else
        { 
            characterHPObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = warningColor + character.status.HP + "</color>";
        }
        characterHPObject.transform.GetChild(3).gameObject.GetComponent<Text>().text = character.status.maxHP + "";

        if((int)(character.status.maxMP/4) < character.status.MP)
        { 
            characterMPObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = character.status.MP + "";
        }
        else
        { 
            characterMPObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = warningColor + character.status.MP + "</color>";
        }
        characterMPObject.transform.GetChild(3).gameObject.GetComponent<Text>().text = character.status.maxMP + "";

        characterSPObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = sympathyColor + character.status.Sympathy+ "</color>";
        characterSPObject.transform.GetChild(3).gameObject.GetComponent<Text>().text = character.status.maxSympathy + "";

        if(character.isEnemy == true)
        { 
            //--ATK
            if(character.status.ATK > GameManager.instance.EnemyCharacter[character.characterDatabaseNum].ATK)
            { 
                characterStatusObject.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = upperColor  + character.status.ATK + "</color>";
            }
            else if(character.status.ATK < GameManager.instance.EnemyCharacter[character.characterDatabaseNum].ATK)
            { 
                characterStatusObject.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = lowerColor + character.status.ATK + "</color>";
            }
            else
            { 
                characterStatusObject.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = character.status.ATK + "";
            }         
            //--MAK
            if(character.status.MAK > GameManager.instance.EnemyCharacter[character.characterDatabaseNum].MAK)
            { 
                characterStatusObject.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Text>().text = upperColor  + character.status.MAK + "</color>";
            }
            else if(character.status.MAK < GameManager.instance.EnemyCharacter[character.characterDatabaseNum].MAK)
            { 
                characterStatusObject.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Text>().text = lowerColor + character.status.MAK + "</color>";
            }
            else
            { 
                characterStatusObject.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Text>().text = character.status.MAK + "";
            }  
            //--move
            if(character.status.move > GameManager.instance.EnemyCharacter[character.characterDatabaseNum].move)
            { 
                characterStatusObject.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = upperColor  + character.status.move + "</color>";
            }
            else if(character.status.move < GameManager.instance.EnemyCharacter[character.characterDatabaseNum].move)
            { 
                characterStatusObject.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = lowerColor + character.status.move + "</color>";
            }
            else
            { 
                characterStatusObject.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = character.status.move + "";
            }  
            //--DEF
            if(character.status.DEF > GameManager.instance.EnemyCharacter[character.characterDatabaseNum].DEF)
            { 
                characterStatusObject.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = upperColor  + character.status.DEF + "</color>";
            }
            else if(character.status.DEF < GameManager.instance.EnemyCharacter[character.characterDatabaseNum].DEF)
            { 
                characterStatusObject.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = lowerColor + character.status.DEF + "</color>";
            }
            else
            { 
                characterStatusObject.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = character.status.DEF + "";
            }  
            //--MDF
            if(character.status.MDF > GameManager.instance.EnemyCharacter[character.characterDatabaseNum].MDF)
            { 
                characterStatusObject.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Text>().text = upperColor  + character.status.MDF + "</color>";
            }
            else if(character.status.MDF < GameManager.instance.EnemyCharacter[character.characterDatabaseNum].MDF)
            { 
                characterStatusObject.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Text>().text = lowerColor + character.status.MDF + "</color>";
            }
            else
            { 
                characterStatusObject.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Text>().text = character.status.MDF + "";
            }  
            //--range
            if(character.status.range > GameManager.instance.EnemyCharacter[character.characterDatabaseNum].range)
            { 
                characterStatusObject.transform.GetChild(5).GetChild(1).gameObject.GetComponent<Text>().text = upperColor  + character.status.range + "</color>";
            }
            else if(character.status.range < GameManager.instance.EnemyCharacter[character.characterDatabaseNum].range)
            { 
                characterStatusObject.transform.GetChild(5).GetChild(1).gameObject.GetComponent<Text>().text = lowerColor + character.status.range + "</color>";
            }
            else
            { 
                characterStatusObject.transform.GetChild(5).GetChild(1).gameObject.GetComponent<Text>().text = character.status.range + "";
            }  

        }
        else
        { 
            //--ATK
            if(character.status.ATK > GameManager.instance.PlayerCharacter[character.characterDatabaseNum].ATK)
            { 
                characterStatusObject.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = upperColor  + character.status.ATK + "</color>";
            }
            else if(character.status.ATK < GameManager.instance.PlayerCharacter[character.characterDatabaseNum].ATK)
            { 
                characterStatusObject.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = lowerColor + character.status.ATK + "</color>";
            }
            else
            { 
                characterStatusObject.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = character.status.ATK + "";
            }
            //--MAK
            if(character.status.MAK > GameManager.instance.PlayerCharacter[character.characterDatabaseNum].MAK)
            { 
                characterStatusObject.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Text>().text = upperColor  + character.status.MAK + "</color>";
            }
            else if(character.status.MAK < GameManager.instance.PlayerCharacter[character.characterDatabaseNum].MAK)
            { 
                characterStatusObject.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Text>().text = lowerColor + character.status.MAK + "</color>";
            }
            else
            { 
                characterStatusObject.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Text>().text = character.status.MAK + "";
            }  
            //--move
            if(character.status.move > GameManager.instance.PlayerCharacter[character.characterDatabaseNum].move)
            { 
                characterStatusObject.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = upperColor  + character.status.move + "</color>";
            }
            else if(character.status.move < GameManager.instance.PlayerCharacter[character.characterDatabaseNum].move)
            { 
                characterStatusObject.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = lowerColor + character.status.move + "</color>";
            }
            else
            { 
                characterStatusObject.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = character.status.move + "";
            }  
            //--DEF
            if(character.status.DEF > GameManager.instance.PlayerCharacter[character.characterDatabaseNum].DEF)
            { 
                characterStatusObject.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = upperColor  + character.status.DEF + "</color>";
            }
            else if(character.status.DEF < GameManager.instance.PlayerCharacter[character.characterDatabaseNum].DEF)
            { 
                characterStatusObject.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = lowerColor + character.status.DEF + "</color>";
            }
            else
            { 
                characterStatusObject.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = character.status.DEF + "";
            }  
            //--MDF
            if(character.status.MDF > GameManager.instance.PlayerCharacter[character.characterDatabaseNum].MDF)
            { 
                characterStatusObject.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Text>().text = upperColor  + character.status.MDF + "</color>";
            }
            else if(character.status.MDF < GameManager.instance.PlayerCharacter[character.characterDatabaseNum].MDF)
            { 
                characterStatusObject.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Text>().text = lowerColor + character.status.MDF + "</color>";
            }
            else
            { 
                characterStatusObject.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Text>().text = character.status.MDF + "";
            }  
            //--range
            if(character.status.range > GameManager.instance.PlayerCharacter[character.characterDatabaseNum].range)
            { 
                characterStatusObject.transform.GetChild(5).GetChild(1).gameObject.GetComponent<Text>().text = upperColor  + character.status.range + "</color>";
            }
            else if(character.status.range < GameManager.instance.PlayerCharacter[character.characterDatabaseNum].range)
            { 
                characterStatusObject.transform.GetChild(5).GetChild(1).gameObject.GetComponent<Text>().text = lowerColor + character.status.range + "</color>";
            }
            else
            { 
                characterStatusObject.transform.GetChild(5).GetChild(1).gameObject.GetComponent<Text>().text = character.status.range + "";
            }
        }       

        SetCharacterStatuseEffectInformation(character);
        SetCharacterInformation(true);
        #endregion
    }

    private void SetCharacterStatuseEffectInformation(Character character)
    { 
        for(int i = 0; i < statusEffectBackgroundObject.transform.childCount; i++)
        { 
            statusEffectBackgroundObject.transform.GetChild(i).gameObject.SetActive(false);
        }        

        for(int i = 0; i < StatusEffectDatabaseManager.instance.GetStatusEffectLength(); i++)
        { 
            if((StatusEffectDatabaseManager.instance.IsSystemStatusEffect(i) == false) && (character.characterStatusEffect[i].isOn == true))
            { 
                if(StatusEffectDatabaseManager.instance.GetStatusEffect(i).isNotTurnCount != true)
                { 
                    statusEffectBackgroundObject.transform.GetChild(i).GetChild(2).GetComponent<Text>().text = "" + character.characterStatusEffect[i].statusEffectTurnCount;
                }
                else
                { 
                    statusEffectBackgroundObject.transform.GetChild(i).GetChild(2).GetComponent<Text>().text = "∞";
                }
                statusEffectBackgroundObject.transform.GetChild(i).gameObject.SetActive(true);
            }
        }     
    }

    private string GetSympathyColor(SympathyType sympathyType)
    { 
        switch(sympathyType)
        { 
            case SympathyType.None:
                return defaultColor; 
            case SympathyType.Rational:
                return rationalColor;
            case SympathyType.Anger:
                return angerColor;
            case SympathyType.Enjoy:
                return enjoyColor;
            default:
                return defaultColor;               
        }      
    }

    private void ClearCharacterInformation()
    {
        #region
        characterNameBackgroundObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
        characterFaceMaskBackgroundObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;
        warningImageObject.gameObject.SetActive(false); //뭔가 있어요(공)

        for(int i = 0; i < statusEffectBackgroundObject.transform.childCount; i++)
        { 
            statusEffectBackgroundObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        SetCharacterInformation(false);  
        /*
        battleManager.interruptObject = null;
        battleManager.isSomethingOnBallMoveRoute = false;
        */
        AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.ShowArea);
        #endregion
    }

    public void CheckBallRoute(GameObject targetObject)
    { 
        #region
        if((battleManager.useSkill.ignoreOtherCharacter != true) && (battleManager.useSkill.ignoreIsHaveBall != true))
        { 
            Vector3 dir = battleManager.targetCharacter.transform.position - targetObject.transform.position;
        
            float RayLength = Mathf.Sqrt(Mathf.Pow(battleManager.targetCharacter.transform.position.x - targetObject.transform.position.x, 2) + Mathf.Pow(battleManager.targetCharacter.transform.position.y - targetObject.transform.position.y, 2));
            Debug.Log("타겟 간의 거리는 " + RayLength);

            RaycastHit2D[] routeChecker; 
            Debug.DrawRay(targetObject.transform.position, dir, Color.blue, 10.0f); //dir * RayLength
            routeChecker = Physics2D.RaycastAll(new Vector2 (targetObject.transform.position.x, targetObject.transform.position.y), new Vector2 (dir.x, dir.y), RayLength); 
                                
            for(int j = 0; j < routeChecker.Length; j++)
            { 
                if((routeChecker[j].collider.gameObject != battleManager.targetCharacter) && (routeChecker[j].collider.gameObject != targetObject))
                { 
                    if(GameManager.instance.InterruptBallMoveCheck(routeChecker[j].transform.tag)== true)
                    { 
                        if(routeChecker[j].transform.tag == "Character")
                        { 
                            if((routeChecker[j].transform.gameObject.GetComponent<Character>().isEnemy != battleManager.targetCharacter.GetComponent<Character>().isEnemy) && (routeChecker[j].transform.gameObject.GetComponent<Character>().GetIsHaveBall() == false))
                            { 
                                SomeThingOnBallRoute(routeChecker[j].collider.gameObject);
                                break;
                            }
                        }
                        else
                        { 
                            SomeThingOnBallRoute(routeChecker[j].collider.gameObject);
                            break;
                        }
                    }
                    else
                    {
                        ResetSomeThingOnBallRoute();
                    }

                }
            }  
        }
        else
        {
            ResetSomeThingOnBallRoute();
        }
        #endregion
    }

    private void SomeThingOnBallRoute(GameObject interruptObject)
    { 
        Debug.Log("뭔가있어요! " + interruptObject);
        warningImageObject.SetActive(true); //뭔가 있어요(공)
        battleManager.interruptObject = interruptObject;
        battleManager.isSomethingOnBallMoveRoute = true;
    }

    public void ResetSomeThingOnBallRoute()
    {
        battleManager.interruptObject = null;
        battleManager.isSomethingOnBallMoveRoute = false;
    }

    public void ClearTargetCharacter()
    { 
        battleManager.targetCharacter = null;
        AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Off);
        Debug.Log("캐릭터 선택 해제");        
    }

    public void AllCharacterSelectionModeClear()
    {
        #region
        for (int i = 0; i < battleManager.playerCharacters.Length; i++)
        { 
            if((battleManager.playerCharacters[i] != null) && (battleManager.playerCharacters[i].GetComponent<Character>() != null))
            { 
                battleManager.playerCharacters[i].GetComponent<Character>().SetCharacterSelectionMode(false);
            }
        }

        for (int i = 0; i < battleManager.enemyCharacters.Length; i++)
        { 
            if((battleManager.enemyCharacters[i] != null) && (battleManager.enemyCharacters[i].GetComponent<Character>() != null))
            { 
                battleManager.enemyCharacters[i].GetComponent<Character>().SetCharacterSelectionMode(false);
            }
        }
        ClearCharacterInformation();
        #endregion
    }

    public void AllMapBlockSelectionModeClear()
    {
        #region
        for (int i = 0; i < battleManager.tileWidth; i++)
        {
            for (int j = 0; j < battleManager.tileHeight; j++)
            {
                if(battleManager.mapBlocks[i, j] != null)
                { 
                    battleManager.mapBlocks[i, j].UnSelectMapBlockByHighlightMode(MapBlock.Highlight.Select);
                }
            }
        }
        ClearTileInformation();
        #endregion
    }

    public void AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight mode)
    {
        #region
        for (int i = 0; i < battleManager.tileWidth; i++)
        {
            for (int j = 0; j < battleManager.tileHeight; j++)
            {
                if(battleManager.mapBlocks[i, j] != null)
                { 
                    battleManager.mapBlocks[i, j].UnSelectMapBlockByHighlightMode(mode);
                }
            }
        }     
        #endregion
    }

    public void SetBattleCommandWindow(bool mode, bool isMoveEnd = false)
    {
        if(mode == true)
        { 
            if (isMoveEnd == true)
            {
                StartCoroutine(CommandEffect());
                return;
            }

            if(battleManager.targetCharacter.GetComponent<Character>().GetIsHaveBall() == false)
            { 
                battleManager.battleCommandWindow.transform.GetChild(1).gameObject.SetActive(false);

                if(SkillDatabaseManager.instance.GetSkill(battleManager.targetCharacter.GetComponent<Character>().status.attackSkillNum).ignoreIsHaveBall == false)
                { 
                    battleManager.battleCommandWindow.transform.GetChild(0).gameObject.SetActive(false);      
                }    
            }

            if (battleManager.isCantUseSkill == true)
            {
                battleManager.battleCommandWindow.transform.GetChild(3).gameObject.SetActive(false);
            }

            battleManager.battleCommandWindow.SetActive(true);

            /* 버튼 이벤트 초기화 문제로 주석처리해둠
            for(int i = 0; i < battleManager.battleCommandWindow.transform.childCount; i++)
            {
                if(battleManager.battleCommandWindow.transform.GetChild(i).gameObject.activeSelf == true)
                { 
                    var eventSystem = EventSystem.current;
                    eventSystem.SetSelectedGameObject(battleManager.battleCommandWindow.transform.GetChild(i).gameObject, new BaseEventData(eventSystem));
                    break;
                }
            }
            */
        }
        else
        { 
            battleManager.battleCommandWindow.SetActive(false);
            for(int i = 0; i < battleManager.battleCommandWindow.transform.childCount; i++)
            {
                battleManager.battleCommandWindow.transform.GetChild(i).gameObject.SetActive(true);
            }                
        }
    }

    IEnumerator CommandEffect()
    {
        battleManager.battlePhaseManager.ChangePhase(BattlePhase.Stay);
        for(int i = 0; i < battleManager.battleCommandWindow.transform.childCount; i++)
        {
            battleManager.battleCommandWindow.transform.GetChild(i).gameObject.SetActive(false);
            battleManager.battleCommandWindow.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }

        battleManager.battleCommandWindow.SetActive(true);
        float delayTime = 0.2f;

        for (int i = battleManager.battleCommandWindow.transform.childCount - 1; i >= 0; i--)
        {
            switch(i)
            {
                case 0:
                    if (battleManager.targetCharacter.GetComponent<Character>().GetIsHaveBall() == false && SkillDatabaseManager.instance.GetSkill(battleManager.targetCharacter.GetComponent<Character>().status.attackSkillNum).ignoreIsHaveBall == false)
                    {
                        continue;
                    }
                    break;
                case 1:
                    if (battleManager.targetCharacter.GetComponent<Character>().GetIsHaveBall() == false)
                    {
                        continue;
                    }
                    break;
                case 3:
                    if (battleManager.isCantUseSkill == true)
                    {
                        continue;
                    }
                    break;
                default:
                    break;
            }

            battleManager.battleCommandWindow.transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(delayTime);
        }

        battleManager.battlePhaseManager.ChangePhase(BattlePhase.MyTurn_Command);
        for(int i = 0; i < battleManager.battleCommandWindow.transform.childCount; i++)
        {
            battleManager.battleCommandWindow.transform.GetChild(i).GetComponent<Button>().interactable = true;
        }
    }

    public void SetBattleMenuWindow(bool mode)
    { 
        if(mode == true)
        { 
            battleManager.SetBattleSceneHinderUI(false);
            battleManager.isMenu = true;
            ClearTileInformation();
            ClearCharacterInformation();
            AllCharacterSelectionModeClear();
            AllMapBlockSelectionModeClear();
            battleManager.battleMenuWindow.SetActive(true);
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(battleManager.battleMenuWindow.transform.GetChild(0).gameObject, new BaseEventData(eventSystem));
        }
        else
        { 
            ActiveFalseMenuWindow();
            SetPlayerStatusCanvas(false);
        }
    }

    public void ActiveFalseMenuWindow()
    { 
        battleManager.battleMenuWindow.SetActive(false);
        battleManager.isMenu = false;        
        battleManager.SetBattleSceneHinderUI(true);
    }

    public void CallShootRay()
    { 
        ShootRay();
    }

    public Sprite GetCharacterFaceMaskBackground(int faceNum)
    {
        if(faceNum < characterFaceMaskBackground.Length) //감정상태에 따른 테두리 뒷면 바꾸기
        { 
            return characterFaceMaskBackground[faceNum];            
        }
        else
        { 
            return GameManager.instance.GetBlank();
        } 
    }

    public void CallShootRayByReset()
    {
        #region
        AllCharacterSelectionModeClear();
        ClearTileInformation();
        ShootRay();
        #endregion
    }

    //전투로그용
    private void SetCharacterInformation(bool mode)
    {
        #region
        if (mode == true)
        {
            battleManager.characterInformation.SetActive(true);
            //battleLogWindow.SetActive(false); 수정
            GameManager.instance.isMouseOnBattleLog = false;
        }
        else
        {          
            battleManager.characterInformation.SetActive(false);
        }
        #endregion
    }

    private void CheckBattleLogWindowSet()
    {
        if ((battleManager.IsCanCheckInformationCheck() == true) && (battleManager.nowPhase != BattlePhase.MyTurn_ReadyToStart))
        {
            battleLogWindow.SetActive(true); 
        }
        else
        {
            battleLogWindow.SetActive(false);
        }
    }

    public void SetBattleLog(Character logTargetCharacter, string log)
    {
        if (battleLogCount >= battleLog.Count)//혹시 몰라서 초기화
        {
            battleLogCount = 0;
        }

        battleLog[battleLogCount].transform.SetAsLastSibling();
        battleLog[battleLogCount].GetComponent<BattleLog>().SetBattleLog(logTargetCharacter, log);

        battleLogCount++;
        if (battleLogCount >= battleLog.Count)
        {
            battleLogCount = 0;
        }
    }

    public int GetBattleLogCount()
    {
        return battleLogCount;
    }

    public int GetBattleLogLength()
    {
        return battleLog.Count;
    }

    public void DeleteBattleLog(int count)
    {
        for (int i = 0; i < count; i++)
        {
            battleLogCount--;
            if (battleLogCount < 0)
            {
                battleLogCount = battleLog.Count - 1;
            }

            battleLog[battleLogCount].GetComponent<BattleLog>().ResetBattleLog();
        }
    }

    //스테이터스 용
    #region
    public void SetPlayerStatusCanvas(bool mode)
    { 
        isEnemyCharacterStatus = false;
        battleStatusCanvas.SetActive(mode);
        if(mode == true)
        { 
            CreatePlayerStatusButtons(isEnemyCharacterStatus);
            SetCharacterStatus(isEnemyCharacterStatus, 0);
        }    
        else
        { 
            SetCharacterStatusFlip(false);
            SetCharacterSkillButton(0, false, isEnemyCharacterStatus);
            BreakPlayerStatusButtons();
            ActiveFalseMenuWindow();
        }           
    }

    public void SetCharacterStatus(bool isEnemy, int num)
    {
        #region
        if((num < GameManager.instance.PlayerCharacter.Length) && (num >= 0))
        { 
            SetCharacterSkillButton(num, false, isEnemy);

            CharacterStatus characterStatus = new CharacterStatus();

            if(isEnemy == false)
            { 
                if((num < GameManager.instance.battleManager.playerCharacters.Length) && (num >= 0))
                { 
                    characterStatus = GameManager.instance.PlayerCharacter[GameManager.instance.battleManager.playerCharacters[num].GetComponent<Character>().characterDatabaseNum];
                }
                else
                { 
                    Debug.LogWarning("SetCharacterStatus 잘못된 값이 들어왔습니다!");
                }
            }
            else
            { 
                if((num < GameManager.instance.battleManager.enemyCharacters.Length) && (num >= 0))
                { 
                    characterStatus = GameManager.instance.EnemyCharacter[GameManager.instance.battleManager.enemyCharacters[num].GetComponent<Character>().characterDatabaseNum];
                }
                else
                { 
                    Debug.LogWarning("SetCharacterStatus에 잘못된 값이 들어왔습니다!");
                }                
            }
            //--

            if(characterStatus.characterInformation.characterIllustration != null)
            { 
                spaceLeftR.transform.GetChild(0).GetComponent<Image>().sprite = characterStatus.characterInformation.characterIllustration;
            }
            else
            { 
                spaceLeftR.transform.GetChild(0).GetComponent<Image>().sprite = GameManager.instance.GetDefaultIllustration();
            }
            spaceRight.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.levelName + " : " + characterStatus.level;           
            spaceRight.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = (float)characterStatus.EXP/characterStatus.maxEXP;
            spaceRight.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = characterStatus.EXP + " / " + characterStatus.maxEXP;
            spaceRight.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = characterStatus.inGameName;
            spaceRight.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>().text = characterStatus.characterInformation.characterHeight + "cm";
            spaceRight.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<Text>().text = characterStatus.characterInformation.characterWeight + "kg";
            spaceRight.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = characterStatus.characterInformation.characterBirthMonth + "월 " + characterStatus.characterInformation.characterBirthDay + "일 " + "(" + characterStatus.characterInformation.characterAge+ ")";
            spaceRight.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = characterStatus.characterInformation.characterExplain;

            spaceRight.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.HPName;
            spaceRight.transform.GetChild(4).GetChild(0).GetChild(1).GetComponent<Text>().text = characterStatus.maxHP + "";

            spaceRight.transform.GetChild(4).GetChild(1).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.MPName;
            spaceRight.transform.GetChild(4).GetChild(1).GetChild(1).GetComponent<Text>().text = characterStatus.maxMP + "";

            spaceRight.transform.GetChild(4).GetChild(2).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.ATKName;
            spaceRight.transform.GetChild(4).GetChild(2).GetChild(1).GetComponent<Text>().text = characterStatus.ATK + "";

            spaceRight.transform.GetChild(4).GetChild(3).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.MAKName;
            spaceRight.transform.GetChild(4).GetChild(3).GetChild(1).GetComponent<Text>().text = characterStatus.MAK + "";

            spaceRight.transform.GetChild(4).GetChild(4).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.DEFName;
            spaceRight.transform.GetChild(4).GetChild(4).GetChild(1).GetComponent<Text>().text = characterStatus.DEF + "";

            spaceRight.transform.GetChild(4).GetChild(5).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.MDFName;
            spaceRight.transform.GetChild(4).GetChild(5).GetChild(1).GetComponent<Text>().text = characterStatus.MDF + "";

            spaceRight.transform.GetChild(4).GetChild(6).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.rangeName;
            spaceRight.transform.GetChild(4).GetChild(6).GetChild(1).GetComponent<Text>().text = characterStatus.range + "";

            spaceRight.transform.GetChild(4).GetChild(7).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.moveName;
            spaceRight.transform.GetChild(4).GetChild(7).GetChild(1).GetComponent<Text>().text = characterStatus.move + "";

            spaceRight.transform.GetChild(5).GetChild(0).GetComponent<Text>().text = NameDatabaseManager.SympathyTypeRationalName + ": " + SympathyDatabaseManager.instance.GetSympathyDataByCharacterSympathyType(characterStatus.sympathyType).minRational + " ~ " + SympathyDatabaseManager.instance.GetSympathyDataByCharacterSympathyType(characterStatus.sympathyType).maxRational;
            spaceRight.transform.GetChild(5).GetChild(1).GetComponent<Text>().text = NameDatabaseManager.SympathyTypeAngerName + ": " + SympathyDatabaseManager.instance.GetSympathyDataByCharacterSympathyType(characterStatus.sympathyType).minAnger+ " ~ " + SympathyDatabaseManager.instance.GetSympathyDataByCharacterSympathyType(characterStatus.sympathyType).maxAnger;
            spaceRight.transform.GetChild(5).GetChild(2).GetComponent<Text>().text = NameDatabaseManager.SympathyTypeEnjoyName + ": " + SympathyDatabaseManager.instance.GetSympathyDataByCharacterSympathyType(characterStatus.sympathyType).minEnjoy+ " ~ " + SympathyDatabaseManager.instance.GetSympathyDataByCharacterSympathyType(characterStatus.sympathyType).maxEnjoy;
            SetCharacterSkillButton(num, true, isEnemy);
                
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

    private void CreatePlayerStatusButtons(bool isEnemy)
    { 
        #region
        if(isEnemy == false)
        { 
            for(int i = 0; i < GameManager.instance.battleManager.playerCharacters.Length; i++)
            { 
                if(characterStatusButtonPrefab != null)
                { 
                    GameObject characterStatusButton = Instantiate(characterStatusButtonPrefab);
                    characterStatusButton.name = "PlayerCharacterStatusButton_" + i;
                    characterStatusButton.GetComponent<CharacterStatusButton>().SetBattleCharacterStatusButton(isEnemy, i);

                    characterStatusButton.transform.SetParent(characterStatusButtonScrollviewContent.transform);  
                    characterStatusButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); 
                }         
            }
        }
        else
        { 
            for(int i = 0; i < GameManager.instance.battleManager.enemyCharacters.Length; i++)
            { 
                if(characterStatusButtonPrefab != null)
                { 
                    GameObject characterStatusButton = Instantiate(characterStatusButtonPrefab);
                    characterStatusButton.name = "EnemyCharacterStatusButton_" + i;
                    characterStatusButton.GetComponent<CharacterStatusButton>().SetBattleCharacterStatusButton(isEnemy, i);

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

    private void SetCharacterSkillButton(int num, bool mode, bool isEnemy)
    { 
        #region
        SetSkillExplainText("");
        if(mode == true)
        { 
            CharacterStatus characterStatus = new CharacterStatus();

            if(isEnemy == false)
            { 
                if((num < GameManager.instance.battleManager.playerCharacters.Length) && (num >= 0))
                { 
                    characterStatus = GameManager.instance.battleManager.playerCharacters[num].GetComponent<Character>().status;
                }
                else
                { 
                    Debug.LogWarning("SetCharacterSkillButton에 잘못된 값이 들어왔습니다!");
                }
            }
            else
            { 
                if((num < GameManager.instance.battleManager.enemyCharacters.Length) && (num >= 0))
                { 
                    characterStatus = GameManager.instance.battleManager.enemyCharacters[num].GetComponent<Character>().status;
                }
                else
                { 
                    Debug.LogWarning("SetCharacterSkillButton에 잘못된 값이 들어왔습니다!");
                }                
            }

            for(int i = 0; i < characterStatus.characterSkill.Length; i++)
            { 
                if(characterStatus.characterSkill[i] == true)
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
    

    public void ChangeStatusDataIsEnemy()
    { 
        if(isEnemyCharacterStatus == true)
        { 
          isEnemyCharacterStatus = false;
        }
        else
        { 
            isEnemyCharacterStatus = true;
        }

        SetCharacterSkillButton(0, false, isEnemyCharacterStatus);
        BreakPlayerStatusButtons();
        CreatePlayerStatusButtons(isEnemyCharacterStatus);
        SetCharacterStatus(isEnemyCharacterStatus, 0);
    }
    #endregion
    //----

    public Sprite[] GetCharacterFaceMaskBackground()
    {
        return characterFaceMaskBackground;
    }
}
