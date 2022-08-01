using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "newGenerationSettings", menuName = "Generation Settings")]
public class WorldGenSettings : ScriptableObject
{
    //a class to hold all data for the world generation settings
    public enum GenerationType //the types of generation available
    {
        spherical,  //generates a spherical world
        playground, //generates a random 3d noise map
        heightmap //uses a 2d texture to generate a proper terrain
    }

    public GenerationType generationType;

    public int seed;//unique id of the generated landscape
    public bool useEdgeInterpolation;//this determines whether or not the marching cubes algorithm will use a complex
                                     //algorithm to produce a more accurate surface
    public int sphericalRadius; //radius of spherical worlds
    public int groundHeight = 3;//base height of the terrain
    public int chunkSize = 20;//the size of the chunks the world is broken up into
    public int size;//size of the world
    public float noiseFrequency; //the frequency of the noise. How much variation there is in the noise
    public float terrainAmplitude;//the multiplier that determines how mountainess the terrain will be
    public int heightmapLayers;//how much detail in the heightmap
    public float surfaceLevel = 0;//the surface level. - kept between -34 and 16 as these are the only possible values for our 
                                  //calculations to return. 
}
