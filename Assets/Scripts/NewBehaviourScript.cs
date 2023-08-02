using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class NewBehaviourScript : MonoBehaviour
{

    public Transform player;
    public Animator animator;





    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player);
        Quaternion v = transform.rotation;
        v.Normalize();
        float y = v.y;
        float x = v.x;
        animator.SetFloat("UpDown", x);
        animator.SetFloat("LeftRight", y);

    }

}
