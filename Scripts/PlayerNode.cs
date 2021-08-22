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
    
    // child nodes
    private KinematicBody _kinematicBody;
    private Sprite _crosshair;
    private Camera _cam;
    private MeshInstance _mesh;

    // private properties
    private Vector3 _velocity = Vector3.Zero;
    private bool _substractStamina = false;
    private float _stamina;
    private bool _canSprint = true;
    private float _sprintCooldown = 0.0f;
    private float _dodgeCooldown = 0.0f;
    private bool _canMove = true;
    private Vector2 _mousePos = Vector2.Zero;
    
    //TODO maybe get those properties from the weapon
    // SHooting
    private PackedScene _bulletScene = (PackedScene) GD.Load("res://Prefabs/Bullet.tscn");
    private float _shootCD = 0.2f; // actual cooldown, get this from the weapon node later
    private float _shootingCooldown = 0.0f; // Time since last shot
    private bool _canShoot = true;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _stamina = MaxStamina;
        
        _kinematicBody = GetNode<KinematicBody>(FindNode("KinematicBody").GetPath());
        _crosshair = GetNode<Sprite>(FindNode("CrossHair").GetPath());
        _cam = GetNode<Camera>(FindNode("Camera").GetPath());
        _mesh = GetNode<MeshInstance>(FindNode("MeshInstance").GetPath());

#if DEBUG
        if (_crosshair == null)
        {
            GD.PrintErr("Sprite with the name 'CrossHair' for the Player Node not found");
        }

        if (_cam == null)
        {
            GD.PrintErr("Camera for the Player Node not found");
        }

        if (_mesh == null)
        {
            GD.PrintErr("Mesh for the Player Node not found");
        }
        
        if (_kinematicBody == null)
        {
            GD.PrintErr("KinematicBody for the Player Node not found");
            return;
        }
#endif
        
        _kinematicBody.CollisionLayer = 2;
        _kinematicBody.CollisionMask = 0b00000000000000001111;
    }

    public override void _Process(float delta)
    {
        // mouse pos 
        _mousePos = GetViewport().GetMousePosition();
        
        UpdateUI();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        HandleInput(delta);
        HandlePhysics(delta);
    }

    private void HandleInput(float delta)
    {
        if (!_canMove) return;
        
        // look to cross hair
        var from = _cam.ProjectRayOrigin(_crosshair.Position);
        var to = from + _cam.ProjectRayNormal(_crosshair.Position) * 1000.0f;
        to.y = 2.0f;
        
        // turns character to the cross hair
        _mesh.LookAt(to, Vector3.Up);

        // Shooting
        if (_canShoot && Input.IsActionPressed("ig_shoot"))
        {
            var bulletDirection = to - _kinematicBody.Transform.origin;
            bulletDirection.y = 0.0f;
            bulletDirection = bulletDirection.Normalized();

            var bulletInstance = (Bullet) _bulletScene.Instance();
            bulletInstance.Initialize(bulletDirection, true);
            AddChild(bulletInstance);
            //AddChildBelowNode(GetTree().Root.GetNode("GameWorld"), bulletInstance);
            
            _shootingCooldown = -delta;
            _canShoot = false;
        }

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
        _dodgeCooldown = -delta;
        _velocity.x = direction.x * DodgeSpeed;
        _velocity.z = direction.y * DodgeSpeed;
    }

    private void HandlePhysics(float delta)
    {
        // Shooting physics
        _shootingCooldown += delta;
        if (_shootingCooldown > _shootCD)
        {
            _canShoot = true;
        }
        
        // moving Physics
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

    private void UpdateUI()
    {
        _crosshair.Position = _mousePos;
    }
}
