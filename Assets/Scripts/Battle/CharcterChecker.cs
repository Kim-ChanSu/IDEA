/* 작성날짜: 
 * 버전: 0.0.1ver 
 * 내용: 캐릭터z위치값 체크 
 * 최근 수정 날짜: 2023-10-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharcterChecker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    { 
        if((other.transform.tag == "Character") && (other.transform.gameObject != this.gameObject.transform.parent.gameObject))
        { 
            ChangeZpos((int)other.transform.position.z - 1);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    { 
        if(other.transform.tag == "Character")
        { 
            this.gameObject.transform.parent.GetComponent<Character>().ResetZPosition();
        }
    }

    private void CheckUnderCharacter()
    { 
        Vector2 worldPoint = new Vector2 ((int)transform.position.x, (int)transform.position.y - 1);
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);
        for(int i = 0; i< hits.Length; i++)
        { 
            if ((hits[i].transform.tag == "Character" ) && (hits[i].transform.gameObject != this.gameObject.transform.parent.gameObject))
            { 
                Debug.Log(this.gameObject + "가 밑에 캐릭터 발견! : " + hits[i].transform.gameObject);
                hits[i].transform.GetChild(0).transform.GetComponent<CharcterChecker>().ChangeZpos((int)this.gameObject.transform.position.z -1);                
            }    
        }
    }

    public void ChangeZpos(int zPos)
    { 
        this.gameObject.transform.parent.GetComponent<Character>().ChangeZPosition(zPos);
        Debug.Log(this.gameObject.transform.parent + " 의 z값 변경 z값은 " + zPos);
        CheckUnderCharacter();        
    }
}
