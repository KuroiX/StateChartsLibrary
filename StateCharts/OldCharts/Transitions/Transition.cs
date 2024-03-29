using System.Collections.Generic;
using StateCharts.Conditions;
using StateCharts.OOP;
using StateCharts.States;

// ctrl + alt + shift + 8
namespace StateCharts.Transitions
{
    public class Transition
    {
        public State Origin { get; }
        public State Next { get; }
        private StateChart _specification;

        public Transition(State origin, State target, StateChart specification)
        {
            Origin = origin;
            Next = target;
            _specification = specification;
            _conditions = new List<Condition>();
        }
        
        // Conditions
        // TODO: more than just booleans
        // remove conditions?
        // conditions public?
        private List<Condition> _conditions;

        public void AddCondition(string name, bool value)
        {
            _conditions.Add(new BoolEquals(name, value));
        }

        public void AddCondition(Condition condition)
        {
            _conditions.Add(condition);
        }

        public bool Evaluate()
        {
            var result = true;

            foreach (var condition in _conditions)
            {
                //result = result && (_specification.GetBool(condition.Name) == condition.Value);
                result = result && (condition.Evaluate(_specification.GetBool(condition.Name)));
            }

            return result;
        }

    }
}