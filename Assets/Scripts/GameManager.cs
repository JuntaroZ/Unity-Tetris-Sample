using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int score = 0;
    public TextMeshPro scoreText; // スコア表示
    public TextMeshPro gameOverText; // GAME OVER表示

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

}  