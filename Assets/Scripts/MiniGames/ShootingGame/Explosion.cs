using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingGame
{

    public class Explosion : MonoBehaviour
    {

        private Animator animator;

        private void Awake()
        {
            ExplosionInit();
        }

        private void OnEnable()
        {
            Invoke("EndEffect", 0.4f);
        }

        public void ExplosionInit()
        {
            this.animator = GetComponent<Animator>();
        }

        public void PlayEffect(string name)
        {         

            /*switch (name)
            {
                case "LeonardSD(Clone)":
                    break;
                case "ExtraIrisSD(Clone)":
                    break;
                case "ExtraLeonardSD(Clone)":
                    break;
                case "KarlSD(Clone)":
                    break;
                case "IrisSD(Clone)":
                    break;
                case "Player":
                    break;
                default:
                    break;
            }*/
        }

        private void EndEffect()
        {
            this.gameObject.SetActive(false);
        }

    }

}