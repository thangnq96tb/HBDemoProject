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

    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;
    [SerializeField] private int m_NumberKunai = 3;
    [SerializeField] Text m_NumberKunaiTxt; 

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

        UIManager.instance.SetCoin(numberCoin);
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
            UpdateKunaiInfo();
            isAttack = true;
            rb.velocity = Vector2.zero;
            ChangeAnim("throw");
            Invoke(nameof(ResetAttack), 0.5f);

            Instantiate(kunaiPrefab, throwPoint.position, throwPoint.rotation);

            if(m_NumberKunai == 0)
            {
                StartCoroutine(RechargeKunai());
            }
        }
    }

    IEnumerator RechargeKunai()
    {
        UIManager.instance.DisableThrowBtn();
        yield return new WaitForSeconds(10f);
        m_NumberKunai = 3;
        UIManager.instance.EnableThowBtn();
        UpdateKunaiInfo();
    }

    public void UpdateKunaiInfo()
    {
        m_NumberKunaiTxt.text = "x" + m_NumberKunai.ToString();
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
            Destroy(collision.gameObject);
        }
        if(collision.tag == "Deathzone")
        {
            ChangeAnim("die");

            Invoke(nameof(OnInit), 1f);
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
}

