using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningMinigame
{
    public class BackGround : MonoBehaviour
    {
        [Tooltip("배경 움직이는 최소속도")]
        [SerializeField]
        private float minSpeed = 10f;
       
        private float curMoveSpeed;
        private float maxSpeed;
        private float width;

        [SerializeField]
        private RunningMinigame.UIAnimation uiAnimation;

        [Tooltip("배경 이동 오프셋 설정")]
        [SerializeField]
        private Vector2 movePosition; 

        private void Awake()
        {
            BackGroundInit();
        }

        void Update()
        {
            Move();
        }

        public void BackGroundInit()
        {
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            this.width = boxCollider.size.x;
            this.maxSpeed = this.minSpeed * 2.5f;
            this.curMoveSpeed = this.minSpeed;
        }

        private void Move()
        {
            if (this.uiAnimation.GetIsSceneLock() == true)
            {
                return;
            }

            this.transform.Translate(Vector3.left * this.curMoveSpeed * Time.deltaTime);
            ResetPosition();
        }

        private void ResetPosition()
        {
            if(this.transform.position.x <= -20)
            {
                Vector2 offset = new Vector2(this.width + this.movePosition.x, this.movePosition.y);
                this.transform.position = (Vector2)this.transform.position + offset;
            }
        }

        public void AddSpeed()
        {
            if(this.curMoveSpeed >= this.maxSpeed)
            {
                this.curMoveSpeed = this.maxSpeed;
            }
            else
            {
                this.curMoveSpeed = this.curMoveSpeed + this.minSpeed * 0.2f;
            }
        }

        public void ResetSpeed()
        {
            this.curMoveSpeed = this.minSpeed;
        }
        
    }
}