using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;
    public GameObject bulletPrefab;     // 총알 프리팹
    public int poolSize = 20;           // 총 총알

    private Queue<GameObject> bulletPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 이 객체 파괴
        }
    }

    private void Start()
    {
        // 총알을 미리 생성하여 풀에 추가
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false); // 비활성화하여 풀에 추가
            bulletPool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue(); // 풀에서 총알 꺼내기
            bullet.SetActive(true); // 활성화
            return bullet;
        }
        return null; // 더 이상 사용 가능한 총알이 없음
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false); // 비활성화
        bulletPool.Enqueue(bullet); // 풀에 반환
    }
}