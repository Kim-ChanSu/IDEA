/* 작성날짜: 2023-11-06
 * 버전: 0.0.1ver 
 * 내용: 대화 캐릭터들을 저장하는 공간
 * 최근 수정 날짜: 2023-11-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkerDatabaseManager : MonoBehaviour
{

    public static TalkerDatabaseManager instance; //싱글톤

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
            Debug.LogWarning("씬에 TalkerDatabaseManager가 2개이상 존재합니다.");
            Destroy(gameObject);
        }
        #endregion
    }

    [SerializeField]
    private Talker[] talker;

    public Talker TalkerDB(int i)
    {       
        return talker[i];
    }

    public int DBLength()
    {         
        return talker.Length;
    }

}
