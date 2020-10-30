using System.Collections.Generic;

namespace StateCharts
{
    public class StateCollection
    {
        public StateCollection(int[] idMasks, Dictionary<int, int[]> initialStates)
        {
            IdMasks = idMasks;
            //InitialMasks = initialMasks;
            InitialStates = initialStates;
        }
        
        public int[] IdMasks { get; }
        //public int[] InitialMasks { get; }
        public Dictionary<int, int[]> InitialStates { get; }
    }
}