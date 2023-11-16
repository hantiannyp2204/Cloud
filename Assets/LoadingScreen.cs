using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public int moveSpeed;


    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(0,0,0.001f*moveSpeed);
        if(transform.localPosition.z <= -5.7)
        {
            Destroy(gameObject);
        }
    }
}
