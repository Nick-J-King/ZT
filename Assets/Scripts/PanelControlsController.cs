using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PanelControlsController : MonoBehaviour {

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
    public Toggle toggleVolume;
    public Toggle toggleValidate;

    public Toggle toggleAddTriangles;
    public Toggle toggleCreateMesh;
    public Toggle toggleOuterLoop;
    public Toggle toggleInnerLoop;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
