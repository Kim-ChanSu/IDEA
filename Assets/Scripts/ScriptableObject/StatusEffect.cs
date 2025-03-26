/* �ۼ���¥: 2023-11-26
 * ����: 0.0.1ver 
 * ����: �����̻����
 * �ֱ� ���� ��¥: 2023-11-26
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
