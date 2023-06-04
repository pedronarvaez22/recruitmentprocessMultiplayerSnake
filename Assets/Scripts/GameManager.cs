using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is null!");
            }
            return _instance;
        }
    }

    [SerializeField]
    private PlayerController playerToSpawn;

    [SerializeField]
    private GameObject CPUPrefab;

    [SerializeField]
    private Transform cpuTailContainerTransform;
    public Transform CpuTailContainerTransform => cpuTailContainerTransform;

    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    private SpriteRenderer foodSpriteRenderer;

    [SerializeField]
    private float holdTimer;
    [SerializeField]
    private float holdDuration = 2f;

    public Transform food;

    public int foodID;
    public int col = 29;
    public int rows = 15;

    public float delayStep;
    public float step;
    public float cpuDeathTimer = 5f;

    public bool cpuDied;
    public bool gameOver;

    public List<PlayerController> activePlayers;

    private bool _gameAboutToStart;

    private CPU _cpu;

    private KeyCode _keyUpTemp;
    private KeyCode _keyRightTemp;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        SetFood();
        holdDuration = 2f;
        Instantiate(CPUPrefab, new Vector3(2, 2, 1), Quaternion.identity);
        _gameAboutToStart = true;
        gameOver = true;
    }

    public void SetFood()
    {
        foodID = Random.Range(0, 4);

        switch(foodID)
        {
            case 0:
                foodSpriteRenderer.color = Color.green;
                break;
            case 1:
                foodSpriteRenderer.color = Color.yellow;
                break;
            case 2:
                foodSpriteRenderer.color = Color.blue;
                break;
            case 3:
                foodSpriteRenderer.color = Color.red;
                break;

        }

        var xMin = (col - 1) / 2 * -1;
        var xMax = (col - 1) / 2;
        var x = Random.Range( xMin, xMax);

        var yMin = (rows - 1) / 2 * -1;
        var yMax = (rows - 1) / 2;
        var y = Random.Range(yMin, yMax);

        food.position = new Vector2(x * step, y * step);
    }

    public void AddNewPlayer(PlayerController newPlayer, KeyCode keyUp, KeyCode keyRight)
    {
        var xMin = (col - 1) / 2 * -1;
        var xMax = (col - 1) / 2;
        var x = Random.Range(xMin, xMax);

        var yMin = (rows - 1) / 2 * -1;
        var yMax = (rows - 1) / 2;
        var y = Random.Range(yMin, yMax);

        newPlayer.keyUp = keyUp;
        newPlayer.keyRight = keyRight;
        PlayerController newPlayerInScene;
        newPlayerInScene = Instantiate(newPlayer, new Vector2(x * step, y * step), Quaternion.identity);
        activePlayers.Add(newPlayerInScene);
        _gameAboutToStart = false;
        uiManager.InstructionsText.gameObject.SetActive(false);
    }

    public void RefreshPlayers()
    {
        if (activePlayers.Count <= 0 && _gameAboutToStart == false)
        {
            GameOver();
        }   
    }

    public void GameOver()
    {
        gameOver = true;
        uiManager.gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        gameOver = false;
        uiManager.gameOverPanel.SetActive(false);
        _cpu = CPU.FindObjectOfType<CPU>();
        if(_cpu != null)
            _cpu.Die();
        uiManager.InstructionsText.gameObject.SetActive(true);
        Time.timeScale = 1;
        SetFood();
    }
    public void MainMenuButton()
    {
        Time.timeScale = 1;
        SetFood();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            RefreshPlayers();

        if (cpuDied)
        {
            cpuDeathTimer -= Time.deltaTime;

            if (cpuDeathTimer <= 0)
            {
                Instantiate(CPUPrefab, new Vector3(0, 0, 1), Quaternion.identity);
                cpuDied = false;
                cpuDeathTimer = 5f;
            }

        }

        foreach (KeyCode vkey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(vkey) && _keyUpTemp == KeyCode.None)
            {
                holdTimer += Time.deltaTime;

                if(holdTimer > holdDuration)
                    _keyUpTemp = vkey;
            }
            else if (Input.GetKey(vkey) && _keyUpTemp != KeyCode.None)
                {
                    if (holdTimer > holdDuration)
                        _keyRightTemp = vkey;
                }
                else if(Input.GetKeyUp(vkey))
                {
                    holdTimer = 0;
                }
            if(_keyUpTemp != KeyCode.None && _keyRightTemp != KeyCode.None && gameOver == false)
            {
                AddNewPlayer(playerToSpawn, _keyUpTemp, _keyRightTemp);
                _keyUpTemp = KeyCode.None;
                _keyRightTemp = KeyCode.None;
                holdTimer = 0;
            }

        }
    }
}
