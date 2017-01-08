using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private MoveController moveController;
    private CreatureController creatureController;
    private bool one = true;
    public GameObject guide;
    // 
    void Start()
    {
        moveController = GetComponent<MoveController>();
        creatureController = GetComponent<CreatureController>();
    }

    // Moving
    void FixedUpdate()
    {
        float moveAlongY = Input.GetAxis("Vertical");
        float moveAlongX = Input.GetAxis("Horizontal");

        if (one && Input.GetButtonDown("Fire3"))
        {
            one = false;
            Invoke("despawn", 11);
            GameObject guidePrefab = (GameObject)Instantiate(guide, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
        }

        moveController.Move(moveAlongX, moveAlongY);
    }

    public void despawn()
    {
        one = true;
    }
}
