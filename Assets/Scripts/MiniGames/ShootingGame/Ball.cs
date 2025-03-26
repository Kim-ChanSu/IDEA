using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingGame
{
    public class Ball : MonoBehaviour
    {
        [SerializeField]
        private int ballDamage;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.tag == "Level")
            {
                this.gameObject.SetActive(false);
            }
        }

        public int GetBallDamage()
        {
            return this.ballDamage;
        }

        public void SetBallDamage(int value)
        {
            this.ballDamage = value;
        }

    }
}