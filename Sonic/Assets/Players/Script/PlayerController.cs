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
    [SerializeField] float currentSpeed = 0f;   // ���� �ӵ�
    [SerializeField] float acceleration; // ���ӵ�
    [SerializeField] float deceleration; // ���Ӱ�
    [SerializeField] int speedValee;      // �ӵ� ��

    [Header("SpinDash")]
    private bool spinDash = false; // ���� ��� ����
    private float spinDashmaxSpeed = 15f; // �ִ� �ӵ�
    private float spinDashspeed = 0f; // ���� ���� ���� �ӵ�
    private float spinDashcurrentSpeed = 0f; // ���� ��� �� ����� �ӵ�

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
        HandleSpinDashInput(); // ���� ���
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
        if (x != 0) // �¿� �Է��� �ִ� ���
        {
            // ���� �ӵ� ����
            currentSpeed += acceleration * Time.deltaTime;

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
            currentSpeed -= deceleration * Time.deltaTime;

            // �ӵ��� 0 �̸����� �������� �ʵ��� ����
            if (currentSpeed < 0)
            {
                currentSpeed = 0;
            }
        }

        // Rigidbody�� �� ����     x : �÷��̾ ���� ������ ����, �ݴ�� ���
        // ���� ������ : (x != 0) ? currentSpeed * Mathf.Sign(x) : 0; �� ���뿡�� x�� ���� 0�� �ƴҶ� ������ ������ ��, 0�϶��� ���� 0���� ��
        float horizontalVelocity = (x != 0) ? currentSpeed * Mathf.Sign(x) : 0; // Y�� �ӵ��� �״�� ���� ���� �ӵ��� ������Ʈ�� ����
        // Mathf.Sign(x)�־��� �� x�� ��ȣ�� ��ȯ ����̸� 1,������ -1 0�̸� 0
        rigid.velocity = new Vector2(horizontalVelocity, rigid.velocity.y);
    }

    #region �̵�����
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

    private int GetSpeedValue()     // �ִϸ��̼��� ������ ��ȯ�� ���� �ӵ��� ���� �� �����
    {
        if (currentSpeed == 0f)
        {
            return 0;
        }
        else if (currentSpeed < 6.5f)
        {
            return 1; // �ӵ��� 0~6.5 ������ ��
        }
        else if (currentSpeed >= 6.5f && currentSpeed < 7.9f)
        {
            return 2; // �ӵ��� 6.5 �̻��̰� 7.9 �̸��� ��
        }
        else // player.currentSpeed >= 7.9f
        {
            return 3; // �ӵ��� 7.9 �̻��� ��
        }
    }

    private void HandleSpinDashInput()
    {
        // ����Ʈ Ű�� ���� �ְ�, ĳ���Ͱ� ���� ���� ��
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            Debug.Log("���ɴ�� ����");
            // �ִ� �ӵ��� ���� �ʵ��� �ӵ��� ������Ŵ
            if (spinDashspeed < spinDashmaxSpeed)
            {
                spinDashspeed += 1f * Time.deltaTime; // 1�� ����, �ð��� ���� �ε巴�� ����
                Debug.Log(spinDashspeed);
            }
        }

        // ����Ʈ Ű���� ���� ���� ��
        if (Input.GetKeyUp(KeyCode.LeftShift) && isGrounded)
        {
            spinDashcurrentSpeed = spinDashspeed; // ���� �ӵ��� currentSpeed�� ����
            spinDash = true; // ���� ��� ����
        }

        // ���� ��� ������ ��
        if (spinDash)
        {
            PerformSpinDash(); // ���� ��� ����
        }
    }

    private void PerformSpinDash()
    {
        // ���� �ӵ��� ���� �߰��Ͽ� ĳ���͸� �̵�
        rigid.AddForce(Vector2.right * spinDashcurrentSpeed, ForceMode2D.Impulse);
        spinDash = false; // ���� ��� ���¸� ����
        spinDashspeed = 0f; // ���� ���� ��ø� ���� �ӵ��� ����
    }
    #endregion
}
