using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningMinigame
{

    public class UIAnimation : MonoBehaviour
    {
        private bool IsSceneLock = true;

        [SerializeField]
        private Animator readyTextAnimator;
        [SerializeField]
        private Animator goTextAnimator;

        private void ShowReady()
        {
            this.goTextAnimator.SetTrigger("ShowGo");
        }
        private void ShowGo()
        {
            this.IsSceneLock = false;
        }


        void Start()
        {
            UIAnimationInit();
        }

        private void UIAnimationInit()
        {
            this.IsSceneLock = true;
            this.readyTextAnimator.SetTrigger("ShowReady");
        }


        public bool GetIsSceneLock()
        {
            return this.IsSceneLock;
        }

    }
}