using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDamageEffect : MonoBehaviour
{
    private float effectTime = 0.0f;
    private int sortingOrder;
    private Vector3 defaultPosition = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 effectPosition = new Vector3(0.0f, 2.0f, 0.0f);

    public string sortingLayerName = "Default";
    public string HPDamageColor = "<color=#FF0000>"; 
    public string HPHealColor = "<color=#00FF00>"; 
    public string MPDamageColor = "<color=#000000>";
    public string MPHealColor = "<color=#0000FF>";  

    public float damageEffectTime = 0.35f;

    void Awake()
    {
        InitializeCharacterDamageEffect();
    } 

    private void InitializeCharacterDamageEffect()
    { 
        sortingOrder = transform.parent.GetComponent<Character>().effectObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        ResetPosition();
    }

    private void ResetPosition()
    { 
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerName = sortingLayerName;
        meshRenderer.sortingOrder = sortingOrder;     
        
        this.transform.localPosition = defaultPosition;
    }

    public void SetCharacterDamageEffect(bool mode)
    {        
        this.gameObject.SetActive(mode);
        effectTime = 0;
    }

    public void SetDamageEffect(int damage, bool isHeal, DamageType damageType = DamageType.HP)
    { 
        ResetPosition();
        this.GetComponent<TextMesh>().text = damage + "";
        SetCharacterDamageEffect(true);

        if(damageType == DamageType.HP)
        { 
            if(isHeal == true)
            { 
                this.GetComponent<TextMesh>().text = HPHealColor  + this.GetComponent<TextMesh>().text  + "</color>";
            }
            else
            { 
                this.GetComponent<TextMesh>().text = HPDamageColor  + this.GetComponent<TextMesh>().text  + "</color>";
            }
        }
        else if(damageType == DamageType.MP)
        { 
            if(isHeal == true)
            { 
                this.GetComponent<TextMesh>().text = MPHealColor  + this.GetComponent<TextMesh>().text  + "</color>";
            }
            else
            { 
                this.GetComponent<TextMesh>().text = MPDamageColor  + this.GetComponent<TextMesh>().text  + "</color>";
            }           
        }    

        if (this.gameObject.activeInHierarchy == true)
        {
            StartCoroutine(DamageEffect());
        }     
    }

    private void EffectEnd()
    { 
        SetCharacterDamageEffect(false);
        ResetPosition();
        this.GetComponent<TextMesh>().text = "";        
    }

    IEnumerator DamageEffect()
    { 
        if((effectTime < 1) && (this.gameObject.activeInHierarchy == true))
        { 
            effectTime += Time.deltaTime/damageEffectTime;
            this.transform.localPosition = Vector3.Lerp(defaultPosition, effectPosition, effectTime);
            yield return null;         
            StartCoroutine(DamageEffect()); 
        }
        else
        { 
            EffectEnd();
        }          
    }
}
