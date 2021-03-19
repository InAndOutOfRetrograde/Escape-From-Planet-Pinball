using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{
    private RangeScript range;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public Transform grappleStart;

    private SpringJoint joint;
    public float spring = 4.5f;
    public float damper = 7f;
    public float massScale = 4.5f;

    private bool grappled;

    void Awake()
    {
        lr = gameObject.GetComponent<LineRenderer>();
        range = gameObject.GetComponentInChildren<RangeScript>();
        grappled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //hasnt attached, 
            if (!grappled && range.GetClosest() != null)
            {
                StartGrapple();
                grappled = true;
            }
            //has attached,
            else if(grappled)
            {
                StopGrapple();
                grappled = false;
            }
        }
    }

    void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        Collider pivot = range.GetClosest();
        //attach to pivot
        grapplePoint = pivot.transform.position;
        joint = this.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        //distance script is omitted because of sphere collider

        if(pivot.name == "PivotPointTighten")
        {
            spring = 80f;
        }
        else if(pivot.name == "PivotPointSwing")
        {
            spring = 10f;
            damper = 70f;
            massScale = 4.5f;
        }
        //variables to adjust
        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;

        lr.positionCount = 2;
    }

    void DrawRope()
    {
        //if not grappling
        if (!joint) return;

        lr.SetPosition(0, grappleStart.position);
        lr.SetPosition(1, grapplePoint);
    }
    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }
}
