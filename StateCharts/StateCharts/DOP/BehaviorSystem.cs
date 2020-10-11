using System;
using System.Collections.Generic;
using System.Linq;

namespace StateCharts.DOP
{
    /// <summary>
    /// We have all behaviors in one class here.
    /// We might make a "smaller" version of this by giving each Specification its own system.
    /// That way the user can decide what they want.
    ///
    /// Differences: Easier to handle, deleting might be different, easier or not needed at all.
    /// </summary>
    public class BehaviorSystem
    {
        public BehaviorSystem()
        {
            specifications = new Dictionary<int, Specification>();
            instances = new Instance[0];
        }
        
        // Specifications
        // Different types of Behaviors: Player, Enemy, Boss, ...
        private Dictionary<int, Specification> specifications;
        
        // InstanceArray
        // One instance consists of Configuration, Conditions and Events
        private Instance[] instances;
        
        // Maybe external conditions here?

        /// <summary>
        /// Executes step function for one instance. This is the important function.
        /// </summary>
        /// <param name="instanceId"></param>
        public void ExecuteStep(int instanceId)
        {
            Instance current = instances[instanceId];

            #region Step 1: Snapshot X, C and E

            // This can be made way more performant with value types (int as IDs)
            List<int> currentStateIds = new List<int>();
            foreach (int state in current.Config)
            {
                currentStateIds.Add(state);
            }
            
            // Possibility: No copy needed if the behavior is executed separately! (Might be more performant)
            Dictionary<string, bool> bools = current.Bools.ToDictionary(entry => entry.Key, entry => entry.Value);
            Dictionary<string, bool> triggers = current.Triggers.ToDictionary(entry => entry.Key, entry => entry.Value);
            Dictionary<string, int> ints = current.Ints.ToDictionary(entry => entry.Key, entry => entry.Value);
            Dictionary<string, float> floats = current.Floats.ToDictionary(entry => entry.Key, entry => entry.Value);
            

            #endregion

            #region Step 2: Clear Events (triggers)

            // Might has to be moved to the end
            foreach (string name in current.Triggers.Keys)
            {
                current.Triggers[name] = false;
            }

            #endregion

            #region Step 3: Store relevant Transitions in set T

            List<Transition> T = new List<Transition>();
            
            foreach (Transition t in specifications[current.SpecificationId].Transitions)
            {
                // TODO: Check source?
                bool result = true;
                
                // Evaluate
                foreach (Condition c in t.Conditions)
                {
                    if (c.Type == 0)
                    {
                        // Trigger

                        result &= triggers[c.Name];
                        /*if (!triggers[c.Name])
                        {
                            result = false;
                        }*/
                    } 
                    else if (c.Type == 1)
                    {
                        // Bool
                        result &= (bools[c.Name] == c.BoolValue);

                        /*if (!bools[c.Name] == c.BoolValue)
                        {
                            result = false;
                        }*/
                    }
                    else if (c.Type == 2)
                    {
                        // Int
                        if (c.Operation == 0)
                        {
                            // ==
                            result &= (ints[c.Name] == c.IntValue);
                        }
                        else if (c.Operation == 1)
                        {
                            // !=
                            result &= (ints[c.Name] != c.IntValue);
                        }
                        else if (c.Operation == 2)
                        {
                            // <
                            result &= (ints[c.Name] < c.IntValue);
                        } 
                        else if (c.Operation == 3)
                        {
                            // >
                            result &= (ints[c.Name] > c.IntValue);
                        }
                        
                    }
                    else
                    {
                        // Float
                        if (c.Operation == 0)
                        {
                            // <
                            result &= (floats[c.Name] < c.FloatValue);
                        } 
                        else if (c.Operation == 1)
                        {
                            // >
                            result &= (floats[c.Name] > c.FloatValue);
                        }
                    }

                    if (!result) break;
                }

                if (result)
                {
                    // Add
                    T.Add(t);
                }
            }
            // Sort can be avoided if transitions are in the right order from the beginning
            // That way we only have to sort on initialize
            // Maybe save transitions in an array? 
            // 1. all in one place
            // 2. priority is ID

            #endregion

            // Step 4+5: Evaluate t with highest priority
            while (true)
            {
                List<Transition> I = new List<Transition>();
                
                // TODO: Idea
                // Instead of finding all relevant transitions and storing it in T first, we can (maybe)
                // find a transition and execute it right afterwards
                // We should be able to do that because the transitions are sorted to begin with
                // This helps us because 
                // 1. We don't have to iterate over T twice in a row
                // 2. There are transitions that get evaluated but fall out right away
                // because of a higher priority transition
                
                // The check needs to be:
                // 1. Is source in X?
                // 2. Are conditions/events true?
                // 3. Is source also in X'?
                // The third one is to check whether a higher priority transition was already executed
                foreach (Transition t in T)
                {
                    // Awesome, everything works over IDs and ints
                    
                    // This if is important because we might have changed currentStateIds in a previous iteration
                    if (currentStateIds.Contains(t.SourceId))
                    {
                        // Same parent/layer?
                        Specification currentSpec = specifications[current.SpecificationId];
                        State sourceState = specifications[current.SpecificationId].States[t.SourceId];
                        State targetState = specifications[current.SpecificationId].States[t.TargetId];

                        // should all be doable with Ids
                        State state1;
                        State state2;
                        if (sourceState.Layer < targetState.Layer)
                        {
                            // Check parent layer until parent layer = target layer
                            // we know in advance how many layers above it is

                            int sameId = -1;
                            State iterate = targetState;
                            for (int i = 0; i < targetState.Layer - sourceState.Layer; i++)
                            {
                                iterate = currentSpec.States[iterate.ParentStateId];
                            }
                            
                            // Safety check:
                            if (iterate.Layer != sourceState.Layer)
                            {
                                throw new Exception("Layer is not the same.");
                            }
                            else
                            {
                                state1 = sourceState;
                                state2 = iterate;
                            }
                        }
                        else if (sourceState.Layer > targetState.Layer)
                        {
                            // Check parent layer until parent layer = target layer
                            // we know in advance how many layers above it is

                            int sameId = -1;
                            State iterate = sourceState;
                            for (int i = 0; i < sourceState.Layer - targetState.Layer; i++)
                            {
                                iterate = currentSpec.States[iterate.ParentStateId];
                            }
                            
                            // Safety check:
                            if (iterate.Layer != sourceState.Layer)
                            {
                                throw new Exception("Layer is not the same.");
                            }
                            else
                            {
                                state1 = iterate;
                                state2 = targetState;
                            }
                        }
                        else
                        {
                            state1 = sourceState;
                            state2 = targetState;
                        }

                        while (state1.ParentStateId != state2.ParentStateId)
                        {
                            // Find lowest layer with same parent
                            state1 = currentSpec.States[state1.ParentStateId];
                            state2 = currentSpec.States[state2.ParentStateId];
                        }
                        
                        // Now we have the highest parent on both sides
                        
                        // State 1 and all children need to be removed from X
                        // Either we find all states or we have the information in the states
                        // Having the information seems way easier for the calculation
                        // But not as easy for the memory
                        // E.g. recursive
                        foreach (int stateId in state1.GetSubStateIds())
                        {
                            // TODO: Add left states to behavior execution queue
                            
                            currentStateIds.Remove(stateId);
                        }
                        
                        // State 2 and all children need to be added to X
                        foreach (int stateId in state2.GetInitialStateIds())
                        {
                            // TODO: Add entered states to behavior execution queue
                            
                            if (currentSpec.States[stateId].ParentStateId == targetState.ParentStateId)
                            {
                                currentStateIds.Add(targetState.Id);

                                if (targetState.IsIntermediate)
                                {
                                    // TODO: Add transition to I
                                    // Find transition?? That kinda sucks
                                }
                            }
                            else
                            {
                                currentStateIds.Add(stateId);
                                
                                if (currentSpec.States[stateId].IsIntermediate)
                                {
                                    // TODO: Add transition to I
                                    // Find transition?? That kinda sucks
                                }
                            }
                            
                        }
                        
                    }
                    else
                    {
                        // Ignore for now
                        
                        // remove transition?
                        // probably not necessary
                    }
                }

                // Step 6: Redo 4+5 with I
                if (I.Count > 0)
                {
                    T.Clear();

                    foreach (Transition t in I)
                    {
                        T.Add(t);
                    }

                    I.Clear();
                    // sort? probably not, because we go by order again from the beginning
                    // we just have to make sure we actually do that
                }
                else
                {
                    break;
                }
            }

            // Step 7: Replace fullConfiguration with X
            /*_fullConfiguration = new List<State>();
            foreach (State state in x)
            {
                _fullConfiguration.Add(state);
            }*/
        }
        
