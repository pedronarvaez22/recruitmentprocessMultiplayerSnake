using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public bool isReversing = false;
    public float recordTime = 5f;

    List<PointInTime> pointsInTime;

    private void Start()
    {
        pointsInTime = new List<PointInTime>();
    }

    private void FixedUpdate()
    {
        if (!isReversing)
        {
            Record();
        }
        else
        {
            Rewind();
        }
    }

    private void Record()
    {   
        if(pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }

        pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
    }

    private void Rewind()
    {
        if(pointsInTime.Count > 0)
        {
            PointInTime pointInTime = pointsInTime[0];
            transform.position = pointInTime.position;
            transform.rotation = pointInTime.rotation;
            pointsInTime.RemoveAt(0);
        }
        else
        {
            isReversing = false;
        }

    }

}
