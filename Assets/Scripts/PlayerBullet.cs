using UnityEngine;

public class PlayerBullet : MonoBehaviour
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
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
            Destroy(this.gameObject);
        }
        else if(other.gameObject.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
}
