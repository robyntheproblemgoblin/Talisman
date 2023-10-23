using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    GameManager m_game;

    [Header("FMOD Music Event References")]
    public FMODUnity.EventReference m_menuMusic;
    public FMODUnity.EventReference m_combatMusic;

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
    FMOD.Studio.EventInstance m_combatMusicInstance;

    [Header("Dialogue Sections")]
    public Dialogue m_intro;
    public Dialogue m_playerDeath;
    public List<Dialogue> m_cinematicDialogue;
    Dictionary<string, Dialogue> m_dialoguesDict;

    bool m_playLines = true;
    [HideInInspector]
    public bool m_stopInteractions = false;

    private void Start()
    {
        m_game = GameManager.Instance;
        m_game.m_audioManager = this;
        foreach (var dialogue in m_cinematicDialogue)
        {
            if (dialogue is DialogueObject)
            {
                if (!m_dialoguesDict.ContainsKey(dialogue.name))
                {
                    m_dialoguesDict.Add(dialogue.name, dialogue);
                }
            }
            else if (dialogue is DialogueSequence)
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
        if (m_stopInteractions)
        {
            m_game.m_player.m_stopInteracts = true;
        }
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
}
