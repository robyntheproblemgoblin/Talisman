using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    // Rotation safety
    protected bool m_rotate = false;
    protected float m_nextY;
    protected Quaternion m_targetRotation;
    protected int m_rotations = 0;
    [HideInInspector]
    public bool m_unlocked = false;
    [HideInInspector]
    public Door m_door;

    [HideInInspector]
    public bool m_updateMana = false;
    [HideInInspector]
    public float m_manaValue = 0f;

    protected Puzzle m_inputObject;
    protected Puzzle m_secondInputObject;

    // Set the first face and the last face
    [Space(5), Header("Starting and End Symbols"), Space(5)]
    public Positions m_input;
    public Positions m_output;

    [HideInInspector]
    public bool m_rewindHere = false;
    [HideInInspector]
    protected bool m_rewindMana;

    public void Start()
    {
        m_nextY = transform.rotation.eulerAngles.y + 120;
    }

    private void FixedUpdate()
    {
        if (m_rotate)
        {
            transform.Rotate(0, 120 * Time.deltaTime, 0);
            if (Quaternion.Angle(transform.rotation, m_targetRotation) <= 5f)
            {
                transform.rotation = Quaternion.Euler(0, m_nextY, 0);
                m_nextY += 120;
                m_rotate = false;
                UpdatePositions();
            }
        }
    }

    public virtual void RotatePuzzle()
    {
        m_targetRotation = Quaternion.Euler(0, m_nextY, 0);
        m_rotations++;
        m_rotate = true;
    }

    public virtual void UpdatePositions() { }

    public virtual void UpdateMana() { }

    public virtual void FailedPuzzle()
    {
        m_rewindHere = true;
        if (m_inputObject != null)
        {
            m_inputObject.FailedPuzzle();
        }
        if (m_secondInputObject != null)
        {
            m_secondInputObject.FailedPuzzle();
        }
    }

    public virtual void RewindPuzzle()
    {

    }

    public void SetInputObject(Puzzle p)
    {
        m_inputObject = p;
    }

    public void SetSecondInputObject(Puzzle p)
    {
        m_secondInputObject = p;
    }
}

public enum Positions
{
    ONE,
    TWO,
    THREE,
}




