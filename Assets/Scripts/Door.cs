using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private SpriteRenderer sr;
    private Collider2D coll;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
    }

    public void Activate()
	{
        sr.enabled = false;
        coll.enabled = false;
	}

    public void Deactivate()
	{
        sr.enabled = true;
        coll.enabled = true;
    }
}
