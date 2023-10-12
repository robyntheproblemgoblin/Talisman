using UnityEngine;

public class ManaPipe : Puzzle
{
    [Space(5), Header("Speed Mana flows"), Space(5)]
    public float m_speed = 1.0f;
    public float m_rewindSpeed = 1.0f;

    [Space(5), Header("Connected Objects"), Space(5)]
    public Puzzle m_outputLeftObject;
    public Puzzle m_outputRightObject;

    bool m_twoOutputs = false;

    public Material m_black;
    public Material m_white;

    MeshRenderer m_pipe;

    private new void Start()
    {
        m_pipe = GetComponent<MeshRenderer>();
        if (m_outputLeftObject != null)
        {
            m_outputLeftObject.SetInputObject(this);
        }
        if (m_outputRightObject != null)
        {
            m_outputRightObject.SetInputObject(this);
        }
        if(m_outputLeftObject != null && m_outputRightObject != null)
        {
            m_twoOutputs = true;
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
        if (m_outputLeftObject != null)
        {
            if (m_outputLeftObject.m_input == Positions.ONE || m_outputLeftObject.m_output == Positions.ONE)
            {
                m_outputLeftObject.m_updateMana = true;
            }
        }
        if (m_outputRightObject != null)
        {
            if (m_outputRightObject.m_input == Positions.ONE || m_outputRightObject.m_output == Positions.ONE)
            {
                m_outputRightObject.m_updateMana = true;
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

    public override void StopRotation()
    {
        if (m_outputLeftObject != null)
        {
            m_outputLeftObject.StopRotation();
        }
        if (m_outputRightObject != null)
        {
            m_outputRightObject.StopRotation();
        }
    }

    public override void RewindPuzzle()
    {
        if (!m_rewindMana)
        {
            if (m_twoOutputs)
            {
                if (m_outputLeftObject != null && m_outputLeftObject.m_manaValue <= 0 &&
                          m_outputRightObject != null && m_outputRightObject.m_manaValue <= 0)
                {
                    m_rewindMana = true;
                    m_updateMana = true;
                }
                else
                {
                    if (m_outputLeftObject != null && m_outputLeftObject.m_manaValue > 0)
                    {
                        m_outputLeftObject.RewindPuzzle();
                    }
                    if (m_outputRightObject != null && m_outputRightObject.m_manaValue > 0)
                    {
                        m_outputRightObject.RewindPuzzle();
                    }
                }
            }
            else if (m_outputLeftObject != null && m_outputLeftObject.m_manaValue > 0)
            {
                m_outputLeftObject.RewindPuzzle();
            }
            else
            {
                m_rewindMana = true;
                m_updateMana = true;
            }
        }
        else
        {
            m_rewindMana = true;
            m_updateMana = true;
        }
    }
}
