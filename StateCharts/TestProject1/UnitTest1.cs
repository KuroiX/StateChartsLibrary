using System.Collections.Generic;
using NUnit.Framework;
using StateCharts.OOP;
using StateCharts.States;
using StateCharts.Transitions;

namespace Tests
{
    public class Tests
    {
        private StateChart chart;
        
        [SetUp]
        public void Setup()
        {
            chart = new StateChart();
            
            AtomicState A = new AtomicState();
            AtomicState B = new AtomicState();

            Transition ab = new Transition(A, B, chart);
            Transition ba = new Transition(B, A, chart);

            chart._initial = A;
            chart._states.Add(0, A);
            chart._states.Add(1, B);
            
            chart._fullConfiguration.Add(A);
            
            chart._transitions.Add(A, new List<Transition>{ab});
            chart._transitions.Add(B, new List<Transition>{ba});

        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}