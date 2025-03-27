using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAILogic : AILogic
{
    public override void SetAICharacter() //�ൿ�� ĳ���͸� �����ϴ� �κ�
    {
        SelectAICharacter();
        aiManager.battleManager.battlePhaseManager.ChangePhase(BattlePhase.EnemyTurn_Moving);
    }

    private void SelectAICharacter() //�ش� ������ �����Ǵ� ĳ���͸� ���� �����̴� �ڵ�
    {
        CheckStrategicPoint(); //������ üũ�Ͽ� ��ǥ�� ��� �ڵ�
        //��ǥ�� �ִ� ĳ���� ���� ��ǥ�� �̵�
        #region
        for (int i = 0; i < aiManager.ActionAbleEnemyCharacter.Count; i++) 
        {
            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().aiTargetMapBlock != null)
            {
                SetSelectAICharacter(aiManager.ActionAbleEnemyCharacter[i]);
                return;
            }
        }
        #endregion
        // ���� ������, �� �濡 �� �� �ִ� ������ �ִ� �� Ȯ��
        #region
        for (int i = 0; i < aiManager.ActionAbleEnemyCharacter.Count; i++) 
        {
            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().GetIsHaveBall() == true)
            {
                continue;
            }

            aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().aiTargetMapBlock = CheckStrategicPointOnCharacterMoveRange(aiManager.ActionAbleEnemyCharacter[i]);
            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().aiTargetMapBlock != null)
            {
                SetSelectAICharacter(aiManager.ActionAbleEnemyCharacter[i]);
                return;
            }
        }
        #endregion
        // ���� ������, �� �濡 ������ �� �� �ִ� �� Ȯ��
        #region 
        for (int i = 0; i < aiManager.ActionAbleEnemyCharacter.Count; i++) 
        {
            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().GetIsHaveBall() == true)
            {
                continue;
            }

            aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().aiTargetMapBlock = CheckBallOnCharacterMoveRange(aiManager.ActionAbleEnemyCharacter[i]);
            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().aiTargetMapBlock != null)
            {
                SetSelectAICharacter(aiManager.ActionAbleEnemyCharacter[i]);
                return;
            }
        }
        #endregion
        // ���� ������, �� �濡 �������� �� �� �ִ��� Ȯ�� 
        #region
        for (int i = 0; i < aiManager.ActionAbleEnemyCharacter.Count; i++) 
        {
            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().GetIsHaveBall() == false)
            {
                continue;
            }

            aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().aiTargetMapBlock = CheckStrategicPointOnCharacterMoveRange(aiManager.ActionAbleEnemyCharacter[i]);
            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().aiTargetMapBlock != null)
            {
                SetSelectAICharacter(aiManager.ActionAbleEnemyCharacter[i]);
                return;
            }
        }
        #endregion
        //���� ������ �� �濡 ���� �ϰ� ��ȣ�ۿ� �������� üũ (���� �켱)
        #region
        for (int i = 0; i < aiManager.ActionAbleEnemyCharacter.Count; i++) 
        {
            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().GetIsHaveBall() == false)
            {
                continue;
            }

            for (int j = 0; j < aiManager.battleManager.playerCharacters.Length; j++)
            {
                List <MapBlock> checkMapBlock = new List <MapBlock>();
                checkMapBlock = aiManager.CheckCharacterCanInteractableMapBlock(aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>(), aiManager.battleManager.playerCharacters[j].GetComponent<Character>());
                if (checkMapBlock.Count > 0)
                {
                    if (CheckPriorityInMoveRange(aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>(), checkMapBlock) == true) 
                    {
                        break;
                    }  
                }
            }

            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().aiTargetMapBlock != null)
            {
                SetSelectAICharacter(aiManager.ActionAbleEnemyCharacter[i]);
                return;
            }
        }
        #endregion
        //���� ������ �� �濡 �ൿ ������ �Ʊ� �ϰ� ��ȣ �ۿ� �������� üũ (�н���, ���� �켱)
        #region
        for (int i = 0; i < aiManager.ActionAbleEnemyCharacter.Count; i++) 
        {
            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().GetIsHaveBall() == false)
            {
                continue;
            }

            for (int j = 0; j < aiManager.ActionAbleEnemyCharacter.Count; j++)
            {
                List <MapBlock> checkMapBlock = new List <MapBlock>();
                if ((aiManager.ActionAbleEnemyCharacter[i] == aiManager.ActionAbleEnemyCharacter[j]) || (aiManager.ActionAbleEnemyCharacter[j].GetComponent<Character>().GetIsHaveBall() == true))
                {
                    continue;
                }
                checkMapBlock = aiManager.CheckCharacterCanInteractableMapBlock(aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>(), aiManager.ActionAbleEnemyCharacter[j].GetComponent<Character>());
                if (checkMapBlock.Count > 0)
                {
                    if (CheckPriorityInMoveRange(aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>(), checkMapBlock) == true) 
                    {
                        break;
                    }  
                }
            }

            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().aiTargetMapBlock != null)
            {
                SetSelectAICharacter(aiManager.ActionAbleEnemyCharacter[i]);
                return;
            }
        }
        #endregion
        //���� ������ ���� �ִ� ��뿡�� ��ȣ�ۿ��� �������� üũ (���� �켱)
        #region
        for (int i = 0; i < aiManager.ActionAbleEnemyCharacter.Count; i++) 
        {
            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().GetIsHaveBall() == true)
            {
                continue;
            }

            for (int j = 0; j < aiManager.battleManager.playerCharacters.Length; j++)
            {
                if (aiManager.battleManager.playerCharacters[j].GetComponent<Character>().GetIsHaveBall() == false)
                {
                    continue;
                }

                List <MapBlock> checkMapBlock = new List <MapBlock>();
                checkMapBlock = aiManager.CheckCharacterCanInteractableMapBlock(aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>(), aiManager.battleManager.playerCharacters[j].GetComponent<Character>());
                if (checkMapBlock.Count > 0)
                {
                    if (CheckPriorityInMoveRange(aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>(), checkMapBlock) == true) 
                    {
                        break;
                    }  
                }
            }

            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().aiTargetMapBlock != null)
            {
                SetSelectAICharacter(aiManager.ActionAbleEnemyCharacter[i]);
                return;
            }
        }
        #endregion
        //���� ���� �� �Ʊ� ĳ���ʹ� ���� �� �ִ� ���� ����� ĳ���� �̴� �ڵ�� �׷� �Ʊ� �Ʊ� �̵��� ĳ���� ���� �������� ū �ǹ� ����

        SetSelectAICharacter(aiManager.ActionAbleEnemyCharacter[Random.Range(0, aiManager.ActionAbleEnemyCharacter.Count)]); //����
    }

    private void SetSelectAICharacter(GameObject targetCharacter)
    {
        aiManager.battleManager.targetCharacter = targetCharacter;
        GameManager.instance.SetCameraPosition(aiManager.battleManager.targetCharacter);
    }

    private void CheckStrategicPoint() //������ ���� ai�ൿ���
    {
        #region
        //���� üũ(��ǻ� �̵� ��ǥ ����)
        if (aiManager.isStrategicPointOnMap == false)
        {
            Debug.Log("�ʿ� ������ �����ϴ�");
            return;
        }

        List<GameObject> CanSetTargetMapBlockCharacter = new List<GameObject>();
        for (int i = 0; i < aiManager.ActionAbleEnemyCharacter.Count; i++)
        {
            if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().aiTargetMapBlock == null)
            {
                CanSetTargetMapBlockCharacter.Add(aiManager.ActionAbleEnemyCharacter[i]);
            }            
        }

        if (aiManager.isEnemyStrategicPointCountEnough == true) //������ ����� ��
        {
            #region
            //���������� �ִ� �� �˻�
            int warningStrategicPointRange = 4; //����
            int carefulStrategicPointRange = 6; //���
            int warningAIMoveRange = 3; //������ �� ai �̵� �ݰ�
            int carefulAIMoveRange = 6; //����� �� ai �̵� �ݰ�

            for (int i = 0; i < aiManager.battleManager.strategicPointMapBlocks.Count; i++)
            {
                MapBlock checkMapBlock = aiManager.battleManager.strategicPointMapBlocks[i];
                if (checkMapBlock.tileOwner == TileOwner.Enemy)
                {                    
                    int playerCharacterCount = 0;
                    int enemyCharacterCount = 0;
                    List<MapBlock> warningMapBlock = new List<MapBlock>();
                    List<GameObject> checkEndCharacter = new List<GameObject>();
                    List<Character> targerToStrategicPointNear = new List<Character>(); //���� �ֺ��� �ְų� ������ �ֺ��� Ÿ������ ��� �ִ� AIĳ���� 
                    // ���� �˻�
                    #region
                    warningMapBlock = aiManager.CheckTargetNearTile(warningStrategicPointRange, checkMapBlock.gameObject);
                    for (int j = 0; j < aiManager.battleManager.enemyCharacters.Length; j++) //��ǥȮ��
                    {
                        if (aiManager.battleManager.enemyCharacters[j].GetComponent<Character>().aiTargetMapBlock != null)
                        {
                            if (warningMapBlock.Contains(aiManager.battleManager.enemyCharacters[j].GetComponent<Character>().aiTargetMapBlock) == true)
                            {
                                enemyCharacterCount++;
                                checkEndCharacter.Add(aiManager.battleManager.enemyCharacters[j]);
                                targerToStrategicPointNear.Add(aiManager.battleManager.enemyCharacters[j].GetComponent<Character>());
                            }
                        }
                    }

                    for (int j = 0; j < warningMapBlock.Count; j++) //������ ��ġ�ϴ��� Ȯ��
                    {
                        RaycastHit2D[] hits;
                        hits = Physics2D.RaycastAll(new Vector2(warningMapBlock[j].gameObject.transform.position.x, warningMapBlock[j].gameObject.transform.position.y), Vector2.zero);

                        for (int k = 0; k < hits.Length; k++)
                        {
                            if ((hits[k].transform.GetComponent<Character>() == true) && (hits[k].transform.tag == "Character"))
                            {
                                if (checkEndCharacter.Contains(hits[k].transform.gameObject) == false)
                                {
                                    if (hits[k].transform.GetComponent<Character>().isEnemy == false)
                                    {
                                        playerCharacterCount++;
                                    }
                                    else
                                    {
                                        enemyCharacterCount++;
                                        targerToStrategicPointNear.Add(hits[k].transform.GetComponent<Character>());
                                    }
                                    checkEndCharacter.Add(hits[k].transform.gameObject);
                                    break;
                                }
                            }
                        }
                    }
                    #endregion                   
                    bool isAIOnSPTile = false;
                    //���� ������ ���� ���� �� ������ ��Ű��(Ÿ���� ��� �ִ� ĳ���Ͱ� �ֳ� Ȯ��)
                    #region
                    if (playerCharacterCount > 0) 
                    {
                        bool isPlayerOnSPTile = false;
                        RaycastHit2D[] hits;
                        hits = Physics2D.RaycastAll(new Vector2(checkMapBlock.gameObject.transform.position.x, checkMapBlock.gameObject.transform.position.y), Vector2.zero);

                        for (int j = 0; j < hits.Length;j++)
                        {
                            if ((hits[j].transform.GetComponent<Character>() == true) && (hits[j].transform.tag == "Character"))
                            {
                                if (hits[j].transform.GetComponent<Character>().isEnemy == false) 
                                { 
                                    isPlayerOnSPTile = true;
                                }
                                else
                                {
                                    isAIOnSPTile = true;                                  
                                }
                                break;
                            }                           
                        }

                        if (isPlayerOnSPTile == true) //��밡 ������ ��� ���� ���
                        {
                            for (int j = 0; j < targerToStrategicPointNear.Count; j++)
                            {
                                targerToStrategicPointNear[j].aiTargetMapBlock = null; //�ش� ������ ���ϴ� ĳ���͵� ��ǥ �ʱ�ȭ
                                if ((CanSetTargetMapBlockCharacter.Contains(targerToStrategicPointNear[j].gameObject) == false) && (targerToStrategicPointNear[j].GetIsTurnEnd() == false))
                                {
                                    CanSetTargetMapBlockCharacter.Add(targerToStrategicPointNear[j].gameObject);
                                }

                            }
                            continue;
                        }

                        if (isAIOnSPTile == false)
                        {   
                            for (int j = 0; j < aiManager.battleManager.enemyCharacters.Length; j++)
                            {
                                if (checkMapBlock == aiManager.battleManager.enemyCharacters[j].GetComponent<Character>().aiTargetMapBlock)
                                {
                                    isAIOnSPTile = true;
                                    break;
                                }
                            }                           
                        }

                        //�ش� ������ ��Ű�� Aiĳ���Ͱ� ���� ���
                        #region
                        if (isAIOnSPTile == false) 
                        {   
                            if (CanSetTargetMapBlockCharacter.Count < 1) //��ǥ�� ������ �� �ִ� ĳ���Ͱ� ���� ���
                            {
                                continue;
                            }
                            if (aiManager.GetNearObject(CanSetTargetMapBlockCharacter, checkMapBlock.gameObject, false) != null) // ��ǥ�� ���� �� �� �ִ� ĳ���� �� ����� Aiĳ���͸� �ش� ������ ��ǥ�� �����̰� ��
                            {
                                Character checkNearAiCharacter = aiManager.GetNearObject(CanSetTargetMapBlockCharacter, checkMapBlock.gameObject, false).GetComponent<Character>(); 
                                if (checkNearAiCharacter != null)
                                {
                                    checkNearAiCharacter.aiTargetMapBlock = checkMapBlock;
                                    CanSetTargetMapBlockCharacter.Remove(checkNearAiCharacter.gameObject);
                                }
                            }
                        }
                        #endregion
                    }                     
                    #endregion

                    //���� ������ �ִ� ĳ���� ���� �˻�
                    if (playerCharacterCount > enemyCharacterCount) 
                    {
                        //�÷��̾ �� ���� ���
                        #region
                        int characterGap = playerCharacterCount - enemyCharacterCount;
                        List <GameObject> setAbleTarget = new List<GameObject>();
                        for (int j = 0; j < CanSetTargetMapBlockCharacter.Count; j++)
                        {
                            setAbleTarget.Add(CanSetTargetMapBlockCharacter[j]);
                        }

                        while (characterGap > 0)
                        {
                            for (int j = 0; j < targerToStrategicPointNear.Count; j++)
                            {
                                if (setAbleTarget.Contains(targerToStrategicPointNear[j].gameObject) == true)
                                {    
                                    setAbleTarget.Remove(targerToStrategicPointNear[j].gameObject);
                                }
                            }

                            if (setAbleTarget.Count < 1)
                            {
                                break;
                            }
                            
                            GameObject nearObject = aiManager.GetNearObject(setAbleTarget, checkMapBlock.gameObject, true);
                            if (nearObject != null)
                            {
                                nearObject.GetComponent<Character>().aiTargetMapBlock = aiManager.FindNearTile(nearObject, checkMapBlock.gameObject, warningAIMoveRange); 
                                setAbleTarget.Remove(nearObject);
                                CanSetTargetMapBlockCharacter.Remove(nearObject);
                            }
                            else
                            { 
                                break;    
                            }
                            characterGap--;
                        }
                        #endregion
                    }
                    else if (enemyCharacterCount > playerCharacterCount)
                    {
                        // AI�� �� ���� ���
                        #region                       
                        int characterGap = enemyCharacterCount - playerCharacterCount;

                        while (characterGap > 0)
                        {                          
                            if (targerToStrategicPointNear.Count < 0)
                            {
                                break;
                            }

                            List<GameObject> checkObject = new List<GameObject>();
                            for (int j = 0; j < targerToStrategicPointNear.Count; j++)
                            {                                
                                checkObject.Add(targerToStrategicPointNear[j].gameObject);
                            }

                            GameObject farObject = aiManager.GetFarObject(checkObject, checkMapBlock.gameObject, true);
                            if (farObject == null)
                            {
                                break;
                            }

                            if((farObject.GetComponent<Character>().aiTargetMapBlock != null) && (warningMapBlock.Contains(farObject.GetComponent<Character>().aiTargetMapBlock) == true))
                            { 
                                farObject.GetComponent<Character>().aiTargetMapBlock = null;                                
                                CanSetTargetMapBlockCharacter.Add(farObject);
                            }
                            targerToStrategicPointNear.Remove(farObject.GetComponent<Character>());
                            characterGap--;
                        }
                        #endregion
                    }
                    else //���� ������ �������ΰ�� ������� �˻�
                    {
                        playerCharacterCount = 0;
                        enemyCharacterCount = 0;
                        warningMapBlock = new List<MapBlock>();
                        checkEndCharacter = new List<GameObject>();
                        targerToStrategicPointNear = new List<Character>();

                        // ��� �˻�
                        #region
                        warningMapBlock = aiManager.CheckTargetNearTile(carefulStrategicPointRange, checkMapBlock.gameObject);
                        for (int j = 0; j < aiManager.battleManager.enemyCharacters.Length; j++) //��ǥȮ��
                        {
                            if (aiManager.battleManager.enemyCharacters[j].GetComponent<Character>().aiTargetMapBlock != null)
                            {
                                if (warningMapBlock.Contains(aiManager.battleManager.enemyCharacters[j].GetComponent<Character>().aiTargetMapBlock) == true)
                                {
                                    enemyCharacterCount++;
                                    checkEndCharacter.Add(aiManager.battleManager.enemyCharacters[j]);
                                    targerToStrategicPointNear.Add(aiManager.battleManager.enemyCharacters[j].GetComponent<Character>());
                                }
                            }
                        }

                        for (int j = 0; j < warningMapBlock.Count; j++) //������ ��ġ�ϴ��� Ȯ��
                        {
                            RaycastHit2D[] hits;
                            hits = Physics2D.RaycastAll(new Vector2(warningMapBlock[j].gameObject.transform.position.x, warningMapBlock[j].gameObject.transform.position.y), Vector2.zero);

                            for (int k = 0; k < hits.Length; k++)
                            {
                                if ((hits[k].transform.GetComponent<Character>() == true) && (hits[k].transform.tag == "Character"))
                                {
                                    if (checkEndCharacter.Contains(hits[k].transform.gameObject) == false)
                                    {
                                        if (hits[k].transform.GetComponent<Character>().isEnemy == false)
                                        {
                                            playerCharacterCount++;
                                        }
                                        else
                                        {
                                            enemyCharacterCount++;
                                            targerToStrategicPointNear.Add(hits[k].transform.GetComponent<Character>());
                                        }
                                        checkEndCharacter.Add(hits[k].transform.gameObject);
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion                   

                        //��� ������ �ִ� ĳ���� ���� �˻�
                        if (playerCharacterCount > enemyCharacterCount) 
                        {
                            //�÷��̾ �� ���� ���
                            #region
                            int characterGap = playerCharacterCount - enemyCharacterCount;
                            List <GameObject> setAbleTarget = new List<GameObject>();
                            for (int j = 0; j < CanSetTargetMapBlockCharacter.Count; j++)
                            {
                                setAbleTarget.Add(CanSetTargetMapBlockCharacter[j]);
                            }

                            while (characterGap > 0)
                            {
                                for (int j = 0; j < targerToStrategicPointNear.Count; j++)
                                {
                                    if (setAbleTarget.Contains(targerToStrategicPointNear[j].gameObject) == true)
                                    {    
                                        setAbleTarget.Remove(targerToStrategicPointNear[j].gameObject);
                                    }
                                }

                                if (setAbleTarget.Count < 1)
                                {
                                    break;
                                }
                            
                                GameObject nearObject = aiManager.GetNearObject(setAbleTarget, checkMapBlock.gameObject, true);
                                if (nearObject != null)
                                {
                                    nearObject.GetComponent<Character>().aiTargetMapBlock = aiManager.FindNearTile(nearObject, checkMapBlock.gameObject, carefulAIMoveRange); 
                                    setAbleTarget.Remove(nearObject);
                                    //CanSetTargetMapBlockCharacter.Remove(nearObject); ���� ����
                                }
                                else
                                { 
                                    break;    
                                }
                                characterGap--;
                            }
                            #endregion
                        }
                        else if (enemyCharacterCount > playerCharacterCount)
                        {
                            // AI�� �� ���� ���
                            #region                       
                            int characterGap = enemyCharacterCount - playerCharacterCount;

                            while (characterGap > 0)
                            {                          
                                if (targerToStrategicPointNear.Count < 0)
                                {
                                    break;
                                }

                                List<GameObject> checkObject = new List<GameObject>();
                                for (int j = 0; j < targerToStrategicPointNear.Count; j++)
                                {                                
                                    checkObject.Add(targerToStrategicPointNear[j].gameObject);
                                }

                                GameObject farObject = aiManager.GetFarObject(checkObject, checkMapBlock.gameObject, true);
                                if (farObject == null)
                                {
                                    break;
                                }

                                if((farObject.GetComponent<Character>().aiTargetMapBlock != null) && (warningMapBlock.Contains(farObject.GetComponent<Character>().aiTargetMapBlock) == true))
                                { 
                                    farObject.GetComponent<Character>().aiTargetMapBlock = null;                                
                                    CanSetTargetMapBlockCharacter.Add(farObject);
                                }
                                targerToStrategicPointNear.Remove(farObject.GetComponent<Character>());
                                characterGap--;
                            }
                            #endregion
                        }
                    }
                }                   
            }
            #endregion
        }
        else //������ ������� ������
        {
            int expectPlayerStrategicPoint = 0;
            int expectEnemyStrategicPoint = 0;
            List<MapBlock> aiCanSetTargetStrategicPoint = new List<MapBlock>();
            for (int i = 0; i < aiManager.battleManager.strategicPointMapBlocks.Count; i++)
            {
                #region
                MapBlock checkMapBlock = aiManager.battleManager.strategicPointMapBlocks[i];
                RaycastHit2D[] hits;
                hits = Physics2D.RaycastAll(new Vector2(checkMapBlock.gameObject.transform.position.x, checkMapBlock.gameObject.transform.position.y), Vector2.zero);

                bool characterChecker = false;
                for (int j = 0; j < hits.Length; j++)
                {
                    if ((hits[j].transform.GetComponent<Character>() == true) && (hits[j].transform.tag == "Character"))
                    {
                        characterChecker = true;
                        if (hits[j].transform.GetComponent<Character>().isEnemy == false)
                        {
                            expectPlayerStrategicPoint++;
                        }
                        else
                        {
                            expectEnemyStrategicPoint++;
                        }
                        characterChecker = true;
                        break;
                    }
                }

                if (characterChecker == false)
                {
                    if (checkMapBlock.tileOwner == TileOwner.Enemy)
                    {
                        expectEnemyStrategicPoint++;
                    }
                    else if (checkMapBlock.tileOwner == TileOwner.Player)
                    {
                        expectPlayerStrategicPoint++;
                    }

                    if (checkMapBlock.tileOwner != TileOwner.Enemy)
                    {
                        aiCanSetTargetStrategicPoint.Add(checkMapBlock);
                    }                   
                }
                #endregion
            }

            if ((aiCanSetTargetStrategicPoint.Count > 0) && (CanSetTargetMapBlockCharacter.Count > 0)) //�� �� �ִ� ������ ������
            {
                int ableToMoveLengthDif = 6; //�÷��̾� ĳ���ͺ��� Ÿ�Ͽ� �󸶳� �ڿ� �ִ��� üũ�ؼ� �����ϴ°�
                #region
                for (int j = 0; j < aiCanSetTargetStrategicPoint.Count; j++)
                {
                    bool alreadyTargetCheck = false;

                    for (int i = 0; i < aiManager.battleManager.enemyCharacters.Length; i++)
                    {
                        if (aiManager.battleManager.enemyCharacters[i].GetComponent<Character>().aiTargetMapBlock == aiCanSetTargetStrategicPoint[j]) //���� ������ �������� ���� �ִ��� üũ
                        {
                            alreadyTargetCheck = true;
                            break;
                        }
                    }

                    if (alreadyTargetCheck == true)
                    {
                        continue;
                    }

                    GameObject nearObject = aiManager.GetNearObject(CanSetTargetMapBlockCharacter, aiCanSetTargetStrategicPoint[j].gameObject, false); //�� �� �ִ� ��(�������� ĳ���Ͱ� ���°��)�� �����ϴ� true�� ĳ���� ����ó�� ���ص� ��
                    if (nearObject != null)
                    {
                        List<GameObject> checkPlayerCharacter = new List<GameObject>();

                        for (int i = 0; i < aiManager.battleManager.playerCharacters.Length; i++)
                        {
                            checkPlayerCharacter.Add(aiManager.battleManager.playerCharacters[i]);
                        }

                        GameObject nearPlayerCharacter = aiManager.GetNearObject(checkPlayerCharacter, aiCanSetTargetStrategicPoint[j].gameObject, false);
                        if (nearPlayerCharacter == null)
                        {
                            nearObject.GetComponent<Character>().aiTargetMapBlock = aiCanSetTargetStrategicPoint[j];
                            CanSetTargetMapBlockCharacter.Remove(nearObject);
                            continue;
                        }
                        int aiRoute = aiManager.GetRouteLength(nearObject, aiCanSetTargetStrategicPoint[j].gameObject, false);
                        int playerRoute = aiManager.GetRouteLength(nearPlayerCharacter, aiCanSetTargetStrategicPoint[j].gameObject, false);

                        if (aiRoute <= playerRoute + ableToMoveLengthDif)
                        {
                            nearObject.GetComponent<Character>().aiTargetMapBlock = aiCanSetTargetStrategicPoint[j];
                            CanSetTargetMapBlockCharacter.Remove(nearObject);
                        }
                    }
                }
                #endregion
            }
        }
        #endregion
    }

    public override void AICharacterMove() //AI�� �̵��� �� �Ǵ�
    {
        #region        
        GameObject targetCharacter = aiManager.battleManager.targetCharacter;
        aiManager.battleManager.battleCharacterManager.GetAICharacterMoveTile(ref targetCharacter);

        if ((aiManager.battleManager.battleCharacterManager.actionAbleTile.Count < 1) || targetCharacter.GetComponent<Character>().aiMoveInPlace == true) //�̵� ���� �� ���� ���ų� ���ڸ� �̵��� ���� ������ ���ڸ� �̵�
        {
            #region
            Debug.Log(targetCharacter + "�� ���ڸ� �̵��� ��");
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(new Vector2(targetCharacter.transform.position.x, targetCharacter.transform.position.y), Vector2.zero);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.GetComponent<MapBlock>() == true)
                {
                    targetCharacter.GetComponent<Character>().AIMovePositionByAnimation(hits[i].transform.GetComponent<MapBlock>(), ref aiManager.battleManager);
                    return;
                }
            }            
            #endregion
        }


        if ((targetCharacter.GetComponent<Character>().aiTargetMapBlock != null)) //��ǥ�� �ִٸ� ��ǥ�� ���� �̵�
        {
            
            //��ǥ�� �� �濡 �� �� �ִ� ��� ��ǥ�� �̵�
            #region
            if (CheckAICanMoveToTargetOnce(targetCharacter) == true)
            {
                Debug.Log("��ǥ�� �̵�!");
                return;
            }
            #endregion

            //��ǥ�� ������ �ƴ� �� �̹� �� ������ �� �濡 �� �� �ְ� ���� ���� ��� ��� ������ �̵�
            #region
            if ((targetCharacter.GetComponent<Character>().aiTargetMapBlock.tileType != TileType.StrategicPoint) && (targetCharacter.GetComponent<Character>().GetIsHaveBall() == false))
            {
                MapBlock checkMapBlock = CheckBallOnCharacterMoveRange(targetCharacter);
                if (checkMapBlock != null)
                {
                    targetCharacter.GetComponent<Character>().aiTargetMapBlock = null;
                    Debug.LogWarning("��ǥ�� �ʱ�ȭ �� ������ �̵�!");
                    AICharacterMove(targetCharacter, checkMapBlock);
                    return;
                }
            }
            #endregion

            //��ǥ�� ������ �ƴ� �� �̹� �� AI�� �������� ���� �������� �� �濡 �� �� �ִ� ��� �������� �̵�
            #region
            if (targetCharacter.GetComponent<Character>().aiTargetMapBlock.tileType != TileType.StrategicPoint)
            {
                MapBlock checkMapBlock = CheckStrategicPointOnCharacterMoveRange(targetCharacter);
                if ((checkMapBlock != null) && (checkMapBlock.tileOwner != TileOwner.Enemy))
                {
                    for (int i = 0; i < aiManager.ActionAbleEnemyCharacter.Count; i++)
                    {
                        int TargetSPCount = 0;
                        if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().aiTargetMapBlock == checkMapBlock) //�ٸ� AI�� �ش� �ϴ� ������ �븮�� �ִ� �� ���θ� üũ
                        {
                            TargetSPCount++;
                            break;
                        }

                        if (TargetSPCount < 1)
                        {
                            targetCharacter.GetComponent<Character>().aiTargetMapBlock = null;
                            Debug.LogWarning("��ǥ�� �ʱ�ȭ �� �������� �̵�!");
                            AICharacterMove(targetCharacter, checkMapBlock);
                            return;
                        }
                    }
                }
            }        
            #endregion

            //��ǥ�� �� �濡 ������ ��� ��ǥ�� ���� �̵�
            #region
            if (MoveToTarget(targetCharacter) == true)
            {
                Debug.Log("��ǥ�� ���Ͽ� �̵�!");
                return;
            }  
            #endregion
        }

        //������Ͱ� ��ǥ�� ���ų� ��ǥ�� ���� ���ϴ� ���   
        targetCharacter.GetComponent<Character>().aiTargetMapBlock = null; //��ǥ�� �����ų� ��ǥ�� ���� ���� ��ǥ �ʱ�ȭ
        MapBlock targetMapBlock = null;

        // �� �濡 �� �� �ִ� ������ �ִ� �� Ȯ��
        #region
        targetMapBlock = CheckStrategicPointOnCharacterMoveRange(targetCharacter);
        if (targetMapBlock != null)
        {
            AICharacterMove(targetCharacter, targetMapBlock);
            return;
        }
 
        #endregion
        // ���� ������, �� �濡 ������ �� �� �ִ� �� Ȯ��
        #region 
        if (targetCharacter.GetComponent<Character>().GetIsHaveBall() == false)
        {
            targetMapBlock = CheckBallOnCharacterMoveRange(targetCharacter);
            if (targetMapBlock != null)
            {
                AICharacterMove(targetCharacter, targetMapBlock);
                return;
            }
        }      
        #endregion
        //���� ������ �� �濡 ���� �ϰ� ��ȣ�ۿ� �������� üũ (���� �켱)
        #region
        if (targetCharacter.GetComponent<Character>().GetIsHaveBall() == true)
        {
            for (int j = 0; j < aiManager.battleManager.playerCharacters.Length; j++)
            {
                List <MapBlock> checkMapBlock = new List <MapBlock>();
                checkMapBlock = aiManager.CheckCharacterCanInteractableMapBlock(targetCharacter.GetComponent<Character>(), aiManager.battleManager.playerCharacters[j].GetComponent<Character>());
                if (checkMapBlock.Count > 0)
                {
                    AICharacterMove(targetCharacter, GetBestTile(targetCharacter.GetComponent<Character>(), checkMapBlock));
                    return;
                }
            } 
        }        
        #endregion
        //���� ������ �� �濡 �ൿ ������ �Ʊ� �ϰ� ��ȣ �ۿ� �������� üũ (�н���, ���� �켱)
        #region
        if (targetCharacter.GetComponent<Character>().GetIsHaveBall() == true)
        {
            for (int j = 0; j < aiManager.ActionAbleEnemyCharacter.Count; j++)
            {
                if ((targetCharacter != aiManager.ActionAbleEnemyCharacter[j]) && (aiManager.ActionAbleEnemyCharacter[j].GetComponent<Character>().GetIsHaveBall() == false))
                {
                    List <MapBlock> checkMapBlock = new List <MapBlock>();
                    checkMapBlock = aiManager.CheckCharacterCanInteractableMapBlock(targetCharacter.GetComponent<Character>(), aiManager.ActionAbleEnemyCharacter[j].GetComponent<Character>());
                    if (checkMapBlock.Count > 0)
                    {
                        AICharacterMove(targetCharacter, GetBestTile(targetCharacter.GetComponent<Character>(), checkMapBlock));
                        return;
                    }
                }                    
            }
        }

        #endregion
        //���� ������ ���� �ִ� ��뿡�� ��ȣ�ۿ��� �������� üũ (���� �켱)
        #region
        if (targetCharacter.GetComponent<Character>().GetIsHaveBall() == true)
        {
            for (int j = 0; j < aiManager.battleManager.playerCharacters.Length; j++)
            {
                if (aiManager.battleManager.playerCharacters[j].GetComponent<Character>().GetIsHaveBall() == true)
                {
                    List <MapBlock> checkMapBlock = new List <MapBlock>();
                    checkMapBlock = aiManager.CheckCharacterCanInteractableMapBlock(targetCharacter.GetComponent<Character>(), aiManager.battleManager.playerCharacters[j].GetComponent<Character>());
                    if (checkMapBlock.Count > 0)
                    {
                        AICharacterMove(targetCharacter, GetBestTile(targetCharacter.GetComponent<Character>(), checkMapBlock));
                        return;
                    }
                }
            }
        }
        #endregion

        //1�� �ȿ� �� �� �ִ� �ൿ�� ���� ���
        //���� ������ ����� ��볪 �ൿ�� ���� �Ʊ��� ���� �̵�(�Ʊ��ʿ� ������ �ɸ�)
        #region
        if (targetCharacter.GetComponent<Character>().GetIsHaveBall() == true)
        {
            int AICorrection = 4;
            int findRange = targetCharacter.GetComponent<Character>().status.range;
            List<GameObject> aiCharacterList = new List<GameObject>();
            List<GameObject> playerCharacterList = new List<GameObject>();
            for (int i = 0; i < aiManager.battleManager.enemyCharacters.Length; i++)
            {
                if ((aiManager.battleManager.enemyCharacters[i] != targetCharacter) && (aiManager.battleManager.enemyCharacters[i].GetComponent<Character>().GetIsHaveBall() == false))
                {
                    GameObject checkObject = null;
                    checkObject = aiManager.FindNearTile(targetCharacter, aiManager.battleManager.enemyCharacters[i], findRange).gameObject;
                    if (checkObject != null)
                    {
                        aiCharacterList.Add(checkObject);
                    }
                }
            }

            for (int i = 0; i < aiManager.battleManager.playerCharacters.Length; i++)
            {
                GameObject checkObject = null;
                checkObject = aiManager.FindNearTile(targetCharacter, aiManager.battleManager.playerCharacters[i], findRange).gameObject;
                if (checkObject != null)
                {
                    playerCharacterList.Add(checkObject);
                }
            }

            GameObject nearAI = aiManager.GetNearObject(aiCharacterList, targetCharacter, false); //Ÿ�� ������� false��
            GameObject nearPlayer = aiManager.GetNearObject(playerCharacterList, targetCharacter, false); //Ÿ�� ������� false��

            int nearAIChecker = aiManager.GetRouteLength(targetCharacter, nearAI, false);
            int nearPlayerChecker = aiManager.GetRouteLength(targetCharacter, nearPlayer, false);

            if (nearAIChecker == -1 && nearPlayerChecker == -1)
            {

            }
            else if (nearAIChecker == -1)
            {
                AICharacterMove(targetCharacter, GetBestMoveableTileToTarget(targetCharacter, nearPlayer));
                return;
            }
            else if (nearPlayerChecker == -1)
            {
                AICharacterMove(targetCharacter, GetBestMoveableTileToTarget(targetCharacter, nearAI));
                return;
            }
            else
            {
                if (nearAIChecker <= nearPlayerChecker + AICorrection)
                {
                    AICharacterMove(targetCharacter, GetBestMoveableTileToTarget(targetCharacter, nearAI));
                    return;
                }
                else
                {
                    AICharacterMove(targetCharacter, GetBestMoveableTileToTarget(targetCharacter, nearPlayer));
                    return;
                }
            }
        }
        #endregion
        //���� ������ �ʵ忡 ������ �ִ� ���̳� ���� ���� ���� ���� ���� �̵�(�ʵ忡 ������ ���� �� �� �Ÿ��� ���� ���� ����)
        #region
        if (targetCharacter.GetComponent<Character>().GetIsHaveBall() == false)
        {
            int AICorrection = 4;
            int findRange = targetCharacter.GetComponent<Character>().status.range;
            List<GameObject> fieldBallList = new List<GameObject>();
            List<GameObject> playerCharacterList = new List<GameObject>();
            
            for (int i = 0; i < aiManager.BallOnField.Count; i++)
            {
                fieldBallList.Add(aiManager.BallOnField[i].gameObject);
            }
            
            for (int i = 0; i < aiManager.battleManager.playerCharacters.Length; i++)
            {
                if (aiManager.battleManager.playerCharacters[i].GetComponent<Character>().GetIsHaveBall() == true)
                {
                    GameObject checkObject = null;
                    checkObject = aiManager.FindNearTile(targetCharacter, aiManager.battleManager.playerCharacters[i], findRange).gameObject;
                    if (checkObject != null)
                    {
                        playerCharacterList.Add(checkObject);
                    }
                }
            }
            
            GameObject nearFieldBall = aiManager.GetNearObject(fieldBallList, targetCharacter, false); //Ÿ���̴� false��
            GameObject nearHaveBallPlayer = aiManager.GetNearObject(playerCharacterList, targetCharacter, false); //Ÿ���̴� false��
            
            int nearFieldChecker = aiManager.GetRouteLength(targetCharacter, nearFieldBall, false); 
            int nearPlayerChecker = aiManager.GetRouteLength(targetCharacter, nearHaveBallPlayer, false); 

            if (nearFieldChecker == -1 && nearPlayerChecker == -1)
            {

            }
            else if (nearFieldChecker == -1)
            {
                AICharacterMove(targetCharacter, GetBestMoveableTileToTarget(targetCharacter, nearHaveBallPlayer));
                return;
            }
            else if (nearPlayerChecker == -1)
            {
                AICharacterMove(targetCharacter, GetBestMoveableTileToTarget(targetCharacter, nearFieldBall));
                return;
            }
            else
            {
                if (nearFieldChecker <= nearPlayerChecker + AICorrection)
                {
                    AICharacterMove(targetCharacter, GetBestMoveableTileToTarget(targetCharacter, nearFieldBall));
                    return;
                }
                else
                {
                    AICharacterMove(targetCharacter, GetBestMoveableTileToTarget(targetCharacter, nearHaveBallPlayer));
                    return;
                }
            }
        }
        #endregion
        //��� ��찡 �ش� �ȵ� �� �ϴ� �̵�, �̵� ���� Ÿ���� ���� �켱������ ���� Ÿ�Ϸ� �̵�
        Debug.LogWarning(targetCharacter + "�� ��� �̵� ������ �������� ����");
        AICharacterMove(targetCharacter, null);
        #endregion
    }

    private void AICharacterMove(GameObject targetCharacter, MapBlock targetMapBlock)
    {
        #region
        aiManager.battleManager.battleCharacterManager.GetAICharacterMoveTile(ref targetCharacter);

        if ((targetMapBlock != null) && (aiManager.battleManager.battleCharacterManager.actionAbleTile.Contains(targetMapBlock.gameObject) == false))
        {
            Debug.LogError("�̹��� �̵���ǥ�� ���� �� ������ �̵� �� �� �����ϴ�");
            targetMapBlock = null;
        }

        if (targetMapBlock == null) //targetMapBlock�� ������ �ּ��� ���� ���� �̵�
        {
            targetMapBlock = GetBestTile(targetCharacter.GetComponent<Character>(), GameObjectListToMapBlockList(aiManager.battleManager.battleCharacterManager.actionAbleTile));
            if (targetMapBlock == null)
            {
                targetMapBlock = aiManager.battleManager.battleCharacterManager.actionAbleTile[Random.Range(0, aiManager.battleManager.battleCharacterManager.actionAbleTile.Count)].GetComponent<MapBlock>();
            }
        }

        targetCharacter.GetComponent<Character>().AIMovePositionByAnimation(targetMapBlock, ref aiManager.battleManager);
        #endregion
    }

    private List<MapBlock> GameObjectListToMapBlockList(List<GameObject> tileList)
    {
        #region
        List<MapBlock> newTileList = new List<MapBlock>();

        for(int i = 0; i < tileList.Count; i++)
        { 
            if (tileList[i].GetComponent<MapBlock>() == true)
            {
                newTileList.Add(tileList[i].GetComponent<MapBlock>());
            }
        }

        return newTileList;
        #endregion
    }

    public override void AICharacterCommand()
    {
        #region
        GameObject targetCharacter = aiManager.battleManager.targetCharacter;
        GameObject skillTarget = null;
        Skill useSkill = null;
        float warningHPMP = 0.2f;
        aiManager.battleManager.battleCharacterManager.AICharacterInteractRange(targetCharacter.GetComponent<Character>().status.range, targetCharacter);
        aiManager.SetAICharacterSkill(); //��밡���� ��ų �޾ƿ���
        //��ų �з�
        #region
        List<Skill> AttackSkill = new List<Skill>();
        List<Skill> NoneBallAttackSkill = new List<Skill>();
        List<Skill> PassSkill = new List<Skill>();
        List<Skill> InterruptBallSkill = new List<Skill>();
        List<Skill> HealSkill = new List<Skill>();
        List<Skill> BuffSkill = new List<Skill>();
        List<Skill> DebuffSkill = new List<Skill>();
        List<Skill> DispelSkill = new List<Skill>();
        List<Skill> ManaHealSkill = new List<Skill>();
        List<Skill> BreakManaSkill = new List<Skill>();
        List<Skill> SympathyIncreaseSkill = new List<Skill>();
        List<Skill> SympathyDecreaseSkill = new List<Skill>();

        for (int i = 0; i < aiManager.aiCharacterSkill.Count; i++)
        {
            SkillDivision[] checkDivision = SkillDatabaseManager.instance.CheckSkillDivision(aiManager.aiCharacterSkill[i]);
            for (int j = 0; j < checkDivision.Length; j++)
            {
                switch (checkDivision[j])
                {
                    case SkillDivision.Attack:
                        AttackSkill.Add(aiManager.aiCharacterSkill[i]);
                        break;
                    case SkillDivision.NoneBallAttack:
                        NoneBallAttackSkill.Add(aiManager.aiCharacterSkill[i]);
                        break;
                    case SkillDivision.Pass:
                        PassSkill.Add(aiManager.aiCharacterSkill[i]);
                        break;
                    case SkillDivision.InterruptBall:
                        InterruptBallSkill.Add(aiManager.aiCharacterSkill[i]);
                        break;
                    case SkillDivision.Heal:
                        HealSkill.Add(aiManager.aiCharacterSkill[i]);
                        break;
                    case SkillDivision.Buff:
                        BuffSkill.Add(aiManager.aiCharacterSkill[i]);
                        break;
                    case SkillDivision.Debuff:
                        DebuffSkill.Add(aiManager.aiCharacterSkill[i]);
                        break;
                    case SkillDivision.Dispel:
                        DispelSkill.Add(aiManager.aiCharacterSkill[i]);
                        break;
                    case SkillDivision.ManaHeal:
                        ManaHealSkill.Add(aiManager.aiCharacterSkill[i]);
                        break;
                    case SkillDivision.BreakMana:
                        BreakManaSkill.Add(aiManager.aiCharacterSkill[i]);
                        break;
                    case SkillDivision.SympathyIncrease:
                        SympathyIncreaseSkill.Add(aiManager.aiCharacterSkill[i]);
                        break;
                    case SkillDivision.SympathyDecrease:
                        SympathyDecreaseSkill.Add(aiManager.aiCharacterSkill[i]);
                        break;
                    default:
                        break;
                }

            }
        }
        #endregion
        //���� �ִ� ���
        if (targetCharacter.GetComponent<Character>().GetIsHaveBall() == true)
        {
            //���� �� �� �ִ� ��밡 �ִ� ���(AttackSkill)
            #region
            if ((aiManager.AttackableTarget.Count > 0) && (AttackSkill.Count > 0))
            {
                List<Character> playerList = new List<Character>();
                for (int i = 0; i < aiManager.AttackableTarget.Count; i++)
                {
                    playerList.Add(aiManager.AttackableTarget[i].GetComponent<Character>());
                }
                skillTarget = GetHPLowestCharacter(playerList).gameObject;
                if (skillTarget != null)
                {
                    useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), AttackSkill));
                    if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
                    {
                        return;
                    }
                }
                skillTarget = null;
                useSkill = null;
            }
            #endregion
            //�н� �� �� �ִ� �ൿ ���� �Ʊ��� �������(PassSkill)
            #region
            if ((aiManager.InteractableTarget.Count > 0) && (PassSkill.Count > 0))
            {
                for (int i = 0; i < aiManager.InteractableTarget.Count; i++)
                {
                    if (aiManager.InteractableTarget[i].GetComponent<Character>().GetIsHaveBall() == false && aiManager.InteractableTarget[i].GetComponent<Character>().GetIsTurnEnd() == false)
                    {
                        skillTarget = aiManager.InteractableTarget[i];
                        useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), PassSkill));
                        if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
                        {
                            return;
                        }
                    }
                    skillTarget = null;
                    useSkill = null;
                }
            }
            #endregion
            //�н� �� �� �ְ� �ൿ�� ���������� ������ ������ ���� �Ʊ��� �ִ� ���(PassSkill)
            #region
            if ((aiManager.InteractableTarget.Count > 0) && (PassSkill.Count > 0))
            {
                for (int i = 0; i < aiManager.InteractableTarget.Count; i++)
                {
                    if ((aiManager.InteractableTarget[i].GetComponent<Character>().GetIsHaveBall() == false) && (aiManager.InteractableTarget[i].GetComponent<Character>().status.DEF > targetCharacter.GetComponent<Character>().status.DEF))
                    {
                        skillTarget = aiManager.InteractableTarget[i];
                        useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), PassSkill));
                        if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
                        {
                            return;
                        }
                    }
                    skillTarget = null;
                    useSkill = null;
                }
            }
            #endregion
        }

        //���� ���ų� ���� ����� �� ���� ���
        //�ֺ��� ���� ���� �÷��̾� ĳ���Ͱ� �ְ� �ڽ��� ���ݷ°� ����� ���� ���̰� ���������� ��(������, InterruptBallSkill)
        #region
        if ((aiManager.AttackableTarget.Count > 0) && (InterruptBallSkill.Count > 0))
        {
            int alpha = Random.Range(5,11);
            for (int i = 0; i < aiManager.AttackableTarget.Count; i++)
            {
                if ((aiManager.AttackableTarget[i].GetComponent<Character>().GetIsHaveBall() == true) && (aiManager.AttackableTarget[i].GetComponent<Character>().status.DEF <= (targetCharacter.GetComponent<Character>().status.ATK + alpha)))
                {
                    skillTarget = aiManager.AttackableTarget[i];
                    useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), InterruptBallSkill));
                    if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
                    {
                        return;
                    }

                    skillTarget = null;
                    useSkill = null;
                }
            }
        }
        #endregion
        //�ڽ��� ü���� ���� ������ ��(HealSkill)
        #region
        if ((targetCharacter.GetComponent<Character>().status.HP <= (int)((float)targetCharacter.GetComponent<Character>().status.maxHP * warningHPMP)) && (HealSkill.Count > 0))
        {
            skillTarget = targetCharacter;
            useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), HealSkill));
            if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
            {
                return;
            }
            skillTarget = null;
            useSkill = null;
        }
        #endregion
        //���� ���� �Ʊ��� ü���� ���� ���� �� ��(HealSkill)
        #region
        if ((aiManager.InteractableTarget.Count > 0) && (HealSkill.Count > 0))
        {
            for (int i = 0; i < aiManager.InteractableTarget.Count; i++)
            {
                if (aiManager.InteractableTarget[i].GetComponent<Character>().status.HP <= (int)((float)aiManager.InteractableTarget[i].GetComponent<Character>().status.maxHP * warningHPMP))
                {
                    skillTarget = aiManager.InteractableTarget[i];
                    useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), HealSkill));
                    if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
                    {
                        return;
                    }
                }
                skillTarget = null;
                useSkill = null;
            }
        }
        #endregion
        //���� ���� ������ �ְ� ���� �ʿ� ���� ���ݷ� ��ų�� ��� ���� �� ��(NoneBallAttackSkill)
        #region
        if ((aiManager.AttackableTarget.Count > 0) && (NoneBallAttackSkill.Count > 0))
        {
            List<Character> playerList = new List<Character>();
            for (int i = 0; i < aiManager.AttackableTarget.Count; i++)
            {
                playerList.Add(aiManager.AttackableTarget[i].GetComponent<Character>());
            }
            skillTarget = GetHPLowestCharacter(playerList).gameObject;
            if (skillTarget != null)
            {
                useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), NoneBallAttackSkill));
                if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
                {
                    return;
                }
            }
            skillTarget = null;
            useSkill = null;
        }
        #endregion
        //���� ���� ������ �ְ� ����� ��ų�� ��� ������ ��(DebuffSkill)
        #region
        if ((aiManager.AttackableTarget.Count > 0) && (DebuffSkill.Count > 0))
        {
            skillTarget = aiManager.AttackableTarget[Random.Range(0, aiManager.AttackableTarget.Count)];
            if (skillTarget != null)
            {
                useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), DebuffSkill));
                if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
                {
                    return;
                }
            }
            skillTarget = null;
            useSkill = null;
        }
        #endregion
        //�ڽſ��� ���� ��ų�� ��밡�� �� ��(BuffSkill)
        #region
        if (BuffSkill.Count > 0)
        {
            skillTarget = targetCharacter;
            useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), BuffSkill));
            if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
            {
                return;
            }
            skillTarget = null;
            useSkill = null;
        }        
        #endregion
        //���� ���� �Ʊ��� �ְ� ������ų�� ��� ������ ��(BuffSkill)
        #region
        if ((aiManager.InteractableTarget.Count > 0) && (BuffSkill.Count > 0))
        {
            skillTarget = aiManager.InteractableTarget[Random.Range(0, aiManager.InteractableTarget.Count)];
            useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), BuffSkill));
            if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
            {
                return;
            }

            skillTarget = null;
            useSkill = null;
        }
        #endregion
        //���� ������ ���� �̻� �� ��(BreakManaSkill)
        #region
        if ((aiManager.AttackableTarget.Count > 0) && (BreakManaSkill.Count > 0))
        {
            for (int i = 0; i < aiManager.AttackableTarget.Count; i++)
            {
                if (aiManager.AttackableTarget[i].GetComponent<Character>().status.MP >= (int)((float)aiManager.AttackableTarget[i].GetComponent<Character>().status.maxMP * (1.0 - warningHPMP)))
                {
                    skillTarget = aiManager.AttackableTarget[i];
                    useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), BreakManaSkill));
                    if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
                    {
                        return;
                    }
                }
                skillTarget = null;
                useSkill = null;
            }
        }
        #endregion
        //���� ������ ���� �̻� �� ��(SympathyDecreaseSkill)(���߿� ���� ��ȭ �ʿ�)
        #region
        if ((aiManager.AttackableTarget.Count > 0) && (SympathyDecreaseSkill.Count > 0))
        {
            for (int i = 0; i < aiManager.AttackableTarget.Count; i++)
            {
                if (aiManager.AttackableTarget[i].GetComponent<Character>().status.Sympathy >= 60)
                {
                    skillTarget = aiManager.AttackableTarget[i];
                    useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), SympathyDecreaseSkill));
                    if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
                    {
                        return;
                    }
                }
                skillTarget = null;
                useSkill = null;
            }
        }
        #endregion
        //�ڽ��� ������ ���� ���� �� ��(ManaHealSkill)
        #region
        if ((targetCharacter.GetComponent<Character>().status.MP <= (int)((float)targetCharacter.GetComponent<Character>().status.maxMP * warningHPMP)) && (ManaHealSkill.Count > 0))
        {
            skillTarget = targetCharacter;
            useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), ManaHealSkill));
            if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
            {
                return;
            }
            skillTarget = null;
            useSkill = null;
        }
        #endregion
        //�Ʊ��� ������ ���� ���� �� ��(ManaHealSkill)
        #region
        if ((aiManager.InteractableTarget.Count > 0) && (ManaHealSkill.Count > 0))
        {
            for (int i = 0; i < aiManager.InteractableTarget.Count; i++)
            {
                if (aiManager.InteractableTarget[i].GetComponent<Character>().status.MP <= (int)((float)aiManager.InteractableTarget[i].GetComponent<Character>().status.maxMP * warningHPMP))
                {
                    skillTarget = aiManager.InteractableTarget[i];
                    useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), ManaHealSkill));
                    if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
                    {
                        return;
                    }
                }
                skillTarget = null;
                useSkill = null;
            }
        }
        #endregion
        //�ֺ��� ���� ���� �÷��̾� ĳ���Ͱ� ���� ��(InterruptBallSkill)
        #region
        if ((aiManager.AttackableTarget.Count > 0) && (InterruptBallSkill.Count > 0))
        {
            for (int i = 0; i < aiManager.AttackableTarget.Count; i++)
            {
                if (aiManager.AttackableTarget[i].GetComponent<Character>().GetIsHaveBall() == true)
                {
                    skillTarget = aiManager.AttackableTarget[i];
                    useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), InterruptBallSkill));
                    if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
                    {
                        return;
                    }

                    skillTarget = null;
                    useSkill = null;
                }
            }
        }
        #endregion
        //���� ������ �����Ǹ� �߰�, �ڽ��� ������ ���� �̻��̸� ���� �� �Ʊ��� ������ �������� �϶�(���� �� �� �������� ���� ���� �ϴ°͵� �߰��ؾ���)

        //���� ������ �����Ǹ� �߰�, �ڽ��� ������ ���� �̻��϶� ���� ���ұ�(���� �ʹ� �������� ������ ��ų�� �ֱ⿡ ������, ���� ������ �� ���� ���ǵ� �� ��ä��� ��밡���� ��ų�� ���ٰ� �Ǵ���) //ai�� �����ϴ� ����Ÿ�Ե� ����� �װ� �̻��ϋ� ���߰� �׷��� ��

        //�ڽ� ����������(SympathyIncreaseSkill)
        #region
        if (SympathyIncreaseSkill.Count > 0)
        {
            skillTarget = targetCharacter;
            useSkill = GetBestDamageSkill(targetCharacter.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, GetUseableSkillList(targetCharacter.GetComponent<Character>(), skillTarget.GetComponent<Character>(), SympathyIncreaseSkill));
            if (UseSkill(targetCharacter, skillTarget, useSkill) == true)
            {
                return;
            }
            skillTarget = null;
            useSkill = null;
        }
        #endregion
        //�� �� �ִ°� ���� ��(���)
        aiManager.battleManager.BattleStay();
        #endregion
    }

    private Character GetHPLowestCharacter(List<Character> checkCharacterList)
    {
        #region
        if (checkCharacterList.Count < 1)
        {
            Debug.LogError("GetHPLowestCharacter�� �߸��� ���� ���Խ��ϴ�!");
            return null;
        }

        int lowestHP = checkCharacterList[0].status.HP;
        Character lowestCharacter = checkCharacterList[0];
        for (int i = 1; i < checkCharacterList.Count; i++)
        {
            int checkHP = checkCharacterList[i].status.HP;

            if (checkHP < lowestHP)
            {
                lowestCharacter = checkCharacterList[i];
            }
            else if (checkHP == lowestHP)
            {
                if (checkCharacterList[i].status.DEF < lowestCharacter.status.DEF)
                {
                    lowestCharacter = checkCharacterList[i];
                }
            }
        }
        return lowestCharacter;
        #endregion
    }

    private List<Skill> GetUseableSkillList(Character attacker, Character target, List<Skill> checkSkillList) //����Ʈ���� �ش��ϴ� ��󿡰� ��밡���� ��ų�� �̴� �ڵ�
    {
        #region
        List<Skill> useableSkillList = new List<Skill>();
        if (attacker == null || target == null || checkSkillList.Count < 1)
        {
            Debug.LogWarning("GetUseableSkillList�Լ��� �߸��� ���� ���Խ��ϴ�!");
            return useableSkillList;
        }

        bool isSameTeam = false;
        bool isSelf = false;
        if (attacker == target)
        {
            isSelf = true;
        }
        else if (attacker.isEnemy == target.isEnemy)
        {
            isSameTeam = true;
        }

        for (int i = 0; i < checkSkillList.Count; i++)
        {
            if((checkSkillList[i].ignoreIsHaveBall == false) && (attacker.GetIsHaveBall() == false)) //�⺻ ����� ������簡 ����� ���� ��ƲĿ�ǵ�Ŵ������� �Ÿ��°� ���ؼ� �� ���� ������ ����ó��
            { 
                continue;    
            }

            switch (checkSkillList[i].skillTarget)
            {
                case SkillTarget.Enemy:
                    if((isSameTeam == false) && (isSelf == false))
                    {
                        useableSkillList.Add(checkSkillList[i]);
                    }
                    break;
                case SkillTarget.Friendly:
                    if((isSameTeam == true) && (isSelf == false))
                    {
                        useableSkillList.Add(checkSkillList[i]);
                    }
                    break;
                case SkillTarget.Self:
                    if(isSelf == true)
                    {
                        useableSkillList.Add(checkSkillList[i]);
                    }
                    break;
                case SkillTarget.FriendlyAndSelf:
                    if((isSameTeam == true) || (isSelf == true))
                    {
                        useableSkillList.Add(checkSkillList[i]);
                    }
                    break;
                case SkillTarget.All:
                    useableSkillList.Add(checkSkillList[i]);
                    break;
                default:
                    break;
            }
        }

        return useableSkillList;
        #endregion
    }

    public Skill GetBestDamageSkill(CharacterStatus attackerStatus, CharacterStatus targetStatus, List<Skill> checkSkillList) //���� ������ ����� ���� ��ų�� �̴� �ڵ�
    {
        #region
        if (attackerStatus == null || targetStatus == null || checkSkillList.Count < 1)
        {
            if (checkSkillList.Count < 1)
            {
                Debug.Log("GetBestDamageSkill����Ʈ�� ���°� �����ϴ�!");
                return null;
            }
            Debug.LogWarning("GetBestDamageSkill�Լ��� �߸��� ���� ���Խ��ϴ�!");
            return null;
        }
        List<Skill> sameDamageSkill = new List<Skill>();
        sameDamageSkill.Add(checkSkillList[0]);
        int bestDamage = 0;

        if (checkSkillList[0].skillType == SkillType.Pass)
        {
            #region
            bestDamage = GetSkillHeal(attackerStatus, targetStatus, checkSkillList[0]);

            for (int i = 1; i < checkSkillList.Count; i++)
            {
                int checkDamage = GetSkillHeal(attackerStatus, targetStatus, checkSkillList[i]);
                if (checkDamage == bestDamage)
                {
                    sameDamageSkill.Add(checkSkillList[i]);
                }
                else if (checkDamage > bestDamage)
                {
                    sameDamageSkill.Clear();
                    sameDamageSkill.Add(checkSkillList[i]);
                }
            }
            #endregion
        }
        else
        {
            #region
            bestDamage = GetSkillDamage(attackerStatus, targetStatus, checkSkillList[0]);

            for (int i = 1; i < checkSkillList.Count; i++)
            {
                int checkDamage = GetSkillDamage(attackerStatus, targetStatus, checkSkillList[i]);
                if (checkDamage == bestDamage)
                {
                    sameDamageSkill.Add(checkSkillList[i]);
                }
                else if (checkDamage > bestDamage)
                {
                    sameDamageSkill.Clear();
                    sameDamageSkill.Add(checkSkillList[i]);
                }
            }
            #endregion
        }

        if (sameDamageSkill.Count > 0)
        {
            return sameDamageSkill[Random.Range(0, sameDamageSkill.Count)];
        }

        return null;
        #endregion
    }

    private int GetSkillDamage(CharacterStatus attackerStatus, CharacterStatus targetStatus, Skill skill)
    {
        #region
        if (attackerStatus == null || targetStatus == null || skill == null)
        {
            Debug.LogWarning("GetBestDamageSkill�Լ��� �߸��� ���� ���Խ��ϴ�!");
            return 0;
        }

        float attackPower = skill.DPHATK * attackerStatus.ATK + skill.DPHMAK * attackerStatus.MAK + skill.DPHDEF * attackerStatus.DEF + skill.DPHMDF * attackerStatus.MDF + skill.DPHMaxHP * attackerStatus.maxHP + skill.DPHHP * attackerStatus.HP + skill.DPHMaxMP * attackerStatus.maxMP + skill.DPHMP * attackerStatus.MP;
        float resistPower = skill.enemyATK * targetStatus.ATK + skill.enemyMAK * targetStatus.MAK + skill.enemyDEF * targetStatus.DEF + skill.enemyMDF * targetStatus.MDF + skill.enemyMaxHP * targetStatus.maxHP + skill.enemyHP * targetStatus.HP + skill.enemyMaxMP * targetStatus.maxMP + skill.enemyMP * targetStatus.MP;
        float innocentDamage = attackPower - resistPower;        
        int damage = (int)innocentDamage+ skill.fixedValue;

        return damage;
        #endregion
    }

    private int GetSkillHeal(CharacterStatus attackerStatus, CharacterStatus targetStatus, Skill skill)
    {
        #region
        if (attackerStatus == null || targetStatus == null || skill == null)
        {
            Debug.LogError("GetBestDamageSkill�Լ��� �߸��� ���� ���Խ��ϴ�!");
            return 0;
        }

        float attackPower = skill.DPHATK * attackerStatus.ATK + skill.DPHMAK * attackerStatus.MAK + skill.DPHDEF * attackerStatus.DEF + skill.DPHMDF * attackerStatus.MDF + skill.DPHMaxHP * attackerStatus.maxHP + skill.DPHHP * attackerStatus.HP + skill.DPHMaxMP * attackerStatus.maxMP + skill.DPHMP * attackerStatus.MP;
        float resistPower = skill.enemyATK * targetStatus.ATK + skill.enemyMAK * targetStatus.MAK + skill.enemyDEF * targetStatus.DEF + skill.enemyMDF * targetStatus.MDF + skill.enemyMaxHP * targetStatus.maxHP + skill.enemyHP * targetStatus.HP + skill.enemyMaxMP * targetStatus.maxMP + skill.enemyMP * targetStatus.MP;
        float innocentDamage = attackPower + resistPower; 
        
        int heal = (int)innocentDamage + skill.fixedValue;

        return heal;
        #endregion
    }

}
