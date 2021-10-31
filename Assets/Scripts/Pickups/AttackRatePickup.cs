using UnityEngine;

namespace Pickups
{
    public class AttackRatePickup : MonoBehaviour
    {
        [SerializeField] private float boostTime;
        [SerializeField] private float boostAmount;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<Player>().AttackRateBoost(boostTime,boostAmount);
                Destroy(this.gameObject);
            }
        }
    }
}
