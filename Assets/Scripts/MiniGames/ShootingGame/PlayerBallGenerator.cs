using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingGame
{

    public class PlayerBallGenerator : ShootingGame.BallGenerator
    {
        [SerializeField]
        private ShootingGame.ObjectManager objectManager;

        [SerializeField]
        private float MaxDelayTime = 0.15f;

        private int ballSpeed = 10;

        void Start()
        {
            PlayerBallGeneratorInit();
        }

        void Update()
        {
            DelayTime();
        }

        public void PlayerBallGeneratorInit()
        {          
            base.GeneratorInit();
            base.SetMaxDelayTime(MaxDelayTime);
            this.ballSpeed = 10;
        }

        override public void MakeSmallBall(Transform transform)
        {
            GameObject smallBall = this.objectManager.CreateObject("PlayerBallTypeA");
            smallBall.transform.position = transform.position;

            Rigidbody2D rigidbody2D = smallBall.GetComponent<Rigidbody2D>();
            rigidbody2D.AddForce(Vector2.right * this.ballSpeed, ForceMode2D.Impulse);
        }

        override public void MakeDoubleSmallBall(Transform transform)
        {
            GameObject rightSmallBall = this.objectManager.CreateObject("PlayerBallTypeA");
            rightSmallBall.transform.position = transform.position + Vector3.down * 0.1f;
            
            GameObject leftSmallBall = this.objectManager.CreateObject("PlayerBallTypeA");
            leftSmallBall.transform.position = transform.position + Vector3.up * 0.1f;

            Rigidbody2D rightBallrigid = rightSmallBall.GetComponent<Rigidbody2D>();
            Rigidbody2D leftBallrigid = leftSmallBall.GetComponent<Rigidbody2D>();

            rightBallrigid.AddForce(Vector2.right * this.ballSpeed, ForceMode2D.Impulse);
            leftBallrigid.AddForce(Vector2.right * this.ballSpeed, ForceMode2D.Impulse);
        }

        override public void MakeTripleSmallBall(Transform transform)
        {
            GameObject smallBall = this.objectManager.CreateObject("PlayerBallTypeA");
            smallBall.transform.position = transform.position; 

            GameObject rightSmallBall = this.objectManager.CreateObject("PlayerBallTypeA");
            rightSmallBall.transform.position = transform.position + Vector3.down * 0.25f;
          
            GameObject leftSmallBall = this.objectManager.CreateObject("PlayerBallTypeA");
            leftSmallBall.transform.position = transform.position + Vector3.up * 0.25f;

            Rigidbody2D rigidbody2D = smallBall.GetComponent<Rigidbody2D>();
            Rigidbody2D rightBallrigid = rightSmallBall.GetComponent<Rigidbody2D>();
            Rigidbody2D leftBallrigid = leftSmallBall.GetComponent<Rigidbody2D>();

            rigidbody2D.AddForce(Vector2.right * this.ballSpeed, ForceMode2D.Impulse);
            rightBallrigid.AddForce(Vector2.right * this.ballSpeed, ForceMode2D.Impulse);
            leftBallrigid.AddForce(Vector2.right * this.ballSpeed, ForceMode2D.Impulse);
        }

        override public void MakeBigBall(Transform transform)
        {
            GameObject bigBall = this.objectManager.CreateObject("PlayerBallTypeB");
            bigBall.transform.position = transform.position;

            Rigidbody2D rigidbody2D = bigBall.GetComponent<Rigidbody2D>();
            rigidbody2D.AddForce(Vector2.right * this.ballSpeed, ForceMode2D.Impulse);
        }

        override public void MakeDoubleBigBall(Transform transform)
        {
            GameObject rightBigBall = this.objectManager.CreateObject("PlayerBallTypeB");
            rightBigBall.transform.position = transform.position + Vector3.down * 0.2f;

            GameObject leftBigBall = this.objectManager.CreateObject("PlayerBallTypeB");
            leftBigBall.transform.position = transform.position + Vector3.up * 0.2f;

            Rigidbody2D rightBallrigid = rightBigBall.GetComponent<Rigidbody2D>();
            Rigidbody2D leftBallrigid = leftBigBall.GetComponent<Rigidbody2D>();

            rightBallrigid.AddForce(Vector2.right * this.ballSpeed, ForceMode2D.Impulse);
            leftBallrigid.AddForce(Vector2.right * this.ballSpeed, ForceMode2D.Impulse);
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
            Explosion explosion = Effect.GetComponent<ShootingGame.Explosion>();

            Effect.transform.position = position;
            explosion.PlayEffect(objectType);
        }

    }
}