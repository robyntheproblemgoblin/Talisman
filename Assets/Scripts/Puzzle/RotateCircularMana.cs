using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCircularMana : Puzzle
{
    [Space(5), Header("Type of visual"), Space(5)]
    public bool m_isThreeWay;
    public bool m_twoInputs;
    public bool m_isLeftBent;

    [Space(5), Header("Speed Mana flows"), Space(5)]
    public float m_speed = 1.0f;

    [Space(5), Header("Connected Objects"), Space(5)]
    public Puzzle m_leftObject;
    public Puzzle m_rightObject;

    private void Start()
    {
        var right = (RotateCircularMana)m_rightObject;
        var left = (RotateCircularMana)m_leftObject;
        if (right != null && right.m_twoInputs)
        {
            right.m_inputObject = this;
        }
        if (left != null && left.m_twoInputs)
        {
            left.m_secondInputObject = this;
        }

        if (m_isThreeWay)
        {
            //Turn on all mesh visuals           
        }
        else if (m_isLeftBent)
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
        else
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
    }

    private void Update()
    {
        if (m_updateMana)
        {
            UpdateMana();
        }
    }

    public override void UpdatePositions()
    {
        if (m_input == Positions.THREE)
        {
            m_input = Positions.ONE;
        }
        else
        {
            m_input++;
        }
        if (m_output == Positions.THREE)
        {
            m_output = Positions.ONE;
        }
        else
        {
            m_output++;
        }
    }

    public override void UpdateMana()
    {
        m_manaValue += Time.deltaTime * m_speed;
        //Update material
        if (m_manaValue > 1.0f)
        {
            m_manaValue = 1.0f;
            m_updateMana = false;
            //Update material
            StartNextSeuquence();
        }
    }

    void StartNextSeuquence()
    {
        if (m_isThreeWay)
        {
            if (m_twoInputs)
            {
                if (m_inputObject.m_manaValue >= 1.0f && m_secondInputObject.m_manaValue >= 1.0f)
                {
                    m_leftObject.m_updateMana = true;
                }
                else
                {
                    //Activate futz graphic
                }

            }
            else
            {
                if (m_leftObject.m_input == Positions.ONE || m_leftObject.m_output == Positions.ONE)
                {
                    m_leftObject.m_updateMana = true;
                }
                else
                {
                    //Activate futz graphic
                }
                if (m_rightObject.m_input == Positions.ONE || m_rightObject.m_output == Positions.ONE)
                {
                    m_rightObject.m_updateMana = true;
                }
                else
                {
                    //Activate futz graphic
                }
            }
        }
        else if (m_input == Positions.ONE)
        {
            if (m_isLeftBent)
            {
                if (m_leftObject.m_input == Positions.ONE || m_leftObject.m_output == Positions.ONE)
                {
                    m_leftObject.m_updateMana = true;
                }
                else
                {
                    //Activate futz graphic
                }
            }
            else
            {
                if (m_rightObject.m_input == Positions.ONE || m_rightObject.m_output == Positions.ONE)
                {
                    m_rightObject.m_updateMana = true;
                }
                else
                {
                    //Activate futz graphic
                }
            }
        }
        else if (m_output == Positions.ONE)
        {
            if (m_isLeftBent)
            {
                if (m_rightObject.m_input == Positions.ONE || m_rightObject.m_output == Positions.ONE)
                {
                    m_rightObject.m_updateMana = true;
                }
                else
                {
                    //Activate futz graphic
                }
            }
            else
            {
                if (m_leftObject.m_input == Positions.ONE || m_leftObject.m_output == Positions.ONE)
                {
                    m_leftObject.m_updateMana = true;
                }
                else
                {
                    //Activate futz graphic
                }
            }
        }

    }
}
