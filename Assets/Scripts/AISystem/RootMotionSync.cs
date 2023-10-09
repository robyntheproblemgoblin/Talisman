using UnityEngine;

namespace AISystem
{
    [RequireComponent(typeof(Animator))]
    public class RootMotionSync : MonoBehaviour
    {
        [SerializeField] Transform m_transform;
        [SerializeField] bool m_applyRotationWarp;
        Animator m_animator;

        float m_angle;
        float m_lastUpdated;
        bool m_warpEnabled = true;

        void Awake()
        {
            m_animator = GetComponent<Animator>();
            m_lastUpdated = Time.time;
        }

        public void SetTurnWarp(float angle)
        {
            m_angle = angle;
        }

        public void SetWarp(bool warpOn)
        {
            m_warpEnabled = warpOn;
        }

        void OnAnimatorMove()
        {
            Vector3 deltaMove = m_animator.deltaPosition;
            deltaMove.y = 0f;

            Quaternion deltaRotation = m_animator.deltaRotation;

            if (m_applyRotationWarp && m_warpEnabled)
            {
                float turn = deltaRotation.eulerAngles.y;
                deltaRotation *= Quaternion.Euler(0f, m_angle * .05f, 0f);
            }

            m_transform.position += deltaMove;
            m_transform.rotation *= deltaRotation;
            m_lastUpdated = Time.time;
        }
    }
}