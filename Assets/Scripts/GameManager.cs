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

    public enum Direction
    {
        LEFT, RIGHT, UP, DOWN
    }

    public float delayStep;
    public float step;

    public Transform Food;
    public int foodID;

    public int col = 29;
    public int rows = 15;

    private int _score;

    [SerializeField]
    private PlayerController _playerToSpawn;
    KeyCode keyUpTemp, keyRightTemp;
    [SerializeField]
    private float holdTimer;
    [SerializeField]
    private float holdDuration = 2f;

    private bool _gameAboutToStart;
    public bool _gameOver;

    [SerializeField]
    private UIManager _uiManager;

    public List<PlayerController> activePlayers;

    public float cpuDeathTimer = 5f;
    [SerializeField]
    private GameObject CPUPrefab;
    public bool cpuDied;
    private CPU _cpu;

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
        _gameOver = true;
        _uiManager = UIManager.FindObjectOfType<UIManager>();
    }

    public void SetFood()
    {
        foodID = Random.Range(0, 4);
        SpriteRenderer foodSpriteRenderer = Food.GetComponent<SpriteRenderer>();

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

        int xMin = (col - 1) / 2 * -1;
        int xMax = (col - 1) / 2;
        int x = Random.Range( xMin, xMax);

        int yMin = (rows - 1) / 2 * -1;
        int yMax = (rows - 1) / 2;
        int y = Random.Range(yMin, yMax);

        Food.position = new Vector2(x * step, y * step);
    }

    public void AddNewPlayer(PlayerController newPlayer, KeyCode keyUp, KeyCode keyRight)
    {
        int xMin = (col - 1) / 2 * -1;
        int xMax = (col - 1) / 2;
        int x = Random.Range(xMin, xMax);

        int yMin = (rows - 1) / 2 * -1;
        int yMax = (rows - 1) / 2;
        int y = Random.Range(yMin, yMax);

        newPlayer.keyUp = keyUp;
        newPlayer.keyRight = keyRight;
        PlayerController newPlayerInScene;
        newPlayerInScene = Instantiate(newPlayer, new Vector2(x * step, y * step), Quaternion.identity);
        activePlayers.Add(newPlayerInScene);
        _gameAboutToStart = false;
        _uiManager.InstructionsText.gameObject.SetActive(false);
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
        _gameOver = true;
        _uiManager.gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        _gameOver = false;
        _uiManager.gameOverPanel.SetActive(false);
        _cpu = CPU.FindObjectOfType<CPU>();
        if(_cpu != null)
            _cpu.Die();
        _uiManager.InstructionsText.gameObject.SetActive(true);
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
            if (Input.GetKey(vkey) && keyUpTemp == KeyCode.None)
            {
                holdTimer += Time.deltaTime;

                if(holdTimer > holdDuration)
                    keyUpTemp = vkey;
            }
            else if (Input.GetKey(vkey) && keyUpTemp != KeyCode.None)
            {
                if (holdTimer > holdDuration)
                    keyRightTemp = vkey;
            }
            else if(Input.GetKeyUp(vkey))
            {
                holdTimer = 0;
            }
            if(keyUpTemp != KeyCode.None && keyRightTemp != KeyCode.None && _gameOver == false)
            {
                AddNewPlayer(_playerToSpawn, keyUpTemp, keyRightTemp);
                keyUpTemp = KeyCode.None;
                keyRightTemp = KeyCode.None;
                holdTimer = 0;
            }

        }
    }
}
