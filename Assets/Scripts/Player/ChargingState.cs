using UnityEngine;

public class ChargingState : RangedAttackState
{
    public bool m_beam = true;
    public float chargeTime = 0.0f;
    Camera m_camera;
    public ChargingState(Animator anim, ParticleSystem ps)
    {
        m_animator = anim;
        m_particles = ps;
        m_camera = Camera.main;
    }
    public override void StartState(float startValue)
    {
        chargeTime = 0.0f;
        m_animator.SetBool("LeftAttacking", true);
        m_particles.Play();
    }

    public override void Update()
    {
        if (m_beam)
        {
            chargeTime += Time.deltaTime;
        }
        else
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

        }
    }
}