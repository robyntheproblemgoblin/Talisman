using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SymbolMatch : Puzzle
{
    #region Symbol Match Fields
    // Set the first face and the last face
    [Space(5), Header("Starting and End Symbols"), Space(5)]
    public Faces m_current;
    public Faces m_end;

    // Door and conditions
    [Space(5), Header("Door and Conditions"), Space(5)]
    public bool m_lockRotationOnSolve = true;
    

    // Lightup to signify in correct position
    [Space(5), Header("Unlock Visual Signifier"), Space(5)]
    public Material m_lightMaterial;
    public MeshRenderer m_lightNotice;
    #endregion

    private void FixedUpdate()
    {
        if (m_rotate)
        {
            transform.RotateAround(transform.position, Vector3.up, 120 * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, m_targetRotation) <= 1f)
            {
                transform.rotation = Quaternion.Euler(0, m_nextY, 0);
                m_nextY += 120;
                m_rotate = false;
                if (m_rotations == 3)
                {
                    m_rotations = 0;
                    m_nextY -= 360;
                    transform.rotation = Quaternion.Euler(0, m_nextY, 0);
                }
                if (m_current == Faces.THREE)
                {
                    m_current = Faces.ONE;
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
    }
}
