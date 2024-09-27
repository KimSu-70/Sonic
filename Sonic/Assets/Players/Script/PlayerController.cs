using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State { Idle, Walk, Run1, Run2, JumpUp, JumpDown, Down, Roll, Dead, Size }
    [SerializeField] private State curState = State.Idle;
    private BaseState[] states = new BaseState[(int)State.Size];


    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer render;
    [SerializeField] GameObject players;
    [SerializeField] LayerMask loop;
    private bool isInLoop = false;
    //[SerializeField] GameManager gameManager;

    [Header("Move")]
    private float x;
    [SerializeField] float maxSpeed;
    [SerializeField] float currentSpeed = 0f;   // 현재 속도
    [SerializeField] float acceleration; // 가속도
    [SerializeField] float deceleration; // 감속값
    [SerializeField] int speedValee;      // 속도 값

    [Header("SpinDash")]
    private bool spinDash = false; // 스핀 대시 상태
    private float spinDashmaxSpeed = 15f; // 최대 속도
    private float spinDashspeed = 0f; // 현재 증가 중인 속도
    private float spinDashcurrentSpeed = 0f; // 스핀 대시 시 적용될 속도

    [Header("Jump")]
    [SerializeField] float jumpPower;
    [SerializeField] float maxFallSpeed;
    [SerializeField] bool isGrounded;
    [SerializeField] LayerMask mask;

    //[Header("UI")]
    //[SerializeField] Image[] imageHp;
    //[SerializeField] TextMeshProUGUI textCoin;

    //[SerializeField] public int hp = 3;
    //[SerializeField] private int coinCount = 0;


    private void FixedUpdate()
    {
        Move(x);
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
        states[(int)State.Roll] = new RollState(this);
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

        x = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        HandleSpinDashInput(); // 스핀 대시
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

    //public void TakeDamage(int damage)
    //{
    //    hp -= damage; // 체력 감소
    //    //HpUI();

    //    if (hp <= 0)
    //    {
    //        ChangeState(State.Dead);
    //    }
    //}

    //private void HpUI()
    //{
    //    for (int i = 0; i < imageHp.Length; i++)
    //    {
    //        // 각 하트 이미지의 활성화 상태를 업데이트
    //        if (i < hp)
    //        {
    //            imageHp[i].enabled = true; // 체력이 남아있으면 하트 이미지 활성화
    //        }
    //        else
    //        {
    //            imageHp[i].enabled = false; // 체력이 없으면 하트 이미지 비활성화
    //        }
    //    }
    //}

    //public void CollectCoin()
    //{
    //    coinCount++; // 코인 수 증가
    //    CoinUI();
    //}

    //private void CoinUI()
    //{
    //    textCoin.text = coinCount.ToString();
    //}

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

    private class DeadState : PlayerState
    {
        public DeadState(PlayerController player) : base(player)
        {
        }

        //public override void Enter()
        //{
        //    player.animator.Play("Hurt");
        //    player.StartCoroutine(IGameOver(1f));
        //    Destroy(player.players, 2f);
        //}

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

    private void Move(float x)
    {
        if (x != 0) // 좌우 입력이 있는 경우
        {
            // 현재 속도 증가
            currentSpeed += acceleration * Time.deltaTime;

            // 최대 속도 제한
            if (currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }

            // 방향에 따라 캐릭터 스프라이트 반전
            if (x < 0)
            {
                render.flipX = true;
            }
            else if (x > 0)
            {
                render.flipX = false;
            }
        }
        else // 입력이 없는 경우
        {
            // 감속
            currentSpeed -= deceleration * Time.deltaTime;

            // 속도가 0 미만으로 떨어지지 않도록 제한
            if (currentSpeed < 0)
            {
                currentSpeed = 0;
            }
        }

        // Rigidbody에 힘 적용     x : 플레이어가 왼쪽 누르면 음수, 반대면 양수
        // 삼항 연산자 : (x != 0) ? currentSpeed * Mathf.Sign(x) : 0; 이 내용에서 x의 값이 0이 아닐때 좌측이 내용이 참, 0일때는 우측 0값이 참
        float horizontalVelocity = (x != 0) ? currentSpeed * Mathf.Sign(x) : 0; // Y축 속도는 그대로 유지 수평 속도만 업데이트를 위해
        // Mathf.Sign(x)주어진 값 x의 부호를 반환 양수이면 1,음수는 -1 0이면 0
        rigid.velocity = new Vector2(horizontalVelocity, rigid.velocity.y);
    }

    #region 이동관련
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

    private void HandleSpinDashInput()
    {
        // 쉬프트 키가 눌려 있고, 캐릭터가 땅에 있을 때
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            Debug.Log("스핀대시 시작");
            // 최대 속도를 넘지 않도록 속도를 증가시킴
            if (spinDashspeed < spinDashmaxSpeed)
            {
                spinDashspeed += 1f * Time.deltaTime; // 1씩 증가, 시간에 따라 부드럽게 증가
                Debug.Log(spinDashspeed);
            }
        }

        // 쉬프트 키에서 손을 뗐을 때
        if (Input.GetKeyUp(KeyCode.LeftShift) && isGrounded)
        {
            spinDashcurrentSpeed = spinDashspeed; // 현재 속도를 currentSpeed에 저장
            spinDash = true; // 스핀 대시 시작
        }

        // 스핀 대시 상태일 때
        if (spinDash)
        {
            PerformSpinDash(); // 스핀 대시 실행
        }
    }

    private void PerformSpinDash()
    {
        // 현재 속도로 힘을 추가하여 캐릭터를 이동
        rigid.AddForce(Vector2.right * spinDashcurrentSpeed, ForceMode2D.Impulse);
        spinDash = false; // 스핀 대시 상태를 리셋
        spinDashspeed = 0f; // 다음 스핀 대시를 위해 속도를 리셋
    }
    #endregion
}
