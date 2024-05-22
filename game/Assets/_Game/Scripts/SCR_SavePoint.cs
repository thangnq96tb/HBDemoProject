using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_SavePoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<SCR_Player>().SavePoint();
        }

    }
}
