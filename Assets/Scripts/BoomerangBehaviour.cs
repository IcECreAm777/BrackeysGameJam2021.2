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
    private float afterFirstHitSpeedScaling = 1.0f;

    private Rigidbody _rb;
    private DVDBehaviour _dvd;
    
    private bool _isInitial = true;
    private bool _isThrown = false;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _dvd = GetComponent<DVDBehaviour>();

        _dvd.WallHitEvent += () =>
        {
            if (!_isInitial) return;
            _dvd.ScaleVelocity(afterFirstHitSpeedScaling);
            _isInitial = false;
        };
        
#if DEBUG
        if (_rb == null)
        {
            Debug.LogError("There was no kinematic Rigidbody assigned for the boomerang");
        }
#endif
    }

    public void Throw(Vector3 spawnPosition, Vector3 direction)
    {
        // don't ask why transform1, Rider said this is more efficient and who am I to judge
        var transform1 = transform;
        transform1.parent = null;
        transform1.position = spawnPosition;
        transform1.rotation = new Quaternion();
        
        _dvd.Initialize(direction.normalized * initialSpeed);
        _isInitial = true;
        _isThrown = true;
    }

    public void ReturnToRestPosition(GameObject restPositionObject)
    {
        // don't ask why transform1, Rider said this is more efficient and who am I to judge
        var transform1 = transform;
        transform1.position = restPositionObject.transform.position;
        transform1.parent = restPositionObject.transform;
        
        _rb.velocity = Vector3.zero;
        _dvd.ScaleVelocity(0.0f);

        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other) 
    {
        // only care about collision when boomerang is thrown
        if(!_isThrown) return;
        
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
                var fruit = other.gameObject.GetComponent<FruitBehaviour>();
                fruit.Split();
                break;
        }
    }
}
