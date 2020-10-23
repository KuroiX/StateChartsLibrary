using System;
using System.Collections.Generic;

namespace StateCharts.DOP
{
    public class State
    {
        //public State parentState { get; }

        public int ParentStateId { get; }
        public int Layer { get; }
        
        // Not sure if I want to have this (or need it)
        public int Id { get; }
        
        public bool IsIntermediate { get; }
        
        public State()
        {
            // TODO
            ParentStateId = -1;
            Layer = 0;
            IsIntermediate = false;
            Id = 0;
        }

        public State(State parent)
        {
            // TODO
            Layer = parent.Layer + 1;
        }

        public List<int> GetSubStateIds()
        {
            throw new NotImplementedException("GetSubStateIds not implemented");
        }

        public List<int> GetInitialStateIds()
        {
            throw new NotImplementedException("GetInitialStateIds not implemented");
        }
    }
}