using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    public Camera beautyCorner;
    public Camera playerCamera;
    public Light m_light;

    FPControls m_inputControl;
    #region Movement
    CameraControls m_camera;
    public float m_cameraSensitivity = 10;
    public Vector3 m_moveDirection = Vector3.zero;
    float m_gravity = 9.81f;
    public float m_walkSpeed = 5;
    public float m_runSpeed = 5;
    bool m_canMove = true;
    public float m_jumpSpeed = 5;
    CharacterController m_characterController;
    public float m_smoothTime = 0.3f;
    public Vector2 MoveDampVelocity;
    // Coyote time
    // Jump Buffer
    #endregion

    #region Animation
    Animator m_animator;
    #endregion

    #region Health
    // Loss is in Chunks (Visually chunks)
    public float m_health;
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

    public ParticleSystem m_fireMana;
    public ParticleSystem m_chargeMana;

    // Loss in Ticks until trigger release (Visually ticks)    
    public float m_damageChargePerTick = 0.1f;
    public float m_maxSize = 2;
    public GameObject projectile;
    Transform m_talisman;
    #endregion

    #region MeleeAttack
    public float m_meleeAttackDistance;
    #endregion

    #region Block
    #endregion

    #region Parry
    #endregion

    void Start()
    {
        m_camera = FindObjectOfType<CameraControls>();
        m_camera.SetupCamera(this.gameObject, m_cameraSensitivity);
        m_animator = GetComponentInChildren<Animator>();
        m_inputControl = new FPControls();
        m_inputControl.Player_Map.Enable();
        m_characterController = GetComponent<CharacterController>();
        m_idle = new IdleState(m_animator, m_chargeMana);
        m_charging = new ChargingState(m_animator, m_chargeMana);
        m_firing = new FiringState(m_animator, m_chargeMana, m_maxSize);
        m_inputControl.Player_Map.ManaAttack.performed += StartCharging;
        m_inputControl.Player_Map.ManaAttack.canceled += StartFiring;
        m_inputControl.Player_Map.MeleeAttack.performed += MeleeAttack;
        m_inputControl.Player_Map.SwapManaStyle.performed += SwapStyle;
        m_talismanState = m_idle;
        m_inputControl.Player_Map.Swap.performed += SwapScenes;
        m_inputControl.Player_Map.Swap1.performed += SwapLights;
        m_inputControl.Player_Map.Interact.performed += CheckPuzzle;
    }
    private void MeleeAttack(InputAction.CallbackContext obj)
    {
        int randomNumber = Random.Range(1, 4);
        Ray camRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(camRay, out hit, m_meleeAttackDistance))
        {
            EnemyBT enemy = hit.transform.gameObject.GetComponent<EnemyBT>();
            if (enemy != null)
            {
                Debug.Log("Melee hit");
                enemy.TakeHit();
            }
        }

        
        m_animator.SetTrigger("Attack" + randomNumber);
    }

    private void CheckPuzzle(InputAction.CallbackContext obj)
    {
        int randomNumber = Random.Range(1, 4);
        Ray camRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(camRay, out hit, m_meleeAttackDistance))
        {
            Puzzle puzzle = hit.transform.gameObject.GetComponentInParent<Puzzle>();
            if (puzzle != null)
            {
                Debug.Log("Puzzle Turn");
                puzzle.RotatePuzzle();
            }
        }


    }

    void StartCharging(InputAction.CallbackContext t)
    {
        m_talismanState.StopState();
        m_talismanState = m_charging;
        m_talismanState.StartState(0);
    }
    void SwapScenes(InputAction.CallbackContext t)
    {
        playerCamera.gameObject.SetActive(!playerCamera.gameObject.activeSelf);
        beautyCorner.gameObject.SetActive(!beautyCorner.gameObject.activeSelf);
        m_light.gameObject.SetActive(!m_light.gameObject.activeSelf);
    
    }
    void SwapLights(InputAction.CallbackContext t)
    {        
        m_light.gameObject.SetActive(!m_light.gameObject.activeSelf);       
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

    void SwapStyle(InputAction.CallbackContext t)
    {
        m_charging.m_beam = !m_charging.m_beam;
        m_firing.m_beam = !m_firing.m_beam;
        if(m_charging.m_beam)
        {
            m_charging.m_particles = m_chargeMana;
            m_firing.m_particles = m_chargeMana;
        }
        else
        {
            m_charging.m_particles = m_fireMana;
            m_firing.m_particles = m_fireMana;
        }
    }

    void FixedUpdate()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector2 move = m_inputControl.Player_Map.Movement.ReadValue<Vector2>();
        bool isRunning = m_inputControl.Player_Map.Sprint.IsInProgress();
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

        UpdateLeanAnimation(speedX, speedY);
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

    void UpdateLeanAnimation(float speedX, float speedY)
    {
        Vector2 animSpeed = new Vector2(m_animator.GetFloat("Forward"), m_animator.GetFloat("Sideways"));
        animSpeed = Vector2.SmoothDamp(animSpeed, new Vector2(speedX, speedY), ref MoveDampVelocity, m_smoothTime);

        m_animator.SetFloat("Forward", animSpeed.x);
        m_animator.SetFloat("Sideways", animSpeed.y);
    }
}

