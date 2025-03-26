using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThrowingMinigame
{
    public class CharacterConroller : MonoBehaviour
    {
        private ThrowingMinigame.BallController miniCharacterBallController;
        [SerializeField]
        private RunningMinigame.UIAnimation uiAnimation;

        private Vector3 moveUpDirection = Vector3.up;
        private Vector3 moveDownDirection = Vector3.down;

        private bool isGoOverTop = false;
        private bool isGoOverBottom = false;

        private bool isShooting = false;
        private bool isMoving = false;

        [SerializeField]
        private Animator charaterAnimator;

        [SerializeField]
        private float moveSpeed = 0.0f;

        private readonly int clickRight = 0;

        void Start()
        {
            CharacterConrollerInit();
        }

        void Update()
        {
            InputCommand();
        }

        private void CharacterConrollerInit()
        {
            this.miniCharacterBallController = GetComponent<ThrowingMinigame.BallController>();
            this.isGoOverTop = false;
            this.isGoOverBottom = false;
            this.isShooting = false;
            this.isMoving = false;
        }

        private void InputCommand()
        {
            if (this.uiAnimation.GetIsSceneLock() == true)
            {
                return;
            }

            float yPos = Input.GetAxisRaw("Vertical");

            if (((yPos == 1.0f) && (this.isGoOverTop == true)) || ((yPos == -1.0f) && (this.isGoOverBottom == true)))
            {
                charaterAnimator.SetBool("move", false);
                yPos = 0;
                this.isMoving = false;
            }
            else if (yPos <= 0.1f && yPos >= -0.1f)
            {
                charaterAnimator.SetBool("move", false);
                this.isMoving = false;
            }
            else
            {
                charaterAnimator.SetBool("move", true);
                this.isMoving = true;
            }

            Vector3 currentPos = this.transform.position;
            Vector3 nextMovePos = new Vector3(0, yPos, 0) * this.moveSpeed * Time.unscaledDeltaTime;
            this.transform.position = currentPos + nextMovePos;

            if (((Input.GetKeyDown(KeyCode.Space)) && (isShooting == false)) || (Input.GetMouseButtonDown(clickRight) && (isShooting == false)))
            {

                if(this.isMoving == true)
                {
                    return;
                }

                miniCharacterBallController.CreatBall();
                isShooting = true;
                charaterAnimator.SetBool("move", false);
                charaterAnimator.SetBool("throw", true);
            }

            SetIsShooting();
        }

        private void SetIsShooting()
        {
            isShooting = !miniCharacterBallController.GetShootLock();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            switch (collision.gameObject.name)
            {
                case "EndLineTop":
                    isGoOverTop = true;
                    break;
                case "EndLineBottom":
                    isGoOverBottom = true;
                    break;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            switch (collision.gameObject.name)
            {
                case "EndLineTop":
                    isGoOverTop = true;
                    break;
                case "EndLineBottom":
                    isGoOverBottom = true;
                    break;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            switch (collision.gameObject.name)
            {
                case "EndLineTop":
                    isGoOverTop = false;
                    break;
                case "EndLineBottom":
                    isGoOverBottom = false;
                    break;
            }
        }

        public void ThrowEnd()
        {
            charaterAnimator.SetBool("throw", false);
            miniCharacterBallController.BallCorrection();
        }

        public void ReadyToThrow()
        {
            miniCharacterBallController.ShootingBallHide();
        }

        public void ThrowStart()
        {
            miniCharacterBallController.ShootingBallReveal();
        }

        public bool GetisShooting()
        {
            return isShooting;
        }

    }
}