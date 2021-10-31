using System;
using UnityEngine;

namespace Pickups
{
    public class HealPickup : MonoBehaviour
    {
        public float healAmount;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<Player>().Heal(healAmount);
                Destroy(this.gameObject);
            }
        }
    }
}
