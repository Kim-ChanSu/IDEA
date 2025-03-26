using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    void Start()
    {
        GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName("GameOver"));
    }

    void Update()
    {
        if(Input.anyKey)
        { 
            GameManager.instance.GoTitleScene();    
        }
    }
}
