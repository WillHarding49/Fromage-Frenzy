using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_rotate : MonoBehaviour
{
    public GameObject Light;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Light.transform.Rotate(speed,0,0);
    }
}
