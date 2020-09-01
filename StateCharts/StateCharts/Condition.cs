namespace StateCharts
{
    public class Condition
    {
        public string Name { get; }
        public bool Value { get; }

        public Condition(string name, bool value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}