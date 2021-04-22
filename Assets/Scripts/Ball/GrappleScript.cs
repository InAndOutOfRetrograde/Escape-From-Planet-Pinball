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
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            //hasnt attached
            if (!grappled && range.GetClosest() != null)
            {
                StartGrapple();
                grappled = true;
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            //has attached
            if (grappled)
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

        if(pivot.name.Contains("PivotPointTighten"))
        {
            spring = 90f;
            damper = 7f;
            massScale = 4.5f;
        }
        else if(pivot.name.Contains("PivotPointSwing"))
        {
            spring = 18f;
            damper = 0.3f;
            massScale = 3f;
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
