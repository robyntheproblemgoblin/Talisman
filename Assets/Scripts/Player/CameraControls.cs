using UnityEngine;

public class CameraControls : MonoBehaviour
{    
    float m_xRotation;
    float m_yRotation;

    private void Start()
    {
        m_xRotation = transform.rotation.eulerAngles.x; 
        m_yRotation = transform.rotation.eulerAngles.y;
    }

    public void MoveCamera(Vector2 mouseMove, float sensitivity)
    {           
        m_xRotation -= mouseMove.y * Time.deltaTime * sensitivity;
        m_xRotation = Mathf.Clamp(m_xRotation, -90, 90);
        m_yRotation += mouseMove.x * Time.deltaTime * sensitivity;
        transform.rotation = Quaternion.Euler(m_xRotation, m_yRotation, 0);   
    }
}
