/* 작성날짜: 2023-10-31
 * 버전: 0.0.1ver 
 * 내용: 거점 버튼 기능
 * 최근 수정 날짜: 2024-01-15
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeButtonController : ButtonController
{
    public void BattleStart()
    {
        if (GameManager.instance.isMainStage == true)
        {
            GameManager.instance.Var[GameManager.MAINSTAGEVARNUM].Var++;
        }

        if (GameManager.instance.CSVName != "")
        {
            base.GoTalkSceneButton();   
        }
        else if (GameManager.instance.nextBattleSceneName != "")
        {
           GameManager.instance.GoNextBattleScene();
        }
        else
        {
            Debug.LogWarning("게임매니저에 CSV혹은 전투가 설정되어있지 않습니다!");
        }
    }

}
