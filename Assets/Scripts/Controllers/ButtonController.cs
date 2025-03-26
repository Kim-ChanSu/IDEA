/* 작성날짜: 2023-09-12
 * 버전: 0.0.2ver 
 * 내용: 버튼기능관리
 * 최근 수정 날짜: 2023-11-21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public void TitleStartGameButton()
    { 
        if (GameManager.instance.isSaveCanvasOn == false)
        {
            GameManager.instance.StartGame();
        }
        
    }

    public void TitleLoadGameButton()
    {
        if (GameManager.instance.isSaveCanvasOn == false)
        {        
            GameManager.instance.SetLoadCanvas();
        }
    }

    public void TitleExitGameButton()
    { 
        if (GameManager.instance.isSaveCanvasOn == false)
        {
            GameManager.instance.ExitGame();
        }
    }

    public void GoTalkSceneButton()
    { 
        GameManager.instance.GoTalkScene();
    }

    public void GoBattleSceneButton()
    {
        GameManager.instance.GoBattleScene();
    }

    public void LoadGameButton()
    { 
  
    }

    public void ExitGameButton()
    { 
        GameManager.instance.ExitGame();    
    }

    public void ReturnTitleButton()
    { 
        GameManager.instance.GoTitleScene();
    }

    public void BattleReturnGameButton()
    { 
        if(GameManager.instance.battleManager != null)
        { 
            GameManager.instance.battleManager.battleGetInformationManager.SetBattleMenuWindow(false);
        }        
    }

    public void BattleAttackButton()
    {
        if(GameManager.instance.battleManager != null)
        { 
            GameManager.instance.battleManager.AttackButton();
        }
    }

    public void BattlePassButton()
    {
        if(GameManager.instance.battleManager != null)
        { 
            GameManager.instance.battleManager.PassButton();
        }
    }

    public void BattleInterruptBallButton()
    {
        if(GameManager.instance.battleManager != null)
        { 
            GameManager.instance.battleManager.InterruptBallButton();
        }
    }

    public void BattleSkillButton(bool mode)
    {
        if(GameManager.instance.battleManager != null)
        { 
            GameManager.instance.battleManager.SkillButton(mode);
        }
    }

    public void BattleStayButton()
    {
        if(GameManager.instance.battleManager != null)
        { 
            GameManager.instance.battleManager.StayButton();
        }
    }
}
