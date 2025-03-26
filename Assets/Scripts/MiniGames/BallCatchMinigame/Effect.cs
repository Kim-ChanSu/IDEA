using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallCatchMinigame
{
    public class Effect : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            ExplosionInit();
        }

        private void OnEnable()
        {
            Invoke("EndEffect", 0.4f);
        }

        public void ExplosionInit()
        {
            this.animator = GetComponent<Animator>();
        }
                
        private void EndEffect()
        {
            this.gameObject.SetActive(false);
        }
    }
}
