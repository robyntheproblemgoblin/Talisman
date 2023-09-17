using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskPatrol : Node
{    
    Transform[] m_waypoints;

    NavMeshAgent m_agent;
    Animator anim;

    int m_currentWaypoint;
    float m_waitTime = 2f;
    float m_waitCounter = 0f;
    bool m_waiting = false;

    public TaskPatrol(Transform transform, Transform[] waypoints, NavMeshAgent agent)
    {
        m_waypoints = waypoints;
        m_agent = agent;
        anim = agent.gameObject.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        if (m_waypoints.Length > 0)
        {
            if (m_waiting)
            {
                m_waitCounter += Time.deltaTime;
                if (m_waitCounter >= m_waitTime)
                {
                    m_agent.isStopped = false;
                    m_agent.SetDestination(m_waypoints[m_currentWaypoint].position);
                    m_waiting = false;
                }
            }
            else
            {
                Transform wp = m_waypoints[m_currentWaypoint];

                if (Vector3.Distance(m_agent.nextPosition, wp.position) <= 1f)
                {
                    m_agent.isStopped = true;
                    m_waitCounter = 0f;
                    m_waiting = true;
                    m_currentWaypoint = (m_currentWaypoint + 1) % m_waypoints.Length;
                }
            }
        }
        m_state = NodeState.RUNNING;        
        return m_state;
    }
}
