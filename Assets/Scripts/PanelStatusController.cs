using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PanelStatusController : MonoBehaviour {

    public Text textTitle;
    public Text textFullFlats;
    public Text textFullDiagonals;
    public Text textFullCorners;
    public Text textPartialFlats;
    public Text textPartialDiagonals;
    public Text textPartialCorners;
    public Text textSubCellsB;
    public Text textSubCellsS;
    public Text textSubCellsE;
    public Text textFullyIn;
    public Text textFullyOut;
    public Text textMeasured;
    public Text textCellCount;
    public Text textVolume;
    public Text textTimePerFigure;
    public Text textTimePerCell;
    public Text textErrorCode;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStats(ZeroTriangleStats stats)
    {
        textFullFlats.text = "Full flats: " + stats.nFullFlats.ToString();
        textFullDiagonals.text = "Full diagonals: " + stats.nFullDiagonals.ToString();
        textFullCorners.text = "Full corners: " + stats.nFullCorners.ToString();

        textPartialFlats.text = "Partial flats: " + stats.nPartialFlats.ToString();
        textPartialDiagonals.text = "Partial diagonals: " + stats.nPartialDiagonals.ToString();
        textPartialCorners.text = "Partial corners: " + stats.nPartialCorners.ToString();

        if (stats.volumeComputed)
        {
            textSubCellsB.text = "SubCellsB: " + stats.nSubCellsB.ToString();
            textSubCellsS.text = "SubCellsS: " + stats.nSubCellsS.ToString();
            textSubCellsE.text = "SubCellsE: " + stats.nSubCellsE.ToString();
        }
        else
        {
            textSubCellsB.text = "SubCellsB: not computed";
            textSubCellsS.text = "SubCellsS: not computed";
            textSubCellsE.text = "SubCellsE: not computed";
        }

        textFullyIn.text = "Fully in: " + stats.nFullyIn.ToString();
        textFullyOut.text = "Fully out: " + stats.nFullyOut.ToString();
        if (stats.volumeComputed)
        {
            textMeasured.text = "Measured: " + stats.nMeasured.ToString();
        }
        else
        {
            textMeasured.text = "Measured: none";
        }
        textCellCount.text = "Cell count: " + stats.nCellCount.ToString();

        if (stats.volumeComputed)
        {
            textVolume.text = "Volume: " + stats.fVolume.ToString();
        }
        else
        {
            textVolume.text = "Volume: not computed";
        }

        textTimePerFigure.text = "Time: " + stats.fTimePerFigure.ToString();
        textTimePerCell.text = "Time/cell: " + stats.fTimePerCell.ToString();

        if (stats.validated)
        {
            textErrorCode.text = "Error code: " + stats.errorCode.ToString();
        }
        else
        {
            textErrorCode.text = "Error code: not validated";
        }
    }
}
