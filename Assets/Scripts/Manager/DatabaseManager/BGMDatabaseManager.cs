/* 작성날짜: 2023-09-12
 * 버전: 0.0.1ver 
 * 내용: 브금들을 저장하는 공간
 * 최근 수정 날짜: 2023-09-12
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMDatabaseManager : MonoBehaviour
{
    public static BGMDatabaseManager instance; //싱글톤

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
            Debug.LogWarning("씬에 BGMDatabaseManager가 2개이상 존재합니다.");
            Destroy(gameObject);
        }
        #endregion
    }

    [System.Serializable]
    private class GameBGM
    {
        public string BGMName;
        public AudioClip BGM;
    }

    [SerializeField]
    private GameBGM[] BGM;

    public AudioClip GetBGMByName(string name) // 이름 (BGMName)으로 브금파일 가져오기
    { 
        #region
        int check = 0;
        for(check = 0; check <= BGM.Length; check++)
        {
            if(check < BGM.Length)
            { 
                if(BGM[check].BGMName  == name)
                {                 
                    break;
                }
            }
        }
        if(check < BGM.Length)
        {
            //GameManager.instance.BGM = BGM[check].BGMName; 예전 작업물에서 세이브로드용으로 쓰던거 구조바뀌면 못쓰는거지만 참고용으로 주석처리
            return BGM[check].BGM;
        }
        else
        { 
            Debug.LogWarning("일치하는 브금이 없습니다!");
            return BGM[0].BGM;
        }
        #endregion
    }


    public AudioClip GetBGM(int i) // 배열번호로 브금파일 가져오기
    {
        #region
        if(i < BGM.Length)
        {
            //GameManager.instance.BGM = BGM[i].BGMName; 예전 작업물에서 세이브로드용으로 쓰던거 구조바뀌면 못쓰는거지만 참고용으로 주석처리
            return BGM[i].BGM;
        }
        else
        {
            Debug.LogWarning("BGMDatabaseManager안 GetBGM함수에 잘못된 값이 들어왔습니다.");
            return BGM[0].BGM;
        }
        #endregion
    }

    public int DBLength()
    {         
        return BGM.Length;
    }

}
