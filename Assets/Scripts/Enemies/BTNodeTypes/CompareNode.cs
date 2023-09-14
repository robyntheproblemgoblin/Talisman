namespace BehaviourTree
{
    public abstract class CompareNode : Node
    {
        public override NodeState Evaluate()
        {
            return Compare() ? NodeState.SUCCESS : NodeState.FAILURE;
        }

        protected abstract bool Compare();
    }
}
