using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum BattlePhase
{
    MyTurn_ReadyToStart, // ��Ʋ �غ��
    MyTurn_Start, // ��Ʋ ���۽�, ���� �� ����
    MyTurn_CharcterSelect, // ĳ���� ����
    MyTurn_Moving, // �̵� ��� ����,ĳ���� ���� �� �̵��� �����ϰ� �ִ� ���� �̵��� �����ϰ� �ִ� ����
    MyTurn_Command, // �̵� �� ��� ����, ��� �ϸ� ���� �� ���۽� ���� ������.
    MyTurn_SkillTargeting, // ���� ��ų�� ������ ����, ���� ����� �����ϸ� ������ ������.
    MyTurn_ItemTargeting, // ����� �������� ������ ����, ������ ��� ����� �����ϸ� �������� ���ȴ�.
    MyTurn_Result, // �ൿ ��� ǥ��
    EnemyTurn_ReadyToStart, // ���� ��Ʋ �غ�
    EnemyTurn_Start, // ���� ��Ʋ ����
    EnemyTurn_Moving, // ���� �̵�
    EnemyTurn_Command, // ���� ����, ��ų ���
    EnemyTurn_Result, // ���� �ൿ ��� ǥ��
    IsAnimation,
    IsSkill,
    IsTurnEffect,
    IsEvent,
    GameClear,
    GameOver,
    GameResult,
    Stay //�ǹ̾��� ������ ��ٸ��� ������ �ٲ�� ��� �̰ɷ� ������ ������
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
    public GameObject targetCharacter; //���� �ڽ� ���� ĳ����
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

    //�޴���
    public GameObject battleMenuWindow;
    //--

    [HideInInspector]
    public int selectTileXpos;
    [HideInInspector]
    public int selectTileYpos;   
    [HideInInspector]
    public bool cantMoveOnThisTile; //�Ⱦ�
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
    public int battleTurnCount = 0; //���� �� ��

    [HideInInspector]
    public bool isMoveReturn = false; //�̵� �� ����ġ�� �ǵ��� ���°� ����

    [HideInInspector]
    public GameObject skillUser; //��ų�����
    [HideInInspector]
    public GameObject skillTarget; //��ų ���
    [HideInInspector]
    public GameObject interruptObject; // ���� ������Ʈ
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

    //������ ��� UI�� 
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
    private void Test() 0,0 ���� �����
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
        if(CheckNumber(tileWidth) == true)//�ݶ��̴��� Size���� ¦���� �ݶ��̴��ϰ� Ÿ���ϰ� ���� �ʱ⿡ ���� ���ִ� �ڵ�
        { 
            this.gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f * GameManager.TILESIZE, this.gameObject.GetComponent<BoxCollider2D>().offset.y);
        }
        tileHeight = (int)this.gameObject.GetComponent<BoxCollider2D>().size.y / GameManager.TILESIZE;
        if(CheckNumber(tileHeight) == true)
        { 
            this.gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(this.gameObject.GetComponent<BoxCollider2D>().offset.x, -0.5f * GameManager.TILESIZE);
        }

        Debug.Log("Ÿ���� x���̴� : " + tileWidth);
        Debug.Log("Ÿ���� y���̴� : " + tileHeight); 
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
                    // ������ ��Ƶ� Ÿ���� �ֳ� üũ                    
                    RaycastHit2D[] hits;
                    hits = Physics2D.RaycastAll(new Vector2 (CreateTileXpos, CreateTileYpos), Vector2.zero);
                    bool isTileOn = false; //���� ��ġ�� Ÿ���� ������ �ִ��� �˻��
                    for(int r = 0; r < hits.Length; r++)
                    { 
                        if((hits[r].transform.gameObject.GetComponent<MapBlock>() == true) && (hits[r].transform.gameObject.CompareTag("Tile"))) //��ǥ�� �̵��ϴµ� �Լ� ��� ������ ������ 0,0 ���� �ɸ��� �� ��..
                        { 
                            if (isTileOn == false)
                            {
                                newTile = hits[r].transform.gameObject;
                                isTileOn = true;
                            }
                            else
                            {
                                Debug.LogWarning(CreateTileXpos + "," + CreateTileYpos + " ��ǥ�� �ߺ��� Ÿ�����ֽ��ϴ�!");
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

                    newTile.GetComponent<BoxCollider2D>().enabled = false; //Ÿ�� �ݶ��̴� ���� (0,0 ���� ȸ�ǿ�)

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

                mapBlock.gameObject.GetComponent<BoxCollider2D>().enabled = true; //Ÿ�� �ݶ��̴� Ű�� (0,0 ���� ȸ�ǿ�)

                index++;
                CreateTileYpos += GameManager.TILESIZE;
            }
            CreateTileYpos = startCreateTileYpos;
            CreateTileXpos += GameManager.TILESIZE;
        }
        #endregion
    }

    private void CheckStrategicPointMapBlock() // InitializeMapBlock���� ������ ȣ�� �Ǿ����
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

        Debug.Log("�˻�� ������ ���� " + strategicPointMapBlocks.Count);
        #endregion
    }

    private void SetStartCreateTilePos() // ������ GameManager.TILESIZE <- �̰� �ٲ𶧸��� �Լ� �ȿ� ���� ���������
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


    public void SetIsAnimation(bool mode) //�̰� �ǹ� ���µ�
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

                    //------------�������ͽ� �����ϱ�
                    //playerCharacters[i].GetComponent<Character>().status = GameManager.instance.PlayerCharacter[GameManager.instance.selectPlayerCharacterNum[i]]; ������ �����ص� ��
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
                    Debug.LogWarning("GameManager.instance.selectPlayerCharacterNum[" + i + "]�� ���� GameManager.instance.PlayerCharacter[]���� ũ�ų� 0���� �۽��ϴ�");    
                    Debug.LogWarning("��� �� �� = " + GameManager.instance.selectPlayerCharacterNum[i]);    

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

                    // ��ų �ֱ�
                    enemyCharacters[i].GetComponent<Character>().status.characterSkill = new bool[SkillDatabaseManager.instance.SkillLength()]; 
                    for(int j = 0; j < enemyCharacters[i].GetComponent<Character>().status.firstCharacterSkillNum.Length; j++)
                    { 
                        if(enemyCharacters[i].GetComponent<Character>().status.firstCharacterSkillNum[j] < SkillDatabaseManager.instance.SkillLength() == true)
                        { 
                            enemyCharacters[i].GetComponent<Character>().status.characterSkill[enemyCharacters[i].GetComponent<Character>().status.firstCharacterSkillNum[j]] = true;  
                        }
                        else
                        { 
                            Debug.LogWarning(enemyCharacters[i].GetComponent<Character>().status.name + "�� characterSkill ���� " + j +"�� �迭�� �߸��� ���� ���Խ��ϴ�! �߸� ���� �� " + enemyCharacters[i].GetComponent<Character>().status.firstCharacterSkillNum[j]);
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
                    Debug.LogWarning("��Ʋ�ʿ� �����ϴ� ���� ���� ���������� ������ ���� �� ���� �����Ƿ� �ı��մϴ�!");
                    Destroy(this.gameObject.transform.GetChild(2).GetChild(i).gameObject);
                }
                
            }
        }

        //Ȥ�� ���� �����ڵ� ���
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
                        Debug.LogWarning(enemyCharacters[i].GetComponent<Character>().status.name + "�� characterSkill ���� " + j +"�� �迭�� �߸��� ���� ���Խ��ϴ�! �߸� ���� �� " + enemyCharacters[i].GetComponent<Character>().status.firstCharacterSkillNum[j]);
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
                Debug.LogWarning(characterAnimation + "�� CharacterAnimationControllert ��ũ��Ʈ�� ��� ���� �ʽ��ϴ�!");
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
        //�鿹���� ���
        //GameManager.instance.EnemyCharacter[i] = CharacterDatabaseManager.instance.DeepCopyCharacterStatus(enemyCharacters[i].GetComponent<Character>().status);
        #endregion
    }

    public void UpdateCharacterStatus() //���߿� �����̻��̶�簡 ������簡 ������ �����ؾ���(��Ȯ�� �ڿ� + �־����) ���� HP MP�� �ٽ� �����غ�����
    {
        #region
        if(isCharacterSettingComplete == true)
        {
            CheckStatus();
            CheckSympathyEffect(); // ���� �̰� ���� CheckStatus���⼭ �ƿ����� �ƴ��� �˻���
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

                //Ÿ�Ϲ��� �˻�
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
                playerCharacters[i].GetComponent<Character>().sympathyType = SympathyDatabaseManager.instance.CheckSympathy(playerCharacters[i].GetComponent<Character>()); //���� ���°� ��������
                if ((beforeSympathyType != playerCharacters[i].GetComponent<Character>().sympathyType) && (isMoveReturn != true))
                {
                    /* ����
                    if (i < battleTopPlayerInform.Length)
                    {
                        battleTopPlayerInform[i].GetComponent<BattleTopCharacterInform>().ChangeFaceNum(playerCharacters[i].GetComponent<Character>());
                    }
                    */
                    SetBattleLog(playerCharacters[i].GetComponent<Character>(), playerCharacters[i].GetComponent<Character>().status.inGameName + "�� ������ " + GameManager.instance.GetSympathyTypeName(playerCharacters[i].GetComponent<Character>().sympathyType) + "��(��) �Ǿ���.");
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

                //Ÿ�Ϲ��� �˻�
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
                    /* ����
                    if (i < battleTopEnemyInform.Length)
                    {
                        battleTopEnemyInform[i].GetComponent<BattleTopCharacterInform>().ChangeFaceNum(enemyCharacters[i].GetComponent<Character>());
                    }
                    */
                    SetBattleLog(enemyCharacters[i].GetComponent<Character>(), enemyCharacters[i].GetComponent<Character>().status.inGameName + "�� ������ " + GameManager.instance.GetSympathyTypeName(enemyCharacters[i].GetComponent<Character>().sympathyType) + "��(��) �Ǿ���.");
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
                    SetBattleLog(playerCharacters[i].GetComponent<Character>(), playerCharacters[i].GetComponent<Character>().status.inGameName + "��(��) �ƿ� �Ǿ���..");
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
                    SetBattleLog(enemyCharacters[i].GetComponent<Character>(), enemyCharacters[i].GetComponent<Character>().status.inGameName + "��(��) �ƿ� �Ǿ���!");
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
        //�¸������� ���º� ���� �����ϱ� �Ʊ� ĳ���ͺ��� ��������
        // �� ������ ���ºΰ� ���� �� ���� ������ Ȥ�� �𸣴� �����
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
    
    public void StayButton() //���⿡ �ڵ� �߰��ϱ�
    { 
        BattleStay();
    }

    public void BattleStay()
    { 
        //SetBattleLog(targetCharacter.GetComponent<Character>(), targetCharacter.GetComponent<Character>().status.inGameName + "��(��) ����ߴ�.");
        //�ൿ���� ���� sp��ȭ(���)
        //�ൿ ������� ��⸦ ����Ͽ��� ��
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
        SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) �����̻� Ȥ�� Ÿ�Ϸ� ���� " + (int)(target.GetComponent<Character>().status.maxHP * percent) + "��ŭ�� ���ظ� �Ծ���!");
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
        //Debug.LogWarning("�ൿ�� ���� ���� ��ȭ ����� " + target + " �ൿ�� " + changeSympathySituation + " ��ȭġ�� " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation));
        ChangeSympathy(target, SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation));

        switch (changeSympathySituation)
        {
            case ChangeSympathySituation.Attack:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) �����Ͽ� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                break;
            case ChangeSympathySituation.Hit:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ���ݿ� ���Ͽ� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                break;
            case ChangeSympathySituation.HitSafe:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ������ ����Ͽ� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ������ ����ߴ�!");
                }
                break;
            case ChangeSympathySituation.BlockBall:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ���� ����ä�µ� �����Ͽ� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ���� ����ë��!");
                }
                break;
            case ChangeSympathySituation.CatchBoundBall:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ƨ���� ���� ���� ��� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ƨ���� ���� ���� ��Ҵ�.");
                }
                break;
            case ChangeSympathySituation.SafeByFriendly:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) �Ʊ����� ���� �޾� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) �Ʊ����� ���� �޾Ҵ�!");
                }
                break;
            case ChangeSympathySituation.Pass:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) �н��Ͽ� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                break;
            case ChangeSympathySituation.GetPass:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) �н��� �޾� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                break;
            case ChangeSympathySituation.FailGetPass:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) �н��� �޴µ� �����Ͽ� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                break;
            case ChangeSympathySituation.FailUseBallSkill:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ���� �ٷ�µ� �����Ͽ� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                break;
            case ChangeSympathySituation.UseIgnoreIsHaveBallSkill:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ��ų�� ����Ͽ� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                break;
            case ChangeSympathySituation.GetIgnoreIsHaveBallSkillByEnemy:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ���� ��ų�� ����� �Ǿ� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                break;
            case ChangeSympathySituation.GetIgnoreIsHaveBallSkillByTeam:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                   SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) �Ʊ� ��ų�� ����� �Ǿ� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                break;
            case ChangeSympathySituation.GetBall:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                   SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ���� �ֿ� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ���� �ֿ���.");
                }
                break;
            case ChangeSympathySituation.Stay:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ����Ͽ� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ����ߴ�.");
                }
                break;
            case ChangeSympathySituation.SafeInterruptBall:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ���� ���ѳ��� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ���� ���ѳ´�!");
                }
                break;
            case ChangeSympathySituation.LoseBallByInterruptBall:
                if (SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) != 0)
                {
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ���� ���� " + NameDatabaseManager.SympathyName + "��(��) " + SympathyDatabaseManager.instance.GetSympathyChangeCount(target.GetComponent<Character>().status.characterNatureType, changeSympathySituation) + "��ŭ ���Ͽ���.");
                }
                else
                { 
                    SetBattleLog(target.GetComponent<Character>(), target.GetComponent<Character>().status.inGameName + "��(��) ���� ���ƴ�!");
                }
                break;
            default:
                Debug.LogWarning("�ش��ϴ� ChangeSympathySituation�� �����ϴ�!");
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
            Debug.Log("�����̻� Ȯ��ǥ�� " + checker);
            
            if(checker <= skill.skillUserStatusEffect[i].statusEffectPercentage)
            { 
                Debug.Log("�����̻� ���� ����");
                target.GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetStatusEffectNum(skill.skillUserStatusEffect[i].statusEffect), true);
            }
            
        }
    }

         
    public int GetDamage(CharacterStatus attackerStatus, CharacterStatus targetStatus, Skill skill)
    { 
        #region
        //��ų ��� �ڽ�Ʈ�� ���� �и���
        float attackPower = skill.DPHATK * attackerStatus.ATK + skill.DPHMAK * attackerStatus.MAK + skill.DPHDEF * attackerStatus.DEF + skill.DPHMDF * attackerStatus.MDF + skill.DPHMaxHP * attackerStatus.maxHP + skill.DPHHP * attackerStatus.HP + skill.DPHMaxMP * attackerStatus.maxMP + skill.DPHMP * attackerStatus.MP;
        float resistPower = skill.enemyATK * targetStatus.ATK + skill.enemyMAK * targetStatus.MAK + skill.enemyDEF * targetStatus.DEF + skill.enemyMDF * targetStatus.MDF + skill.enemyMaxHP * targetStatus.maxHP + skill.enemyHP * targetStatus.HP + skill.enemyMaxMP * targetStatus.maxMP + skill.enemyMP * targetStatus.MP;
        float varianceDamage = Random.Range(1.0f - skill.variance, 1.0f + skill.variance);
        float innocentDamage = attackPower - resistPower; 
        
        int damage = (int)(innocentDamage * varianceDamage) + skill.fixedValue;

        if(damage <= skill.fixedValue)
        {
            if(skill.fixedValue > 0)
            { 
                Debug.Log("�������� " + skill.fixedValue);
                return skill.fixedValue;                
            }
            else
            { 
                Debug.Log("�������� " + defaultDamage);
                return defaultDamage;
            }
        }
        else
        { 
            Debug.Log("�������� " + damage);
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
        //��ų ��� �ڽ�Ʈ�� ���� �и���
        float attackPower = skill.DPHATK * attackerStatus.ATK + skill.DPHMAK * attackerStatus.MAK + skill.DPHDEF * attackerStatus.DEF + skill.DPHMDF * attackerStatus.MDF + skill.DPHMaxHP * attackerStatus.maxHP + skill.DPHHP * attackerStatus.HP + skill.DPHMaxMP * attackerStatus.maxMP + skill.DPHMP * attackerStatus.MP;
        float resistPower = skill.enemyATK * targetStatus.ATK + skill.enemyMAK * targetStatus.MAK + skill.enemyDEF * targetStatus.DEF + skill.enemyMDF * targetStatus.MDF + skill.enemyMaxHP * targetStatus.maxHP + skill.enemyHP * targetStatus.HP + skill.enemyMaxMP * targetStatus.maxMP + skill.enemyMP * targetStatus.MP;
        float varianceDamage = Random.Range(1.0f - skill.variance, 1.0f + skill.variance);
        float innocentDamage = attackPower + resistPower; 
        
        int heal = (int)(innocentDamage * varianceDamage) + skill.fixedValue;

        heal = heal * -1;

        if(heal >= defaultHeal)
        { 
            Debug.Log("���� " + defaultHeal);
            return defaultHeal;
        }
        else
        { 
            Debug.Log("���� " + heal);
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
        Debug.Log("��ų ����ڴ� " + attacker + " ��� ��ų�� " + useSkill);
        SetBattleLog(skillUser.GetComponent<Character>(), skillUser.GetComponent<Character>().status.inGameName + "��(��) " + useSkill.ingameSkillName + "��(��) ���! ��ų ����� " + target.GetComponent<Character>().status.inGameName);

        battleGetInformationManager.CheckBallRoute(target);//����Ʈ �˻� �߰� ai�� ���ġ���־���

        if((attacker.transform.position.x - target.transform.position.x > 0) && (attacker.GetComponent<Character>() == true))
        { 
            attacker.GetComponent<Character>().ChangeCharacterFlipX(true); 
        }
        else if((attacker.transform.position.x - target.transform.position.x < 0) && (attacker.GetComponent<Character>() == true))
        { 
            attacker.GetComponent<Character>().ChangeCharacterFlipX(false);
        }

        battlePhaseManager.ChangePhase(BattlePhase.IsSkill);
        skillUser.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(skill.skillUserEffect)); //��ų ��� ����Ʈ, ���� ��� ����Ʈ�� ����Ʈ�ѷ� �ʿ� ���� ���� ��ų ��Ʈ�ѷ��� �ִ��ڵ�

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
        //�ൿ���� ���� sp��ȭ(��ų)
        if (useSkill.ignoreIsHaveBall == false)
        {
            if (useSkill.skillType == SkillType.Attack)
            {
                //������ �� �� 
                ChangeSympathyByNature(skillUser, ChangeSympathySituation.Attack);
            }
            else if(useSkill.skillType == SkillType.Pass)
            {
                //�н��� �� �� 
                ChangeSympathyByNature(skillUser, ChangeSympathySituation.Pass);
            }
            else
            {
                //Ư�����ε� �ϴ� �ӽ�ó���� ���� �ʿ� ���� ��ų�� ����� ���ϰ� ���� �ɷ� ó���ϰ���
                ChangeSympathyByNature(skillUser, ChangeSympathySituation.UseIgnoreIsHaveBallSkill);
            }
        }
        else
        {
            //���� �ʿ� ���� ��ų�� ����� �� 
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
        //�ൿ���� ���� sp��ȭ(��ų)
        if (useSkill.skillType != SkillType.InterruptBall) //������ �迭�� ��ų ��Ʈ�ѷ��� ������ ����(���� �α� ������ ������簡..)
        {
            if (useSkill.ignoreIsHaveBall == false)
            {
                if (isUseBallSkillSuccess == false)
                {
                    //���� ����� ��ȣ�ۿ��� ���� ���� �� 
                    ChangeSympathyByNature(skillUser, ChangeSympathySituation.FailUseBallSkill);
                }

                if ((useSkill.skillType == SkillType.Attack) && (isUseBallSkillSuccess == true))
                {
                    //���ݿ� ���ظ� �Ծ��� �� 
                    ChangeSympathyByNature(skillTarget, ChangeSympathySituation.Hit);
                }
                else if(useSkill.skillType == SkillType.Pass)
                {
                    if (isUseBallSkillSuccess == true)
                    {
                        //�н��� �޾��� �� 
                        ChangeSympathyByNature(skillTarget, ChangeSympathySituation.GetPass);
                    }
                    else
                    {
                        //�н��� ���� ���Ͽ��� �� 
                        ChangeSympathyByNature(skillTarget, ChangeSympathySituation.FailGetPass);
                    }
                }
            }
            else
            {
                if ((skillUser != skillTarget) && (skillUser.GetComponent<Character>().isEnemy != skillTarget.GetComponent<Character>().isEnemy))
                {
                    //���� �ʿ� ���� ��ų�� ����� �Ǿ��� ��(��ų����ڰ� �ڽ��� �ƴ� ��)
                    ChangeSympathyByNature(skillTarget, ChangeSympathySituation.GetIgnoreIsHaveBallSkillByEnemy);
                }
                else if((skillUser != skillTarget) && (skillUser.GetComponent<Character>().isEnemy == skillTarget.GetComponent<Character>().isEnemy))
                {
                    //���� �ʿ� ���� ��ų�� ����� �Ǿ��� ��(��ų����ڰ� �ڽ��� �ƴ� �Ʊ�)
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
                    SetBattleLog(skillTarget.GetComponent<Character>(), "��ų�� �������� " + skillTarget.GetComponent<Character>().status.inGameName + "�� "+ NameDatabaseManager.HPName + "�� " + skillHPDamage * -1 + "��ŭ ȸ���Ͽ� ���� " +  NameDatabaseManager.HPName + "�� " + GameManager.instance.PlayerCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].HP + "��(��) �Ǿ���!");          
                }
                else
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "��ų�� �������� " + skillTarget.GetComponent<Character>().status.inGameName + "�� "+ NameDatabaseManager.HPName + "�� " + skillHPDamage * -1 + "��ŭ ȸ���Ͽ� ���� " +  NameDatabaseManager.HPName + "�� " + GameManager.instance.EnemyCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].HP  + "��(��) �Ǿ���!");
                }
            }
            else
            {
                if(skillTarget.GetComponent<Character>().isEnemy != true)
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "��ų�� �������� " + skillTarget.GetComponent<Character>().status.inGameName + "�� "+ NameDatabaseManager.HPName + "�� " + skillHPDamage + "��ŭ ���ظ� �Ծ� ���� " +  NameDatabaseManager.HPName + "�� " + GameManager.instance.PlayerCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].HP + "��(��) �Ǿ���!");          
                }
                else
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "��ų�� �������� " + skillTarget.GetComponent<Character>().status.inGameName + "�� "+ NameDatabaseManager.HPName + "�� " + skillHPDamage + "��ŭ ���ظ� �Ծ� ���� " +  NameDatabaseManager.HPName + "�� " + GameManager.instance.EnemyCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].HP  + "��(��) �Ǿ���!");
                }
            }
        }


        if (skillMPDamage != 0)
        {
            if (skillMPDamage < 0)
            {
                if(skillTarget.GetComponent<Character>().isEnemy != true)
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "��ų�� �������� " + skillTarget.GetComponent<Character>().status.inGameName + "�� "+ NameDatabaseManager.MPName + "�� " + skillMPDamage * -1 + "��ŭ ȸ���Ͽ� ���� " +  NameDatabaseManager.MPName + "�� " + GameManager.instance.PlayerCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].MP + "��(��) �Ǿ���!");          
                }
                else
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "��ų�� �������� " + skillTarget.GetComponent<Character>().status.inGameName + "�� "+ NameDatabaseManager.MPName + "�� " + skillMPDamage * -1 + "��ŭ ȸ���Ͽ� ���� " +  NameDatabaseManager.MPName + "�� " + GameManager.instance.EnemyCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].MP  + "��(��) �Ǿ���!");
                }
            }
            else
            {
                if(skillTarget.GetComponent<Character>().isEnemy != true)
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "��ų�� �������� " + skillTarget.GetComponent<Character>().status.inGameName + "�� "+ NameDatabaseManager.MPName + "�� " + skillMPDamage + "��ŭ ���ظ� �Ծ� ���� " +  NameDatabaseManager.MPName + "�� " + GameManager.instance.PlayerCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].MP + "��(��) �Ǿ���!");          
                }
                else
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "��ų�� �������� " + skillTarget.GetComponent<Character>().status.inGameName + "�� "+ NameDatabaseManager.MPName + "�� " + skillMPDamage + "��ŭ ���ظ� �Ծ� ���� " +  NameDatabaseManager.MPName + "�� " + GameManager.instance.EnemyCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].MP  + "��(��) �Ǿ���!");
                }
            }
        }

        if (skillSympathyDamage != 0)
        {
            if (skillSympathyDamage < 0)
            {
                if(skillTarget.GetComponent<Character>().isEnemy != true)
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "��ų�� �������� " + skillTarget.GetComponent<Character>().status.inGameName + "�� "+ NameDatabaseManager.SympathyName + "�� " + skillSympathyDamage * -1 + "��ŭ �����Ͽ� ���� " +  NameDatabaseManager.SympathyName + "�� " + GameManager.instance.PlayerCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].Sympathy + "��(��) �Ǿ���!");          
                }
                else
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "��ų�� �������� " + skillTarget.GetComponent<Character>().status.inGameName + "�� "+ NameDatabaseManager.SympathyName + "�� " + skillSympathyDamage * -1 + "��ŭ �����Ͽ� ���� " +  NameDatabaseManager.SympathyName + "�� " + GameManager.instance.EnemyCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].Sympathy  + "��(��) �Ǿ���!");
                }
            }
            else
            {
                if(skillTarget.GetComponent<Character>().isEnemy != true)
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "��ų�� �������� " + skillTarget.GetComponent<Character>().status.inGameName + "�� "+ NameDatabaseManager.SympathyName + "�� " + skillSympathyDamage + "��ŭ �����Ͽ� ���� " +  NameDatabaseManager.SympathyName + "�� " + GameManager.instance.PlayerCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].Sympathy + "��(��) �Ǿ���!");          
                }
                else
                { 
                    SetBattleLog(skillTarget.GetComponent<Character>(), "��ų�� �������� " + skillTarget.GetComponent<Character>().status.inGameName + "�� "+ NameDatabaseManager.SympathyName + "�� " + skillSympathyDamage + "��ŭ �����Ͽ� ���� " +  NameDatabaseManager.SympathyName + "�� " + GameManager.instance.EnemyCharacter[skillTarget.GetComponent<Character>().characterDatabaseNum].Sympathy  + "��(��) �Ǿ���!");
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
                SetBattleLog(skillTarget.GetComponent<Character>(), skillTarget.GetComponent<Character>().status.inGameName + "��(��) " + StatusEffectDatabaseManager.instance.GetStatusEffect(skillStatusEffectNum[i]).inGameName + "���°� �Ǿ���!");
                skillTarget.GetComponent<Character>().SetStatusEffect(skillStatusEffectNum[i], true);
            }
        }

        for(int i = 0; i < skillDispelStatusEffectNum.Length; i++)
        { 
            if(skillDispelStatusEffectNum[i] != ignoreSkillStatusEffectNum)
            { 
                if (skillTarget.GetComponent<Character>().characterStatusEffect[skillDispelStatusEffectNum[i]].isOn == true)
                {
                    SetBattleLog(skillTarget.GetComponent<Character>(), skillTarget.GetComponent<Character>().status.inGameName + "�� " + StatusEffectDatabaseManager.instance.GetStatusEffect(skillDispelStatusEffectNum[i]).inGameName + "���°� �����Ǿ���!");
                }
                skillTarget.GetComponent<Character>().SetStatusEffect(skillDispelStatusEffectNum[i], false);
            }
        }

        if((isFriendly == true) && (skillTarget.GetComponent<Character>().GetIsTurnEnd() == false)) // �Ʊ����� ����� �Ʊ� ���� ������ �ʾҴٸ� �����̻� ����
        { 
            Debug.Log("��ų ����ڿ� ��ų ����� ���� �� ������ ��ų ����� �����̻��� �˻��մϴ�");
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
                    //SetBattleLog(playerCharacters[i].GetComponent<Character>(), playerCharacters[i].GetComponent<Character>().status.inGameName + "��(��) ����ߴ�.");
                    //�ൿ���� ���� sp��ȭ(���)
                    //�ൿ ������� ��⸦ ����Ͽ��� ��
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
            Debug.LogWarning("��� �� �Դϴ�!");    
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
            if(GameManager.instance.CantMoveTagCheck(hits[i].transform.tag) == true) //������ ĳ���� ���� �˻��ϰ�(CantMoveTagCheck���⿡ ĳ���Ͱ� �ֱ⶧��) ���Ŀ� Ÿ�ϰ˻��ϴ� ���� 
            {
                if((hits[i].transform.tag == "Character") && (isIgnoreCharacterOrBall == false))//�̵� �Ұ��� �� ���� ĳ���Ͱ� �ִ°� ��ü�� ���׶� ��ġ�°� ��� ���ϰ���
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
                    if (hits[i].transform.GetComponent<MapBlock>().tileType == TileType.UnableMoveTile) //�̵��Ұ����� Ÿ������ �˻�
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

    //�̸����� ĳ���� ã��
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
                                Debug.Log("�ٿ�� ������ Ÿ�� �߰�");
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
                        if (hits[i].transform.GetComponent<MapBlock>().tileType == TileType.UnableMoveTile) //�̵��Ұ����� Ÿ������ �˻�
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
    // ���ӿ�����Ʈ�� �ش� ��ġ �ֺ��� �ִ��� Ȯ���ϱ�
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
    // ��Ʋ�� ��� UI��
    #region
    public void SetBattleSceneHinderUI(bool mode)
    {
        battleTopWindow.SetActive(mode);
    }
    #endregion
    //------

    public bool IsBattleCharacterCanMove()
    {
        if ((IsPlayerSelectToCharacterMove() == true) && (GameManager.instance.KeyCheckAccept() == true)) //&& (cantMoveOnThisTile == false)) �̰� ������ ������ġ�� �̵� �������
            return true;
        else
            return false;
    }

    
    // �̰� �ٷ� ���� ���� ���� �� ����
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


    public bool IsAnimation() //�̰� ������ ����� �ִϸ��̼����� �Ѿ
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
