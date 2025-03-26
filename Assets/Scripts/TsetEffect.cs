using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TsetEffect : MonoBehaviour
{
    [SerializeField]
    private Image hpImage;
    [SerializeField]
    private Image heartImage;
    public bool isAddA;

    void Update()
    {
        SetEffect();
        SetColorA();
    }
  
    private void SetEffect()
    {
        if(this.hpImage.fillAmount <= 0.25f)
        {
            
            if(isAddA == false)
            {
                this.heartImage.color = new Color(1f, 1f, 1f, heartImage.color.a - Time.deltaTime);
            }
            else
            {
                this.heartImage.color = new Color(1f, 1f, 1f, heartImage.color.a + Time.deltaTime);
            }
        }
    }

    private void SetColorA()
    {
        if (this.heartImage.color.a >= 1f)
        {
            isAddA = false;
        }

        if (this.heartImage.color.a <= 0.5f)
        {
            isAddA = true;
        }

    }

}
