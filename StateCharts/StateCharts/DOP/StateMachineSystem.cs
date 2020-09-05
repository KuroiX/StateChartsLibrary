using System.Collections.Generic;
using StateCharts.Transitions;

namespace StateCharts.DOP.SimpleStateMachine
{
    public class StateMachineSystem
    {
        private static Dictionary<int, Specification> _specifications;
        
        public static void Next(Configuration configuration)
        {
            /*
            foreach (Transition transition in _transitions[_current])
            {
                if (transition.Evaluate())
                {
                    // better to execute all configurations at once?
                    foreach (StateChartBehavior behavior in _current.Behaviors)
                    {
                        behavior.OnStateExit();
                    }
                    
                    _current = transition.Next;
                    
                    foreach (StateChartBehavior behavior in _current.Behaviors)
                    {
                        behavior.OnStateEnter();
                    }
                    
                }
            } 
            */

            foreach (Transition transition in _specifications[configuration.SpecificationId].Transitions[_specifications[configuration.SpecificationId].States[configuration.CurrentStateId]])
            {
                if (transition.Evaluate())
                {
                    // Execute behavior

                    //configuration.CurrentStateId = transition.Next.id;
                    
                    // Execute behavior
                }
            }
        }

        /// <summary>
        /// This function executes the behavior of a given configuration depending on the state the configuration.
        /// </summary>
        /// <param name="configuration">The configuration that the behavior shall be executed</param>
        public static void Execute(Configuration configuration)
        {
            foreach (StateChartBehavior behavior in _specifications[configuration.SpecificationId].States[configuration.CurrentStateId].Behaviors)
            {
                behavior.OnStateUpdate();
            }
        }
    }
}