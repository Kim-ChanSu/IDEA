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
    [Header("메인 스테이지용")]
    public int D_day;
    [Header("서브 스테이지용")]
    public int stageNeedMainStageNum;

    [Header("--스테이지 기본정보 설정--")]
    public string stageCSVName;
    public string stageCSVPart;
    public string stageBattleSceneName;
    public string stageEndCSVName;
    public string stageEndCSPart;
    [Header("경기에 참여하는 플레이어 캐릭터 수")]
    [Header("--전투인원 세팅--")]
    public int playerCharacterCount;
    [Header("적 캐릭터")]
    public int[] enemyCharacterNum; 
    [Header("----보상설정----")]
    public int clearEXP;
    public int clearGold;
}
