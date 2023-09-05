using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

public class MeleeBT : EnemyBT
{
    public List<AudioClip> m_mainStory;
    public List<AudioClip> m_otherStory;
    int m_mainIndex = 0;
    int m_rotationIndex = 0;
    public AudioSource m_source;

    public void PlayNextMainAudio()
    {
        m_source.clip = m_mainStory[m_mainIndex];
        m_source.Play();
        m_mainIndex++;
    }

    public void PlayRotationRoomClip()
    {
        m_source.clip = m_otherStory[m_rotationIndex];
        m_source.Play();
        m_rotationIndex++;
    }



















    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new StatueMode(transform, this),
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
    });

        return root;
    }
}


