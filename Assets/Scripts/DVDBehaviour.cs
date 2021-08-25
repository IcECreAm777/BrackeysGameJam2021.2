using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DVDBehaviour : MonoBehaviour
{
    // event for hitting the wall
    public delegate void WallHitDelegate();
    public event WallHitDelegate WallHitEvent;
    
    // private 
    private Vector3 _velocity = Vector3.zero;
    private Rigidbody _rb;

    // ENGINE Methods 
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
    }

    private void FixedUpdate()
    {
        var targetPos = transform.position + _velocity;
        _rb.MovePosition(targetPos);
    }
    
    // COLLISION

    private void OnCollisionEnter(Collision other)
    {
        // get the raycast to the colliding object
        var currentPos = transform.position;
        var direction = (other.transform.position - currentPos).normalized;
        var ray = new Ray(currentPos, direction);
        
        // raycast and make sure something was hit
        if(!Physics.Raycast(ray, out var hit)) return;
        if (hit.collider == null) return;

        // get the normal of the hit in world space
        var normal = hit.normal;
        normal = hit.transform.TransformDirection(normal);
        
        // get the direction
        if (normal == hit.transform.forward || normal == -hit.transform.forward)
        {
            _velocity.z = -_velocity.z;
        }

        if (normal == hit.transform.right || normal == -hit.transform.right)
        {
            _velocity.x = -_velocity.x;
        }

        WallHitEvent?.Invoke();
    }
    
    // OTHER FUNCTIONALITY
    public void Initialize(Vector3 initVelocity)
    {
        _velocity = initVelocity;
    }

    public void ScaleVelocity(float scaling)
    {
        _velocity *= scaling;
    }
}
