using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoop : MonoBehaviour
{
    public FMODUnity.EventReference m_audio;
    FMOD.Studio.EventInstance m_audioInstance;
    void Start()
    {
        m_audioInstance = RuntimeManager.CreateInstance(m_audio);
        RuntimeManager.AttachInstanceToGameObject(m_audioInstance, gameObject.transform);
        m_audioInstance.start();
    }

    private void OnDestroy()
    {
        m_audioInstance.release();
        m_audioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
