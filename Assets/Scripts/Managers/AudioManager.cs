using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Indexes
    int m_initialLinesIndex = 0;
    int m_circlePuzzleIndex = 0;
    int m_rotatingPuzzleIndex = 0;
    int m_finalLinesIndex = 0;
    [HideInInspector] public int m_musicIndex;

    [Header("Voice Lines")]
    public List<AudioClip> m_voiceLinesInitial;
    public List<AudioClip> m_voiceLinesPuzzleLeft;
    public List<AudioClip> m_voiceLinesPuzzleRight;
    public List<AudioClip> m_voiceLinesEnd;

    [Header("Music")]
    public List<AudioClip> m_music;

    [Header("SFX")]
    public List<AudioClip> m_soundEffects;

    //Source on other things? dragged into this?
    public AudioSource m_source;

    #region Singleton
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayInitialVoiceLines()
    {
        m_source.clip = m_voiceLinesInitial[m_initialLinesIndex];
        m_source.Play();
        m_initialLinesIndex++;
    }

    public void PlayCirclePuzzleClip()
    {
        m_source.clip = m_voiceLinesInitial[m_circlePuzzleIndex];
        m_source.Play();
        m_circlePuzzleIndex++;
    }
    public void PlayRotatingPuzzleClip()
    {
        m_source.clip = m_voiceLinesInitial[m_rotatingPuzzleIndex];
        m_source.Play();
        m_rotatingPuzzleIndex++;
    }
    public void PlayFinalVoiceLines()
    {
        m_source.clip = m_voiceLinesInitial[m_finalLinesIndex];
        m_source.Play();
        m_finalLinesIndex++;
    }
    public void PlayMusic()
    {
        m_source.clip = m_music[m_musicIndex];
        m_source.Play();
    }

    // From other script... maybe? bad code, just a thought process.
    /*public void OnTriggerEnter(Collider other)
    {
        AudioManager.m_musicIndex = 1;
        gameObject.SetActive(false);
    }*/
    
}
