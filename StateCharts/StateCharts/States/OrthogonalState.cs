using System.Collections.Generic;

namespace StateCharts.States
{
    public class OrthogonalState : State
    {
        private State states;
        public override List<State> GetSubStates()
        {
            throw new System.NotImplementedException();
        }

        public override List<State> GetInitialStates()
        {
            throw new System.NotImplementedException();
        }
    }
}