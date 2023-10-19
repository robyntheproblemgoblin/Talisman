using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Drawbridge : MonoBehaviour
{
    public Vector3 m_angle;
    [HideInInspector]
    public bool m_draw;

    private void Update()
    {
        if(m_draw && gameObject.transform.rotation.eulerAngles != m_angle)
        {
            gameObject.transform.rotation.SetFromToRotation(gameObject.transform.rotation.eulerAngles, m_angle);
        }
    }
}
