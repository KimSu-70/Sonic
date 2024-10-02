using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && player.hitCheck)
        {
            shields.Play("PowerOn");
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.MonsterDead);
            collision.GetComponentInParent<PlayerController>().shield.SetActive(true);
        }
    }
}
