using System;
using System.Collections.Generic;

namespace StateCharts
{
    // TODO: this class MUST be split in data and logic or entity and system
    public class Configuration
    {
        // probably as ID for better data management
        private Specification _specification;
        
        // current active states
        // Bit masks! problem: how many layers? finite possibilities, but fast
        // other possibility: layers as an object and working on the each layer step by step
        // other possibility: AND, OR, XOR composition

        // priority?
    }
}