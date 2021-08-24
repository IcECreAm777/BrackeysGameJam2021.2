using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Collectable Fruit", menuName = "Fruit/Collectable Fruit")]
public class CollectableFruitScriptableObject : ScriptableObject
{
    [Tooltip("The name of the fruit")]
    public string fruitName;
    
    [Tooltip("the points one of this object gives you when catching it in the bowl")]
    public int points;
}
