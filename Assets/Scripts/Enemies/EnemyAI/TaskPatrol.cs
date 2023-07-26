using UnityEngine;
using BehaviourTree;

public class TaskPatrol : Node
{
    Transform m_transform;
    Transform[] m_waypoints;

    //Animator m_animator;

    int m_currentWaypoint;
    float m_waitTime = 1f;
    float m_waitCounter = 0f;
    bool m_waiting = false;    

    public TaskPatrol(Transform transform, Transform[] waypoints)
    {
        m_transform = transform;
        m_waypoints = waypoints;
        //m_animator = transform.GetComponenet<Animator>();
    }

    public override NodeState Evaluate()
    {
        if (m_waiting)
        {
            m_waitCounter += Time.deltaTime;
            if (m_waitCounter >= m_waitTime)
            {
                m_waiting = false;
                //m_animator.SetBool("Walking", true);
            }
        }
        else
        {
            Transform wp = m_waypoints[m_currentWaypoint];

            if (Vector3.Distance(m_transform.position, wp.position) < 0.01f)
            {
                m_transform.position = wp.position;
                m_waitCounter = 0f;
                m_waiting = true;
                m_currentWaypoint = (m_currentWaypoint + 1) % m_waypoints.Length;
                //m_animator.SetBool("Walking", false);
            }
            else
            {
                Vector3 relativePos = wp.position - m_transform.position;
                Quaternion toRotation = Quaternion.LookRotation(relativePos);
                m_transform.rotation = Quaternion.Lerp(m_transform.rotation, toRotation, EnemyBT.m_speed * Time.deltaTime);

                m_transform.position = Vector3.MoveTowards(m_transform.position, wp.position, EnemyBT.m_speed * Time.deltaTime);
            }
        }

        m_state = NodeState.RUNNING;
        return m_state;
    }
}
