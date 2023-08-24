using UnityEngine;

public class ChargingState : LeftHandState
{
    public bool m_isProjectile = true;
    public float m_chargeTime = 0.0f;
    
    public ChargingState(Animator anim, ParticleSystem ps)
    {
        m_animator = anim;
        m_particles = ps;        
    }
    public override void StartState(float startValue)
    {
        m_chargeTime = 0.0f;
        m_animator.SetBool("LeftAttacking", true);
        m_particles.Play();
    }

    public override void Update()
    {
        if (m_isProjectile)
        {
            m_chargeTime += Time.deltaTime;
        }        
    }
}