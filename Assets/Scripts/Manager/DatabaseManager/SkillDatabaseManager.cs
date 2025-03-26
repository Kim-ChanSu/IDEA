using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillDivision
{
    Attack, //����
    NoneBallAttack, //�� ���� ���� ����
    Pass, //�н�
    InterruptBall, //�����ϱ�
    Heal, //��(����� �Ʊ��� ����)
    Buff, //����(�Ʊ����� �ִ� ��� ���� ���)
    Debuff, //�����(������ �ִ� ��� ����� ���)
    Dispel, //���º�ȭ ���ű�
    ManaHeal, //���� ȸ����
    BreakMana, //�� ���� ���ű�
    SympathyIncrease, // ���� ������
    SympathyDecrease, // ���� ���ұ�
}

public class SkillDatabaseManager : MonoBehaviour
{
    public static SkillDatabaseManager instance; //�̱���

    void Awake() 
    {
        // �̱���
        #region
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("���� SkillDatabaseManager�� 2���̻� �����մϴ�.");
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
            Debug.LogWarning("SkillDatabaseManager�� GetSkill�Լ��� �߸��� ���� ���Խ��ϴ�.");
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

        Debug.LogWarning("��ų �����ͺ��̽��� ��ϵ��� ���� ��ų�Դϴ�!");
        return GameManager.RESETINDEX;
    }

    public int SkillLength()
    { 
        return skill.Length;
    }

