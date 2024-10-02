using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] Animator GoalPlay;

    private void Start()
    {
        GoalPlay = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GoalPlay.Play("HitEnd");
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.MonsterDead);
        }
    }
}
