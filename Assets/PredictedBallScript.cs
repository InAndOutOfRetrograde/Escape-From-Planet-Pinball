using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictedBallScript : MonoBehaviour
{
    void DistanceVisualizer(Vector3 loc)
    {
        this.transform.position = loc;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.parent.gameObject.GetComponent<BallMovementScript>().grounded == false)
        {
            this.gameObject.transform.GetComponent<MeshRenderer>().enabled = true;

            DistanceVisualizer(this.transform.parent.gameObject.GetComponent<BallMovementScript>().predictBallLoc);
        }
        else
        {
            this.gameObject.transform.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
