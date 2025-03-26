using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThrowingMinigame
{
    public class TargetSelection : MonoBehaviour
    {

        [SerializeField]
        private Sprite targetRed;
         [SerializeField]
        private Sprite targetBlue;
         [SerializeField]
        private Sprite targetGreen;
         [SerializeField]
        private Sprite targetPink;
         [SerializeField]
        private Sprite targetPurple;
         //[SerializeField]
        //private Sprite targetBlack;

        public Sprite GetRamdomTargetSprite()
        {
            int tempRandom = Random.Range(0, 5);

            switch (tempRandom)
            {
                case 0: 
                    return targetRed;
                case 1:
                    return targetBlue;
                case 2:
                    return targetGreen;
                case 3:
                    return targetPink;
                default:
                    return targetPurple;
            }

        }
        
    }
}
