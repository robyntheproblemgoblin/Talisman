using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPipe : Puzzle
{
    [Space(5), Header("Speed Mana flows"), Space(5)]
    public float m_speed = 1.0f;
    public float m_rewindSpeed = 1.0f;

    [Space(5), Header("Connected Objects"), Space(5)]
    public Puzzle m_outputObject;

    public Material m_black;
    public Material m_white;

    MeshRenderer m_pipe;

    private new void Start()
    {
        m_pipe = GetComponent<MeshRenderer>();
        if (m_outputObject != null)
        {
            m_outputObject.SetInputObject(this);
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
                if (m_inputObject != null && !(m_inputObject is Lever))
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
        if (m_outputObject != null)
        {
            if (m_outputObject.m_input == Positions.ONE || m_outputObject.m_output == Positions.ONE)
            {
                m_outputObject.m_updateMana = true;
            }
        }
        else
        {
          // Activate futz graphic
        }
    }

    public override void RotatePuzzle()
    {
       
    }

    public override void RewindPuzzle()
    {
        if (!m_rewindMana)
        {
            if (m_outputObject != null && m_outputObject.m_manaValue > 0)
            {
                m_outputObject.RewindPuzzle();
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
            m_rewindHere = false;
            m_rewindMana = true;
            m_updateMana = true;
        }

    }
}
