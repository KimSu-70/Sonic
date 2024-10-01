using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;
    public GameObject bulletPrefab;     // �Ѿ� ������
    public int poolSize = 20;           // �� �Ѿ�

    private Queue<GameObject> bulletPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� �� ��ü �ı�
        }
    }

    private void Start()
    {
        // �Ѿ��� �̸� �����Ͽ� Ǯ�� �߰�
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false); // ��Ȱ��ȭ�Ͽ� Ǯ�� �߰�
            bulletPool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue(); // Ǯ���� �Ѿ� ������
            bullet.SetActive(true); // Ȱ��ȭ
            return bullet;
        }
        return null; // �� �̻� ��� ������ �Ѿ��� ����
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false); // ��Ȱ��ȭ
        bulletPool.Enqueue(bullet); // Ǯ�� ��ȯ
    }
}