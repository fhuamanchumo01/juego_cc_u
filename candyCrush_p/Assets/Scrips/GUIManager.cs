using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    private int moveCounter;

    public Text movesText, scoreText;
    public static GUIManager sharedInstance;

    private int score;

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            scoreText.text = "Score: " + score;
        }
    }

    public int MoveCounter
    {
        get
        {
            return moveCounter;
        }
        set
        {
            moveCounter = value;
            movesText.text = "Moves: "+ moveCounter;
            if (moveCounter <= 0)
            {
                moveCounter = 0;
                StartCoroutine(gameOver());
            }
        }
    }
    void Start()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        score = 0;
        moveCounter = 30;
        movesText.text = "Moves: "+ moveCounter;
        scoreText.text = "Score: " + score;
    }

    private IEnumerator gameOver()
    {
        yield return new WaitUntil(() => !Tablero_manager.shared_instance.se_intercambia);
        yield return new WaitForSeconds(0.25f);
        //pantalla
    }
    
}
