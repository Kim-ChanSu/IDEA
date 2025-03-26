using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDatabaseManager : MonoBehaviour
{
    public static EffectDatabaseManager instance; //�̱���

    void Awake() 
    {
        // �̱���
        #region
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("���� EffectDatabaseManager�� 2���̻� �����մϴ�.");
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
            Debug.LogWarning("GetEffect�� �߸� �� ���� ���Խ��ϴ�!");
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
            Debug.LogWarning("shootBallInterruptSuccessEffectNum������ �߸� �� ���� ��� �ֽ��ϴ�!");
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
            Debug.LogWarning("boundBallSuccessEffectNum������ �߸� �� ���� ��� �ֽ��ϴ�!");
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
            Debug.LogWarning("safeEffectNum������ �߸� �� ���� ��� �ֽ��ϴ�!");
            return effect[0];
        }        
    }

    /*
    private Effect DeepCopyEffect(Effect copyEffect) ���� ���� �߸� ����ΰ� ���Ƽ� �����µ� �߸����ǰ�
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
