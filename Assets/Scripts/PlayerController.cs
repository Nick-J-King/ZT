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
                        CheckDiagonalFace(v000i, v011i, v111i, v100i, sliderFullInt, 3); // Along x
                        CheckDiagonalFace(v010i, v001i, v101i, v110i, sliderFullInt, 4); // Along x

                        CheckDiagonalFace(v000i, v101i, v111i, v010i, sliderFullInt, 5); // Along y
                        CheckDiagonalFace(v001i, v100i, v110i, v011i, sliderFullInt, 6); // Along y

                        CheckDiagonalFace(v000i, v110i, v111i, v001i, sliderFullInt, 7); // Along z
                        CheckDiagonalFace(v010i, v100i, v101i, v011i, sliderFullInt, 8); // Along z


                        // Flat faces

                        CheckFlatFace(v000i, v010i, v011i, v001i, sliderFullInt, 0);    // Along x = 0
                        CheckFlatFace(v000i, v001i, v101i, v100i, sliderFullInt, 1);    // Along y = 0
                        CheckFlatFace(v000i, v100i, v110i, v010i, sliderFullInt, 2);    // Along z = 0


                        // Do cases of 3 "on" corners now!

                        // Corners
/*
                        CheckTriangle(nIsSet001, v001, nIsSet010, v010, nIsSet100, v100, 9);      // Around 000
                        CheckTriangle(nIsSet000, v000, nIsSet101, v101, nIsSet110, v110, 10);   // Around 100
                        CheckTriangle(nIsSet100, v100, nIsSet111, v111, nIsSet001, v001, 11);   // Around 101
                        CheckTriangle(nIsSet111, v111, nIsSet100, v100, nIsSet010, v010, 12);   // Around 110

                        CheckTriangle(nIsSet110, v110, nIsSet101, v101, nIsSet011, v011, 9);   // Around 111
                        CheckTriangle(nIsSet110, v110, nIsSet000, v000, nIsSet011, v011, 11);   // Around 010

                        CheckTriangle(nIsSet111, v111, nIsSet001, v001, nIsSet010, v010, 10);   // Around 011
                        CheckTriangle(nIsSet101, v101, nIsSet011, v011, nIsSet000, v000, 12);   // Around 001
*/

                        /*

                        // Do cases of 2 "on" corners... ("edges")

                        // Do cases of 1 "on" corners... ("vertices")
                        */

                        //else
                        {
                            if (nSet >= 13)
                            {
                                AddQuadBoth(v000, v010, v001, v011, 13);    // x-
                                AddQuadBoth(v000, v001, v100, v101, 13);    // y-
                                AddQuadBoth(v000, v100, v010, v110, 13);    // z-

                                AddQuadBoth(v100, v110, v101, v111, 13);    // x+
                                AddQuadBoth(v010, v011, v110, v111, 13);    // y+
                                AddQuadBoth(v001, v101, v011, v111, 13);    // z+
                            }
                        }
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
        if (in00 == 0 && in66 == 0 && inC0 == 0 && in63 == 0) AddTriangleBoth(IntVectorToWorld(v00), IntVectorToWorld(v66), IntVectorToWorld(vC0), mesh);
        if (in00 == 0 && in0C == 0 && in66 == 0 && in36 == 0) AddTriangleBoth(IntVectorToWorld(v00), IntVectorToWorld(v0C), IntVectorToWorld(v66), mesh);
        if (in0C == 0 && inCC == 0 && in66 == 0 && in69 == 0) AddTriangleBoth(IntVectorToWorld(v0C), IntVectorToWorld(vCC), IntVectorToWorld(v66), mesh);
        if (inCC == 0 && inC0 == 0 && in66 == 0 && in96 == 0) AddTriangleBoth(IntVectorToWorld(vCC), IntVectorToWorld(vC0), IntVectorToWorld(v66), mesh);
    }


    // Check the "diagonal" rectangles of the cubic lattice.
    /// <summary>
    /// Checks the diagonal face.
    /// >>> Still need to split finer.
    /// >>> Then want to "merge" if possible / needed...
    /// </summary>
    /// <param name="v000">V000.</param>
    /// <param name="v00C">V00 c.</param>
    /// <param name="vCCC">V ccc.</param>
    /// <param name="vCC0">V CC.</param>
    /// <param name="edge4">Edge4.</param>
    /// <param name="mesh">Mesh.</param>
    public void CheckDiagonalFace(Vector3Int v000, Vector3Int v00C, Vector3Int vCCC, Vector3Int vCC0, int edge4, int mesh)
    {
        // Get unit vectors in the square to navigate it.
        Vector3Int vu000toCC0 = new Vector3Int(((vCC0.x - v000.x) / 12), ((vCC0.y - v000.y) / 12), ((vCC0.z - v000.z) / 12));   // Unit vector from v000 to vCC0
        Vector3Int vu000to00C = new Vector3Int(((v00C.x - v000.x) / 12), ((v00C.y - v000.y) / 12), ((v00C.z - v000.z) / 12));   // Unit vector from v000 to v00C

        // Get the rest of the "points of interest".
        Vector3Int v666 = MixVectors3Int(v000, vu000toCC0, 6, vu000to00C, 6);

        Vector3Int v660 = MixVectors3Int(v000, vu000toCC0, 6, vu000to00C, 0);
        Vector3Int v66C = MixVectors3Int(v000, vu000toCC0, 6, vu000to00C, 12);

        // "Middle" points.
        Vector3Int v336 = MixVectors3Int(v000, vu000toCC0, 3, vu000to00C, 6);
        Vector3Int v996 = MixVectors3Int(v000, vu000toCC0, 9, vu000to00C, 6);

        Vector3Int v442 = MixVectors3Int(v000, vu000toCC0, 4, vu000to00C, 2);
        Vector3Int v44A = MixVectors3Int(v000, vu000toCC0, 4, vu000to00C, 10);
        Vector3Int v882 = MixVectors3Int(v000, vu000toCC0, 8, vu000to00C, 2);
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
        int in442 = CanFormTriangle4(v442, edge4);
        int in44A = CanFormTriangle4(v44A, edge4);
        int in882 = CanFormTriangle4(v882, edge4);
        int in88A = CanFormTriangle4(v88A, edge4);

        // Form triangles where possible.
        // The last test is for the "internal" point.
        // Go clockwise...
        if (in000 == 0 && in666 == 0 && in660 == 0 && in442 == 0) AddTriangleBoth(IntVectorToWorld(v000), IntVectorToWorld(v666), IntVectorToWorld(v660), mesh);
        if (in000 == 0 && in00C == 0 && in666 == 0 && in336 == 0) AddTriangleBoth(IntVectorToWorld(v000), IntVectorToWorld(v00C), IntVectorToWorld(v666), mesh);
        if (in00C == 0 && in66C == 0 && in666 == 0 && in44A == 0) AddTriangleBoth(IntVectorToWorld(v00C), IntVectorToWorld(v66C), IntVectorToWorld(v666), mesh);
        if (in66C == 0 && inCCC == 0 && in666 == 0 && in88A == 0) AddTriangleBoth(IntVectorToWorld(v66C), IntVectorToWorld(vCCC), IntVectorToWorld(v666), mesh);
        if (inCCC == 0 && inCC0 == 0 && in666 == 0 && in996 == 0) AddTriangleBoth(IntVectorToWorld(vCCC), IntVectorToWorld(vCC0), IntVectorToWorld(v666), mesh);
        if (in660 == 0 && in666 == 0 && inCC0 == 0 && in882 == 0) AddTriangleBoth(IntVectorToWorld(v660), IntVectorToWorld(v666), IntVectorToWorld(vCC0), mesh);
    }


    // Check the "corner" triangles of the cubic lattice.
    public void CheckCornerTriangle()
    {

    }



    public void CheckQuads(int in00, Vector3 v00, int in01, Vector3 v01, int in10, Vector3 v10, int in11, Vector3 v11, int mesh)
    {
        if (in00 == 0 && in01 == 0 && in10 == 0 && in11 == 0)
        {
            AddQuadBoth(v00, v01, v10, v11, mesh);
        }
        else
        {
            CheckTriangle(in01, v01, in10, v10, in11, v11, mesh);
            CheckTriangle(in00, v00, in10, v10, in11, v11, mesh);
            CheckTriangle(in00, v00, in01, v01, in11, v11, mesh);
            CheckTriangle(in00, v00, in01, v01, in10, v10, mesh);
        }
    }

    public void CheckTriangle(int in00, Vector3 v00, int in01, Vector3 v01, int in10, Vector3 v10, int mesh)
    {
        if (in00 == 0 && in01 == 0 && in10 == 0)
        {
            AddTriangleBoth(v00, v01, v10, mesh);
        }
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
        //if (v.z  >= 24)
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

        int c1 = CanFormTriangleInt(s2, s3, s4);
        int c2 = CanFormTriangleInt(s1, s3, s4);
        int c3 = CanFormTriangleInt(s1, s2, s4);
        int c4 = CanFormTriangleInt(s1, s2, s3);

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


    public int CanFormTriangleInt(int s1, int s2, int s3)
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
