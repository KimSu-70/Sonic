using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTime : MonoBehaviour
{
    [SerializeField] float destroyTime;
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}