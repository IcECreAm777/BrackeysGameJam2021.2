using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    // exposed variables
    [Header("Game Properties")]
    [SerializeField]
    private int numOfBowls = 5;
    [SerializeField] 
    private int gameTime = 120;
    
    [Header("Boomerang")]
    [SerializeField]
    private GameObject boomerang;
    [SerializeField]
    private GameObject boomerangSpawn;
    [SerializeField]
    private GameObject dummy;

    [Header("Slicing")]
    [SerializeField]
    private GameObject sliceCollider;
    [SerializeField]
    private float attackWindow = 0.0f;
    [SerializeField]
    private GameObject knife;

    [Header("Bowl")]
    [SerializeField]
    private BowlBehaviour bowl;

    [Header("Movement")]
    [SerializeField]
    private float speed = 7.0f;
    [SerializeField] 
    private float sprintMultiplier = 1.35f;
    [SerializeField]
    private int maxStamina = 100;
    [SerializeField]
    private float staminaConsumption = 5.0f;
    [SerializeField]
    private float staminaRegenerationRate = 3.7f;
    [SerializeField]
    private float sprintCooldown = 1.0f;
    
    [Header("Cool downs")]
    [SerializeField]
    private float boomerangCooldown = 5.0f;
    [SerializeField]
    private float swordCooldown = 0.2f;
    [SerializeField]
    private float bowlCooldown = 10.0f;

    [Header("Animations")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AnimationClip spinningAnimation;

    [Header("UI")] 
    [SerializeField] 
    private PlayerUi playerUi;
    [SerializeField]
    private PostGameUI postGameUI;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip countDownLow;
    [SerializeField]
    private AudioClip countDownHigh;
    
    [Header("Input")]
    [SerializeField]
    private InputAction movingInputAction;
    [SerializeField]
    private InputAction sprintInputAction;
    [SerializeField]
    private InputAction shootInputAction;
    [SerializeField]
    private InputAction sliceInputAction;
    [SerializeField]
    private InputAction bowlInputAction;
    [SerializeField]
    private InputAction mousePos;
    [SerializeField]
    private InputAction pause;

    // other components
    private CharacterController _characterController;
    private Camera _cam;
    private AudioSource _audio;

    // private properties
    private Vector2 _velocity = new Vector2();
    private bool _substractStamina = false;
    private float _stamina;
    private bool _canSprint = true;
    private Vector2 _mousePos = new Vector2();

    // Shooting
    private bool _canShoot = true;
    private bool _canSlice = true;
    private bool _canBowl = true;

    private bool _isBoomerangAvailable = true;
    private bool _isSlicing = false;
    private int _numBowls = 0;

    // bowling
    private List<CollectableFruitScriptableObject>[] _collectedBowls;
    
    // Animation Control
    private Animation _animation;
    private static readonly int Throw = Animator.StringToHash("Throw");
    private static readonly int Bowl = Animator.StringToHash("Bowl");
    private static readonly int Catch = Animator.StringToHash("Catch");
    private static readonly int Forwards = Animator.StringToHash("Forwards");
    private static readonly int Sidewards = Animator.StringToHash("Sidewards");
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Swing = Animator.StringToHash("Swing");


    // ENGINE FUNCTIONS 
    
    void Awake()
    {
        // init default values
        _stamina = maxStamina;
        _collectedBowls = new List<CollectableFruitScriptableObject>[numOfBowls];
        for (var i = 0; i < numOfBowls; i++)
        {
            _collectedBowls[i] = new List<CollectableFruitScriptableObject>();
        }

        // get other components
        _characterController = GetComponent<CharacterController>();
        _cam = Camera.main;
        _audio = GetComponent<AudioSource>();
        
        // set up animations (for everything but the character model)
        _animation = GetComponent<Animation>();
        _animation.AddClip(spinningAnimation, "spin");

        // initialize input maps - They'll be enabled, when the start routine is over
        movingInputAction.performed += context => { _velocity = context.ReadValue<Vector2>(); };
        movingInputAction.canceled += context => { _velocity = Vector2.zero; };
        shootInputAction.performed += context => { StartCoroutine(OnShoot()); };
        sliceInputAction.performed += context => { _isSlicing = true; };
        sliceInputAction.canceled += context => { _isSlicing = false; };
        sprintInputAction.performed += context => { _substractStamina = true; };
        sprintInputAction.canceled += context => { _substractStamina = false; };
        bowlInputAction.performed += OnBowl;
        mousePos.performed += context => { _mousePos = context.ReadValue<Vector2>(); };
        pause.performed += context => { playerUi.PauseGame(); };
    }

    private void Start()
    {
#if DEBUG
        if (playerUi == null)
        {
            Debug.LogError("The UI controller is not assigned for the player");
        }

        if (boomerangSpawn == null)
        {
            Debug.LogError("There was no game Object assigned as the boomerang spawn");
        }

        if (sliceCollider == null)
        {
            Debug.LogError("No Collider as slice hit box assigned for the player");
        }
#endif
        
        StartCoroutine(GameStartRoutine());
        playerUi.Initialize(boomerangCooldown, swordCooldown, bowlCooldown, maxStamina);
        mousePos.Enable();
    }

    private void FixedUpdate()
    {
        // Movement
        var normalizedDirection = _velocity.normalized;
        var currentSpeed = speed;

        if (_canSprint && _substractStamina)
        {
            currentSpeed *= sprintMultiplier;
            _stamina -= Time.deltaTime * staminaConsumption;
            if (_stamina <= 0.0f)
            {
                StartCoroutine(SprintCooldown());
                _substractStamina = false;
            }
        }
        else
        {
            _stamina += Time.deltaTime * staminaRegenerationRate;
            _stamina = _stamina >= maxStamina ? maxStamina : _stamina;
        }
        
        playerUi.UpdateStamina(_stamina);
        
        // Animation State
        animator.SetBool(IsWalking, _velocity != Vector2.zero);
        if (_velocity != Vector2.zero)
        {
            var forward = transform.forward;
            var forward2D = new Vector2(forward.x, forward.z);
            var angle = Vector2.Angle(forward2D, normalizedDirection);
            var sin = Mathf.Sin(Mathf.Deg2Rad * angle);
            var cos = Mathf.Cos(Mathf.Deg2Rad * angle);
            animator.SetFloat(Forwards, sin);
            animator.SetFloat(Sidewards, cos);
        }

        normalizedDirection *= currentSpeed;
        var velocity = new Vector3(normalizedDirection.x, 0, normalizedDirection.y);
        _characterController.SimpleMove(velocity);
        
        // mouse pos
        playerUi.UpdateCrossHairPosition(_mousePos);
        transform.LookAt(GetCameraRaycastPositionThroughMouseCursor());
    }

    private void Update()
    {
        // Slicing
        if (!_isSlicing || !_canSlice) return;
        
        animator.SetTrigger(Swing);
        StartCoroutine(ActivateSliceHitBox());
        StartCoroutine(SliceCooldown());
    }

    // INPUT ACTIONS
    
    private IEnumerator OnShoot()
    {
        // return when player can't shoot or the boomerang is currently travelling
        if (!_canShoot || !_isBoomerangAvailable) yield break;

        _canSlice = false;
        dummy.SetActive(true);
        knife.SetActive(false);

        var spawnPoint = boomerangSpawn.transform.position;
        var target = GetCameraRaycastPositionThroughMouseCursor();
        var direction = target - spawnPoint;

        // play throw animation and wait for it
        animator.SetTrigger(Throw);
        yield return new WaitForSeconds(0.31f);
        
        dummy.SetActive(false);
        boomerang.SetActive(true);
        boomerang.GetComponent<BoomerangBehaviour>().Throw(spawnPoint, direction);

        _isBoomerangAvailable = false;
        StartCoroutine(BoomerangCooldown());

        playerUi.UpdateBoomerangCatched(false);

        _canSlice = true;
        knife.SetActive(true);
    }

    private void OnBowl(InputAction.CallbackContext context)
    {
        // return when player can't bowl 
        if (!_canBowl) return;

        StartCoroutine(Bowling());
        StartCoroutine(BowlCooldown());
    }

    // Other events
    public void PutBoomerangAway(BoomerangBehaviour boomer)
    {
        _isBoomerangAvailable = true;
        boomer.ReturnToRestPosition();
        dummy.SetActive(true);
        playerUi.UpdateBoomerangCatched(true);
        animator.SetTrigger(Catch);
        StartCoroutine(BoomerangToWaist());
    }

    private IEnumerator BoomerangToWaist()
    {
        _canSlice = false;
        knife.SetActive(false);
        yield return new WaitForSeconds(0.458f);
        _canSlice = true;
        dummy.SetActive(false);
        knife.SetActive(true);
    }

    private void EndGame()
    {
        StopAllCoroutines();
        
        movingInputAction.Disable();
        shootInputAction.Disable();
        sliceInputAction.Disable();
        sprintInputAction.Disable();
        bowlInputAction.Disable();
        mousePos.Disable();
        
        playerUi.gameObject.SetActive(false);

        postGameUI.gameObject.SetActive(true);
        postGameUI.Initialize(_collectedBowls);
    }

    // COOLDOWN COROUTINES
    
    private IEnumerator BoomerangCooldown()
    {
        _canShoot = false;
        var cd = boomerangCooldown;
        while (cd > 0.0f)
        {
            yield return new WaitForSeconds(0.1f);
            cd -= 0.1f;
            playerUi.UpdateBoomerangCooldown(cd);
        }
        
        playerUi.UpdateBoomerangCooldown(cd);
        _canShoot = true;
    }

    private IEnumerator SliceCooldown()
    {
        _canSlice = false;
        var cd = swordCooldown;
        while (cd > 0.0f)
        {
            yield return new WaitForSeconds(0.1f);
            cd -= 0.1f;
            playerUi.UpdateKnifeCooldown(cd);
        }
        
        playerUi.UpdateKnifeCooldown(cd);
        _canSlice = true;
    }

    private IEnumerator BowlCooldown()
    {
        _canBowl = false;
        var cd = bowlCooldown;
        while (cd > 0.0f)
        {
            yield return new WaitForSeconds(0.1f);
            cd -= 0.1f;
            playerUi.UpdateBowlCooldown(cd);
        }
        
        playerUi.UpdateBowlCooldown(cd);
        _canBowl = true;
    }

    private IEnumerator SprintCooldown()
    {
        _canSprint = false;
        yield return new WaitForSeconds(sprintCooldown);
        _canSprint = true;
    }

    private IEnumerator ActivateSliceHitBox()
    {
        sliceCollider.SetActive(true);
        yield return new WaitForSeconds(attackWindow);
        sliceCollider.SetActive(false);
    }

    private IEnumerator Bowling()
    {
        _canSlice = false;
        bowl.SwingBowl();
        _animation.Play("spin");
        animator.SetTrigger(Bowl);
        yield return WaitForAnimation();
        _collectedBowls[_numBowls] = bowl.PutBowlAway();

        _numBowls++;
        
        playerUi.UpdateBowlsLeft(numOfBowls - _numBowls);

        if (_numBowls >= numOfBowls)
        {
            EndGame();
        }

        _canSlice = true;
    }

    private IEnumerator GameStartRoutine()
    {
        // wait half a second to make sure the game is loaded
        yield return new WaitForSeconds(0.5f);
        _audio.clip = countDownLow;
        for (var i = 3; i >= 0; i--)
        {
            playerUi.UpdateCountdown(i);
            _audio.Play();

            if (i == 0)
            {
                _audio.clip = countDownHigh;
                _audio.Play();
            }

            yield return new WaitForSeconds(1.0f);
        }
        
        playerUi.UpdateCountdown(-1);
        
        movingInputAction.Enable();
        shootInputAction.Enable();
        sliceInputAction.Enable();
        sprintInputAction.Enable();
        bowlInputAction.Enable();
        pause.Enable();

        StartCoroutine(GameTime());
    }

    private IEnumerator GameTime()
    {
        var time = gameTime;
        while (time > 0)
        {
            yield return new WaitForSeconds(1f);
            time -= 1;
            playerUi.UpdateTimeLeft(time);

            if (time > 10 || time < 1) continue;
            _audio.clip = countDownLow;
            _audio.Play();
            
            if (time > 5) continue;
            _audio.clip = countDownHigh;
            _audio.Play();
        }
        
        EndGame();
    }

    // UTILITY METHODS
    private Vector3 GetCameraRaycastPositionThroughMouseCursor()
    {
        var ray = _cam.ScreenPointToRay(_mousePos);
        if (!Physics.Raycast(ray, out var hit)) return Vector3.zero;
        var pos = hit.point;
        pos.y = 2.0f;
        return pos;
    }
    
    private IEnumerator WaitForAnimation()
    {
        do { yield return null; } 
        while (_animation.isPlaying);
    }
}
