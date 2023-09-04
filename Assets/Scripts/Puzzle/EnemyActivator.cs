using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivator : Puzzle
{
    public EnemyBT m_enemy;    

    // Start is called before the first frame update
    void Start()
    {
        m_enemy = GetComponentInChildren<EnemyBT>();        
    }

    // Update is called once per frame
    void Update()
    {
     if(m_updateMana && m_enemy.m_isStatue)
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
