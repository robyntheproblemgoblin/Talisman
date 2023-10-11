using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using AISystem.Data;
using AISystem.Contracts;
using AISystem.Pathing;

namespace AISystem.Systems
{
    public class AIMovement
    {
        const float m_arrivalThreshold = 1f;
        MovementSettings m_settings;
        IManager m_aiManager;
        IBeing m_attachedBeing;
        RootMotionSync m_rootMotionSync;
        Animator m_animator;

        public Path m_currentPath;
        MovePace m_pace;

        bool m_movementEnabled;
        float m_speed = 0;

        float m_targetSpeed => m_pace switch
        {
            MovePace.RUN => m_settings.m_run,
            MovePace.WALK => m_settings.m_walk,
            _ => 1f
        };

        public bool m_atDestination { get; private set; } = true;

        public bool m_isStatue = true;

        public bool m_isInterrupted = false;
        public CapsuleCollider m_swordCollider;

        public enum MovePace
        {
            WALK,
            RUN,
        }

        public AIMovement(MovementSettings settings, [CanBeNull] Animator animator, IBeing attachedBeing, IManager manager, RootMotionSync rootMotionSync, CapsuleCollider swordCollider)
        {
            m_settings = settings;
            m_animator = animator;
            m_attachedBeing = attachedBeing;
            m_aiManager = manager;
            m_rootMotionSync = rootMotionSync;
            m_swordCollider = swordCollider;
        }

        public void EnableMovement()
        {
            m_movementEnabled = true;
            MovementLoop().Forget();
        }

        public void DisableMovement() => m_movementEnabled = false;

        public void SetMovePace(MovePace movePace) => m_pace = movePace;

        public void Stop()
        {
            m_currentPath = Path.Empty;
            m_atDestination = true;
        }

        public void SetWarp(bool rootMotionOn) => m_rootMotionSync.SetWarp(rootMotionOn);

        public void SetDestination(Vector3 dest)
        {
            if (Vector3.SqrMagnitude(m_attachedBeing.m_position - dest) <= m_arrivalThreshold * m_arrivalThreshold)
            {
                m_atDestination = true;
                return;
            }

            Path path = m_aiManager.GeneratePath(new PathRequest()
            {
                m_destination = dest,
                m_destinationDirection = Vector3.Normalize(dest - m_attachedBeing.m_position),
                m_origin = m_attachedBeing.m_position,
                m_originDirection = m_attachedBeing.m_forward,
            });

            if (path.m_isEmpty)
            {
                return;
            }

            m_currentPath = path;
            m_atDestination = false;
        }

        async UniTask MovementLoop()
        {
            while (m_movementEnabled)
            {
                if (m_currentPath.m_isEmpty == false && m_animator != null)
                {
                    float sqrmag = Vector3.SqrMagnitude(m_attachedBeing.m_position - (Vector3)m_currentPath.m_destination);
                    m_atDestination = sqrmag <= m_arrivalThreshold * m_arrivalThreshold;

                    if (!m_atDestination)
                    {
                        UpdateForwardBackwards();
                        UpdateSideWays();
                    }
                    else
                    {
                        m_speed = 0;
                        m_animator.SetFloat("ForwardsBackwards", 0); 
                        m_rootMotionSync.SetTurnWarp(0);
                        m_animator.SetFloat("Sideways", 0);
                    }
                }                

                await UniTask.Yield();
            }
        }

        void UpdateSideWays()
        {
            m_currentPath.GetRelativePoint(m_attachedBeing.m_position, 0.1f, out float3 predictPos, out float3 predictTan);

            //DEBUG
            /*Debug.DrawRay(m_attachedBeing.m_position, predictTan, Color.green);
            Debug.DrawRay(m_attachedBeing.m_position, m_attachedBeing.m_forward, Color.red);
            Debug.DrawRay(m_aiManager.GetCloseBeings(15, m_attachedBeing.m_position)[0].m_position + new Vector3(0, -1.5f, 0), -predictTan, Color.blue);
            Debug.DrawRay(m_aiManager.GetCloseBeings(15, m_attachedBeing.m_position)[0].m_position + new Vector3(0, -1.5f, 0), m_aiManager.GetCloseBeings(15, m_attachedBeing.m_position)[0].m_forward, Color.magenta);*/

            float angle = Vector3.SignedAngle(m_attachedBeing.m_forward, predictTan, Vector3.up);

            if (Vector3.Angle(m_attachedBeing.m_forward, predictTan) <= 1)
            {
                m_rootMotionSync.SetTurnWarp(0);
                m_animator.SetFloat("Sideways", 0);
            }
            else
            {

                m_rootMotionSync.SetTurnWarp(angle);
                m_animator.SetFloat("Sideways", angle * Mathf.Deg2Rad);
            }
        }

        void UpdateForwardBackwards()
        {
            float distToDest = Vector3.Distance(m_currentPath.m_destination, m_attachedBeing.m_position);
            
            if (distToDest >= m_arrivalThreshold * m_arrivalThreshold)
            {
                m_speed = m_targetSpeed;
                m_animator.SetFloat("ForwardsBackwards", m_pace == MovePace.WALK ? 2 : 4);
            }
            else
            {
                m_speed = 0;
                m_animator.SetFloat("ForwardsBackwards", 0);
            }
        }

        public void AwakenStatue()
        {
            m_animator.enabled = true;
        }

    }
}