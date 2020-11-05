using System.Collections.Generic;

namespace StateCharts
{
    public struct Entity
    {
        // TODO: proper constructor
        
        public Entity(int specificationId)
        {
            SpecificationId = specificationId;
            Config = new List<int>();
            Bools = new Dictionary<int, bool>();
            Triggers = new Dictionary<int, bool>();
            Ints = new Dictionary<int, int>();
            Floats = new Dictionary<int, float>();
        }

        public int SpecificationId { get; }
        public List<int> Config { get; set; }

        
        public Dictionary<int, bool> Bools { get; set; }
        public Dictionary<int, bool> Triggers { get; set; }
        public Dictionary<int, int> Ints { get; set; }
        public Dictionary<int, float> Floats { get; set; }

    }
}