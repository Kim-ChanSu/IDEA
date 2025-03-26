using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameBackground //배경 클래스
{
    public string backgroundName; // 배경이름
    public Sprite backgroundImage; //배경이미지
    public RuntimeAnimatorController backgroundAnimator;
}

[System.Serializable]
public class GameCutscene 
{
    public string cutsceneName; // 배경이름
    public Sprite cutsceneImage; //배경이미지
}

public class BackgroundDatabaseManager : MonoBehaviour
{
    public static BackgroundDatabaseManager instance; //싱글톤

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
            Debug.LogWarning("씬에 게임매니저가 2개이상 존재합니다.");
            Destroy(gameObject);
        }
        #endregion
    }

    [SerializeField]
    private GameBackground[] background; 
    [SerializeField]
    private GameCutscene[] cutscene; 

    public Sprite GetBackgroundByName(string name) // 이름(backgroundName)을 이용해서 이미지 가져오기
    { 
        #region
        int check = 0;
        for(check = 0; check <= background.Length; check++)
        {
            if(check < background.Length)
            { 
                if(background[check].backgroundName  == name)
                {                 
                    break;
                }
            }
        }
        if(check < background.Length)
        {
            //GameManager.instance.background = background[check].backgroundName; 예전 작업물에서 세이브로드용으로 쓰던거 구조바뀌면 못쓰는거지만 참고용으로 주석처리
            return background[check].backgroundImage;
        }
        else
        { 
            Debug.LogWarning("일치하는 배경이 없습니다!");
            return background[0].backgroundImage;
        }
        #endregion
    }

    public Sprite GetBackground(int num) // 배열 번호를 이용하여 이미지 가져오기
    {
        #region
        if(num < background.Length)
        {
            //GameManager.instance.Background = background[num].backgroundName; 예전 작업물에서 세이브로드용으로 쓰던거 구조바뀌면 못쓰는거지만 참고용으로 주석처리
            return background[num].backgroundImage;
        }
        else
        {
            Debug.LogWarning("BackgroundDatabaseManager안 GetBackGround함수에 잘못된 값이 들어왔습니다.");
            return background[0].backgroundImage;
        }
        #endregion
    }

    public GameBackground GetBackgroundWithAnimationByName(string name)
    {
        #region
        int check = 0;
        for(check = 0; check <= background.Length; check++)
        {
            if(check < background.Length)
            { 
                if(background[check].backgroundName  == name)
                {                 
                    break;
                }
            }
        }
        if(check < background.Length)
        {
            return background[check];
        }
        else
        { 
            Debug.LogWarning("일치하는 배경이 없습니다!");
            return background[0];
        }
        #endregion
    }

    public GameBackground GetBackgroundWithAnimation(int num)
    {
        #region
        if((0 <= num) && (num < background.Length))
        {
            return background[num];
        }
        else
        {
            Debug.LogWarning("BackgroundDatabaseManager안 GetBackGround함수에 잘못된 값이 들어왔습니다.");
            return background[0];
        }
        #endregion
    }

    public int DBLength() // 배열 길이 가져오기
    {         
        return background.Length;
    }

    public Sprite GetCutsceneByName(string name)
    {
        #region
        int check = 0;
        for(check = 0; check <= cutscene.Length; check++)
        {
            if(check < cutscene.Length)
            { 
                if(cutscene[check].cutsceneName  == name)
                {                 
                    break;
                }
            }
        }

        if(check < cutscene.Length)
        {
            return cutscene[check].cutsceneImage;
        }
        else
        { 
            Debug.LogWarning("일치하는 배경이 없습니다!");
            return cutscene[0].cutsceneImage;
        }
        #endregion
    }
}
