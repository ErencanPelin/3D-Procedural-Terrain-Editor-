using System.Collections;
using UnityEngine;

public class Cube //just a class to hold data
{
    public float[] vertexValues; //the scalar-field values at each vertex of the cube
    public Vector3[] nodePositions; //the positions of the nodes at each vertex of the cube

    // a class that can be edited via external scripts at different instances
    public Cube(float[] _vertexValues, Vector3[] _nodePositions) 
    {
        //apply these values to the script
        vertexValues = _vertexValues; 
        nodePositions = _nodePositions;
    }
}
