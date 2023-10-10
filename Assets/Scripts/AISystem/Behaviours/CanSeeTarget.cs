using AISystem.Data;
using AISystem.BehaviourTrees;

namespace AISystem.Behaviours
{
    public class CanSeeTarget : ConditionalNode
    {
        protected override bool Compare(float dt)
        {
            if (m_input.m_aIKnowledge.CanSeeBeing(out BeingKnowledge info))
            {
                m_input.m_blackboard.m_target = info.m_being;
                UnityEngine.Debug.Log("Seen Target");
                return true;
            }
            UnityEngine.Debug.Log("Failed to see Target");
            return false;
        }
    }
}
