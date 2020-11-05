using System;
using System.Runtime.InteropServices;
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

            int id = mySystem.CreateSpecification("");
            
            
            Console.WriteLine("Nice");

            for (int i = 0; i < 1000; i++)
            {
                mySystem.CreateInstance(id);
            }
            int instanceId = mySystem.CreateInstance(id);

            Console.WriteLine("DoubleNice");

            for (int i = 0; i < 10; i++)
            {
                mySystem.ExecuteStepAll();
            }

            Console.WriteLine("UltraTripleNice");
        }
    }

    interface myInterface
    {
        void Test();
    }

    struct interfaceStruct : myInterface
    {
        public void Test()
        {
            throw new NotImplementedException();
        }
    }
}