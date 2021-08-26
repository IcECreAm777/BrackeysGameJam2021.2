using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class OutOfBoundTeleporterBehaviour : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> teleportPosition;

#if DEBUG
    private void Awake()
    {
        if (teleportPosition == null || teleportPosition.Count == 0)
        {
            Debug.LogError("There was no game object as teleport point assigned");
        }
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        var index = Random.Range(0, teleportPosition.Count);
        other.transform.position = teleportPosition[index].transform.position;
    }
}
