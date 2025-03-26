/* 작성날짜: 2023-09-15
 * 버전: 0.0.1ver 
 * 내용: 캐릭터의 초기값을 저장하는 공간
 * 최근 수정 날짜: 2024-01-26
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CharacterStatus
{
    #region
    public string name;
    public string inGameName;
    public Sprite[] face;
    public Sprite inGameCharacter;
    public GameObject characterAnimation;
    public CharacterSympathyType sympathyType;
    public CharacterNatureType characterNatureType;
    public CharacterVoiceType voice;

    public CharacterInformation characterInformation;
    
    public int level;
    public int maxEXP;
    public int EXP;

    public int maxHP;
    public int HP;
    public int maxMP;
    public int MP;
    [HideInInspector]
    public int maxSympathy = 100; 
    public int Sympathy; 

    public int ATK;
    public int MAK;
    public int DEF;
    public int MDF;
    public int move;
    public int range;

    public int attackSkillNum;

    public int[] firstCharacterSkillNum; 
    [HideInInspector]
    public bool[] characterSkill;

    public StatusValue HPValue;
    public StatusValue MPValue;
    public StatusValue ATKValue;
    public StatusValue MAKValue;
    public StatusValue DEFValue;
    public StatusValue MDFValue;

    [HideInInspector]
    public int maxHPEXP;
    [HideInInspector]
    public int maxMPEXP;
    [HideInInspector]
    public int ATKEXP;
    [HideInInspector]
    public int MAKEXP;
    [HideInInspector]
    public int DEFEXP;
    [HideInInspector]
    public int MDFEXP;

    [HideInInspector] 
    public int increaseMaxHPByTrain;
    [HideInInspector] 
    public int increaseMaxMPByTrain;
    [HideInInspector] 
    public int increaseATKByTrain;
    [HideInInspector] 
    public int increaseMAKByTrain;
    [HideInInspector] 
    public int increaseDEFByTrain;
    [HideInInspector] 
    public int increaseMDFByTrain;
    
    [HideInInspector] 
    public IncreasableStatus trainStatus;
    #endregion
}

public enum StatusValue
{
    S,
    A,
    B,
    C,
    D,
    E,
    F
}

public enum IncreasableStatus
{
    maxHP,
    maxMP,
    ATK,
    MAK,
    DEF,
    MDF
}

[System.Serializable]
public class CharacterInformation
{ 
    public Sprite characterIllustration;
    [TextArea]
    public string characterExplain;
    public int characterAge;
    public int characterBirthMonth;
    public int characterBirthDay;
    public int characterHeight;
    public int characterWeight;    
}


public class CharacterDatabaseManager : MonoBehaviour
{
    public static CharacterDatabaseManager instance; //싱글톤

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
            Debug.LogWarning("씬에 CharacterDatabaseManager가 2개이상 존재합니다.");
            Destroy(gameObject);
        }
        #endregion
    }

    void Start()
    { 
        SetCharacterSkill();
    }

    private void SetCharacterSkill()
    { 
        #region
        for(int i = 0; i < PlayerCharacter.Length; i++)
        { 
            PlayerCharacter[i].characterSkill = new bool[SkillDatabaseManager.instance.SkillLength()]; 

            for(int j = 0; j < PlayerCharacter[i].firstCharacterSkillNum.Length; j++)
            { 
                if(PlayerCharacter[i].firstCharacterSkillNum[j] < SkillDatabaseManager.instance.SkillLength() == true)
                { 
                    PlayerCharacter[i].characterSkill[PlayerCharacter[i].firstCharacterSkillNum[j]] = true;  
                }
                else
                { 
                    Debug.LogWarning(PlayerCharacter[i].name + "의 firstCharacterSkill 변수 " + j +"번 배열에 잘못된 값이 들어왔습니다! 잘못 들어온 값 " + PlayerCharacter[i].firstCharacterSkillNum[j]);
                }
            }
        }
        #endregion

        //GetAllSkill(PlayerCharacter[0]); //스킬테스트용
    }

    private void GetAllSkill(CharacterStatus playerCharacter) //모든스킬배우기
    {
        for(int i = 0; i < SkillDatabaseManager.instance.SkillLength(); i++)
        { 
            playerCharacter.characterSkill[i] = true;
        }
        
    }


    public CharacterStatus[] PlayerCharacter;

    public CharacterStatus GetPlayerCharacterByName(string name)
    { 
        #region
        int check = 0;
        for(check = 0; check <= PlayerCharacter.Length; check++)
        {
            if(check < PlayerCharacter.Length)
            { 
                if(PlayerCharacter[check].name  == name)
                {                 
                    break;
                }
            }
        }
        if(check < PlayerCharacter.Length)
        {
            return PlayerCharacter[check];
        }
        else
        { 
            Debug.LogWarning("일치하는 캐릭터가 없습니다!");
            return PlayerCharacter[0];
        }
        #endregion
    }

    public CharacterStatus GetPlayerCharacter(int i)
    {
        #region
        if(i < PlayerCharacter.Length)
        {
            return PlayerCharacter[i];
        }
        else
        {
            Debug.LogWarning("CharacterDatabaseManager안 GetPlayerCharacter함수에 잘못된 값이 들어왔습니다.");
            return PlayerCharacter[0];
        }
        #endregion
    }

    public int DBLength()
    {         
        return PlayerCharacter.Length;
    }

    public CharacterStatus DeepCopyCharacterStatus(CharacterStatus copyCharacterStatus)
    {
        #region
        CharacterStatus newStatus = new CharacterStatus();

        newStatus.name = copyCharacterStatus.name;
        newStatus.inGameName = copyCharacterStatus.inGameName;
        newStatus.face = new Sprite [copyCharacterStatus.face.Length];
        for(int i = 0; i < newStatus.face.Length; i++)
        { 
            newStatus.face[i] = copyCharacterStatus.face[i];
        }

        newStatus.inGameCharacter = copyCharacterStatus.inGameCharacter;
        newStatus.sympathyType = copyCharacterStatus.sympathyType;
        newStatus.characterNatureType = copyCharacterStatus.characterNatureType;
        newStatus.voice = copyCharacterStatus.voice;
    
        newStatus.level = copyCharacterStatus.level;
        newStatus.maxEXP = copyCharacterStatus.maxEXP;
        newStatus.EXP = copyCharacterStatus.EXP;

        newStatus.maxHP = copyCharacterStatus.maxHP;
        newStatus.HP = copyCharacterStatus.HP;
        newStatus.maxMP = copyCharacterStatus.maxMP;
        newStatus.MP = copyCharacterStatus.MP;
        newStatus.maxSympathy = copyCharacterStatus.maxSympathy; 
        newStatus.Sympathy = copyCharacterStatus.Sympathy; 

        newStatus.ATK = copyCharacterStatus.ATK;
        newStatus.MAK = copyCharacterStatus.MAK;
        newStatus.DEF = copyCharacterStatus.DEF;
        newStatus.MDF = copyCharacterStatus.MDF;
        newStatus.range = copyCharacterStatus.range;
        newStatus.move = copyCharacterStatus.move;   
        newStatus.attackSkillNum = copyCharacterStatus.attackSkillNum;

        newStatus.firstCharacterSkillNum = new int[copyCharacterStatus.firstCharacterSkillNum.Length];
        for(int i = 0; i < newStatus.firstCharacterSkillNum.Length; i++)
        { 
            newStatus.firstCharacterSkillNum[i] =copyCharacterStatus.firstCharacterSkillNum[i];
        }
        newStatus.characterSkill = new bool[copyCharacterStatus.characterSkill.Length];
        for(int i = 0; i < newStatus.characterSkill.Length; i++)
        { 
            newStatus.characterSkill[i] = copyCharacterStatus.characterSkill[i];
        }

        newStatus.HPValue = copyCharacterStatus.HPValue;
        newStatus.MPValue = copyCharacterStatus.MPValue;
        newStatus.ATKValue = copyCharacterStatus.ATKValue;
        newStatus.MAKValue = copyCharacterStatus.MAKValue;
        newStatus.DEFValue = copyCharacterStatus.DEFValue;
        newStatus.MDFValue = copyCharacterStatus.MDFValue;

        newStatus.characterInformation = new CharacterInformation();

        newStatus.characterInformation.characterIllustration = copyCharacterStatus.characterInformation.characterIllustration;
        newStatus.characterInformation.characterExplain = copyCharacterStatus.characterInformation.characterExplain;
        newStatus.characterInformation.characterAge = copyCharacterStatus.characterInformation.characterAge;
        newStatus.characterInformation.characterBirthMonth = copyCharacterStatus.characterInformation.characterBirthMonth;
        newStatus.characterInformation.characterBirthDay = copyCharacterStatus.characterInformation.characterBirthDay;
        newStatus.characterInformation.characterHeight = copyCharacterStatus.characterInformation.characterHeight;
        newStatus.characterInformation.characterWeight = copyCharacterStatus.characterInformation.characterWeight;

        newStatus.maxHPEXP = copyCharacterStatus.maxHPEXP;
        newStatus.maxMPEXP = copyCharacterStatus.maxMPEXP;
        newStatus.ATKEXP = copyCharacterStatus.ATKEXP;
        newStatus.MAKEXP = copyCharacterStatus.MAKEXP;
        newStatus.DEFEXP = copyCharacterStatus.DEFEXP;
        newStatus.MDFEXP = copyCharacterStatus.MDFEXP;

        newStatus.increaseMaxHPByTrain = copyCharacterStatus.increaseMaxHPByTrain;
        newStatus.increaseMaxMPByTrain = copyCharacterStatus.increaseMaxMPByTrain;
        newStatus.increaseATKByTrain = copyCharacterStatus.increaseATKByTrain;
        newStatus.increaseMAKByTrain = copyCharacterStatus.increaseMAKByTrain;
        newStatus.increaseDEFByTrain = copyCharacterStatus.increaseDEFByTrain;
        newStatus.increaseMDFByTrain = copyCharacterStatus.increaseMDFByTrain;
        newStatus.trainStatus = copyCharacterStatus.trainStatus;

        newStatus.characterAnimation =  copyCharacterStatus.characterAnimation;

        return newStatus;
        #endregion
    }
    
}
