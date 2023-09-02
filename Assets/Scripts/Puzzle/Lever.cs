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

    private void Start()
    {
       m_connectedPuzzle.SetInputObject(this);
        base.Start();
    }

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
            if(transform.rotation.eulerAngles.z == m_offAngle)
            {
                m_canBeInteracted = true;
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

    public override void RotatePuzzle()
    {
        if (m_canBeInteracted)
        {
            m_canBeInteracted = false;
            m_isOn = !m_isOn;
            m_connectedPuzzle.m_updateMana = true;
        }
    }

    public override void FailedPuzzle()
    {
        if (!m_rewindHere)
        {
            m_rewindHere = true;
            Debug.Log("First Fail");
        }
        else
        {   
            m_connectedPuzzle.RewindPuzzle();
            Debug.Log("Lever Fail");
        }
    }

    public override void RewindPuzzle()
    {
        if (m_rewindMana)
        {
            m_isOn = !m_isOn;
            m_rewindMana = false;
        }
        else
        {
            m_updateMana = true;
            m_rewindMana = true;
        }
    }
}
