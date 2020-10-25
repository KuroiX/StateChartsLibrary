using System.Net.Sockets;

namespace StateCharts.Conditions
{
    public abstract class Condition
    {
        public string Name { get; }
        public bool Bool { get; protected set; }
        public int Int { get; protected set; }
        public float Float { get; protected set; }

        protected Condition(string name)
        {
            this.Name = name;
        }

        public virtual bool Evaluate(bool compare)
        {
            return false;
        }
        public virtual bool Evaluate(int compare)
        {
            return false;
        }
        public virtual bool Evaluate(float compare)
        {
            return false;
        }

    }
    
}