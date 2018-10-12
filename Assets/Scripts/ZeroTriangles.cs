using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Data for main (full) grid cache.
// These are the "corner" points of the 12 x 12 x 12 cells.
// Used to quickly skip over full / empty cells.
// NOTE: 12 x 12 x 12 is necessary and sufficient to compute all possible geometries.

public struct CellData
{
    public int status;
    /// public Vector3 worldCoords; >>> NOTE: Not used yet...
}


// Gather the parameters for the Zero Triangles figure.
// >>> Split off "computed" parameters.
// >>> RENAME for clarity!

public struct ZeroTriangleParameters
{
    public int nDivisions;     // "Main" divisions.
    public int nFullDivisions; // nDivisions * 12 for finers sub-divisions.

    public bool displayVertices;
    public bool computeVolume;
    public bool validate;

    public float scale;

    public int sliderFullInt;
    public int sliderFullInt5thEdge;

    public int dropdownEdgesInt;
    public bool doClosure;

    public float vertexSize;

    public float max;
    public float fullMax;

    // Internal parameters.
    public float size;
    public float sizeOnTwo;

}


// Public struct stats
public struct ZeroTriangleStats
{
    public int nFullFlats;
    public int nFullDiagonals;
    public int nFullCorners;

    public int nPartialFlats;
    public int nPartialDiagonals;
    public int nPartialCorners;


    // Cell counts.
    public int nSubCellsB;     // "Big"
    public int nSubCellsS;     // "Small"
    public int nSubCellsE;     // "Edge"

    public int nFullyIn;
    public int nFullyOut;

    public int nMeasured;
    public int nCellCount;

    public float fVolume;
    public bool volumeComputed;

    public float fTimePerFigure;
    public float fTimePerCell;

    public bool validated;
    public int errorCode;
}



// The main figure to display.
// Controls the generation and display (as a mesh).

public class ZeroTriangles : MonoBehaviour {

    // The parameters for the figure that can be controlled externally...
    public ZeroTriangleParameters parameters;

    public ZeroTriangleStats stats;

    // Materials.
    public Material vertexMaterial;
    public Material vertexMaterialMarkB;
    public Material vertexMaterialMarkS;
    public Material vertexMaterialMarkE;

    public Material vertexMaterialConstruction;
    public Material vertexMaterialConstructionOff;

    // Mesh gameobjects.
    public GameObject mfMain;

    public MeshFilter mfMain0;
    public MeshFilter mfMain1;
    public MeshFilter mfMain2;
    public MeshFilter mfMain3;
    public MeshFilter mfMain4;
    public MeshFilter mfMain5;
    public MeshFilter mfMain6;
    public MeshFilter mfMain7;
    public MeshFilter mfMain8;
    public MeshFilter mfMain9;
    public MeshFilter mfMain10;
    public MeshFilter mfMain11;
    public MeshFilter mfMain12;
    public MeshFilter mfMain13;


    // PRIVATE members --------------------------

    // Internal cache for building meshes.
    private int[] myNumVerts;
    private int[] myNumTriangles;
    private List<Vector3>[] myVerts;
    private List<int>[] myTriangles;


    private MeshFilter[] mfSub;  // Point to the 14 "sub meshes" mfMain0 tp mfMain13

    private int MAXTVERTS = 65530;

    public int ERROR_CACHE = 1;
    public int ERROR_VOLUME_IN_FULL_OUT = 2;
    public int ERROR_NOT_FULL_VOLUME_IN_FULL_IN = 3;

    // List of vertex spheres.
    private GameObject s;
    private ArrayList myList;


    // Cache the cube corner info for each layer of (x,y) as we move through z
    private CellData[,,] xcc;    // x,y, layer (0,1)
    private int xccActiveLayer;
    private int xccWriteLayer;


    public ZeroTriangleStats GetStats()
    {
        return stats;
    }


    // Use this for initialization
    public void Initialise()
    {
        // Create the array of meshes.
        mfSub = new MeshFilter[14];

        mfSub[0] = mfMain0;
        mfSub[1] = mfMain1;
        mfSub[2] = mfMain2;
        mfSub[3] = mfMain3;
        mfSub[4] = mfMain4;
        mfSub[5] = mfMain5;
        mfSub[6] = mfMain6;
        mfSub[7] = mfMain7;
        mfSub[8] = mfMain8;
        mfSub[9] = mfMain9;
        mfSub[10] = mfMain10;
        mfSub[11] = mfMain11;
        mfSub[12] = mfMain12;
        mfSub[13] = mfMain13;

        // Create the builder info for each of the meshes.
        myNumVerts = new int[14];
        myNumTriangles = new int[14];
        myVerts = new List<Vector3>[14];
        myTriangles = new List<int>[14];

        // Create the list of vertex spheres.
        myList = new ArrayList();

        // Set the basic size of the figure to match the cube frame.
        parameters.size = 10.0f;                            // Size of the "configuration cube".
        parameters.sizeOnTwo = parameters.size / 2.0f;      // Used to center the cube.
            // >>> FIX!!!
    }


    // We have the internal parameters set.
    // Now, compute the geometry of the figure.
   
    public void ComputeGeometry()
    {

        float start = Time.realtimeSinceStartup;

        // Reset all counters...

        stats.nFullFlats = 0;
        stats.nFullDiagonals = 0;
        stats.nFullCorners = 0;

        stats.nPartialFlats = 0;
        stats.nPartialDiagonals = 0;
        stats.nPartialCorners = 0;

        stats.nSubCellsB = 0;
        stats.nSubCellsS = 0;
        stats.nSubCellsE = 0;

        stats.nFullyIn = 0;
        stats.nFullyOut = 0;
        stats.nMeasured = 0;
        stats.nCellCount = 0;

        stats.errorCode = 0;
        stats.validated = parameters.validate;
        stats.volumeComputed = parameters.computeVolume;
        stats.fVolume = 0.0f;

        // Clear away all vertices & meshes...

        foreach (GameObject s in myList)
        {
            Destroy(s);
        }

        for (int i = 0; i < 14; i++)
        {
            ResetMesh(i);
        }


        // Construct the main figure.

        DoFullFigure();


        // Now put the list of triangles in each mesh.
        for (int i = 0; i < 14; i++)
        {
            ProcessMesh(i);
        }

        float elapsed = Time.realtimeSinceStartup - start;

        stats.fTimePerFigure = elapsed;
        stats.fTimePerCell = elapsed / stats.nCellCount;

        float vol = CalculateVolume();   // Calculate the volume from (cell) counts...
        stats.fVolume = vol;
    }


    // Given the "base" point of the 12 x 12 x 12 cell, count up the types of components "inside".
    // If the volume is completely full, there should be:
    //      4 * 6 "1/2 of a 24th"s  (Big pyramid)
    //      4 * 6 "1/6 of a 24th"s. (Small kite shape)
    //  2 * 4 * 6 "1/6 of a 24th"s. ("Edge pyramids")
    // For a complete 1 volume unit.

