using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Direction moveDirection;

    public Transform snakeHead;

    public List<Transform> snakeTail;
    [SerializeField]
    private GameObject _tailPrefab;
    private Vector3 _lastPos;

    public KeyCode keyUp,keyRight;

    public float playerStepRateDefault;
    public float speedMultiplier;

    public GameObject playerTailContainerPrefab;
    public GameObject playerTailContainer;

    [SerializeField]
    private int foodCount, enginePowerCount, timeTravelCount, batteringRamCount;
    [SerializeField]
    private bool hasTimeTravel, hasBatteringRam;

    //rewindTime
    private TimeController playerTime;

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    private void OnEnable()
    {
        playerTailContainer = Instantiate(playerTailContainerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        playerTime = GetComponent<TimeController>();
    }

    private void Start()
    {
        playerStepRateDefault = GameManager.Instance.delayStep;
        snakeHead = gameObject.transform;
        StartCoroutine(MoveSnake());
        foodCount = 0;
        enginePowerCount = 0;
        timeTravelCount = 0;
        batteringRamCount = 0;
    }

    private void Update()
    {
        //Inputs
        if (Input.GetKeyDown(keyUp) && playerTime.isReversing == false)
        {
            switch (moveDirection)
            {
                case Direction.Down:
                    moveDirection = Direction.Left;
                    break;
                case Direction.Up:
                    moveDirection = Direction.Right;
                    break;
                case Direction.Left:
                    moveDirection = Direction.Up;
                    break;
                case Direction.Right:
                    moveDirection = Direction.Down;
                    break;
            }
        }

        if (Input.GetKeyDown(keyRight) && playerTime.isReversing == false)
        {
            switch (moveDirection)
            {
                case Direction.Down:
                    moveDirection = Direction.Right;
                    break;
                case Direction.Up:
                    moveDirection = Direction.Left;
                    break;
                case Direction.Left:
                    moveDirection = Direction.Down;
                    break;
                case Direction.Right:
                    moveDirection = Direction.Up;
                    break;
            }
        }
    }

    private IEnumerator RewindTime()
    {
        foreach (Transform t in snakeTail)
        {
            t.GetComponent<TimeController>().isReversing = true;
        }

        playerTime.isReversing = true;
        
        yield return new WaitForSeconds(.5f);

        foreach (Transform t in snakeTail)
        {
            t.GetComponent<TimeController>().isReversing = false;
        }

        playerTime.isReversing = false;
    }

    public IEnumerator MoveSnake()
    {
        yield return new WaitForSeconds(playerStepRateDefault);
        var nextPos = Vector3.zero;

        switch (moveDirection)
        {
            case Direction.Down:
                nextPos = Vector3.down;
                snakeHead.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case Direction.Left:
                nextPos = Vector3.left;
                snakeHead.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Direction.Right:
                nextPos = Vector3.right;
                snakeHead.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case Direction.Up:
                nextPos = Vector3.up;
                snakeHead.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }

        nextPos *= GameManager.Instance.step;
        var position = snakeHead.position;
        _lastPos = position;
        position += nextPos;
        snakeHead.position = position;

        foreach (var t in snakeTail)
        {
            (t.position, _lastPos) = (_lastPos, t.position);
            t.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }

        StartCoroutine(MoveSnake());

    }

    private void AdjustSpeed(bool isEnginePower)
    {
        if(!isEnginePower)
        {
            float newSpeedPenalty = 0.01f;
            playerStepRateDefault += newSpeedPenalty;
        }
    }

    private void Eat(int foodID)
    {
        Vector3 tailPosition = snakeHead.position;

        if (snakeTail.Count > 0)
        {
            tailPosition = snakeTail[snakeTail.Count - 1].position;
        }

        GameObject temp = Instantiate(_tailPrefab, tailPosition, transform.localRotation);
        temp.transform.parent = playerTailContainer.transform;
        temp.GetComponent<PlayerTail>().TailID(gameObject);
        snakeTail.Add(temp.transform);

        switch(foodID)
        {
            case 0:
                AdjustSpeed(false);
                foodCount++;
                break;
            case 1:
                AdjustSpeed(true);
                enginePowerCount++;
                break;
            case 2:
                AdjustSpeed(false);
                timeTravelCount++;
                break;
            case 3:
                AdjustSpeed(false);
                batteringRamCount++;
                break;
        }

        RefreshPowerUps();

        GameManager.Instance.SetFood();
    }

    private void RefreshPowerUps()
    {
        if (batteringRamCount > 0)
            hasBatteringRam = true;
        else if (batteringRamCount <= 0)
        {
            hasBatteringRam = false;
            batteringRamCount = 0;
        }

        if (timeTravelCount > 0)
            hasTimeTravel = true;
        else if (timeTravelCount <= 0)
        {
            hasTimeTravel = false;
            timeTravelCount = 0;
        }
            
    }

    private void Die()
    {
        GameManager.Instance.activePlayers.Remove(this);
        GameManager.Instance.RefreshPlayers();
        Destroy(playerTailContainer);
        Destroy(gameObject);
        foreach (Transform t in snakeTail)
        {
            Destroy(t.gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Food":
                Eat(GameManager.Instance.foodID);
                break;

            case "Tail":
                PlayerTail pl = collision.gameObject.GetComponent<PlayerTail>();
                GameObject pc = pl.playerRef;

                if (hasBatteringRam && !hasTimeTravel)
                {
                    pc.GetComponent<PlayerController>().Die();
                    batteringRamCount--;
                    RefreshPowerUps();
                }
                else if(!hasBatteringRam && hasTimeTravel)
                    {
                        timeTravelCount--;
                        StartCoroutine(RewindTime());
                        RefreshPowerUps();
                    }
                    else if(hasBatteringRam && hasTimeTravel)
                        {
                            pc.GetComponent<PlayerController>().Die();
                            batteringRamCount--;
                            RefreshPowerUps();
                        }
                        else if(!hasBatteringRam && !hasTimeTravel)
                            Die();
                break;

            case "Bounds":
                if (hasTimeTravel)
                {
                    timeTravelCount--;
                    StartCoroutine(RewindTime());
                    RefreshPowerUps();
                }
                else
                    Die();
                break;

            case "TailCPU":
                if (hasBatteringRam && !hasTimeTravel)
                {
                    GameObject cpuref = GameObject.Find("CPU(Clone)");
                    cpuref.GetComponent<CPU>().Die();
                    batteringRamCount--;
                    RefreshPowerUps();
                }
                else if(!hasBatteringRam && hasTimeTravel)
                    {
                        timeTravelCount--;
                        StartCoroutine(RewindTime());
                        RefreshPowerUps();
                    }
                    else if(hasBatteringRam && hasTimeTravel)
                        {
                            GameObject cpuref = GameObject.Find("CPU(Clone)");
                            cpuref.GetComponent<CPU>().Die();
                            batteringRamCount--;
                            RefreshPowerUps();
                        }
                        else if(!hasBatteringRam && !hasTimeTravel)
                        {
                            Die();
                        }
                break;
        }
    }

}
