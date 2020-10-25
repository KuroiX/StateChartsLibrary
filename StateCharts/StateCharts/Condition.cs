namespace StateCharts
{
    public class Condition
    {
        public Condition(int type, int operation, string name)
        {
            Type = type;
            Operation = operation;
            Name = name;
        }
        
        public Condition(int type, int operation, string name, int value)
        {
            Type = type;
            Operation = operation;
            Name = name;
            IntValue = value;
        }
        
        public Condition(int type, int operation, string name, float value)
        {
            Type = type;
            Operation = operation;
            Name = name;
            FloatValue = value;
        }
        
        // Idee:
        // Kinda enum
        public int Type { get; set; }
        public int Operation { get; set; }
        
        // REFERENCE TYPE
        public string Name { get; set; }
        
        // TODO: Idea
        // Just take one and interpret it differently
        // they are all bits after all
        // takes some effort with unsafe code
        public int IntValue { get; set; }
        public float FloatValue { get; set; }
    }
}