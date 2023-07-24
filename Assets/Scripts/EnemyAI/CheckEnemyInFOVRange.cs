using BehaviourTree;
using UnityEngine;

public class CheckEnemyInFOVRange : Node
{
    Transform m_transform;

    private static int m_enemyLayerMask = 1 << 6;    

    //private Animator m_animator;

    public CheckEnemyInFOVRange(Transform transform)
    {
        m_transform = transform;
        //m_animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(m_transform.position, EnemyBT.m_fovRange, m_enemyLayerMask);

            if(colliders.Length > 0)
            {
                m_parent.m_parent.SetData("target", colliders[0].transform);
                //m_animator.SetBool("Walking", true);
                m_state = NodeState.SUCCESS;
                return m_state;
            }
            m_state = NodeState.FAILURE;
            return m_state;
        }

        m_state = NodeState.SUCCESS;
        return m_state;
    }
}
