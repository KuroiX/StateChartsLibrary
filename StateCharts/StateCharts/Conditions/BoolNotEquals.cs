namespace StateCharts.Conditions
{
    public class BoolNotEquals: Condition
    {
        public BoolNotEquals(string name, bool value) : base(name)
        {
            Bool = value;
        }

        public override bool Evaluate(bool compare)
        {
            return Bool != compare;
        }
    }
}