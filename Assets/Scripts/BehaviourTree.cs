using UnityEngine;

public class BehaviourTree : MonoBehaviour
{
    public Transform[] m_waypoints;
    int m_currentWaypoint;
    float m_speed = 2.0f;

    private void Update()
    {
        Transform wp = m_waypoints[m_currentWaypoint];
        if(Vector3.Distance(transform.position, wp.position) < 0.01f)
        {
            m_currentWaypoint = (m_currentWaypoint + 1) % m_waypoints.Length;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, wp.position, m_speed * Time.deltaTime);
        }
    }
}