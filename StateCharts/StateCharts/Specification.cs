using System.Collections.Generic;

namespace StateCharts
{
    public struct Specification
    {
        // TODO: proper constructor
        
        public Dictionary<int, int[]> InitialStates { get; set; }
        
        public int[] SourceIds { get; set; }
        public int[] TargetIds { get; set; }
        public Condition[][] Conditions { get; set; }

        // Behavior
        //public Dictionary<int, List<StateBehavior>> Behavior { get; }
    }
}