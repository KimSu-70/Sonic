using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private int coin = 0;         // 코인
    private bool gameOver = false;

    private float times;            // 초
    private int timems;             // 분
    private int timem;

    private int score = 0;

    public TextMeshProUGUI timemText;
    public TextMeshProUGUI timesText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI scoreText;


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
        if (!gameOver)
        {
            times += Time.deltaTime;

            if(times >= 60)
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
        score += 100;
        coinText.text = coin.ToString();
        scoreText.text = score.ToString();
    }

    public int GetCoin()
    {
        return coin;
    }
}
