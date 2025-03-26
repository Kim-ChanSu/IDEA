/* 작성날짜: 2023-11-20
 * 버전: 0.0.1ver 
 * 내용: 공감타입들을 저장하는 공간
 * 최근 수정 날짜: 2023-11-20
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterSympathyType
{
    Extra,
    Ray,
    Ria,
    Karl,
    Marilyn,
    Marzen,
    Iris,
    IrisExtra
}

public enum SympathyType
{
    None,
    Rational,   
    Anger,
    Enjoy   
}

[System.Serializable]
public class LearnSkill
{ 
    public int level;
    public int skillNum;
}

public enum CharacterNatureType
{
    Normal,
    Careful, //레이
    Jovial, //리아
    Sincerity, //카를
    Bold, //마릴린
    Determined, //마르젠
    Playful //아이리스
}

public enum ChangeSympathySituation
{
    Attack, //
    Hit, //
    HitSafe, //
    BlockBall, //
    CatchBoundBall, //
    SafeByFriendly, //
    Pass, //
    GetPass, //
    FailGetPass, //
    FailUseBallSkill, //
    UseIgnoreIsHaveBallSkill, //
    GetIgnoreIsHaveBallSkillByEnemy, //
    GetIgnoreIsHaveBallSkillByTeam, //
    GetBall, //
    Stay, //
    SafeInterruptBall, //
    LoseBallByInterruptBall //
}

public class SympathyDatabaseManager : MonoBehaviour
{
    public static SympathyDatabaseManager instance; //싱글톤

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
            Debug.LogWarning("씬에 SympathyDatabaseManager가 2개이상 존재합니다.");
            Destroy(gameObject);
        }
        #endregion
    }

    [System.Serializable]
    public class SympathyData
    { 
        public CharacterSympathyType characterSympathyType;
        public int minRational;
        public int maxRational; 
        public int minEnjoy;
        public int maxEnjoy;
        public int minAnger;        
        public int maxAnger;

        public LearnSkill[] learnSkill;
    }

    [SerializeField]
    private SympathyData[] sympathyData;

    [System.Serializable]
    public class NatureData
    {
        #region
        public string ingameName;
        public CharacterNatureType characterNatureType;

        [Header ("공격을 할 때 ")]
        public int Attack;
        [Header ("공격에 피해를 입었을 때 ")]
        public int Hit;
        [Header ("방어에 성공하였을 때 ")]
        public int HitSafe;
        [Header ("공을 사용한 스킬을 막았을 때 ")]
        public int BlockBall;
        [Header ("팅겨나온 공을 잡았을 때 ")]
        public int CatchBoundBall;
        [Header ("아군이 공을 잡아 나를 커버 쳐줬을 때 " )]
        public int SafeByFriendly;
        [Header ("패스를 할 때 ")]
        public int Pass;
        [Header ("패스를 받았을 때 ")]
        public int GetPass;
        [Header ("패스를 받지 못하였을 때 ")]
        public int FailGetPass;
        [Header ("공을 사용한 상호작용을 실패 했을 때")]
        public int FailUseBallSkill;
        [Header ("공이 필요 없는 스킬을 사용할 때")]
        public int UseIgnoreIsHaveBallSkill;
        [Header ("공이 필요 없는 스킬의 대상이 되었을 때(스킬사용자가 적)")]
        public int GetIgnoreIsHaveBallSkillByEnemy;
        [Header ("공이 필요 없는 스킬의 대상이 되었을 때(스킬사용자가 아군)")]
        public int GetIgnoreIsHaveBallSkillByTeam;
        [Header ("필드에서 공을 주었을 때(주운 측)")]
        public int GetBall;
        [Header ("행동 페이즈에서 대기를 사용하였을 때")]
        public int Stay;
        [Header ("방해하기를 방어 했을 때")]
        public int SafeInterruptBall;
        [Header ("방해하기에 당해 공을 뺏겼을 때")]
        public int LoseBallByInterruptBall;
        #endregion
    }

    [SerializeField]
    private NatureData[] natureData;

    public SympathyType CheckSympathy(Character character)
    { 
        int characterSympathy = character.status.Sympathy;
        SympathyData type = new SympathyData();
        int check = 0;
        for(check = 0; check < sympathyData.Length; check++)
        { 
            if(sympathyData[check].characterSympathyType == character.status.sympathyType)
            { 
                type = sympathyData[check];
                break;    
            }
        }

        if(check >= sympathyData.Length)
        { 
            Debug.LogWarning("일치하는 공감타입이 없습니다!");
            type = sympathyData[0];
        }
        
        if(CheckSympathyRange(type.maxRational, type.minRational, characterSympathy) == true)
        { 
            return SympathyType.Rational;
        }
        else if(CheckSympathyRange(type.maxAnger, type.minAnger, characterSympathy) == true)
        { 
            return SympathyType.Anger;
        }
        else if(CheckSympathyRange(type.maxEnjoy, type.minEnjoy, characterSympathy) == true)
        { 
            return SympathyType.Enjoy;
        }
        else
        { 
            return SympathyType.None;
        }
    }

    public SympathyData GetSympathyDataByCharacterSympathyType(CharacterSympathyType sympathyType)
    { 
        SympathyData type = new SympathyData();
        int check = 0;

        for(check = 0; check <= sympathyData.Length; check++)
        {
            if(check < sympathyData.Length)
            { 
                if(sympathyData[check].characterSympathyType  == sympathyType)
                {                 
                    break;
                }
            }
        }
        if(check < sympathyData.Length)
        {
            type = sympathyData[check];
            return type;
        }
        else
        { 
            Debug.LogWarning("일치하는 감정타입이 없습니다!");
            type = sympathyData[0];
            return type;
        }
    }

    public void SetCharacterSympathy(Character character, SympathyType sympathyType)
    {     
        #region
        SympathyData type = new SympathyData();
        int check = 0;
        int changeSP = 0;
        for(check = 0; check < sympathyData.Length; check++)
        { 
            if(sympathyData[check].characterSympathyType == character.status.sympathyType)
            { 
                type = sympathyData[check];
                break;    
            }
        }

        if(check >= sympathyData.Length)
        { 
            Debug.LogWarning("일치하는 공감타입이 없습니다!");
            type = sympathyData[0];
        }

        if(sympathyType == SympathyType.None)
        { 
            changeSP = 0;
        }    
        else if(sympathyType == SympathyType.Rational)
        { 
            changeSP = GetMiddle(type.maxRational, type.minRational);
        }
        else if(sympathyType == SympathyType.Anger)
        { 
            changeSP = GetMiddle(type.maxAnger, type.minAnger);
        }
        else if(sympathyType == SympathyType.Enjoy)
        { 
            changeSP = GetMiddle(type.maxEnjoy, type.minEnjoy);
        }
        
        if(character.isEnemy == false)
        { 
            GameManager.instance.PlayerCharacter[character.characterDatabaseNum].Sympathy = changeSP;
        }
        else
        { 
            GameManager.instance.EnemyCharacter[character.characterDatabaseNum].Sympathy = changeSP;
        }
        #endregion
    }

    private bool CheckSympathyRange(int max, int min, int checker)
    { 
        if((min <= checker) && (checker <= max))
        { 
            return true;
        }
        else
        { 
            return false;   
        }
    }

    private int GetMiddle(int max, int min)
    { 
        return (int)((max + min)/2);
    }

    public int GetSympathyChangeCount(CharacterNatureType natureType, ChangeSympathySituation situation)
    {
        for (int i = 0; i < natureData.Length; i++)
        {
            if (natureData[i].characterNatureType == natureType)
            {
                switch (situation)
                {
                    case ChangeSympathySituation.Attack:
                        return natureData[i].Attack;
                    case ChangeSympathySituation.Hit:
                        return natureData[i].Hit;
                    case ChangeSympathySituation.HitSafe:
                        return natureData[i].HitSafe;
                    case ChangeSympathySituation.BlockBall:
                        return natureData[i].BlockBall;
                    case ChangeSympathySituation.CatchBoundBall:
                        return natureData[i].CatchBoundBall;
                    case ChangeSympathySituation.SafeByFriendly:
                        return natureData[i].SafeByFriendly;
                    case ChangeSympathySituation.Pass:
                        return natureData[i].Pass;
                    case ChangeSympathySituation.GetPass:
                        return natureData[i].GetPass;
                    case ChangeSympathySituation.FailGetPass:
                        return natureData[i].FailGetPass;
                    case ChangeSympathySituation.FailUseBallSkill:
                        return natureData[i].FailUseBallSkill;
                    case ChangeSympathySituation.UseIgnoreIsHaveBallSkill:
                        return natureData[i].UseIgnoreIsHaveBallSkill;
                    case ChangeSympathySituation.GetIgnoreIsHaveBallSkillByEnemy:
                        return natureData[i].GetIgnoreIsHaveBallSkillByEnemy;
                    case ChangeSympathySituation.GetIgnoreIsHaveBallSkillByTeam:
                        return natureData[i].GetIgnoreIsHaveBallSkillByTeam;
                    case ChangeSympathySituation.GetBall:
                        return natureData[i].GetBall;
                    case ChangeSympathySituation.Stay:
                        return natureData[i].Stay;
                    case ChangeSympathySituation.SafeInterruptBall:
                        return natureData[i].SafeInterruptBall;
                    case ChangeSympathySituation.LoseBallByInterruptBall:
                        return natureData[i].LoseBallByInterruptBall;
                    default:
                        Debug.LogWarning("해당하는 ChangeSympathySituation가 없습니다!");
                        return 0;
                }

            }
        }

        Debug.LogWarning("해당하는 natureData가 없습니다!");
        return 0;
    }
}
