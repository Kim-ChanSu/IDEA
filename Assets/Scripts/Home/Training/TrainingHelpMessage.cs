using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingHelpMessage : MonoBehaviour
{
    [SerializeField]
    private GameObject helpText; 
    private List<Dictionary<string, object>> helpData;
    private string helpMessagePath = "HelpMessage/HelpMessage";


    void Start()
    {
        SetHelpText();
    }

    private string GetHelpText()
    {
        helpData = CSVReader.Read(GameManager.instance.CSVFolder + helpMessagePath);
        Debug.Log("메시지의 수는 " + helpData.Count);

        int messageNum = 0;
        messageNum = Random.Range(0, helpData.Count);

        return helpData[messageNum]["Content"].ToString();
    }

    private void SetHelpText()
    {
        helpText.GetComponent<Text>().text = GetHelpText();
    }
}
