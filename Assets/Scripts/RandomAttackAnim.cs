using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomAttackAnim : MonoBehaviour
{

    FPControls m_inputControl;

    public Animator characterAnimator;
    public Animator enemyAnimator;
    public ParticleSystem flamesLeft;
    public ParticleSystem bloodSparks;
    public LookAtPlayer spearmanController;




    void Start()
    {
        m_inputControl = new FPControls();        
        m_inputControl.Player_Map.Enable();;

    }



    void Update()
    {
//        if (m_inputControl.Player_Map.Attack.WasPressedThisFrame())
        {
            Attack();
            Debug.Log("Attacked");
        }

   //     if (m_inputControl.Player_Map.AttackLeft.WasPressedThisFrame())
        {
            LeftAttack();
            Debug.Log("LeftAttack");
        }
      //  else if (m_inputControl.Player_Map.AttackLeft.WasReleasedThisFrame())
        {
            characterAnimator.SetBool("LeftAttacking", false);
            flamesLeft.Stop();
        }



     //   characterAnimator.SetFloat("Forward", fpController.speedX);
      //  characterAnimator.SetFloat("Sideways", fpController.speedY);


    }


    void Attack()
    {
        int randomNumber = Random.Range(1, 4);
        characterAnimator.SetTrigger("Attack" + randomNumber);
    }

    void LeftAttack()
    {
        characterAnimator.SetBool("LeftAttacking", true);
        flamesLeft.Play();
    }

    public void GetHit()
    {
        HitParticles();

        if (!spearmanController.isAttacking)
        {
            enemyAnimator.SetTrigger("Hit");
        }

        
    }

    public void GetHitLight()
    {
        if (!spearmanController.isAttacking)
        {
            enemyAnimator.SetTrigger("SmallHit");
        }

    }

    public void HitParticles()
    {
        bloodSparks.Play();
    }







}
