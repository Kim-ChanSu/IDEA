using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThrowingMinigame
{
    public class TargetConroller : MonoBehaviour
    {
        private ThrowingMinigame.TargetSelection targetSelection;
        [SerializeField]
        private RunningMinigame.UIAnimation uiAnimation;

        [Tooltip("표적 시작 위치")]
        [SerializeField]
        private Vector3 startTargetPosition = Vector3.zero;
        private Vector3 targetMoveDirection = Vector3.zero;

        [Tooltip("표적 Y좌표 최댓값")]
        [SerializeField]
        private float maxYPos;
        [Tooltip("표적 Y좌표 최솟값")]
        [SerializeField]
        private float minYPos;

        [Tooltip("표적 이동속도 최솟값")]
        [SerializeField]
        private float minMoveSpeed;
        [Tooltip("표적 이동속도 최댓값")]
        [SerializeField]
        private float maxMoveSpeed;
        private float currentSpeed;

        private Animator hitAnimator;
        private SpriteRenderer targetSprite;

        private bool isTargetHit = false;
        private int hitNumber = 0;

        private bool IsHit = false;

        void Start()
        {
            TargetConrollerInit();
        }

        void Update()
        {
            TargetMoving();
        }

        public void TargetConrollerInit()
        {
            this.targetSelection = GetComponent<TargetSelection>();
            this.targetSprite = this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
            this.targetSprite.sprite = targetSelection.GetRamdomTargetSprite();
            this.hitAnimator = GetComponent<Animator>();
            this.hitNumber = 0;
            ResetTarget();
        }

        public void SetTargetSpeed(float value)
        {
            this.currentSpeed = value;
        }

        private void TargetMoving()
        {
            if (this.uiAnimation.GetIsSceneLock() == true)
            {
                return;
            }

            this.GetComponent<Transform>().position = this.GetComponent<Transform>().position + this.targetMoveDirection * this.currentSpeed * Time.deltaTime;
            ChangeMoveDirection();
        }

        private void SetPosition()
        {
            this.GetComponent<Transform>().position = this.startTargetPosition;
        }
        private void SetRandomSpeed()
        {
            this.currentSpeed = Random.Range(this.minMoveSpeed, this.maxMoveSpeed);  
            //SetTargetSpeed(this.targetSprite.sprite.name);
        }

        private void ResetTarget()
        {
            this.IsHit = false;
            this.targetMoveDirection = Vector3.down;
            this.targetSprite.sprite = targetSelection.GetRamdomTargetSprite();
            SetPosition();
            SetIstargetHit(false);
            SetRandomSpeed();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            switch (collision.gameObject.name)
            {
                case "EndLineRight":
                    ResetTarget();
                    break;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {

            if(this.IsHit == true)
            {
                return;
            }

            switch (collision.gameObject.name)
            {
                case "Ball(Clone)":
                    SetRadomEffect();
                    this.hitNumber = this.hitNumber + 1;
                    this.IsHit = true;
                    this.targetMoveDirection = Vector3.right;
                    break;           
            }
        }

        private void SetRadomEffect()
        {
            int tempRandom = Random.Range(0, 2);
            switch(tempRandom)
            {
                case 0:
                    if (GameManager.instance != null)
                    {
                        GameManager.instance.PlaySE(SEDatabaseManager.instance.GetSEByName("Hit"));
                    }
                    this.hitAnimator.SetBool("Hit", true);
                    break;
                case 1:
                    if (GameManager.instance != null)
                    {
                        GameManager.instance.PlaySE(SEDatabaseManager.instance.GetSEByName("Explosion"));
                    }
                    this.hitAnimator.SetBool("SpecialHit", true);
                    break;
                default:
                    if (GameManager.instance != null)
                    {
                        GameManager.instance.PlaySE(SEDatabaseManager.instance.GetSEByName("Hit"));
                    }
                    this.hitAnimator.SetBool("Hit", true);
                    break;
            }
        }

        private void SetIstargetHit(bool ishit)
        {
            this.isTargetHit = ishit;
        }

        public bool GetIstargetHit()
        {
            return this.isTargetHit;
        }

        public int GetHitNumber()
        {
            return this.hitNumber;
        }

        public void ChangeMoveDirection()
        {
            if(this.gameObject.transform.position.y >= this.maxYPos)
            {
                this.targetMoveDirection = Vector3.down;
            }
            else if(this.gameObject.transform.position.y <= this.minYPos)
            {
                this.targetMoveDirection = Vector3.up;
            }

        }

        public void SetTargetSpeed(string targetName)
        {

            switch(targetName)
            {
                case "SD_Red":
                    this.currentSpeed = this.maxMoveSpeed;
                    break;
                case "SD_Blue":
                    this.currentSpeed = this.maxMoveSpeed;
                    break;
                case "SD_Green":
                    this.currentSpeed = this.maxMoveSpeed;
                    break;
                case "SD_Pink":
                    this.currentSpeed = this.maxMoveSpeed;
                    break;
                case "SD_Purple":
                    this.currentSpeed = this.maxMoveSpeed;
                    break;
                case "SD_darmi2":
                    this.currentSpeed = this.maxMoveSpeed;
                    break;
            }


        }


    }
}
