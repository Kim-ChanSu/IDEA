using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterStatusButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private int characterNum;
    private Sprite[] characterFace;
    private bool isEnemy;

    private bool isSelectOrDeselect = false;
    private int originalCharacterFaceNum = 0;
    private int mouseOnCharacterFaceNum = 1;
    private int selectCharacterFaceNum = 3;


    public void SetCharacterStatusButton(int num)
    { 
        if((num < GameManager.instance.PlayerCharacter.Length) && (num >= 0))
        { 
            characterNum = num;
            characterFace = new Sprite[GameManager.instance.PlayerCharacter[num].face.Length];
            for(int i = 0; i < GameManager.instance.PlayerCharacter[num].face.Length; i++)
            { 
                characterFace[i] = GameManager.instance.PlayerCharacter[num].face[i];
            }    
            ChangeFace(0);
        }
        else
        { 
            Debug.LogWarning("�߸� �� ���� ���� CharacterStatusButton�� �����Ǿ����ϴ�!");
            Destroy(this.gameObject);
        }
    }

    public void SetPlayerStatus()
    { 
        GameManager.instance.homeManager.SetPlayerStatus(characterNum);
    }

    public void SetFace(int mode)
    { 
        #region
        //0 select, 1 deselect, 2 pointerenter, 3 pointerexit
        //���� bool�� �����Ϸ��� bool�ΰ��� �������ؼ� int�� ������..

        if(mode == 0)
        { 
            //������ ���
            isSelectOrDeselect = true;
            ChangeFace(selectCharacterFaceNum);
        }     
        
        if(mode == 1)
        { 
            //�������� �� ���
            isSelectOrDeselect = false;
            ChangeFace(originalCharacterFaceNum);
        }    

        if(mode == 2)
        { 
            //���콺�� �ø� ���
            if(isSelectOrDeselect == false)
            { 
                ChangeFace(mouseOnCharacterFaceNum);
            }
        }  

        if(mode == 3)
        { 
            //���콺�� ������ ���� ���
            if(isSelectOrDeselect == false)
            { 
                ChangeFace(originalCharacterFaceNum);         
            }
        } 
        #endregion
    }

    private void ChangeFace(int num)
    { 
        if(num < characterFace.Length && (num >= 0)) 
        { 
            this.gameObject.transform.GetChild(0).transform.GetComponent<Image>().sprite = characterFace[num];
        }        
        else
        { 
            this.gameObject.transform.GetChild(0).transform.GetComponent<Image>().sprite = characterFace[0];
        }
    }

    //-- ��Ʋ����
    public void SetBattleCharacterStatusButton(bool targetIsEnemy, int num)
    { 
        isEnemy = targetIsEnemy;
        characterNum = num;
        CharacterStatus characterStatus = new CharacterStatus();

        if(isEnemy == false)
        { 
            if((num < GameManager.instance.battleManager.playerCharacters.Length) && (num >= 0))
            { 
                characterStatus = GameManager.instance.battleManager.playerCharacters[characterNum].GetComponent<Character>().status;
            }
            else
            { 
                Debug.LogWarning("�߸� �� ���� ���� CharacterStatusButton�� �����Ǿ����ϴ�!");
                Destroy(this.gameObject);
            }
        }
        else
        { 
            if((num < GameManager.instance.battleManager.enemyCharacters.Length) && (num >= 0))
            { 
                characterStatus = GameManager.instance.battleManager.enemyCharacters[characterNum].GetComponent<Character>().status;
            }
            else
            { 
                Debug.LogWarning("�߸� �� ���� ���� CharacterStatusButton�� �����Ǿ����ϴ�!");
                Destroy(this.gameObject);
            }            
        }
        
        characterFace = new Sprite[characterStatus.face.Length];
        for(int i = 0; i < characterStatus.face.Length; i++)
        { 
            characterFace[i] = characterStatus.face[i];
        }    
        ChangeFace(0);

    }

    public void SetBattleCharacterStatus()
    { 
        GameManager.instance.battleManager.battleGetInformationManager.SetCharacterStatus(isEnemy, characterNum);
    }

    public void OnPointerEnter(PointerEventData eventData)
    { 
        ChangeFace(3);
    }

    public void OnPointerExit(PointerEventData eventData)
    { 
        ChangeFace(1);
    }

    public void OnSelect(BaseEventData eventData)
    { 
        ChangeFace(0);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ChangeFace(2);
    }
}
