using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using AISystem;
using AISystem.Contracts;
using Cysharp.Threading.Tasks;

public class PlayerController : MonoBehaviour, IBeing
{
    public Vector3 m_position => transform.position;
    public Vector3 m_forward => transform.forward;
    public Vector3 m_headPosition => transform.position;

    public GameManager m_game;
    public FPControls m_inputControl;
    #region Movement Fields
    public CameraControls m_camera;
    [Header("Camera and Movement"), Space(5)]
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
    public Animator m_animator;
    #endregion

    #region Health Fields    
    [Space(10), Header("Player Health"), Space(5)]
    public float m_health;
    [HideInInspector]
    public float m_currentHealth;
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
    int m_currentAttack = 1;
    public bool m_canAttack = true;
    public float m_attackTime = 22f;
    #endregion

    #region Block Parry Fields
    public bool m_isBlocking = false;  
            
    #endregion

    #region Interaction Fields
    [Space(10)]
    [Header("Interactions")]
    [Space(5)]
    public float m_interactionDistance;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        m_inputControl = new FPControls();
        m_game = GameManager.Instance;
    }
    void Start()
    {
        m_camera = FindObjectOfType<CameraControls>();
        m_camera.m_parentTransform = transform;
        m_characterController = GetComponent<CharacterController>();

        m_animator = GetComponentInChildren<Animator>();
        m_animator.rootRotation = transform.rotation;
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
        m_inputControl.Player_Map.BlockParry.canceled += StopBlockParry;

        m_inputControl.UI.Cancel.started += m_game.m_menuManager.Cancel;
        m_inputControl.Player_Map.Pause.started += PauseGame;

        m_currentHealth = m_health;
        m_currentMana = m_startMana;
        m_game.OnGameStateChanged += OnGameStateChanged;
        m_game.m_menuManager.UpdateHealth();
        m_game.m_menuManager.UpdateMana();

        m_game.m_aiManager.RegisterBeing(this);
    }

    private void PauseGame(InputAction.CallbackContext obj)
    {
        if(m_game.m_gameState == GameState.GAME)
        {
            m_game.UpdateGameState(GameState.PAUSE);
        }
        else if(m_game.m_gameState == GameState.PAUSE)
        {
            m_game.UpdateGameState(GameState.GAME);
        }
    }


    void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.TITLE:
                m_inputControl.UI.Enable();
                break;
            case GameState.MENU:
                m_inputControl.UI.Enable();
                break;
            case GameState.GAME:
                m_inputControl.UI.Disable();
                m_inputControl.Player_Map.Enable();
                break;
            case GameState.PAUSE:
                m_inputControl.Player_Map.Disable();
                m_inputControl.UI.Enable();
                break;
            case GameState.CINEMATIC:
                m_inputControl.Player_Map.Disable();
                break;
            case GameState.DEATH:
                m_inputControl.Player_Map.Disable();
                m_inputControl.UI.Enable();
                break;
            case GameState.CREDITS:
                m_inputControl.Player_Map.Disable();
                m_inputControl.UI.Enable();
                break;
        }
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.layer == (int)Mathf.Log(LayerMask.GetMask("Enemy"), 2))
        {
            Enemy e = hit.gameObject.GetComponentInParent<Enemy>();
            if (e != null && HitAlready(e.gameObject.name) == false && !m_isBlocking)
            {
                e.m_swordCollider.enabled = false;                
                TakeDamage(e.m_damage);
            }
            else if (e != null && HitAlready(e.gameObject.name) == false && m_isBlocking)
            {
                StopBlock();
                e.m_swordCollider.enabled = false;
                e.Interrupt();
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector2 move = m_inputControl.Player_Map.Movement.ReadValue<Vector2>().normalized;
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
        m_camera.MoveCamera(m_inputControl.Player_Map.Look.ReadValue<Vector2>(), 
            m_game.m_menuManager.m_currentController == ControllerType.KEYBOARD ? m_cameraSensitivity/50 : m_cameraSensitivity);
    }
    void Update()
    {
        m_talismanState.Update();
        UpdateInteracts();
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
        m_game.m_menuManager.UpdateHealth();
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

    void ClearData(List<string> keys)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            ClearSingleData(keys[i]);
        }
    }

    public void ClearSingleData(string key)
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
            m_game.m_menuManager.UpdateMana();
            m_game.m_menuManager.UpdateHealth();
        }
        else if (m_currentMana < 0)
        {
            m_currentMana = 0;
        }
    }

    #endregion

    #region Mana Methods
    public bool SpendMana(float mana)
    {
        if (m_currentMana <= 0 || m_currentMana - mana <= 0)
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
            m_game.m_menuManager.UpdateMana();
            return true;
        }
    }

    public void AddMana(float mana)
    {
        m_currentMana += mana;
        if (m_currentMana > m_maxMana)
        {
            m_currentMana = m_maxMana;
        }
        m_game.m_menuManager.UpdateMana();
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
        m_talismanState.StartState(m_charging.m_chargeTime * m_projectileDamage);
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
        if (m_canAttack)
        {
            m_canAttack = false;
            m_swordCollider.enabled = true;
            ResetAttack().Forget();
            m_animator.SetTrigger("Attack" + m_currentAttack);
            m_currentAttack++;
            if (m_currentAttack == 4)
            {
                m_currentAttack = 1;
            }
        }
    }

    async UniTask ResetAttack()
    {
        float time = Time.time;
        while (Time.time < time + m_attackTime)
        {
            await UniTask.Yield();
        }
        m_canAttack = true;
    }

    public void HitReticle()
    {
        ResetCollider();
        m_game.m_menuManager.HitReticle().Forget();
    }

    public void ResetCollider()
    {
        m_swordCollider.enabled = false;
    }
    #endregion

    #region Block Parry Methods
    private void BlockParry(InputAction.CallbackContext obj)
    {
        m_isBlocking = true;
        m_animator.SetTrigger("Block");
    }
    private void StopBlockParry(InputAction.CallbackContext obj)
    {
        StopBlock();
    }

    private void StopBlock()
    {
        if (m_isBlocking)
        {
            m_isBlocking = false;
            m_animator.SetTrigger("StopBlock");
        }
    }

    #endregion

    #region Interaction Methods
    private void UpdateInteracts()
    {
        Ray camRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(camRay, out hit, m_interactionDistance))
        {
            Puzzle puzzle = hit.transform.gameObject.GetComponentInParent<Puzzle>();
            Interactable interactable = hit.transform.gameObject.GetComponentInParent<Interactable>();
            ManaPool manaPool = hit.transform.gameObject.GetComponentInParent<ManaPool>();
            if (puzzle != null || interactable != null || manaPool != null)
            {
                m_game.m_menuManager.SetInteract(hit);
            }
            else
            {
                m_game.m_menuManager.StopInteract();
            }
        }
        else
        {
            m_game.m_menuManager.StopInteract();
        }
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        Ray camRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(camRay, out hit, m_interactionDistance))
        {
            Puzzle puzzle = hit.transform.gameObject.GetComponentInParent<Puzzle>();
            Interactable interactable = hit.transform.gameObject.GetComponentInParent<Interactable>();
            ManaPool manaPool = hit.transform.gameObject.GetComponentInParent<ManaPool>();
            if (puzzle != null)
            {
                puzzle.RotatePuzzle();
            }
            else if (interactable != null)
            {
                m_game.CinematicTrigger(interactable);
            }
            else if (manaPool != null)
            {
                manaPool.Interact(this);
            }
        }
    }

    public void FinishCinematic()
    {
        m_swordCollider.gameObject.transform.localPosition = new Vector3(0, 0.001446927f, 0);
        m_game.UpdateGameState(GameState.GAME);
    }
    #endregion
}