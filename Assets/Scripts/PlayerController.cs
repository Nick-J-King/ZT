using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



// Player controller
public class PlayerController : MonoBehaviour
{
    // The main figure...
    public ZeroTriangles zeroTriangles;

    // Controls on panel.
    public Text textDivisions;
    public Slider sliderDivisions;

    public Dropdown DropdownEdges;

    public Text text4thEdge;
    public Slider slider4thEdge;
    public Slider slider5thEdge;

    public Text textTitleLightAzimuthElevation;
    public Slider sliderLightAzimuth;
    public Slider sliderLightElevation;

    public Toggle togglePoints;
    public Slider sliderVertexSize;

    public Toggle toggleAnimate;
    public Slider SliderAnimateSpeed;

    public Toggle toggleFlip;
    public Toggle toggleFrame;
    public Toggle toggleClosure;

    public Text textStatus;

    public GameObject goFrame;

    // External game objects.
    public Light directionalLight;

    public CameraController mainCamController;
    public Camera mainCam;


    void Start()
    {
        zeroTriangles.Initialise();

        GetParametersFromControls();
        SetLightFromControls();

        int nCellCount = zeroTriangles.ComputeGeometry();

        textStatus.text = nCellCount.ToString();
    }


    // called per frame, before performing physics
    void FixedUpdate()
    {
    }


    // called per frame, before performing physics
    void Update()
    {
        if (toggleAnimate.isOn)
        {
            zeroTriangles.mfMain.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * 100.0f * SliderAnimateSpeed.value);
            goFrame.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * 100.0f * SliderAnimateSpeed.value);
        }
    }


    // Read the light controls, and update the directional light.
    public void SetLightFromControls()
    {
        float y = 360.0f - sliderLightAzimuth.value;
        float x = sliderLightElevation.value;
        float z = 0.0f;

        textTitleLightAzimuthElevation.text = "Azm: " + y.ToString() + " Ele: " + x.ToString();

        directionalLight.transform.localRotation = Quaternion.Euler(x, y, z);
        directionalLight.transform.localPosition = Vector3.zero;
    }


    // A geometry control has changed.
    // Get the new parameters, and recompute the geometry.
    public void CheckGeometryControls()
    {
        bool changed = GetParametersFromControls();
        if (changed)
        {
            int nCellCount = zeroTriangles.ComputeGeometry();

            textStatus.text = nCellCount.ToString();

        }
    }


    // Read the geometry parameters from the controls,
    // and work out the internal parameters.
    public bool GetParametersFromControls()
    {
        bool changed = false;

        // Lattice divisions.
        if (zeroTriangles.parameters.nDivisions != (int)sliderDivisions.value)
        {
            zeroTriangles.parameters.nDivisions = (int)sliderDivisions.value;
            changed = true;
        }

        textDivisions.text = "Divisions: " + zeroTriangles.parameters.nDivisions.ToString();

        // Edges
        if (zeroTriangles.parameters.dropdownEdgesInt != DropdownEdges.value)
        {
            zeroTriangles.parameters.dropdownEdgesInt = DropdownEdges.value;
            changed = true;
        }

        // 4th edge
        float sliderFloat = slider4thEdge.value;
        int sliderInt = (int)(sliderFloat * (zeroTriangles.parameters.nDivisions + 1));
        if (sliderInt > zeroTriangles.parameters.nDivisions) sliderInt = zeroTriangles.parameters.nDivisions;

        // 5th edge
        float sliderFloat5thEdge = slider5thEdge.value;
        int sliderInt5thEdge = (int)(sliderFloat5thEdge * (zeroTriangles.parameters.nDivisions + 1));
        if (sliderInt5thEdge > zeroTriangles.parameters.nDivisions) sliderInt5thEdge = zeroTriangles.parameters.nDivisions;


        text4thEdge.text = "Edges: " + sliderInt.ToString() + " " + sliderInt5thEdge.ToString();

        // Vertices
        if (zeroTriangles.parameters.displayVertices != togglePoints.isOn)
        {
            zeroTriangles.parameters.displayVertices = togglePoints.isOn;
            changed = true;
        }

        if (zeroTriangles.parameters.vertexSize != sliderVertexSize.value)
        {
            zeroTriangles.parameters.vertexSize = sliderVertexSize.value;
            changed = true;
        }


        if (zeroTriangles.parameters.doClosure != toggleClosure.isOn)
        {
            zeroTriangles.parameters.doClosure = toggleClosure.isOn;
            changed = true;
        }


        // Internal parameters.
        zeroTriangles.parameters.nFullDivisions = zeroTriangles.parameters.nDivisions * 12;
        if (zeroTriangles.parameters.sliderFullInt != sliderInt * 12)
        {
            zeroTriangles.parameters.sliderFullInt = sliderInt * 12;
            changed = true;
        }
        if (zeroTriangles.parameters.sliderFullInt5thEdge != sliderInt5thEdge * 12)
        {
            zeroTriangles.parameters.sliderFullInt5thEdge = sliderInt5thEdge * 12;
            changed = true;
        }

        zeroTriangles.parameters.max = (float)zeroTriangles.parameters.nDivisions;
        zeroTriangles.parameters.fullMax = (float)zeroTriangles.parameters.nFullDivisions;

        zeroTriangles.parameters.scale = zeroTriangles.parameters.size / zeroTriangles.parameters.max * zeroTriangles.parameters.vertexSize + 0.05f;

        return changed;
    }


    public void CheckFrameToggle()
    {
        goFrame.SetActive(toggleFrame.isOn);
    }


    public void CheckFlipToggle()
    {
        if (toggleFlip.isOn)
        {
            zeroTriangles.mfMain.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
        }
        else
        {
            zeroTriangles.mfMain.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }


    public void ResetAnimation()
    {
        toggleAnimate.isOn = false;
        zeroTriangles.mfMain.transform.localEulerAngles = Vector3.zero;
    }


    public void ResetCamera()
    {
        mainCamController.azimuthElevation.azimuth = 0;
        mainCamController.azimuthElevation.elevation = 0;
        mainCamController.SetCameraAzimuthElevation(mainCamController.azimuthElevation);
    }
}
