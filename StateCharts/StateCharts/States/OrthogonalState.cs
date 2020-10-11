using System.Collections.Generic;
using System.Linq;

namespace StateCharts.States
{
    public class OrthogonalState : State
    {
        private List<State> states;

        public OrthogonalState()
        {
            states = new List<State>();
        }
        
        public override List<State> GetSubStates()
        {
            List<State> subStates = new List<State> {this};

            foreach (State state in states)
            {
                foreach (State state2 in state.GetSubStates())
                {
                    subStates.Add(state2);
                }
            }

            return subStates;
        }

        public override List<State> GetInitialStates()
        {
            List<State> initialStates = new List<State>();
            
            foreach (State state in states)
            {
                foreach (State state2 in state.GetInitialStates())
                {
                    initialStates.Add(state2);
                }
            }

            return initialStates;
        }
    }
}