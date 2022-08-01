using System.Collections;
using UnityEngine;
using UnityEngine.UI; //imports the required UI components

public class CameraScript : MonoBehaviour
{
    public GameObject brushCursor; //the object to use as the visible brush

    //preferred keys to use
    public KeyCode forward = KeyCode.W;
    public KeyCode back = KeyCode.S;
    public KeyCode right = KeyCode.D;
    public KeyCode left = KeyCode.A;
    public KeyCode focus = KeyCode.F;
    public KeyCode increaseBrushSize = KeyCode.Equals;
    public KeyCode decreaseBrushSize = KeyCode.Minus;

    public float moveSpeed; //how fast the movement of the camera is, this will be editable via the in game menu as larger worlds 
                            //my require a faster movement and visa versa
    
    public float scrollSpeed = 0.7f;    //how fast the camera zooms in/out
    public bool invertMouse = false;    //invert the mouse look controls?
    [Range(50, 450)]//provides a range for the sensitivity of the mouse
    public float mouseSensitivity = 100f;//how sensitive the mouse look should be
    public float clampAngle = 80.0f; //maximum/minimum clamp angle, 80 was found to be the best value
    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis

    public float panSpeed = 2;  //how fast should the camera pan
    private Vector3 dragOrigin; //this is used later on in the code only to store data

    void Start()
    {
        Application.targetFrameRate = 150; //cap the framerate 

        Cursor.lockState = CursorLockMode.Confined;

        //gets/sets the components needed for the mouse look to the transform's current values, this enables them to be edited later
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    private void LateUpdate()
    {
        //two input axes for the mouse position, this uses the inbuild input axes
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        //checks when rightclicked, this allows the user to look around the scene while he is holding right click
        if (Input.GetMouseButton(1))
        {
            //invert the mouse controls, this will be included in the in-game settings and can be changed per preference later
            if (invertMouse)
            {
                //by multiplying by time.delta time, we get a steadier movement as it is calculated off the pc's clock rather than the
                //frames per second
                rotY -= mouseX * mouseSensitivity * Time.deltaTime;
                rotX -= mouseY * mouseSensitivity * Time.deltaTime;
            }
            else
            {
                rotY += mouseX * mouseSensitivity * Time.deltaTime;
                rotX += mouseY * mouseSensitivity * Time.deltaTime;
            }

            //clamps the x rotation between the maximum turn angles on both sides, stopping it from turning too much
            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
            //turns this data into a rotation quaternion and a vector3 for the rotation on all axes
            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            //sets teh camera's rotation to the new rotation
            transform.rotation = localRotation;
        }

        //d is the value of the scrollwheel axis
        var d = Input.GetAxis("Mouse ScrollWheel");
        //checks when the wheel is being turned by checking its value whether its above or below zero
        //transform.forward allows for camera relative movment, which is precisely what I need for a fps controller
        if (d > 0f)
        {
            //scrolling up
            //move the camera forward, zooming IN to the scene
            transform.position += transform.forward * scrollSpeed;
        }
        else if (d < 0f)
        {
            //scrolling out
            //move the camera back, zooming OUT from the scene
            transform.position -= transform.forward * scrollSpeed;
        }

        //checks each key
        //by checking the keys off a keycode variable, the user will be able to edit their movement controls via the settings to find
        //the controls most comfortable to them, between arrow keyso, IJKL or WASD - or any other preferred key combination
        if (Input.GetKey(forward))
        {
            //move forward relative to the camera by the movespeed;
            transform.position += transform.forward * moveSpeed;
        }
        if (Input.GetKey(back))
        {
            //move back relative to the camera by the movespeed;
            transform.position -= transform.forward * moveSpeed;
        }
        if (Input.GetKey(left))
        {
            //move left relative to the camera by the movespeed;
            transform.position -= transform.right * moveSpeed;
        }
        if (Input.GetKey(right))
        {
            //move right relative to the camera by the movespeed;
            transform.position += transform.right * moveSpeed;
        }

        //when the middle mouse button is first pressed
        if (Input.GetMouseButtonDown(2))
        {
            //set the drag origin
            dragOrigin = Input.mousePosition;
            return;
        }

        //when the middle mouse button is held down
        if (Input.GetMouseButton(2))
        {
            //creates a new vector 3 to hold the data of the mouse position of the screen relative to the original mouse position origin
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            //which axes should the camera be able to move on
            Vector3 move = new Vector3(Mathf.Clamp(pos.x, -1, 1) * panSpeed, Mathf.Clamp(pos.y, -1, 1) * panSpeed, 0);

            //move the camera relative to the objects local space
            transform.Translate(-move, Space.Self);
        }

        //raycasting
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //creates a ray which will return data, shooting it at an infinite length,
                                                                     //from the mouse position - converted into a ray
        RaycastHit hit; //holds the hit data for the raycast including where and what it hit
        if (Physics.Raycast(ray, out hit)) //did we hit something?
        {
            brushCursor.SetActive(true);//set the brush to be visible
            brushCursor.transform.position = hit.point; //sets the brush position to the point where it hit
            if (Input.GetKeyDown(focus)) //this is to focus the camera on the brush, it will be used as a shortcut in the editor
            {
                Cursor.lockState = CursorLockMode.Locked;//reposition the cursor to the centre of the screen
                float distToHitPoint = Vector3.Distance(hit.point, transform.position); //find the distance between the hit point and the current camera position
                distToHitPoint -= 1f; //take away the camera's bounds
                //Camera.main.transform.LookAt(brushCursor.transform);//look where the mouse is pointing
                //move the camera forward towards the brush object, taking away the objects bounds to fit in the whole brush
                transform.position += transform.forward * (distToHitPoint - (brushCursor.transform.localScale.x + 20));
            }
            else 
            {
                Cursor.lockState = CursorLockMode.None;//allow the mouse to move freely again after repositioning it
            }
        }
        else 
        {
            brushCursor.SetActive(false);//set the brush to be invisible
        }
    }

    //NAVIGATION CONTROLS
    //set the values relative to the UI in the game scene
    public void SetSensitivity(Slider slider) 
    {
        mouseSensitivity = slider.value;
    }
    public void SetZoomSpeed(Slider slider)
    {
        scrollSpeed = slider.value;
    }
    public void SetMoveSpeed(Slider slider)
    {
        moveSpeed = slider.value;
    }
    public void SetDragSpeed(Slider slider)
    {
        panSpeed = slider.value;
    }
    public void SetMouseInvert(Toggle toggle) 
    {
        invertMouse = toggle.isOn;
    }
}