    public SkillDivision[] CheckSkillDivision(Skill skill)
    {
        #region
        //�����
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

        if (CheckSkillHaveDPH(skill) == true) // ����� �ִ� ��ų
        {
            if (skill.skillType == SkillType.Attack) //��ų Ÿ���� ������ ���
            {
                if (skill.ignoreIsHaveBall == false) //���� ���� ��ų�� ���
                {
                    if (skill.damageType == DamageType.HP) // ��ų ����Ÿ���� HP�� ���
                    {
                        return new SkillDivision[] {SkillDivision.Attack}; 
                    }
                    else // ��ų ����Ÿ���� MP�� ���
                    {
                        return new SkillDivision[] {SkillDivision.BreakMana};
                    }
                }
                else //���� �����ϴ� ��ų�� ���
                {
                    if (skill.damageType == DamageType.HP) // ��ų ����Ÿ���� HP�� ���
                    {
                        if (CheckSkillHaveStatusEffect(skill) == true) //��ų�� �����̻��� �ִ� ���
                        {
                            if ((skill.skillTarget == SkillTarget.Friendly) || (skill.skillTarget == SkillTarget.Self) || (skill.skillTarget == SkillTarget.FriendlyAndSelf)) // ��ų ����� �Ʊ��� ���
                            { 
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Buff});

                            }
                            else if (skill.skillTarget == SkillTarget.Enemy) // ��ų ����� ���� ���
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Debuff, SkillDivision.NoneBallAttack});
                            }
                            else // ��ų ����� ����� ���
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.NoneBallAttack});
                            }
                        }
                        else if(CheckSkillHaveDispelStatusEffect(skill) == true) // ��ų�� �����̻� ������ �ִ� ���
                        { 
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Dispel, SkillDivision.NoneBallAttack});
                        }
                        else //��ų�� �����̻�� �����̻� ������ ���� ��� 
                        {   
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.NoneBallAttack});
                        }
                    }
                    else // ��ų ����Ÿ���� MP�� ���
                    {
                        if (CheckSkillHaveStatusEffect(skill) == true) //��ų�� �����̻��� �ִ� ���
                        {
                            if ((skill.skillTarget == SkillTarget.Friendly) || (skill.skillTarget == SkillTarget.Self) || (skill.skillTarget == SkillTarget.FriendlyAndSelf)) // ��ų ����� �Ʊ��� ���
                            {  
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Buff});
                            }
                            else if (skill.skillTarget == SkillTarget.Enemy) // ��ų ����� ���� ���
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Debuff, SkillDivision.BreakMana});
                            }
                            else // ��ų ����� ����� ���
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.BreakMana});
                            }
                        }
                        else if(CheckSkillHaveDispelStatusEffect(skill) == true) // ��ų�� �����̻� ������ �ִ� ���
                        { 
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Dispel, SkillDivision.BreakMana});
                        }
                        else //��ų�� �����̻�� �����̻� ������ ���� ��� 
                        {
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.BreakMana});
                        }
                        
                    }
                }
            }
            else if (skill.skillType == SkillType.Pass) //��ų Ÿ���� �н��� ���
            {
                if (skill.ignoreIsHaveBall == false) //���� ���� ��ų�� ���
                {
                    if (skill.damageType == DamageType.HP) // ��ų ȸ�� Ÿ���� HP�� ���
                    {
                        return new SkillDivision[] {SkillDivision.Pass, SkillDivision.Heal}; 
                    }
                    else
                    {
                        return new SkillDivision[] {SkillDivision.Pass, SkillDivision.ManaHeal}; // ��ų ȸ�� Ÿ���� MP�� ���
                    }
                }
                else //���� �����ϴ� ��ų�� ���
                {
                    if (skill.damageType == DamageType.HP) // ��ų ����Ÿ���� HP�� ���
                    {
                        if (CheckSkillHaveStatusEffect(skill) == true) //��ų�� �����̻��� �ִ� ���
                        {
                            if ((skill.skillTarget == SkillTarget.Friendly) || (skill.skillTarget == SkillTarget.Self) || (skill.skillTarget == SkillTarget.FriendlyAndSelf)) // ��ų ����� �Ʊ��� ���
                            { 
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Buff, SkillDivision.Heal});
                            }
                            else if (skill.skillTarget == SkillTarget.Enemy) // ��ų ����� ���� ���
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Debuff});
                            }
                            else // ��ų ����� ����� ���
                            { 
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Heal});
                            }
                        }
                        else if(CheckSkillHaveDispelStatusEffect(skill) == true) // ��ų�� �����̻� ������ �ִ� ���
                        { 
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Dispel, SkillDivision.Heal});
                        }
                        else //��ų�� �����̻�� �����̻� ������ ���� ��� 
                        {  
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Heal});
                        }
                    }
                    else // ��ų ȸ�� Ÿ���� MP�� ���
                    {
                        if (CheckSkillHaveStatusEffect(skill) == true) //��ų�� �����̻��� �ִ� ���
                        {
                            if ((skill.skillTarget == SkillTarget.Friendly) || (skill.skillTarget == SkillTarget.Self) || (skill.skillTarget == SkillTarget.FriendlyAndSelf)) // ��ų ����� �Ʊ��� ���
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Buff, SkillDivision.ManaHeal});
                            }
                            else if (skill.skillTarget == SkillTarget.Enemy) // ��ų ����� ���� ���
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Debuff});
                            }
                            else // ��ų ����� ����� ���
                            {
                                return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.ManaHeal});
                            }
                        }
                        else if(CheckSkillHaveDispelStatusEffect(skill) == true) // ��ų�� �����̻� ������ �ִ� ���
                        { 
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Dispel, SkillDivision.ManaHeal});
                        }
                        else //��ų�� �����̻�� �����̻� ������ ���� ��� 
                        {
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.ManaHeal});
                        }
                    }
                }
            }
            else //��ų Ÿ���� �����ϱ��� ���
            {
                return new SkillDivision[] {SkillDivision.InterruptBall};
            }
        }
        else //����� ���� ��ų
        {
            if (skill.ignoreIsHaveBall == false) // ���� ����ϴ� ���
            {
                if (skill.skillType == SkillType.Attack) //������ ���
                {
                    return new SkillDivision[] {SkillDivision.Attack}; //��������� ��ų ������ ������, ���� ���� ���ϴ� ���ݸ� �����һ� 
                }
                else if(skill.skillType == SkillType.Pass)
                {
                    if (CheckSkillHaveStatusEffect(skill) == true) //��ų�� �����̻��� �ִ� ���
                    {
                        if ((skill.skillTarget == SkillTarget.Friendly) || (skill.skillTarget == SkillTarget.Self) || (skill.skillTarget == SkillTarget.FriendlyAndSelf)) // ��ų ����� �Ʊ��� ���
                        {
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Buff, SkillDivision.Pass});
                        }
                        else if (skill.skillTarget == SkillTarget.Enemy) // ��ų ����� ���� ���
                        {
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Debuff, SkillDivision.Pass});
                        }
                        else // ��ų ����� ����� ���
                        {
                            return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Pass});
                        }
                    }
                    else if(CheckSkillHaveDispelStatusEffect(skill) == true) // ��ų�� �����̻� ������ �ִ� ���
                    { 
                        return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Pass, SkillDivision.Dispel});
                    }
                    else //��ų�� �����̻�� �����̻� ������ ���� ��� 
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
                if (CheckSkillHaveStatusEffect(skill) == true) //��ų�� �����̻��� �ִ� ���
                {
                    if ((skill.skillTarget == SkillTarget.Friendly) || (skill.skillTarget == SkillTarget.Self) || (skill.skillTarget == SkillTarget.FriendlyAndSelf)) // ��ų ����� �Ʊ��� ���
                    {
                        return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Buff});
                    }
                    else if (skill.skillTarget == SkillTarget.Enemy) // ��ų ����� ���� ���
                    {
                        return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Debuff});
                    }
                    else // ��ų ����� ����� ���
                    {
                        return CheckSkillSympathyDamage(skill, new SkillDivision[0]);
                    }
                }
                else if(CheckSkillHaveDispelStatusEffect(skill) == true) // ��ų�� �����̻� ������ �ִ� ���
                { 
                    return CheckSkillSympathyDamage(skill, new SkillDivision[] {SkillDivision.Dispel});
                }
                else //��ų�� �����̻�� �����̻� ������ ���� ��� 
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

        if (skill.enemySympathyDamage > 0) //����� ������ �����ϴ� ���
        {
            afterSkillDivision[beforeSkillDivison.Length] = SkillDivision.SympathyDecrease;
            return afterSkillDivision;
        }
        else if (skill.enemySympathyDamage < 0) //����� ������ �����ϴ� ���
        {
            afterSkillDivision[beforeSkillDivison.Length] = SkillDivision.SympathyIncrease;
            return afterSkillDivision;  
        }
        else //����� ������ ��ȭ ���� ���
        {
            return beforeSkillDivison;    
        }
    }

}
