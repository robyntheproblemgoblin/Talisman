using BehaviourTree;
using UnityEngine;

public class CheckEnemyInFOVRange : Node
{
    Transform m_transform;

    private static int m_enemyLayerMask;

    public CheckEnemyInFOVRange(Transform transform)
    {
        m_transform = transform;
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {

        }

        m_state = NodeState.SUCCESS;
        return m_state;
    }
}
