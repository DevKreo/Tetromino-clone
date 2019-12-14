using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetro : MonoBehaviour
{
    float fall=0;
    private float fallSpeed;
    public bool isAllowRotation = true;
    public bool IsLimitRotation = false;
    public int individualScore = 100;
    private float individualScoreTime;

    private float continuousVertSpeed = 0.05f; // down speed
    private float continuousHorizSpeed = 0.1f; // horizontal speed (left or right)
    private float buttonDownWaitMax = 0.2f; //how long to wait the tetro recognized that a button being held down
    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float buttonDownWaitTimerHorizontal = 0;
    private float buttonDownWaitTimerVertical = 0;

    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;

    //Переменные для тачскрина
    private int touchSensitivityHorizontal = 8;
    private int touchSensitivityVertical = 4;
    Vector2 previosUnitPosition = Vector2.zero;
    Vector2 direction = Vector2.zero;
    bool isMoved = false;

    private void Start()
    {
        fallSpeed = GameObject.Find("GameScript").GetComponent<Game>().fallSpeed;
    }
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

#if UNITY_ANDROID

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                previosUnitPosition = new Vector2(touch.position.x, touch.position.y);

            }else if(touch.phase == TouchPhase.Moved)
            {
                Vector2 tochDeltaPosition = touch.deltaPosition;
                direction = tochDeltaPosition.normalized;

                if(Mathf.Abs(touch.position.x - previosUnitPosition.x) >= touchSensitivityHorizontal && direction.x < 0 && touch.deltaPosition.y > -10 && touch.deltaPosition.y <10)
                {
                    //left
                    MoveLeft();
                    previosUnitPosition = touch.position;
                    isMoved = true;

                } else if (Mathf.Abs(touch.position.x - previosUnitPosition.x) >= touchSensitivityHorizontal && direction.x > 0 && touch.deltaPosition.y > -10 && touch.deltaPosition.y < 10)
                {
                    //right
                    MoveRight();
                    previosUnitPosition = touch.position;
                    isMoved = true;
                } else if(Mathf.Abs(touch.position.y - previosUnitPosition.y) >= touchSensitivityVertical && direction.y < 0 && touch.deltaPosition.x > -10 && touch.deltaPosition.x < 10)
                {
                    //down
                    MoveDown();
                    previosUnitPosition = touch.position;
                    isMoved = true;
                }
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                if(!isMoved && touch.position.x > Screen.width / 4)
                {
                    Rotate();
                }
                isMoved = false;
            }
        }

        if(Time.time - fall >= fallSpeed){
            MoveDown();
        }

#else

        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            movedImmediateHorizontal = false;
            horizontalTimer = 0;
            buttonDownWaitTimerHorizontal = 0;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            movedImmediateVertical = false;
            verticalTimer = 0;
            buttonDownWaitTimerVertical = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
        {
            MoveDown();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rotate();
        }

        #endif
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

    void MoveLeft()
    {
        if (movedImmediateHorizontal)
        {
            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        if (!movedImmediateHorizontal)
            movedImmediateHorizontal = true;

        horizontalTimer = 0;

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

    void MoveRight()
    {
        if (movedImmediateHorizontal)
        {
            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }
            if (horizontalTimer < continuousHorizSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
            movedImmediateHorizontal = true;

        horizontalTimer = 0;

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

    void MoveDown()
    {
        if (movedImmediateVertical)
        {
            if (buttonDownWaitTimerVertical < buttonDownWaitMax)
            {
                buttonDownWaitTimerVertical += Time.deltaTime;
                return;
            }

            if (verticalTimer < continuousVertSpeed)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
        }
        if (!movedImmediateVertical)
            movedImmediateVertical = true;

        verticalTimer = 0;

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

    void Rotate()
    {
        if (isAllowRotation)
        {
            if (IsLimitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }
            }
            else
            {
                transform.Rotate(0, 0, 90);
            }
            if (CheckIsValidPosition())
            {

            }

            else
            {
                if (IsLimitRotation)
                {
                    if (transform.rotation.eulerAngles.z >= 90)
                    {
                        transform.Rotate(0, 0, -90);
                    }
                    else
                        transform.Rotate(0, 0, 90);
                }
                else
                {
                    transform.Rotate(0, 0, -90);
                }
            }
        }
    }
}

