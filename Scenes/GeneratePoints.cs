using System.Collections;
using UnityEngine;

public class GeneratePoints : MonoBehaviour
{
	public WorldGenSettings worldGenSettings; //a reference to the world gen settings

	public float[,,] points; //the scalar-field of points, stored in a 3D float array where each slot holds a float value

	//called once the play button is pressed
	void Start () 
	{
		//set the size of the scalar-field to the size of the world in a 3D space.
		points = new float[worldGenSettings.size, worldGenSettings.size, worldGenSettings.size];

		worldGenSettings.sphericalRadius = Mathf.RoundToInt(0.3f * worldGenSettings.size); //sets th radius to 75% the world size and rounds it to an integer for more steady results

		if (worldGenSettings.seed == 0)
		{
			worldGenSettings.seed = Random.Range(-100000, 100000); // set the seed if none is input
		}

		//are we going to need 3d noise? only these two generation types require 3d noise
		if (worldGenSettings.generationType == WorldGenSettings.GenerationType.playground || worldGenSettings.generationType == WorldGenSettings.GenerationType.spherical)
		{
			//generate world, given a random seed
			Generate3D(); //generates using 3d noise
		}
		else if (worldGenSettings.generationType == WorldGenSettings.GenerationType.heightmap) 
		{
			//use a different method to generate a 2d heightmap
			Generate2D();
		}

		//calls the 'Generate()' function to begin the world generation
		GetComponent<DrawWorld>().Generate();
	}

	//this generates the scalar-field of values 
	void Generate3D() 
	{
		//loops through every point in the world based on the given size of the world
		for (int x = 0; x < worldGenSettings.size; x++) 
		{
			for (int y = 0; y < worldGenSettings.size; y++) 
			{
				for (int z = 0; z < worldGenSettings.size; z++) 
				{
					if (worldGenSettings.generationType == WorldGenSettings.GenerationType.playground) //if were using the playground gen method
					{
						if (y > worldGenSettings.groundHeight)//this generates a solid ground plane by setting all the values below the groundHeight as on top of the surface level
						{
							//creates a new float to hold the noise value for further calculations
							//this passes in a vector3 which is used the 'CalculateNoise' method which returns a float value AKA the noise value
							float noiseValue = CalculateNoise(new Vector3((x + worldGenSettings.seed) * worldGenSettings.noiseFrequency, (y + worldGenSettings.seed) * worldGenSettings.noiseFrequency, (z + worldGenSettings.seed) * worldGenSettings.noiseFrequency));

							//normalizes the noiseValue
							noiseValue = (noiseValue - 0.47f);
							//multiplies the value by 34 if it is below 0, and by 16 if it is not.
							//this clamps the noise value between -34 and 16 which from previous tests and reseach, proved to be the best
							//even range to clamp it to as it visually displayed the best, most even results
							if (noiseValue < 0)
								noiseValue *= 34;
							else
								noiseValue *= 16;

							//saves this noiseValue to the scalar-field
							points[x, y, z] = noiseValue - (y * worldGenSettings.noiseFrequency);
						}
						else
						{
							//ensures the egdes are not visible, producing a solid shape
							points[x, y, z] = worldGenSettings.surfaceLevel + 0.1f;
						}
					}
					else if (worldGenSettings.generationType == WorldGenSettings.GenerationType.spherical) // were using the spherical gen method
					{
						float distbyRadius = worldGenSettings.terrainAmplitude + Vector3.Distance(new Vector3(x, y, z), new Vector3((worldGenSettings.size - 1) / 2, (worldGenSettings.size - 1) / 2, (worldGenSettings.size - 1) / 2));
						//creates a new float to hold the noise value for further calculations
						//this passes in a vector3 which is used the 'CalculateNoise' method which returns a float value AKA the noise value
						float noiseValue = CalculateNoise(new Vector3((x + worldGenSettings.seed) * worldGenSettings.noiseFrequency, (y + worldGenSettings.seed) * worldGenSettings.noiseFrequency, (z + worldGenSettings.seed) * worldGenSettings.noiseFrequency));

						//normalizes the noiseValue
						noiseValue = (noiseValue - 0.47f);
						//multiplies the value by 34 if it is below 0, and by 16 if it is not.
						//this clamps the noise value between -34 and 16 which from previous tests and reseach, proved to be the best
						//even range to clamp it to as it visually displayed the best, most even results
						if (noiseValue < 0)
							noiseValue *= 34;
						else
							noiseValue *= 16;

						//saves this noiseValue to the scalar-field
						points[x, y, z] = ((worldGenSettings.surfaceLevel + (worldGenSettings.sphericalRadius)) - ((distbyRadius - noiseValue) / 2)); //FIX THIS
					}
				}
			}
		}
	}

