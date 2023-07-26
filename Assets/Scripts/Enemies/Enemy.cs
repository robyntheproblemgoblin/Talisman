using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int m_hp;

    private void Awake()
    {
        m_hp = 30;
    }

    public bool TakeHit()
    {
        m_hp -= 10;
        bool isDead = m_hp <= 0;
        if (isDead) Die();
        return isDead;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
