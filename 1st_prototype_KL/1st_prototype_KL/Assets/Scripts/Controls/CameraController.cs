using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float cameraDistance = 10.0f;

    public float smoothTimeX;
    public float smoothTimeY;

    public float movementDistance = 20.0f;
    public float speedZoomOut;
    public float speedZoomIn;

    public float scalePercentage = 0.5f;

    public float timeBeforeZoomIn;

    private Vector2 velocity;

    private GameObject player;
    private float startTime = -1.0f;

    // Use this for initialization
    void Start () 
    {
        player = GameObject.FindGameObjectWithTag("Player");

        cameraDistance = -cameraDistance;
        movementDistance = -movementDistance;
    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {

        float actualCameraDistance = cameraDistance * (player.transform.localScale.x * scalePercentage);
        float actualMovementDistance = movementDistance * (player.transform.localScale.x * scalePercentage);

        float zDistance = actualCameraDistance;

        float posX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
        float posY = Mathf.SmoothDamp(transform.position.y, player.transform.position.y, ref velocity.y, smoothTimeY);

        if(player.GetComponent<MoveController>().GetVelocity() != Vector3.zero)
        {
            if (transform.position.z >= (actualMovementDistance + 0.1f))
                zDistance = Mathf.Lerp(transform.position.z, actualMovementDistance, speedZoomOut * player.GetComponent<MoveController>().GetVelocity().magnitude);
            else
                zDistance = actualMovementDistance;
        }
        else
        {
            if(transform.position.z < (actualCameraDistance - 0.001f * player.transform.localScale.x))
            {
                if (startTime == -1.0f)
                {
                    startTime = Time.realtimeSinceStartup;
                    zDistance = transform.position.z;
                }
                if (Time.realtimeSinceStartup - startTime >= timeBeforeZoomIn)
                    zDistance = Mathf.Lerp(transform.position.z, actualCameraDistance, speedZoomIn);
                else
                    zDistance = transform.position.z;
            }
            else
            {
                startTime = -1.0f;
            }

        }

        //Debug.Log(transform.position.z);

        transform.position = new Vector3(posX, posY, zDistance);

    }
}
