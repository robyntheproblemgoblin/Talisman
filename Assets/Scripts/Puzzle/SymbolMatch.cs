using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SymbolMatch : Puzzle
{    
    // Set the first face and the last face
    [Space(5), Header("Starting and End Symbols"), Space(5)]
    public Positions m_current;
    public Positions m_end;

    // Door and conditions
    [Space(5), Header("Door and Conditions"), Space(5)]
    public bool m_lockRotationOnSolve = true;
    
    // Lightup to signify in correct position
    [Space(5), Header("Unlock Visual Signifier"), Space(5)]
    public Material m_lightMaterial;
    public MeshRenderer m_lightNotice; 

    public override void UpdatePositions()
    {
        if (m_current == Positions.THREE)
        {
            m_current = Positions.ONE;
        }
        else
        {
            m_current++;
        }
        if (m_current == m_end)
        {
            m_unlocked = m_lockRotationOnSolve;
            List<Material> nm = m_lightNotice.sharedMaterials.ToList();
            nm.Add(m_lightMaterial);
            m_lightNotice.sharedMaterials = nm.ToArray();
            m_door.CheckState();
        }
    }

   
}
