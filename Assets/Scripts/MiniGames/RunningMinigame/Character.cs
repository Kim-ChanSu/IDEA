using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningMinigame
{
    public class Character : MonoBehaviour
    {
        [Tooltip("캐릭터 시작 위치")]
        [SerializeField]
        private GameObject startPoint;

        [Tooltip("시작 레일번호")]
        [Range (1,3)]
        [SerializeField]
        private int StartRailNumber;

        [Tooltip("위 아래 움직이는 Y축값")]
        [SerializeField]
        private float moveYPos;

        private Animator animator;

        [SerializeField]
        private RunningMinigame.UIAnimation uiAnimation;

        private bool IsCharacterMove = false;

        void Start()
        {
            CharacterControllerInit();
        }

        void Update()
        {
            Move();
        }


        public void CharacterControllerInit()
        {
            this.transform.position = this.startPoint.transform.position;
            this.animator = GetComponent<Animator>();
            this.IsCharacterMove = false;
            this.animator.SetBool("Idle", true);
        }


        private void Move()
        {
            if (this.uiAnimation.GetIsSceneLock() == true)
            {
                return;
            }
            else
            {
                this.animator.SetBool("Idle", false);
                this.animator.SetBool("Move", true);
            }

            if ((Input.GetKeyDown(KeyCode.W) == true) || (Input.GetKeyDown(KeyCode.UpArrow) == true))
            {
                if (this.StartRailNumber == 1)
                {
                    return;
                }

                this.StartRailNumber--;
                Vector3 MovePos = new Vector3(0, moveYPos, 0);
                this.transform.position = this.transform.position + MovePos;
            }

            if ((Input.GetKeyDown(KeyCode.S) == true) || (Input.GetKeyDown(KeyCode.DownArrow) == true))
            {
                if (this.StartRailNumber == 3)
                {
                    return;
                }
                this.StartRailNumber++;
                Vector3 MovePos = new Vector3(0, -moveYPos, 0);
                this.transform.position = this.transform.position + MovePos;
            }

            MoveAnimation();

        }

        private void MoveAnimation()
        {
            this.animator.SetBool("Move", true);
            this.IsCharacterMove = true;
        }

        public bool GetIsCharacterMove()
        {
            return this.IsCharacterMove;
        }

    }
}
