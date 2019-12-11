using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Game : MonoBehaviour
{
    private static int gridWidth = 10;
    private static int gridHeight = 20;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public int scoreOneLine = 40;
    public int scoreTwoLine = 80;
    public int scoreThreeLine = 160;
    public int scoreFourLine = 340;

    public Text hud_score;

    private int numOfRowsIsFull = 0;
    public static int currentScore = 0;

    private GameObject prevTetro;
    private GameObject nextTetro;
    private bool gameStarted = false;
    private Vector2 previewTetrominoPosition = new Vector2(-7.5f, 15.5f);

    // Start is called before the first frame update
    void Start()
    {
        SpawnNextTetro();
    }

    void Update()
    {
        UpdateScore();
        UpdateUI();
    }

    public void UpdateUI()
    {
        hud_score.text = currentScore.ToString();
    }

    public void UpdateScore()
    {
        if (numOfRowsIsFull > 0)
        {
            if (numOfRowsIsFull == 1)
            {
                ClearedOneLine();
            } else if (numOfRowsIsFull == 2)
            {
                ClearedTwoLine();
            }
            else if (numOfRowsIsFull == 3)
            {
                ClearedThreeLine();
            }
            else if (numOfRowsIsFull == 4)
            {
                ClearedFourLine();
            }

            numOfRowsIsFull = 0;
        }
    }

    public void ClearedOneLine()
    {
        currentScore += scoreOneLine;
    }
    public void ClearedTwoLine()
    {
        currentScore += scoreTwoLine;
    }
    public void ClearedThreeLine()
    {
        currentScore += scoreThreeLine;
    }
    public void ClearedFourLine()
    {
        currentScore += scoreFourLine;
    }

    public bool CheckPositionOnGameOver(Tetro tetro)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            foreach (Transform mino in tetro.transform)
            {
                Vector2 position = Round(mino.position);
                if (position.y > gridHeight - 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsFullRowAr(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        numOfRowsIsFull++;
        return true;
    }

    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < gridHeight; i++)
        {
            MoveRowDown(i);
        }
    }

    public void DeleteRow()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            if (IsFullRowAr(y))
            {
                DeleteMinoAt(y);
                MoveAllRowsDown(y + 1);
                y--;
            }
        }
    }

    public void UpdateGrid(Tetro tetro)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tetro.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform mino in tetro.transform)
        {
            Vector2 pos = Round(mino.position);
            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }


    public Transform GetTransformAtGridPosition(Vector2 position)
    {
        if (position.y > gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)position.x, (int)position.y];
        }
    }

    public void SpawnNextTetro()
    {
        if (!gameStarted)
        {
            gameStarted = true;

            nextTetro = (GameObject)Instantiate(Resources.Load(TetroRandomize(), typeof(GameObject)), new Vector2(5.0f, 21.0f), Quaternion.identity);
            prevTetro = (GameObject)Instantiate(Resources.Load(TetroRandomize(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            prevTetro.GetComponent<Tetro>().enabled = false;
        }
        else
        {
            prevTetro.transform.localPosition = new Vector2(5.0f, 20.0f);
            nextTetro = prevTetro;
            nextTetro.GetComponent<Tetro>().enabled = true;

            prevTetro = (GameObject)Instantiate(Resources.Load(TetroRandomize(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            prevTetro.GetComponent<Tetro>().enabled = false;
        }
    }


    public bool CheckGrid (Vector2 position)
    {
        return ((int)position.x >= 0 && (int)position.x < gridWidth && (int)position.y >= 0);
    }
    public Vector2 Round(Vector2 position)
    {
        return new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
    }



    string TetroRandomize()
    {
        int randomTetro = Random.Range(1, 8);
        string randomTetroName = "Tetromino_t";

        switch (randomTetro)
        {
            case 1:
                randomTetroName = "Tetromino_t";
                break;
            case 2:
                randomTetroName = "Tetromino_z";
                break;
            case 3:
                randomTetroName = "Tetromino_plank";
                break;
            case 4:
                randomTetroName = "Tetromino_long";
                break;
            case 5:
                randomTetroName = "Tetromino_cube";
                break;
            case 6:
                randomTetroName = "Tertomino_j_x";
                break;
            case 7:
                randomTetroName = "Tetromino_s";
                break;
        }
        return randomTetroName;
    }
    public void GameOver()
    {
        Application.LoadLevel("Game Over");
    }
}
