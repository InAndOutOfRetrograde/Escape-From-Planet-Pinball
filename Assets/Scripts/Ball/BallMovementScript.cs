using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallMovementScript : MonoBehaviour
{
    //GENERIC VARS//
    Vector3 v3ForceUp = new Vector3(0, 0, 1);
    Vector3 v3ForceRight = new Vector3(1, 0, 0);
    float XandZMagnitude;
    float YMagnitude;
    Vector3 velocityToBe;
    Vector3 direction = new Vector3(0, 0, 0);

    //SPEED VARIABLES//
    //Change decrease in speed, top speed vert and hori here.
    public float topSpeedReduction = .09f;
    public float topSpeedXAndZ = 100f;
    public float slopeIncreaseModifier = 0f;
    float slopeSpeedCap = 0f;
    public float maxSpeedY = 90f; //find how to use later

    public float constantDeceleration = 0.1f;
    float currentDeceleration = 0f;
    public float acceleration = 20f;

    //BUTT SLAM VARIABLES//
    bool canSlam = true;
    private float buttSlamTimer = 0;
    private float buttSlamAirTime = 0;
    //a number for changing how long the player is in the air based on time
    public float distanceWeight = 1;

    //METEOR MODE//
    ParticleSystem bloom;
    public float meteorStateMin = .6f;
    bool meteorState = false;

    //TIMER VARS//
    //Vars used to limit movement things for a period of time.
    public float cooldownVar = 1f;
    public float movementCooldown;
    public bool movementAction = true;

    public bool grounded = true;
    public Vector3 slamSpeed = new Vector3(0, -200, 0);

    //PREDICTION LINE//
    public Vector3 predictBallLoc;
    [SerializeField]
    private Transform pos;

    private Rigidbody rb;

    //SLOPE DEBUGGING TOOLS//
    Transform center;
    Vector3 slopeForce;
    public float slopeStrength = 100f;

    public bool WorldUp = true;
    public bool LocalUp = true;
    public bool CrossVector = true;
    public bool SlopeVector = true;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        center = gameObject.transform.GetChild(3).transform;//for slope

        bloom = gameObject.transform.GetChild(1).GetComponentInChildren<ParticleSystem>();//for meteor mode
        bloom.Stop();
    }

    void Deacceleration()
    {
        velocityToBe = rb.velocity;
        //if going in a direction and there is no slope force in that direction, slow it down.
        if (velocityToBe.x > 0f && !(slopeForce.x > 0f))
        {
            velocityToBe.x -= currentDeceleration;
            if (velocityToBe.x < 0f) velocityToBe.x = 0f;
        }
        if (velocityToBe.x < 0f && !(slopeForce.x < 0f))
        {
            velocityToBe.x += currentDeceleration;
            if (velocityToBe.x > 0f) velocityToBe.x = 0f;
        }
        if (velocityToBe.z > 0f && !(slopeForce.y > 0f))
        {
            velocityToBe.z -= currentDeceleration;
            if (velocityToBe.z < 0f) velocityToBe.z = 0f;
        }
        if (velocityToBe.z < 0f && !(slopeForce.y < 0f))
        {
            velocityToBe.z += currentDeceleration;
            if (velocityToBe.z > 0f) velocityToBe.z = 0f;
        }
    }

    //moves based on input. 
    //as is, player will still go left right up and down based on WASD, but we should find a way to do this with slope.
    //maybe y force down?
    void DirectionalInput()
    {
        if (Input.GetKey("up") || Input.GetKey(KeyCode.W))
        {
            direction += v3ForceUp;

            currentDeceleration = 0f;

            velocityToBe.z = rb.velocity.z;
            if (slopeForce.z > 0)
            {
                velocityToBe.z += slopeForce.z;
            }
        }
        if (Input.GetKey("down") || Input.GetKey(KeyCode.S))
        {
            direction -= v3ForceUp;

            currentDeceleration = 0f;
            velocityToBe.z = rb.velocity.z;
            if (slopeForce.z < 0)
            {
                velocityToBe.z -= slopeForce.z;
            }
        }
        if (Input.GetKey("left") || Input.GetKey(KeyCode.A))
        {
            direction -= v3ForceRight;

            currentDeceleration = 0f;
            velocityToBe.x = rb.velocity.x;
            if (slopeForce.x > 0)
            {
                velocityToBe.x -= slopeForce.x;
            }
        }
        if (Input.GetKey("right") || Input.GetKey(KeyCode.D))
        {
            direction += v3ForceRight;

            currentDeceleration = 0f;
            velocityToBe.x = rb.velocity.x;
            if (slopeForce.x < 0)
            {
                velocityToBe.x += slopeForce.x;
            }
        }

        //adjusting direction based on slope
        Vector3 floorNorm = Vector3.up;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.75f))
        {
            floorNorm = hit.normal;
            direction = Vector3.ProjectOnPlane(direction, floorNorm);
        }
    }
    void MovementInput()
    {
        //controls
        DirectionalInput();

        //butt slam.
        if (Input.GetKeyDown(KeyCode.LeftControl) && canSlam)
        {
            canSlam = false;
            //read distance to ground.
            RaycastHit hit;
            Ray ray = new Ray(transform.position, -Vector3.up);
            Physics.Raycast(ray, out hit);
            //create a time
            buttSlamAirTime = hit.distance * distanceWeight;
            if(buttSlamAirTime > 0.7f)
            {
                buttSlamAirTime = 0.7f;
            }
        }

        //butt slam based on distance
        if (buttSlamTimer < buttSlamAirTime && canSlam == false)
        {
            buttSlamTimer += Time.deltaTime;
            velocityToBe = new Vector3(0, 0, 0);
            transform.position += Vector3.up * 0.1f;
            //if the player has gotten enough height, catch fire
            if (buttSlamTimer >= meteorStateMin)
            {
                meteorState = true;
            }
        }
        else if (buttSlamTimer >= buttSlamAirTime && canSlam == false)
        {
            //slam down and reset
            velocityToBe = slamSpeed;
            buttSlamTimer = 0;
            buttSlamAirTime = 0;
            canSlam = true;
        }

        //pause
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        //slow down
        if (!Input.anyKey)
        {
            currentDeceleration += constantDeceleration;
        }
        

        rb.velocity = velocityToBe;
        rb.velocity += direction.normalized * acceleration;
    }

    bool isGrounded()
    {
        Ray ray = new Ray(transform.position, -Vector3.up);

        if (Physics.SphereCast(ray, 0.4f, 0.2f))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        return grounded;
    }

    Vector3 SlopeMovement()
    {
        //get normal
        RaycastHit hit;
        Physics.Raycast(center.position, -Vector3.up, out hit, 3f);
        Debug.DrawRay(center.position, -Vector3.up, Color.green);
        Vector3 norm = hit.normal;

        Vector3 cross = Vector3.Cross(norm, Vector3.up); // get angle
        Vector3 slope = Vector3.Cross(norm, cross); //get the slope using the new plane

        //debugging tools
        if (WorldUp)
            Debug.DrawRay(center.position, Vector3.up * 5, Color.green);
        if (LocalUp)
            Debug.DrawRay(center.position, norm * 5, Color.magenta);
        if (CrossVector)
            Debug.DrawRay(center.position, cross * 5, Color.blue);
        if (SlopeVector)
            Debug.DrawRay(center.position, slope * 5, Color.red);

        //slope is now made, but we need to see which direction is steeper.

        //apply that force and scale for the slope every frame.
        return slope;
    }

    void SpeedCap(float maxSpeedXAndZ)
    {
        //slows the ball to top speed
        Vector3 adjustedVelocity = rb.velocity;
        XandZMagnitude = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
        if (XandZMagnitude > maxSpeedXAndZ)
        {
            float diff = XandZMagnitude - maxSpeedXAndZ;
            diff *= topSpeedReduction;
            Vector2 xAndZNormalized = new Vector2(rb.velocity.x, rb.velocity.z).normalized * (maxSpeedXAndZ + diff); //add ramp speed here);
            adjustedVelocity.x = xAndZNormalized.x;
            adjustedVelocity.z = xAndZNormalized.y;
        }
        adjustedVelocity.y = Mathf.Clamp(adjustedVelocity.y, -maxSpeedY, maxSpeedY);

        rb.velocity = adjustedVelocity;
    }
    //USELESS RN
    void Timer()
    {
        if (movementCooldown > 0 && movementAction == false)
        {
            movementCooldown -= Time.deltaTime;
        }
        else
        {
            movementCooldown = cooldownVar;
            movementAction = true;
        }
    }

    void LineForPrediction()
    {
        RaycastHit hit;
        //create a line
        Physics.Raycast(transform.position, -Vector3.up, out hit, 100f);
        //place an opaque hemisphere here.
        predictBallLoc = hit.point;
    }

    //a method to turn on all the properties that come with meteor mode
    void MeteorMode(bool state)
    {
        if(state)
        {
            Debug.Log("on");
            bloom.Play();
        }
        else
        {
            Debug.Log("off");
            bloom.Stop();
        }
    }

    void FixedUpdate()
    {
        isGrounded();
        //slope force
        if (grounded)
        {
            slopeForce = SlopeMovement();
            //add to top speed by slopemovement scale
            slopeSpeedCap = slopeForce.magnitude * 3;

            meteorState = false;
        }
        else
        {
            slopeForce = new Vector3(0,0,0);
        }
        //timer that is used to see when the player is free to move again/when deacceleration should slow them
        Timer();
        //slows the player every frame that a button is not being pressed/moving in another direction
        Deacceleration();
        LineForPrediction();
        //zeros out the direction for this frame so that a new one can be added
        direction = new Vector3(0, 0, 0);
        
        MovementInput();
        
        //caps speed to current numbers. Should pass through a new number any time the player changes tiered speed.
        SpeedCap(topSpeedXAndZ + slopeSpeedCap);
        MeteorMode(meteorState);
        Debug.Log(meteorState);
    }

    void OnCollisionEnter(Collision collide)
    {
        //Arcade boosters

        if(collide.transform.tag == "Boost")
        {

            //get the forward vector, add force that way
            Vector3 boostDir = direction.normalized * 100;

            velocityToBe += boostDir;
        }

        if(collide.transform.tag == "Break" && meteorState)
        {
            Destroy(collide.gameObject);
        }

        //if player goes over speed pads, then change acceleration for a limited amount of time.

    }
}
