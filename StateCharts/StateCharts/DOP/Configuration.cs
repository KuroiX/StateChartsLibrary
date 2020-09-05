using System.Collections.Generic;
using StateCharts.States;

namespace StateCharts.DOP.SimpleStateMachine
{
    public class Configuration
    {
        public int SpecificationId { get; }
        public int CurrentStateId { get; set; }                        // Used for Configuration

        private Dictionary<string, bool> _bools;                       // Used for both
        private Dictionary<string, bool> _triggers;                    // Used for both
        private Dictionary<string, int> _ints;                         // Used for both
        private Dictionary<string, float> _floats;                     // Used for both
        
        // probably as ID for better data management
        //private Specification _specification;
        
        // current active states
        // Bit masks! problem: how many layers? finite possibilities, but fast
        // other possibility: layers as an object and working on the each layer step by step
        // other possibility: AND, OR, XOR composition

        // priority?
    }
}