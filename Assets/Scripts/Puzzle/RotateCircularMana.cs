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
    public float m_rewindSpeed = 1.0f;

    [Space(5), Header("Connected Objects"), Space(5)]
    public Puzzle m_leftObject;
    public Puzzle m_rightObject;

    // DELETE THESE TESTING PURPOSES
    public Material m_grey;
    public Material m_black;
    public Material m_white;

    public MeshRenderer m_one;
    public MeshRenderer m_two;
    public MeshRenderer m_three;

    MeshRenderer m_in;
    MeshRenderer m_out;
    // STOP DELETE

    private new void Start()
    {
        if (m_rightObject != null)
        {
            m_rightObject.SetInputObject(this);
        }
        if (m_leftObject != null)
        {
            var left = (RotateCircularMana)m_leftObject;
            if (left.m_twoInputs)
            {
                left.SetSecondInputObject(this);
            }
            else
            {
                m_leftObject.SetInputObject(this);
            }
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
        base.Start();
        //DELETE THIS
        if (m_input == Positions.ONE)
        { m_in = m_one; }
        else if (m_input == Positions.TWO)
        { m_in = m_two; }
        else
        { m_in = m_three; }
        if (m_output == Positions.ONE)
        { m_out = m_one; }
        if (m_output == Positions.TWO)
        { m_out = m_two; }
        else
        { m_out = m_three; }
        // STOP DELETE
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
        if (m_input == Positions.ONE)
        {
            m_input = Positions.THREE;
        }
        else
        {
            m_input--;
        }
        if (m_output == Positions.ONE)
        {
            m_output = Positions.THREE;
        }
        else
        {
            m_output--;
        }
    }

    public override void UpdateMana()
    {
        if (m_rewindMana)
        {
            m_manaValue -= Time.deltaTime * m_speed;
            //Update material
            if (m_manaValue < 0.0f)
            {
                m_manaValue = 0.0f;
                m_updateMana = false;
                m_rewindMana = false;
                //Update material
                //DELETE THIS
                m_in.material = m_black;
                m_out.material = m_black;
                //STOP DELETE
                if (m_inputObject != null)
                {
                    m_inputObject.RewindPuzzle();
                }               
                if (m_secondInputObject != null)
                {
                    m_secondInputObject.RewindPuzzle();
                }
            }
        }
        else
        {
            m_manaValue += Time.deltaTime * m_speed;
            //Update material
            //DELETE THIS
            m_in.material = m_white;
            m_out.material = m_white;
            //STOP DELETE
            if (m_manaValue > 1.0f)
            {
                m_manaValue = 1.0f;
                m_updateMana = false;
                //Update material
                StartNextSequence();
            }
        }
    }

    void StartNextSequence()
    {
        if (m_isThreeWay)
        {
            if (m_twoInputs)
            {
                if (m_inputObject.m_manaValue >= 1.0f && m_secondInputObject.m_manaValue >= 1.0f)
                {
                    if (m_leftObject == null)
                    {
                        FailedPuzzle();
                    }
                    else
                    {
                        m_leftObject.m_updateMana = true;
                    }
                }
                else
                {
                    //Activate futz graphic
                    FailedPuzzle();
                }
            }
            else
            {
                if (m_leftObject != null && (m_leftObject.m_input == Positions.ONE || m_leftObject.m_output == Positions.ONE))
                {
                    m_leftObject.m_updateMana = true;
                }
                else
                {
                    //Activate futz graphic
                    FailedPuzzle();
                }
                if (m_rightObject != null && (m_rightObject.m_input == Positions.ONE || m_rightObject.m_output == Positions.ONE))
                {
                    m_rightObject.m_updateMana = true;
                }
                else
                {
                    //Activate futz graphic
                    FailedPuzzle();
                }
            }
        }
        else if (m_input == Positions.ONE)
        {
            if (m_isLeftBent)
            {
                if (m_leftObject != null && (m_leftObject.m_input == Positions.ONE || m_leftObject.m_output == Positions.ONE))
                {
                    m_leftObject.m_updateMana = true;
                    if (m_rightObject != null)
                    {
                        m_rightObject.FailedPuzzle();
                    }
                }
                else
                {
                    //Activate futz graphic
                    FailedPuzzle();
                }
            }
            else
            {
                if (m_rightObject != null && (m_rightObject.m_input == Positions.ONE || m_rightObject.m_output == Positions.ONE))
                {
                    m_rightObject.m_updateMana = true;
                    if (m_leftObject != null)
                    {
                        m_leftObject.FailedPuzzle();
                    }
                }
                else
                {
                    //Activate futz graphic
                    FailedPuzzle();
                }
            }
        }
        else if (m_output == Positions.ONE)
        {
            if (m_isLeftBent)
            {
                if (m_rightObject != null && (m_rightObject.m_input == Positions.ONE || m_rightObject.m_output == Positions.ONE))
                {
                    m_rightObject.m_updateMana = true;
                    if (m_leftObject != null)
                    {
                        m_leftObject.FailedPuzzle();
                    }
                }
                else
                {
                    //Activate futz graphic
                    FailedPuzzle();
                }
            }
            else
            {
                if (m_leftObject != null && (m_leftObject.m_input == Positions.ONE || m_leftObject.m_output == Positions.ONE))
                {
                    m_leftObject.m_updateMana = true;
                    if (m_rightObject != null)
                    {
                        m_rightObject.FailedPuzzle();
                    }
                }
                else
                {
                    //Activate futz graphic
                    FailedPuzzle();
                }
            }
        }
    }

    public override void RewindPuzzle()
    {        
        if (!m_rewindMana)
        {            
            if (m_isThreeWay)
            {

            }
            else if (m_input == Positions.ONE)
            {
                if (m_isLeftBent)
                {
                    if (m_leftObject != null && m_leftObject.m_rewindHere)
                    {
                        m_leftObject.RewindPuzzle();
                    }
                    else
                    {
                        m_rewindHere = false;
                        m_rewindMana = true;
                        m_updateMana = true;
                    }
                }
                else
                {
                    if (m_rightObject != null && m_rightObject.m_rewindHere)
                    {
                        m_rightObject.RewindPuzzle();
                    }
                    else
                    {
                        m_rewindHere = false;
                        m_rewindMana = true;
                        m_updateMana = true;
                    }
                }
            }
            else if (m_output == Positions.ONE)
            {
                if (m_isLeftBent)
                {
                    if (m_rightObject != null && m_rightObject.m_rewindHere)
                    {
                        m_rightObject.RewindPuzzle();
                    }
                    else
                    {
                        m_rewindHere = false;
                        m_rewindMana = true;
                        m_updateMana = true;
                    }
                }
                else
                {
                    if (m_leftObject != null && m_leftObject.m_rewindHere)
                    {
                        m_leftObject.RewindPuzzle();
                    }
                    else
                    {
                        m_rewindHere = false;
                        m_rewindMana = true;
                        m_updateMana = true;
                    }
                }
            }
        }
        else
        {
            Debug.Log("LogicBroken");
        }
    }
}