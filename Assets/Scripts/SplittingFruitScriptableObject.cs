using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Splitting Fruit", menuName = "Fruit/Splitting Fruit")]
public class SplittingFruitScriptableObject : ScriptableObject
{
    public List<GameObject> splitFruits;
    [Range(0.0f, 0.25f)] 
    public float splittingMinForce;
    [Range(0.0f, 0.6f)]
    public float splittingMaxForce;
}
