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
}

public enum Faces
{
    ONE,
    TWO,
    THREE,
}




