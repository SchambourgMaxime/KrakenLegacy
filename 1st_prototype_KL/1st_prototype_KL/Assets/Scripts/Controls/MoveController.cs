using UnityEngine;
using System.Collections;

public class MoveController : MonoBehaviour {

    public Vector2 StartPosition;

   // public Vector3 collideScaling = new Vector3(1.0f, 1.0f, 0);
    public Vector3 ratioColliding = Vector3.zero;
    public float percentAccelLost = 0f;
    public bool impactAccelLoss = false;
    public bool cancelDeviationCollision = false;
    public bool cancelAccelCollision = false;

    public int   timeToMaxSpeed = 1500; // en ms
    public float maxSpeed = 10.0f;
    public float inputAcceleration = 1.0f;
    public float turnRate = 0.5f;
    public float percentageDrag = 0.9f; // ne pas dépasser 1
    public float waterFrictionVelocity = 3.0f;
    public float ratioFrictionAccel = 0.95f;
    public float flipAngleTurnAround = 0.1f;
    public float flipPercentageDrag = 0.4f;
    public float flipPercentageAccelerationRemaining = 0.7f;
    public float dashForce = 1700.0f;
    public int   dashDuration = 750; //en ms
    public float dashMaxSpeed = 65.0f;
    public int dashCD = 250; //ms
    public float dashTurnRate = 0.05f;
    public float ratioDeceleration = 0.95f; //higher = smoother deceleration (SHOULD BE < 1)
    public float baseMaxSpeed; //stores the base max speed (because we change it during the dashes)

    private float baseMaxSpeedInitiale = 10.0f; //Utilisé par les algues, permet de savoir quelle force leur appliquer suivant la vitesse de pulpy
    public float BaseMaxSpeedInitiale
    {
        get { return baseMaxSpeedInitiale; } 
        set { baseMaxSpeedInitiale = value; } 
    }

    public float valueBeforeNull = 0.05f; // bad name

    public Vector3 currentAcceleration = Vector3.zero;

    public float waitedAccel = 0.0f;
    public float waitedVeloc = 0.0f;

    private float maxAcceleration;
    private int dashTimer = 0; //it should be at 0 when the game starts, then it gets reset everytime the player dashes
    private bool isDashing = false;
    private bool isDecelerating = false;
    public bool isDashUnlocked = false;

    public float angleMinRotation = 0.3f; // entre 0 et 1, plus c'est proche de 1 et plus l'angle est important


    //Debug a enlever
    public int i = 0;

    public float veloc;

    //private 

    private Rigidbody rb;

    private float baseTurnRate; //stores the base turnrate (because we change it during the dashes)

    private bool canBeKnockedBack = true;
    private Animator anim;

    private bool isPlayer = false;
    private float ratioAccel = 1.0f;





    // Use this for initialization
    void Start() {

        init();
        //transform.position = new Vector3(StartPosition.x, StartPosition.y, -1.0f);

        if (GetComponent<PlayerController>())
            isPlayer = true;

    }

    public void init()
    {

        anim = GetComponentInChildren<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();

        // need to fix; time value under 1000 break
        maxAcceleration = maxSpeed / ((float)timeToMaxSpeed / 1000.0f);

        baseMaxSpeed = maxSpeed;
        baseTurnRate = turnRate;

        //Pourrait ne pas être la bonne valeur a cause du checkpoint ?
        //baseMaxSpeedInitiale = maxSpeed;

    }

    void Rotate(float moveAlongX, float moveAlongY)
    {
        Vector3 projectedRot = new Vector3(moveAlongX, moveAlongY, 0.0f);
        projectedRot.Normalize();

        //Debug.Log(projectedRot);
        Quaternion differenceRot = Quaternion.FromToRotation(transform.up, projectedRot);

        if ((differenceRot.x > 0.8 || differenceRot.y > 0.8 || differenceRot.z > 0.8)&&(!isDashing)&&(!isDecelerating))
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, -Mathf.Atan2(moveAlongX, moveAlongY) * Mathf.Rad2Deg);


