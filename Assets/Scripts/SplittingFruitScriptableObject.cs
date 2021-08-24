using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Splitting Fruit", menuName = "Fruit/Splitting Fruit")]
public class SplittingFruitScriptableObject : ScriptableObject
{
    public List<GameObject> splitFruits;
    [Range(0.1f, 100.0f)] 
    public float splittingMinForce;
    [Range(0.1f, 200.0f)]
    public float splittingMaxForce;
}
