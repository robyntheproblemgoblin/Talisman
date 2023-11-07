using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SwordBlockVFXOffset : MonoBehaviour
{


    public float timeResetCount = 0f;


    public Material swordBlockShader;
    public ParticleSystem swordShellBlock;

    public void Start()
    {
        timeResetCount = 0f;
    }

    private void Update()
    {
        if(Input.GetKeyDown("g"))
        {
            resetTime();
        }
    }





    public void resetTime()
    {
        timeResetCount = Time.time;
        swordBlockShader.SetFloat("_TimeReset", timeResetCount);
        swordShellBlock.Play();
    }



}
