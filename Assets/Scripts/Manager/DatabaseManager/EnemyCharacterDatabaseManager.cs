using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacterDatabaseManager : MonoBehaviour
{
    public static EnemyCharacterDatabaseManager instance; 

    void Awake() 
    {
        #region
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("���� EnemyCharacterDatabaseManager 2���̻� �����մϴ�.");
            Destroy(gameObject);
        }
        #endregion
    }

    void Start()
    { 
        SetEnemyCharacterSkill();
    }

    [SerializeField]
    private EnemyCharacterData[] enemyCharacter;

    private void SetEnemyCharacterSkill()
    { 
        #region
        for(int i = 0; i < enemyCharacter.Length; i++)
        { 
            enemyCharacter[i].enemyStatus.characterSkill = new bool[SkillDatabaseManager.instance.SkillLength()]; 

            for(int j = 0; j < enemyCharacter[i].enemyStatus.firstCharacterSkillNum.Length; j++)
            { 
                if(enemyCharacter[i].enemyStatus.firstCharacterSkillNum[j] < SkillDatabaseManager.instance.SkillLength() == true)
                { 
                    enemyCharacter[i].enemyStatus.characterSkill[enemyCharacter[i].enemyStatus.firstCharacterSkillNum[j]] = true;  
                }
                else
                { 
                    Debug.LogWarning(enemyCharacter[i].enemyStatus.name + "�� firstCharacterSkill ���� " + j +"�� �迭�� �߸��� ���� ���Խ��ϴ�! �߸� ���� �� " + enemyCharacter[i].enemyStatus.firstCharacterSkillNum[j]);
                }
            }
        }    
        #endregion
    }

    //DeepCopy CharacterDatabaseManager���� ����

    public EnemyCharacterData GetEnemyCharacterByName(string name)
    { 
        #region
        int check = 0;
        for(check = 0; check <= enemyCharacter.Length; check++)
        {
            if(check < enemyCharacter.Length)
            { 
                if(enemyCharacter[check].name  == name)
                {                 
                    break;
                }
            }
        }
        if(check < enemyCharacter.Length)
        {
            return enemyCharacter[check];
        }
        else
        { 
            Debug.LogWarning("��ġ�ϴ� ĳ���Ͱ� �����ϴ�!");
            return enemyCharacter[0];
        }
        #endregion
    }

    public EnemyCharacterData GetEnemyCharacter(int i)
    {
        #region
        if(i < enemyCharacter.Length)
        {
            return enemyCharacter[i];
        }
        else
        {
            Debug.LogWarning("EnemyCharacterDatabaseManager�� GetenemyCharacter�Լ��� �߸��� ���� ���Խ��ϴ�.");
            return enemyCharacter[0];
        }
        #endregion
    }
}
