using UnityEngine;

using System.Collections;

public class SteeringBehaviours : MonoBehaviour {

    public int maxForce = 150;
    public float mass = 100;
    public float gravity = 9.81f;
    public int maxRunningSpeed = 15;
    public int maxWalkSpeed = 5;
    public int rotateSpeed = 2;

    public string behaviour;//what this GO is are doing
    public string defaultBehaviour;//at startup
    public string behaviourAfterSomeRandomWandering;//what this GO is supposed to do but can't so wanders a bit instead

    public bool ObstacleAvoidanceOn;
    public bool SeparationOn;
    public bool CohesionOn;

    public float fleeForce = 100;
    public float fleeRadius = 25;
    public float arriveRadius = 5;
    public float arriveDamping = 6;
    public float wanderRadius = 10;
    public float wanderDistance = 1;
    private float wanderJitter;
    public float wanderJitterMin = 0.5f;
    public float wanderJitterMax = 2;
    private float tmrWander = 999;
    public float minDistToPathPoint = 3;
    private int indexOfCurrentPathPoint = 0;
    private float tmrTimeSpentOnPathPoint = 0;
    public float MaxTimeToSpendOnPathPoint = 8;
    private float tmrWanderSomeRandomTime = 999;
    private float randomWanderTime;
    public float randomTimeToWanderMin = 2;
    public float randomTimeToWanderMax = 5;
    private float tmrCheckForStuck = 0;
    public float checkForStuckTime = 5;
    public float separationDistance = 5;
    public float separationForce = 200;
    public float ObstacleAvoidanceDistance = 2;
    public float ObstacleAvoidanceForce = 200;
    public float ObstacleAvoidanceForceFactorDuringWander = 8;
    public float cohesionRadius = 30;
    public float cohesionForce = 200;
    public float alignmentRadius = 30;
    public float alignmentForce = 200;
    public float minShadowDistance = 10;
    public float maxShadowDistance = 50;
    
    
    public Vector3 velocity;
    public Vector3 lastVelocity;
    public Vector3 acceleration;
    public Vector3 steerForce;
    public Vector3 heading;
    public Vector3 wandertargetPosition;

    public Vector3 targetPosition;//where to go

    Transform WayPointContainer;
    Vector3 currentPathPoint;
    Vector3[] Path;

