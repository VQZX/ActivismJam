using System.Collections.Generic;

namespace Flusk.Utility
{
    public class StateMachine<TState> where TState : IState
    {
        protected List<TState> states;
        public IState CurrentState {get; protected set;}

        public StateMachine()
        {
            states = new List<TState>();
        }
        public virtual void AddState (TState state)
        {
            states.Add(state);
        }

        public void Tick ()
        {
            if ( CurrentState == null )
            {
                return;
            }
            CurrentState.Tick();
        }

        public virtual void ChangeState(IState state)
        {
            var previousState = CurrentState;
            if ( CurrentState != null )
            {
                CurrentState.Exit(state);
            }
            CurrentState = state;
            CurrentState.Enter(previousState);
        }
    }

}
