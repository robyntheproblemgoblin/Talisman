using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class ToonShaderLightSettings : MonoBehaviour
{
	private Light mainLight;
	Light[] otherLights;

	void OnEnable()
	{		
		mainLight = GetComponent<Light>();
		otherLights = FindObjectsOfType<Light>();			
		otherLights.ToList().Remove(mainLight);
			
	}
	
	void Update ()
	{
		Shader.SetGlobalVector("_ToonLightDirection", -transform.forward);
		Shader.SetGlobalColor("_ToonLightColor", mainLight.color);
        Shader.SetGlobalFloat("_ToonLightIntensity", mainLight.intensity);		
	}
}