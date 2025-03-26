using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ShootingGame
{

    public class Follower : MonoBehaviour
    {
        [SerializeField]
        private float maxDelayTime = 2;
        private float currentTime = 0;
        [SerializeField]
        private ShootingGame.ObjectManager objectManager;
        [SerializeField]
        private int followerDelay = 0;
        [SerializeField]
        private Vector3 followerPosition;
        [SerializeField]
        private Transform parent;
        private Queue<Vector3> playerPosition;

        private void Awake()
        {
            FollowerInit();
        }

        void Update()
        {
            FollowerMove();
        }

        public void FollowerInit()
        {
            playerPosition = new Queue<Vector3>();
        }

        private void FollowerMove()
        {
            SettingPosition();
            Move();
            ShootBall();
            DelayTime();
        }

        private void SettingPosition()
        {

            if (this.playerPosition.Contains(this.parent.position) == false)
            {
                this.playerPosition.Enqueue(this.parent.position);
            }

            if (this.playerPosition.Count > this.followerDelay)
            {
                this.followerPosition = this.playerPosition.Dequeue();
            }
            else if (this.playerPosition.Count < this.followerDelay)
            {
                this.followerPosition = this.parent.position;
            }

        }

        private void Move()
        {
            this.transform.position = this.followerPosition;
        }

        private void ShootBall()
        {

            if (Input.GetButton("Fire1") == false)
            {
                return;
            }

            if (IsShootBallDelay() == true)
            {
                return;
            }

            MakeFollowerBall(this.transform);
            ResetDelayTime();
        }

        public void DelayTime()
        {
            this.currentTime = this.currentTime + Time.unscaledDeltaTime;
        }

        public bool IsShootBallDelay()
        {
            if (this.currentTime < this.maxDelayTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ResetDelayTime()
        {
            this.currentTime = 0f;
        }

        public void MakeFollowerBall(Transform transform)
        {
            GameObject bigBall = this.objectManager.CreateObject("PlayerFollowerBall");
            bigBall.transform.position = transform.position;
        }

        public void FollowerResetPositon()
        {
            playerPosition.Clear();
            this.playerPosition.Enqueue(this.parent.position);
        }

    }
}
