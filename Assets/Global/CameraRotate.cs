using UnityEngine;

namespace Global {
    public class CameraRotate : MonoBehaviour {

        private void Update() {
            transform.rotation = Input.gyro.attitude;
        }
    }
}
