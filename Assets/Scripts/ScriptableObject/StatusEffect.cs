/* 작성날짜: 2023-11-26
 * 버전: 0.0.1ver 
 * 내용: 상태이상관련
 * 최근 수정 날짜: 2023-11-26
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffect")]
public class StatusEffect : ScriptableObject
{
    public string inGameName;
    public Sprite statusEffectIcon;
    [TextArea]
    public string statusEffectExplain;

    public bool isDead;
    public bool isCantMove;
    public bool isCantUseSkill;
    public bool isNotTurnCount;

    public int continueTurn;

    public float ATKChange;
    public float MAKChange;
    public float DEFChange;
    public float MDFChange;
    public int moveChange;
    public int rangeChange;

    public float turnDamage;
}
