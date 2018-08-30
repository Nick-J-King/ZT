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


    private int myNumVerts0;
    private int myNumTriangles0;
    private List<Vector3> myVerts0;
    private List<int> myTriangles0;

    private int myNumVerts1;
    private int myNumTriangles1;
    private List<Vector3> myVerts1;
    private List<int> myTriangles1;

    private int myNumVerts2;
    private int myNumTriangles2;
    private List<Vector3> myVerts2;
    private List<int> myTriangles2;

    private int myNumVerts3;
    private int myNumTriangles3;
    private List<Vector3> myVerts3;
    private List<int> myTriangles3;

    private int myNumVerts4;
    private int myNumTriangles4;
    private List<Vector3> myVerts4;
    private List<int> myTriangles4;

    private int myNumVerts5;
    private int myNumTriangles5;
    private List<Vector3> myVerts5;
    private List<int> myTriangles5;

    private int myNumVerts6;
    private int myNumTriangles6;
    private List<Vector3> myVerts6;
    private List<int> myTriangles6;

    private int myNumVerts7;
    private int myNumTriangles7;
    private List<Vector3> myVerts7;
    private List<int> myTriangles7;

    private int myNumVerts8;
    private int myNumTriangles8;
    private List<Vector3> myVerts8;
    private List<int> myTriangles8;

    private int myNumVerts9;
    private int myNumTriangles9;
    private List<Vector3> myVerts9;
    private List<int> myTriangles9;

    private int myNumVerts10;
    private int myNumTriangles10;
    private List<Vector3> myVerts10;
    private List<int> myTriangles10;

    private int myNumVerts11;
    private int myNumTriangles11;
    private List<Vector3> myVerts11;
    private List<int> myTriangles11;

    private int myNumVerts12;
    private int myNumTriangles12;
    private List<Vector3> myVerts12;
    private List<int> myTriangles12;

    private int myNumVerts13;
    private int myNumTriangles13;
    private List<Vector3> myVerts13;
    private List<int> myTriangles13;

    private float size;
    private float sizeOnTwo;
    private float scale;

    private int nDivisions;     // "Main" divisions.
    private int nFullDivisions; // nDivisions * 8 for finers sub-divisions.
    private float max;
    private float fullMax;

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

    public int MAXTVERTS = 65530;

    private GameObject s;
    private ArrayList myList;


    void Start()
    {
        myList = new ArrayList();

        nDivisions = 50;                    // <<<
        nFullDivisions = nDivisions * 8;    // <<<


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


    public void ResetMesh(ref int numVerts, ref int numTriangles, ref List<Vector3> verts, ref List<int> triangles, ref MeshFilter mf)
    {
        numVerts = 0;
        numTriangles = 0;
        verts = new List<Vector3>();
        triangles = new List<int>();
        mf.mesh.Clear();
    }


    public void CheckSliders()
    {
        CheckLight();

        foreach (GameObject s in myList)
        {
            Destroy(s);
        }

        nDivisions = (int)sliderDivisions.value;
        nFullDivisions = nDivisions * 8;

        textDivisions.text = "Max edge: " + nDivisions.ToString() + " (" + nFullDivisions.ToString() + ")";


        float sliderFloat = slider4thEdge.value;
        int sliderInt = (int)(sliderFloat * (nDivisions + 1));
        if (sliderInt > nDivisions) sliderInt = nDivisions;
        int sliderFullInt = sliderInt * 8;

        text4thEdge.text = "4th edge: " + sliderInt.ToString() + " (" + sliderFullInt.ToString() + ")";    // + " " + sliderFloat.ToString();


        float vertexSize = sliderVertexSize.value;


        max = (float)nDivisions;
        fullMax = (float)nFullDivisions;

        scale = size / max * vertexSize + 0.06f;


        ResetMesh(ref myNumVerts0, ref myNumTriangles0, ref myVerts0, ref myTriangles0, ref mfMain0);
        ResetMesh(ref myNumVerts1, ref myNumTriangles1, ref myVerts1, ref myTriangles1, ref mfMain1);
        ResetMesh(ref myNumVerts2, ref myNumTriangles2, ref myVerts2, ref myTriangles2, ref mfMain2);
        ResetMesh(ref myNumVerts3, ref myNumTriangles3, ref myVerts3, ref myTriangles3, ref mfMain3);
        ResetMesh(ref myNumVerts4, ref myNumTriangles4, ref myVerts4, ref myTriangles4, ref mfMain4);
        ResetMesh(ref myNumVerts5, ref myNumTriangles5, ref myVerts5, ref myTriangles5, ref mfMain5);
        ResetMesh(ref myNumVerts6, ref myNumTriangles6, ref myVerts6, ref myTriangles6, ref mfMain6);
        ResetMesh(ref myNumVerts7, ref myNumTriangles7, ref myVerts7, ref myTriangles7, ref mfMain7);
        ResetMesh(ref myNumVerts8, ref myNumTriangles8, ref myVerts8, ref myTriangles8, ref mfMain8);
        ResetMesh(ref myNumVerts9, ref myNumTriangles9, ref myVerts9, ref myTriangles9, ref mfMain9);
        ResetMesh(ref myNumVerts10, ref myNumTriangles10, ref myVerts10, ref myTriangles10, ref mfMain10);
        ResetMesh(ref myNumVerts11, ref myNumTriangles11, ref myVerts11, ref myTriangles11, ref mfMain11);
        ResetMesh(ref myNumVerts12, ref myNumTriangles12, ref myVerts12, ref myTriangles12, ref mfMain12);
        ResetMesh(ref myNumVerts13, ref myNumTriangles13, ref myVerts13, ref myTriangles13, ref mfMain13);


        float x0;
        float y0;
        float z0;

        float x8;
        float y8;
        float z8;

        for (int intX = 0; intX <= nDivisions; intX++)
        {
            int intXfull = intX * 8;

            x0 = cubeToWorld(intToFloat(intXfull));
            x8 = cubeToWorld(intToFloat(intXfull + 8));

            for (int intY = 0; intY <= nDivisions; intY++)
            {
                int intYfull = intY * 8;

                y0 = cubeToWorld(intToFloat(intYfull));
                y8 = cubeToWorld(intToFloat(intYfull + 8));

                for (int intZ = 0; intZ <= nDivisions; intZ++)
                {
                    int intZfull = intZ * 8;

                    z0 = cubeToWorld(intToFloat(intZfull));
                    z8 = cubeToWorld(intToFloat(intZfull + 8));


                    //if (x > y && y > z)
                    {
                        int nIsSet000 = CanFormTriangle4Int(intXfull, intYfull, intZfull, sliderFullInt);
                        int nIsSet100 = CanFormTriangle4Int(intXfull + 8, intYfull, intZfull, sliderFullInt);
                        int nIsSet010 = CanFormTriangle4Int(intXfull, intYfull + 8, intZfull, sliderFullInt);
                        int nIsSet110 = CanFormTriangle4Int(intXfull + 8, intYfull + 8, intZfull, sliderFullInt);
                        int nIsSet001 = CanFormTriangle4Int(intXfull, intYfull, intZfull + 8, sliderFullInt);
                        int nIsSet101 = CanFormTriangle4Int(intXfull + 8, intYfull, intZfull + 8, sliderFullInt);
                        int nIsSet011 = CanFormTriangle4Int(intXfull, intYfull + 8, intZfull + 8, sliderFullInt);
                        int nIsSet111 = CanFormTriangle4Int(intXfull + 8, intYfull + 8, intZfull + 8, sliderFullInt);

                        Vector3 v000 = new Vector3(x0, y0, z0);
                        Vector3 v100 = new Vector3(x8, y0, z0);
                        Vector3 v010 = new Vector3(x0, y8, z0);
                        Vector3 v110 = new Vector3(x8, y8, z0);
                        Vector3 v001 = new Vector3(x0, y0, z8);
                        Vector3 v101 = new Vector3(x8, y0, z8);
                        Vector3 v011 = new Vector3(x0, y8, z8);
                        Vector3 v111 = new Vector3(x8, y8, z8);
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
                        //if (false)
                        {
                            s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            s.transform.parent = mfMain.transform;

                            s.transform.localPosition = v000;
                            s.transform.localScale = new Vector3(scale, scale, scale);


                            myList.Add(s);

                            //AddQuadBoth(v000, v010, v001, v011, ref myNumVerts13, ref myVerts13, ref myTriangles13);    // x-
                            //AddQuadBoth(v000, v001, v100, v101, ref myNumVerts13, ref myVerts13, ref myTriangles13);    // y-
                            //AddQuadBoth(v000, v100, v010, v110, ref myNumVerts13, ref myVerts13, ref myTriangles13);    // z-

                            //AddQuadBoth(v100, v110, v101, v111, ref myNumVerts13, ref myVerts13, ref myTriangles13);    // x+
                            //AddQuadBoth(v010, v011, v110, v111, ref myNumVerts13, ref myVerts13, ref myTriangles13);    // y+
                            //AddQuadBoth(v001, v101, v011, v111, ref myNumVerts13, ref myVerts13, ref myTriangles13);    // z+

                        }


                        if (nSet > 4)
                        {
                            // Handle "hard cases"...
                            // NOTE: Don't confuse the "clean" cases below with extraneous points on...
                        }

                        // Do cases of 4 "on" corners now!

                        // Diagonals across faces

                        CheckQuads(nIsSet000, v000, nIsSet011, v011, nIsSet100, v100, nIsSet111, v111, ref myNumVerts3, ref myVerts3, ref myTriangles3);    // Along x
                        CheckQuads(nIsSet010, v010, nIsSet001, v001, nIsSet110, v110, nIsSet101, v101, ref myNumVerts4, ref myVerts4, ref myTriangles4);    // Along x

                        CheckQuads(nIsSet000, v000, nIsSet101, v101, nIsSet010, v010, nIsSet111, v111, ref myNumVerts5, ref myVerts5, ref myTriangles5);    // Along y
                        CheckQuads(nIsSet100, v100, nIsSet001, v001, nIsSet110, v110, nIsSet011, v011, ref myNumVerts6, ref myVerts6, ref myTriangles6);    // Along y

                        CheckQuads(nIsSet000, v000, nIsSet110, v110, nIsSet001, v001, nIsSet111, v111, ref myNumVerts7, ref myVerts7, ref myTriangles7);    // Along z
                        CheckQuads(nIsSet100, v100, nIsSet010, v010, nIsSet101, v101, nIsSet011, v011, ref myNumVerts8, ref myVerts8, ref myTriangles8);    // Along z


                        // Flat faces

                        CheckQuads(nIsSet000, v000, nIsSet001, v001, nIsSet010, v010, nIsSet011, v011, ref myNumVerts0, ref myVerts0, ref myTriangles0);    // Along x = 0
                        CheckQuads(nIsSet000, v000, nIsSet001, v001, nIsSet100, v100, nIsSet101, v101, ref myNumVerts1, ref myVerts1, ref myTriangles1);    // Along y = 0
                        CheckQuads(nIsSet000, v000, nIsSet010, v010, nIsSet100, v100, nIsSet110, v110, ref myNumVerts2, ref myVerts2, ref myTriangles2);    // Along z = 0


                        // Do cases of 3 "on" corners now!

                        // Corners

                        CheckTriangle(nIsSet001, v001, nIsSet010, v010, nIsSet100, v100, ref myNumVerts9, ref myVerts9, ref myTriangles9);      // Around 000
                        CheckTriangle(nIsSet000, v000, nIsSet101, v101, nIsSet110, v110, ref myNumVerts10, ref myVerts10, ref myTriangles10);   // Around 100
                        CheckTriangle(nIsSet100, v100, nIsSet111, v111, nIsSet001, v001, ref myNumVerts11, ref myVerts11, ref myTriangles11);   // Around 101
                        CheckTriangle(nIsSet111, v111, nIsSet100, v100, nIsSet010, v010, ref myNumVerts12, ref myVerts12, ref myTriangles12);   // Around 110

                        CheckTriangle(nIsSet110, v110, nIsSet101, v101, nIsSet011, v011, ref myNumVerts9, ref myVerts9, ref myTriangles9);   // Around 111
                        CheckTriangle(nIsSet110, v110, nIsSet000, v000, nIsSet011, v011, ref myNumVerts11, ref myVerts11, ref myTriangles11);   // Around 010

                        CheckTriangle(nIsSet111, v111, nIsSet001, v001, nIsSet010, v010, ref myNumVerts10, ref myVerts10, ref myTriangles10);   // Around 011
                        CheckTriangle(nIsSet101, v101, nIsSet011, v011, nIsSet000, v000, ref myNumVerts12, ref myVerts12, ref myTriangles12);   // Around 001

                        /*

                        // Do cases of 2 "on" corners... ("edges")

                        // Do cases of 1 "on" corners... ("vertices")
                        */

                        //else
                        {
                            if (nSet >= 13)
                            {
                                AddQuadBoth(v000, v010, v001, v011, ref myNumVerts13, ref myVerts13, ref myTriangles13);    // x-
                                AddQuadBoth(v000, v001, v100, v101, ref myNumVerts13, ref myVerts13, ref myTriangles13);    // y-
                                AddQuadBoth(v000, v100, v010, v110, ref myNumVerts13, ref myVerts13, ref myTriangles13);    // z-

                                AddQuadBoth(v100, v110, v101, v111, ref myNumVerts13, ref myVerts13, ref myTriangles13);    // x+
                                AddQuadBoth(v010, v011, v110, v111, ref myNumVerts13, ref myVerts13, ref myTriangles13);    // y+
                                AddQuadBoth(v001, v101, v011, v111, ref myNumVerts13, ref myVerts13, ref myTriangles13);    // z+
                            }
                        }
                    }
                }
            }
        }

        //
        ProcessMesh(ref mfMain0, ref myVerts0, ref myTriangles0);
        ProcessMesh(ref mfMain1, ref myVerts1, ref myTriangles1);
        ProcessMesh(ref mfMain2, ref myVerts2, ref myTriangles2);
        ProcessMesh(ref mfMain3, ref myVerts3, ref myTriangles3);
        ProcessMesh(ref mfMain4, ref myVerts4, ref myTriangles4);
        ProcessMesh(ref mfMain5, ref myVerts5, ref myTriangles5);
        ProcessMesh(ref mfMain6, ref myVerts6, ref myTriangles6);
        ProcessMesh(ref mfMain7, ref myVerts7, ref myTriangles7);
        ProcessMesh(ref mfMain8, ref myVerts8, ref myTriangles8);
        ProcessMesh(ref mfMain9, ref myVerts9, ref myTriangles9);
        ProcessMesh(ref mfMain10, ref myVerts10, ref myTriangles10);
        ProcessMesh(ref mfMain11, ref myVerts11, ref myTriangles11);
        ProcessMesh(ref mfMain12, ref myVerts12, ref myTriangles12);
        ProcessMesh(ref mfMain13, ref myVerts13, ref myTriangles13);
    }


    public void ProcessMesh(ref MeshFilter mf, ref List<Vector3> verts, ref List<int> triangles)
    {
        mf.mesh.vertices = verts.ToArray();
        mf.mesh.triangles = triangles.ToArray();
        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();
    }


    public void CheckQuads(int in00, Vector3 v00, int in01, Vector3 v01, int in10, Vector3 v10, int in11, Vector3 v11, ref int numVerts, ref List<Vector3> verts, ref List<int> triangles)
    {
        if (in00 == 0 && in01 == 0 && in10 == 0 && in11 == 0)
        {
            AddQuadBoth(v00, v01, v10, v11, ref numVerts, ref verts, ref triangles);
        }
        else
        {
            CheckTriangle(in01, v01, in10, v10, in11, v11, ref numVerts, ref verts, ref triangles);
            CheckTriangle(in00, v00, in10, v10, in11, v11, ref numVerts, ref verts, ref triangles);
            CheckTriangle(in00, v00, in01, v01, in11, v11, ref numVerts, ref verts, ref triangles);
            CheckTriangle(in00, v00, in01, v01, in10, v10, ref numVerts, ref verts, ref triangles);
        }
    }

    public void CheckTriangle(int in00, Vector3 v00, int in01, Vector3 v01, int in10, Vector3 v10, ref int numVerts, ref List<Vector3> verts, ref List<int> triangles)
    {
        if (in00 == 0 && in01 == 0 && in10 == 0)
        {
            AddTriangleBoth(v00, v01, v10, ref numVerts, ref verts, ref triangles);
        }
    }


    public void AddQuadBoth(Vector3 v00, Vector3 v01, Vector3 v10, Vector3 v11, ref int numVerts, ref List<Vector3> verts, ref List<int> triangles)
    {
        ///if (myNumVerts > MAXTRIANGLES) return;

        verts.Add(v00);
        verts.Add(v10);
        verts.Add(v01);
        verts.Add(v11);

        verts.Add(v00);
        verts.Add(v10);
        verts.Add(v01);
        verts.Add(v11);

        triangles.Add(numVerts + 0);
        triangles.Add(numVerts + 2);
        triangles.Add(numVerts + 1);

        triangles.Add(numVerts + 2);
        triangles.Add(numVerts + 3);
        triangles.Add(numVerts + 1);

        // Other side;
        triangles.Add(numVerts + 0 + 4);
        triangles.Add(numVerts + 1 + 4);
        triangles.Add(numVerts + 2 + 4);

        triangles.Add(numVerts + 2 + 4);
        triangles.Add(numVerts + 1 + 4);
        triangles.Add(numVerts + 3 + 4);

        numVerts += 8;
    }


    public void AddTriangleBoth(Vector3 v00, Vector3 v01, Vector3 v10, ref int numVerts, ref List<Vector3> verts, ref List<int> triangles)
    {
        //if (numVerts > MAXTRIANGLES) return;

        verts.Add(v00);
        verts.Add(v01);
        verts.Add(v10);

        verts.Add(v00);
        verts.Add(v10);
        verts.Add(v01);

        triangles.Add(numVerts + 0);
        triangles.Add(numVerts + 2);
        triangles.Add(numVerts + 1);

        // Other side;
        triangles.Add(numVerts + 0 + 3);
        triangles.Add(numVerts + 2 + 3);
        triangles.Add(numVerts + 1 + 3);

        numVerts += 6;
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
