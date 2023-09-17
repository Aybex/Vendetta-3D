using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float tilt = 60f;
    public float distance = 30f;
    public float height = 0f;

    public float zoomSpeed = 1f;
    public float maxDistance = 50f;
    public float minDistance = 10f;


    Vector3 positionOffset; 

    void Start()
    {
        positionOffset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        //follow target
        transform.position = target.position + positionOffset;
       
        /*
        transform.position = target.position - target.forward * distance + target.up * height;
        //look at target
        transform.LookAt(target.position + target.forward * 5f);
        //tilt
        transform.RotateAround(target.position, target.right, tilt);
        

        //smooth zoom
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            distance -= zoomSpeed;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            distance += zoomSpeed;
        }
    
        height = Mathf.Clamp(distance-25, 0f, maxDistance);
        */
    }
}
