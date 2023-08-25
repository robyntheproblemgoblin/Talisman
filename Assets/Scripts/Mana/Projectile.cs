using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float m_lifeTime = 5;
    float m_startTime;
    public float m_damage;    

    void Start()
    {
        m_startTime = Time.realtimeSinceStartup;
    }
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        EnemyBT enemy = collision.gameObject.GetComponent<EnemyBT>();
        if(enemy != null)
        {            
            enemy.TakeHit(m_damage);
        }
    }
    void Update()
    {
        if(Time.realtimeSinceStartup >= m_startTime + m_lifeTime) { Destroy(gameObject); }
    }
}
