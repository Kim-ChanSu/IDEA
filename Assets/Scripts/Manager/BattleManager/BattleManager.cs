using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum BattlePhase
{
    MyTurn_ReadyToStart, // 배틀 준비시
    MyTurn_Start, // 배틀 시작시, 적의 턴 종료
    MyTurn_CharcterSelect, // 캐릭터 선택
    MyTurn_Moving, // 이동 대상 선택,캐릭터 선택 후 이동를 선택하고 있는 상태 이동를 선택하고 있는 상태
    MyTurn_Command, // 이동 후 명령 선택, 대기 하면 적의 턴 시작시 까지 날린다.
    MyTurn_SkillTargeting, // 공격 스킬을 선택한 상태, 공격 대상을 선택하면 공격이 나간다.
    MyTurn_ItemTargeting, // 사용할 아이템을 선택한 상태, 아이템 사용 대상을 선택하면 아이템이 사용된다.
    MyTurn_Result, // 행동 결과 표시
    EnemyTurn_ReadyToStart, // 적군 배틀 준비
    EnemyTurn_Start, // 적군 배틀 시작
    EnemyTurn_Moving, // 적군 이동
    EnemyTurn_Command, // 적군 공격, 스킬 사용
    EnemyTurn_Result, // 적군 행동 결과 표시
    IsAnimation,
    IsSkill,
    IsTurnEffect,
    IsEvent,
    GameClear,
    GameOver,
    GameResult,
    Stay //의미없는 페이즈 기다리면 페이즈 바뀌는 경우 이걸로 페이즈 돌리기
}
  

public class BattleManager : MonoBehaviour
{
    private const int TILEZPOS = 12;

    private int thisObjectXPos; 
    private int thisObjectYPos;
    private int startCreateTileXpos;
    private int startCreateTileYpos;

    private bool isCharacterSettingComplete = false;

    [HideInInspector]
    public BattlePhaseManager battlePhaseManager;
    [HideInInspector]
    public BattleGetInformationManager battleGetInformationManager;
    [HideInInspector]
    public BattleCharacterManager battleCharacterManager;
    [HideInInspector]
    public BattleCommandManager battleCommandManager;
    [HideInInspector]
    public BattleEventManager battleEventManager;
    [HideInInspector]
    public AIManager aiManager;

    [HideInInspector]
    public int tileWidth;
    [HideInInspector]
    public int tileHeight; 
    [HideInInspector]
    public GameObject targetCharacter; //현재 자신 턴인 캐릭터
    [HideInInspector]
    public GameObject[] playerCharacters;
    [HideInInspector]
    public GameObject[] enemyCharacters;

    [HideInInspector]
    public MapBlock[,] mapBlocks;
    [HideInInspector]
    public List<MapBlock> strategicPointMapBlocks = new List<MapBlock>();
    private int playerStrategicPointCount = 0;
    private int enemyStrategicPointCount = 0;

    [HideInInspector]
    public Skill useSkill;

    public int defaultDamage;
    public int defaultHeal;

    public GameObject tilePrefab;    
    public GameObject characterPrefab;
    public GameObject playerSpawner;
    public GameObject tileInformation;
    public GameObject characterInformation;
    public GameObject battleCommandWindow;
    public GameObject battleSkillWindow;

    //메뉴용
    public GameObject battleMenuWindow;
    //--

    [HideInInspector]
    public int selectTileXpos;
    [HideInInspector]
    public int selectTileYpos;   
    [HideInInspector]
    public bool cantMoveOnThisTile; //안씀
    [HideInInspector]
    public bool isSomethingOnBallMoveRoute;
    [HideInInspector]
    public bool isAnimation;
    [HideInInspector]
    public bool isBattleSkillWindow;
    [HideInInspector]
    public bool isMenu;
    [HideInInspector]
    public BattlePhase nowPhase;
    [HideInInspector]
    public int battleTurnCount = 0; //현재 턴 수

    [HideInInspector]
    public bool isMoveReturn = false; //이동 후 원위치로 되돌아 갔는가 여부

    [HideInInspector]
    public GameObject skillUser; //스킬사용자
    [HideInInspector]
    public GameObject skillTarget; //스킬 대상
    [HideInInspector]
    public GameObject interruptObject; // 방해 오브젝트
    [HideInInspector]
    public int skillHPDamage;
    [HideInInspector]
    public int skillMPDamage;
    [HideInInspector]
    public int skillSympathyDamage;
    [HideInInspector]
    public int[] skillStatusEffectNum = new int[1] {-1};
    [HideInInspector]
    public int[] skillDispelStatusEffectNum = new int[1] {-1};
    [HideInInspector]
    public int ignoreSkillStatusEffectNum = -1;
    private bool isUseBallSkillSuccess = false;

    public int interruptBallPercentage = 50;
    public int defaultBallBoundRange = 3;
    [HideInInspector]
    public List<GameObject> boundAblePosition = new List<GameObject>(); 
    private bool findBoundAbleCharacter = false;

    //전투씬 상단 UI용 
    [SerializeField]
    private GameObject battleTopWindow;
    [SerializeField]
    private GameObject battleTopTurnText;
    [SerializeField]
    private BattleTopCharacterInformWindow battleTopPlayerInformCharacterWindow;
    [SerializeField]
    private BattleTopCharacterInformWindow battleTopEnemyInformCharacterWindow;

    [HideInInspector]
    public bool isCantUseSkill = false;

    void Start()
    {
        InitializeBattleManager();
    }

    void Update()
    { 
        if(nowPhase != BattlePhase.GameOver && nowPhase != BattlePhase.GameClear && nowPhase != BattlePhase.GameResult && nowPhase != BattlePhase.IsTurnEffect)
        { 
            UpdateCharacterStatus();
        }
    }

    private void InitializeBattleManager()
    {
        #region
        battlePhaseManager = this.gameObject.GetComponent<BattlePhaseManager>();
        battleGetInformationManager = this.gameObject.GetComponent<BattleGetInformationManager>();
        battleCharacterManager = this.gameObject.GetComponent<BattleCharacterManager>();
        battleCommandManager = this.gameObject.GetComponent<BattleCommandManager>();
        battleEventManager = this.gameObject.GetComponent<BattleEventManager>();
        aiManager = this.gameObject.GetComponent<AIManager>();
        isCharacterSettingComplete = false;
        battleTurnCount = 0;
        CheckTileLength();
        CreateTile();
        InitializeMapBlock();
        CheckStrategicPointMapBlock();

        GameManager.instance.SetBattleHPbyTeamHealth();
        InitializePlayerCharacter();
        InitializeEnemyCharacter();
        GameManager.instance.battleManager = this;
        SetBattleSceneHinderUI(false);
        StatusEffectDatabaseManager.instance.ClearStrategicPointEffect();
        UpdateStrategicPointCount();
        InitBattleTopWindow();
        #endregion
    }

    /*
    private void Test() 0,0 버그 실험용
    {
        if (true)
        {
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(new Vector2 (0, 0), Vector2.zero);
            for(int r = 0; r < hits.Length; r++)
            { 
                Debug.LogError(hits[r].transform.gameObject);
                Debug.LogError(hits[r].transform.gameObject.transform.position);
            }
        }
    }
    */

    private bool CheckNumber(int n)
    {
        #region
        if ((n % 2 == 0) && (n != 0))
        {
            return true;
        }
        else if (n % 2 == 1)
        {
            return false;
        }
        else
        {
            return false;
        }
        #endregion
    }

    private void CheckTileLength()
    { 
        #region
        tileWidth = (int)this.gameObject.GetComponent<BoxCollider2D>().size.x / GameManager.TILESIZE;
        if(CheckNumber(tileWidth) == true)//콜라이더의 Size값이 짝수면 콜라이더하고 타일하고 맞지 않기에 보정 해주는 코드
        { 
            this.gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f * GameManager.TILESIZE, this.gameObject.GetComponent<BoxCollider2D>().offset.y);
        }
        tileHeight = (int)this.gameObject.GetComponent<BoxCollider2D>().size.y / GameManager.TILESIZE;
        if(CheckNumber(tileHeight) == true)
        { 
            this.gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(this.gameObject.GetComponent<BoxCollider2D>().offset.x, -0.5f * GameManager.TILESIZE);
        }

