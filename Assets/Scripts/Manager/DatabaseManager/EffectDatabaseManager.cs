using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDatabaseManager : MonoBehaviour
{
    public static EffectDatabaseManager instance; //싱글톤

    void Awake() 
    {
        // 싱글톤
        #region
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("씬에 EffectDatabaseManager가 2개이상 존재합니다.");
            Destroy(gameObject);
        }
        #endregion
    }

    [SerializeField]
    private Effect[] effect;

    private int shootBallInterruptSuccessEffectNum = 1;
    private int boundBallSuccessEffectNum = 1;
    private int safeEffectNum = 1;

    public Effect GetEffect(int num)
    { 
        if((num < effect.Length) && (num >= 0))
        { 
            return effect[num];
        }
        else
        { 
            Debug.LogWarning("GetEffect에 잘못 된 값이 들어왔습니다!");
            return effect[0];
        }
    }

    public Effect GetShootBallInterruptSuccessEffect()
    { 
        if((shootBallInterruptSuccessEffectNum < effect.Length) && (shootBallInterruptSuccessEffectNum >= 0))
        { 
            return effect[shootBallInterruptSuccessEffectNum];
        }
        else
        { 
            Debug.LogWarning("shootBallInterruptSuccessEffectNum변수에 잘못 된 값이 들어 있습니다!");
            return effect[0];
        }        
    }

    public Effect GetBoundBallSuccessEffectNumEffect()
    { 
        if((boundBallSuccessEffectNum < effect.Length) && (boundBallSuccessEffectNum >= 0))
        { 
            return effect[boundBallSuccessEffectNum];
        }
        else
        { 
            Debug.LogWarning("boundBallSuccessEffectNum변수에 잘못 된 값이 들어 있습니다!");
            return effect[0];
        }        
    }

    public Effect GetSafeEffect()
    { 
        if((safeEffectNum < effect.Length) && (safeEffectNum >= 0))
        { 
            return effect[safeEffectNum];
        }
        else
        { 
            Debug.LogWarning("safeEffectNum변수에 잘못 된 값이 들어 있습니다!");
            return effect[0];
        }        
    }

    /*
    private Effect DeepCopyEffect(Effect copyEffect) 뭔가 값이 잘못 덮어쓰인거 같아서 적었는데 잘못본건가
    { 
        Effect newEffect = new Effect();
        newEffect.effectName = copyEffect.effectName;
        newEffect.isEffectCharacterUp = copyEffect.isEffectCharacterUp;
        newEffect.effect = copyEffect.effect;
        newEffect.effectSE = copyEffect.effectSE;
        return newEffect;
    }
    */
}
