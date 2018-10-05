using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct CellData
{
    public int status;
    public Vector3 worldCoords;
}


public struct ZeroTriangleParameters
{
    public int nDivisions;     // "Main" divisions.
    public int nFullDivisions; // nDivisions * 12 for finers sub-divisions.

    public bool displayVertices;

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


public class ZeroTriangles : MonoBehaviour {

    // The parameters for the figure that can be controlled externally...
    public ZeroTriangleParameters parameters;

    // Materials.
    public Material vertexMaterial;
    public Material vertexMaterialMarkB;
    public Material vertexMaterialMarkS;
    public Material vertexMaterialMarkE;

    private int nFullFlats;
    private int nFullDiagonals;
    private int nFullCorners;
    private int nFullyInOrOut;

    // Sub-cell counts.
    private int nSubCellsB;
    private int nSubCellsS;
    private int nSubCellsE;

    private int nCellCount;


    // Internal cache for building meshes.
    private int[] myNumVerts;
    private int[] myNumTriangles;
    private List<Vector3>[] myVerts;
    private List<int>[] myTriangles;


    // Mesh gameobjects.
    public GameObject mfMain;

    public MeshFilter[] mfSub;  // Point to the 14 "sub meshes"

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

    private int MAXTVERTS = 65530;

    // List of vertex spheres.
    private GameObject s;
    private ArrayList myList;


    
    // Cache the cube corner info for each layer of (x,y) as we move through z
    private CellData[,,] xcc;    // x,y, layer (0,1)
    private int xccAL;
    private int xccWL;


    // Use this for initialization
    public void Initialise()
    {
        //parameters = new ZeroTriangleParameters();

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
    }


    // We have the internal parameters set.
    // Now, compute the geometry of the figure.
   
    public int ComputeGeometry()
    {
        nFullFlats = 0;
        nFullDiagonals = 0;
        nFullCorners = 0;
        nFullyInOrOut = 0;

        nSubCellsB = 0;
        nSubCellsS = 0;
        nSubCellsE = 0;

        nCellCount = 0;

        foreach (GameObject s in myList)
        {
            Destroy(s);
        }

        for (int i = 0; i < 14; i++)
        {
            ResetMesh(i);
        }


        DoFullFigure();
        //DoTestFigure();


        // Now put the list of triangles in each mesh.
        for (int i = 0; i < 14; i++)
        {
            ProcessMesh(i);
        }

        return nCellCount;

    }


