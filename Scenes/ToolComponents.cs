using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ToolComponents : MonoBehaviour
{
    public enum Tool //what types of tools the user can use
    {
        paintTerrain,
        buildTerrain
    }
    public Tool tool; //what tool they are currently using

    public Image brushColorView; //the image which displays the current color
    public Color brushCol; //the actual color of the brush

    public GameObject brush; //the brush itself as a gameobject
    public Slider brushSizeSlider; //the ui holding the data for the size of the brush
    
    private void Start()
    {
       
        //set all the values
        brush.transform.localScale = new Vector3(brushSizeSlider.value, brushSizeSlider.value, brushSizeSlider.value);
    }

    private void Update()
    {
        if (Input.GetKey(GetComponent<CameraScript>().increaseBrushSize)) //allows for shortcuts to increase/decrease the brush size
        {
            brushSizeSlider.value += 0.25f;
        }
        else if (Input.GetKey(GetComponent<CameraScript>().decreaseBrushSize))
        {
            brushSizeSlider.value -= 0.25f;
        }

        brush.transform.localScale = new Vector3(brushSizeSlider.value, brushSizeSlider.value, brushSizeSlider.value); //update teh brush size
    }

    //TOOLS CONTROLS
    public void SetTool(Dropdown dropdown) 
    {
        if (dropdown.value == 0)
            tool = Tool.buildTerrain;
        else if (dropdown.value == 1)
            tool = Tool.paintTerrain;
    }

    public void SetBrushSize(Slider slider)
    {
        brush.transform.localScale = new Vector3(slider.value, slider.value, slider.value);//set the scale of the brush object
    }

    public void ColorRed(Slider slider) //controls the red variable of a rgb color
    {
        brushColorView.color = new Color(slider.value, brushColorView.color.g, brushColorView.color.b);
    }
    public void ColorGreen(Slider slider) //controls the red variable of a rgb color
    {
        brushColorView.color = new Color(brushColorView.color.r, slider.value, brushColorView.color.b);
    }
    public void ColorBlue(Slider slider) //controls the red variable of a rgb color
    {
        brushColorView.color = new Color(brushColorView.color.r, brushColorView.color.g, slider.value);
    }
}
