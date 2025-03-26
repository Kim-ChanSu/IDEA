using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallCatchMinigame
{

    public class Judgment : MonoBehaviour
    {

        [SerializeField]
        private BallCatchMinigame.Character character;
        [HideInInspector]
        public GameObject ball;

        [SerializeField]
        private GameObject perfectZone;

        [SerializeField]
        private GameObject hitEffect;

        private float judgmentXpos;
        private float ballXpos = 11;

        [SerializeField]
        private BoxCollider2D boxCollider;
        private double currentTiming;
        private int score;

        [SerializeField]
        private float currentBallSpeed;
        [Tooltip("공 이동 속도 최댓값")]
        [SerializeField]
        private float maxBallSpeed;
        [Tooltip("공 이동 속도 최솟값")]
        [SerializeField]
        private float minBallSpeed;

        [Tooltip("판정 프레임 수")]
        [Range(1,60)]
        [SerializeField]
        private int judgmentFrame = 15;
        private double curFrame;

        private readonly int RIGHT = 0;
        private float curTime;

        void Start()
        {
            JudgmentInit();
        }

        void Update()
        {
            Judge();
        }

        private void JudgmentInit()
        {
            this.judgmentXpos = this.perfectZone.gameObject.transform.position.x;
            this.boxCollider = this.gameObject.GetComponent<BoxCollider2D>();
            this.score = 0;
            this.currentBallSpeed = this.minBallSpeed;
        }

        private void CheckTiming()
        {          
            this.currentTiming = Vector2.Distance(new Vector2(this.ballXpos, 0f), new Vector2(this.judgmentXpos, 0f));

        }

        private void Judge()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(RIGHT)) 
            {
                if (ball != null)
                {
                    this.ballXpos = ball.transform.position.x;
                }
                else
                {
                    this.ballXpos = 11f;
                }

                CheckTiming();

                this.curFrame = this.currentTiming / (Time.fixedDeltaTime * this.currentBallSpeed * Time.fixedDeltaTime); //클릭시 남은 거리 프레임수
                Debug.Log((int)(this.currentTiming / (Time.fixedDeltaTime * this.currentBallSpeed * Time.fixedDeltaTime))); // 프레임수

                if ((int)this.curFrame <= this.judgmentFrame)
                {
                    BallCatchSuccess();
                }

            }
        }

        private void BallCatchSuccess()
        {
            this.score = this.score + 1;
            Debug.Log("Catch");
            Destroy(this.ball);
            AddBallSpeed();
            this.character.Move();
            if (GameManager.instance != null)
            {
                GameManager.instance.PlaySE(SEDatabaseManager.instance.GetSEByName("BallCatch"));
            }
            this.hitEffect.SetActive(true);
        }

        private void AddBallSpeed()
        {
            if(this.currentBallSpeed >= this.maxBallSpeed)
            {
                this.currentBallSpeed = this.maxBallSpeed;
            }
            else
            {
                this.currentBallSpeed = this.currentBallSpeed + this.minBallSpeed * 0.25f;
            }
        }

        public void ResetBallSpeed()
        {
            this.currentBallSpeed = this.minBallSpeed;
        }

        public int GetScore()
        {
            return this.score;
        }

        public float GetBallSpeed()
        {
            return this.currentBallSpeed;
        }

       

    }
}