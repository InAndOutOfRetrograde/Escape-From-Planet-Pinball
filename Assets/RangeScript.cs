using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeScript : MonoBehaviour
{
    private List<Collider> colliders = new List<Collider>();

    public List<Collider> GetColliders() { return colliders; }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Grapplable"))
        {
            colliders.Add(other); //hashset automatically handles duplicates
            other.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grapplable"))
        {
            colliders.Remove(other);
            other.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    public Collider GetClosest()
    {
        float nearestDist = 10000;
        int colliderToReturn = 0;
        if(colliders.Count > 0)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                float distance = (colliders[i].transform.position - this.gameObject.transform.position).magnitude;
                if (distance < nearestDist)
                {
                    nearestDist = distance;
                    colliderToReturn = i;
                }
            }

            return colliders[colliderToReturn];
        }
        return null;
    }
}
