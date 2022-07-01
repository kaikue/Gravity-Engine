using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moonling : MonoBehaviour
{
    public bool facingLeft;
    public float speed = 5;

    protected Rigidbody2D rb;
    protected EdgeCollider2D ec;
    protected SpriteRenderer sr;

    protected bool onGround;
    protected bool wallLeft;
    protected bool wallRight;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        ec = gameObject.GetComponent<EdgeCollider2D>();
    }

    protected bool CheckPoint(int ecPoint, Vector2 direction)
    {
        Vector2 point = rb.position + ec.points[ecPoint] + direction * 0.02f;
        return Physics2D.OverlapPoint(point, LayerMask.GetMask("Tiles")) != null;
    }

    protected Collider2D RaycastTiles(Vector2 startPoint, Vector2 endPoint)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPoint, endPoint - startPoint, Vector2.Distance(startPoint, endPoint), LayerMask.GetMask("Tiles"));
        return hit.collider;
    }

    protected bool CheckSide(int point0, int point1, Vector2 direction)
    {
        Vector2 startPoint = rb.position + ec.points[point0] + direction * 0.02f;
        Vector2 endPoint = rb.position + ec.points[point1] + direction * 0.02f;
        Collider2D collider = RaycastTiles(startPoint, endPoint);
        return collider != null;
    }

    protected virtual Vector2 GetVelocity()
    {
        return rb.velocity;
    }

    protected virtual void UpdateDirection()
    {
        
    }

    private void FixedUpdate()
    {
        onGround = CheckSide(4, 3, Vector2.down);
        wallLeft = CheckSide(1, 0, new Vector2(-1, 3));
        wallRight = CheckSide(2, 3, new Vector2(1, 3));

        rb.velocity = new Vector2(0, rb.velocity.y);
    }
}
