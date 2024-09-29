using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        Idle, Walk, Run1, Run2, JumpUp, JumpDown,
        Down, SpinDash, Roll, Hit, Dead, Size
    }
    [SerializeField] public State curState = State.Idle;
    private BaseState[] states = new BaseState[(int)State.Size];

    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer render;
    [SerializeField] GameObject players;
    [SerializeField] GameObject Coin;
    [SerializeField] GameObject shield;

    [Header("God")]
    [SerializeField] public int godLock;

    [Header("Move")]
    [SerializeField] float maxSpeed;
    [SerializeField] float currentSpeed = 0f;   // 현재 속도
    [SerializeField] float acceleration; // 가속도
    [SerializeField] float deceleration; // 감속값
    [SerializeField] float frictionForce; // 마찰력
    [SerializeField] int speedValee;      // 속도 값
    [SerializeField] float controlLock;   // 제어 장금

    [Header("SpinDash")]
    [SerializeField] float spinDashmaxSpeed = 30f; // 최대 속도
    [SerializeField] float spinDashspeed = 0f; // 현재 증가 중인 속도
    [SerializeField] float spinDashcurrentSpeed = 0f; // 스핀 대시 시 적용될 속도

    [Header("Jump")]
    [SerializeField] float jumpPower;
    [SerializeField] float maxFallSpeed;
    [SerializeField] bool isGrounded;
    [SerializeField] LayerMask mask;

    private void FixedUpdate()
    {
        if (controlLock == 0)
        {
            Move();
        }
    }

    private void Awake()
    {
        states[(int)State.Idle] = new IdleState(this);
        states[(int)State.Walk] = new WalkState(this);
        states[(int)State.Run1] = new Run1State(this);
        states[(int)State.Run2] = new Run2State(this);
        states[(int)State.JumpUp] = new JumpUpState(this);
        states[(int)State.JumpDown] = new JumpDownState(this);
        states[(int)State.Down] = new DownState(this);
        states[(int)State.SpinDash] = new SpinDashState(this);
        states[(int)State.Roll] = new RollState(this);
        states[(int)State.Hit] = new HitState(this);
        states[(int)State.Dead] = new DeadState(this);
    }

    private void Start()
    {
        states[(int)curState].Enter();
    }

    private void OnDestroy()
    {
        states[(int)curState].Exit();
    }

    private void Update()
    {
        GroundCheck();
        speedValee = GetSpeedValue();
        states[(int)curState].Update();
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

    public void TakeDamage()
    {
        if(UIManager.Instance.GetCoin() == 0)
        {
            ChangeState(State.Dead);
        }
        else
        {
            ChangeState(State.Hit);
        }
    }

    private class PlayerState : BaseState
    {
        public PlayerController player;

        public PlayerState(PlayerController player)
        {
            this.player = player;
        }
    }

    private class IdleState : PlayerState
    {
        public IdleState(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            player.animator.Play("Idle");
        }

        public override void Update()
        {
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                player.ChangeState(State.Walk);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                player.ChangeState(State.JumpUp);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                player.ChangeState(State.Down);
            }
        }
    }

    private class WalkState : PlayerState
    {
        public WalkState(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            player.animator.Play("Walk");
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.ChangeState(State.JumpUp);
            }
            else if (player.speedValee == 2)
            {
                player.ChangeState(State.Run1);
            }
            else if (player.speedValee == 0)
            {
                player.ChangeState(State.Idle);
            }

        }
    }

    private class Run1State : PlayerState
    {
        public Run1State(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            player.animator.Play("Run1");
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.ChangeState(State.JumpUp);
            }
            else if (player.speedValee == 3)
            {
                player.ChangeState(State.Run2);
            }

            else if (player.speedValee == 1)
            {
                player.ChangeState(State.Walk);
            }
        }
    }

    private class Run2State : PlayerState
    {
        public Run2State(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            player.animator.Play("Run2");
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.ChangeState(State.JumpUp);
            }
            else if (player.speedValee == 2)
            {
                player.ChangeState(State.Run1);
            }
        }
    }

    private class JumpUpState : PlayerState
    {
        public JumpUpState(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            player.animator.Play("Roll");
            player.Jump();

        }

        public override void Update()
        {
            if (player.rigid.velocity.y < -0.1f)
            {
                player.ChangeState(State.JumpDown);
            }
        }
    }

    private class JumpDownState : PlayerState
    {
        public JumpDownState(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            player.animator.Play("Roll");
        }

        public override void Update()
        {
            if (player.rigid.velocity.y > 0)
            {
                player.ChangeState(State.Idle);
            }
        }

    }

    private class DownState : PlayerState
    {
        public DownState(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            player.animator.Play("Down");
        }

        public override void Update()
        {
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                player.ChangeState(State.Idle);
            }
            else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.Space))
            {
                player.ChangeState(State.SpinDash);
            }
        }
    }

    private class SpinDashState : PlayerState
    {
        public SpinDashState(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            player.controlLock = 1;
            player.animator.Play("SpinDash");
            player.spinDashspeed = 0f;
        }

        public override void Update()
        {
            // 스핀 대시 입력 처리
            if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.Space) && player.isGrounded)
            {
                // 최대 속도를 넘지 않도록 속도를 증가시킴
                if (player.spinDashspeed < player.spinDashmaxSpeed)
                {
                    player.spinDashspeed += 10f * Time.deltaTime; // 1씩 증가
                }
            }

            // 스페이스바에서 손을 뗐을 때
            if (Input.GetKeyUp(KeyCode.Space) && player.isGrounded)
            {
                player.spinDashcurrentSpeed = player.spinDashspeed; // 현재 속도를 저장
                player.PerformSpinDash(); // 스핀 대시 실행
                player.animator.Play("Roll");
            }

            if(Input.GetAxisRaw("Horizontal") != 0)
            {
                player.SpinDashEnd();
            }
        }

        public override void Exit()
        {
            player.controlLock = 0;
        }
    }

    private class RollState : PlayerState
    {
        public RollState(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            player.animator.Play("Roll");
        }

        public override void Update()
        {
            if (player.rigid.velocity.y > 0)
            {
                player.ChangeState(State.Idle);
            }
        }
    }

    private class HitState : PlayerState
    {
        public HitState(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            player.PlayerHit();
            if (player.render.flipX == false)
            {
                player.animator.Play("Hit");
                player.PlayerGodStart();
                player.rigid.AddForce(new Vector2(-8f, 5), ForceMode2D.Impulse);
            }
            else
            {
                player.animator.Play("Hit");
                player.PlayerGodStart();
                player.rigid.AddForce(new Vector2(+8f, 5), ForceMode2D.Impulse);
            }
        }
    }


    private class DeadState : PlayerState
    {
        public DeadState(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            player.animator.Play("Dead");
            player.PlayerDaed();
            //player.StartCoroutine(IGameOver(1f));
            //Destroy(player.players, 2f);
        }

        //public override void Exit()
        //{
        //    player.StopCoroutine(IGameOver(0f));
        //}

        //private IEnumerator IGameOver(float waitTime)
        //{
        //    yield return new WaitForSeconds(waitTime);
        //    player.gameManager.GameOver();
        //}
    }
    public void Jump()
    {
        if (isGrounded == false)
            return;

        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }

    private void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.5f), Vector2.down, 0.5f, mask);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - 0.5f), Vector2.down * 0.5f, Color.red);
        if (hit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (rigid.velocity.y < -maxFallSpeed)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, -maxFallSpeed);
        }
    }
    public void PlayerHit()
    {
        int coin = UIManager.Instance.GetCoin();
        UIManager.Instance.HitCoins();
        for (int i = 0; i < coin; i++)
        {
            GameObject coinP = Instantiate(Coin, transform.position, transform.rotation);
            Rigidbody2D coins = coinP.GetComponent<Rigidbody2D>();

            float dd = Mathf.Deg2Rad * 360f / (coin + 1);
            Vector2 dir = new Vector2(Mathf.Cos(dd * i), Mathf.Sin(dd * i));
            coins.AddForce(dir * 7f, ForceMode2D.Impulse);
        }
    }
    #region 이동관련
    private void Move()
    {
        MoveInput();
        FrictionForce();
        MoveCharacter();
    }

    private void MoveInput()
    {
        // 좌우 입력 받기
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput < 0) // 왼쪽 입력
        {
            render.flipX = true;
            if (currentSpeed > 0) // 현재 오른쪽으로 이동 중
            {
                currentSpeed -= deceleration; // 감속
                if (currentSpeed <= 0)
                    currentSpeed = -0.5f; // 감속 퀴크
            }
            else if (currentSpeed > -maxSpeed) // 왼쪽으로 이동 중
            {
                currentSpeed -= acceleration; // 가속
                if (currentSpeed <= -maxSpeed)
                    currentSpeed = -maxSpeed; // 최고 속도 제한
            }
        }
        else if (horizontalInput > 0) // 오른쪽 입력
        {
            render.flipX = false;
            if (currentSpeed < 0) // 현재 왼쪽으로 이동 중
            {
                currentSpeed += deceleration; // 감속
                if (currentSpeed >= 0)
                    currentSpeed = 0.5f; // 감속 퀴크
            }
            else if (currentSpeed < maxSpeed) // 오른쪽으로 이동 중
            {
                currentSpeed += acceleration; // 가속
                if (currentSpeed >= maxSpeed)
                    currentSpeed = maxSpeed; // 최고 속도 제한
            }
        }

    }

    private void FrictionForce()
    {
        if (controlLock <= 0) // 제어 잠금이 없을 경우
        {
            // 마찰 적용
            if (currentSpeed != 0)
            {   //              Mathf.Min(a , b) 두 값을 비교하여 더 작은 값을 반환
                //              Mathf.Abs(a) a값이 음수든 양수든 상관없이 양수값만 반환
                //              Mathf.Sign(a)값이 양수이면 1값, 음수면 -1, 0이면 0값을 반환(부호 확인용)
                currentSpeed -= Mathf.Min(Mathf.Abs(currentSpeed), frictionForce) * Mathf.Sign(currentSpeed);
                if (Mathf.Abs(currentSpeed) < 0.01f) // 거의 정지 상태일 경우
                {
                    currentSpeed = 0;
                }
            }
        }
    }

    private void MoveCharacter()
    {
        // RigidBody의 속도 설정
        rigid.velocity = new Vector2(currentSpeed, rigid.velocity.y);
    }

    private int GetSpeedValue()     // 애니메이션의 원할한 전환을 위해 속도에 대한 값 만들기
    {
        if (currentSpeed == 0f)
        {
            return 0;
        }
        else if (currentSpeed < 6.5f)
        {
            return 1; // 속도가 0~6.5 이하일 때
        }
        else if (currentSpeed >= 6.5f && currentSpeed < 7.9f)
        {
            return 2; // 속도가 6.5 이상이고 7.9 미만일 때
        }
        else // player.currentSpeed >= 7.9f
        {
            return 3; // 속도가 7.9 이상일 때
        }
    }
    #endregion
    #region 스핀대시
    private void PerformSpinDash()
    {
        // 현재 속도로 힘을 추가하여 캐릭터를 이동
        if (render.flipX == false)
        {
            rigid.AddForce(Vector2.right * spinDashcurrentSpeed, ForceMode2D.Impulse);
            spinDashspeed = 0f; // 다음 스핀 대시를 위해 속도를 리셋
        }
        else
        {
            rigid.AddForce(Vector2.left * spinDashcurrentSpeed, ForceMode2D.Impulse);
            spinDashspeed = 0f; // 다음 스핀 대시를 위해 속도를 리셋
        }
    }

    private void SpinDashEnd()
    {
        ChangeState(State.Idle);
    }
    #endregion
    #region 플레이어 피격시 무적
    private void PlayerGodStart()
    {
        godLock = 1;
        gameObject.layer = 9;

        render.color = new Color(1, 1, 1, 0.4f);

        Invoke("PlayerGodEnd", 2);
        Invoke("PlayerGodChangeState", 0.5f);
    }

    private void PlayerGodEnd()
    {
        godLock = 0;
        gameObject.layer = 8;

        render.color = new Color(1, 1, 1, 1);
    }

    private void PlayerGodChangeState()
    {
        ChangeState(State.Idle);
    }
    #endregion
    private void PlayerDaed()
    {
        godLock = 1;
        gameObject.layer = 11;
        rigid.AddForce(new Vector2(0, 7), ForceMode2D.Impulse);
    }
}


