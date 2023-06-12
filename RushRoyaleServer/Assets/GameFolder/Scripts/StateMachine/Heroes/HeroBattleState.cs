using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeroBattleState : State
{
    private Hero _hero;

    public HeroBattleState(ICharacter hero) : base(hero)
    {
        _hero = _character as Hero;
    }

    public override void Tick()
    {
        AttackTarget();

        if (_hero.OpponentColliders.Length == 0)
            _hero.SetState(new HeroIdleState(_hero));
    }

    private void AttackTarget()
    {
        _hero.Attack();
    }

    public override void OnStateEnter()
    {
        _hero.StartCoroutine(nameof(_hero.FindLeadingOpponent));
    }

    public override void OnStateExit()
    {
        _hero.StopCoroutine(nameof(_hero.FindLeadingOpponent));
    }
}
