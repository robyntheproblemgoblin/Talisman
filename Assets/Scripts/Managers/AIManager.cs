using UnityEngine;
using System.Collections.Generic;

public class AIManager : MonoBehaviour
{
    public Dictionary<EnemyBT, KeyValuePair<Vector3, Quaternion>> m_enemies;

    private void Awake()
    {
        m_enemies = new Dictionary<EnemyBT, KeyValuePair<Vector3, Quaternion>>();
    }

    public void ResetEnemies()
    {
        Debug.Log(m_enemies.Count);
        foreach(KeyValuePair<EnemyBT, KeyValuePair<Vector3, Quaternion>> enemy in m_enemies)
        {
            enemy.Key.ResetToPosition(enemy.Value.Key, enemy.Value.Value);            
        }
    }
}
