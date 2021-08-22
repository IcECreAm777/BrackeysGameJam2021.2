using Godot;
using System;

public class PlayerNode : Spatial
{
    // exposed variables
    [Export]
    public float Speed = 7.0f;
    [Export] 
    public float SprintMultiplier = 1.35f;
    [Export]
    public int MaxStamina = 100;
    [Export]
    public float StaminaConsumption = 5.0f;
    [Export]
    public float StaminaRegenerationRate = 3.7f;
    [Export]
    public float SprintCooldown = 1.0f;
    [Export]
    public float DodgeDuration = 0.2f;
    [Export]
    public float DodgeSpeed = 15.0f;
    
    // private unexposed variables
    private KinematicBody _kinematicBody;
    private Vector3 _velocity = Vector3.Zero;
    private bool _substractStamina = false;
    private float _stamina;
    private bool _canSprint = true;
    private float _sprintCooldown = 0.0f;
    private float _dodgeCooldown = 0.0f;
    private bool _canMove = true;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _kinematicBody = GetChild<KinematicBody>(0);
        _stamina = MaxStamina;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        HandleInput();
        HandlePhysics(delta);
    }

    private void HandleInput()
    {
        if (!_canMove) return;
        
        // Movement
        var direction = Vector2.Zero;
        var speed = Speed;
        _substractStamina = false;
        
        if (Input.IsActionPressed("ig_up"))
        {
            direction.y -= 1;
        }

        if (Input.IsActionPressed("ig_down"))
        {
            direction.y += 1;
        }

        if (Input.IsActionPressed("ig_left"))
        {
            direction.x -= 1;
        }

        if (Input.IsActionPressed("ig_right"))
        {
            direction.x += 1;
        }

        if (_canSprint && Input.IsActionPressed("ig_sprint"))
        {
            speed *= SprintMultiplier;
            _substractStamina = true;
        }
        
        if (direction != Vector2.Zero)
        {
            direction = direction.Normalized();
        }

        _velocity.x = direction.x * speed;
        _velocity.z = direction.y * speed;

        if (!Input.IsActionJustPressed("ig_dodge")) return;
        _canMove = false;
        _dodgeCooldown = 0.0f;
        _velocity.x = direction.x * DodgeSpeed;
        _velocity.z = direction.y * DodgeSpeed;
    }

    private void HandlePhysics(float delta)
    {
        _kinematicBody.MoveAndSlide(_velocity, Vector3.Up);

        _sprintCooldown += delta;
        _dodgeCooldown += delta;
        
        if (_dodgeCooldown > DodgeDuration)
        {
            _canMove = true;
        }

        if (_substractStamina)
        {
            _stamina -= StaminaConsumption;
            
            if (_stamina > 0.0f) return;
            Input.ActionRelease("ig_sprint");
            _canSprint = false;
            _sprintCooldown = 0.0f;
            return;
        }

        if (_sprintCooldown > SprintCooldown)
        {
            _canSprint = true;
        }

        _stamina += StaminaRegenerationRate;
        _stamina = _stamina > MaxStamina ? MaxStamina : _stamina;
    }
}
