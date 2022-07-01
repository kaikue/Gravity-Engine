using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Activatable
{
    public Sprite openSprite;
    private Sprite closedSprite;
    private SpriteRenderer sr;
    private Collider2D coll;

    protected override void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        closedSprite = sr.sprite;
        base.Start();
    }

    protected override void SetActive(bool activated)
    {
        if (activated)
        {
            sr.sprite = openSprite;
        }
        else
        {
            sr.sprite = closedSprite;
        }
        coll.enabled = !activated;
    }
}
