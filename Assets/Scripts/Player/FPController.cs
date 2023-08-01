using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPController : MonoBehaviour
{   
    FPControls m_inputControl;

    #region Movement
    CameraControls m_camera;
    public float m_cameraSensitivity = 10;
    Vector3 m_moveDirection = Vector3.zero;
    float m_gravity = 9.81f;
    public float m_walkSpeed = 5;
    public float m_runSpeed = 5;
    bool m_canMove = true;
    public float m_jumpSpeed = 5;
    CharacterController m_characterController;
    #endregion

    #region Animation
    Animator m_animator;
    #endregion

    #region Health
    // Loss is in Chunks (Visually chunks)
    #endregion

    #region Heal
    // Tick while button held (Visually Ticks)
    #endregion

    #region Mana
    // Comes in chunks from the pool (Visually ticks)
    #endregion

    #region ManaAttack

    TalismanState m_talismanState;
    ChargingState m_charging;
    FiringState m_firing;
    IdleState m_idle;

    // Loss in Ticks until trigger release (Visually ticks)
    public bool m_chargeType = true;
    bool m_manaAttackCharging = false;
    public float m_damageChargePerTick = 0.1f;
    #endregion

    #region MeleeAttack
    #endregion

    #region Block
    #endregion

    #region Parry
    #endregion

    void Start()
    {
        m_camera = FindObjectOfType<CameraControls>();
        m_camera.SetupCamera(this.gameObject, m_cameraSensitivity);
        m_inputControl = new FPControls();
        m_inputControl.Player_Map.Enable();
        m_characterController = GetComponent<CharacterController>();
        m_talismanState = m_idle;
        m_inputControl.Player_Map.ManaAttack.performed += StartCharging;                
        m_inputControl.Player_Map.ManaAttack.canceled += _ => m_talismanState = m_firing;
    }
    
    void StartCharging(InputAction.CallbackContext t)
    {
        m_talismanState = m_charging;
        m_talismanState.StartState();
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
    
      void Update()
    {
      
    }
}

enum TalismanStateEnum
{
    STATE_FIRING,
    STATE_CHARGING,
    STATE_IDLE
}

abstract class TalismanState
{
    abstract public void StartState();
    abstract public void Update();
    abstract public void StopState();
}

class FiringState : TalismanState
{
    public override void StartState()
    {

    }

    public override void Update()
    {

    }
    public override void StopState()
    {

    }
}
class ChargingState : TalismanState
{
        float finalDamage = 0.0f;
    public override void StartState()
    {
        //Start the animation
        finalDamage = 0.0f;
    }

    public override void Update()
    {

    }

    public override void StopState()
    {

    }
}
class IdleState : TalismanState
{
    public override void StartState()
    {

    }

    public override void Update()
    {

    }

    public override void StopState()
    {

    }
}