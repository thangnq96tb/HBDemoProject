using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] protected HealthBar healthBar;
    [SerializeField] protected CombatText combatTextPfb;
    [SerializeField] protected float hp;

    private string currentAnimName;

    public bool IsDeath => hp <= 0;

    void Start()
    {
        OnInit();
    }

    void Update()
    {

    }

    public virtual void OnInit()
    {
        healthBar.OnInit(100, transform);
    }

    public virtual void OnDespawn()
    {

    }

    public virtual void OnDeath()
    {
        ChangeAnim("die");
        Invoke(nameof(OnDespawn), 2f);
    }

    public void OnHit(float damage)
    {
        if(!IsDeath)
        {
            hp -= damage;

            if(IsDeath)
            {
                hp = 0;
                OnDeath();
            }

            healthBar.SetNewHp(hp);
            Instantiate(combatTextPfb, transform.position + Vector3.up, Quaternion.identity).OnInit(damage);
        }
    }

    protected void ChangeAnim(string animName)
    {
        if (currentAnimName != animName)
        {
            anim.ResetTrigger(animName);
            currentAnimName = animName;
            anim.SetTrigger(currentAnimName);
        }
    }
}
