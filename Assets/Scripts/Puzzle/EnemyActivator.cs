using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivator : Puzzle
{
    EnemyBT m_enemy;    

    // Start is called before the first frame update
    void Start()
    {
        m_enemy = GetComponentInChildren<EnemyBT>();        
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public override void RotatePuzzle()
    {
    }   
    
}
