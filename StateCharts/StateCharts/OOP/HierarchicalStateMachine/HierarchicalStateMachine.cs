using System.Collections.Generic;
using StateCharts.States;
using StateCharts.Transitions;

namespace StateCharts.OOP.HierarchicalStateMachine
{
    public class HierarchicalStateMachine
    {
        // AtomicStates only (StateMachines)

        public HierarchicalStateMachine(string _json)
        {
            // TODO: read json file as specification
            _initial = new AtomicState();
            _states.Add(0, _initial);
        }
        
        #region Data
        
        private State _initial;
        private State basicConfiguration;
        private List<State> fullConfiguration;
        private Dictionary<int, State> _states;
        private Dictionary<State, List<Transition>> _transitions;
        
        private Dictionary<string, bool> _bools;
        private Dictionary<string, bool> _triggers;
        private Dictionary<string, int> _ints;
        private Dictionary<string, float> _floats;
        
        #endregion

        #region System
        public void Execute()
        {
            foreach (State state in fullConfiguration)
            {
                foreach (StateChartBehavior behavior in state.Behaviors)
                {
                    behavior.OnStateUpdate();
                }
            }
        }

        public void Next()
        {
            // Assumption: fullConfiguration is sorted by priority
            foreach (State state in fullConfiguration)
            {
                foreach (Transition transition in _transitions[state])
                {
                    if (transition.Evaluate())
                    {
                        // Execute leave
                        
                        // Update fullConfiguration
                        // by taking out all children
                        fullConfiguration = new List<State> {transition.Next};
                        // enter possible sub-states
                        
                        
                        // Execute enter

                        return;

                    }
                }
            }
        }
        
        #region Conditions
        /*
         * Transitions with conditions like "is in state A" can be modelled with "OnStateEnter" and "OnStateLeave" functions
         */

        // TODO: same functions with hash-key -> faster
        // same for int, float, (trigger?)
       

        public void AddBool(string name, bool value)
        {
            _bools.Add(name, value);
        }

        public void SetBool(string name, bool value)
        {
            // TODO: test
            _bools[name] = value;
        }

        public bool GetBool(string name)
        {
            return _bools[name];
        }
        
        
        #endregion

        #endregion
    }
}