using System;
using UnityEngine;

public class EnemySightRange : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponentInParent<Enemy>().playerIsInSight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponentInParent<Enemy>().playerIsInSight = false;
        }
    }
}
