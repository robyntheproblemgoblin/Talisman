using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    GameManager m_game;

    public FMODUnity.EventReference m_menuMusic;
    public FMODUnity.EventReference m_combatMusic;

    FMOD.Studio.EventInstance m_dialogueInstance;
    FMOD.Studio.EventInstance m_menuMusicInstance;
    FMOD.Studio.EventInstance m_combatMusicInstance;

    public DialogueObject m_intro;
    
    bool m_playLines = true;    

    private void Start()
    {
        m_game = GameManager.Instance;
        m_game.m_audioManager = this;        
    }

    void PlayOneShot(string fmodEvent, Vector3 pos)
    {
        RuntimeManager.PlayOneShot("event:/" + fmodEvent, pos);
    }

    void PlayOneShotAttachedDialogue(FMODUnity.EventReference fmodEvent, GameObject go)
    {
        FMOD.Studio.PLAYBACK_STATE current;
        m_dialogueInstance.getPlaybackState(out current);

        if (current == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {            
            m_dialogueInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);            
            m_dialogueInstance.getPlaybackState(out current);
            Debug.Log(current);
            m_dialogueInstance.clearHandle();
            m_dialogueInstance.release();
        }

        m_dialogueInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
        RuntimeManager.AttachInstanceToGameObject(m_dialogueInstance, go.transform);
        m_dialogueInstance.start();
    }

    void EndFmodLoop(FMOD.Studio.EventInstance eventInstance)
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);        
    }

    void StartFmodLoop(FMOD.Studio.EventInstance eventInstance)
    {
        eventInstance.start();
    }

    public async UniTask PlayDialogue(Dialogue reference)
    {
        if(reference is DialogueSequence)
        {
            PlayDialogueSequence((DialogueSequence)reference);
        }
        else if(reference is DialogueObject)
        {
            OneOffDialogue((DialogueObject)reference);
        }
    }

        public async UniTask PlayDialogueSequence(DialogueSequence reference)
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
        PlayOneShotAttachedDialogue(Dobject.m_eventReference, m_game.m_player.gameObject);
    }
}