    private void MeasureCell(int xFull, int yFull, int zFull, ref int nSubCellsB, ref int nSubCellsS, ref int nSubCellsE)
    {

        // z = 0 face -----------------------------------

        // y = 0 edge
        CheckSubCell(xFull + 6, yFull + 2, zFull + 1, ref nSubCellsB);  // 621
        CheckSubCell(xFull + 6, yFull + 4, zFull + 3, ref nSubCellsS);  // 643
        CheckSubCell(xFull + 4, yFull + 3, zFull + 2, ref nSubCellsE);  // 432
        CheckSubCell(xFull + 8, yFull + 3, zFull + 2, ref nSubCellsE);  // 832

        // y = c edge
        CheckSubCell(xFull + 6, yFull + 10, zFull + 1, ref nSubCellsB);  // 6A1
        CheckSubCell(xFull + 6, yFull + 8, zFull + 3, ref nSubCellsS);  // 683
        CheckSubCell(xFull + 4, yFull + 9, zFull + 2, ref nSubCellsE);  // 492
        CheckSubCell(xFull + 8, yFull + 9, zFull + 2, ref nSubCellsE);  // 892

        // x = 0 edge
        CheckSubCell(xFull + 2, yFull + 6, zFull + 1, ref nSubCellsB);  // 261
        CheckSubCell(xFull + 4, yFull + 6, zFull + 3, ref nSubCellsS);  // 463
        CheckSubCell(xFull + 3, yFull + 4, zFull + 2, ref nSubCellsE);  // 342
        CheckSubCell(xFull + 3, yFull + 8, zFull + 2, ref nSubCellsE);  // 382

        // x = c edge
        CheckSubCell(xFull + 10, yFull + 6, zFull + 1, ref nSubCellsB);  // A61
        CheckSubCell(xFull + 8, yFull + 6, zFull + 3, ref nSubCellsS);  // 863
        CheckSubCell(xFull + 9, yFull + 4, zFull + 2, ref nSubCellsE);  // 942
        CheckSubCell(xFull + 9, yFull + 8, zFull + 2, ref nSubCellsE);  // 982

        // z = c face -----------------------------------

        // y = 0 edge
        CheckSubCell(xFull + 6, yFull + 2, zFull + 11, ref nSubCellsB);  // 62B
        CheckSubCell(xFull + 6, yFull + 4, zFull + 9, ref nSubCellsS);  // 649
        CheckSubCell(xFull + 4, yFull + 3, zFull + 10, ref nSubCellsE);  // 43A
        CheckSubCell(xFull + 8, yFull + 3, zFull + 10, ref nSubCellsE);  // 83A

        // y = c edge
        CheckSubCell(xFull + 6, yFull + 10, zFull + 11, ref nSubCellsB);  // 6AB
        CheckSubCell(xFull + 6, yFull + 8, zFull + 9, ref nSubCellsS);  // 689
        CheckSubCell(xFull + 4, yFull + 9, zFull + 10, ref nSubCellsE);  // 49A
        CheckSubCell(xFull + 8, yFull + 9, zFull + 10, ref nSubCellsE);  // 89A

        // x = 0 edge
        CheckSubCell(xFull + 2, yFull + 6, zFull + 11, ref nSubCellsB);  // 26B
        CheckSubCell(xFull + 4, yFull + 6, zFull + 9, ref nSubCellsS);  // 469
        CheckSubCell(xFull + 3, yFull + 4, zFull + 10, ref nSubCellsE);  // 34A
        CheckSubCell(xFull + 3, yFull + 8, zFull + 10, ref nSubCellsE);  // 38A

        // x = c edge
        CheckSubCell(xFull + 10, yFull + 6, zFull + 11, ref nSubCellsB);  // A6B
        CheckSubCell(xFull + 8, yFull + 6, zFull + 9, ref nSubCellsS);  // 869
        CheckSubCell(xFull + 9, yFull + 4, zFull + 10, ref nSubCellsE);  // 94A
        CheckSubCell(xFull + 9, yFull + 8, zFull + 10, ref nSubCellsE);  // 98A



        // y = 0 face -----------------------------------

        // z = 0 edge
        CheckSubCell(xFull + 6, yFull + 1, zFull + 2, ref nSubCellsB);  // 612
        CheckSubCell(xFull + 6, yFull + 3, zFull + 4, ref nSubCellsS);  // 634
        CheckSubCell(xFull + 4, yFull + 2, zFull + 3, ref nSubCellsE);  // 423
        CheckSubCell(xFull + 8, yFull + 2, zFull + 3, ref nSubCellsE);  // 823

        // z = c edge
        CheckSubCell(xFull + 6, yFull + 1, zFull + 10, ref nSubCellsB);  // 61A
        CheckSubCell(xFull + 6, yFull + 3, zFull + 8, ref nSubCellsS);  // 638
        CheckSubCell(xFull + 4, yFull + 2, zFull + 9, ref nSubCellsE);  // 429
        CheckSubCell(xFull + 8, yFull + 2, zFull + 9, ref nSubCellsE);  // 829

        // x = 0 edge
        CheckSubCell(xFull + 2, yFull + 1, zFull + 6, ref nSubCellsB);  // 216
        CheckSubCell(xFull + 4, yFull + 3, zFull + 6, ref nSubCellsS);  // 436
        CheckSubCell(xFull + 3, yFull + 2, zFull + 4, ref nSubCellsE);  // 324
        CheckSubCell(xFull + 3, yFull + 2, zFull + 8, ref nSubCellsE);  // 328

        // x = c edge
        CheckSubCell(xFull + 10, yFull + 1, zFull + 6, ref nSubCellsB);  // A16
        CheckSubCell(xFull + 8, yFull + 3, zFull + 6, ref nSubCellsS);  // 836
        CheckSubCell(xFull + 9, yFull + 2, zFull + 4, ref nSubCellsE);  // 924
        CheckSubCell(xFull + 9, yFull + 2, zFull + 8, ref nSubCellsE);  // 928

        // y = c face -----------------------------------

        // z = 0 edge
        CheckSubCell(xFull + 6, yFull + 11, zFull + 2, ref nSubCellsB);  // 6B2
        CheckSubCell(xFull + 6, yFull + 9, zFull + 4, ref nSubCellsS);  // 694
        CheckSubCell(xFull + 4, yFull + 10, zFull + 3, ref nSubCellsE);  // 4A3
        CheckSubCell(xFull + 8, yFull + 10, zFull + 3, ref nSubCellsE);  // 8A3

        // z = c edge
        CheckSubCell(xFull + 6, yFull + 11, zFull + 10, ref nSubCellsB);  // 6BA
        CheckSubCell(xFull + 6, yFull + 9, zFull + 8, ref nSubCellsS);  // 698
        CheckSubCell(xFull + 4, yFull + 10, zFull + 9, ref nSubCellsE);  // 4A9
        CheckSubCell(xFull + 8, yFull + 10, zFull + 9, ref nSubCellsE);  // 8A9

        // x = 0 edge
        CheckSubCell(xFull + 2, yFull + 11, zFull + 6, ref nSubCellsB);  // 2B6
        CheckSubCell(xFull + 4, yFull + 9, zFull + 6, ref nSubCellsS);  // 496
        CheckSubCell(xFull + 3, yFull + 10, zFull + 4, ref nSubCellsE);  // 3A4
        CheckSubCell(xFull + 3, yFull + 10, zFull + 8, ref nSubCellsE);  // 3A8

        // x = c edge
        CheckSubCell(xFull + 10, yFull + 11, zFull + 6, ref nSubCellsB);  // AB6
        CheckSubCell(xFull + 8, yFull + 9, zFull + 6, ref nSubCellsS);  // 896
        CheckSubCell(xFull + 9, yFull + 10, zFull + 4, ref nSubCellsE);  // 9A4
        CheckSubCell(xFull + 9, yFull + 10, zFull + 8, ref nSubCellsE);  // 9A8



        // x = 0 face -----------------------------------

        // z = 0 edge
        CheckSubCell(xFull + 1, yFull + 6, zFull + 2, ref nSubCellsB);  // 162
        CheckSubCell(xFull + 3, yFull + 6, zFull + 4, ref nSubCellsS);  // 364
        CheckSubCell(xFull + 2, yFull + 4, zFull + 3, ref nSubCellsE);  // 243
        CheckSubCell(xFull + 2, yFull + 8, zFull + 3, ref nSubCellsE);  // 283

        // z = c edge
        CheckSubCell(xFull + 1, yFull + 6, zFull + 10, ref nSubCellsB);  // 16A
        CheckSubCell(xFull + 3, yFull + 6, zFull + 8, ref nSubCellsS);  // 368
        CheckSubCell(xFull + 2, yFull + 4, zFull + 9, ref nSubCellsE);  // 249
        CheckSubCell(xFull + 2, yFull + 8, zFull + 9, ref nSubCellsE);  // 289

        // y = 0 edge
        CheckSubCell(xFull + 1, yFull + 2, zFull + 6, ref nSubCellsB);  // 126
        CheckSubCell(xFull + 3, yFull + 4, zFull + 6, ref nSubCellsS);  // 346
        CheckSubCell(xFull + 2, yFull + 3, zFull + 4, ref nSubCellsE);  // 234
        CheckSubCell(xFull + 2, yFull + 3, zFull + 8, ref nSubCellsE);  // 238

        // y = c edge
        CheckSubCell(xFull + 1, yFull + 10, zFull + 6, ref nSubCellsB);  // 1A6
        CheckSubCell(xFull + 3, yFull + 8, zFull + 6, ref nSubCellsS);  // 386
        CheckSubCell(xFull + 2, yFull + 9, zFull + 4, ref nSubCellsE);  // 294
        CheckSubCell(xFull + 2, yFull + 9, zFull + 8, ref nSubCellsE);  // 298

        // x = c face -----------------------------------

        // z = 0 edge
        CheckSubCell(xFull + 11, yFull + 6, zFull + 2, ref nSubCellsB);  // B62
        CheckSubCell(xFull + 9, yFull + 6, zFull + 4, ref nSubCellsS);  // 964
        CheckSubCell(xFull + 10, yFull + 4, zFull + 3, ref nSubCellsE);  // A43
        CheckSubCell(xFull + 10, yFull + 8, zFull + 3, ref nSubCellsE);  // A83

        // z = c edge
        CheckSubCell(xFull + 11, yFull + 6, zFull + 10, ref nSubCellsB);  // B6A
        CheckSubCell(xFull + 9, yFull + 6, zFull + 8, ref nSubCellsS);  // 968
        CheckSubCell(xFull + 10, yFull + 4, zFull + 9, ref nSubCellsE);  // A49
        CheckSubCell(xFull + 10, yFull + 8, zFull + 9, ref nSubCellsE);  // A89

        // y = 0 edge
        CheckSubCell(xFull + 11, yFull + 2, zFull + 6, ref nSubCellsB);  // B26
        CheckSubCell(xFull + 9, yFull + 4, zFull + 6, ref nSubCellsS);  // 946
        CheckSubCell(xFull + 10, yFull + 3, zFull + 4, ref nSubCellsE);  // A34
        CheckSubCell(xFull + 10, yFull + 3, zFull + 8, ref nSubCellsE);  // A38

        // y = c edge
        CheckSubCell(xFull + 11, yFull + 10, zFull + 6, ref nSubCellsB);  // BA6
        CheckSubCell(xFull + 9, yFull + 8, zFull + 6, ref nSubCellsS);  // 986
        CheckSubCell(xFull + 10, yFull + 9, zFull + 4, ref nSubCellsE);  // A94
        CheckSubCell(xFull + 10, yFull + 9, zFull + 8, ref nSubCellsE);  // A98
    }


