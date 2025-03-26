using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultLearnSkillPrefab : MonoBehaviour
{
    private float effectTime = 0.0f;
    private float breakTime = 2.0f;

    void Update()
    {
        CheckBreakThisObject();
    }

    private void CheckBreakThisObject()
    { 
        if(effectTime >= breakTime)
        { 
            Destroy(this.gameObject);    
        }
        else
        { 
            effectTime += Time.deltaTime;
        }
    }

    public void SetBattleResultLearnSkill(int skillNum)
    { 
        Skill skill = SkillDatabaseManager.instance.GetSkill(skillNum);
        this.transform.GetChild(0).GetComponent<Image>().sprite = skill.skillSprite;
        this.transform.GetChild(1).GetComponent<Text>().text = skill.ingameSkillName + "¸¦ ¹è¿ü´Ù!";
        effectTime = 0.0f;
    }
}
