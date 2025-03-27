using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAILogic : AILogic
{
    public override void SetAICharacter() //행동할 캐릭터를 선택하는 부분
    {
        SelectAICharacter();
        aiManager.battleManager.battlePhaseManager.ChangePhase(BattlePhase.EnemyTurn_Moving);
    }

    private void SelectAICharacter() //해당 조건이 충족되는 캐릭터를 먼저 움직이는 코드
    {
        CheckStrategicPoint(); //거점을 체크하여 목표로 잡는 코드
        //목표가 있는 캐릭터 먼저 목표로 이동
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
        // 공이 없으며, 한 방에 갈 수 있는 거점이 있는 지 확인
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
        // 공이 없으며, 한 방에 공으로 갈 수 있는 지 확인
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
        // 공이 있으며, 한 방에 거점으로 갈 수 있는지 확인 
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
        //공이 있으며 한 방에 상대방 하고 상호작용 가능한지 체크 (거점 우선)
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
        //공이 있으며 한 방에 행동 가능한 아군 하고 상호 작용 가능한지 체크 (패스용, 거점 우선)
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
        //공이 없으며 공이 있는 상대에게 상호작용이 가능한지 체크 (거점 우선)
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
        //공이 없을 때 아군 캐릭터는 여기 안 넣는 이유 여기는 캐릭터 뽑는 코드라 그럼 아군 아군 이동은 캐릭터 선택 과정에서 큰 의미 없음

        SetSelectAICharacter(aiManager.ActionAbleEnemyCharacter[Random.Range(0, aiManager.ActionAbleEnemyCharacter.Count)]); //랜덤
    }

    private void SetSelectAICharacter(GameObject targetCharacter)
    {
        aiManager.battleManager.targetCharacter = targetCharacter;
        GameManager.instance.SetCameraPosition(aiManager.battleManager.targetCharacter);
    }

    private void CheckStrategicPoint() //거점에 따른 ai행동방식
    {
        #region
        //거점 체크(사실상 이동 목표 설정)
        if (aiManager.isStrategicPointOnMap == false)
        {
            Debug.Log("맵에 거점이 없습니다");
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

        if (aiManager.isEnemyStrategicPointCountEnough == true) //거점이 충분할 때
        {
            #region
            //위험지역이 있는 지 검사
            int warningStrategicPointRange = 4; //위험
            int carefulStrategicPointRange = 6; //경계
            int warningAIMoveRange = 3; //위험할 때 ai 이동 반경
            int carefulAIMoveRange = 6; //경계할 떄 ai 이동 반경

            for (int i = 0; i < aiManager.battleManager.strategicPointMapBlocks.Count; i++)
            {
                MapBlock checkMapBlock = aiManager.battleManager.strategicPointMapBlocks[i];
                if (checkMapBlock.tileOwner == TileOwner.Enemy)
                {                    
                    int playerCharacterCount = 0;
                    int enemyCharacterCount = 0;
                    List<MapBlock> warningMapBlock = new List<MapBlock>();
                    List<GameObject> checkEndCharacter = new List<GameObject>();
                    List<Character> targerToStrategicPointNear = new List<Character>(); //거점 주변에 있거나 거점을 주변을 타겟으로 삼고 있는 AI캐릭터 
                    // 위험 검사
                    #region
                    warningMapBlock = aiManager.CheckTargetNearTile(warningStrategicPointRange, checkMapBlock.gameObject);
                    for (int j = 0; j < aiManager.battleManager.enemyCharacters.Length; j++) //목표확인
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

                    for (int j = 0; j < warningMapBlock.Count; j++) //실제로 위치하는지 확인
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
                    //위험 지역에 적이 있을 때 거점을 지키는(타일을 밟고 있는 캐릭터가 있나 확인)
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

                        if (isPlayerOnSPTile == true) //상대가 거점을 밟고 있을 경우
                        {
                            for (int j = 0; j < targerToStrategicPointNear.Count; j++)
                            {
                                targerToStrategicPointNear[j].aiTargetMapBlock = null; //해당 거점을 향하는 캐릭터들 목표 초기화
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

                        //해당 거점을 지키는 Ai캐릭터가 없는 경우
                        #region
                        if (isAIOnSPTile == false) 
                        {   
                            if (CanSetTargetMapBlockCharacter.Count < 1) //목표를 설정할 수 있는 캐릭터가 없는 경우
                            {
                                continue;
                            }
                            if (aiManager.GetNearObject(CanSetTargetMapBlockCharacter, checkMapBlock.gameObject, false) != null) // 목표를 설정 할 수 있는 캐릭터 중 가까운 Ai캐릭터를 해당 거점을 목표로 움직이게 함
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

                    //위험 거점에 있는 캐릭터 숫자 검사
                    if (playerCharacterCount > enemyCharacterCount) 
                    {
                        //플레이어가 더 많은 경우
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
                        // AI가 더 많을 경우
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
                    else //위험 지역이 안정적인경우 경계지역 검사
                    {
                        playerCharacterCount = 0;
                        enemyCharacterCount = 0;
                        warningMapBlock = new List<MapBlock>();
                        checkEndCharacter = new List<GameObject>();
                        targerToStrategicPointNear = new List<Character>();

                        // 경계 검사
                        #region
                        warningMapBlock = aiManager.CheckTargetNearTile(carefulStrategicPointRange, checkMapBlock.gameObject);
                        for (int j = 0; j < aiManager.battleManager.enemyCharacters.Length; j++) //목표확인
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

                        for (int j = 0; j < warningMapBlock.Count; j++) //실제로 위치하는지 확인
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

                        //경계 거점에 있는 캐릭터 숫자 검사
                        if (playerCharacterCount > enemyCharacterCount) 
                        {
                            //플레이어가 더 많은 경우
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
                                    //CanSetTargetMapBlockCharacter.Remove(nearObject); 경계는 빼둠
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
                            // AI가 더 많을 경우
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
        else //거점이 충분하지 않을때
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

            if ((aiCanSetTargetStrategicPoint.Count > 0) && (CanSetTargetMapBlockCharacter.Count > 0)) //갈 수 있는 거점이 있을때
            {
                int ableToMoveLengthDif = 6; //플레이어 캐릭터보다 타일에 얼마나 뒤에 있는지 체크해서 보정하는거
                #region
                for (int j = 0; j < aiCanSetTargetStrategicPoint.Count; j++)
                {
                    bool alreadyTargetCheck = false;

                    for (int i = 0; i < aiManager.battleManager.enemyCharacters.Length; i++)
                    {
                        if (aiManager.battleManager.enemyCharacters[i].GetComponent<Character>().aiTargetMapBlock == aiCanSetTargetStrategicPoint[j]) //같은 곳으로 여러명이 가고 있는지 체크
                        {
                            alreadyTargetCheck = true;
                            break;
                        }
                    }

                    if (alreadyTargetCheck == true)
                    {
                        continue;
                    }

                    GameObject nearObject = aiManager.GetNearObject(CanSetTargetMapBlockCharacter, aiCanSetTargetStrategicPoint[j].gameObject, false); //갈 수 있는 곳(목적지에 캐릭터가 없는경우)만 조사하니 true로 캐릭터 예외처리 안해도 됨
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

    public override void AICharacterMove() //AI가 이동할 곳 판단
    {
        #region        
        GameObject targetCharacter = aiManager.battleManager.targetCharacter;
        aiManager.battleManager.battleCharacterManager.GetAICharacterMoveTile(ref targetCharacter);

        if ((aiManager.battleManager.battleCharacterManager.actionAbleTile.Count < 1) || targetCharacter.GetComponent<Character>().aiMoveInPlace == true) //이동 가능 한 곳이 없거나 제자리 이동이 켜져 있으면 제자리 이동
        {
            #region
            Debug.Log(targetCharacter + "가 제자리 이동을 함");
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


        if ((targetCharacter.GetComponent<Character>().aiTargetMapBlock != null)) //목표가 있다면 목표를 향해 이동
        {
            
            //목표로 한 방에 갈 수 있는 경우 목표로 이동
            #region
            if (CheckAICanMoveToTargetOnce(targetCharacter) == true)
            {
                Debug.Log("목표로 이동!");
                return;
            }
            #endregion

            //목표가 거점이 아닐 때 이번 턴 공으로 한 방에 갈 수 있고 공이 없는 경우 경우 공으로 이동
            #region
            if ((targetCharacter.GetComponent<Character>().aiTargetMapBlock.tileType != TileType.StrategicPoint) && (targetCharacter.GetComponent<Character>().GetIsHaveBall() == false))
            {
                MapBlock checkMapBlock = CheckBallOnCharacterMoveRange(targetCharacter);
                if (checkMapBlock != null)
                {
                    targetCharacter.GetComponent<Character>().aiTargetMapBlock = null;
                    Debug.LogWarning("목표를 초기화 후 공으로 이동!");
                    AICharacterMove(targetCharacter, checkMapBlock);
                    return;
                }
            }
            #endregion

            //목표가 거점이 아닐 때 이번 턴 AI가 소유하지 않은 거점으로 한 방에 갈 수 있는 경우 거점으로 이동
            #region
            if (targetCharacter.GetComponent<Character>().aiTargetMapBlock.tileType != TileType.StrategicPoint)
            {
                MapBlock checkMapBlock = CheckStrategicPointOnCharacterMoveRange(targetCharacter);
                if ((checkMapBlock != null) && (checkMapBlock.tileOwner != TileOwner.Enemy))
                {
                    for (int i = 0; i < aiManager.ActionAbleEnemyCharacter.Count; i++)
                    {
                        int TargetSPCount = 0;
                        if (aiManager.ActionAbleEnemyCharacter[i].GetComponent<Character>().aiTargetMapBlock == checkMapBlock) //다른 AI가 해당 하는 거점을 노리고 있는 지 여부를 체크
                        {
                            TargetSPCount++;
                            break;
                        }

                        if (TargetSPCount < 1)
                        {
                            targetCharacter.GetComponent<Character>().aiTargetMapBlock = null;
                            Debug.LogWarning("목표를 초기화 후 거점으로 이동!");
                            AICharacterMove(targetCharacter, checkMapBlock);
                            return;
                        }
                    }
                }
            }        
            #endregion

            //목표로 한 방에 못가는 경우 목표를 향해 이동
            #region
            if (MoveToTarget(targetCharacter) == true)
            {
                Debug.Log("목표를 향하여 이동!");
                return;
            }  
            #endregion
        }

        //여기부터가 목표가 없거나 목표로 가지 못하는 경우   
        targetCharacter.GetComponent<Character>().aiTargetMapBlock = null; //목표로 못가거나 목표가 없는 경우니 목표 초기화
        MapBlock targetMapBlock = null;

        // 한 방에 갈 수 있는 거점이 있는 지 확인
        #region
        targetMapBlock = CheckStrategicPointOnCharacterMoveRange(targetCharacter);
        if (targetMapBlock != null)
        {
            AICharacterMove(targetCharacter, targetMapBlock);
            return;
        }
 
        #endregion
        // 공이 없으며, 한 방에 공으로 갈 수 있는 지 확인
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
        //공이 있으며 한 방에 상대방 하고 상호작용 가능한지 체크 (거점 우선)
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
        //공이 있으며 한 방에 행동 가능한 아군 하고 상호 작용 가능한지 체크 (패스용, 거점 우선)
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
        //공이 없으며 공이 있는 상대에게 상호작용이 가능한지 체크 (거점 우선)
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

        //1턴 안에 할 수 있는 행동이 없는 경우
        //공이 있으면 가까운 상대나 행동을 안한 아군을 향해 이동(아군쪽에 보정이 걸림)
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

            GameObject nearAI = aiManager.GetNearObject(aiCharacterList, targetCharacter, false); //타일 찍었으니 false로
            GameObject nearPlayer = aiManager.GetNearObject(playerCharacterList, targetCharacter, false); //타일 찍었으니 false로

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
        //공이 없으면 필드에 떨어져 있는 공이나 공을 가진 가진 적을 향해 이동(필드에 떨어진 공이 좀 더 거리에 보정 값을 가짐)
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
            
            GameObject nearFieldBall = aiManager.GetNearObject(fieldBallList, targetCharacter, false); //타일이니 false로
            GameObject nearHaveBallPlayer = aiManager.GetNearObject(playerCharacterList, targetCharacter, false); //타일이니 false로
            
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
        //모든 경우가 해당 안될 때 하는 이동, 이동 가능 타일중 가장 우선순위가 높은 타일로 이동
        Debug.LogWarning(targetCharacter + "가 모든 이동 조건을 충족하지 않음");
        AICharacterMove(targetCharacter, null);
        #endregion
    }

    private void AICharacterMove(GameObject targetCharacter, MapBlock targetMapBlock)
    {
        #region
        aiManager.battleManager.battleCharacterManager.GetAICharacterMoveTile(ref targetCharacter);

        if ((targetMapBlock != null) && (aiManager.battleManager.battleCharacterManager.actionAbleTile.Contains(targetMapBlock.gameObject) == false))
        {
            Debug.LogError("이번턴 이동목표로 설정 된 곳으로 이동 할 수 없습니다");
            targetMapBlock = null;
        }

        if (targetMapBlock == null) //targetMapBlock이 없으면 최선의 수로 랜덤 이동
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
        aiManager.SetAICharacterSkill(); //사용가능한 스킬 받아오기
        //스킬 분류
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
        //공이 있는 경우
        if (targetCharacter.GetComponent<Character>().GetIsHaveBall() == true)
        {
            //공격 할 수 있는 상대가 있는 경우(AttackSkill)
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
            //패스 할 수 있는 행동 안한 아군이 있을경우(PassSkill)
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
            //패스 할 수 있고 행동을 종료했지만 나보다 방어력이 높은 아군이 있는 경우(PassSkill)
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

        //공이 없거나 공을 사용할 수 없는 경우
        //주변에 공을 가진 플레이어 캐릭터가 있고 자신의 공격력과 상대의 방어력 차이가 일정이하일 때(보정값, InterruptBallSkill)
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
        //자신의 체력이 일정 이하일 때(HealSkill)
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
        //범위 내의 아군의 체력이 일정 이하 일 때(HealSkill)
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
        //범위 내의 적군이 있고 공이 필요 없는 공격류 스킬을 사용 가능 할 때(NoneBallAttackSkill)
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
        //범위 내의 적군이 있고 디버프 스킬을 사용 가능할 때(DebuffSkill)
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
        //자신에게 버프 스킬을 사용가능 할 때(BuffSkill)
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
        //범위 내의 아군이 있고 버프스킬을 사용 가능할 때(BuffSkill)
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
        //적의 마나가 일정 이상 일 때(BreakManaSkill)
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
        //적의 감정이 일정 이상 일 때(SympathyDecreaseSkill)(나중에 조건 강화 필요)
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
        //자신의 마나가 일정 이하 일 때(ManaHealSkill)
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
        //아군의 마나가 일정 이하 일 때(ManaHealSkill)
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
        //주변에 공을 가진 플레이어 캐릭터가 있을 때(InterruptBallSkill)
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
        //추후 기준이 정립되면 추가, 자신의 감정이 일정 이상이며 범위 내 아군의 감정이 일정이하 일때(조건 좀 더 강해지면 감정 감소 하는것도 추가해야함)

        //추후 기준이 정립되면 추가, 자신의 감정이 일정 이상일때 감정 감소기(감정 너무 높아지면 못쓰는 스킬이 있기에 방지용, 감정 높은데 저 위에 조건들 다 못채운건 사용가능한 스킬이 없다고도 판단함) //ai가 지향하는 감정타입도 만들면 그거 이상일떄 낮추고 그러면 됨

        //자신 감정증가기(SympathyIncreaseSkill)
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
        //할 수 있는게 없을 때(대기)
        aiManager.battleManager.BattleStay();
        #endregion
    }

    private Character GetHPLowestCharacter(List<Character> checkCharacterList)
    {
        #region
        if (checkCharacterList.Count < 1)
        {
            Debug.LogError("GetHPLowestCharacter에 잘못된 값이 들어왔습니다!");
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

    private List<Skill> GetUseableSkillList(Character attacker, Character target, List<Skill> checkSkillList) //리스트에서 해당하는 대상에게 사용가능한 스킬을 뽑는 코드
    {
        #region
        List<Skill> useableSkillList = new List<Skill>();
        if (attacker == null || target == null || checkSkillList.Count < 1)
        {
            Debug.LogWarning("GetUseableSkillList함수에 잘못된 값이 들어왔습니다!");
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
            if((checkSkillList[i].ignoreIsHaveBall == false) && (attacker.GetIsHaveBall() == false)) //기본 기술에 버프라든가 디버프 들어가면 배틀커맨드매니저에서 거르는거 피해서 올 수도 있으니 예외처리
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

    public Skill GetBestDamageSkill(CharacterStatus attackerStatus, CharacterStatus targetStatus, List<Skill> checkSkillList) //가장 데미지 밸류가 높은 스킬을 뽑는 코드
    {
        #region
        if (attackerStatus == null || targetStatus == null || checkSkillList.Count < 1)
        {
            if (checkSkillList.Count < 1)
            {
                Debug.Log("GetBestDamageSkill리스트에 들어온게 없습니다!");
                return null;
            }
            Debug.LogWarning("GetBestDamageSkill함수에 잘못된 값이 들어왔습니다!");
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
            Debug.LogWarning("GetBestDamageSkill함수에 잘못된 값이 들어왔습니다!");
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
            Debug.LogError("GetBestDamageSkill함수에 잘못된 값이 들어왔습니다!");
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
