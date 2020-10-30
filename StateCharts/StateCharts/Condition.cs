using System.Runtime.InteropServices;

namespace StateCharts
{
    public struct Condition
    {
        public Condition(int type, int key)
        {
            Type = type;
            Key = key;
            Value = 0;
        }
        
        public Condition(int type, int key, int value)
        {
            Type = type;
            Key = key;
            Value = value;
        }
        
        public Condition(int type, int key, float value)
        {
            Type = type;
            Key = key;
            unsafe
            {
                Value = * (int*) &value;
            }
        }
        
        // REFERENCE TYPE
        //public string Name { get; set; }
        
        public int Type { get; }
        public int Key { get; }
        public int Value { get; }
    }
}