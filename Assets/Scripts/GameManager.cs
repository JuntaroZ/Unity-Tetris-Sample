using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int score = 0;
    public TextMeshPro scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText.text = "SCORE:0";
    }
    public void AddScore(int addScore)
    {
        score += addScore;
        scoreText.text = "SCORE:" + score.ToString();
    }
}  