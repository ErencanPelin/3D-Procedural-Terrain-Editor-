using System.Collections;
using System.Collections.Generic; //import to use lists
using UnityEngine;
using ErencanCustomTriangulationTable; //import my custom namespace that holds the triangulation table

public class EJMarchingCubes2 : MonoBehaviour
{
    public List<ChunkData> chunks = new List<ChunkData>();

    public WorldGenSettings worldGenSettings; //holds the global world generation settings to read/write to

    [HideInInspector]
    public float[,,] points;

    //the default material to use
    public Material defaultMaterial;

    //this method is called externally from the GeneratePoints class once the noise map/scalar-field is generated
    public void Generate()
    {
        points = GetComponent<GeneratePoints>().points;
        for (int x = 0; x < (worldGenSettings.size - 1) / worldGenSettings.chunkSize; x++)
        {
            for (int y = 0; y < (worldGenSettings.size - 1) / worldGenSettings.chunkSize; y++)
            {
                for (int z = 0; z < (worldGenSettings.size - 1) / worldGenSettings.chunkSize; z++)
                {
                    //create a new chunkObject, name it based on its position
                    GameObject newChunk = new GameObject { name = x.ToString() + "_" + y.ToString() + "_" + z.ToString() };
                    newChunk.transform.parent = this.transform; //set its parent to the transform of the object this script is attached to
                    newChunk.AddComponent<MeshFilter>(); //holds the mesh
                    newChunk.AddComponent<MeshRenderer>(); //shows the mesh
                    newChunk.AddComponent<MeshCollider>(); //to detect collision
                    newChunk.GetComponent<Renderer>().material = defaultMaterial; //sets the material
                    newChunk.AddComponent<Rigidbody>();
                    newChunk.GetComponent<Rigidbody>().isKinematic = true;
                    newChunk.AddComponent<ChunkBehaviour>();
                    newChunk.GetComponent<ChunkBehaviour>().worldAlgorithm = this;
                    newChunk.GetComponent<ChunkBehaviour>().chunkData = CreateChunkData(newChunk, new Vector3Int(x, y, z));
                    //chunks.Add(CreateChunkData(newChunk, new Vector3Int(x, y, z)));
                }
            }
        }
    }

    ChunkData CreateChunkData(GameObject chunkObject, Vector3Int position) 
    {
        ChunkData newChunk = new ChunkData(chunkObject, position, 
            new Vector3Int((position.x * worldGenSettings.chunkSize) + (worldGenSettings.chunkSize / 2), 
            (position.y * worldGenSettings.chunkSize) + (worldGenSettings.chunkSize / 2),
            (position.z * worldGenSettings.chunkSize) + (worldGenSettings.chunkSize / 2)), new Mesh(), new List<Vector3>(),new List<int>());
        
        return newChunk;
    }

