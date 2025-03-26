using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallCatchMinigame
{

    public class BallGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject ball;
        [Tooltip("공 생성 위치")]
        [SerializeField]
        private GameObject ballSpawnPosition;

        [Tooltip("최대 공 생성 시간")]
        [SerializeField]
        private float maxDelayTime;

        private float currentTime;
        private float setDelayTime;

        [SerializeField]
        private Judgment judgment;

        private float setBallMoveSpeed;

        void Start()
        {
            BallGeneratorInit();
        }

        void Update()
        {
            BallCreate();
        }

        public void BallGeneratorInit()
        {
            this.currentTime = 0f;
            this.setDelayTime = this.maxDelayTime;
        }

        private void BallCreate()
        {
            if(this.currentTime < this.setDelayTime)
            {
                this.currentTime = this.currentTime + Time.deltaTime;
            }
            else
            {
                MakeBall();
                this.currentTime = 0f;
            }
        }

        private void MakeBall()
        {
            this.setBallMoveSpeed = this.judgment.GetBallSpeed();
            this.judgment.ball = Instantiate(this.ball, ballSpawnPosition.transform);
            this.judgment.ball.GetComponent<BallCatchMinigame.Ball>().SetBallSpeed(this.setBallMoveSpeed);
        }

    }
}