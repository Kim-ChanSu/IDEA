using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillDivision
{
    Attack, //공격
    NoneBallAttack, //공 없이 쓰는 공격
    Pass, //패스
    InterruptBall, //방해하기
    Heal, //힐(대상이 아군일 때만)
    Buff, //버프(아군에게 주는 경우 버프 취급)
    Debuff, //디버프(적에게 주는 경우 디버프 취급)
    Dispel, //상태변화 제거기
    ManaHeal, //마나 회복기
    BreakMana, //적 마나 제거기
    SympathyIncrease, // 감정 증가기
    SympathyDecrease, // 감정 감소기
}

public class SkillDatabaseManager : MonoBehaviour
{
    public static SkillDatabaseManager instance; //싱글톤

    void Awake() 
    {
        // 싱글톤
        #region
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("씬에 SkillDatabaseManager가 2개이상 존재합니다.");
            Destroy(gameObject);
        }
        #endregion
    }

    [SerializeField]
    private Skill[] skill;

    public Skill GetSkill(int i)
    { 
        if(i < skill.Length)
        {
            return skill[i];
        }
        else
        {
            Debug.LogWarning("SkillDatabaseManager안 GetSkill함수에 잘못된 값이 들어왔습니다.");
            return skill[0];
        }        
    }

    public int GetSkillNum(Skill getSkill)
    { 
        for(int i = 0; i < skill.Length; i++)
        { 
            if(skill[i] == getSkill)
            { 
                return i;    
            }
        }

        Debug.LogWarning("스킬 데이터베이스에 등록되지 않은 스킬입니다!");
        return GameManager.RESETINDEX;
    }

    public int SkillLength()
    { 
        return skill.Length;
    }

    public SkillDivision[] CheckSkillDivision(Skill skill)
    {
        #region
        //전용기
        #region
        if (skill.skillType == SkillType.RaySpecial)
        {
            return new SkillDivision[] {SkillDivision.SympathyIncrease};
        }

        if (skill.skillType == SkillType.RiaSpecial)
        {
            return new SkillDivision[] {SkillDivision.Attack};
        }

        if (skill.skillType == SkillType.KarlSpecial)
        {
            return new SkillDivision[] {SkillDivision.Attack};
        }

        if (skill.skillType == SkillType.MarilynSpecial)
        {
            return new SkillDivision[] {SkillDivision.Attack};
        }

        if (skill.skillType == SkillType.MarzenSpecial)
        {
            return new SkillDivision[] {SkillDivision.Debuff};
        }
        #endregion

        if (CheckSkillHaveDPH(skill) == true) // 계수가 있는 스킬
        {
            if (skill.skillType == SkillType.Attack) //스킬 타입이 공격인 경우
            {
                if (skill.ignoreIsHaveBall == false) //공을 쓰는 스킬인 경우
                {
                    if (skill.damageType == DamageType.HP) // 스킬 공격타입이 HP인 경우
                    {
                        return new SkillDivision[] {SkillDivision.Attack}; 
                    }
                    else // 스킬 공격타입이 MP인 경우
                    {
                        return new SkillDivision[] {SkillDivision.BreakMana};
                    }
                }
                else //공을 무시하는 스킬인 경우
                {
                    if (skill.damageType == DamageType.HP) // 스킬 공격타입이 HP인 경우
                    {
                        if (CheckSkillHaveStatusEffect(skill) == true) //스킬이 상태이상이 있는 경우
                        {
                            if ((skill.skillTarget == SkillTarget.Friendly) || (skill.skillTarget == SkillTarget.Self) || (skill.skillTarget == SkillTarget.FriendlyAndSelf)) // 스킬 대상이 아군인 경우
                            { 
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Buff});

                            }
                            else if (skill.skillTarget == SkillTarget.Enemy) // 스킬 대상이 적인 경우
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Debuff, SkillDivision.NoneBallAttack});
                            }
                            else // 스킬 대상이 모두인 경우
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.NoneBallAttack});
                            }
                        }
                        else if(CheckSkillHaveDispelStatusEffect(skill) == true) // 스킬이 상태이상 해제가 있는 경우
                        { 
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Dispel, SkillDivision.NoneBallAttack});
                        }
                        else //스킬이 상태이상과 상태이상 해제가 없는 경우 
                        {   
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.NoneBallAttack});
                        }
                    }
                    else // 스킬 공격타입이 MP인 경우
                    {
                        if (CheckSkillHaveStatusEffect(skill) == true) //스킬이 상태이상이 있는 경우
                        {
                            if ((skill.skillTarget == SkillTarget.Friendly) || (skill.skillTarget == SkillTarget.Self) || (skill.skillTarget == SkillTarget.FriendlyAndSelf)) // 스킬 대상이 아군인 경우
                            {  
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Buff});
                            }
                            else if (skill.skillTarget == SkillTarget.Enemy) // 스킬 대상이 적인 경우
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Debuff, SkillDivision.BreakMana});
                            }
                            else // 스킬 대상이 모두인 경우
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.BreakMana});
                            }
                        }
                        else if(CheckSkillHaveDispelStatusEffect(skill) == true) // 스킬이 상태이상 해제가 있는 경우
                        { 
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Dispel, SkillDivision.BreakMana});
                        }
                        else //스킬이 상태이상과 상태이상 해제가 없는 경우 
                        {
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.BreakMana});
                        }
                        
                    }
                }
            }
            else if (skill.skillType == SkillType.Pass) //스킬 타입이 패스인 경우
            {
                if (skill.ignoreIsHaveBall == false) //공을 쓰는 스킬인 경우
                {
                    if (skill.damageType == DamageType.HP) // 스킬 회복 타입이 HP인 경우
                    {
                        return new SkillDivision[] {SkillDivision.Pass, SkillDivision.Heal}; 
                    }
                    else
                    {
                        return new SkillDivision[] {SkillDivision.Pass, SkillDivision.ManaHeal}; // 스킬 회복 타입이 MP인 경우
                    }
                }
                else //공을 무시하는 스킬인 경우
                {
                    if (skill.damageType == DamageType.HP) // 스킬 공격타입이 HP인 경우
                    {
                        if (CheckSkillHaveStatusEffect(skill) == true) //스킬이 상태이상이 있는 경우
                        {
                            if ((skill.skillTarget == SkillTarget.Friendly) || (skill.skillTarget == SkillTarget.Self) || (skill.skillTarget == SkillTarget.FriendlyAndSelf)) // 스킬 대상이 아군인 경우
                            { 
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Buff, SkillDivision.Heal});
                            }
                            else if (skill.skillTarget == SkillTarget.Enemy) // 스킬 대상이 적인 경우
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Debuff});
                            }
                            else // 스킬 대상이 모두인 경우
                            { 
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Heal});
                            }
                        }
                        else if(CheckSkillHaveDispelStatusEffect(skill) == true) // 스킬이 상태이상 해제가 있는 경우
                        { 
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Dispel, SkillDivision.Heal});
                        }
                        else //스킬이 상태이상과 상태이상 해제가 없는 경우 
                        {  
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Heal});
                        }
                    }
                    else // 스킬 회복 타입이 MP인 경우
                    {
                        if (CheckSkillHaveStatusEffect(skill) == true) //스킬이 상태이상이 있는 경우
                        {
                            if ((skill.skillTarget == SkillTarget.Friendly) || (skill.skillTarget == SkillTarget.Self) || (skill.skillTarget == SkillTarget.FriendlyAndSelf)) // 스킬 대상이 아군인 경우
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Buff, SkillDivision.ManaHeal});
                            }
                            else if (skill.skillTarget == SkillTarget.Enemy) // 스킬 대상이 적인 경우
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Debuff});
                            }
                            else // 스킬 대상이 모두인 경우
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.ManaHeal});
                            }
                        }
                        else if(CheckSkillHaveDispelStatusEffect(skill) == true) // 스킬이 상태이상 해제가 있는 경우
                        { 
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Dispel, SkillDivision.ManaHeal});
                        }
                        else //스킬이 상태이상과 상태이상 해제가 없는 경우 
                        {
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.ManaHeal});
                        }
                    }
                }
            }
            else //스킬 타입이 방해하기인 경우
            {
                return new SkillDivision[] {SkillDivision.InterruptBall};
            }
        }
        else //계수가 없는 스킬
        {
            if (skill.ignoreIsHaveBall == false) // 공을 사용하는 경우
            {
                if (skill.skillType == SkillType.Attack) //공격인 경우
                {
                    return new SkillDivision[] {SkillDivision.Attack}; //여기들어오면 스킬 설정쪽 버그임, 절대 성공 안하는 공격만 존재할뿐 
                }
                else if(skill.skillType == SkillType.Pass)
                {
                    if (CheckSkillHaveStatusEffect(skill) == true) //스킬이 상태이상이 있는 경우
                    {
                        if ((skill.skillTarget == SkillTarget.Friendly) || (skill.skillTarget == SkillTarget.Self) || (skill.skillTarget == SkillTarget.FriendlyAndSelf)) // 스킬 대상이 아군인 경우
                        {
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Buff, SkillDivision.Pass});
                        }
                        else if (skill.skillTarget == SkillTarget.Enemy) // 스킬 대상이 적인 경우
                        {
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Debuff, SkillDivision.Pass});
                        }
                        else // 스킬 대상이 모두인 경우
                        {
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Pass});
                        }
                    }
                    else if(CheckSkillHaveDispelStatusEffect(skill) == true) // 스킬이 상태이상 해제가 있는 경우
                    { 
                        return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Pass, SkillDivision.Dispel});
                    }
                    else //스킬이 상태이상과 상태이상 해제가 없는 경우 
                    {
                        return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Pass});
                    }
                }
                else
                {
                    return new SkillDivision[] {SkillDivision.InterruptBall};
                }
            }
            else if (skill.skillType == SkillType.InterruptBall)
            {
                return new SkillDivision[] {SkillDivision.InterruptBall};
            }
            else
            {
                if (CheckSkillHaveStatusEffect(skill) == true) //스킬이 상태이상이 있는 경우
                {
                    if ((skill.skillTarget == SkillTarget.Friendly) || (skill.skillTarget == SkillTarget.Self) || (skill.skillTarget == SkillTarget.FriendlyAndSelf)) // 스킬 대상이 아군인 경우
                    {
                        return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Buff});
                    }
                    else if (skill.skillTarget == SkillTarget.Enemy) // 스킬 대상이 적인 경우
                    {
                        return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Debuff});
                    }
                    else // 스킬 대상이 모두인 경우
                    {
                        return CheckSkillSympathyDamage(skill, new SkillDivision[0]);
                    }
                }
                else if(CheckSkillHaveDispelStatusEffect(skill) == true) // 스킬이 상태이상 해제가 있는 경우
                { 
                    return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Dispel});
                }
                else //스킬이 상태이상과 상태이상 해제가 없는 경우 
                {
                    return CheckSkillSympathyDamage(skill, new SkillDivision[0]);
                }
            }
        }
        #endregion
    }

    private bool CheckSkillHaveDPH(Skill skill) 
    {
        #region
        int checker = 0;
        float checkMinDPH = 0.01f;

        if (((-1 * checkMinDPH) > skill.DPHMaxHP) || (skill.DPHMaxHP > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.DPHHP) || (skill.DPHHP > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.DPHMaxMP) || (skill.DPHMaxMP > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.DPHMP) || (skill.DPHMP > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.DPHATK) || (skill.DPHATK > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.DPHMAK) || (skill.DPHMAK > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.DPHDEF) || (skill.DPHDEF > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.DPHMDF) || (skill.DPHMDF > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.enemyMaxHP) || (skill.enemyMaxHP > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.enemyHP) || (skill.enemyHP > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.enemyMaxMP) || (skill.enemyMaxMP > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.enemyMP) || (skill.enemyMP > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.enemyATK) || (skill.enemyATK > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.enemyMAK) || (skill.enemyMAK > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.enemyDEF) || (skill.enemyDEF > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (((-1 * checkMinDPH) > skill.enemyMDF) || (skill.enemyMDF > checkMinDPH))
        {
            checker = checker + 1;
        }
        if (skill.fixedValue != 0)
        {
            checker = checker + 1;
        }

        if (checker > 0)
        {
            return true;
        }
        else
        {
            return false; 
        }         
        #endregion
    }

    private bool CheckSkillHaveStatusEffect(Skill skill)
    {
        #region
        if (skill.targetStatusEffect.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
        #endregion
    }

    private bool CheckSkillHaveDispelStatusEffect(Skill skill)
    {
        #region
        if (skill.targetDispelStatusEffect.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
        #endregion
    }

    private SkillDivision[] CheckSkillSympathyDamage(Skill skill, SkillDivision[] beforeSkillDivison)
    {
        SkillDivision[] afterSkillDivision = new SkillDivision[beforeSkillDivison.Length + 1];

        for (int i = 0; i < beforeSkillDivison.Length; i++)
        {
            afterSkillDivision[i] = beforeSkillDivison[i];
        }

        if (skill.enemySympathyDamage > 0) //대상의 감정이 감소하는 경우
        {
            afterSkillDivision[beforeSkillDivison.Length] = SkillDivision.SympathyDecrease;
            return afterSkillDivision;
        }
        else if (skill.enemySympathyDamage < 0) //대상의 감정이 증가하는 경우
        {
            afterSkillDivision[beforeSkillDivison.Length] = SkillDivision.SympathyIncrease;
            return afterSkillDivision;  
        }
        else //대상의 감정이 변화 없는 경우
        {
            return beforeSkillDivison;    
        }
    }

}
