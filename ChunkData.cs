using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ChunkData
{
    public GameObject chunkObject;
    public Vector3Int chunkPos;
    public Vector3Int worldPosition;

    public Mesh chunkMesh;
    public List<Vector3> meshVertices;
    public List<int> meshTriangles;
   public ChunkData(GameObject _chunkObj, Vector3Int _chunkPos, Vector3Int _worldPos, Mesh _chunkMesh, List<Vector3> _meshVertices, List<int> _meshTriangles) 
    {
        chunkObject = _chunkObj;
        chunkPos = _chunkPos;
        worldPosition = _worldPos;
        chunkMesh = _chunkMesh;
        meshVertices = _meshVertices;
        meshTriangles = _meshTriangles;
    }
}
