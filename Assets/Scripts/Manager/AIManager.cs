using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum AIType
{
    Default
}

public class AIManager : MonoBehaviour
{
    [HideInInspector]
    public BattleManager battleManager;
    private AIType aiType;
    private AILogic aiLogic;
    [HideInInspector]
    public List<GameObject> ActionAbleEnemyCharacter = new List<GameObject>(); //�ൿ ������ AIĳ����
    [HideInInspector]
    public List<GameObject> AttackableTarget = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> InteractableTarget = new List<GameObject>();
    [HideInInspector]
    public List<MapBlock> BallOnField = new List<MapBlock>();
    [HideInInspector]
    public List<AstarNode> TileRoute = new List<AstarNode>();
    [HideInInspector]
    public List<Skill> aiCharacterSkill = new List<Skill>();
    [HideInInspector]
    public bool isStrategicPointOnMap = false; // �ʿ� ������ �ִ��� ������ üũ
    [HideInInspector]
    public bool isEnemyStrategicPointCountEnough = false; // ������ ����� �� üũ
    
    void Start()
    {
        InitAIManager();
    }

    public void InitAIManager()
    {
        SetAILogic(); //AI�γ��� �����ϴ� �ڵ�
        battleManager = this.gameObject.GetComponent<BattleManager>();
    }

    private void SetAILogic()
    {
        #region
        switch (aiType)
        {
            default:
            aiLogic = new DefaultAILogic();
            break;
        }

        aiLogic.SetAIManager(this);
        #endregion
    }

    public void ReadyForAITurnStart()
    {
        aiLogic.ReadyForAITurnStart();
    }

    public void AITurnStart() // ĳ���� ���ýø��� �߻� 
    {
        #region
        SetActionAbleEnemyCharacter(); //�ൿ ������ ĳ���Ͱ� �ִ��� �˻�
        CheckAITargetMapBlockOnSomeOne(); // ĳ���� ��ǥ Ÿ�� ���� ������ �ֳ� �˻�
        CheckBallOnField(); //�ʵ����� ���� �ֳ� �˻�
        if (SelectAICharacter() == false) //ĳ���� ����
        {
            return;
        }
        battleManager.battlePhaseManager.ChangePhase(BattlePhase.Stay);
        Invoke("AICharacterSelectEnd", 0.5f);       
        #endregion
    }

    private void AICharacterSelectEnd()
    {
        battleManager.battlePhaseManager.ChangePhase(BattlePhase.EnemyTurn_Moving);
    }

    private void SetActionAbleEnemyCharacter()
    {
        #region
        ActionAbleEnemyCharacter = new List<GameObject>();
        ActionAbleEnemyCharacter.Clear();
        for (int i = 0; i < battleManager.enemyCharacters.Length; i++)
        {
            if (battleManager.enemyCharacters[i].GetComponent<Character>().GetIsTurnEnd() == false)
            {
                ActionAbleEnemyCharacter.Add(battleManager.enemyCharacters[i]);
            }
        }
        #endregion
    }

    private void CheckBallOnField()
    {
        #region
        BallOnField = new List<MapBlock>();
        BallOnField.Clear();

        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        for (int i = 0; i < balls.Length; i++)
        {
            if ((balls[i].activeSelf == true) && (balls[i].GetComponent<BallController>() == true))
            {
                RaycastHit2D[] hits;
                hits = Physics2D.RaycastAll(new Vector2 (balls[i].transform.position.x, balls[i].transform.position.y), Vector2.zero);
                for (int j = 0; j < hits.Length; j++)
                {
                    if (hits[j].transform.GetComponent<MapBlock>() == true)
                    {
                        BallOnField.Add(hits[j].transform.GetComponent<MapBlock>());
                        break;
                    }
                }
            }
        }
        #endregion
    }

