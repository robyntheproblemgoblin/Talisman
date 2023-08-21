using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaCircle : MonoBehaviour
{
    public float m_windUpTime;
    public float m_activeTime;
    
    ParticleSystem[] m_particles;
    CapsuleCollider m_collider;

    void Start()
    {
        m_particles = GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem particle in m_particles)
        {
            var main = particle.main;
            main.duration = m_windUpTime+m_activeTime;  
        }
        StartCoroutine(ActivateDamage());
    }

    IEnumerator ActivateDamage()
    {
        yield return new WaitForSeconds(m_windUpTime);
        m_collider.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        
    }
}
