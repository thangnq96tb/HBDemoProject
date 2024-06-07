using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [SerializeField] private Character m_Character;
    private float damage;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<Character>().OnHit(30f);
        }

        if (collision.tag == "Enemy")
        {
            collision.GetComponent<Character>().OnHit(m_Character.GetComponent<SCR_Player>().m_DamagePerSword);
        }    
    }
}
