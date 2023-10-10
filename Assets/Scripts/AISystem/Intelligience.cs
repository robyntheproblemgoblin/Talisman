using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using AISystem.Data;
using AISystem.Sensors;
using AISystem.Systems;

namespace AISystem
{
    public class Intelligience
    {
        List<IOptic> m_optics;
        AIKnowledge m_aiKnowledge;
        AIMovement m_aiMovement;
        BehaviourManager m_behaviourPerformer;

        bool m_isActive;

        public Intelligience(List<IOptic> optics, AIKnowledge aiKnowledge, AIMovement aiMovement, BehaviourManager behaviourPerformer)
        {
            m_optics = optics;
            m_aiKnowledge = aiKnowledge;
            m_aiMovement = aiMovement;
            m_behaviourPerformer = behaviourPerformer;
        }

        public bool IsStatue()
        {
            return m_aiMovement.m_isStatue;
        }

        public void SetStatue(bool state)
        {
            m_aiMovement.m_isStatue = state;
            if(state)
            {
                DisableIntelligience();
            }
            else
            {
                m_aiMovement.AwakenStatue();
                EnableIntelligience();
            }
        }

        public void EnableIntelligience()
        {
            m_isActive = true;

            foreach (IOptic optic in m_optics)
            {
                optic.m_observations += OnOpticReceived;
                optic.StartOptics();
            }

            m_aiMovement.EnableMovement();
            BehaviourUpdate().Forget();
        }

        public void DisableIntelligience()
        {
            foreach (IOptic optic in m_optics)
            {
                optic.m_observations -= OnOpticReceived;
                optic.StopOptics();
            }

            m_aiMovement.DisableMovement();
            m_isActive = false;
        }

        async UniTask BehaviourUpdate()
        {
            while (m_isActive)
            {
                m_behaviourPerformer.Update(Time.deltaTime);
                await UniTask.Yield();
            }
        }

        void OnOpticReceived(Observation observation)
        {
            m_aiKnowledge.ProcessObservations(observation);
        }
    }
}