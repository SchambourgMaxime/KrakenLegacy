public class C_FiniteStateMachine
{
    private C_BaseGameEntity owner;
    private I_State currentState;
    private I_State previousState;
    private I_State globalState;

    public void Awake()
    {
        currentState = null;
        previousState = null;
        globalState = null;
    }

    public void Init(C_BaseGameEntity owner, I_State initialState)
    {
        this.owner = owner;
        ChangeState(initialState);
    }

    public void Update()
    {
        if (globalState != null)
        {
            globalState.Execute(this.owner);
        }
        if (currentState != null)
        {
            currentState.Execute(this.owner);
        }
    }

    public void ChangeState(I_State newState)
    {
        previousState = currentState;
        if (currentState != null)
        {
            currentState.Exit(this.owner);
        }
        currentState = newState;
        if (currentState != null)
        {
            currentState.Enter(this.owner);
        }
    }

    public void RevertToPreviousState()
    {
        if (previousState != null)
        {
            ChangeState(previousState);
        }
    }

    public I_State getCurrentState() { return currentState; }
}