using System.Collections;
using UnityEngine;

//this is a custom class which does not derrive from a monobehaviour meaning it does not have to be applied to a gameobject to function
//the purpose of this is solely to hold data for each vertice so that it can be accessed later
public class VerticeClass 
{
    public bool active;    //the active state of the vertice
    public float value;    //the scalar-point value of the vertice

    //creates a new vertice class which takes in the active state, position, and scalar value
    public VerticeClass (bool _active, float _value) 
    {
        //sets the variables back to the scripts ones above so that they are saved
        active = _active;
        value = _value;
    }
}
//this script works similar to a dictionary would to hold a definition for each vertice. These custom classes are useful to hold data
//but can be used for a variety of other instances as well