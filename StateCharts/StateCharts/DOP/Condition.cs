namespace StateCharts.DOP
{
    public class Condition
    {
        public Condition()
        {
            // mal überlegen
        }
        
        // Idee:
        // Kinda enum
        public int Type { get; }
        public int Operation { get; }
        
        public string Name { get; }
        
        public bool BoolValue { get; }
        public int IntValue { get; }
        public float FloatValue { get; }
    }
}