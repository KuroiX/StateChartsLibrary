using System.Collections.Generic;
using StateCharts.States;
using StateCharts.Transitions;

namespace StateCharts
{
    public class SimpleStateMachine
    {
        // AtomicStates only (StateMachines)

        public SimpleStateMachine(string _json)
        {
            // TODO: read json file as specification
            _initial = new AtomicState();
            _states.Add(0, _initial);
        }
        
        #region Data
        
        private State _initial;                                        // Used for Specification
        private State _current;                                        // Used for Configuration
        private Dictionary<int, State> _states;                        // Used for Specification
        private Dictionary<State, List<Transition>> _transitions;      // Used for Specification
        
        private Dictionary<string, bool> _bools;                       // Used for both
        private Dictionary<string, bool> _triggers;                    // Used for both
        private Dictionary<string, int> _ints;                         // Used for both
        private Dictionary<string, float> _floats;                     // Used for both
        
        #endregion

        #region System
        public void Execute()
        {
            foreach (StateChartBehavior behavior in _current.Behaviors)
            {
                behavior.OnStateUpdate();
            }
        }

        public void Next()
        {
            foreach (Transition transition in _transitions[_current])
            {
                if (transition.Evaluate())
                {
                    // better to execute all configurations at once?
                    foreach (StateChartBehavior behavior in _current.Behaviors)
                    {
                        behavior.OnStateExit();
                    }
                    
                    _current = transition.Next;
                    
                    foreach (StateChartBehavior behavior in _current.Behaviors)
                    {
                        behavior.OnStateEnter();
                    }

                    return;
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