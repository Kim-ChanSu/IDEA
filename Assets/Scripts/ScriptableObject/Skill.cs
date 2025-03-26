/* �ۼ���¥: 2023-09-22
 * ����: 0.0.1ver 
 * ����: ��ų����
 * �ֱ� ���� ��¥: 2023-10-17
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillTarget
{ 
    Enemy,
    Friendly,
    Self,
    FriendlyAndSelf, 
    All
}

public enum SkillType
{ 
    Attack,
    Pass,
    InterruptBall,
    RaySpecial,
    RiaSpecial,
    KarlSpecial,
    MarilynSpecial,
    MarzenSpecial

}

public enum DamageType
{ 
    HP,
    MP,
    //Sympathy
}

[System.Serializable]
public class TargetStatusEffect
{ 
    public StatusEffect statusEffect;
    public int statusEffectPercentage;    
}

[CreateAssetMenu(menuName = "Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public string ingameSkillName;
    public Sprite skillSprite;
    [TextArea]
    public string skillExplain;

    public SkillTarget skillTarget;
    public SkillType skillType;
    public DamageType damageType;
    public SympathyType skillSympathyType;

    public int skillUserEffect;
    public int skillTargetEffect;

    public bool ignoreIsHaveBall;
    public bool ignoreOtherCharacter;
    public bool ignoreBound;

    public int useHP;
    public int useMP;
    public int useSympathy;

    [Header ("��ų ����ڿ��� �ɸ��� �����̻�")]
    public TargetStatusEffect[] skillUserStatusEffect;
    [Header ("��ų ��󿡰� �ɸ��� �����̻�")]
    public TargetStatusEffect[] targetStatusEffect;
    [Header ("��ų ����� �����Ǵ� �����̻�")]
    public TargetStatusEffect[] targetDispelStatusEffect;

    //��ų���
    public float DPHMaxHP;
    public float DPHHP;
    public float DPHMaxMP;
    public float DPHMP;
    public float DPHATK;
    public float DPHMAK;
    public float DPHDEF;
    public float DPHMDF;

    //�ݰ�(���� ��)
    public float enemyMaxHP;
    public float enemyHP;
    public float enemyMaxMP;
    public float enemyMP;
    public float enemyATK;
    public float enemyMAK;
    public float enemyDEF;
    public float enemyMDF;

    [Range(0.0f, 1.0f)]
    public float variance;    
    public int fixedValue;

    
    public int enemySympathyDamage; //��ų Ÿ�� ����ġ ����
    public float specialCount; //Ư���� ���� ��ġ
}
