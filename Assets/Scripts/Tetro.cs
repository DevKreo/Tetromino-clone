using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetro : MonoBehaviour
{
    float fall=0;
    public float fallSpeed = 1;

    public int individualScore = 100;

    private float individualScoreTime;

    // Update is called once per frame
    void Update()
    {
        CheckUserInput();
        UpadeteIndividScore();
    }

    void UpadeteIndividScore()
    {
        if(individualScoreTime < 1)
        {
            individualScoreTime += Time.deltaTime;

        }else
        {
            individualScoreTime = 0;
            individualScore = Mathf.Max(individualScore - 10, 0);
        }
    }

    void CheckUserInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);
            if (CheckIsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
            else
            {
                transform.position += new Vector3(-1, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);
            if (CheckIsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
            else
            {
                transform.position += new Vector3(1, 0, 0);
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
        {
            transform.position += new Vector3(0, -1, 0);
            if (CheckIsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
            else
            {
                transform.position += new Vector3(0, 1, 0);

                FindObjectOfType<Game>().DeleteRow();

                if (FindObjectOfType<Game>().CheckPositionOnGameOver(this))
                {
                    FindObjectOfType<Game>().GameOver();
                }

                enabled = false;

                Game.currentScore += individualScore;

                FindObjectOfType<Game>().SpawnNextTetro();
            }

            fall = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.Rotate(0, 0, 90);
            if (CheckIsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
            else
            {
                transform.Rotate(0, 0, -90);
            }
        }
    }

    bool CheckIsValidPosition()
    {
        foreach(Transform mino in transform)
        {
            Vector2 position = FindObjectOfType<Game>().Round(mino.position);

            if (FindObjectOfType<Game>().CheckGrid(position) == false)
                return false;
            if(FindObjectOfType<Game>().GetTransformAtGridPosition(position) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(position).parent != transform)
            {
                return false;
            }
        }
        return true;
    }
}

