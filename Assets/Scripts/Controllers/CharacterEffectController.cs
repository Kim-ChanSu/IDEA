using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectController : MonoBehaviour
{
    public void EffectEnd()
    { 
        if(this.transform.parent.GetComponent<Character>() == true)
        { 
            this.transform.parent.GetComponent<Character>().EffectEnd();
        }     
        else
        { 
            Debug.LogWarning("CharacterEffectController��ũ��Ʈ EffectEnd �Լ��� ����Ǿ����� �θ� ������Ʈ�� Character��ũ��Ʈ�� ������ ���� �ʽ��ϴ�!");    
        }
    }
    
}
