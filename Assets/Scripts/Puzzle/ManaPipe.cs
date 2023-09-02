using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPipe : Puzzle
{
    [Space(5), Header("Speed Mana flows"), Space(5)]
    public float m_speed = 1.0f;
    public float m_rewindSpeed = 1.0f;

    [Space(5), Header("Connected Objects"), Space(5)]
    public Puzzle m_leftObject;

    public Material m_black;
    public Material m_white;

    MeshRenderer m_pipe;

    private new void Start()
    {
        m_pipe = GetComponent<MeshRenderer>();
        m_leftObject.SetInputObject(this);
    }

    private void Update()
    {
        if (m_updateMana)
        {
            UpdateMana();
        }
    }

    public override void RotatePuzzle() { }

    public override void UpdateMana()
    {
        if (m_rewindMana)
        {
            m_manaValue -= Time.deltaTime * m_rewindSpeed;
            //Update material
            if (m_manaValue < 0.0f)
            {
                m_manaValue = 0.0f;
                m_rewindMana = false;
                m_updateMana = false;
                //Update material
                //DELETE THIS
                m_pipe.material = m_black;
                //STOP DELETE
                if (m_inputObject != null)
                {
                    m_inputObject.RewindPuzzle();
                }
            }
        }
        else
        {
            m_manaValue += Time.deltaTime * m_speed;
            //Update material
            //DELETE THIS
            m_pipe.material = m_white;
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
        if (m_leftObject.m_input == Positions.ONE || m_leftObject.m_output == Positions.ONE)
        {
            m_leftObject.m_updateMana = true;
        }
    }

    public override void RewindPuzzle()
    {
        if (!m_rewindMana)
        {
            Debug.Log("Pipe Sent Fail");

            m_leftObject.RewindPuzzle();
        }
        else
        {
            m_updateMana = true;
        }
    }
}
