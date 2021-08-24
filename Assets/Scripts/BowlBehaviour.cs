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
        
        //TODO check if the fruit can be collected (if it's small enough)
        //TODO add it to the list
        //TODO make the fruit disappear
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
