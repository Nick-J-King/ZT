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

    public Text TextStatus;

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

        float x0;
        float y0;
        float z0;

        float x8;
        float y8;
        float z8;
        x0 = GridToWorld(0);
        y0 = GridToWorld(0);
        z0 = GridToWorld(0);

        x8 = GridToWorld(nFullDivisions);
        y8 = GridToWorld(nFullDivisions);
        z8 = GridToWorld(nFullDivisions);


        Vector3 v000 = new Vector3(x0, y0, z0);
        Vector3 v100 = new Vector3(x8, y0, z0);
        Vector3 v010 = new Vector3(x0, y8, z0);
        Vector3 v110 = new Vector3(x8, y8, z0);
        Vector3 v001 = new Vector3(x0, y0, z8);
        Vector3 v101 = new Vector3(x8, y0, z8);
        Vector3 v011 = new Vector3(x0, y8, z8);
        Vector3 v111 = new Vector3(x8, y8, z8);


        //AddQuadBoth(v000, v010, v001, v011, 1);     // Left
        //AddQuadBoth(v100, v110, v101, v111, 1);     // Right

        //AddQuadBoth(v000, v010, v100, v110, 1);     // Font

        AddQuadBoth(v000, v010, v101, v111, 1);     // 
        AddQuadBoth(v100, v110, v001, v011, 1);     // 

        AddQuadBoth(v000, v100, v011, v111, 1);     // 
        AddQuadBoth(v010, v110, v001, v101, 1);     // 

        AddQuadBoth(v000, v110, v001, v111, 1);     // 
        AddQuadBoth(v010, v100, v011, v101, 1);     // 

        AddTriangleBoth(v010, v100, v001, 1);     // 000
        AddTriangleBoth(v011, v101, v000, 1);     // 001
        AddTriangleBoth(v000, v110, v011, 1);     // 010
        AddTriangleBoth(v110, v000, v101, 1);     // 100

        AddTriangleBoth(v100, v010, v111, 1);     // 110
        AddTriangleBoth(v111, v001, v100, 1);     // 101

        AddTriangleBoth(v101, v011, v110, 1);     // 111
        AddTriangleBoth(v001, v111, v010, 1);     // 011

        if (true)
        {
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

        if (false)
        {
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
                                /*
                                                        Vector3 v000 = new Vector3(x0, y0, z0);
                                                        Vector3 v100 = new Vector3(x8, y0, z0);
                                                        Vector3 v010 = new Vector3(x0, y8, z0);
                                                        Vector3 v110 = new Vector3(x8, y8, z0);
                                                        Vector3 v001 = new Vector3(x0, y0, z8);
                                                        Vector3 v101 = new Vector3(x8, y0, z8);
                                                        Vector3 v011 = new Vector3(x0, y8, z8);
                                                        Vector3 v111 = new Vector3(x8, y8, z8);
                                */
                                Vector3Int v000i = new Vector3Int(intXfull, intYfull, intZfull);
                                Vector3Int v100i = new Vector3Int(intXfull + 12, intYfull, intZfull);
                                Vector3Int v010i = new Vector3Int(intXfull, intYfull + 12, intZfull);
                                Vector3Int v110i = new Vector3Int(intXfull + 12, intYfull + 12, intZfull);
                                Vector3Int v001i = new Vector3Int(intXfull, intYfull, intZfull + 12);
                                Vector3Int v101i = new Vector3Int(intXfull + 12, intYfull, intZfull + 12);
                                Vector3Int v011i = new Vector3Int(intXfull, intYfull + 12, intZfull + 12);
                                Vector3Int v111i = new Vector3Int(intXfull + 12, intYfull + 12, intZfull + 12);

                                //
                                //
                                /*
                                                        int nSet = 0;
                                                        if (nIsSet000 == 0) nSet++;
                                                        if (nIsSet001 == 0) nSet++;
                                                        if (nIsSet010 == 0) nSet++;
                                                        if (nIsSet011 == 0) nSet++;
                                                        if (nIsSet100 == 0) nSet++;
                                                        if (nIsSet101 == 0) nSet++;
                                                        if (nIsSet110 == 0) nSet++;
                                                        if (nIsSet111 == 0) nSet++;
                                */
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

                                CheckFlatFace(v000i, v010i, v011i, v001i, 0);    // Along x = 0
                                CheckFlatFace(v000i, v001i, v101i, v100i, 1);    // Along y = 0
                                CheckFlatFace(v000i, v100i, v110i, v010i, 2);    // Along z = 0


                                // Diagonals across faces

                                CheckDiagonalFace(v000i, v100i, v111i, v011i, 3); // Along x
                                CheckDiagonalFace(v010i, v110i, v101i, v001i, 4); // Along x

                                CheckDiagonalFace(v000i, v010i, v111i, v101i, 5); // Along y
                                CheckDiagonalFace(v001i, v011i, v110i, v100i, 6); // Along y

                                CheckDiagonalFace(v000i, v001i, v111i, v110i, 7); // Along z
                                CheckDiagonalFace(v010i, v011i, v101i, v100i, 8); // Along z


                                // Corners

                                CheckCornerTriangle(v100i, v010i, v001i, 9);   // Around 000
                                CheckCornerTriangle(v110i, v101i, v011i, 9);   // Around 111

                                CheckCornerTriangle(v000i, v101i, v110i, 10);  // Around 100
                                CheckCornerTriangle(v111i, v001i, v010i, 10);  // Around 011

                                CheckCornerTriangle(v100i, v111i, v001i, 11);  // Around 101
                                CheckCornerTriangle(v110i, v000i, v011i, 11);  // Around 010

                                CheckCornerTriangle(v101i, v011i, v000i, 12);  // Around 001
                                CheckCornerTriangle(v111i, v100i, v010i, 12);  // Around 110
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


        // Now put the list of triangles in each mesh.
        for (int i = 0; i < 14; i++)
        {
            ProcessMesh(i);
        }


        TextStatus.text = "F: " + nFullFlats.ToString() + " D:" + nFullDiagonals.ToString() + " C:" + nFullCorners.ToString() + " IO:" + nFullyInOrOut.ToString();
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


    public void CheckPrimitiveTriangle(int in1, Vector3Int v1, int in2, Vector3Int v2, int in3, Vector3Int v3, int inMid, int mesh)
    {
        if (in1 == 0 && in2 == 0 && in3 == 0 && inMid == 0) AddTriangleBoth(IntVectorToWorld(v1), IntVectorToWorld(v2), IntVectorToWorld(v3), mesh);
    }


    public void CheckPrimitiveQuad(int in1, Vector3Int v1, int in2, Vector3Int v2, int in3, Vector3Int v3, int in4, Vector3Int v4, int inMid, int mesh)
    {
        if (in1 == 0 && in2 == 0 && in3 == 0 && in4 == 0 && inMid == 0) AddQuadBoth(IntVectorToWorld(v1), IntVectorToWorld(v2), IntVectorToWorld(v4), IntVectorToWorld(v3), mesh);
    }


    //
    // "Generic" handlers for the shapes, and their internal "primative" triangles.
    //
    // FIRST: Recompute stuff as needed...
    //

    // Check the "outer" faces of the cubic lattice.
    // Here, we assume the vertex vectors are in steps of 12.
    // This allows perfect integer arithmetic to determine "zero-triangularity".
    public void CheckFlatFace(Vector3Int v00, Vector3Int v0C, Vector3Int vCC, Vector3Int vC0, int mesh)
    {
        // Get unit vectors in the square to navigate it.
        Vector3Int vu00toC0 = new Vector3Int(((vC0.x - v00.x) / 12), ((vC0.y - v00.y) / 12), ((vC0.z - v00.z) / 12));   // Unit vector from v00 to vC0
        Vector3Int vu00to0C = new Vector3Int(((v0C.x - v00.x) / 12), ((v0C.y - v00.y) / 12), ((v0C.z - v00.z) / 12));   // Unit vector from v00 to v0C

        // Get the rest of the "points of interest".
        Vector3Int v66 = MixVectors3Int(v00, vu00toC0, 6, vu00to0C, 6);

        // "Middle" points.
        Vector3Int v36 = MixVectors3Int(v00, vu00toC0, 3, vu00to0C, 6);
        Vector3Int v69 = MixVectors3Int(v00, vu00toC0, 6, vu00to0C, 9);
        Vector3Int v96 = MixVectors3Int(v00, vu00toC0, 9, vu00to0C, 6);
        Vector3Int v63 = MixVectors3Int(v00, vu00toC0, 6, vu00to0C, 3);

        // Get the status of the points.
        int in00 = CanFormTriangle(v00);
        int inC0 = CanFormTriangle(vC0);
        int in0C = CanFormTriangle(v0C);
        int inCC = CanFormTriangle(vCC);
        int in66 = CanFormTriangle(v66);

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
            // Form triangles where possible.
            // The last test is for the "internal" point.
            // Go clockwise...
            CheckPrimitiveTriangle(in00, v00, in66, v66, inC0, vC0, in63, mesh);
            CheckPrimitiveTriangle(in00, v00, in0C, v0C, in66, v66, in36, mesh);
            CheckPrimitiveTriangle(in0C, v0C, inCC, vCC, in66, v66, in69, mesh);
            CheckPrimitiveTriangle(inCC, vCC, inC0, vC0, in66, v66, in96, mesh);
        }
    }


    // Check the "diagonal" rectangles of the cubic lattice.
    public void CheckDiagonalFace(Vector3Int v000, Vector3Int v00C, Vector3Int vCCC, Vector3Int vCC0, int mesh)
    {
        // Get unit vectors in the square to navigate it.
        Vector3Int vu000toCC0 = new Vector3Int(((vCC0.x - v000.x) / 12), ((vCC0.y - v000.y) / 12), ((vCC0.z - v000.z) / 12));   // Unit vector from v000 to vCC0
        Vector3Int vu000to00C = new Vector3Int(((v00C.x - v000.x) / 12), ((v00C.y - v000.y) / 12), ((v00C.z - v000.z) / 12));   // Unit vector from v000 to v00C

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

        // Get the status of the points.
        int in000 = CanFormTriangle(v000);
        int in660 = CanFormTriangle(v660);
        int inCC0 = CanFormTriangle(vCC0);
        int in666 = CanFormTriangle(v666);
        int in00C = CanFormTriangle(v00C);
        int in66C = CanFormTriangle(v66C);
        int inCCC = CanFormTriangle(vCCC);

        int in336 = CanFormTriangle(v336);
        int in996 = CanFormTriangle(v996);
        int in444 = CanFormTriangle(v444);
        int in448 = CanFormTriangle(v448);
        int in884 = CanFormTriangle(v884);
        int in888 = CanFormTriangle(v888);


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

            // Form triangles where possible.
            // The last test is for the "internal" point.
            // Go clockwise...
            CheckPrimitiveTriangle(in000, v000, in444, v444, in660, v660, in442, mesh);             // 0
            CheckPrimitiveTriangle(in000, v000, in336, v336, in444, v444, in334, mesh);             // 1
            CheckPrimitiveTriangle(in000, v000, in00C, v00C, in336, v336, in226, mesh);             // 2
            CheckPrimitiveTriangle(in660, v660, in444, v444, in666, v666, in554, mesh);             // 3
            CheckPrimitiveQuad(in448, v448, in666, v666, in444, v444, in336, v336, in446, mesh);    // 4
            CheckPrimitiveTriangle(in00C, v00C, in448, v448, in336, v336, in338, mesh);             // 5
            CheckPrimitiveTriangle(in66C, v66C, in666, v666, in448, v448, in558, mesh);             // 6
            CheckPrimitiveTriangle(in00C, v00C, in66C, v66C, in448, v448, in44A, mesh);             // 7

            CheckPrimitiveTriangle(in660, v660, in884, v884, inCC0, vCC0, in882, mesh);             // 8
            CheckPrimitiveTriangle(in884, v884, in996, v996, inCC0, vCC0, in994, mesh);             // 9
            CheckPrimitiveTriangle(inCCC, vCCC, inCC0, vCC0, in996, v996, inAA6, mesh);             // 10
            CheckPrimitiveTriangle(in660, v660, in666, v666, in884, v884, in774, mesh);             // 11
            CheckPrimitiveQuad(in666, v666, in888, v888, in996, v996, in884, v884, in886, mesh);    // 12
            CheckPrimitiveTriangle(in888, v888, inCCC, vCCC, in996, v996, in998, mesh);             // 13
            CheckPrimitiveTriangle(in666, v666, in66C, v66C, in888, v888, in778, mesh);             // 14
            CheckPrimitiveTriangle(in66C, v66C, inCCC, vCCC, in888, v888, in88A, mesh);             // 15
        }
    }

    // Check the "corner" triangles of the cubic lattice.
    public void CheckCornerTriangle(Vector3Int v00C, Vector3Int v0C0, Vector3Int vC00, int mesh)
    {
        // Get unit vectors in the square to navigate it.
        Vector3Int vu00CtoC00 = new Vector3Int(((vC00.x - v00C.x) / 12), ((vC00.y - v00C.y) / 12), ((vC00.z - v00C.z) / 12));   // Unit vector from v00C to vC00 (x)
        Vector3Int vu00Cto0C0 = new Vector3Int(((v0C0.x - v00C.x) / 12), ((v0C0.y - v00C.y) / 12), ((v0C0.z - v00C.z) / 12));   // Unit vector from v00C to v0C0 (y)

        // Get the rest of the "points of interest".
        Vector3Int v444 = MixVectors3Int(v00C, vu00CtoC00, 4, vu00Cto0C0, 4);
        Vector3Int v066 = MixVectors3Int(v00C, vu00CtoC00, 0, vu00Cto0C0, 6);
        Vector3Int v363 = MixVectors3Int(v00C, vu00CtoC00, 3, vu00Cto0C0, 6);
        Vector3Int v660 = MixVectors3Int(v00C, vu00CtoC00, 6, vu00Cto0C0, 6);
        Vector3Int v336 = MixVectors3Int(v00C, vu00CtoC00, 3, vu00Cto0C0, 3);
        Vector3Int v633 = MixVectors3Int(v00C, vu00CtoC00, 6, vu00Cto0C0, 3);
        Vector3Int v606 = MixVectors3Int(v00C, vu00CtoC00, 6, vu00Cto0C0, 0);

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

        // Get the status of the points.
        int in00C = CanFormTriangle(v00C);
        int in0C0 = CanFormTriangle(v0C0);
        int inC00 = CanFormTriangle(vC00);
        int in444 = CanFormTriangle(v444);

        int in066 = CanFormTriangle(v066);
        int in363 = CanFormTriangle(v363);
        int in660 = CanFormTriangle(v660);
        int in336 = CanFormTriangle(v336);
        int in633 = CanFormTriangle(v633);
        int in606 = CanFormTriangle(v606);

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

        if (in00C == 0 && in0C0 == 0 && inC00 == 0 && in444 == 0 && in066 == 0 && in363 == 0 && in660 == 0 && in336 == 0 && in633 == 0 && in606 == 0
            && in417 == 0 && in147 == 0 && in174 == 0 && in471 == 0 && in741 == 0 && in714 == 0 && in435 == 0 && in345 == 0 && in354 == 0 && in453 == 0 && in543 == 0 && in534 == 0)
        {
            AddTriangleBoth(IntVectorToWorld(v00C), IntVectorToWorld(v0C0), IntVectorToWorld(vC00), mesh);
            nFullCorners++;
        }
        else
        {
            // Form triangles where possible.
            // The last test is for the "internal" point.
            // Go clockwise...
            CheckPrimitiveTriangle(in00C, v00C, in336, v336, in606, v606, in417, mesh);             // 0
            CheckPrimitiveTriangle(in00C, v00C, in066, v066, in336, v336, in147, mesh);             // 1
            CheckPrimitiveTriangle(in066, v066, in0C0, v0C0, in363, v363, in174, mesh);             // 2
            CheckPrimitiveTriangle(in0C0, v0C0, in660, v660, in363, v363, in471, mesh);             // 3
            CheckPrimitiveTriangle(in633, v633, in660, v660, inC00, vC00, in741, mesh);             // 4
            CheckPrimitiveTriangle(in606, v606, in633, v633, inC00, vC00, in714, mesh);             // 5

            CheckPrimitiveTriangle(in336, v336, in444, v444, in606, v606, in435, mesh);             // 6
            CheckPrimitiveTriangle(in066, v066, in444, v444, in336, v336, in345, mesh);             // 7
            CheckPrimitiveTriangle(in363, v363, in444, v444, in066, v066, in354, mesh);             // 8
            CheckPrimitiveTriangle(in660, v660, in444, v444, in363, v363, in453, mesh);             // 9
            CheckPrimitiveTriangle(in633, v633, in444, v444, in660, v660, in543, mesh);             // 10
            CheckPrimitiveTriangle(in606, v606, in444, v444, in633, v633, in534, mesh);             // 11
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



    //---------------


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
}
