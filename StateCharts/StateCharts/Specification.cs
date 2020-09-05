using System.Collections.Generic;
using StateCharts.States;
using StateCharts.Transitions;

namespace StateCharts
{
    public class Specification
    {
        // AtomicStates only (StateMachines)

        private State _initial;
        private Dictionary<int, State> _states;
        private Dictionary<State, Transition> _transitions;
        
        public Specification(string _json)
        {
            // TODO: read json file as specification
            _initial = new AtomicState();
            _states.Add(0, _initial);
        }

        #region Conditions
        // functions like AddBool(bool value, string name) to add a condition to a hashmap and then
        // SetBool, GetBool
        
        // same for int, float, (trigger?)
        
        // transitions with conditions like "is in state A" can be modelled with "OnStateEnter" and OnStateLeave" functions

        private Dictionary<string, bool> _bools;

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
        
        // TODO: same functions with hash-key -> faster
        
        #endregion
        
    }
}