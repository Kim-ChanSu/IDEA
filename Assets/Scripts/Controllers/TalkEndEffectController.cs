using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkEndEffectController : MonoBehaviour
{
    private bool isInit = false;
    private Vector3 defaultPosition = new Vector3 (0.0f, 0.0f, 0.0f);
    private float effectTime = 0.0f;
    private float moveTime = 0.5f;
    private float effectMovePos = 10.0f;
    private bool isDownMoveEnd = false;

    private void Awake()
    {
        if (isInit == false)
        {
            InitTalkEndEffect();
        }
    }

    private void OnEnable()
    {
        if (isInit == false)
        {
            InitTalkEndEffect();
        }
        ClearPosition();
    }

    private void OnDisable()
    {
        if (isInit == true)
        {
            ClearPosition();
        }       
    }

    private void Update()
    {
        TalkEndEffect();
    }

    private void InitTalkEndEffect()
    {
        defaultPosition = this.gameObject.GetComponent<RectTransform>().anchoredPosition;
        this.gameObject.SetActive(false);
        effectTime = 0.0f;
        isDownMoveEnd = false;
        isInit = true;
    }

    private void ClearPosition()
    {
        this.gameObject.GetComponent<RectTransform>().anchoredPosition = defaultPosition;
        effectTime = 0.0f;
        isDownMoveEnd = false;
    }

    private void TalkEndEffect()
    {
        if (isInit == true)
        {
            effectTime = effectTime + Time.deltaTime / moveTime;
            if (isDownMoveEnd == false)
            {
                if (effectTime <= 1)
                {
                    float movePos = Mathf.Lerp(defaultPosition.y, defaultPosition.y - effectMovePos, effectTime);
                    this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3 (defaultPosition.x,movePos ,defaultPosition.z);
                }
                else
                {
                    effectTime = 0;
                    isDownMoveEnd = true;
                }
            }
            else
            {
                if (effectTime <= 1)
                {
                    float movePos = Mathf.Lerp(defaultPosition.y - effectMovePos, defaultPosition.y, effectTime);
                    this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3 (defaultPosition.x,movePos ,defaultPosition.z);
                }
                else
                {
                    effectTime = 0;
                    isDownMoveEnd = false;
                }
            }
        }
    }
}
