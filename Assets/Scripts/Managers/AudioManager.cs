using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    GameManager m_game;
    
    [HideInInspector] public int m_musicIndex;

    public List<DialogueList> m_dialogues;
    Dictionary<string, List<DialogueObject>> m_dictionary;

    public DialogueObject m_intro;

    [Header("Cinematic Sources")]
    public AudioSource[] m_playerSources;
    [HideInInspector]
    public double m_initDelayLines = 0.01d;
    int m_playerSourceToggle;
    double m_nextStartTime;
    bool m_playLines = true;

    [Header("Menu Source")]
    public AudioSource m_menuSource;
    public List<AudioClip> m_menuClips;

    private void Start()
    {
        m_game = GameManager.Instance;
        m_game.m_audioManager = this;
        m_dictionary = new Dictionary<string, List<DialogueObject>>();
        foreach (DialogueList dl in m_dialogues)
        {
            m_dictionary.Add(dl.m_section, dl.m_sequence);
        }
    }

    void PlayOneShot(string fmodEvent, Vector3 pos)
    {
        RuntimeManager.PlayOneShot("event:/" + fmodEvent, pos);
    }

    void PlayOneShotAttached(string fmodEvent, GameObject go)
    {
        RuntimeManager.PlayOneShotAttached("event:/" + fmodEvent, go);
    }

    void EndFmodLoop(FMOD.Studio.EventInstance eventInstance)
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    void StartFmodLoop(FMOD.Studio.EventInstance eventInstance)
    {
        eventInstance.start();
    }

    public async UniTask PlayVoiceSequence(string reference)
    {
        List<DialogueObject> current = m_dictionary[reference];
        int index = 0;
        m_playLines = true;
        while (m_playLines)
        {
            /*AudioClip clipToPlay = current[index].m_clip;
            // Loads the next Clip to play and schedules when it will start
            m_playerSources[0].clip = clipToPlay;
            m_playerSources[0].Play();            
            */index++;
            if (index >= current.Count)
            {
                m_playLines = false;
            }
            else
            {
                await UniTask.WaitUntil(() => m_playerSources[0].isPlaying == false);
            }
        }
    }

    public async UniTask PlayIntroEffect()
    {
       /* if (m_menuClips.Count > 0)
        {
            m_menuSource.clip = m_menuClips[0];
            m_menuSource.Play();
        }
        while (m_menuSource.isPlaying)
        {
            await UniTask.Yield();
        }*/
        OneOffDialogue(m_intro);
    }

    public void OneOffDialogue(DialogueObject Dobject)
    {        
        m_game.m_menuManager.SetSubtitle(Dobject.m_subtitle);
       // PlayOneShotAttached(Dobject.m_FMODEvent, m_game.m_player.gameObject);
    }
}
