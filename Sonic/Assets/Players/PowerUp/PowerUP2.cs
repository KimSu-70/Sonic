using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUP2 : MonoBehaviour
{
    [SerializeField] Animator shields;
    [SerializeField] GameObject players;
    [SerializeField] PlayerController player;

    private void Start()
    {
        shields = GetComponent<Animator>();
        players = GameObject.FindGameObjectWithTag("Player");
        if (players != null)
        {
            player = players.GetComponent<PlayerController>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && player.hitCheck)
        {
            shields.Play("PowerOn");
            collision.gameObject.GetComponentInParent<PlayerController>().shield.SetActive(true);
        }
    }
}
