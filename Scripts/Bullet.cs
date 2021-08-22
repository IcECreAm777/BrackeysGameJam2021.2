using Godot;
using System;

public class Bullet : RigidBody
{
    [Export]
    public float BulletSpeed = 24.0f;
    [Export]
    public float LiveSpan = 5.0f;

    private Vector3 _velocity;
    private float _timeAlive = 0.0f;
    private bool _isFriendly;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AxisLockLinearY = true;
        ContactMonitor = true;

        CollisionLayer = 4;
        CollisionMask = 0b00000000000000000111;
        
        var mesh = GetNode<MeshInstance>(FindNode("MeshInstance").GetPath());
        var mat = (SpatialMaterial) mesh.GetActiveMaterial(0);
        mat.Emission = _isFriendly ? new Color(0.0f, 1.0f, 0.0f) : new Color(1.0f, 0.0f, 0.0f);
        mat.EmissionEnabled = true;
        mesh.SetSurfaceMaterial(0, mat);
        mesh.MaterialOverride = mat;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        _timeAlive += delta;
        if (_timeAlive >= LiveSpan)
        {
            QueueFree();
        }
    }

    public void Initialize(Vector3 direction, bool isFriendly = false)
    {
        _velocity = direction * BulletSpeed;
        _isFriendly = isFriendly;
        
        ApplyCentralImpulse(_velocity);
    }

    public void OnBodyEntered()
    {
        GD.Print("Yes");
    }
}
