using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public List<Puzzle> m_puzzleList;
    public bool m_unlocked;
    public Transform m_lockedPos;
    public Transform m_unlockedPos;
    public float m_speed;
    public List<Puzzle> m_outputPuzzles;
    bool m_manaContinue = true;

    public FMODUnity.EventReference m_openSound;
    public FMODUnity.EventReference m_closeSound;

    private void Start()
    {
        foreach(Puzzle puzzle in m_puzzleList)
        {
            puzzle.m_doors.Add(this);            
        }
    }

    public void CheckState()
    {
        foreach (Puzzle p in m_puzzleList)
        {
            if (p.m_unlocked == false)
            {
                if(m_unlocked == true)
                {
                    GameManager.Instance.m_audioManager.PlayOneShot(m_closeSound, gameObject.transform.position);
                }
                m_unlocked = false;
                return;
            }
        }
        GameManager.Instance.m_audioManager.PlayOneShot(m_openSound, gameObject.transform.position);
        m_unlocked = true;
        if(m_outputPuzzles.Count > 0 && m_manaContinue)
        {
            m_manaContinue = false;
            foreach (Puzzle puzzle in m_outputPuzzles)
            {
                puzzle.m_updateMana = true;
            }
        }
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

    public void CloseDoor()
    {
        GameManager.Instance.m_audioManager.PlayOneShot(m_closeSound, gameObject.transform.position);
        m_unlocked = false;
    }

    public void OpenDoor()
    {
        GameManager.Instance.m_audioManager.PlayOneShot(m_openSound, gameObject.transform.position);
        m_unlocked = true;
    }
}
