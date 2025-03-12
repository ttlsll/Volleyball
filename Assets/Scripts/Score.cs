using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Score : MonoBehaviour
{
    public int LeftScore = 0;
    public int RightScore = 0;
    public TMP_Text leftScore;
    public TMP_Text rightScore;
    public BallBehaviour ball;

    public Vector3 nextServe;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("LeftHalf"))
        {
            RightScore += 1;
            nextServe = new Vector3(8, -1, 0);
            AddScore(rightScore, RightScore);
        }
        if (collision.gameObject.CompareTag("RightHalf"))
        {
            LeftScore += 1;
            nextServe = new Vector3(-10, -1, 0);
            AddScore(leftScore, LeftScore);
        }
    }

    private void AddScore(TMP_Text text, int score)
    {
        text.text = score.ToString();
        ball.ResetBall(nextServe);
    }
}