	void Generate2D() 
	{
		//create the heightmap
		float[,] noiseMap = new float[worldGenSettings.size, worldGenSettings.size];//a 2d float array to hold the final 'texture' for
		//height map
		float highestPoint = 0; //holds the heighest point in the world, - used later
		float lowestPoint = 100000; //holds the lowest point in the world, - used later
		for (int i = 1; i <= worldGenSettings.heightmapLayers; i++)
		{
			//add the values into the noiseMap heightmap, it loops this depending on the heightmap layers and each loop,
			//it increases the frequency, reducing the strength to add detail

			//loop through and fill the array with perlin noise values
			for (int x = 0; x < worldGenSettings.size; x++)
			{
				for (int z = 0; z < worldGenSettings.size; z++)
				{
					//generate a perlin noise value
					float pixValue = Mathf.PerlinNoise((x + worldGenSettings.seed) * (worldGenSettings.noiseFrequency * (i * i)), (z + worldGenSettings.seed) * (worldGenSettings.noiseFrequency * (i * i))) * worldGenSettings.terrainAmplitude;

					//add the new value onto the current value at that index, to create the detailed texture
					noiseMap[x, z] += pixValue / (i * i * i);

					//set the heights value
					if (noiseMap[x, z] > highestPoint)
						highestPoint = noiseMap[x, z];
					//set the lowest value
					if (noiseMap[x, z] < lowestPoint) 
						lowestPoint = noiseMap[x, z];
				}
			}
		}

		
		for (int x = 0; x < noiseMap.GetLength(0); x++) 
		{
			for (int z = 0; z < noiseMap.GetLength(1); z++)
			{
				for (int y = 0; y < worldGenSettings.size; y++)
				{
					float height = noiseMap[x, z] * 3;
					float noiseValue = height - y;
					points[x, y, z] = noiseValue;
				}
			}
		}

		GetComponent<DrawWorld>().highestPoint = highestPoint;
		GetComponent<DrawWorld>().lowestPoint = lowestPoint;
	}

	//this function was used solely for debugging to visualise the scalar-field as a bunch of dots on screen. Simply to test if
	//the scalar-field was working or not
	/*void OnDrawGizmos() 
	{
		//loop through each point in the scalar-field
		for (int x = 0; x < size; x++)
		{
			for (int z = 0; z < size; z++)
			{
				for (int y = 0; y < size; y++)
				{
					//check if the value is below the set surface level
					if (points[x, y, z] >= surfaceLevel)
					{
						//draw a sphere at that point with a radius of 0.1
						Gizmos.DrawSphere(new Vector3(x, y, z), 0.1f);
						//set the sphere's colour to a black-white colour based on the scalar-value of that point
						Gizmos.color = new Color(Mathf.Clamp01(points[x, y, z]), Mathf.Clamp01(points[x, y, z]), Mathf.Clamp01(points[x, y, z]));
					}
				}
			}
		}
	}*/

	//this returns a floating point value which is the 3D noise value of the given vector3 point
	float CalculateNoise(Vector3 coords) 
	{
		//get the perlin noise values for each of the vectors in the given 'coordinates'. These will be averaged later
		float ab = Mathf.PerlinNoise(coords.x, coords.y);
		float bc = Mathf.PerlinNoise(coords.y, coords.z);
		float ac = Mathf.PerlinNoise(coords.x, coords.z);

		//get the inverse of these values
		float ba = Mathf.PerlinNoise(coords.y, coords.x);
		float cb = Mathf.PerlinNoise(coords.z, coords.y);
		float ca = Mathf.PerlinNoise(coords.z, coords.x);

		//return the average of all these values as a floating point number, AKA the noise value at that given point
		return (ab + bc + ac + ba + cb + ca) / 6;
	}
}
