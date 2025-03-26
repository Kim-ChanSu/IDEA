using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AILogic
{
    protected AIManager aiManager; 

    public void SetAIManager(AIManager newAIManager)
    {
        aiManager = newAIManager;
    }

    public virtual void ReadyForAITurnStart() //거점이 충분한지 체크
    {
        #region
        if (aiManager.isStrategicPointOnMap == false)
        {
            aiManager.isEnemyStrategicPointCountEnough = true;
            return;
        }
        bool saveIsEnemyStrategicPointCountEnough = aiManager.isEnemyStrategicPointCountEnough;

        if ((aiManager.battleManager.strategicPointMapBlocks.Count / 2) < aiManager.battleManager.GetEnemyStrategicPointCount())
        {
            aiManager.isEnemyStrategicPointCountEnough = true;
        }
        else
        {
            aiManager.isEnemyStrategicPointCountEnough = false;
        }

        if (saveIsEnemyStrategicPointCountEnough != aiManager.isEnemyStrategicPointCountEnough) //거점이 충분한지 여부가 바뀌었으면 목표 초기화
        {
            for (int i = 0; i < aiManager.battleManager.enemyCharacters.Length; i++)
            {
                aiManager.battleManager.enemyCharacters[i].GetComponent<Character>().aiTargetMapBlock = null;
            }
        }
        #endregion
    }

    public abstract void SetAICharacter(); //행동할 캐릭터를 선택하는 부분
    
    public abstract void AICharacterMove(); //AI가 이동할 곳 판단하는 부분

    public abstract void AICharacterCommand(); //AI의 행동을 판단하는 부분

    public virtual bool MoveToTarget(GameObject targetCharacter) //목표를 향하여 이동
    {
        #region
        if ((targetCharacter != null) && (targetCharacter.GetComponent<Character>().aiTargetMapBlock != null))
        {
            MapBlock targetMapBlock = GetBestMoveableTileToTarget(targetCharacter, targetCharacter.GetComponent<Character>().aiTargetMapBlock.gameObject);
            if (targetMapBlock != null)
            {
                targetCharacter.GetComponent<Character>().AIMovePositionByAnimation(targetMapBlock, ref aiManager.battleManager);
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
        #endregion
    }

    public virtual MapBlock GetBestMoveableTileToTarget(GameObject moveObject, GameObject targetMapBlock) // targetMapBlock까지의 거리중 가장 이번턴 moveObject가 이동가능한 최고 거리의 타일을 반환
    {
        aiManager.battleManager.battleCharacterManager.GetAICharacterMoveTile(ref moveObject);
        aiManager.GetRouteForAstar(moveObject, targetMapBlock);
        if (aiManager.TileRoute.Count < 1)
        {
            return null;
        }

        List<AstarNode> moveAbleRoute = new List<AstarNode>();

        for (int i = 0; i < aiManager.TileRoute.Count; i++)
        {
            for (int j = 0; j < aiManager.battleManager.battleCharacterManager.actionAbleTile.Count; j++)
            {                    
                if (aiManager.TileRoute[i].mapBlock.gameObject == aiManager.battleManager.battleCharacterManager.actionAbleTile[j])
                {
                    moveAbleRoute.Add(aiManager.TileRoute[i]);
                    break;
                }
            }                
        }
            
        if (moveAbleRoute.Count < 1)
        {
            return null;
        }

        AstarNode target = moveAbleRoute[0];

        for (int i = 0; i < moveAbleRoute.Count; i++)
        {
            if (target.costG < moveAbleRoute[i].costG)
            {
                target = moveAbleRoute[i];
            }
        }

        return target.mapBlock;
    }

    public virtual bool CheckAICanMoveToTargetOnce(GameObject targetCharacter) //목표로 한방에 이동가능하면 한 방에 이동
    {
        #region
        if ((targetCharacter != null) && (targetCharacter.GetComponent<Character>().aiTargetMapBlock != null))
        {
            aiManager.battleManager.battleCharacterManager.GetAICharacterMoveTile(ref targetCharacter);
       
            for (int i = 0; i < aiManager.battleManager.battleCharacterManager.actionAbleTile.Count; i++)
            {
                if (aiManager.battleManager.battleCharacterManager.actionAbleTile[i] == targetCharacter.GetComponent<Character>().aiTargetMapBlock.gameObject)
                {
                    targetCharacter.GetComponent<Character>().AIMovePositionByAnimation(targetCharacter.GetComponent<Character>().aiTargetMapBlock, ref aiManager.battleManager);
                    return true;
                }
            }
        }
        
        return false;
        #endregion
    }

    public virtual bool CheckCanMoveToBallOnce(GameObject targetCharacter) //공까지 한 방에 갈 수 있으면 한방에 이동
    {
        #region
        if (targetCharacter != null)
        {
            MapBlock targetMapBlock = CheckBallOnCharacterMoveRange(targetCharacter);
            if (targetMapBlock != null)
            {
                targetCharacter.GetComponent<Character>().AIMovePositionByAnimation(targetMapBlock, ref aiManager.battleManager);
                return true;
            }
        }
        return false;
        #endregion
    }

    public virtual bool CheckCanMoveToStrategicPointOnce(GameObject targetCharacter) //거점까지 한 방에 갈 수 잇으면 한방에 이동
    {
        #region
        if (targetCharacter != null)
        {
            MapBlock targetMapBlock = CheckStrategicPointOnCharacterMoveRange(targetCharacter);
            if (targetMapBlock != null)
            {
                targetCharacter.GetComponent<Character>().AIMovePositionByAnimation(targetMapBlock, ref aiManager.battleManager);
                return true;
            }
        }
        return false;
        #endregion
    }

    public virtual MapBlock CheckBallOnCharacterMoveRange(GameObject targetCharacter)  //이동 반경에 공이 있는지 체크
    {
        #region
        if ((targetCharacter != null))
        {
            MapBlock targetMapBlock = null;

            aiManager.battleManager.battleCharacterManager.GetAICharacterMoveTile(ref targetCharacter);

            for (int i = 0; i < aiManager.battleManager.battleCharacterManager.actionAbleTile.Count; i++)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(aiManager.battleManager.battleCharacterManager.actionAbleTile[i].transform.position.x, aiManager.battleManager.battleCharacterManager.actionAbleTile[i].transform.position.y), Vector2.zero);
                for (int j = 0; j < hits.Length; j++)
                {
                    if (hits[j].transform.GetComponent<BallController>() == true)
                    {
                        targetMapBlock = aiManager.battleManager.battleCharacterManager.actionAbleTile[i].GetComponent<MapBlock>();
                        aiManager.battleManager.battleCharacterManager.actionAbleTile.Clear();
                        return targetMapBlock;
                    }
                }
            }
        }
        aiManager.battleManager.battleCharacterManager.actionAbleTile.Clear();
        return null;
        #endregion
    }

    public virtual MapBlock CheckStrategicPointOnCharacterMoveRange(GameObject targetCharacter)  //이동 반경에 거점이 있는지 체크
    {
        #region
        if (targetCharacter != null)
        {
            aiManager.battleManager.battleCharacterManager.GetAICharacterMoveTile(ref targetCharacter);
            List<MapBlock> checkstrategicPointMapBlocks = new List<MapBlock>();

            for (int i = 0; i < aiManager.battleManager.strategicPointMapBlocks.Count; i++)
            {
                if (aiManager.battleManager.strategicPointMapBlocks[i].tileOwner != TileOwner.Enemy)
                {
                    checkstrategicPointMapBlocks.Add(aiManager.battleManager.strategicPointMapBlocks[i]);
                }
            }

            if (checkstrategicPointMapBlocks.Count < 1)
            {
                aiManager.battleManager.battleCharacterManager.actionAbleTile.Clear();
                return null;
            }

            for (int i = 0; i < aiManager.battleManager.battleCharacterManager.actionAbleTile.Count; i++)
            {
                for (int j = 0; j < checkstrategicPointMapBlocks.Count; j++)
                {
                    if (checkstrategicPointMapBlocks[j] == aiManager.battleManager.battleCharacterManager.actionAbleTile[i].GetComponent<MapBlock>())
                    {
                        aiManager.battleManager.battleCharacterManager.actionAbleTile.Clear();
                        return checkstrategicPointMapBlocks[j];
                    }
                }              
            }
        }
        aiManager.battleManager.battleCharacterManager.actionAbleTile.Clear();
        return null;
        #endregion
    }

    protected virtual bool CheckPriorityInMoveRange(Character targetCharacter, List<MapBlock> checkMapBlock) //해당 리스트에서 우선 순위를 선택하여 목표로 설정 우선 순위가 없을 시 랜덤(거점 -> 회복타일 -> 일반타일 -> 독타일 순)
    {
        #region
        if (targetCharacter == null || checkMapBlock.Count < 1)
        {
            Debug.LogWarning("CheckStrategicPointInMoveRange에 잘못된 값이 들어 왔습니다!");
            return false;
        }

        MapBlock bestMapBlock = GetBestTile(targetCharacter, checkMapBlock);
        if (bestMapBlock != null)
        {
            targetCharacter.aiTargetMapBlock = bestMapBlock;

            if (targetCharacter.aiTargetMapBlock.tileType == TileType.StrategicPoint) //베스트가 자기자신이 잇는 곳이며 거점 일때 제자리 이동을 킴
            {
                RaycastHit2D[] hits;
                hits = Physics2D.RaycastAll(new Vector2(targetCharacter.gameObject.transform.position.x, targetCharacter.gameObject.transform.position.y), Vector2.zero);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].transform.GetComponent<MapBlock>() == true)
                    {
                        if (targetCharacter.aiTargetMapBlock == hits[i].transform.GetComponent<MapBlock>())
                        {
                            targetCharacter.aiMoveInPlace = true;
                        }
                        break;
                    }
                }
            }

            return true;
        }            
        return false;
        #endregion
    }

    protected virtual MapBlock GetBestTile(Character targetCharacter, List<MapBlock> checkMapBlock) //리스트 중 가장 우선 순위가 높고 그 중 타일효과가 좋은 것을 리턴받는 코드
    {
        #region
        if (targetCharacter == null || checkMapBlock.Count < 1)
        {
            Debug.LogWarning("CheckStrategicPointInMoveRange에 잘못된 값이 들어 왔습니다!");
            return null;
        }

        List<MapBlock> strategicPointMapBlock = new List<MapBlock>();
        List<MapBlock> healTileMapBlock = new List<MapBlock>();
        List<MapBlock> normalTileMapBlock = new List<MapBlock>();
        List<MapBlock> damageTileMapBlock = new List<MapBlock>();

        for (int i = 0; i < checkMapBlock.Count; i++)
        {
            switch (checkMapBlock[i].tileType)
            {
                case TileType.StrategicPoint: 
                    strategicPointMapBlock.Add(checkMapBlock[i]);
                    break;
                case TileType.HealTile: 
                    healTileMapBlock.Add(checkMapBlock[i]);
                    break;
                case TileType.DamageTile: 
                    damageTileMapBlock.Add(checkMapBlock[i]);
                    break;
                default:
                    normalTileMapBlock.Add(checkMapBlock[i]);
                    break;
            }
        }

        MapBlock bestMapBlock  = null;
        if (strategicPointMapBlock.Count > 0)
        {
            bestMapBlock = GetBestTilebuffTile(targetCharacter, strategicPointMapBlock);
            if (bestMapBlock != null)
            {
                return bestMapBlock;
            }
        }
        
        if(healTileMapBlock.Count > 0)
        { 
            bestMapBlock = GetBestTilebuffTile(targetCharacter, healTileMapBlock);
            if (bestMapBlock != null)
            {
                return bestMapBlock;
            }            
        }
        if(normalTileMapBlock.Count > 0)
        { 
            bestMapBlock = GetBestTilebuffTile(targetCharacter, normalTileMapBlock);
            if (bestMapBlock != null)
            {
                return bestMapBlock;
            }            
        }

        if(damageTileMapBlock.Count > 0)
        { 
            bestMapBlock = GetBestTilebuffTile(targetCharacter, damageTileMapBlock);
            if (bestMapBlock != null)
            {
                return bestMapBlock;
            }            
        }

        return null;
        #endregion
    }

    protected virtual MapBlock GetBestTilebuffTile(Character targetCharacter, List<MapBlock> checkMapBlock) //리스트중 가장 타일 효과가 좋은것을 리턴 받는 코드
    {
        #region
        if (targetCharacter == null || checkMapBlock.Count < 1)
        {
            Debug.LogWarning("GetBestTileBuff에 잘못된 값이 들어 왔습니다!");
            return null;
        }

        List<MapBlock> bestTile = new List<MapBlock>();

        bestTile.Add(checkMapBlock[0]);
        int buffCheck = GetTilebuffEffect(targetCharacter, checkMapBlock[0]);

        for (int i = 1; i < checkMapBlock.Count; i++)
        {   
            int nextTilebuffChecker = GetTilebuffEffect(targetCharacter, checkMapBlock[i]);
            if (buffCheck == nextTilebuffChecker)
            {
                bestTile.Add(checkMapBlock[i]);
            }
            else if(buffCheck < nextTilebuffChecker)
            {
                bestTile.Clear();
                bestTile.Add(checkMapBlock[i]);
            }
        }

        if (bestTile.Count < 1)
        {
            Debug.LogError("코드가 잘못 되었으니 고쳐 여기서 리스트 갯수가 0가 나올 수가 없는데 나왔음");
            return null;
        }
        
        return bestTile[Random.Range(0, bestTile.Count)];
        #endregion
    }

    protected virtual int GetTilebuffEffect(Character targetCharacter,MapBlock checkMapBlock) //타일 버프 검사하는 코드
    {
        #region
        if (targetCharacter == null || checkMapBlock == null)
        {
            Debug.LogWarning("GetTilebuffEffect에 잘못된 값이 들어 왔습니다!");
            return -1;
        }

        float changeCheck = StatusEffectDatabaseManager.instance.GetMinCheckStatusChange();    
        int statusChange = 0;
        CharacterStatus status = GameManager.instance.EnemyCharacter[targetCharacter.GetComponent<Character>().characterDatabaseNum];

        if (checkMapBlock.tilebuff.ATKChange > changeCheck)
        {
            statusChange = statusChange + (int)(status.ATK * checkMapBlock.tilebuff.ATKChange) - status.ATK;
        }

        if (checkMapBlock.tilebuff.MAKChange > changeCheck)
        {
            statusChange = statusChange + (int)(status.MAK * checkMapBlock.tilebuff.MAKChange) - status.MAK;
        }

        if (checkMapBlock.tilebuff.DEFChange > changeCheck)
        {
            statusChange = statusChange + (int)(status.DEF * checkMapBlock.tilebuff.DEFChange) - status.DEF;
        }

        if (checkMapBlock.tilebuff.MDFChange > changeCheck)
        {
            statusChange = statusChange + (int)(status.MDF * checkMapBlock.tilebuff.MDFChange) - status.MDF;
        }

        if (checkMapBlock.tilebuff.moveChange != 0)
        {
            statusChange = statusChange + (checkMapBlock.tilebuff.moveChange * 5);
        }

        if (checkMapBlock.tilebuff.rangeChange != 0)
        {
            statusChange = statusChange + (checkMapBlock.tilebuff.rangeChange * 5);
        }

        return statusChange;
        #endregion
    }

    protected virtual bool UseSkill(GameObject attacker, GameObject target, Skill skill)
    {
        if ((attacker != null) && (target != null) && (skill != null))
        {
            Debug.LogWarning("AI: " + attacker + "가 " + target + "에게 " + skill + "을 사용!");
            aiManager.battleManager.UseSkill(attacker, target, skill);
            return true;
        }
        else
        {
            return false;
        }
    }
}
