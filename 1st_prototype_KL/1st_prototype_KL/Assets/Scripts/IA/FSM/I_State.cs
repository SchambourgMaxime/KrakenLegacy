public interface I_State
{
    void Enter(C_BaseGameEntity entity);

    void Execute(C_BaseGameEntity entity);

    void Exit(C_BaseGameEntity entity);
}
