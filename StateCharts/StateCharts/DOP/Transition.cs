using System.Collections.Generic;

namespace StateCharts.DOP
{
    public class Transition
    {
        public Transition(int sourceId, int targetId)
        {
            SourceId = sourceId;
            TargetId = targetId;
            
            // TODO: initialize properly
            //Conditions = new List<int>();
        }


        // source state
        // dest state
        public int SourceId { get; }
        public int TargetId { get; }

        // list conditions
        public List<Condition> Conditions { get; }

        // executables
    }
}