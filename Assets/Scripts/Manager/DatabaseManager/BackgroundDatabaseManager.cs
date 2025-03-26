using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameBackground //��� Ŭ����
{
    public string backgroundName; // ����̸�
    public Sprite backgroundImage; //����̹���
    public RuntimeAnimatorController backgroundAnimator;
}

[System.Serializable]
public class GameCutscene 
{
    public string cutsceneName; // ����̸�
    public Sprite cutsceneImage; //����̹���
}

public class BackgroundDatabaseManager : MonoBehaviour
{
    public static BackgroundDatabaseManager instance; //�̱���

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
            Debug.LogWarning("���� ���ӸŴ����� 2���̻� �����մϴ�.");
            Destroy(gameObject);
        }
        #endregion
    }

    [SerializeField]
    private GameBackground[] background; 
    [SerializeField]
    private GameCutscene[] cutscene; 

    public Sprite GetBackgroundByName(string name) // �̸�(backgroundName)�� �̿��ؼ� �̹��� ��������
    { 
        #region
        int check = 0;
        for(check = 0; check <= background.Length; check++)
        {
            if(check < background.Length)
            { 
                if(background[check].backgroundName  == name)
                {                 
                    break;
                }
            }
        }
        if(check < background.Length)
        {
            //GameManager.instance.background = background[check].backgroundName; ���� �۾������� ���̺�ε������ ������ �����ٲ�� �����°����� ��������� �ּ�ó��
            return background[check].backgroundImage;
        }
        else
        { 
            Debug.LogWarning("��ġ�ϴ� ����� �����ϴ�!");
            return background[0].backgroundImage;
        }
        #endregion
    }

    public Sprite GetBackground(int num) // �迭 ��ȣ�� �̿��Ͽ� �̹��� ��������
    {
        #region
        if(num < background.Length)
        {
            //GameManager.instance.Background = background[num].backgroundName; ���� �۾������� ���̺�ε������ ������ �����ٲ�� �����°����� ��������� �ּ�ó��
            return background[num].backgroundImage;
        }
        else
        {
            Debug.LogWarning("BackgroundDatabaseManager�� GetBackGround�Լ��� �߸��� ���� ���Խ��ϴ�.");
            return background[0].backgroundImage;
        }
        #endregion
    }

    public GameBackground GetBackgroundWithAnimationByName(string name)
    {
        #region
        int check = 0;
        for(check = 0; check <= background.Length; check++)
        {
            if(check < background.Length)
            { 
                if(background[check].backgroundName  == name)
                {                 
                    break;
                }
            }
        }
        if(check < background.Length)
        {
            return background[check];
        }
        else
        { 
            Debug.LogWarning("��ġ�ϴ� ����� �����ϴ�!");
            return background[0];
        }
        #endregion
    }

    public GameBackground GetBackgroundWithAnimation(int num)
    {
        #region
        if((0 <= num) && (num < background.Length))
        {
            return background[num];
        }
        else
        {
            Debug.LogWarning("BackgroundDatabaseManager�� GetBackGround�Լ��� �߸��� ���� ���Խ��ϴ�.");
            return background[0];
        }
        #endregion
    }

    public int DBLength() // �迭 ���� ��������
    {         
        return background.Length;
    }

    public Sprite GetCutsceneByName(string name)
    {
        #region
        int check = 0;
        for(check = 0; check <= cutscene.Length; check++)
        {
            if(check < cutscene.Length)
            { 
                if(cutscene[check].cutsceneName  == name)
                {                 
                    break;
                }
            }
        }

        if(check < cutscene.Length)
        {
            return cutscene[check].cutsceneImage;
        }
        else
        { 
            Debug.LogWarning("��ġ�ϴ� ����� �����ϴ�!");
            return cutscene[0].cutsceneImage;
        }
        #endregion
    }
}
