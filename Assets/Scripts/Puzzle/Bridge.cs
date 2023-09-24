using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Bridge : MonoBehaviour
{
    public List<Puzzle> m_puzzleList;
    public bool m_unlocked;
    public float m_speed;
    List<Material> m_materials;
    MeshRenderer[] m_meshes;
    Color m_color;
    public GameObject m_colliders;

    private void Start()
    {
        foreach(Puzzle p in m_puzzleList)
        {
            p.m_bridge = this;
        }
        m_materials = new List<Material>();
        m_meshes = gameObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < m_meshes.Length; i++)
        {
            m_materials.Add(m_meshes[i].material);
        }
        m_color = m_materials[0].color;
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
        if (m_unlocked && m_color.a < 1f)
        {
            m_color = new Color(m_color.r, m_color.g, m_color.b, m_color.a + step);
            foreach (Material m in m_materials)
            {
                m.color = m_color;
            }
        }
        else if (!m_unlocked && m_color.a > 0f)
        {
            m_color = new Color(m_color.r, m_color.g, m_color.b, m_color.a - step);
            foreach (Material m in m_materials)
            {
                m.color = m_color;
            }
        }
    }   
}
