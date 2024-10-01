using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSky : MonoBehaviour
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
    [SerializeField] Transform playerPos;
    [SerializeField] Transform firePos;
    [SerializeField] Transform firePos2;
    private Transform MainfirePos;

    [SerializeField] float traceRange;
    [SerializeField] float attackRange;
    [SerializeField] float moveSpeed;
    [SerializeField] float shootTime = 1;
    [SerializeField] float shootcurTime = 0;
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
        if (player != null)
        {
            playerhit = player.GetComponent<PlayerController>();
        }
         playerPos = player.GetComponent<Transform>();
        GetFirePos();
        GetFirePos2();
        states[(int)curState].Enter();
    }

    private void OnDestroy()
    {
        states[(int)curState].Exit();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerhit.hitCheck) // 플레이어와 충돌했을 때
        {
            //if (collision.transform.position.y > transform.position.y)
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
        public MonsterSky monsters;

        public MonsterState(MonsterSky monster)
        {
            this.monsters = monster;
        }
    }

    private class IdleState : MonsterState
    {
        public IdleState(MonsterSky monsters) : base(monsters)
        {
        }

        public override void Enter()
        {
            monsters.animator.Play("move2");
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
        public TraceState(MonsterSky monsters) : base(monsters)
        {
        }

        public override void Enter()
        {
            monsters.animator.Play("move2");
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
        public ReturnState(MonsterSky monsters) : base(monsters)
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
        public AttackState(MonsterSky monsters) : base(monsters)
        {
        }

        public override void Enter()
        {
            monsters.animator.Play("Atk");
            monsters.shootcurTime = Time.time;
        }

        public override void Update()
        {
            if(Time.time - monsters.shootcurTime >= monsters.shootTime)
            {
                if (Vector2.Distance(monsters.transform.position, monsters.player.transform.position) < monsters.attackRange)
                {
                    monsters.shootcurTime = Time.time; // 마지막 공격 시간 갱신
                    if (monsters.playerhit.godLock == 0)
                    {
                        monsters.AttackPlayer();
                    }
                }
                else
                {
                    monsters.ChangeState(State.Trace);
                }
            }
        }
    }

    private void AttackPlayer()
    {
        Shoot();
    }

    private void Shoot()
    {
        FirePosFlip(); // 발사 위치를 설정
        GameObject bullet = ObjectPoolManager.Instance.GetBullet(); // 오브젝트 풀에서 총알 가져오기
        if (bullet != null)
        {
            // 플레이어의 위치 방향으로 총알 발사
            Vector2 direction = (playerPos.position - MainfirePos.position).normalized; // 방향 계산
            bullet.transform.position = MainfirePos.position; // 발사 위치 설정

            // 총알의 방향과 속도를 설정
            bullet.GetComponent<Bullet>().InputBullet(direction);
        }
    }

    private void FirePosFlip()
    {
        if(render.flipX == false)
        {
            MainfirePos = firePos.transform;
        }
        else
        {
            MainfirePos = firePos2.transform;
        }
    }



    private class DeadState : MonsterState
    {
        public DeadState(MonsterSky monsters) : base(monsters)
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

    private void GetFirePos()
    {
        Transform fire = transform.Find("FirePos");
        if (fire != null)
        {
            firePos = fire;
        }
    }

    private void GetFirePos2()
    {
        Transform fire = transform.Find("FirePos2");
        if (fire != null)
        {
            firePos2 = fire;
        }
    }
}
