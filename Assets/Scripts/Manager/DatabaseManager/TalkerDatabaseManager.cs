/* �ۼ���¥: 2023-11-06
 * ����: 0.0.1ver 
 * ����: ��ȭ ĳ���͵��� �����ϴ� ����
 * �ֱ� ���� ��¥: 2023-11-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkerDatabaseManager : MonoBehaviour
{

    public static TalkerDatabaseManager instance; //�̱���

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
            Debug.LogWarning("���� TalkerDatabaseManager�� 2���̻� �����մϴ�.");
            Destroy(gameObject);
        }
        #endregion
    }

    [SerializeField]
    private Talker[] talker;

    public Talker TalkerDB(int i)
    {       
        return talker[i];
    }

    public int DBLength()
    {         
        return talker.Length;
    }

}
