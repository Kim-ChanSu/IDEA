/* �ۼ���¥: 2023-09-12
 * ����: 0.0.1ver 
 * ����: ��ݵ��� �����ϴ� ����
 * �ֱ� ���� ��¥: 2023-09-12
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMDatabaseManager : MonoBehaviour
{
    public static BGMDatabaseManager instance; //�̱���

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
            Debug.LogWarning("���� BGMDatabaseManager�� 2���̻� �����մϴ�.");
            Destroy(gameObject);
        }
        #endregion
    }

    [System.Serializable]
    private class GameBGM
    {
        public string BGMName;
        public AudioClip BGM;
    }

    [SerializeField]
    private GameBGM[] BGM;

    public AudioClip GetBGMByName(string name) // �̸� (BGMName)���� ������� ��������
    { 
        #region
        int check = 0;
        for(check = 0; check <= BGM.Length; check++)
        {
            if(check < BGM.Length)
            { 
                if(BGM[check].BGMName  == name)
                {                 
                    break;
                }
            }
        }
        if(check < BGM.Length)
        {
            //GameManager.instance.BGM = BGM[check].BGMName; ���� �۾������� ���̺�ε������ ������ �����ٲ�� �����°����� ��������� �ּ�ó��
            return BGM[check].BGM;
        }
        else
        { 
            Debug.LogWarning("��ġ�ϴ� ����� �����ϴ�!");
            return BGM[0].BGM;
        }
        #endregion
    }


    public AudioClip GetBGM(int i) // �迭��ȣ�� ������� ��������
    {
        #region
        if(i < BGM.Length)
        {
            //GameManager.instance.BGM = BGM[i].BGMName; ���� �۾������� ���̺�ε������ ������ �����ٲ�� �����°����� ��������� �ּ�ó��
            return BGM[i].BGM;
        }
        else
        {
            Debug.LogWarning("BGMDatabaseManager�� GetBGM�Լ��� �߸��� ���� ���Խ��ϴ�.");
            return BGM[0].BGM;
        }
        #endregion
    }

    public int DBLength()
    {         
        return BGM.Length;
    }

}
