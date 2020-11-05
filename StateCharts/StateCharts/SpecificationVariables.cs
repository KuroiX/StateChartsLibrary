using System.Collections.Generic;

namespace StateCharts
{
    public struct SpecificationVariables
    {
        // TODO: proper constructor
        
        public Dictionary<int, int> Ints;
        public Dictionary<int, float> Floats;
        public Dictionary<int, bool> Bools;
        public Dictionary<int, bool> Triggers;
    }
}