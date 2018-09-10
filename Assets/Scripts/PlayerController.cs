using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


// Player controller
public class PlayerController : MonoBehaviour
{
    public Material vertexMaterial;

    public int nFullFlats;
    public int nFullDiagonals;
    public int nFullCorners;
    public int nFullyInOrOut;

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


    public GameObject goFrame;

    // External game objects.
    public Light directionalLight;

    // Internal cache for building meshes.
    public int [] myNumVerts;
    public int [] myNumTriangles;
    public List<Vector3> [] myVerts;
    public List<int> [] myTriangles;

    // Internal parameters.
    private float size;
    private float sizeOnTwo;

    private int nDivisions;     // "Main" divisions.
    private int nFullDivisions; // nDivisions * 12 for finers sub-divisions.

    bool displayVertices;
    float vertexSize;

    private float max;
    private float fullMax;

    private float scale;

    private int sliderFullInt;
    private int sliderFullInt5thEdge;

    private int dropdownEdgesInt;
    private bool doClosure;

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


    public CameraController mainCamController;
    public Camera mainCam;


    void Start()
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
        size = 10.0f;               // Size of the "configuration cube".
        sizeOnTwo = size / 2.0f;    // Used to center the cube.

        GetParametersFromControls();
        SetLightFromControls();
        ComputeGeometry();
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
            mfMain.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * 100.0f * SliderAnimateSpeed.value);
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
            ComputeGeometry();
        }
    }


    // Read the geometry parameters from the controls,
    // and work out the internal parameters.
    public bool GetParametersFromControls()
    {
        bool changed = false;

        // Lattice divisions.
        if (nDivisions != (int)sliderDivisions.value)
        {
            nDivisions = (int)sliderDivisions.value;
            changed = true;
        }

        textDivisions.text = "Divisions: " + nDivisions.ToString();

        // Edges
        if (dropdownEdgesInt != DropdownEdges.value)
        {
            dropdownEdgesInt = DropdownEdges.value;
            changed = true;
        }

        // 4th edge
        float sliderFloat = slider4thEdge.value;
        int sliderInt = (int)(sliderFloat * (nDivisions + 1));
        if (sliderInt > nDivisions) sliderInt = nDivisions;

        // 5th edge
        float sliderFloat5thEdge = slider5thEdge.value;
        int sliderInt5thEdge = (int)(sliderFloat5thEdge * (nDivisions + 1));
        if (sliderInt5thEdge > nDivisions) sliderInt5thEdge = nDivisions;


        text4thEdge.text = "Edges: " + sliderInt.ToString() + " " + sliderInt5thEdge.ToString();

        // Vertices
        if (displayVertices != togglePoints.isOn)
        {
            displayVertices = togglePoints.isOn;
            changed = true;
        }

        if (vertexSize != sliderVertexSize.value)
        {
            vertexSize = sliderVertexSize.value;
            changed = true;
        }


        if (doClosure != toggleClosure.isOn)
        {
            doClosure = toggleClosure.isOn;
            changed = true;
        }


        // Internal parameters.
        nFullDivisions = nDivisions * 12;
        if (sliderFullInt != sliderInt * 12)
        {
            sliderFullInt = sliderInt * 12;
            changed = true;
        }
        if (sliderFullInt5thEdge != sliderInt5thEdge * 12)
        {
            sliderFullInt5thEdge = sliderInt5thEdge * 12;
            changed = true;
        }

        max = (float)nDivisions;
        fullMax = (float)nFullDivisions;

        scale = size / max * vertexSize + 0.05f;

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
            mfMain.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
        }
        else
        {
            mfMain.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }


    public void ResetAnimation()
    {
        toggleAnimate.isOn = false;
        mfMain.transform.localEulerAngles = Vector3.zero;
    }

    public void ResetCamera()
    {
        mainCamController.azimuthElevation.azimuth = 0;
        mainCamController.azimuthElevation.elevation = 0;
        mainCamController.SetCameraAzimuthElevation(mainCamController.azimuthElevation);
    }

    public void CountSubCellOccupancy()
    {

    }

    // We have the internal parameters set.
    // Now, compute the geometry of the figure.
    public void ComputeGeometry()
    {
        nFullFlats = 0;
        nFullDiagonals = 0;
        nFullCorners = 0;
        nFullyInOrOut = 0;

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


        //TextStatus.text = "F: " + nFullFlats.ToString() + " D:" + nFullDiagonals.ToString() + " C:" + nFullCorners.ToString() + " IO:" + nFullyInOrOut.ToString();
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
        v1 = GridToWorld(nDivisions * 1);
        v2 = GridToWorld(nDivisions * 2);
        v3 = GridToWorld(nDivisions * 3);
        v4 = GridToWorld(nDivisions * 4);
        v5 = GridToWorld(nDivisions * 5);
        v6 = GridToWorld(nDivisions * 6);
        v7 = GridToWorld(nDivisions * 7);
        v8 = GridToWorld(nDivisions * 8);
        v9 = GridToWorld(nDivisions * 9);
        vA = GridToWorld(nDivisions * 0);
        vB = GridToWorld(nDivisions * 11);
        vC = GridToWorld(nFullDivisions);


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


        AddTriangleBoth(v000, vC00, v666, 1);

        AddTriangleBoth(v444, v660, vC00, 1);
        AddTriangleBoth(v000, v660, v844, 1);

        /*

        //AddQuadBoth(v000, v010, v001, v011, 1);     // Left
        //AddQuadBoth(v100, v110, v101, v111, 1);     // Right

        //AddQuadBoth(v000, v010, v100, v110, 1);     // Font


        // AddQuadBoth(v000, v010, v101, v111, 1);     // along x
        //AddQuadBoth(v100, v110, v001, v011, 1);     // 

        AddQuadBoth(v000, vC00, v0CC, vCCC, 1);     // along y
        AddQuadBoth(v0C0, vCC0, v00C, vC0C, 1);     // 

        AddQuadBoth(v000, vCC0, v00C, vCCC, 1);     // along z
        AddQuadBoth(v0C0, vC00, v0CC, vC0C, 1);     // 


        AddTriangleBoth(v0C0, vC00, v00C, 1);     // 000
        //AddTriangleBoth(v011, v101, v000, 1);     // 001
        AddTriangleBoth(v000, vCC0, v0CC, 1);     // 010
        AddTriangleBoth(vCC0, v000, vC0C, 1);     // 100

        AddTriangleBoth(vC00, v0C0, vCCC, 1);     // 110
        //AddTriangleBoth(v111, v001, v100, 1);     // 101

        //AddTriangleBoth(v101, v011, v110, 1);     // 111
        //AddTriangleBoth(v001, v111, v010, 1);     // 011
        */


        float x0;
        float y0;
        float z0;

        for (int intX = 0; intX <= nDivisions; intX++)
        {
            int intXfull = intX * 12;

            x0 = GridToWorld(intXfull);
            //x8 = cubeToWorld(intToFloat(intXfull + 12));

            for (int intY = 0; intY <= nDivisions; intY++)
            {
                int intYfull = intY * 12;

                y0 = GridToWorld(intYfull);
                //y8 = cubeToWorld(intToFloat(intYfull + 12));

                for (int intZ = 0; intZ <= nDivisions; intZ++)
                {
                    int intZfull = intZ * 12;

                    z0 = GridToWorld(intZfull);
                    //z8 = cubeToWorld(intToFloat(intZfull + 12));


                    {
                        s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        s.transform.parent = mfMain.transform;

                        s.transform.localPosition = new Vector3(x0, y0, z0);    // v000;
                        s.transform.localScale = new Vector3(scale, scale, scale);

                        s.GetComponent<Renderer>().material = vertexMaterial;
                        s.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;


                        myList.Add(s);
                    }

                }
            }
        }
    }


    private void DoFullFigure()
    {
        Vector3Int v010 = new Vector3Int(0, 1, 0);
        Vector3Int v001 = new Vector3Int(0, 0, 1);
        Vector3Int v011 = new Vector3Int(0, 1, 1);
        Vector3Int v0m11 = new Vector3Int(0, -1, 1);
        Vector3Int v100 = new Vector3Int(1, 0, 0);
        Vector3Int v101 = new Vector3Int(1, 0, 1);
        Vector3Int v110 = new Vector3Int(1, 1, 0);
        Vector3Int v10m1 = new Vector3Int(1, 0, -1);
        Vector3Int v1m10 = new Vector3Int(1, -1, 0);

        Vector3Int vm110 = new Vector3Int(-1, 1, 0);
        Vector3Int vm101 = new Vector3Int(-1, 0, 1);

        Vector3Int vm1m10 = new Vector3Int(-1, -1, 0);
        Vector3Int vm10m1 = new Vector3Int(-1, 0, -1);

        Vector3Int v0m1m1 = new Vector3Int(0, -1, -1);

        float x0;
        float y0;
        float z0;

        for (int intX = 0; intX <= nDivisions; intX++)
        {
            int intXfull = intX * 12;

            x0 = GridToWorld(intXfull);
            //x8 = cubeToWorld(intToFloat(intXfull + 12));

            for (int intY = 0; intY <= nDivisions; intY++)
            {
                int intYfull = intY * 12;

                y0 = GridToWorld(intYfull);
                //y8 = cubeToWorld(intToFloat(intYfull + 12));

                for (int intZ = 0; intZ <= nDivisions; intZ++)
                {
                    int intZfull = intZ * 12;

                    z0 = GridToWorld(intZfull);
                    //z8 = cubeToWorld(intToFloat(intZfull + 12));


                    //if (x > y && y > z)
                    {
                        int nIsSet000 = CanFormTriangleEx(intXfull, intYfull, intZfull);
                        int nIsSet100 = CanFormTriangleEx(intXfull + 12, intYfull, intZfull);
                        int nIsSet010 = CanFormTriangleEx(intXfull, intYfull + 12, intZfull);
                        int nIsSet110 = CanFormTriangleEx(intXfull + 12, intYfull + 12, intZfull);
                        int nIsSet001 = CanFormTriangleEx(intXfull, intYfull, intZfull + 12);
                        int nIsSet101 = CanFormTriangleEx(intXfull + 12, intYfull, intZfull + 12);
                        int nIsSet011 = CanFormTriangleEx(intXfull, intYfull + 12, intZfull + 12);
                        int nIsSet111 = CanFormTriangleEx(intXfull + 12, intYfull + 12, intZfull + 12);

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
                            if (displayVertices && CanFormTriangleVertex(intXfull, intYfull, intZfull) == 0)
                            {
                                s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                s.transform.parent = mfMain.transform;

                                s.transform.localPosition = new Vector3(x0, y0, z0);    // v000;
                                s.transform.localScale = new Vector3(scale, scale, scale);

                                s.GetComponent<Renderer>().material = vertexMaterial;
                                s.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                                myList.Add(s);
                            }


                            // Flat faces

                            CheckFlatFace(v000i, v010i, v011i, v001i, 0, v010, v001);    // Along x = 0
                            CheckFlatFace(v000i, v001i, v101i, v100i, 1, v001, v100);    // Along y = 0
                            CheckFlatFace(v000i, v100i, v110i, v010i, 2, v100, v010);    // Along z = 0


                            // Diagonals across faces

                            CheckDiagonalFace(v000i, v100i, v111i, v011i, 3, v100, v011); // Along x
                            CheckDiagonalFace(v010i, v110i, v101i, v001i, 4, v100, v0m11); // Along x

                            CheckDiagonalFace(v000i, v010i, v111i, v101i, 5, v010, v101); // Along y
                            CheckDiagonalFace(v001i, v011i, v110i, v100i, 6, v010, v10m1); // Along y

                            CheckDiagonalFace(v000i, v001i, v111i, v110i, 7, v001, v110); // Along z
                            CheckDiagonalFace(v010i, v011i, v101i, v100i, 8, v001, v1m10); // Along z


                            // Corners

                            CheckCornerTriangle(v100i, v010i, v001i, 9, vm110, vm101);   // Around 000
                            CheckCornerTriangle(v110i, v101i, v011i, 9, v0m11, vm101);   // Around 111

                            CheckCornerTriangle(v000i, v101i, v110i, 10, v101, v110);  // Around 100
                            CheckCornerTriangle(v111i, v001i, v010i, 10, vm1m10, vm10m1);  // Around 011

                            CheckCornerTriangle(v100i, v111i, v001i, 11, v011, vm101);  // Around 101
                            CheckCornerTriangle(v110i, v000i, v011i, 11, vm1m10, vm101);  // Around 010

                            CheckCornerTriangle(v101i, v011i, v000i, 12, vm110, vm10m1);  // Around 001
                            CheckCornerTriangle(v111i, v100i, v010i, 12, v0m1m1, vm10m1);  // Around 110
                        }
                        else
                        {
                            nFullyInOrOut++;
                        }
                    }
                }
            }
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
        return (float)coord / fullMax;
    }


    // Convert float coord (0 - 1) into world coordinate.
    private float CubeToWorld(float coord)
    {
        return coord * size - sizeOnTwo;
    }


    private float GridToWorld(int coord)
    {
        return ((float)coord / fullMax) * size - sizeOnTwo;
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
        else
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
        else
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
        else
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
        if (dropdownEdgesInt == 2)
        {
            nResult = CanFormTriangle5Int(s1, s2, s3);
        }
        else if (dropdownEdgesInt == 1)
        {
            nResult = CanFormTriangle4Int(s1, s2, s3);
        }
        else
        {
            nResult = CanFormTriangle3Int(s1, s2, s3);
        }

        if (doClosure)
        {
            if (nResult == 1)
            {
                if (s1 == 0 || s1 == nFullDivisions || s2 == 0 || s2 == nFullDivisions || s3 == 0 || s3 == nFullDivisions)
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
        if (dropdownEdgesInt == 2)
        {
            nResult = CanFormTriangle5Int(s1, s2, s3);
        }
        else if (dropdownEdgesInt == 1)
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
        if (s1 < 0 || s1 > nFullDivisions) return -2;
        if (s2 < 0 || s2 > nFullDivisions) return -2;
        if (s3 < 0 || s3 > nFullDivisions) return -2;

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
        int s4 = sliderFullInt;

        if (s1 < 0 || s1 > nFullDivisions) return -2;
        if (s2 < 0 || s2 > nFullDivisions) return -2;
        if (s3 < 0 || s3 > nFullDivisions) return -2;
        if (s4 < 0 || s4 > nFullDivisions) return -2;

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
        int s4 = sliderFullInt;
        int s5 = sliderFullInt5thEdge;

        if (s1 < 0 || s1 > nFullDivisions) return -2;
        if (s2 < 0 || s2 > nFullDivisions) return -2;
        if (s3 < 0 || s3 > nFullDivisions) return -2;
        if (s4 < 0 || s4 > nFullDivisions) return -2;
        if (s5 < 0 || s5 > nFullDivisions) return -2;

        int c1 =  CanFormTriangle3Int(s3, s4, s5);
        int c2 =  CanFormTriangle3Int(s2, s4, s5);
        int c3 =  CanFormTriangle3Int(s2, s3, s5);
        int c4 =  CanFormTriangle3Int(s2, s3, s4);
        int c5 =  CanFormTriangle3Int(s1, s4, s5);
        int c6 =  CanFormTriangle3Int(s1, s3, s5);
        int c7 =  CanFormTriangle3Int(s1, s3, s4);
        int c8 =  CanFormTriangle3Int(s1, s2, s5);
        int c9 =  CanFormTriangle3Int(s1, s2, s4);
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
