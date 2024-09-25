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
    [SerializeField] float currentSpeed = 0f;   // ���� �ӵ�
    [SerializeField]float acceleration; // ���ӵ�
    [SerializeField]float deceleration; // ���Ӱ�

    [Header("SpinDash")]
    [SerializeField] float spinDashMaxSpeed;    // �ִ� �ӵ�
    [SerializeField] float spinDashSpeedA;       // ���ӵ�
    [SerializeField] float spinDashSpeedD;      // ���� ��

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
    //    hp -= damage; // ü�� ����
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
    //        // �� ��Ʈ �̹����� Ȱ��ȭ ���¸� ������Ʈ
    //        if (i < hp)
    //        {
    //            imageHp[i].enabled = true; // ü���� ���������� ��Ʈ �̹��� Ȱ��ȭ
    //        }
    //        else
    //        {
    //            imageHp[i].enabled = false; // ü���� ������ ��Ʈ �̹��� ��Ȱ��ȭ
    //        }
    //    }
    //}

    //public void CollectCoin()
    //{
    //    coinCount++; // ���� �� ����
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
        if (x != 0) // �¿� �Է��� �ִ� ���
        {
            // ���� �ӵ� ����
            currentSpeed += acceleration;

            // �ִ� �ӵ� ����
            if (currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }

            // ���⿡ ���� ĳ���� ��������Ʈ ����
            if (x < 0)
            {
                render.flipX = true;
            }
            else if (x > 0)
            {
                render.flipX = false;
            }
        }
        else // �Է��� ���� ���
        {
            // ����
            currentSpeed -= deceleration;

            // �ӵ��� 0 �̸����� �������� �ʵ��� ����
            if (currentSpeed < 0)
            {
                currentSpeed = 0;
            }
        }

        // Rigidbody�� �� ����          ���� �ӵ�                 // Y�� �ӵ��� �״�� ���� ���� �ӵ��� ������Ʈ�� ����
        rigid.velocity = new Vector2(currentSpeed * Mathf.Sign(x), rigid.velocity.y);
                                                 // Mathf.Sign(x)�־��� �� x�� ��ȣ�� ��ȯ ����̸� 1,������ -1 0�̸� 0
    }

    #region �̵�����
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
