using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInformationNameController : MonoBehaviour
{
    public GameObject HPNameText;
    public GameObject MPNameText;
    public GameObject SPNameText;
    public GameObject ATKNameText;
    public GameObject MAKNameText;
    public GameObject DEFNameText;
    public GameObject MDFNameText;
    public GameObject moveNameText;
    public GameObject rangeNameText;

    private string HPColor = "<color=#D1180B>";
    private string MPColor = "<color=#4B89DC>";
    private string SPColor  = "<color=#0BFA77>";

    void Start()
    { 
        InitialzeCharacterInformationName();
    }

    void InitialzeCharacterInformationName()
    { 
        HPNameText.GetComponent<Text>().text = HPColor + NameDatabaseManager.HPName + "</color>";
        MPNameText.GetComponent<Text>().text = MPColor + NameDatabaseManager.MPName + "</color>";
        SPNameText.GetComponent<Text>().text = SPColor + NameDatabaseManager.SympathyName + "</color>";
        ATKNameText.GetComponent<Text>().text = NameDatabaseManager.ATKName;
        MAKNameText.GetComponent<Text>().text = NameDatabaseManager.MAKName;
        DEFNameText.GetComponent<Text>().text = NameDatabaseManager.DEFName;
        MDFNameText.GetComponent<Text>().text = NameDatabaseManager.MDFName;
        moveNameText.GetComponent<Text>().text = NameDatabaseManager.moveName;
        rangeNameText.GetComponent<Text>().text = NameDatabaseManager.rangeName;
    }
}
