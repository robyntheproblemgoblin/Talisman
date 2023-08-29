using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCircularMana : Puzzle
{
    public bool m_isLeftBent;

    [Space(5), Header("Starting and End Symbols"), Space(5)]
    public Positions m_input;
    Positions m_output;

    private void Start()
    {
        if(m_isLeftBent)
        {
            if (m_input == Positions.THREE)
            {
                m_output = Positions.ONE;
            }
            else
            {                
                m_output = m_input + 1;
            }
        }
        else
        {
            if (m_input == Positions.ONE)
            {
                m_output = Positions.THREE;
            }
            else
            {
                m_output = m_input - 1;
            }
        }
    }

    public override void UpdatePositions()
    {
        
    }
}
