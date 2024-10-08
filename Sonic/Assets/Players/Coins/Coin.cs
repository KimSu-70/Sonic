using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            UIManager.Instance.SetCoin();
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Coin);
            Destroy(gameObject, 0.01f);
        }
    }
}
