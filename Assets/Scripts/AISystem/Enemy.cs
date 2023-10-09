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
        public bool m_isStatue;
        public EnemyActivator m_activator;
        public float m_damage;
        #endregion

        void Start()
        {
            m_aiManager = GameManager.Instance.m_aiManager;
            m_aiManager.RegisterBeing(this);

            m_animator ??= GetComponentInChildren<Animator>();

            List<IOptic> optics = CreateOptics();
            AIKnowledge aIKnowledge = new AIKnowledge();
            AIMovement aIMovement = new AIMovement(AISettings.MovementSettings, m_animator, this, m_aiManager, m_rootMotionSync);
            BehaviourManager behaviourManager = new BehaviourManager(UnpackBehaviourTree(AISettings.BehaviourTree, new BehaviourInput()
            {
                m_aIKnowledge = aIKnowledge,
                m_aIMovement = aIMovement,
                m_go = gameObject,
            }));

            m_intelligience = new Intelligience(optics, aIKnowledge, aIMovement, behaviourManager);
            m_intelligience.EnableIntelligience();
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

        public void ResetToPosition(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
            m_animator.rootPosition = position;            
            m_animator.Rebind();
            m_animator.Update(-1);
            m_animator.enabled = false;
            m_isStatue = true;
        }

        public bool TakeHit(float damage)
        {
            m_currentHP -= damage;
            bool isDead = m_currentHP <= 0;
            if (isDead)
            {
                Die().Forget();
            }
            return isDead;
        }

        protected async UniTask Die()
        {
            //Setup what is happening on die
            m_activator.EnemyDead();
            m_animator.SetTrigger("Die");
            GameManager.Instance.m_aiManager.m_enemies.Remove(this);
            gameObject.GetComponent<Collider>().enabled = false;
            float time = Time.time;
            while (Time.time < time + 3)
            {
                await UniTask.Yield();
            }
            Destroy(gameObject);
        }


    }

}