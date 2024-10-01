using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 3;              // 총알 속도
    [SerializeField] Vector2 direction;             // 총알 방향
    [SerializeField] PlayerController playerHit;
    [SerializeField] GameObject Player;
    [SerializeField] Rigidbody2D rigid;

    public void InputBullet(Vector2 newDirection)
    {
        direction = newDirection.normalized; // 방향을 정규화

        if (rigid != null)
        {
            rigid.AddForce(direction * speed, ForceMode2D.Impulse); // 초기 속도 설정
        }
    }

    private void Awake()
    {
        if (rigid == null)
        {
            rigid = GetComponent<Rigidbody2D>(); // 초기화가 필요할 경우
        }
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null)
        {
            playerHit = Player.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerHit != null) // null 체크
            {
                playerHit.TakeDamage();
            }
            ObjectPoolManager.Instance.ReturnBullet(gameObject);
        }
        else
        {
            ObjectPoolManager.Instance.ReturnBullet(gameObject);
        }
    }
}
