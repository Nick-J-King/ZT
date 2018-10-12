using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameController : MonoBehaviour {


    public GameObject goFrame;

    public GameObject cube4X;
    public GameObject cube4Y;
    public GameObject cube4Z;

    public GameObject cube5X;
    public GameObject cube5Y;
    public GameObject cube5Z;


    private int edges;  // Number of edges to display.
    private bool active; // Whether to display.


    // Use this for initialization
    void Start ()
    {
        edges = 0;
        active = true;
	}
	

	// Update is called once per frame
	void Update () {
		
	}

    public void SetActive(bool activeIn)
    {
        active = activeIn;

        goFrame.SetActive(active);
        cube4X.SetActive(active && edges >= 4);
        cube4Y.SetActive(active && edges >= 4);
        cube4Z.SetActive(active && edges >= 4);
        cube5X.SetActive(active && edges >= 5);
        cube5Y.SetActive(active && edges >= 5);
        cube5Z.SetActive(active && edges >= 5);
    }


    // Set the sizes of the edge length indicators

    public void SetEdges(int edge4, int edge5, int divisions, int edgesIn)
    {
        edges = edgesIn;

        if (edges >= 4)
        {
            float scaleL = (float)(edge4 * 10) / (float)divisions;
            float scaleW = 0.1f;
            float pos = -5.0f + scaleL / 2.0f;

            cube4X.transform.localScale = new Vector3(scaleL, scaleW, scaleW);
            cube4X.transform.localPosition = new Vector3(pos, -6.0f, -6.0f);

            cube4Y.transform.localScale = new Vector3(scaleW, scaleL, scaleW);
            cube4Y.transform.localPosition = new Vector3(-6.0f, pos, -6.0f);

            cube4Z.transform.localScale = new Vector3(scaleW, scaleW, scaleL);
            cube4Z.transform.localPosition = new Vector3(-6.0f, -6.0f, pos);

            cube4X.SetActive(active);
            cube4Y.SetActive(active);
            cube4Z.SetActive(active);
        }
        else
        {
            cube4X.SetActive(false);
            cube4Y.SetActive(false);
            cube4Z.SetActive(false);
        }

        if (edges >= 5)
        {
            float scaleL5 = (float)(edge5 * 10) / (float)divisions;
            float scaleW5 = 0.1f;
            float pos5 = -5.0f + scaleL5 / 2.0f;

            cube5X.transform.localScale = new Vector3(scaleL5, scaleW5, scaleW5);
            cube5X.transform.localPosition = new Vector3(pos5, -6.5f, -6.5f);

            cube5Y.transform.localScale = new Vector3(scaleW5, scaleL5, scaleW5);
            cube5Y.transform.localPosition = new Vector3(-6.5f, pos5, -6.5f);

            cube5Z.transform.localScale = new Vector3(scaleW5, scaleW5, scaleL5);
            cube5Z.transform.localPosition = new Vector3(-6.5f, -6.5f, pos5);

            cube5X.SetActive(active);
            cube5Y.SetActive(active);
            cube5Z.SetActive(active);

        }
        else
        {
            cube5X.SetActive(false);
            cube5Y.SetActive(false);
            cube5Z.SetActive(false);
        }
    }
}
