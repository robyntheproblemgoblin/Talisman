using UnityEngine;

public class ChargingState : LeftHandState
{
    public bool m_isProjectile = true;
    public float m_chargeTime = 0.0f;
    PlayerController m_playerController;
    float m_projectileManaCost;
    float m_minProjectileManaCost;
    float m_flameCost;
    float m_startMana;
    public ChargingState(Animator anim, ParticleSystem ps, PlayerController pc)
    {
        m_animator = anim;
        m_particles = ps;
        m_playerController = pc;
        m_minProjectileManaCost = pc.m_minProjectileCost;
        m_projectileManaCost = pc.m_projectileManaCost;
        m_flameCost = pc.m_flameCost;
    }
    public override void StartState(float startValue)
    {
        m_chargeTime = 0.0f;
        m_animator.SetBool("LeftAttacking", true);
        m_particles.Play();
        m_startMana = m_playerController.m_currentMana;
    }

    public override void Update()
    {

        if (m_isProjectile)
        {
            if (m_startMana >= m_minProjectileManaCost)
            {
                float cost = m_projectileManaCost * Time.deltaTime;
                if (m_playerController.SpendMana(cost))
                {
                    m_chargeTime += cost;
                }
            }
        }
        else
        {
            if (m_playerController.m_currentMana <= 0)
            {
                m_particles.Stop();
            }
            float cost = m_flameCost * Time.deltaTime;
            m_playerController.SpendMana(cost);
        }
    }


    public override void StopState()
    {
        if (m_chargeTime < m_minProjectileManaCost)
        {
            if (m_playerController.SpendMana(m_minProjectileManaCost - m_chargeTime))
            {
                m_chargeTime = m_minProjectileManaCost;
            }
            else
            {
                m_chargeTime = 0;
            }
        }
    }
}