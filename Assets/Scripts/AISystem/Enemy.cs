using System.Collections.Generic;
using UnityEngine;
using AISystem.BehaviourTrees;
using AISystem.Data;
using AISystem.Sensors;
using AISystem.Systems;
using AISystem.Contracts;
using Cysharp.Threading.Tasks;
using System;

namespace AISystem
{
    public class Enemy : MonoBehaviour, IBeing
    {
        #region AI Fields
        public Vector3 m_position => transform.position;
        public Vector3 m_forward => transform.forward;
        public Vector3 m_headPosition => m_head.position;

        [field: SerializeField] public EnemySettings AISettings { get; private set; } = new();
        [SerializeField] Animator m_animator;
        [SerializeField] RootMotionSync m_rootMotionSync;
        [SerializeField] Transform m_head;

        Intelligience m_intelligience;
        AIManager m_aiManager;
        #endregion

        #region  Game Fields
        [SerializeField] float m_startingHP;
        public float m_currentHP;        
        public EnemyActivator m_activator;
        public float m_damage;
        public CapsuleCollider m_swordCollider;
        PlayerController m_playerController;
        int m_playerMask;
        Vector3 m_startPosition;
        Quaternion m_startRotation;

        SkinnedMeshRenderer m_mesh;

        public FMODUnity.EventReference m_stoneAwake;
        public FMODUnity.EventReference m_armourAwake;
        public List<FMODUnity.EventReference> m_gruntsAwake;
        public FMODUnity.EventReference m_takeDamageSound;
        public FMODUnity.EventReference m_deathSound;
        public FMODUnity.EventReference m_playerHitEnemySound;

        #endregion

        void Start()
        {
            m_aiManager = GameManager.Instance.m_aiManager;
            m_aiManager.RegisterBeing(this);

            m_animator ??= GetComponentInChildren<Animator>();
            m_mesh = GetComponentInChildren<SkinnedMeshRenderer>();
            m_mesh.materials[0].SetFloat("_EmissiveFreq", 0);
            m_mesh.materials[1].SetFloat("_ArmorFade", 0);

            List<IOptic> optics = CreateOptics();
            AIKnowledge aIKnowledge = new AIKnowledge();
            AIMovement aIMovement = new AIMovement(AISettings.MovementSettings, m_animator, this, m_aiManager, m_rootMotionSync, m_swordCollider, m_mesh);
            BehaviourManager behaviourManager = new BehaviourManager(UnpackBehaviourTree(AISettings.BehaviourTree, new BehaviourInput()
            {
                m_aIKnowledge = aIKnowledge,
                m_aIMovement = aIMovement,
                m_go = gameObject,
            }));

            m_intelligience = new Intelligience(optics, aIKnowledge, aIMovement, behaviourManager);

            m_currentHP = m_startingHP;

            m_playerController = FindObjectOfType<PlayerController>();
            m_playerMask = (int)Mathf.Log(LayerMask.GetMask("Sword"), 2);

            m_startPosition = transform.position;
            m_startRotation = transform.rotation;
           
            m_animator.enabled = false;

            int grunt = UnityEngine.Random.Range(0, m_gruntsAwake.Count - 1);

            m_intelligience.SetStatue(true, m_stoneAwake, m_armourAwake, m_gruntsAwake[grunt]);
            m_swordCollider.enabled = false;
        }

        void OnDestroy()
        {
            m_intelligience?.DisableIntelligience();
            m_aiManager?.DeregisterBeing(this);
        }

        List<IOptic> CreateOptics()
        {
            List<IOptic> createdOptics = new List<IOptic>();
            createdOptics.Add(new OpticSensor(AISettings.ObservationSettings, m_aiManager, this));

            return createdOptics;
        }

        BehaviourTree UnpackBehaviourTree(BTAsset asset, BehaviourInput input)
        {
            BTAsset BTInstance = ScriptableObject.Instantiate(asset);
            var bt = BTInstance.m_behaviourTree;
            bt.SetBehaviourInput(input);
            return bt;
        }

        public void ResetToPosition()
        {
            m_swordCollider.enabled = false;
            transform.SetPositionAndRotation(m_startPosition, m_startRotation);
            m_animator.rootPosition = m_startPosition;            
            m_animator.Rebind();
            m_animator.Update(0);
            m_animator.enabled = false;
            m_mesh.materials[0].SetFloat("_EmissiveFreq", 0);
            m_mesh.materials[1].SetFloat("_ArmorFade", 0);
            m_intelligience.SetStatue(true, m_stoneAwake, m_armourAwake, m_gruntsAwake[0]);
        }

        public bool IsStatue()
        {
            return m_intelligience.IsStatue();
        }

        public void SetStatue(bool state)
        {
            int grunt = UnityEngine.Random.Range(0, m_gruntsAwake.Count - 1);
            m_intelligience.SetStatue(state, m_stoneAwake, m_armourAwake, m_gruntsAwake[grunt]);
        }

        public void Interrupt()
        {
            m_intelligience.Interrupt();
        }

        public bool TakeHit(float damage, Vector2 angle)
        {
            m_playerController.HitReticle();
            m_swordCollider.enabled = false;
            m_currentHP -= damage;
            m_mesh.materials[1].SetFloat("_ArmorFade", m_currentHP/m_startingHP);
            GameManager.Instance.m_audioManager.PlayOneShot(m_takeDamageSound, gameObject.transform.position);
            bool isDead = m_currentHP <= 0;
            if (isDead)
            {
                Die().Forget();
            }
            else
            {
                m_intelligience.IsHit(angle);
            }
            return isDead;
        }

        protected async UniTask Die()
        {
            //Setup what is happening on die
            m_swordCollider.enabled = false;
            m_activator.EnemyDead();
            m_animator.SetTrigger("Die");
            m_rootMotionSync.SetDead();
            gameObject.GetComponent<Collider>().enabled = false;
            GameManager.Instance.m_audioManager.PlayOneShot(m_deathSound, gameObject.transform.position);
            float time = Time.time;
            while (Time.time < time + 3)
            {
                await UniTask.Yield();
            }
            Destroy(gameObject);
        }

        protected void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.layer == m_playerMask && !m_intelligience.IsStatue())
            {                
                GameManager.Instance.m_audioManager.PlayOneShot(m_playerHitEnemySound, collision.gameObject.transform.position);
                Vector3 direction = -collision.GetContact(0).normal;                
                Vector2 angle = new Vector2(direction.x, direction.z);
                TakeHit(m_playerController.m_meleeDamage, angle);
            }
        }
    }

}