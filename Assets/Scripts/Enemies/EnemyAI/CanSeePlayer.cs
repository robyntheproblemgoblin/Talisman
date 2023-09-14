using BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

public class CanSeePlayer : CompareNode
{
    Transform m_transform;

    int m_playerLayerMask = LayerMask.GetMask("Player");    
    float m_fovRange;

    public CanSeePlayer(Transform transform, float fovRange)
    {
        m_transform = transform;        
        m_fovRange = fovRange;
    }

    protected override bool Compare()
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
                    return true;
                }
            }
            return false;
        }
        else
        {
            return CanSee(t.position);
        }
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