    private void CheckAITargetMapBlockOnSomeOne() //��ǥ ���� ������ �ִ��� �˻�, ��ǥ�� �� ���� ��ǥ �ʱ�ȭ�ϱ� ����
    {
        #region
        for (int j = 0; j < battleManager.enemyCharacters.Length; j++)
        {
            Character checkCharacter = battleManager.enemyCharacters[j].GetComponent<Character>();
            if (checkCharacter.aiTargetMapBlock != null)
            {
                RaycastHit2D[] hits;
                hits = Physics2D.RaycastAll(new Vector2(checkCharacter.aiTargetMapBlock.gameObject.transform.position.x, checkCharacter.aiTargetMapBlock.gameObject.transform.position.y), Vector2.zero);

                for (int i = 0; i < hits.Length; i++)
                {
                    if ((hits[i].transform.GetComponent<Character>() == true) && (hits[i].transform.tag == "Character"))
                    {
                        checkCharacter.aiTargetMapBlock = null;
                        Debug.Log("��ǥ �ʱ�ȭ!"); 
                        break;
                    }
                }
            }  
        }
        #endregion
    }

    private bool SelectAICharacter()
    {
        #region
        if (ActionAbleEnemyCharacter.Count <= 0)
        {
            Debug.LogError("�ൿ������ AIĳ���Ͱ� �����ϴ�!");
            battleManager.battlePhaseManager.ChangePhase(BattlePhase.EnemyTurn_Result);
            return false;
        }

        aiLogic.SetAICharacter();
        return true;
        #endregion
    }

    public void AIMove()
    {
        battleManager.battlePhaseManager.ChangePhase(BattlePhase.Stay);
        aiLogic.AICharacterMove();
    }

    public void AICommand()
    {
        battleManager.battlePhaseManager.ChangePhase(BattlePhase.Stay);
        aiLogic.AICharacterCommand();
    }

    public void AITurnEnd()
    { 
        #region
        ActionAbleEnemyCharacter.Remove(battleManager.targetCharacter);
        battleManager.targetCharacter.GetComponent<Character>().SetIsTurnEnd(true);
        battleManager.ClearUseSkill();
        battleManager.targetCharacter = null;
        battleManager.battlePhaseManager.CheckAITurn();

        battleManager.battleCharacterManager.actionAbleTile.Clear();
        aiCharacterSkill.Clear();
        #endregion
    }

    public void AllAITurnEnd() //��� ĳ������ ���� �������
    {
        ClearList();
        battleManager.battlePhaseManager.ChangePhase(BattlePhase.MyTurn_Start);
    }

    private void ClearList()
    {
        aiCharacterSkill.Clear();
        AttackableTarget.Clear();
        InteractableTarget.Clear();
        ActionAbleEnemyCharacter.Clear();
        BallOnField.Clear();
        TileRoute.Clear();
        battleManager.battleCharacterManager.actionAbleTile.Clear();
    }

    public void ClearCharacterInteractList()
    {
        AttackableTarget.Clear();
        InteractableTarget.Clear();
    }

    public void SetAICharacterSkill() //aiCharacterSkill ����Ʈ�� �����ϴ� �ڵ�(��밡���� ��ų�� �־��)
    {
        aiCharacterSkill.Clear();
        if (battleManager.targetCharacter.GetComponent<Character>().isEnemy == false)
        {
            Debug.LogError("Ÿ������ ������ ĳ���Ͱ� AIĳ���Ͱ� �ƴմϴ�!");
            return;
        }

        List<Skill> aiSkill = new List<Skill>();
        aiSkill.Add(SkillDatabaseManager.instance.GetSkill(battleManager.targetCharacter.GetComponent<Character>().status.attackSkillNum));
        aiSkill.Add(SkillDatabaseManager.instance.GetSkill(GameManager.DEFAULTPASSSKILLNUM));
        aiSkill.Add(SkillDatabaseManager.instance.GetSkill(GameManager.DEFAULTINTERRUPTBALLNUM));
        for (int i = 0; i < battleManager.targetCharacter.GetComponent<Character>().status.characterSkill.Length; i++)
        {
            if (battleManager.targetCharacter.GetComponent<Character>().status.characterSkill[i] == true)
            {
                Skill checkSkill = SkillDatabaseManager.instance.GetSkill(i);
                if (aiSkill.Contains(checkSkill) == false)
                {
                    aiSkill.Add(checkSkill);
                }               
            }          
        }

        for (int i = 0; i < aiSkill.Count; i++)
        {
            if (battleManager.battleCommandManager.CheckCanUseSkill(aiSkill[i]) == true)
            {
                aiCharacterSkill.Add(aiSkill[i]);
            }
        }
    }