    /*
    List<Vector3> CalculateChunkVertices(ChunkData chunkData) 
    {
        List<Vector3> vertices = new List<Vector3>();

        for (int x = chunkData.chunkPos.x * worldGenSettings.chunkSize; x < (chunkData.chunkPos.x * worldGenSettings.chunkSize) + worldGenSettings.chunkSize; x++) //calculates the start of the chunk, and loops until the end
        {
            for (int y = chunkData.chunkPos.y * worldGenSettings.chunkSize; y < (chunkData.chunkPos.y * worldGenSettings.chunkSize) + worldGenSettings.chunkSize; y++)
            {
                for (int z = chunkData.chunkPos.z * worldGenSettings.chunkSize; z < (chunkData.chunkPos.z * worldGenSettings.chunkSize) + worldGenSettings.chunkSize; z++)
                {
                    foreach (Vector3 i in CalculateCubeVertices(new Vector3Int(x, y, z))) 
                    {
                        vertices.Add(i);
                    }
                }
            }
        }

        return vertices;
    }
    
    //this method creates a new cube and sets all the data for it
    List<Vector3> CalculateCubeVertices(Vector3Int origin)
    {
        //set the vertice values and create new vertice classes for each vertice of the cube
        VerticeClass v0 = new VerticeClass(CheckCubeVertice(new Vector3Int(origin.x, origin.y, origin.z + 1)), points[origin.x, origin.y, origin.z + 1], new Vector3(origin.x, origin.y, origin.z + 1));
        VerticeClass v1 = new VerticeClass(CheckCubeVertice(new Vector3Int(origin.x + 1, origin.y, origin.z + 1)), points[origin.x + 1, origin.y, origin.z + 1], new Vector3(origin.x + 1, origin.y, origin.z + 1));
        VerticeClass v2 = new VerticeClass(CheckCubeVertice(new Vector3Int(origin.x + 1, origin.y, origin.z)), points[origin.x + 1, origin.y, origin.z], new Vector3(origin.x + 1, origin.y, origin.z));
        VerticeClass v3 = new VerticeClass(CheckCubeVertice(new Vector3Int(origin.x, origin.y, origin.z)), points[origin.x, origin.y, origin.z], new Vector3(origin.x, origin.y, origin.z));

        VerticeClass v4 = new VerticeClass(CheckCubeVertice(new Vector3Int(origin.x, origin.y + 1, origin.z + 1)), points[origin.x, origin.y + 1, origin.z + 1], new Vector3(origin.x, origin.y + 1, origin.z + 1));
        VerticeClass v5 = new VerticeClass(CheckCubeVertice(new Vector3Int(origin.x + 1, origin.y + 1, origin.z + 1)), points[origin.x + 1, origin.y + 1, origin.z + 1], new Vector3(origin.x + 1, origin.y + 1, origin.z + 1));
        VerticeClass v6 = new VerticeClass(CheckCubeVertice(new Vector3Int(origin.x + 1, origin.y + 1, origin.z)), points[origin.x + 1, origin.y + 1, origin.z], new Vector3(origin.x + 1, origin.y + 1, origin.z));
        VerticeClass v7 = new VerticeClass(CheckCubeVertice(new Vector3Int(origin.x, origin.y + 1, origin.z)), points[origin.x, origin.y + 1, origin.z], new Vector3(origin.x, origin.y + 1, origin.z));

        //create an array of nodes. Keeping this as an array makes it easier when trying to create triangles from these
        //node datas. Set each node data and create a new node class for every node within the cube
        NodeClass[] nodes = new NodeClass[12];
        //P = P1 + (isovalue - V1) (P2 - P1) / (V2 - V1)
        //nodePos = vert1 + (surfaceLevel - Vert1.value) * (vert1 - vert2) / (vert2.value - vert1.value);
        //above is the code to determine where the surface actually intersects with the edge of a cube. This will give us a much 
        //smoother looking terrain.
        nodes[0] = new NodeClass(new Vector3(origin.x + CalculateP(v0, v1), origin.y, origin.z + 1));
        nodes[1] = new NodeClass(new Vector3(origin.x + 1, origin.y, origin.z + CalculateP(v2, v1)));
        nodes[2] = new NodeClass(new Vector3(origin.x + CalculateP(v3, v2), origin.y, origin.z));
        nodes[3] = new NodeClass(new Vector3(origin.x, origin.y, origin.z + CalculateP(v3, v0)));

        nodes[4] = new NodeClass(new Vector3(origin.x + CalculateP(v4, v5), origin.y + 1, origin.z + 1));
        nodes[5] = new NodeClass(new Vector3(origin.x + 1, origin.y + 1, origin.z + CalculateP(v6, v5)));
        nodes[6] = new NodeClass(new Vector3(origin.x + CalculateP(v7, v6), origin.y + 1, origin.z));
        nodes[7] = new NodeClass(new Vector3(origin.x, origin.y + 1, origin.z + CalculateP(v7, v4)));

        nodes[8] = new NodeClass(new Vector3(origin.x, origin.y + CalculateP(v0, v4), origin.z + 1));
        nodes[9] = new NodeClass(new Vector3(origin.x + 1, origin.y + CalculateP(v1, v5), origin.z + 1));
        nodes[10] = new NodeClass(new Vector3(origin.x + 1, origin.y + CalculateP(v2, v6), origin.z));
        nodes[11] = new NodeClass(new Vector3(origin.x, origin.y + CalculateP(v3, v7), origin.z));

        //determine the triangulation config based on the active states of each vertice
        //this words similar to how binary to decimal translating works
        int triangulationConfig = 0;
        if (v0.active)
            triangulationConfig += 1;
        if (v1.active)
            triangulationConfig += 2;
        if (v2.active)
            triangulationConfig += 4;
        if (v3.active)
            triangulationConfig += 8;
        if (v4.active)
            triangulationConfig += 16;
        if (v5.active)
            triangulationConfig += 32;
        if (v6.active)
            triangulationConfig += 64;
        if (v7.active)
            triangulationConfig += 128;

        //create a clean new list of node indexes to be used in later codes
        List<int> nodeIndexes = new List<int>();
        //clear the numbers we dont need
        for (int i = 0; i < TriangulationTable.triangulationTable[triangulationConfig].Count; i++)
        {
            //if a number is -1 (end of the list) don't include it in the new clean, nodeIndexs list
            if (TriangulationTable.triangulationTable[triangulationConfig][i] != -1)
            {
                //add the number
                nodeIndexes.Add(TriangulationTable.triangulationTable[triangulationConfig][i]);
            }
            //this turns lists such as 1,3,5,2,5,7,-1,-1,-1,-1,-1,-1 Into:
            //1,3,5,2,5,7 - meaning that only the non '-1' numbers are kept, creating a clean list of actual values we can use later
        }

        //loop through every element in nodeIndexes in groups of three and pass each three numbers into the DrawTriangle method.
        //this works as a triangle requires 3 points to join to create a triangle. This passes in 3 integers at a time and the nodes list
        //it then uses the three integers as indexes to look for the 3 nodes in the nodes list and then joins these nodes based on their 
        //position to form a triangle

        List<Vector3> cubeVerts = new List<Vector3>();

        for (int i = 0; i < nodeIndexes.Count; i += 3)
        {
            //pass in the next three numbers and nodes array to form a triangle
            //also passes in the chunk the cube will be sorted into
            //DrawTriangle(nodeIndexes[i], nodeIndexes[i + 1], nodeIndexes[i + 2], nodes, Color.green);
            cubeVerts.Add(nodes[nodeIndexes[i]].pos);
            cubeVerts.Add(nodes[nodeIndexes[i + 1]].pos);
            cubeVerts.Add(nodes[nodeIndexes[i + 2]].pos);
        }
        
        return cubeVerts;
    }*/
    
