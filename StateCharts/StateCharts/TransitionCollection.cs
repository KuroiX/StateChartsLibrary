namespace StateCharts
{
    public class TransitionCollection
    {
        public TransitionCollection(int[] sourceIds, int[] targetIds, Condition[][] conditions)
        {
            SourceIds = sourceIds;
            TargetIds = targetIds;
            Conditions = conditions;
        }
        
        public int[] SourceIds { get; }
        public int[] TargetIds { get; }
        public Condition[][] Conditions { get; }
    }
}