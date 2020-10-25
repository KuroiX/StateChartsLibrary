namespace StateCharts
{
    public abstract class StateBehavior
    {
        public abstract void OnStateEnter(Specification specification, Instance instance);
        public abstract void OnStateUpdate(Specification specification, Instance instance);
        public abstract void OnStateExit(Specification specification, Instance instance);
    }
}