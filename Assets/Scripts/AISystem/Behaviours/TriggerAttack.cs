using UnityEngine;
using AISystem.BehaviourTrees;
using AISystem.Data;

namespace Behaviours
{
    public class TriggerAttack : ActionNode
    {
        public Attack m_attack;
        Animator m_animator;
        float m_startTime;

        protected override void BeginNode()
        {
            base.BeginNode();
            m_animator = m_input.m_go.GetComponentInChildren<Animator>();
            m_animator.SetTrigger(m_attack.m_attackHandle);
            m_input.m_aIMovement.m_swordCollider.enabled = true;
            m_startTime = Time.time;

            m_input.m_aIMovement.SetWarp(false);            
        }

        protected override NodeState Update(float dt)
        {
            if (Time.time < m_startTime + m_attack.m_attackDuration)
            {
                return NodeState.RUNNING;
            }

            m_input.m_aIMovement.SetWarp(true);
            return NodeState.SUCCESS;
        }
    }
}
