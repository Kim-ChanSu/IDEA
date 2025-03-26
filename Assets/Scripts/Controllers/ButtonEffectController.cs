using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEffectController : MonoBehaviour
{
    private bool isSelect;
    private bool effectMode;
    private float effectTime;
    public Sprite originalSprite;
    public Sprite selectSprite;
    public GameObject buttonObject;
    public GameObject effectObject;
    public float fadeTime;

    public void StartSelectEffect(bool select)
    { 
        if ((GameManager.instance == null) || (GameManager.instance.isOpeningEnd == false))
        {
            return;
        }

        //--
        if(select == false)
        { 
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(this.gameObject, new BaseEventData(eventSystem)); 
            return; 
        }
        //--

        if(select == true)
        { 
            isSelect = true;
        }    

        if((isSelect == true) && (select == false))
        { 
            return;    
        }        

        effectMode = true;

        buttonObject.GetComponent<Image>().sprite = selectSprite;
        ChangeObjectAlpha(buttonObject, 0);
        effectObject.GetComponent<Image>().sprite = originalSprite;
        ChangeObjectAlpha(effectObject, 1);

        effectTime = 0;
        StartCoroutine(SelectEffect());

    }

    public void StartDeselectEffect(bool deselect)
    { 
        if ((GameManager.instance == null) || (GameManager.instance.isOpeningEnd == false))
        {
            return;
        }

        //--
        if(deselect == false)
        { 
            if(EventSystem.current.currentSelectedGameObject == this.gameObject)
            { 
                var eventSystem = EventSystem.current;
                eventSystem.SetSelectedGameObject(null, new BaseEventData(eventSystem)); 
            }
            return; 
        }
        //--

        if((isSelect == true) && (deselect == false))
        { 
            return;    
        }
        
        if(deselect == true)
        { 
            isSelect = false;
        }

        effectMode = false;

        buttonObject.GetComponent<Image>().sprite = originalSprite;
        ChangeObjectAlpha(buttonObject, 0);
        effectObject.GetComponent<Image>().sprite = selectSprite;
        ChangeObjectAlpha(effectObject, 1);

        effectTime = 0;
        StartCoroutine(DeselectEffect());
    }

 
    IEnumerator SelectEffect()
    { 
        if((effectMode == true) && (effectTime < fadeTime))
        { 
            effectTime += Time.deltaTime/fadeTime;

            ChangeObjectAlpha(buttonObject, Mathf.Lerp(0.0f, 1.0f, effectTime));
            ChangeObjectAlpha(effectObject, Mathf.Lerp(1.0f, 0.0f, effectTime));
            yield return null;
            StartCoroutine(SelectEffect());  
        }
        else
        { 
            ChangeObjectAlpha(buttonObject, 1);
            ChangeObjectAlpha(effectObject, 0);
        }
    }

    IEnumerator DeselectEffect()
    { 
        if((effectMode == false) && (effectTime < fadeTime))
        { 
            effectTime += Time.deltaTime/fadeTime;

            ChangeObjectAlpha(buttonObject, Mathf.Lerp(0.0f, 1.0f, effectTime));
            ChangeObjectAlpha(effectObject, Mathf.Lerp(1.0f, 0.0f, effectTime));
            yield return null;
            StartCoroutine(DeselectEffect());  
        }
        else
        { 
            ChangeObjectAlpha(buttonObject, 1);
            ChangeObjectAlpha(effectObject, 0);
        }
    }


    private void ChangeObjectAlpha(GameObject alphaObject, float alpha)
    { 
        Color color = alphaObject.GetComponent<Image>().color;
        color.a = alpha;        
        alphaObject.GetComponent<Image>().color = color;
    }
}
