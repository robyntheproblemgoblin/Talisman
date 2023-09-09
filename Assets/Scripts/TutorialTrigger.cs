using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    MenuManager m_manager;
    public string m_tutorialText;

    private void Start()
    {
        m_manager = GameManager.Instance.m_menuManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            m_manager.SetTutorial(m_tutorialText);
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
}
