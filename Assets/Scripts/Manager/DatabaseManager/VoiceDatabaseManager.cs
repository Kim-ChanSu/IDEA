/* 작성날짜: 2023-09-18
 * 버전: 0.0.1ver 
 * 내용: 보이스들을 저장하는 공간
 * 최근 수정 날짜: 2023-09-18
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
    public static VoiceDatabaseManager instance; //싱글톤

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
            Debug.LogWarning("씬에 VoiceDatabaseManager가 2개이상 존재합니다.");
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
            Debug.LogWarning("일치하는 캐릭터타입이 없습니다!");
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
