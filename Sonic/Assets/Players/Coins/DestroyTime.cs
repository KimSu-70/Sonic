using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTime : MonoBehaviour
{
    [SerializeField] float destroyTime = 3.75f;
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}