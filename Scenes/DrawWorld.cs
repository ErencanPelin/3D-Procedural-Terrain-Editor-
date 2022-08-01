using System.Collections;
using UnityEngine;

public class DrawWorld : MonoBehaviour
{
    public Material defaultMaterial; //the material holding the vertex shader
    public Gradient colourMap; //the gradient for the colourmap for the world
    public WorldGenSettings worldGenSettings; //a reference to the world gen settings
    [HideInInspector] //doens't show this data in inspector to keep the workplace clean
    public float[,,] points;//a 3d array of points used to calculate where vertices are placed in the world

    [HideInInspector]
    public float highestPoint; //the highest point from the generate points method
    [HideInInspector]
    public float lowestPoint; //the lowest point from the generate points method

    public void Generate() 
    {
        points = GetComponent<GeneratePoints>().points;

        //loop through the world, placing a chunk where it should be based onthe chunk size
        for (int x = 0; x < (worldGenSettings.size - 1) / worldGenSettings.chunkSize; x++)
        {
            for (int y = 0; y < (worldGenSettings.size - 1) / worldGenSettings.chunkSize; y++)
            {
                for (int z = 0; z < (worldGenSettings.size - 1) / worldGenSettings.chunkSize; z++)
                {
                    //create a new chunkObject, name it based on its position and adds the needed componenets
                    GameObject newChunk = new GameObject { name = x.ToString() + "_" + y.ToString() + "_" + z.ToString() };
                    newChunk.transform.parent = this.transform; //set its parent to the transform of the object this script is attached to
                    newChunk.AddComponent<ChunkGenerator>();
                    newChunk.GetComponent<Renderer>().material = defaultMaterial;
                    newChunk.GetComponent<ChunkGenerator>().chunkOrigin = new Vector3Int(x, y, z);
                    newChunk.GetComponent<ChunkGenerator>().worldDraw = this;
                }
            }
        }
    }
}
