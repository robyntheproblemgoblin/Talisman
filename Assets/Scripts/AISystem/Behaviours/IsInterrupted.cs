using AISystem.Data;
using AISystem.BehaviourTrees;

namespace AISystem.Behaviours
{
    public class IsInterrupted : ConditionalNode
    {
        protected override bool Compare(float dt)
        {
            if (m_input.m_aIMovement.m_isInterrupted)
            {
                UnityEngine.Debug.Log("Interrupted");
                m_input.m_aIMovement.m_isInterrupted = false;
                return true;
            }
            UnityEngine.Debug.Log("Not Interrupted");
            return false;
        }
    }
}
