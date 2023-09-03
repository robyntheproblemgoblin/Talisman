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
    public Puzzle m_outputLeftObject;
    public Puzzle m_outputRightObject;

    // DELETE THESE TESTING PURPOSES
    public Material m_grey;
    public Material m_black;
    public Material m_white;

    public MeshRenderer m_one;
    public MeshRenderer m_two;
    public MeshRenderer m_three;

    MeshRenderer m_in;
    MeshRenderer m_out;
    MeshRenderer m_third;
    // STOP DELETE

    private new void Start()
    {
        if (m_outputRightObject != null)
        {
            m_outputRightObject.SetInputObject(this);
        }
        if (m_outputLeftObject != null)
        {
            var left = (RotateCircularMana)m_outputLeftObject;
            if (left.m_twoInputs)
            {
                left.SetSecondInputObject(this);
            }
            else
            {
                m_outputLeftObject.SetInputObject(this);
            }
        }

        if (m_isThreeWay)
        {
            //Turn on all mesh visuals           
            m_one.material = m_black;
            m_two.material = m_black;
            m_three.material = m_black;
            m_isLeftBent = false;
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

        if (m_isThreeWay)
        {
            if (m_in == m_one && m_out == m_two || m_in == m_two && m_out == m_one)
            {
                m_third = m_three;
            }
            else if (m_in == m_three && m_out == m_one || m_in == m_one && m_out == m_three)
            {
                m_third = m_two;
            }
            else
            {
                m_third = m_one;
            }
            m_third.material = m_black;
        }
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
                if (m_isThreeWay)
                {
                    m_third.material = m_black;
                }
                //STOP DELETE
                if (m_inputObject != null && m_inputObject.m_manaValue >= 1.0f && m_inputObject.m_rewindHere)
                {
                    m_inputObject.RewindPuzzle();
                }
                if (m_secondInputObject != null && m_secondInputObject.m_manaValue >= 1.0f && m_secondInputObject.m_rewindHere)
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
            if (m_isThreeWay)
            {
                m_third.material = m_white;
            }
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
                if ((m_inputObject.m_manaValue >= 1.0f && (m_inputObject.m_input == Positions.TWO || m_inputObject.m_output == Positions.TWO))
                    && (m_secondInputObject.m_manaValue >= 1.0f && (m_inputObject.m_input == Positions.THREE || m_secondInputObject.m_output == Positions.THREE)))
                {
                    if (m_outputLeftObject == null)
                    {
                        FailedPuzzle();
                    }
                    else
                    {
                        m_outputLeftObject.m_updateMana = true;
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
                if (m_outputLeftObject != null && (m_outputLeftObject.m_input == Positions.ONE || m_outputLeftObject.m_output == Positions.ONE))
                {
                    m_outputLeftObject.m_updateMana = true;
                }
                else
                {
                    //Activate futz graphic
                    FailedPuzzle();
                }
                if (m_outputRightObject != null && (m_outputRightObject.m_input == Positions.ONE || m_outputRightObject.m_output == Positions.ONE))
                {
                    m_outputRightObject.m_updateMana = true;
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
                if (m_outputLeftObject != null && (m_outputLeftObject.m_input == Positions.ONE || m_outputLeftObject.m_output == Positions.ONE))
                {
                    m_outputLeftObject.m_updateMana = true;
                    if (m_outputRightObject != null)
                    {
                        m_outputRightObject.FailedPuzzle();
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
                if (m_outputRightObject != null && (m_outputRightObject.m_input == Positions.ONE || m_outputRightObject.m_output == Positions.ONE))
                {
                    m_outputRightObject.m_updateMana = true;
                    if (m_outputLeftObject != null)
                    {
                        m_outputLeftObject.FailedPuzzle();
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
                if (m_outputRightObject != null && (m_outputRightObject.m_input == Positions.ONE || m_outputRightObject.m_output == Positions.ONE))
                {
                    m_outputRightObject.m_updateMana = true;
                    if (m_outputLeftObject != null)
                    {
                        m_outputLeftObject.FailedPuzzle();
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
                if (m_outputLeftObject != null && (m_outputLeftObject.m_input == Positions.ONE || m_outputLeftObject.m_output == Positions.ONE))
                {
                    m_outputLeftObject.m_updateMana = true;
                    if (m_outputRightObject != null)
                    {
                        m_outputRightObject.FailedPuzzle();
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

    public override void FailedPuzzle()
    {
        if (m_twoInputs)
        {
            m_rewindHere = true;
            if (m_inputObject != null && !m_inputObject.m_rewindHere)
            {
                m_inputObject.FailedPuzzle();
            }
            if (m_secondInputObject != null && !m_secondInputObject.m_rewindHere)
            {
                m_secondInputObject.FailedPuzzle();
            }
        }
        else
        {
            base.FailedPuzzle();
        }
    }

    public override void RewindPuzzle()
    {
        if (!m_rewindMana)
        {
            if (m_isThreeWay)
            {
                if (m_twoInputs)
                {
                    if (m_outputLeftObject != null && m_outputLeftObject.m_rewindHere)
                    {
                        m_outputLeftObject.RewindPuzzle();
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
                    if (m_outputLeftObject != null && m_outputLeftObject.m_rewindHere)
                    {
                        m_outputLeftObject.RewindPuzzle();
                    }
                    if (m_outputRightObject != null && m_outputRightObject.m_rewindHere)
                    {
                        m_outputRightObject.RewindPuzzle();
                    }
                    if (!m_outputLeftObject.m_rewindHere && !m_outputRightObject.m_rewindHere)
                    {

                        m_rewindHere = false;

                        m_rewindMana = true;
                        m_updateMana = true;
                    }
                }
            }
            else if (m_input == Positions.ONE)
            {
                if (m_isLeftBent)
                {
                    if (m_outputLeftObject != null && m_outputLeftObject.m_rewindHere)
                    {
                        Debug.Log(gameObject.name + " is the fucker");
                        m_outputLeftObject.RewindPuzzle();
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
                    if (m_outputRightObject != null && m_outputRightObject.m_rewindHere)
                    {
                        Debug.Log(gameObject.name + " is the fucker");
                        m_outputRightObject.RewindPuzzle();
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
                    if (m_outputRightObject != null && m_outputRightObject.m_rewindHere)
                    {
                        Debug.Log(gameObject.name + " is the fucker");
                        m_outputRightObject.RewindPuzzle();
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
                    if (m_outputLeftObject != null && m_outputLeftObject.m_rewindHere)
                    {
                        Debug.Log(gameObject.name + " is the fucker");
                        m_outputLeftObject.RewindPuzzle();
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