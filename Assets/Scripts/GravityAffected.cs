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
        gravDirection = gravDir;
	}

	private void FixedUpdate()
	{
		//TODO
	}
}
