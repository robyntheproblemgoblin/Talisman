using UnityEngine;

public class FiringState : LeftHandState
{
    Camera m_camera;
    float m_damage;
    public bool m_isProjectile = true;
    float m_manaBoltSpeed = 30.0f;
    float m_minSize = 1;
    Transform m_projectileSpawn;


    public FiringState(Animator anim, ParticleSystem ps, Transform t)
    {
        m_animator = anim;
        m_particles = ps;
        m_camera = Camera.main;
        m_projectileSpawn = t;
    }
    public override void StartState(float damage)
    {
        if (m_isProjectile)
            m_particles.Play();
        else
            m_particles.Stop();
        m_damage = damage;
    }

    public override void Update()
    {
        if (m_isProjectile && m_damage != 0)
        {
            Ray camRay = m_camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            Vector3 destination;
            if (Physics.Raycast(camRay, out hit))
            {
                destination = hit.point;
            }
            else
            {
                destination = camRay.GetPoint(50);
            }
            var manaBolt = MonoBehaviour.Instantiate(MonoBehaviour.FindObjectOfType<PlayerController>().projectile, m_projectileSpawn.position + m_projectileSpawn.forward, Quaternion.identity) as GameObject;
            Projectile p = manaBolt.GetComponent<Projectile>();
            p.m_damage = m_damage;
            if (m_damage > m_minSize)
            {
                var main = manaBolt.GetComponentInChildren<ParticleSystem>().main;
                main.startSize = m_damage;
            }
            manaBolt.GetComponent<Rigidbody>().velocity = (destination - m_projectileSpawn.position).normalized * m_manaBoltSpeed;
        }

        MonoBehaviour.FindObjectOfType<PlayerController>().StartIdle();
    }
    public override void StopState()
    {
        m_animator.SetBool("LeftAttacking", false);
    }
}