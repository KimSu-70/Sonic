using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 3;              // �Ѿ� �ӵ�
    [SerializeField] Vector2 direction;             // �Ѿ� ����
    [SerializeField] PlayerController playerHit;
    [SerializeField] GameObject Player;
    [SerializeField] Rigidbody2D rigid;

    public void InputBullet(Vector2 newDirection)
    {
        direction = newDirection.normalized; // ������ ����ȭ

        if (rigid != null)
        {
            rigid.AddForce(direction * speed, ForceMode2D.Impulse); // �ʱ� �ӵ� ����
        }
    }

    private void Awake()
    {
        if (rigid == null)
        {
            rigid = GetComponent<Rigidbody2D>(); // �ʱ�ȭ�� �ʿ��� ���
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
            if (playerHit != null) // null üũ
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
