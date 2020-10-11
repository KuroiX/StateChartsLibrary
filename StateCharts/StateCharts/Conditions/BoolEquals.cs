using System.Text;

namespace StateCharts.Conditions
{
    public class BoolEquals: Condition
    {
        public BoolEquals(string name, bool value) : base(name)
        {
            Bool = value;
        }

        public override bool Evaluate(bool compare)
        {
            return Bool == compare;
        }
    }
}