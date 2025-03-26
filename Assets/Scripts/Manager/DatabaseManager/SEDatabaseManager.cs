using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEDatabaseManager : MonoBehaviour
{
    public static SEDatabaseManager instance; //싱글톤

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
            Debug.LogWarning("씬에 SEDatabaseManager 2개이상 존재합니다.");
            Destroy(gameObject);
        }
        #endregion
    }

    [System.Serializable]
    private class GameSE
    {
        public string SEName;
        public AudioClip SE;
    }

    [SerializeField]
    private GameSE[] SE;

    public AudioClip GetSEByName(string name)
    { 
        #region
        int check = 0;
        for(check = 0; check <= SE.Length; check++)
        {
            if(check < SE.Length)
            { 
                if(SE[check].SEName  == name)
                {                 
                    break;
                }
            }
        }
        if(check < SE.Length)
        {
            return SE[check].SE;
        }
        else
        { 
            Debug.LogWarning("일치하는 SE가 없습니다!");
            return SE[0].SE;
        }
        #endregion
    }


    public AudioClip GetSE(int i)
    {
        #region
        if(i < SE.Length)
        {
            return SE[i].SE;
        }
        else
        {
            Debug.LogWarning("SEDatabaseManager안 GetSE함수에 잘못된 값이 들어왔습니다.");
            return SE[0].SE;
        }
        #endregion
    }

    public int DBLength()
    {         
        return SE.Length;
    }

}
