using System.Collections.Generic;

namespace StateCharts.States
{
    public class AtomicState: State
    {
        public override List<State> GetSubStates()
        {
            return new List<State> {this};
        }

        public override List<State> GetInitialStates()
        {
            return new List<State> {this};
        }
    }
}