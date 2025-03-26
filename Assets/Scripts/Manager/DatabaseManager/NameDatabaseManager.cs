using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameDatabaseManager : MonoBehaviour
{
    public static NameDatabaseManager instance; //�̱���

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
            Debug.LogWarning("���� NameDatabaseManager�� 2���̻� �����մϴ�.");
            Destroy(gameObject);
        }
        #endregion
    }

    public const string levelName = "LV";
    public const string EXPName = "����ġ";
    public const string HPName = "HP";
    public const string MPName = "MP";
    public const string SympathyName = "SP";
    public const string ATKName = "���ݷ�";
    public const string MAKName = "������";
    public const string DEFName = "����";
    public const string MDFName = "���׷�";
    public const string rangeName = "��Ÿ�";
    public const string moveName = "�̵���";
    public const string goldName = "���";

    public const string SympathyTypeNoneName = "�⺻";
    public const string SympathyTypeRationalName = "����";
    public const string SympathyTypeAngerName = "�г�";
    public const string SympathyTypeEnjoyName = "����";
}
