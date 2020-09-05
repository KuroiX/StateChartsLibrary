using System.Collections.Generic;
using StateCharts.States;
using StateCharts.Transitions;

namespace StateCharts.DOP.SimpleStateMachine
{
    public class Specification
    {
        private State _initial;                                        // Used for Specification
        public Dictionary<int, State> States { get; }                  // Used for Specification
        public Dictionary<State, List<Transition>> Transitions { get; }// Used for Specification
        
        private Dictionary<string, int> _ints;                         // Used for both
        private Dictionary<string, float> _floats;                     // Used for both
        private Dictionary<string, bool> _bools;                       // Used for both
        private Dictionary<string, bool> _triggers;                    // Used for both
        
        public Specification(string json)
        {
            // TODO: read json file as specification
            _initial = new AtomicState();
            States.Add(0, _initial);
        }
    }
}