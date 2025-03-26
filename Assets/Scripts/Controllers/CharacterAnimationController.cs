using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private float filpXScale = -1.0f;
    private float OriginalXScale = 1.0f;
    public float setYPosition = -1.0f;

    public void SetCharacterAnimationController()
    { 
        if(this.transform.parent.GetComponent<Character>() == true)
        {             
            this.transform.parent.GetComponent<Character>().SetCharacterAnimationController(this);
            this.transform.position = new Vector3(this.transform.parent.transform.position.x, this.transform.parent.transform.position.y + setYPosition, this.transform.parent.transform.position.z);
            ChangeAnimationFlip(this.transform.parent.GetComponent<SpriteRenderer>().flipX);
        }     
        else
        { 
            Debug.LogWarning("CharacterAnimationController��ũ��Ʈ SetCharacterAnimationController �Լ��� ����Ǿ����� �θ� ������Ʈ�� Character��ũ��Ʈ�� ������ ���� �ʽ��ϴ�!");    
        }        
    }


    public void ChangeAnimationFlip(bool mode)
    {
        if (mode == true)
        {
            this.transform.localScale = new Vector3(filpXScale, this.transform.localScale.y, this.transform.localScale.z);
        }
        else
        { 
            this.transform.localScale = new Vector3(OriginalXScale, this.transform.localScale.y, this.transform.localScale.z);
        }
    }

    public float GetXScale()
    { 
        return this.transform.localScale.x;
    }

    public void SetThisGameObjectActive(bool mode)
    {
        this.gameObject.SetActive(mode); // color = new Color(100/255f, 100/255f, 100/255f);
    }


    public void SetMoveAnimation(bool mode)
    { 
        this.gameObject.GetComponent<Animator>().SetBool("move", mode);    
    }

    public void SetThrowAnimation(bool mode)
    { 
        this.gameObject.GetComponent<Animator>().SetBool("throw", mode);
    }

    public void ThrowEnd()
    { 
        SetThrowAnimation(false);
        if ((GameManager.instance != null) && (GameManager.instance.battleManager != null))
        {
            GameManager.instance.battleManager.skillUseAnimationEnd();
        }
    }
}
