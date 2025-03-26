using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace ShootingGame
{

    public class EnemyGenerator : MonoBehaviour
    {
        
        private string[] enemyGameObjects;
        [SerializeField]
        private Transform[] enemySpawnPoints;
        [SerializeField]
        private ShootingGame.ObjectManager objectManager;
        [SerializeField]
        private ShootingGame.UIManager uiManager;
        [SerializeField]
        private ShootingGame.Character playerCharacter;

        private List<ShootingGame.Spawn> enemySpawnData;
        private int enemySpawnIndex = 0;
        private bool isSpawnDone = false;

        private float nextSpawnDeleyTime;
        private float currentSpawnTime;

        private int currentStage = 0;
        private int MaxStage = 1;

        private readonly string FILEPATH = "ShootingGame/Level_";

        private void Awake()
        {
            EnemyGeneratorInit();
        }

        void Update()
        {
            DelayTime();
            SpawnEnemy();
        }

        public void EnemyGeneratorInit()
        {
            if(this.objectManager == null)
            {
                this.objectManager = GetComponent<ShootingGame.ObjectManager>();
            }

            if(this.uiManager == null)
            {
                this.uiManager = GetComponent<ShootingGame.UIManager>();
            }

            this.enemySpawnData = new List<ShootingGame.Spawn>();
            this.enemyGameObjects = new string[] { "BossEnemyTypeA","EnemyTypeA", "EnemyTypeB", "EnemyTypeC", "EnemyTypeD" };
            this.currentStage = 0;
            StageStart();

        }

        public void StageStart()
        {
            this.uiManager.SettingStartStageUI(this.currentStage);
            ReadEnemySpawnFile();
            this.uiManager.SettingFadeInStage();
        }

        public void StageEnd()
        {
            this.uiManager.SettingFadeOutStage();
            this.objectManager.AllEnemyBallClear();
            this.uiManager.SettingEndStageUI();
            this.playerCharacter.ResetPosition();
            this.currentStage = this.currentStage + 1;

            if(this.currentStage > this.MaxStage)
            {
                this.uiManager.GameOver();
            }
            else
            {
                Invoke("StageStart", 5f);
            }

        }

        private void DelayTime()
        {
            this.currentSpawnTime = this.currentSpawnTime + Time.deltaTime;
        }

        private void SpawnEnemy()
        {
            if((this.currentSpawnTime > this.nextSpawnDeleyTime) && (this.isSpawnDone == false))
            {
                CreatEnemy();
                this.currentSpawnTime = 0f;
            }   
        }

        private void CreatEnemy()
        {
            int enemyIndex = 0;
            switch(this.enemySpawnData[enemySpawnIndex].enemyType)
            {
                case "BossEnemyTypeA":
                    enemyIndex = 0;
                    break;
                case "EnemyTypeA":
                    enemyIndex = 1;
                    break;
                case "EnemyTypeB":
                    enemyIndex = 2;
                    break;
                case "EnemyTypeC":
                    enemyIndex = 3;
                    break;
                case "EnemyTypeD":
                    enemyIndex = 4;
                    break;
                default:
                    enemyIndex = 1;
                    break;
            }


            int enemyPosition = this.enemySpawnData[enemySpawnIndex].spawnPoint;

            GameObject tempEnemy = this.objectManager.CreateObject(this.enemyGameObjects[enemyIndex]);
            tempEnemy.transform.position = this.enemySpawnPoints[enemyPosition].transform.position;

            Rigidbody2D tempEnemyRigid = tempEnemy.GetComponent<Rigidbody2D>();
            Enemy enemy = tempEnemy.GetComponent<ShootingGame.Enemy>();

            if(enemy != null) // 일반 적이동 경로
            {
                SetEnemyMoveVector(enemyPosition, tempEnemyRigid, enemy.GetMoveSpeed());
            }
            else // 보스 이동 경로
            {
                SetEnemyMoveVector(enemyPosition, tempEnemyRigid, 1.0f);
            }

            this.enemySpawnIndex = this.enemySpawnIndex + 1;
            if (this.enemySpawnIndex >= this.enemySpawnData.Count)
            {            
                this.isSpawnDone = true;
                return;
            }

            this.nextSpawnDeleyTime = this.enemySpawnData[this.enemySpawnIndex].delayTime;

        }

        private void SetEnemyMoveVector(int enemyPositionNumber, Rigidbody2D enemyRigid, float moveSpeed)
        {
            if (enemyPositionNumber == 5 || enemyPositionNumber == 6)
            {
                enemyRigid.velocity = new Vector3(-1.0f, -moveSpeed, 0f);
            }
            else if (enemyPositionNumber == 7 || enemyPositionNumber == 8)
            {
                enemyRigid.velocity = new Vector3(-1.0f, moveSpeed, 0f);
            }
            else
            {
                enemyRigid.velocity = new Vector3(-moveSpeed, 0f, 0f);
            }
        }

       void ReadEnemySpawnFile()
       {
            this.enemySpawnData.Clear();
            this.enemySpawnIndex = 0;
            this.isSpawnDone = false;

            TextAsset textFile = Resources.Load(this.FILEPATH + this.currentStage.ToString()) as TextAsset;
            StringReader stringReader = new StringReader(textFile.text);

            while(stringReader != null)
            {
                string line = stringReader.ReadLine();
                Debug.Log(line);

                if(line == null)
                {
                    break;
                }

                Spawn tempSpawn = new Spawn();
                tempSpawn.delayTime = float.Parse(line.Split(',')[0]);
                tempSpawn.enemyType = line.Split(',')[1];
                tempSpawn.spawnPoint = int.Parse(line.Split(',')[2]);

                this.enemySpawnData.Add(tempSpawn);
            }

            stringReader.Close();

            if(this.enemySpawnData.Count == 0)
            {
                return;
            }

            this.nextSpawnDeleyTime = this.enemySpawnData[0].delayTime;
       }

    }
}