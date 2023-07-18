using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CombatItem : MonoBehaviour
{
    //the combat items stuff itself
    [SerializeField] private Animator m_animator;
    [SerializeField] public List<Move> m_moves;
    [SerializeField] public List<HurtBox> m_hurtBoxes;
    [SerializeField] public bool m_debugHurtBoxes;
    [SerializeField] public bool m_debugHitBoxes;
    [SerializeField] private bool m_debugHurtBoxesLastFrame;
    [SerializeField] private bool m_debugHitBoxesLastFrame;

    private bool m_hitStopDone = false;
    private bool m_knockbackDone = false;


    private void OnEnable()
    {
        m_animator = GetComponent<Animator>();
        if (m_moves == null)
        {
            m_moves = new List<Move>();
        }
        if (m_hurtBoxes == null)
        {
            m_hurtBoxes = new List<HurtBox>();
        }
        m_debugHitBoxesLastFrame = m_debugHitBoxes;
        m_debugHurtBoxesLastFrame = m_debugHurtBoxes;
        for (int i = 0; i < 31; i++)
        {
            if (LayerMask.NameToLayer("Melee") != i)
            {
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Melee"), i);
            }
        }
    }

    private void Update()
    {
        ResolveCollisions();
    }

    public List<HitBox> CheckAllCollisions()
    {
        List<HitBox> collidingList = new List<HitBox>();
        foreach (Move m in m_moves)
        {
            foreach (HitBox hB in m.m_moveHitBoxes)
            {
                if (hB.m_isColliding)
                {
                    collidingList.Add(hB);
                }
            }
        }
        return collidingList;
    }

    public void DoHitStop(HitBox hitBox)
    {
        if (hitBox.m_automaticHitStop)
        {
            float stopTime = (float)((hitBox.m_endFrame - hitBox.m_startFrame) / hitBox.m_endFrame) * hitBox.m_hitStopMultiplier;
            StartCoroutine(HitStopForSeconds(stopTime));
        }
        else
        {
            StartCoroutine(HitStopForSeconds(hitBox.m_hitStopLength));
        }
    }

    private IEnumerator HitStopForSeconds(float time)
    {
        m_hitStopDone = false;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
        m_hitStopDone = true;
    }

    public void DoKnockBack(HitBox hitBox)
    {

        HurtBox receivingHurtBox = hitBox.m_collidingHurtBox;

        //conditional displacement angle based on if it should be automatically calculated
        Vector3 displacementAngle = hitBox.m_automaticKnockbackAngle ?
            Vector3.Normalize(receivingHurtBox.m_hurtBoxObject.transform.position - hitBox.m_hitBoxObject.transform.position) :
            new Vector3(Mathf.Sin(hitBox.m_knockbackAngle.x), Mathf.Sin(hitBox.m_knockbackAngle.y), Mathf.Sin(hitBox.m_knockbackAngle.z));

        Vector3 goal = displacementAngle * hitBox.m_knockbackDistance;

        if (receivingHurtBox.m_owner.GetComponent<Rigidbody>() != null)
        {
            Rigidbody receivingBody = receivingHurtBox.m_owner.GetComponent<Rigidbody>();
            // physics knockback
            StartCoroutine(KnockBackWithForce(receivingBody, hitBox.m_knockbackTime, goal));
        }
        else
        {
            // raw translate
            StartCoroutine(KnockBackRaw(receivingHurtBox.m_owner.transform, hitBox.m_knockbackTime, goal, hitBox));

        }

    }

    //not working as intended way too much force being added
    private IEnumerator KnockBackWithForce(Rigidbody rB, float totalTime, Vector3 goal)
    {
        m_knockbackDone = false;
        if (totalTime == 0)
        {
            rB.transform.Translate(goal);
            m_knockbackDone = true;
            yield break;
        }

        float timeIncremement = 0;
        rB.AddForce(2 * goal / totalTime, ForceMode.VelocityChange);
        while (timeIncremement < totalTime)
        {
            rB.AddForce(-2 * goal / totalTime / totalTime, ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
            timeIncremement += Time.fixedDeltaTime;
        }
        m_knockbackDone = true;
    }

    private IEnumerator KnockBackRaw(Transform obj, float totalTime, Vector3 goal, HitBox hitBox)
    {
        m_knockbackDone = false;
        float timeIncremement = 0;
        Vector3 objOriginalPos = obj.position;
        if (totalTime == 0)
        {
            obj.Translate(goal);
            yield return null;
        }
        while (timeIncremement < totalTime)
        {
            obj.position = Vector3.Lerp(objOriginalPos, objOriginalPos + goal, timeIncremement / totalTime);
            yield return 0;
            timeIncremement += Time.deltaTime;
        }
        m_knockbackDone = true;
    }

    public float GetDamage(HitBox hitBox)
    {
        if (hitBox.m_automaticDamageCalculation)
        {
            float invFrameLength = 1 / (float)(hitBox.m_endFrame - hitBox.m_startFrame);
            invFrameLength += 1;
            return hitBox.m_endFrame * invFrameLength;
        }
        return hitBox.m_damage;
    }
    public IEnumerator HitBoxStandardSolve(HitBox hB)
    {
        //stops hitbox from being solved twice
        hB.m_isColliding = false;
        DoHitStop(hB);
        yield return new WaitWhile(GetHitStopDone);
        DoKnockBack(hB);
        yield return new WaitWhile(GetKnockBackDone);

    }
    private bool GetHitStopDone()
    {
        return !m_hitStopDone;
    }
    private bool GetKnockBackDone()
    {
        return !m_knockbackDone;
    }

    virtual public void ResolveCollisions()
    {
        List<HitBox> collidingBoxes = CheckAllCollisions();
        foreach (HitBox hB in collidingBoxes)
        {
            StartCoroutine(HitBoxStandardSolve(hB));
        }
    }


    public void AddHurtBoxBox()
    {
        HurtBox newHurtBox = new HurtBox(HurtBox.Shape.Box, transform);
        newHurtBox.NameHurtBoxObject($"Hurt Box {m_hurtBoxes.Count + 1}: Box");

        m_hurtBoxes.Add(newHurtBox);
    }
    public void AddHurtBoxSphere()
    {
        HurtBox newHurtBox = new HurtBox(HurtBox.Shape.Sphere, transform);
        newHurtBox.NameHurtBoxObject($"Hurt Box {m_hurtBoxes.Count + 1}: Sphere");

        m_hurtBoxes.Add(newHurtBox);
    }
    public void AddHurtBoxCapsule()
    {
        HurtBox newHurtBox = new HurtBox(HurtBox.Shape.Capsule, transform);
        newHurtBox.NameHurtBoxObject($"Hurt Box {m_hurtBoxes.Count + 1}: Capsule");

        m_hurtBoxes.Add(newHurtBox);
    }

    public void RemoveHurtBox()
    {
        if (m_hurtBoxes.Count != 0)
        {
            m_hurtBoxes[m_hurtBoxes.Count - 1].DestroyHurtBox();
            m_hurtBoxes.RemoveAt(m_hurtBoxes.Count - 1);
        }
    }
    public void ClearHurtBoxes()
    {
        foreach (HurtBox hB in m_hurtBoxes)
        {
            hB.DestroyHurtBox();
        }
        m_hurtBoxes.Clear();
    }

    public void UseMove(int moveLocation)
    {
        StartCoroutine(UseMoveCoroutine(moveLocation));
    }

    //feels like it should be inside of move but coroutines can only be called on monobehaviours gross
    private IEnumerator UseMoveCoroutine(int moveLocation)
    {
        float secondsToWait = 1 / m_moves[moveLocation].GetMoveFrameRate();
        if (m_moves[moveLocation].GetCurrentAnimationFrame() == 0)
        {
            m_moves[moveLocation].StartMove();
        }

        while (m_moves[moveLocation].GetCurrentAnimationFrame() <= m_moves[moveLocation].GetTotalAnimationFrames())
        {
            m_moves[moveLocation].ConstructHitBoxesPerFrame();
            m_moves[moveLocation].DestroyHitBoxesPerFrame();

            //waits 1 animation frame, not 1 in game frame
            yield return new WaitForSeconds(secondsToWait);
            //move forward one frame for next loop
            m_moves[moveLocation].IncrementAnimationFrame();
        }

        m_moves[moveLocation].ResetCurrentFrame();
        m_moves[moveLocation].StopMove();
        yield return null;
    }


    public void DebugBoxCheck()
    {
        if (m_debugHurtBoxes != m_debugHurtBoxesLastFrame)
        {
            foreach (HurtBox hB in m_hurtBoxes)
            {
                hB.DebugHurtBoxesSwitch(m_debugHurtBoxes);
            }
        }
        if (m_debugHitBoxes != m_debugHitBoxesLastFrame)
        {
            foreach (Move move in m_moves)
            {
                foreach (HitBox hB in move.m_moveHitBoxes)
                {
                    hB.DebugHitBoxesSwitch(m_debugHitBoxes);
                }
            }
        }
        m_debugHitBoxesLastFrame = m_debugHitBoxes;
        m_debugHurtBoxesLastFrame = m_debugHurtBoxes;
    }
    public void ToggleDebug()
    {
        m_debugHitBoxes = !m_debugHitBoxes;
        m_debugHurtBoxes = !m_debugHurtBoxes;
    }

    public void AddMove()
    {
        Move newMove = new Move();
        newMove.m_animator = m_animator;
        m_moves.Add(newMove);
    }
    public void RemoveMove()
    {
        if (m_moves.Count != 0)
            m_moves.RemoveAt(m_moves.Count - 1);
    }
    public void AddHitBoxToMove(int moveNumber)
    {
        m_moves[moveNumber].m_moveHitBoxes.Add(new HitBox());
    }
    public void RemoveHitBoxFromMove(int moveNumber)
    {
        if (m_moves.Count != 0 && m_moves[moveNumber].m_moveHitBoxes.Count != 0)
            m_moves[moveNumber].m_moveHitBoxes.RemoveAt(m_moves[moveNumber].m_moveHitBoxes.Count - 1);
    }
    public void UpdateMove()
    {
        foreach (Move m in m_moves)
        {
            if (m.m_moveAnimation != null)
            {
                m.m_moveName = m.m_moveAnimation.name;
                m.SetTotalFrames();
                if (!m.IsMoveInAnimator())
                {
                    m.AddMoveToAnimator();
                }
            }
        }
    }
}