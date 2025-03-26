using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultCharacterFace : MonoBehaviour
{
    private Color isDeadColor = new Color(77f/255f, 77f/255f, 77f/255f, 255f/255f);
    private int faceNum = 0;

    public void SetBattleResultCharacterFace(Character character)
    { 
        if(faceNum < character.status.face.Length)
        { 
            this.transform.GetChild(0).GetComponent<Image>().sprite = character.status.face[faceNum];
        }
        else
        { 
            this.transform.GetChild(0).GetComponent<Image>().sprite = GameManager.instance.GetDefaultFace();
        }

        if(character.characterStatusEffect[StatusEffectDatabaseManager.instance.GetIsDeadEffectNum()].isOn == true)
        { 
            this.transform.GetChild(0).GetComponent<Image>().GetComponent<Image>().color = isDeadColor;
        }
    }
}
