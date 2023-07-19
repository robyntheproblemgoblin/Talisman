using UnityEngine;
using BehaviourTree;

public class EnemyBT : BehaviourTree.Tree
{
    public Transform[] m_waypoints;

    public static float m_speed = 2;

    public static float fovRange = 6f;

    protected override Node SetupTree() 
    {
        Node root = new TaskPatrol(transform, m_waypoints);
        return root;
    }
}
