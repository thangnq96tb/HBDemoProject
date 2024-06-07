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
}
