using UnityEngine;

public class Spike : MonoBehaviour
{
    public float damageAmount;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().TakeDamage(damageAmount);
        }
    }
}
