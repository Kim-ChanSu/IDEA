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
        //�׽�Ʈ��
        /*
        SkillDivision[] sd = SkillDatabaseManager.instance.CheckSkillDivision(skill);
        Debug.LogWarning("����� ��ų�� " + skill.skillName);
        Debug.LogWarning("��ų �з��� ");
        for (int i = 0; i < sd.Length; i++)
        {
            Debug.LogWarning(sd[i]);
        }
        Debug.LogWarning("��");
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

                if((skill.skillType == SkillType.MarilynSpecial) && (battleManager.interruptObject.transform.tag == "Character")) // ������ �����
                {                    
                    battleManager.skillTarget = battleManager.interruptObject;
                    skillTarget = battleManager.interruptObject;
                    Debug.Log("Ÿ�ٺ���, Ÿ���� " + battleManager.skillTarget);
                    battleManager.SetBattleLog(skillUser.GetComponent<Character>(),skill.ingameSkillName  + "�� ��ų ����� " + skillTarget.GetComponent<Character>().status.inGameName + "��(��) �Ǿ���!");
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
                    Debug.LogWarning("Ȯ��ǥ�� " + checker);
                    if(checker > battleManager.interruptBallPercentage)
                    { 
                        // ������ ���� ��Ʈ�� ���ع��� �־����� ��ų��� ����
                        isSkillSuccess = true;
                    }
                    else
                    { 
                        // ������ ���� ��Ʈ�� ���ع��� ���� ��ų��� ����
                        isSkillSuccess = false;
                        damage = battleManager.defaultDamage;

                        //�ൿ���� ���� sp��ȭ(��ų)
                        if ((battleManager.interruptObject.transform.tag == "Character"))
                        {
                            //���� ����� ��ų�� ������ �� 
                            battleManager.ChangeSympathyByNature(battleManager.interruptObject, ChangeSympathySituation.BlockBall);
                            //�Ʊ��� ���� ��� ���� Ŀ�� ������ ��(����ä��)
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
                // ������ ���� ��Ʈ�� ���ع��� ���⿡ ��ų ��� ����
                isSkillSuccess = true;
            }

            //������ �����
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
                Debug.Log("�������� " + damageIncrease);
                damage = (int)(damage * damageIncrease);

            }
            #endregion

            if(skill.ignoreBound == true)
            { 
                //�ٿ�带 �����ϴ� ��� 
                if(isSkillSuccess == true)
                { 
                    //���ݿ� ������ ���
                    BallCatch(battleManager, skillUser, skillTarget); 
                }     
                else
                { 
                    //���ظ� �޾� ���ݿ� �����Ѱ��
                    damage = battleManager.defaultDamage;
                    if(battleManager.interruptObject.transform.tag == "Character")
                    { 
                        //���ظ� �Ѱ� ĳ������ ���
                        BallCatch(battleManager, skillUser, battleManager.interruptObject);                   
                    }
                    else
                    { 
                        //���ظ� �Ѱ� �ٸ� ������Ʈ�� ��� ������Ʈ�� ����� �ȵǱ⿡ ���⼭�� �ٿ�带 ��
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
                        //���ݿ� ���������� �������� ����Ʈ�� ���
                        BallCatch(battleManager, skillUser, skillTarget);
                        isSkillSuccess = false;

                        //�ൿ���� ���� sp��ȭ(��ų)
                        //�� �����Ͽ��� �� 
                        battleManager.ChangeSympathyByNature(skillTarget, ChangeSympathySituation.HitSafe);
                    }
                    else if(isAttackSkill == true)
                    { 
                        //���ݿ� ������ ���
                        isSameSideCatchBoundBall = BallBound(battleManager, skillUser, skillTarget, skill);
                        if(isSameSideCatchBoundBall == true)
                        { 
                            //���� ���� ���� ��Ƽ� Ŀ�� ���� ���
                            damage = battleManager.defaultDamage;   
                            isSkillSuccess = false;

                            //�ൿ���� ���� sp��ȭ(��ų)
                            //�Ʊ��� ���� ��� ���� Ŀ�� ������ ��  
                            battleManager.ChangeSympathyByNature(skillTarget, ChangeSympathySituation.SafeByFriendly);
                        }
                        else 
                        { 
                            if(skill.skillType == SkillType.KarlSpecial)//ī�� �����
                            { 
                                KarlSpecial(battleManager, skillUser, skill);
                            }
                        }

                    }
                    else
                    { 
                        BallCatch(battleManager, skillUser, skillTarget); //���ݿ� ��ų�� �ƴҰ�� ignoreBound�� üũ���� �ʾƵ� �˾Ƽ� �����ϰ� ���ִ� ����
                    }
                }
                else
                { 
                    //���ظ� �޾� ���ݿ� �����Ѱ��
                    damage = battleManager.defaultDamage;
                    if(battleManager.interruptObject.transform.tag == "Character")
                    { 
                        //���ظ� �Ѱ� ĳ������ ���
                        BallCatch(battleManager, skillUser, battleManager.interruptObject);                   
                    }
                    else
                    { 
                        //���ظ� �Ѱ� �ٸ� ������Ʈ�� ���
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
            //�� �������θ� �����ϴ� ��ų�� ���
            //skillTarget.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(skill.skillTargetEffect)); //����Ʈ //����
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
                //���� ���� ��� ���׹���
            }

            Debug.Log("�������� " + standardInterruptPercent + " Ȯ��ǥ�� " + checker);
            skillTarget.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(skill.skillTargetEffect)); //����Ʈ

            if(checker > standardInterruptPercent)
            {         
                //����
                Debug.Log("���ؼ���!");
                //�ൿ���� ���� sp��ȭ(�����ϱ�)
                //�����ϱ⿡ ���� ���� ������ ��  
                battleManager.ChangeSympathyByNature(skillTarget, ChangeSympathySituation.LoseBallByInterruptBall);

                InterruptBallBound(battleManager, skillTarget);
                // ��밡 ƨ��� ������� ����� ó�� ���ٲ��� �� ������ ���� �����͵� void -> bool�� �ٲٰ�
                SetTargetDamage(ref battleManager, skillDamage, sympathyDamage);
                SetTargetStatusEffect(ref battleManager, skill);
            }
            else
            { 
                //����
                Debug.Log("���ؽ���!");
                //�ൿ���� ���� sp��ȭ(�����ϱ�)
                //�����ϱ⸦ ��� ���� �� 
                battleManager.ChangeSympathyByNature(skillTarget, ChangeSympathySituation.SafeInterruptBall);

                battleManager.BallMoveEnd();
            }
        }
        else
        { 
            Debug.LogWarning("����� ���� ������ ���� �ʽ��ϴ�!");
            battleManager.BallMoveEnd();
        }
        #endregion
    }

    private static bool BallBound(BattleManager battleManager, GameObject skillUser,GameObject skillTarget, Skill skill, GameObject startBoundPos = null)//skill�� ���� Ư���� ������� �߰�
    { 
        bool isSameSideCatchBoundBall = false;
        int boundAblePosNumChecker = 1; //�̰� ���� �ʻ�� ������ 1�̾����

        Vector2 targetPos1;
        if (startBoundPos == null)
        {
            targetPos1 = new Vector2((int)skillTarget.transform.position.x, (int)skillTarget.transform.position.y);
        }
        else
        { 
            targetPos1 = new Vector2((int)startBoundPos.transform.position.x, (int)startBoundPos.transform.position.y);
        }

        battleManager.boundAblePosition.Clear(); //���� Ƣ����� ���°� ����
        while (battleManager.boundAblePosition.Count < boundAblePosNumChecker)
        {
            int i = 0;
            if (skill.skillType == SkillType.RiaSpecial) //���� �����
            { 
                battleManager.GetBallBoundPositionWithCheckCharacterName(skillTarget, battleManager.defaultBallBoundRange + i, CharacterDatabaseManager.instance.GetPlayerCharacter(0).name);
            }
            else
            { 
                battleManager.GetBallBoundPosition(skillTarget, battleManager.defaultBallBoundRange + i);
            }

            i++;
        }

        Debug.Log("�ٿ�� ������ ��ġ�� " + battleManager.boundAblePosition.Count);
        int boundNum = Random.Range(0, battleManager.boundAblePosition.Count);
        if((battleManager.boundAblePosition[boundNum].transform.tag == "Character") && (skillTarget.GetComponent<Character>().GetIsHaveBall() == false))
        { 
            //�ൿ���� ���� sp��ȭ(��ų)
            //�ðܳ��� ���� ����� �� 
            battleManager.ChangeSympathyByNature(battleManager.boundAblePosition[boundNum], ChangeSympathySituation.CatchBoundBall);

            if((skillTarget.GetComponent<Character>().isEnemy == battleManager.boundAblePosition[boundNum].GetComponent<Character>().isEnemy) && (skillTarget.GetComponent<Character>().GetIsHaveBall() == false))
            { 
                isSameSideCatchBoundBall = true;
            }
        }
        Vector2 targetPos2 = new Vector2((int)battleManager.boundAblePosition[boundNum].transform.position.x, (int)battleManager.boundAblePosition[boundNum].transform.position.y);
        //skillUser.GetComponent<Character>().ball.GetComponent<BallController>().SetBallPosition(new Vector2 (skillUser.transform.position.x ,skillUser.transform.position.y));
        //skillUser.GetComponent<Character>().ball.GetComponent<BallController>().SetBallActive(true);
        //���� �ΰ� �Ʒ����� ������ ���� ������ ������ ����
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
        //���� �ΰ� �Ʒ����� ������ ���� ������ ������ ����
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
                Debug.Log("�����̻� Ȯ��ǥ�� " + checker);
            
                if(checker <= useSkill.targetStatusEffect[i].statusEffectPercentage)
                { 
                    Debug.Log("�����̻� ���� ����");
                    battleManager.skillStatusEffectNum[i] = StatusEffectDatabaseManager.instance.GetStatusEffectNum(useSkill.targetStatusEffect[i].statusEffect);
                }
                else
                { 
                    Debug.Log("�����̻� ���� ����");
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
                Debug.Log("�����̻� Ȯ��ǥ�� " + checker);
            
                if(checker <= useSkill.targetDispelStatusEffect[i].statusEffectPercentage)
                { 
                    Debug.Log("�����̻� ���� ���� ����");
                    battleManager.skillDispelStatusEffectNum[i] = StatusEffectDatabaseManager.instance.GetStatusEffectNum(useSkill.targetDispelStatusEffect[i].statusEffect);
                }
                else
                { 
                    Debug.Log("�����̻� ���� ���� ����");
                    battleManager.skillDispelStatusEffectNum[i] = battleManager.ignoreSkillStatusEffectNum;
                }
            }
        }
    }

    //---- Ư�����
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
                    //battleManager.SetBattleLog(battleManager.playerCharacters[i].GetComponent<Character>(), battleManager.playerCharacters[i].GetComponent<Character>().status.inGameName + "�� ������ " + GameManager.instance.GetSympathyTypeName(targetSympathyType) + "��(��) �Ǿ���.");
                    battleManager.playerCharacters[i].GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(skill.skillTargetEffect)); //����Ʈ
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
                    //battleManager.SetBattleLog(battleManager.enemyCharacters[i].GetComponent<Character>(), battleManager.enemyCharacters[i].GetComponent<Character>().status.inGameName + "�� ������ " + GameManager.instance.GetSympathyTypeName(targetSympathyType) + "��(��) �Ǿ���.");
                    battleManager.enemyCharacters[i].GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(skill.skillTargetEffect)); //����Ʈ
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
            battleManager.SetBattleLog(skillUser.GetComponent<Character>(), skillUser.GetComponent<Character>().status.inGameName + "��(��) " + karlSpecial.inGameName + "���°� �Ǿ���!");
        }

        karlSpecial.ATKChange = karlSpecial.ATKChange + skill.specialCount;
        battleManager.SetBattleLog(null, karlSpecial.inGameName + "�� ȿ���� " + skill.specialCount + "��ŭ ���� �� " + karlSpecial.ATKChange + "��(��) �Ǿ���!");
        #endregion
    }

    private static void MarzenSpecial(BattleManager battleManager, GameObject skillUser, Skill skill)
    { 
        #region
        List<Character> skillTaret = new List<Character>();

        if(battleManager.CheckGameObjectByPositionAndName(new Vector2(skillUser.transform.position.x, skillUser.transform.position.y), CharacterDatabaseManager.instance.GetPlayerCharacter(3).name ,(int)skill.specialCount) == true)
        { 
            Debug.Log(CharacterDatabaseManager.instance.GetPlayerCharacter(3).name + " �߰�!");
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
                battleManager.SetBattleLog(skillTaret[i].GetComponent<Character>(), skillTaret[i].GetComponent<Character>().status.inGameName + "��(��) " + skill.targetStatusEffect[j].statusEffect.inGameName + "���°� �Ǿ���!");
                skillTaret[i].StartEffect(EffectDatabaseManager.instance.GetEffect(skill.skillTargetEffect)); //����Ʈ
            }
        }
        battleManager.BallMoveEnd();
        #endregion
    }
    #endregion
}
