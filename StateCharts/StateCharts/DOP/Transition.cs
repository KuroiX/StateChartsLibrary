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
            Id = -1;
            //Conditions = new List<int>();
        }


        // source state
        // dest state
        public int SourceId { get; }
        public int TargetId { get; }
        
        public int Id { get; }

        // list conditions
        public List<Condition> Conditions { get; }

        // executables
        
        // Maybe move all reference types to a different location
    }
}