using System;
using System.Collections.Generic;

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
            specVariables = new Dictionary<int, SpecificationVariables>();
            Entities = new Entity[0];
        }
        
        // Specifications
        // Different types of Behaviors: Player, Enemy, Boss, ...
        private Dictionary<int, Specification> specifications;
        private Dictionary<int, SpecificationVariables> specVariables;
        
        // InstanceArray
        // One instance consists of Configuration, Conditions and Events
        public Entity[] Entities { get; private set; }
        
        // Maybe external conditions here?
        
        #region Execute Behavior
        /*
        private void ExecuteEnterBehavior(int instanceId, int stateId)
        {
            foreach(StateBehavior behavior in specifications[instanceId].Behavior[stateId])
            {
                // TODO: if instance stays value type, we need to make it have a return value
                // not really, because so far the conditions/events are reference types
                
                // possible improvement: just give behavior system and id
                behavior.OnStateEnter(specifications[instanceId], Instances[instanceId]);
            }
        }

        private void ExecuteUpdateBehavior(int instanceId, int stateId)
        {
            foreach(StateBehavior behavior in specifications[instanceId].Behavior[stateId])
            {
                // TODO: if instance stays value type, we need to make it have a return value
                // not really, because so far the conditions/events are reference types
                behavior.OnStateUpdate(specifications[instanceId], Instances[instanceId]);
            }
        }

        private void ExecuteExitBehavior(int instanceId, int stateId)
        {
            foreach(StateBehavior behavior in specifications[instanceId].Behavior[stateId])
            {
                // TODO: if instance stays value type, we need to make it have a return value
                // not really, because so far the conditions/events are reference types
                behavior.OnStateExit(specifications[instanceId], Instances[instanceId]);
            }
        }

        private void ExecuteTransitionBehavior()
        {
            
        }
        */
        #endregion
        
        /// <summary>
        /// Executes step function for one instance. This is the important function.
        /// </summary>
        /// <param name="instanceId">The Id of the instance.</param>
        public void ExecuteStep(int instanceId)
        {
            Entity current = Entities[instanceId];
            Specification currentSpec = specifications[current.SpecificationId];
            
            #region Copy configuration
            
            List<int> currentStateIds = new List<int>();
            foreach (int state in current.Config)
            {
                currentStateIds.Add(state);
            }
            
            #endregion

            #region Execute maximal subset of non-conflicting transitions by priority

            // For each transition with source state in X
            int[] sourceIds = currentSpec.SourceIds;
            for (int t = 0; t < sourceIds.Length; t++) 
            {
                // Check if source state is still part of X'
                if (currentStateIds.Contains(sourceIds[t]) && current.Config.Contains(sourceIds[t]))
                {
                    bool result = true;

                    Condition[] conditions = currentSpec.Conditions[t];
                    // Check if conditions/events evaluate to true
                    for (int j = 0; j < conditions.Length; j++)
                    {
                        // CARE: Might have to work with copies of Triggers, Bools, and so on later 
                        // when doing the behavior

                        #region Evaluate condition
                        Condition c = conditions[j];

                        switch (c.Type)
                        {
                            case 0:
                                result &= current.Triggers[c.Key];
                                break;
                            case 1:
                                result &= current.Bools[c.Key];
                                break;
                            case 2:
                                result &= !current.Bools[c.Key];
                                break;
                            case 3:
                                result &= (current.Ints[c.Key] < c.Value);
                                break;
                            case 4:
                                result &= (current.Ints[c.Key] > c.Value);
                                break;
                            case 5:
                                result &= (current.Ints[c.Key] == c.Value);
                                break;
                            case 6:
                                result &= (current.Ints[c.Key] != c.Value);
                                break;
                            case 7:
                                int f = c.Value;
                                unsafe
                                {
                                    result &= (current.Floats[c.Key] < *(float*) &f);
                                }
                                break;
                            case 8:
                                int g = c.Value;
                                unsafe
                                {
                                    result &= (current.Floats[c.Key] > *(float*) &g);
                                }
                                break;
                            default:
                                // Error handling
                                break;
                        }

                        #endregion
                        
                        if (!result) break;
                    }
                    
                    // If transition is relevant, update X' accordingly
                    if (result)
                    {
                        #region Find closest common relative (transition mask)

                        int sourceId = sourceIds[t];
                        int targetId = currentSpec.TargetIds[t];

                        int transitionMask = 15;
                        
                        for (int i = 1; i < 8; i++)
                        {
                            if ((targetId & transitionMask) == (sourceId & transitionMask))
                            {
                                transitionMask = (transitionMask << 4) + 15;
                            }
                            else
                            {
                                break;
                            }
                        }

                        #endregion

                        #region Remove states
                        
                        int sourceSystemIdMask = sourceId & transitionMask;
                        
                        for (int i = currentStateIds.Count - 1 ; i >= 0; i--)
                        {
                            if ((currentStateIds[i] & sourceSystemIdMask) == sourceSystemIdMask)
                            {
                                // Remove
                                currentStateIds.RemoveAt(i);
                            }
                        }
                        
                        #endregion
                        
                        #region Add states

                        // For debugging:
                        //string binary = Convert.ToString(transitionMask, 2);
                        
                        int targetSystemIdMask = targetId & transitionMask;
                        
                        AddStatesRec(currentStateIds, currentSpec.InitialStates, targetSystemIdMask, targetId, transitionMask);

                        #endregion
                        
                    }
                }
            }

            #endregion
            
            #region Clear Events (triggers)

            // Should be moved to the beginning of the function when behavior is executed too
            foreach (int key in current.Triggers.Keys)
            {
                current.Triggers[key] = false;
            }

            #endregion
            
            current.Config = currentStateIds;
            Entities[instanceId] = current;
        }

        void AddStatesRec(List<int> config, Dictionary<int, int[]> initialStates, int targetSystemMask, int targetId, int layerMask)
        {
            if (targetId == targetSystemMask)
            {
                AddStatesRecSimple(config, initialStates, targetId);
                return;
            }
            
            config.Add(targetSystemMask);
            layerMask = (layerMask << 4) + 15;
            
            if (!initialStates.ContainsKey(targetSystemMask))
            {
                return;
            }

            int[] states = initialStates[targetSystemMask];

            if (states.Length > 1)
            {
                for (int i = 0; i < states.Length; i++)
                {
                    if ((targetId & layerMask) == (states[i] & layerMask))
                    {
                        AddStatesRec(config, initialStates, states[i], targetId, layerMask);
                    }
                    else
                    {
                        AddStatesRecSimple(config, initialStates, states[i]);
                    }
                }
            }
            else
            {
                if ((targetId & layerMask) == targetId)
                {
                    AddStatesRecSimple(config, initialStates, targetId);
                }
                else
                {
                    AddStatesRec(config, initialStates, (targetId & layerMask), targetId, layerMask);
                }
            }

            // Old version and other stuff
            /*for (int i = 0; i < length; i++)
            {
                int idMask = states[i];
                if (length >= 2)
                {
                    // Parallel states
                    // This means we do not have to check for targetIdMask, as parallel states are never 
                    // targets for transitions
                    // (the parent is, so indirectly every orthogonal state gets entered anyways)
                    
                    // Our targetIdMask is a child of one of the orthogonal states, but not the other ones
                    // Let's check
                    
                    // We need the currentLayerMask for this
                    int targetParent = (targetId & layerMask);
                    int test = (targetParent & idMask);
                    if (test != targetParent)
                    {
                        // The current idMask is in a different orthogonal component than the target state
                        // and can therefore be added without problems
                        AddSubStatesRecSimple(config, initialStates, idMask);
                    }
                    else
                    {
                        // The current idMask is in the same orthogonal component
                        // We can add the current component without checking, but need to check in recursive function calls
                        AddSubStatesRec(config, initialStates, idMask, targetId, (layerMask << 4) + 15);
                    }
                }
                else
                {
                    // Check if the targetIdMask is the current layer

                    int test = targetId & layerMask;
                    if (test == targetId)
                    {
                        // If yes, we simply add targetId
                        AddSubStatesRecSimple(config, initialStates, targetId);
                    }
                    else
                    {
                        // If no, we are still in a higher layer than targetIdMask
                        if ((targetId & layerMask) == (idMask & layerMask))
                        {
                            // In this case, idMask is a parent of targetIdMask and we can add it
                            AddSubStatesRec(config, initialStates, idMask, targetId, (layerMask << 4) + 15);
                        }
                        else
                        {
                            // In this case, our idMask is not a parent, so we need to add the parent
                            AddSubStatesRec(config, initialStates, (targetId & layerMask), targetId,
                                (layerMask << 4) + 15);
                        }
                    }
                }
                
                // Test for same parent
                //int layer = FindLayer(targetIdMask);

                //int currentMask = (-1) - (15 << layer);
                /*
                int currentMask = parentIdMask;
                int parent = (currentMask & targetIdMask);
                int currentParent = (idMask & currentMask);

                if (parent == currentParent && parent != idMask)
                {
                    // This should be the case when they share the same parent and therefore we need
                    // to execute everything from now on with targetIdMask as targetSystemIdMask
                    AddSubStatesRecSimple(config, targetIdMask, initialStates);
                    
                    // Might be optimized so that AddSubStatesRecSimple gets executed for other sub-trees as well
                }
                else
                {
                    AddSubStatesRec(config, initialStates, idMask, targetIdMask, parentIdMask);
                }
                }
                */

        }
        
        void AddStatesRecSimple(List<int> config, Dictionary<int, int[]> initialStates, int targetSystemIdMask)
        {
            config.Add(targetSystemIdMask);
            
            if (!initialStates.ContainsKey(targetSystemIdMask))
            {
                return;
            }

            int[] states = initialStates[targetSystemIdMask];

            for (int i = 0; i < states.Length; i++)
            {
                AddStatesRecSimple(config, initialStates, states[i]);
            }
        }

        /// <summary>
        /// Executes step function for all instances. This is the important function.
        /// </summary>
        public void ExecuteStepAll()
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                ExecuteStep(i);
            }
        }

        /// <summary>
        /// Same as ExecuteStepAll but parallel
        /// </summary>
        public void ExecuteStepAllParallel()
        {
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
            switch (json)
            {
                case "0":
                    return CreateTest0();
                case "1":
                    return CreateTest1();
                case "2":
                    return CreateTest2();
                case "3":
                    return CreateTest3();
                case "4":
                    return CreateTest4();
                case "5":
                    return CreateTest5();
                default:
                    return -1;
            }
        }
        
        #region Lazy test implementations

        private int CreateTest0()
        {
            Specification specification = new Specification();
            
            int size = specifications.Count;
            specifications.Add(size, specification);
            return size;
        }
        
        private int CreateTest1()
        {
            //throw new NotImplementedException();

            #region States
            State AB = new State(-1, 0, 1, 0);

            State A = new State(AB.IdMask, AB.Layer + 1, AB.IdMask + 1*16, AB.IdMask);
            State B = new State(AB.IdMask, AB.Layer + 1, AB.IdMask + 2*16, 0);

            
            State Y = new State(-1, 0, 2, 0);

            State Y1 = new State(Y.IdMask, 1, Y.IdMask + 1 * 16, Y.IdMask);
            State Y2 = new State(Y.IdMask, 1, Y.IdMask + 2 * 16, Y.IdMask);
            
            State C = new State(Y1.IdMask, Y1.Layer +1, Y1.IdMask + 1*16*16, Y1.IdMask);
            State D = new State(Y1.IdMask, Y1.Layer +1, Y1.IdMask + 2*16*16, 0);
            State E = new State(Y1.IdMask, Y1.Layer +1, Y1.IdMask + 3*16*16, 0);
            State F = new State(Y2.IdMask, Y2.Layer +1, Y2.IdMask + 1*16*16, Y2.IdMask);
            State G = new State(Y2.IdMask, Y2.Layer +1, Y2.IdMask + 2*16*16, 0);
            
            List<State> states = new List<State>();
            states.Add(AB);
            states.Add(A);
            states.Add(B);
            states.Add(Y);
            states.Add(Y1);
            states.Add(Y2);
            states.Add(C);
            states.Add(D);
            states.Add(E);
            states.Add(F);
            states.Add(G);
            #endregion
            
            #region Transitions
            //Transition AtoB = new Transition(A.IdMask, B.IdMask, 0);
            //Transition BtoY = new Transition(B.IdMask, Y.IdMask, 1);
            //Transition CtoD = new Transition(C.IdMask, D.IdMask, 2);
            //Transition DtoE = new Transition(D.IdMask, E.IdMask, 3);
            //Transition FtoG = new Transition(F.IdMask, G.IdMask, 5);
            //Transition GtoAB = new Transition(G.IdMask, AB.IdMask, 4);

            int[] sourceIds = {A.IdMask, B.IdMask, C.IdMask, D.IdMask, G.IdMask, F.IdMask };
            int[] targetIds = {B.IdMask, Y.IdMask, D.IdMask, E.IdMask, AB.IdMask, G.IdMask };
            
            
            #endregion
            
            #region Conditions
            // BoolTrue
            Condition cAB = new Condition(1, 0);
            // BoolFalse
            Condition cBY = new Condition(2, 1);
            // IntLess
            Condition cCD = new Condition(3, 2, 1);
            // IntGreater
            Condition cDE = new Condition(4, 3, -1);
            // IntEq
            Condition cFG = new Condition(5, 4, 0);
            // IntNeq
            Condition cGAB = new Condition(6, 5, 1);

            Condition[][] conditions =
            {
                new [] {cAB},
                new [] {cBY},
                new [] {cCD},
                new [] {cDE},
                new [] {cGAB},
                new [] {cFG},
            };

            //AtoB.Conditions.Add(cAB);
            //BtoY.Conditions.Add(cBY);
            //CtoD.Conditions.Add(cCD);
            //DtoE.Conditions.Add(cDE);
            //GtoAB.Conditions.Add(cGAB);
            //FtoG.Conditions.Add(cFG);
            
            TransitionCollection transitions = new TransitionCollection(sourceIds, targetIds, conditions);

            #endregion
            
            #region Specification

            Specification specification = new Specification();
            //specification.InitialStates[-1] = new List<int>() {AB.IdMask};

            //int[] idMasks = { AB.IdMask, A.IdMask, B.IdMask, Y.IdMask, Y1.IdMask, Y2.IdMask, C.IdMask, D.IdMask, E.IdMask, F.IdMask, G.IdMask };
            //int[] initialMasks = { AB.IdMask, Y.IdMask, Y.IdMask };
            
            

            Dictionary<int, int[]> initialStates = new Dictionary<int, int[]>
            {
                [0] = new[] {AB.IdMask},
                [AB.IdMask] = new[] {A.IdMask}, [Y.IdMask] = new[] {Y1.IdMask, Y2.IdMask}, [Y1.IdMask] = new[] {C.IdMask}, [Y2.IdMask] = new[] {F.IdMask}
            };

            /*foreach (State state in states)
            {
                if (!initialStates.ContainsKey(state.IdMask))
                {
                    initialStates[state.IdMask] = new int[0];
                }
            }*/

            specification.InitialStates = initialStates;

            //specification.States.Add(AB.IdMask, AB);
            //specification.States.Add(A.IdMask, A);
            //specification.States.Add(B.IdMask, B);
            
            //specification.States.Add(Y.IdMask, Y);
            //specification.States.Add(C.IdMask, C);
            //specification.States.Add(D.IdMask, D);
            //specification.States.Add(E.IdMask, E);
            //specification.States.Add(F.IdMask, F);
            //specification.States.Add(G.IdMask, G);
            
            //specification.Transitions.Add(AtoB);
            //specification.Transitions.Add(BtoY);
            //specification.Transitions.Add(CtoD);
            //specification.Transitions.Add(DtoE);
            //specification.Transitions.Add(FtoG);
            //specification.Transitions.Add(GtoAB);

            //specification.Transitions = transitions;
            specification.SourceIds = sourceIds;
            specification.TargetIds = targetIds;
            specification.Conditions = conditions;

            SpecificationVariables variables = new SpecificationVariables();
            
            variables.Bools = new Dictionary<int, bool>();
            variables.Ints = new Dictionary<int, int>();
            variables.Floats = new Dictionary<int, float>();
            variables.Triggers = new Dictionary<int, bool>();
            
            variables.Bools.Add(0, true);
            variables.Bools.Add(1, false);

            variables.Ints.Add(2, 0);
            variables.Ints.Add(3, 0);
            variables.Ints.Add(4, 0);
            variables.Ints.Add(5, 0);

            //specification.Hierarchy[0] = new List<int> {1, 2};
            //specification.Hierarchy[3] = new List<int> {4, 5, 6, 7, 8};

            //specification.InitialStates[1] = new List<int> {1+1*16};
            //specification.InitialStates[2] = new List<int> {2+1*16, 2+4*16};

            #endregion

            // how do i get a random key?
            int size = specifications.Count;
            specifications.Add(size, specification);
            specVariables.Add(size, variables);
            return size;
        }

        private int CreateTest2()
        {
            //throw new NotImplementedException();

            #region States

            int A = 1;
            int B = 2;
            int B2 = B + 1 * 16;
            int B3 = B2 + 1 * 16 * 16;
            int B4 = B3 + 1 * 16 * 16 * 16;
            int B5 = B4 + 1 * 16 * 16 * 16 * 16;
            int B6 = B5 + 1 * 16 * 16 * 16 * 16 * 16;
            int B7 = B6 + 1 * 16 * 16 * 16 * 16 * 16 * 16;
            int B8 = B7 + 1 * 16 * 16 * 16 * 16 * 16 * 16 * 16;
            int B9 = B7 + 2 * 16 * 16 * 16 * 16 * 16 * 16 * 16;
            
            #endregion
            
            #region Transitions & Conditions

            int[] sourceIds = {A, B9};
            int[] targetIds = {B9, A};
            
            // BoolTrue
            Condition cAB = new Condition(1, 0);
            // BoolFalse
            Condition cBY = new Condition(2, 1);

            Condition[][] conditions =
            {
                new [] {cAB},
                new [] {cBY},
            };

            TransitionCollection transitions = new TransitionCollection(sourceIds, targetIds, conditions);

            #endregion
            
            
            #region Specification

            Specification specification = new Specification();

            Dictionary<int, int[]> initialStates = new Dictionary<int, int[]>
            {
                [0] = new[] {A}, [B] = new[] {B2}, [B2] = new[] {B3}, [B3] = new[] {B4}, [B4] = new[] {B5}, [B5] = new[] {B6}, [B6] = new[] {B7}, [B7] = new[] {B8} 
            };

            specification.InitialStates = initialStates;

            specification.SourceIds = sourceIds;
            specification.TargetIds = targetIds;
            specification.Conditions = conditions;

            SpecificationVariables variables = new SpecificationVariables();
            
            variables.Bools = new Dictionary<int, bool>();
            variables.Ints = new Dictionary<int, int>();
            variables.Floats = new Dictionary<int, float>();
            variables.Triggers = new Dictionary<int, bool>();
            
            variables.Bools.Add(0, true);
            variables.Bools.Add(1, false);

            #endregion

            // how do i get a random key?
            int size = specifications.Count;
            specifications.Add(size, specification);
            specVariables.Add(size, variables);
            return size;
        }
        
        private int CreateTest3()
        {
            //throw new NotImplementedException();

            #region States

            int A = 1;
            int B = 2;
            int B2 = B + 1 * 16;
            int B3 = B2 + 1 * 16 * 16;
            int B4 = B3 + 1 * 16 * 16 * 16;
            int B5 = B4 + 1 * 16 * 16 * 16 * 16;
            int B6 = B5 + 1 * 16 * 16 * 16 * 16 * 16;
            int B7 = B6 + 1 * 16 * 16 * 16 * 16 * 16 * 16;
            int B8 = B7 + 1 * 16 * 16 * 16 * 16 * 16 * 16 * 16;
            
            int C5 = B4 + 2 * 16 * 16 * 16 * 16;
            int C6 = C5 + 2 * 16 * 16 * 16 * 16 * 16;
            int C7 = C6 + 2 * 16 * 16 * 16 * 16 * 16 * 16;
            int C8 = C7 + 2 * 16 * 16 * 16 * 16 * 16 * 16 * 16;
            
            #endregion
            
            #region Transitions & Conditions

            int[] sourceIds = {A, C6};
            int[] targetIds = {C5, A};
            
            // BoolTrue
            Condition cAB = new Condition(1, 0);
            // BoolFalse
            Condition cBY = new Condition(2, 1);

            Condition[][] conditions =
            {
                new [] {cAB},
                new [] {cBY},
            };

            TransitionCollection transitions = new TransitionCollection(sourceIds, targetIds, conditions);

            #endregion
            
            
            #region Specification

            Specification specification = new Specification();

            Dictionary<int, int[]> initialStates = new Dictionary<int, int[]>
            {
                [0] = new[] {A}, [B] = new[] {B2}, [B2] = new[] {B3}, [B3] = new[] {B4}, [B4] = new[] {B5}, [B5] = new[] {B6}, [B6] = new[] {B7}, [B7] = new[] {B8},
                [C5] = new[] {C6}, [C6] = new[] {C7}, [C7] = new[] {C8},
            };

            specification.InitialStates = initialStates;

            specification.SourceIds = sourceIds;
            specification.TargetIds = targetIds;
            specification.Conditions = conditions;

            SpecificationVariables variables = new SpecificationVariables();
            
            variables.Bools = new Dictionary<int, bool>();
            variables.Ints = new Dictionary<int, int>();
            variables.Floats = new Dictionary<int, float>();
            variables.Triggers = new Dictionary<int, bool>();
            
            variables.Bools.Add(0, true);
            variables.Bools.Add(1, false);

            #endregion

            // how do i get a random key?
            int size = specifications.Count;
            specifications.Add(size, specification);
            specVariables.Add(size, variables);
            return size;
        }
        
        private int CreateTest4()
        {
            //throw new NotImplementedException();

            #region States

            int A = 1;
            int A2 = A + 1 * 16;
            int A3 = A2 + 1 * 16 * 16;
            
            int B = 2;
            int B2 = B + 1 * 16;
            int B3 = B2 + 1 * 16 * 16;
            int B4 = B3 + 1 * 16 * 16 * 16;
            
            #endregion
            
            #region Transitions & Conditions

            int[] sourceIds = {A3, B4};
            int[] targetIds = {B4, A2};
            
            // BoolTrue
            Condition cAB = new Condition(1, 0);
            // BoolFalse
            Condition cBY = new Condition(2, 1);

            Condition[][] conditions =
            {
                new [] {cAB},
                new [] {cBY},
            };

            TransitionCollection transitions = new TransitionCollection(sourceIds, targetIds, conditions);

            #endregion
            
            
            #region Specification

            Specification specification = new Specification();

            Dictionary<int, int[]> initialStates = new Dictionary<int, int[]>
            {
                [0] = new[] {A}, [B] = new[] {B2}, [B2] = new[] {B3}, [B3] = new[] {B4}, 
                [A] = new[] {A2}, [A2] = new[] {A3}, 
            };

            specification.InitialStates = initialStates;

            specification.SourceIds = sourceIds;
            specification.TargetIds = targetIds;
            specification.Conditions = conditions;

            SpecificationVariables variables = new SpecificationVariables();
            
            variables.Bools = new Dictionary<int, bool>();
            variables.Ints = new Dictionary<int, int>();
            variables.Floats = new Dictionary<int, float>();
            variables.Triggers = new Dictionary<int, bool>();
            
            variables.Bools.Add(0, true);
            variables.Bools.Add(1, false);

            #endregion

            // how do i get a random key?
            int size = specifications.Count;
            specifications.Add(size, specification);
            specVariables.Add(size, variables);
            return size;
        }

        private int CreateTest5()
        {
            //throw new NotImplementedException();

            #region States

            int X = 1;
            int A = X + 1 * 16;
            int A1 = A + 1 * 16 * 16;
            int A2 = A + 2 * 16 * 16;
            int B = X + 2 * 16;
            int B1 = B + 1 * 16 * 16;
            int B2 = B + 2 * 16 * 16;
            int C = B2 + 1 * 16 * 16 * 16;
            int B3 = B + 3 * 16 * 16;
            
            int Y = 2;
            int D = Y + 1 * 16;
            int D1 = D + 1 * 16 * 16;
            int D2 = D1 + 1 * 16 * 16 * 16;
            int E = Y + 2 * 16;
            int E1 = E + 1 * 16 * 16;
            int E2 = E + 2 * 16 * 16;
            int F = E2 + 1 * 16 * 16 * 16;
            int F1 = F + 1 * 16 * 16 * 16 * 16;
            int F2 = F1 + 1 * 16 * 16 * 16 * 16;
            int G = E2 + 2 * 16 * 16 * 16;
            
            #endregion
            
            #region Transitions & Conditions

            int[] sourceIds = {B1, B2, B3, D1, A1};
            int[] targetIds = {B2, B3, F2, X, A2};
            
            // BoolTrue
            Condition cAB = new Condition(1, 0);
            // BoolFalse
            Condition cBY = new Condition(2, 1);

            Condition[][] conditions =
            {
                new [] {cAB, cBY},
                new [] {cBY},
                new [] {cBY},
                new [] {cBY},
                new [] {cBY},
                new [] {cBY},
            };

            TransitionCollection transitions = new TransitionCollection(sourceIds, targetIds, conditions);

            #endregion
            
            
            #region Specification

            Specification specification = new Specification();

            Dictionary<int, int[]> initialStates = new Dictionary<int, int[]>
            {
                [0] = new[] {X},
                
                [X] = new[] {A, B}, 
                [A] = new[] {A1}, 
                [B] = new[] {B1},
                [B2] = new[] {C},
                
                [Y] = new[] {D, E},
                [D] = new[] {D1},
                [D1] = new[] {D2},

                [E] = new[] {E1},
                [E2] = new[] {F, G},
                [F] = new[] {F1},
            };

            specification.InitialStates = initialStates;

            specification.SourceIds = sourceIds;
            specification.TargetIds = targetIds;
            specification.Conditions = conditions;
            
            SpecificationVariables variables = new SpecificationVariables();
            
            variables.Bools = new Dictionary<int, bool>();
            variables.Ints = new Dictionary<int, int>();
            variables.Floats = new Dictionary<int, float>();
            variables.Triggers = new Dictionary<int, bool>();

            variables.Bools.Add(0, true);
            variables.Bools.Add(1, false);

            #endregion
            
            int size = specifications.Count;
            specifications.Add(size, specification);
            specVariables.Add(size, variables);
            return size;
        }
        
        #endregion
        
        /// <summary>
        /// Creates an Instance of a given specification ID.
        /// </summary>
        /// <param name="specificationId">ID of the specification that is used to create the Instance</param>
        /// <returns>ID of the Instance</returns>
        /// <exception cref="NotImplementedException"></exception>
        public int CreateInstance(int specificationId)
        {
            Entity entity = new Entity(specificationId);

            foreach (int key in specVariables[specificationId].Bools.Keys)
            {
                entity.Bools.Add(key, specVariables[specificationId].Bools[key]);
            }

            foreach (int key in specVariables[specificationId].Ints.Keys)
            {
                entity.Ints.Add(key, specVariables[specificationId].Ints[key]);
            }

            foreach (int key in specVariables[specificationId].Floats.Keys)
            {
                entity.Floats.Add(key, specVariables[specificationId].Floats[key]);
            }

            foreach (int key in specVariables[specificationId].Triggers.Keys)
            {
                entity.Triggers.Add(key, specVariables[specificationId].Triggers[key]);
            }

            try
            {
                int initial = specifications[specificationId].InitialStates[0][0];
                {
                    AddStatesRecSimple(entity.Config, specifications[specificationId].InitialStates, initial);
                }
            }
            catch
            {
                // ignored
            }

            Entity[] newInstances = new Entity[Entities.Length + 1];
            for (int i = 0; i < Entities.Length; i++)
            {
                newInstances[i] = Entities[i];
            }
            newInstances[Entities.Length] = entity;

            Entities = newInstances;

            return Entities.Length;

            //throw new NotImplementedException();
        }

        /// <summary>
        /// Tricky because we want everything to be in one place in memory.
        /// We dont want a lag spike when something dies for example.
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
        /// <param name="key">key of the condition that is supposed to be set</param>
        /// <param name="value">value of the condition variable</param>
        public void SetInstanceCondition(int instanceId, int key, bool value)
        {
            // Probably needs to be split into different types (bool, int, float, trigger)
            // Equivalent to Unity-Animator variables
            throw new NotImplementedException();
        }
    }
}