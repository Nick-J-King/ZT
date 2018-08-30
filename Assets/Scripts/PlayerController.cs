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


    public void ResetMesh(int mesh)
    {
        myNumVerts[mesh] = 0;
        myNumTriangles[mesh] = 0;
        myVerts[mesh] = new List<Vector3>();
        myTriangles[mesh] = new List<int>();
        mfSub[mesh].mesh.Clear();
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

                        CheckQuads(nIsSet000, v000, nIsSet011, v011, nIsSet100, v100, nIsSet111, v111, 3);    // Along x
                        CheckQuads(nIsSet010, v010, nIsSet001, v001, nIsSet110, v110, nIsSet101, v101, 4);    // Along x

                        CheckQuads(nIsSet000, v000, nIsSet101, v101, nIsSet010, v010, nIsSet111, v111, 5);    // Along y
                        CheckQuads(nIsSet100, v100, nIsSet001, v001, nIsSet110, v110, nIsSet011, v011, 6);    // Along y

                        CheckQuads(nIsSet000, v000, nIsSet110, v110, nIsSet001, v001, nIsSet111, v111, 7);    // Along z
                        CheckQuads(nIsSet100, v100, nIsSet010, v010, nIsSet101, v101, nIsSet011, v011, 8);    // Along z


                        // Flat faces

                        CheckQuads(nIsSet000, v000, nIsSet001, v001, nIsSet010, v010, nIsSet011, v011, 0);    // Along x = 0
                        CheckQuads(nIsSet000, v000, nIsSet001, v001, nIsSet100, v100, nIsSet101, v101, 1);    // Along y = 0
                        CheckQuads(nIsSet000, v000, nIsSet010, v010, nIsSet100, v100, nIsSet110, v110, 2);    // Along z = 0


                        // Do cases of 3 "on" corners now!

                        // Corners

                        CheckTriangle(nIsSet001, v001, nIsSet010, v010, nIsSet100, v100, 9);      // Around 000
                        CheckTriangle(nIsSet000, v000, nIsSet101, v101, nIsSet110, v110, 10);   // Around 100
                        CheckTriangle(nIsSet100, v100, nIsSet111, v111, nIsSet001, v001, 11);   // Around 101
                        CheckTriangle(nIsSet111, v111, nIsSet100, v100, nIsSet010, v010, 12);   // Around 110

                        CheckTriangle(nIsSet110, v110, nIsSet101, v101, nIsSet011, v011, 9);   // Around 111
                        CheckTriangle(nIsSet110, v110, nIsSet000, v000, nIsSet011, v011, 11);   // Around 010

                        CheckTriangle(nIsSet111, v111, nIsSet001, v001, nIsSet010, v010, 10);   // Around 011
                        CheckTriangle(nIsSet101, v101, nIsSet011, v011, nIsSet000, v000, 12);   // Around 001

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