    // Do the simple check on given vertex within the sub-cell.
    // This is here to add up the volume of possible triangles of the "config cube".

    private void CheckSubCell(int s1, int s2, int s3, ref int typeCount)
    {
        int cftv = CanFormTriangleVertex(s1, s2, s3);
        if (cftv == 1)
        {
            ///DrawVertex(GridToWorld(s1), GridToWorld(s2), GridToWorld(s3), vertexMaterialConstruction);
            typeCount++;
        }
        else if (cftv == -1)
        {
            ///DrawVertex(GridToWorld(s1), GridToWorld(s2), GridToWorld(s3), vertexMaterialConstructionOff);

        }
        else if (cftv == 0)
        {
            // ZERO!! error!
            //throw System.Exception; //("ERROR!!! subcell coords should not be on a zer0 surface!!!");
            Debug.Log("cftv == 0  !!!");
        }
        else if (cftv == -2)
        {
            // border!! ERROR
 //           Debug.Log("cftv == -2  !!!");
        }
    }


    // Given sub-cell counts, return the volume of the "in".
    // (For now, use only SubCell counts - then use fullyIn count for speed, etc...)

    public float CalculateVolume()
    {
        if (!parameters.computeVolume)
        {
            return 0.0f;
        }

        int denominator = parameters.nDivisions * parameters.nDivisions * parameters.nDivisions * 144;
        int numerator = 3 * stats.nSubCellsB + stats.nSubCellsS + stats.nSubCellsE + 144 * stats.nFullyIn;

        float volume = (float)numerator / (float)denominator;
        return volume;
    }


    // Construct the "Zero Triangles" figure...

