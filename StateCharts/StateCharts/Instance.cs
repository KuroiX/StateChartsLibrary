using System.Collections.Generic;

namespace StateCharts
{
    public class Instance
    {
        public Instance(int specificationId)
        {
            SpecificationId = specificationId;
            // TODO: initial state
            Config = new List<int>();
            Bools = new Dictionary<string, bool>();
            Triggers = new Dictionary<string, bool>();
            Ints = new Dictionary<string, int>();
            Floats = new Dictionary<string, float>();
        }

        // Information about the specification
        public int SpecificationId { get; }
        
        // Information about the Configuration
        // TODO: create something like a bit mask or something faster
        // Bit masks! problem: how many layers? finite possibilities, but fast
        // other possibility: layers as an object and working on the each layer step by step
        // other possibility: AND, OR, XOR composition
        // Basic idea: each state is one digit of int/long
        // With int as the option, we can save configurations with up to 12 parallel states on the lowest level
        // With long, we can go up to 20
        // Question: Is it worth it? How long does de- and encoding take?
        public List<int> Config { get; set; }

        // Info about the Conditions/Events
        // Best case all in one place memory wise
        public Dictionary<string, bool> Bools { get; set; }
        public Dictionary<string, bool> Triggers { get; set; }
        public Dictionary<string, int> Ints { get; set; }
        public Dictionary<string, float> Floats { get; set; }

    }
}