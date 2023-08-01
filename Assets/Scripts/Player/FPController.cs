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
    public LineRenderer m_lineRenderer;
    public ParticleSystem m_particles;
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
    public float m_damageChargePerTick = 0.1f;
    public GameObject projectile;
    Transform m_talisman;
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
        m_idle = new IdleState(m_animator, m_lineRenderer, m_particles);
        m_charging = new ChargingState(m_animator, m_lineRenderer, m_particles);
        m_firing = new FiringState(m_animator, m_lineRenderer, m_particles);
        m_inputControl.Player_Map.ManaAttack.performed += StartCharging;
        m_inputControl.Player_Map.ManaAttack.canceled += StartFiring;
        m_talismanState = m_idle;
    }

    void StartCharging(InputAction.CallbackContext t)
    {
        m_talismanState.StopState();
        m_talismanState = m_charging;
        m_talismanState.StartState(0);
    }

    void StartFiring(InputAction.CallbackContext t)
    {
        m_talismanState.StopState();
        m_talismanState = m_firing;
        m_talismanState.StartState(m_charging.chargeTime);
    }

    public void StartIdle()
    {
        m_talismanState.StopState();
        m_talismanState = m_idle;
        m_talismanState.StartState(0);
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
        m_talismanState.Update();
    }
}

class TalismanState
{
    public Animator m_animator;
    public LineRenderer m_lineRenderer;
    public ParticleSystem m_particles;
    public virtual void StartState(float startValue) { }
    public virtual void Update() { }
    public virtual void StopState() { }
}

class FiringState : TalismanState
{
    Camera m_camera;
    Vector3 m_destination;
    float m_damageTime;
    bool m_beam;
    float m_manaBoltSpeed = 30.0f;


    public FiringState(Animator anim, LineRenderer lr, ParticleSystem ps)
    {
        m_animator = anim;
        m_lineRenderer = lr;
        m_particles = ps;
        m_camera = Camera.main;
    }
    public override void StartState(float startValue)
    {
        m_lineRenderer.enabled = true;
        m_particles.Play();
        m_damageTime = startValue;
        m_beam = startValue >= 0.5f ? true : false;
    }

    public override void Update()
    {
        if (m_beam)
        {
            if (m_damageTime > 0)
            {
                Ray camRay = m_camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;

                if (Physics.Raycast(camRay, out hit))
                {
                    m_lineRenderer.SetPosition(1, m_lineRenderer.transform.InverseTransformPoint(hit.point));
                }
                else
                {
                    m_lineRenderer.SetPosition(1, m_lineRenderer.transform.InverseTransformPoint(camRay.GetPoint(50)));
                }
                m_damageTime -= Time.deltaTime;
            }
            else
            {
                MonoBehaviour.FindObjectOfType<FPController>().StartIdle();
            }
        }
        else 
        {
            Ray camRay = m_camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            Vector3 destination;
            if (Physics.Raycast(camRay, out hit))
            {
                destination = hit.point;                
            }
            else
            {
                destination = camRay.GetPoint(50);                
            }
            var manaBolt = MonoBehaviour.Instantiate(MonoBehaviour.FindObjectOfType<FPController>().projectile, m_particles.transform.position, Quaternion.identity) as GameObject;
            manaBolt.GetComponent<Rigidbody>().velocity = (destination - m_particles.transform.position).normalized * m_manaBoltSpeed;
                MonoBehaviour.FindObjectOfType<FPController>().StartIdle();
        }
    }
}
class ChargingState : TalismanState
{
    public float chargeTime = 0.0f;
    public ChargingState(Animator anim, LineRenderer lr, ParticleSystem ps)
    {
        m_animator = anim;
        m_lineRenderer = lr;
        m_particles = ps;
    }
    public override void StartState(float startValue)
    {
        chargeTime = 0.0f;
        m_particles.Play();
        m_lineRenderer.enabled = false;
    }

    public override void Update()
    {
        chargeTime += Time.deltaTime;
    }

    // StopState goes into the end of then charge anim
}
class IdleState : TalismanState
{
    public IdleState(Animator anim, LineRenderer lr, ParticleSystem ps)
    {
        m_animator = anim;
        m_lineRenderer = lr;
        m_particles = ps;
    }
    public override void StartState(float startValue)
    {
        m_particles.Stop();
        m_lineRenderer.enabled = false;
    }
}