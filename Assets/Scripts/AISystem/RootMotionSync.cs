using AISystem.Systems;
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
        Vector3 m_yLevel;
        bool m_warpEnabled = true;
        bool m_isDead = false;

        public AIMovement m_movement;

        void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        public void SetTurnWarp(float angle)
        {
            m_angle = angle;
        }

        public void SetWarp(bool warpOn)
        {
            m_warpEnabled = warpOn;
        }

        public void SetDead()
        {
            m_isDead = true;
        }

        public void SetYPos(float y)
        {
            Vector3 pos = transform.position;
            pos.y = y;
            m_transform.position = pos;
        }

        bool isFirst = true;
        Quaternion debug;
        Quaternion zero = new Quaternion(0,0,0,1);  

        void OnAnimatorMove()
        {
            if (!m_isDead)
            {
                Vector3 deltaMove = m_animator.deltaPosition;
                deltaMove.y = 0f;

                Quaternion deltaRotation = m_animator.deltaRotation;

                if (m_applyRotationWarp && m_warpEnabled)
                {
                    deltaRotation *= Quaternion.Euler(0f, m_angle * 0.05f, 0f);
                }                

                
                if(debug == Quaternion.Inverse(deltaRotation) && debug != zero)
                {
                    Debug.Log("Issue is here");                     
                }
                    debug = deltaRotation;

                m_transform.position += deltaMove;
                m_transform.rotation *= deltaRotation;
            }
        }
    }
}