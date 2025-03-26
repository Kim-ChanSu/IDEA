using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningMinigame
{

    public class Hurdle : MonoBehaviour
    {

        [SerializeField]
        private GameObject startPoint;
        private Quaternion startRotation;
        private Quaternion hitRotation;
        private bool IsHit;

        [SerializeField]
        private RunningMinigame.HurdleController hurdleController;

        private void OnEnable()
        {
            HurdleInit();
        }

        void Start()
        {
            HurdleInit();
        }   

        private void HurdleInit()
        {
            this.startRotation = Quaternion.Euler(0f, 0f, 0f);
            this.hitRotation = Quaternion.Euler(0f, 0f, -45f);
            this.transform.rotation = this.startRotation;
            this.IsHit = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.gameObject.tag == "Finish")
            {
                
            }

            if (collision.gameObject.tag == "Player")
            {
                this.transform.rotation = this.hitRotation;
                if (GameManager.instance != null)
                {
                    GameManager.instance.PlaySE(SEDatabaseManager.instance.GetSEByName("Hit"));
                }
                this.hurdleController.HurdleSettingReset();
                IsHit = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Finish")
            {
                this.transform.rotation = this.startRotation;

                if(IsHit == true)
                {
                    this.hurdleController.HurdleSettingReset(); 
                }
            }

        }

    }
}