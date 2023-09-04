using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Lever : Puzzle
{
    public bool m_isOn = true;

    public float m_onAngle = 25f;
    public float m_offAngle = -25f;

    public float m_leverSpeed = 10;
    public float m_manaSpeed = 10;

    public Puzzle m_connectedPuzzle;
    public bool m_canBeInteracted = true;

    private void Start()
    {
        m_connectedPuzzle.SetInputObject(this);
        base.Start();
    }

    private void FixedUpdate()
    {
        if (m_isOn && Quaternion.Angle(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, m_onAngle)) >= 1)
        {
            float step = m_leverSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, m_onAngle), step);
        }
        else if (m_isOn == false && Quaternion.Angle(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, m_offAngle)) >= 1)
        {            
            float step = m_leverSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, m_offAngle), step);
        }
        if (!m_canBeInteracted)
        {
            if (m_isOn && Quaternion.Angle(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, m_onAngle)) < 1)
            {
                m_canBeInteracted = true;
            }
            else if (!m_isOn && Quaternion.Angle(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, m_offAngle)) < 1)
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
            if (!m_isOn)
            {
                m_canBeInteracted = false;
                m_isOn = true;
                m_connectedPuzzle.m_updateMana = true;
            }
            else
            {
                m_canBeInteracted = false;
                m_isOn = false;
                m_connectedPuzzle.RewindPuzzle();
            }
        }
    }
}
