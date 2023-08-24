using UnityEngine;

public class HealingState : RangedAttackState
{   
    PlayerController m_player;
    public HealingState(Animator anim, ParticleSystem ps, PlayerController player)
    {
        m_animator = anim;
        m_particles = ps; 
        m_player = player;
    }
    public override void StartState(float startValue)
    {       
        m_animator.SetBool("LeftAttacking", true);
        m_particles.Play();
    }

    public override void Update()
    {
        m_player.m_health += m_player.m_healRate * Time.deltaTime;
    }
}