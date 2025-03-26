using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallCatchMinigame
{

    public class Ball : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed;

        private void Start()
        {
            this.GetComponent<Rigidbody2D>().AddForce(Vector2.left * this.moveSpeed);
        }

        void Update()
        {
            BallMove();
        }

        public void SetBallSpeed(float value)
        {
            this.moveSpeed = value;
        }

        private void BallMove()
        {
            //this.transform.Rotate(new Vector3(0f, 0f, this.moveSpeed * 0.25f));
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.name == "EndLineLeft")
            {
                Destroy(this.gameObject);
            }
        }
                
    }
}