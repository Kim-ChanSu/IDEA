using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEDatabaseManager : MonoBehaviour
{
    public static SEDatabaseManager instance; //�̱���

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
            Debug.LogWarning("���� SEDatabaseManager 2���̻� �����մϴ�.");
            Destroy(gameObject);
        }
        #endregion
    }

    [System.Serializable]
    private class GameSE
    {
        public string SEName;
        public AudioClip SE;
    }

    [SerializeField]
    private GameSE[] SE;

    public AudioClip GetSEByName(string name)
    { 
        #region
        int check = 0;
        for(check = 0; check <= SE.Length; check++)
        {
            if(check < SE.Length)
            { 
                if(SE[check].SEName  == name)
                {                 
                    break;
                }
            }
        }
        if(check < SE.Length)
        {
            return SE[check].SE;
        }
        else
        { 
            Debug.LogWarning("��ġ�ϴ� SE�� �����ϴ�!");
            return SE[0].SE;
        }
        #endregion
    }


    public AudioClip GetSE(int i)
    {
        #region
        if(i < SE.Length)
        {
            return SE[i].SE;
        }
        else
        {
            Debug.LogWarning("SEDatabaseManager�� GetSE�Լ��� �߸��� ���� ���Խ��ϴ�.");
            return SE[0].SE;
        }
        #endregion
    }

    public int DBLength()
    {         
        return SE.Length;
    }

}
