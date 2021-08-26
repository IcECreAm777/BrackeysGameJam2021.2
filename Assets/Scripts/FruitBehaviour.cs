using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FruitBehaviour : MonoBehaviour
{
    [Header("Splitting")]
    [SerializeField]
    private bool canSplit;
    [SerializeField]
    private SplittingFruitScriptableObject splittingInfo;

    [Header("Collectable")]
    [SerializeField]
    private bool isCollectable;
    [SerializeField]
    private CollectableFruitScriptableObject collectableInfo;

    private GameObject[] _instantiatedFruits;
    private bool _childrenSpawned;
    private bool _aliveLongEnough;
    
    // ENGINE FUNCTIONS

#if DEBUG
    private void Awake()
    {
        if (canSplit && isCollectable)
        {
            Debug.LogWarning($"This fruit ('{name}') is tagged as splitting and as collectable, which might lead to errors");
        }

        if (!canSplit && !isCollectable)
        {
            Debug.LogWarning($"This fruit ('{name}') is neither tagged as collectable nor as splitting");
        }

        if (canSplit && splittingInfo == null)
        {
            Debug.LogWarning($"This fruit ('{name}') is tagged as splitting, but no splitting info was added");
        }

        if (isCollectable && collectableInfo == null)
        {
            Debug.LogError($"This fruit ('{name}') is tagged as collectable but has no information assigned to it");
        }
    }
#endif

    private void Start()
    {
        // fruits can be split after a certain amount of time
        StartCoroutine(AliveLongEnough());
        
        // some fruits are spawning the children earlier so they don't have to spawn them again
        if (_childrenSpawned) return;
        SpawnChildren();
    }
    
    // OTHER FUNCTIONALITY
    public CollectableFruitScriptableObject Collect()
    {
        if (!isCollectable) return null;
        
        Destroy(gameObject, 0.01f);
        return collectableInfo;
    }

    public void Split()
    {
        if (!canSplit || !_aliveLongEnough) return;
        
        var position = transform.position;
        foreach (var fruit in _instantiatedFruits)
        {
            fruit.SetActive(true);
            fruit.transform.position = position;
            var dir = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
            var force = dir.normalized * Random.Range(splittingInfo.splittingMinForce, 
                splittingInfo.splittingMaxForce);
            var dvd = fruit.GetComponent<DVDBehaviour>();
            dvd.Initialize(force);
        }
        
        Destroy(gameObject);
    }

    public void SpawnChildren()
    {
        // return when fruit is not splittable (a collectable fruit)
        if (!canSplit)
        {
            _childrenSpawned = true;
            return;
        }
        
        var basket = GameObject.Find("FruitsBasket");
        
#if DEBUG
        if (basket == null)
        {
            Debug.LogError("No game object with the name 'FruitsBasket found'");
            return;
        }
#endif
        
        var pos = basket.transform.position;
        _instantiatedFruits = new GameObject[splittingInfo.numOfSpawnedFruits];
        for (var i = 0; i < splittingInfo.numOfSpawnedFruits; i++)
        {
            var index = 0;
            if (splittingInfo.splitFruits.Count > 1)
            {
                index = Random.Range(0, splittingInfo.splitFruits.Count);
            }
            
            _instantiatedFruits[i] = Instantiate(splittingInfo.splitFruits[index]);
            _instantiatedFruits[i].transform.position = pos;
            _instantiatedFruits[i].GetComponent<FruitBehaviour>().SpawnChildren();
            _instantiatedFruits[i].SetActive(false);
        }

        _childrenSpawned = true;
    }
    
    // COROUTINES

    private IEnumerator AliveLongEnough()
    {
        yield return new WaitForSeconds(1.0f);
        _aliveLongEnough = true;
    }
}
