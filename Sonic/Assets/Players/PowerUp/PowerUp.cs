using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] GameObject shield;

    private void Start()
    {
        shield = GameObject.FindGameObjectWithTag("Shield");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            shield.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
