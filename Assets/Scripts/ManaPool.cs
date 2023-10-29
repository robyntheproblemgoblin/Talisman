using System.Collections.Generic;
using UnityEngine;

public class ManaPool : MonoBehaviour
{
    public float m_manaAmount;
    public Light m_light;
    float m_intensity;
    public float m_lightDimSpeed = 1f;

    public bool m_spritesFirst;
    public List<string> m_interactStrings = new List<string>();
    public List<ControlSprites> m_interactSprites = new List<ControlSprites>();
    public bool m_isEnd;
    bool m_isActive = true;
    bool m_isFirst = true;

    public List<Door> m_doorList = new List<Door>();


    private void Start()
    {
        m_intensity = m_light.intensity;
    }

    void Update()
    {
        float step = m_lightDimSpeed * Time.deltaTime;
        if(m_isActive && m_light.intensity <= m_intensity)
        {
            m_light.intensity += step;
        }
        else if (!m_isActive && m_light.intensity >= 0)
        {
            m_light.intensity -= step;
        }
    }

    public void Interact(PlayerController pc)
    {
        if (m_isEnd)
        {
            GameManager.Instance.m_menuManager.m_falseEnd.SetActive(true);
            GameManager.Instance.m_menuManager.m_eventSystem.SetSelectedGameObject(GameManager.Instance.m_menuManager.m_falseQuit.gameObject);
            GameManager.Instance.m_player.m_inputControl.Player_Map.Disable();
            GameManager.Instance.m_player.m_inputControl.UI.Enable();
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }
        else if (m_isActive)
        {
            m_isActive = false;
            pc.AddMana(m_manaAmount);
            if(m_isFirst)
            {
                m_isFirst = false;
                foreach(Door door in m_doorList)
                {
                    door.OpenDoor();
                }
            }
        }
    }



    public void ResetPool()
    {
        m_isActive = true;        
        m_light.intensity = m_intensity;
    }

    public bool IsActive()
    {
        return m_isActive;
    }
}
