using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float impulseRangeX = 0.0f;
    public float impulseRangeZ = 0.0f;
    public float impulseY = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            float impulseX = Random.Range(-impulseRangeX, impulseRangeX);
            float impulseZ = Random.Range(-impulseRangeZ, impulseRangeZ);
            Vector3 impulse = new Vector3(impulseX, impulseY, impulseZ);
            GetComponent<Rigidbody>().AddForce(impulse, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {

    }
}
