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
    [SerializeField] float currentSpeed = 0f;   // ���� �ӵ�
    [SerializeField] float acceleration; // ���ӵ�
    [SerializeField] float deceleration; // ���Ӱ�
    [SerializeField] float frictionForce; // ������
    [SerializeField] int speedValee;      // �ӵ� ��
    [SerializeField] float controlLock;   // ���� ���

    [Header("SpinDash")]
    [SerializeField] float spinDashmaxSpeed = 30f; // �ִ� �ӵ�
    [SerializeField] float spinDashspeed = 0f; // ���� ���� ���� �ӵ�
    [SerializeField] float spinDashcurrentSpeed = 0f; // ���� ��� �� ����� �ӵ�

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
            // ���� ��� �Է� ó��
            if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.Space) && player.isGrounded)
            {
                // �ִ� �ӵ��� ���� �ʵ��� �ӵ��� ������Ŵ
                if (player.spinDashspeed < player.spinDashmaxSpeed)
                {
                    player.spinDashspeed += 10f * Time.deltaTime; // 1�� ����
                }
            }

            // �����̽��ٿ��� ���� ���� ��
            if (Input.GetKeyUp(KeyCode.Space) && player.isGrounded)
            {
                player.spinDashcurrentSpeed = player.spinDashspeed; // ���� �ӵ��� ����
                player.PerformSpinDash(); // ���� ��� ����
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
    #region �̵�����
    private void Move()
    {
        MoveInput();
        FrictionForce();
        MoveCharacter();
    }

    private void MoveInput()
    {
        // �¿� �Է� �ޱ�
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput < 0) // ���� �Է�
        {
            render.flipX = true;
            if (currentSpeed > 0) // ���� ���������� �̵� ��
            {
                currentSpeed -= deceleration; // ����
                if (currentSpeed <= 0)
                    currentSpeed = -0.5f; // ���� ��ũ
            }
            else if (currentSpeed > -maxSpeed) // �������� �̵� ��
            {
                currentSpeed -= acceleration; // ����
                if (currentSpeed <= -maxSpeed)
                    currentSpeed = -maxSpeed; // �ְ� �ӵ� ����
            }
        }
        else if (horizontalInput > 0) // ������ �Է�
        {
            render.flipX = false;
            if (currentSpeed < 0) // ���� �������� �̵� ��
            {
                currentSpeed += deceleration; // ����
                if (currentSpeed >= 0)
                    currentSpeed = 0.5f; // ���� ��ũ
            }
            else if (currentSpeed < maxSpeed) // ���������� �̵� ��
            {
                currentSpeed += acceleration; // ����
                if (currentSpeed >= maxSpeed)
                    currentSpeed = maxSpeed; // �ְ� �ӵ� ����
            }
        }

    }

    private void FrictionForce()
    {
        if (controlLock <= 0) // ���� ����� ���� ���
        {
            // ���� ����
            if (currentSpeed != 0)
            {   //              Mathf.Min(a , b) �� ���� ���Ͽ� �� ���� ���� ��ȯ
                //              Mathf.Abs(a) a���� ������ ����� ������� ������� ��ȯ
                //              Mathf.Sign(a)���� ����̸� 1��, ������ -1, 0�̸� 0���� ��ȯ(��ȣ Ȯ�ο�)
                currentSpeed -= Mathf.Min(Mathf.Abs(currentSpeed), frictionForce) * Mathf.Sign(currentSpeed);
                if (Mathf.Abs(currentSpeed) < 0.01f) // ���� ���� ������ ���
                {
                    currentSpeed = 0;
                }
            }
        }
    }

    private void MoveCharacter()
    {
        // RigidBody�� �ӵ� ����
        rigid.velocity = new Vector2(currentSpeed, rigid.velocity.y);
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
    #endregion
    #region ���ɴ��
    private void PerformSpinDash()
    {
        // ���� �ӵ��� ���� �߰��Ͽ� ĳ���͸� �̵�
        if (render.flipX == false)
        {
            rigid.AddForce(Vector2.right * spinDashcurrentSpeed, ForceMode2D.Impulse);
            spinDashspeed = 0f; // ���� ���� ��ø� ���� �ӵ��� ����
        }
        else
        {
            rigid.AddForce(Vector2.left * spinDashcurrentSpeed, ForceMode2D.Impulse);
            spinDashspeed = 0f; // ���� ���� ��ø� ���� �ӵ��� ����
        }
    }

    private void SpinDashEnd()
    {
        ChangeState(State.Idle);
    }
    #endregion
    #region �÷��̾� �ǰݽ� ����
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


#region ������ �ۼ��� �̵���(���ɴ�� ���� ������ ���� �ӽ� ����)
//private void Move()
//{
//    if (x != 0) // �¿� �Է��� �ִ� ���
//    {
//        // ���� �ӵ� ����
//        currentSpeed += acceleration * Time.deltaTime;

//        // �ִ� �ӵ� ����
//        if (currentSpeed > maxSpeed)
//        {
//            currentSpeed = maxSpeed;
//        }

//        // ���⿡ ���� ĳ���� ��������Ʈ ����
//        if (x < 0)
//        {
//            render.flipX = true;
//        }
//        else if (x > 0)
//        {
//            render.flipX = false;
//        }
//    }
//    else // �Է��� ���� ���
//    {
//        // ����
//        currentSpeed -= deceleration * Time.deltaTime;

//        // �ӵ��� 0 �̸����� �������� �ʵ��� ����
//        if (currentSpeed < 0)
//        {
//            currentSpeed = 0;
//        }
//    }

//    // Rigidbody�� �� ����     x : �÷��̾ ���� ������ ����, �ݴ�� ���
//    // ���� ������ : (x != 0) ? currentSpeed * Mathf.Sign(x) : 0; �� ���뿡�� x�� ���� 0�� �ƴҶ� ������ ������ ��, 0�϶��� ���� 0���� ��
//    float horizontalVelocity = (x != 0) ? currentSpeed * Mathf.Sign(x) : 0; // Y�� �ӵ��� �״�� ���� ���� �ӵ��� ������Ʈ�� ����
//                                                                            // Mathf.Sign(x)�־��� �� x�� ��ȣ�� ��ȯ ����̸� 1,������ -1 0�̸� 0
//    rigid.velocity = new Vector2(horizontalVelocity, rigid.velocity.y);
//}
#endregion