using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour, ICharacter
{
    private State _currentState;


    private void Start()
    {
        SetState(new OpponentIdleState(this));
    }

    private void Update()
    {
        _currentState.Tick();
    }

    public void SetState(State state)
    {
        if (_currentState != null)
            _currentState.OnStateExit();

        _currentState = state;

        if (_currentState != null)
            _currentState.OnStateEnter();
    }

    public void SetPosition(Vector3 position)
    {
        // To be filled later
    }
}
