namespace StateCharts
{
    public abstract class StateChartBehavior
    {
        // possibly more parameters
        
        /// <summary>
        /// This function is called once when the state is entered
        /// </summary>
        public abstract void OnStateEnter();

        /// <summary>
        /// This function is called each frame (or update) while the state chart is in this state
        /// </summary>
        public abstract void OnStateUpdate();

        /// <summary>
        /// This function is called once when the state is exited
        /// </summary>
        public abstract void OnStateExit();
    }
}