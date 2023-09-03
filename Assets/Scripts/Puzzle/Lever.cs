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
            m_isOn = true;
            m_connectedPuzzle.m_updateMana = true;
        }
    }

    public override void FailedPuzzle()
    {
        if (!m_rewindHere)
        {
            Debug.Log("First Lever Fail");
            m_rewindHere = true;            
        }
        else
        {
            Debug.Log("Second Lever Fail");
            m_connectedPuzzle.RewindPuzzle();            
        }
    }

    public override void RewindPuzzle()
    {
        Debug.Log(" is Rewinding");
        m_isOn = false;
        m_rewindHere = false;
        m_canBeInteracted = true;
    }
}