class TalismanState
{
    public Animator m_animator;
    public ParticleSystem m_particles;
    public virtual void StartState(float startValue) { }
    public virtual void Update() { }
    public virtual void StopState() { }
}

class FiringState : TalismanState
{
    Camera m_camera;
    float m_damageTime;
    public bool m_beam = true;
    float m_manaBoltSpeed = 30.0f;
    float m_maxSize = 2;
    float m_minSize = 1;


    public FiringState(Animator anim, ParticleSystem ps, float max)
    {
        m_animator = anim;
        m_particles = ps;
        m_camera = Camera.main;
        m_maxSize = max;
    }
    public override void StartState(float startValue)
    {
        if (m_beam)
            m_particles.Play();
        else
            m_particles.Stop();
        m_damageTime = startValue;
    }

    public override void Update()
    {
        if (m_beam)
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
            var manaBolt = MonoBehaviour.Instantiate(MonoBehaviour.FindObjectOfType<PlayerController>().projectile, m_particles.transform.position + m_particles.transform.forward, Quaternion.identity) as GameObject;
            if (m_damageTime > m_minSize)
            {
                manaBolt.GetComponent<SphereCollider>().radius *= (m_damageTime <= m_maxSize ? m_damageTime : m_maxSize);
                var main = manaBolt.GetComponentInChildren<ParticleSystem>().main;
                main.startSize = m_damageTime;
            }
            manaBolt.GetComponent<Rigidbody>().velocity = (destination - m_particles.transform.position).normalized * m_manaBoltSpeed;
        }
        MonoBehaviour.FindObjectOfType<PlayerController>().StartIdle();
    }
    public override void StopState()
    {
        m_animator.SetBool("LeftAttacking", false);
    }
}
class ChargingState : TalismanState
{
    public bool m_beam = true;
    public float chargeTime = 0.0f;
    Camera m_camera;
    public ChargingState(Animator anim, ParticleSystem ps)
    {
        m_animator = anim;
        m_particles = ps;
        m_camera = Camera.main;
    }
    public override void StartState(float startValue)
    {
        chargeTime = 0.0f;
        m_animator.SetBool("LeftAttacking", true);
        m_particles.Play();
    }

    public override void Update()
    {
        if (m_beam)
        {
            chargeTime += Time.deltaTime;
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

        }
    }

    // StopState goes into the end of then charge anim?
}
class IdleState : TalismanState
{
    public IdleState(Animator anim, ParticleSystem ps)
    {
        m_animator = anim;
        m_particles = ps;
    }
    public override void StartState(float startValue)
    {
        m_particles.Stop();
    }
}