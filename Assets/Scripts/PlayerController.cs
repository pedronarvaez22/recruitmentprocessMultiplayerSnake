using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum Direction
    {
        LEFT, RIGHT, UP, DOWN
    }

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

    private void OnEnable()
    {
        playerTailContainer = Instantiate(playerTailContainerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        playerTime = this.GetComponent<TimeController>();
    }

    private void Start()
    {
        playerStepRateDefault = GameManager.Instance.delayStep;
        snakeHead = this.gameObject.transform;
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
                case Direction.DOWN:
                    moveDirection = Direction.LEFT;
                    break;
                case Direction.UP:
                    moveDirection = Direction.RIGHT;
                    break;
                case Direction.LEFT:
                    moveDirection = Direction.UP;
                    break;
                case Direction.RIGHT:
                    moveDirection = Direction.DOWN;
                    break;
            }
        }

        if (Input.GetKeyDown(keyRight) && playerTime.isReversing == false)
        {
            switch (moveDirection)
            {
                case Direction.DOWN:
                    moveDirection = Direction.RIGHT;
                    break;
                case Direction.UP:
                    moveDirection = Direction.LEFT;
                    break;
                case Direction.LEFT:
                    moveDirection = Direction.DOWN;
                    break;
                case Direction.RIGHT:
                    moveDirection = Direction.UP;
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

    private IEnumerator MoveSnake()
    {
        yield return new WaitForSeconds(playerStepRateDefault);
        Vector3 nextPos = Vector3.zero;

        switch (moveDirection)
        {
            case Direction.DOWN:
                nextPos = Vector3.down;
                snakeHead.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case Direction.LEFT:
                nextPos = Vector3.left;
                snakeHead.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Direction.RIGHT:
                nextPos = Vector3.right;
                snakeHead.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case Direction.UP:
                nextPos = Vector3.up;
                snakeHead.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }

        nextPos = nextPos * GameManager.Instance.step;
        _lastPos = snakeHead.position;
        snakeHead.position += nextPos;

        foreach (Transform t in snakeTail)
        {
            Vector3 temp = t.position;
            t.position = _lastPos;
            _lastPos = temp;
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
        temp.GetComponent<PlayerTail>().TailID(this.gameObject);
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
        Destroy(this.gameObject);
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
