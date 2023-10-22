using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Drawbridge : MonoBehaviour
{
    public Vector3 m_angle;
    [HideInInspector]
    public bool m_draw;
    public float m_speed = 10f;

    private void Update()
    {
        if(m_draw && transform.rotation.eulerAngles != m_angle)
        {
            var step  =  m_speed * Time.deltaTime;

            Quaternion target = Quaternion.Euler(m_angle);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, step);
        }
    }
}
