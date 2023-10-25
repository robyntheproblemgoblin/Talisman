using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FMODUnity;
using System;

public class AudioManager : MonoBehaviour
{
    GameManager m_game;

    FMOD.Studio.Bus Music;
    FMOD.Studio.Bus SFX;
    FMOD.Studio.Bus Dialogue;
    FMOD.Studio.Bus Master;
    public float MusicVolume = 0.5f;
    public float SFXVolume = 0.5f;
    public float DialogueVolume = 0.5f;
    public float MasterVolume = 1f;

    [Header("FMOD Music Event References")]
    public FMODUnity.EventReference m_menuMusic;    

    [Space(5), Header("Murray Puzzle Dialogue References")]
    public Dialogue m_murrayHalfSolve;
    public Dialogue m_murrayFullSolve;
    public Dialogue m_murrayFirstFail;
    public Dialogue m_murraySecondFail;
    int m_murraySolveInstances = 0;
    int m_murrayFailInstances = 0;
    
    [Space(5), Header("Rotation Puzzle Dialogue References")]    
    public Dialogue m_rotationFirstFail;
    public Dialogue m_rotationSecondFail;    
    int m_rotationFailInstances = 0;

    [Space(5), Header("Sword Room")]
    public Dialogue m_swordRoomEnd;

    FMOD.Studio.EventInstance m_dialogueInstance;
    FMOD.Studio.EventInstance m_menuMusicInstance;    

    [Header("Dialogue Sections")]
    public Dialogue m_intro;
    public Dialogue m_playerDeath;

    public Dialogue m_talismanGrab;
    public Dialogue m_teleport;
    public Dialogue m_ending;
    int m_cinematics = 0;
    bool m_nextCinematic = false;

    bool m_playLines = true;
    [HideInInspector]
    public bool m_stopInteractions = false;

    private void Awake()
    {
        Music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        SFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");
        Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        Dialogue = FMODUnity.RuntimeManager.GetBus("bus:/Master/Dialogue");
    }

    void Start()
    {
        m_game = GameManager.Instance;
        m_game.m_audioManager = this;
        
        m_menuMusicInstance= FMODUnity.RuntimeManager.CreateInstance(m_menuMusic);
        StartFmodLoop(m_menuMusicInstance);        
    }

    void Update()
    {
        Music.setVolume(MusicVolume);
        SFX.setVolume(SFXVolume);
        Master.setVolume(MasterVolume);
        Dialogue.setVolume(DialogueVolume);       
    }

    public void MasterVolumeLevel(float newMasterVolume)
    {
        MasterVolume = newMasterVolume;
    }

    public void MusicVolumeLevel(float newMusicVolume)
    {
        MusicVolume = newMusicVolume;
    }

    public void SFXVolumeLevel(float newSFXVolume)
    {
        SFXVolume = newSFXVolume;

    }
    public void DialogueVolumeLevel(float newDialogueVolume)
    {
        SFXVolume = newDialogueVolume;
    }

    public void PlayOneShot(EventReference fmodEvent, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(fmodEvent, pos);
    }

    void EndFmodLoop(FMOD.Studio.EventInstance eventInstance)
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    void StartFmodLoop(FMOD.Studio.EventInstance eventInstance)
    {
        eventInstance.start();
    }

    public void PlayDialogue(Dialogue reference)
    {
        if (reference is DialogueSequence)
        {
            PlayDialogueSequence((DialogueSequence)reference).Forget();
        }
        else if (reference is DialogueObject)
        {
            PlayOneShotAttachedDialogue((DialogueObject)reference, m_game.m_player.gameObject);
        }
    }

    public async UniTask PlayDialogueSequence(DialogueSequence reference)
    {
        FMOD.Studio.PLAYBACK_STATE current;
        m_dialogueInstance.getPlaybackState(out current);

        if (current == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            m_dialogueInstance.release();
            m_dialogueInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        int index = 0;
        m_playLines = true;
        if (m_stopInteractions)
        {
            m_game.m_player.m_stopInteracts = true;
        }
        while (m_playLines)
        {
            m_dialogueInstance = FMODUnity.RuntimeManager.CreateInstance(reference.m_sequence[index].m_eventReference);
            RuntimeManager.AttachInstanceToGameObject(m_dialogueInstance, m_game.m_player.gameObject.transform);
            m_dialogueInstance.start();
            m_dialogueInstance.release();
            m_dialogueInstance.getPlaybackState(out current);

            m_game.m_menuManager.SetSubtitle(reference.m_sequence[index].m_subtitle);
            index++;
            if (index >= reference.m_sequence.Count)
            {
            Debug.Log(index);
                m_playLines = false;
                StopInteractions(m_dialogueInstance).Forget();
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
        m_nextCinematic = false;
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

        if (m_stopInteractions)
        {
            StopInteractions(m_dialogueInstance).Forget();
        }
    }

    async UniTask StopInteractions(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE current;
        instance.getPlaybackState(out current);
        m_game.m_player.m_stopInteracts = true;
        while (current != FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            instance.getPlaybackState(out current);
            await UniTask.Yield();
        }
        m_game.m_player.m_stopInteracts = false;
        m_stopInteractions = false;
    }

    public void PlayIntroDialogue()
    {
        PlayDialogue(m_intro);
    }

    public void PlayDeathDialogue()
    {
        PlayDialogue(m_playerDeath);
    }

    public void PlayMurrayPuzzleDialogue()
    {
        if (m_murraySolveInstances == 0)
        {
            m_murraySolveInstances++;
            PlayDialogue(m_murrayHalfSolve);
        }
        else if (m_murraySolveInstances == 1)
        {
            m_murraySolveInstances++;
            PlayDialogue(m_murrayFullSolve);
        }
    }

    public void PlayMurrayPuzzleRoom()
    {
        FMOD.Studio.PLAYBACK_STATE current;
        m_dialogueInstance.getPlaybackState(out current);

        if (m_murrayFailInstances == 0)
        {
            m_murrayFailInstances++;
            PlayDialogue(m_murrayFirstFail);
            m_dialogueInstance.getPlaybackState(out current);
        }
        else if (m_murrayFailInstances == 1 && current == FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            m_murrayFailInstances++;
            PlayDialogue(m_murraySecondFail);
        }
    }

    public void PlayRoationPuzzleRoom()
    {
        if (m_rotationFailInstances == 0)
        {
            m_rotationFailInstances++;
            PlayDialogue(m_rotationFirstFail);
        }
        else if (m_rotationFailInstances == 1)
        {
            m_rotationFailInstances++;
            PlayDialogue(m_rotationSecondFail);
        }
    }

    public void PlaySwordRoomEndDialogue()
    {
        m_stopInteractions = true;
        PlayDialogue(m_swordRoomEnd);
    }

    public async UniTask PlayCinematic()
    {
        m_nextCinematic = true;
        if(m_cinematics > 2)
        {
            return;
        }
        else if(m_cinematics == 0)
        {
            PlayDialogue(m_talismanGrab);
            while(m_nextCinematic)
            {
                await UniTask.Yield();
            }
            m_game.SecondCinematic().Forget();
        }
        else if(m_cinematics == 1)
        {
            PlayDialogue(m_teleport);
        }
        else if(m_cinematics == 2)
        {
            PlayDialogue(m_ending);
            while (m_nextCinematic)
            {
                await UniTask.Yield();
            }
            m_game.LastCinematic();
        }
        m_cinematics++;
    }    
}
