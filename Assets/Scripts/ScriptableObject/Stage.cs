using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage")]
public class Stage : ScriptableObject
{
    public string stageName;
    [TextArea]
    public string stageExplain;
    [TextArea]
    public string stageWinCondition;
    public string enemyTeamName;
    public int stageUseHealth;
    [Header("���� ����������")]
    public int D_day;
    [Header("���� ����������")]
    public int stageNeedMainStageNum;

    [Header("--�������� �⺻���� ����--")]
    public string stageCSVName;
    public string stageCSVPart;
    public string stageBattleSceneName;
    public string stageEndCSVName;
    public string stageEndCSPart;
    [Header("��⿡ �����ϴ� �÷��̾� ĳ���� ��")]
    [Header("--�����ο� ����--")]
    public int playerCharacterCount;
    [Header("�� ĳ����")]
    public int[] enemyCharacterNum; 
    [Header("----������----")]
    public int clearEXP;
    public int clearGold;
}
