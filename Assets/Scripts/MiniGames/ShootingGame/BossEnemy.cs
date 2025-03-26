using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingGame
{

    public class BossEnemy : MonoBehaviour
    {
        [SerializeField]
        Rigidbody2D bossRigid;

        [SerializeField]
        private float maxDelayTime;
        private float currentTime;

        [SerializeField]
        private int maxHP = 100;
        [SerializeField]
        private int currentHP = 100;
        [SerializeField]
        private int enemyScore = 5000;

        private int patternIndex = -1;
        private int curPatternCount;
        [SerializeField]
        private int[] maxPatternIndex;

        private ShootingGame.Character player;
        private ShootingGame.ObjectManager objectManager;
        private ShootingGame.EnemyGenerator enemyGenerator;

        private SpriteRenderer spriteRenderer;
        private Color hitColor = new Color(200f, 0f, 0f, 0.5f);
        private Color resetColor = new Color(255f, 255f, 255f, 1.0f);

        private void OnEnable()
        {
            ResetBossEnemy();
            Invoke("StopPosition", 3.5f);          
        }

        void Start()
        {
            BossEnemyInit();
        }

        void Update()
        {
            DelayTime();

            if (IsShootBallDelay() == true)
            {
                return;
            }

            ResetDelayTime();
        }

        public void ResetBossEnemy()
        {
            this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<ShootingGame.Character>();
            this.objectManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ShootingGame.ObjectManager>();
            this.enemyGenerator = GameObject.FindGameObjectWithTag("GameController").GetComponent<ShootingGame.EnemyGenerator>();
        }

        public void BossEnemyInit()
        {
            this.spriteRenderer = GetComponent<SpriteRenderer>();
            this.bossRigid = GetComponent<Rigidbody2D>();
            this.currentHP = this.maxHP;
        }

        private void StopPosition()
        {
            if(this.gameObject.activeSelf == false)
            {
                return;
            }

            this.bossRigid = GetComponent<Rigidbody2D>();
            this.bossRigid.velocity = Vector3.zero;

            if(this.currentHP > 0)
            {
                Invoke("BossBehaviourPattern", 2f);
            }
           
        }

        private void BossBehaviourPattern()
        {
            if(this.currentHP <= 0)
            {
                return;
            }

            this.curPatternCount = 0;

            if (this.patternIndex == 3)
            {
                this.patternIndex = 0;
            }
            else
            {
                this.patternIndex = this.patternIndex + 1;
            }

            switch(this.patternIndex)
            {
                case 0:
                    FireFoward();
                    break;
                case 1:
                    FireShot();
                    break;
                case 2:
                    FireArc();
                    break;
                case 3:
                    FireAround();
                    break;
            }

        }

        private void FireFoward()
        {
            if (this.currentHP <= 0)
            {
                return;
            }

            GameObject bossBall = this.objectManager.CreateObject("BossBallTypeA");
            bossBall.transform.position = transform.position;

            GameObject rightbossBall = this.objectManager.CreateObject("BossBallTypeA");
            rightbossBall.transform.position = transform.position + Vector3.down * 3.5f;

            GameObject leftbossBall = this.objectManager.CreateObject("BossBallTypeA");
            leftbossBall.transform.position = transform.position + Vector3.up * 3.5f;

            Rigidbody2D rigidbody2D = bossBall.GetComponent<Rigidbody2D>();
            Rigidbody2D rightBallrigid = rightbossBall.GetComponent<Rigidbody2D>();
            Rigidbody2D leftBallrigid = leftbossBall.GetComponent<Rigidbody2D>();

            rigidbody2D.AddForce(Vector2.left * 6f, ForceMode2D.Impulse);
            rightBallrigid.AddForce(Vector2.left * 6f, ForceMode2D.Impulse);
            leftBallrigid.AddForce(Vector2.left * 6f, ForceMode2D.Impulse);

            this.curPatternCount++;

            if(this.curPatternCount<this.maxPatternIndex[this.patternIndex])
            {
                Invoke("FireFoward", 1.2f);
            }
            else
            {
                Invoke("BossBehaviourPattern", 3f);
            }
            
        }

        private void FireShot()
        {
            if (this.currentHP <= 0)
            {
                return;
            }

            for (int i = 0; i < 5; i++)
            {
                GameObject bossBall = this.objectManager.CreateObject("BossBallTypeA");
                bossBall.transform.position = transform.position;

                Rigidbody2D rigidbody2D = bossBall.GetComponent<Rigidbody2D>();
                Vector3 playerDirection = player.transform.position - this.transform.position;
                Vector3 tempDirection = new Vector3(i * -0.4f, i * -0.4f, 0);

                playerDirection = playerDirection + tempDirection;

                rigidbody2D.AddForce(playerDirection.normalized * 3f, ForceMode2D.Impulse);
            }

            this.curPatternCount++;

            if (this.curPatternCount < this.maxPatternIndex[this.patternIndex])
            {
                Invoke("FireShot", 1f);
            }
            else
            {
                Invoke("BossBehaviourPattern", 3f);
            }
        }

        private void FireArc()
        {
            if (this.currentHP <= 0)
            {
                return;
            }

            GameObject bossBall = this.objectManager.CreateObject("EnemyBallTypeB");
            bossBall.transform.position = transform.position;
            Rigidbody2D rigidbody2D = bossBall.GetComponent<Rigidbody2D>();
            
            Vector2 tempDirection = new Vector2(-1, Mathf.Sin(Mathf.PI * 1.3f * this.curPatternCount / this.maxPatternIndex[this.patternIndex]));
            rigidbody2D.AddForce(tempDirection.normalized * 10f, ForceMode2D.Impulse);

            this.curPatternCount++;
            if (this.curPatternCount < this.maxPatternIndex[this.patternIndex])
            {
                Invoke("FireArc", 0.15f);
            }
            else
            {
                Invoke("BossBehaviourPattern", 3f);
            }
        }

        private void FireAround()
        {
            if (this.currentHP <= 0)
            {
                return;
            }

            int roundNum = 40;

            if(this.curPatternCount % 2 == 0)
            {
                roundNum = 40;
            }
            else
            {
                roundNum = 30;
            }

            for (int i = 0; i < roundNum; i++) 
            {
                GameObject bossBall = this.objectManager.CreateObject("EnemyBallTypeA");
                bossBall.transform.position = this.transform.position;
                Rigidbody2D rigidbody2D = bossBall.GetComponent<Rigidbody2D>();

                Vector2 tempDirection = new Vector2(Mathf.Cos(Mathf.PI * 2f * i / roundNum), Mathf.Sin(Mathf.PI * 2f * i / roundNum));
                rigidbody2D.AddForce(tempDirection.normalized * 10f, ForceMode2D.Impulse);

            }


            this.curPatternCount++;
          
            if (this.curPatternCount < this.maxPatternIndex[this.patternIndex])
            {
                Invoke("FireAround", 1.5f);
            }
            else
            {
                Invoke("BossBehaviourPattern", 3f);
            }
        }

        private void DelayTime()
        {
            this.currentTime = this.currentTime + Time.deltaTime;
        }

        private void ResetDelayTime()
        {
            this.currentTime = 0f;
        }

        private bool IsShootBallDelay()
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

        public void HitBall(int damage)
        {
            if (this.currentHP <= 0)
            {
                return;
            }

            this.currentHP = this.currentHP - damage;
            HitImage();
            DestoryEnemy();
        }
        private void HitImage()
        {
            SetHitImage();
            Invoke("ResetHitImage", 0.1f);
        }

        private void SetHitImage()
        {
            this.spriteRenderer.color = this.hitColor;
        }

        private void ResetHitImage()
        {
            this.spriteRenderer.color = this.resetColor;
        }

        private void DestoryEnemy()
        {
            if (this.currentHP <= 0)
            {
                EnemyDead();
            }
        }

        private void EnemyDead()
        {
            this.objectManager.AllEnemyBallClear();
            this.player.SetAddPlayerScore(this.enemyScore);
            this.player.SetAddPlayerPowerGauge(100);
            this.enemyGenerator.StageEnd();
            this.gameObject.SetActive(false);
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {           
            if (collision.gameObject.tag == "Ball")
            {
                HitBall(collision.gameObject.GetComponent<ShootingGame.Ball>().GetBallDamage());
                collision.gameObject.SetActive(false);
            }
        }

    }


}