              currentAcceleration = currentAcceleration * flipPercentageAccelerationRemaining;
              rb.velocity = rb.velocity * flipPercentageDrag;

        }
        else
        { //rotation normale
                 //  ++test;
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, -Mathf.Atan2(moveAlongX, moveAlongY) * Mathf.Rad2Deg, turnRate);
            //Debug.Log(differenceRot.eulerAngles + " test " + test);
            //Debug.Log("current angle   :" + transform.eulerAngles);
            //Debug.Log("projected angle : " + Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle);
        }
    }


    // moveAlongX et  moveAlongY entre -1 et 1
    public void Move(float moveAlongX, float moveAlongY)
    {
        if (maxSpeed > baseMaxSpeed && isDecelerating) 
        {
            LowerMaxSpeed();
        }

        if(baseMaxSpeed != maxSpeed && (!isDashing && !isDecelerating))
        {
            maxSpeed = baseMaxSpeed;
            maxAcceleration = maxSpeed / ((float)timeToMaxSpeed / 1000.0f);
        }

        if (moveAlongX != 0 || moveAlongY != 0) 
        {
            // MOVEMENT

            Rotate(moveAlongX, moveAlongY);

            UpdateAcceleration(moveAlongX, moveAlongY);

        } 
        else 
        {

            // INERTIA

            if (currentAcceleration != Vector3.zero || rb.velocity != Vector3.zero) {
                if (valueBeforeNull > Mathf.Abs(currentAcceleration.x) && valueBeforeNull > Mathf.Abs(currentAcceleration.y))
                    currentAcceleration = Vector3.zero;
                else
                    currentAcceleration = ratioFrictionAccel * currentAcceleration;


                if (valueBeforeNull > Mathf.Abs(rb.velocity.x) && valueBeforeNull > Mathf.Abs(rb.velocity.y))
                    rb.velocity = Vector3.zero;
                else
                    rb.AddForce(-(rb.velocity * waterFrictionVelocity));
                

            }

        }

        
        if(anim != null)
        {
            anim.SetFloat("Speed", rb.velocity.magnitude);
            anim.SetBool("IsDashing", isDashing);
        }

        

        if (isDashing) 
        {
            int diff = (int)(Time.realtimeSinceStartup * 1000) - dashTimer;
            if (diff >= dashDuration) 
            {
                isDashing = false;
                isDecelerating = true;
                //turnRate = baseTurnRate;
            }
        }


        if ((isDashUnlocked)&&(this.CompareTag("Player")) && (Input.GetButtonDown("Dash")) && !isDashing && (((int)(Time.realtimeSinceStartup * 1000) - dashTimer) >= dashCD))
        {
            Dash();
        }

        UpdateVelocity();

    }

    void LowerMaxSpeed() 
    {
        {
           if (maxSpeed * ratioDeceleration <= baseMaxSpeed) 
            {
                maxSpeed = baseMaxSpeed;
                isDecelerating = false;
                turnRate = baseTurnRate;
            }
               if(maxSpeed < baseMaxSpeed)
               {
                   maxSpeed = baseMaxSpeed;
                   isDecelerating = false;
                   turnRate = baseTurnRate;
               }
            else
                maxSpeed = maxSpeed * ratioDeceleration;
            //Debug.Log(maxSpeed);
        }
    }



    void UpdateAcceleration(float x, float y) 
    {

        if (!impactAccelLoss && cancelDeviationCollision)
            currentAcceleration = Vector3.zero;
        else {
            if (!cancelAccelCollision)
            {
                if (isPlayer)
                {

                    Vector2 calculRatio = new Vector2(x, y);

                    if (calculRatio.magnitude>1) // on fait en sorte de pas dépasser 1.
                        calculRatio.Normalize();

                    ratioAccel = calculRatio.magnitude;

                    //Debug.Log(ratioAccel);
                    //currentAcceleration += new Vector3(x, y, 0.0f) * inputAcceleration * ratioAccel;
                    currentAcceleration += new Vector3(x, y, 0.0f) * inputAcceleration;
                }
                else // pour tout les poissons et créatures concurrentes
                    currentAcceleration += new Vector3(x, y, 0.0f) * inputAcceleration; // comme avant
            }
            else
                currentAcceleration = Vector3.zero;
        }



        if (currentAcceleration.magnitude > maxAcceleration)
        {
            currentAcceleration.Normalize();
            currentAcceleration *= maxAcceleration;
        }

        //Ajout permettant de doser l'accélération avec le joystick
        if(isPlayer)
        {
            if (currentAcceleration.magnitude > maxAcceleration * ratioAccel)
            {
                Vector3 testAcceleration = currentAcceleration * ratioFrictionAccel;

                if (testAcceleration.magnitude > maxAcceleration * ratioAccel)
                    currentAcceleration = testAcceleration;
                else {
                    currentAcceleration.Normalize();
                    currentAcceleration *= maxAcceleration * ratioAccel;
                }
            }
        }


        if (ratioColliding != Vector3.zero)
        {

            Vector3 solutionPBSigne = Vector3.one;
            Vector3 newAccel;

            /*
            Debug.DrawLine(transform.position - transform.up * 3, transform.position + transform.up * 3, Color.green);
            Debug.DrawLine(transform.position - ratioColliding * 3, transform.position + ratioColliding * 3, Color.red);
             * */

            if (transform.up.x < -ratioColliding.x)
                solutionPBSigne.x = -1;  
            if (transform.up.y < -ratioColliding.y)
                solutionPBSigne.y = -1;

            if (!impactAccelLoss)
            {
                impactAccelLoss = true;
                waitedAccel = currentAcceleration.magnitude * (100.0f - percentAccelLost) / 100.0f;
                currentAcceleration.Normalize();
                currentAcceleration *= waitedAccel;
            }

            if (!cancelDeviationCollision)
            {
                newAccel = new Vector3(Mathf.Abs(ratioColliding.y), Mathf.Abs(ratioColliding.x), 0); //On inverse x et y car on veut aller a la parallele du mur !
                newAccel *= currentAcceleration.magnitude;
                newAccel = new Vector3(newAccel.x * solutionPBSigne.x, newAccel.y * solutionPBSigne.y, 0);
                currentAcceleration = newAccel;
            }

        }

        //Debug.DrawLine(transform.position, transform.position + currentAcceleration, Color.black);
    }

    void UpdateVelocity() 
    {
        
        rb.AddForce(currentAcceleration);

        //Ajout permettant de doser la vitesse avec le joystick
        if (isPlayer)
        {
            if (rb.velocity.magnitude > maxSpeed)
            {
                Vector3 testVitesse = rb.velocity;
                testVitesse.Normalize();
                rb.velocity = testVitesse * maxSpeed;
            }

            if (rb.velocity.magnitude > maxSpeed * ratioAccel)
            {
                rb.AddForce(-(rb.velocity * waterFrictionVelocity));
            }

        }
        else
        {
            if (rb.velocity.magnitude > maxSpeed * ratioAccel)
            {
                Vector3 testVitesse = rb.velocity;
                testVitesse.Normalize();
                rb.velocity = testVitesse * maxSpeed * ratioAccel;
            }
        }



        rb.velocity = (rb.velocity * percentageDrag) + ((transform.up * rb.velocity.magnitude) * (1 - percentageDrag));

        veloc = rb.velocity.magnitude;

        //Debug.DrawLine(transform.position, transform.position + rb.velocity, Color.grey);

    }



    void Dash() 
    {             
        dashTimer = (int)(Time.realtimeSinceStartup * 1000);
        rb.velocity = Vector3.zero;
        currentAcceleration = Vector3.zero;
        isDashing = true;
        maxSpeed = dashMaxSpeed;
        turnRate = dashTurnRate;
        //   rb.AddForce(rb.transform.TransformDirection(Vector3.up) * dashForce, ForceMode.Acceleration);
        rb.velocity = rb.transform.TransformDirection(Vector3.up) * dashForce;
        if (CompareTag("Player"))
            GetComponent<AudioController>().PlayDashSound();
    }

    public Vector3 GetVelocity() 
    {
        return rb.velocity;
    }

    public void Knockback(float knockbackForce, Vector3 hazardPosition) 
    {
        if (canBeKnockedBack) 
        {
            Vector3 direction = transform.position - hazardPosition;
            //direction.Normalize();

            rb.velocity = Vector3.zero;
            currentAcceleration = Vector3.zero;

            rb.AddForce(direction * knockbackForce, ForceMode.Impulse);

            isDashing = false;
            maxSpeed = baseMaxSpeed;
        }

        //Debug.Log(-direction * knockbackForce);
    }

    public void instantStop() 
    {
        rb.velocity = Vector3.zero;
        currentAcceleration = Vector3.zero;
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    public bool IsDecelerating()
    {
        return isDecelerating;
    }

    public void SetMaxSpeed(float speedToSet)
    {
        maxSpeed = speedToSet;
        baseMaxSpeed = speedToSet;
    }

    public void SetDashUnlocked()
    {
        isDashUnlocked = true;
    }

    public bool GetDashUnlocked() 
    {
        return isDashUnlocked;
    }

    public void setcanBeKnockedBack(bool _canBeKnockedBack)
    {
        canBeKnockedBack = _canBeKnockedBack;
    }
}