    CharacterController controller;//this GO's CharacterController

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();//this GO's CharacterController
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);//for the first time use
        /*WayPointContainer = GameObject.FindWithTag("WayPoints").transform;//for path finding, other paths also possible
        Path = new Vector3[WayPointContainer.GetChildCount()];//init Path
        for (int i = 0; i < WayPointContainer.GetChildCount(); i++)//fill in the locations in the Path array
        {
            Path[i] = WayPointContainer.GetChild(i).transform.position;
        }*/
        randomWanderTime = Random.Range(randomTimeToWanderMin, randomTimeToWanderMax);//for the first time use

	}

	// Update is called once per frame
	void Update () {
       
        //behaviour
        if (behaviour != "Idle")//when Idle, do nothing, not even Wander
        {
            steerForce = Vector3.zero;//start from scratch

            //check for stuck
            tmrCheckForStuck += Time.deltaTime;
            if (tmrCheckForStuck > checkForStuckTime)
            {
                tmrCheckForStuck = 0;//reset clock
                if ((lastVelocity - velocity).magnitude < 0.1)//still the same velocity => stuck
                {
                    if (behaviour != "WanderSomeRandomTime")//allready wandering some random time, no need to remember this
                    {
                        behaviourAfterSomeRandomWandering = behaviour;//remember this and switch back to this
                        behaviour = "WanderSomeRandomTime";//hopefully the GO will correct himself
                    }
                }
                else
                {
                    lastVelocity = velocity;//remember this velocity for next check
                }
            }

            //steering
            if (behaviour != "Follow Path")//reset the path finding, otherwise at a new Follow Path command, the GO will seek the last point from last Follow Path
            {
                currentPathPoint = Vector3.zero;
            }
            //actual steering
            if (behaviour == "Seek")
            {
                steerForce = Seek(targetPosition);
            }
            else if (behaviour == "Flee")
            {
                steerForce = Flee(targetPosition);
            }
            else if (behaviour == "Arrive")
            {
                steerForce = Arrive(targetPosition);
            }
            else if (behaviour == "Wander")
            {
                steerForce = Wander();
            } 
            else if (behaviour == "Flock") 
            {
                steerForce = Flock();
            }
            else if (behaviour == "WanderSomeRandomTime")//sometimes necessary
            {
                steerForce = Wander();//just Wander around
                tmrWanderSomeRandomTime += Time.deltaTime;//keep track of time
                if (tmrWanderSomeRandomTime > randomWanderTime)//long enough
                {
                    tmrWanderSomeRandomTime = 0;//reset clock
                    behaviour = behaviourAfterSomeRandomWandering;//switch back to the normal behaviour
                    randomWanderTime = Random.Range(randomTimeToWanderMin, randomTimeToWanderMax);//for the next time use
                }
            }
            else if (behaviour == "Follow Path")
            {
                steerForce = FollowPath(Path);
            }
            else if (behaviour == "Shadow")
            {
                steerForce = Shadow(targetPosition);
            }

            //options i.e. corrections for the steering
            if (SeparationOn)
            {
                //adjust steering for separation so the GO won't bump into each other
                steerForce += Separation();
            }

            if (ObstacleAvoidanceOn)
            {
                //adjust steering for walls etc.
                steerForce += ObstacleAvoidance();
            }

            if (CohesionOn)
            {
                //adjust steering for cohesion so GO's flock together
                steerForce += Cohesion();
            }

            //calc movement
            Truncate(ref steerForce, maxForce);// not > max
            acceleration = steerForce / mass;
            velocity += acceleration;//velocity = transform.TransformDirection(velocity);
            heading = velocity.normalized;

            if (behaviour == "Wander" || behaviour == "WanderSomeRandomTime")
            {
                Truncate(ref velocity, maxWalkSpeed);// not > max
            }
            else
            {
                Truncate(ref velocity, maxRunningSpeed);// not > max
            }
            
            //move
            if (controller.isGrounded)
            {
                controller.Move(velocity * Time.deltaTime);//move
                
            }
            else
            {
                controller.Move(new Vector3(0, -gravity * Time.deltaTime, 0));//fall down
            }

            //rotate
            if (new Vector3(velocity.x, 0, velocity.z) != Vector3.zero)//otherwise warning
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(velocity.x, 0, velocity.z)), rotateSpeed * Time.deltaTime);
            }
        }
	}


    public Vector3 Seek(Vector3 seekPosition)
    {
        Vector3 mySteeringForce = (seekPosition - transform.position).normalized * maxForce;//look at target direction, normalized and scaled
        Debug.DrawLine(transform.position, seekPosition, Color.green);
        return mySteeringForce;
    }

    public Vector3 Flee(Vector3 fleePosition)
    {
        float distance = Vector3.Distance(transform.position, fleePosition);//distance to target
        Vector3 mySteeringForce = Vector3.zero;
        if (distance < fleeRadius)//if close enough
        {
            mySteeringForce = (transform.position - fleePosition).normalized * fleeForce / distance;//look away from target direction, normalized and scaled
            Debug.DrawLine(transform.position, fleePosition, Color.red);
        }
        return mySteeringForce;
    }

    public Vector3 Flock() {
        Vector3 mySteeringForce = Cohesion() + Separation() + Alignment();
        return mySteeringForce;
    }

    public Vector3 Arrive(Vector3 arrivePosition)
    {
        float distanceToTarget = Vector3.Distance(arrivePosition, transform.position);//calc distance
        float scaleFactor = 0;//for scaling the steering
        Vector3 mySteeringForce;//to return
        if (distanceToTarget > arriveRadius)//decrease acceleration
        {
            scaleFactor = maxForce * ((distanceToTarget - arriveRadius) / distanceToTarget);   
        }
        else//come to halt
        {
            scaleFactor = 0;//no more accelerations
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * arriveDamping);//go to zero in some time
        }
        mySteeringForce = (arrivePosition - transform.position).normalized * scaleFactor;//look at target direction, normalized and scaled
        Debug.DrawLine(transform.position, arrivePosition, Color.blue);
        return mySteeringForce;
    }

    public Vector3 Wander()
    {
        tmrWander += Time.deltaTime;//keep track of time
        if (tmrWander > wanderJitter || Vector3.Distance(wandertargetPosition, transform.position) < 1)//waited long enough or close enough
        {
            tmrWander = 0;//reset tmr
            wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);//new random time
            heading = velocity.normalized;//where we're going
            //where we are + forward + random
            wandertargetPosition = transform.position + transform.forward * wanderDistance + Random.onUnitSphere * wanderRadius;
            wandertargetPosition.y = transform.position.y;//same height
        }
        Vector3 mySteeringForce = (wandertargetPosition - transform.position).normalized * maxForce;//look at pointOnCircle, normalized and scaled
        Debug.DrawLine(transform.position, wandertargetPosition, Color.yellow);
        return mySteeringForce;
    }

    public Vector3 FollowPath(Vector3[] myPath) 
    {
        tmrTimeSpentOnPathPoint += Time.deltaTime;//keep track of time
        //if no currentPathPoint is selected, pick closest one
        if (currentPathPoint == Vector3.zero)//no currentPathPoint selected, find closest one
        {
            indexOfCurrentPathPoint = 0;
            for (int i = 0; i < myPath.Length; i++)//go through all the waypoints
            {
                if (!Physics.Linecast(transform.position, myPath[i]))//returns NOT true if there is any collider intersecting the line between start and end
                {
                    float distanceToWayPoint = Vector3.Distance(transform.position, myPath[i]);//calc distance
                    if (distanceToWayPoint <= Vector3.Distance(transform.position, myPath[indexOfCurrentPathPoint]))//a closer waypoint found
                    {
                        currentPathPoint = myPath[i];//select the closer one
                        indexOfCurrentPathPoint = i;
                        tmrTimeSpentOnPathPoint = 0;//reset clock for this point
                    }
                }
            }
            if (currentPathPoint == Vector3.zero)//no currentPathPoint selected because none was visible => wander
            {
                behaviour = "WanderSomeRandomTime";//hopefully the GO will correct himself
                behaviourAfterSomeRandomWandering = "Follow Path";//remember this and switch back to this
            }
        }
        else if (Vector3.Distance(transform.position, currentPathPoint) < minDistToPathPoint)//if close enough pick next one
        {
            tmrTimeSpentOnPathPoint = 0;//reset clock for this point
            indexOfCurrentPathPoint++;//increase index
            if (indexOfCurrentPathPoint == myPath.Length)//set to the first one if out of bounds
            {
                indexOfCurrentPathPoint = 0;
            }
            currentPathPoint = myPath[indexOfCurrentPathPoint];//pick the next one
        }
        if (tmrTimeSpentOnPathPoint > MaxTimeToSpendOnPathPoint)//spend a lot of time on the current point, maybe stuck => wander around
        {
            tmrTimeSpentOnPathPoint = 0;//reset clock for this point
            behaviour = "WanderSomeRandomTime";//hopefully the GO will correct himself
            behaviourAfterSomeRandomWandering = "Follow Path";//remember this and switch back to this
        }
        //go to currentPathPoint like seek
        return Seek(currentPathPoint);
    }

    public Vector3 Shadow(Vector3 shadowPosition)
    {
        //hold everything
        Vector3 mySteeringForce = Vector3.zero;

        //to far => Seek
        if(Vector3.Distance(transform.position, shadowPosition) > maxShadowDistance){
            mySteeringForce = Seek(shadowPosition);
        }
        //to close => Flee
        else if (Vector3.Distance(transform.position, shadowPosition) < minShadowDistance)
        {
            mySteeringForce = Flee(shadowPosition);
        }
        else //slow down
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * arriveDamping);//go to zero in some time
        }
        Debug.DrawLine(transform.position, shadowPosition, Color.magenta);
        return mySteeringForce;
    }

    public Vector3 Separation()
    {
        Vector3 mySteeringForce = Vector3.zero;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(transform.tag);
        for (int i = 0; i < enemies.Length; i++)
        {
            //add a force away for each enemy ~ 1/r, far away enemies yield small force
            float dist = Vector3.Distance(transform.position, enemies[i].transform.position);
            if (dist < separationDistance && dist != 0)//close enough to take into account, but not 0, because we will divide by it, dist = 0 means this GO
            {
                mySteeringForce += (transform.position - enemies[i].transform.position).normalized * separationForce / dist;
            }
        }        
        Debug.DrawRay(transform.position, mySteeringForce, Color.cyan);
        return mySteeringForce;
    }

    public Vector3 ObstacleAvoidance()
    {
        //two ray system, like antenna's
        //one extra ray for the middle
        Vector3 mySteeringForceL = Vector3.zero;
        Vector3 mySteeringForceR = Vector3.zero;
        Vector3 mySteeringForceM = Vector3.zero;
        Vector3 directionL = 2 * transform.forward - transform.right;
        Vector3 directionR = 2 * transform.forward + transform.right;
        Vector3 directionM = transform.forward;
        RaycastHit hitR;
        RaycastHit hitL;
        RaycastHit hitM;
        //check for obstacle
        bool hitObstacleOnTheLeft = DetectObstacle(directionL, out hitL, ObstacleAvoidanceDistance);
        bool hitObstacleOnTheRight = DetectObstacle(directionR, out hitR, ObstacleAvoidanceDistance);
        bool hitObstacleInTheMiddle = DetectObstacle(directionM, out hitM, ObstacleAvoidanceDistance);
        //if found => can go over it?
        if (hitObstacleOnTheLeft || hitObstacleOnTheRight ||hitObstacleInTheMiddle) //obstacle found
        {
            if (CheckIfCanGoOverIt())//can go over obstacle? 
            {
                //climb
                return Vector3.zero;//no steering force to use for avoidance 
            }
            else //avoid
            {
                //calc forces for each direction
                if (hitObstacleOnTheLeft)
                {
                    mySteeringForceL = CalcAvoidanceForce(hitL);
                }
                if (hitObstacleOnTheRight)
                {
                    mySteeringForceR = CalcAvoidanceForce(hitR);
                }
                if (hitObstacleInTheMiddle)
                {
                    mySteeringForceM = CalcAvoidanceForce(hitM);
                }
                //sum them
                if (mySteeringForceL != Vector3.zero && mySteeringForceR != Vector3.zero && mySteeringForceM == Vector3.zero)
                {//possible narrow pathway
                    return Vector3.zero;//keep on going 
                }
                else if ((behaviour == "Wander" || behaviour == "WanderSomeRandomTime") && (mySteeringForceL != Vector3.zero || mySteeringForceR != Vector3.zero || mySteeringForceM != Vector3.zero))
                {//less force during a sort of wander
                    tmrWander = randomTimeToWanderMin;//less twurling that way, pick a new wander destination
                    return ObstacleAvoidanceForceFactorDuringWander * (mySteeringForceL + mySteeringForceR + mySteeringForceM);//a stronger avoidance
                }
                else
                {//full force
                    return ObstacleAvoidanceForce * (mySteeringForceL + mySteeringForceR + mySteeringForceM);//just return the sum of all three
                }
            }
        }
        return Vector3.zero;//no steering force to use for avoidance because no abstacle was detected
    }

    private bool CheckIfCanGoOverIt()
    {
        return false;
    }

    private bool DetectObstacle(Vector3 myDirection, out RaycastHit myHit, float myObstacleAvoidanceDistance)
    {
        if (Physics.Raycast(transform.position, myDirection, out myHit, ObstacleAvoidanceDistance))//raycast
        {
            Debug.DrawLine(transform.position, myHit.point, Color.grey);
            return true;
        }
        return false;
        
    }

    private Vector3 CalcAvoidanceForce(RaycastHit myHit)
    {
        Vector3 mySteeringForce = Vector3.zero;//a zero force
        mySteeringForce += (transform.position - myHit.point).normalized * ObstacleAvoidanceForce / myHit.distance;//calc force
        return mySteeringForce;
    }

    private Vector3 Cohesion()
    {
        Vector3 centerOfMass = Vector3.zero;
        //get all neighbours
        int neighbours = 0;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(transform.tag);//GO with the same tag as this GO
        for (int i = 0; i < enemies.Length; i++)
        {
            if (!Physics.Linecast(transform.position, enemies[i].transform.position))//returns NOT true if there is any collider intersecting the line between start and end
            {
                if (Vector3.Distance(transform.position, enemies[i].transform.position) < cohesionRadius)//close enough              
                {
                    centerOfMass += enemies[i].transform.position;//adjust position of center of flock
                    neighbours++;//count the neighbours
                }
            }           
        }
        if (neighbours != 0)//some neighbours
        {
            centerOfMass /= neighbours;
        }
        else //no neighbour
        {
            centerOfMass = transform.position;
        }
        //steer towards centerOfMass
        float scaleFactor = cohesionForce * Vector3.Distance(transform.position, centerOfMass);//further away, more pull towards center
        Vector3 mySteeringForce = (centerOfMass - transform.position).normalized * scaleFactor ;//look at target direction, normalized and scaled
        Debug.DrawLine(transform.position, centerOfMass, Color.black);
        return mySteeringForce;
    }
    private Vector3 Alignment() {
        Vector3 AverageHeading = Vector3.zero;

       int neighbours = 0;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(transform.tag);//GO with the same tag as this GO


        for (int i = 0; i < enemies.Length; i++) {
          //  if (!Physics.Linecast(transform.position, enemies[i].transform.position))//returns NOT true if there is any collider intersecting the line between start and end
           // {
                if (Vector3.Distance(transform.position, enemies[i].transform.position) < alignmentRadius)//close enough              
                {
                    //Ajout de la position ds le calcul
                    AverageHeading += enemies[i].GetComponent<SteeringBehaviours>().heading + enemies[i].transform.position;//adjust position of center of flock
                    Debug.Log(AverageHeading);
                    neighbours++;//count the neighbours
                }
           // }
        }
        if (neighbours != 0)//some neighbours
        {
            AverageHeading /= neighbours;
        } else //no neighbour
          {
            AverageHeading = transform.position;
        }
        //steer towards centerOfMass
        float scaleFactor = alignmentForce * Vector3.Distance(transform.position, AverageHeading);//further away, more pull towards center
        Vector3 mySteeringForce = (AverageHeading - transform.position).normalized * scaleFactor;//look at target direction, normalized and scaled
        Debug.DrawLine(transform.position, AverageHeading, Color.red);
        return mySteeringForce;
    }

    private void Truncate(ref Vector3 myVector, int myMax)//not above max
    {
        if (myVector.magnitude > myMax)
        {
            myVector.Normalize();// Vector3.normalized returns this vector with a magnitude of 1
            myVector *= myMax;//scale to max
        }
    }

    public void SetBehaviour(string newBehaviour)//manipulated by other scripts
    {
        behaviour = newBehaviour;
    }

    private void ToggleOption(string optionToAdjust)//manipulated by other scripts
    {
        if (optionToAdjust == "ObstacleAvoidance")
        {
            ObstacleAvoidanceOn = !ObstacleAvoidanceOn;
        }
        if (optionToAdjust == "Separation")
        {
            SeparationOn = !SeparationOn;
        }
        if (optionToAdjust == "Cohesion")
        {
            CohesionOn = !CohesionOn;
        }
        
    }

    private void SetPath(Vector3[] newPath)//manipulated by other scripts
    {
        Path = new Vector3[newPath.Length];//re-init Path

        for (int i = 0; i < newPath.Length; i++)//fill in the locations in the Path array
        {
            Path[i] = newPath[i];//copy every point
        }
        currentPathPoint = Vector3.zero;//so the Follow path behaviour will start from scratch and calc a new path point
    }

    private void SetTarget(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
    }

    
}
