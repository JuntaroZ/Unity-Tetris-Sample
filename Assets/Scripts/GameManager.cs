using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private int score = 0;
    public TextMeshPro scoreText; // スコア表示
    public TextMeshPro gameOverText; // GAME OVER表示
    public float repeatFallTime = 0.05f;

    [System.Serializable]
    public class AutoFallData
    {
        public int score = 0;
        public float autoFallTime = 1f;
    }
    public List<AutoFallData> autoFallDataList = new List<AutoFallData>();

    void Start()
    {
        scoreText.text = "SCORE:0";
        gameOverText.gameObject.SetActive(false);
    }
    void Update()
    {

    }
    public void AddScore(int addScore)
    {
        score += addScore;
        scoreText.text = "SCORE:" + score.ToString();
    }
    public void SetGameOver()
    {
        gameOverText.gameObject.SetActive(true);
    }

    public float GetAutoFallTime()
    {        
        float autoFallTime = 0.0f;
        foreach (var autoFallData in autoFallDataList)
        {
            if (score >= autoFallData.score)
            {
                autoFallTime = autoFallData.autoFallTime;
            }
        }
        return autoFallTime;
    }
}