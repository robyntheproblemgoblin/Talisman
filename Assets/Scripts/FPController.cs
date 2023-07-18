using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPController : MonoBehaviour
{
    public GameObject m_ground;

    CameraControls m_camera;
    public float m_cameraSensitivity = 10;
    FPControls m_inputControl;
    Vector3 m_moveDirection = Vector3.zero;
    float m_gravity = 9.81f;
    public float m_walkSpeed = 5;
    public float m_runSpeed = 5;
    bool m_canMove = true;
    public float m_jumpSpeed = 5;
    CharacterController m_characterController;

    void Start()
    {
        m_camera = FindObjectOfType<CameraControls>();
        m_camera.SetupCamera(this.gameObject, m_cameraSensitivity);
        m_inputControl = new FPControls();
        m_inputControl.Player_Map.Enable();
        m_characterController = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector2 move = m_inputControl.Player_Map.Movement.ReadValue<Vector2>();
        bool isRunning = m_inputControl.Player_Map.Sprint.enabled;
        float speedX = m_canMove ? (isRunning ? m_runSpeed : m_walkSpeed) * move.y : 0;
        float speedY = m_canMove ? (isRunning ? m_runSpeed : m_walkSpeed) * move.x : 0;
        float yDirection = m_moveDirection.y;
        m_moveDirection = (forward * speedX) + (right * speedY);

        if (m_inputControl.Player_Map.Jump.IsPressed() && m_characterController.isGrounded)
        {
            m_moveDirection.y = m_jumpSpeed;
        }
        else
        {
            m_moveDirection.y = yDirection;
        }

        if (!m_characterController.isGrounded)
        {
            m_moveDirection.y -= m_gravity * Time.deltaTime;
        }
        m_characterController.Move(m_moveDirection * Time.deltaTime);

    }

    void LateUpdate()
    {
        m_camera.MoveCamera(m_inputControl.Player_Map.Look.ReadValue<Vector2>());
    }

}
