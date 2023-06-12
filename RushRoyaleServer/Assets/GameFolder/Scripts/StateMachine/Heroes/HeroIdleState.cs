using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroIdleState : State
{
    private Hero _hero;

    public HeroIdleState(ICharacter hero) : base(hero)
    {
        _hero = hero as Hero;
    }

    public override void Tick()
    {
        WaitOnIdle();

        if (_hero.OpponentColliders.Length != 0)
            _hero.SetState(new HeroBattleState(_hero));
    }

    private void WaitOnIdle()
    {
        // To be filled later
    }

    public override void OnStateEnter()
    {
        // To be filled later
    }

    public override void OnStateExit()
    {
        // To be filled later
    }
}
