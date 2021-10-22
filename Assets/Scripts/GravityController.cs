using System;
using UnityEngine;

public class GravityController : MonoBehaviour {
    public static GravityController Instance { get; private set; }

    public void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
    }

    public Vector3 gravity => Input.acceleration;
    public Vector2 normalizedGravity => new Vector2(gravity.x, gravity.y).normalized;
    public float angle => (float)Math.Acos(normalizedGravity.y);
}