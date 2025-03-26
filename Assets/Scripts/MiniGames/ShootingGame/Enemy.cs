using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingGame
{

    public class Enemy : MonoBehaviour
    {

        [SerializeField]
        private float moveSpeed = 10;
        [SerializeField]
        private int maxHP = 10;
        private int currentHP = 10;

        [SerializeField]
        private int enemyScore = 100;
        [SerializeField]
        private int enemyPowerGauge = 2;

        private ShootingGame.Character player;

        private ShootingGame.EnemyBallGenerator enemyBallGenerator;
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private GameObject lifeItem;
        [SerializeField]
        private GameObject powerItem;

        private Color hitColor = new Color(200f, 0f, 0f, 0.5f);
        private Color resetColor = new Color(255f, 255f, 255f, 1.0f);

        private void Awake()
        {
            EnemyInit();
        }

        private void OnEnable()
        {
            ResetEnemyHP();
        }

        void Update()
        {
            ShootBall();
        }

        public void EnemyInit()
        {
            this.spriteRenderer = GetComponent<SpriteRenderer>();
            this.enemyBallGenerator = GetComponent<ShootingGame.EnemyBallGenerator>();
            this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<ShootingGame.Character>();
        }

        public void ResetEnemyHP()
        {
            this.currentHP = this.maxHP;
        }

        public void SetEnemyHP()
        {
            switch(this.gameObject.name)
            {
                case "ExtraIrisSD(Clone)":
                    this.currentHP = this.maxHP;
                    break;
                case "ExtraLeonardSD(Clone)":
                     this.currentHP = this.maxHP;
                    break;
                case "KarlSD(Clone)":
                    this.currentHP = this.maxHP;
                    break;
                case "IrisSD(Clone)":
                    this.currentHP = this.maxHP;
                    break;
                default:
                    this.currentHP = this.maxHP;
                    break;

            }
        }

        public int GetEnemyHP()
        {
            return this.currentHP;
        }

        public float GetMoveSpeed()
        {
            return this.moveSpeed;
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

        public void DestoryEnemy()
        {
            if(this.currentHP <= 0)
            {
                EnemyDead();
            }
        }

        public void EnemyDead()
        {
            this.player.SetAddPlayerScore(this.enemyScore);
            this.player.SetAddPlayerPowerGauge(this.enemyPowerGauge);
            DropItem();
            this.gameObject.SetActive(false);
            this.enemyBallGenerator.PlayBallHitEffect(this.transform.position, this.name);
        }


        public void HitImage()
        {
            SetHitImage();
            Invoke("ResetHitImage", 0.1f);
        }

        public void SetHitImage()
        {
            this.spriteRenderer.color = this.hitColor;
        }

        public void ResetHitImage()
        {
            this.spriteRenderer.color = this.resetColor;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.tag == "Level")
            {
                this.gameObject.SetActive(false);
            }
            else if(collision.gameObject.tag == "Ball")
            {
                HitBall(collision.gameObject.GetComponent<ShootingGame.Ball>().GetBallDamage());
                collision.gameObject.SetActive(false);
            }
        }


        private void ShootBall()
        {        
            if (this.enemyBallGenerator.IsShootBallDelay() == true)
            {
                return;
            }

            if(this.gameObject.name == "ExtraIrisSD(Clone)")
            {
                this.enemyBallGenerator.MakeSmallBall(this.transform);
            }
            else if (this.gameObject.name == "ExtraLeonardSD(Clone)")
            {
                this.enemyBallGenerator.MakeDoubleSmallBall(this.transform);
            }
            else if (this.gameObject.name == "KarlSD(Clone)")
            {
                this.enemyBallGenerator.MakeBigBall(this.transform);
            }
            else if (this.gameObject.name == "IrisSD(Clone)")
            {
                this.enemyBallGenerator.MakeDoubleBigBall(this.transform);
            }
            else
            {
                this.enemyBallGenerator.MakeSmallBall(this.transform);
            }

            this.enemyBallGenerator.ResetDelayTime();
        }

        private void DropItem()
        {
            if(this.gameObject.name == "KarlSD(Clone)")
            {
                ShootingGame.ObjectManager tempObjectManager = this.gameObject.GetComponent<ShootingGame.EnemyBallGenerator>().objectManager;
                GameObject item = tempObjectManager.CreateObject("LifeItem");
                item.transform.position = this.transform.position;
            }
            else if(this.gameObject.name == "IrisSD(Clone)")
            {
                ShootingGame.ObjectManager tempObjectManager = this.gameObject.GetComponent<ShootingGame.EnemyBallGenerator>().objectManager;
                GameObject item = tempObjectManager.CreateObject("PowerItem");
                item.transform.position = this.transform.position;
            }

        }

     
    }
}