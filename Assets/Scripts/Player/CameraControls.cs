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
    Vector2 m_mouseMovement;
    float m_cameraY;
    Vector3 m_pos;
    [HideInInspector]
    public float m_mouseSensitivity;
    Camera cam;

    public void SetupCamera(GameObject go, float sensitivity)
    {
        m_player = go;
        m_mouseSensitivity = sensitivity;
        m_cameraY = transform.position.y - m_player.transform.position.y;
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
    }
    
    public void MoveCamera(Vector2 mouseMove)
    {
        m_pos = m_player.transform.position;
        m_pos.y += m_cameraY;
        transform.position = m_pos;
        m_mouseMovement = mouseMove;
        m_xRotation -= m_mouseMovement.y * Time.smoothDeltaTime * m_mouseSensitivity;
        m_xRotation = Mathf.Clamp(m_xRotation, -90, 90);
        m_yRotation += m_mouseMovement.x * Time.smoothDeltaTime* m_mouseSensitivity;
        transform.rotation = Quaternion.Euler(m_xRotation, m_yRotation, 0);
        cam.transform.rotation = transform.rotation;
        m_player.transform.localRotation = Quaternion.Euler(0, m_yRotation, 0);
    }
}
