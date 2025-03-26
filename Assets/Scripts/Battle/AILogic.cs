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

    public virtual void ReadyForAITurnStart() //������ ������� üũ
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

        if (saveIsEnemyStrategicPointCountEnough != aiManager.isEnemyStrategicPointCountEnough) //������ ������� ���ΰ� �ٲ������ ��ǥ �ʱ�ȭ
        {
            for (int i = 0; i < aiManager.battleManager.enemyCharacters.Length; i++)
            {
                aiManager.battleManager.enemyCharacters[i].GetComponent<Character>().aiTargetMapBlock = null;
            }
        }
        #endregion
    }

    public abstract void SetAICharacter(); //�ൿ�� ĳ���͸� �����ϴ� �κ�
    
    public abstract void AICharacterMove(); //AI�� �̵��� �� �Ǵ��ϴ� �κ�

    public abstract void AICharacterCommand(); //AI�� �ൿ�� �Ǵ��ϴ� �κ�

    public virtual bool MoveToTarget(GameObject targetCharacter) //��ǥ�� ���Ͽ� �̵�
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

    public virtual MapBlock GetBestMoveableTileToTarget(GameObject moveObject, GameObject targetMapBlock) // targetMapBlock������ �Ÿ��� ���� �̹��� moveObject�� �̵������� �ְ� �Ÿ��� Ÿ���� ��ȯ
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

    public virtual bool CheckAICanMoveToTargetOnce(GameObject targetCharacter) //��ǥ�� �ѹ濡 �̵������ϸ� �� �濡 �̵�
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

    public virtual bool CheckCanMoveToBallOnce(GameObject targetCharacter) //������ �� �濡 �� �� ������ �ѹ濡 �̵�
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

    public virtual bool CheckCanMoveToStrategicPointOnce(GameObject targetCharacter) //�������� �� �濡 �� �� ������ �ѹ濡 �̵�
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

    public virtual MapBlock CheckBallOnCharacterMoveRange(GameObject targetCharacter)  //�̵� �ݰ濡 ���� �ִ��� üũ
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

    public virtual MapBlock CheckStrategicPointOnCharacterMoveRange(GameObject targetCharacter)  //�̵� �ݰ濡 ������ �ִ��� üũ
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

    protected virtual bool CheckPriorityInMoveRange(Character targetCharacter, List<MapBlock> checkMapBlock) //�ش� ����Ʈ���� �켱 ������ �����Ͽ� ��ǥ�� ���� �켱 ������ ���� �� ����(���� -> ȸ��Ÿ�� -> �Ϲ�Ÿ�� -> ��Ÿ�� ��)
    {
        #region
        if (targetCharacter == null || checkMapBlock.Count < 1)
        {
            Debug.LogWarning("CheckStrategicPointInMoveRange�� �߸��� ���� ��� �Խ��ϴ�!");
            return false;
        }

        MapBlock bestMapBlock = GetBestTile(targetCharacter, checkMapBlock);
        if (bestMapBlock != null)
        {
            targetCharacter.aiTargetMapBlock = bestMapBlock;

            if (targetCharacter.aiTargetMapBlock.tileType == TileType.StrategicPoint) //����Ʈ�� �ڱ��ڽ��� �մ� ���̸� ���� �϶� ���ڸ� �̵��� Ŵ
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

    protected virtual MapBlock GetBestTile(Character targetCharacter, List<MapBlock> checkMapBlock) //����Ʈ �� ���� �켱 ������ ���� �� �� Ÿ��ȿ���� ���� ���� ���Ϲ޴� �ڵ�
    {
        #region
        if (targetCharacter == null || checkMapBlock.Count < 1)
        {
            Debug.LogWarning("CheckStrategicPointInMoveRange�� �߸��� ���� ��� �Խ��ϴ�!");
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

    protected virtual MapBlock GetBestTilebuffTile(Character targetCharacter, List<MapBlock> checkMapBlock) //����Ʈ�� ���� Ÿ�� ȿ���� �������� ���� �޴� �ڵ�
    {
        #region
        if (targetCharacter == null || checkMapBlock.Count < 1)
        {
            Debug.LogWarning("GetBestTileBuff�� �߸��� ���� ��� �Խ��ϴ�!");
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
            Debug.LogError("�ڵ尡 �߸� �Ǿ����� ���� ���⼭ ����Ʈ ������ 0�� ���� ���� ���µ� ������");
            return null;
        }
        
        return bestTile[Random.Range(0, bestTile.Count)];
        #endregion
    }

    protected virtual int GetTilebuffEffect(Character targetCharacter,MapBlock checkMapBlock) //Ÿ�� ���� �˻��ϴ� �ڵ�
    {
        #region
        if (targetCharacter == null || checkMapBlock == null)
        {
            Debug.LogWarning("GetTilebuffEffect�� �߸��� ���� ��� �Խ��ϴ�!");
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
            Debug.LogWarning("AI: " + attacker + "�� " + target + "���� " + skill + "�� ���!");
            aiManager.battleManager.UseSkill(attacker, target, skill);
            return true;
        }
        else
        {
            return false;
        }
    }
}