#region 기존에 작성된 이동문(스핀대시 동작 구현을 위해 임시 보류)
//private void Move()
//{
//    if (x != 0) // 좌우 입력이 있는 경우
//    {
//        // 현재 속도 증가
//        currentSpeed += acceleration * Time.deltaTime;

//        // 최대 속도 제한
//        if (currentSpeed > maxSpeed)
//        {
//            currentSpeed = maxSpeed;
//        }

//        // 방향에 따라 캐릭터 스프라이트 반전
//        if (x < 0)
//        {
//            render.flipX = true;
//        }
//        else if (x > 0)
//        {
//            render.flipX = false;
//        }
//    }
//    else // 입력이 없는 경우
//    {
//        // 감속
//        currentSpeed -= deceleration * Time.deltaTime;

//        // 속도가 0 미만으로 떨어지지 않도록 제한
//        if (currentSpeed < 0)
//        {
//            currentSpeed = 0;
//        }
//    }

//    // Rigidbody에 힘 적용     x : 플레이어가 왼쪽 누르면 음수, 반대면 양수
//    // 삼항 연산자 : (x != 0) ? currentSpeed * Mathf.Sign(x) : 0; 이 내용에서 x의 값이 0이 아닐때 좌측이 내용이 참, 0일때는 우측 0값이 참
//    float horizontalVelocity = (x != 0) ? currentSpeed * Mathf.Sign(x) : 0; // Y축 속도는 그대로 유지 수평 속도만 업데이트를 위해
//                                                                            // Mathf.Sign(x)주어진 값 x의 부호를 반환 양수이면 1,음수는 -1 0이면 0
//    rigid.velocity = new Vector2(horizontalVelocity, rigid.velocity.y);
//}
#endregion