    public float CalculateP(VerticeClass P1, VerticeClass P2)
    {
        //create a value which will be set to the value between 0 and 1 where the node will be placed along the edge
        float p = 0;

        //are we using the edge interpolation method?
        if (worldGenSettings.useEdgeInterpolation)
        {
            //interpolation math
            //P = P1 + (isovalue - V1) (P2 - P1) / (V2 - V1) --- OBSELETE
            
            //FROM DESK CHECK
            //=IF(C6<E6,(D$3-C6)/(E6-C6),1- (D$3-E6)/(C6-E6)) ---- from deskCheck in Excel
            
            //SUB IN THE VALUES ---->
            //=IF(v1 < v2,(surfaceLevel - V1Val)/(v2Val - v1Val) ELSE: 1 - (surfaceLevel - V2Val)/(V1Val - V2Val)) ---- from deskCheck in Excel

            //checks which value is greater, this will determine which equation to use
            if (P1.value < P2.value)
            {
                //same equation as the excel deskcheck. this uses the value of each node and through this equation, it returns a value
                //as a decimal point between 0 and 1.
                p = (worldGenSettings.surfaceLevel - P1.value) / (P2.value - P1.value);
            }
            else if (P1.value > P2.value)
            {
                //inerted method if the first value is greater than the second instead
                p = 1 - (worldGenSettings.surfaceLevel - P2.value) / (P1.value - P2.value);
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

    //this checks a given point and compares whether it's value is less than the surface level or not. Returning a true or false 
    //after. This true/false is the active state of this vertice
    public bool CheckCubeVertice(Vector3Int vertexPoint)
    {
        //if the scalar-point's value is beneath the surface value, this point is active. Otherwise it is not
        if (points[vertexPoint.x, vertexPoint.y, vertexPoint.z] <= worldGenSettings.surfaceLevel)
        {
            return true;
        }
        else
        {
            return false;
        }
        //returns a bool
    }
}