    public List<MapBlock> CheckCharacterCanInteractableMapBlock(Character moveCharacter, Character targetCharacter) //1���̳��� 2ĳ���Ͱ� ��ȣ �ۿ��� ������ Ÿ�� ���� return����
    {
        #region
        GameObject moveObject = moveCharacter.gameObject;
        battleManager.battleCharacterManager.GetAICharacterMoveTile(ref moveObject);
        List<MapBlock> moveAbleTile = new List<MapBlock>();
        List<MapBlock> interactAbleTile = new List<MapBlock>();
        for (int i = 0; i < battleManager.battleCharacterManager.actionAbleTile.Count; i++)
        {
            moveAbleTile.Add(battleManager.battleCharacterManager.actionAbleTile[i].GetComponent<MapBlock>());
        }
        interactAbleTile = CharacterInteractRange(moveCharacter.status.range, targetCharacter.gameObject);

        return CheckCompareMapTile(moveAbleTile, interactAbleTile);
        #endregion
    }

    public List<MapBlock> CheckTargetNearTile(int checkRange, GameObject target) //�ش� Ÿ���� checkRange������ŭ(�̵������� ��Ʈ�θ�) ������ �ִ� Ÿ�ϵ��� ����Ʈ�� return��
    {
        return GetTargetNearTile(checkRange, target);
    }

    public List<MapBlock> CheckCompareMapTile(List<MapBlock> checkList0, List<MapBlock> checkList1)
    {
        var compare = (checkList0.Intersect(checkList1)).ToList();
        return compare;
    }

    public MapBlock FindNearTile(GameObject moveObject, GameObject targetObject, int findRange) //targetObject�κ��� moveObject�� ���� ����� Ÿ���� ã�� �ڵ�
    {
        #region
        List<MapBlock> checkMapBlock = CheckTargetNearTile(findRange, targetObject);

        MapBlock nearMapBlock = null;
        int length = 9999;    

        for (int i = 0; i < checkMapBlock.Count; i++)
        {
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(new Vector2 (checkMapBlock[i].gameObject.transform.position.x, checkMapBlock[i].gameObject.transform.position.y), Vector2.zero);
            bool characterOnTileCheck = false;
            for (int j = 0; j < hits.Length; j++)
            {                
                if (hits[j].transform.GetComponent<Character>() == true)
                {
                    characterOnTileCheck = true;
                    break;
                }
            }

            if (characterOnTileCheck == false)
            {
                int checker = GetRouteLength(moveObject, checkMapBlock[i].gameObject);
                if ((checker != -1) && (checker < length))
                {
                    nearMapBlock = checkMapBlock[i];
                    length = checker;
                }
            }
            
        }

        if (length != 9999)
        {
            return nearMapBlock;
        }
        else
        {
            return null;
        }
        #endregion
    }

    public GameObject GetNearObject(List<GameObject> checkList, GameObject targetObject, bool isIgnoreCharacter) //��ǥ������Ʈ�ϰ� ���� ������ �ִ� ������Ʈ�� ��ȯ�޴� �ڵ�
    {
        #region
        if (checkList.Count < 1)
        {
            return null;
        }

        GameObject nearObject = null;
        int length = 999999;
       
        for (int i = 0; i < checkList.Count; i++)
        { 
            int checker = -1;
            if (checkList[i].GetComponent<Character>() == false && targetObject.GetComponent<Character>() == true)
            {
                checker = GetRouteLength(targetObject, checkList[i], isIgnoreCharacter);
            }
            else
            { 
                checker = GetRouteLength(checkList[i], targetObject, isIgnoreCharacter);
            }

            if ((checker != -1) && (checker < length))
            {
                nearObject = checkList[i];
                length = checker;
            }
        }

        if (length != 999999)
        {
            return nearObject;
        }
        else
        {
            return null;
        }
        #endregion
    }

