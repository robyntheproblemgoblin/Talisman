using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;

public class LookAtPlayer : MonoBehaviour
{
    public Transform player;

    public Animator spearmanAnimator;

    public ParticleSystem attackingParticles;



    public float turnSpeed = 1f;

    public float attackTurnSpeed = 1f;


    private Vector3 firstVector;
    private Vector3 secondVector;
    private float upVector;

    float sideAngle;
    float upAngle;

    public GameObject spearman;
    public GameObject playerObject;

    

    // Attack Variables

    public bool isAttacking;
    public bool canTurnAttack;
    public float attackStateSwapDelay = 1.5f;

    private float timeLastAttack;
    private float attackDelay;

    public float minAttackDelay = 1.5f;
    public float maxAttackDelay = 5f;




    private void Start()
    {
        isAttacking = false;

        firstVector = Vector3.zero;
        secondVector = Vector3.zero;
        sideAngle = 0.0f;
        // Time.timeScale = 0.1f;

    }





    void Update()
    {
        Attack();
        LookAtThePlayer();
        AdditiveLookLeftRight();




    }

    private void FixedUpdate()
    {
        
    }


    public void Attack()
    {
        if(Time.time > timeLastAttack + attackDelay)
        {
            isAttacking = true;

            

            spearmanAnimator.SetTrigger("Attack");
            //Vector3 targetPosition = new Vector3(player.position.x, this.transform.position.y, player.position.z);
            //transform.LookAt(targetPosition);
            attackingParticles.Play();

            Invoke("AttackStateSwap", attackStateSwapDelay);
            AttackLookTowardsPlayer();


            timeLastAttack = Time.time;
            attackDelay = UnityEngine.Random.Range(minAttackDelay, maxAttackDelay);

        }
                      
        
    }

    public void AttackLookTowardsPlayer()
    {
        canTurnAttack = true;
        Invoke("AttackTurnStateSwap", 0.45f);
        
    }

    public void AttackStateSwap()
    {
        isAttacking = false;

    }

    public void AttackTurnStateSwap()
    {
        canTurnAttack = false;

    }



    public void LookAtThePlayer()
    {
        if (!isAttacking)
        {
            Vector3 targetPosition = new Vector3(player.position.x, this.transform.position.y, player.position.z);
            Quaternion OriginalRot = transform.rotation;
            transform.LookAt(targetPosition);
            Quaternion NewRot = transform.rotation;
            transform.rotation = OriginalRot;
            transform.rotation = Quaternion.Lerp(transform.rotation, NewRot, turnSpeed * Time.deltaTime);

        }
        
        
        else if (isAttacking && canTurnAttack)
        {

                Vector3 targetPosition = new Vector3(player.position.x, this.transform.position.y, player.position.z);
                Quaternion OriginalRot = transform.rotation;
                transform.LookAt(targetPosition);
                Quaternion NewRot = transform.rotation;
                transform.rotation = OriginalRot;
                transform.rotation = Quaternion.Lerp(transform.rotation, NewRot, attackTurnSpeed * Time.deltaTime);
                
        }







    }


    private void AdditiveLookLeftRight()
    {
        firstVector = new Vector3(spearman.transform.position.x, spearman.transform.position.y, spearman.transform.position.z);
        //secondVector = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, playerObject.transform.position.z);
        secondVector = (playerObject.transform.position - spearman.transform.position);
        secondVector.Normalize();
        
        upVector = secondVector.y;

        secondVector.y = 0f;


        sideAngle = Vector3.SignedAngle(spearman.transform.forward, secondVector, Vector3.up);

        float minLR = -90f;
        float maxLR = 90f;

        float minSideAngle = -1f;
        float maxSideAngle = 1f;

        sideAngle = math.remap(minLR, maxLR, minSideAngle, maxSideAngle, sideAngle);     
        

        Debug.DrawLine(spearman.transform.position, spearman.transform.position + spearman.transform.forward, Color.magenta);
        Debug.DrawLine(spearman.transform.position, spearman.transform.position + secondVector, Color.blue);

        spearmanAnimator.SetFloat("LeftRight", sideAngle);
        spearmanAnimator.SetFloat("UpDown", upVector);
    }

    void SlowTime()
    {
        Time.timeScale = 0.3f;
    }

    void ReturnTime()
    {
        Time.timeScale = 1f;
    }



}
