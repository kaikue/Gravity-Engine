using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public Sprite toggledSprite;
    private Sprite defaultSprite;
    private SpriteRenderer sr;
    private bool toggled = false;

    public Door door;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        defaultSprite = sr.sprite;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb)
        {
            Vector2 relVel = transform.InverseTransformDirection(rb.velocity);
            if (relVel.x > 0 && !toggled) //rb moving right relative to unrotated transform
            {
                toggled = true;
                sr.sprite = toggledSprite;
                door.Activate();
            }
            else if (relVel.x < 0 && toggled) //rb moving left relative to unrotated transform
            {
                toggled = false;
                sr.sprite = defaultSprite;
                door.Deactivate();
            }
        }
    }
}
