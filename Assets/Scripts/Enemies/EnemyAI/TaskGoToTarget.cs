using BehaviourTree;
using UnityEngine;

public class TaskGoToTarget : Node
{
    Transform m_transform;
    public TaskGoToTarget(Transform transform)
    {
        m_transform = transform;
    }
    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if(Vector3.Distance(m_transform.position, target.position) > 0.01f)
        {
            m_transform.position = Vector3.MoveTowards(
                m_transform.position, 
                target.position,
                MeleeBT.m_speed * Time.deltaTime);
            m_transform.LookAt(target.position);
        }

        m_state = NodeState.RUNNING;
        return m_state;
    }
}
