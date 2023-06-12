using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentBattleState : State
{
    public OpponentBattleState(ICharacter opponent) : base(opponent)
    {

    }

    public override void Tick()
    {
        FollowPath();
    }

    private void FollowPath()
    {
        // To be filled later
    }
}
