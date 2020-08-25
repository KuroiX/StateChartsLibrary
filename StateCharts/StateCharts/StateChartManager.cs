using StateCharts.States;

namespace StateCharts
{
    /// <summary>
    /// Manager for all StateCharts
    /// </summary>
    public class StateChartManager
    {
        // Conditions
        // Events

        /// <summary>
        /// This method is supposed to be the 'next(X, C, E)' from the paper.
        ///
        /// For now I think C and E might be members of the manager instead of being part of the function head.
        ///
        /// The single Update-method is probably better for usage with Unitys built-in ECS and job system
        /// </summary>
        /// <param name="config">The configuration that wants the next timeframe calculated</param>
        public static void UpdateConfiguration(int config) 
        {
            // TODO: important stuff here!
        }
        
        /// <summary>
        /// If you don't have a build in ECS or job system like unity, you can use this function fairly well
        /// </summary>
        /// <param name="configs">All configurations that want to have the next timeframe calculated</param>
        public static void UpdateConfigurations(int[] configs)
        {
            // TODO: parallel threads or sth
            
            foreach (int i in configs)
            {
                UpdateConfiguration(i);
            }
        }

        /// <summary>
        /// Possible function for executing the behavior of the states the configuration is in at that moment in time
        /// </summary>
        /// <param name="config"></param>
        public static void ExecuteBehavior(int config)
        {
            
        }
    }
}