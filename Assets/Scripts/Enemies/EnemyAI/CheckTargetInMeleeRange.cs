using UnityEngine;
using BehaviourTree;
using UnityEngine.Rendering;

public class CheckTargetInMeleeRange : Node
{
    Transform m_transform;
    Animator m_animator;
    EnemyBT m_enemy;
    float time = 1f;
    bool canAttack = true;
    float m_attackRange;

    public CheckTargetInMeleeRange(Transform transform, EnemyBT enemy, float attackRange)
    {
        m_transform = transform;
        m_animator = transform.GetComponent<Animator>();
        m_enemy = enemy;
        m_attackRange = attackRange;
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            m_state = NodeState.FAILURE;        
            return m_state;
        }        
        Transform target = (Transform)t;
        if (Vector3.Distance(m_transform.position, target.position) <= m_attackRange && canAttack)
        {
            canAttack = false;
            m_animator.SetBool("AttackB", true);
            time = 3f;        
            m_state = NodeState.SUCCESS;        
            return m_state;
        }
        else if (!canAttack)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                canAttack = true;                
            }
            else if (time <= 2.5f)
            {
                m_animator.SetBool("AttackB", false);                
            }
            m_state = NodeState.RUNNING;               
            return m_state;
        }        
        
        m_state = NodeState.FAILURE;
        return m_state;
    }
}
