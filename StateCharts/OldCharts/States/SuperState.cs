using System.Collections.Generic;

namespace StateCharts.States
{
    public class SuperState: State
    {
        // Initial
        public State Initial { get; set; }
        public List<State> States { get; set; }

        private State current;
        // History
        private State history;

        public SuperState()
        {
            States = new List<State>();
        }
        

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
            List<State> initialStates = Initial.GetInitialStates();
            initialStates.Add(Initial);

            return initialStates;
        }
    }
}