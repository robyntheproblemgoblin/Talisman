using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    FPControls m_inputControl;
    #region Movement Fields
    CameraControls m_camera;
    public float m_cameraSensitivity = 10;
    public Vector3 m_moveDirection = Vector3.zero;
    float m_gravity = 9.81f;
    public float m_walkSpeed = 5;
    public float m_runSpeed = 8;
    bool m_canMove = true;
    public float m_jumpSpeed = 5;
    CharacterController m_characterController;
    public float m_smoothTime = 0.3f;
    public Vector2 MoveDampVelocity;
    // Coyote time
    // Jump Buffer
    #endregion

    #region Animation Fields
    Animator m_animator;
    #endregion

    #region Health Fields
    // Loss is in Chunks (Visually chunks)
    public float m_health;
    public bool m_canBeHit;
    public float m_hitBuffer;
    //List<string> m_enemiesHaveHit = new List<string>();
    Dictionary<string, float> m_enemiesHaveHit = new Dictionary<string, float>();
    #endregion

    #region Mana Fields
    // Comes in chunks from the pool (Visually ticks)
    #endregion

    #region Mana Attack Fields
    // Loss in Ticks until trigger release (Visually ticks)     

    RangedAttackState m_talismanState;
    ChargingState m_charging;
    FiringState m_firing;
    IdleState m_idle;

    public ParticleSystem m_fireMana;
    public ParticleSystem m_chargeMana;

    public float m_damageChargePerTick = 0.1f;
    public float m_maxSize = 2;
    public GameObject projectile;
    Transform m_talisman;
    #endregion

    #region Melee Attack Fields
    public float m_meleeAttackDistance;
    public Collider m_swordCollider;
    #endregion

    #region Block Parry Fields
    bool m_isBlocking = false;
    float m_blockPressedTime;
    public float m_parryTime;
    public float m_blockingReset;
    #endregion

    #region Unity Methods
    void Start()
    {
        m_camera = FindObjectOfType<CameraControls>();
        m_camera.SetupCamera(this.gameObject, m_cameraSensitivity);
        m_animator = GetComponentInChildren<Animator>();
        m_characterController = GetComponent<CharacterController>();
        m_inputControl = new FPControls();
        m_inputControl.Player_Map.Enable();
        m_idle = new IdleState(m_animator, m_chargeMana);
        m_charging = new ChargingState(m_animator, m_chargeMana);
        m_firing = new FiringState(m_animator, m_chargeMana, m_maxSize);
        m_talismanState = m_idle;
        m_inputControl.Player_Map.Heal.performed += StartHealing;
        //m_inputControl.Player_Map.Heal.canceled += StopHealing;
        m_inputControl.Player_Map.ManaAttack.performed += StartCharging;
        m_inputControl.Player_Map.ManaAttack.canceled += StartFiring;
        m_inputControl.Player_Map.MeleeAttack.performed += MeleeAttack;
        m_inputControl.Player_Map.SwapManaStyle.performed += SwapStyle;
        m_inputControl.Player_Map.Interact.performed += Interact;
        m_inputControl.Player_Map.BlockParry.performed += BlockParry;
    }

   

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == 8)
        {
            EnemyBT e = hit.gameObject.GetComponentInParent<EnemyBT>();
            if (!HitAlready(hit.gameObject.name))
            {
                SetData(hit.gameObject.name, 5);
                TakeDamage(e.m_damage);
            }
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
        UpdateDictionary();
    }
    #endregion

    #region Animation Methods
    void UpdateLeanAnimation(float speedX, float speedY)
    {
        Vector2 animSpeed = new Vector2(m_animator.GetFloat("Forward"), m_animator.GetFloat("Sideways"));
        animSpeed = Vector2.SmoothDamp(animSpeed, new Vector2(speedX, speedY), ref MoveDampVelocity, m_smoothTime);

        m_animator.SetFloat("Forward", animSpeed.x);
        m_animator.SetFloat("Sideways", animSpeed.y);
    }
    #endregion

    #region Health Methods
    public void TakeDamage(float damage)
    {
        m_health -= damage;
        Debug.Log(m_health);
    }

    void SetData(string key, float value)
    {
        m_enemiesHaveHit[key] = value;
    }

    bool HitAlready(string key)
    {
        if (m_enemiesHaveHit.ContainsKey(key))
        {
            return true;
        }
        return false;
    }

    void UpdateDictionary()
    {
        List<string> list = new List<string>();
        Dictionary<string, float> dict = new Dictionary<string, float>();
        foreach (KeyValuePair<string, float> kvp in m_enemiesHaveHit)
        {
            dict[kvp.Key] = kvp.Value - Time.deltaTime;
            if (dict[kvp.Key] <= 0)
            {
                list.Add(kvp.Key);
            }
        }
        m_enemiesHaveHit = dict;
        ClearData(list);
    }

    void ClearData(List<string> keys)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            ClearSingleData(keys[i]);
        }
    }

    void ClearSingleData(string key)
    {
        if (m_enemiesHaveHit.ContainsKey(key))
        {
            m_enemiesHaveHit.Remove(key);
        }
    }

    private void StartHealing(InputAction.CallbackContext obj)
    {
        m_talismanState.StopState();
        m_talismanState = m_charging;
        m_talismanState.StartState(0);
    }
    #endregion

    #region Mana Attack Methods
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

    void SwapStyle(InputAction.CallbackContext t)
    {
        m_charging.m_beam = !m_charging.m_beam;
        m_firing.m_beam = !m_firing.m_beam;
        if (m_charging.m_beam)
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
    #endregion

    #region Melee Attack Methods

    private void MeleeAttack(InputAction.CallbackContext obj)
    {
        int randomNumber = Random.Range(1, 4);
        m_swordCollider.enabled = true;
        Invoke("ResetCollider", 2.0f);
        m_animator.SetTrigger("Attack" + randomNumber);
    }

    void ResetCollider()
    {
        m_swordCollider.enabled = false;
    }
    #endregion

    #region Block Parry Methods
    private void BlockParry(InputAction.CallbackContext obj)
    {
        m_isBlocking = true;
        m_blockPressedTime = Time.realtimeSinceStartup;
        Invoke("StopBlock", m_blockingReset);
    }

    private void StopBlock()
    {
        if (m_isBlocking)
            m_isBlocking = false;
    }

    #endregion

    private void Interact(InputAction.CallbackContext obj)
    {
        Ray camRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(camRay, out hit, m_meleeAttackDistance))
        {
            Puzzle puzzle = hit.transform.gameObject.GetComponentInParent<Puzzle>();
            if (puzzle != null)
            {
                puzzle.PuzzleInteraction();
            }
        }
    }
}