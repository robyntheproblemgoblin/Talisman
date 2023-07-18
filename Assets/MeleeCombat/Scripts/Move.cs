using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[Serializable]
public class Move
{
    [HideInInspector] public Animator m_animator;
    public AnimationClip m_moveAnimation;
    public List<HitBox> m_moveHitBoxes;
    public string m_moveName;
    public bool m_isBeingUsed;


    private int m_totalAnimationFrames;
    private int m_currentAnimationFrame = 0;

    public int GetCurrentAnimationFrame()
    {
        return m_currentAnimationFrame;
    }
    public void IncrementAnimationFrame()
    {
        m_currentAnimationFrame++;
    }
    public void ResetCurrentFrame()
    {
        m_currentAnimationFrame = 0;
    }

    public int GetTotalAnimationFrames()
    {
        return m_totalAnimationFrames;
    }

    public float GetMoveFrameRate()
    {
        return m_moveAnimation.frameRate;
    }


    public void SetTotalFrames()
    {
        m_totalAnimationFrames = (int)(m_moveAnimation.length * m_moveAnimation.frameRate);
    }


    public void ConstructHitBoxesPerFrame()
    {
        foreach (HitBox hB in m_moveHitBoxes)
        {
            if (hB.m_startFrame == m_currentAnimationFrame)
            {
                hB.ConstructHitbox();
            }
        }
    }
    public void DestroyHitBoxesPerFrame()
    {
        foreach (HitBox hB in m_moveHitBoxes)
        {
            if (hB.m_endFrame == m_currentAnimationFrame)
            {
                hB.DestroyHitBoxObject();
            }
        }
    }

    public void StartMove()
    {
        m_animator.Play(m_moveName);
    }
    public void StopMove()
    {
        m_animator.Play("No Motion");
    }


    public bool IsMoveInAnimator()
    {
        List<AnimatorState> animStates = new List<AnimatorState>();
        AnimatorController animatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(m_animator.runtimeAnimatorController));
        foreach (AnimatorControllerLayer aCL in animatorController.layers)
        {
            ChildAnimatorState[] childAnimStates = aCL.stateMachine.states;
            foreach (ChildAnimatorState cAS in childAnimStates)
            {
                animStates.Add(cAS.state);
            }
        }

        foreach (AnimatorState aS in animStates)
        {
            if (aS.name == m_moveName)
            {
                return true;
            }
        }
        return false;
    }

    public void AddMoveToAnimator()
    {
        AnimatorController animatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(m_animator.runtimeAnimatorController));
        animatorController.AddMotion(m_moveAnimation, 0);
    }
}