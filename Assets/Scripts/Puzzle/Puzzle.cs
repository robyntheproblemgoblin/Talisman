using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Puzzle : MonoBehaviour
{
    // Rotation safety
    protected bool m_rotate = false;
    protected float m_nextY;
    protected Quaternion m_targetRotation;
    protected int m_rotations = 0;
    [HideInInspector]
    public bool m_unlocked = false;
    public Door m_door;    

    private void Start()
    {
        m_nextY = transform.rotation.eulerAngles.y + 120;
        m_door.puzzleList.Add(this);
    }
   
    public void RotatePuzzle()
    {
        m_targetRotation = Quaternion.Euler(0, m_nextY, 0);
        m_rotations++;
        m_rotate = true;
    }

    public virtual void UpdatePositions()
    {

    }

    private void FixedUpdate()
    {
        if (m_rotate)
        {
            transform.RotateAround(transform.position, Vector3.up, 120 * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, m_targetRotation) <= 1f)
            {
                transform.rotation = Quaternion.Euler(0, m_nextY, 0);
                m_nextY += 120;
                m_rotate = false;
                if (m_rotations == 3)
                {
                    m_rotations = 0;
                    m_nextY -= 360;
                    transform.rotation = Quaternion.Euler(0, m_nextY, 0);
                }
                UpdatePositions();
            }
        }
    }
}

public enum Positions
{
    ONE,
    TWO,
    THREE,
}




