using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private int coin = 0;         // ����
    private int coinCount = 0;
    private bool gameOver = false;

    private float times;            // ��
    private int timems;             // ��
    private int timem;

    private int score = 0;          // ����

    [SerializeField] private int playerLife = 3;     // �÷��̾� ���

    public TextMeshProUGUI timemText;
    public TextMeshProUGUI timesText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI playerLifeText;
    [SerializeField] PlayerController player;
    [SerializeField] GameObject gameOvers;
    [SerializeField] GameObject gameStart;

    private void Start()
    {
        Application.targetFrameRate = 60;
        player.OnDied += GameOver;
        gameOvers.SetActive(false);
        gameStart.SetActive(false);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("SonicGame");
        }

        if (!gameOver)
        {
            times += Time.deltaTime;

            if (times >= 60)
            {
                times = 0;
                timem++;
                timemText.text = timem.ToString();
            }
            timems = (int)times;
            timesText.text = timems.ToString();
        }
    }

    public void SetCoin()
    {
        coin++;
        coinCount++;
        coinText.text = coin.ToString();

        if (coinCount == 10)
        {
            score += 100;
            coinCount = 0;
            scoreText.text = score.ToString();
        }
    }

    public int GetCoin()
    {
        return coin;
    }

    public void HitCoins()
    {
        coin = 0;
        coinText.text = coin.ToString();
    }

    public void PlayerLife()
    {
        if (playerLife > 0)
        {
            playerLife--;
            playerLifeText.text = playerLife.ToString();
            player.Respawn();
        }
        else if (playerLife == 0) // ������ 0�� �Ǿ��� �� ���� ����
        {
            GameOver(); // ���� ���� UI ȣ��
        }
    }

    public void GameOver()
    {
        gameOver = true;
        gameOvers.SetActive(true);
    }

    public void GameStart()
    {
        gameStart.SetActive(true);
    }

    public void GameStartEnd()
    {
        gameStart.SetActive(false);
    }
}
