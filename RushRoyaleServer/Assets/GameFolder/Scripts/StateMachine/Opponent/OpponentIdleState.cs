using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentIdleState : State
{
    public OpponentIdleState(ICharacter opponent) : base(opponent)
    {

    }

    public override void Tick()
    {
        WaitOnIdle();
    }

    private void WaitOnIdle()
    {
        // To be filled later
    }
}
