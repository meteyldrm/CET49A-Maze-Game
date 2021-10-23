using System;
using UnityEngine;

public class GravityController : Singleton<GravityController> 
{
    


    public Vector3 gravity => Input.acceleration;
    public Vector2 normalizedGravity => new Vector2(gravity.x, gravity.y).normalized;
    public float angle => (float)Math.Acos(normalizedGravity.y);
    
    
}