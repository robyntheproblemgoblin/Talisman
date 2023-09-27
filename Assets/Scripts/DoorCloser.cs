using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCloser : MonoBehaviour
{
    public Door m_door;
    public DeathGuard m_safetyNet;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerController>() != null)
        {
            m_door.m_unlocked = !m_door.m_unlocked;
            if(m_safetyNet != null)
            {
                m_safetyNet.gameObject.SetActive(true);
            }
            gameObject.SetActive(false);
        }
    }

    public void ResetDoor()
    {
        m_door.OpenDoor();
        gameObject.SetActive(true);
        m_safetyNet.gameObject.SetActive(false);
    }
}
