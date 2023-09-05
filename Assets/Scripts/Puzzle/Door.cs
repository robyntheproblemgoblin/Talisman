using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    public List<Puzzle> m_puzzleList;
    bool m_unlocked;
    public Transform m_lockedPos;
    public Transform m_unlockedPos;
    public float m_speed;

    private void Start()
    {
        foreach(Puzzle puzzle in m_puzzleList)
        {
            puzzle.m_door = this;
            if(puzzle is SymbolMatch)
            {
                SymbolMatch sm = puzzle as SymbolMatch;
                sm.CheckSolve();
            }
        }
    }

    public void CheckState()
    {
        foreach (Puzzle p in m_puzzleList)
        {
            if (p.m_unlocked == false)
            {
                m_unlocked = false;
                return;
            }
        }
        m_unlocked = true;
    }

    private void Update()
    {
        var step = m_speed * Time.deltaTime;
        if (m_unlocked && transform.position != m_unlockedPos.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_unlockedPos.position, step);
        }
        else if (!m_unlocked && transform.position != m_lockedPos.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_lockedPos.position, step);
        }
    }
}
