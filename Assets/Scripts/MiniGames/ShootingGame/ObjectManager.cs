using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ShootingGame
{

    public class ObjectManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject bossEnemyTypeAPrefab;
        [SerializeField]
        private GameObject enemyTypeAPrefab;
        [SerializeField]
        private GameObject enemyTypeBPrefab;
        [SerializeField]
        private GameObject enemyTypeCPrefab;
        [SerializeField]
        private GameObject enemyTypeDPrefab;
        [SerializeField]
        private GameObject lifeItemPrefab;
        [SerializeField]
        private GameObject powerItemPrefab;
        [SerializeField]
        private GameObject playerBallTypeAPrefab;
        [SerializeField]
        private GameObject playerBallTypeBPrefab;
        [SerializeField]
        private GameObject playerFollowerBallPrefab;
        [SerializeField]
        private GameObject enemyBallTypeAPrefab;
        [SerializeField]
        private GameObject enemyBallTypeBPrefab;
        [SerializeField]
        private GameObject bossBallTypeAPrefab;
        [SerializeField]
        private GameObject bossBallTypeBPrefab;
        [SerializeField]
        private GameObject hitEffectPrefab;

        private GameObject[] bossEnemyTypeA;
        private GameObject[] enemyTypeA;
        private GameObject[] enemyTypeB;
        private GameObject[] enemyTypeC;
        private GameObject[] enemyTypeD;

        private GameObject[] lifeItem;
        private GameObject[] powerItem;

        private GameObject[] playerBallTypeA;
        private GameObject[] playerBallTypeB;

        private GameObject[] playerFollowerBall;

        private GameObject[] enemyBallTypeA;
        private GameObject[] enemyBallTypeB;

        private GameObject[] bossBallTypeA;
        private GameObject[] bossBallTypeB;

        private GameObject[] createTargetObject;
        private GameObject[] hitEffect;


        private void Awake()
        {
            ObjectManagerInit();
        }

        public void ObjectManagerInit()
        {
            try
            {
                this.bossEnemyTypeA = new GameObject[1];
                this.enemyTypeA = new GameObject[15];
                this.enemyTypeB = new GameObject[15];
                this.enemyTypeC = new GameObject[5];
                this.enemyTypeD = new GameObject[5];

                this.lifeItem = new GameObject[5];
                this.powerItem = new GameObject[5];
                this.playerBallTypeA = new GameObject[200];
                this.playerBallTypeB = new GameObject[100];
                this.playerFollowerBall = new GameObject[50];
                this.enemyBallTypeA = new GameObject[100];
                this.enemyBallTypeB = new GameObject[100];
                this.bossBallTypeA = new GameObject[100];
                this.bossBallTypeB = new GameObject[30];
                this.hitEffect = new GameObject[50];
            }
            catch (NullReferenceException ex)
            {
                Debug.LogError("ObjectManager에서 게임오브젝트 프리맵이 없습니다.");
                Debug.LogException(ex);
            }

            Generate();
        }

        private void Generate()
        {
            
            for (int i = 0; i < this.bossEnemyTypeA.Length; i++)
            {
                this.bossEnemyTypeA[i] = Instantiate(this.bossEnemyTypeAPrefab);
                this.bossEnemyTypeA[i].SetActive(false);
            }

            for (int i=0; i < this.enemyTypeA.Length; i++)
            {
                this.enemyTypeA[i] = Instantiate(this.enemyTypeAPrefab);
                this.enemyTypeA[i].SetActive(false);
            }

            for(int i=0; i < this.enemyTypeB.Length; i++)
            {
                this.enemyTypeB[i] = Instantiate(this.enemyTypeBPrefab);
                this.enemyTypeB[i].SetActive(false);
            }

            for (int i = 0; i < this.enemyTypeC.Length; i++)
            {
                this.enemyTypeC[i] = Instantiate(this.enemyTypeCPrefab);
                this.enemyTypeC[i].SetActive(false);
            }

            for (int i = 0; i < this.enemyTypeD.Length; i++)
            {
                this.enemyTypeD[i] = Instantiate(this.enemyTypeDPrefab);
                this.enemyTypeD[i].SetActive(false);
            }

            for (int i = 0; i < this.lifeItem.Length; i++)
            {
                this.lifeItem[i] = Instantiate(this.lifeItemPrefab);
                this.lifeItem[i].SetActive(false);
            }

            for(int i = 0; i < this.powerItem.Length; i++)
            {
                this.powerItem[i] = Instantiate(this.powerItemPrefab);
                this.powerItem[i].SetActive(false);
            }

            for(int i = 0; i < this.playerBallTypeA.Length; i++)
            {
                this.playerBallTypeA[i] = Instantiate(this.playerBallTypeAPrefab);
                this.playerBallTypeA[i].SetActive(false);
            }

            for(int i = 0; i < this.playerBallTypeB.Length; i++)
            {
                this.playerBallTypeB[i] = Instantiate(this.playerBallTypeBPrefab);
                this.playerBallTypeB[i].SetActive(false);
            }

            for(int i=0; i< this.playerFollowerBall.Length; i++)
            {
                this.playerFollowerBall[i] = Instantiate(this.playerFollowerBallPrefab);
                this.playerFollowerBall[i].SetActive(false);
            }

            for (int i = 0; i < this.enemyBallTypeA.Length; i++)
            {
                this.enemyBallTypeA[i] = Instantiate(this.enemyBallTypeAPrefab);
                this.enemyBallTypeA[i].SetActive(false);
            }

            for (int i = 0; i < this.enemyBallTypeB.Length; i++) 
            {
                this.enemyBallTypeB[i] = Instantiate(this.enemyBallTypeBPrefab);
                this.enemyBallTypeB[i].SetActive(false);
            }

            for (int i = 0; i < this.bossBallTypeA.Length; i++)
            {
                this.bossBallTypeA[i] = Instantiate(this.bossBallTypeAPrefab);
                this.bossBallTypeA[i].SetActive(false);
            }

            for (int i = 0; i < this.bossBallTypeB.Length; i++)
            {
                this.bossBallTypeB[i] = Instantiate(this.bossBallTypeBPrefab);
                this.bossBallTypeB[i].SetActive(false);
            }

            for(int i=0;i< this.hitEffect.Length; i++)
            {
                this.hitEffect[i] = Instantiate(this.hitEffectPrefab);
                this.hitEffect[i].SetActive(false);
            }

        }

        public GameObject CreateObject(string typeName)
        {

            switch(typeName)
            {
                case "BossEnemyTypeA":
                    this.createTargetObject = this.bossEnemyTypeA;
                    break;
                case "EnemyTypeA":
                    this.createTargetObject = this.enemyTypeA;
                        break;
                case "EnemyTypeB":
                    this.createTargetObject = this.enemyTypeB;
                        break;
                case "EnemyTypeC":
                    this.createTargetObject = this.enemyTypeC;
                    break;
                case "EnemyTypeD":
                    this.createTargetObject = this.enemyTypeD;
                    break;
                case "LifeItem":
                    this.createTargetObject = this.lifeItem;
                    break;
                case "PowerItem":
                    this.createTargetObject = this.powerItem;
                    break;
                case "PlayerBallTypeA":
                    this.createTargetObject = this.playerBallTypeA;
                    break;
                case "PlayerBallTypeB":
                    this.createTargetObject = this.playerBallTypeB;
                    break;
                case "PlayerFollowerBall":
                    this.createTargetObject = this.playerFollowerBall;
                    break;
                case "EnemyBallTypeA":
                    this.createTargetObject = this.enemyBallTypeA;
                    break;
                case "EnemyBallTypeB":
                    this.createTargetObject = this.enemyBallTypeB;
                    break;
                case "BossBallTypeA":
                    this.createTargetObject = this.bossBallTypeA;
                    break;
                case "BossBallTypeB":
                    this.createTargetObject = this.bossBallTypeB;
                    break;
                case "HitEffect":
                    this.createTargetObject = this.hitEffect;
                    break;
                default:
                    Debug.LogWarning("오브젝트타입 이름이 형식에 맞지 않습니다.");
                    break;

            }

            for (int i = 0; i < this.createTargetObject.Length; i++) 
            {
                if(this.createTargetObject[i].activeSelf == false)
                {
                    this.createTargetObject[i].SetActive(true);
                    return createTargetObject[i];
                }
            }

            return null;

        }

        public void AllEnemyBallClear()
        {
            for (int i = 0; i < this.enemyBallTypeA.Length; i++)
            {
                this.enemyBallTypeA[i].SetActive(false);
            }

            for (int i = 0; i < this.enemyBallTypeB.Length; i++)
            {
                this.enemyBallTypeB[i].SetActive(false);
            }

            for (int i = 0; i < this.bossBallTypeA.Length; i++)
            {
                this.bossBallTypeA[i].SetActive(false);
            }

            for (int i = 0; i < this.bossBallTypeB.Length; i++)
            {
                this.bossBallTypeB[i].SetActive(false);
            }

        }


    }
}
