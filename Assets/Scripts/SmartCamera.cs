using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartCamera : MonoBehaviour
{
    //in case we want more than 2 players ever
    public List<Transform> targetsOnCam;
    //in case we want to adjust the position of our camera
    public Vector3 offset;
    //for the smoothing
    public float smoothTime = .7f;
    private Vector3 velocity;
    float distance;

    //updates a frame after the normal one. will make smooth like butter
    void FixedUpdate()
    {
        if (targetsOnCam.Count > 1)
        {
            distance = Vector3.Distance(targetsOnCam[0].position, targetsOnCam[1].position);
        }
        else
        {
            distance = 1f;
        }

        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset - (transform.forward * distance);
        //adjust position
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    //self explanatory.
    public Vector3 GetCenterPoint()
    {
        //in case only one is on screen because the other was blown up or something
        if (targetsOnCam.Count == 1)
        {
            return targetsOnCam[0].position;
        }

        //bounds does the thing where it encapsulates two targets for
        //you and gets the center. Kind of lucky it just exists
        var bounds = new Bounds(targetsOnCam[0].position, Vector3.zero);
        for (int i = 0; i < targetsOnCam.Count; i++)
        {
            bounds.Encapsulate(targetsOnCam[i].position);
        }

        Vector3 center = bounds.center;

        return center;
    }
    //use bounds to check distance. then pull back relative to the distance between the two
}

