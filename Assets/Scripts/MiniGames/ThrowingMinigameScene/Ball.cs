using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThrowingMinigame
{
    public class Ball : MonoBehaviour
    {
        [SerializeField]
        private Vector3 rotationDirection = Vector3.zero;

        private void Update()
        {
            Move();
        }

        public void Move()
        {
            this.transform.Rotate(rotationDirection * Time.deltaTime);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            switch (collision.gameObject.name)
            {
                case "Target":
                    break;
                case "EndLineRight":
                    Destroy(this.gameObject);
                    break;

            }
        }
    }
}