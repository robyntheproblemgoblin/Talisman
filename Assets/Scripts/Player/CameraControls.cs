using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    [HideInInspector]
    public GameObject m_player;
    float m_xRotation;
    float m_yRotation;       
    
    [HideInInspector]
    public float m_mouseSensitivity;    

    public void SetupCamera(GameObject go, float sensitivity)
    {
        m_player = go;
        m_mouseSensitivity = sensitivity;       
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void MoveCamera(Vector2 mouseMove)
    {           
        m_xRotation -= mouseMove.y * Time.deltaTime * m_mouseSensitivity;
        m_xRotation = Mathf.Clamp(m_xRotation, -90, 90);
        m_yRotation += mouseMove.x * Time.deltaTime * m_mouseSensitivity;
        transform.rotation = Quaternion.Euler(m_xRotation, m_yRotation, 0);   
    }
}
