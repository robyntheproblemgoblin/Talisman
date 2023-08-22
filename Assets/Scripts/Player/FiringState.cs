using UnityEngine;

public class FiringState : RangedAttackState
{
    Camera m_camera;
    float m_damageTime;
    public bool m_beam = true;
    float m_manaBoltSpeed = 30.0f;
    float m_maxSize = 2;
    float m_minSize = 1;


    public FiringState(Animator anim, ParticleSystem ps, float max)
    {
        m_animator = anim;
        m_particles = ps;
        m_camera = Camera.main;
        m_maxSize = max;
    }
    public override void StartState(float startValue)
    {
        if (m_beam)
            m_particles.Play();
        else
            m_particles.Stop();
        m_damageTime = startValue;
    }

    public override void Update()
    {
        if (m_beam)
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
            var manaBolt = MonoBehaviour.Instantiate(MonoBehaviour.FindObjectOfType<PlayerController>().projectile, m_particles.transform.position + m_particles.transform.forward, Quaternion.identity) as GameObject;
            if (m_damageTime > m_minSize)
            {
                manaBolt.GetComponent<SphereCollider>().radius *= (m_damageTime <= m_maxSize ? m_damageTime : m_maxSize);
                var main = manaBolt.GetComponentInChildren<ParticleSystem>().main;
                main.startSize = m_damageTime;
            }
            manaBolt.GetComponent<Rigidbody>().velocity = (destination - m_particles.transform.position).normalized * m_manaBoltSpeed;
        }
        MonoBehaviour.FindObjectOfType<PlayerController>().StartIdle();
    }
    public override void StopState()
    {
        m_animator.SetBool("LeftAttacking", false);
    }
}