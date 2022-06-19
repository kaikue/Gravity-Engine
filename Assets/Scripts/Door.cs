using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool startOpen;
    public Sprite openSprite;
    private Sprite closedSprite;
    private SpriteRenderer sr;
    private Collider2D coll;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        closedSprite = sr.sprite;
        if (startOpen)
        {
            SetOpen(true);
        }
    }

    private void SetOpen(bool activated)
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

    public void Activate()
    {
        SetOpen(!startOpen);
    }

    public void Deactivate()
    {
        SetOpen(startOpen);
    }
}
