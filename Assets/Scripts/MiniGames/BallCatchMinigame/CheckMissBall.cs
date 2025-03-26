using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallCatchMinigame
{
    public class CheckMissBall : MonoBehaviour
    {
        [SerializeField]
        private BallCatchMinigame.Judgment judgment;

        [SerializeField]
        private GameObject hitEffect;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.name == "Ball(Clone)")
            {
                if (GameManager.instance != null)
                {
                    GameManager.instance.PlaySE(SEDatabaseManager.instance.GetSEByName("Explosion"));
                }

                this.hitEffect.SetActive(true);
                this.judgment.ResetBallSpeed();
                Debug.Log("Miss");
            }
        }
                
    }
}