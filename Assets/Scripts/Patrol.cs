using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Transform[] m_waypoints;
    int m_currentWaypoint;
    float m_speed = 2.0f;
    float m_waitTime = 1f;
    float m_waitCounter = 0f;
    bool m_waiting = false;

    private void Update()
    {
        if (m_waiting)
        {
            m_waitCounter += Time.deltaTime;
            if (m_waitCounter < m_waitTime)
                return;
        }

        Transform wp = m_waypoints[m_currentWaypoint];
        if (Vector3.Distance(transform.position, wp.position) < 0.01f)
        {
            transform.position = wp.position;
            m_waitCounter = 0f;
            m_waiting = true;
            m_currentWaypoint = (m_currentWaypoint + 1) % m_waypoints.Length;
        }
        else
        {
            Vector3 relativePos = wp.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, m_speed * Time.deltaTime);

            transform.position = Vector3.MoveTowards(transform.position, wp.position, m_speed * Time.deltaTime);
        }
    }
}
