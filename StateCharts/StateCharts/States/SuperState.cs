using System.Collections.Generic;

namespace StateCharts.States
{
    public class SuperState: State
    {
        // Initial
        private State initial;

        private State current;
        // History
        private State history;

        // States
        public override List<State> GetSubStates()
        {
            List<State> subStates = current.GetSubStates();
            subStates.Add(current);

            return subStates;
        }

        public override List<State> GetInitialStates()
        {
            // TODO: history
            List<State> initialStates = initial.GetInitialStates();
            initialStates.Add(initial);

            return initialStates;
        }
    }
}