using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainButton : MonoBehaviour
{
    public TrainManager trainManager;
    public GameObject trainUICanvas;

    private void Awake()
    {
        //SettingTrainButton();
    }

    void Start()
    {
        TrainButtonInitialize();
    }
      
    private void SettingTrainButton()
    {
        if (GameManager.instance.isTimeToMainScenario() == true)
        {
            this.GetComponent<Button>().enabled = false;
        }
        else
        {
            this.GetComponent<Button>().enabled = true;
        }
    }

    public void TrainButtonInitialize()
    {
        trainUICanvas.SetActive(false);               
    }

    public void ShowTrainUI()
    {              
        if(GameManager.instance.isTimeToMainScenario() == false)
        {
            trainUICanvas.SetActive(true);
            trainManager.SettingTrainUI();
            trainManager.ResetInputGold();

            if (GameManager.instance.homeManager != null)
            {
                GameManager.instance.homeManager.SetAllHomeButtons(false);
                GameManager.instance.homeManager.isTrainingUIOn = true;
            }
        }
    }

    public void ExitTrainUI()
    {
        trainManager.EndTrainUI(); 
    }
       
}
