using UnityEngine;

public class SolvedFlare : Puzzle
{
    public ParticleSystem m_flare;

    [Space(5), Header("Connected Objects"), Space(5)]
    public Puzzle m_outputObject;
    public bool m_murayPuzzle;
    public Light m_light;

    bool m_isSolved = false;
    private void Update()
    {
        if(m_updateMana && !m_isSolved)
        {
            m_unlocked = true;
            m_isSolved = true;
            m_flare.Play();
            m_light.gameObject.SetActive(true);
            GameManager.Instance.m_audioManager.PlayOneShot(m_manaFlowOn, gameObject.transform.position);
            GameManager.Instance.m_audioManager.PlayOneShot(m_manaFlowOff, gameObject.transform.position);
            foreach (Door door in m_doors)
            {
                door.CheckState();
            }
            if(m_bridge != null)
            {
                m_bridge.CheckState();
            }
            if(m_outputObject != null)
            {
                m_outputObject.m_updateMana = true;
            }
            if (m_murayPuzzle)
            {
                GameManager.Instance.m_audioManager.PlayMurrayPuzzleDialogue();
            }
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
