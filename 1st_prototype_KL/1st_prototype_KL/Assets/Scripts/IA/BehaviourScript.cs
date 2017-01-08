using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BehaviourScript : MonoBehaviour
{



    public GameObject roomOrigine;

    public int maxForce = 150;
    public int mass = 50;
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
    private float turnRate = 5;
    private float rotate = 180;

    //private Quaternion targetRotation;
    //private Quaternion previousRotation;

    public float randomTimeToWanderMin = 2;
    public float randomTimeToWanderMax = 5;

    private float tmrCheckForStuck = 0;
    public float checkForStuckTime = 5;

    public float ObstacleAvoidanceDistance = 2;
    public float ObstacleAvoidanceForce = 15;
    public float ObstacleAvoidanceForceFactorDuringWander = 8;

    public float cohesionForce = 200;
    public float alignmentForce = 200;
    public float separationForce = 200;
    public float separationDistance = 5;

    List<GameObject> neighbouringAgents = new List<GameObject>();
    int neighboursCount;
    public float neighboursRadius = 30;

    public Vector3 velocity;
    public Vector3 lastVelocity;
    public Vector3 acceleration;
    public Vector3 steerForce;
    public Vector3 heading;
    public Vector3 wandertargetPosition;

    private Transform fishMesh;

    public Vector3 targetPosition;//where to go

    bool left = false;
    bool rotation = false;
    bool rotationLocked = false;

    private bool needNeighbours;

    Transform WayPointContainer;
    Vector3 currentPathPoint;
    public Vector3[] Path;

    MoveController moveController;

    // Use this for initialization
    void Start()
    {
        moveController = GetComponent<MoveController>();
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);//for the first time use
        randomWanderTime = Random.Range(randomTimeToWanderMin, randomTimeToWanderMax);//for the first time use
        getNeighbours();

        //Transform fishMesh = this.gameObject.transform.GetChild(0);
        //if (fishMesh.name == "Fish_Mesh")
        //{
        //    targetRotation = new Quaternion(0, 0, 0, 0);
        //}
    }

    // Update is called once per frame
    void Update()
    {

        //behaviour

        if (behaviour != "Idle")//when Idle, do nothing, not even Wander
        {
            //steerForce = Vector3.zero;//start from scratch

            //check for stuck
            /*tmrCheckForStuck += Time.deltaTime;
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
            }*/

            //steering

            if (behaviour != "Follow Path")//reset the path finding, otherwise at a new Follow Path command, the GO will seek the last point from last Follow Path
            {
                needNeighbours = false;
                currentPathPoint = Vector3.zero;
            }
            //actual steering
            if (behaviour == "Seek")
            {
                needNeighbours = false;
                steerForce = Seek(targetPosition);
            }
            else if (behaviour == "Flee")
            {
                needNeighbours = false;
                steerForce = Flee(targetPosition);
            }
            else if (behaviour == "Arrive")
            {
                needNeighbours = false;
                steerForce = Arrive(targetPosition);
            }
            else if (behaviour == "Wander")
            {
                needNeighbours = false;
                steerForce = Wander();
            }
            else if (behaviour == "WanderSomeRandomTime")//sometimes necessary
            {
                needNeighbours = false;
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
                needNeighbours = false;
                steerForce = FollowPath(Path);
            }
            else if (behaviour == "Flock")
            {
                if(!needNeighbours)
                {
                    getNeighbours();
                }
                needNeighbours = true;
                steerForce = Flock();
            }
            /*else
            {
                Debug.Log("Steering Behaviours inconnu");
            }*/

            //options i.e. corrections for the steering
            if (SeparationOn)
            {
                //adjust steering for separation so the GO won't bump into each other
                steerForce += separation();
            }

            if (CohesionOn)
            {
                //adjust steering for cohesion so GO's flock together
                steerForce += cohesion();
            }

            if (ObstacleAvoidanceOn)
            {
                //adjust steering for walls etc.
                steerForce += ObstacleAvoidance();
            }
            //}
            //calc movement
            Truncate(ref steerForce, maxForce);// not > max
            acceleration = steerForce / mass;
            acceleration.z = 0;
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
            /* TESTER SI DIFFERENCE
            //moveController.Move(Mathf.Clamp(velocity.x, -1.0f, 1.0f), Mathf.Clamp(velocity.y, -1.0f, 1.0f));
            */
            moveController.Move(velocity.x, velocity.y);

            if (!rotationLocked)
            {
                if (!rotation)
                {
                    if (Vector3.Dot(Vector3.right, velocity) < -0.9 && !left)
                    {
                        left = true;
                        //fishMesh.rotation *= Quaternion.Euler(0, 0, 180);
                        rotation = true;
                    }
                    else if (Vector3.Dot(Vector3.right, velocity) > 0.9 && left)
                    {
                        left = false;
                        //fishMesh.rotation *= Quaternion.Euler(0, 0, 180);
                        rotation = true;
                    }
                }
                else
                {
                    rotationZ();
                }
            }
            else
            {
                if (rotation)
                {
                    rotate = 180;
                    rotation = false;
                }
            }
        }
    }

    private void rotationZ()
    {
        if (rotate > 0)
        {
            fishMesh = this.gameObject.transform.Find("Fish_Mesh");
            if (fishMesh)
            {
                fishMesh.transform.Rotate(0, 0, turnRate);
                rotate -= turnRate;
            }           
        }
        else
        {
            rotate = 180;
            rotation = false;
        }
    }

    public void setBehaviour(string newBehaviour)
    {
        behaviour = newBehaviour;
    }

    public Vector3 Seek(Vector3 seekPosition)
    {
        Vector3 mySteeringForce = (seekPosition - transform.position).normalized * maxForce;//look at target direction, normalized and scaled
        //Debug.DrawLine(transform.position, seekPosition, Color.green);
        return mySteeringForce;
    }

    public Vector3 Flee(Vector3 fleePosition)
    {
        float distance = Vector3.Distance(transform.position, fleePosition);//distance to target
        Vector3 mySteeringForce = Vector3.zero;
        if (distance != 0 && distance < fleeRadius)//if close enough
        {
            mySteeringForce = (transform.position - fleePosition).normalized * fleeForce;//look away from target direction, normalized and scaled
            //Debug.DrawLine(transform.position, fleePosition, Color.red);
        }
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
            GetComponent<MoveController>().instantStop();
            //scaleFactor = 0;//no more accelerations
            //velocity = Vector3.Lerp(velocity, Vector3.zero, 0.7f);//go to zero in some time
        }
        mySteeringForce = (arrivePosition - transform.position).normalized * scaleFactor;//look at target direction, normalized and scaled
        //Debug.DrawLine(transform.position, arrivePosition, Color.blue);
        return mySteeringForce*100;
    }

    public Vector3 Wander()
    {
        tmrWander += Time.deltaTime;//keep track of time
        Vector3 mySteeringForce;
        if (velocity != Vector3.zero)
        {
            int x = 20;
            int angle = Random.Range(-x, x);
            mySteeringForce = (Quaternion.Euler(0, 0, angle) * velocity).normalized * maxForce;
        }
        else
        {
            if (tmrWander > wanderJitter || Vector3.Distance(wandertargetPosition, transform.position) < 1)//waited long enough or close enough
            {
                tmrWander = 0;//reset tmr
                wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);//new random time
                heading = velocity.normalized;//where we're going
                //where we are + forward + random
                wandertargetPosition = transform.position + transform.forward * wanderDistance + Random.onUnitSphere * wanderRadius;
                wandertargetPosition.z = transform.position.z;//same depth
            }
            mySteeringForce = (wandertargetPosition - transform.position).normalized * maxForce;//look at pointOnCircle, normalized and scaled
            //Debug.DrawLine(transform.position, wandertargetPosition, Color.yellow);
        }
        return mySteeringForce;
    }

    public void setPath(Vector3[] newPath)
    {
        Path = newPath;
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
                if (gameObject.tag == "Untagged" || !Physics.Linecast(transform.position, myPath[i]))//returns NOT true if there is any collider intersecting the line between start and end
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
            /*if (indexOfCurrentPathPoint == myPath.Length)//set to the first one if out of bounds
            {
                indexOfCurrentPathPoint = 0; //loop
            }
            currentPathPoint = myPath[indexOfCurrentPathPoint];//pick the next one
            */
            if (indexOfCurrentPathPoint == myPath.Length)//set to the first one if out of bounds
            {
                if (gameObject.tag != "Untagged")
                {
                    behaviour = "Wander"; //wander after follow path
                }
            }
            else
            {
                if (indexOfCurrentPathPoint < myPath.Length)
                {
                    currentPathPoint = myPath[indexOfCurrentPathPoint];//pick the next one
                }
            }
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

    public Vector3 ObstacleAvoidance()
    {
        //two ray system, like antenna's
        //one extra ray for the middle
        Vector3 mySteeringForce = Vector3.zero;
        Vector3 directionL = 2 * velocity - transform.right;
        Vector3 directionR = 2 * velocity + transform.right;
        RaycastHit hitR;
        RaycastHit hitL;
        //check for obstacle
        bool hitObstacleOnTheLeft = DetectObstacle(directionL, out hitL, ObstacleAvoidanceDistance);
        bool hitObstacleOnTheRight = DetectObstacle(directionR, out hitR, ObstacleAvoidanceDistance);

        if (hitObstacleOnTheLeft)
        {
            mySteeringForce = transform.right * ObstacleAvoidanceForce;
            return mySteeringForce;
        }
        else if (hitObstacleOnTheRight)
        {
            mySteeringForce = -transform.right * ObstacleAvoidanceForce;
            return mySteeringForce;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private bool DetectObstacle(Vector3 myDirection, out RaycastHit myHit, float myObstacleAvoidanceDistance)
    {
        if (Physics.Raycast(transform.position, myDirection, out myHit, ObstacleAvoidanceDistance))//raycast
        {
            //Debug.DrawLine(transform.position, myHit.point, Color.grey);
            return true;
        }
        return false;
    }

    public Vector3 Flock()
    {
        Vector3 cohesionValue = cohesion();
        Vector3 alignmentValue = alignment();

        /*if(Vector3.Dot(cohesionValue,alignmentValue) > 0)
        {
            GetComponent<MoveController>().SetMaxSpeed(15);
        }
        else
        {
            GetComponent<MoveController>().SetMaxSpeed(10);
        }*/

        return alignmentValue * alignmentForce + cohesionValue * cohesionForce + separation() * separationForce;
    }

    Vector3 alignment()
    {
        Vector3 averageDirection = Vector3.zero;

        if (neighboursCount == 0)
            return averageDirection;

        foreach (var agent in neighbouringAgents)
        {
            if (agent && agent.GetComponent<BehaviourScript>() != null)
            {
                averageDirection += agent.GetComponent<BehaviourScript>().velocity;
            }
            else
            {
                //Debug.Log("Script non trouvé");
            }
        }

        averageDirection /= neighboursCount;
        return averageDirection.normalized;
    }

    Vector3 cohesion()
    {
        Vector3 averagePosition = Vector3.zero;

        foreach (var agent in neighbouringAgents)
        {
            if (agent)
            {
                averagePosition += (Vector3)agent.transform.position;
            }
        }

        averagePosition /= neighboursCount;

        return (averagePosition - transform.position).normalized;
    }

    Vector3 separation()
    {
        Vector3 moveDirection = Vector3.zero;

        foreach (var agent in neighbouringAgents)
        {
            if (agent && Vector3.Distance(transform.position, agent.transform.position) < separationDistance)//close enough              
            {
                moveDirection += (Vector3)agent.transform.position - transform.position;
            }
        }

        return (moveDirection * -1);
    }

    private void getNeighbours()
    {
        RoomManager room = gameObject.GetComponentInParent<RoomManager>();
        if (room)
        {
            List<GameObject> fishes = gameObject.GetComponentInParent<RoomManager>().GetFishesInRoom();
            neighbouringAgents.Clear();
            neighboursCount = 0;

            if (fishes != null)
            {
                foreach (var fish in fishes)
                {
                    if (fish && fish.tag == tag && Vector3.Distance(transform.position, fish.transform.position) < neighboursRadius && fish.transform.position != transform.position)
                    {
                        neighbouringAgents.Add(fish);
                        neighboursCount++;
                    }
                }
            }
        }

        if (needNeighbours)
        {
            Invoke("getNeighbours", 0.5f);
        }
    }

    private void Truncate(ref Vector3 myVector, int myMax)//not above max
    {
        if (myVector.magnitude > myMax)
        {
            myVector.Normalize();// Vector3.normalized returns this vector with a magnitude of 1
            myVector *= myMax;//scale to max
        }
    }

    private void SetBehaviour(string newBehaviour)//manipulated by other scripts
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

    public void SetTarget(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
    }

    public bool isLeft() { return left; }

    public void SetRotationLocked(bool b) { rotationLocked = b; }
}