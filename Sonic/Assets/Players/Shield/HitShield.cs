using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitShield : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            gameObject.SetActive(false);
        }
    }
}
