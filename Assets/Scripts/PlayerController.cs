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
        float volume = zeroTriangles.ComputeGeometry();
        ZeroTriangleStats stats = zeroTriangles.GetStats();

        panelStatus.textFullFlats.text = "Full flats: " + stats.nFullFlats.ToString();
        panelStatus.textFullDiagonals.text = "Full diagonals: " + stats.nFullDiagonals.ToString();
        panelStatus.textFullCorners.text = "Full corners: " + stats.nFullCorners.ToString();

        panelStatus.textPartialFlats.text = "Partial flats: " + stats.nPartialFlats.ToString();
        panelStatus.textPartialDiagonals.text = "Partial diagonals: " + stats.nPartialDiagonals.ToString();
        panelStatus.textPartialCorners.text = "Partial corners: " + stats.nPartialCorners.ToString();

        panelStatus.textSubCellsB.text = "SubCellsB : " + stats.nSubCellsB.ToString();
        panelStatus.textSubCellsS.text = "SubCellsS : " + stats.nSubCellsS.ToString();
        panelStatus.textSubCellsE.text = "SubCellsE : " + stats.nSubCellsE.ToString();

        panelStatus.textFullyIn.text = "Fully in: " + stats.nFullyIn.ToString();
        panelStatus.textFullyOut.text = "Fully out: " + stats.nFullyOut.ToString();
        panelStatus.textMeasured.text = "Measured: " + stats.nMeasured.ToString();
        panelStatus.textCellCount.text = "Cell count: " + stats.nCellCount.ToString();
    
        panelStatus.textVolume.text = "Volume: " + stats.fVolume.ToString();

        panelStatus.textTimePerFigure.text = "Time: " + stats.fTimePerFigure.ToString();
        panelStatus.textTimePerCell.text = "Time/cell: " + stats.fTimePerCell.ToString();

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
            ComputeGeometryAndGetStats();
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
