using System;
using StateCharts;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            float f = 2.0f;
            int j;

            unsafe
            {
                j = *((int*) &f);
            }

            Console.WriteLine(j);

            test myTest = new test {floatValue = 2.0f};

            Console.WriteLine(myTest.boolValue + " " + myTest.intValue + " " + myTest.floatValue);
            */
            
            // For now, this should be a State Machine with three states.
            
            BehaviorSystem mySystem = new BehaviorSystem();

            int id = mySystem.CreateSpecification("5");
            
            
            Console.WriteLine("Nice");

            for (int i = 0; i < 100; i++)
            {
                mySystem.CreateInstance(id);
            }
            //int instanceId = mySystem.CreateInstance(id);

            Console.WriteLine("DoubleNice");

            for (int i = 0; i < 10; i++)
            {
                mySystem.ExecuteStepAll();
            }

            Console.WriteLine("UltraTripleNice");
        }
    }
}