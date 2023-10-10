using AISystem;
using System.Diagnostics;

public class EnemyActivator : Puzzle
{
    public Enemy m_enemy;
    public Puzzle m_puzzle;
    public TutorialTrigger m_tutorial;

    void Update()
    {
        if (m_updateMana && m_enemy != null && m_enemy.IsStatue())
        {
            m_updateMana = false;
            m_enemy.SetStatue(false);
        }        
    }

    public override void RotatePuzzle() { }

    public void EnemyDead()
    {
        m_unlocked = true;
        if (m_puzzle != null)
        {
            m_puzzle.m_updateMana = true;
        }
        foreach (Door door in m_doors)
        {
            door.CheckState();
        }
        
        if(m_tutorial != null)
        {
            m_tutorial.SecondTutorial();
        }
    }
}
