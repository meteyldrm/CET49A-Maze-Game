using UnityEngine;

namespace SceneScripts.SampleScene {
    public class BallScript : MonoBehaviour {
        public Vector3 gravityVector;
        public float acceleration = 2;
        
        private void Start() {
            gravityVector = Input.acceleration;
            gravityVector.z = 0;
        }

        private void FixedUpdate() {
            //Physics stuff

            gravityVector.x = Input.acceleration.x;
            gravityVector.y = Input.acceleration.y;
            
            // clamp acceleration vector to unit sphere
            if (gravityVector.sqrMagnitude > 1)
                gravityVector.Normalize();

            // Make it move 10 meters per second instead of 10 meters per frame...
            gravityVector *= Time.fixedDeltaTime;

            // Move object
            transform.Translate(gravityVector * acceleration);
        }
    }
}