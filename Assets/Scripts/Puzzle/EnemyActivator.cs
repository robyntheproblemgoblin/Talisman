using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivator : Puzzle
{
    public EnemyBT m_enemy;
    public Puzzle m_puzzle;
    public TutorialTrigger m_tutorial;

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

    public void EnemyDead()
    {
        m_unlocked = true;
        if (m_puzzle != null)
        {
            m_puzzle.m_updateMana = true;
        }
        foreach (Door door in m_doors)
        {
            door.CheckState();
        }
        
        if(m_tutorial != null)
        {
            m_tutorial.SecondTutorial();
        }
    }
}
