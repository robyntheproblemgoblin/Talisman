using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Talisman/AudioSubtitle", fileName = "New Audio Subtitle")]
public class AudioSubtitle : ScriptableObject
{
    public AudioClip m_clip;
    public string m_subtitle;
}
