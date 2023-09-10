using UnityEngine;

public class SolvedFlare : Puzzle
{
    public ParticleSystem m_flare;

    [Space(5), Header("Connected Objects"), Space(5)]
    public Puzzle m_outputObject;

    bool m_isSolved = false;
    private void Update()
    {
        if(m_updateMana && !m_isSolved)
        {
            m_isSolved = true;
            m_flare.Play();
            //m_outputObject.m_updateMana = true;
        }
    }

    public override void RotatePuzzle()
    {
        
    }

    public override void RewindPuzzle()
    {
        m_inputObject.RewindPuzzle();
    }
}
