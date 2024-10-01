using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitShield : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] PlayerController playerhit;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerhit = player.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if (!playerhit.hitCheck)
            {
                playerhit.ShieldHit();
                gameObject.SetActive(false);
            }
        }
    }
}
