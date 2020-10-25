using System;
using StateCharts;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // For now, this should be a State Machine with three states.
            
            BehaviorSystem mySystem = new BehaviorSystem();

            int id = mySystem.CreateSpecification("");
            
            Console.WriteLine("Nice");

            for (int i = 0; i < 10000; i++)
            {
                mySystem.CreateInstance(id);
            }
            int instanceId = mySystem.CreateInstance(id);

            Console.WriteLine("DoubleNice");

            for (int i = 0; i < 1; i++)
            {
                mySystem.ExecuteStepAll();
            }

            Console.WriteLine("UltraTripleNice");
        }
    }
}