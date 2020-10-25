using System.Collections.Generic;
using StateCharts.Transitions;

namespace StateCharts.States
{
    public abstract class State
    {
        // Behavior
        public List<StateChartBehavior> Behaviors { get; set; }

        // Transitions
        private Transition _transition;
        
        public abstract List<State> GetSubStates();
        public abstract List<State> GetInitialStates();
    }
}