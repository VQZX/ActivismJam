namespace Flusk.Utility
{
    public interface IState
    {
        void Enter(IState previousState);

        void Tick();

        void Exit(IState nextState);
    }
}
