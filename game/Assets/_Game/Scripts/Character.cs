using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private float hp;
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
        hp = 100;
    }

    public virtual void OnDespawn()
    {

    }

    public virtual void OnDeath()
    {

    }

    public void OnHit(float damage)
    {
        if(!IsDeath)
        {
            hp -= damage;

            if(IsDeath)
            {
                OnDeath();
            }
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
