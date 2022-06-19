using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public static Vector2 GetGravityVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector2.up;
            case Direction.Down:
                return Vector2.down;
            case Direction.Left:
                return Vector2.left;
            case Direction.Right:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }

    public void ChangeGravity(Direction direction)
    {
        Vector2 gravVec = GetGravityVector(direction);
        Physics2D.gravity = gravVec * Player.gravityForce;

        GravityAffected[] gas = FindObjectsOfType<GravityAffected>();
        foreach (GravityAffected ga in gas)
        {
            ga.SetGravity(gravVec);
        }
    }
}
