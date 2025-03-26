using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallCatchMinigame
{

    public class Character : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;
              

        public void Move()
        {
            this.animator.SetTrigger("Catch");            
        }

        public float GetXpos()
        {
            return this.transform.transform.position.x;
        }
    }
}