    private void DoFullFigure()
    {
        // First, construct the "base" vectors.

        Vector3Int vu100 = new Vector3Int(1, 0, 0);
        Vector3Int vu010 = new Vector3Int(0, 1, 0);
        Vector3Int vu001 = new Vector3Int(0, 0, 1);

        Vector3Int vu0m11 = new Vector3Int(0, -1, 1);
        Vector3Int vu0m1m1 = new Vector3Int(0, -1, -1);

        Vector3Int vu011 = new Vector3Int(0, 1, 1);
        Vector3Int vu101 = new Vector3Int(1, 0, 1);
        Vector3Int vu110 = new Vector3Int(1, 1, 0);

        Vector3Int vu10m1 = new Vector3Int(1, 0, -1);
        Vector3Int vu1m10 = new Vector3Int(1, -1, 0);

        Vector3Int vum110 = new Vector3Int(-1, 1, 0);
        Vector3Int vum101 = new Vector3Int(-1, 0, 1);
        Vector3Int vum1m10 = new Vector3Int(-1, -1, 0);
        Vector3Int vum10m1 = new Vector3Int(-1, 0, -1);


        // Clear the cache...

        xcc = new CellData[parameters.nDivisions + 2, parameters.nDivisions + 2, 2];

        for (int a = 0; a <= parameters.nDivisions + 1; a++)
        {
            for (int b = 0; b <= parameters.nDivisions + 1; b++)
            {
                xcc[a, b, 0] = new CellData();
                xcc[a, b, 1] = new CellData();
            }
        }

        // Set the "swappping" cache pointers.

        xccActiveLayer = 0;
        xccWriteLayer = 1;


        float x0;
        float y0;
        float z0;


        // NOW, go through each cell...

        for (int intZ = 0; intZ <= parameters.nDivisions; intZ++)
        {
            int intZfull = intZ * 12;

            z0 = GridToWorld(intZfull);

            for (int intY = 0; intY <= parameters.nDivisions; intY++)
            {
                int intYfull = intY * 12;

                y0 = GridToWorld(intYfull);

                for (int intX = 0; intX <= parameters.nDivisions; intX++)
                {
                    int intXfull = intX * 12;

                    stats.nCellCount++;
                    //MeasureCell(intXfull, intYfull, intZfull, ref stats.nSubCellsB, ref stats.nSubCellsS, ref stats.nSubCellsE);

                    /// >>>  
                    /// 
                    /// 
                    x0 = GridToWorld(intXfull);

                    int nIsSet000;
                    int nIsSet100;
                    int nIsSet010;
                    int nIsSet110;
                    int nIsSet001;
                    int nIsSet101;
                    int nIsSet011;
                    int nIsSet111;



//                    Debug.Log("=====================================================================================");
//                    Debug.Log(intX.ToString() + ", " + intY.ToString() + " ," + intZ.ToString());


                    // Get from generic position.

                    //
                    // Sync cache.
                    //
                    // During this pass, the "Active" layer is "Z", the "Write" layer is for the "next "Z + 1" layer...
                    // When the Z layer is done, the pointers are swapped,
                    //  so the "active" layer is now for the "bottom" (can be read from cache) and the "write" layer is for the "top" of the current cube...
                    //
                    // NOTE: Will always compute nIsSet111 (at the end). This cannot come from cache.

                    if (intZ == 0)      // ------------------ >>> z = 0 the very bottom slice...
                    {
                        // We may be on the very bottom slice, but we may be able to use values already computed...

                        if (intY == 0)
                        {
                            if (intX == 0)
                            {
                                // z = 0, y = 0, x = 0

                                // For this case, must compute & cache everything (except for pos 1,1,1 which is always done last)!

                                nIsSet000 = CanFormTriangleEx(intXfull, intYfull, intZfull);
                                nIsSet100 = CanFormTriangleEx(intXfull + 12, intYfull, intZfull);
                                nIsSet010 = CanFormTriangleEx(intXfull, intYfull + 12, intZfull);
                                nIsSet110 = CanFormTriangleEx(intXfull + 12, intYfull + 12, intZfull);

                                nIsSet001 = CanFormTriangleEx(intXfull, intYfull, intZfull + 12);
                                nIsSet101 = CanFormTriangleEx(intXfull + 12, intYfull, intZfull + 12);
                                nIsSet011 = CanFormTriangleEx(intXfull, intYfull + 12, intZfull + 12);

                                // write "bottom" and "top" face to cache.

                                xcc[intX, intY, xccActiveLayer].status = nIsSet000;
                                xcc[intX + 1, intY, xccActiveLayer].status = nIsSet100;
                                xcc[intX, intY + 1, xccActiveLayer].status = nIsSet010;
                                xcc[intX + 1, intY + 1, xccActiveLayer].status = nIsSet110;
                                    // Only write to this ActiveLayer when z = 0!

                                xcc[intX, intY, xccWriteLayer].status = nIsSet001;
                                xcc[intX + 1, intY, xccWriteLayer].status = nIsSet101;
                                xcc[intX, intY + 1, xccWriteLayer].status = nIsSet011;

                            }
                            else 
                            {
                                // z = 0, y = 0, x > 0.

                                // We have just stepped into the cache.
                                // We can look back to the last cell to get the "left" face of the cell (x = 0)

                                nIsSet000 = xcc[intX, intY, xccActiveLayer].status;
                                nIsSet100 = CanFormTriangleEx(intXfull + 12, intYfull, intZfull);
                                nIsSet010 = xcc[intX, intY + 1, xccActiveLayer].status;
                                nIsSet110 = CanFormTriangleEx(intXfull + 12, intYfull + 12, intZfull);

                                nIsSet001 = xcc[intX, intY, xccWriteLayer].status;
                                nIsSet101 = CanFormTriangleEx(intXfull + 12, intYfull, intZfull + 12);
                                nIsSet011 = xcc[intX, intY + 1, xccWriteLayer].status;

                                // Write new vertices to cache.

                                xcc[intX + 1, intY, xccActiveLayer].status = nIsSet100;
                                xcc[intX + 1, intY + 1, xccActiveLayer].status = nIsSet110;
                                    // Only write to this ActiveLayer when z = 0!

                                xcc[intX + 1, intY, xccWriteLayer].status = nIsSet101;
                            }


                        }
                        else
                        {
                            // intY is not zero, so even though we shouldn't read the "write" layer, we CAN look at the previous Y row.

                            if (intX == 0)
                            {
                                // z = 0, y > 0, x = 0

                                // We have just stepped into  new row in the cache.

                                nIsSet000 = xcc[intX, intY, xccActiveLayer].status;
                                nIsSet100 = xcc[intX + 1, intY, xccActiveLayer].status;
                                nIsSet010 = CanFormTriangleEx(intXfull, intYfull + 12, intZfull);
                                nIsSet110 = CanFormTriangleEx(intXfull + 12, intYfull + 12, intZfull);

                                nIsSet001 = xcc[intX, intY, xccWriteLayer].status;
                                nIsSet101 = xcc[intX + 1, intY, xccWriteLayer].status; 
                                nIsSet011 = CanFormTriangleEx(intXfull, intYfull + 12, intZfull + 12);

                                // Write new vertices to cache.

                                xcc[intX, intY + 1, xccActiveLayer].status = nIsSet010;
                                xcc[intX + 1, intY + 1, xccActiveLayer].status = nIsSet110;
                                    // Only write to this ActiveLayer when z = 0!

                                xcc[intX, intY + 1, xccWriteLayer].status = nIsSet011;
                            }
                            else
                            {
                                // z = 0, y > 0, x > 0

                                // We are in the middle of a row in the middle of the bottom plane...

                                nIsSet000 = xcc[intX, intY, xccActiveLayer].status;
                                nIsSet100 = CanFormTriangleEx(intXfull + 12, intYfull, intZfull);
                                nIsSet010 = xcc[intX, intY + 1, xccActiveLayer].status;
                                nIsSet110 = CanFormTriangleEx(intXfull + 12, intYfull + 12, intZfull);

                                nIsSet001 = xcc[intX, intY, xccWriteLayer].status;
                                nIsSet101 = CanFormTriangleEx(intXfull + 12, intYfull, intZfull + 12);
                                nIsSet011 = xcc[intX, intY + 1, xccWriteLayer].status;

                                // Write new vertices to cache.

                                xcc[intX + 1, intY + 1, xccActiveLayer].status = nIsSet110;
                                    // Only write to this ActiveLayer when z = 0!

                                xcc[intX + 1, intY, xccWriteLayer].status = nIsSet101;

                            }

                        }

                    }
                    else                // ------------------ >>> z > 0, we are well into the cache...
                    {
                        // Get the "bottom" of this cube from previous z slice!
                        nIsSet000 = xcc[intX, intY, xccActiveLayer].status;
                        nIsSet100 = xcc[intX + 1, intY, xccActiveLayer].status;
                        nIsSet010 = xcc[intX, intY + 1, xccActiveLayer].status;
                        nIsSet110 = xcc[intX + 1, intY + 1, xccActiveLayer].status;

                        // We are not on the very bottom z-slice, but we may be on the edge...

                        if (intY == 0)
                        {

                            if (intX == 0)
                            {
                                // z > 0, y = 0, x = 0.

                                // For this case, must compute new values for the top of the cube!
                                nIsSet001 = CanFormTriangleEx(intXfull, intYfull, intZfull + 12);
                                nIsSet101 = CanFormTriangleEx(intXfull + 12, intYfull, intZfull + 12);
                                nIsSet011 = CanFormTriangleEx(intXfull, intYfull + 12, intZfull + 12);

                                // Write new vertices to cache.

                                xcc[intX, intY, xccWriteLayer].status = nIsSet001;
                                xcc[intX + 1, intY, xccWriteLayer].status = nIsSet101;
                                xcc[intX, intY + 1, xccWriteLayer].status = nIsSet011;

                            }
                            else
                            {
                                // z > 0, y = 0, x > 0.

                                // For this case, must compute a value.
                                nIsSet001 = xcc[intX, intY, xccWriteLayer].status;
                                nIsSet101 = CanFormTriangleEx(intXfull + 12, intYfull, intZfull + 12);
                                nIsSet011 = xcc[intX, intY + 1, xccWriteLayer].status;

                                // Write new vertices to cache.

                                xcc[intX + 1, intY, xccWriteLayer].status = nIsSet101;


                            }

                        }
                        else
                        {
                            if (intX == 0)
                            {
                                // z > 0, y > 0, x = 0.

                                nIsSet001 = xcc[intX, intY, xccWriteLayer].status;
                                nIsSet101 = xcc[intX + 1, intY, xccWriteLayer].status;

                                nIsSet011 = CanFormTriangleEx(intXfull, intYfull + 12, intZfull + 12);

                                // Save new value to cache.

                                xcc[intX, intY + 1, xccWriteLayer].status = nIsSet011;
                            }
                            else
                            {
                                // Just check cache!
                                nIsSet011 = xcc[intX, intY + 1, xccWriteLayer].status;      // was WL!
                                nIsSet001 = xcc[intX, intY, xccWriteLayer].status;          // was WL!!!
                                nIsSet101 = xcc[intX + 1, intY, xccWriteLayer].status;

                                // NOTE: Here, in the "generic" position, the only thing to add is the new point!
                            }
                        }
                    }

                    // Compute last new vertex for this cube cell.
                    nIsSet111 = CanFormTriangleEx(intXfull + 12, intYfull + 12, intZfull + 12);

                    // Save to cache
                    xcc[intX + 1, intY + 1, xccWriteLayer].status = nIsSet111;


                    if (parameters.validate) // check cache!
                    {
                        int nCacheErrors1 = 0;
                        int nCacheErrors2 = 0;
                        int nCacheErrors3 = 0;
                        int nCacheErrors4 = 0;
                        int nCacheErrors5 = 0;
                        int nCacheErrors6 = 0;
                        int nCacheErrors7 = 0;
                        int nCacheErrors8 = 0;

                        int nIsSet000new = CanFormTriangleEx(intXfull, intYfull, intZfull);
                        int nIsSet010new = CanFormTriangleEx(intXfull, intYfull + 12, intZfull);
                        int nIsSet100new = CanFormTriangleEx(intXfull + 12, intYfull, intZfull);
                        int nIsSet110new = CanFormTriangleEx(intXfull + 12, intYfull + 12, intZfull);

                        int nIsSet001new = CanFormTriangleEx(intXfull, intYfull, intZfull + 12);
                        int nIsSet011new = CanFormTriangleEx(intXfull, intYfull + 12, intZfull + 12);
                        int nIsSet101new = CanFormTriangleEx(intXfull + 12, intYfull, intZfull + 12);
                        int nIsSet111new = CanFormTriangleEx(intXfull + 12, intYfull + 12, intZfull + 12);


                        if (nIsSet000 != CanFormTriangleEx(intXfull, intYfull, intZfull)) nCacheErrors1++;
                        if (nIsSet010 != CanFormTriangleEx(intXfull, intYfull + 12, intZfull)) nCacheErrors2++;
                        if (nIsSet100 != CanFormTriangleEx(intXfull + 12, intYfull, intZfull)) nCacheErrors3++;
                        if (nIsSet110 != CanFormTriangleEx(intXfull + 12, intYfull + 12, intZfull)) nCacheErrors4++;

                        if (nIsSet001 != CanFormTriangleEx(intXfull, intYfull, intZfull + 12)) nCacheErrors5++;
                        if (nIsSet011 != CanFormTriangleEx(intXfull, intYfull + 12, intZfull + 12)) nCacheErrors6++;
                        if (nIsSet101 != CanFormTriangleEx(intXfull + 12, intYfull, intZfull + 12)) nCacheErrors7++;
                        if (nIsSet111 != CanFormTriangleEx(intXfull + 12, intYfull + 12, intZfull + 12)) nCacheErrors8++;

                        if (nCacheErrors1 != 0 || nCacheErrors2 != 0 || nCacheErrors3 != 0 || nCacheErrors4 != 0 || nCacheErrors5 != 0 || nCacheErrors6 != 0 || nCacheErrors7 != 0 || nCacheErrors8 != 0)
                        {
                            int nCacheErrors = nCacheErrors1 + nCacheErrors2 + nCacheErrors3 + nCacheErrors4 + nCacheErrors5 + nCacheErrors6 + nCacheErrors7 + nCacheErrors8;
                            Debug.Log("Cache errors: " + nCacheErrors.ToString() + "(" + intX + "," + intY + "," + intZ + ")");
                            stats.errorCode = ERROR_CACHE;
                        }
                    }


                    // Don't bother if cube corners are all fully in or fully out.
                    if (nIsSet000 == 0 || nIsSet100 == 0 || nIsSet010 == 0 || nIsSet110 == 0 || nIsSet001 == 0 || nIsSet101 == 0 || nIsSet011 == 0 || nIsSet111 == 0)
                    {
                        if (parameters.computeVolume)
                        {
                            stats.nMeasured++;
                            MeasureCell(intXfull, intYfull, intZfull, ref stats.nSubCellsB, ref stats.nSubCellsS, ref stats.nSubCellsE);
                        }

                        Vector3Int v000i = new Vector3Int(intXfull, intYfull, intZfull);
                        Vector3Int v100i = new Vector3Int(intXfull + 12, intYfull, intZfull);
                        Vector3Int v010i = new Vector3Int(intXfull, intYfull + 12, intZfull);
                        Vector3Int v110i = new Vector3Int(intXfull + 12, intYfull + 12, intZfull);
                        Vector3Int v001i = new Vector3Int(intXfull, intYfull, intZfull + 12);
                        Vector3Int v101i = new Vector3Int(intXfull + 12, intYfull, intZfull + 12);
                        Vector3Int v011i = new Vector3Int(intXfull, intYfull + 12, intZfull + 12);
                        Vector3Int v111i = new Vector3Int(intXfull + 12, intYfull + 12, intZfull + 12);


                        // Show "base" vertex v000 if needed.

                        if (parameters.displayVertices)
                        {
                            if (CanFormTriangleVertex(intXfull, intYfull, intZfull) == 0)
                            {
                                DrawVertex(x0, y0, z0, vertexMaterial);
                            }
                        }


                        // Flat faces

                        CheckFlatFace(v000i, v010i, v011i, v001i, 0, vu010, vu001);    // Along x = 0
                        CheckFlatFace(v000i, v001i, v101i, v100i, 1, vu001, vu100);    // Along y = 0
                        CheckFlatFace(v000i, v100i, v110i, v010i, 2, vu100, vu010);    // Along z = 0


                        // Diagonals across faces

                        CheckDiagonalFace(v000i, v100i, v111i, v011i, 3, vu100, vu011); // Along x
                        CheckDiagonalFace(v010i, v110i, v101i, v001i, 4, vu100, vu0m11); // Along x

                        CheckDiagonalFace(v000i, v010i, v111i, v101i, 5, vu010, vu101); // Along y
                        CheckDiagonalFace(v001i, v011i, v110i, v100i, 6, vu010, vu10m1); // Along y

                        CheckDiagonalFace(v000i, v001i, v111i, v110i, 7, vu001, vu110); // Along z
                        CheckDiagonalFace(v010i, v011i, v101i, v100i, 8, vu001, vu1m10); // Along z


                        // Corners

                        CheckCornerTriangle(v100i, v010i, v001i, 9, vum110, vum101);   // Around 000
                        CheckCornerTriangle(v110i, v101i, v011i, 9, vu0m11, vum101);   // Around 111

                        CheckCornerTriangle(v000i, v101i, v110i, 10, vu101, vu110);  // Around 100
                        CheckCornerTriangle(v111i, v001i, v010i, 10, vum1m10, vum10m1);  // Around 011

                        CheckCornerTriangle(v100i, v111i, v001i, 11, vu011, vum101);  // Around 101
                        CheckCornerTriangle(v110i, v000i, v011i, 11, vum1m10, vum101);  // Around 010

                        CheckCornerTriangle(v101i, v011i, v000i, 12, vum110, vum10m1);  // Around 001
                        CheckCornerTriangle(v111i, v100i, v010i, 12, vu0m1m1, vum10m1);  // Around 110
                    }
                    else if (nIsSet000 == 1 && nIsSet100 == 1 && nIsSet010 == 1 && nIsSet110 == 1 && nIsSet001 == 1 && nIsSet101 == 1 && nIsSet011 == 1 && nIsSet111 == 1)
                    {
                        stats.nFullyIn++;

                        if (parameters.validate)
                        {
                            int cellsB = 0;
                            int cellsS = 0;
                            int cellsE = 0;

                            MeasureCell(intXfull, intYfull, intZfull, ref cellsB, ref cellsS, ref cellsE);

                            if (cellsB != 24 || cellsS != 24 || cellsE != 48)
                            {
                                Debug.Log("fully in mismatch");
                                stats.errorCode = ERROR_NOT_FULL_VOLUME_IN_FULL_IN;
                            }
                        }
                    }
                    else
                    {
                        stats.nFullyOut++;

                        if (parameters.validate)
                        {
                            int cellsB = 0;
                            int cellsS = 0;
                            int cellsE = 0;

                            MeasureCell(intXfull, intYfull, intZfull, ref cellsB, ref cellsS, ref cellsE);

                            if (cellsB != 0 || cellsS != 0 || cellsE != 0)
                            {
                                Debug.Log("fully out mismatch");
                                stats.errorCode = ERROR_VOLUME_IN_FULL_OUT;
                            }
                        }
                    }
                }
            }


            // We have done the entire x-y slab.
            // Now, swap xcc cache, and proceed to next slab in the z direction.

            int temp = xccActiveLayer;
            xccActiveLayer = xccWriteLayer;
            xccWriteLayer = temp;
        }
    }


