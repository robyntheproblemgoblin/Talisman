using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    MenuManager m_manager;
    public string m_tutorialText;
    public string m_secondText;
    public EnemyActivator m_enemy;
    bool m_firstDone = false;

    private void Start()
    {
        m_manager = GameManager.Instance.m_menuManager;        
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            m_manager.SetTutorial(m_firstDone?m_secondText:m_tutorialText);
            if(m_enemy != null && !m_enemy.m_updateMana)
            {                
                m_enemy.m_updateMana = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            m_manager.ClearTutorial();
        }
    }

    public void SecondTutorial()
    {
        if (m_firstDone)
        {
            m_manager.ClearTutorial();
            Destroy(this);
        }
        else
        {
            m_firstDone = true;
            m_manager.SetTutorial(m_secondText);
        }
    }
}
