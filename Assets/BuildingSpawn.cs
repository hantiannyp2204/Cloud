using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawn : MonoBehaviour
{
    public int moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(0, 0, 0.001f * moveSpeed);
        if (transform.localPosition.z <= 0)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, Random.Range(-5.5f, 0), Random.Range(25, 30)) ;
        }
    }
}
