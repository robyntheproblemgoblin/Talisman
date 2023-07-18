using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxObject : MonoBehaviour
{
    [HideInInspector] public HitBox m_hitbox;
    public List<HurtBoxObject> m_alreadyCollidedWithThisHurtBox = new List<HurtBoxObject>();
    private void Update()
    {
        if (m_hitbox.m_owner.m_debugHurtBoxes)
        {
            m_hitbox.UpdateDebugMeshSize();
        }
    }
    private void OnTriggerEnter(Collider collision)
    {    
        //check if we collided with a hurt box
        if (collision.gameObject.GetComponent<HurtBoxObject>() == null)
        {
            m_hitbox.m_isColliding = false;
            return;
        }
        HurtBoxObject collidingHurtBox = collision.gameObject.GetComponent<HurtBoxObject>();

        foreach(HurtBoxObject hBO in m_alreadyCollidedWithThisHurtBox)
        {
            if(hBO == collidingHurtBox)
            {
                m_hitbox.m_isColliding = false;
                return;
            }
        }

        //check we haven't hit our own hurt box
        if (collidingHurtBox.m_hurtbox.m_owner == m_hitbox.m_owner)
        {
            m_hitbox.m_isColliding = false;
            return;
        }

        m_hitbox.m_collidingHurtBox = collidingHurtBox.m_hurtbox;
        m_hitbox.m_isColliding = true;
        m_alreadyCollidedWithThisHurtBox.Add(collidingHurtBox);
    }

}
