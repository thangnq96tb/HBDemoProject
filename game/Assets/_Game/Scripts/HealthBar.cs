using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image imgFill;

    private float hp, maxHp;

    public void OnInit(float maxHp)
    {
        this.maxHp = maxHp;
        hp = maxHp;
        imgFill.fillAmount = 1; 
    }

    public void SetNewHp(float hp)
    {
        this.hp = hp;
        //imgFill.fillAmount = hp / maxHp;
    }

    void Update()
    {
        imgFill.fillAmount = Mathf.Lerp(imgFill.fillAmount, hp / maxHp, Time.deltaTime * 5f);
    }
}
