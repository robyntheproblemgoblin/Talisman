using Cysharp.Threading.Tasks;
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
    public float m_particleDimSpeed = 1f;
    float m_emissiveMax;
    float m_currentEmissive;
    float m_maxParticleAlpha;
    float m_currentParticleAlpha;
    public MeshRenderer m_waterMesh;
    public ParticleSystem m_manaRing;
    

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
        if (!m_isEnd)
        {
            m_emissiveMax = m_waterMesh.material.GetFloat("_EmissiveStrength");
            m_currentEmissive = m_emissiveMax;
            m_maxParticleAlpha = m_manaRing.startColor.a;
            m_currentParticleAlpha = m_maxParticleAlpha;
            m_loopInstance = RuntimeManager.CreateInstance(m_loopSound);
            RuntimeManager.AttachInstanceToGameObject(m_loopInstance, gameObject.transform);
            m_loopInstance.start();
            m_loopInstance.release();
        }
    }

    void Update()
    {
        float lightStep = m_lightDimSpeed * Time.deltaTime;
        if (m_isActive && m_light.intensity <= m_intensity)
        {
            m_light.intensity += lightStep;
        }
        else if (!m_isActive && m_light.intensity >= 0)
        {
            m_light.intensity -= lightStep;
        }
        if (!m_isEnd)
        {
            float waterStep = m_waterDimSpeed * Time.deltaTime;
            float particleStep = m_particleDimSpeed * Time.deltaTime;

            if (m_isActive && m_waterMesh.material.GetFloat("_EmissiveStrength") <= m_emissiveMax)
            {
                m_waterMesh.material.SetFloat("_EmissiveStrength", m_currentEmissive += waterStep);
            }
            else if (!m_isActive && m_waterMesh.material.GetFloat("_EmissiveStrength") >= 0)
            {
                m_waterMesh.material.SetFloat("_EmissiveStrength", m_currentEmissive -= waterStep);
            }
            if (m_isActive && m_manaRing.startColor.a <= m_maxParticleAlpha)
            {
                var mana = m_manaRing.startColor;
                mana.a = m_currentParticleAlpha += particleStep;
                m_manaRing.startColor = mana;
            }
            else if (!m_isActive && m_manaRing.startColor.a >= 0)
            {
                var mana = m_manaRing.startColor;
                mana.a = m_currentParticleAlpha -= particleStep;
                m_manaRing.startColor = mana;
            }
        }
    }

    public void Interact(PlayerController pc)
    {
        if (m_isEnd)
        {
            GameManager.Instance.LastCinematic().Forget();
        }
        else if (m_isActive)
        {
            m_isActive = false;
            GameManager.Instance.m_audioManager.PlayOneShot(m_interactSound, transform.position);
            pc.AddMana(m_manaAmount);
            if (m_isFirst)
            {
                m_isFirst = false;
                foreach (Door door in m_doorList)
                {
                    door.OpenDoor();
                }
            }
        }
    }



    public void ResetPool()
    {
        m_isActive = true;        
    }

    public bool IsActive()
    {
        return m_isActive;
    }
}