    //-----------------------------------------------------
    //-----------------------------------------------------

    // Draw a vertex at the "zero surface", if applicable.

    public void DrawVertex(float x0, float y0, float z0, Material material)
    {
        s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s.transform.parent = mfMain.transform;

        s.transform.localPosition = new Vector3(x0, y0, z0);
        s.transform.localScale = new Vector3(parameters.scale, parameters.scale, parameters.scale);

        s.GetComponent<Renderer>().material = material;
        s.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        myList.Add(s);
    }


    public void ResetMesh(int mesh)
    {
        myNumVerts[mesh] = 0;
        myNumTriangles[mesh] = 0;
        myVerts[mesh] = new List<Vector3>();
        myTriangles[mesh] = new List<int>();
        mfSub[mesh].mesh.Clear();
    }


    public void ProcessMesh(int mesh)
    {
        mfSub[mesh].mesh.vertices = myVerts[mesh].ToArray();
        mfSub[mesh].mesh.triangles = myTriangles[mesh].ToArray();
        mfSub[mesh].mesh.RecalculateBounds();
        mfSub[mesh].mesh.RecalculateNormals();
    }


    //-----------------------------------------------------
    //-----------------------------------------------------

    //
    // Utils for vectors
    //

    // Convert a full integer coord to a float from 0 to 1.
    private float IntToFloat(int coord)
    {
        return (float)coord / parameters.fullMax;
    }


