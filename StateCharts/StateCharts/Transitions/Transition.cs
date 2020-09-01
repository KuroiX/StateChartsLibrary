using System;
using System.Collections.Generic;
using StateCharts.States;

// ctrl + alt + shift + 8
namespace StateCharts.Transitions
{
    public class Transition
    {
        private State _origin;
        public State Next { get; }
        private Specification _specification;

        public Transition(State origin, State target, Specification specification)
        {
            _origin = origin;
            Next = target;
            _specification = specification;
        }
        
        // Conditions
        // TODO: more than just booleans
        // remove conditions?
        // conditions public?
        private List<Condition> _conditions;

        public void AddCondition(string name, bool value)
        {
            _conditions.Add(new Condition(name, value));
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
                result = result && (_specification.GetBool(condition.Name) == condition.Value);
            }

            return result;
        }

    }
}