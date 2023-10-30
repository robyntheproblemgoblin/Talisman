using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class ManaPool : MonoBehaviour
{
    public float m_manaAmount;
    public Light m_light;
    float m_intensity;
    public float m_lightDimSpeed = 1f;
    public float m_waterDimSpeed = 1f;
    float m_emissiveMax;
    float m_currentEmissive;
    public MeshRenderer m_waterMesh;

    public bool m_spritesFirst;
    public List<string> m_interactStrings = new List<string>();
    public List<ControlSprites> m_interactSprites = new List<ControlSprites>();
    public bool m_isEnd;
    bool m_isActive = true;
    bool m_isFirst = true;

    public List<Door> m_doorList = new List<Door>();

    public FMODUnity.EventReference m_loopSound;
    public FMODUnity.EventReference m_interactSound;
    FMOD.Studio.EventInstance m_loopInstance;


    private void Start()
    {
        m_intensity = m_light.intensity;
        m_emissiveMax = m_waterMesh.material.GetFloat("_EmissivStrength");
        m_currentEmissive = m_emissiveMax;
        if(!m_isEnd)
        {
            m_loopInstance = RuntimeManager.CreateInstance(m_loopSound);
            RuntimeManager.AttachInstanceToGameObject(m_loopInstance, gameObject.transform);
            m_loopInstance.start();
            m_loopInstance.release();
        }
    }

    void Update()
    {
        float lightStep = m_lightDimSpeed * Time.deltaTime;
        float waterStep = m_waterDimSpeed * Time.deltaTime;
        if(m_isActive && m_light.intensity <= m_intensity)
        {
            m_light.intensity += lightStep;
        }
        else if (!m_isActive && m_light.intensity >= 0)
        {
            m_light.intensity -= lightStep;
        }
        
        if (m_isActive && m_waterMesh.material.GetFloat("_EmissivStrength") <= m_emissiveMax)
        {
            m_waterMesh.material.SetFloat("_EmissiveStrength", m_currentEmissive += waterStep);
        }
        else if(!m_isActive && m_waterMesh.material.GetFloat("_EmissivStrength") >= 0)
        {
            m_waterMesh.material.SetFloat("_EmissiveStrength", m_currentEmissive -= waterStep);
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
            GameManager.Instance.m_audioManager.PlayOneShot(m_interactSound, transform.position);
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
