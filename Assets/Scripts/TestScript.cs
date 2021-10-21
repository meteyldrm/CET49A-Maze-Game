using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private float turnSpeed;
    
    void Update()
    {

        transform.Rotate(Vector3.forward * (turnSpeed * Time.deltaTime * Input.acceleration.x));
        
        
        // if (Input.GetKey(KeyCode.A))
        // {
        //     transform.Rotate(Vector3.forward * (turnSpeed * Time.deltaTime));
        // }
        //
        // if (Input.GetKey(KeyCode.D))
        // {
        //     transform.Rotate(Vector3.back * (turnSpeed * Time.deltaTime));
        // }
    }
}
