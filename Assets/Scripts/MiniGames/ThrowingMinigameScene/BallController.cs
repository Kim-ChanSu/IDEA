using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThrowingMinigame
{
    public class BallController : MonoBehaviour
    {
        private GameObject movingBall;
        [SerializeField]
        private GameObject haveBall;
        [SerializeField]
        private GameObject shootingBall;
        [SerializeField]
        private GameObject tempBallSpawnPoint;
        [SerializeField]
        private GameObject ballSpawnPoint;
        [SerializeField]
        private GameObject shootingPoint;

        [SerializeField]
        private float ballSpeed = 0.0f;
        private Vector3 moveRight = Vector3.right;

        private bool shootLock = true;

        void Start()
        {
            BallControllerInit();
        }

        void Update()
        {
            Shooting();
        }

        public void BallControllerInit()
        {
            this.haveBall.gameObject.SetActive(true);
        }

        public void CreatBall()
        {
            if (movingBall == null)
            {
                this.movingBall = Instantiate(shootingBall, tempBallSpawnPoint.transform);
                this.movingBall.transform.position = ballSpawnPoint.transform.position;
                this.haveBall.gameObject.SetActive(false);
            }
        }

        public void Shooting()
        {
            if (haveBall.activeSelf == false && movingBall != null)
            {
                if (shootLock == false)
                {
                    BallMove();
                }
            }


            if (tempBallSpawnPoint.gameObject.transform.childCount == 0)
            {
               this.ballSpawnPoint.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }

            if (movingBall == null)
            {
                shootLock = true;
            }
        }

        public bool GetShootLock()
        {
            return shootLock;
        }

        public void BallMove()
        {
            this.movingBall.transform.position = this.movingBall.transform.position + this.moveRight * this.ballSpeed * Time.deltaTime;
        }

        public void BallCorrection()
        {
            this.movingBall.transform.position = shootingPoint.transform.position;
            shootLock = false;
        }

        public void ShootingBallHide()
        {
            this.movingBall.gameObject.SetActive(false);
        }

        public void ShootingBallReveal()
        {
            this.movingBall.gameObject.SetActive(true);
        }

        public bool isHaveBall()
        {
            return this.haveBall.activeSelf;
        }

    }
}
