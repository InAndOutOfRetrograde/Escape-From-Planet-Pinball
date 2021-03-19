using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallMovementScript : MonoBehaviour
{
    //GENERIC VARS//
    Vector3 v3ForceUp = new Vector3(0,0,1);
    Vector3 v3ForceRight = new Vector3(1,0,0);
    float XandZMagnitude;
    float YMagnitude;
    Vector3 velocityToBe;
    Vector3 direction = new Vector3(0, 0, 0);

    //SPEED VARIABLES//
    //Change decrease in speed, top speed vert and hori here.
    public float topSpeedReduction = .09f;
    public float topSpeedXAndZ = 100f;
    public float maxSpeedY = 90f; //find how to use later

    public float constantDeceleration = 0.1f;
    float currentDeceleration = 0f;
    public float acceleration = 20f;

    //TIERED SPEED//
    //Change acceleration, top speed, numerical tier here
    int newSpeedMax = 0;
    int newSpeedTier = 0;

    //TIMER VARS//
    //Vars used to limit movement things for a period of time.
    public float cooldownVar = 1f;
    public float movementCooldown;
    public bool movementAction = true;

    public bool grounded = true;
    public Vector3 slamSpeed = new Vector3(0,-100,0);

    //PREDICTION LINE//
    public Vector3 predictBallLoc;

    public Rigidbody rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        movementCooldown = cooldownVar;
    }

    void Deacceleration()
    {
        velocityToBe = rb.velocity;
        if (velocityToBe.x > 0f)
        {
            velocityToBe.x -= currentDeceleration;
            if (velocityToBe.x < 0f) velocityToBe.x = 0f;
        }
        if (velocityToBe.x < 0f)
        {
            velocityToBe.x += currentDeceleration;
            if (velocityToBe.x > 0f) velocityToBe.x = 0f;
        }
        if (velocityToBe.z > 0f)
        {
            velocityToBe.z -= currentDeceleration;
            if (velocityToBe.z < 0f) velocityToBe.z = 0f;
        }
        if (velocityToBe.z < 0f)
        {
            velocityToBe.z += currentDeceleration;
            if (velocityToBe.z > 0f) velocityToBe.z = 0f;
        }
    }

    void MovementInput()
    {
        //controls
        if (Input.GetKey("up") || Input.GetKey(KeyCode.W))
        {
            /*if (rb.velocity.z < 0)
            {
                rb.velocity += Vector3.Scale(v3ForceUp, new Vector3(1, 1, 2));
            }
            else
            {*/
            direction += v3ForceUp;
            //}
            currentDeceleration = 0f;
            velocityToBe.z = rb.velocity.z;
            
        }
        if (Input.GetKey("down") || Input.GetKey(KeyCode.S))
        {
            /*if(rb.velocity.z > 0)
            {
                rb.velocity -= Vector3.Scale(v3ForceUp, new Vector3(1, 1, 2));
            }
            else
            {*/
            direction -= v3ForceUp;
            //}
            currentDeceleration = 0f;
            velocityToBe.z = rb.velocity.z;
        }
        if (Input.GetKey("left") || Input.GetKey(KeyCode.A))
        {
            /*if (rb.velocity.x < 0)
            {
                rb.velocity -= Vector3.Scale(v3ForceRight, new Vector3(2, 1, 1));
            }
            else
            {*/
            direction -= v3ForceRight;
            //}
            currentDeceleration = 0f;
            velocityToBe.x = rb.velocity.x;
        }
        if (Input.GetKey("right") || Input.GetKey(KeyCode.D))
        {
            /*if(rb.velocity.x > 0)
            {
                rb.velocity += Vector3.Scale(v3ForceRight, new Vector3(2, 1, 1));
            }
            else
            {*/
            direction += v3ForceRight;
            //}
            currentDeceleration = 0f;
            velocityToBe.x = rb.velocity.x;
        }
        //butt slam.
        if (Input.GetKey(KeyCode.LeftControl))
        {
            //set unable to be pressed again until grounded has been true for 3 seconds.
            velocityToBe = slamSpeed;
            Debug.Log("holding left control");
        }
        if(Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        if (!Input.anyKey)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, -Vector3.up, out hit, 0.1f);
            Vector3 normal = hit.normal;

            float gradient = GradientMaker(normal) * 3;
            currentDeceleration += constantDeceleration; //*gradient
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

    //BROKEN
    float GradientMaker(Vector3 norm)
    {
        float angle = Vector3.Angle(norm, Vector3.up);
        angle = Mathf.Rad2Deg*angle;
        if (angle < 0)
        {
            angle = Mathf.Abs(angle);
        }
        return angle;
    }

    void SpeedCap(float maxSpeedXAndZ)
    {
        //what if i made it so it slowed to top speed instead of immediately slowing to top speed?
        //speed cap
        Vector3 adjustedVelocity = rb.velocity;
        XandZMagnitude = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
        if (XandZMagnitude > maxSpeedXAndZ)
        {
            float diff = XandZMagnitude - maxSpeedXAndZ;
            diff *= topSpeedReduction;
            Vector2 xAndZNormalized = new Vector2(rb.velocity.x, rb.velocity.z).normalized * (maxSpeedXAndZ + diff);
            adjustedVelocity.x = xAndZNormalized.x;
            adjustedVelocity.z = xAndZNormalized.y;
        }
        if (rb.velocity.y > Mathf.Abs(maxSpeedY)) Debug.Log("MAXED OUT SPEED");
        adjustedVelocity.y = Mathf.Clamp(adjustedVelocity.y, -maxSpeedY, maxSpeedY);

        rb.velocity = adjustedVelocity;
    }
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

    public int GetTier(int tierOfSpeed)
    {
        //i need a new top speed
        //select between 4 tiers of speed (respresented by ints 1-4)
        //depending on tier, it returns a new top speed.
        newSpeedMax = 0;
        if (tierOfSpeed == 1)
        {
            newSpeedMax = 30;
            newSpeedTier = 1;
            //create a method that handles all of this and puts a timer on it if necessary.
        }
        if (tierOfSpeed == 2)
        {
            newSpeedMax = 60;
            newSpeedTier = 2;
        }
        if (tierOfSpeed == 3)
        {
            newSpeedMax = 90;
            newSpeedTier = 3;
        }
        if (tierOfSpeed == 4)
        {
            newSpeedMax = 120;
            newSpeedTier = 4;
        }
        return newSpeedMax;
    }

    public void MovementChange()
    {
        movementAction = false;
        movementCooldown = cooldownVar;
    }

    void FixedUpdate()
    {
        isGrounded();
        //if(grounded == false)
        //{
        LineForPrediction();
        //}
        //timer that is used to see when the player is free to move again/when deacceleration should slow them
        Timer();
        //slows the player every frame that a button is not being pressed/moving in another direction
        Deacceleration();
        //zeros out the direction for this frame so that a new one can be added
        direction = new Vector3(0, 0, 0);
        //moves the player based on input if they are allowed to move
        //if (movementAction == true)
        //{
            MovementInput();
        //}
        //caps speed to current numbers. Should pass through a new number any time the player changes tiered speed.
        //if(movementAction == false)
        //{
            SpeedCap(topSpeedXAndZ);
        //}
        
        //timer that lowers speed by tier

    }

    //code for changing stuff on the player based on what it has collided into recently.//

    void OnCollisionEnter(Collision collide)
    {
        //breaking?
        //
        //{
        //MovementChange();
        //tier changer
        /*foreach (MonoBehaviour script in collide.transform.GetComponents<MonoBehaviour>())
        {
            MonoBehaviour Script = gameObject.GetComponent(typeof(script)) as MonoBehaviour;
            if (Script is Machine)
            {
                GetTier(collide.transform.GetComponent<>().tierOfSpeedGive);
            }
        }*/
        //Arcade boosters
        if (collide.transform.tag == "Mover")
        {
            if(collide.transform.GetComponent<Redirector>() != null)
            {
                GetTier(collide.transform.GetComponent<Redirector>().tierOfSpeedGive);
            }
            else if (collide.transform.GetComponent<FlipperScript>() != null)
            {
                GetTier(collide.transform.GetComponent<FlipperScript>().tierOfSpeedGive);
            }
            else if (collide.transform.GetComponent<Trampoline>() != null)
            {
                GetTier(collide.transform.GetComponent<Trampoline>().tierOfSpeedGive);
            }
        }
        //Code for walls so you can't enter eareas until you have the proper tier
        if(collide.transform.tag == "Restrictor")
        {
            //run code to make it so you can't move through certain things without having achieved a certain tier. 
            //glow red, give feedback.
        }
        if(collide.transform.tag == "SpeedArch")
        {
            if(collide.transform.GetComponent<SpeedArch>() != null)
            {
                GetTier(collide.transform.GetComponent<SpeedArch>().tierOfSpeedGive);
            }
        }
        //}

    }
}
