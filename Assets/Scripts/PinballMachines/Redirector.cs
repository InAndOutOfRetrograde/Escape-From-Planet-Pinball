 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redirector : Machine
{
    public float bumperForce = 0;

    public void OnCollisionEnter(Collision collide)
    {
        if (collide.gameObject.name == "Player")
        {
            collide.transform.GetComponent<Rigidbody>().velocity = collide.transform.GetComponent<Rigidbody>().velocity.normalized * 2;
            collide.transform.GetComponent<Rigidbody>().AddForce(-collide.contacts[0].normal * bumperForce, ForceMode.Impulse);
        }
    }
}
