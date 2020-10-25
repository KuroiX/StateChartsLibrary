using System;
using System.Collections.Generic;

namespace StateCharts
{
    public struct State
    {
        //public State parentState { get; }

        public int ParentStateId { get; }
        public int Layer { get; }
        
        // Not sure if I want to have this (or need it)
        public int Id { get; set; }
        
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
            Id = 0;
        }

        public State(int parentId, int layer, int id)
        {
            ParentStateId = parentId;
            Layer = layer;
            Id = id;
            IsIntermediate = false;
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