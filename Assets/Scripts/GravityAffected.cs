using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAffected : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 gravDirection = Vector2.down;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetGravity(Vector2 gravDir)
    {
        rb.velocity = Vector2.zero;
        gravDirection = gravDir;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(rb.velocity.x * Mathf.Abs(gravDirection.x), rb.velocity.y * Mathf.Abs(gravDirection.y));
    }
}
