/* �ۼ���¥: 2023-11-20
 * ����: 0.0.1ver 
 * ����: ����Ÿ�Ե��� �����ϴ� ����
 * �ֱ� ���� ��¥: 2023-11-20
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
    Careful, //����
    Jovial, //����
    Sincerity, //ī��
    Bold, //������
    Determined, //������
    Playful //���̸���
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
    public static SympathyDatabaseManager instance; //�̱���

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
            Debug.LogWarning("���� SympathyDatabaseManager�� 2���̻� �����մϴ�.");
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

        [Header ("������ �� �� ")]
        public int Attack;
        [Header ("���ݿ� ���ظ� �Ծ��� �� ")]
        public int Hit;
        [Header ("�� �����Ͽ��� �� ")]
        public int HitSafe;
        [Header ("���� ����� ��ų�� ������ �� ")]
        public int BlockBall;
        [Header ("�ðܳ��� ���� ����� �� ")]
        public int CatchBoundBall;
        [Header ("�Ʊ��� ���� ��� ���� Ŀ�� ������ �� " )]
        public int SafeByFriendly;
        [Header ("�н��� �� �� ")]
        public int Pass;
        [Header ("�н��� �޾��� �� ")]
        public int GetPass;
        [Header ("�н��� ���� ���Ͽ��� �� ")]
        public int FailGetPass;
        [Header ("���� ����� ��ȣ�ۿ��� ���� ���� ��")]
        public int FailUseBallSkill;
        [Header ("���� �ʿ� ���� ��ų�� ����� ��")]
        public int UseIgnoreIsHaveBallSkill;
        [Header ("���� �ʿ� ���� ��ų�� ����� �Ǿ��� ��(��ų����ڰ� ��)")]
        public int GetIgnoreIsHaveBallSkillByEnemy;
        [Header ("���� �ʿ� ���� ��ų�� ����� �Ǿ��� ��(��ų����ڰ� �Ʊ�)")]
        public int GetIgnoreIsHaveBallSkillByTeam;
        [Header ("�ʵ忡�� ���� �־��� ��(�ֿ� ��)")]
        public int GetBall;
        [Header ("�ൿ ������� ��⸦ ����Ͽ��� ��")]
        public int Stay;
        [Header ("�����ϱ⸦ ��� ���� ��")]
        public int SafeInterruptBall;
        [Header ("�����ϱ⿡ ���� ���� ������ ��")]
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
            Debug.LogWarning("��ġ�ϴ� ����Ÿ���� �����ϴ�!");
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
            Debug.LogWarning("��ġ�ϴ� ����Ÿ���� �����ϴ�!");
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
            Debug.LogWarning("��ġ�ϴ� ����Ÿ���� �����ϴ�!");
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
                        Debug.LogWarning("�ش��ϴ� ChangeSympathySituation�� �����ϴ�!");
                        return 0;
                }

            }
        }

        Debug.LogWarning("�ش��ϴ� natureData�� �����ϴ�!");
        return 0;
    }
}
