using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuffler : Moonling
{
    protected override void UpdateDirection()
    {
        if (wallLeft && facingLeft)
        {
            facingLeft = false;
            return;
        }

        if (wallRight && !facingLeft)
        {
            facingLeft = true;
            return;
        }

        if (onGround)
        {
            bool leftEmpty = !CheckPoint(0, new Vector2(-1, -1));
            if (leftEmpty && facingLeft)
            {
                facingLeft = false;
                return;
            }
            bool rightEmpty = !CheckPoint(3, new Vector2(1, -1));
            if (rightEmpty && !facingLeft)
            {
                facingLeft = true;
                return;
            }
        }
    }

    protected override Vector2 GetVelocity()
    {
        float dir = facingLeft ? -1 : 1;
        return new Vector2(speed * dir, rb.velocity.y);
    }
}
