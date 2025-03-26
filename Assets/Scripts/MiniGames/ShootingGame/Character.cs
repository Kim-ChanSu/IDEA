using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingGame
{

    public class Character : MonoBehaviour
    {
        [SerializeField]
        private ShootingGame.Follower follower;

        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private Transform startPosition;

        private Color hitColor = new Color(200f, 0f, 0f, 0.5f);
        private Color resetColor = new Color(255f, 255f, 255f, 1.0f);
        private Color exModeColor = new Color(255f, 144f, 0f, 1.0f);

        private ShootingGame.PlayerBallGenerator generator;

        [SerializeField]
        private float moveSpeed;
        [SerializeField]
        private int ballPower = 1;
        private int maxBallPower = 5;
        [SerializeField]
        private int respawnTime = 1;
        [SerializeField]
        private int playerHP = 100;
        [SerializeField]
        private int playerPowerGauge = 0;
        [SerializeField]
        private int score = 0;

        private bool isHit = false;

        private bool isReachTop;
        private bool isReachBottom;
        private bool isReachRight;
        private bool isReachLeft;

        private readonly string ENEMYBALL = "Tile";
        private readonly string ENEMYCHARACTER = "Character";

        void Start()
        {
            CharacterInit();
        }

        void Update()
        {
            Move();
            ShootBall();
        }

        public void CharacterInit()
        {
            if(this.generator == null)
            {
                this.generator = GetComponent<ShootingGame.PlayerBallGenerator>();
            }
   
            if(this.spriteRenderer == null)
            {
                this.spriteRenderer = GetComponent<SpriteRenderer>();
            }

            this.score = 0;
        }

        public void Hit()
        {
            if(this.isHit == true)
            {
                return;
            }

            this.isHit = true;
            SetHitImage();
            SetAddPlayerHP(-30);
            Invoke("ResetHit", respawnTime);
        }

        public void SetHitImage()
        {
            this.spriteRenderer.color = this.hitColor;
        }

        public void ResetHitImage()
        {
            if (this.GetPlayerHP() <= 10) // EXmode
            {
                this.spriteRenderer.color = this.exModeColor;
            }
            else
            {
                this.spriteRenderer.color = this.resetColor;
            }
        }

        public void SetPlayerHP(int value)
        {
            this.playerHP = value;

            if(this.playerHP > 100)
            {
                this.playerHP = 100;
            }
        }

        public bool GetIsHit()
        {
            return this.isHit;
        }

        public void ResetHit()
        {
            ResetHitImage();
            this.isHit = false;
        }

        public int GetPlayerHP()
        {
            return this.playerHP;
        }

        public void SetAddPlayerHP(int value)
        {
            this.playerHP = this.playerHP + value;

            if (this.playerHP > 100)
            {
                this.playerHP = 100;
            }
        }

        public void SetAddPlayerScore(int value)
        {
            this.score = this.score + value;
        }

        public int GetPlayerScore()
        {
            return this.score;
        }

        public void SetAddPlayerPowerGauge(int value)
        {
            this.playerPowerGauge = this.playerPowerGauge + value;
        }

        public int GetPlayerPowerGauge()
        {
            return this.playerPowerGauge;
        }

        private void Move()
        {
            float xPos = Input.GetAxisRaw("Horizontal");

            if(((xPos == 1.0f) && (this.isReachRight == true)) || ((xPos == -1.0f) && (this.isReachLeft == true)))
            {
                xPos = 0;
            }

            float yPos = Input.GetAxisRaw("Vertical");

            if (((yPos == 1.0f) && (this.isReachTop == true)) || ((yPos == -1.0f) && (this.isReachBottom == true)))
            {
                yPos = 0;
            }

            Vector3 currentPos = this.transform.position;
            Vector3 nextMovePos = new Vector3(xPos, yPos, 0) * this.moveSpeed * Time.unscaledDeltaTime;

            this.transform.position = currentPos + nextMovePos;

            if(Input.GetKey(KeyCode.Z))
            {
                Time.timeScale = 0.5f;
            }
            else
            {
                Time.timeScale = 1f;
            }

        }

        public void ResetPosition()
        {
            this.transform.position = this.startPosition.position;
            this.follower.FollowerResetPositon(); 
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            CheckCharacterEnterCollision(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            CheckCharacterExitCollision(collision);
        }

        private void CheckCharacterEnterCollision(Collider2D collision)
        {
            if (collision.name == "Top")
            {
                this.isReachTop = true;
            }

            if (collision.name == "Bottom")
            {
                this.isReachBottom = true;
            }

            if (collision.name == "Right")
            {
                this.isReachRight = true;
            }

            if (collision.name == "Left")
            {
                this.isReachLeft = true;
            }

            if(collision.gameObject.tag == this.ENEMYBALL)
            {
                Hit();
            }

            if(collision.gameObject.tag == this.ENEMYCHARACTER)
            {
                Hit();
            }

            if (collision.gameObject.name == "Power_Item(Clone)")
            {
                collision.gameObject.SetActive(false);
                this.ballPower = this.ballPower + 1;

                if(this.ballPower > this.maxBallPower)
                {
                    this.ballPower = this.maxBallPower;
                }
            }

            if(collision.gameObject.name == "HP_Item(Clone)")
            {
                collision.gameObject.SetActive(false);
                this.SetAddPlayerHP(50);
            }

        }

        private void CheckCharacterExitCollision(Collider2D collision)
        {
            if (collision.name == "Top")
            {
                this.isReachTop = false;
            }

            if (collision.name == "Bottom")
            {
                this.isReachBottom = false;
            }

            if (collision.name == "Right")
            {
                this.isReachRight = false;
            }

            if (collision.name == "Left")
            {
                this.isReachLeft = false;
            }
        }

        private void ShootBall()
        {
            if(Input.GetButton("Fire1") == false)
            {
                return;
            }

            if(this.generator.IsShootBallDelay() == true)
            {
                return;
            }

            ShootBallPowerCount(this.ballPower);
            this.generator.ResetDelayTime();
        }

        private void ShootBallPowerCount(int ballnum)
        {
            switch(ballnum)
            {
                case 1:
                    this.generator.MakeSmallBall(this.transform);
                    break;
                case 2:
                    this.generator.MakeDoubleSmallBall(this.transform);
                    break;
                case 3:
                    this.generator.MakeTripleSmallBall(this.transform);
                    break;
                case 4:
                    this.generator.MakeBigBall(this.transform);
                    break;
                case 5:
                    this.generator.MakeDoubleBigBall(this.transform);
                    break;
                default:
                    this.generator.MakeSmallBall(this.transform);
                    break;
            }
        }

    }
}
