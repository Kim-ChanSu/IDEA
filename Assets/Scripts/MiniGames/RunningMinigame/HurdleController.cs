using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningMinigame
{

    public class HurdleController : MonoBehaviour
    {
        [Tooltip("시작 속도")]
        [SerializeField]
        private float startMoveSpeed = 6.0f;
        private float currentMoveSpeed = 6.0f;
        private float maxMoveSpeed;
        private float addMoveSpeed;

        [Tooltip("장애물 생성 위치")]
        [SerializeField]
        private GameObject startPoint;
        private Vector3 moveVector = Vector2.left;

        [SerializeField]
        private GameObject[] hurdleGameObjects;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private GameObject[] characters;

        private int getPoint;
        private int currentPoint;

        [SerializeField]
        private RunningMinigame.UIAnimation uiAnimation;

        [SerializeField]
        private GameObject[] backgrounds;


        void Start()
        {
            HurdleControllerInit();
            SettingHurdle();
        }

        void Update()
        {
            Move();
        }

        public void HurdleControllerInit()
        {
            this.transform.position = this.startPoint.transform.position;
            this.currentMoveSpeed = this.startMoveSpeed;
            this.maxMoveSpeed = this.startMoveSpeed * 2.5f;
            this.addMoveSpeed = this.startMoveSpeed * 0.2f;
            this.getPoint = 1;
            this.currentPoint = 0;
            SettingCharacterAnimator();
        }

        public int GetScore()
        {
            return this.currentPoint;
        }

        private void Move()
        {
            if (this.uiAnimation.GetIsSceneLock() == true)
            {
               return;
            }

            this.transform.position = this.transform.position + this.moveVector * this.currentMoveSpeed * Time.deltaTime;
        }

        public void SettingCharacterAnimator()
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (this.characters[i].activeSelf == true)
                {
                    this.animator = characters[i].GetComponent<Animator>();
                }
            }
        }

        public void HurdleSettingReset()
        {
            this.getPoint = 0;
            this.currentMoveSpeed = this.startMoveSpeed;
            this.animator.speed = 1f;

            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].GetComponent<RunningMinigame.BackGround>().ResetSpeed();
            }
        }

        private void SettingHurdle()
        {
            int activeNumber = Random.Range(1, hurdleGameObjects.Length);
            int caseNumber = Random.Range(0, 3);

            switch (activeNumber)
            {
                case 1:
                    SingleHurdle(caseNumber);
                    break;
                case 2:
                    DoubleHurdle(caseNumber);
                    break;
                default:
                    SingleHurdle(caseNumber);
                    break;

            }

        }

        private void SingleHurdle(int num)
        {
            for (int i = 0; i < hurdleGameObjects.Length; i++)
            {
                hurdleGameObjects[i].SetActive(false);
            }

            hurdleGameObjects[num].SetActive(true);
        }

        private void DoubleHurdle(int num)
        {
            for (int i = 0; i < hurdleGameObjects.Length; i++)
            {
                hurdleGameObjects[i].SetActive(false);
            }

            for (int i = 0; i < hurdleGameObjects.Length; i++)
            {
                hurdleGameObjects[i].SetActive(true);
            }

            hurdleGameObjects[num].SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision) // 스피트 가속
        {

            if (collision.gameObject.tag == "Finish")
            {
                this.transform.position = this.startPoint.transform.position;
                this.currentPoint = this.currentPoint + this.getPoint;

                for (int i = 0; i < backgrounds.Length; i++) 
                {
                    backgrounds[i].GetComponent<RunningMinigame.BackGround>().AddSpeed();
                }

                if (this.currentMoveSpeed < this.maxMoveSpeed)
                {
                    this.currentMoveSpeed = this.currentMoveSpeed + this.addMoveSpeed;
                    this.animator.speed = this.animator.speed + 0.25f;

                }
                else
                {
                    this.currentMoveSpeed = this.maxMoveSpeed;
                }
            }

            SettingHurdle();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Finish")
            {
                this.getPoint = 1;
            }
        }


    }
}
