using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAP2D;

public class CPU : MonoBehaviour
{
    [SerializeField]
    private SAP2DAgent _S2A;

    public Transform snakeHead;

    public List<Transform> snakeTail;
    [SerializeField]
    private GameObject _tailPrefab;

    public bool cpuDied;

    [SerializeField]
    private GameObject _tailContainer;

    private void Awake()
    {
        _S2A = GetComponent<SAP2DAgent>();
        cpuDied = false;
        _S2A.Target = GameObject.FindGameObjectWithTag("Food").GetComponent<Transform>();
    }
    private void OnEnable()
    {
        _S2A.Target = GameObject.FindGameObjectWithTag("Food").GetComponent<Transform>();
        _tailContainer = GameObject.Find("CPUTailContainer");
    }

    private void AdjustSpeed(bool isEnginePower)
    {
        if (!isEnginePower)
        {
            float newSpeedPenalty = 0.01f;
            _S2A.MovementSpeed -= newSpeedPenalty;
        }
    }

    private void Eat(int foodID)
    {
        Vector3 tailPosition = snakeHead.position + new Vector3(0,0,0);

        if (snakeTail.Count > 0)
        {
            tailPosition = snakeTail[snakeTail.Count - 1].position;
        }

        GameObject temp = Instantiate(_tailPrefab, tailPosition, transform.localRotation);
        int snakeTailIndex = snakeTail.Count;
        temp.transform.parent = _tailContainer.transform;

        snakeTail.Add(temp.transform);

        if (snakeTailIndex == 0)
        {
            snakeTail[0].GetComponent<CPUTail>().objRef = snakeHead;
        }
        else
        {
            snakeTail[snakeTailIndex].GetComponent<CPUTail>().objRef = snakeTail[snakeTailIndex - 1].transform;
        }

        if (foodID == 1)
            AdjustSpeed(true);
        else
            AdjustSpeed(false);
            

        GameManager.Instance.SetFood();
    }

    public void Die()
    {
        foreach (Transform t in snakeTail)
        {
            Destroy(t.gameObject);
        }
        GameManager.Instance.cpuDied = true;
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Food":
                Eat(GameManager.Instance.foodID);
                break;
            case "Tail":
                Die();
                break;
            case "Bounds":
                Die();
                break;
        }
    }

}
