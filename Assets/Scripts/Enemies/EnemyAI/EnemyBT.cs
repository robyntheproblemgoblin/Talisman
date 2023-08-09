using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class EnemyBT : MonoBehaviour
{
    Node m_root = null;

    public Transform[] m_waypoints;

    public static float m_speed = 2.5f;

    public static float m_fovRange = 10f;

    public static float m_attackRange = 4f;

    [SerializeField]
    protected int m_startingHP = 30;
    [HideInInspector]
    public int m_currentHP;

    Animator m_animator;
    protected NavMeshAgent m_agent;
    NavMeshTriangulation m_triangulation;

    [SerializeField]
    LookAt m_lookAt;

    Vector2 m_velocity;
    Vector2 m_smoothDeltaPosition;
    void Start()
    {
        m_currentHP = m_startingHP;
        m_animator = transform.gameObject.GetComponent<Animator>();
        m_animator.applyRootMotion = true;
        m_agent = transform.gameObject.GetComponent<NavMeshAgent>();
        if(m_waypoints.Length > 0  )
        {
            m_agent.SetDestination(m_waypoints[0].position);
        }
        m_agent.updatePosition = false;
        m_agent.updateRotation = true;
        m_root = SetupTree();        
    }

    protected virtual Node SetupTree()
    {        
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckTargetInAttackRange(transform),
                new TaskGoToTarget(transform),
            }),
            new Sequence(new List<Node>
            {
                new CheckTargetInFOVRange(transform),
                new TaskGoToTarget(transform),
            }),
            new TaskPatrol(transform, m_waypoints, m_agent),
        });
        return root;
    }    

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = m_agent.nextPosition;
        transform.position = rootPosition;
        m_agent.nextPosition = rootPosition;
    }

    public bool TakeHit()
    {
        m_currentHP -= 10;
        bool isDead = m_currentHP <= 0;
        if (isDead) Die();
        return isDead;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (m_root != null)
        {
            m_root.Evaluate();
        }
        SyncAnimation();
    }

    void SyncAnimation()
    {
        Vector3 worldDeltaPosition = m_agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;
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

        bool shouldMove = m_velocity.magnitude > 0.5f && m_agent.remainingDistance > m_agent.stoppingDistance;

        
        m_animator.SetBool("Idle", !shouldMove);
        m_animator.SetFloat("MovementSpeed", m_velocity.magnitude);

        m_lookAt.lookAtTargetPosition = m_agent.steeringTarget + transform.forward;

        //float deltaMagnitude = worldDeltaPosition.magnitude;
        //if (deltaMagnitude > Agent.radius / 2)
        //{
        //    transform.position = Vector3.Lerp(Animator.rootPosition, Agent.nextPosition, smooth);
        //}

    }
}
