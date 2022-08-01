using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ErencanCustomTriangulationTable; //import my custom/edited triangulation table

//define all the components this script requires to run correctly
//this also adds these compoenents automatically when the script is created, saving me from adding them from code
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Rigidbody))]
public class ChunkGenerator : MonoBehaviour
{
    //where the chunk is placed
    public Vector3Int chunkOrigin;

    //a reference to the script that made this gameobject (the world generator)
    public DrawWorld worldDraw;

    //a list of where all the vertices will be placed in this chunk
    private List<Vector3> chunkVertices = new List<Vector3>();

    private void Awake()
    {
        //set it so that the chunk can detect colliders but is not affected by physics forces
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Start()
    {
        //calculates the start of the chunk, and loops until the end
        for (int x = chunkOrigin.x * worldDraw.worldGenSettings.chunkSize; x < (chunkOrigin.x * worldDraw.worldGenSettings.chunkSize) + worldDraw.worldGenSettings.chunkSize; x++) 
        {
            for (int y = chunkOrigin.y * worldDraw.worldGenSettings.chunkSize; y < (chunkOrigin.y * worldDraw.worldGenSettings.chunkSize) + worldDraw.worldGenSettings.chunkSize; y++)
            {
                for (int z = chunkOrigin.z * worldDraw.worldGenSettings.chunkSize; z < (chunkOrigin.z * worldDraw.worldGenSettings.chunkSize) + worldDraw.worldGenSettings.chunkSize; z++)
                {
                    //create a new cube and corresponding cube data at every interval of the chunk
                    DrawCube(CreateCubeData(new Vector3Int(x, y, z)));
                }
            }
        }

        //generate the mesh of the chunk based on the vertices added to the vertece list
        RenderChunkMesh(chunkVertices.ToArray());
    }

    public void DrawCube(Cube cube) 
    {
        //gets the configuration of the cube by checking if each vertice is active
        int config = 0;
        if (cube.vertexValues[0] <= worldDraw.worldGenSettings.surfaceLevel)
            config += 1;
        if (cube.vertexValues[1] <= worldDraw.worldGenSettings.surfaceLevel)
            config += 2;
        if (cube.vertexValues[2] <= worldDraw.worldGenSettings.surfaceLevel)
            config += 4;
        if (cube.vertexValues[3] <= worldDraw.worldGenSettings.surfaceLevel)
            config += 8;
        if (cube.vertexValues[4] <= worldDraw.worldGenSettings.surfaceLevel)
            config += 16;
        if (cube.vertexValues[5] <= worldDraw.worldGenSettings.surfaceLevel)
            config += 32;
        if (cube.vertexValues[6] <= worldDraw.worldGenSettings.surfaceLevel)
            config += 64;
        if (cube.vertexValues[7] <= worldDraw.worldGenSettings.surfaceLevel)
            config += 128;

        //gets the corresponding configuration from the triangulation table, turns it into an array, and adds each needed
        //vertice into the chunk vertice list if the index value of that node is not -1 as this is not a valid number, but it indicates
        //the end of the sequence in the triangulation table
        for (int i = 0; i < CustomTable.triangulationTable[config].ToArray().Length; i++) 
        {
            if (CustomTable.triangulationTable[config].ToArray()[i] != -1) //only pass in valid numbers
            {
                //add this node's position into the vertice list to be used to generate the mesh later
                chunkVertices.Add(cube.nodePositions[CustomTable.triangulationTable[config].ToArray()[i]]);
            }
        }
    }

    public Cube CreateCubeData(Vector3Int origin) 
    {
        //create a new cube data with corresponding vertice values and node positions
        Cube newCubeData = new Cube(new float[8], new Vector3[12]);

        //get the scalar field values at each vertex of the cube and sets it to the corresponding vertice in the new cube class
        newCubeData.vertexValues[0] = worldDraw.points[origin.x, origin.y, origin.z + 1];
        newCubeData.vertexValues[1] = worldDraw.points[origin.x + 1, origin.y, origin.z + 1];
        newCubeData.vertexValues[2] = worldDraw.points[origin.x + 1, origin.y, origin.z];
        newCubeData.vertexValues[3] = worldDraw.points[origin.x, origin.y, origin.z];

        newCubeData.vertexValues[4] = worldDraw.points[origin.x, origin.y + 1, origin.z + 1];
        newCubeData.vertexValues[5] = worldDraw.points[origin.x + 1, origin.y + 1, origin.z + 1];
        newCubeData.vertexValues[6] = worldDraw.points[origin.x + 1, origin.y + 1, origin.z];
        newCubeData.vertexValues[7] = worldDraw.points[origin.x, origin.y + 1, origin.z];

        //set the positions for the nodes of the cube
        newCubeData.nodePositions[0] = new Vector3(origin.x + CalculateP(newCubeData.vertexValues[0], newCubeData.vertexValues[1]), origin.y, origin.z + 1);
        newCubeData.nodePositions[1] = new Vector3(origin.x + 1, origin.y, origin.z + CalculateP(newCubeData.vertexValues[2], newCubeData.vertexValues[1]));
        newCubeData.nodePositions[2] = new Vector3(origin.x + CalculateP(newCubeData.vertexValues[3], newCubeData.vertexValues[2]), origin.y, origin.z);
        newCubeData.nodePositions[3] = new Vector3(origin.x, origin.y, origin.z + CalculateP(newCubeData.vertexValues[3], newCubeData.vertexValues[0]));

        newCubeData.nodePositions[4] = new Vector3(origin.x + CalculateP(newCubeData.vertexValues[4], newCubeData.vertexValues[5]), origin.y + 1, origin.z + 1);
        newCubeData.nodePositions[5] = new Vector3(origin.x + 1, origin.y + 1, origin.z + CalculateP(newCubeData.vertexValues[6], newCubeData.vertexValues[5]));
        newCubeData.nodePositions[6] = new Vector3(origin.x + CalculateP(newCubeData.vertexValues[7], newCubeData.vertexValues[6]), origin.y + 1, origin.z);
        newCubeData.nodePositions[7] = new Vector3(origin.x, origin.y + 1, origin.z + CalculateP(newCubeData.vertexValues[7], newCubeData.vertexValues[4]));

        newCubeData.nodePositions[8] = new Vector3(origin.x, origin.y + CalculateP(newCubeData.vertexValues[0], newCubeData.vertexValues[4]), origin.z + 1);
        newCubeData.nodePositions[9] = new Vector3(origin.x + 1, origin.y + CalculateP(newCubeData.vertexValues[1], newCubeData.vertexValues[5]), origin.z + 1);
        newCubeData.nodePositions[10] = new Vector3(origin.x + 1, origin.y + CalculateP(newCubeData.vertexValues[2], newCubeData.vertexValues[6]), origin.z);
        newCubeData.nodePositions[11] = new Vector3(origin.x, origin.y + CalculateP(newCubeData.vertexValues[3], newCubeData.vertexValues[7]), origin.z);

        //passes back the generated cube data
        return newCubeData;
    }

    public float CalculateP(float P1, float P2)
    {
        //create a value which will be set to the value between 0 and 1 where the node will be placed along the edge
        float p = 0;

        //are we using the edge interpolation method?
        if (worldDraw.worldGenSettings.useEdgeInterpolation)
        {
            //interpolation math
            //P = P1 + (isovalue - V1) (P2 - P1) / (V2 - V1) --- OBSELETE

            //FROM DESK CHECK
            //=IF(C6<E6,(D$3-C6)/(E6-C6),1- (D$3-E6)/(C6-E6)) ---- from deskCheck in Excel

            //SUB IN THE VALUES ---->
            //=IF(v1 < v2,(surfaceLevel - V1Val)/(v2Val - v1Val) ELSE: 1 - (surfaceLevel - V2Val)/(V1Val - V2Val)) ---- from deskCheck in Excel

            //checks which value is greater, this will determine which equation to use
            if (P1 < P2)
            {
                //same equation as the excel deskcheck. this uses the value of each node and through this equation, it returns a value
                //as a decimal point between 0 and 1.
                p = (worldDraw.worldGenSettings.surfaceLevel - P1) / (P2 - P1);
            }
            else if (P1 > P2)
            {
                //inerted method if the first value is greater than the second instead
                p = 1 - (worldDraw.worldGenSettings.surfaceLevel - P2) / (P1 - P2);
            }
            else
            {
                //if they're equal or both greater than the surface value, or both below the surface value, this node will not be
                //used so set it to the midpoint again anyway as it is useless
                p = 0.5f;
            }
        }
        else
        {
            //otherwise just set it to the midpoint
            p = 0.5f;
        }

        //clamps the final value between 0 and 1. This stops calculations that return greater than one, and stops the mesh from turning
        //into spaghetti
        return Mathf.Clamp01(p);
    }

    public float EvaluateColourMap(float low, float high, int Ypos) 
    {
        //evaluates the color of the vertice based on its position to the lowest and highest points of the world
        float colour = (Ypos - low) / (high - low);
        return Mathf.Clamp01(colour); //return this colour by clamping it between 0 and 1 to avoid potential errors
    }

    public void RenderChunkMesh(Vector3[] vertices)
    {
        //create a new mesh
        Mesh chunkMesh = new Mesh();
        //creates the array of triangles
        int[] triangles = new int[vertices.Length];
        //creates the array of vertex colors
        Color[] colors = new Color[vertices.Length];

        //sets the colors for each vertex by passing in distances and positions relative to the world and recieving a color in return
        for (int i = 0; i < colors.Length; i++) 
        {
            if (worldDraw.worldGenSettings.generationType == WorldGenSettings.GenerationType.heightmap)
            {
                //use these valeus to evaluate if it is a heightmap world
                colors[i] = worldDraw.colourMap.Evaluate(EvaluateColourMap(worldDraw.lowestPoint, worldDraw.worldGenSettings.chunkSize, Mathf.RoundToInt(vertices[i].y / 3)));
            }
            else if (worldDraw.worldGenSettings.generationType == WorldGenSettings.GenerationType.playground) 
            {
                //use these values to evaluate if it is a playground world
                colors[i] = worldDraw.colourMap.Evaluate(EvaluateColourMap(worldDraw.lowestPoint, worldDraw.worldGenSettings.chunkSize, Mathf.RoundToInt(vertices[i].y / 3)));
            }
            else if (worldDraw.worldGenSettings.generationType == WorldGenSettings.GenerationType.spherical)
            {
                //use these values to evaluate if it is a spherical world
                colors[i] = worldDraw.colourMap.Evaluate(((Vector3.Distance(new Vector3((worldDraw.worldGenSettings.size / 2) - 1, (worldDraw.worldGenSettings.size / 2) - 1, (worldDraw.worldGenSettings.size / 2) - 1),
                vertices[i])) - worldDraw.worldGenSettings.groundHeight) / 100);
            }

        }

        //adds all the values into the triangles array. This is simple as all the vertices are added in order so when the mesh is 
        //generated, no errors are made anyway
        for (int t = 0; t < triangles.Length; t++)
        {
            triangles[t] = t;
        }

        //apply the data to the mesh
        chunkMesh.vertices = vertices;
        chunkMesh.triangles = triangles;
        chunkMesh.colors = colors;
        //recalculate the lighting of the mesh
        chunkMesh.RecalculateNormals();
        //apply the mesh to the object
        GetComponent<MeshFilter>().mesh = chunkMesh;
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }
}
