using BehaviourTree;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class CanSeePlayer : Node
{
    Transform m_transform;

    int m_playerLayerMask = LayerMask.GetMask("Player");
    Transform targetPos;
    NavMeshAgent m_agent;    
    float m_fovRange;

    public CanSeePlayer(Transform transform, float fovRange)
    {
        m_transform = transform;
        m_agent = transform.gameObject.GetComponent<NavMeshAgent>();
        m_fovRange = fovRange;
    }

    public override NodeState Evaluate()
    {
        Transform t = (Transform)GetData("target");
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(m_transform.position, m_fovRange, m_playerLayerMask);

            if (colliders.Length > 0)
            {
                if (CanSee(colliders[0].transform.position))
                {
                    m_parent.m_parent.SetData("target", colliders[0].gameObject.transform);
                    targetPos = colliders[0].gameObject.transform;
                    m_agent.SetDestination(new Vector3(targetPos.position.x, m_agent.gameObject.transform.position.y, targetPos.position.z));
                }
            }
        }
        else if (CanSee(targetPos.position))
        {
            m_agent.SetDestination(new Vector3(targetPos.position.x, m_agent.gameObject.transform.position.y, targetPos.position.z));
        }

        m_state = NodeState.SUCCESS;
        return m_state;

    }


    bool CanSee(Vector3 pos)
    {
        Ray canSee = new Ray(m_transform.position, pos - m_transform.position);
        RaycastHit hit;

        if (Physics.Raycast(canSee, out hit, Vector3.Distance(m_transform.position, pos)))
        {
            PlayerController player = hit.transform.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                return true;
            }
        }
        return false;
    }
}
