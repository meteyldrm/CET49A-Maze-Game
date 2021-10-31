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
}
