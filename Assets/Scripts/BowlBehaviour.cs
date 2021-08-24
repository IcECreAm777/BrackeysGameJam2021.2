using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlBehaviour : MonoBehaviour
{
    private Collider _collider;
    private List<CollectableFruitScriptableObject> _collectedFruits = new List<CollectableFruitScriptableObject>();

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // bowl can only collide with fruits, so an extra check for the layer is unnecessary
        var fruitInfo = other.gameObject.GetComponent<FruitBehaviour>().Collect();
        if(fruitInfo == null) return; // this fruit is not collectable
        _collectedFruits.Add(fruitInfo);
    }

    public void SwingBowl()
    {
        _collider.enabled = true;
    }

    public List<CollectableFruitScriptableObject> PutBowlAway()
    {
        _collider.enabled = false;
        var tmp = new List<CollectableFruitScriptableObject>(_collectedFruits);
        _collectedFruits.Clear();
        return tmp;
    }
}
