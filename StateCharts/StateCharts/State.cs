using System;
using System.Collections.Generic;

namespace StateCharts
{
    public struct State
    {
        //public State parentState { get; }

        //public int LayerMask { get; }
        public int ParentStateId { get; }
        public int Layer { get; }
        // Combination of Id and LayerMask
        public int IdMask { get; }
        public int InitialMask { get; }
        
        public bool IsIntermediate { get; }
        
        // Does this part make sense? We will always have at least as many AtomicStates as SuperStates+OrthogonalStates
        // So it is a "huge" memory overhead
        //public bool IsSuperstate { get; }
        
        //public bool IsOrthogonalState { get; }
        
        //public long ChildrenInitialStateIds { get; }
        
        public State(int i)
        {
            // TODO
            ParentStateId = -1;
            Layer = 0;
            IsIntermediate = false;
            //IsSuperstate = false;
            //IsOrthogonalState = false;
            //ChildrenInitialStateIds = 0;
            IdMask = 0;
            InitialMask = 0;
        }

        public State(int parentId, int layer, int idMask, int initialMask)
        {
            ParentStateId = parentId;
            Layer = layer;
            IdMask = idMask;
            IsIntermediate = false;
            InitialMask = initialMask;
        }

        /*
        public State(State parent)
        {
            // TODO
            Layer = parent.Layer + 1;
            ParentStateId = parent.Id;
            Id = 0;
            ChildrenInitialStateIds = 0;
        }
        */
    }
}