using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monsters : MonoBehaviour
{
    public enum State { Idle, Trace, Return, Attack, Dead, Size }
    [SerializeField] State curState = State.Idle;
    private BaseState[] states = new BaseState[(int)State.Size];

    [SerializeField] GameObject player;
    [SerializeField] GameObject monster;
    [SerializeField] GameObject animal;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer render;
    [SerializeField] PlayerController playerhit;

    [SerializeField] float traceRange;
    [SerializeField] float attackRange;
    [SerializeField] float moveSpeed;
    [SerializeField] Vector2 startPos;

    private void Awake()
    {
        states[(int)State.Idle] = new IdleState(this);
        states[(int)State.Trace] = new TraceState(this);
        states[(int)State.Return] = new ReturnState(this);
        states[(int)State.Attack] = new AttackState(this);
        states[(int)State.Dead] = new DeadState(this);
    }

    private void Start()
    {
        monster = this.gameObject;
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        startPos = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        animal = GameObject.FindGameObjectWithTag("Animal");
        if (player != null)
        {
            playerhit = player.GetComponent<PlayerController>();
        }

        states[(int)curState].Enter();
    }

    private void OnDestroy()
    {
        states[(int)curState].Exit();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // 플레이어와 충돌했을 때
        {
            if (collision.transform.position.y > transform.position.y)
            ChangeState(State.Dead);
        }
    }

    private void Update()
    {
        states[(int)curState].Update();
        //stateText.text = curState.ToString();
    }

    public void ChangeState(State nextState)
    {
        if (curState != nextState)
        {
            states[(int)curState].Exit();
            curState = nextState;
            states[(int)curState].Enter();
        }
    }

    private class MonsterState : BaseState
    {
        public Monsters monsters;

        public MonsterState(Monsters monster)
        {
            this.monsters = monster;
        }
    }

    private class IdleState : MonsterState
    {
        public IdleState(Monsters monsters) : base(monsters)
        {
        }

        public override void Enter()
        {
            monsters.animator.Play("move");
        }

        public override void Update()
        {
            // Idle 행동만 구현
            // 가만히 있기
            // 다른 상태로 전환
            if (Vector2.Distance(monsters.transform.position, monsters.player.transform.position) < monsters.traceRange)
            {
                monsters.ChangeState(State.Trace);
            }
        }
    }

    private class TraceState : MonsterState
    {
        public TraceState(Monsters monsters) : base(monsters)
        {
        }

        public override void Enter()
        {
            monsters.animator.Play("move");
        }

        public override void Update()
        {
            Vector2 direction = (monsters.player.transform.position - monsters.transform.position).normalized;
            monsters.transform.position = Vector2.MoveTowards(monsters.transform.position, monsters.player.transform.position, monsters.moveSpeed * Time.deltaTime);

            // 이동 방향에 따라 방향 반전
            if (direction.x < 0)
            {
                monsters.render.flipX = false;
            }
            else if (direction.x > 0)
            {
                monsters.render.flipX = true;
            }

            // 다른 상태로 전환
            if (Vector2.Distance(monsters.transform.position, monsters.player.transform.position) > monsters.traceRange)
            {
                monsters.ChangeState(State.Return);
            }
            else if (Vector2.Distance(monsters.transform.position, monsters.player.transform.position) < monsters.attackRange)
            {
                monsters.ChangeState(State.Attack);
            }
        }
    }

    private class ReturnState : MonsterState
    {
        public ReturnState(Monsters monsters) : base(monsters)
        {
        }

        public override void Update()
        {
            // Return 행동만 구현
            Vector2 targetPosition = new Vector2(monsters.startPos.x, monsters.transform.position.y);
            monsters.transform.position = Vector2.MoveTowards(monsters.transform.position, targetPosition, monsters.moveSpeed * Time.deltaTime);

            if (Vector2.Distance(new Vector2(monsters.transform.position.x, 0), new Vector2(monsters.startPos.x, 0)) < 0.01f)
            {
                monsters.ChangeState(State.Idle);
            }
        }
    }

    private class AttackState : MonsterState
    {
        public AttackState(Monsters monsters) : base(monsters)
        {
        }

        public override void Enter()
        {
            // 플레이어의 현재 상태가 SpinDashState 또는 JumpDownState가 아닐 때만 공격
            if (monsters.playerhit != null &&
                monsters.playerhit.curState != PlayerController.State.SpinDash &&
                monsters.playerhit.curState != PlayerController.State.JumpDown)
            {
                if (monsters.playerhit.godLock == 0)
                {
                    monsters.AttackPlayer();
                }
            }
            else
            {
                // 공격할 수 없는 경우 Idle 상태로 변경
                monsters.ChangeState(State.Idle);
            }
        }

        public override void Update()
        {
            // Attack 행동만 구현
            if (Vector2.Distance(monsters.transform.position, monsters.player.transform.position) >monsters.attackRange)
            {
                monsters.ChangeState(State.Trace);
            }
        }
    }

    private void AttackPlayer()
    {
        playerhit.TakeDamage();
    }



    private class DeadState : MonsterState
    {
        public DeadState(Monsters monsters) : base(monsters)
        {
        }
        public override void Enter()
        {
            monsters.animator.Play("Dead");
            
            Destroy(monsters.monster, 0.3f);
            Vector2 spawnPosition = monsters.transform.position; // 몬스터의 현재 위치
            GameObject animalInstance = Object.Instantiate(monsters.animal, spawnPosition, Quaternion.identity);
                                        
        }

        public override void Exit()
        {

        }
    }
}
