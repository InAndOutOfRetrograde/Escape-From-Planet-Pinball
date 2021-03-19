/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class LayerMasking : MonoBehaviour
{
    public GameObject camera;
    public GameObject target;
    LayerMask mylayermask;

    void Start()
    {
        mylayermask = LayerMask.GetMask("Wall");
    }

    void Update()
    {
        RaycastHit hit;

        Debug.DrawRay(camera.transform.position, (target.transform.position - camera.transform.position).normalized * 1000f, Color.green);

        //Does the ray intersect with the sphere?
        if (Physics.Raycast(camera.transform.position, (target.transform.position - camera.transform.position).normalized, out hit, (target.transform.position - camera.transform.position).magnitude, mylayermask))
        {
            //if it collides with the WALL, scale it down w/ dotween
            if (hit.collider.gameObject.tag == "wall")
            {
                Debug.Log("Hit something");
                target.transform.DOScale(10, 2);
            }
        }
        //else if it doesn't, scale it to 0
        else
        {
            target.transform.DOScale(0, 2);
        }
    }
}*/