    public GameObject GetFarObject(List<GameObject> checkList, GameObject targetObject, bool isIgnoreCharacter) //��ǥ������Ʈ�ϰ� ���� �ָ� �ִ� ������Ʈ�� ��ȯ�޴� �ڵ�
    {
        #region
        if (checkList.Count < 1)
        {
            return null;
        }

        GameObject farObject = null;
        int length = -1;
       
        for (int i = 0; i < checkList.Count; i++)
        {
            int checker = -1;
            if (checkList[i].GetComponent<Character>() == false && targetObject.GetComponent<Character>() == true)
            {
                checker = GetRouteLength(targetObject, checkList[i], isIgnoreCharacter);
            }
            else
            { 
                checker = GetRouteLength(checkList[i], targetObject, isIgnoreCharacter);
            }

            if ((checker != -1) && (checker > length))
            {
                farObject = checkList[i];
                length = checker;
            }
        }

        if (length != -1)
        {
            return farObject;
        }
        else
        {
            return null;
        }
        #endregion
    }

    //--
    private List<MapBlock> CharacterInteractRange(int range, GameObject targetObject)
    { 
        #region
        List<MapBlock> InteractAbleMapBlock = new List<MapBlock>();
        GetInteractableTileInformation(new Vector2((int)targetObject.transform.position.x, (int)targetObject.transform.position.y), InteractAbleMapBlock);
        SideCharacterInteractRange(range, (int)targetObject.transform.position.x, (int)targetObject.transform.position.y, -1, InteractAbleMapBlock);

        for(int i = 0; i < 2; i++)
        { 
            int checkRange = range;
            int targetXPosition = (int)targetObject.transform.position.x;
            int targetYPosition = (int)targetObject.transform.position.y;

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

                GetInteractableTileInformation(new Vector2(targetXPosition, targetYPosition), InteractAbleMapBlock);
                checkRange--;
                SideCharacterInteractRange(checkRange,targetXPosition, targetYPosition, -1, InteractAbleMapBlock);
            }           
        }

