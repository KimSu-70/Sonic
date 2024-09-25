using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum State { Idle, Walk, Run1, Run2, Jumping, Roll, SpinDashing, Dead, Size }
    [SerializeField] private State curState = State.Idle;
    private BaseState[] states = new BaseState[(int)State.Size];


    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer render;
    [SerializeField] GameObject players;
    //[SerializeField] GameManager gameManager;

    [Header("Move")]
    private float x;
    [SerializeField] float maxSpeed;
    [SerializeField] float currentSpeed = 0f;   // 현재 속도
    [SerializeField]float acceleration; // 가속도
    [SerializeField]float deceleration; // 감속값

    [Header("SpinDash")]
    [SerializeField] float spinDashMaxSpeed;    // 최대 속도
    [SerializeField] float spinDashSpeedA;       // 가속도
    [SerializeField] float spinDashSpeedD;      // 감속 값

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
        states[(int)State.Jumping] = new JumpingState(this);
        states[(int)State.Roll] = new RollState(this);
        states[(int)State.SpinDashing] = new SpinDashingState(this);
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
        GroundCheck();
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
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                player.ChangeState(State.Walk);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                player.ChangeState(State.Jumping);
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
                player.ChangeState(State.Jumping);
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
    }

    private class JumpingState : PlayerState
    {
        public JumpingState(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            player.animator.Play("Jump");
        }

        public override void Update()
        {

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

    private class SpinDashingState : PlayerState
    {
        public SpinDashingState(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            player.animator.Play("SpinDash");
        }

        public override void Update()
        {

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
            currentSpeed += acceleration;

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
            currentSpeed -= deceleration;

            // 속도가 0 미만으로 떨어지지 않도록 제한
            if (currentSpeed < 0)
            {
                currentSpeed = 0;
            }
        }

        // Rigidbody에 힘 적용          현재 속도                 // Y축 속도는 그대로 유지 수평 속도만 업데이트를 위해
        rigid.velocity = new Vector2(currentSpeed * Mathf.Sign(x), rigid.velocity.y);
                                                 // Mathf.Sign(x)주어진 값 x의 부호를 반환 양수이면 1,음수는 -1 0이면 0
    }

    #region 이동관련
    public void Jump()
    {
        if (isGrounded == false)
            return;

        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }

    public void SpinDash()
    {
        
    }

    private void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y -0.5f), Vector2.down, 0.5f, mask);
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

    #endregion
}