        /// <summary>
        /// Executes step function for all instances. This is the important function.
        /// </summary>
        public void ExecuteStepAll()
        {
            // Space Complexity
            for (int i = 0; i < instances.Length; i++)
            {
                ExecuteStep(i);
            }
        }

        /// <summary>
        /// Same as ExecuteStepAll but parallel
        /// </summary>
        public void ExecuteStepAllParallel()
        {
            // Parallel? Im not sure if the memory thing works for parallel execution.
            // Just test it i guess.
        }

        /// <summary>
        /// Executes behavior for all instances. This is important later.
        /// </summary>
        public void ExecuteBehaviorAll()
        {
            
        }
        
        /// <summary>
        /// This function creates a specification based on a certain input, saves it in the (Dictionary?)
        /// for specifications and returns the key as an integer so one can create more instances.
        /// </summary>
        /// <param name="json">A string formatted as a json that specifies the StateChart</param>
        /// <returns>Key that is used to save the created specification in the (Dictionary?)</returns>
        /// <exception cref="NotImplementedException"></exception>
        public int CreateSpecification(string json)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an Instance of a given specification ID.
        /// </summary>
        /// <param name="specification">ID of the specification that is used to create the Instance</param>
        /// <returns>ID of the Instance</returns>
        /// <exception cref="NotImplementedException"></exception>
        public int CreateInstance(int specification)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tricky because we want everything to be in one place in memory.
        /// We dont want a lag spike when someone dies.
        /// For now it doesnt matter. Important is the Step-Function. Stuff like this could be in the "Future work" Section
        /// Removes instance
        /// </summary>
        /// <param name="instanceId"></param>
        public void RemoveInstance(int instanceId)
        {
            // Evaluation -> Critique
        }

        /// <summary>
        /// Sets a internal Condition/Event
        /// </summary>
        /// <param name="instanceId">ID of the instance that the condition is supposed to be set</param>
        /// <param name="name">name of the condition that is supposed to be set</param>
        /// <param name="value">value of the condition variable</param>
        public void SetInstanceCondition(int instanceId, string name, bool value)
        {
            // Probably needs to be split into different types (bool, int, float, trigger)
            // Equivalent to Unity-Animator variables
        }
    }
}