using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FMODUnity;
using static UnityEditor.Profiling.RawFrameDataView;

public class AudioManager : MonoBehaviour
{
    GameManager m_game;    

    public FMODUnity.EventReference m_menuMusic;
    public FMODUnity.EventReference m_combatMusic;
    
    FMOD.Studio.EventInstance m_dialogueInstance;
    FMOD.Studio.EventInstance m_menuMusicInstance;
    FMOD.Studio.EventInstance m_combatMusicInstance;

    [Header("Dialogue Sections")]
    public Dialogue m_intro;
    public Dialogue m_playerDeath;
    public List<Dialogue> m_cinematicDialogue;
    Dictionary<string, Dialogue> m_dialoguesDict;

    bool m_playLines = true;

    private void Start()
    {
        m_game = GameManager.Instance;
        m_game.m_audioManager = this;
        foreach (var dialogue in m_cinematicDialogue)
        { 
            if(dialogue is DialogueObject)
            {
                if(!m_dialoguesDict.ContainsKey(dialogue.name))
                {
                    m_dialoguesDict.Add(dialogue.name, dialogue);
                }
            }
            else if(dialogue is DialogueSequence)
            {
                DialogueSequence dialogueSequence = (DialogueSequence)dialogue;
                if (!m_dialoguesDict.ContainsKey(dialogueSequence.m_section))
                {
                    m_dialoguesDict.Add(dialogueSequence.m_section, dialogue);
                }
            }
        }
    }

    void PlayOneShot(string fmodEvent, Vector3 pos)
    {
        RuntimeManager.PlayOneShot("event:/" + fmodEvent, pos);
    }

    void PlayOneShotAttachedDialogue(DialogueObject fmodEvent, GameObject go)
    {
        FMOD.Studio.PLAYBACK_STATE current;
        m_dialogueInstance.getPlaybackState(out current);

        if (current == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            m_dialogueInstance.release();
            m_dialogueInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        m_dialogueInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent.m_eventReference);
        RuntimeManager.AttachInstanceToGameObject(m_dialogueInstance, go.transform);
        m_dialogueInstance.start();
        m_game.m_menuManager.SetSubtitle(fmodEvent.m_subtitle);
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
        if (reference is DialogueSequence)
        {
            PlayDialogueSequence((DialogueSequence)reference);
        }
        else if (reference is DialogueObject)
        {
            PlayOneShotAttachedDialogue((DialogueObject)reference, m_game.m_player.gameObject);
        }
    }

    public async UniTask PlayDialogueSequence(DialogueSequence reference)
    {
        int index = 0;
        m_playLines = true;

        while (m_playLines)
        {

            m_dialogueInstance = FMODUnity.RuntimeManager.CreateInstance(reference.m_sequence[index].m_eventReference);
            RuntimeManager.AttachInstanceToGameObject(m_dialogueInstance, m_game.m_player.gameObject.transform);
            m_dialogueInstance.start();            

            FMOD.Studio.PLAYBACK_STATE current;
            m_dialogueInstance.getPlaybackState(out current);

            m_game.m_menuManager.SetSubtitle(reference.m_sequence[index].m_subtitle);            
            index++;
            if (index >= reference.m_sequence.Count)
            {
                m_playLines = false;
            }
            else
            {
                while (current != FMOD.Studio.PLAYBACK_STATE.STOPPED)
                {
                    m_dialogueInstance.getPlaybackState(out current);
                    await UniTask.Yield();
                }
            }
        }
    }

    public void PlayIntroDialogue()
    {
        PlayDialogue(m_intro);        
    }    

    public void PlayDeathDialogue()
    {
        PlayDialogue(m_playerDeath);
    }
}
