using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleCommandButtonEffectController : MonoBehaviour
{
    private float selectEffectTime = 0.5f;
    private float deselectEffectTime = 0.5f; 
    private float effectTime = 0.0f;

    private float buttonDefaultXPosition = 435.0f;
    private float selectEffectXPosition = 365.0f;
    private float nowXPosition = 0.0f;
    //private float spriteXLength = 0.0f;

    private bool isActive = false;
    private bool eventMode = false;
    
    
    private void Start()
    { 
        InitializeBattleCommandButtonEffectController();
    }

    private void OnEnable()
    { 
        isActive = true;
    }

    private void OnDisable()
    { 
        isActive = false;
        ButtonEventClear();
    }

    private void InitializeBattleCommandButtonEffectController()
    {   
        #region
        /*
        if(this.GetComponent<Image>().sprite != null)
        {
            spriteXLength = this.GetComponent<Image>().sprite.bounds.size.x;
        }
        */
        //ResetButtonPosition();
        #endregion
    }

    /*
    public void ResetButtonPosition()
    {
        buttonPosition = new Vector2 (this.GetComponent<RectTransform>().anchoredPosition.x, this.GetComponent<RectTransform>().anchoredPosition.y);
        nowXPosition = this.GetComponent<RectTransform>().anchoredPosition.x;
    }
    */

    private void ChangeThisPosition(float targetXPosition)
    { 
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2 (targetXPosition, this.GetComponent<RectTransform>().anchoredPosition.y);
    }

    public void ButtonEventClear()
    { 
        ChangeThisPosition(buttonDefaultXPosition);
    }
 
    public void ButtonPointerEnterEvent()
    { 
        if (GameManager.instance.battleManager.nowPhase == BattlePhase.Stay)
        {
            return;
        }
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(this.gameObject, new BaseEventData(eventSystem));         
    }

    public void ButtonPointerExitEvent()
    { 
        if (GameManager.instance.battleManager.nowPhase == BattlePhase.Stay)
        {
            return;
        }
        if(EventSystem.current.currentSelectedGameObject == this.gameObject)
        { 
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(null, new BaseEventData(eventSystem)); 
        }        
    }

    public void ButtonSelectEvent()
    { 
        if (GameManager.instance.battleManager.nowPhase == BattlePhase.Stay)
        {
            return;
        }
        eventMode = true;
        effectTime = 0;
        nowXPosition = this.GetComponent<RectTransform>().anchoredPosition.x;
        StartCoroutine(SelectEffect());
    }

    public void ButtonDeselectEvent()
    { 
        if (GameManager.instance.battleManager.nowPhase == BattlePhase.Stay)
        {
            return;
        }
        eventMode = false;
        effectTime = 0;
        nowXPosition = this.GetComponent<RectTransform>().anchoredPosition.x;
        StartCoroutine(DeselectEffect()); 
    }

    IEnumerator SelectEffect()
    { 
        if(isActive == true)
        { 
            if((eventMode == true) && (effectTime < selectEffectTime))
            { 
                effectTime += Time.deltaTime/selectEffectTime;

                ChangeThisPosition(Mathf.Lerp(nowXPosition, selectEffectXPosition, effectTime));

                yield return null;
                StartCoroutine(SelectEffect());  
            }
            else if(effectTime >= selectEffectTime)
            { 
                ChangeThisPosition(selectEffectXPosition);
            }
        }
    }

    IEnumerator DeselectEffect()
    { 
        if(isActive == true)
        { 
            if((eventMode == false) && (effectTime < deselectEffectTime))
            { 
                effectTime += Time.deltaTime/deselectEffectTime;

                ChangeThisPosition(Mathf.Lerp(nowXPosition, buttonDefaultXPosition ,effectTime));

                yield return null;
                StartCoroutine(DeselectEffect());  
            }
            else if(effectTime >= deselectEffectTime)
            { 
                ChangeThisPosition(buttonDefaultXPosition);
            }
        }
    }
}
