using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAP2D;

public class CPU : MonoBehaviour
{
    [SerializeField]
    private Transform snakeHead;
    [SerializeField]
    private GameObject tailPrefab;

    [SerializeField]
    private SAP2DAgent s2A;

    public List<Transform> snakeTail;

    private void OnEnable()
    {
        s2A.Target = GameManager.Instance.food;
    }

    private void AdjustSpeed()
    {
        float newSpeedPenalty = 0.01f;
        s2A.MovementSpeed -= newSpeedPenalty;
    }

    private void Eat(int foodID)
    {
        Vector3 tailPosition = snakeHead.position + new Vector3(0,0,0);

        if (snakeTail.Count > 0)
        {
            tailPosition = snakeTail[snakeTail.Count - 1].position;
        }

        GameObject temp = Instantiate(tailPrefab, tailPosition, transform.localRotation);
        int snakeTailIndex = snakeTail.Count;
        temp.transform.parent = GameManager.Instance.CpuTailContainerTransform;

        snakeTail.Add(temp.transform);

        if (snakeTailIndex == 0)
        {
            snakeTail[0].GetComponent<CPUTail>().objRef = snakeHead;
        }
        else
        {
            snakeTail[snakeTailIndex].GetComponent<CPUTail>().objRef = snakeTail[snakeTailIndex - 1].transform;
        }

        if (foodID != 1)
        {
            AdjustSpeed();
        }

        GameManager.Instance.SetFood();
    }

    public void Die()
    {
        foreach (Transform t in snakeTail)
        {
            Destroy(t.gameObject);
        }
        GameManager.Instance.cpuDied = true;
        Destroy(gameObject);
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
