using System;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class HurtBoxObject : MonoBehaviour
{
    [HideInInspector] public HurtBox m_hurtbox;

    private void Update()
    {
        if (m_hurtbox.m_owner.m_debugHurtBoxes)
        {
            m_hurtbox.UpdateDebugMeshSize();
        }
    }
    public void StupidDestroyHurtBox()
    {
        DestroyImmediate(this.gameObject);
    }
}