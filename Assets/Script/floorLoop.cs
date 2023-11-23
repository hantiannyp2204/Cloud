using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class floorLoop : MonoBehaviour
{
    [SerializeField]
    GameObject road;
    GameObject spawnedPrefab;
    public Transform deletePoint, spawnPoint;
    // Update is called once per frame
    private void Start()
    {
        spawnedPrefab = Instantiate(road, spawnPoint);
    }
    void Update()
    {
        spawnRoad();

    }
    void spawnRoad()
    {

        if (spawnedPrefab.transform.position.z <= spawnPoint.transform.position.z)
        {
            spawnedPrefab = Instantiate(road, spawnPoint);
        }
    }
}
