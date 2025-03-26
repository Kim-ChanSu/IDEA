using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController
{
    static DamageType damageType;

    public static void UseSkill(BattleManager battleManager, GameObject skillUser,GameObject skillTarget, Skill skill)
    { 
        battleManager.SkillCost(skillUser, skill);
        damageType = skill.damageType;
        //테스트용
        /*
        SkillDivision[] sd = SkillDatabaseManager.instance.CheckSkillDivision(skill);
        Debug.LogWarning("사용한 스킬은 " + skill.skillName);
        Debug.LogWarning("스킬 분류는 ");
        for (int i = 0; i < sd.Length; i++)
        {
            Debug.LogWarning(sd[i]);
        }
        Debug.LogWarning("끝");
        */
        //
        switch(skill.skillType)
        {       
            case SkillType.Attack:               
                UseAttackSkill(battleManager, skillUser, skillTarget, skill);
                break;
            case SkillType.Pass:
                UsePassSkill(battleManager, skillUser, skillTarget, skill);
                break;
            case SkillType.InterruptBall:
                UseInterruptBallSkill(battleManager, skillUser, skillTarget, skill);
                break;
            case SkillType.RaySpecial:
                RaySpecial(battleManager, skillUser, skill);  
                break;
            case SkillType.RiaSpecial:
                UseAttackSkill(battleManager, skillUser, skillTarget, skill);
                break;
            case SkillType.KarlSpecial:
                UseAttackSkill(battleManager, skillUser, skillTarget, skill); 
                break;
            case SkillType.MarilynSpecial:
                UseAttackSkill(battleManager, skillUser, skillTarget, skill);  
                break;
            case SkillType.MarzenSpecial:
                MarzenSpecial(battleManager, skillUser, skill);                  
                break;
            default:
                UseAttackSkill(battleManager, skillUser, skillTarget, skill);
                break; 
        }
    }


    private static void UseAttackSkill(BattleManager battleManager, GameObject attacker, GameObject target, Skill useSkill)
    {                    
        ShootBall(battleManager, attacker, target, useSkill, battleManager.GetDamage(attacker.GetComponent<Character>().status, target.GetComponent<Character>().status, useSkill), true);
    }

    private static void UsePassSkill(BattleManager battleManager, GameObject attacker, GameObject target, Skill useSkill)
    { 
        ShootBall(battleManager, attacker, target, useSkill, battleManager.GetHeal(attacker.GetComponent<Character>().status, target.GetComponent<Character>().status, useSkill), false);
    }

    private static void UseInterruptBallSkill(BattleManager battleManager, GameObject attacker, GameObject target, Skill useSkill)
    { 
        InterruptBall(battleManager, attacker, target, useSkill, battleManager.GetDamage(attacker.GetComponent<Character>().status, target.GetComponent<Character>().status, useSkill));
    }

    private static void ShootBall(BattleManager battleManager, GameObject skillUser,GameObject skillTarget, Skill skill, int skillDamage, bool isAttackSkill)
    {
        #region
        int damage = skillDamage;//battleManager.GetDamage(skillUser.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, skill)
        int sympathyDamage = skill.enemySympathyDamage;

        if(skill.ignoreIsHaveBall != true)
        { 
            bool isSkillSuccess;
            bool isSameSideCatchBoundBall = false;
            
            if((battleManager.isSomethingOnBallMoveRoute == true) && (skill.ignoreOtherCharacter == false))
            { 
                bool isMarilynSpecialChangeTarget = false;

                if((skill.skillType == SkillType.MarilynSpecial) && (battleManager.interruptObject.transform.tag == "Character")) // 마릴린 스페셜
                {                    
                    battleManager.skillTarget = battleManager.interruptObject;
                    skillTarget = battleManager.interruptObject;
                    Debug.Log("타겟변경, 타겟은 " + battleManager.skillTarget);
                    battleManager.SetBattleLog(skillUser.GetComponent<Character>(),skill.ingameSkillName  + "의 스킬 대상이 " + skillTarget.GetComponent<Character>().status.inGameName + "이(가) 되었다!");
                    //---------
                    damage = battleManager.GetDamage(skillUser.GetComponent<Character>().status, skillTarget.GetComponent<Character>().status, skill);
                    //--------
                    isSkillSuccess = true;
                    isMarilynSpecialChangeTarget = true;
                }

                if(isMarilynSpecialChangeTarget == false)
                { 
                    int checker = 0;
                    checker = Random.Range(1,101);
                    Debug.LogWarning("확률표는 " + checker);
                    if(checker > battleManager.interruptBallPercentage)
                    { 
                        // 대상까지 가는 루트에 방해물이 있었으나 스킬사용 성공
                        isSkillSuccess = true;
                    }
                    else
                    { 
                        // 대상까지 가는 루트에 방해물로 인해 스킬사용 실패
                        isSkillSuccess = false;
                        damage = battleManager.defaultDamage;

                        //행동으로 인한 sp변화(스킬)
                        if ((battleManager.interruptObject.transform.tag == "Character"))
                        {
                            //공을 사용한 스킬을 막았을 때 
                            battleManager.ChangeSympathyByNature(battleManager.interruptObject, ChangeSympathySituation.BlockBall);
                            //아군이 공을 잡아 나를 커버 쳐줬을 때(가로채기)
                            battleManager.ChangeSympathyByNature(skillTarget, ChangeSympathySituation.SafeByFriendly);
                        }
                    }
                }
                else
                {
                    isSkillSuccess = true;
                }
            }
            else
            { 
                // 대상까지 가는 루트에 방해물이 없기에 스킬 사용 성공
                isSkillSuccess = true;
            }

            //마릴린 스페셜
            #region 
            if(skill.skillType == SkillType.MarilynSpecial)
            { 
                float damagemaxMPCorrection = (float)(skillUser.GetComponent<Character>().status.MP) / (float)(skillUser.GetComponent<Character>().status.maxMP);
                float damageMPCorrection = (float)skillUser.GetComponent<Character>().status.MP / 100.0f;
                float damageIncrease = damageMPCorrection * damagemaxMPCorrection;

                if(damageIncrease > skill.specialCount)
                { 
                    damageIncrease = skill.specialCount;                
                }

                if(skillUser.GetComponent<Character>().isEnemy == false)
                { 
                    GameManager.instance.PlayerCharacter[skillUser.GetComponent<Character>().characterDatabaseNum].MP = 0;
                }
                else
                { 
                    GameManager.instance.EnemyCharacter[skillUser.GetComponent<Character>().characterDatabaseNum].MP = 0;
                }
                Debug.Log("증폭값은 " + damageIncrease);
                damage = (int)(damage * damageIncrease);

            }
            #endregion

            if(skill.ignoreBound == true)
            { 
                //바운드를 무시하는 경우 
                if(isSkillSuccess == true)
                { 
                    //공격에 성공한 경우
                    BallCatch(battleManager, skillUser, skillTarget); 
                }     
                else
                { 
                    //방해를 받아 공격에 실패한경우
                    damage = battleManager.defaultDamage;
                    if(battleManager.interruptObject.transform.tag == "Character")
                    { 
                        //방해를 한게 캐릭터일 경우
                        BallCatch(battleManager, skillUser, battleManager.interruptObject);                   
                    }
                    else
                    { 
                        //방해를 한게 다른 오브젝트일 경우 오브젝트에 낑기면 안되기에 여기서는 바운드를 줌
                        isSameSideCatchBoundBall = BallBound(battleManager, skillUser, battleManager.interruptObject, skill);
                    }                    
                }
            }            
            else
            { 
                if(isSkillSuccess == true)
                {                
                    if((damage == battleManager.defaultDamage) && (isAttackSkill == true))
                    { 
                        //공격에 성공했지만 데미지가 디폴트인 경우
                        BallCatch(battleManager, skillUser, skillTarget);
                        isSkillSuccess = false;

                        //행동으로 인한 sp변화(스킬)
                        //방어에 성공하였을 때 
                        battleManager.ChangeSympathyByNature(skillTarget, ChangeSympathySituation.HitSafe);
                    }
                    else if(isAttackSkill == true)
                    { 
                        //공격에 성공한 경우
                        isSameSideCatchBoundBall = BallBound(battleManager, skillUser, skillTarget, skill);
                        if(isSameSideCatchBoundBall == true)
                        { 
                            //같은 팀이 공을 잡아서 커버 쳐준 경우
                            damage = battleManager.defaultDamage;   
                            isSkillSuccess = false;

                            //행동으로 인한 sp변화(스킬)
                            //아군이 공을 잡아 나를 커버 쳐줬을 때  
                            battleManager.ChangeSympathyByNature(skillTarget, ChangeSympathySituation.SafeByFriendly);
                        }
                        else 
                        { 
                            if(skill.skillType == SkillType.KarlSpecial)//카를 스페셜
                            { 
                                KarlSpecial(battleManager, skillUser, skill);
                            }
                        }

                    }
                    else
                    { 
                        BallCatch(battleManager, skillUser, skillTarget); //공격용 스킬이 아닐경우 ignoreBound를 체크하지 않아도 알아서 무시하게 해주는 보정
                    }
                }
                else
                { 
                    //방해를 받아 공격에 실패한경우
                    damage = battleManager.defaultDamage;
                    if(battleManager.interruptObject.transform.tag == "Character")
                    { 
                        //방해를 한게 캐릭터일 경우
                        BallCatch(battleManager, skillUser, battleManager.interruptObject);                   
                    }
                    else
                    { 
                        //방해를 한게 다른 오브젝트일 경우
                        isSameSideCatchBoundBall = BallBound(battleManager, skillUser, skillTarget, skill, battleManager.interruptObject);
                    }
                }
            }

            battleManager.SetIsUseBallSkillSuccess(isSkillSuccess);
            if(isSkillSuccess == false)
            { 
                sympathyDamage = 0;
            }
            else
            { 
                SetTargetStatusEffect(ref battleManager, skill);
            }
            SetTargetDamage(ref battleManager, damage, sympathyDamage);                     
        }
        else
        { 
            //공 소유여부를 무시하는 스킬인 경우
            //skillTarget.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(skill.skillTargetEffect)); //이펙트 //이전
            SetTargetDamage(ref battleManager, damage, sympathyDamage);
            SetTargetStatusEffect(ref battleManager, skill);
            //battleManager.BallMoveEnd();
            battleManager.SetIsUseBallSkillSuccess(false);
            battleManager.SetIgnoreIsHaveBallSkillEffect();
        }    
        #endregion
    }
    private static void InterruptBall(BattleManager battleManager, GameObject skillUser, GameObject skillTarget, Skill skill, int skillDamage)
    { 
        #region
        if(skillTarget.GetComponent<Character>().GetIsHaveBall() == true)
        { 
            int sympathyDamage = skill.enemySympathyDamage;

            int checker = 0;
            int standardInterruptPercent = 50 + (skillTarget.GetComponent<Character>().status.DEF * 2 - skillUser.GetComponent<Character>().status.ATK * 2);
            checker = Random.Range(1,101);
              
            if(skillUser.GetComponent<Character>().isEnemy == skillTarget.GetComponent<Character>().isEnemy)
            { 
                standardInterruptPercent = 0;
                //같은 편일 경우 저항무시
            }

            Debug.Log("기준점은 " + standardInterruptPercent + " 확률표는 " + checker);
            skillTarget.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(skill.skillTargetEffect)); //이펙트

            if(checker > standardInterruptPercent)
            {         
                //성공
                Debug.Log("방해성공!");
                //행동으로 인한 sp변화(방해하기)
                //방해하기에 당해 공을 뺏겼을 때  
                battleManager.ChangeSympathyByNature(skillTarget, ChangeSympathySituation.LoseBallByInterruptBall);

                InterruptBallBound(battleManager, skillTarget);
                // 상대가 튕긴공 잡았을때 대미지 처리 안줄꺼면 이 밑으로 수정 위엣것도 void -> bool로 바꾸고
                SetTargetDamage(ref battleManager, skillDamage, sympathyDamage);
                SetTargetStatusEffect(ref battleManager, skill);
            }
            else
            { 
                //실패
                Debug.Log("방해실패!");
                //행동으로 인한 sp변화(방해하기)
                //방해하기를 방어 했을 때 
                battleManager.ChangeSympathyByNature(skillTarget, ChangeSympathySituation.SafeInterruptBall);

                battleManager.BallMoveEnd();
            }
        }
        else
        { 
            Debug.LogWarning("대상이 공을 가지고 있지 않습니다!");
            battleManager.BallMoveEnd();
        }
        #endregion
    }

    private static bool BallBound(BattleManager battleManager, GameObject skillUser,GameObject skillTarget, Skill skill, GameObject startBoundPos = null)//skill은 리아 특수기 만드려고 추가
    { 
        bool isSameSideCatchBoundBall = false;
        int boundAblePosNumChecker = 1; //이거 리아 필살기 때문에 1이어야함

        Vector2 targetPos1;
        if (startBoundPos == null)
        {
            targetPos1 = new Vector2((int)skillTarget.transform.position.x, (int)skillTarget.transform.position.y);
        }
        else
        { 
            targetPos1 = new Vector2((int)startBoundPos.transform.position.x, (int)startBoundPos.transform.position.y);
        }

        battleManager.boundAblePosition.Clear(); //공이 튀길곳이 없는거 방지
        while (battleManager.boundAblePosition.Count < boundAblePosNumChecker)
        {
            int i = 0;
            if (skill.skillType == SkillType.RiaSpecial) //리아 스페셜
            { 
                battleManager.GetBallBoundPositionWithCheckCharacterName(skillTarget, battleManager.defaultBallBoundRange + i, CharacterDatabaseManager.instance.GetPlayerCharacter(0).name);
            }
            else
            { 
                battleManager.GetBallBoundPosition(skillTarget, battleManager.defaultBallBoundRange + i);
            }

            i++;
        }

        Debug.Log("바운드 가능한 위치는 " + battleManager.boundAblePosition.Count);
        int boundNum = Random.Range(0, battleManager.boundAblePosition.Count);
        if((battleManager.boundAblePosition[boundNum].transform.tag == "Character") && (skillTarget.GetComponent<Character>().GetIsHaveBall() == false))
        { 
            //행동으로 인한 sp변화(스킬)
            //팅겨나온 공을 잡았을 때 
            battleManager.ChangeSympathyByNature(battleManager.boundAblePosition[boundNum], ChangeSympathySituation.CatchBoundBall);

            if((skillTarget.GetComponent<Character>().isEnemy == battleManager.boundAblePosition[boundNum].GetComponent<Character>().isEnemy) && (skillTarget.GetComponent<Character>().GetIsHaveBall() == false))
            { 
                isSameSideCatchBoundBall = true;
            }
        }
        Vector2 targetPos2 = new Vector2((int)battleManager.boundAblePosition[boundNum].transform.position.x, (int)battleManager.boundAblePosition[boundNum].transform.position.y);
        //skillUser.GetComponent<Character>().ball.GetComponent<BallController>().SetBallPosition(new Vector2 (skillUser.transform.position.x ,skillUser.transform.position.y));
        //skillUser.GetComponent<Character>().ball.GetComponent<BallController>().SetBallActive(true);
        //위에 두개 아래에서 묶었음 버그 없으면 지워도 무방
        skillUser.GetComponent<Character>().SetBallActiveByThisPosition();
        skillUser.GetComponent<Character>().ball.GetComponent<BallController>().SetBallMoving(ref battleManager, new Vector2[2] {targetPos1, targetPos2});
        skillUser.GetComponent<Character>().SetIsHaveBall(false, null);        
        return isSameSideCatchBoundBall;
    }

    private static void BallCatch(BattleManager battleManager, GameObject skillUser,GameObject skillTarget)
    {
        Vector2 targetPos1 = new Vector2((int)skillTarget.transform.position.x, (int)skillTarget.transform.position.y);

        //skillUser.GetComponent<Character>().ball.GetComponent<BallController>().SetBallPosition(new Vector2 (skillUser.transform.position.x ,skillUser.transform.position.y));
        //skillUser.GetComponent<Character>().ball.GetComponent<BallController>().SetBallActive(true);
        //위에 두개 아래에서 묶었음 버그 없으면 지워도 무방
        skillUser.GetComponent<Character>().SetBallActiveByThisPosition();
        skillUser.GetComponent<Character>().ball.GetComponent<BallController>().SetBallMoving(ref battleManager, new Vector2[1] {targetPos1});
        skillUser.GetComponent<Character>().SetIsHaveBall(false, null);            
    }

    private static void InterruptBallBound(BattleManager battleManager, GameObject skillTarget)
    {        
        battleManager.GetBallBoundPosition(skillTarget, battleManager.defaultBallBoundRange);
        int boundNum = Random.Range(0, battleManager.boundAblePosition.Count);

        Vector2 targetPos1 = new Vector2((int)battleManager.boundAblePosition[boundNum].transform.position.x, (int)battleManager.boundAblePosition[boundNum].transform.position.y);
        skillTarget.GetComponent<Character>().SetBallActiveByThisPosition();
        skillTarget.GetComponent<Character>().ball.GetComponent<BallController>().SetBallMoving(ref battleManager, new Vector2[1] {targetPos1});
        skillTarget.GetComponent<Character>().SetIsHaveBall(false, null);

    }


    private static void SetTargetDamage(ref BattleManager battleManager,int damage, int sympathyDamage)
    { 
        switch(damageType)
        { 
            case DamageType.HP:
                battleManager.skillHPDamage = damage;
                break;
            case DamageType.MP:
                battleManager.skillMPDamage = damage;
                break;

            default:
                battleManager.skillHPDamage = damage;
                break;
        }       

        battleManager.skillSympathyDamage = sympathyDamage;
    }

    private static void SetTargetStatusEffect(ref BattleManager battleManager, Skill useSkill)
    { 
        if(useSkill.targetStatusEffect.Length > 0)
        { 
             battleManager.skillStatusEffectNum = new int[useSkill.targetStatusEffect.Length];
            for(int i = 0; i < battleManager.skillStatusEffectNum.Length; i++)
            { 
                battleManager.skillStatusEffectNum[i] = battleManager.ignoreSkillStatusEffectNum;
            }

            for(int i = 0; i < useSkill.targetStatusEffect.Length; i++)
            { 
                int checker = 0;
                checker = Random.Range(1,101);
                Debug.Log("상태이상 확률표는 " + checker);
            
                if(checker <= useSkill.targetStatusEffect[i].statusEffectPercentage)
                { 
                    Debug.Log("상태이상 판정 성공");
                    battleManager.skillStatusEffectNum[i] = StatusEffectDatabaseManager.instance.GetStatusEffectNum(useSkill.targetStatusEffect[i].statusEffect);
                }
                else
                { 
                    Debug.Log("상태이상 판정 실패");
                    battleManager.skillStatusEffectNum[i] = battleManager.ignoreSkillStatusEffectNum;
                }
            }
        }

        if(useSkill.targetDispelStatusEffect.Length > 0)
        { 
             battleManager.skillDispelStatusEffectNum = new int[useSkill.targetDispelStatusEffect.Length];
            for(int i = 0; i < battleManager.skillDispelStatusEffectNum.Length; i++)
            { 
                battleManager.skillDispelStatusEffectNum[i] = battleManager.ignoreSkillStatusEffectNum;
            }

            for(int i = 0; i < useSkill.targetDispelStatusEffect.Length; i++)
            { 
                int checker = 0;
                checker = Random.Range(1,101);
                Debug.Log("상태이상 확률표는 " + checker);
            
                if(checker <= useSkill.targetDispelStatusEffect[i].statusEffectPercentage)
                { 
                    Debug.Log("상태이상 해제 판정 성공");
                    battleManager.skillDispelStatusEffectNum[i] = StatusEffectDatabaseManager.instance.GetStatusEffectNum(useSkill.targetDispelStatusEffect[i].statusEffect);
                }
                else
                { 
                    Debug.Log("상태이상 해제 판정 실패");
                    battleManager.skillDispelStatusEffectNum[i] = battleManager.ignoreSkillStatusEffectNum;
                }
            }
        }
    }

    //---- 특수기들
    #region
    private static void RaySpecial(BattleManager battleManager, GameObject skillUser, Skill skill)
    {
        #region
        bool isEnemy = skillUser.GetComponent<Character>().isEnemy;
        SympathyType targetSympathyType = skillUser.GetComponent<Character>().sympathyType;

        if(isEnemy == false)
        { 
            for(int i = 0; i < battleManager.playerCharacters.Length; i++)
            { 
                if(skillUser != battleManager.playerCharacters[i].transform.gameObject)
                { 
                    SympathyDatabaseManager.instance.SetCharacterSympathy(battleManager.playerCharacters[i].GetComponent<Character>(), targetSympathyType);
                    //battleManager.SetBattleLog(battleManager.playerCharacters[i].GetComponent<Character>(), battleManager.playerCharacters[i].GetComponent<Character>().status.inGameName + "의 감정은 " + GameManager.instance.GetSympathyTypeName(targetSympathyType) + "이(가) 되었다.");
                    battleManager.playerCharacters[i].GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(skill.skillTargetEffect)); //이펙트
                }
            }
        }
        else
        { 
            for(int i = 0; i < battleManager.enemyCharacters.Length; i++)
            { 
                if(skillUser != battleManager.enemyCharacters[i].transform.gameObject)
                { 
                    SympathyDatabaseManager.instance.SetCharacterSympathy(battleManager.enemyCharacters[i].GetComponent<Character>(), targetSympathyType);
                    //battleManager.SetBattleLog(battleManager.enemyCharacters[i].GetComponent<Character>(), battleManager.enemyCharacters[i].GetComponent<Character>().status.inGameName + "의 감정은 " + GameManager.instance.GetSympathyTypeName(targetSympathyType) + "이(가) 되었다.");
                    battleManager.enemyCharacters[i].GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(skill.skillTargetEffect)); //이펙트
                }
            }            
        }
        battleManager.BallMoveEnd();
        #endregion
    }

    private static void KarlSpecial(BattleManager battleManager, GameObject skillUser, Skill skill)
    { 
        #region
        StatusEffect karlSpecial = StatusEffectDatabaseManager.instance.GetKarlSpecialStatusEffect();

        if(skillUser.GetComponent<Character>().characterStatusEffect[StatusEffectDatabaseManager.instance.GetKarlSpecialStatusEffectNum()].isOn == false)
        { 
            karlSpecial.ATKChange = 1.0f;
            karlSpecial.MAKChange = 1.0f;
            karlSpecial.DEFChange = 1.0f;
            karlSpecial.MDFChange = 1.0f;

            skillUser.GetComponent<Character>().SetStatusEffect(StatusEffectDatabaseManager.instance.GetKarlSpecialStatusEffectNum(), true);
            battleManager.SetBattleLog(skillUser.GetComponent<Character>(), skillUser.GetComponent<Character>().status.inGameName + "은(는) " + karlSpecial.inGameName + "상태가 되었다!");
        }

        karlSpecial.ATKChange = karlSpecial.ATKChange + skill.specialCount;
        battleManager.SetBattleLog(null, karlSpecial.inGameName + "의 효과가 " + skill.specialCount + "만큼 증가 해 " + karlSpecial.ATKChange + "이(가) 되었다!");
        #endregion
    }

    private static void MarzenSpecial(BattleManager battleManager, GameObject skillUser, Skill skill)
    { 
        #region
        List<Character> skillTaret = new List<Character>();

        if(battleManager.CheckGameObjectByPositionAndName(new Vector2(skillUser.transform.position.x, skillUser.transform.position.y), CharacterDatabaseManager.instance.GetPlayerCharacter(3).name ,(int)skill.specialCount) == true)
        { 
            Debug.Log(CharacterDatabaseManager.instance.GetPlayerCharacter(3).name + " 발견!");
            if(skillUser.GetComponent<Character>().isEnemy == true)
            { 
                for(int i = 0; i < battleManager.playerCharacters.Length; i++)
                { 
                    skillTaret.Add(battleManager.playerCharacters[i].GetComponent<Character>());
                }
            }
            else
            {
                for(int i = 0; i < battleManager.enemyCharacters.Length; i++)
                { 
                    skillTaret.Add(battleManager.enemyCharacters[i].GetComponent<Character>());
                }                
            }
        }
        else
        { 
            for(int i = 0; i < battleManager.battleCharacterManager.AttackableTarget.Count; i++)
            {                      
                skillTaret.Add(battleManager.battleCharacterManager.AttackableTarget[i].GetComponent<Character>());
            } 
        }

        for(int i = 0; i < skillTaret.Count; i++)
        { 
            for(int j = 0; j < skill.targetStatusEffect.Length; j++)
            { 
                skillTaret[i].SetStatusEffect(StatusEffectDatabaseManager.instance.GetStatusEffectNum(skill.targetStatusEffect[j].statusEffect), true);
                battleManager.SetBattleLog(skillTaret[i].GetComponent<Character>(), skillTaret[i].GetComponent<Character>().status.inGameName + "은(는) " + skill.targetStatusEffect[j].statusEffect.inGameName + "상태가 되었다!");
                skillTaret[i].StartEffect(EffectDatabaseManager.instance.GetEffect(skill.skillTargetEffect)); //이펙트
            }
        }
        battleManager.BallMoveEnd();
        #endregion
    }
    #endregion
}