    // Given the "base" point of the 12 x 12 x 12 cell, count up the types of components "inside".
    private void MeasureCell(int xFull, int yFull, int zFull)
    {

        // z = 0 face -----------------------------------

        // y = 0 edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 621
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 643
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 432
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 832

        // y = c edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 6A1
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 683
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 492
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 892

        // x = 0 edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 261
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 463
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 342
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 382

        // x = c edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // A61
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 863
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 942
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 982

        // z = c face -----------------------------------

        // y = 0 edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 62B
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 649
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 43A
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 83A

        // y = c edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 6AB
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 689
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 49A
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 89A

        // x = 0 edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 26B
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 469
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 34A
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 38A

        // x = c edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // A6B
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 869
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 94A
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 98A



        // y = 0 face -----------------------------------

        // z = 0 edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 612
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 634
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 423
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 823

        // z = c edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 61A
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 638
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 429
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 829

        // x = 0 edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 216
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 436
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 324
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 328

        // x = c edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // A16
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 836
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 924
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 928

        // y = c face -----------------------------------

        // z = 0 edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 6B2
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 694
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 4A3
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 8A3

        // z = c edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 6BA
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 698
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 4A9
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 8A9

        // x = 0 edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 2B6
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 496
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 3A4
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 3A8

        // x = c edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // AB6
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 896
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 9A4
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 9A8



        // x = 0 face -----------------------------------

        // z = 0 edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 162
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 364
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 243
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 283

        // z = c edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 16A
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 368
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 249
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 289

        // y = 0 edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 126
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 346
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 234
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 238

        // y = c edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // 1A6
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 386
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 294
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // 298

        // x = c face -----------------------------------

        // z = 0 edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // B62
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 964
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // A43
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // A83

        // z = c edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // B6A
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 968
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // A49
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // A89

        // y = 0 edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // B26
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 946
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // A34
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // A38

        // y = c edge
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsB);  // BA6
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsS);  // 986
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // A94
        CheckSubCell(xFull, yFull, zFull, ref nSubCellsE);  // A98
    }


    private void CheckSubCell(int s1, int s2, int s3, ref int typeCount)
    {
        if (CanFormTriangleVertex(s1, s2, s3) == 1)
        {
            typeCount++;
        }
    }


    private void DoTestFigure()
    {
        float v0;
        float v1;
        float v2;
        float v3;
        float v4;
        float v5;
        float v6;
        float v7;
        float v8;
        float v9;
        float vA;
        float vB;
        float vC;

        v0 = GridToWorld(0);
        v1 = GridToWorld(parameters.nDivisions * 1);
        v2 = GridToWorld(parameters.nDivisions * 2);
        v3 = GridToWorld(parameters.nDivisions * 3);
        v4 = GridToWorld(parameters.nDivisions * 4);
        v5 = GridToWorld(parameters.nDivisions * 5);
        v6 = GridToWorld(parameters.nDivisions * 6);
        v7 = GridToWorld(parameters.nDivisions * 7);
        v8 = GridToWorld(parameters.nDivisions * 8);
        v9 = GridToWorld(parameters.nDivisions * 9);
        vA = GridToWorld(parameters.nDivisions * 10);
        vB = GridToWorld(parameters.nDivisions * 11);
        vC = GridToWorld(parameters.nFullDivisions);


        Vector3 v000 = new Vector3(v0, v0, v0);
        Vector3 vC00 = new Vector3(vC, v0, v0);
        Vector3 v0C0 = new Vector3(v0, vC, v0);
        Vector3 vCC0 = new Vector3(vC, vC, v0);
        Vector3 v00C = new Vector3(v0, v0, vC);
        Vector3 vC0C = new Vector3(vC, v0, vC);
        Vector3 v0CC = new Vector3(v0, vC, vC);
        Vector3 vCCC = new Vector3(vC, vC, vC);

        Vector3 v666 = new Vector3(v6, v6, v6);

        Vector3 v660 = new Vector3(v6, v6, v0);
        Vector3 v600 = new Vector3(v6, v0, v0);
        Vector3 v633 = new Vector3(v6, v3, v3);
        Vector3 v444 = new Vector3(v4, v4, v4);
        Vector3 v844 = new Vector3(v8, v4, v4);

        // z = 0 face -----------------------------------

        // y = 0 edge
        Vector3 v621 = new Vector3(v6, v2, v1); // B
        Vector3 v643 = new Vector3(v6, v4, v3); // S
        Vector3 v432 = new Vector3(v4, v3, v2); // E
        Vector3 v832 = new Vector3(v8, v3, v2); // E

        DoVertex(v621, vertexMaterialMarkB);
        DoVertex(v643, vertexMaterialMarkS);
        DoVertex(v432, vertexMaterialMarkE);
        DoVertex(v832, vertexMaterialMarkE);

        // y = c edge
        Vector3 v6A1 = new Vector3(v6, vA, v1); // B
        Vector3 v683 = new Vector3(v6, v8, v3); // S
        Vector3 v492 = new Vector3(v4, v9, v2); // E
        Vector3 v892 = new Vector3(v8, v9, v2); // E

        DoVertex(v6A1, vertexMaterialMarkB);
        DoVertex(v683, vertexMaterialMarkS);
        DoVertex(v492, vertexMaterialMarkE);
        DoVertex(v892, vertexMaterialMarkE);

        // x = 0 edge
        Vector3 v261 = new Vector3(v2, v6, v1); // B
        Vector3 v463 = new Vector3(v4, v6, v3); // S
        Vector3 v342 = new Vector3(v3, v4, v2); // E
        Vector3 v382 = new Vector3(v3, v8, v2); // E

        DoVertex(v261, vertexMaterialMarkB);
        DoVertex(v463, vertexMaterialMarkS);
        DoVertex(v342, vertexMaterialMarkE);
        DoVertex(v382, vertexMaterialMarkE);

        // x = c edge
        Vector3 vA61 = new Vector3(vA, v6, v1); // B
        Vector3 v863 = new Vector3(v8, v6, v3); // S
        Vector3 v942 = new Vector3(v9, v4, v2); // E
        Vector3 v982 = new Vector3(v9, v8, v2); // E

        DoVertex(vA61, vertexMaterialMarkB);
        DoVertex(v863, vertexMaterialMarkS);
        DoVertex(v942, vertexMaterialMarkE);
        DoVertex(v982, vertexMaterialMarkE);

        // z = c face -----------------------------------

        // y = 0 edge
        Vector3 v62B = new Vector3(v6, v2, vB); // B
        Vector3 v649 = new Vector3(v6, v4, v9); // S
        Vector3 v43A = new Vector3(v4, v3, vA); // E
        Vector3 v83A = new Vector3(v8, v3, vA); // E

        DoVertex(v62B, vertexMaterialMarkB);
        DoVertex(v649, vertexMaterialMarkS);
        DoVertex(v43A, vertexMaterialMarkE);
        DoVertex(v83A, vertexMaterialMarkE);

        // y = c edge
        Vector3 v6AB = new Vector3(v6, vA, vB); // B
        Vector3 v689 = new Vector3(v6, v8, v9); // S
        Vector3 v49A = new Vector3(v4, v9, vA); // E
        Vector3 v89A = new Vector3(v8, v9, vA); // E

        DoVertex(v6AB, vertexMaterialMarkB);
        DoVertex(v689, vertexMaterialMarkS);
        DoVertex(v49A, vertexMaterialMarkE);
        DoVertex(v89A, vertexMaterialMarkE);

        // x = 0 edge
        Vector3 v26B = new Vector3(v2, v6, vB); // B
        Vector3 v469 = new Vector3(v4, v6, v9); // S
        Vector3 v34A = new Vector3(v3, v4, vA); // E
        Vector3 v38A = new Vector3(v3, v8, vA); // E

        DoVertex(v26B, vertexMaterialMarkB);
        DoVertex(v469, vertexMaterialMarkS);
        DoVertex(v34A, vertexMaterialMarkE);
        DoVertex(v38A, vertexMaterialMarkE);

        // x = c edge
        Vector3 vA6B = new Vector3(vA, v6, vB); // B
        Vector3 v869 = new Vector3(v8, v6, v9); // S
        Vector3 v94A = new Vector3(v9, v4, vA); // E
        Vector3 v98A = new Vector3(v9, v8, vA); // E

        DoVertex(vA6B, vertexMaterialMarkB);
        DoVertex(v869, vertexMaterialMarkS);
        DoVertex(v94A, vertexMaterialMarkE);
        DoVertex(v98A, vertexMaterialMarkE);

        //--------------------------------------------------
        //--------------------------------------------------


        // y = 0 face -----------------------------------

        // z = 0 edge
        Vector3 v612 = new Vector3(v6, v1, v2); // B
        Vector3 v634 = new Vector3(v6, v3, v4); // S
        Vector3 v423 = new Vector3(v4, v2, v3); // E
        Vector3 v823 = new Vector3(v8, v2, v3); // E

        DoVertex(v612, vertexMaterialMarkB);
        DoVertex(v634, vertexMaterialMarkS);
        DoVertex(v423, vertexMaterialMarkE);
        DoVertex(v823, vertexMaterialMarkE);

        // z = c edge
        Vector3 v61A = new Vector3(v6, v1, vA); // B
        Vector3 v638 = new Vector3(v6, v3, v8); // S
        Vector3 v429 = new Vector3(v4, v2, v9); // E
        Vector3 v829 = new Vector3(v8, v2, v9); // E

        DoVertex(v61A, vertexMaterialMarkB);
        DoVertex(v638, vertexMaterialMarkS);
        DoVertex(v429, vertexMaterialMarkE);
        DoVertex(v829, vertexMaterialMarkE);

        // x = 0 edge......
        Vector3 v216 = new Vector3(v2, v1, v6); // B
        Vector3 v436 = new Vector3(v4, v3, v6); // S
        Vector3 v324 = new Vector3(v3, v2, v4); // E
        Vector3 v328 = new Vector3(v3, v2, v8); // E

        DoVertex(v216, vertexMaterialMarkB);
        DoVertex(v436, vertexMaterialMarkS);
        DoVertex(v324, vertexMaterialMarkE);
        DoVertex(v328, vertexMaterialMarkE);

        // x = c edge
        Vector3 vA16 = new Vector3(vA, v1, v6); // B
        Vector3 v836 = new Vector3(v8, v3, v6); // S
        Vector3 v924 = new Vector3(v9, v2, v4); // E
        Vector3 v928 = new Vector3(v9, v2, v8); // E

        DoVertex(vA16, vertexMaterialMarkB);
        DoVertex(v836, vertexMaterialMarkS);
        DoVertex(v924, vertexMaterialMarkE);
        DoVertex(v928, vertexMaterialMarkE);

        // y = c face -----------------------------------

        // z = 0 edge
        Vector3 v6B2 = new Vector3(v6, vB, v2); // B
        Vector3 v694 = new Vector3(v6, v9, v4); // S
        Vector3 v4A3 = new Vector3(v4, vA, v3); // E
        Vector3 v8A3 = new Vector3(v8, vA, v3); // E

        DoVertex(v6B2, vertexMaterialMarkB);
        DoVertex(v694, vertexMaterialMarkS);
        DoVertex(v4A3, vertexMaterialMarkE);
        DoVertex(v8A3, vertexMaterialMarkE);

        // z = c edge
        Vector3 v6BA = new Vector3(v6, vB, vA); // B
        Vector3 v698 = new Vector3(v6, v9, v8); // S
        Vector3 v4A9 = new Vector3(v4, vA, v9); // E
        Vector3 v8A9 = new Vector3(v8, vA, v9); // E

        DoVertex(v6BA, vertexMaterialMarkB);
        DoVertex(v698, vertexMaterialMarkS);
        DoVertex(v4A9, vertexMaterialMarkE);
        DoVertex(v8A9, vertexMaterialMarkE);

        // x = 0 edge
        Vector3 v2B6 = new Vector3(v2, vB, v6); // B
        Vector3 v496 = new Vector3(v4, v9, v6); // S
        Vector3 v3A4 = new Vector3(v3, vA, v4); // E
        Vector3 v3A8 = new Vector3(v3, vA, v8); // E

        DoVertex(v2B6, vertexMaterialMarkB);
        DoVertex(v496, vertexMaterialMarkS);
        DoVertex(v3A4, vertexMaterialMarkE);
        DoVertex(v3A8, vertexMaterialMarkE);

        // x = c edge
        Vector3 vAB6 = new Vector3(vA, vB, v6); // B
        Vector3 v896 = new Vector3(v8, v9, v6); // S
        Vector3 v9A4 = new Vector3(v9, vA, v4); // E
        Vector3 v9A8 = new Vector3(v9, vA, v8); // E

        DoVertex(vAB6, vertexMaterialMarkB);
        DoVertex(v896, vertexMaterialMarkS);
        DoVertex(v9A4, vertexMaterialMarkE);
        DoVertex(v9A8, vertexMaterialMarkE);

        //--------------------------------------------------
        //--------------------------------------------------

        // x = 0 face -----------------------------------

        // z = 0 edge
        Vector3 v162 = new Vector3(v1, v6, v2); // B
        Vector3 v364 = new Vector3(v3, v6, v4); // S
        Vector3 v243 = new Vector3(v2, v4, v3); // E
        Vector3 v283 = new Vector3(v2, v8, v3); // E..

        DoVertex(v162, vertexMaterialMarkB);
        DoVertex(v364, vertexMaterialMarkS);
        DoVertex(v243, vertexMaterialMarkE);
        DoVertex(v283, vertexMaterialMarkE);

        // z = c edge
        Vector3 v16A = new Vector3(v1, v6, vA); // B
        Vector3 v368 = new Vector3(v3, v6, v8); // S
        Vector3 v249 = new Vector3(v2, v4, v9); // E
        Vector3 v289 = new Vector3(v2, v8, v9); // E

        DoVertex(v16A, vertexMaterialMarkB);
        DoVertex(v368, vertexMaterialMarkS);
        DoVertex(v249, vertexMaterialMarkE);
        DoVertex(v289, vertexMaterialMarkE);

        // y = 0 edge......
        Vector3 v126 = new Vector3(v1, v2, v6); // B
        Vector3 v346 = new Vector3(v3, v4, v6); // S
        Vector3 v234 = new Vector3(v2, v3, v4); // E
        Vector3 v238 = new Vector3(v2, v3, v8); // E

        DoVertex(v126, vertexMaterialMarkB);
        DoVertex(v346, vertexMaterialMarkS);
        DoVertex(v234, vertexMaterialMarkE);
        DoVertex(v238, vertexMaterialMarkE);

        // y = c edge
        Vector3 v1A6 = new Vector3(v1, vA, v6); // B
        Vector3 v386 = new Vector3(v3, v8, v6); // S
        Vector3 v294 = new Vector3(v2, v9, v4); // E
        Vector3 v298 = new Vector3(v2, v9, v8); // E

        DoVertex(v1A6, vertexMaterialMarkB);
        DoVertex(v386, vertexMaterialMarkS);
        DoVertex(v294, vertexMaterialMarkE);
        DoVertex(v298, vertexMaterialMarkE);

        // x = c face -----------------------------------

        // z = 0 edge
        Vector3 vB62 = new Vector3(vB, v6, v2); // B
        Vector3 v964 = new Vector3(v9, v6, v4); // S
        Vector3 vA43 = new Vector3(vA, v4, v3); // E
        Vector3 vA83 = new Vector3(vA, v8, v3); // E

        DoVertex(vB62, vertexMaterialMarkB);
        DoVertex(v964, vertexMaterialMarkS);
        DoVertex(vA43, vertexMaterialMarkE);
        DoVertex(vA83, vertexMaterialMarkE);

        // z = c edge
        Vector3 vB6A = new Vector3(vB, v6, vA); // B
        Vector3 v968 = new Vector3(v9, v6, v8); // S
        Vector3 vA49 = new Vector3(vA, v4, v9); // E
        Vector3 vA89 = new Vector3(vA, v8, v9); // E

        DoVertex(vB6A, vertexMaterialMarkB);
        DoVertex(v968, vertexMaterialMarkS);
        DoVertex(vA49, vertexMaterialMarkE);
        DoVertex(vA89, vertexMaterialMarkE);

        // y = 0 edge
        Vector3 vB26 = new Vector3(vB, v2, v6); // B
        Vector3 v946 = new Vector3(v9, v4, v6); // S
        Vector3 vA34 = new Vector3(vA, v3, v4); // E
        Vector3 vA38 = new Vector3(vA, v3, v8); // E

        DoVertex(vB26, vertexMaterialMarkB);
        DoVertex(v946, vertexMaterialMarkS);
        DoVertex(vA34, vertexMaterialMarkE);
        DoVertex(vA38, vertexMaterialMarkE);

        // y = c edge
        Vector3 vBA6 = new Vector3(vB, vA, v6); // B
        Vector3 v986 = new Vector3(v9, v8, v6); // S
        Vector3 vA94 = new Vector3(vA, v9, v4); // E
        Vector3 vA98 = new Vector3(vA, v9, v8); // E

        DoVertex(vBA6, vertexMaterialMarkB);
        DoVertex(v986, vertexMaterialMarkS);
        DoVertex(vA94, vertexMaterialMarkE);
        DoVertex(vA98, vertexMaterialMarkE);

        //--------------------------------------------------


        AddTriangleBoth(v000, vC00, v666, 1);

        AddTriangleBoth(v444, v660, vC00, 1);
        AddTriangleBoth(v000, v660, v844, 1);

        float x0;
        float y0;
        float z0;

        for (int intX = 0; intX <= parameters.nDivisions; intX++)
        {
            int intXfull = intX * 12;
            x0 = GridToWorld(intXfull);

            for (int intY = 0; intY <= parameters.nDivisions; intY++)
            {
                int intYfull = intY * 12;
                y0 = GridToWorld(intYfull);

                for (int intZ = 0; intZ <= parameters.nDivisions; intZ++)
                {
                    int intZfull = intZ * 12;
                    z0 = GridToWorld(intZfull);

                    s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    s.transform.parent = mfMain.transform;

                    s.transform.localPosition = new Vector3(x0, y0, z0);    // v000;
                    s.transform.localScale = new Vector3(parameters.scale, parameters.scale, parameters.scale);

                    s.GetComponent<Renderer>().material = vertexMaterial;
                    s.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;


                    myList.Add(s);
                }
            }
        }
    }


    private void DoVertex(Vector3 v, Material material)
    {
        s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s.transform.parent = mfMain.transform;

        s.transform.localPosition = v;
        s.transform.localScale = new Vector3(parameters.scale, parameters.scale, parameters.scale);

        s.GetComponent<Renderer>().material = material;
        s.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;


        myList.Add(s);
    }


    private void DoFullFigure()
    {
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

        xcc = new CellData[parameters.nDivisions + 2, parameters.nDivisions + 2, 2];

        for (int a = 0; a <= parameters.nDivisions + 1; a++)
        {
            for (int b = 0; b <= parameters.nDivisions + 1; b++)
            {
                xcc[a, b, 0] = new CellData();
                xcc[a, b, 1] = new CellData();
            }
        }

        xccAL = 0;
        xccWL = 1;


        float x0;
        float y0;
        float z0;

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

                    MeasureCell(intXfull, intYfull, intZfull);

                    x0 = GridToWorld(intXfull);

                    int nIsSet000;
                    int nIsSet100;
                    int nIsSet010;
                    int nIsSet110;
                    int nIsSet001;
                    int nIsSet101;
                    int nIsSet011;
                    int nIsSet111;


                    // Get from generic position.

                    //
                    // Sync cache.
                    //

                    if (intZ == 0)
                    {
                        // Case #1: X is 0. Y is 0. Z is 0.
                        //
                        // For this case, must compute everything!
                        nIsSet000 = CanFormTriangleEx(intXfull, intYfull, intZfull);
                        nIsSet100 = CanFormTriangleEx(intXfull + 12, intYfull, intZfull);
                        nIsSet010 = CanFormTriangleEx(intXfull, intYfull + 12, intZfull);
                        nIsSet110 = CanFormTriangleEx(intXfull + 12, intYfull + 12, intZfull);

                        nIsSet001 = CanFormTriangleEx(intXfull, intYfull, intZfull + 12);
                        nIsSet101 = CanFormTriangleEx(intXfull + 12, intYfull, intZfull + 12);
                        nIsSet011 = CanFormTriangleEx(intXfull, intYfull + 12, intZfull + 12);

                        xcc[intX, intY, xccWL].status = nIsSet001;
                        xcc[intX + 1, intY, xccWL].status = nIsSet101;
                        xcc[intX, intY + 1, xccWL].status = nIsSet011;

                    }
                    else
                    {
                        // Get from previous z slice!
                        nIsSet000 = xcc[intX, intY, xccAL].status;
                        nIsSet100 = xcc[intX + 1, intY, xccAL].status;
                        nIsSet010 = xcc[intX, intY + 1, xccAL].status;
                        nIsSet110 = xcc[intX + 1, intY + 1, xccAL].status;
                    }


                    if (intY == 0)
                    {
                        // For this case, must compute new y values!
                        nIsSet001 = CanFormTriangleEx(intXfull, intYfull, intZfull + 12);
                        nIsSet101 = CanFormTriangleEx(intXfull + 12, intYfull, intZfull + 12);

                        //xcc[intX, intY, xccWL].status = nIsSet001;
                        //xcc[intX + 1, intY, xccWL].status = nIsSet101;

                    }
                    else
                    {
                        // Get from previous y run!
                        nIsSet001 = xcc[intX, intY - 1, xccWL].status;
                        nIsSet101 = xcc[intX + 1, intY - 1, xccWL].status;
                    }


                    if (intX == 0)
                    {
                        // For this case, must compute new x values!
                        nIsSet011 = CanFormTriangleEx(intXfull, intYfull, intZfull + 12);
                    }
                    else
                    {
                        // Get from previous x shot!
                        nIsSet011 = xcc[intX - 1, intY, xccWL].status;
                    }



                    // Compute new vertex for this cube cell. and save to cache.
                    nIsSet111 = CanFormTriangleEx(intXfull + 12, intYfull + 12, intZfull + 12);

                    xcc[intX + 1, intY + 1, xccWL].status = nIsSet111;


                    // Don't bother if cube corners are all fully in or fully out.
                    if (nIsSet000 == 0 || nIsSet100 == 0 || nIsSet010 == 0 || nIsSet110 == 0 || nIsSet001 == 0 || nIsSet101 == 0 || nIsSet011 == 0 || nIsSet111 == 0)
                    {
                        Vector3Int v000i = new Vector3Int(intXfull, intYfull, intZfull);
                        Vector3Int v100i = new Vector3Int(intXfull + 12, intYfull, intZfull);
                        Vector3Int v010i = new Vector3Int(intXfull, intYfull + 12, intZfull);
                        Vector3Int v110i = new Vector3Int(intXfull + 12, intYfull + 12, intZfull);
                        Vector3Int v001i = new Vector3Int(intXfull, intYfull, intZfull + 12);
                        Vector3Int v101i = new Vector3Int(intXfull + 12, intYfull, intZfull + 12);
                        Vector3Int v011i = new Vector3Int(intXfull, intYfull + 12, intZfull + 12);
                        Vector3Int v111i = new Vector3Int(intXfull + 12, intYfull + 12, intZfull + 12);


                        // Show "base" vertex if needed.

                        if (parameters.displayVertices)
                        {
                            DrawVertex(intXfull, intYfull, intZfull, x0, y0, z0);
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
                    else
                    {
                        nFullyInOrOut++;
                    }
                }
            }

            // We have done the entire x-y slab.
            // Now, swap xcc cache, and proceed to next slab in the z direction.
            int temp = xccAL;
            xccAL = xccWL;
            xccWL = temp;
        }
    }


    // Draw a vertex at the "zero surface", if applicable.
    public void DrawVertex(int xFull, int yFull, int zFull, float x0, float y0, float z0)
    {
        if (CanFormTriangleVertex(xFull, yFull, zFull) == 0)
        {
            s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.parent = mfMain.transform;

            s.transform.localPosition = new Vector3(x0, y0, z0);
            s.transform.localScale = new Vector3(parameters.scale, parameters.scale, parameters.scale);

            s.GetComponent<Renderer>().material = vertexMaterial;
            s.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            myList.Add(s);
        }
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


    public void CheckPrimitiveTriangle(Vector3Int v1, Vector3Int v2, Vector3Int v3, int inMid, int mesh)
    {
        if (inMid == 0) AddTriangleBoth(IntVectorToWorld(v1), IntVectorToWorld(v2), IntVectorToWorld(v3), mesh);
    }


    public void CheckPrimitiveQuad(Vector3Int v1, Vector3Int v2, Vector3Int v3, Vector3Int v4, int inMid, int mesh)
    {
        if (inMid == 0) AddQuadBoth(IntVectorToWorld(v1), IntVectorToWorld(v2), IntVectorToWorld(v4), IntVectorToWorld(v3), mesh);
    }


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
        // "Middle" points.
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
            nFullFlats++;
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
            nFullDiagonals++;
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
            nFullCorners++;
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
        }
    }


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


    public int CanFormTriangle(Vector3Int v)
    {
        int nResult = CanFormTriangleEx(v.x, v.y, v.z);

        return nResult;
    }


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
