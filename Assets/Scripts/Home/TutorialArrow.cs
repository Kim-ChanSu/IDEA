using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialArrow : MonoBehaviour
{
    private Vector3 thisPosition;
    private float arrowEffectTime = 0.0f;
    private float arrowMoveTime = 0.75f;
    private float arrowMoveRange = 20.0f;
    private bool arrowMoveMode = false;

    void Start()
    {
        thisPosition = this.gameObject.GetComponent<RectTransform>().anchoredPosition;
        arrowEffectTime = 0.0f;
    }

    void Update()
    {
        ArrowMoveEffect();
    }

    private void ArrowMoveEffect()
    { 
        #region
        arrowEffectTime += Time.deltaTime/arrowMoveTime;

        if(arrowMoveMode == false)
        {            
            float moveYPosition = Mathf.Lerp(thisPosition.y, thisPosition.y + arrowMoveRange, arrowEffectTime);
            this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(thisPosition.x ,moveYPosition , thisPosition.z);
            
            if(arrowEffectTime >= 1)
            { 
                arrowEffectTime = 0;
                arrowMoveMode = true;
            }
        }
        else
        { 
            float moveYPosition = Mathf.Lerp(thisPosition.y + arrowMoveRange, thisPosition.y, arrowEffectTime);
            this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(thisPosition.x ,moveYPosition , thisPosition.z);     
            
            if(arrowEffectTime >= 1)
            { 
                arrowEffectTime = 0;
                arrowMoveMode = false;
            }
        }  
        #endregion
    }

    public void SetTutorialArrowPosition(Vector3 newPos, bool mode = true)
    {
        thisPosition = newPos;
        this.gameObject.GetComponent<RectTransform>().anchoredPosition = thisPosition;
        if (mode == true)
        {
            this.gameObject.transform.localScale = new Vector3 (1, -1, 1);
        }
        else
        {
            this.gameObject.transform.localScale = new Vector3 (1, 1, 1);
        }
        this.gameObject.SetActive(true);
    }
}