    // Convert float coord (0 - 1) into world coordinate.
    private float CubeToWorld(float coord)
    {
        return coord * parameters.size - parameters.sizeOnTwo;
    }


    private float GridToWorld(int coord)
    {
        return ((float)coord / parameters.fullMax) * parameters.size - parameters.sizeOnTwo;
    }


    public Vector3Int MixVectors3Int(Vector3Int baseVector, Vector3Int dir1, int mag1, Vector3Int dir2, int mag2)
    {
        Vector3Int result = new Vector3Int(baseVector.x + dir1.x * mag1 + dir2.x * mag2, baseVector.y + dir1.y * mag1 + dir2.y * mag2, baseVector.z + dir1.z * mag1 + dir2.z * mag2);

        return result;
    }


    public Vector3 IntVectorToWorld(Vector3Int intVector)
    {
        Vector3 result = new Vector3(GridToWorld(intVector.x), GridToWorld(intVector.y), GridToWorld(intVector.z));

        return result;
    }

    //-----------------------------------------------------

    //-----------------------------------------------------

    public void CheckPrimitiveTriangle(Vector3Int v1, Vector3Int v2, Vector3Int v3, int inMid, int mesh)
    {
        if (inMid == 0) AddTriangleBoth(IntVectorToWorld(v1), IntVectorToWorld(v2), IntVectorToWorld(v3), mesh);
    }


    public void CheckPrimitiveQuad(Vector3Int v1, Vector3Int v2, Vector3Int v3, Vector3Int v4, int inMid, int mesh)
    {
        if (inMid == 0) AddQuadBoth(IntVectorToWorld(v1), IntVectorToWorld(v2), IntVectorToWorld(v4), IntVectorToWorld(v3), mesh);
    }

    //-----------------------------------------------------

    //-----------------------------------------------------
    //
    // "Generic" handlers for the shapes, and their internal "primative" triangles.
    //
    // FIRST: Recompute stuff as needed...
    //

    // Check the "outer" faces of the cubic lattice.
    // Here, we assume the vertex vectors are in steps of 12.
    // This allows perfect integer arithmetic to determine "zero-triangularity".

    public void CheckFlatFace(Vector3Int v00, Vector3Int v0C, Vector3Int vCC, Vector3Int vC0, int mesh, Vector3Int vu00to0C, Vector3Int vu00toC0)
    {
        // "Middle" points of possible triangular sub-facets...

        Vector3Int v36 = MixVectors3Int(v00, vu00toC0, 3, vu00to0C, 6);
        Vector3Int v69 = MixVectors3Int(v00, vu00toC0, 6, vu00to0C, 9);
        Vector3Int v96 = MixVectors3Int(v00, vu00toC0, 9, vu00to0C, 6);
        Vector3Int v63 = MixVectors3Int(v00, vu00toC0, 6, vu00to0C, 3);

        // Get the status of the middle points.

        int in36 = CanFormTriangle(v36);
        int in69 = CanFormTriangle(v69);
        int in96 = CanFormTriangle(v96);
        int in63 = CanFormTriangle(v63);

        if (in36 == 0 && in69 == 0 && in96 == 0 && in63 == 0)
        {
            AddQuadBoth(IntVectorToWorld(v00), IntVectorToWorld(v0C), IntVectorToWorld(vC0), IntVectorToWorld(vCC), mesh);

            stats.nFullFlats++;
        }
        else if (in36 == 0 || in69 == 0 || in96 == 0 || in63 == 0)
        {
            // Get the rest of the "points of interest".

            Vector3Int v66 = MixVectors3Int(v00, vu00toC0, 6, vu00to0C, 6);

            // Form triangles where possible.
            // The last test is for the "internal" point.
            // Go clockwise...

            CheckPrimitiveTriangle(v00, v66, vC0, in63, mesh);
            CheckPrimitiveTriangle(v00, v0C, v66, in36, mesh);
            CheckPrimitiveTriangle(v0C, vCC, v66, in69, mesh);
            CheckPrimitiveTriangle(vCC, vC0, v66, in96, mesh);

            stats.nPartialFlats++;
        }
    }


    // Check the "diagonal" rectangles of the cubic lattice.
    public void CheckDiagonalFace(Vector3Int v000, Vector3Int v00C, Vector3Int vCCC, Vector3Int vCC0, int mesh, Vector3Int vu000to00C, Vector3Int vu000toCC0)
    {
        // "Middle" points.

        Vector3Int v442 = MixVectors3Int(v000, vu000toCC0, 4, vu000to00C, 2);
        Vector3Int v334 = MixVectors3Int(v000, vu000toCC0, 3, vu000to00C, 4);
        Vector3Int v226 = MixVectors3Int(v000, vu000toCC0, 2, vu000to00C, 6);
        Vector3Int v554 = MixVectors3Int(v000, vu000toCC0, 5, vu000to00C, 4);
        Vector3Int v446 = MixVectors3Int(v000, vu000toCC0, 4, vu000to00C, 6);
        Vector3Int v338 = MixVectors3Int(v000, vu000toCC0, 3, vu000to00C, 8);
        Vector3Int v558 = MixVectors3Int(v000, vu000toCC0, 5, vu000to00C, 8);
        Vector3Int v44A = MixVectors3Int(v000, vu000toCC0, 4, vu000to00C, 10);
        Vector3Int v882 = MixVectors3Int(v000, vu000toCC0, 8, vu000to00C, 2);
        Vector3Int v994 = MixVectors3Int(v000, vu000toCC0, 9, vu000to00C, 4);
        Vector3Int vAA6 = MixVectors3Int(v000, vu000toCC0, 10, vu000to00C, 6);
        Vector3Int v774 = MixVectors3Int(v000, vu000toCC0, 7, vu000to00C, 4);
        Vector3Int v886 = MixVectors3Int(v000, vu000toCC0, 8, vu000to00C, 6);
        Vector3Int v998 = MixVectors3Int(v000, vu000toCC0, 9, vu000to00C, 8);
        Vector3Int v778 = MixVectors3Int(v000, vu000toCC0, 7, vu000to00C, 8);
        Vector3Int v88A = MixVectors3Int(v000, vu000toCC0, 8, vu000to00C, 10);


        // Get the status of the middle points.

        int in442 = CanFormTriangle(v442);
        int in334 = CanFormTriangle(v334);
        int in226 = CanFormTriangle(v226);
        int in554 = CanFormTriangle(v554);
        int in446 = CanFormTriangle(v446);
        int in338 = CanFormTriangle(v338);
        int in558 = CanFormTriangle(v558);
        int in44A = CanFormTriangle(v44A);

        int in882 = CanFormTriangle(v882);
        int in994 = CanFormTriangle(v994);
        int inAA6 = CanFormTriangle(vAA6);
        int in774 = CanFormTriangle(v774);
        int in886 = CanFormTriangle(v886);
        int in998 = CanFormTriangle(v998);
        int in778 = CanFormTriangle(v778);
        int in88A = CanFormTriangle(v88A);

        if (in442 == 0 && in334 == 0 && in226 == 0 && in554 == 0 && in446 == 0 && in338 == 0
            && in558 == 0 && in44A == 0 && in882 == 0 && in994 == 0 && inAA6 == 0 && in774 == 0
            && in886 == 0 && in998 == 0 && in778 == 0 && in88A == 0)
        {
            AddQuadBoth(IntVectorToWorld(v000), IntVectorToWorld(v00C), IntVectorToWorld(vCC0), IntVectorToWorld(vCCC), mesh);

            stats.nFullDiagonals++;
        }
        else if (in442 == 0 || in334 == 0 || in226 == 0 || in554 == 0 || in446 == 0 || in338 == 0
            || in558 == 0 || in44A == 0 || in882 == 0 || in994 == 0 || inAA6 == 0 || in774 == 0
            || in886 == 0 || in998 == 0 || in778 == 0 || in88A == 0)
        {
            // Get the rest of the "points of interest".
            Vector3Int v666 = MixVectors3Int(v000, vu000toCC0, 6, vu000to00C, 6);

            Vector3Int v660 = MixVectors3Int(v000, vu000toCC0, 6, vu000to00C, 0);
            Vector3Int v66C = MixVectors3Int(v000, vu000toCC0, 6, vu000to00C, 12);

            Vector3Int v444 = MixVectors3Int(v000, vu000toCC0, 4, vu000to00C, 4);
            Vector3Int v336 = MixVectors3Int(v000, vu000toCC0, 3, vu000to00C, 6);
            Vector3Int v448 = MixVectors3Int(v000, vu000toCC0, 4, vu000to00C, 8);
            Vector3Int v884 = MixVectors3Int(v000, vu000toCC0, 8, vu000to00C, 4);
            Vector3Int v996 = MixVectors3Int(v000, vu000toCC0, 9, vu000to00C, 6);
            Vector3Int v888 = MixVectors3Int(v000, vu000toCC0, 8, vu000to00C, 8);

            // Form triangles where possible.
            // The last test is for the "internal" point.
            // Go clockwise...
            CheckPrimitiveTriangle(v000, v444, v660, in442, mesh);      // 0
            CheckPrimitiveTriangle(v000, v336, v444, in334, mesh);      // 1
            CheckPrimitiveTriangle(v000, v00C, v336, in226, mesh);      // 2
            CheckPrimitiveTriangle(v660, v444, v666, in554, mesh);      // 3

            CheckPrimitiveQuad(v448, v666, v444, v336, in446, mesh);    // 4

            CheckPrimitiveTriangle(v00C, v448, v336, in338, mesh);      // 5
            CheckPrimitiveTriangle(v66C, v666, v448, in558, mesh);      // 6
            CheckPrimitiveTriangle(v00C, v66C, v448, in44A, mesh);      // 7

            CheckPrimitiveTriangle(v660, v884, vCC0, in882, mesh);      // 8
            CheckPrimitiveTriangle(v884, v996, vCC0, in994, mesh);      // 9
            CheckPrimitiveTriangle(vCCC, vCC0, v996, inAA6, mesh);      // 10
            CheckPrimitiveTriangle(v660, v666, v884, in774, mesh);      // 11

            CheckPrimitiveQuad(v666, v888, v996, v884, in886, mesh);    // 12

            CheckPrimitiveTriangle(v888, vCCC, v996, in998, mesh);      // 13
            CheckPrimitiveTriangle(v666, v66C, v888, in778, mesh);      // 14
            CheckPrimitiveTriangle(v66C, vCCC, v888, in88A, mesh);      // 15

            stats.nPartialDiagonals++;
        }
    }


