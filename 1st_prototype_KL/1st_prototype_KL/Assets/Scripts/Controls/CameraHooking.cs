using UnityEngine;
using System.Collections;

public class CameraHooking : MonoBehaviour {

    public Transform cameraHook;
    public float transitionTime;

    private GameObject mainCamera;
    private float startTime;
    private bool isCameraHooked;
    private CameraController cameraController;
    private Transform mainCameraNormalPos;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCameraNormalPos = mainCamera.transform;
    }

    void Update()
    {
        float t = (Time.time - startTime) / transitionTime;
        if(isCameraHooked)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCameraNormalPos.position, cameraHook.position, t);
        }
    }

	void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if(isCameraHooked)
            {
                Unhooking();
            }
            else
            {
                Hooking();
            }
        }
    }

    public void Hooking()
    {
        startTime = Time.time;
        isCameraHooked = true;
        cameraController = mainCamera.GetComponent<CameraController>();
        cameraController.GetComponentInParent<AudioSource>().Stop();
        GetComponentInChildren<AudioSource>().Play();
        cameraController.enabled = false;
    }

    public void Unhooking()
    {
        startTime = Time.time;
        isCameraHooked = false;
        cameraController = mainCamera.GetComponent<CameraController>();
        cameraController.GetComponentInParent<AudioSource>().Play();
        GetComponentInChildren<AudioSource>().Stop();
        cameraController.enabled = true;
        mainCameraNormalPos = mainCamera.transform;
    }
}
