using System;
using TMPro;
using UnityEngine;

namespace SceneScripts.SampleScene {
    public class BallScript : MonoBehaviour {
        private Vector3 gravityVector;
        public float acceleration;
        public TextMeshProUGUI tmpx;
        public TextMeshProUGUI tmpy;

        public Rigidbody rb;
        
        private void Start() {
            rb = GetComponent<Rigidbody>();
            gravityVector = Input.acceleration;
            gravityVector.z = 0;
        }

        private void FixedUpdate() {
            //Physics stuff

            gravityVector.x = Input.acceleration.x;
            gravityVector.y = Input.acceleration.y;

            tmpx.SetText("Gravity X: " + gravityVector.x);
            tmpy.SetText("Gravity Y: " + gravityVector.y);
            
            // clamp acceleration vector to unit sphere
            if (gravityVector.sqrMagnitude > 1)
                gravityVector.Normalize();

            // Make it move 10 meters per second instead of 10 meters per frame...
            gravityVector *= Time.fixedDeltaTime;

            rb.velocity += gravityVector * acceleration;
        }
    }
}