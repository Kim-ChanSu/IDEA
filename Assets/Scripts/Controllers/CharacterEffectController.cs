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
            Debug.LogWarning("CharacterEffectController스크립트 EffectEnd 함수가 실행되었으나 부모 오브젝트가 Character스크립트를 가지고 있지 않습니다!");    
        }
    }
    
}
