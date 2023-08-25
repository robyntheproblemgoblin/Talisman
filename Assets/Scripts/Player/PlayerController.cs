using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    FPControls m_inputControl;
    #region Movement Fields
    CameraControls m_camera;
    [Header("Camera and Movement")]
    [Space(5)]
    public float m_cameraSensitivity = 10;
    Vector3 m_moveDirection = Vector3.zero;
    float m_gravity = 9.81f;
    public float m_walkSpeed = 5;
    public float m_runSpeed = 8;
    bool m_canMove = true;
    public float m_jumpSpeed = 5;
    CharacterController m_characterController;
    float m_smoothTime = 0.3f;
    Vector2 MoveDampVelocity;
    // Coyote time
    // Jump Buffer
    #endregion

    #region Animation Fields
    Animator m_animator;
    #endregion

    #region Health Fields    
    [Space(10)]
    [Header("Player Health")]
    [Space(5)]
    public float m_health;    
    float m_currentHealth;
    public float m_healRate;
    public ParticleSystem m_healParticles;
    HealingState m_healing; 
    Dictionary<string, float> m_enemiesHaveHit = new Dictionary<string, float>();
    #endregion

    #region Mana Fields    
    [Space(10)]
    [Header("Player Mana")]
    [Space(5)]
    public float m_maxMana;
    public float m_startMana;
    public float m_manaHealCost;    
    public float m_currentMana;
    #endregion

    #region Mana Attack Fields
    LeftHandState m_talismanState;
    ChargingState m_charging;
    FiringState m_firing;
    IdleState m_idle;

    [Space(10)]
    [Header("Mana Attacks")]
    [Space(5)]
    public ParticleSystem m_fireMana;
    public ParticleSystem m_projectileMana;
    public GameObject projectile;
    public float m_flameDamage = 0.1f;
    public float m_flameCost = 1;
    public float m_minProjectileCost = 1;
    public float m_projectileManaCost = 1;
    public float m_projectileDamage = 1;
    public Transform m_talisman;
    #endregion

    #region Melee Attack Fields
    [Space(10)]
    [Header("Melee Attack")]
    [Space(5)]
    public Collider m_swordCollider;
    public float m_meleeDamage;
    #endregion

    #region Block Parry Fields
    bool m_isBlocking = false;
    float m_blockPressedTime;
    [Space(10)]
    [Header("Blocking and Parrying")]
    [Space(5)]
    public float m_parryTime;
    public float m_blockingResetTime;
    #endregion

    #region Interaction Fields
    [Space(10)]
    [Header("Interactions")]
    [Space(5)]
    public float m_interactionDistance;
    #endregion

    #region Unity Methods
    void Start()
    {
        m_camera = FindObjectOfType<CameraControls>();
        m_camera.SetupCamera(this.gameObject, m_cameraSensitivity);
        m_characterController = GetComponent<CharacterController>();
        m_inputControl = new FPControls();
        m_inputControl.Player_Map.Enable();

        m_animator = GetComponentInChildren<Animator>();
        m_idle = new IdleState(m_animator, m_projectileMana);
        m_charging = new ChargingState(m_animator, m_projectileMana, this);
        m_firing = new FiringState(m_animator, m_projectileMana, m_talisman);
        m_healing = new HealingState(m_animator, m_healParticles, this);
        m_talismanState = m_idle;

        m_inputControl.Player_Map.Heal.performed += StartHealing;
        m_inputControl.Player_Map.Heal.canceled += StopHealing;
        m_inputControl.Player_Map.ManaAttack.performed += StartCharging;
        m_inputControl.Player_Map.ManaAttack.canceled += StartFiring;
        m_inputControl.Player_Map.MeleeAttack.performed += MeleeAttack;
        m_inputControl.Player_Map.SwapManaStyle.performed += SwapStyle;
        m_inputControl.Player_Map.Interact.performed += Interact;
        m_inputControl.Player_Map.BlockParry.performed += BlockParry;

        m_currentHealth = m_health;
        m_currentMana = m_startMana;
    }   

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == 8)
        {
            EnemyBT e = hit.gameObject.GetComponentInParent<EnemyBT>();
            if (!HitAlready(hit.gameObject.name))
            {
                RegisterEnemyHit(hit.gameObject.name, 5);
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
        m_currentHealth -= damage;
        Debug.Log(m_currentHealth);
    }

    void RegisterEnemyHit(string key, float value)
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
        m_talismanState = m_healing;
        m_talismanState.StartState(0);
    }
    private void StopHealing(InputAction.CallbackContext obj)
    {
        m_talismanState.StopState();
        m_talismanState = m_idle;
        m_talismanState.StartState(0);
    }

    public void Heal()
    {
        if (m_currentHealth > m_health)
        {
            m_currentHealth = m_health;
        }
        else if (m_currentHealth < m_health && m_currentMana > 0)
        {
            m_currentMana -= m_manaHealCost * Time.deltaTime;
            m_currentHealth += m_healRate * Time.deltaTime;
        }
        else if(m_currentMana < 0)
        {
            m_currentMana = 0;
        }
    }

    #endregion

    #region Mana Methods
    public bool SpendMana(float mana)
    {
        if(m_currentMana <= 0 || m_currentMana - mana <= 0)
        {
            m_currentMana = 0;
            return false;
        }
        else
        {
            m_currentMana -= mana;
            if (m_currentMana <= 0)
            {
                m_currentMana = 0;
            }
            return true;
        }
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
        m_talismanState.StartState(m_charging.m_chargeTime);
    }

    public void StartIdle()
    {
        m_talismanState.StopState();
        m_talismanState = m_idle;
        m_talismanState.StartState(0);
    }

    void SwapStyle(InputAction.CallbackContext t)
    {
        m_charging.m_isProjectile = !m_charging.m_isProjectile;
        m_firing.m_isProjectile = !m_firing.m_isProjectile;
        if (m_charging.m_isProjectile)
        {
            m_charging.m_particles = m_projectileMana;
            m_firing.m_particles = m_projectileMana;
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
        Invoke("StopBlock", m_blockingResetTime);
    }

    private void StopBlock()
    {
        if (m_isBlocking)
            m_isBlocking = false;
    }

    #endregion

    #region Interaction Methods
    private void Interact(InputAction.CallbackContext obj)
    {
        Ray camRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(camRay, out hit, m_interactionDistance))
        {
            Puzzle puzzle = hit.transform.gameObject.GetComponentInParent<Puzzle>();
            if (puzzle != null)
            {
                puzzle.PuzzleInteraction();
            }
        }
    }
    #endregion
}