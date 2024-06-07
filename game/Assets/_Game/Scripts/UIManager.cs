using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;  

    //public static UIManager Instance
    //{
    //    get
    //    {
    //        if(instance == null)
    //        {
    //            instance = FindObjectOfType<UIManager>();
    //        }

    //        return instance;
    //    }
    //}

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] Text coinText;
    [SerializeField] Text kunaiText;
    [SerializeField] Button m_ThrowBtn;
    [SerializeField] Animator m_ReloadAnim;
    [SerializeField] GameObject m_WaterCollider;
    [SerializeField] GameObject m_PotionHealthPfb;
    [SerializeField] GameObject m_PotionWaterPfb;

    [SerializeField] int m_DropHealthPotionRate = 30;
    [SerializeField] int m_DropWaterPotionRate = 80;

    [SerializeField] public Text m_DamagePerKunaiTxt;
    [SerializeField] public Text m_DamagePerSwordTxt;

    public void SetCoin(int coin)
    {
        coinText.text = coin.ToString();
    }

    public void SetKunai(int numberKunai)
    {
        kunaiText.text = "x" + numberKunai;
        if(numberKunai == 0)
        {
            m_ReloadAnim.SetTrigger("reload");
        }    
    }    

    public void DisableThrowBtn()
    {
        m_ThrowBtn.interactable = false;
    }

    public void EnableThowBtn()
    {
        m_ThrowBtn.interactable = true;
    }

    public void ActiveWaterCollider(float time)
    {
        StartCoroutine(TriggerWaterPotion(time));
    }    

    IEnumerator TriggerWaterPotion(float time)
    {
        m_WaterCollider.SetActive(true);
        yield return new WaitForSeconds(time);
        m_WaterCollider.SetActive(false);
    }    

    public void DropItemWhenEnemyDead(Transform enemy)
    {
        //30% drop Health potion, else 80% drop Water potion (Health is high piority)
        int randomChance = Random.Range(0, 100);
        if(randomChance < m_DropHealthPotionRate)
        {
            Instantiate(m_PotionHealthPfb, enemy.position, Quaternion.identity);
        }    
        else if(randomChance < m_DropWaterPotionRate)
        {
            Instantiate(m_PotionWaterPfb, enemy.position, Quaternion.identity);
        }
    }

    public void UpdatePlayerStatDisplay(float dmgKunai, float dmgSword)
    {
        m_DamagePerKunaiTxt.text = "Kunai: " + dmgKunai;
        m_DamagePerSwordTxt.text = "Sword: " + dmgSword;
    }
}
