using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class EnemyBT : MonoBehaviour
{
    protected Node m_root = null;

    public Transform[] m_waypoints;

    public float m_speed = 2.5f;

    public float m_fovRange = 90f;

    public float m_attackRange = 10f;

    protected int m_playerFlameLayer;
    protected int m_playerSwordLayer;
    protected PlayerController m_playerController;

    public float m_startingHP = 30;
    public float m_currentHP;

    protected Animator m_animator;
    protected NavMeshAgent m_agent;

    [SerializeField]
    protected LookAt m_lookAt;

    protected Vector2 m_velocity;
    protected Vector2 m_smoothDeltaPosition;

    public float m_damage = 5;
    public bool m_isStatue = true;

    public EnemyActivator m_activator;

    public void Start()
    {
        m_currentHP = m_startingHP;
        m_animator = transform.gameObject.GetComponent<Animator>();
        m_animator.applyRootMotion = true;
        m_animator.enabled = false;
        m_agent = transform.gameObject.GetComponent<NavMeshAgent>();
        if (m_waypoints.Length > 0)
        {
            m_agent.SetDestination(m_waypoints[0].position);
        }
        m_agent.updatePosition = false;
        m_agent.updateRotation = true;
        m_root = SetupTree();
        m_playerFlameLayer = (int)Mathf.Log(LayerMask.GetMask("Flame"), 2);
        m_playerSwordLayer = (int)Mathf.Log(LayerMask.GetMask("Sword"), 2);
        m_playerController = FindObjectOfType<PlayerController>();
    }

    protected virtual Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new StatueMode(transform, this),
        });
        return root;
    }

    protected void OnAnimatorMove()
    {
        Vector3 rootPosition = m_animator.rootPosition;
        rootPosition.y = m_agent.nextPosition.y;
        transform.position = rootPosition;
        m_agent.nextPosition = rootPosition;
    }

    public void StopStatue()
    {
        m_animator.enabled = true;
        m_isStatue = false;
    }

    public bool TakeHit(float damage)
    {
        m_currentHP -= damage;
        bool isDead = m_currentHP <= 0;
        if (isDead) Die();
        return isDead;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == m_playerSwordLayer)
        {
            TakeHit(m_playerController.m_meleeDamage);
        }
    }

    protected void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.layer == m_playerFlameLayer)
        {
            TakeHit(m_playerController.m_flameDamage);
        }
        else
        {
            Debug.Log(m_playerFlameLayer);
        }
    }

    protected void Die()
    {
        //Setup what is happening on die
        m_activator.EnemyDead();
        Destroy(gameObject);
    }

    protected virtual void Update()
    {
        if (m_root != null)
        {
            m_root.Evaluate();
        }
        SyncAnimation();
    }

    protected void SyncAnimation()
    {
        Vector3 worldDeltaPosition = m_agent.nextPosition - transform.position;
        //worldDeltaPosition.y = 0;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        m_smoothDeltaPosition = Vector2.Lerp(m_smoothDeltaPosition, deltaPosition, smooth);

        m_velocity = m_smoothDeltaPosition / Time.deltaTime;
        if (m_agent.remainingDistance <= m_agent.stoppingDistance)
        {
            m_velocity = Vector2.Lerp(Vector2.zero, m_velocity, m_agent.remainingDistance);
        }
        
        m_animator.SetFloat("MovementSpeed", m_velocity.magnitude);

        m_lookAt.lookAtTargetPosition = m_agent.steeringTarget + transform.forward;

        float deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > m_agent.radius/* / 2*/)
        {
            transform.position = Vector3.Lerp(m_animator.rootPosition, m_agent.nextPosition, smooth);
        }
    }
}
