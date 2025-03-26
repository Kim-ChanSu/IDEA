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
            Debug.LogWarning("씬에 EnemyCharacterDatabaseManager 2개이상 존재합니다.");
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
                    Debug.LogWarning(enemyCharacter[i].enemyStatus.name + "의 firstCharacterSkill 변수 " + j +"번 배열에 잘못된 값이 들어왔습니다! 잘못 들어온 값 " + enemyCharacter[i].enemyStatus.firstCharacterSkillNum[j]);
                }
            }
        }    
        #endregion
    }

    //DeepCopy CharacterDatabaseManager에서 돌림

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
            Debug.LogWarning("일치하는 캐릭터가 없습니다!");
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
            Debug.LogWarning("EnemyCharacterDatabaseManager안 GetenemyCharacter함수에 잘못된 값이 들어왔습니다.");
            return enemyCharacter[0];
        }
        #endregion
    }
}
