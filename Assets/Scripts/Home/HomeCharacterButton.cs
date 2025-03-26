using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeCharacterButton : MonoBehaviour
{
    [SerializeField]
    private GameObject homeCharacter;

    public void setCharacterOutline(bool mode)
    { 
        if(homeCharacter.GetComponent<Outline>() == true)
        { 
            homeCharacter.GetComponent<Outline>().enabled = mode;
        }
    }
}
