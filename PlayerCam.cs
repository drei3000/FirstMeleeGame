using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    public Camera cam;
    
    float xRotation;
    float yRotation;
    public float normalFOV;
    public float sprintFOV;
    public float fovSpeed;

    private float targetFOV;
    private PlayerMovement playerMovement; // reference to PlayerMovement

    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerMovement = FindObjectOfType<PlayerMovement>();
        targetFOV = normalFOV;
    }

 
    void Update()
    {
        handleMouseLook();
        UpdateFOV();
    }

    void handleMouseLook()
    {
        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    // My own code
    void UpdateFOV()
    {
        if (playerMovement != null)
        {
            targetFOV = playerMovement.sprinting ? sprintFOV : normalFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);
        }
    }
}
