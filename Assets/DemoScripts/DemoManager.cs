using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoManager : MonoBehaviour
{
    public Transform m_defenceObject;
    CombatItem m_defenceCombatItem;
    Vector3 m_defenceObjectOriginalLocation;
    public Transform m_offenceObject;
    CombatItem m_offenceCombatItem;
    Slider m_knocbackStrength;

    private void Start()
    {
        m_offenceCombatItem = m_offenceObject.GetComponent<CombatItem>();
        m_defenceCombatItem = m_defenceObject.GetComponent<CombatItem>();
        m_defenceObjectOriginalLocation = m_defenceObject.position;
    }
    private void Update()
    {
    }

    public void ResetDefenceObject()
    {
        m_defenceObject.position = m_defenceObjectOriginalLocation;
    }

    public void DoMoveOne()
    {
        m_offenceCombatItem.UseMove(0);
    }
    public void DoMoveTwo()
    {
        m_offenceCombatItem.UseMove(1);
    }

    public void UpdateKnockBack(float knockBackDistance)
    {

        foreach (Move m in m_offenceCombatItem.m_moves)
        {
            foreach (HitBox hB in m.m_moveHitBoxes)
            {
                hB.m_knockbackDistance = knockBackDistance;
            }
        }
    }

    public void ToggleDebug()
    {
        m_offenceCombatItem.ToggleDebug();
        m_defenceCombatItem.ToggleDebug();
    }
}
