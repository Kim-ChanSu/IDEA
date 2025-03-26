/* 작성날짜: 2023-11-21
 * 버전: 0.0.1ver 
 * 내용: 배틀 스킬 버튼
 * 최근 수정 날짜: 2023-11-21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleSkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    private int skillNum;
    private string IncreaseColor = "<color=#0000FF>";
    private string DecreaseColor = "<color=#FF0000>";
    private string skillSympathyTypeText;
    
    public void OnPointerEnter(PointerEventData eventData)
    { 
        SelectButton();
    }

    public void OnPointerExit(PointerEventData eventData)
    { 
        ClearExplain();
    }

    public void OnSelect(BaseEventData eventData)
    { 
        SelectButton();
    }

    private void SkillCostSetting(Skill thisSkill)
    { 
        #region
        if(thisSkill.useHP == 0)
        { 
            this.gameObject.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = "0";
        }
        else if(thisSkill.useHP < 0)
        { 
            this.gameObject.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = IncreaseColor + thisSkill.useHP * -1 + "</color>";
        }
        else
        { 
            this.gameObject.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = DecreaseColor + thisSkill.useHP + "</color>";
        }

        if(thisSkill.useMP == 0)
        { 
            this.gameObject.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = "0";
        }
        else if(thisSkill.useMP < 0)
        { 
            this.gameObject.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = IncreaseColor + thisSkill.useMP * -1 + "</color>";
        }
        else
        { 
            this.gameObject.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = DecreaseColor + thisSkill.useMP + "</color>";
        }

        if(thisSkill.useSympathy == 0)
        { 
            this.gameObject.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Text>().text = "0";
        }
        else if(thisSkill.useSympathy < 0)
        { 
            this.gameObject.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Text>().text = IncreaseColor + thisSkill.useSympathy * -1 + "</color>";
        }
        else
        { 
            this.gameObject.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Text>().text = DecreaseColor + thisSkill.useSympathy + "</color>";
        }    
        #endregion
    }

    public void SetSkillNum(int num)
    { 
        if((0 <= num) && (num < SkillDatabaseManager.instance.SkillLength()))
        { 
            skillNum = num;
            Skill thisSkill = SkillDatabaseManager.instance.GetSkill(skillNum);

            skillSympathyTypeText = "(" + GameManager.instance.GetSympathyTypeName(thisSkill.skillSympathyType) + ") ";
            

            this.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = thisSkill.skillSprite;
            this.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = thisSkill.ingameSkillName;
            SkillCostSetting(thisSkill);
            if((GameManager.instance.battleManager.battleCommandManager.CheckCanUseSkill(thisSkill) == false))
            { 
                this.gameObject.GetComponent<Button>().interactable = false;
            }
        }
        else
        { 
            Debug.LogWarning("SetSkillNum에 잘못 된 값이 들어 왔습니다 들어온 값은 " + num);  
            Destroy(this.gameObject);
        }
    }

    public void SkillButton()
    { 
        GameManager.instance.battleManager.SetUseSkill(skillNum);    
    }

    public void SelectButton()
    { 
        GameManager.instance.battleManager.battleCommandManager.SetSkillExplainText(skillSympathyTypeText + SkillDatabaseManager.instance.GetSkill(skillNum).skillExplain);
    }

    public void ClearExplain()
    { 
        GameManager.instance.battleManager.battleCommandManager.SetSkillExplainText("");
    }

}
