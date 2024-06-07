using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SCR_Player : Character
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 100;
    [SerializeField] private float jumpForce = 350;
    [SerializeField] private float cooldown = 2;

    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;
    [SerializeField] private int m_NumberKunai;

    [SerializeField] private float m_dmgPercentIncreasePerCoin;
    [SerializeField] public float m_DamagePerKunai;
    [SerializeField] public float m_DamagePerSword;

    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isAttack = false;
    private bool isDeath = false;

    private float horizontal;

    private int numberCoin = 0;

    private Vector3 savePoint; //origin pos of player 

    private void Awake()
    {
        numberCoin = PlayerPrefs.GetInt("numberCoin", 0);
    }
    public override void OnInit()
    {
        base.OnInit();

        isAttack = false;

        transform.position = savePoint;
        ChangeAnim("idle");
        DeactiveAttack();
        SavePoint();

        UIManager.instance.SetKunai(m_NumberKunai);
        UIManager.instance.SetCoin(numberCoin);
        UIManager.instance.UpdatePlayerStatDisplay(m_DamagePerKunai, m_DamagePerSword);
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }

    public override void OnDeath()
    {
        base.OnDeath();
    }

    void Update()
    {
        if(IsDeath)
        {
            return;
        }    

        isGrounded = CheckGrounded();

        //-1: left; 1: right; 0: nothing... (keyboard) 
        horizontal = Input.GetAxisRaw("Horizontal");

        if(isAttack)
        {
            rb.velocity = Vector2.zero;
            return;
        }    

        if (isGrounded)
        {
            if (isJumping)
            {
                return; //do nothing when jumping
            }

            //jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            //change anim run
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                ChangeAnim("run");
            }

            //attack
            if (Input.GetKeyDown(KeyCode.C))
            {
                Attack();
            }

            //throw
            if (Input.GetKeyDown(KeyCode.V))
            {
                Throw();
            }
        }

        //check falling
        if (!isGrounded && rb.velocity.y < 0)
        {
            isJumping = false;
            ChangeAnim("fall");
        }

        //moving
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y); //using velocity.y because player still falling when moving in air (not ground)
            //transform.localPosition = new Vector3(horizontal, 1, 1); //not option, using rotation to flip
            transform.rotation = Quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : 180, 0));
        }
        else if (isGrounded && !isAttack)
        {
            ChangeAnim("idle");
            rb.velocity = Vector2.zero;
        }
    }

    private bool CheckGrounded()
    {
        //Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        return hit.collider != null;
    }

    public void Attack()
    {
        isAttack = true;
        rb.velocity = Vector2.zero;
        ChangeAnim("attack");
        Invoke(nameof(ResetAttack), 0.5f);

        ActiveAttack();
        Invoke(nameof(DeactiveAttack), 0.5f);
    }

    public void Throw()
    {
        if(m_NumberKunai > 0)
        {
            m_NumberKunai--;
            UIManager.instance.SetKunai(m_NumberKunai);
            isAttack = true;
            rb.velocity = Vector2.zero;
            ChangeAnim("throw");
            Invoke(nameof(ResetAttack), 0.5f);

            Instantiate(kunaiPrefab, throwPoint.position, throwPoint.rotation).GetComponent<Kunai>().OnInit(m_DamagePerKunai);

            if(m_NumberKunai == 0)
            {
                StartCoroutine(RechargeKunai(cooldown));
            }
        }
    }

    IEnumerator RechargeKunai(float time)
    {
        UIManager.instance.DisableThrowBtn();
        yield return new WaitForSeconds(time);
        m_NumberKunai = 3;
        UIManager.instance.EnableThowBtn();
        UIManager.instance.SetKunai(m_NumberKunai);
    }

    public void Jump()
    {
        isJumping = true;
        ChangeAnim("jump");
        rb.AddForce(jumpForce * Vector2.up);
    }

    private void ResetAttack()
    {
        isAttack = false;
        ChangeAnim("idle");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Coin")
        {
            numberCoin++;
            PlayerPrefs.SetInt("numberCoin", numberCoin);
            UIManager.instance.SetCoin(numberCoin);
            BuffDamageKunai(m_dmgPercentIncreasePerCoin);
            BuffDamageSword(m_dmgPercentIncreasePerCoin);
            Destroy(collision.gameObject);
        }

        if(collision.tag == "Deathzone")
        {
            ChangeAnim("die");

            Invoke(nameof(OnInit), 1f);
        }

        if (collision.tag == "PotionHealth")
        {
            this.HealingHP(collision.GetComponent<SCR_Potion_Health>().percent);
            Destroy(collision.gameObject);
        }

        if (collision.tag == "PotionWater")
        {
            UIManager.instance.ActiveWaterCollider(collision.GetComponent<SCR_Potion_Water>().duration);
            Destroy(collision.gameObject);
        }
    }
    internal void SavePoint()
    {
        savePoint = transform.position;
    }

    private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }

    private void DeactiveAttack()
    {
        attackArea.SetActive(false);
    }

    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }

    private void HealingHP(float percent)
    {
        hp += percent / 100 * m_MaxHP;

        if(hp > m_MaxHP)
        {
            hp = m_MaxHP;
        }
        healthBar.SetNewHp(hp);
    }    

    private void BuffDamageKunai(float percent)
    {
        m_DamagePerKunai += m_DamagePerKunai * percent / 100;
        UIManager.instance.UpdatePlayerStatDisplay(m_DamagePerKunai, m_DamagePerSword);
    }

    private void BuffDamageSword(float percent)
    {
        m_DamagePerSword += m_DamagePerSword * percent / 100;
        UIManager.instance.UpdatePlayerStatDisplay(m_DamagePerKunai, m_DamagePerSword);
    }


}

