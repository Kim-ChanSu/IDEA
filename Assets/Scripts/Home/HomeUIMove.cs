using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeUIMove : MonoBehaviour
{
    private Vector3 UIPosition;
    private float UIEffectTime = 0.0f;
    [SerializeField]
    private float UIMoveTime = 0.75f;
    [SerializeField]
    private Vector3 UIMoveRange = Vector3.zero;
    private bool UIMoveMode = false;

    void Start()
    {
        this.UIPosition = this.gameObject.GetComponent<RectTransform>().anchoredPosition;
        this.UIEffectTime = 0.0f;
    }

    void Update()
    {
        UIMoveEffect();
    }

    private void UIMoveEffect()
    {
        #region
        UIEffectTime += Time.deltaTime / UIMoveTime;

        if (UIMoveMode == false)
        {
            float moveXPosition = Mathf.Lerp(this.UIPosition.x, this.UIPosition.x + this.UIMoveRange.x, UIEffectTime);
            float moveYPosition = Mathf.Lerp(this.UIPosition.y, this.UIPosition.y + this.UIMoveRange.y, UIEffectTime);
            this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(moveXPosition, moveYPosition, this.UIPosition.z);

            if (UIEffectTime >= 1)
            {
                UIEffectTime = 0;
                UIMoveMode = true;
            }
        }
        else
        {
            float moveXPosition = Mathf.Lerp(this.UIPosition.x + this.UIMoveRange.x, this.UIPosition.x, UIEffectTime);
            float moveYPosition = Mathf.Lerp(this.UIPosition.y + this.UIMoveRange.y, this.UIPosition.y, UIEffectTime);
            this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(moveXPosition, moveYPosition, this.UIPosition.z);

            if (UIEffectTime >= 1)
            {
                UIEffectTime = 0;
                UIMoveMode = false;
            }
        }
        #endregion
    }

  
}
