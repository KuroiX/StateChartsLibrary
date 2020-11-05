namespace StateCharts
{
    public abstract class StateBehavior
    {
        public abstract void OnStateEnter(Specification specification, Entity entity);
        public abstract void OnStateUpdate(Specification specification, Entity entity);
        public abstract void OnStateExit(Specification specification, Entity entity);
    }
}