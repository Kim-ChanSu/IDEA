/* �ۼ���¥: 2023-11-26
 * ����: 0.0.1ver 
 * ����: �����̻���� �����ϴ� ����
 * �ֱ� ���� ��¥: 2023-11-26
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStatusEffect
{ 
    public bool isOn;
    public int statusEffectTurnCount;
}

[System.Serializable]
public class SympathyEffect
{ 
    public float ATKChange;
    public float MAKChange;
    public float DEFChange;
    public float MDFChange;
    public int moveChange;
    public int rangeChange;
}


public class StatusEffectDatabaseManager : MonoBehaviour
{
    public static StatusEffectDatabaseManager instance; //�̱���

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
            Debug.LogWarning("���� StatusEffectDatabaseManager�� 2���̻� �����մϴ�.");
            Destroy(gameObject);
        }
        #endregion
    }

    [SerializeField]
    private int isDeadEffectNum = 0;
    [SerializeField]
    private int playerAngerSympathyEffectNum = 1;
    [SerializeField]
    private int playerEnjoySympathyEffectNum = 2;
    [SerializeField]
    private int enemyAngerSympathyEffectNum = 3;
    [SerializeField]
    private int enemyEnjoySympathyEffectNum = 4;
    [SerializeField]
    private int playerStrategicPointEffectNum = 20; //�����̻� Ű�°� Character��ũ��Ʈ���� �ϰ���
    [SerializeField]
    private int enemyStrategicPointEffectNum = 21;
    [SerializeField]
    private int[] systemStatusEffectNum;
    [SerializeField]
    private int[] cantDispelBySympathyStatusEffectNum;

    [SerializeField]
    private SympathyEffect[] angerSympathyEffect;
    [SerializeField]
    private SympathyEffect[] enjoySympathyEffect;
    [SerializeField]
    private SympathyEffect strategicPointEffect;

    [SerializeField]
    private StatusEffect[] statusEffect;

    [SerializeField]
    private float minCheckStatusChange;

    [SerializeField]
    private int karlSpecialStatusEffectNum = 18;

    public int GetIsDeadEffectNum()
    { 
        return isDeadEffectNum;
    }

    public int GetPlayerAngerSympathyEffectNum()
    { 
        return playerAngerSympathyEffectNum;
    }

    public int GetPlayerEnjoySympathyEffectNum()
    { 
        return playerEnjoySympathyEffectNum;
    }
    
    public int GetEnemyAngerSympathyEffectNum()
    { 
        return enemyAngerSympathyEffectNum;
    }

    public int GetEnemyEnjoySympathyEffectNum()
    { 
        return enemyEnjoySympathyEffectNum;
    }

    public int GetPlayerStrategicPointEffectNum()
    {
        return playerStrategicPointEffectNum;
    }

    public int GetEnemyStrategicPointEffectNum()
    {
        return enemyStrategicPointEffectNum;
    }

    public StatusEffect GetStatusEffect(int num)
    { 
        if(num < statusEffect.Length)
        { 
            return statusEffect[num];    
        }
        else
        { 
            Debug.LogWarning("StatusEffectDatabaseManager ��ũ��Ʈ�� GetStatusEffect�Լ��� �߸��� ���� ���Խ��ϴ�.");
            return statusEffect[0];    
        }
    }

    public int GetStatusEffectNum(StatusEffect checkStatusEffect)
    { 
        for(int i = 0; i < statusEffect.Length; i++)
        { 
            if(checkStatusEffect == statusEffect[i])
            { 
                return i;    
            }
        }

        Debug.LogWarning("StatusEffectDatabaseManager ��ũ��Ʈ�� GetStatusEffectNum�Լ��� �߸��� ���� ���Խ��ϴ�.");
        return playerEnjoySympathyEffectNum; //���Ŷ�� ������ �ٷ� �����⿡ ���� ������
    }

    public int GetStatusEffectLength()
    { 
        return statusEffect.Length;
    }

    public float GetMinCheckStatusChange()
    { 
        return minCheckStatusChange;    
    }

    public void SetAngerSympathyEffect(int characterCount, bool isEnemy)
    { 
        if(characterCount >= angerSympathyEffect.Length)
        { 
            characterCount = angerSympathyEffect.Length - 1;
        }

        if(isEnemy == true)
        { 
            SetEffect(GetEnemyAngerSympathyEffectNum(), angerSympathyEffect[characterCount]);
        }
        else
        { 
            SetEffect(GetPlayerAngerSympathyEffectNum(), angerSympathyEffect[characterCount]);
        }
    }

    public void SetEnjoySympathyEffect(int characterCount, bool isEnemy)
    { 
        if(characterCount >= enjoySympathyEffect.Length)
        { 
            characterCount = enjoySympathyEffect.Length - 1;
        }

        if(isEnemy == true)
        { 
            SetEffect(GetEnemyEnjoySympathyEffectNum(), enjoySympathyEffect[characterCount]);
        }
        else
        { 
            SetEffect(GetPlayerEnjoySympathyEffectNum(), enjoySympathyEffect[characterCount]);
        }        
    }

    private void SetEffect(int effectNum, SympathyEffect sympathyEffect)
    { 
        if(effectNum < statusEffect.Length)
        {             
            statusEffect[effectNum].ATKChange = sympathyEffect.ATKChange;
            statusEffect[effectNum].MAKChange = sympathyEffect.MAKChange;
            statusEffect[effectNum].DEFChange = sympathyEffect.DEFChange;
            statusEffect[effectNum].MDFChange = sympathyEffect.MDFChange;
            statusEffect[effectNum].moveChange = sympathyEffect.moveChange;
            statusEffect[effectNum].rangeChange = sympathyEffect.rangeChange;
        }
        else
        { 
            Debug.LogWarning("StatusEffectDatabaseManager ��ũ��Ʈ�� SetEffect�Լ��� �߸��� ���� ���Խ��ϴ�.");
        }
    }

    public StatusEffect GetKarlSpecialStatusEffect()
    { 
        return statusEffect[karlSpecialStatusEffectNum];
    }

    public int GetKarlSpecialStatusEffectNum()
    { 
        return karlSpecialStatusEffectNum;
    }

    public bool IsCanDispelBySympathy(int num) // ����ȿ���� ���� �����ʴ�  �����̻�
    { 
        if((num != isDeadEffectNum) && (num != playerAngerSympathyEffectNum) && (num != playerEnjoySympathyEffectNum) && (num != enemyAngerSympathyEffectNum) && (num != enemyEnjoySympathyEffectNum) && (num != playerStrategicPointEffectNum) && (num != enemyStrategicPointEffectNum) && (num >= 0) && (num < statusEffect.Length))
        {
            return true;           
        }
        else
        { 
            for (int i = 0; i < cantDispelBySympathyStatusEffectNum.Length; i++)
            {
                if(num == cantDispelBySympathyStatusEffectNum[i])
                { 
                    return false;
                }
            }

            return false;
        }
    }

    public bool IsSystemStatusEffect(int num) // ����â�� ǥ��ȵǴ� �����̻�
    { 
        if((num == isDeadEffectNum) || (num == playerAngerSympathyEffectNum) || (num == playerEnjoySympathyEffectNum) || (num == enemyAngerSympathyEffectNum) || (num == enemyEnjoySympathyEffectNum) || (num == playerStrategicPointEffectNum) || (num == enemyStrategicPointEffectNum))
        {
            return true;           
        }
        else
        { 
            for (int i = 0; i < systemStatusEffectNum.Length; i++)
            {
                if(num == systemStatusEffectNum[i])
                { 
                    return true;
                }
            }
            return false;
        }        
    }

    // �ý��ۿ�

    public int GetSystemSilent()
    {
        return 17;
    }

    public void ClearStrategicPointEffect()
    {
        statusEffect[playerStrategicPointEffectNum].ATKChange = 1.0f;
        statusEffect[playerStrategicPointEffectNum].MAKChange = 1.0f;
        statusEffect[playerStrategicPointEffectNum].DEFChange = 1.0f;
        statusEffect[playerStrategicPointEffectNum].MDFChange = 1.0f;
        statusEffect[playerStrategicPointEffectNum].moveChange = 0;
        statusEffect[playerStrategicPointEffectNum].rangeChange = 0;

        statusEffect[enemyStrategicPointEffectNum].ATKChange = 1.0f;
        statusEffect[enemyStrategicPointEffectNum].MAKChange = 1.0f;
        statusEffect[enemyStrategicPointEffectNum].DEFChange = 1.0f;
        statusEffect[enemyStrategicPointEffectNum].MDFChange = 1.0f;
        statusEffect[enemyStrategicPointEffectNum].moveChange = 0;
        statusEffect[enemyStrategicPointEffectNum].rangeChange = 0;
    }

    public void UpdateStrategicPointEffect(int playerStrategicPointNum, int enemyStrategicPointNum)
    {
        ClearStrategicPointEffect();

        statusEffect[playerStrategicPointEffectNum].ATKChange = statusEffect[playerStrategicPointEffectNum].ATKChange + (strategicPointEffect.ATKChange * playerStrategicPointNum);
        statusEffect[playerStrategicPointEffectNum].MAKChange = statusEffect[playerStrategicPointEffectNum].MAKChange + (strategicPointEffect.MAKChange * playerStrategicPointNum);
        statusEffect[playerStrategicPointEffectNum].DEFChange = statusEffect[playerStrategicPointEffectNum].DEFChange + (strategicPointEffect.DEFChange * playerStrategicPointNum);
        statusEffect[playerStrategicPointEffectNum].MDFChange = statusEffect[playerStrategicPointEffectNum].MDFChange + (strategicPointEffect.MDFChange * playerStrategicPointNum);
        statusEffect[playerStrategicPointEffectNum].moveChange = statusEffect[playerStrategicPointEffectNum].moveChange + (strategicPointEffect.moveChange * playerStrategicPointNum);
        statusEffect[playerStrategicPointEffectNum].rangeChange = statusEffect[playerStrategicPointEffectNum].rangeChange + (strategicPointEffect.rangeChange * playerStrategicPointNum);

        statusEffect[enemyStrategicPointEffectNum].ATKChange = statusEffect[enemyStrategicPointEffectNum].ATKChange + (strategicPointEffect.ATKChange * enemyStrategicPointNum);
        statusEffect[enemyStrategicPointEffectNum].MAKChange = statusEffect[enemyStrategicPointEffectNum].MAKChange + (strategicPointEffect.MAKChange * enemyStrategicPointNum);
        statusEffect[enemyStrategicPointEffectNum].DEFChange = statusEffect[enemyStrategicPointEffectNum].DEFChange + (strategicPointEffect.DEFChange * enemyStrategicPointNum);
        statusEffect[enemyStrategicPointEffectNum].MDFChange = statusEffect[enemyStrategicPointEffectNum].MDFChange + (strategicPointEffect.MDFChange * enemyStrategicPointNum);
        statusEffect[enemyStrategicPointEffectNum].moveChange = statusEffect[enemyStrategicPointEffectNum].moveChange + (strategicPointEffect.moveChange * enemyStrategicPointNum);
        statusEffect[enemyStrategicPointEffectNum].rangeChange = statusEffect[enemyStrategicPointEffectNum].rangeChange + (strategicPointEffect.rangeChange * enemyStrategicPointNum);
    }
}
