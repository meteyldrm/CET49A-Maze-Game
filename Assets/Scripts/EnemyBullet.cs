using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletDamage;
    
    private void Update()
    {
        var transform1 = transform;
        transform1.position+= transform1.right * (bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().TakeDamage(bulletDamage);
            Destroy(this.gameObject);
        }
        else if(other.gameObject.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
}
