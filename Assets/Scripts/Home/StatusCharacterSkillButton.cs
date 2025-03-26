using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatusCharacterSkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    private int skillNum;
    private string IncreaseColor = "<color=#0000FF>";
    private string DecreaseColor = "<color=#FF0000>";
    private string skillSympathyTypeText;
    
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
        }
        else
        { 
            Debug.LogWarning("SetSkillNum에 잘못 된 값이 들어 왔습니다 들어온 값은 " + num);  
            Destroy(this.gameObject);
        }
    }

    public void SelectButton()
    { 
        GameManager.instance.homeManager.SetSkillExplainText(skillSympathyTypeText + SkillDatabaseManager.instance.GetSkill(skillNum).skillExplain);
    }

    public void ClearExplain()
    { 
        GameManager.instance.homeManager.SetSkillExplainText("");
    }

    //배틀씬용

    public void BattleSelectButton()
    { 
        GameManager.instance.battleManager.battleGetInformationManager.SetSkillExplainText(skillSympathyTypeText + SkillDatabaseManager.instance.GetSkill(skillNum).skillExplain);
    }

    public void BattleClearExplain()
    { 
        GameManager.instance.battleManager.battleGetInformationManager.SetSkillExplainText("");
    }

    public void OnPointerEnter(PointerEventData eventData)
    { 
        if(GameManager.instance.homeManager != null)
        { 
            SelectButton();
        }

        if(GameManager.instance.battleManager != null)
        { 
            BattleSelectButton();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    { 
        if(GameManager.instance.homeManager != null)
        { 
            ClearExplain();
        }

        if(GameManager.instance.battleManager != null)
        { 
            BattleClearExplain();
        }
    }

    public void OnSelect(BaseEventData eventData)
    { 
        if(GameManager.instance.homeManager != null)
        { 
            SelectButton();
        }

        if(GameManager.instance.battleManager != null)
        { 
            BattleSelectButton();
        }
    }
}
