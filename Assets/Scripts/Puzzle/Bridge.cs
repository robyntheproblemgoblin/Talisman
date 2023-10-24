using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public List<Puzzle> m_puzzleList;
    [HideInInspector]
    public bool m_unlocked;
    public float m_speed;
    MeshRenderer m_mesh;
    float m_alpha;
    public GameObject m_colliders;

    private void Start()
    {
        foreach (Puzzle p in m_puzzleList)
        {
            p.m_bridge = this;
        }
        m_mesh = GetComponentInChildren<MeshRenderer>();
        foreach (Material m in m_mesh.materials)
        {
            m.SetFloat("_ArmorFade", 0);
        }
    }

    public void CheckState()
    {
        foreach (Puzzle p in m_puzzleList)
        {
            if (p.m_unlocked == false)
            {
                SetBridgeState(false);
                return;
            }
        }
        SetBridgeState(true);
    }

    public void SetBridgeState(bool state)
    {
        m_unlocked = state;
        m_colliders.SetActive(state);
    }

    private void Update()
    {
        var step = m_speed * Time.deltaTime;
        if (m_unlocked && m_alpha < 1f)
        {
            m_alpha += step;
        }
        else if (!m_unlocked && m_alpha > 0f)
        {
            m_alpha -= step;
        }
        foreach (Material m in m_mesh.materials)
        {
            m.SetFloat("_ArmorFade", m_alpha);
        }
    }
}
