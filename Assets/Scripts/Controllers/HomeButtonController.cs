/* �ۼ���¥: 2023-10-31
 * ����: 0.0.1ver 
 * ����: ���� ��ư ���
 * �ֱ� ���� ��¥: 2024-01-15
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
            Debug.LogWarning("���ӸŴ����� CSVȤ�� ������ �����Ǿ����� �ʽ��ϴ�!");
        }
    }

}
