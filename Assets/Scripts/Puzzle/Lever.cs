using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Puzzle
{
    public bool m_isOn = true;

    public float m_onAngle = 25f;
    public float m_offAngle = -25f;

    public float m_leverSpeed = 10;
    public float m_manaSpeed = 10;

    public Puzzle m_connectedPuzzle;
    bool m_canBeInteracted = true;

    private void FixedUpdate()
    {
        if (m_isOn && transform.rotation.eulerAngles.z != m_onAngle)
        {
            float step = m_leverSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, m_onAngle), step);
        }
        else if (!m_isOn && transform.rotation.eulerAngles.z != m_offAngle)
        {
            float step = m_leverSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, m_offAngle), step);
        }
    }

    private void Update()
    {
        if (m_updateMana)
        {
            UpdateMana();
        }
    }
    public override void UpdateMana()
    {
        if (m_rewindMana)
        {
            m_manaValue += Time.deltaTime * m_manaSpeed;
            //Update material
            if (m_manaValue < 0.0f)
            {
                m_manaValue = 0.0f;
                m_updateMana = false;
                //Update material    
                m_canBeInteracted = true;
            }
        }
        else
        {
            m_manaValue += Time.deltaTime * m_manaSpeed;
            //Update material
            if (m_manaValue > 1.0f)
            {
                m_manaValue = 1.0f;
                m_updateMana = false;
                //Update material
                m_connectedPuzzle.m_updateMana = true;
            }
        }
    }


    public override void RotatePuzzle()
    {
        if (m_canBeInteracted)
        {
            m_canBeInteracted = false;
            m_isOn = !m_isOn;
        }
    }

    public override void FailedPuzzle()
    {
        if (!m_rewindHere)
        {
            m_rewindHere = true;
        }
        else
        {
            m_connectedPuzzle.RewindPuzzle();
        }
    }

    public override void RewindPuzzle()
    {
        if (m_rewindMana)
        {
            m_isOn = !m_isOn;
        }
        else
        {
            m_rewindMana = true;
        }
    }
}
