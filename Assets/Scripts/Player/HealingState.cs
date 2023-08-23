using UnityEngine;

public class HealingState : RangedAttackState
{        
    
    public HealingState(Animator anim, ParticleSystem ps)
    {
        m_animator = anim;
        m_particles = ps; 
    }
    public override void StartState(float startValue)
    {       
        m_animator.SetBool("LeftAttacking", true);
        m_particles.Play();
    }

    public override void Update()
    {
        
    }
}