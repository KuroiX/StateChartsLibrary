using System.Collections.Generic;
using System.Linq;

namespace StateCharts
{
    public class Specification
    {
        public Specification()
        {
            // TODO:
            States = new Dictionary<int, State>();
            Transitions = new List<Transition>();
            Behavior = new Dictionary<int, List<StateBehavior>>();
            Hierarchy = new Dictionary<int, List<int>>();
            InitialStates = new Dictionary<int, List<int>>();
            
            Ints = new Dictionary<string, int>();
            Floats = new Dictionary<string, float>();
            Bools = new Dictionary<string, bool>();
            Triggers = new Dictionary<string, bool>();
        }
        
        // Can be made immutable?
        // Set of states
        public Dictionary<int, State> States { get; }
        public Dictionary<int, List<int>> Hierarchy { get; }

        public List<int> GetSubStates(int initialId)
        {
            List<int> subStates = new List<int> {initialId};
            
            if (!Hierarchy.Keys.Contains(initialId) || Hierarchy[initialId].Count == 0)
            {
                return subStates;
            }

            foreach (int id in Hierarchy[initialId])
            {
                foreach (int i in GetSubStates(id))
                {
                    subStates.Add(i);
                }
            }

            return subStates;
        }
        
        public Dictionary<int, List<int>> InitialStates { get; }

        public List<int> GetInitialStates(int id)
        {
            List<int> subStates = new List<int> {id};
            
            if (!InitialStates.Keys.Contains(id) || InitialStates[id].Count == 0)
            {
                return subStates;
            }

            foreach (int Id in InitialStates[id])
            {
                foreach (int i in GetInitialStates(Id))
                {
                    subStates.Add(i);
                }
            }

            return subStates;
        }
        
        // Set of transitions
        public List<Transition> Transitions { get; set; }
        // Set of conditions
        
        // Information about the initial state
        
        // Behavior
        public Dictionary<int, List<StateBehavior>> Behavior { get; }

        /*private State _initial;                                        // Used for Specification
        public Dictionary<int, State> States { get; }                  // Used for Specification
        public Dictionary<State, List<Transition>> Transitions { get; }// Used for Specification
        */
        public Dictionary<string, int> Ints;                         // Used for both
        public Dictionary<string, float> Floats;                     // Used for both
        public Dictionary<string, bool> Bools;                       // Used for both
        public Dictionary<string, bool> Triggers;                    // Used for both
        /*
        public Specification(string json)
        {
            
        }*/
    }
}