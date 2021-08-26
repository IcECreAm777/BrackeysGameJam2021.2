using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // collision is only activate for fruits so no safe guard should be necessary
        
        var fruit = other.gameObject.GetComponent<FruitBehaviour>();
        
#if DEBUG
        if (fruit == null)
        {
            Debug.LogError($"The fruit '{other.gameObject.name}' has no fruit behaviour");
        }
#endif
        
        fruit.Split();
    }
}
