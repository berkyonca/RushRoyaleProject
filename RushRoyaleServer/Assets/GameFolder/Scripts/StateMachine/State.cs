public abstract class State
{
    protected ICharacter _character;

    public abstract void Tick();

    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }

    public State(ICharacter character)
    {
        _character = character;
    }
}
