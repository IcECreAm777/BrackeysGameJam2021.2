using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangBehaviour : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField]
    private float initialSpeed;
    [SerializeField]
    private float travelSpeed;

    [Header("CollisionDetection")]
    [SerializeField]
    private BoomerangCollider upCollision;
    [SerializeField]
    private BoomerangCollider downCollision;
    [SerializeField]
    private BoomerangCollider leftCollision;
    [SerializeField]
    private BoomerangCollider rightCollision;

    private Rigidbody _rb;
    
    private Vector3 _velocity;
    private bool _isInitial = true;
    private bool _isThrown = false;
    private bool _canChangeZ = true;
    private bool _canChangeX = true;

    private const float Epsilon = 0.001f;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        upCollision.WallHitEvent += ChangeDirection;
        downCollision.WallHitEvent += ChangeDirection;
        leftCollision.WallHitEvent += ChangeDirection;
        rightCollision.WallHitEvent += ChangeDirection;
        
#if DEBUG
        if (_rb == null)
        {
            Debug.LogError("There was no kinematic Rigidbody assigned for the boomerang");
        }
#endif
    }

    private void FixedUpdate()
    {
        if (!_isThrown) return;

        var targetPos = transform.position + _velocity;
        _rb.MovePosition(targetPos);
    }

    public void Throw(Vector3 spawnPosition, Vector3 direction)
    {
        // don't ask why transform1, Rider said this is more efficient and who am I to judge
        var transform1 = transform;
        transform1.parent = null;
        transform1.position = spawnPosition;
        transform1.rotation = new Quaternion();
        
        _velocity = direction.normalized * initialSpeed;
        _isInitial = true;
        _isThrown = true;
    }

    public void ReturnToRestPosition(GameObject restPositionObject)
    {
        // don't ask why transform1, Rider said this is more efficient and who am I to judge
        var transform1 = transform;
        transform1.position = restPositionObject.transform.position;
        transform1.parent = restPositionObject.transform;
        
        _isThrown = false;
        _velocity = Vector3.zero;
        _rb.velocity = Vector3.zero;

        _canChangeX = true;
        _canChangeZ = true;
        
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other) 
    {
        // do stuff based on the collision layer
        var otherObject = other.gameObject;
        switch (otherObject.layer)
        {
            case 3:
                // collision with a player
                if(!_isThrown) break;
                var player = otherObject.GetComponentInParent<PlayerBehaviour>();
                player.PutBoomerangAway(this);
                break;
            case 7:
                // collision with a fruit
                //TODO cut fruit if possible
                break;
        }
    }

    private void ChangeDirection(bool flipZ)
    {
        // open scissor when hitting a wall the first time and reducing it's speed
        if (_isInitial)
        {
            //TODO do animation of scissor being opened

            _velocity = _velocity.normalized * travelSpeed;
            _isInitial = false;
        }
        
        if (flipZ && _canChangeZ)
        {
            _velocity.z = -_velocity.z;
            StartCoroutine(DisableFlipZDirection());
            return;
        }

        if(!_canChangeX) return;
        _velocity.x = -_velocity.x;
        StartCoroutine(DisableFlipXDirection());
    }
    
    private IEnumerator DisableFlipZDirection()
    {
        _canChangeZ = false;
        yield return new WaitForSeconds(1.0f);
        _canChangeZ = true;
    }
    
    private IEnumerator DisableFlipXDirection()
    {
        _canChangeX = false;
        yield return new WaitForSeconds(1.0f);
        _canChangeX = true;
    }
}
