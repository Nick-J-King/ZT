using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Player controller
public class PlayerController : MonoBehaviour
{

    public Slider sliderDivisions;
    public Text textDivisions;

    public Slider slider4thEdge;
    public Text text4thEdge;

    public Slider sliderLightAzimuth;
    public Slider sliderLightElevation;

    public Toggle togglePoints;
    public Slider sliderVertexSize;

    public Light directionalLight;

    public Toggle toggleAnimate;

    public int [] myNumVerts;
    public int [] myNumTriangles;
    public List<Vector3> [] myVerts;
    public List<int> [] myTriangles;

    private float size;
    private float sizeOnTwo;
    private float scale;

    private int nDivisions;     // "Main" divisions.
    private int nFullDivisions; // nDivisions * 8 for finers sub-divisions.
    private float max;
    private float fullMax;

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

    public int MAXTVERTS = 65530;


    public Toggle toggleX;
    public Toggle toggleY;
    public Toggle toggleZ;
    public Toggle toggleX1;
    public Toggle toggleX2;
    public Toggle toggleY1;
    public Toggle toggleY2;
    public Toggle toggleZ1;
    public Toggle toggleZ2;
    public Toggle toggle000;
    public Toggle toggle001;
    public Toggle toggle010;
    public Toggle toggle011;
    public Toggle toggle100;
    public Toggle toggle101;
    public Toggle toggle110;
    public Toggle toggle111;


    private GameObject s;
    private ArrayList myList;


