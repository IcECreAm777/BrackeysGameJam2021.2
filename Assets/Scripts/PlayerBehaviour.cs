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
    private GameObject boomerangSpawn;
    
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

    [Header("UI")] 
    [SerializeField] 
    private PlayerUi playerUi;

    // other components
    private CharacterController _characterController;
    private Camera _cam;

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
    private bool _isSlicing = true;
    private int _numBowlsLeft;


    // ENGINE FUNCTIONS 
    
    void Awake()
    {
        // init default values
        _stamina = maxStamina;
        _numBowlsLeft = numOfBowls;

        // get other components
        _characterController = GetComponent<CharacterController>();
        _cam = Camera.main;

        // initialize input maps
        movingInputAction.performed += context => { _velocity = context.ReadValue<Vector2>(); };
        movingInputAction.canceled += context => { _velocity = Vector2.zero; };
        shootInputAction.performed += OnShoot;
        sliceInputAction.performed += context => { _isSlicing = true; };
        sliceInputAction.canceled += context => { _isSlicing = false; };
        sprintInputAction.performed += context => { _substractStamina = true; };
        sprintInputAction.canceled += context => { _substractStamina = false; };
        bowlInputAction.performed += OnBowl;
        mousePos.performed += context => { _mousePos = context.ReadValue<Vector2>(); };

        movingInputAction.Enable();
        shootInputAction.Enable();
        sliceInputAction.Enable();
        sprintInputAction.Enable();
        bowlInputAction.Enable();
        mousePos.Enable();
    }

    private void Start()
    {
        //TODO spawn boomerang below the level
        
#if DEBUG
        if (playerUi == null)
        {
            Debug.LogError("The UI controller is not assigned for the player");
        }

        if (boomerangSpawn == null)
        {
            Debug.LogError("There was no game Object assigned as the boomerang spawn");
        }
#endif
        
        //TODO initialize UI (like max Stamina and stuff)
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

        normalizedDirection *= currentSpeed;
        var velocity = new Vector3(normalizedDirection.x, 0, normalizedDirection.y);
        _characterController.SimpleMove(velocity);
        
        // mouse pos
        playerUi.UpdateCrossHairPosition(_mousePos);
        transform.LookAt(GetCameraRaycastPositionThroughMouseCursor());
    }
    
    // INPUT ACTIONS
    
    private void OnShoot(InputAction.CallbackContext context)
    {
        // return when player can't shoot or the boomerang is currently travelling
        if (!_canShoot || !_isBoomerangAvailable) return;

        var spawnPoint = boomerangSpawn.transform.position;
        var target = GetCameraRaycastPositionThroughMouseCursor();
        var direction = target - spawnPoint;

        //TODO spawn boomerang

        _isBoomerangAvailable = false;
        StartCoroutine(BoomerangCooldown());
    }

    private void OnBowl(InputAction.CallbackContext context)
    {
        // return when player can't bowl 
        if (!_canBowl) return;
        
        //TODO do spinning animation
        //TODO collect everything in range and save the bowl

        if (--numOfBowls <= 0)
        {
            //TODO end game
            return;
        }
        
        StartCoroutine(BowlCooldown());
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

    private IEnumerator SwordCooldown()
    {
        _canSlice = false;
        var cd = swordCooldown;
        while (cd > 0.0f)
        {
            yield return new WaitForSeconds(0.1f);
            cd -= 0.1f;
            playerUi.UpdateSwordCooldown(cd);
        }
        
        playerUi.UpdateSwordCooldown(cd);
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
    
    // UTILITY METHODS
    private Vector3 GetCameraRaycastPositionThroughMouseCursor()
    {
        var ray = _cam.ScreenPointToRay(_mousePos);
        if (!Physics.Raycast(ray, out var hit)) return Vector3.zero;
        var pos = hit.point;
        pos.y = 2.0f;
        return pos;
    }
}
