using System.Collections;
using UnityEngine;

//very similar to the VerticeClass where this script is a custom class which does not derrive from a monobehaviour. meaning it does
//not have to be applied to a gameobject to function. This script is used solely as a data holder for each node of each cube
public class NodeClass 
{
    public Vector3 pos;    //the position relatvie to the world for that node

    //creates a new NodeClass to save the data
    public NodeClass(Vector3 _pos) 
    {
        //saves the data passed into this class from other scripts into the class
        pos = _pos;
    }
}
//this script works similar to a dictionary would to hold a definition for each vertice. These custom classes are useful to hold data
//but can be used for a variety of other instances as well