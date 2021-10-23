using System;
using UnityEngine;

public class GravityController : Singleton<GravityController> 
{
    


    public Vector3 gravity;
    public Vector2 normalizedGravity;
    public float angle;

    private void Update() {
        gravity = Input.acceleration;
        normalizedGravity = new Vector2(gravity.x, gravity.y).normalized;
        angle = (float)Math.Asin(-normalizedGravity.x);
    }
}