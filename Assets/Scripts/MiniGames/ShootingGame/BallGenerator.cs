using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingGame
{

    public abstract class BallGenerator : MonoBehaviour
    {
        private float maxDelayTime;
        private float currentTime;


        private void Start()
        {
            GeneratorInit();
        }

        public void SetMaxDelayTime(float time)
        {
            this.maxDelayTime = time;
        }

        virtual public void GeneratorInit()
        {
            this.currentTime = 0f;
        }

        virtual public void MakeSmallBall(Transform transform)
        {
            
        }

        virtual public void MakeDoubleSmallBall(Transform transform)
        {
            
        }

        virtual public void MakeTripleSmallBall(Transform transform)
        {
           
        }

        virtual public void MakeBigBall(Transform transform)
        { 
           
        }

        virtual public void MakeDoubleBigBall(Transform transform)
        {
           
        }

        virtual public void DelayTime()
        {
            this.currentTime = this.currentTime + Time.deltaTime;
        }

        virtual public void ResetDelayTime()
        {
            this.currentTime = 0f;
        }

        virtual public bool IsShootBallDelay()
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


    }
}