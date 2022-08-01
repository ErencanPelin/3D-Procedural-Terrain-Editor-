using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ErencanCustomTriangulationTable;

public class ChunkBehaviour : MonoBehaviour
{
    public EJMarchingCubes2 worldAlgorithm;
    public ChunkData chunkData;

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "brush") 
        {
            if (Input.GetMouseButton(0)) 
            {
                //update itself
                UpdateChunk();
            }
        }
    }

    private void Start()
    {
        //generate its own mesh data
        GenerateChunk();
    }

    void GenerateChunk() 
    {
        chunkData.meshVertices = CalculateChunkVertices(chunkData);

        for (int t = 0; t < chunkData.meshVertices.Count; t++)
        {
            chunkData.meshTriangles.Add(t);
        }

        chunkData.chunkMesh.vertices = chunkData.meshVertices.ToArray();
        chunkData.chunkMesh.triangles = chunkData.meshTriangles.ToArray();
        chunkData.chunkMesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = chunkData.chunkMesh;
        GetComponent<MeshCollider>().sharedMesh = chunkData.chunkMesh;
    }

    void UpdateChunk() 
    {
        chunkData.chunkMesh.Clear();
        chunkData.chunkMesh.vertices = chunkData.meshVertices.ToArray();
        chunkData.chunkMesh.triangles = chunkData.meshTriangles.ToArray();
        chunkData.chunkMesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = chunkData.chunkMesh;
        GetComponent<MeshCollider>().sharedMesh = chunkData.chunkMesh;
    }

    List<Vector3> CalculateChunkVertices(ChunkData chunkData)
    {
        List<Vector3> vertices = new List<Vector3>();

        //calculates the start of the chunk, and loops until the end
        for (int x = chunkData.chunkPos.x * worldAlgorithm.worldGenSettings.chunkSize; x < (chunkData.chunkPos.x * worldAlgorithm.worldGenSettings.chunkSize) + worldAlgorithm.worldGenSettings.chunkSize; x++)
        {
            for (int y = chunkData.chunkPos.y * worldAlgorithm.worldGenSettings.chunkSize; y < (chunkData.chunkPos.y * worldAlgorithm.worldGenSettings.chunkSize) + worldAlgorithm.worldGenSettings.chunkSize; y++)
            {
                for (int z = chunkData.chunkPos.z * worldAlgorithm.worldGenSettings.chunkSize; z < (chunkData.chunkPos.z * worldAlgorithm.worldGenSettings.chunkSize) + worldAlgorithm.worldGenSettings.chunkSize; z++)
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
        VerticeClass v0 = new VerticeClass(worldAlgorithm.CheckCubeVertice(new Vector3Int(origin.x, origin.y, origin.z + 1)), worldAlgorithm.points[origin.x, origin.y, origin.z + 1]);
        VerticeClass v1 = new VerticeClass(worldAlgorithm.CheckCubeVertice(new Vector3Int(origin.x + 1, origin.y, origin.z + 1)), worldAlgorithm.points[origin.x + 1, origin.y, origin.z + 1]);
        VerticeClass v2 = new VerticeClass(worldAlgorithm.CheckCubeVertice(new Vector3Int(origin.x + 1, origin.y, origin.z)), worldAlgorithm.points[origin.x + 1, origin.y, origin.z]);
        VerticeClass v3 = new VerticeClass(worldAlgorithm.CheckCubeVertice(new Vector3Int(origin.x, origin.y, origin.z)), worldAlgorithm.points[origin.x, origin.y, origin.z]);

        VerticeClass v4 = new VerticeClass(worldAlgorithm.CheckCubeVertice(new Vector3Int(origin.x, origin.y + 1, origin.z + 1)), worldAlgorithm.points[origin.x, origin.y + 1, origin.z + 1]);
        VerticeClass v5 = new VerticeClass(worldAlgorithm.CheckCubeVertice(new Vector3Int(origin.x + 1, origin.y + 1, origin.z + 1)), worldAlgorithm.points[origin.x + 1, origin.y + 1, origin.z + 1]);
        VerticeClass v6 = new VerticeClass(worldAlgorithm.CheckCubeVertice(new Vector3Int(origin.x + 1, origin.y + 1, origin.z)), worldAlgorithm.points[origin.x + 1, origin.y + 1, origin.z]);
        VerticeClass v7 = new VerticeClass(worldAlgorithm.CheckCubeVertice(new Vector3Int(origin.x, origin.y + 1, origin.z)), worldAlgorithm.points[origin.x, origin.y + 1, origin.z]);

        //create an array of nodes. Keeping this as an array makes it easier when trying to create triangles from these
        //node datas. Set each node data and create a new node class for every node within the cube
        NodeClass[] nodes = new NodeClass[12];
        //P = P1 + (isovalue - V1) (P2 - P1) / (V2 - V1)
        //nodePos = vert1 + (surfaceLevel - Vert1.value) * (vert1 - vert2) / (vert2.value - vert1.value);
        //above is the code to determine where the surface actually intersects with the edge of a cube. This will give us a much 
        //smoother looking terrain.
        nodes[0] = new NodeClass(new Vector3(origin.x + worldAlgorithm.CalculateP(v0, v1), origin.y, origin.z + 1));
        nodes[1] = new NodeClass(new Vector3(origin.x + 1, origin.y, origin.z + worldAlgorithm.CalculateP(v2, v1)));
        nodes[2] = new NodeClass(new Vector3(origin.x + worldAlgorithm.CalculateP(v3, v2), origin.y, origin.z));
        nodes[3] = new NodeClass(new Vector3(origin.x, origin.y, origin.z + worldAlgorithm.CalculateP(v3, v0)));

        nodes[4] = new NodeClass(new Vector3(origin.x + worldAlgorithm.CalculateP(v4, v5), origin.y + 1, origin.z + 1));
        nodes[5] = new NodeClass(new Vector3(origin.x + 1, origin.y + 1, origin.z + worldAlgorithm.CalculateP(v6, v5)));
        nodes[6] = new NodeClass(new Vector3(origin.x + worldAlgorithm.CalculateP(v7, v6), origin.y + 1, origin.z));
        nodes[7] = new NodeClass(new Vector3(origin.x, origin.y + 1, origin.z + worldAlgorithm.CalculateP(v7, v4)));

        nodes[8] = new NodeClass(new Vector3(origin.x, origin.y + worldAlgorithm.CalculateP(v0, v4), origin.z + 1));
        nodes[9] = new NodeClass(new Vector3(origin.x + 1, origin.y + worldAlgorithm.CalculateP(v1, v5), origin.z + 1));
        nodes[10] = new NodeClass(new Vector3(origin.x + 1, origin.y + worldAlgorithm.CalculateP(v2, v6), origin.z));
        nodes[11] = new NodeClass(new Vector3(origin.x, origin.y + worldAlgorithm.CalculateP(v3, v7), origin.z));

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
        for (int i = 0; i < CustomTable.triangulationTable[triangulationConfig].Count; i++)
        {
            //if a number is -1 (end of the list) don't include it in the new clean, nodeIndexs list
            if (CustomTable.triangulationTable[triangulationConfig][i] != -1)
            {
                //add the number
                nodeIndexes.Add(CustomTable.triangulationTable[triangulationConfig][i]);
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
    }

}