        Debug.Log("타일의 x길이는 : " + tileWidth);
        Debug.Log("타일의 y길이는 : " + tileHeight); 
        thisObjectXPos = (int)this.gameObject.transform.position.x;
        thisObjectYPos = (int)this.gameObject.transform.position.y;
        SetStartCreateTilePos();
        #endregion
    }

    private void CreateTile()
    { 
        #region
        if(tilePrefab != null)
        {
            int CreateTileXpos = startCreateTileXpos;
            int CreateTileYpos = startCreateTileYpos;
            for(int i = 0; i < tileWidth ; i ++)
            { 
                for(int j = 0; j < tileHeight; j ++)
                { 
                    GameObject newTile = null;
                    // 기존에 깔아둔 타일이 있나 체크                    
                    RaycastHit2D[] hits;
                    hits = Physics2D.RaycastAll(new Vector2 (CreateTileXpos, CreateTileYpos), Vector2.zero);
                    bool isTileOn = false; //같은 위치에 타일이 여러개 있는지 검사용
                    for(int r = 0; r < hits.Length; r++)
                    { 
                        if((hits[r].transform.gameObject.GetComponent<MapBlock>() == true) && (hits[r].transform.gameObject.CompareTag("Tile"))) //좌표는 이동하는데 함수 기능 끝나기 전까지 0,0 에서 걸리네 이 뭔..
                        { 
                            if (isTileOn == false)
                            {
                                newTile = hits[r].transform.gameObject;
                                isTileOn = true;
                            }
                            else
                            {
                                Debug.LogWarning(CreateTileXpos + "," + CreateTileYpos + " 좌표에 중복된 타일이있습니다!");
                                Destroy(hits[r].transform.gameObject);
                            }

                        }
                    }
                    //-------------------------

                    if (newTile == null)
                    {
                        newTile = Instantiate(tilePrefab);
                    }
                    newTile.transform.SetParent(this.gameObject.transform.GetChild(0).transform);
                    newTile.transform.SetAsLastSibling();
                    newTile.name = "Tile(" + CreateTileXpos + "," + CreateTileYpos + ")";
                    newTile.gameObject.transform.position = new Vector3 (CreateTileXpos,CreateTileYpos, TILEZPOS);

                    newTile.GetComponent<BoxCollider2D>().enabled = false; //타일 콜라이더 끄고 (0,0 버그 회피용)

                    CreateTileYpos += GameManager.TILESIZE;
                }
                CreateTileYpos = startCreateTileYpos;
                CreateTileXpos += GameManager.TILESIZE;
            }
        }
        #endregion
    }
    
    private void InitializeMapBlock()
    {
        #region
        int index = 0;
        int CreateTileXpos = startCreateTileXpos;
        int CreateTileYpos = startCreateTileYpos;
        mapBlocks = new MapBlock[tileWidth, tileHeight];
        for (int i = 0; i < tileWidth; i++)
        {
            for (int j = 0; j < tileHeight; j++)
            {
                var mapBlock = this.gameObject.transform.GetChild(0).GetChild(index).GetComponent<MapBlock>();
                mapBlocks[i, j] = mapBlock;
                mapBlock.mapBlockXpos = CreateTileXpos;
                mapBlock.mapBlockYpos = CreateTileYpos;

                mapBlock.gameObject.GetComponent<BoxCollider2D>().enabled = true; //타일 콜라이더 키고 (0,0 버그 회피용)

                index++;
                CreateTileYpos += GameManager.TILESIZE;
            }
            CreateTileYpos = startCreateTileYpos;
            CreateTileXpos += GameManager.TILESIZE;
        }
        #endregion
    }

    private void CheckStrategicPointMapBlock() // InitializeMapBlock보다 느리게 호출 되어야함
    {
        #region
        strategicPointMapBlocks = new List<MapBlock>();

        for (int i = 0; i < mapBlocks.GetLength(0); i++)
        {
            for (int j = 0; j < mapBlocks.GetLength(1); j++)
            {
                if (mapBlocks[i, j].tileType == TileType.StrategicPoint)
                {
                    strategicPointMapBlocks.Add(mapBlocks[i, j]);
                }
            }
        }

        if (strategicPointMapBlocks.Count <= 0)
        {
            aiManager.isStrategicPointOnMap = false;
        }
        else
        {
            aiManager.isStrategicPointOnMap = true;
        }

        Debug.Log("검사된 거점의 수는 " + strategicPointMapBlocks.Count);
        #endregion
    }

    private void SetStartCreateTilePos() // 지금은 GameManager.TILESIZE <- 이거 바뀔때마다 함수 안에 내용 갈아줘야함
    { 
        #region
        //startCreateTileXpos = thisObjectXPos - (int)this.gameObject.GetComponent<BoxCollider2D>().size.x / 2 + 1;
        //startCreateTileYpos = thisObjectYPos - (int)this.gameObject.GetComponent<BoxCollider2D>().size.y / 2 + 1;     
        
        int xColliderChecker = thisObjectXPos;
        int yColliderChecker = thisObjectYPos;

        for(int i = 0; i < tileWidth; i++)
        { 
            bool isBattleManagerOn = false;
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(new Vector2 (xColliderChecker, 0), Vector2.zero);
            for(int j = 0; j < hits.Length; j++)
            { 
                if(hits[j].transform.gameObject == this.gameObject)
                { 
                    isBattleManagerOn = true;
                }
            }

            if(isBattleManagerOn == true)
            { 
                xColliderChecker -= GameManager.TILESIZE;
            }
            else
            { 
                xColliderChecker += GameManager.TILESIZE;
                break;
            }
        }
       
        for(int i = 0; i < tileHeight; i++)
        { 
            bool isBattleManagerOn = false;
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(new Vector2 (0, yColliderChecker), Vector2.zero);
            for(int j = 0; j < hits.Length; j++)
            { 
                if(hits[j].transform.gameObject == this.gameObject)
                { 
                    isBattleManagerOn = true;
                }
            }

            if(isBattleManagerOn == true)
            { 
                yColliderChecker -= GameManager.TILESIZE;
            }
            else
            { 
                yColliderChecker += GameManager.TILESIZE;
                break;
            }
        }
        
        startCreateTileXpos = xColliderChecker;
        startCreateTileYpos = yColliderChecker;
        #endregion
    }

    public void ClearMapBlockNavTile(int length)
    { 
        for (int i = 0; i < tileWidth; i++)
        {
            for (int j = 0; j < tileHeight; j++)
            {
                if(mapBlocks[i, j] != null)
                { 
                    mapBlocks[i, j].ClearNavTileByLength(length);
                }
            }
        }        
    }
    
   
    public void UpdateStrategicPointCount()
    {
        playerStrategicPointCount = 0;
        enemyStrategicPointCount = 0;

        for (int i = 0; i < strategicPointMapBlocks.Count; i++)
        {
            if (strategicPointMapBlocks[i].tileOwner == TileOwner.Player)
            {
                playerStrategicPointCount = playerStrategicPointCount + 1;
            }
            else if (strategicPointMapBlocks[i].tileOwner == TileOwner.Enemy)
            {
                enemyStrategicPointCount = enemyStrategicPointCount + 1;
            }
        }
    }
    
    public int GetPlayerStrategicPointCount()
    {
        return playerStrategicPointCount;
    }

    public int GetEnemyStrategicPointCount()
    {
        return enemyStrategicPointCount;
    }

    public void SetCharacterSettingComplete(bool mode)
    {
        isCharacterSettingComplete = mode;
    }

    public bool GetCharacterSettingComplete()
    {
        return isCharacterSettingComplete;
    }


    public void SetIsAnimation(bool mode) //이거 의미 없는듯
    { 
        if(mode == true)
        { 
            isAnimation = true;
            battlePhaseManager.ChangePhase(BattlePhase.IsAnimation);
        }
        else
        { 
            isAnimation = false;    
        }

    }

    private void InitializePlayerCharacter()
    { 
        #region
        if(characterPrefab != null)
        { 
            playerCharacters = new GameObject[GameManager.instance.selectPlayerCharacterNum.Length];
            for(int i = 0; i < GameManager.instance.selectPlayerCharacterNum.Length; i++)
            { 
                if((GameManager.instance.selectPlayerCharacterNum[i] < GameManager.instance.PlayerCharacter.Length) && (GameManager.instance.selectPlayerCharacterNum[i] >= 0))
                { 
                    GameObject newPlayerCharacter = Instantiate(characterPrefab);
                    newPlayerCharacter.transform.SetParent(this.gameObject.transform.GetChild(1).transform);
                    var playerCharacter = newPlayerCharacter;
                    playerCharacters[i] = playerCharacter;

                    playerCharacters[i].GetComponent<Character>().characterDatabaseNum = GameManager.instance.selectPlayerCharacterNum[i];

                    //------------스테이터스 복제하기
                    //playerCharacters[i].GetComponent<Character>().status = GameManager.instance.PlayerCharacter[GameManager.instance.selectPlayerCharacterNum[i]]; 기존꺼 삭제해도 됨
                    InitializePlayerCharacterStatus(i);
                    //---------------------------------------------------------------------
                    newPlayerCharacter.name = playerCharacters[i].GetComponent<Character>().status.name;
                    playerCharacters[i].SetActive(false);

                    if (i == GameManager.instance.playerLeaderNum)
                    {
                        playerCharacters[i].GetComponent<Character>().SetLeaderIcon(true);
                    }
                    else
                    {
                        playerCharacters[i].GetComponent<Character>().SetLeaderIcon(false);
                    }

                    CheckCharacterAnimation(playerCharacters[i]);               
                }
                else
                { 
                    Debug.LogWarning("GameManager.instance.selectPlayerCharacterNum[" + i + "]의 값이 GameManager.instance.PlayerCharacter[]보다 크거나 0보다 작습니다");    
                    Debug.LogWarning("들어 온 값 = " + GameManager.instance.selectPlayerCharacterNum[i]);    

                    GameObject[] saveGameObject = playerCharacters;
                    playerCharacters = new GameObject[i];

                    for(int j = 0; j < i; j++)
                    { 
                        playerCharacters[j] = saveGameObject[j];
                    }
                    break;
                }                
            }

            playerSpawner.SetActive(true);
            playerSpawner.GetComponent<PlayerSpawner>().StartPlayerSetting(this);
        }
        #endregion    
    }

    private void InitializeEnemyCharacter()
    { 
        #region
        if(this.gameObject.transform.GetChild(2).transform.childCount <= GameManager.instance.EnemyCharacter.Length)
        { 
            enemyCharacters = new GameObject[this.gameObject.transform.GetChild(2).transform.childCount];
        }
        else
        { 
            enemyCharacters = new GameObject[GameManager.instance.EnemyCharacter.Length];
        }

        for(int i = 0; i < this.gameObject.transform.GetChild(2).transform.childCount; i++)
        { 
            if(this.gameObject.transform.GetChild(2).GetChild(i).GetComponent<Character>() == true)
            { 
                if(i < GameManager.instance.EnemyCharacter.Length)
                {
                    enemyCharacters[i] = this.gameObject.transform.GetChild(2).GetChild(i).gameObject;
                    enemyCharacters[i].GetComponent<Character>().characterDatabaseNum = i;

                    enemyCharacters[i].GetComponent<Character>().status = new CharacterStatus();
                    enemyCharacters[i].GetComponent<Character>().status = CharacterDatabaseManager.instance.DeepCopyCharacterStatus(GameManager.instance.EnemyCharacter[i]);
                    enemyCharacters[i].name = enemyCharacters[i].GetComponent<Character>().status.name;

                    // 스킬 넣기
                    enemyCharacters[i].GetComponent<Character>().status.characterSkill = new bool[SkillDatabaseManager.instance.SkillLength()]; 
                    for(int j = 0; j < enemyCharacters[i].GetComponent<Character>().status.firstCharacterSkillNum.Length; j++)
                    { 
                        if(enemyCharacters[i].GetComponent<Character>().status.firstCharacterSkillNum[j] < SkillDatabaseManager.instance.SkillLength() == true)
                        { 
                            enemyCharacters[i].GetComponent<Character>().status.characterSkill[enemyCharacters[i].GetComponent<Character>().status.firstCharacterSkillNum[j]] = true;  
                        }
                        else
                        { 
                            Debug.LogWarning(enemyCharacters[i].GetComponent<Character>().status.name + "의 characterSkill 변수 " + j +"번 배열에 잘못된 값이 들어왔습니다! 잘못 들어온 값 " + enemyCharacters[i].GetComponent<Character>().status.firstCharacterSkillNum[j]);
                        }
                    }  
                    //aitype
                    //enemyCharacters[i].GetComponent<Character>().aiType = GameManager.instance.EnemyCharacter[i].aiType ;
                    InitializeEnemyCharacterStatus(i);
                    CheckCharacterAnimation(enemyCharacters[i]);

                    if (i == GameManager.instance.enemyLeaderNum)
                    {
                        enemyCharacters[i].GetComponent<Character>().SetLeaderIcon(true);
                    }
                    else
                    {
                        enemyCharacters[i].GetComponent<Character>().SetLeaderIcon(false);
                    }
                }
                else
                { 
                    Debug.LogWarning("배틀맵에 존재하는 적의 수가 스테이지에 설정된 적의 수 보다 많으므로 파괴합니다!");
                    Destroy(this.gameObject.transform.GetChild(2).GetChild(i).gameObject);
                }
                
            }
        }

        //혹시 몰라 이전코드 백업
        #region
        /* 
        GameManager.instance.EnemyCharacter = new CharacterStatus[enemyCharacters.Length];
        GameManager.instance.InitializeEnemyCharacterStatusClass();

        for(int i = 0; i < this.gameObject.transform.GetChild(2).transform.childCount; i++)
        { 
            if(this.gameObject.transform.GetChild(2).GetChild(i).GetComponent<Character>() == true)
            { 
                enemyCharacters[i] = this.gameObject.transform.GetChild(2).GetChild(i).gameObject;
                enemyCharacters[i].GetComponent<Character>().characterDatabaseNum = i;

                enemyCharacters[i].GetComponent<Character>().status.characterSkill = new bool[SkillDatabaseManager.instance.SkillLength()]; 
                for(int j = 0; j < enemyCharacters[i].GetComponent<Character>().status.firstCharacterSkillNum.Length; j++)
                { 
                    if(enemyCharacters[i].GetComponent<Character>().status.firstCharacterSkillNum[j] < SkillDatabaseManager.instance.SkillLength() == true)
                    { 
                        enemyCharacters[i].GetComponent<Character>().status.characterSkill[enemyCharacters[i].GetComponent<Character>().status.firstCharacterSkillNum[j]] = true;  
                    }
                    else
                    { 
                        Debug.LogWarning(enemyCharacters[i].GetComponent<Character>().status.name + "의 characterSkill 변수 " + j +"번 배열에 잘못된 값이 들어왔습니다! 잘못 들어온 값 " + enemyCharacters[i].GetComponent<Character>().status.firstCharacterSkillNum[j]);
                    }
                }

                InitializeEnemyCharacterStatus(i);
            }
        }
        */
        #endregion
        #endregion
    }

    private void InitializePlayerCharacterStatus(int i)
    { 
        #region
        playerCharacters[i].GetComponent<Character>().status = CharacterDatabaseManager.instance.DeepCopyCharacterStatus(GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum]);     
        playerCharacters[i].GetComponent<SpriteRenderer>().sprite = playerCharacters[i].GetComponent<Character>().status.inGameCharacter;
        #endregion
    }

    private void InitBattleTopWindow()
    {
        #region
        battleTopPlayerInformCharacterWindow.InitBattleTopCharacterInformWindow(this, false);
        battleTopEnemyInformCharacterWindow.InitBattleTopCharacterInformWindow(this, true);        
        #endregion
    }

    private void CheckCharacterAnimation(GameObject character)
    { 
        if(character.GetComponent<Character>().status.characterAnimation != null)
        { 
            GameObject characterAnimation = Instantiate(character.GetComponent<Character>().status.characterAnimation);
            characterAnimation.transform.SetParent(character.transform);
            if(characterAnimation.GetComponent<CharacterAnimationController>() == false)
            { 
                Debug.LogWarning(characterAnimation + "이 CharacterAnimationControllert 스크립트를 들고 있지 않습니다!");
                return;    
            }

            characterAnimation.GetComponent<CharacterAnimationController>().SetCharacterAnimationController();
            characterAnimation.name = character.GetComponent<Character>().status.name + "_Animation";
            character.GetComponent<Character>().SetCharacterAnimation(true);
        }

    }

    public void CallInitializePlayerCharacterStatus(int i)
    { 
        InitializePlayerCharacterStatus(i);
    }

    private void InitializeEnemyCharacterStatus(int i)
    { 
        #region
        enemyCharacters[i].GetComponent<SpriteRenderer>().sprite = enemyCharacters[i].GetComponent<Character>().status.inGameCharacter;
        //↓예전꺼 백업
        //GameManager.instance.EnemyCharacter[i] = CharacterDatabaseManager.instance.DeepCopyCharacterStatus(enemyCharacters[i].GetComponent<Character>().status);
        #endregion
    }

    public void UpdateCharacterStatus() //나중에 상태이상이라든가 버프라든가 나오면 수정해야함(정확히 뒤에 + 넣어야함) 이제 HP MP쪽 다시 생각해봐야함
    {
        #region
        if(isCharacterSettingComplete == true)
        {
            CheckStatus();
            CheckSympathyEffect(); // 순서 이거 맞음 CheckStatus여기서 아웃인지 아닌지 검사함
            CheckStrategicPointEffect();         
            
            for(int i = 0; i < playerCharacters.Length; i++)
            { 
                float ATKChange = 1;
                float MAKChange = 1;
                float DEFChange = 1;
                float MDFChange = 1;
                int moveChange = 0;
                int rangeChange = 0;
                float changeCheck = StatusEffectDatabaseManager.instance.GetMinCheckStatusChange();

                //타일버프 검사
                #region
                TileBuff tilebuff = playerCharacters[i].GetComponent<Character>().GetCharacterTileBuff();

                if(tilebuff.ATKChange > changeCheck)
                { 
                    ATKChange = AddStatusEffectChange(ATKChange, tilebuff.ATKChange);
                }

                if(tilebuff.MAKChange > changeCheck)
                { 
                    MAKChange = AddStatusEffectChange(MAKChange, tilebuff.MAKChange);
                }

                if(tilebuff.DEFChange > changeCheck)
                { 
                    DEFChange = AddStatusEffectChange(DEFChange, tilebuff.DEFChange);
                }

                if(tilebuff.MDFChange > changeCheck)
                { 
                    MDFChange = AddStatusEffectChange(MDFChange, tilebuff.MDFChange);
                }

                if(tilebuff.moveChange != 0)
                { 
                    moveChange += tilebuff.moveChange;
                }

                if(tilebuff.rangeChange != 0)
                { 
                    rangeChange += tilebuff.rangeChange;
                }
                #endregion

                for(int j = 0; j < playerCharacters[i].GetComponent<Character>().characterStatusEffect.Length; j++)
                { 
                    if(playerCharacters[i].GetComponent<Character>().characterStatusEffect[j].isOn == true)
                    { 
                        StatusEffect checkEffect = StatusEffectDatabaseManager.instance.GetStatusEffect(j);

                        if(checkEffect.ATKChange > changeCheck)
                        { 
                            ATKChange = AddStatusEffectChange(ATKChange, checkEffect.ATKChange);
                        }

                        if(checkEffect.MAKChange > changeCheck)
                        { 
                            MAKChange = AddStatusEffectChange(MAKChange, checkEffect.MAKChange);
                        }

                        if(checkEffect.DEFChange > changeCheck)
                        { 
                            DEFChange = AddStatusEffectChange(DEFChange, checkEffect.DEFChange);
                        }

                        if(checkEffect.MDFChange > changeCheck)
                        { 
                            MDFChange = AddStatusEffectChange(MDFChange, checkEffect.MDFChange);
                        }

                        if(checkEffect.moveChange != 0)
                        { 
                            moveChange += checkEffect.moveChange;
                        }

                        if(checkEffect.rangeChange != 0)
                        { 
                            rangeChange += checkEffect.rangeChange;
                        }
                    }
                }

                playerCharacters[i].GetComponent<Character>().status.level = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].level;
                playerCharacters[i].GetComponent<Character>().status.maxEXP = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].maxEXP;
                playerCharacters[i].GetComponent<Character>().status.EXP = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].EXP;

                playerCharacters[i].GetComponent<Character>().status.maxHP = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].maxHP;
                playerCharacters[i].GetComponent<Character>().status.HP = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].HP;
                playerCharacters[i].GetComponent<Character>().status.maxMP = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].maxMP;
                playerCharacters[i].GetComponent<Character>().status.MP = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].MP;

                playerCharacters[i].GetComponent<Character>().status.maxSympathy = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].maxSympathy;
                playerCharacters[i].GetComponent<Character>().status.Sympathy = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].Sympathy;

                SympathyType beforeSympathyType = playerCharacters[i].GetComponent<Character>().sympathyType;
                playerCharacters[i].GetComponent<Character>().sympathyType = SympathyDatabaseManager.instance.CheckSympathy(playerCharacters[i].GetComponent<Character>()); //공감 상태값 가져오기
                if ((beforeSympathyType != playerCharacters[i].GetComponent<Character>().sympathyType) && (isMoveReturn != true))
                {
                    /* 수정
                    if (i < battleTopPlayerInform.Length)
                    {
                        battleTopPlayerInform[i].GetComponent<BattleTopCharacterInform>().ChangeFaceNum(playerCharacters[i].GetComponent<Character>());
                    }
                    */
                    SetBattleLog(playerCharacters[i].GetComponent<Character>(), playerCharacters[i].GetComponent<Character>().status.inGameName + "의 감정은 " + GameManager.instance.GetSympathyTypeName(playerCharacters[i].GetComponent<Character>().sympathyType) + "이(가) 되었다.");
                }
                playerCharacters[i].GetComponent<Character>().status.ATK = (int)(GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].ATK * ATKChange);
                playerCharacters[i].GetComponent<Character>().status.MAK = (int)(GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].MAK * MAKChange);
                playerCharacters[i].GetComponent<Character>().status.DEF = (int)(GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].DEF * DEFChange);
                playerCharacters[i].GetComponent<Character>().status.MDF = (int)(GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].MDF * MDFChange);
                playerCharacters[i].GetComponent<Character>().status.range = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].range + rangeChange;
                playerCharacters[i].GetComponent<Character>().status.move = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].move + moveChange;                  
            }

            if(isMoveReturn == true)
            { 
                isMoveReturn = false;
            }
        
            for(int i = 0; i < enemyCharacters.Length; i++)
            { 
                float ATKChange = 1;
                float MAKChange = 1;
                float DEFChange = 1;
                float MDFChange = 1;
                int moveChange = 0;
                int rangeChange = 0;
                float changeCheck = StatusEffectDatabaseManager.instance.GetMinCheckStatusChange();

                //타일버프 검사
                #region
                TileBuff tilebuff = enemyCharacters[i].GetComponent<Character>().GetCharacterTileBuff();

                if(tilebuff.ATKChange > changeCheck)
                { 
                    ATKChange = AddStatusEffectChange(ATKChange, tilebuff.ATKChange);
                }

                if(tilebuff.MAKChange > changeCheck)
                { 
                    MAKChange = AddStatusEffectChange(MAKChange, tilebuff.MAKChange);
                }

                if(tilebuff.DEFChange > changeCheck)
                { 
                    DEFChange = AddStatusEffectChange(DEFChange, tilebuff.DEFChange);
                }

                if(tilebuff.MDFChange > changeCheck)
                { 
                    MDFChange = AddStatusEffectChange(MDFChange, tilebuff.MDFChange);
                }

                if(tilebuff.moveChange != 0)
                { 
                    moveChange += tilebuff.moveChange;
                }

                if(tilebuff.rangeChange != 0)
                { 
                    rangeChange += tilebuff.rangeChange;
                }
                #endregion

                for(int j = 0; j < enemyCharacters[i].GetComponent<Character>().characterStatusEffect.Length; j++)
                { 
                    if(enemyCharacters[i].GetComponent<Character>().characterStatusEffect[j].isOn == true)
                    { 
                        StatusEffect checkEffect = StatusEffectDatabaseManager.instance.GetStatusEffect(j);

                        if(checkEffect.ATKChange > changeCheck)
                        { 
                            ATKChange = AddStatusEffectChange(ATKChange, checkEffect.ATKChange);
                        }

                        if(checkEffect.MAKChange > changeCheck)
                        { 
                            MAKChange = AddStatusEffectChange(MAKChange, checkEffect.MAKChange);
                        }

                        if(checkEffect.DEFChange > changeCheck)
                        { 
                            DEFChange = AddStatusEffectChange(DEFChange, checkEffect.DEFChange);
                        }

                        if(checkEffect.MDFChange > changeCheck)
                        { 
                            MDFChange = AddStatusEffectChange(MDFChange, checkEffect.MDFChange);
                        }

                        if(checkEffect.moveChange != 0)
                        { 
                            moveChange += checkEffect.moveChange;
                        }

                        if(checkEffect.rangeChange != 0)
                        { 
                            rangeChange += checkEffect.rangeChange;
                        }
                    }
                }

                enemyCharacters[i].GetComponent<Character>().status.level = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].level;
                enemyCharacters[i].GetComponent<Character>().status.maxEXP = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].maxEXP;
                enemyCharacters[i].GetComponent<Character>().status.EXP = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].EXP;

                enemyCharacters[i].GetComponent<Character>().status.maxHP = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].maxHP;
                enemyCharacters[i].GetComponent<Character>().status.HP = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].HP;
                enemyCharacters[i].GetComponent<Character>().status.maxMP = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].maxMP;
                enemyCharacters[i].GetComponent<Character>().status.MP = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].MP;

                enemyCharacters[i].GetComponent<Character>().status.maxSympathy = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].maxSympathy;
                enemyCharacters[i].GetComponent<Character>().status.Sympathy = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].Sympathy;

                SympathyType beforeSympathyType = enemyCharacters[i].GetComponent<Character>().sympathyType;
                enemyCharacters[i].GetComponent<Character>().sympathyType = SympathyDatabaseManager.instance.CheckSympathy(enemyCharacters[i].GetComponent<Character>());
                if (beforeSympathyType != enemyCharacters[i].GetComponent<Character>().sympathyType)
                {
                    /* 수정
                    if (i < battleTopEnemyInform.Length)
                    {
                        battleTopEnemyInform[i].GetComponent<BattleTopCharacterInform>().ChangeFaceNum(enemyCharacters[i].GetComponent<Character>());
                    }
                    */
                    SetBattleLog(enemyCharacters[i].GetComponent<Character>(), enemyCharacters[i].GetComponent<Character>().status.inGameName + "의 감정은 " + GameManager.instance.GetSympathyTypeName(enemyCharacters[i].GetComponent<Character>().sympathyType) + "이(가) 되었다.");
                }

                enemyCharacters[i].GetComponent<Character>().status.ATK = (int)(GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].ATK * ATKChange);
                enemyCharacters[i].GetComponent<Character>().status.MAK = (int)(GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].MAK * MAKChange);
                enemyCharacters[i].GetComponent<Character>().status.DEF = (int)(GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].DEF * DEFChange);
                enemyCharacters[i].GetComponent<Character>().status.MDF = (int)(GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].MDF * MDFChange);
                enemyCharacters[i].GetComponent<Character>().status.range = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].range + rangeChange;
                enemyCharacters[i].GetComponent<Character>().status.move = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].move + moveChange;               
            }

            UpdatePlayerTopWindow();
            CheckGameEnd();
        }
        #endregion
    }

    private void CheckStatus()
    { 
        #region
        for(int i = 0; i < playerCharacters.Length; i++)
        {
            if(GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].maxHP < GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].HP)
            { 
                GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].HP = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].maxHP;      
            }
            else if(GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].HP <= 0)
            { 
                if(playerCharacters[i].GetComponent<Character>().characterStatusEffect[StatusEffectDatabaseManager.instance.GetIsDeadEffectNum()].isOn == false)
                { 
                    playerCharacters[i].GetComponent<Character>().SetIsDead(true);
                    GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].HP = 0;
                    SetBattleLog(playerCharacters[i].GetComponent<Character>(), playerCharacters[i].GetComponent<Character>().status.inGameName + "은(는) 아웃 되었다..");
                }
            }

            if(GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].maxMP < GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].MP)
            { 
                GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].MP = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].maxMP;      
            }
            else if(GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].MP < 0)
            { 
                GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].MP = 0;
            }

            if(GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].maxSympathy < GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].Sympathy)
            { 
                GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].Sympathy = GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].maxSympathy;      
            }
            else if(GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].Sympathy < 0)
            { 
                GameManager.instance.PlayerCharacter[playerCharacters[i].GetComponent<Character>().characterDatabaseNum].Sympathy = 0;
            }
        }

        for(int i = 0; i < enemyCharacters.Length; i++)
        {
            if(GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].maxHP < GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].HP)
            { 
                GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].HP = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].maxHP;      
            }
            else if(GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].HP <= 0)
            { 
                if(enemyCharacters[i].GetComponent<Character>().characterStatusEffect[StatusEffectDatabaseManager.instance.GetIsDeadEffectNum()].isOn == false)
                { 
                    enemyCharacters[i].GetComponent<Character>().SetIsDead(true);
                    GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].HP = 0;
                    SetBattleLog(enemyCharacters[i].GetComponent<Character>(), enemyCharacters[i].GetComponent<Character>().status.inGameName + "은(는) 아웃 되었다!");
                }
            }

            if(GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].maxMP < GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].MP)
            { 
                GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].MP = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].maxMP;      
            }
            else if(GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].MP < 0)
            { 
                GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].MP = 0;
            }

            if(GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].maxSympathy < GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].Sympathy)
            { 
                GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].Sympathy = GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].maxSympathy;      
            }
            else if(GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].Sympathy < 0)
            { 
                GameManager.instance.EnemyCharacter[enemyCharacters[i].GetComponent<Character>().characterDatabaseNum].Sympathy = 0;
            }
        }
        #endregion
    }

    private void CheckSympathyEffect()
    { 
        #region
        int angerCount = 0;
        int enjoyCount = 0;

        for(int i = 0; i < playerCharacters.Length; i++)
        {
            if(playerCharacters[i].GetComponent<Character>().GetIsDead() == false)
            { 
                if(playerCharacters[i].GetComponent<Character>().sympathyType == SympathyType.Anger)
                { 
                    angerCount++;
                    playerCharacters[i].GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetPlayerAngerSympathyEffectNum(), true);
                }
                else
                { 
                    playerCharacters[i].GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetPlayerAngerSympathyEffectNum(), false);
                }    

                if(playerCharacters[i].GetComponent<Character>().sympathyType == SympathyType.Enjoy)
                { 
                    enjoyCount++;
                    playerCharacters[i].GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetPlayerEnjoySympathyEffectNum(), true);
                }
                else
                { 
                    playerCharacters[i].GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetPlayerEnjoySympathyEffectNum(), false);
                }
            }
        }

        StatusEffectDatabaseManager.instance.SetAngerSympathyEffect(angerCount, false);
        StatusEffectDatabaseManager.instance.SetEnjoySympathyEffect(enjoyCount, false);        
        angerCount = 0;
        enjoyCount = 0;

        for(int i = 0; i < enemyCharacters.Length; i++)
        {
            if(enemyCharacters[i].GetComponent<Character>().GetIsDead() == false)
            { 
                if(enemyCharacters[i].GetComponent<Character>().sympathyType == SympathyType.Anger)
                { 
                    angerCount++;
                    enemyCharacters[i].GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetEnemyAngerSympathyEffectNum(), true);
                }
                else
                { 
                    enemyCharacters[i].GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetEnemyAngerSympathyEffectNum(), false);
                }    

                if(enemyCharacters[i].GetComponent<Character>().sympathyType == SympathyType.Enjoy)
                { 
                    enjoyCount++;
                    enemyCharacters[i].GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetEnemyEnjoySympathyEffectNum(), true);
                }
                else
                { 
                    enemyCharacters[i].GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetEnemyEnjoySympathyEffectNum(), false);
                }
            }
        }

        StatusEffectDatabaseManager.instance.SetAngerSympathyEffect(angerCount, true);
        StatusEffectDatabaseManager.instance.SetEnjoySympathyEffect(enjoyCount, true);     
        angerCount = 0;
        enjoyCount = 0;
        #endregion
    }

    private void CheckStrategicPointEffect()
    {
        #region
        int playerStrategicPointNum = 0;
        int enemyStrategicPointNum = 0;

        for (int i = 0; i < strategicPointMapBlocks.Count; i++)
        {
            switch (strategicPointMapBlocks[i].tileOwner)
            {
                case TileOwner.Player:
                    playerStrategicPointNum++;
                    break;
                case TileOwner.Enemy:
                    enemyStrategicPointNum++;
                    break;
                default:
                    break;
            }
        }

        StatusEffectDatabaseManager.instance.UpdateStrategicPointEffect(playerStrategicPointNum, enemyStrategicPointNum);
        #endregion
    }

    private float AddStatusEffectChange(float original, float add)
    {
        return (original + add - 1.0f);
    }

    private void CheckGameEnd()
    { 
        //승리조건이 무승부 경우는 없으니까 아군 캐릭터부터 돌리겠음
        // 뭐 구조상 무승부가 나올 수 없긴 하지만 혹시 모르니 적어둠
        if(playerCharacters[GameManager.instance.playerLeaderNum].GetComponent<Character>().GetIsDead() == true)
        { 
            battlePhaseManager.ChangePhase(BattlePhase.GameOver);
        }
        else if(enemyCharacters[GameManager.instance.enemyLeaderNum].GetComponent<Character>().GetIsDead() == true)
        { 
            battlePhaseManager.ChangePhase(BattlePhase.GameClear);
        }
       
    }

    private void UpdatePlayerTopWindow()
    {
        battleTopTurnText.GetComponent<Text>().text = battleTurnCount + "";
        battleTopPlayerInformCharacterWindow.UpdateBattleTopCharacterInformWindow();
        battleTopEnemyInformCharacterWindow.UpdateBattleTopCharacterInformWindow();
    }

    public void CallGetMapBlockByMousePosition()
    {
        battleGetInformationManager.CallShootRay();
    }

    public void AttackButton()
    {
        SetUseSkill(targetCharacter.GetComponent<Character>().status.attackSkillNum);
    }

    public void PassButton()
    {
        SetUseSkill(GameManager.DEFAULTPASSSKILLNUM);
    }

    public void InterruptBallButton()
    {
        SetUseSkill(GameManager.DEFAULTINTERRUPTBALLNUM);   
    }

    public void SkillButton(bool mode)
    { 
        battleCommandManager.SetBattleSkillWindow(mode);
    }
    
    public void StayButton() //여기에 코드 추가하기
    { 
        BattleStay();
    }

    public void BattleStay()
    { 
        //SetBattleLog(targetCharacter.GetComponent<Character>(), targetCharacter.GetComponent<Character>().status.inGameName + "은(는) 대기했다.");
        //행동으로 인한 sp변화(대기)
        //행동 페이즈에서 대기를 사용하였을 때
        ChangeSympathyByNature(targetCharacter, ChangeSympathySituation.Stay);

        if(targetCharacter.GetComponent<Character>().isEnemy == true)
        { 
            aiManager.AITurnEnd();
        }
        else
        { 
           CharacterTurnEnd(); 
        }
    }
    
    private void CharacterTurnEnd()
    { 
        if(targetCharacter == null)
        {
            return;
        }

        targetCharacter.GetComponent<Character>().SetIsTurnEnd(true);
        ClearUseSkill();
        battleGetInformationManager.ClearTargetCharacter();         
        battleGetInformationManager.SetBattleCommandWindow(false); 
        battlePhaseManager.ChangePhase(BattlePhase.MyTurn_Result);
        battleCharacterManager.actionAbleTile.Clear();
    }


    public void SetUseSkill(int num)
    { 
        useSkill = SkillDatabaseManager.instance.GetSkill(num);
        battleCommandManager.SetBattleSkillWindow(false);
        battleCharacterManager.ShowSkillTargetCharacter(useSkill);        
        battlePhaseManager.ChangePhase(BattlePhase.MyTurn_SkillTargeting);    
        battleGetInformationManager.SetBattleCommandWindow(false);        
    }

    public void ClearUseSkill()
    { 
        useSkill = null;
    }

    public void HPDamage(GameObject target ,int damage)
    {
        #region
        if(target.GetComponent<Character>().isEnemy != true)
        { 
            GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].HP -= damage; 

            if(GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].HP < 0)
            {
                GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].HP = 0;
            } 

            if (GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].HP > GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].maxHP)
            {
                GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].HP = GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].maxHP;
            }
        }
        else
        { 
            GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].HP -= damage;

            if(GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].HP < 0)
            {
                GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].HP = 0;
            } 

            if (GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].HP > GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].maxHP)
            {
                GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].HP = GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].maxHP;
            }
        }
        target.GetComponent<Character>().status.HP -= damage;
        UpdateCharacterStatus();
        #endregion
    }

    public void HPDamageByMaxHP(GameObject target ,float percent)
    {
        #region
        if(target.GetComponent<Character>().isEnemy != true)
        { 
            GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].HP -= (int)(GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].maxHP * percent);          
        }
        else
        { 
            GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].HP -= (int)(GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].maxHP * percent);
        }
        UpdateCharacterStatus();
        #endregion
    }

    public void TurnHPDamage(GameObject target ,float percent)
    { 
        DamageEffect((int)(target.GetComponent<Character>().status.maxHP * percent),target);
        HPDamageByMaxHP(target, percent);
        SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 상태이상 혹은 타일로 인해 " + (int)(target.GetComponent<Character>().status.maxHP * percent) + "만큼의 피해를 입었다!");
    }

    public void MPDamage(GameObject target ,int damage)
    {
        #region
        Debug.Log(target + " " + damage);
        if(target.GetComponent<Character>().isEnemy != true)
        { 
            GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].MP -= damage;
            if(GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].MP < 0)
            {
                GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].MP = 0;
            }

            if (GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].MP > GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].maxMP)
            {
                GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].MP = GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].maxMP;
            }
        }
        else
        { 
            GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].MP -= damage;
            if(GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].MP < 0)
            {
                GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].MP = 0;
            }

            if (GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].MP > GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].maxMP)
            {
                GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].MP = GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].maxMP;
            }
        }
        UpdateCharacterStatus();
        #endregion
    }

    public void SympathyDamage(GameObject target ,int damage)
    {
        #region
        if(target.GetComponent<Character>().isEnemy != true)
        { 
            GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].Sympathy -= damage;
            if(GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].Sympathy < 0)
            {
                GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].Sympathy = 0;
            }

            if (GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].Sympathy > GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].maxSympathy)
            {
                GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].Sympathy = GameManager.instance.PlayerCharacter[target.GetComponent<Character>().characterDatabaseNum].maxSympathy;
            }
        }
        else
        { 
            GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].Sympathy -= damage;
            if(GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].Sympathy < 0)
            {
                GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].Sympathy = 0;
            }

            if (GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].Sympathy > GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].maxSympathy)
            {
                GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].Sympathy = GameManager.instance.EnemyCharacter[target.GetComponent<Character>().characterDatabaseNum].maxSympathy;
            }
        }
        UpdateCharacterStatus();
        #endregion
    }

    public void ChangeSympathy(GameObject target ,int damage)
    {
        SympathyDamage(target, -1 * damage);
        UpdateCharacterStatus();
        battleGetInformationManager.CallShootRayByReset();
    }

    public void ChangeSympathyByNature(GameObject target ,ChangeSympathySituation changeSympathySituation)
    {
        #region
        //Debug.LogWarning("행동에 따른 감정 변화 대상은 " + target + " 행동은 " + changeSympathySituation + " 변화치는 " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation));
        ChangeSympathy(target, SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation));

        switch (changeSympathySituation)
        {
            case ChangeSympathySituation.Attack:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 공격하여 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                break;
            case ChangeSympathySituation.Hit:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 공격에 당하여 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                break;
            case ChangeSympathySituation.HitSafe:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 공격을 방어하여 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 공격을 방어했다!");
                }
                break;
            case ChangeSympathySituation.BlockBall:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 공을 가로채는데 성공하여 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 공을 가로챘다!");
                }
                break;
            case ChangeSympathySituation.CatchBoundBall:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 튕겨져 나온 공을 잡아 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 튕겨져 나온 공을 잡았다.");
                }
                break;
            case ChangeSympathySituation.SafeByFriendly:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 아군에게 도움 받아 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 아군에게 도움 받았다!");
                }
                break;
            case ChangeSympathySituation.Pass:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 패스하여 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                break;
            case ChangeSympathySituation.GetPass:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 패스를 받아 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                break;
            case ChangeSympathySituation.FailGetPass:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 패스를 받는데 실패하여 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                break;
            case ChangeSympathySituation.FailUseBallSkill:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 공을 다루는데 실패하여 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                break;
            case ChangeSympathySituation.UseIgnoreIsHaveBallSkill:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 스킬을 사용하여 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                break;
            case ChangeSympathySituation.GetIgnoreIsHaveBallSkillByEnemy:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 상대방 스킬의 대상이 되어 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                break;
            case ChangeSympathySituation.GetIgnoreIsHaveBallSkillByTeam:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                   SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 아군 스킬의 대상이 되어 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                break;
            case ChangeSympathySituation.GetBall:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                   SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 공을 주워 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 공을 주웠다.");
                }
                break;
            case ChangeSympathySituation.Stay:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 대기하여 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 대기했다.");
                }
                break;
            case ChangeSympathySituation.SafeInterruptBall:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 공을 지켜내어 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 공을 지켜냈다!");
                }
                break;
            case ChangeSympathySituation.LoseBallByInterruptBall:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 공을 놓쳐 " + NameDatabaseManager.SympathyName + "이(가) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "만큼 변하였다.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "은(는) 공을 놓쳤다!");
                }
                break;
            default:
                Debug.LogWarning("해당하는 ChangeSympathySituation가 없습니다!");
                break;
        }
        #endregion
    }

    public void SetBattleLog(Character logTargetCharacter, string log)
    {
        battleGetInformationManager.SetBattleLog(logTargetCharacter, log);
    }

    public void SkillCost(GameObject target, Skill skill)
    { 
        HPDamage(target, skill.useHP);     
        MPDamage(target, skill.useMP);     
        SympathyDamage(target, skill.useSympathy);

        for(int i = 0; i < skill.skillUserStatusEffect.Length; i++)
        { 
            int checker = 0;
            checker = Random.Range(1,101);
            Debug.Log("상태이상 확률표는 " + checker);
            
            if(checker <= skill.skillUserStatusEffect[i].statusEffectPercentage)
            { 
                Debug.Log("상태이상 판정 성공");
                target.GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetStatusEffectNum(skill.skillUserStatusEffect[i].statusEffect), true);
            }
            
        }
    }

         
    public int GetDamage(CharacterStatus attackerStatus, CharacterStatus targetStatus, Skill skill)
    { 
        #region
        //스킬 사용 코스트는 따로 분리함
        float attackPower = skill.DPHATK * attackerStatus.ATK + skill.DPHMAK * attackerStatus.MAK + skill.DPHDEF * attackerStatus.DEF + skill.DPHMDF * attackerStatus.MDF + skill.DPHMaxHP * attackerStatus.maxHP + skill.DPHHP * attackerStatus.HP + skill.DPHMaxMP * attackerStatus.maxMP + skill.DPHMP * attackerStatus.MP;
        float resistPower = skill.enemyATK * targetStatus.ATK + skill.enemyMAK * targetStatus.MAK + skill.enemyDEF * targetStatus.DEF + skill.enemyMDF * targetStatus.MDF + skill.enemyMaxHP * targetStatus.maxHP + skill.enemyHP * targetStatus.HP + skill.enemyMaxMP * targetStatus.maxMP + skill.enemyMP * targetStatus.MP;
        float varianceDamage = Random.Range(1.0f - skill.variance, 1.0f + skill.variance);
        float innocentDamage = attackPower - resistPower; 
        
        int damage = (int)(innocentDamage * varianceDamage) + skill.fixedValue;

        if(damage <= skill.fixedValue)
        {
            if(skill.fixedValue > 0)
            { 
                Debug.Log("데미지는 " + skill.fixedValue);
                return skill.fixedValue;                
            }
            else
            { 
                Debug.Log("데미지는 " + defaultDamage);
                return defaultDamage;
            }
        }
        else
        { 
            Debug.Log("데미지는 " + damage);
           return damage; 
        }     
        #endregion
    }

    public int GetMinimumSkillDamage(CharacterStatus attackerStatus, CharacterStatus targetStatus, Skill skill)
    {
        float attackPower = skill.DPHATK * attackerStatus.ATK + skill.DPHMAK * attackerStatus.MAK + skill.DPHDEF * attackerStatus.DEF + skill.DPHMDF * attackerStatus.MDF + skill.DPHMaxHP * attackerStatus.maxHP + skill.DPHHP * attackerStatus.HP + skill.DPHMaxMP * attackerStatus.maxMP + skill.DPHMP * attackerStatus.MP;
        float resistPower = skill.enemyATK * targetStatus.ATK + skill.enemyMAK * targetStatus.MAK + skill.enemyDEF * targetStatus.DEF + skill.enemyMDF * targetStatus.MDF + skill.enemyMaxHP * targetStatus.maxHP + skill.enemyHP * targetStatus.HP + skill.enemyMaxMP * targetStatus.maxMP + skill.enemyMP * targetStatus.MP;
        float varianceDamage = 1.0f - skill.variance;
        float innocentDamage = attackPower - resistPower;

        int damage = (int)(innocentDamage * varianceDamage) + skill.fixedValue;

        return damage;
    }


    public int GetHeal(CharacterStatus attackerStatus, CharacterStatus targetStatus, Skill skill)
    { 
        #region
        //스킬 사용 코스트는 따로 분리함
        float attackPower = skill.DPHATK * attackerStatus.ATK + skill.DPHMAK * attackerStatus.MAK + skill.DPHDEF * attackerStatus.DEF + skill.DPHMDF * attackerStatus.MDF + skill.DPHMaxHP * attackerStatus.maxHP + skill.DPHHP * attackerStatus.HP + skill.DPHMaxMP * attackerStatus.maxMP + skill.DPHMP * attackerStatus.MP;
        float resistPower = skill.enemyATK * targetStatus.ATK + skill.enemyMAK * targetStatus.MAK + skill.enemyDEF * targetStatus.DEF + skill.enemyMDF * targetStatus.MDF + skill.enemyMaxHP * targetStatus.maxHP + skill.enemyHP * targetStatus.HP + skill.enemyMaxMP * targetStatus.maxMP + skill.enemyMP * targetStatus.MP;
        float varianceDamage = Random.Range(1.0f - skill.variance, 1.0f + skill.variance);
        float innocentDamage = attackPower + resistPower; 
        
        int heal = (int)(innocentDamage * varianceDamage) + skill.fixedValue;

        heal = heal * -1;

        if(heal >= defaultHeal)
        { 
            Debug.Log("힐량 " + defaultHeal);
            return defaultHeal;
        }
        else
        { 
            Debug.Log("힐량 " + heal);
           return heal; 
        }     
        #endregion
    }

    public void UseSkill(GameObject attacker, GameObject target, Skill skill)
    {
        skillUser = attacker;
        skillTarget = target;      
        useSkill = skill;
        SetIsUseBallSkillSuccess(true);
        Debug.Log("스킬 사용자는 " + attacker + " 사용 스킬은 " + useSkill);
        SetBattleLog(skillUser.GetComponent<Character>(), skillUser.GetComponent<Character>().status.inGameName + "이(가) " + useSkill.ingameSkillName + "을(를) 사용! 스킬 대상은 " + target.GetComponent<Character>().status.inGameName);

        battleGetInformationManager.CheckBallRoute(target);//볼루트 검사 추가 ai는 사기치고있었음

        if((attacker.transform.position.x - target.transform.position.x > 0) && (attacker.GetComponent<Character>() == true))
        { 
            attacker.GetComponent<Character>().ChangeCharacterFlipX(true); 
        }
        else if((attacker.transform.position.x - target.transform.position.x < 0) && (attacker.GetComponent<Character>() == true))
        { 
            attacker.GetComponent<Character>().ChangeCharacterFlipX(false);
        }

        battlePhaseManager.ChangePhase(BattlePhase.IsSkill);
        skillUser.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(skill.skillUserEffect)); //스킬 사용 이펙트, 맞은 대상 이펙트는 볼컨트롤러 쪽에 있음 원래 스킬 컨트롤러에 있던코드

        if((skillUser.GetComponent<Character>().CheckCharacterAnimationController() == true) && (useSkill.ignoreIsHaveBall == false) && (useSkill.skillType != SkillType.InterruptBall))
        { 
            skillUser.GetComponent<Character>().SetCharacterThrowAnimation(true);
        }
        else
        { 
            skillUseAnimationEnd();
        }
        
    }

    public void skillUseAnimationEnd()
    { 
        //행동으로 인한 sp변화(스킬)
        if (useSkill.ignoreIsHaveBall == false)
        {
            if (useSkill.skillType == SkillType.Attack)
            {
                //공격을 할 때 
                ChangeSympathyByNature(skillUser, ChangeSympathySituation.Attack);
            }
            else if(useSkill.skillType == SkillType.Pass)
            {
                //패스를 할 때 
                ChangeSympathyByNature(skillUser, ChangeSympathySituation.Pass);
            }
            else
            {
                //특수기인데 일단 임시처리로 공이 필요 없는 스킬을 사용할 때하고 같은 걸로 처리하겠음
                ChangeSympathyByNature(skillUser, ChangeSympathySituation.UseIgnoreIsHaveBallSkill);
            }
        }
        else
        {
            //공이 필요 없는 스킬을 사용할 때 
            ChangeSympathyByNature(skillUser, ChangeSympathySituation.UseIgnoreIsHaveBallSkill);
        }

        SkillController.UseSkill(this, skillUser, skillTarget, useSkill);
    }

    public void SetIgnoreIsHaveBallSkillEffect()
    { 
        StartCoroutine(IgnoreIsHaveBallSkillEffect());
    }

    private IEnumerator IgnoreIsHaveBallSkillEffect()
    { 
        if(skillUser.GetComponent<Character>().GetIsAnimation() == true)
        { 
            yield return null;
            StartCoroutine(IgnoreIsHaveBallSkillEffect());
        }
        else
        { 
            skillTarget.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(useSkill.skillTargetEffect));
            BallMoveEnd();
        }
    }

    public void BallMoveEnd() 
    {  
        //행동으로 인한 sp변화(스킬)
        if (useSkill.skillType != SkillType.InterruptBall) //공뺏기 계열은 스킬 컨트롤러에 움직일 예정(사유 로그 찍히는 순서라든가..)
        {
            if (useSkill.ignoreIsHaveBall == false)
            {
                if (isUseBallSkillSuccess == false)
                {
                    //공을 사용한 상호작용을 실패 했을 때 
                    ChangeSympathyByNature(skillUser, ChangeSympathySituation.FailUseBallSkill);
                }

                if ((useSkill.skillType == SkillType.Attack) && (isUseBallSkillSuccess == true))
                {
                    //공격에 피해를 입었을 때 
                    ChangeSympathyByNature(skillTarget, ChangeSympathySituation.Hit);
                }
                else if(useSkill.skillType == SkillType.Pass)
                {
                    if (isUseBallSkillSuccess == true)
                    {
                        //패스를 받았을 때 
                        ChangeSympathyByNature(skillTarget, ChangeSympathySituation.GetPass);
                    }
                    else
                    {
                        //패스를 받지 못하였을 때 
                        ChangeSympathyByNature(skillTarget, ChangeSympathySituation.FailGetPass);
                    }
                }
            }
            else
            {
                if ((skillUser != skillTarget) && (skillUser.GetComponent<Character>().isEnemy != skillTarget.GetComponent<Character>().isEnemy))
                {
                    //공이 필요 없는 스킬의 대상이 되었을 때(스킬사용자가 자신이 아닌 적)
                    ChangeSympathyByNature(skillTarget, ChangeSympathySituation.GetIgnoreIsHaveBallSkillByEnemy);
                }
                else if((skillUser != skillTarget) && (skillUser.GetComponent<Character>().isEnemy == skillTarget.GetComponent<Character>().isEnemy))
                {
                    //공이 필요 없는 스킬의 대상이 되었을 때(스킬사용자가 자신이 아닌 아군)
                    ChangeSympathyByNature(skillTarget, ChangeSympathySituation.GetIgnoreIsHaveBallSkillByTeam);
                }
            }
        }

        //Debug.LogWarning("BallMoveEnd, targetCharacter = " + targetCharacter);
        bool isFriendly = false;
        if(targetCharacter.GetComponent<Character>().isEnemy == skillTarget.GetComponent<Character>().isEnemy)
        { 
            isFriendly = true;
        }

        CheckSkillDamage();        
        if(targetCharacter.GetComponent<Character>().isEnemy == true)
        {
            aiManager.AITurnEnd();
        }
        else
        {
            CharacterTurnEnd();                
        }
        
        CheckSkillStatusEffect(isFriendly);
        boundAblePosition.Clear();
        interruptObject = null;
        skillTarget = null;
        skillUser = null;
        battleGetInformationManager.ResetSomeThingOnBallRoute();

        battleCharacterManager.AttackableTarget.Clear();
        battleCharacterManager.InteractableTarget.Clear();

        UpdateCharacterStatus();
        battleGetInformationManager.CallShootRayByReset();
    }

    private void CheckSkillDamage()
    { 
        HPDamage(skillTarget, skillHPDamage);
        MPDamage(skillTarget, skillMPDamage);
        SympathyDamage(skillTarget, skillSympathyDamage);
        
        if(skillHPDamage != 0)
        { 

            DamageEffect(skillHPDamage, skillTarget, DamageType.HP);
        }
        else if(skillMPDamage != 0)
        { 
            DamageEffect(skillMPDamage, skillTarget, DamageType.MP);
        }

        #region
        if (skillHPDamage != 0)
        {
            if (skillHPDamage < 0)
            {
                if(skillTarget.GetComponent<Character>().isEnemy != true)
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "스킬의 영향으로 " + skillTarget.GetComponent<Character>().status.inGameName + "의 "+ NameDatabaseManager.HPName + "가 " + skillHPDamage * -1 + "만큼 회복하여 남은 " +  NameDatabaseManager.HPName + "가 " + GameManager.instance.PlayerCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].HP + "이(가) 되었다!");          
                }
                else
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "스킬의 영향으로 " + skillTarget.GetComponent<Character>().status.inGameName + "의 "+ NameDatabaseManager.HPName + "가 " + skillHPDamage * -1 + "만큼 회복하여 남은 " +  NameDatabaseManager.HPName + "가 " + GameManager.instance.EnemyCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].HP  + "이(가) 되었다!");
                }
            }
            else
            {
                if(skillTarget.GetComponent<Character>().isEnemy != true)
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "스킬의 영향으로 " + skillTarget.GetComponent<Character>().status.inGameName + "의 "+ NameDatabaseManager.HPName + "가 " + skillHPDamage + "만큼 피해를 입어 남은 " +  NameDatabaseManager.HPName + "가 " + GameManager.instance.PlayerCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].HP + "이(가) 되었다!");          
                }
                else
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "스킬의 영향으로 " + skillTarget.GetComponent<Character>().status.inGameName + "의 "+ NameDatabaseManager.HPName + "가 " + skillHPDamage + "만큼 피해를 입어 남은 " +  NameDatabaseManager.HPName + "가 " + GameManager.instance.EnemyCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].HP  + "이(가) 되었다!");
                }
            }
        }


        if (skillMPDamage != 0)
        {
            if (skillMPDamage < 0)
            {
                if(skillTarget.GetComponent<Character>().isEnemy != true)
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "스킬의 영향으로 " + skillTarget.GetComponent<Character>().status.inGameName + "의 "+ NameDatabaseManager.MPName + "가 " + skillMPDamage * -1 + "만큼 회복하여 남은 " +  NameDatabaseManager.MPName + "가 " + GameManager.instance.PlayerCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].MP + "이(가) 되었다!");          
                }
                else
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "스킬의 영향으로 " + skillTarget.GetComponent<Character>().status.inGameName + "의 "+ NameDatabaseManager.MPName + "가 " + skillMPDamage * -1 + "만큼 회복하여 남은 " +  NameDatabaseManager.MPName + "가 " + GameManager.instance.EnemyCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].MP  + "이(가) 되었다!");
                }
            }
            else
            {
                if(skillTarget.GetComponent<Character>().isEnemy != true)
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "스킬의 영향으로 " + skillTarget.GetComponent<Character>().status.inGameName + "의 "+ NameDatabaseManager.MPName + "가 " + skillMPDamage + "만큼 피해를 입어 남은 " +  NameDatabaseManager.MPName + "가 " + GameManager.instance.PlayerCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].MP + "이(가) 되었다!");          
                }
                else
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "스킬의 영향으로 " + skillTarget.GetComponent<Character>().status.inGameName + "의 "+ NameDatabaseManager.MPName + "가 " + skillMPDamage + "만큼 피해를 입어 남은 " +  NameDatabaseManager.MPName + "가 " + GameManager.instance.EnemyCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].MP  + "이(가) 되었다!");
                }
            }
        }

        if (skillSympathyDamage != 0)
        {
            if (skillSympathyDamage < 0)
            {
                if(skillTarget.GetComponent<Character>().isEnemy != true)
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "스킬의 영향으로 " + skillTarget.GetComponent<Character>().status.inGameName + "의 "+ NameDatabaseManager.SympathyName + "가 " + skillSympathyDamage * -1 + "만큼 증가하여 현재 " +  NameDatabaseManager.SympathyName + "가 " + GameManager.instance.PlayerCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].Sympathy + "이(가) 되었다!");          
                }
                else
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "스킬의 영향으로 " + skillTarget.GetComponent<Character>().status.inGameName + "의 "+ NameDatabaseManager.SympathyName + "가 " + skillSympathyDamage * -1 + "만큼 증가하여 현재 " +  NameDatabaseManager.SympathyName + "가 " + GameManager.instance.EnemyCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].Sympathy  + "이(가) 되었다!");
                }
            }
            else
            {
                if(skillTarget.GetComponent<Character>().isEnemy != true)
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "스킬의 영향으로 " + skillTarget.GetComponent<Character>().status.inGameName + "의 "+ NameDatabaseManager.SympathyName + "가 " + skillSympathyDamage + "만큼 감소하여 현재 " +  NameDatabaseManager.SympathyName + "가 " + GameManager.instance.PlayerCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].Sympathy + "이(가) 되었다!");          
                }
                else
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "스킬의 영향으로 " + skillTarget.GetComponent<Character>().status.inGameName + "의 "+ NameDatabaseManager.SympathyName + "가 " + skillSympathyDamage + "만큼 감소하여 현재 " +  NameDatabaseManager.SympathyName + "가 " + GameManager.instance.EnemyCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].Sympathy  + "이(가) 되었다!");
                }
            }
        }
        #endregion

        skillHPDamage = 0;
        skillMPDamage = 0;
        skillSympathyDamage = 0;
    }

    private void DamageEffect(int damage, GameObject target, DamageType damageType = DamageType.HP)
    { 
        target.GetComponent<Character>().DamageEffect(damage, target, damageType);
    }

    private void CheckSkillStatusEffect(bool isFriendly)
    { 
        for(int i = 0; i < skillStatusEffectNum.Length; i++)
        { 
            if(skillStatusEffectNum[i] != ignoreSkillStatusEffectNum)
            { 
                SetBattleLog(skillTarget.GetComponent<Character>(), skillTarget.GetComponent<Character>().status.inGameName + "은(는) " + StatusEffectDatabaseManager.instance.GetStatusEffect(skillStatusEffectNum[i]).inGameName + "상태가 되었다!");
                skillTarget.GetComponent<Character>().SetStatusEffect(skillStatusEffectNum[i], true);
            }
        }

        for(int i = 0; i < skillDispelStatusEffectNum.Length; i++)
        { 
            if(skillDispelStatusEffectNum[i] != ignoreSkillStatusEffectNum)
            { 
                if (skillTarget.GetComponent<Character>().characterStatusEffect[skillDispelStatusEffectNum[i]].isOn == true)
                {
                    SetBattleLog(skillTarget.GetComponent<Character>(), skillTarget.GetComponent<Character>().status.inGameName + "의 " + StatusEffectDatabaseManager.instance.GetStatusEffect(skillDispelStatusEffectNum[i]).inGameName + "상태가 해제되었다!");
                }
                skillTarget.GetComponent<Character>().SetStatusEffect(skillDispelStatusEffectNum[i], false);
            }
        }

        if((isFriendly == true) && (skillTarget.GetComponent<Character>().GetIsTurnEnd() == false)) // 아군한테 적용시 아군 턴이 끝나지 않았다면 상태이상 적용
        { 
            Debug.Log("스킬 사용자와 스킬 대상이 같은 편 임으로 스킬 대상의 상태이상을 검사합니다");
            battlePhaseManager.CheckTurnEndStatusEffect(skillTarget);
        }

        skillStatusEffectNum = new int[1] {ignoreSkillStatusEffectNum };
        skillDispelStatusEffectNum = new int[1] {ignoreSkillStatusEffectNum };        
    }

    public void SetIsUseBallSkillSuccess(bool mode)
    {
        isUseBallSkillSuccess = mode;
    }

    public void SkipPlayerTurn()
    { 
        if(battlePhaseManager.GetIsEnemyTurn() == false)
        { 
            for(int i = 0; i < playerCharacters.Length; i++)
            { 
                if(playerCharacters[i].GetComponent<Character>().GetIsTurnEnd() == false)
                { 
                    //SetBattleLog(playerCharacters[i].GetComponent<Character>(), playerCharacters[i].GetComponent<Character>().status.inGameName + "은(는) 대기했다.");
                    //행동으로 인한 sp변화(대기)
                    //행동 페이즈에서 대기를 사용하였을 때
                    ChangeSympathyByNature(playerCharacters[i], ChangeSympathySituation.Stay);
                    playerCharacters[i].GetComponent<Character>().SetIsTurnEnd(true);
                }
            }

            ClearUseSkill();
            battleGetInformationManager.ClearTargetCharacter();         
            battleGetInformationManager.SetBattleCommandWindow(false); 
            battlePhaseManager.ChangePhase(BattlePhase.MyTurn_Result);
        } 
        else
        { 
            Debug.LogWarning("상대 턴 입니다!");    
        }
    }

    //--------------------------------------------------------------------------------------
    public void GetBallBoundPosition(GameObject targetCharacterObject, int boundRange, bool isIgnoreCharacterOrBall = false)
    {
        #region
        boundAblePosition.Clear();
        SideBallBoundPosition(boundRange, (int)targetCharacterObject.transform.position.x, (int)targetCharacterObject.transform.position.y, GameManager.RESETINDEX, isIgnoreCharacterOrBall);
        
        for(int i = 0; i < 2; i++)
        { 
            int checkRange = boundRange;
            int targetXPosition = (int)targetCharacterObject.transform.position.x;
            int targetYPosition = (int)targetCharacterObject.transform.position.y;

            while(checkRange > 0)
            { 
                switch(i)
                { 
                    case 0:
                        targetYPosition += GameManager.TILESIZE;
                        break;
                    case 1:
                        targetYPosition -= GameManager.TILESIZE;
                        break;
                }

                GetBoundAblePosition(new Vector2(targetXPosition, targetYPosition), isIgnoreCharacterOrBall);
                checkRange--;
                SideBallBoundPosition(checkRange,targetXPosition, targetYPosition, GameManager.RESETINDEX, isIgnoreCharacterOrBall);
            }           
        }      
        #endregion
    }

    private void SideBallBoundPosition(int checkRange, int targetXPosition, int targetYPosition, int overlapNum, bool isIgnoreCharacterOrBall)
    { 
        #region
        for(int i = 0; i < 2; i++)
        { 
            int sideCheckRange = checkRange;
            int sideTargetXPosition = targetXPosition;
            int sideTargetYPosition = targetYPosition;
            int sideOverlapNum = overlapNum;

            if(sideOverlapNum != i)
            { 
                while(sideCheckRange > 0)
                { 
                    switch(i)
                    { 
                        case 0:
                            sideTargetXPosition -= GameManager.TILESIZE;
                            sideOverlapNum = 1;
                            break;
                        case 1:
                            sideTargetXPosition += GameManager.TILESIZE;
                            sideOverlapNum = 0;
                            break;
                    }

                    GetBoundAblePosition(new Vector2(sideTargetXPosition, sideTargetYPosition), isIgnoreCharacterOrBall);
                    sideCheckRange--;
                    SideBallBoundPosition(sideCheckRange,sideTargetXPosition, sideTargetYPosition, sideOverlapNum, isIgnoreCharacterOrBall);
                    break;
                }   
            }
        }
        #endregion
    }

    private void GetBoundAblePosition(Vector2 ray, bool isIgnoreCharacterOrBall)
    { 
        #region
        int tileCheckCount = 0;
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(ray, Vector2.zero);


        for(int i = 0; i< hits.Length; i++)
        { 
            if(GameManager.instance.CantMoveTagCheck(hits[i].transform.tag) == true) //구조가 캐릭터 먼저 검사하고(CantMoveTagCheck여기에 캐릭터가 있기때문) 이후에 타일검사하는 형식 
            {
                if((hits[i].transform.tag == "Character") && (isIgnoreCharacterOrBall == false))//이동 불가능 한 곳에 캐릭터가 있는거 자체가 버그라 겹치는거 고려 안하겠음
                {
                    int overlapCount = 0;
                    for(int j = 0; j < boundAblePosition.Count; j++)
                    { 
                        if(boundAblePosition[j] == hits[i].transform.gameObject)
                        { 
                            overlapCount++;
                        }
                    }
                    
                    if(overlapCount == 0)
                    { 
                        boundAblePosition.Add(hits[i].transform.gameObject);
                    }
                }
                tileCheckCount ++;
                break;
            }   
            else if ((hits[i].transform.tag == "Ball") && (hits[i].transform.gameObject.GetComponent<BallController>() == true) && (isIgnoreCharacterOrBall == true))
            {
                tileCheckCount ++;
                break;
            }
        }  

        if(tileCheckCount < 1)
        { 
            for(int i = 0; i< hits.Length; i++)
            { 
                if(hits[i].transform.tag == "Tile")
                { 
                    if (hits[i].transform.GetComponent<MapBlock>().tileType == TileType.UnableMoveTile) //이동불가능한 타일인지 검사
                    {
                        return;
                    }

                    int overlapCount = 0;
                    for(int j = 0; j < boundAblePosition.Count; j++)
                    { 
                        if(boundAblePosition[j] == hits[i].transform.gameObject)
                        { 
                            overlapCount++;
                        }
                    }
                    
                    if(overlapCount == 0)
                    { 
                        boundAblePosition.Add(hits[i].transform.gameObject);
                    }
                }    
            } 
        }     
        #endregion
    }

    //이름으로 캐릭터 찾기
    public void GetBallBoundPositionWithCheckCharacterName(GameObject targetCharacterObject, int boundRange, string findCharacterName)
    { 
        #region
        boundAblePosition.Clear();
        findBoundAbleCharacter = false;  

        SideBallBoundPositionWithCheckCharacterName(boundRange, (int)targetCharacterObject.transform.position.x, (int)targetCharacterObject.transform.position.y, GameManager.RESETINDEX, findCharacterName);
        
        for(int i = 0; i < 2; i++)
        { 
            int checkRange = boundRange;
            int targetXPosition = (int)targetCharacterObject.transform.position.x;
            int targetYPosition = (int)targetCharacterObject.transform.position.y;

            while(checkRange > 0)
            { 
                switch(i)
                { 
                    case 0:
                        targetYPosition += GameManager.TILESIZE;
                        break;
                    case 1:
                        targetYPosition -= GameManager.TILESIZE;
                        break;
                }

                GetBoundAblePositionWithCheckCharacterName(new Vector2(targetXPosition, targetYPosition), findCharacterName);
                checkRange--;
                SideBallBoundPositionWithCheckCharacterName(checkRange,targetXPosition, targetYPosition, GameManager.RESETINDEX, findCharacterName);
            }           
        }      
        #endregion        
    }

    private void SideBallBoundPositionWithCheckCharacterName(int checkRange, int targetXPosition, int targetYPosition, int overlapNum, string findCharacterName)
    { 
        #region
        for(int i = 0; i < 2; i++)
        { 
            int sideCheckRange = checkRange;
            int sideTargetXPosition = targetXPosition;
            int sideTargetYPosition = targetYPosition;
            int sideOverlapNum = overlapNum;

            if(sideOverlapNum != i)
            { 
                while(sideCheckRange > 0)
                { 
                    switch(i)
                    { 
                        case 0:
                            sideTargetXPosition -= GameManager.TILESIZE;
                            sideOverlapNum = 1;
                            break;
                        case 1:
                            sideTargetXPosition += GameManager.TILESIZE;
                            sideOverlapNum = 0;
                            break;
                    }

                    GetBoundAblePositionWithCheckCharacterName(new Vector2(sideTargetXPosition, sideTargetYPosition), findCharacterName);
                    sideCheckRange--;
                    SideBallBoundPositionWithCheckCharacterName(sideCheckRange,sideTargetXPosition, sideTargetYPosition, sideOverlapNum, findCharacterName);
                    break;
                }   
            }
        }
        #endregion
    }

    private void GetBoundAblePositionWithCheckCharacterName(Vector2 ray, string findCharacterName)
    { 
        #region
        if(findBoundAbleCharacter == false)
        { 
            int tileCheckCount = 0;
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(ray, Vector2.zero);

            for(int i = 0; i< hits.Length; i++)
            { 
                if(GameManager.instance.CantMoveTagCheck(hits[i].transform.tag) == true)
                {
                    if(hits[i].transform.tag == "Character")
                    {
                        if(hits[i].transform.gameObject.GetComponent<Character>() == true)
                        { 
                            if(hits[i].transform.gameObject.GetComponent<Character>().status.name == findCharacterName)
                            { 
                                boundAblePosition.Clear();
                                findBoundAbleCharacter = true;   
                                boundAblePosition.Add(hits[i].transform.gameObject);
                                Debug.Log("바운드 가능한 타겟 발견");
                                return;
                            }
                        }

                        int overlapCount = 0;
                        for(int j = 0; j < boundAblePosition.Count; j++)
                        {
                            if(boundAblePosition[j] == hits[i].transform.gameObject)
                            { 
                                overlapCount++;
                            }
                        }
                    
                        if(overlapCount == 0)
                        { 
                            boundAblePosition.Add(hits[i].transform.gameObject);
                        }
                    }
                    tileCheckCount ++;
                }    
            }  

            if(tileCheckCount < 1)
            { 
                for(int i = 0; i< hits.Length; i++)
                { 
                    if(hits[i].transform.tag == "Tile")
                    { 
                        if (hits[i].transform.GetComponent<MapBlock>().tileType == TileType.UnableMoveTile) //이동불가능한 타일인지 검사
                        {
                            return;
                        }

                        int overlapCount = 0;
                        for(int j = 0; j < boundAblePosition.Count; j++)
                        { 
                            if(boundAblePosition[j] == hits[i].transform.gameObject)
                            { 
                                overlapCount++;
                            }
                        }
                    
                        if(overlapCount == 0)
                        { 
                            boundAblePosition.Add(hits[i].transform.gameObject);
                        }
                    }    
                } 
            }  
        }
        #endregion
    }
    //--------------------------------------------------------------------------------------
    // 게임오브젝트가 해당 위치 주변에 있는지 확인하기
    public bool CheckGameObjectByPositionAndName(Vector2 targetPosition, string findObject, int range)
    { 
        #region
        if(SideCheckGameObjectByPositionAndName((int)targetPosition.x, (int)targetPosition.y, findObject, range, GameManager.RESETINDEX) == true)
        { 
            return true;  
        }

        for(int i = 0; i < 2; i++)
        { 
            int checkRange = range;
            int targetXPosition = (int)targetPosition.x;
            int targetYPosition = (int)targetPosition.y;

            while(checkRange > 0)
            { 
                switch(i)
                { 
                    case 0:
                        targetYPosition += GameManager.TILESIZE;
                        break;
                    case 1:
                        targetYPosition -= GameManager.TILESIZE;
                        break;
                }

                if(CheckGameObjectByName(new Vector2(targetXPosition, targetYPosition), findObject) == true)
                { 
                    return true;
                }

                checkRange--;
                if(SideCheckGameObjectByPositionAndName(targetXPosition, targetYPosition,findObject, checkRange, GameManager.RESETINDEX) == true)
                { 
                    return true;    
                }
            } 
        }

        return false;
        #endregion
    }

    private bool SideCheckGameObjectByPositionAndName(int targetXPosition, int targetYPosition, string findObject, int range, int overlapNum)
    { 
        #region
        for(int i = 0; i < 2; i++)
        { 
            int sideCheckRange = range;
            int sideTargetXPosition = targetXPosition;
            int sideTargetYPosition = targetYPosition;
            int sideOverlapNum = overlapNum;

            if(sideOverlapNum != i)
            { 
                while(sideCheckRange > 0)
                { 
                    switch(i)
                    { 
                        case 0:
                            sideTargetXPosition -= GameManager.TILESIZE;
                            sideOverlapNum = 1;
                            break;
                        case 1:
                            sideTargetXPosition += GameManager.TILESIZE;
                            sideOverlapNum = 0;
                            break;
                    }

                    if(CheckGameObjectByName(new Vector2(sideTargetXPosition, sideTargetYPosition), findObject) == true)
                    { 
                        return true;
                    }
                    sideCheckRange--;

                    if(SideCheckGameObjectByPositionAndName(sideTargetXPosition, sideTargetYPosition, findObject, sideCheckRange, sideOverlapNum) == true)
                    { 
                        return true;    
                    }
                }   
            }
        } 
        return false;
        #endregion
    }

    private bool CheckGameObjectByName(Vector2 ray, string findObject)
    { 
        #region
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(ray, Vector2.zero);

        for(int i = 0; i< hits.Length; i++)
        { 
            if((hits[i].transform.tag == "Character") && (hits[i].transform.GetComponent<Character>() == true))
            { 
                if(hits[i].transform.GetComponent<Character>().status.name == findObject)
                { 
                    return true;    
                }
            }
        } 
        return false;
        #endregion
    }
    //--------------------------------------------------------------------------------------
    // 배틀맵 상단 UI용
    #region
    public void SetBattleSceneHinderUI(bool mode)
    {
        battleTopWindow.SetActive(mode);
    }
    #endregion
    //------

    public bool IsBattleCharacterCanMove()
    {
        if ((IsPlayerSelectToCharacterMove() == true) && (GameManager.instance.KeyCheckAccept() == true)) //&& (cantMoveOnThisTile == false)) 이거 뺀이유 원래위치로 이동 만드려고
            return true;
        else
            return false;
    }

    
    // 이거 바로 위에 말고 쓰는 곳 없음
    public bool IsCanSelectCharacter()
    {
        if ((IsPlayerSelectToCharacterMove() == false) && (GameManager.instance.KeyCheckAccept() == true))
            return true;
        else
            return false;
    }
    
    public bool IsPlayerSelectToCharacterMove()
    { 
        if(isCharacterSettingComplete == false || targetCharacter == null)
        { 
            return false;
        }    
        else
        { 
            return true;   
        }
    }  


    public bool IsAnimation() //이게 켜지면 페이즈가 애니메이션으로 넘어감
    { 
        if(isAnimation == true)
        { 
            return true;    
        }
        else
        {
            return false;
        }

    }

    public bool IsMenu()
    { 
        if(isMenu == true)
        { 
            return true;    
        }
        else
        {
            return false;
        }        
    }

    public bool IsCanCameraMoveCheck()
    { 
        if((isMenu != true) && (isBattleSkillWindow != true) && (nowPhase != BattlePhase.GameResult) && (nowPhase != BattlePhase.IsEvent))
        { 
            return true;    
        }
        else
        {
            return false;
        }           
    }
   
    public bool IsCanCheckInformationCheck()
    { 
        if((isMenu != true) && (isBattleSkillWindow != true) && (nowPhase != BattlePhase.IsEvent) && (nowPhase != BattlePhase.GameResult))
        { 
            return true;    
        }
        else
        {
            return false;
        }           
    }
}
