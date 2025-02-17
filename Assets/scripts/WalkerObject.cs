using System;
using UnityEngine;

public class WalkerObject
{
    public Vector2 Position;
    public Vector2 Direction;
    public float ChanceToChange;
    public int Type;

    public WalkerObject(Vector2 pos, Vector2 dir, float chanceToChange, int type){
        Position = pos;
        Direction = dir;
        ChanceToChange = chanceToChange;
        Type = type;
    }
}