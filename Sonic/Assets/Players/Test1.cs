using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    [SerializeField] Rigidbody2D rd;
    [SerializeField] float speed;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            rd.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
        }
    }
}
