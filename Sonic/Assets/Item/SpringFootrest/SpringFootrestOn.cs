using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringFootrestOn : MonoBehaviour
{
    [SerializeField] Animator animatorStart;
    [SerializeField] GameObject players;
    [SerializeField] PlayerController player;

    private void Start()
    {
        animatorStart = GetComponent<Animator>();
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
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.SpringFootrest);
            animatorStart.SetBool("Check", true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        animatorStart.SetBool("Check", false);
    }
}
