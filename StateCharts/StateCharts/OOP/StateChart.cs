using System.Collections.Generic;
using StateCharts.States;
using StateCharts.Transitions;

namespace StateCharts.OOP
{
    public class StateChart
    {
        public StateChart(string json)
        {
            // TODO: read json file as specification
            _initial = new AtomicState();
            _states.Add(0, _initial);
        }
        
        #region Data
        
        private State _initial;
        private State basicConfiguration;
        private List<State> fullConfiguration;
        private Dictionary<int, State> _states;
        private Dictionary<State, List<Transition>> _transitions;
        
        private Dictionary<string, bool> _bools;
        private Dictionary<string, bool> _triggers;
        private Dictionary<string, int> _ints;
        private Dictionary<string, float> _floats;
        
        #endregion

        #region System

        public void Next()
        {
            // TODO: behavior
            
            #region Step 1: Snapshot X, C and E
            // TODO: dont copy pointers
            List<State> x = fullConfiguration;
            Dictionary<string, bool> bools = _bools;
            Dictionary<string, bool> triggers = _triggers;
            Dictionary<string, int> ints = _ints;
            Dictionary<string, float> floats = _floats;
            
            #endregion
            
            #region Step 2: Clear Events (triggers)
            foreach (string name in _triggers.Keys)
            {
                _triggers[name] = false;
            }
            
            #endregion
            
            #region Step 3: store relevant Transitions in set T
            List<Transition> T = new List<Transition>();
            foreach (State state in fullConfiguration)
            {
                foreach (Transition t in _transitions[state])
                {
                    // TODO: Evaluate
                    if (t.Evaluate())
                    {
                        T.Add(t);
                    }
                }
            }
            // TODO: Sort T
            
            #endregion
            
            // Step 4+5: Evaluate t with highest priority
            while (true)
            {
                List<Transition> I = new List<Transition>();
                foreach (Transition t in T)
                {
                    if (x.Contains(t.Origin))
                    {
                        // Remove all sub-states
                        foreach (State state in t.Origin.GetSubStates())
                        {
                            x.Remove(state);
                        }
                        // Add new sub-states
                        foreach (State state in t.Next.GetInitialStates())
                        {
                            x.Add(state);
                        }

                        // Add transitions to set I (not necessary atm?)
                    }
                    else
                    {
                        // remove transition?
                        // probably not necessary
                    }
                }

                // Step 6: Redo 4+5 with I
                /*if (I.Count > 0)
                {
                    // TODO: no pointer copy
                    T = I;
                    // sort? probably yes
                }
                else
                {
                    break;
                }*/
                break;
            }

            // Step 7: Replace fullConfiguration with X
            // TODO: no pointer copy
            fullConfiguration = x;

        }
        
        public void Execute()
        {
            foreach (State state in fullConfiguration)
            {
                foreach (StateChartBehavior behavior in state.Behaviors)
                {
                    behavior.OnStateUpdate();
                }
            }
        }
        
        #region Conditions
        /*
         * Transitions with conditions like "is in state A" can be modelled with "OnStateEnter" and "OnStateLeave" functions
         */

        // TODO: same functions with hash-key -> faster
        // same for int, float, (trigger?)
       

        public void AddBool(string name, bool value)
        {
            _bools.Add(name, value);
        }

        public void SetBool(string name, bool value)
        {
            // TODO: test
            _bools[name] = value;
        }

        public bool GetBool(string name)
        {
            return _bools[name];
        }
        
        
        #endregion

        #endregion
    }
}