    void Start()
    {
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

        myNumVerts = new int[14];
        myNumTriangles = new int[14];
        myVerts = new List<Vector3>[14];
        myTriangles = new List<int>[14];

        myList = new ArrayList();


        nDivisions = 50;                    // <<<
        nFullDivisions = nDivisions * 12;    // <<<


        size = 10.0f;               // Size of the "configuration cube".
        sizeOnTwo = size / 2.0f;    // Used to center the cube.


        CheckSliders();
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
            mfMain.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * 30.0f);
        }
    }


    private float intToFloat(int coord)
    {
        return (float)coord / fullMax;
    }


    // Convert float coord (0 - 1) into world coordinate.
    private float cubeToWorld(float coord)
    {
        return coord * size - sizeOnTwo;
    }


    public void CheckLight()
    {
        float y = 360.0f - sliderLightAzimuth.value;
        float x = sliderLightElevation.value;
        float z = 0.0f;

        directionalLight.transform.localRotation = Quaternion.Euler(x, y, z);
        directionalLight.transform.localPosition = Vector3.zero;
    }


    public void ResetMesh(int mesh)
    {
        myNumVerts[mesh] = 0;
        myNumTriangles[mesh] = 0;
        myVerts[mesh] = new List<Vector3>();
        myTriangles[mesh] = new List<int>();
        mfSub[mesh].mesh.Clear();
    }

    public void CheckSurfaceToggles()
    {
        CheckSliders();
    }

    public void CheckSlidersTest()
    {

        CheckLight();

        foreach (GameObject s in myList)
        {
            Destroy(s);
        }

        nDivisions = (int)sliderDivisions.value;
        nFullDivisions = nDivisions * 12;

        textDivisions.text = "Max edge: " + nDivisions.ToString() + " (" + nFullDivisions.ToString() + ")";


        float sliderFloat = slider4thEdge.value;
        int sliderInt = (int)(sliderFloat * (nDivisions + 1));
        if (sliderInt > nDivisions) sliderInt = nDivisions;
        int sliderFullInt = sliderInt * 12;

        text4thEdge.text = "4th edge: " + sliderInt.ToString() + " (" + sliderFullInt.ToString() + ")";    // + " " + sliderFloat.ToString();


        float vertexSize = sliderVertexSize.value;


        max = (float)nDivisions;
        fullMax = (float)nFullDivisions;

        scale = size / max * vertexSize + 0.06f;


        float x0 = cubeToWorld(intToFloat(0));
        float x1 = cubeToWorld(intToFloat(nFullDivisions));

        float y0 = cubeToWorld(intToFloat(0));
        float y1 = cubeToWorld(intToFloat(nFullDivisions));

        float z0 = cubeToWorld(intToFloat(0));
        float z1 = cubeToWorld(intToFloat(nFullDivisions));


        Vector3 v000 = new Vector3(x0, y0, z0);
        Vector3 v001 = new Vector3(x0, y0, z1);
        Vector3 v010 = new Vector3(x0, y1, z0);
        Vector3 v011 = new Vector3(x0, y1, z1);
        Vector3 v100 = new Vector3(x1, y0, z0);
        Vector3 v101 = new Vector3(x1, y0, z1);
        Vector3 v110 = new Vector3(x1, y1, z0);
        Vector3 v111 = new Vector3(x1, y1, z1);



        if (togglePoints.isOn)
        {
            for (int x = 0; x <= nDivisions; x++)
            {
                float xx = cubeToWorld(intToFloat(x * 12));

                for (int y = 0; y <= nDivisions; y++)
                {
                    float yy = cubeToWorld(intToFloat(y * 12));

                    for (int z = 0; z <= nDivisions; z++)
                    {
                        float zz = cubeToWorld(intToFloat(z * 12));

                        s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        s.transform.parent = mfMain.transform;

                        s.transform.localPosition = new Vector3(xx, yy, zz);
                        s.transform.localScale = new Vector3(scale, scale, scale);

                        myList.Add(s);


                    }
                }
            }
        }

        for (int i = 0; i < 14; i++)
        {
            ResetMesh(i);
        }

        if (toggleX.isOn) AddQuadBoth(v000, v001, v010, v011, 1);
        if (toggleY.isOn) AddQuadBoth(v000, v001, v100, v101, 1);
        if (toggleZ.isOn) AddQuadBoth(v000, v010, v100, v110, 1);

        if (toggleX1.isOn) AddQuadBoth(v000, v011, v100, v111, 1);
        if (toggleX2.isOn) AddQuadBoth(v010, v001, v110, v101, 1);

        if (toggleY1.isOn) AddQuadBoth(v000, v101, v010, v111, 1);
        if (toggleY2.isOn) AddQuadBoth(v100, v001, v110, v011, 1);

        if (toggleZ1.isOn) AddQuadBoth(v000, v110, v001, v111, 1);
        if (toggleZ2.isOn) AddQuadBoth(v100, v010, v101, v011, 1);

        if (toggle000.isOn) AddTriangleBoth(v001, v010, v100, 4);
        if (toggle100.isOn) AddTriangleBoth(v000, v101, v110, 4);
        if (toggle101.isOn) AddTriangleBoth(v100, v111, v001, 4);
        if (toggle110.isOn) AddTriangleBoth(v111, v100, v010, 4);

        if (toggle111.isOn) AddTriangleBoth(v110, v101, v011, 4);
        if (toggle010.isOn) AddTriangleBoth(v110, v000, v011, 4);
        if (toggle011.isOn) AddTriangleBoth(v111, v001, v010, 4);
        if (toggle001.isOn) AddTriangleBoth(v101, v011, v000, 4);


        for (int i = 0; i < 14; i++)
        {
            ProcessMesh(i);
        }
    }

    public void CheckSliders()
    {
        CheckLight();

        foreach (GameObject s in myList)
        {
            Destroy(s);
        }

        nDivisions = (int)sliderDivisions.value;
        nFullDivisions = nDivisions * 12;

        textDivisions.text = "Max edge: " + nDivisions.ToString() + " (" + nFullDivisions.ToString() + ")";


        float sliderFloat = slider4thEdge.value;
        int sliderInt = (int)(sliderFloat * (nDivisions + 1));
        if (sliderInt > nDivisions) sliderInt = nDivisions;
        int sliderFullInt = sliderInt * 12;

        text4thEdge.text = "4th edge: " + sliderInt.ToString() + " (" + sliderFullInt.ToString() + ")";    // + " " + sliderFloat.ToString();


        float vertexSize = sliderVertexSize.value;


        max = (float)nDivisions;
        fullMax = (float)nFullDivisions;

        scale = size / max * vertexSize + 0.06f;

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

        for (int intX = 0; intX <= nDivisions; intX++)
        {
            int intXfull = intX * 12;

            x0 = cubeToWorld(intToFloat(intXfull));
            x8 = cubeToWorld(intToFloat(intXfull + 12));

            for (int intY = 0; intY <= nDivisions; intY++)
            {
                int intYfull = intY * 12;

                y0 = cubeToWorld(intToFloat(intYfull));
                y8 = cubeToWorld(intToFloat(intYfull + 12));

                for (int intZ = 0; intZ <= nDivisions; intZ++)
                {
                    int intZfull = intZ * 12;

                    z0 = cubeToWorld(intToFloat(intZfull));
                    z8 = cubeToWorld(intToFloat(intZfull + 12));


                    //if (x > y && y > z)
                    {
                        int nIsSet000 = CanFormTriangle4Int(intXfull, intYfull, intZfull, sliderFullInt);
                        int nIsSet100 = CanFormTriangle4Int(intXfull + 12, intYfull, intZfull, sliderFullInt);
                        int nIsSet010 = CanFormTriangle4Int(intXfull, intYfull + 12, intZfull, sliderFullInt);
                        int nIsSet110 = CanFormTriangle4Int(intXfull + 12, intYfull + 12, intZfull, sliderFullInt);
                        int nIsSet001 = CanFormTriangle4Int(intXfull, intYfull, intZfull + 12, sliderFullInt);
                        int nIsSet101 = CanFormTriangle4Int(intXfull + 12, intYfull, intZfull + 12, sliderFullInt);
                        int nIsSet011 = CanFormTriangle4Int(intXfull, intYfull + 12, intZfull + 12, sliderFullInt);
                        int nIsSet111 = CanFormTriangle4Int(intXfull + 12, intYfull + 12, intZfull + 12, sliderFullInt);

                        Vector3 v000 = new Vector3(x0, y0, z0);
                        Vector3 v100 = new Vector3(x8, y0, z0);
                        Vector3 v010 = new Vector3(x0, y8, z0);
                        Vector3 v110 = new Vector3(x8, y8, z0);
                        Vector3 v001 = new Vector3(x0, y0, z8);
                        Vector3 v101 = new Vector3(x8, y0, z8);
                        Vector3 v011 = new Vector3(x0, y8, z8);
                        Vector3 v111 = new Vector3(x8, y8, z8);

                        Vector3Int v000i = new Vector3Int(intXfull, intYfull, intZfull);
                        Vector3Int v100i = new Vector3Int(intXfull + 12, intYfull, intZfull);
                        Vector3Int v010i = new Vector3Int(intXfull, intYfull + 12, intZfull);
                        Vector3Int v110i = new Vector3Int(intXfull + 12, intYfull + 12, intZfull);
                        Vector3Int v001i = new Vector3Int(intXfull, intYfull, intZfull + 12);
                        Vector3Int v101i = new Vector3Int(intXfull + 12, intYfull, intZfull + 12);
                        Vector3Int v011i = new Vector3Int(intXfull, intYfull + 12, intZfull + 12);
                        Vector3Int v111i = new Vector3Int(intXfull + 12, intYfull + 12, intZfull + 12);
                        // f
                        //
                        //
                        int nSet = 0;
                        if (nIsSet000 == 0) nSet++;
                        if (nIsSet001 == 0) nSet++;
                        if (nIsSet010 == 0) nSet++;
                        if (nIsSet011 == 0) nSet++;
                        if (nIsSet100 == 0) nSet++;
                        if (nIsSet101 == 0) nSet++;
                        if (nIsSet110 == 0) nSet++;
                        if (nIsSet111 == 0) nSet++;

                        if (nIsSet000 == 0 && togglePoints.isOn)
                        {
                            s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            s.transform.parent = mfMain.transform;

                            s.transform.localPosition = v000;
                            s.transform.localScale = new Vector3(scale, scale, scale);

                            myList.Add(s);
                        }


                        if (nSet > 4)
                        {
                            // Handle "hard cases"...
                            // NOTE: Don't confuse the "clean" cases below with extraneous points on...
                        }

                        // Do cases of 4 "on" corners now!

                        // Diagonals across faces
                        CheckDiagonalFace(v000i, v100i, v111i, v011i, sliderFullInt, 3); // Along x
                        CheckDiagonalFace(v010i, v110i, v101i, v001i, sliderFullInt, 4); // Along x

                        CheckDiagonalFace(v000i, v010i, v111i, v101i, sliderFullInt, 5); // Along y
                        CheckDiagonalFace(v001i, v011i, v110i, v100i, sliderFullInt, 6); // Along y

                        CheckDiagonalFace(v000i, v001i, v111i, v110i, sliderFullInt, 7); // Along z
                        CheckDiagonalFace(v010i, v011i, v101i, v100i, sliderFullInt, 8); // Along z


                        // Flat faces

                        CheckFlatFace(v000i, v010i, v011i, v001i, sliderFullInt, 0);    // Along x = 0
                        CheckFlatFace(v000i, v001i, v101i, v100i, sliderFullInt, 1);    // Along y = 0
                        CheckFlatFace(v000i, v100i, v110i, v010i, sliderFullInt, 2);    // Along z = 0


                        // Do cases of 3 "on" corners now!

                        // Corners
                        CheckCornerTriangle(v100i, v010i, v001i, sliderFullInt, 9);  // Around 000
                        CheckCornerTriangle(v000i, v101i, v110i, sliderFullInt, 10);  // Around 100

                        CheckCornerTriangle(v100i, v111i, v001i, sliderFullInt, 11);  // Around 101
                        CheckCornerTriangle(v111i, v100i, v010i, sliderFullInt, 12);  // Around 110

                        CheckCornerTriangle(v110i, v101i, v011i, sliderFullInt, 9);  // Around 111
                        CheckCornerTriangle(v110i, v000i, v011i, sliderFullInt, 11);  // Around 010

                        CheckCornerTriangle(v111i, v001i, v010i, sliderFullInt, 10);  // Around 011
                        CheckCornerTriangle(v101i, v011i, v000i, sliderFullInt, 12);  // Around 001

                        /*

                        // Do cases of 2 "on" corners... ("edges")

                        // Do cases of 1 "on" corners... ("vertices")
                        */

                    }
                }
            }
        }

        //
        for (int i = 0; i < 14; i++)
        {
            ProcessMesh(i);
        }
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

    public Vector3Int MixVectors3Int(Vector3Int baseVector, Vector3Int dir1, int mag1, Vector3Int dir2, int mag2)
    {

        Vector3Int result = new Vector3Int(baseVector.x + dir1.x * mag1 + dir2.x * mag2, baseVector.y + dir1.y * mag1 + dir2.y * mag2, baseVector.z + dir1.z * mag1 + dir2.z * mag2);

        return result;
    }

    public Vector3 IntVectorToWorld(Vector3Int intVector)
    {
        Vector3 result = new Vector3(cubeToWorld(intToFloat(intVector.x)), cubeToWorld(intToFloat(intVector.y)), cubeToWorld(intToFloat(intVector.z)));

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
    public void CheckFlatFace(Vector3Int v00, Vector3Int v0C, Vector3Int vCC, Vector3Int vC0, int edge4, int mesh)
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
        int in00 = CanFormTriangle4(v00, edge4);
        int inC0 = CanFormTriangle4(vC0, edge4);
        int in0C = CanFormTriangle4(v0C, edge4);
        int inCC = CanFormTriangle4(vCC, edge4);
        int in66 = CanFormTriangle4(v66, edge4);

        int in36 = CanFormTriangle4(v36, edge4);
        int in69 = CanFormTriangle4(v69, edge4);
        int in96 = CanFormTriangle4(v96, edge4);
        int in63 = CanFormTriangle4(v63, edge4);

        // Form triangles where possible.
        // The last test is for the "internal" point.
        // Go clockwise...
        CheckPrimitiveTriangle(in00, v00, in66, v66, inC0, vC0, in63, mesh);
        CheckPrimitiveTriangle(in00, v00, in0C, v0C, in66, v66, in36, mesh);
        CheckPrimitiveTriangle(in0C, v0C, inCC, vCC, in66, v66, in69, mesh);
        CheckPrimitiveTriangle(inCC, vCC, inC0, vC0, in66, v66, in96, mesh);
    }


    // Check the "diagonal" rectangles of the cubic lattice.
    public void CheckDiagonalFace(Vector3Int v000, Vector3Int v00C, Vector3Int vCCC, Vector3Int vCC0, int edge4, int mesh)
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
        int in000 = CanFormTriangle4(v000, edge4);
        int in660 = CanFormTriangle4(v660, edge4);
        int inCC0 = CanFormTriangle4(vCC0, edge4);
        int in666 = CanFormTriangle4(v666, edge4);
        int in00C = CanFormTriangle4(v00C, edge4);
        int in66C = CanFormTriangle4(v66C, edge4);
        int inCCC = CanFormTriangle4(vCCC, edge4);

        int in336 = CanFormTriangle4(v336, edge4);
        int in996 = CanFormTriangle4(v996, edge4);
        int in444 = CanFormTriangle4(v444, edge4);
        int in448 = CanFormTriangle4(v448, edge4);
        int in884 = CanFormTriangle4(v884, edge4);
        int in888 = CanFormTriangle4(v888, edge4);


        int in442 = CanFormTriangle4(v442, edge4);
        int in334 = CanFormTriangle4(v334, edge4);
        int in226 = CanFormTriangle4(v226, edge4);
        int in554 = CanFormTriangle4(v554, edge4);
        int in446 = CanFormTriangle4(v446, edge4);
        int in338 = CanFormTriangle4(v338, edge4);
        int in558 = CanFormTriangle4(v558, edge4);
        int in44A = CanFormTriangle4(v44A, edge4);

        int in882 = CanFormTriangle4(v882, edge4);
        int in994 = CanFormTriangle4(v994, edge4);
        int inAA6 = CanFormTriangle4(vAA6, edge4);
        int in774 = CanFormTriangle4(v774, edge4);
        int in886 = CanFormTriangle4(v886, edge4);
        int in998 = CanFormTriangle4(v998, edge4);
        int in778 = CanFormTriangle4(v778, edge4);
        int in88A = CanFormTriangle4(v88A, edge4);

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


    // Check the "corner" triangles of the cubic lattice.
    public void CheckCornerTriangle(Vector3Int v00C, Vector3Int v0C0, Vector3Int vC00, int edge4, int mesh)
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
        int in00C = CanFormTriangle4(v00C, edge4);
        int in0C0 = CanFormTriangle4(v0C0, edge4);
        int inC00 = CanFormTriangle4(vC00, edge4);
        int in444 = CanFormTriangle4(v444, edge4);
        int in066 = CanFormTriangle4(v066, edge4);
        int in363 = CanFormTriangle4(v363, edge4);
        int in660 = CanFormTriangle4(v660, edge4);
        int in336 = CanFormTriangle4(v336, edge4);
        int in633 = CanFormTriangle4(v633, edge4);
        int in606 = CanFormTriangle4(v606, edge4);

        int in417 = CanFormTriangle4(v417, edge4);
        int in147 = CanFormTriangle4(v147, edge4);
        int in174 = CanFormTriangle4(v174, edge4);
        int in471 = CanFormTriangle4(v471, edge4);
        int in741 = CanFormTriangle4(v741, edge4);
        int in714 = CanFormTriangle4(v714, edge4);
        int in435 = CanFormTriangle4(v435, edge4);
        int in345 = CanFormTriangle4(v345, edge4);
        int in354 = CanFormTriangle4(v354, edge4);
        int in453 = CanFormTriangle4(v453, edge4);
        int in543 = CanFormTriangle4(v543, edge4);
        int in534 = CanFormTriangle4(v534, edge4);

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


    public void AddQuadBoth(Vector3 v00, Vector3 v01, Vector3 v10, Vector3 v11, int mesh)
    {
        ///if (myNumVerts > MAXTRIANGLES) return;

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
        //if (numVerts > MAXTRIANGLES) return;

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



    public int CanFormTriangle4(Vector3Int v, int e)
    {
        //if (v.y  == v.z)
        //    return 0;
        //return -1;

        return CanFormTriangle4Int(v.x, v.y, v.z, e);
    }


    public int CanFormTriangle4Int(int s1, int s2, int s3, int s4)
    {
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
