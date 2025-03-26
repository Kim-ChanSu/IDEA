using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOnBattleLogoChecker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    { 
        GameManager.instance.isMouseOnBattleLog = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    { 
        GameManager.instance.isMouseOnBattleLog = false;
    }
}
