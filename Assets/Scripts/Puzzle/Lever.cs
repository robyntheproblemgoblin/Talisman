using UnityEngine;

public class Lever : Puzzle
{
    public bool m_isOn = true;

    public float m_onAngle = -110f;
    public float m_offAngle = -65f;

    public float m_leverSpeed = 10;
    public float m_manaSpeed = 10;

    public Puzzle m_connectedPuzzle;
    public bool m_canBeInteracted = true;

    private new void Start()
    {
        if(m_connectedPuzzle != null)
        {
            m_connectedPuzzle.SetInputObject(this);
        }
        
        base.Start();
    }

    private void FixedUpdate()
    {
        if (m_isOn && Quaternion.Angle(transform.rotation, Quaternion.Euler(m_onAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z)) >= 1)
        {
            float step = m_leverSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(m_onAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), step);
        }
        else if (m_isOn == false && Quaternion.Angle(transform.rotation, Quaternion.Euler(m_onAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z)) >= 1)
        {            
            float step = m_leverSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(m_onAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), step);
        }
        if (!m_canBeInteracted)
        {
            if (m_isOn && Quaternion.Angle(transform.rotation, Quaternion.Euler(m_onAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z)) < 1)
            {
                m_canBeInteracted = true;
            }
            else if (!m_isOn && Quaternion.Angle(transform.rotation, Quaternion.Euler(m_onAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z)) < 1)
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
                if (m_connectedPuzzle != null)
                {
                    m_connectedPuzzle.m_updateMana = true;
                    m_connectedPuzzle.StopRotation();
                }
            }
            else
            {
                m_canBeInteracted = false;
                m_isOn = false; 
                if (m_connectedPuzzle != null)
                {
                    m_connectedPuzzle.StopRotation();
                    m_connectedPuzzle.RewindPuzzle();
                }
            }
            foreach(Door door in m_doors)
            {
                door.CheckState();
            }
        }
    }
}
