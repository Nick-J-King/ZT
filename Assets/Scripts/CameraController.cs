using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AzimuthElevation
{
    public float azimuth;
    public float elevation;
}

public class XYZ
{
    public float x;
    public float y;
    public float z;
}


public class CameraController : MonoBehaviour {

    public GameObject player;

    private float RotateAmount = 1.0f;

    public AzimuthElevation azimuthElevation;
    public XYZ xyz;


    void Start ()
    {
        azimuthElevation = new AzimuthElevation();
        azimuthElevation.azimuth = 0.0f;
        azimuthElevation.elevation = 0.0f;

        xyz = new XYZ();
        xyz.x = 0.0f;
        xyz.y = 0.0f;
        xyz.z = 0.0f;

        SetCameraAzimuthElevation(azimuthElevation);
    }

    /*
	// Update is called once per frame
	void LateUpdate () {
        transform.position = player.transform.position + offset;
    }*/


    void LateUpdate()
    {
        OrbitCamera();
    }

    public void OrbitCamera()
    {
        bool isCtrlKeyDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (isCtrlKeyDown && Input.GetMouseButton(0))
        {
            azimuthElevation.azimuth += Input.GetAxis("Mouse X") * RotateAmount;
            azimuthElevation.elevation += Input.GetAxis("Mouse Y") * RotateAmount;

            azimuthElevation.azimuth = azimuthElevation.azimuth % 360;
            while (azimuthElevation.azimuth >= 360.0f) azimuthElevation.azimuth -= 360.0f;
            while (azimuthElevation.azimuth < 0.0f) azimuthElevation.azimuth += 360.0f;

            if (azimuthElevation.elevation > 89.0f) azimuthElevation.elevation = 89.0f;
            if (azimuthElevation.elevation < -89.0f) azimuthElevation.elevation = -89.0f;

            SetCameraAzimuthElevation(azimuthElevation);
        }
    }


    public void SetCameraAzimuthElevation(AzimuthElevation azimuthElevation)
    {
        Vector3 position = new Vector3(0.0f, 0.0f, -50.0f);

        Quaternion rotation = Quaternion.Euler(-azimuthElevation.elevation, azimuthElevation.azimuth, 0.0f);

        Vector3 rotatedVector = rotation * position;
        xyz.x = rotatedVector.x;
        xyz.y = rotatedVector.y;
        xyz.z = rotatedVector.z;

        transform.position = rotatedVector;
        transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
    }
    /*

    //
    // VARIABLES
    //

    public float turnSpeed = 4.0f;      // Speed of camera turning when mouse moves in along an axis
    public float panSpeed = 4.0f;       // Speed of the camera when being panned
    public float zoomSpeed = 4.0f;      // Speed of the camera going back and forth

    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private bool isPanning;     // Is the camera being panned?
    private bool isRotating;    // Is the camera being rotated?
    private bool isZooming;     // Is the camera zooming?

    //
    // UPDATE
    //

    void Update()
    {
        // Get the left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            // Get mouse origin
            mouseOrigin = Input.mousePosition;
            isRotating = true;
        }

        // Get the right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            // Get mouse origin
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }

        // Get the middle mouse button
        if (Input.GetMouseButtonDown(2))
        {
            // Get mouse origin
            mouseOrigin = Input.mousePosition;
            isZooming = true;
        }

        // Disable movements on button release
        if (!Input.GetMouseButton(0)) isRotating = false;
        if (!Input.GetMouseButton(1)) isPanning = false;
        if (!Input.GetMouseButton(2)) isZooming = false;

        // Rotate camera along X and Y axis
        if (isRotating)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            transform.RotateAround(transform.position, transform.right, -pos.y * turnSpeed);
            transform.RotateAround(transform.position, Vector3.up, pos.x * turnSpeed);
        }

        // Move the camera on it's XY plane
        if (isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
            transform.Translate(move, Space.Self);
        }

        // Move the camera linearly along Z axis
        if (isZooming)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = pos.y * zoomSpeed * transform.forward;
            transform.Translate(move, Space.World);
        }
    }
    */
}