        return InteractAbleMapBlock;
        #endregion
    }

    private void SideCharacterInteractRange(int checkRange, int targetXPosition, int targetYPosition, int overlapNum, List<MapBlock> InteractAbleMapBlock)
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

                    GetInteractableTileInformation(new Vector2(sideTargetXPosition, sideTargetYPosition), InteractAbleMapBlock);
                    sideCheckRange--;
                    SideCharacterInteractRange(sideCheckRange,sideTargetXPosition, sideTargetYPosition, sideOverlapNum, InteractAbleMapBlock);
                    break;
                }   
            }
        }
        #endregion
    }

    private void GetInteractableTileInformation(Vector2 ray, List<MapBlock> InteractAbleMapBlock)
    { 
        #region
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(ray, Vector2.zero);

        for(int i = 0; i< hits.Length; i++)
        { 
            if(hits[i].transform.tag == "Tile")
            { 
                InteractAbleMapBlock.Add(hits[i].transform.GetComponent<MapBlock>());
            }    
        } 
        
        #endregion
    }

    private List<MapBlock> GetTargetNearTile(int move, GameObject target)
    { 
        #region
        List<MapBlock> nearTile = new List<MapBlock>();

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(new Vector2 ((int)target.transform.position.x, (int)target.transform.position.y), Vector2.zero);
        for(int  v = 0; v < hits.Length; v++)
        { 
            if(hits[v].transform.tag == "Tile" && hits[v].transform.gameObject.GetComponent<MapBlock>() == true)
            { 
                nearTile.Add(hits[v].transform.gameObject.GetComponent<MapBlock>());
                break;
            }
        }
        

        for(int i = 0; i < 4 ;i++) //0123 ������� �����¿� / 0 : �� / 1 : �� / 2 : �� / 3 : �� /
        { 
            int checkRange = move;
            int targetXPosition = (int)target.transform.position.x;
            int targetYPosition = (int)target.transform.position.y;
            int overlapNum = -1;

            while(checkRange > 0)
            { 
                switch(i)
                { 
                    case BattleCharacterManager.UP:
                        overlapNum = BattleCharacterManager.DOWN;
                        targetYPosition += GameManager.TILESIZE;
                        break;
                    case BattleCharacterManager.DOWN:
                        overlapNum = BattleCharacterManager.UP;
                        targetYPosition -= GameManager.TILESIZE;
                        break;
                    case BattleCharacterManager.LEFT:
                        overlapNum = BattleCharacterManager.RIGHT;
                        targetXPosition -= GameManager.TILESIZE;
                        break;
                    case BattleCharacterManager.RIGHT:
                        overlapNum = BattleCharacterManager.LEFT;
                        targetXPosition += GameManager.TILESIZE;
                        break;                        
                }

                if(GetMoveableTileInformation(new Vector2(targetXPosition, targetYPosition), nearTile) == true)
                { 
                    checkRange = checkRange - 1;
                    CheckSideTargetNearTile(checkRange, targetXPosition, targetYPosition, overlapNum, nearTile);
                    break;
                }
                else
                { 
                    checkRange = 0;
                }

            }
        }  

        return nearTile;
        #endregion
    }

    private void CheckSideTargetNearTile(int checkRange, int targetXPosition, int targetYPosition, int overlapNum, List<MapBlock> nearTile)
    {  
        #region
        int sideOverlapNum = overlapNum;
        for(int j = 0; j < 4; j++)
        { 
            int sideCheckRange = checkRange;

            int sideTargetXPosition = targetXPosition;
            int sideTargetYPosition = targetYPosition;

            if(j != overlapNum)
            { 
                while(sideCheckRange > 0)
                { 
                    switch(j)
                    { 
                        case BattleCharacterManager.UP:                           
                            sideOverlapNum = BattleCharacterManager.DOWN;
                            sideTargetYPosition += GameManager.TILESIZE;
                            break;
                        case BattleCharacterManager.DOWN:                        
                            sideOverlapNum = BattleCharacterManager.UP;
                            sideTargetYPosition -= GameManager.TILESIZE;
                            break;
                        case BattleCharacterManager.LEFT:                            
                            sideOverlapNum = BattleCharacterManager.RIGHT;
                            sideTargetXPosition -= GameManager.TILESIZE;
                            break;
                        case BattleCharacterManager.RIGHT:                            
                            sideOverlapNum = BattleCharacterManager.LEFT;
                            sideTargetXPosition += GameManager.TILESIZE;
                            break;
                    }

                    if(GetMoveableTileInformation(new Vector2(sideTargetXPosition, sideTargetYPosition), nearTile) == true)
                    {    
                        sideCheckRange = sideCheckRange - 1;
                        CheckSideTargetNearTile(sideCheckRange, sideTargetXPosition, sideTargetYPosition, sideOverlapNum, nearTile);
                        break;
                    }
                    else
                    { 
                        sideCheckRange = 0;
                    }
                }
            }
        } 
        #endregion
    }

    private bool GetMoveableTileInformation(Vector2 ray, List<MapBlock> nearTile)
    { 
        #region
        int tileCheckCount = 0;
        int isTileTrue = 0;
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(ray, Vector2.zero);

        for(int i = 0; i< hits.Length; i++)
        { 
            if ((GameManager.instance.CantMoveTagCheck(hits[i].transform.tag) == true) && (hits[i].transform.tag != "Character"))
            { 
                tileCheckCount ++;
            }    
            else if(hits[i].transform.tag == "Tile")
            { 
                isTileTrue ++;
            }
        }  

        if((tileCheckCount == 0) && (isTileTrue > 0))
        { 
            for(int i = 0; i< hits.Length; i++)
            { 
                if(hits[i].transform.tag == "Tile")
                { 
                    int overlapCount = 0;

                    if (hits[i].transform.GetComponent<MapBlock>().tileType == TileType.UnableMoveTile) //�̵��Ұ����� Ÿ������ �˻�
                    {
                        return false;
                    }

                    for(int j = 0; j < nearTile.Count; j++)
                    { 
                        if(hits[i].transform.gameObject == nearTile[j].gameObject)
                        {                                                    
                            overlapCount++;
                            break;
                        }
                    }

                    if(overlapCount == 0)
                    {      
                        nearTile.Add(hits[i].transform.GetComponent<MapBlock>());
                    }
                }
            }
            return true;
        } 
        else
        { 
            return false;    
        }
        #endregion
    }

    //
    public int GetRouteLength(GameObject moveObject, GameObject targetObject, bool isIgnoreCharacter = false)
    {
        GetRouteForAstar(moveObject, targetObject, isIgnoreCharacter);
        if (TileRoute.Count < 1)
        {
            TileRoute.Clear();
            return -1;
        }

        int routeLength = TileRoute[(TileRoute.Count - 1)].costG;
        TileRoute.Clear();
        return routeLength;
    }

    public void GetRouteForAstar(GameObject moveObject, GameObject targetObject, bool isIgnoreCharacter = false)
    {
        Debug.Log("���̽�Ÿ ����");
        TileRoute = new List<AstarNode>();
        TileRoute.Clear();
        if (moveObject == null || targetObject == null)
        {
            Debug.Log("���̽�Ÿ�� null���� ���Խ��ϴ�!");
            return;
        }

        AstarGrid astarGrid = new AstarGrid();
        astarGrid.InitAstarGrid(isIgnoreCharacter);

        AstarNode startTile = astarGrid.GetAstarNodeFromPosition(new Vector2(moveObject.transform.position.x, moveObject.transform.position.y));
        AstarNode endTile = astarGrid.GetAstarNodeFromPosition(new Vector2(targetObject.transform.position.x, targetObject.transform.position.y));

        List<AstarNode> openList= new List<AstarNode>();
        HashSet<AstarNode> closeList= new HashSet<AstarNode>();
        openList.Add(startTile);

        while (openList.Count > 0)
        {
            AstarNode currentNode = openList[0];

            for (int i = 0; i < openList.Count; i++)
            {
                if ((openList[i].costF < currentNode.costF) || ((openList[i].costF == currentNode.costF) && (openList[i].costH < currentNode.costH)))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            if (currentNode == endTile)
            {
                RetracePath(startTile, endTile);
                return;
            }

            foreach (AstarNode n in astarGrid.GetNeighbours(currentNode))
            {
                if (n.isMoveAble == false || closeList.Contains(n) == true)
                {
                    continue;
                }

                int newCurrentToNeighbourCost = GetGCost(currentNode, n);
                if (newCurrentToNeighbourCost < n.costG || openList.Contains(n) == false)
                {
                    n.costG = newCurrentToNeighbourCost;
                    n.costH = GetDistanceCost(n, endTile);
                    n.parentNode = currentNode;

                    if (openList.Contains(n) == false)
                    {
                        openList.Add(n);
                    }
                }
            }
        }
        Debug.Log("�̵��Ұ�");
    }

    private void RetracePath(AstarNode startTile, AstarNode endTile)
    {
        //Debug.LogError("�˻�Ϸ�");
        AstarNode currentNode = endTile;

        while (currentNode != startTile)
        {
            TileRoute.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        TileRoute.Reverse();

        /*
        for (int i = 0; i < TileRoute.Count; i++)
        {
            Debug.LogWarning(TileRoute[i].mapBlock.gameObject);
        }
        */
    }

    private int GetGCost(AstarNode nodeA, AstarNode nodeB)
    {
        return (nodeA.costG + nodeB.thisTileMoveCost);
    }

    private int GetDistanceCost(AstarNode nodeA, AstarNode nodeB)
    {
        int dirX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dirY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        
        return (dirX + dirY);
    }
}

public class AstarNode
{
    public MapBlock mapBlock;
    public bool isMoveAble = true;
    public Vector2 worldPosition;
    public int gridX; //�ش� ����� �迭�� x��ǥ
    public int gridY; // �ش� ����� �迭�� y��ǥ

    public int thisTileMoveCost;
    public int costG = 0; //�̵��ϴµ� ��� ���
    public int costH = 0;
    public AstarNode parentNode;

    public AstarNode(MapBlock newMapBlock, int x, int y, bool isIgnoreCharacter)
    {
        #region
        if (newMapBlock != null)
        {
            mapBlock = newMapBlock;
            worldPosition = new Vector2(mapBlock.gameObject.transform.position.x, mapBlock.gameObject.transform.position.y);

            if (mapBlock.tileType == TileType.UnableMoveTile)
            {
                isMoveAble = false;
            }
            else
            {
                RaycastHit2D[] hits;
                hits = Physics2D.RaycastAll(worldPosition, Vector2.zero);

                for (int i = 0; i < hits.Length; i++)
                {
                    if (GameManager.instance.CantMoveTagCheck(hits[i].transform.tag) == true)
                    {
                        if ((isIgnoreCharacter == true) && (hits[i].transform.tag == "Character"))
                        {
                            break;
                        }
                        isMoveAble = false;
                        break;
                    }
                }
            }
        }

        thisTileMoveCost = mapBlock.tileMoveValue;
        gridX = x;
        gridY = y;
        #endregion
    }

    public int costF
    { 
        get { return costG + costH; }     
    }
}

public class AstarGrid
{
    AstarNode[,] tile;
    int tileXLength;
    int tileYLength; 

    public void InitAstarGrid(bool isIgnoreCharacter)
    {
        tileXLength = GameManager.instance.battleManager.mapBlocks.GetLength(0);
        tileYLength = GameManager.instance.battleManager.mapBlocks.GetLength(1); 

        CreateAstarNode(isIgnoreCharacter);
    }

    void CreateAstarNode(bool isIgnoreCharacter)
    {
        tile = new AstarNode [tileXLength, tileYLength];

        for (int i = 0; i < tileXLength; i++)
        {
            for (int j = 0; j < tileYLength; j++)
            {
                tile[i,j] = new AstarNode(GameManager.instance.battleManager.mapBlocks[i,j], i, j, isIgnoreCharacter);
            }
        }
    }

    public List<AstarNode> GetNeighbours(AstarNode astarNode) // �̿� ��带 ����Ʈ�� ��ȯ�޴� �ڵ�
    {
        #region
        List<AstarNode> neighbours = new List<AstarNode>();
        int xPos = astarNode.gridX;
        int yPos = astarNode.gridY;

        if ((xPos + 1) < tileXLength) 
        {
            neighbours.Add(tile[xPos + 1 , yPos]);     
        }

        if ((xPos - 1) >= 0) 
        {
            neighbours.Add(tile[xPos - 1 , yPos]);          
        }

        if ((yPos + 1) < tileYLength)
        {
           neighbours.Add(tile[xPos, yPos  + 1]);             
        }

        if ((yPos - 1) >= 0)
        {
             neighbours.Add(tile[xPos, yPos  - 1]);
        }

        return neighbours;
        #endregion
    }    

    public AstarNode GetAstarNodeFromPosition(Vector2 pos) // ��ǥ������ ��带 �����޴� �ڵ�
    {
        #region
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(pos, Vector2.zero);       

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.GetComponent<MapBlock>() == true)
            {
                for (int j = 0; j < tileXLength; j++)
                {
                    for (int k = 0; k < tileYLength; k++)
                    {
                        if (hits[i].transform.GetComponent<MapBlock>() == tile[j, k].mapBlock)
                        {
                            return tile[j, k];
                        }
                    }
                }
            }
        }

        return null;
        #endregion
    }
}