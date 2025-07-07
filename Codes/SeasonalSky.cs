using UnityEngine;

public class SeasonalSky : MonoBehaviour
{
    public Material skyboxMaterial;  // The skybox material to set for the scene

    void Start()
    {
        // Assign the specified skybox material to the scene's RenderSettings at start
        RenderSettings.skybox = skyboxMaterial;
    }
}
