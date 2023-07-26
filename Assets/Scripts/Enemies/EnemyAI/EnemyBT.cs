using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class EnemyBT : BehaviourTree.BehaviourTree
{
    public Transform[] m_waypoints;

    public static float m_speed = 2;

    public static float m_fovRange = 6f;

    public static float m_attackRange = 1.75f;

    protected override Node SetupTree()
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
            new TaskPatrol(transform, m_waypoints),
        });

        return root;
    }
}
