using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaCircle : MonoBehaviour
{
    public float m_windUpTime;
    public float m_activeTime;
    float m_timeActived;
    ParticleSystem[] m_particles;
    CapsuleCollider m_collider;

    // Start is called before the first frame update
    void Start()
    {
        m_particles = GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem particle in m_particles)
        {
            var main = particle.main;
            main.duration = m_windUpTime+m_activeTime;
        }        
        StartCoroutine()
    }

}
