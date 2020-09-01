using System;
using System.Collections.Generic;

namespace StateCharts
{
    // TODO: this class MUST be split in data and logic or entity and system
    public class Configuration
    {
        // probably as ID for better data management
        private Specification _specification;
        
        // current active states
        // Bit masks! problem: how many layers? finite possibilities, but fast
        // other possibility: layers as an object and working on the each layer step by step
        // other possibility: AND, OR, XOR composition

        // priority?

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