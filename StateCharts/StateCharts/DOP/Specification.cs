using System.Collections.Generic;

namespace StateCharts.DOP
{
    public class Specification
    {
        public Specification()
        {
            // TODO:
            //States = new List<State>();
        }
        
        // Can be made immutable?
        // Set of states
        public Dictionary<int, State> States { get; }
        // Set of transitions
        public List<Transition> Transitions { get; }
        // Set of conditions
        
        // Information about the initial state
        
        // Behavior
        public Dictionary<int, List<StateBehavior>> Behavior { get; }

        /*private State _initial;                                        // Used for Specification
        public Dictionary<int, State> States { get; }                  // Used for Specification
        public Dictionary<State, List<Transition>> Transitions { get; }// Used for Specification
        
        private Dictionary<string, int> _ints;                         // Used for both
        private Dictionary<string, float> _floats;                     // Used for both
        private Dictionary<string, bool> _bools;                       // Used for both
        private Dictionary<string, bool> _triggers;                    // Used for both
        
        public Specification(string json)
        {
            
        }*/
    }
}