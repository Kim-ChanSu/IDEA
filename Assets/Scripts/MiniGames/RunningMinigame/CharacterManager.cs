using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningMinigame
{
    public class CharacterManager : MonoBehaviour
    {

        [SerializeField]
        private GameObject[] characters;

        private int randomNumber;

        private void Start()
        {
            CharacterManagerInit();
        }

        private void CharacterManagerInit()
        {
            this.randomNumber = Random.Range(0, characters.Length);
            this.characters[0].SetActive(true);
        }

    }
}