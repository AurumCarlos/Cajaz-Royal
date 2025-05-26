using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected AIAgent Enemy;
    public abstract void Enter();
    public abstract void Update();
   
    public State(AIAgent ai)
    {
        Enemy = ai;
    }
}

