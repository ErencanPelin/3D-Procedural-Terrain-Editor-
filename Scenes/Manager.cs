using System.Collections;
using UnityEngine;
using UnityEngine.UI; //import the ui methods
using UnityEngine.SceneManagement; //import the scene-controller methods

public class Manager : MonoBehaviour
{
    public Slider heightmapDetailSlider;
    public Slider heightmapAmplitudeSlider;

    public Text description; //the on screen text used to describe the generation methods

    public WorldGenSettings worldGenSettings; //write all the data into this file so that the editor can read it on generation

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject); //keep this object throughout all scenes

        //set defaults
        worldGenSettings.seed = 0;
        worldGenSettings.useEdgeInterpolation = true;
        worldGenSettings.sphericalRadius = 12;
        worldGenSettings.groundHeight = 3;
        worldGenSettings.chunkSize = 40;
        worldGenSettings.size = 100;
        worldGenSettings.noiseFrequency = 0.03f;
        worldGenSettings.terrainAmplitude = 10;
        worldGenSettings.heightmapLayers = 8;
        worldGenSettings.surfaceLevel = 0;
        worldGenSettings.generationType = WorldGenSettings.GenerationType.heightmap;

        description.text = "Generates a mountainess terrain.";
    }

    public void SetSeed(Text seedTXT) //set the seed the user gave into the settings
    {
        worldGenSettings.seed = int.Parse(seedTXT.text); //turn the input field string into an int and parse it into the gen settings
    }

    public void SetHeightMapDetail(Slider detailSlider) //set the heightmpa detail in the settings
    {
        worldGenSettings.heightmapLayers = Mathf.RoundToInt(detailSlider.value);
        detailSlider.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Height Map Detail: " + detailSlider.value.ToString() + "x"; //set the text to show the value of the slider
    }

    public void SetAmplitude(Slider ampSlider) //set the heightmpa detail in the settings
    {
        worldGenSettings.terrainAmplitude = Mathf.RoundToInt(ampSlider.value);
        ampSlider.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Terrain Amplitude " + ampSlider.value.ToString(); //set the text to show the value of the slider
    }

    public void SetWorldSize(Slider sizeSlider) //sets the world size
    {
        worldGenSettings.size = Mathf.RoundToInt(sizeSlider.value);
        //show this value on screen like a tooltip
        sizeSlider.gameObject.transform.GetChild(0).GetComponent<Text>().text = "World Size: " + sizeSlider.value.ToString() + "x" + sizeSlider.value.ToString();
    }
    public void SetNoiseFrequency(Slider freqSlider) //sets the world size
    {
        worldGenSettings.noiseFrequency = Mathf.RoundToInt(freqSlider.value) / 100f;
        //show this value on screen like a tooltip
        freqSlider.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Noise Frequency: " + freqSlider.value.ToString() + "%";
    }

    public void SetSurfaceSmoothing(Toggle toggle) //use edge interpolation?
    {
        worldGenSettings.useEdgeInterpolation = toggle.isOn;
    }

    public void ChooseSettings(Dropdown dropdown) 
    {
        //sets the generation type according to the drop down menu
        if (dropdown.value == 0)
        {
            worldGenSettings.generationType = WorldGenSettings.GenerationType.heightmap;
            description.text = "Generates a mountainess terrain."; //provides some information reguarding the generation type on screen
            heightmapDetailSlider.gameObject.SetActive(true); //in/activates the needed UI for each generation method to clean up the useless UI
            heightmapAmplitudeSlider.gameObject.SetActive(true);
        }
        else if (dropdown.value == 1)
        {
            worldGenSettings.generationType = WorldGenSettings.GenerationType.playground;
            description.text = "Generates a messy world using 3D perlin noise.";
            heightmapDetailSlider.gameObject.SetActive(false);
            heightmapAmplitudeSlider.gameObject.SetActive(false);
        }
        else if (dropdown.value == 2)
        {
            worldGenSettings.generationType = WorldGenSettings.GenerationType.spherical;
            description.text = "Generates a spherical world similar to a planet";
            heightmapDetailSlider.gameObject.SetActive(false);
            heightmapAmplitudeSlider.gameObject.SetActive(false);
        }
    }

    public void Generate() 
    {
        SceneManager.LoadScene("Editor");
    }
}
