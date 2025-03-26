using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingGame
{
    public class EnemyBallGenerator : ShootingGame.BallGenerator
    {

        [SerializeField]
        private float MaxDelayTime = 0.15f;

        private int ballSpeed = 10;

        private Vector3 smallBallcorrection;
        private Vector3 bigBallcorrection;

        private GameObject player;
        [HideInInspector]
        public ShootingGame.ObjectManager objectManager;

        private void Awake()
        {
            EnemyBallGeneratorScriptInit();
        }

        void Start()
        {
            EnemyBallGeneratorInit();
        }

        void Update()
        {
            DelayTime();
        }

        public void EnemyBallGeneratorInit()
        {
            base.GeneratorInit();
            base.SetMaxDelayTime(MaxDelayTime);
            this.ballSpeed = 10;
            this.smallBallcorrection = new Vector3(0f, 0.7f, 0f);
            this.bigBallcorrection = new Vector3(0f, 0.65f, 0f);
        }

        public void EnemyBallGeneratorScriptInit()
        {
            this.player = GameObject.FindGameObjectWithTag("Player");
            this.objectManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ShootingGame.ObjectManager>();
        }

        override public void MakeSmallBall(Transform transform)
        {
            GameObject smallBall = this.objectManager.CreateObject("EnemyBallTypeA");
            smallBall.transform.position = transform.position + this.smallBallcorrection;

            Rigidbody2D rigidbody2D = smallBall.GetComponent<Rigidbody2D>();
            Vector3 playerDirection = player.transform.position - this.transform.position;
            rigidbody2D.AddForce(playerDirection.normalized * this.ballSpeed, ForceMode2D.Impulse);
        }

        override public void MakeDoubleSmallBall(Transform transform)
        {
            GameObject rightSmallBall = this.objectManager.CreateObject("EnemyBallTypeA");
            rightSmallBall.transform.position = transform.position + this.smallBallcorrection + Vector3.down * 0.1f;

            GameObject leftSmallBall = this.objectManager.CreateObject("EnemyBallTypeA");
            leftSmallBall.transform.position = transform.position + this.smallBallcorrection + Vector3.up * 0.1f;

            Rigidbody2D rightBallrigid = rightSmallBall.GetComponent<Rigidbody2D>();
            Rigidbody2D leftBallrigid = leftSmallBall.GetComponent<Rigidbody2D>();

            Vector3 playerDirection = player.transform.position - this.transform.position;

            rightBallrigid.AddForce(playerDirection.normalized * this.ballSpeed, ForceMode2D.Impulse);
            leftBallrigid.AddForce(playerDirection.normalized * this.ballSpeed, ForceMode2D.Impulse);
        }

        override public void MakeTripleSmallBall(Transform transform)
        {
            GameObject smallBall = this.objectManager.CreateObject("EnemyBallTypeA");
            smallBall.transform.position = transform.position + this.smallBallcorrection;

            GameObject rightSmallBall = this.objectManager.CreateObject("EnemyBallTypeA");
            rightSmallBall.transform.position = transform.position + this.smallBallcorrection + Vector3.down * 0.25f;

            GameObject leftSmallBall = this.objectManager.CreateObject("EnemyBallTypeA");
            leftSmallBall.transform.position = transform.position + this.smallBallcorrection + Vector3.up * 0.25f;

            Rigidbody2D rigidbody2D = smallBall.GetComponent<Rigidbody2D>();
            Rigidbody2D rightBallrigid = rightSmallBall.GetComponent<Rigidbody2D>();
            Rigidbody2D leftBallrigid = leftSmallBall.GetComponent<Rigidbody2D>();

            Vector3 playerDirection = player.transform.position - this.transform.position;

            rigidbody2D.AddForce(playerDirection.normalized * this.ballSpeed, ForceMode2D.Impulse);
            rightBallrigid.AddForce(playerDirection.normalized * this.ballSpeed, ForceMode2D.Impulse);
            leftBallrigid.AddForce(playerDirection.normalized * this.ballSpeed, ForceMode2D.Impulse);
        }

        override public void MakeBigBall(Transform transform)
        {
            GameObject bigBall = this.objectManager.CreateObject("EnemyBallTypeB");
            bigBall.transform.position = transform.position + this.bigBallcorrection;

            Rigidbody2D rigidbody2D = bigBall.GetComponent<Rigidbody2D>();
            Vector3 playerDirection = player.transform.position - this.transform.position;
            rigidbody2D.AddForce(playerDirection.normalized * this.ballSpeed, ForceMode2D.Impulse);
        }

        override public void MakeDoubleBigBall(Transform transform)
        {
            GameObject rightBigBall = this.objectManager.CreateObject("EnemyBallTypeB");
            rightBigBall.transform.position = transform.position + this.bigBallcorrection + Vector3.down * 0.2f;

            GameObject leftBigBall = this.objectManager.CreateObject("EnemyBallTypeB");
            leftBigBall.transform.position = transform.position + this.bigBallcorrection + Vector3.up * 0.2f;

            Rigidbody2D rightBallrigid = rightBigBall.GetComponent<Rigidbody2D>();
            Rigidbody2D leftBallrigid = leftBigBall.GetComponent<Rigidbody2D>();

            Vector3 playerDirection = player.transform.position - this.transform.position;

            rightBallrigid.AddForce(playerDirection.normalized * this.ballSpeed, ForceMode2D.Impulse);
            leftBallrigid.AddForce(playerDirection.normalized * this.ballSpeed, ForceMode2D.Impulse);
        }

        override public void DelayTime()
        {
            base.DelayTime();
        }

        override public void ResetDelayTime()
        {
            base.ResetDelayTime();
        }

        override public bool IsShootBallDelay()
        {
            return base.IsShootBallDelay();
        }

        public void PlayBallHitEffect(Vector3 position, string objectType)
        {
            GameObject Effect = this.objectManager.CreateObject("HitEffect");
            Explosion explosion = Effect.GetComponent<Explosion>();

            Effect.transform.position = position;
            explosion.PlayEffect(objectType);
        }

    }


}