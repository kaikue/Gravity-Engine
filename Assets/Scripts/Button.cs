using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Sprite pressedSprite;
    private Sprite unpressedSprite;
    private SpriteRenderer sr;
    private int pressers = 0;

    public Activatable activatable;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        unpressedSprite = sr.sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb)
        {
            if (pressers == 0)
            {
                sr.sprite = pressedSprite;
                activatable.Activate();
            }
            pressers++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb)
        {
            pressers--;
            if (pressers == 0)
            {
                sr.sprite = unpressedSprite;
                activatable.Deactivate();
            }
        }
    }
}
