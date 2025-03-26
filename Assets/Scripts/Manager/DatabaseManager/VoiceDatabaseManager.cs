/* �ۼ���¥: 2023-09-18
 * ����: 0.0.1ver 
 * ����: ���̽����� �����ϴ� ����
 * �ֱ� ���� ��¥: 2023-09-18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterVoiceType
{
    Girl,
    Man 
}

public enum VoiceType
{
    Select,
    Move    
}


public class VoiceDatabaseManager : MonoBehaviour
{
    public static VoiceDatabaseManager instance; //�̱���

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
            Debug.LogWarning("���� VoiceDatabaseManager�� 2���̻� �����մϴ�.");
            Destroy(gameObject);
        }
        #endregion
    }

    [System.Serializable]
    private class GameVoice
    { 
        public CharacterVoiceType characterVoiceType;
        public AudioClip[] select;
        public AudioClip[] move;        
    }

    [SerializeField]
    private GameVoice[] Voice;



    public void PlayVoice(CharacterVoiceType character, VoiceType voicetype) 
    {  
       //PlayGameVoice(FindVoice(character),voicetype);
    }

    private GameVoice FindVoice(CharacterVoiceType character)
    { 
        #region
        int check = 0;
        for(check = 0; check <= Voice.Length; check++)
        {
            if(check < Voice.Length)
            { 
                if(Voice[check].characterVoiceType  == character)
                {                 
                    break;
                }
            }
        }
        if(check < Voice.Length)
        {  
            return Voice[check];
        }
        else
        { 
            Debug.LogWarning("��ġ�ϴ� ĳ����Ÿ���� �����ϴ�!");
            return Voice[0];
        }
        #endregion
    }

    private void PlayGameVoice(GameVoice voice, VoiceType voiceType)
    {
        #region
         switch(voiceType)
        {
            case VoiceType.Select:
                GameManager.instance.PlaySE(voice.select[Random.Range(0,voice.select.Length)]);
                break;
            case VoiceType.Move:
                GameManager.instance.PlaySE(voice.move[Random.Range(0,voice.move.Length)]);
                break;
        }       
        #endregion
    }


}
