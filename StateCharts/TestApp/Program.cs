using System;
using System.Collections.Generic;
using StateCharts.OOP;

namespace TestApp
{
    class Program
    {
        struct test
        {
            public int i;
            public testi t;
        }

        class testi
        {
            public int i;
        }

        static void Main(string[] args)
        {
            test myTest;
            myTest.i = 5;
            testi t = new testi();
            t.i = 4;
            myTest.t = t;

            Dictionary<int, test> testDict = new Dictionary<int, test> {{1, myTest}};

            //Console.WriteLine(myTest.i);
            Console.WriteLine(testDict[1].i);
            //Console.WriteLine(testDict[1].t.i);

            test MYtest = new test();

            testDict[1] = MYtest;
            testDict[2] = myTest;

            
            MYtest.i = 3;

            myTest.i = 0;
            myTest.t.i = 0;
            
            //Console.WriteLine(myTest.i);
            Console.WriteLine(testDict[1].i);
            Console.WriteLine(testDict[2].i);
            //Console.WriteLine(testDict[1].t.i);
        }
    }
}