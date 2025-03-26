using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingGame
{
    public class Item : MonoBehaviour
    {
        [SerializeField]
        private string itemName;

        private Rigidbody2D itemRigidbody;

        private void OnEnable()
        {
            ItemInit();
        }

        public void ItemInit()
        {
            this.itemRigidbody = GetComponent<Rigidbody2D>();
            this.itemRigidbody.velocity = Vector2.left * 2;
        }
    }
}