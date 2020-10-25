using System;
using System.Collections.Generic;
using System.Linq;

namespace StateCharts
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
        public Instance[] instances { get; private set; }
        
        // Maybe external conditions here?
        
        #region Execute Behavior
        private void ExecuteEnterBehavior(int instanceId, int stateId)
        {
            foreach(StateBehavior behavior in specifications[instanceId].Behavior[stateId])
            {
                // TODO: if instance stays value type, we need to make it have a return value
                // not really, because so far the conditions/events are reference types
                
                // possible improvement: just give behavior system and id
                behavior.OnStateEnter(specifications[instanceId], instances[instanceId]);
            }
        }

        private void ExecuteUpdateBehavior(int instanceId, int stateId)
        {
            foreach(StateBehavior behavior in specifications[instanceId].Behavior[stateId])
            {
                // TODO: if instance stays value type, we need to make it have a return value
                // not really, because so far the conditions/events are reference types
                behavior.OnStateUpdate(specifications[instanceId], instances[instanceId]);
            }
        }

        private void ExecuteExitBehavior(int instanceId, int stateId)
        {
            foreach(StateBehavior behavior in specifications[instanceId].Behavior[stateId])
            {
                // TODO: if instance stays value type, we need to make it have a return value
                // not really, because so far the conditions/events are reference types
                behavior.OnStateExit(specifications[instanceId], instances[instanceId]);
            }
        }

        private void ExecuteTransitionBehavior()
        {
            
        }
        
        #endregion
        
        /// <summary>
        /// Executes step function for one instance. This is the important function.
        /// </summary>
        /// <param name="instanceId"></param>
        public void ExecuteStep(int instanceId)
        {
            // Helper
            Instance current = instances[instanceId];
            Specification currentSpec = specifications[current.SpecificationId];

            
            #region Step 1: Snapshot X, C and E

            // This can be made way more performant with value types (int as IDs)
            List<int> currentStateIds = new List<int>();
            foreach (int state in current.Config)
            {
                currentStateIds.Add(state);
            }
            
            // Possibility: No copy needed if the behavior is executed separately! (Might be more performant)
            //Dictionary<string, bool> bools = current.Bools.ToDictionary(entry => entry.Key, entry => entry.Value);
            //Dictionary<string, bool> triggers = current.Triggers.ToDictionary(entry => entry.Key, entry => entry.Value);
            //Dictionary<string, int> ints = current.Ints.ToDictionary(entry => entry.Key, entry => entry.Value);
            //Dictionary<string, float> floats = current.Floats.ToDictionary(entry => entry.Key, entry => entry.Value);
            

            #endregion

            /*
            #region Step 2: Clear Events (triggers)

            // Might has to be moved to the end
            foreach (string name in current.Triggers.Keys)
            {
                current.Triggers[name] = false;
            }

            #endregion
            */
            
            #region Execute maximal subset of non-conflicting transitions by priority
            
            // TODO: Test
            // Iterate twice and then sort or iterate once over sorted transitions
            
            // For each transition with source state in X
            
            foreach (Transition t in currentSpec.Transitions)
            {
                // Check if source state is still part of X'
                if (currentStateIds.Contains(t.SourceId) && current.Config.Contains(t.SourceId))
                {
                    bool result = true;
                    
                    // Check if conditions/events evaluate to true
                    foreach (Condition c in t.Conditions)
                    {
                        // CARE: Might have to work with copies of Triggers, Bools, and so on later 
                        // when doing the behavior
                        
                        // Triggers
                        if (c.Type == 0)
                        {
                            result &= current.Triggers[c.Name];
                        }
                        // Bools
                        else if (c.Type == 1)
                        {
                            if (c.Operation == 0)
                            {
                                result &= current.Bools[c.Name];
                            } 
                            else if (c.Operation == 1)
                            {
                                result &= !current.Bools[c.Name];
                            }
                        }
                        // Ints
                        else if (c.Type == 2)
                        {
                            // <
                            if (c.Operation == 0)
                            {
                                result &= (current.Ints[c.Name] < c.IntValue);
                            }
                            // >
                            else if (c.Operation == 1)
                            {
                                result &= (current.Ints[c.Name] > c.IntValue);
                            }
                            // ==
                            else if (c.Operation == 2)
                            {
                                result &= (current.Ints[c.Name] == c.IntValue);
                            }
                            // !=
                            else if (c.Operation == 3)
                            {
                                result &= (current.Ints[c.Name] != c.IntValue);
                            }
                    
                        }
                        // Floats
                        else
                        {
                            // <
                            if (c.Operation == 0)
                            {
                                result &= (current.Floats[c.Name] < c.FloatValue);
                            }
                            // >
                            else if (c.Operation == 1)
                            {
                                result &= (current.Floats[c.Name] > c.FloatValue);
                            }
                        }

                        if (!result) break;
                    }
                    
                    // If transition is relevant, update X' accordingly
                    if (result)
                    {
                        #region Find closest common relative
                        // Same parent/layer?
                        State sourceState = currentSpec.States[t.SourceId];
                        State targetState = currentSpec.States[t.TargetId];

                        State sourceSystem = sourceState;
                        State targetSystem = targetState;
                        
                        // Set sourceSystem and targetSystems layer to same layer
                        if (sourceState.Layer < targetState.Layer)
                        {
                            // Find closest relative
                            
                            // Check parent layer until parent layer = target layer
                            // we know in advance how many layers above it is

                            for (int i = 0; i < targetState.Layer - sourceState.Layer; i++)
                            {
                                targetSystem = currentSpec.States[targetSystem.ParentStateId];
                            }
                        
                            // Safety check:
                            if (targetSystem.Layer != sourceSystem.Layer)
                            {
                                throw new Exception("Layer is not the same.");
                            }

                        }
                        else if (sourceState.Layer > targetState.Layer)
                        {
                            for (int i = 0; i < sourceState.Layer - targetState.Layer; i++)
                            {
                                sourceSystem = currentSpec.States[sourceSystem.ParentStateId];
                            }
                        
                            // Safety check:
                            if (targetSystem.Layer != sourceSystem.Layer)
                            {
                                throw new Exception("Layer is not the same.");
                            }
                        }
                        
                        // At this point, we still need to have both systems have the same parent
                        while (sourceSystem.ParentStateId != targetSystem.ParentStateId)
                        {
                            // Find lowest layer with same parent
                            sourceSystem = currentSpec.States[sourceSystem.ParentStateId];
                            targetSystem = currentSpec.States[targetSystem.ParentStateId];
                        }
                        #endregion
                        
                        #region Update X'
                    
                        // State 1 and all children need to be removed from X
                        // Either we find all states or we have the information in the states
                        // Having the information seems way easier for the calculation
                        // But not as easy for the memory
                        // E.g. recursive
                        List<int> subStates = currentSpec.GetSubStates(sourceSystem.Id);
                        foreach (int stateId in subStates) 
                        {
                            // Behaviors will probably be saved in extra object?
                            // That way: I only need the ID 
                            // Also: Data oriented programming
                            //exitedStates.Add(stateId);
                            
                            currentStateIds.Remove(stateId);
                        }
                        
                        // State 2 and all children need to be added to X
                        foreach (int stateId in currentSpec.GetInitialStates(targetSystem.Id))
                        {
                            //enteredStates.Add(stateId);
                            
                            // This if is for the case that we have a transition to a lower level state
                            // than the common parent state
                            if (currentSpec.States[stateId].ParentStateId == targetState.ParentStateId)
                            {
                                currentStateIds.Add(targetState.Id);

                                if (targetState.IsIntermediate)
                                {
                                    // TODO: Add transition to I
                                    // Find transition?? That kinda sucks
                                    // Maybe intermediate transitions have their transitions set
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
                        #endregion

                    }
                    
                }
            }

            #endregion
            
            /*
            #region Step 3: Store relevant Transitions in set T

            
            // Better iterate over states in X?
            // But then states become bigger memory wise
            List<Transition> T = new List<Transition>();
            
            foreach (Transition t in specifications[current.SpecificationId].Transitions)
            {
                // TODO: Check source?
                if (!current.Config.Contains(t.SourceId)) continue;
                bool result = true;
                
                // Evaluate
                foreach (Condition c in t.Conditions)
                {
                    if (c.Type == 0)
                    {
                        // Trigger

                        result &= triggers[c.Name];
                    } 
                    else if (c.Type == 1)
                    {
                        // Bool
                        result &= (bools[c.Name] == c.BoolValue);

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
            */
            
            //List<int> exitedStates = new List<int>();
            //List<int> enteredStates = new List<int>();
            //List<int> transitedTransitions = new List<int>();

            /*
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
                
                // Instead of checking if source is in X and X' every time, we might be able to save the result
                // and look it up by ID or sth
                foreach (Transition t in T)
                {
                    // Awesome, everything works over IDs and ints
                    
                    // This if is important because we might have changed currentStateIds in a previous iteration
                    if (currentStateIds.Contains(t.SourceId))
                    {
                        // Add to transition behavior
                        //transitedTransitions.Add(t.Id);
                        
                        // Same parent/layer?
                        //Specification currentSpec = specifications[current.SpecificationId];
                        State sourceState = specifications[current.SpecificationId].States[t.SourceId];
                        State targetState = specifications[current.SpecificationId].States[t.TargetId];

                        // should all be doable with Ids
                        State state1;
                        State state2;
                        if (sourceState.Layer < targetState.Layer)
                        {
                            // Check parent layer until parent layer = target layer
                            // we know in advance how many layers above it is

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
                            // Behaviors will probably be saved in extra object?
                            // That way: I only need the ID 
                            // Also: Data oriented programming
                            //exitedStates.Add(stateId);
                            
                            currentStateIds.Remove(stateId);
                        }
                        
                        // State 2 and all children need to be added to X
                        foreach (int stateId in state2.GetInitialStateIds())
                        {
                            //enteredStates.Add(stateId);
                            
                            // This if is for the case that we have a transition to a lower level state
                            // than the common parent state
                            if (currentSpec.States[stateId].ParentStateId == targetState.ParentStateId)
                            {
                                currentStateIds.Add(targetState.Id);

                                if (targetState.IsIntermediate)
                                {
                                    // TODO: Add transition to I
                                    // Find transition?? That kinda sucks
                                    // Maybe intermediate transitions have their transitions set
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
            */
            
            // For the OnUpdates: We need to know if it was in the same state the time step before
            // so we need to check for this as well.
            // Or we have a dictionary for each instance that points from state id to enum {entered, updated, exited, (nothing)}
            
            // If we execute the behavior while entering or exiting, we only need one bool that says "entered" and then we can iterate
            // over X' pretty easily

            // Step 7: Replace fullConfiguration with X
            // This should work because we have a new currentStateIds list for every iteration
            current.Config = currentStateIds;

            /*foreach (int i in transitedTransitions)
            {
                // Execute
                
                // For now, we dont have it because we dont need it to compare to unity animator?
                ExecuteTransitionBehavior();
            }

            foreach (int stateId in specifications[instanceId].States.Keys)
            {
                if (exitedStates.Contains(stateId))
                {
                    ExecuteExitBehavior(instanceId, stateId);
                } 
                else if (enteredStates.Contains(stateId))
                {
                    ExecuteEnterBehavior(instanceId, stateId);
                }
                else
                {
                    ExecuteUpdateBehavior(instanceId, stateId);
                }
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes behavior for all instances. This is important later.
        /// </summary>
        public void ExecuteBehaviorAll()
        {
            throw new NotImplementedException();
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
            //throw new NotImplementedException();

            #region States
            State AB = new State(-1, 0, 0);

            State A = new State(AB.Id, AB.Layer + 1, 1);
            State B = new State(AB.Id, AB.Layer + 1, 2);

            
            State Y = new State(-1, 0, 3);
            
            State C = new State(Y.Id, Y.Layer +1, 4);
            State D = new State(Y.Id, Y.Layer +1, 5);
            State E = new State(Y.Id, Y.Layer +1, 6);
            State F = new State(Y.Id, Y.Layer +1, 7);
            State G = new State(Y.Id, Y.Layer +1, 8);
            #endregion
            
            #region Transitions
            Transition AtoB = new Transition(A.Id, B.Id, 0);
            Transition BtoY = new Transition(B.Id, Y.Id, 1);
            Transition CtoD = new Transition(C.Id, D.Id, 2);
            Transition DtoE = new Transition(D.Id, E.Id, 3);
            Transition FtoG = new Transition(F.Id, G.Id, 5);
            Transition GtoAB = new Transition(G.Id, AB.Id, 4);
            #endregion
            
            #region Conditions
            Condition cAB = new Condition(1, 0, "boolTrue");
            Condition cBY = new Condition(1, 1, "boolFalse");
            Condition cCD = new Condition(2, 0, "intLess", 1);
            Condition cDE = new Condition(2, 1, "intGreater", -1);
            Condition cFG = new Condition(2, 2, "intEq", 0);
            Condition cGAB = new Condition(2, 3, "intNeq", 1);
            
            AtoB.Conditions.Add(cAB);
            BtoY.Conditions.Add(cBY);
            CtoD.Conditions.Add(cCD);
            DtoE.Conditions.Add(cDE);
            GtoAB.Conditions.Add(cGAB);
            FtoG.Conditions.Add(cFG);

            #endregion
            
            #region Specification

            Specification specification = new Specification();
            specification.InitialStates[-1] = new List<int>() {AB.Id};

            specification.States.Add(AB.Id, AB);
            specification.States.Add(A.Id, A);
            specification.States.Add(B.Id, B);
            
            specification.States.Add(Y.Id, Y);
            specification.States.Add(C.Id, C);
            specification.States.Add(D.Id, D);
            specification.States.Add(E.Id, E);
            specification.States.Add(F.Id, F);
            specification.States.Add(G.Id, G);
            
            specification.Transitions.Add(AtoB);
            specification.Transitions.Add(BtoY);
            specification.Transitions.Add(CtoD);
            specification.Transitions.Add(DtoE);
            specification.Transitions.Add(FtoG);
            specification.Transitions.Add(GtoAB);

            specification.Bools.Add("boolTrue", true);
            specification.Bools.Add("boolFalse", false);

            specification.Ints.Add("intLess", 0);
            specification.Ints.Add("intGreater", 0);
            specification.Ints.Add("intEq", 0);
            specification.Ints.Add("intNeq", 0);

            specification.Hierarchy[0] = new List<int> {1, 2};
            specification.Hierarchy[3] = new List<int> {4, 5, 6, 7, 8};

            specification.InitialStates[0] = new List<int> {1};
            specification.InitialStates[3] = new List<int> {4, 7};

            #endregion

            // how do i get a random key?
            int size = specifications.Count;
            specifications.Add(size, specification);
            return size;
        }

        /// <summary>
        /// Creates an Instance of a given specification ID.
        /// </summary>
        /// <param name="specificationId">ID of the specification that is used to create the Instance</param>
        /// <returns>ID of the Instance</returns>
        /// <exception cref="NotImplementedException"></exception>
        public int CreateInstance(int specificationId)
        {
            Instance instance = new Instance(specificationId);

            foreach (string key in specifications[specificationId].Bools.Keys)
            {
                instance.Bools.Add(key, specifications[specificationId].Bools[key]);
            }

            foreach (string key in specifications[specificationId].Ints.Keys)
            {
                instance.Ints.Add(key, specifications[specificationId].Ints[key]);
            }

            foreach (string key in specifications[specificationId].Floats.Keys)
            {
                instance.Floats.Add(key, specifications[specificationId].Floats[key]);
            }

            foreach (string key in specifications[specificationId].Triggers.Keys)
            {
                instance.Triggers.Add(key, specifications[specificationId].Triggers[key]);
            }
            
            instance.Config.Add(0);
            instance.Config.Add(1);

            Instance[] newInstances = new Instance[instances.Length + 1];
            for (int i = 0; i < instances.Length; i++)
            {
                newInstances[i] = instances[i];
            }
            newInstances[instances.Length] = instance;

            instances = newInstances;

            return instances.Length;

            //throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}