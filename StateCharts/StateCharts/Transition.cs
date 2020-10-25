using System.Collections.Generic;

namespace StateCharts
{
    public class Transition
    {
        public Transition(int sourceId, int targetId, int id)
        {
            SourceId = sourceId;
            TargetId = targetId;
            Id = id;
            
            // TODO: initialize properly
            Conditions = new List<Condition>();
        }


        // source state
        // dest state
        public int SourceId { get; }
        public int TargetId { get; }
        
        public int Id { get; set; }

        // list conditions
        public List<Condition> Conditions { get; }

        // executables
        
        // Maybe move all reference types to a different location
    }
}