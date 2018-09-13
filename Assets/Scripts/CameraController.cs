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

    //public GameObject player;

    private float RotateAmount = 1.0f;

    public AzimuthElevation azimuthElevation;
    public XYZ xyz;

    Ray ray;
    RaycastHit hit;


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


    void LateUpdate()
    {
        OrbitCamera();
    }


    public void OrbitCamera()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            print(hit.collider.name);
        }

        bool isCtrlKeyDown = true; // Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
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
        Vector3 position = new Vector3(0.0f, 0.0f, -40.0f);

        Quaternion rotation = Quaternion.Euler(-azimuthElevation.elevation, azimuthElevation.azimuth, 0.0f);

        Vector3 rotatedVector = rotation * position;
        xyz.x = rotatedVector.x;
        xyz.y = rotatedVector.y;
        xyz.z = rotatedVector.z;

        transform.position = rotatedVector;
        transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
    }
}