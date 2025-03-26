using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterManager : MonoBehaviour
{
    public const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3;

    private BattleManager battleManager;
    private AIManager aiManager;

    private int characterXPosition;
    private int characterYPosition;

    public List<GameObject> actionAbleTile = new List<GameObject>(); 

    public List<GameObject> AttackableTarget = new List<GameObject>(); //광역기 용, BattleManager의 BallMoveEnd 시점과 actionableTile계산할때 초기화
    public List<GameObject> InteractableTarget = new List<GameObject>(); // 광역기 용, BattleManager의 BallMoveEnd 시점과 actionableTile계산할때 초기화

    private int tileMoveValue = 1;

    void Start()
    {
        InitializeBattleCharacterManager();
    }

    private void InitializeBattleCharacterManager()
    {
        battleManager = this.gameObject.GetComponent<BattleManager>();
        aiManager = this.gameObject.GetComponent<AIManager>();
        actionAbleTile.Clear();
    }

   
    public void CharacterMove(ref GameObject targetCharacterObject)
    {
        #region
       
        // 플레이어가 적캐릭터 마우스이용해서 움직이지 못하게 하는 기능
        if ((targetCharacterObject == null) || (targetCharacterObject.GetComponent<Character>().isEnemy == true))
        {
            return;
        }

        if(battleManager.IsBattleCharacterCanMove() == true)
        {
            int toMoveXPos = 0;
            int toMoveYPos = 0;
                
            GameObject targetObject = null;
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.tag == "Tile")
                {
                    if(hits[i].transform.GetComponent<MapBlock>().isActionAble == true)
                    { 
                        targetObject = hits[i].collider.gameObject;

                        if (targetCharacterObject != null)
                        {
                            toMoveXPos = targetObject.GetComponent<MapBlock>().mapBlockXpos;
                            toMoveYPos = targetObject.GetComponent<MapBlock>().mapBlockYpos;
                        }

                        targetCharacterObject.GetComponent<Character>().MovePositionByAnimation(targetObject.GetComponent<MapBlock>(), ref battleManager);

                        Debug.Log(targetCharacterObject.GetComponent<Character>().status.inGameName + "의 이동 위치는 " + (int)toMoveXPos + "," + (int)toMoveYPos);
                        VoiceDatabaseManager.instance.PlayVoice(targetCharacterObject.GetComponent<Character>().status.voice, VoiceType.Move);

                        AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Moveable);
                        //battleManager.battleGetInformationManager.ClearTargetCharacter(); //나중에 이거 순서 미뤄야함 이동 후 커맨드창 만들꺼면
                    }
                }
            }
            
        }
        #endregion
    }
   

    public void CharacterTap()
    {
        if(GameManager.instance.KeyCheckAccept() == true)
        { 
            CharacterTapActive();
        }
    }

    public void CharacterTapActive()
    { 
        #region
        if (battleManager.targetCharacter != null)
        {
            characterXPosition = battleManager.targetCharacter.GetComponent<Character>().CharacterCurrentXPosition;
            characterYPosition = battleManager.targetCharacter.GetComponent<Character>().CharacterCurrentYPosition;
            for (int i = 0; i < battleManager.tileWidth; i++)
            {
                for (int j = 0; j < battleManager.tileHeight; j++)
                {
                    if (battleManager.mapBlocks[i, j].mapBlockXpos == characterXPosition && battleManager.mapBlocks[i, j].mapBlockYpos == characterYPosition)
                    {
                        battleManager.mapBlocks[i, j].SetSelectionMode(MapBlock.Highlight.Moveable);
                    }
                }
            }

            GameManager.instance.SetCameraPosition(battleManager.targetCharacter);

            Debug.Log("이동범위 = " + battleManager.targetCharacter.GetComponent<Character>().status.move);
            ShowCharacterMoveArea();
        }     
        #endregion
    }

    public void ShowCharacterMoveArea()
    { 
        CharacterMoveRange(battleManager.targetCharacter.GetComponent<Character>().status.move, ref battleManager.targetCharacter);                
        ActiveActionAbleTile(MapBlock.Highlight.Moveable);
        battleManager.battlePhaseManager.ChangePhase(BattlePhase.MyTurn_Moving);       
    }

    public void ShowCharacterMoveRange(ref GameObject character) // 그냥 이동 범위만 뛰어줄때는 이것을 쓸것
    { 
        if(character.GetComponent<Character>() == true)
        { 
            CharacterMoveRange(character.GetComponent<Character>().status.move, ref character);                
            ActiveActionAbleTile(MapBlock.Highlight.ShowArea);       
        }      
    }

    public void GetAICharacterMoveTile(ref GameObject character)
    {
        //aiManager.AIInteractionCharacterReset();
        if (character.GetComponent<Character>() == true)
        {
            CharacterMoveRange(character.GetComponent<Character>().status.move, ref character);
        }
    }

    public void GetAICharacterInteractionInMoveTile(ref GameObject character) // AI 모든 이동력 타일 중 상호작용 가능한 타일 검사
    {
        aiManager.ClearCharacterInteractList();

        if (character.GetComponent<Character>() == true)
        {
            CharacterInteractRange(character.GetComponent<Character>().status.move, character);
        }
    }

    public void GetAICharacterInteractionInRangeTile(ref GameObject character) // AI 모든 사거리 타일 중 상호작용 가능한 타일 검사
    {
        aiManager.ClearCharacterInteractList();

        if (character.GetComponent<Character>() == true)
        {
            CharacterInteractRange(character.GetComponent<Character>().status.range, character);
        }
    }

    public void ReturnForBeforeMove()
    {  
        #region
        if (battleManager.targetCharacter != null)
        { 
            AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Off);
            battleManager.battleGetInformationManager.SetBattleCommandWindow(false);
            if(battleManager.targetCharacter.GetComponent<Character>().isHaveBall != battleManager.targetCharacter.GetComponent<Character>().saveIsHaveBall)
            { 
                battleManager.targetCharacter.GetComponent<Character>().ball.GetComponent<BallController>().GetForBallBeforeMove();
            }

            battleManager.targetCharacter.GetComponent<Character>().ReturnForBeforeMove(); 
            battleManager.isMoveReturn = true; 
            Invoke("CharacterTapActive", 0.1f); //캐릭터를 원 위치로 복귀시켜도 이동했던 위치에 캐릭터가 있다고 인식해서 지연함수 넣어둠
        }
        #endregion
    }

    public void CharacterMoveEnd(bool isReturnCommand = false)
    {
        #region
         AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Off);//초기화

        if(battleManager.targetCharacter == null)
        {
            return;
        }

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(new Vector2 ((int)battleManager.targetCharacter.transform.position.x, (int)battleManager.targetCharacter.transform.position.y), Vector2.zero);
        for(int i = 0; i < hits.Length; i++)
        { 
            if(hits[i].transform.tag == "Tile")
            {
                if(battleManager.targetCharacter.GetComponent<Character>().isEnemy == false)
                {
                    hits[i].transform.GetComponent<MapBlock>().SetSelectionMode(MapBlock.Highlight.Target);
                }
            }

            if((hits[i].transform.tag == "Ball") && (battleManager.targetCharacter.GetComponent<Character>().ball == null))
            {          
                TargetCharacterGetBall(hits[i].transform.gameObject);   
                //행동으로 인한 sp변화(이동)
                //필드에서 공을 주었을 때
                battleManager.ChangeSympathyByNature(battleManager.targetCharacter, ChangeSympathySituation.GetBall);               
            }
        }
        //GameManager.instance.SetCameraPosition(battleManager.targetCharacter); //카메라 이동
        GameManager.instance.ResetCameraMoveTarget();//카메라 이동

        if(battleManager.targetCharacter.GetComponent<Character>().isEnemy == true)
        {
            Invoke("SetEnemyTurnCommand", 0.5f);
            battleManager.battlePhaseManager.ChangePhase(BattlePhase.Stay);
            return;
        }

        battleManager.battlePhaseManager.ChangePhase(BattlePhase.MyTurn_Command);
        CharacterInteractRange(battleManager.targetCharacter.GetComponent<Character>().status.range, battleManager.targetCharacter);
        if (isReturnCommand == false)
        {
            battleManager.battleGetInformationManager.SetBattleCommandWindow(true, true);
        }
        else
        {
            battleManager.battleGetInformationManager.SetBattleCommandWindow(true);
        }      
        #endregion
    }

    public void SetEnemyTurnCommand()
    {
        battleManager.battlePhaseManager.ChangePhase(BattlePhase.EnemyTurn_Command);
    }

    public void ReturnForBeforeCommand()
    { 
        AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Off);
        battleManager.ClearUseSkill();
        CharacterMoveEnd(true);
    }
   
    public void TargetCharacterGetBall(GameObject ball)
    { 
        if(battleManager.targetCharacter != null)
        {             
            battleManager.targetCharacter.GetComponent<Character>().SetIsHaveBall(true, ball);
        }
    }

    private void CharacterMoveRange(int range, ref GameObject targetCharacterObject)
    { 
        #region
        actionAbleTile.Clear();
        battleManager.ClearMapBlockNavTile(range);
        int characterMove = range;


        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(new Vector2 ((int)targetCharacterObject.transform.position.x, (int)targetCharacterObject.transform.position.y), Vector2.zero);
        for(int  v = 0; v < hits.Length; v++)
        { 
            if(hits[v].transform.tag == "Tile")
            { 
                actionAbleTile.Add(hits[v].transform.gameObject);
                break;
            }
        }
        

        for(int i = 0; i < 4 ;i++) //0123 순서대로 상하좌우 / 0 : 상 / 1 : 하 / 2 : 좌 / 3 : 우 /
        { 
            int checkRange = range;
            int targetXPosition = (int)targetCharacterObject.transform.position.x;
            int targetYPosition = (int)targetCharacterObject.transform.position.y;
            int overlapNum = 0;

            List<int> navTile = new List<int>();

            while(checkRange > 0)
            { 
                switch(i)
                { 
                    case UP:
                        navTile.Add(UP);
                        overlapNum = DOWN;
                        targetYPosition += GameManager.TILESIZE;
                        break;
                    case DOWN:
                        navTile.Add(DOWN);
                        overlapNum = UP;
                        targetYPosition -= GameManager.TILESIZE;
                        break;
                    case LEFT:
                        navTile.Add(LEFT);
                        overlapNum = RIGHT;
                        targetXPosition -= GameManager.TILESIZE;
                        break;
                    case RIGHT:
                        navTile.Add(RIGHT);
                        overlapNum = LEFT;
                        targetXPosition += GameManager.TILESIZE;
                        break;                        
                }

                if(GetMoveableTileInformation(new Vector2(targetXPosition, targetYPosition), navTile) == true)
                { 
                    checkRange = checkRange - tileMoveValue;
                    tileMoveValue = 1;
                    SideCharacterMoveRange(checkRange, targetXPosition, targetYPosition, overlapNum, navTile, characterMove);
                    break;
                }
                else
                { 
                    checkRange = 0;
                }

            }
        }  
        #endregion
    }

    private void SideCharacterMoveRange(int checkRange, int targetXPosition, int targetYPosition, int overlapNum, List<int> navTile, int characterMove)
    {  
        #region
        int sideOverlapNum = overlapNum;
        for(int j = 0; j < 4; j++)
        { 
            int sideCheckRange = checkRange;

            int sideTargetXPosition = targetXPosition;
            int sideTargetYPosition = targetYPosition;            
            List<int> sideNavTile = new List<int>();
            
            for(int z = 0; z < navTile.Count; z++)
            { 
                sideNavTile.Add(navTile[z]);
            }
            

            if(j != overlapNum)
            { 
                while(sideCheckRange > 0)
                { 
                    switch(j)
                    { 
                        case UP:
                            sideNavTile.Add(UP);
                            sideOverlapNum = DOWN;
                            sideTargetYPosition += GameManager.TILESIZE;
                            break;
                        case DOWN:
                            sideNavTile.Add(DOWN);
                            sideOverlapNum = UP;
                            sideTargetYPosition -= GameManager.TILESIZE;
                            break;
                        case LEFT:
                            sideNavTile.Add(LEFT);
                            sideOverlapNum = RIGHT;
                            sideTargetXPosition -= GameManager.TILESIZE;
                            break;
                        case RIGHT:
                            sideNavTile.Add(RIGHT);
                            sideOverlapNum = LEFT;
                            sideTargetXPosition += GameManager.TILESIZE;
                            break;
                    }

                    if(GetMoveableTileInformation(new Vector2(sideTargetXPosition, sideTargetYPosition), sideNavTile) == true)
                    {    
                        sideCheckRange = sideCheckRange - tileMoveValue;
                        tileMoveValue = 1;
                        SideCharacterMoveRange(sideCheckRange, sideTargetXPosition, sideTargetYPosition, sideOverlapNum, sideNavTile, characterMove);
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

    private bool GetMoveableTileInformation(Vector2 ray, List<int> navTile)
    { 
        #region
        int tileCheckCount = 0;
        int isTileTrue = 0;
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(ray, Vector2.zero);

        for(int i = 0; i< hits.Length; i++)
        { 
            if (GameManager.instance.CantMoveTagCheck(hits[i].transform.tag) == true)
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

                    if (hits[i].transform.GetComponent<MapBlock>().tileType == TileType.UnableMoveTile) //이동불가능한 타일인지 검사
                    {
                        return false;
                    }

                    tileMoveValue = hits[i].transform.gameObject.GetComponent<MapBlock>().tileMoveValue;

                    for(int j = 0; j < actionAbleTile.Count; j++)
                    { 
                        int tileNavCheck = 0;
                        int ovelapNavCheck = 0;

                        if(hits[i].transform.gameObject == actionAbleTile[j])
                        {                          
                            tileNavCheck = navTile.Count;
                            ovelapNavCheck = actionAbleTile[j].GetComponent<MapBlock>().navTile.Length;
                           

                            if(ovelapNavCheck > tileNavCheck)
                            { 
                                actionAbleTile[j].GetComponent<MapBlock>().navTile = new int[navTile.Count];
                                for(int e = 0; e < navTile.Count; e++)
                                { 
                                    actionAbleTile[j].GetComponent<MapBlock>().navTile[e] = navTile[e];   
                                }                                                               
                                
                            }                           
                            overlapCount++;
                        }
                    }

                    if(overlapCount == 0)
                    { 
                        hits[i].transform.GetComponent<MapBlock>().navTile = new int[navTile.Count];

                        for(int e = 0; e < navTile.Count; e++)
                        { 
                            hits[i].transform.GetComponent<MapBlock>().navTile[e] = navTile[e];   
                        }        

                        actionAbleTile.Add(hits[i].transform.gameObject);

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

    private void CharacterInteractRange(int range, GameObject targetCharacterObject)
    { 
        #region
        actionAbleTile.Clear();
        AttackableTarget.Clear();
        InteractableTarget.Clear();
        SideCharacterInteractRange(range, (int)targetCharacterObject.transform.position.x, (int)targetCharacterObject.transform.position.y, -1, targetCharacterObject.GetComponent<Character>().isEnemy);

        for(int i = 0; i < 2; i++)
        { 
            int checkRange = range;
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

                GetInteractableTileInformation(new Vector2(targetXPosition, targetYPosition), targetCharacterObject.GetComponent<Character>().isEnemy);
                checkRange--;
                SideCharacterInteractRange(checkRange,targetXPosition, targetYPosition, -1,targetCharacterObject.GetComponent<Character>().isEnemy);
            }           
        }
        #endregion
    }

    private void SideCharacterInteractRange(int checkRange, int targetXPosition, int targetYPosition, int overlapNum, bool isEnemy)
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

                    GetInteractableTileInformation(new Vector2(sideTargetXPosition, sideTargetYPosition), isEnemy);
                    sideCheckRange--;
                    SideCharacterInteractRange(sideCheckRange,sideTargetXPosition, sideTargetYPosition, sideOverlapNum, isEnemy);
                    break;
                }   
            }
        }
        #endregion
    }

    private void GetInteractableTileInformation(Vector2 ray, bool isEnemy)
    { 
        #region
        int tileCheckCount = 0;
        bool enemyCheck = false;
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(ray, Vector2.zero);


        for(int i = 0; i< hits.Length; i++)
        { 
            if(hits[i].transform.tag == "Character")
            { 
                tileCheckCount ++;

                if(hits[i].transform.GetComponent<Character>().isEnemy != isEnemy)
                { 
                    enemyCheck = true;
                    AttackableTarget.Add(hits[i].transform.gameObject);
                }
                else
                { 
                    enemyCheck = false;
                    InteractableTarget.Add(hits[i].transform.gameObject);
                }

                if(isEnemy == true)
                { 
                    if(enemyCheck == true)
                    { 
                        aiManager.AttackableTarget.Add(hits[i].transform.gameObject);
                    }
                    else
                    { 
                        aiManager.InteractableTarget.Add(hits[i].transform.gameObject);
                    }
                    return;    
                }
                break;
            }    
        }  

        if(tileCheckCount >= 1)
        { 
            for(int i = 0; i< hits.Length; i++)
            { 
                if(hits[i].transform.tag == "Tile")
                { 

                    if(enemyCheck == true)
                    { 
                        SetActiveAbleMapBlock(hits[i].transform.GetComponent<MapBlock>(), MapBlock.Highlight.Attackable);
                    }
                    else
                    { 
                        SetActiveAbleMapBlock(hits[i].transform.GetComponent<MapBlock>(), MapBlock.Highlight.Interactable);
                    }
                }    
            } 
        }     
        #endregion
    }

    public void ShowSkillTargetCharacter(Skill useSKill)
    { 
        switch(useSKill.skillTarget)
        {
            case SkillTarget.Enemy:
                AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Interactable);
                break; 
            case SkillTarget.Friendly:
                AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Attackable);
                break; 
            case SkillTarget.Self:
                AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Interactable);
                AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Attackable);
                SetTargetCharacterTileToActiveAbleMapBlock();
                break; 
            case SkillTarget.FriendlyAndSelf:
                AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight.Attackable);
                SetTargetCharacterTileToActiveAbleMapBlock();
                break; 
            case SkillTarget.All:
                SetTargetCharacterTileToActiveAbleMapBlock();
                break;
        }   

        if(useSKill.skillType == SkillType.InterruptBall)
        { 
            CheckActionAbleTileCharacterIsHaveBall();
        }
    }

    private void SetTargetCharacterTileToActiveAbleMapBlock()
    { 
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(new Vector2 ((int)battleManager.targetCharacter.transform.position.x, (int)battleManager.targetCharacter.transform.position.y), Vector2.zero);
        for(int i = 0; i < hits.Length; i++)
        { 
            if(hits[i].transform.tag == "Tile")
            { 
                SetActiveAbleMapBlock(hits[i].transform.GetComponent<MapBlock>(), MapBlock.Highlight.Interactable);
                break;
            }
        }        
    }

    private void ActiveActionAbleTile(MapBlock.Highlight mode)
    { 
        Debug.Log("저장된 타일의 갯수는 " + actionAbleTile.Count);
        for(int i = 0; i < actionAbleTile.Count; i++)
        {             
            actionAbleTile[i].GetComponent<MapBlock>().SetSelectionMode(mode);
            if(mode != MapBlock.Highlight.ShowArea)
            { 
                actionAbleTile[i].GetComponent<MapBlock>().isActionAble = true;
            }
        }
    }

    private void SetActiveAbleMapBlock(MapBlock mapBlock, MapBlock.Highlight mode)
    { 
        mapBlock.SetSelectionMode(mode);
        mapBlock.isActionAble = true;
    }

    private void CheckActionAbleTileCharacterIsHaveBall()
    { 
        #region
        for (int i = 0; i < battleManager.tileWidth; i++)
        {
            for (int j = 0; j < battleManager.tileHeight; j++)
            {
                if(battleManager.mapBlocks[i, j] != null)
                { 
                    if(battleManager.mapBlocks[i, j].isActionAble == true)
                    { 
                        bool ballCheck = false;

                        RaycastHit2D[] hits;
                        hits = Physics2D.RaycastAll(new Vector2 (battleManager.mapBlocks[i, j].gameObject.transform.position.x, battleManager.mapBlocks[i, j].gameObject.transform.position.y), Vector2.zero);
                        for(int k = 0; k < hits.Length; k++)
                        { 
                            if(hits[k].transform.tag == "Character")
                            { 
                                if(hits[k].transform.gameObject.GetComponent<Character>().GetIsHaveBall() == true)
                                { 
                                    ballCheck = true;
                                    break;
                                }
                            }                                
                        }

                        if(ballCheck != true)
                        { 
                            battleManager.mapBlocks[i, j].UnSelectMapBlockByHighlightMode(MapBlock.Highlight.Off);
                        }
                    }
                }
            }
        }  
        #endregion
    }

    public void AllMapBlockSelectionModeClearByHighlightMode(MapBlock.Highlight mode)
    { 
        battleManager.battleGetInformationManager.AllMapBlockSelectionModeClearByHighlightMode(mode); 
    }

    public void AICharacterInteractRange(int range, GameObject checkTarget)
    {
        battleManager.aiManager.ClearCharacterInteractList();
        CharacterInteractRange(range, checkTarget);
    }

    
}
