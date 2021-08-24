using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangCollider : MonoBehaviour
{
    [SerializeField]
    private bool invertZ;
    
    public delegate void WallHitDelegate(bool invertZ);
    public event WallHitDelegate WallHitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != 0) return;

        WallHitEvent?.Invoke(invertZ);
    }
}
