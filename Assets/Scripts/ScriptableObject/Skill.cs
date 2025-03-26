/* 작성날짜: 2023-09-22
 * 버전: 0.0.1ver 
 * 내용: 스킬관련
 * 최근 수정 날짜: 2023-10-17
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

    [Header ("스킬 사용자에게 걸리는 상태이상")]
    public TargetStatusEffect[] skillUserStatusEffect;
    [Header ("스킬 대상에게 걸리는 상태이상")]
    public TargetStatusEffect[] targetStatusEffect;
    [Header ("스킬 대상이 해제되는 상태이상")]
    public TargetStatusEffect[] targetDispelStatusEffect;

    //스킬계수
    public float DPHMaxHP;
    public float DPHHP;
    public float DPHMaxMP;
    public float DPHMP;
    public float DPHATK;
    public float DPHMAK;
    public float DPHDEF;
    public float DPHMDF;

    //반감(방어력 등)
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

    
    public int enemySympathyDamage; //스킬 타겟 공감치 변동
    public float specialCount; //특수기 전용 수치
}