    // Check the "corner" triangles of the cubic lattice.

    public void CheckCornerTriangle(Vector3Int v00C, Vector3Int v0C0, Vector3Int vC00, int mesh, Vector3Int vu00Cto0C0, Vector3Int vu00CtoC00)
    {
        // "Middle" points.

        Vector3Int v417 = MixVectors3Int(v00C, vu00CtoC00, 4, vu00Cto0C0, 1);  // 0
        Vector3Int v147 = MixVectors3Int(v00C, vu00CtoC00, 1, vu00Cto0C0, 4);  // 1
        Vector3Int v174 = MixVectors3Int(v00C, vu00CtoC00, 1, vu00Cto0C0, 7);  // 2
        Vector3Int v471 = MixVectors3Int(v00C, vu00CtoC00, 4, vu00Cto0C0, 7);  // 3
        Vector3Int v741 = MixVectors3Int(v00C, vu00CtoC00, 7, vu00Cto0C0, 4);  // 4
        Vector3Int v714 = MixVectors3Int(v00C, vu00CtoC00, 7, vu00Cto0C0, 1);  // 5
        Vector3Int v435 = MixVectors3Int(v00C, vu00CtoC00, 4, vu00Cto0C0, 3);  // 6
        Vector3Int v345 = MixVectors3Int(v00C, vu00CtoC00, 3, vu00Cto0C0, 4);  // 7
        Vector3Int v354 = MixVectors3Int(v00C, vu00CtoC00, 3, vu00Cto0C0, 5);  // 8
        Vector3Int v453 = MixVectors3Int(v00C, vu00CtoC00, 4, vu00Cto0C0, 5);  // 9
        Vector3Int v543 = MixVectors3Int(v00C, vu00CtoC00, 5, vu00Cto0C0, 4);  // 10
        Vector3Int v534 = MixVectors3Int(v00C, vu00CtoC00, 5, vu00Cto0C0, 3);  // 11

        // Get the status of the middle points.

        int in417 = CanFormTriangle(v417);
        int in147 = CanFormTriangle(v147);
        int in174 = CanFormTriangle(v174);
        int in471 = CanFormTriangle(v471);
        int in741 = CanFormTriangle(v741);
        int in714 = CanFormTriangle(v714);

        int in435 = CanFormTriangle(v435);
        int in345 = CanFormTriangle(v345);
        int in354 = CanFormTriangle(v354);
        int in453 = CanFormTriangle(v453);
        int in543 = CanFormTriangle(v543);
        int in534 = CanFormTriangle(v534);

        if (in417 == 0 && in147 == 0 && in174 == 0 && in471 == 0 && in741 == 0 && in714 == 0
            && in435 == 0 && in345 == 0 && in354 == 0 && in453 == 0 && in543 == 0 && in534 == 0)
        {
            AddTriangleBoth(IntVectorToWorld(v00C), IntVectorToWorld(v0C0), IntVectorToWorld(vC00), mesh);

            stats.nFullCorners++;
        }
        else if (in417 == 0 || in147 == 0 || in174 == 0 || in471 == 0 || in741 == 0 || in714 == 0
            || in435 == 0 || in345 == 0 || in354 == 0 || in453 == 0 || in543 == 0 || in534 == 0)
        {
            // Get the rest of the "points of interest".
            Vector3Int v444 = MixVectors3Int(v00C, vu00CtoC00, 4, vu00Cto0C0, 4);
            Vector3Int v066 = MixVectors3Int(v00C, vu00CtoC00, 0, vu00Cto0C0, 6);
            Vector3Int v363 = MixVectors3Int(v00C, vu00CtoC00, 3, vu00Cto0C0, 6);
            Vector3Int v660 = MixVectors3Int(v00C, vu00CtoC00, 6, vu00Cto0C0, 6);
            Vector3Int v336 = MixVectors3Int(v00C, vu00CtoC00, 3, vu00Cto0C0, 3);
            Vector3Int v633 = MixVectors3Int(v00C, vu00CtoC00, 6, vu00Cto0C0, 3);
            Vector3Int v606 = MixVectors3Int(v00C, vu00CtoC00, 6, vu00Cto0C0, 0);

            // Form triangles where possible.
            // The last test is for the "internal" point.
            // Go clockwise...

            CheckPrimitiveTriangle(v00C, v336, v606, in417, mesh);             // 0
            CheckPrimitiveTriangle(v00C, v066, v336, in147, mesh);             // 1
            CheckPrimitiveTriangle(v066, v0C0, v363, in174, mesh);             // 2
            CheckPrimitiveTriangle(v0C0, v660, v363, in471, mesh);             // 3
            CheckPrimitiveTriangle(v633, v660, vC00, in741, mesh);             // 4
            CheckPrimitiveTriangle(v606, v633, vC00, in714, mesh);             // 5

            CheckPrimitiveTriangle(v336, v444, v606, in435, mesh);             // 6
            CheckPrimitiveTriangle(v066, v444, v336, in345, mesh);             // 7
            CheckPrimitiveTriangle(v363, v444, v066, in354, mesh);             // 8
            CheckPrimitiveTriangle(v660, v444, v363, in453, mesh);             // 9
            CheckPrimitiveTriangle(v633, v444, v660, in543, mesh);             // 10
            CheckPrimitiveTriangle(v606, v444, v633, in534, mesh);             // 11

            stats.nPartialCorners++;
        }
    }


    //-----------------------------------------------------
    //-----------------------------------------------------

    //
    // Mesh utils...
    //

    public void AddQuadBoth(Vector3 v00, Vector3 v01, Vector3 v10, Vector3 v11, int mesh)
    {
        if (myNumVerts[mesh] > MAXTVERTS) return;

        myVerts[mesh].Add(v00);
        myVerts[mesh].Add(v10);
        myVerts[mesh].Add(v01);
        myVerts[mesh].Add(v11);

        myVerts[mesh].Add(v00);
        myVerts[mesh].Add(v10);
        myVerts[mesh].Add(v01);
        myVerts[mesh].Add(v11);

        myTriangles[mesh].Add(myNumVerts[mesh] + 0);
        myTriangles[mesh].Add(myNumVerts[mesh] + 2);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1);

