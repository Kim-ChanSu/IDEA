using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCommandManager : MonoBehaviour
{
    private BattleManager battleManager;

    public GameObject skillButtonPrefab;
    public GameObject skillInventoryContent;
    public GameObject skillExplainText;

    void Start()
    {
        InitializeBattleCommandManagerManager();
    }

    private void InitializeBattleCommandManagerManager()
    {
        battleManager = this.gameObject.GetComponent<BattleManager>();
    }

    public void SetBattleSkillWindow(bool mode)
    { 
        #region
        if(mode == true)
        { 
            battleManager.SetBattleSceneHinderUI(false);
            skillExplainText.GetComponent<Text>().text = "";
            battleManager.battleSkillWindow.SetActive(true);
            battleManager.battleGetInformationManager.SetBattleCommandWindow(false);
                
            if((battleManager.targetCharacter != null) && (skillButtonPrefab != null))
            { 
                for(int i = 0; i < battleManager.targetCharacter.GetComponent<Character>().status.characterSkill.Length; i++)
                { 
                    if(battleManager.targetCharacter.GetComponent<Character>().status.characterSkill[i] == true)
                    { 
                        GameObject skillButton = Instantiate(skillButtonPrefab);
                        skillButton.name = "SkillButton_" + i;
                        skillButton.GetComponent<BattleSkillButton>().SetSkillNum(i);

                        skillButton.transform.SetParent(skillInventoryContent.transform);
                        skillButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); 
                    }
                }
            }
            
        }     
        else
        { 
            if(skillInventoryContent.transform.childCount != 0)
            { 
                for(int i = 0; i < skillInventoryContent.transform.childCount; i++)
                { 
                    Destroy(skillInventoryContent.transform.GetChild(i).gameObject);
                }
            }

            skillExplainText.GetComponent<Text>().text = "";
            battleManager.battleSkillWindow.SetActive(false);
            battleManager.battleGetInformationManager.SetBattleCommandWindow(true);    
            battleManager.SetBattleSceneHinderUI(true);
        }
        battleManager.isBattleSkillWindow = mode;
        #endregion
    }

    public bool CheckCanUseSkill(Skill thisSkill)
    { 
        int skillNumChecker = SkillDatabaseManager.instance.GetSkillNum(thisSkill);
        if((skillNumChecker != GameManager.DEFAULTINTERRUPTBALLNUM) && (skillNumChecker != GameManager.DEFAULTPASSSKILLNUM) && (skillNumChecker != battleManager.targetCharacter.GetComponent<Character>().status.attackSkillNum))
        { 
            if(battleManager.targetCharacter.GetComponent<Character>().GetIsCantUseSkill() == true)
            { 
                return false;
            }

            if((battleManager.targetCharacter.GetComponent<Character>().status.HP <= thisSkill.useHP) || (battleManager.targetCharacter.GetComponent<Character>().status.MP < thisSkill.useMP) || (battleManager.targetCharacter.GetComponent<Character>().status.Sympathy < thisSkill.useSympathy))
            { 
                return false;
            }

            if((thisSkill.ignoreIsHaveBall == false) && (battleManager.targetCharacter.GetComponent<Character>().GetIsHaveBall() == false))
            { 
                return false;
            }

            if((thisSkill.skillSympathyType != SympathyType.None) && (thisSkill.skillSympathyType != GameManager.instance.battleManager.targetCharacter.GetComponent<Character>().sympathyType))
            { 
                return false;
            }     
        
            return true;
        }
        else
        { 
            return true;    
        }
    }

    public void SetSkillExplainText(string explain)
    { 
        skillExplainText.GetComponent<Text>().text = explain;
    }


    public void SelectTarget()
    { 
        if(GameManager.instance.KeyCheckAccept() == true)
        { 
            GameObject skillTarget = null;
            bool isActiconableTile = false;

            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);
            
            for(int i = 0; i < hits.Length; i++)
            { 
                if (hits[i].transform.tag == "Tile")
                {
                    if(hits[i].transform.GetComponent<MapBlock>().isActionAble == true)
                    {
                        isActiconableTile = true;
                    }
                }
                else if(hits[i].transform.tag == "Character")
                { 
                    skillTarget = hits[i].transform.gameObject;
                }
            }

            if((isActiconableTile == true) && (skillTarget != null))
            {
                battleManager.UseSkill(battleManager.targetCharacter, skillTarget, battleManager.useSkill);
            }

        }
    }
}
