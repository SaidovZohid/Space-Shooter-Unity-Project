using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 8.0f; 

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        
        if (transform.position.y > 8.0f)
        {
            Destroy(gameObject);
        }
    }
}
