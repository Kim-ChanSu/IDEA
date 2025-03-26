using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainCutSceneManager : MonoBehaviour
{

    [SerializeField]
    private GameObject PlayPassScene;
    [SerializeField]
    private GameObject PlayPassHitScene;

    private int playSceneNumber = 0;

    public void ToLoadHomeScene()
    {
        SceneManager.LoadScene("TrainingResultScene");
    }

    private void Awake()
    {
        ResetPlayScene();
    }

    private void Start()
    {
        SetResult();
        SetRandom();
        SetPlayScene();
    }

    private void SetRandom()
    {
        playSceneNumber = Random.Range(0, 2);
    }

    private void SetPlayScene()
    {
        switch (playSceneNumber)
        {
            case 0:
                PlayPassScene.gameObject.SetActive(true);
                break;
            case 1:
                PlayPassHitScene.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void ResetPlayScene()
    {
        PlayPassScene.gameObject.SetActive(false);
        PlayPassHitScene.gameObject.SetActive(false);
    }

    private void SetResult()
    {
        PlayerPrefs.SetString("ResultReward", "COPPER");
    }
}
