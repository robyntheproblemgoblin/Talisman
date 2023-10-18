using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    GameManager m_game;
    
    [HideInInspector] public int m_musicIndex;
    
    Dictionary<string, List<DialogueObject>> m_dictionary;

    public DialogueObject m_intro;

    
    bool m_playLines = true;

    [Header("Menu Source")]
    public AudioSource m_menuSource;
    public List<AudioClip> m_menuClips;

    private void Start()
    {
        m_game = GameManager.Instance;
        m_game.m_audioManager = this;
        m_dictionary = new Dictionary<string, List<DialogueObject>>();
      /*  foreach (DialogueList dl in m_dialogues)
        {
            m_dictionary.Add(dl.m_section, dl.m_sequence);
        }*/
    }

    void PlayOneShot(string fmodEvent, Vector3 pos)
    {
        RuntimeManager.PlayOneShot("event:/" + fmodEvent, pos);
    }

    void PlayOneShotAttached(FMODUnity.EventReference fmodEvent, GameObject go)
    {
        RuntimeManager.PlayOneShotAttached(fmodEvent, go);
    }

    void EndFmodLoop(FMOD.Studio.EventInstance eventInstance)
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);        
    }

    void StartFmodLoop(FMOD.Studio.EventInstance eventInstance)
    {
        eventInstance.start();
    }

    public async UniTask PlayDialogueSequence(Dialogue reference)
    {
        /*List<DialogueObject> current = m_dictionary[reference];
        int index = 0;
        m_playLines = true;
        while (m_playLines)
        {
            m_game.m_menuManager.SetSubtitle(current[index].m_subtitle);
            PlayOneShotAttached(current[index].m_eventReference, m_game.m_player.gameObject);
            index++;
            if (index >= current.Count)
            {
                m_playLines = false;
            }
            else
            {
               
              //  await UniTask.WaitUntil(current[index -1].m_eventReference.);
            }
        }*/
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
        PlayOneShotAttached(Dobject.m_eventReference, m_game.m_player.gameObject);
    }
}
