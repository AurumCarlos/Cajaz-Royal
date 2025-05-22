using System;
using UnityEngine;

public class WalkerObject
{
    public Vector2 Position { get; set; }
    private Vector2 _direction;
    public Vector2 Size { get; set; }
    private Vector2 _size;
    public Vector2 Direction { get; set; }
    public float ChanceToChange;
    public int Type { get; set; }
    private int _type;

    public WalkerObject(Vector2 pos, Vector2 dir, float chanceToChange, int type, int sizex = 0, int sizey = 0)
    {
        Position = pos;
        Direction = dir;
        ChanceToChange = chanceToChange;
        Type = type;
    }
    
}