        myTriangles[mesh].Add(myNumVerts[mesh] + 2);
        myTriangles[mesh].Add(myNumVerts[mesh] + 3);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1);

        // Other side;
        myTriangles[mesh].Add(myNumVerts[mesh] + 0 + 4);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1 + 4);
        myTriangles[mesh].Add(myNumVerts[mesh] + 2 + 4);

        myTriangles[mesh].Add(myNumVerts[mesh] + 2 + 4);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1 + 4);
        myTriangles[mesh].Add(myNumVerts[mesh] + 3 + 4);

        myNumVerts[mesh] += 8;
    }


    public void AddTriangleBoth(Vector3 v00, Vector3 v01, Vector3 v10, int mesh)
    {
        if (myNumVerts[mesh] > MAXTVERTS) return;

        myVerts[mesh].Add(v00);
        myVerts[mesh].Add(v01);
        myVerts[mesh].Add(v10);

        myVerts[mesh].Add(v00);
        myVerts[mesh].Add(v10);
        myVerts[mesh].Add(v01);

        myTriangles[mesh].Add(myNumVerts[mesh] + 0);
        myTriangles[mesh].Add(myNumVerts[mesh] + 2);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1);

        // Other side;
        myTriangles[mesh].Add(myNumVerts[mesh] + 0 + 3);
        myTriangles[mesh].Add(myNumVerts[mesh] + 2 + 3);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1 + 3);

        myNumVerts[mesh] += 6;
    }


    //-----------------------------------------------------
    //-----------------------------------------------------

    //
    // "Zero triangle" test utils...
    //

    public int CanFormTriangle(Vector3Int v)
    {
        int nResult = CanFormTriangleEx(v.x, v.y, v.z);

        return nResult;
    }


    // 
    public int CanFormTriangleEx(int s1, int s2, int s3)
    {

        int nResult;
        if (parameters.dropdownEdgesInt == 2)
        {
            nResult = CanFormTriangle5Int(s1, s2, s3);
        }
        else if (parameters.dropdownEdgesInt == 1)
        {
            nResult = CanFormTriangle4Int(s1, s2, s3);
        }
        else
        {
            nResult = CanFormTriangle3Int(s1, s2, s3);
        }

        if (parameters.doClosure)
        {
            if (nResult == 1)
            {
                if (s1 == 0 || s1 == parameters.nFullDivisions || s2 == 0 || s2 == parameters.nFullDivisions || s3 == 0 || s3 == parameters.nFullDivisions)
                {
                    nResult = 0;
                }
            }
        }
        return nResult;
    }


    // Only put vertex spheres at "true" zero points.

    public int CanFormTriangleVertex(int s1, int s2, int s3)
    {
        int nResult;
        if (parameters.dropdownEdgesInt == 2)
        {
            nResult = CanFormTriangle5Int(s1, s2, s3);
        }
        else if (parameters.dropdownEdgesInt == 1)
        {
            nResult = CanFormTriangle4Int(s1, s2, s3);
        }
        else
        {
            nResult = CanFormTriangle3Int(s1, s2, s3);
        }

        return nResult;
    }


    //-----------------------------------------------------

    // Given 3 edges, return:
    //
    // +1   Can form a solid triangle.
    // 0    Can form a "zero triangle".
    // -1   Cannot form a triangle.
    // -2   An edge is out of bounds.

    public int CanFormTriangle3Int(int s1, int s2, int s3)
    {
        if (s1 < 0 || s1 > parameters.nFullDivisions) return -2;
        if (s2 < 0 || s2 > parameters.nFullDivisions) return -2;
        if (s3 < 0 || s3 > parameters.nFullDivisions) return -2;

        //==============

        // test 3 - 4 divs first easy fail.

        // try : x1 = x2 planes
        int test = 0;

        switch (test)
        {
            case 1: // ok
                if (s1 > s2) return -1;
                if (s1 == s2) return 0;
                return 1;
            case 2:
                if (s1 > s3) return -1;
                if (s1 == s3) return 0;
                return 1;
            case 3:
                if (s2 > s3) return -1;
                if (s2 == s3) return 0;
                return 1;

            case 4: // ok
                if (s1 + s2 < parameters.nFullDivisions) return -1;
                if (s1 + s2 == parameters.nFullDivisions) return 0;
                return 1;

        }


        //
        //==============

        if (s1 > s2 + s3) return -1;
        if (s2 > s1 + s3) return -1;
        if (s3 > s1 + s2) return -1;

        if (s1 == s2 + s3) return 0;
        if (s2 == s1 + s3) return 0;
        if (s3 == s1 + s2) return 0;

        return +1;
    }


    //-----------------------------------------------------

    // Given the 4 edges, return:
    //
    // +1   Can form a solid triangle.
    // 0    Can form a "zero triangle".
    // -1   Cannot form a triangle.
    // -2   An edge is out of bounds.

    public int CanFormTriangle4Int(int s1, int s2, int s3)
    {
        int s4 = parameters.sliderFullInt;

        if (s1 < 0 || s1 > parameters.nFullDivisions) return -2;
        if (s2 < 0 || s2 > parameters.nFullDivisions) return -2;
        if (s3 < 0 || s3 > parameters.nFullDivisions) return -2;
        if (s4 < 0 || s4 > parameters.nFullDivisions) return -2;

        int c1 = CanFormTriangle3Int(s2, s3, s4);
        int c2 = CanFormTriangle3Int(s1, s3, s4);
        int c3 = CanFormTriangle3Int(s1, s2, s4);
        int c4 = CanFormTriangle3Int(s1, s2, s3);

        if (c1 == +1) return +1;
        if (c2 == +1) return +1;
        if (c3 == +1) return +1;
        if (c4 == +1) return +1;

        if (c1 == 0) return 0;
        if (c2 == 0) return 0;
        if (c3 == 0) return 0;
        if (c4 == 0) return 0;

        return -1;
    }


    //-----------------------------------------------------

    // Given the 5 edges, return:
    //
    // +1   Can form a solid triangle.
    // 0    Can form a "zero triangle".
    // -1   Cannot form a triangle.
    // -2   An edge is out of bounds.

    public int CanFormTriangle5Int(int s1, int s2, int s3)
    {
        int s4 = parameters.sliderFullInt;
        int s5 = parameters.sliderFullInt5thEdge;

        if (s1 < 0 || s1 > parameters.nFullDivisions) return -2;
        if (s2 < 0 || s2 > parameters.nFullDivisions) return -2;
        if (s3 < 0 || s3 > parameters.nFullDivisions) return -2;
        if (s4 < 0 || s4 > parameters.nFullDivisions) return -2;
        if (s5 < 0 || s5 > parameters.nFullDivisions) return -2;

        int c1 = CanFormTriangle3Int(s3, s4, s5);
        int c2 = CanFormTriangle3Int(s2, s4, s5);
        int c3 = CanFormTriangle3Int(s2, s3, s5);
        int c4 = CanFormTriangle3Int(s2, s3, s4);
        int c5 = CanFormTriangle3Int(s1, s4, s5);
        int c6 = CanFormTriangle3Int(s1, s3, s5);
        int c7 = CanFormTriangle3Int(s1, s3, s4);
        int c8 = CanFormTriangle3Int(s1, s2, s5);
        int c9 = CanFormTriangle3Int(s1, s2, s4);
        int c10 = CanFormTriangle3Int(s1, s2, s3);


        if (c1 == +1) return +1;
        if (c2 == +1) return +1;
        if (c3 == +1) return +1;
        if (c4 == +1) return +1;
        if (c5 == +1) return +1;
        if (c6 == +1) return +1;
        if (c7 == +1) return +1;
        if (c8 == +1) return +1;
        if (c9 == +1) return +1;
        if (c10 == +1) return +1;

        if (c1 == 0) return 0;
        if (c2 == 0) return 0;
        if (c3 == 0) return 0;
        if (c4 == 0) return 0;
        if (c5 == 0) return 0;
        if (c6 == 0) return 0;
        if (c7 == 0) return 0;
        if (c8 == 0) return 0;
        if (c9 == 0) return 0;
        if (c10 == 0) return 0;

        return -1;
    }
}
