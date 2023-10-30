using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using AISystem;
using AISystem.Contracts;
using Cysharp.Threading.Tasks;
using System;
using FMODUnity;

public class PlayerController : MonoBehaviour, IBeing
{
    public Vector3 m_position => transform.position;
    public Vector3 m_forward => transform.forward;
    public Vector3 m_headPosition => transform.position;

    public GameManager m_game;
    public FPControls m_inputControl;

    public Enemy m_enemy;
    #region Movement Fields
    public CameraControls m_camera;
    [Header("Camera and Movement"), Space(5)]
    public float m_cameraSensitivity = 10;
    Vector3 m_moveDirection = Vector3.zero;
    float m_gravity = 9.81f;
    public float m_gravityMultiplier = 1f;
    public float m_walkSpeed = 5;
    public float m_runSpeed = 8;
    bool m_canMove = true;
    public float m_jumpSpeed = 5;
    CharacterController m_characterController;
    float m_smoothTime = 0.3f;
    Vector2 MoveDampVelocity;
    public SkinnedMeshRenderer m_skinnedMeshRenderer;
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
    LeftHandState m_talismanState;    
    IdleState m_idle;
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
    public float m_attackTime;
    #endregion

    #region Block Parry Fields
    public bool m_isBlocking = false;

    #endregion

    #region Interaction Fields
    [Space(10)]
    [Header("Interactions")]
    [Space(5)]
    public float m_interactionDistance;
    [HideInInspector]
    public bool m_stopInteracts = false;
    #endregion



    #region SFX
    [Space(10), Header("Sound Effects"), Space(5)]
    public FMODUnity.EventReference m_groundedSFX;
    bool m_jumped = false;
    public FMODUnity.EventReference m_weaponHit;
    public List<FMODUnity.EventReference> m_playerGrunts;
    public FMODUnity.EventReference m_healingSound;
    FMOD.Studio.EventInstance m_healingInstance;
    public FMODUnity.EventReference m_attackSwing;
    public FMODUnity.EventReference m_blockedSound;
    public FMODUnity.EventReference m_deathSound;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        m_inputControl = new FPControls();
        m_game = GameManager.Instance;
        m_skinnedMeshRenderer.enabled = false;
    }
    void Start()
    {
        m_camera = FindObjectOfType<CameraControls>();
        m_camera.m_parentTransform = transform;
        m_characterController = GetComponent<CharacterController>();

        m_animator = GetComponentInChildren<Animator>();
        m_animator.rootRotation = transform.rotation;
        m_idle = new IdleState(m_animator, m_healParticles);
        m_healing = new HealingState(m_animator, m_healParticles, this);
        m_talismanState = m_idle;

        m_inputControl.Player_Map.Heal.performed += StartHealing;
        m_inputControl.Player_Map.Heal.canceled += StopHealing;
        m_inputControl.Player_Map.MeleeAttack.performed += MeleeAttack;
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

    private void EnemyStart(InputAction.CallbackContext obj)
    {
        m_enemy.SetStatue(false);
        m_skinnedMeshRenderer.enabled = (true);
    }

    private void PauseGame(InputAction.CallbackContext obj)
    {
        if (m_game.m_gameState == GameState.GAME)
        {
            m_game.UpdateGameState(GameState.PAUSE);
        }
        else if (m_game.m_gameState == GameState.PAUSE)
        {
            m_game.UpdateGameState(GameState.GAME);
        }
    }


    void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.CONTROLS:
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
                m_game.m_audioManager.PlayOneShot(m_weaponHit, hit.ClosestPoint(transform.position));
                e.m_swordCollider.enabled = false;
                TakeDamage(e.m_damage);
            }
            else if (e != null && HitAlready(e.gameObject.name) == false && m_isBlocking)
            {
                m_game.m_audioManager.PlayOneShot(m_blockedSound, gameObject.transform.position);
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
        bool wasJumping = m_characterController.isGrounded;

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
            m_moveDirection.y -= (m_gravity * m_gravityMultiplier) * Time.deltaTime;            
        }

        if(m_characterController.isGrounded && wasJumping)
        {
            m_game.m_audioManager.PlayOneShot(m_groundedSFX, transform.position - Vector3.down);            
        }

        UpdateLeanAnimation(speedX, speedY);
        m_characterController.Move(m_moveDirection * Time.deltaTime);
    }
    void LateUpdate()
    {
        m_camera.MoveCamera(m_inputControl.Player_Map.Look.ReadValue<Vector2>(),
        m_game.m_menuManager.m_currentController == ControllerType.KEYBOARD ? m_cameraSensitivity / 10 : m_cameraSensitivity);

    }
    void Update()
    {
        m_talismanState.Update();
        UpdateInteracts();
    }

    public void ChangeSensitivity(float change)
    {
        m_cameraSensitivity = change;
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
        if (m_skinnedMeshRenderer.enabled == false)
        {
            return;
        }
        m_talismanState.StopState();
        m_healingInstance = RuntimeManager.CreateInstance(m_healingSound);
        RuntimeManager.AttachInstanceToGameObject(m_healingInstance, m_game.m_player.gameObject.transform);
        m_healingInstance.start();
        m_healingInstance.release();
        m_talismanState = m_healing;
        m_talismanState.StartState(0);
    }
    private void StopHealing(InputAction.CallbackContext obj)
    {
        if (m_skinnedMeshRenderer.enabled == false)
        {
            return;
        }
        m_talismanState.StopState();
        m_healingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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

    #region Melee Attack Methods

    private void MeleeAttack(InputAction.CallbackContext obj)
    {
        if (m_skinnedMeshRenderer.enabled == false)
        {
            return;
        }
        if (m_canAttack)
        {
            m_canAttack = false;
            m_swordCollider.enabled = true;
            ResetAttack().Forget();
            m_animator.SetTrigger("Attack" + m_currentAttack);
            m_game.m_audioManager.PlayOneShot(m_attackSwing, m_position);
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
        if(m_skinnedMeshRenderer.enabled == false)
        {
            return;
        }
        m_isBlocking = true;
        m_animator.SetTrigger("Block");
    }
    private void StopBlockParry(InputAction.CallbackContext obj)
    {
        if (m_skinnedMeshRenderer.enabled == false)
        {
            return;
        }
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
        if (m_stopInteracts)
        {            
            return;
        }
        Ray camRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(camRay, out hit, m_interactionDistance))
        {
            Puzzle puzzle = hit.transform.gameObject.GetComponentInParent<Puzzle>();
            Interactable interactable = hit.transform.gameObject.GetComponentInParent<Interactable>();
            ManaPool manaPool = hit.transform.gameObject.GetComponentInParent<ManaPool>();
            if ((puzzle != null && puzzle.CanInteract() == true) || interactable != null || (manaPool != null && manaPool.IsActive()))
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
        if (m_stopInteracts)
        {
            return;
        }
        Ray camRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(camRay, out hit, m_interactionDistance))
        {
            Puzzle puzzle = hit.transform.gameObject.GetComponentInParent<Puzzle>();
            Interactable interactable = hit.transform.gameObject.GetComponentInParent<Interactable>();
            ManaPool manaPool = hit.transform.gameObject.GetComponentInParent<ManaPool>();
            if (puzzle != null)
            {
                int random = UnityEngine.Random.Range(0, m_playerGrunts.Count - 1);
                m_game.m_audioManager.PlayOneShot(m_playerGrunts[random], gameObject.transform.position);
                puzzle.RotatePuzzle();
            }
            else if (interactable != null)
            {
                m_game.FirstCinematic().Forget();
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