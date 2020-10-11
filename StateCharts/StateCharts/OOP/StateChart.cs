using System.Collections.Generic;
using System.Linq;
using StateCharts.States;
using StateCharts.Transitions;

namespace StateCharts.OOP
{
    public class StateChart
    {
        public StateChart(string json)
        {
            // TODO: read json file as specification
        }

        public StateChart()
        {
            _bools = new Dictionary<string, bool>();
            //_triggers = new Dictionary<string, bool>();
            //_ints = new Dictionary<string, int>();
            //_floats = new Dictionary<string, float>();
            
            _transitions = new Dictionary<State, List<Transition>>();
            _states = new Dictionary<int, State>();
            _fullConfiguration = new List<State>();
            _basicConfiguration = new AtomicState();
            
            SuperState A = new SuperState();
            State B = new AtomicState();
            State C = new AtomicState();
            State D = new AtomicState();
            State E = new AtomicState();
            
            Transition t_a_e = new Transition(A, E, this);
            Transition tEa = new Transition(E, A, this);
            Transition tBC = new Transition(B, C, this);
            Transition tCD = new Transition(C, D, this);
            Transition tDB = new Transition(D, B, this);

            A.Initial = B;
            A.States.Add(B);
            A.States.Add(C);
            A.States.Add(D);

            _initial = A;
            _fullConfiguration.Add(A);
            _fullConfiguration.Add(B);
            
            _transitions.Add(A, new List<Transition> {t_a_e});
            _transitions.Add(E, new List<Transition> {tEa});
            _transitions.Add(B, new List<Transition> {tBC});
            _transitions.Add(C, new List<Transition> {tCD});
            _transitions.Add(D, new List<Transition> {tDB});
        }
        
        #region Data
        
        public State _initial;
        public State _basicConfiguration;
        public List<State> _fullConfiguration;
        public Dictionary<int, State> _states;
        public Dictionary<State, List<Transition>> _transitions;
        
        public Dictionary<string, bool> _bools;
        //public Dictionary<string, bool> _triggers;
        //public Dictionary<string, int> _ints;
        //public Dictionary<string, float> _floats;
        
        #endregion

        #region System

        public void Update()
        {
            // TODO: behavior
            
            #region Step 1: Snapshot X, C and E
            // This can be made way more performant with value types (int as IDs)
            List<State> x = new List<State>();
            foreach (State state in _fullConfiguration)
            {
                x.Add(state);
            }
            
            // Possibility: No copy needed if the behavior is executed separately! (Might be more performant)
            Dictionary<string, bool> bools = _bools.ToDictionary(entry => entry.Key, entry => entry.Value);
            //Dictionary<string, bool> triggers = _triggers.ToDictionary(entry => entry.Key, entry => entry.Value);
            //Dictionary<string, int> ints = _ints.ToDictionary(entry => entry.Key, entry => entry.Value);
            //Dictionary<string, float> floats = _floats.ToDictionary(entry => entry.Key, entry => entry.Value);
            
            #endregion
            
            #region Step 2: Clear Events (triggers)
            // Might has to be moved to the end
            /*foreach (string name in _triggers.Keys)
            {
                _triggers[name] = false;
            }*/
            
            #endregion
            
            #region Step 3: Store relevant Transitions in set T
            List<Transition> T = new List<Transition>();
            foreach (State state in _fullConfiguration)
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
                        // TODO: maybe also remove all parent states that are connected to the t.Origin in case t.Next is in a different parent state
                        
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
                        // if (t.Next.IsIntermediate) ...
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
            _fullConfiguration = new List<State>();
            foreach (State state in x)
            {
                _fullConfiguration.Add(state);
            }
        }
        
        public void Execute()
        {
            foreach (State state in _fullConfiguration)
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