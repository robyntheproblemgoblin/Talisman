using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public List<Puzzle> puzzleList;
    bool unlocked;

    public void CheckState()
    {
        foreach (Puzzle p in puzzleList)
        {
            if (p.m_unlocked == false)
            {
                return;
            }
        }
        unlocked = true;
    }

    private void Update()
    {
        if (unlocked)
        {
            transform.position += new Vector3(0, 5 * Time.deltaTime);
        }
    }
}
