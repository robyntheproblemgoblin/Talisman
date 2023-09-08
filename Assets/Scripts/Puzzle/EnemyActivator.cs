using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivator : Puzzle
{
    public EnemyBT m_enemy;

    void Update()
    {
        if (m_updateMana && m_enemy.m_isStatue)
        {
            m_updateMana = false;
            m_enemy.m_isStatue = false;
        }
        else
        {
            m_updateMana = false;
        }
    }

    public override void RotatePuzzle() { }
}
