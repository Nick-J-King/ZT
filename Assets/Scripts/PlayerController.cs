using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



// Player controller
public class PlayerController : MonoBehaviour
{
    // The main figure...
    public ZeroTriangles zeroTriangles;


    public PanelStatusController panelStatus;
    public PanelControlsController panelControls;



    public GameObject goFrame;

    // External game objects.
    public Light directionalLight;

    public MainCamera mainCamController;
    public Camera mainCam;


    void Start()
    {
        zeroTriangles.Initialise();

        GetParametersFromControls();
        SetLightFromControls();

        ComputeGeometryAndGetStats();
    }


    void ComputeGeometryAndGetStats()
    {
        zeroTriangles.ComputeGeometry();

        ZeroTriangleStats stats = zeroTriangles.GetStats();

        panelStatus.SetStats(stats);
    }


    // called per frame, before performing physics
    void FixedUpdate()
    {
    }


    // called per frame, before performing physics
    void Update()
    {
        if (panelControls.toggleAnimate.isOn)
        {
            zeroTriangles.mfMain.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * 100.0f * panelControls.SliderAnimateSpeed.value);
            goFrame.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * 100.0f * panelControls.SliderAnimateSpeed.value);
        }
    }


    // Read the light controls, and update the directional light.
    public void SetLightFromControls()
    {
        float y = 360.0f - panelControls.sliderLightAzimuth.value;
        float x = panelControls.sliderLightElevation.value;
        float z = 0.0f;

        panelControls.textTitleLightAzimuthElevation.text = "Azm: " + y.ToString() + " Ele: " + x.ToString();

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
            ComputeGeometryAndGetStats();
        }
    }


    // Read the geometry parameters from the controls,
    // and work out the internal parameters.
    public bool GetParametersFromControls()
    {
        bool changed = false;

        // Lattice divisions.
        if (zeroTriangles.parameters.nDivisions != (int)panelControls.sliderDivisions.value)
        {
            zeroTriangles.parameters.nDivisions = (int)panelControls.sliderDivisions.value;
            changed = true;
        }

        panelControls.textDivisions.text = "Divisions: " + zeroTriangles.parameters.nDivisions.ToString();

        // Edges
        if (zeroTriangles.parameters.dropdownEdgesInt != panelControls.DropdownEdges.value)
        {
            zeroTriangles.parameters.dropdownEdgesInt = panelControls.DropdownEdges.value;
            changed = true;
        }

        // 4th edge
        float sliderFloat = panelControls.slider4thEdge.value;
        int sliderInt = (int)(sliderFloat * (zeroTriangles.parameters.nDivisions + 1));
        if (sliderInt > zeroTriangles.parameters.nDivisions) sliderInt = zeroTriangles.parameters.nDivisions;

        // 5th edge
        float sliderFloat5thEdge = panelControls.slider5thEdge.value;
        int sliderInt5thEdge = (int)(sliderFloat5thEdge * (zeroTriangles.parameters.nDivisions + 1));
        if (sliderInt5thEdge > zeroTriangles.parameters.nDivisions) sliderInt5thEdge = zeroTriangles.parameters.nDivisions;


        panelControls.text4thEdge.text = "Edges: " + sliderInt.ToString() + " " + sliderInt5thEdge.ToString();

        // Vertices
        if (zeroTriangles.parameters.displayVertices != panelControls.togglePoints.isOn)
        {
            zeroTriangles.parameters.displayVertices = panelControls.togglePoints.isOn;
            changed = true;
        }

        if (zeroTriangles.parameters.vertexSize != panelControls.sliderVertexSize.value)
        {
            zeroTriangles.parameters.vertexSize = panelControls.sliderVertexSize.value;
            changed = true;
        }


        if (zeroTriangles.parameters.doClosure != panelControls.toggleClosure.isOn)
        {
            zeroTriangles.parameters.doClosure = panelControls.toggleClosure.isOn;
            changed = true;
        }

        if (zeroTriangles.parameters.computeVolume != panelControls.toggleVolume.isOn)
        {
            zeroTriangles.parameters.computeVolume = panelControls.toggleVolume.isOn;
            changed = true;
        }

        if (zeroTriangles.parameters.validate != panelControls.toggleValidate.isOn)
        {
            zeroTriangles.parameters.validate = panelControls.toggleValidate.isOn;
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
        goFrame.SetActive(panelControls.toggleFrame.isOn);
    }


    public void CheckFlipToggle()
    {
        if (panelControls.toggleFlip.isOn)
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
        panelControls.toggleAnimate.isOn = false;
        zeroTriangles.mfMain.transform.localEulerAngles = Vector3.zero;
        goFrame.transform.localEulerAngles = Vector3.zero;
    }


    public void ResetCamera()
    {
        mainCamController.azimuthElevation.azimuth = 0;
        mainCamController.azimuthElevation.elevation = 0;
        mainCamController.SetCameraAzimuthElevation(mainCamController.azimuthElevation);
    }
}
