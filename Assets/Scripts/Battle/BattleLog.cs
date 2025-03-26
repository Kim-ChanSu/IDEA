using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleLog : MonoBehaviour
{
    [SerializeField] private GameObject characterFace;
    [SerializeField] private GameObject characterFaceMaskBackground;
    [SerializeField] private GameObject logText;


    public void SetBattleLog(Character logTargetCharacter, string log)
    {
        if (logTargetCharacter != null)
        {
            characterFaceMaskBackground.SetActive(true);
            int faceNum = (int)logTargetCharacter.sympathyType;

            if(logTargetCharacter.status.face.Length > faceNum) 
            { 
                characterFace.GetComponent<Image>().sprite = logTargetCharacter.status.face[faceNum];
            }
            else
            { 
                if(logTargetCharacter.status.face.Length > 0)
                { 
                    characterFace.GetComponent<Image>().sprite = logTargetCharacter.status.face[0];
                }
                else
                { 
                    characterFace.GetComponent<Image>().sprite = GameManager.instance.GetDefaultFace();
                }
            }

            characterFaceMaskBackground.GetComponent<Image>().sprite = GameManager.instance.battleManager.battleGetInformationManager.GetCharacterFaceMaskBackground(faceNum);
        }
        else
        {
            characterFaceMaskBackground.SetActive(false);
        }

        logText.GetComponent<Text>().text = log;

        this.gameObject.SetActive(true);
    }

    public void ResetBattleLog()
    {
        this.gameObject.SetActive(false);
    }
}
